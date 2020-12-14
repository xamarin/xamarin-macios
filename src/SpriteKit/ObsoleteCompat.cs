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

#nullable enable

namespace SpriteKit {

#if !XAMCORE_3_0 && !MONOMAC
	public partial class SKAction {

		[Obsolete ("Use the 'FalloffBy' method.")]
		public static SKAction Falloff (float /* float, not CGFloat */ to, double duration)
		{
			return FalloffBy (to, duration);
		}

		[Obsolete ("Use the 'TimingFunction2' property.")]
		public virtual void SetTimingFunction (SKActionTimingFunction? timingFunction)
		{
			TimingFunction = timingFunction;
		}
	}
#endif
}
