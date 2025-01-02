using System;

using Foundation;
using ObjCRuntime;
using UIKit;
using UserNotifications;

namespace NotificationServiceExtension {
	[Register ("NotificationServiceClass")]
	public class NotificationServiceClass : UNNotificationServiceExtension {
		protected NotificationServiceClass (NativeHandle handle) : base (handle)
		{
			// Note: this .ctor should not contain any initialization logic,
			// it only exists so that the OS can instantiate an instance of this class.
		}

		public override void DidReceiveNotificationRequest (UNNotificationRequest request, Action<UNNotificationContent> contentHandler)
		{
			// Called when the OS receives a notification that can be muteated

			// Create a mutable copy of the notification
			var mutableRequest = (UNMutableNotificationContent) request.Content.MutableCopy ();

			// Modify the notification content here...
			mutableRequest.Title = $"[modified] {mutableRequest.Title}";

			// Call the contentHandler callback to let the OS know about the modified notification.
			contentHandler (mutableRequest);
		}

		public override void TimeWillExpire ()
		{
			// Called just before the extension will be terminated by the system.
			// Use this as an opportunity to deliver your "best attempt" at modified content, otherwise the original push payload will be used.
		}
	}
}
