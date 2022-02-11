//
// UNNotificationAttachment extensions & syntax sugar
//
// Authors:
//	Alex Soto  <alex.soto@xamarin.com>
//
// Copyright 2016 Xamarin Inc. All rights reserved.
//

#if !TVOS
using System;
using Foundation;
using System.Runtime.Versioning;

namespace UserNotifications {
#if NET
	[SupportedOSPlatform ("ios10.0")]
	[SupportedOSPlatform ("macos10.14")]
	[SupportedOSPlatform ("maccatalyst")]
	[UnsupportedOSPlatform ("tvos")]
#endif
	public partial class UNNotificationAttachment {

		public static UNNotificationAttachment FromIdentifier (string identifier, NSUrl url, UNNotificationAttachmentOptions attachmentOptions, out NSError error)
		{
			return FromIdentifier (identifier, url, attachmentOptions?.Dictionary, out error);
		}
	}
}
#endif // !TVOS
