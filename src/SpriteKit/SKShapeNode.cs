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
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		public static SKShapeNode FromPoints (CGPoint [] points)
		{
			if (points is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (points));

			return FromPoints (ref points [0], (nuint) points.Length);
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		public static SKShapeNode FromPoints (CGPoint [] points, int offset, int length)
		{
			if (points is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (points));
			if (offset > points.Length - length)
				throw new InvalidOperationException ("offset + length must not be greater than the length of the array");

			return FromPoints (ref points [offset], (nuint) length);
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		public static SKShapeNode FromSplinePoints (CGPoint [] points)
		{
			if (points is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (points));

			return FromSplinePoints (ref points [0], (nuint) points.Length);
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		public static SKShapeNode FromSplinePoints (CGPoint [] points, int offset, int length)
		{
			if (points is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (points));
			if (offset > points.Length - length)
				throw new InvalidOperationException ("offset + length must not be greater than the length of the array");

			return FromSplinePoints (ref points [offset], (nuint) length);
		}
	}
}
