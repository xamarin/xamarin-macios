//
// Compat.cs: Compatibility functions
//
// Authors:
//   Miguel de Icaza (miguel@xamarin.com)
//
// Copyright 2013-2014, 2016 Xamarin Inc

using System;
using ObjCRuntime;
using CoreGraphics;

namespace SpriteKit {

#if !XAMCORE_3_0 && !MONOMAC
	public partial class SKAction {

		[Obsolete ("Use the 'FalloffBy' method.")]
		public static SKAction Falloff (float /* float, not CGFloat */ to, double duration)
		{
			return FalloffBy (to, duration);
		}

		[Obsolete ("Use the 'TimingFunction2' property.")]
		public virtual void SetTimingFunction (SKActionTimingFunction timingFunction)
		{
			TimingFunction = timingFunction;
		}
	}
#endif

#if !XAMCORE_2_0 && !MONOMAC
	public partial class SKPhysicsBody {

		[Obsolete ("Use the 'FromBodies' method instead.")]
		public static SKPhysicsBody BodyWithBodies (SKPhysicsBody [] bodies)
		{
			return FromBodies (bodies);
		}

		[Obsolete ("Use the 'CreateCircularBody' method instead.")]
		public static SKPhysicsBody BodyWithCircleOfRadius (nfloat radius)
		{
			return CreateCircularBody (radius);
		}

		[Obsolete ("Use the 'CreateCircularBody' method instead.")]
		public static SKPhysicsBody BodyWithCircleOfRadius (nfloat radius, CGPoint center)
		{
			return CreateCircularBody (radius, center);
		}

		[Obsolete ("Use the 'CreateRectangularBody' method instead.")]
		public static SKPhysicsBody BodyWithRectangleOfSize (CGSize size)
		{
			return CreateRectangularBody (size);
		}

		[Obsolete ("Use the 'CreateRectangularBody' method instead.")]
		public static SKPhysicsBody BodyWithRectangleOfSize (CGSize size, CGPoint center)
		{
			return CreateRectangularBody (size, center);
		}

		[Obsolete ("Use the 'CreateBodyFromPath' method instead.")]
		public static SKPhysicsBody BodyWithPolygonFromPath (CGPath path)
		{
			return CreateBodyFromPath (path);
		}

		[Obsolete ("Use the 'CreateEdge' method instead.")]
		public static SKPhysicsBody BodyWithEdgeFromPoint (CGPoint fromPoint, CGPoint toPoint)
		{
			return CreateEdge (fromPoint, toPoint);
		}

		[Obsolete ("Use the 'CreateEdgeChain' method instead.")]
		public static SKPhysicsBody BodyWithEdgeChainFromPath (CGPath path)
		{
			return CreateEdgeChain (path);
		}

		[Obsolete ("Use the 'CreateEdgeLoop' method instead.")]
		public static SKPhysicsBody BodyWithEdgeLoopFromPath (CGPath path)
		{
			return CreateEdgeLoop (path);
		}

		[Obsolete ("Use the 'CreateEdgeLoop' method instead.")]
		public static SKPhysicsBody BodyWithEdgeLoopFromRect (CGRect rect)
		{
			return CreateEdgeLoop (rect);
		}
	}
#endif
}
