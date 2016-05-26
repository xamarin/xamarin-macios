using System.Drawing;
using System;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.ObjCRuntime;

namespace Desk79124 {
	[BaseType (typeof (UIView))]
	public interface WYPopoverBackgroundView : IUIAppearance {
		[Export ("glossShadowOffset", ArgumentSemantic.Assign)]
		[Appearance]
		SizeF GlossShadowOffset { get; set; }
	}
}