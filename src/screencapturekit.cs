//
// ScreenCaptureKit bindings
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright (C) Microsoft Corporation. All rights reserved.
//

using System;
using System.ComponentModel;
using AVFoundation;
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

	[NoiOS, NoTV, Mac (12, 3), MacCatalyst (18, 2)]
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
		FailedToStartMicrophoneCapture = -3820,
		SystemStoppedStream = -3821,
	}

	[NoiOS, NoTV, Mac (12, 3), MacCatalyst (18, 2)]
	[Native]
	enum SCFrameStatus : long {
		Complete,
		Idle,
		Blank,
		Suspended,
		Started,
		Stopped,
	}

	[NoiOS, NoTV, Mac (12, 3), MacCatalyst (18, 2)]
	[Native]
	enum SCStreamOutputType : long {
		Screen,
		[Mac (13, 0)]
		Audio,
		[Mac (15, 0)]
		Microphone,
	}

	[Deprecated (PlatformName.MacOSX, 15, 0, message: "Use 'SCShareableContentStyle' instead.")]
	[NoiOS, NoTV, Mac (14, 0), NoMacCatalyst]
	[Native]
	public enum SCStreamType : long {
		Window,
		Display,
	}

	[NoiOS, NoTV, Mac (14, 0), MacCatalyst (18, 2)]
	[Native]
	public enum SCPresenterOverlayAlertSetting : long {
		System,
		Never,
		Always,
	}

	[NoiOS, NoTV, Mac (14, 0), MacCatalyst (18, 2)]
	[Native]
	public enum SCCaptureResolutionType : long {
		Automatic,
		Best,
		Nominal,
	}

	[Flags, NoiOS, NoTV, Mac (14, 0), MacCatalyst (18, 2)]
	[Native]
	public enum SCContentSharingPickerMode : ulong {
		SingleWindow = 1 << 0,
		MultipleWindows = 1 << 1,
		SingleApplication = 1 << 2,
		MultipleApplications = 1 << 3,
		SingleDisplay = 1 << 4,
	}

	[NoiOS, NoTV, Mac (14, 0), MacCatalyst (18, 2)]
	[Native]
	public enum SCShareableContentStyle : long {
		None,
		Window,
		Display,
		Application,
	}

	[NoiOS, NoTV, Mac (15, 0), MacCatalyst (18, 2)]
	[Native]
	public enum SCCaptureDynamicRange : long {
		Sdr,
		HdrLocalDisplay,
		HdrCanonicalDisplay,
	}

	[NoiOS, NoTV, Mac (15, 0), MacCatalyst (18, 2)]
	[Native]
	public enum SCStreamConfigurationPreset : long {
		CaptureHdrStreamLocalDisplay,
		CaptureHdrStreamCanonicalDisplay,
		CaptureHdrScreenshotLocalDisplay,
		CaptureHdrScreenshotCanonicalDisplay,
	}

	[NoiOS, NoTV, Mac (12, 3), MacCatalyst (18, 2)]
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

		[Mac (13, 1)]
		[Field ("SCStreamFrameInfoScreenRect")]
		NSString ScreenRect { get; }

		[Mac (14, 0)]
		[Field ("SCStreamFrameInfoBoundingRect")]
		NSString BoundingRect { get; }

		[Mac (14, 2)]
		[Field ("SCStreamFrameInfoPresenterOverlayContentRect")]
		NSString PresenterOverlayContentRect { get; }
	}

	[NoiOS, NoTV, Mac (12, 3), MacCatalyst (18, 2)]
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

	[NoiOS, NoTV, Mac (12, 3), MacCatalyst (18, 2)]
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

		[Mac (13, 1)]
		[Export ("active")]
		bool Active { [Bind ("isActive")] get; }
	}


	[NoiOS, NoTV, Mac (12, 3), MacCatalyst (18, 2)]
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

	[NoiOS, NoTV, Mac (12, 3), MacCatalyst (18, 2)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SCShareableContent {

		[Async]
		[Static]
		[Export ("getShareableContentWithCompletionHandler:")]
		void GetShareableContent (Action<SCShareableContent, NSError> completionHandler);

		[Mac (14, 4)]
		[Async]
		[Static]
		[Export ("getCurrentProcessShareableContentWithCompletionHandler:")]
		void GetCurrentProcessShareableContent (Action<SCShareableContent, NSError> completionHandler);

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

		[Mac (14, 0)]
		[Static]
		[Export ("infoForFilter:")]
		SCShareableContentInfo GetInfo (SCContentFilter filter);
	}

	[NoiOS, NoTV, Mac (12, 3), MacCatalyst (18, 2)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SCContentFilter {

		[Export ("initWithDesktopIndependentWindow:")]
		NativeHandle Constructor (SCWindow window);

		[Internal]
		[Export ("initWithDisplay:excludingWindows:")]
		NativeHandle _InitWithDisplayExcludingWindows (SCDisplay display, SCWindow [] excludedWindows);

		[Internal]
		[Export ("initWithDisplay:includingWindows:")]
		NativeHandle _InitWithDisplayIncludingWindows (SCDisplay display, SCWindow [] includedWindows);

		[Internal]
		[Export ("initWithDisplay:includingApplications:exceptingWindows:")]
		NativeHandle _InitWithDisplayIncludingApplications (SCDisplay display, SCRunningApplication [] includingApplications, SCWindow [] exceptingWindows);

		[Internal]
		[Export ("initWithDisplay:excludingApplications:exceptingWindows:")]
		NativeHandle _InitWithDisplayExcludingApplications (SCDisplay display, SCRunningApplication [] excludingApplications, SCWindow [] exceptingWindows);

		// per docs, the following selectors are available for 12.3+
		// but return types are SCStreamType and SCShareableContentStyle are 14.0+
		[Deprecated (PlatformName.MacOSX, 14, 2, message: "Use 'Style' instead.")]
		[NoMacCatalyst]
		[Mac (14, 0)]
		[Export ("streamType")]
		SCStreamType StreamType { get; }

		[Mac (14, 0)]
		[Export ("style")]
		SCShareableContentStyle Style { get; }

		[Mac (14, 0)]
		[Export ("pointPixelScale")]
		float PointPixelScale { get; }

		[Mac (14, 0)]
		[Export ("contentRect")]
		CGRect ContentRect { get; }

		[Mac (14, 2)]
		[Export ("includeMenuBar")]
		bool IncludeMenuBar { get; set; }

		[Mac (15, 2), MacCatalyst (18, 2)]
		[Export ("includedDisplays")]
		SCDisplay [] IncludedDisplays { get; }

		[Mac (15, 2), MacCatalyst (18, 2)]
		[Export ("includedApplications")]
		SCRunningApplication [] IncludedApplications { get; }

		[Mac (15, 2), MacCatalyst (18, 2)]
		[Export ("includedWindows")]
		SCWindow [] IncludedWindows { get; }
	}

	[NoiOS, NoTV, Mac (12, 3), MacCatalyst (18, 2)]
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

		[Mac (14, 0)]
		[Export ("preservesAspectRatio")]
		bool PreservesAspectRatio { get; set; }

		[Mac (14, 0)]
		[NullAllowed]
		[Export ("streamName", ArgumentSemantic.Strong)]
		string StreamName { get; set; }

		[Mac (14, 0)]
		[Export ("ignoreShadowsDisplay")]
		bool IgnoreShadowsDisplay { get; set; }

		[Mac (14, 0)]
		[Export ("ignoreShadowsSingleWindow")]
		bool IgnoreShadowsSingleWindow { get; set; }

		[Mac (14, 0)]
		[Export ("captureResolution", ArgumentSemantic.Assign)]
		SCCaptureResolutionType CaptureResolution { get; set; }

		[Mac (14, 0)]
		[Export ("capturesShadowsOnly")]
		bool CapturesShadowsOnly { get; set; }

		[Mac (14, 0)]
		[Export ("shouldBeOpaque")]
		bool ShouldBeOpaque { get; set; }

		[Mac (14, 0)]
		[Export ("ignoreGlobalClipDisplay")]
		bool IgnoreGlobalClipDisplay { get; set; }

		[Mac (14, 0)]
		[Export ("ignoreGlobalClipSingleWindow")]
		bool IgnoreGlobalClipSingleWindow { get; set; }

		[Mac (14, 0)]
		[Export ("presenterOverlayPrivacyAlertSetting", ArgumentSemantic.Assign)]
		SCPresenterOverlayAlertSetting PresenterOverlayPrivacyAlertSetting { get; set; }

		[Mac (14, 2)]
		[Export ("includeChildWindows")]
		bool IncludeChildWindows { get; set; }

		[Mac (15, 0)]
		[Export ("showMouseClicks", ArgumentSemantic.Assign)]
		bool ShowMouseClicks { get; set; }

		[Mac (15, 0)]
		[Export ("captureMicrophone", ArgumentSemantic.Assign)]
		bool CaptureMicrophone { get; set; }

		[NoMacCatalyst]
		[Mac (15, 0)]
		[Export ("microphoneCaptureDeviceID", ArgumentSemantic.Strong), NullAllowed]
		string MicrophoneCaptureDeviceId { get; set; }

		[Mac (15, 0)]
		[Export ("captureDynamicRange", ArgumentSemantic.Assign)]
		SCCaptureDynamicRange CaptureDynamicRange { get; set; }

		[Static]
		[Mac (15, 0)]
		[Export ("streamConfigurationWithPreset:")]
		SCStreamConfiguration Create (SCStreamConfigurationPreset preset);
	}

	[NoiOS, NoTV, Mac (12, 3), MacCatalyst (18, 2)]
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
		[NullAllowed]
		CMClock SynchronizationClock { get; }

		[Mac (15, 0)]
		[Export ("addRecordingOutput:error:")]
		bool AddRecordingOutput (SCRecordingOutput recordingOutput, out NSError error);

		[Mac (15, 0)]
		[Export ("removeRecordingOutput:error:")]
		bool RemoveRecordingOutput (SCRecordingOutput recordingOutput, out NSError error);
	}

	interface ISCStreamDelegate { }

	[NoiOS, NoTV, Mac (12, 3), MacCatalyst (18, 2)]
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

#if !XAMCORE_5_0
		// Looks like this was a beta method that got removed in stable, but we ended up releasing the binding for it anyways.
		[EditorBrowsable (EditorBrowsableState.Never)]
		[Obsolete ("Do not use this method.")]
		[Mac (14, 4)]
		[NoMacCatalyst]
		[Export ("userDidStopStream:")]
		void UserDidStop (SCStream stream);
#endif

		[Mac (14, 0)]
		[Export ("outputVideoEffectDidStartForStream:")]
		void OutputVideoEffectDidStart (SCStream stream);

		[Mac (14, 0)]
		[Export ("outputVideoEffectDidStopForStream:")]
		void OutputVideoEffectDidStop (SCStream stream);

		[Mac (15, 2), MacCatalyst (18, 2)]
		[Export ("streamDidBecomeActive:")]
		void StreamDidBecomeActive (SCStream stream);

		[Mac (15, 2), MacCatalyst (18, 2)]
		[Export ("streamDidBecomeInactive:")]
		void StreamDidBecomeInactive (SCStream stream);
	}

	interface ISCStreamOutput { }

	[NoiOS, NoTV, Mac (12, 3), MacCatalyst (18, 2)]
	[Protocol]
	interface SCStreamOutput {

		[Export ("stream:didOutputSampleBuffer:ofType:")]
		void DidOutputSampleBuffer (SCStream stream, CMSampleBuffer sampleBuffer, SCStreamOutputType type);
	}

	[NoiOS, NoTV, Mac (14, 0), MacCatalyst (18, 2)]
	[BaseType (typeof (NSObject))]
	interface SCContentSharingPickerConfiguration {
		[Export ("allowedPickerModes", ArgumentSemantic.Assign)]
		SCContentSharingPickerMode AllowedPickerModes { get; set; }

		[Export ("excludedWindowIDs", ArgumentSemantic.Strong)]
		NSNumber [] ExcludedWindowIds { get; set; }

		[Export ("excludedBundleIDs", ArgumentSemantic.Strong)]
		string [] ExcludedBundleIds { get; set; }

		[Export ("allowsChangingSelectedContent")]
		bool AllowsChangingSelectedContent { get; set; }
	}

	[NoiOS, NoTV, Mac (14, 0), MacCatalyst (18, 2)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SCContentSharingPicker {
		[Static]
		[Export ("sharedPicker")]
		SCContentSharingPicker SharedPicker { get; }

		[Export ("defaultConfiguration", ArgumentSemantic.Copy)]
		SCContentSharingPickerConfiguration DefaultConfiguration { get; set; }

		[NullAllowed]
		[BindAs (typeof (int))]
		[Export ("maximumStreamCount", ArgumentSemantic.Strong)]
		NSNumber MaximumStreamCount { get; set; }

		[Export ("active")]
		bool Active { [Bind ("isActive")] get; set; }

		[Export ("addObserver:")]
		void AddObserver (ISCContentSharingPickerObserver observer);

		[Export ("removeObserver:")]
		void RemoveObserver (ISCContentSharingPickerObserver observer);

		[Export ("setConfiguration:forStream:")]
		void SetConfiguration ([NullAllowed] SCContentSharingPickerConfiguration pickerConfig, SCStream stream);

		[Export ("present")]
		void Present ();

		[Export ("presentPickerUsingContentStyle:")]
		void Present (SCShareableContentStyle contentStyle);

		[Export ("presentPickerForStream:")]
		void Present (SCStream stream);

		[Export ("presentPickerForStream:usingContentStyle:")]
		void Present (SCStream stream, SCShareableContentStyle contentStyle);
	}

	interface ISCContentSharingPickerObserver { }

	[NoiOS, NoTV, Mac (14, 0), MacCatalyst (18, 2)]
	[Protocol]
	[Model]
	[BaseType (typeof (NSObject))]
	interface SCContentSharingPickerObserver {
		[Abstract]
		[Export ("contentSharingPicker:didCancelForStream:")]
		void DidCancel (SCContentSharingPicker picker, [NullAllowed] SCStream stream);

		[Abstract]
		[Export ("contentSharingPicker:didUpdateWithFilter:forStream:")]
		void DidUpdate (SCContentSharingPicker picker, SCContentFilter filter, [NullAllowed] SCStream stream);

		[Abstract]
		[Export ("contentSharingPickerStartDidFailWithError:")]
		void DidFail (NSError error);
	}

	[NoiOS, NoTV, Mac (14, 0), MacCatalyst (18, 2)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SCShareableContentInfo {
		[Export ("style")]
		SCShareableContentStyle Style { get; }

		[Export ("pointPixelScale")]
		float PointPixelScale { get; }

		[Export ("contentRect")]
		CGRect ContentRect { get; }
	}

	delegate void SCScreenshotManagerCaptureImageCallback ([NullAllowed] CGImage image, [NullAllowed] NSError error);

	[NoiOS, NoTV, Mac (14, 0), MacCatalyst (18, 2)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SCScreenshotManager {
		[Static]
		[Export ("captureSampleBufferWithFilter:configuration:completionHandler:")]
		[Async]
		void CaptureSampleBuffer (SCContentFilter contentFilter, SCStreamConfiguration config, [NullAllowed] Action<CMSampleBuffer, NSError> completionHandler);

		[Static]
		[Export ("captureImageWithFilter:configuration:completionHandler:")]
		[Async]
		void CaptureImage (SCContentFilter contentFilter, SCStreamConfiguration config, [NullAllowed] Action<CGImage, NSError> completionHandler);

		[Mac (15, 2)]
		[Static]
		[Export ("captureImageInRect:completionHandler:")]
		[Async]
		void CaptureImage (CGRect rect, [NullAllowed] SCScreenshotManagerCaptureImageCallback completionHandler);
	}

	[Mac (15, 0), NoiOS, NoTV, MacCatalyst (18, 2)]
	[BaseType (typeof (NSObject))]
	interface SCRecordingOutputConfiguration {
		[Export ("outputURL", ArgumentSemantic.Copy)]
		NSUrl OutputUrl { get; set; }

		[Export ("videoCodecType", ArgumentSemantic.Copy)]
		[BindAs (typeof (AVVideoCodecType))]
		NSString VideoCodecType { get; set; }

		[Export ("outputFileType", ArgumentSemantic.Copy)]
		[BindAs (typeof (AVFileTypes))]
		NSString OutputFileType { get; set; }

		[Export ("availableVideoCodecTypes")]
		[BindAs (typeof (AVVideoCodecType []))]
		NSString [] AvailableVideoCodecTypes { get; }

		[Export ("availableOutputFileTypes")]
		[BindAs (typeof (AVFileTypes []))]
		NSString [] AvailableOutputFileTypes { get; }
	}

	[Mac (15, 0), NoiOS, NoTV, MacCatalyst (18, 2)]
	[Protocol (BackwardsCompatibleCodeGeneration = false), Model]
	[BaseType (typeof (NSObject))]
	interface SCRecordingOutputDelegate {
		[Export ("recordingOutputDidStartRecording:")]
		void DidStartRecording (SCRecordingOutput recordingOutput);

		[Export ("recordingOutput:didFailWithError:")]
		void DidFail (SCRecordingOutput recordingOutput, NSError error);

		[Export ("recordingOutputDidFinishRecording:")]
		void DidFinishRecording (SCRecordingOutput recordingOutput);
	}

	interface ISCRecordingOutputDelegate { }

	[Mac (15, 0), NoiOS, NoTV, MacCatalyst (18, 2)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SCRecordingOutput {
		[Export ("recordedDuration")]
		CMTime RecordedDuration { get; }

		[Export ("recordedFileSize")]
		nint RecordedFileSize { get; }

		[Export ("initWithConfiguration:delegate:")]
		NativeHandle Constructor (SCRecordingOutputConfiguration recordingOutputConfiguration, ISCRecordingOutputDelegate @delegate);
	}
}
