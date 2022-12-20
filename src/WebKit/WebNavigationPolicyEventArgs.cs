//
// WebNavigationPolicyEventArgs.cs
//
// Authors:
//   Aaron Bockover (abock@xamarin.com)
//
// Copyright 2013 Xamarin Inc

#if __MACOS__

#nullable enable

using System;

using Foundation;

namespace WebKit {

	// Convenience enum.
	public enum WebActionMouseButton {
		None = -1,
		Left = 0,
		Middle = 1,
		Right = 2
	}

	partial class WebNavigationPolicyEventArgs {

		public WebNavigationType NavigationType {
			get { return (WebNavigationType) ((NSNumber) ActionInformation [WebPolicyDelegate.WebActionNavigationTypeKey]).Int32Value; }
		}

		public NSDictionary? ElementInfo {
			get { return ActionInformation [WebPolicyDelegate.WebActionElementKey] as NSDictionary; }
		}

		public WebActionMouseButton MouseButton {
			get {
				var number = ActionInformation [WebPolicyDelegate.WebActionButtonKey] as NSNumber;
				if (number is null) {
					return WebActionMouseButton.None;
				}

				return (WebActionMouseButton) number.Int32Value;
			}
		}

		public uint Flags {
			get { return ((NSNumber) ActionInformation [WebPolicyDelegate.WebActionModifierFlagsKey]).UInt32Value; }
		}

		public NSUrl? OriginalUrl {
			get { return ActionInformation [WebPolicyDelegate.WebActionOriginalUrlKey] as NSUrl; }
		}
	}
}

#endif // __MACOS__
