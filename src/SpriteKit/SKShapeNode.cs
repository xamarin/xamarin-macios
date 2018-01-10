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

#if XAMCORE_2_0 || !MONOMAC
namespace SpriteKit {
	public partial class SKShapeNode : SKNode {

		[iOS (8, 0)]
		[Mac (10, 10)]
		public static SKShapeNode FromPoints (CGPoint [] points)
		{
			if (points == null)
				throw new ArgumentNullException (nameof (points));

			return FromPoints (ref points[0], (nuint) points.Length);
		}

		[iOS (8, 0)]
		[Mac (10, 10)]
		public static SKShapeNode FromPoints (CGPoint [] points, int offset, int length)
		{
			if (points == null)
				throw new ArgumentNullException (nameof (points));
			if (offset > points.Length - length)
				throw new InvalidOperationException ("offset + length must not be greater than the length of the array");

			return FromPoints (ref points [offset], (nuint) length);
		}

		[iOS (8, 0)]
		[Mac (10, 10)]
		public static SKShapeNode FromSplinePoints (CGPoint [] points)
		{
			if (points == null)
				throw new ArgumentNullException (nameof (points));

			return FromSplinePoints (ref points[0], (nuint) points.Length);
		}

		[iOS (8, 0)]
		[Mac (10, 10)]
		public static SKShapeNode FromSplinePoints (CGPoint [] points, int offset, int length)
		{
			if (points == null)
				throw new ArgumentNullException (nameof (points));
			if (offset > points.Length - length)
				throw new InvalidOperationException ("offset + length must not be greater than the length of the array");

			return FromSplinePoints (ref points [offset], (nuint) length);
		}
	}
}

#endif // XAMCORE_2_0 || !MONOMAC
