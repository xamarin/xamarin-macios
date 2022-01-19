// 
// UIScreen.cs: Helper methods for UIScreen.
//
// Authors:
//   Miguel de Icaza
//     
// Copyright 2010 Novell, Inc
// Copyright 2014 Xamarin Inc.
//

#if !WATCH

using System;
using System.Collections;
using System.Runtime.InteropServices;

using Foundation; 
using ObjCRuntime;
using CoreGraphics;

namespace UIKit {
	public partial class UIScreen {

		public CoreAnimation.CADisplayLink CreateDisplayLink (Action action)
		{
			if (action == null)
				throw new ArgumentNullException ("action");
			var d = new NSActionDispatcher (action);
			return CreateDisplayLink (d, NSActionDispatcher.Selector);
		}

		public UIImage Capture ()
		{
			if (SystemVersion.CheckiOS (7, 0)) {
				// This is from https://developer.apple.com/library/content/qa/qa1817/_index.html
				try {
					var view = UIApplication.SharedApplication.KeyWindow;
#if NO_NFLOAT_OPERATORS
					UIGraphics.BeginImageContextWithOptions (view.Bounds.Size, view.Opaque, new NFloat (0));
#else
					UIGraphics.BeginImageContextWithOptions (view.Bounds.Size, view.Opaque, 0);
#endif
					view.DrawViewHierarchy (view.Bounds, true);
					return UIGraphics.GetImageFromCurrentImageContext ();
				} finally {
					UIGraphics.EndImageContext ();
				}
			} 

			// This is from: https://developer.apple.com/library/ios/#qa/qa2010/qa1703.html
			var selScreen = new Selector ("screen");
			var size = Bounds.Size;

#if NO_NFLOAT_OPERATORS
			UIGraphics.BeginImageContextWithOptions (size, false, new NFloat (0));
#else
			UIGraphics.BeginImageContextWithOptions (size, false, 0);
#endif

			try {
				var context = UIGraphics.GetCurrentContext ();

				foreach (var window in UIApplication.SharedApplication.Windows) {
					if (window.RespondsToSelector (selScreen) && window.Screen != this)
						continue;

					context.SaveState ();
					context.TranslateCTM (window.Center.X, window.Center.Y);
					context.ConcatCTM (window.Transform);
#if NO_NFLOAT_OPERATORS
					context.TranslateCTM (new NFloat (-window.Bounds.Size.Width.Value * window.Layer.AnchorPoint.X.Value), new NFloat (-window.Bounds.Size.Height.Value * window.Layer.AnchorPoint.Y.Value));
#else
					context.TranslateCTM (-window.Bounds.Size.Width * window.Layer.AnchorPoint.X, -window.Bounds.Size.Height * window.Layer.AnchorPoint.Y);
#endif

					window.Layer.RenderInContext (context);
					context.RestoreState ();
				}

				return UIGraphics.GetImageFromCurrentImageContext ();
			} finally {
				UIGraphics.EndImageContext ();
			}
		}
	}
}

#endif // !WATCH
