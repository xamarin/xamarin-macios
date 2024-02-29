//
// UserNotificationsUI bindings
//
// Authors:
//	Alex Soto  <alex.soto@xamarin.com>
//
// Copyright 2016 Xamarin Inc. All rights reserved.
//

using System;
using CoreGraphics;
using Foundation;
using ObjCRuntime;
#if MONOMAC
using AppKit;
using UIColor = AppKit.NSColor;
#else
using UIKit;
#endif
using UserNotifications;

namespace UserNotificationsUI {

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[Unavailable (PlatformName.WatchOS)]
	[Unavailable (PlatformName.TvOS)]
	[Native]
	public enum UNNotificationContentExtensionMediaPlayPauseButtonType : ulong {
		None,
		Default,
		Overlay
	}

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[Unavailable (PlatformName.WatchOS)]
	[Unavailable (PlatformName.TvOS)]
	[Native]
	public enum UNNotificationContentExtensionResponseOption : ulong {
		DoNotDismiss,
		Dismiss,
		DismissAndForwardAction
	}

	interface IUNNotificationContentExtension { }

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
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

	[Introduced (PlatformName.MacCatalyst, 14, 0)]
	[Unavailable (PlatformName.WatchOS)]
	[Unavailable (PlatformName.TvOS)]
	[Category]
	[BaseType (typeof (NSExtensionContext))]
	interface NSExtensionContext_UNNotificationContentExtension {

		[Export ("mediaPlayingStarted")]
		void MediaPlayingStarted ();

		[Export ("mediaPlayingPaused")]
		void MediaPlayingPaused ();

		[MacCatalyst (14, 0)]
		[Export ("performNotificationDefaultAction")]
		void PerformNotificationDefaultAction ();

		[MacCatalyst (14, 0)]
		[Export ("dismissNotificationContentExtension")]
		void DismissNotificationContentExtension ();

		// property, but we have to add the two methods since it is a category.
		[MacCatalyst (14, 0)]
		[Export ("notificationActions")]
		UNNotificationAction [] GetNotificationActions ();

		[MacCatalyst (14, 0)]
		[Export ("setNotificationActions:")]
		void SetNotificationActions (UNNotificationAction [] actions);
	}
}
