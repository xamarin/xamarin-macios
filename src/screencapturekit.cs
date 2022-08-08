//
// ScreenCaptureKit bindings
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright (C) Microsoft Corporation. All rights reserved.
//

using System;
using ObjCRuntime;
using CoreVideo;
using CoreGraphics;
using Foundation;
using CoreFoundation;
using CoreMedia;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace ScreenCaptureKit {

	[NoiOS, NoTV, NoWatch, Mac (12,3), NoMacCatalyst]
	[ErrorDomain ("SCStreamErrorDomain")]
	[Native]
	enum SCStreamErrorCode : long {
		UserDeclined = -3801,
		FailedToStart = -3802,
		MissingEntitlements = -3803,
		FailedApplicationConnectionInvalid = -3804,
		FailedApplicationConnectionInterrupted = -3805,
		FailedNoMatchingApplicationContext = -3806,
		AttemptToStartStreamState = -3807,
		AttemptToStopStreamState = -3808,
		AttemptToUpdateFilterState = -3809,
		AttemptToConfigState = -3810,
		InternalError = -3811,
		InvalidParameter = -3812,
		NoWindowList = -3813,
		NoDisplayList = -3814,
		NoCaptureSource = -3815,
		RemovingStream = -3816,
		UserStopped = -3817,
		FailedToStartAudioCapture = -3818,
		FailedToStopAudioCapture = -3819,
	}

	[NoiOS, NoTV, NoWatch, Mac (12,3), NoMacCatalyst]
	[Native]
	enum SCFrameStatus : long {
		Complete,
		Idle,
		Blank,
		Suspended,
		Started,
		Stopped,
	}

	[NoiOS, NoTV, NoWatch, Mac (12,3), NoMacCatalyst]
	[Native]
	enum SCStreamOutputType : long {
		Screen,
		[Mac (13,0)]
		Audio,
	}

	[NoiOS, NoTV, NoWatch, Mac (12,3), NoMacCatalyst]
	[Static]
	interface SCStreamFrameInfoKeys {

		[Field ("SCStreamFrameInfoStatus")]
		NSString Status { get; }

		[Field ("SCStreamFrameInfoDisplayTime")]
		NSString DisplayTime { get; }

		[Field ("SCStreamFrameInfoScaleFactor")]
		NSString InfoScaleFactor { get; }

		[Field ("SCStreamFrameInfoContentScale")]
		NSString ContentScale { get; }

		[Field ("SCStreamFrameInfoContentRect")]
		NSString ContentRect { get; }

		[Field ("SCStreamFrameInfoDirtyRects")]
		NSString DirtyRects { get; }
	}

	[NoiOS, NoTV, NoWatch, Mac (12,3), NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SCRunningApplication {

		[Export ("bundleIdentifier")]
		string BundleIdentifier { get; }

		[Export ("applicationName")]
		string ApplicationName { get; }

		[Export ("processID")]
		int ProcessId { get; }
	}

	[NoiOS, NoTV, NoWatch, Mac (12,3), NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SCWindow {

		[Export ("windowID")]
		uint WindowId { get; }

		[Export ("frame")]
		CGRect Frame { get; }

		[NullAllowed, Export ("title")]
		string Title { get; }

		[Export ("windowLayer")]
		nint WindowLayer { get; }

		[NullAllowed, Export ("owningApplication")]
		SCRunningApplication OwningApplication { get; }

		[Export ("onScreen")]
		bool OnScreen { [Bind ("isOnScreen")] get; }
	}


	[NoiOS, NoTV, NoWatch, Mac (12,3), NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SCDisplay {

		[Export ("displayID")]
		uint DisplayId { get; }

		[Export ("width")]
		nint Width { get; }

		[Export ("height")]
		nint Height { get; }

		[Export ("frame")]
		CGRect Frame { get; }
	}

	[NoiOS, NoTV, NoWatch, Mac (12,3), NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SCShareableContent {

		[Async]
		[Static]
		[Export ("getShareableContentWithCompletionHandler:")]
		void GetShareableContent (Action<SCShareableContent, NSError> completionHandler);

		[Async]
		[Static]
		[Export ("getShareableContentExcludingDesktopWindows:onScreenWindowsOnly:completionHandler:")]
		void GetShareableContent (bool excludeDesktopWindows, bool onScreenWindowsOnly, Action<SCShareableContent, NSError> completionHandler);

		[Async]
		[Static]
		[Export ("getShareableContentExcludingDesktopWindows:onScreenWindowsOnlyBelowWindow:completionHandler:")]
		void GetShareableContentBelowWindow (bool excludeDesktopWindows, SCWindow onScreenWindowsOnlyBelowWindow, Action<SCShareableContent, NSError> completionHandler);

		[Async]
		[Static]
		[Export ("getShareableContentExcludingDesktopWindows:onScreenWindowsOnlyAboveWindow:completionHandler:")]
		void GetShareableContentAboveWindow (bool excludeDesktopWindows, SCWindow onScreenWindowsOnlyAboveWindow, Action<SCShareableContent, NSError> completionHandler);

		[Export ("windows")]
		SCWindow [] Windows { get; }

		[Export ("displays")]
		SCDisplay [] Displays { get; }

		[Export ("applications")]
		SCRunningApplication [] Applications { get; }
	}

	[NoiOS, NoTV, NoWatch, Mac (12,3), NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SCContentFilter {

		[Export ("initWithDesktopIndependentWindow:")]
		NativeHandle Constructor (SCWindow window);

		[Internal]
		[Export ("initWithDisplay:excludingWindows:")]
		NativeHandle InitWithDisplayExcludingWindows (SCDisplay display, SCWindow [] excludedWindows);

		[Internal]
		[Export ("initWithDisplay:includingWindows:")]
		NativeHandle InitWithDisplayIncludingWindows (SCDisplay display, SCWindow [] includedWindows);

		[Internal]
		[Export ("initWithDisplay:includingApplications:exceptingWindows:")]
		NativeHandle InitWithDisplayIncludingApplications (SCDisplay display, SCRunningApplication [] includingApplications, SCWindow [] exceptingWindows);

		[Internal]
		[Export ("initWithDisplay:excludingApplications:exceptingWindows:")]
		NativeHandle InitWithDisplayExcludingApplications (SCDisplay display, SCRunningApplication [] excludingApplications, SCWindow [] exceptingWindows);
	}

	[NoiOS, NoTV, NoWatch, Mac (12,3), NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	interface SCStreamConfiguration {

		[Export ("width")]
		nuint Width { get; set; }

		[Export ("height")]
		nuint Height { get; set; }

		[Export ("minimumFrameInterval", ArgumentSemantic.Assign)]
		CMTime MinimumFrameInterval { get; set; }

		[Export ("pixelFormat")]
		CVPixelFormatType PixelFormat { get; set; }

		[Export ("scalesToFit")]
		bool ScalesToFit { get; set; }

		[Export ("showsCursor")]
		bool ShowsCursor { get; set; }

		[Export ("backgroundColor", ArgumentSemantic.Assign)]
		CGColor BackgroundColor { get; set; }

		[Export ("sourceRect", ArgumentSemantic.Assign)]
		CGRect SourceRect { get; set; }

		[Export ("destinationRect", ArgumentSemantic.Assign)]
		CGRect DestinationRect { get; set; }

		[Export ("queueDepth")]
		nint QueueDepth { get; set; }

		// Usign weak prefix in case we want to strong-type these puppies in the future.

		[Advice ("Use the constants inside 'CGDisplayStreamYCbCrMatrixOptionKeys' class.")]
		[Export ("colorMatrix", ArgumentSemantic.Assign)]
		NSString WeakColorMatrix { get; set; }

		[Advice ("Use the constants inside 'CGColorSpaceNames' class.")]
		[Export ("colorSpaceName", ArgumentSemantic.Assign)]
		NSString WeakColorSpaceName { get; set; }

		[Mac (13, 0)]
		[Export ("capturesAudio")]
		bool CapturesAudio { get; set; }

		[Mac (13, 0)]
		[Export ("sampleRate")]
		nint SampleRate { get; set; }

		[Mac (13, 0)]
		[Export ("channelCount")]
		nint ChannelCount { get; set; }

		[Mac (13, 0)]
		[Export ("excludesCurrentProcessAudio")]
		bool ExcludesCurrentProcessAudio { get; set; }
	}

	[NoiOS, NoTV, NoWatch, Mac (12,3), NoMacCatalyst]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SCStream {

		[Export ("initWithFilter:configuration:delegate:")]
		NativeHandle Constructor (SCContentFilter contentFilter, SCStreamConfiguration streamConfig, [NullAllowed] ISCStreamDelegate aDelegate);

		[Export ("addStreamOutput:type:sampleHandlerQueue:error:")]
		bool AddStreamOutput (ISCStreamOutput output, SCStreamOutputType type, [NullAllowed] DispatchQueue sampleHandlerQueue, [NullAllowed] out NSError error);

		[Export ("removeStreamOutput:type:error:")]
		bool RemoveStreamOutput (ISCStreamOutput output, SCStreamOutputType type, [NullAllowed] out NSError error);

		[Async]
		[Export ("updateContentFilter:completionHandler:")]
		void UpdateContentFilter (SCContentFilter contentFilter, [NullAllowed] Action<NSError> completionHandler);

		[Async]
		[Export ("updateConfiguration:completionHandler:")]
		void UpdateConfiguration (SCStreamConfiguration streamConfig, [NullAllowed] Action<NSError> completionHandler);

		// No Async even on Swift and it makes sense, these are callback APIs.
		[Export ("startCaptureWithCompletionHandler:")]
		void StartCapture ([NullAllowed] Action<NSError> completionHandler);

		// No Async even on Swift and it makes sense, these are callback APIs.
		[Export ("stopCaptureWithCompletionHandler:")]
		void StopCapture ([NullAllowed] Action<NSError> completionHandler);

		[Mac (13, 0)]
		[Export ("synchronizationClock")]
		CMClock SynchronizationClock { [return: NullAllowed] get; }
	}

	interface ISCStreamDelegate {}

	[NoiOS, NoTV, NoWatch, Mac (12,3), NoMacCatalyst]
	[Protocol]
#if NET
	[Model]
#else
	[Model (AutoGeneratedName = true)]
#endif
	[BaseType (typeof (NSObject))]
	interface SCStreamDelegate {

		[Export ("stream:didStopWithError:")]
		void DidStop (SCStream stream, NSError error);
	}

	interface ISCStreamOutput {}

	[NoiOS, NoTV, NoWatch, Mac (12,3), NoMacCatalyst]
	[Protocol]
	interface SCStreamOutput {

		[Export ("stream:didOutputSampleBuffer:ofType:")]
		void DidOutputSampleBuffer (SCStream stream, CMSampleBuffer sampleBuffer, SCStreamOutputType type);
	}
}
