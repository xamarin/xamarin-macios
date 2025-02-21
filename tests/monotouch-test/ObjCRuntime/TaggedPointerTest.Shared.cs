using System;

using Foundation;

namespace MonoTouchFixtures.ObjCRuntime {
	public partial class TaggedPointerTest {
		public static bool ThrowsObjectDisposedExceptions ()
		{
			try {
				var notificationData =
					"""
					{
						action = "action";
						aps =
						{
							category = "ee";
							"content-available" = dd;
							"mutable-content" = cc;
							sound = bb;
							"thread-id" = "aa";
						};
						"em-account" = "a";
						"em-account-id" = "b";
						"em-body" = "c";
						"em-date" = "d";
						"em-from" = "e";
						"em-from-address" = "f";
						"em-notification" = g;
						"em-notification-id" = h;
						"em-subject" = "i";
						"gcm.message_id" = j;
						"google.c.a.e" = k;
						"google.c.fid" = l;
						"google.c.sender.id" = m;
					}
					""";

				var data = NSData.FromString (notificationData);
				var fmt = NSPropertyListFormat.OpenStep;
				var userInfo = (NSMutableDictionary) NSPropertyListSerialization.PropertyListWithData (data, NSPropertyListReadOptions.Immutable, ref fmt, out var error);
				var apsKey = new NSString ("aps");
				// Iteration here should not throw ObjectDisposedExceptions
				foreach (var kv in userInfo) {
					apsKey.Dispose ();
				}

				return false;
			} catch (ObjectDisposedException) {
				return true;
			}
		}
	}
}
