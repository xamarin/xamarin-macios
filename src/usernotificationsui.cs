//
// UserNotificationsUI bindings
//
// Authors:
//	Alex Soto  <alex.soto@xamarin.com>
//
// Copyright 2016 Xamarin Inc. All rights reserved.
//

using System;
using XamCore.CoreGraphics;
using XamCore.Foundation;
using XamCore.ObjCRuntime;
using XamCore.UIKit;
using XamCore.UserNotifications;

namespace XamCore.UserNotificationsUI {

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[Unavailable (PlatformName.TvOS)]
	[Native]
	public enum UNNotificationContentExtensionMediaPlayPauseButtonType : nuint {
		None,
		Default,
		Overlay
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[Unavailable (PlatformName.TvOS)]
	[Native]
	public enum UNNotificationContentExtensionResponseOption : nuint {
		DoNotDismiss,
		Dismiss,
		DismissAndForwardAction
	}

	interface IUNNotificationContentExtension { }

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[Unavailable (PlatformName.TvOS)]
	[Protocol]
	interface UNNotificationContentExtension {

		[Abstract]
		[Export ("didReceiveNotification:")]
		void DidReceiveNotification (UNNotification notification);

		[Export ("didReceiveNotificationResponse:completionHandler:")]
		void DidReceiveNotificationResponse (UNNotificationResponse response, Action<UNNotificationContentExtensionResponseOption> completion);

		[Export ("mediaPlayPauseButtonType", ArgumentSemantic.Assign)]
		UNNotificationContentExtensionMediaPlayPauseButtonType MediaPlayPauseButtonType { get; }

		[Export ("mediaPlayPauseButtonFrame", ArgumentSemantic.Assign)]
		CGRect MediaPlayPauseButtonFrame { get; }

		[Export ("mediaPlayPauseButtonTintColor", ArgumentSemantic.Copy)]
		UIColor MediaPlayPauseButtonTintColor { get; }

		[Export ("mediaPlay")]
		void PlayMedia ();

		[Export ("mediaPause")]
		void PauseMedia ();
	}

	[Introduced (PlatformName.iOS, 10, 0)]
	[Unavailable (PlatformName.MacOSX)]
	[Unavailable (PlatformName.WatchOS)]
	[Unavailable (PlatformName.TvOS)]
	[Category]
	[BaseType (typeof (NSExtensionContext))]
	interface NSExtensionContext_UNNotificationContentExtension {

		[Export ("mediaPlayingStarted")]
		void MediaPlayingStarted ();

		[Export ("mediaPlayingPaused")]
		void MediaPlayingPaused ();
	}
}

