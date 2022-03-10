//
// SKShapeNode.cs: extensions to SKShapeNode
//
// Authors:
//   Alex Soto (alex.soto@xamarin.com)
//
// Copyright 2016 Xamarin Inc.
//

using System;
using CoreGraphics;
using ObjCRuntime;

#nullable enable

namespace SpriteKit {
	public partial class SKShapeNode : SKNode {

#if NET
		[SupportedOSPlatform ("ios8.0")]
		[SupportedOSPlatform ("macos10.10")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#else
		[iOS (8, 0)]
		[Mac (10, 10)]
#endif
		public static SKShapeNode FromPoints (CGPoint [] points)
		{
			if (points == null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (points));

			return FromPoints (ref points[0], (nuint) points.Length);
		}

#if NET
		[SupportedOSPlatform ("ios8.0")]
		[SupportedOSPlatform ("macos10.10")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#else
		[iOS (8, 0)]
		[Mac (10, 10)]
#endif
		public static SKShapeNode FromPoints (CGPoint [] points, int offset, int length)
		{
			if (points == null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (points));
			if (offset > points.Length - length)
				throw new InvalidOperationException ("offset + length must not be greater than the length of the array");

			return FromPoints (ref points [offset], (nuint) length);
		}

#if NET
		[SupportedOSPlatform ("ios8.0")]
		[SupportedOSPlatform ("macos10.10")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#else
		[iOS (8, 0)]
		[Mac (10, 10)]
#endif
		public static SKShapeNode FromSplinePoints (CGPoint [] points)
		{
			if (points == null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (points));

			return FromSplinePoints (ref points[0], (nuint) points.Length);
		}

#if NET
		[SupportedOSPlatform ("ios8.0")]
		[SupportedOSPlatform ("macos10.10")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#else
		[iOS (8, 0)]
		[Mac (10, 10)]
#endif
		public static SKShapeNode FromSplinePoints (CGPoint [] points, int offset, int length)
		{
			if (points == null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (points));
			if (offset > points.Length - length)
				throw new InvalidOperationException ("offset + length must not be greater than the length of the array");

			return FromSplinePoints (ref points [offset], (nuint) length);
		}
	}
}
