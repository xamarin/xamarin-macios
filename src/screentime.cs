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

namespace ScreenTime {

	[Mac (11, 0), iOS (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface STScreenTimeConfiguration : NSSecureCoding {
		[Export ("enforcesChildRestrictions")]
		bool EnforcesChildRestrictions { get; }
	}

	[Mac (11,0), iOS (14,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface STScreenTimeConfigurationObserver {
		[Export ("initWithUpdateQueue:")]
		[DesignatedInitializer]
		IntPtr Constructor (DispatchQueue updateQueue);

		[Export ("startObserving")]
		void StartObserving ();

		[Export ("stopObserving")]
		void StopObserving ();

		[NullAllowed, Export ("configuration", ArgumentSemantic.Strong)]
		STScreenTimeConfiguration Configuration { get; }
	}

	[Mac (11, 0), iOS (14, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface STWebHistory {
		[Export ("initWithBundleIdentifier:")]
		[DesignatedInitializer]
		IntPtr Constructor (string bundleIdentifier);

		[Export ("deleteHistoryForURL:")]
		void DeleteHistory (NSUrl url);

		[Export ("deleteHistoryDuringInterval:")]
		void DeleteHistory (NSDateInterval interval);

		[Export ("deleteAllHistory")]
		void DeleteAllHistory ();
	}

	[Mac (11, 0), iOS (14, 0)]
	[BaseType (typeof (UIViewController))]
	[DisableDefaultCtor]
	interface STWebpageController
	{
		[DesignatedInitializer]
		[Export ("initWithNibName:bundle:")]
		IntPtr Constructor ([NullAllowed] string nibNameOrNull, [NullAllowed] NSBundle nibBundleOrNull);

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

		[Export ("bundleIdentifier")]
		string BundleIdentifier { get; set; }
	}

}
