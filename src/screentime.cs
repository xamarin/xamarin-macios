using System;
using ObjCRuntime;
using Foundation;
using CoreFoundation;

#if !MONOMAC
using UIKit;
#else
using AppKit;
using UIViewController = AppKit.NSViewController;
#endif

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace ScreenTime {

	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface STScreenTimeConfiguration : NSSecureCoding {
		[Export ("enforcesChildRestrictions")]
		bool EnforcesChildRestrictions { get; }
	}

	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface STScreenTimeConfigurationObserver {
		[Export ("initWithUpdateQueue:")]
		[DesignatedInitializer]
		NativeHandle Constructor (DispatchQueue updateQueue);

		[Export ("startObserving")]
		void StartObserving ();

		[Export ("stopObserving")]
		void StopObserving ();

		[NullAllowed, Export ("configuration", ArgumentSemantic.Strong)]
		STScreenTimeConfiguration Configuration { get; }
	}

	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface STWebHistory {

		[Export ("initWithBundleIdentifier:error:")]
		NativeHandle Constructor (string bundleIdentifier, [NullAllowed] out NSError error);

		[Export ("deleteHistoryForURL:")]
		void DeleteHistory (NSUrl url);

		[Export ("deleteHistoryDuringInterval:")]
		void DeleteHistory (NSDateInterval interval);

		[Export ("deleteAllHistory")]
		void DeleteAllHistory ();
	}

	[iOS (14, 0)]
	[MacCatalyst (14, 0)]
	[BaseType (typeof (UIViewController))]
	[DisableDefaultCtor]
	interface STWebpageController {
		[DesignatedInitializer]
		[Export ("initWithNibName:bundle:")]
		NativeHandle Constructor ([NullAllowed] string nibNameOrNull, [NullAllowed] NSBundle nibBundleOrNull);

		[Export ("suppressUsageRecording")]
		bool SuppressUsageRecording { get; set; }

		[NullAllowed, Export ("URL", ArgumentSemantic.Copy)]
		NSUrl Url { get; set; }

		[Export ("URLIsPlayingVideo")]
		bool UrlIsPlayingVideo { get; set; }

		[Export ("URLIsPictureInPicture")]
		bool UrlIsPictureInPicture { get; set; }

		[Export ("URLIsBlocked")]
		bool UrlIsBlocked { get; }

		[Export ("setBundleIdentifier:error:")]
		bool SetBundleIdentifier (string bundleIdentifier, [NullAllowed] out NSError error);
	}

}
