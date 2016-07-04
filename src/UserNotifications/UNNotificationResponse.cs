//
// UNNotificationResponse extensions & syntax sugar
//
// Authors:
//	Alex Soto  <alex.soto@xamarin.com>
//
// Copyright 2016 Xamarin Inc. All rights reserved.
//

#if !TVOS
using System;
namespace XamCore.UserNotifications {
	public partial class UNNotificationResponse {
		public UNActionIdentifier ActionIdentifier => _ActionIdentifier == UNNotificationResponse.DefaultActionIdentifier ? UNActionIdentifier.Default : UNActionIdentifier.Dismiss;
	}
}
#endif // !TVOS

