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

namespace SpriteKit {

#if !XAMCORE_4_0
	[Obsolete ("Use 'SKActionTimingFunction2' instead.")]
	public delegate void SKActionTimingFunction (float /* float, not CGFloat */ time);
#endif

#if XAMCORE_2_0 || !MONOMAC
	public partial class SKAction {

		public static SKAction ResizeTo (CGSize size, double duration)
		{
			return SKAction.ResizeTo (size.Width, size.Height, duration);
		}

#if !XAMCORE_2_0
		[Obsolete ("Use 'Run(Action)' instead.")]
		public static SKAction RunBlock (Action block)
		{
			return Run (block);
		}

		[Obsolete ("Use 'Run(Action,DispatchQueue)' instead.")]
		public static SKAction RunBlock (Action block, DispatchQueue queue)
		{
			return Run (block, queue);
		}
#endif
#if !XAMCORE_4_0
		[Obsolete ("Use 'TimingFunction2' instead.")]
		public virtual SKActionTimingFunction TimingFunction { get; set; }
#endif
	}
#endif
}
