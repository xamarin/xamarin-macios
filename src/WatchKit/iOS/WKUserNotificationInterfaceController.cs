#nullable enable

#if __IOS__ && !NET
using System;
using System.ComponentModel;
using System.Runtime.Versioning;

using Foundation;
using ObjCRuntime;

namespace WatchKit {
	[Register ("WKUserNotificationInterfaceController", SkipRegistration = true)]
	[Unavailable (PlatformName.iOS, PlatformArchitecture.All)]
	[Obsolete (Constants.WatchKitRemoved)]
	[EditorBrowsable (EditorBrowsableState.Never)]
	public class WKUserNotificationInterfaceController : WKInterfaceController {
		public override IntPtr ClassHandle { get { throw new PlatformNotSupportedException (Constants.WatchKitRemoved); } }

		protected WKUserNotificationInterfaceController (NSObjectFlag t) : base (t)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		protected internal WKUserNotificationInterfaceController (IntPtr handle) : base (handle)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		public WKUserNotificationInterfaceController ()
			: base (NSObjectFlag.Empty)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		public virtual void DidReceiveLocalNotification (global::UIKit.UILocalNotification localNotification, [BlockProxy (typeof (ObjCRuntime.Trampolines.NIDActionArity1V209))]global::System.Action<WKUserNotificationInterfaceType> completionHandler)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		public virtual void DidReceiveNotification (global::UserNotifications.UNNotification notification, [BlockProxy (typeof (ObjCRuntime.Trampolines.NIDActionArity1V209))]global::System.Action<WKUserNotificationInterfaceType> completionHandler)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		public virtual void DidReceiveRemoteNotification (NSDictionary remoteNotification, [BlockProxy (typeof (ObjCRuntime.Trampolines.NIDActionArity1V209))]global::System.Action<WKUserNotificationInterfaceType> completionHandler)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		public new virtual void DismissController ()
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}
	} /* class WKUserNotificationInterfaceController */
}
#endif // __IOS__ && !NET
