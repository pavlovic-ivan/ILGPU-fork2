﻿// ---------------------------------------------------------------------------------------
//                                        ILGPU
//                        Copyright (c) 2020-2021 ILGPU Project
//                                    www.ilgpu.net
//
// File: CompareIntrinsics.cs
//
// This file is part of ILGPU and is distributed under the University of Illinois Open
// Source License. See LICENSE.txt for details.
// ---------------------------------------------------------------------------------------

using ILGPU.IR.Values;
using System;

namespace ILGPU.Frontend.Intrinsic
{
    /// <summary>
    /// Marks compare intrinsics that are built in.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    sealed class CompareIntriniscAttribute : IntrinsicAttribute
    {
        public CompareIntriniscAttribute(CompareKind intrinsicKind)
            : this(intrinsicKind, CompareFlags.None)
        { }

        public CompareIntriniscAttribute(
            CompareKind intrinsicKind,
            CompareFlags intrinsicFlags)
        {
            IntrinsicKind = intrinsicKind;
            IntrinsicFlags = intrinsicFlags;
        }

        public override IntrinsicType Type => IntrinsicType.Compare;

        /// <summary>
        /// Returns the associated intrinsic kind.
        /// </summary>
        public CompareKind IntrinsicKind { get; }

        /// <summary>
        /// Returns the associated intrinsic flags.
        /// </summary>
        public CompareFlags IntrinsicFlags { get; }
    }

    partial class Intrinsics
    {
        /// <summary>
        /// Handles compare operations.
        /// </summary>
        /// <param name="context">The current invocation context.</param>
        /// <param name="attribute">The intrinsic attribute.</param>
        /// <returns>The resulting value.</returns>
        private static ValueReference HandleCompareOperation(
            ref InvocationContext context,
            CompareIntriniscAttribute attribute) =>
            context.Builder.CreateCompare(
                context.Location,
                context[0],
                context[1],
                attribute.IntrinsicKind,
                attribute.IntrinsicFlags);
    }
}
