using System.Drawing;
using System;

using Foundation;
using UIKit;
using ObjCRuntime;

namespace Desk79124 {
	[BaseType (typeof (UIView))]
	public interface WYPopoverBackgroundView : IUIAppearance {
		[Export ("glossShadowOffset", ArgumentSemantic.Assign)]
		[Appearance]
		SizeF GlossShadowOffset { get; set; }
	}
}
