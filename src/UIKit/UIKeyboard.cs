//
// Mostly notifications for keyboard events
//

#if !WATCH

using Foundation;
using ObjCRuntime;
using CoreGraphics;
using System.Runtime.Versioning;

using System;

namespace UIKit {
	public partial class UIKeyboard {
		static CGRect RectangleFFrom (NSObject key, NSNotification n)
		{
			if (n == null || n.UserInfo == null)
				throw new ArgumentNullException ("n");
			var val = n.UserInfo [key] as NSValue;
			if (val != null)
				return val.CGRectValue;
			return CGRect.Empty;
		}

#if !TVOS && !__MACCATALYST__
#if !NET
#if NET
		[UnsupportedOSPlatform ("ios3.2")]
#if IOS
		[Obsolete ("Starting with ios3.2.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		[UnsupportedOSPlatform ("tvos")]
#else
		[Deprecated (PlatformName.iOS, 3, 2)]
#endif
		public static CGRect BoundsFromNotification (NSNotification n)
		{
			return RectangleFFrom (BoundsUserInfoKey, n);
		}
#endif

		public static double AnimationDurationFromNotification (NSNotification n)
		{
			if (n == null || n.UserInfo == null)
				throw new ArgumentNullException ("n");
			var val = n.UserInfo [AnimationDurationUserInfoKey] as NSNumber;
			if (val == null)
				return 0;
			return val.DoubleValue;
		}

		public static uint AnimationCurveFromNotification (NSNotification n)
		{
			if (n == null || n.UserInfo == null)
				throw new ArgumentNullException ("n");
			var val = n.UserInfo [AnimationCurveUserInfoKey] as NSNumber;
			if (val == null)
				return 0;
			return val.UInt32Value;
		}
#endif

		static CGPoint PointFFrom (NSObject key, NSNotification n)
		{
			if (n == null || n.UserInfo == null)
				throw new ArgumentNullException ("n");
			var val = n.UserInfo [key] as NSValue;
			if (val == null)
				return CGPoint.Empty;
			return val.CGPointValue;
		}

#if !TVOS && !__MACCATALYST__
#if !NET
#if NET
		[UnsupportedOSPlatform ("ios3.2")]
#if IOS
		[Obsolete ("Starting with ios3.2.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		[UnsupportedOSPlatform ("tvos")]
#else
		[Deprecated (PlatformName.iOS, 3, 2)]
#endif
		static public CGPoint CenterBeginFromNotification (NSNotification n)
		{
			return PointFFrom (CenterBeginUserInfoKey, n);
		}
#endif

#if !NET
#if NET
		[UnsupportedOSPlatform ("ios3.2")]
#if IOS
		[Obsolete ("Starting with ios3.2.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		[UnsupportedOSPlatform ("tvos")]
#else
		[Deprecated (PlatformName.iOS, 3, 2)]
#endif
		static public CGPoint CenterEndFromNotification (NSNotification n)
		{
			return PointFFrom (CenterEndUserInfoKey, n);
		}
#endif

		static public CGRect FrameBeginFromNotification (NSNotification n)
		{
			return RectangleFFrom (FrameBeginUserInfoKey, n);
		}

		static public CGRect FrameEndFromNotification (NSNotification n)
		{
			return RectangleFFrom (FrameEndUserInfoKey, n);
		}
#endif
	}
}

#endif // !WATCH
