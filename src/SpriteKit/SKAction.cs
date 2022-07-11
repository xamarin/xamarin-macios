//
// SKAction helpers
//
// Authors:
//   Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2013 Xamarin Inc.
//

using System;
using CoreGraphics;
using CoreFoundation;

#nullable enable

namespace SpriteKit {

#if !NET
	[Obsolete ("Use 'SKActionTimingFunction2' instead.")]
	public delegate void SKActionTimingFunction (float /* float, not CGFloat */ time);
#endif

	public partial class SKAction {

		public static SKAction ResizeTo (CGSize size, double duration)
		{
			return SKAction.ResizeTo (size.Width, size.Height, duration);
		}

#if !NET
		[Obsolete ("Use 'TimingFunction2' instead.")]
		public virtual SKActionTimingFunction? TimingFunction { get; set; }
#endif
	}
}
