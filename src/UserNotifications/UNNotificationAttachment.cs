//
// UNNotificationAttachment extensions & syntax sugar
//
// Authors:
//	Alex Soto  <alex.soto@xamarin.com>
//
// Copyright 2016 Xamarin Inc. All rights reserved.
//

#if XAMCORE_2_0 && !TVOS
using System;
using Foundation;

namespace UserNotifications {
	public partial class UNNotificationAttachment {

		public static UNNotificationAttachment FromIdentifier (string identifier, NSUrl url, UNNotificationAttachmentOptions attachmentOptions, out NSError error)
		{
			return FromIdentifier (identifier, url, attachmentOptions?.Dictionary, out error);
		}
	}
}
#endif // XAMCORE_2_0 && !TVOS

