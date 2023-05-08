//
// Mostly notifications for keyboard events
//

#if !WATCH

using Foundation;
using ObjCRuntime;
using CoreGraphics;

using System;

namespace UIKit {
	public partial class UIKeyboard {
		static CGRect RectangleFFrom (NSObject key, NSNotification n)
		{
			if (n is null || n.UserInfo is null)
				throw new ArgumentNullException ("n");
			var val = n.UserInfo [key] as NSValue;
			if (val is not null)
				return val.CGRectValue;
			return CGRect.Empty;
		}

#if !TVOS && !__MACCATALYST__
#if !NET
		[Deprecated (PlatformName.iOS, 3, 2)]
		public static CGRect BoundsFromNotification (NSNotification n)
		{
			return RectangleFFrom (BoundsUserInfoKey, n);
		}
#endif

		public static double AnimationDurationFromNotification (NSNotification n)
		{
			if (n is null || n.UserInfo is null)
				throw new ArgumentNullException ("n");
			var val = n.UserInfo [AnimationDurationUserInfoKey] as NSNumber;
			if (val is null)
				return 0;
			return val.DoubleValue;
		}

		public static uint AnimationCurveFromNotification (NSNotification n)
		{
			if (n is null || n.UserInfo is null)
				throw new ArgumentNullException ("n");
			var val = n.UserInfo [AnimationCurveUserInfoKey] as NSNumber;
			if (val is null)
				return 0;
			return val.UInt32Value;
		}
#endif

		static CGPoint PointFFrom (NSObject key, NSNotification n)
		{
			if (n is null || n.UserInfo is null)
				throw new ArgumentNullException ("n");
			var val = n.UserInfo [key] as NSValue;
			if (val is null)
				return CGPoint.Empty;
			return val.CGPointValue;
		}

#if !TVOS && !__MACCATALYST__
#if !NET
		[Deprecated (PlatformName.iOS, 3, 2)]
		static public CGPoint CenterBeginFromNotification (NSNotification n)
		{
			return PointFFrom (CenterBeginUserInfoKey, n);
		}
#endif

#if !NET
		[Deprecated (PlatformName.iOS, 3, 2)]
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
