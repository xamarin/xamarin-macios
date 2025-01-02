using System;

using Foundation;
using ObjCRuntime;
using UIKit;
using UserNotifications;
using UserNotificationsUI;

namespace NotificationContentExtension {
	[Register ("NotificationViewController")]
	public class NotificationViewController : UIViewController, IUNNotificationContentExtension {
		const int LabelHeight = 88;
		UILabel? notificationLabel;

		protected NotificationViewController (NativeHandle handle) : base (handle)
		{
			// Note: this .ctor should not contain any initialization logic,
			// it only exists so that the OS can instantiate an instance of this class.
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Do any required initialization here

			View.BackgroundColor = UIColor.Clear;

			notificationLabel = new UILabel () {
				TranslatesAutoresizingMaskIntoConstraints = false,
				Lines = 0,
				TextAlignment = UITextAlignment.Center,
			};
			View.AddSubview (notificationLabel);

			notificationLabel.TopAnchor.ConstraintEqualTo (View.TopAnchor).Active = true;
			notificationLabel.LeadingAnchor.ConstraintEqualTo (View.LeadingAnchor).Active = true;
			notificationLabel.TrailingAnchor.ConstraintEqualTo (View.TrailingAnchor).Active = true;
			notificationLabel.HeightAnchor.ConstraintEqualTo (LabelHeight).Active = true;
		}

		public void DidReceiveNotification (UNNotification notification)
		{
			if (notificationLabel is not null) {
				notificationLabel.Text =
					$"➡️ {notification.Request.Content.Title}\n" +
					$"↪️ {notification.Request.Content.Subtitle}\n" +
					$"⏩ {notification.Request.Content.Body}";
			}
		}
	}
}
