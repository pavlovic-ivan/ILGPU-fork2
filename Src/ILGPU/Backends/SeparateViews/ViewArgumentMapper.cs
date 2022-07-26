﻿// ---------------------------------------------------------------------------------------
//                                        ILGPU
//                        Copyright (c) 2019-2021 ILGPU Project
//                                    www.ilgpu.net
//
// File: ViewArgumentMapper.cs
//
// This file is part of ILGPU and is distributed under the University of Illinois Open
// Source License. See LICENSE.txt for details.
// ---------------------------------------------------------------------------------------

using ILGPU.Backends.EntryPoints;
using ILGPU.Backends.IL;
using System;

namespace ILGPU.Backends.SeparateViews
{
    /// <summary>
    /// Maps array views to separate view implementations.
    /// </summary>
    /// <remarks>Members of this class are not thread safe.</remarks>
    public abstract class ViewArgumentMapper : ArgumentMapper
    {
        #region Nested Types

        /// <summary>
        /// Wraps a value source and created a new view instance from value references.
        /// </summary>
        /// <typeparam name="TSource">The source type.</typeparam>
        private readonly struct ViewImplementationSource<TSource> : IRawValueSource
            where TSource : struct, ISource
        {
            public ViewImplementationSource(TSource source)
            {
                Source = source;
            }

            /// <summary>
            /// Returns the parent source.
            /// </summary>
            public TSource Source { get; }

            /// <summary>
            /// Emits a new view-value construction.
            /// </summary>
            public readonly void EmitLoadSource<TILEmitter>(
                in TILEmitter emitter)
                where TILEmitter : struct, IILEmitter
            {
                // Load source and create custom view type
                Source.EmitLoadSource(emitter);
                emitter.EmitCall(
                    ViewImplementation.GetCreateMethod(Source.SourceType));
            }
        }

        #endregion

        #region Instance

        /// <summary>
        /// Constructs a new view argument mapper.
        /// </summary>
        /// <param name="context">The current context.</param>
        protected ViewArgumentMapper(Context context)
            : base(context)
        { }

        #endregion

        #region Methods

        /// <summary>
        /// Maps an internal view type to a pointer implementation type.
        /// </summary>
        protected sealed override Type MapViewType(Type viewType, Type elementType) =>
            typeof(ViewImplementation);

        /// <summary>
        /// Maps an internal view instance to a pointer instance.
        /// </summary>
        protected sealed override void MapViewInstance<TILEmitter, TSource, TTarget>(
            in TILEmitter emitter,
            Type elementType,
            in TSource source,
            in TTarget target)
        {
            var viewSource = new ViewImplementationSource<TSource>(source);
            target.EmitStoreTarget(emitter, viewSource);
        }

        #endregion
    }
}
