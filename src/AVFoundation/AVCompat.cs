// Copyright 2014-2016 Xamarin Inc. All rights reserved.

#if !WATCH

using System;
using System.ComponentModel;
using OpenTK;
using XamCore.CoreMedia;
using XamCore.Foundation;
using XamCore.ObjCRuntime;

namespace XamCore.AVFoundation {

#if !XAMCORE_2_0
	partial class AVAssetResourceLoadingDataRequest {

		[Obsolete ("Type is not meant to be created by user code.")]
		public AVAssetResourceLoadingDataRequest ()
		{
		}
	}
#endif

#if !XAMCORE_3_0
	partial class AVAsset {

		[Obsolete ("Use 'GetChapterMetadataGroups'.")]
		public virtual AVMetadataItem[] ChapterMetadataGroups (NSLocale forLocale, AVMetadataItem[] commonKeys)
		{
			return null;
		}
	}

	partial class AVAssetTrack {

		[Obsolete ("Use 'GetAssociatedTracks'.")]
		public virtual NSString GetAssociatedTracksOfType (NSString avAssetTrackTrackAssociationType)
		{
			return null;
		}
	}

	partial class AVMutableCompositionTrack {

		[Obsolete ("Use 'InsertTimeRanges' overload accepting an 'NSValue' array.")]
		public virtual bool InsertTimeRanges (NSValue cmTimeRanges, AVAssetTrack[] tracks, CMTime startTime, out NSError error)
		{
			return InsertTimeRanges (new NSValue [] { cmTimeRanges }, tracks, startTime, out error);
		}
	}


	partial class AVCaptureAudioDataOutputSampleBufferDelegate {

		[Obsolete ("This member only exists for 'AVCaptureVideoDataOutputSampleBufferDelegate'.")]
		public virtual void DidDropSampleBuffer (AVCaptureOutput captureOutput, CMSampleBuffer sampleBuffer, AVCaptureConnection connection)
		{
		}
	}

	static partial class AVCaptureAudioDataOutputSampleBufferDelegate_Extensions {

		[Obsolete ("This member only exists for 'AVCaptureVideoDataOutputSampleBufferDelegate'.")]
		public static void DidDropSampleBuffer (IAVCaptureAudioDataOutputSampleBufferDelegate This, AVCaptureOutput captureOutput, CMSampleBuffer sampleBuffer, AVCaptureConnection connection)
		{
		}
	}
#endif

#if !XAMCORE_4_0
	partial class AVCaptureInputPort {

		[Obsolete ("Valid instance of this type cannot be directly created.")]
		public AVCaptureInputPort ()
		{
		}
	}
	
	partial class AVAudioChannelLayout {

		[Obsolete ("Valid instance of this type cannot be directly created.")]
		public AVAudioChannelLayout ()
		{
		}
	}

	partial class AVAudioConnectionPoint {

		[Obsolete ("Valid instance of this type cannot be directly created.")]
		public AVAudioConnectionPoint ()
		{
		}
	}

	partial class AVAudioUnitComponentManager {

		[Obsolete ("Please use the static 'SharedInstance' property as this type is not meant to be created.")]
		public AVAudioUnitComponentManager ()
		{
		}
	}

#if !MONOMAC
	partial class AVSampleBufferAudioRenderer
	{
		[Obsolete ("This API is not available on this platform.")]
		public virtual string AudioOutputDeviceUniqueId {
			get { throw new NotImplementedException (); }
			set { throw new NotImplementedException (); }
		}
	}
#endif

#if !IOS
	partial class AVContentKeyRequest
	{
		[Obsolete ("This API is not available on this platform.")]
		public virtual void RespondByRequestingPersistableContentKeyRequest ()
		{
			throw new NotImplementedException ();
		}
	}
#endif

#if TVOS
	// tvOS removed some types - we need to keep stubs of them for binary compatibility
	[Obsolete ("Removed in tvOS 10.")]
	[Deprecated (PlatformName.TvOS, 10, 0, PlatformArchitecture.None)]
	public class AVAssetDownloadDelegate : NSObject, IAVAssetDownloadDelegate {
		public AVAssetDownloadDelegate ()
		{
			throw new NotImplementedException ();
		}

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		protected AVAssetDownloadDelegate (NSObjectFlag t)
		{
			throw new NotImplementedException ();
		}

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		protected internal AVAssetDownloadDelegate (IntPtr handle)
		{
			throw new NotImplementedException ();
		}

		public virtual void DidCompleteWithError (NSUrlSession session, NSUrlSessionTask task, NSError error)
		{
			throw new NotImplementedException ();
		}

		public virtual void DidFinishCollectingMetrics (NSUrlSession session, NSUrlSessionTask task, NSUrlSessionTaskMetrics metrics)
		{
			throw new NotImplementedException ();
		}

		public virtual void DidFinishDownloadingToUrl (NSUrlSession session, AVAssetDownloadTask assetDownloadTask, NSUrl location)
		{
			throw new NotImplementedException ();
		}

		public virtual void DidLoadTimeRange (NSUrlSession session, AVAssetDownloadTask assetDownloadTask, CMTimeRange timeRange, NSValue[] loadedTimeRanges, CMTimeRange timeRangeExpectedToLoad)
		{
			throw new NotImplementedException ();
		}

		public virtual void DidReceiveChallenge (NSUrlSession session, NSUrlSessionTask task, NSUrlAuthenticationChallenge challenge, [BlockProxy (typeof(Trampolines.NIDActionArity2V0))] Action<NSUrlSessionAuthChallengeDisposition, NSUrlCredential> completionHandler)
		{
			throw new NotImplementedException ();
		}

		public virtual void DidResolveMediaSelection (NSUrlSession session, AVAssetDownloadTask assetDownloadTask, AVMediaSelection resolvedMediaSelection)
		{
			throw new NotImplementedException ();
		}

		public virtual void DidSendBodyData (NSUrlSession session, NSUrlSessionTask task, long bytesSent, long totalBytesSent, long totalBytesExpectedToSend)
		{
			throw new NotImplementedException ();
		}

		public virtual void NeedNewBodyStream (NSUrlSession session, NSUrlSessionTask task, [BlockProxy (typeof(Trampolines.NIDActionArity1V0))] Action<NSInputStream> completionHandler)
		{
			throw new NotImplementedException ();
		}

		public virtual void WillPerformHttpRedirection (NSUrlSession session, NSUrlSessionTask task, NSHttpUrlResponse response, NSUrlRequest newRequest, [BlockProxy (typeof(Trampolines.NIDActionArity1V1))] Action<NSUrlRequest> completionHandler)
		{
			throw new NotImplementedException ();
		}
	}

	[Obsolete ("Removed in tvOS 10.")]
	[Deprecated (PlatformName.TvOS, 10, 0, PlatformArchitecture.None)]
	public interface IAVAssetDownloadDelegate : INativeObject, IDisposable, INSUrlSessionTaskDelegate, INSUrlSessionDelegate {
	}

	[Obsolete ("Removed in tvOS 10.")]
	[Deprecated (PlatformName.TvOS, 10, 0, PlatformArchitecture.None)]
	public static class AVAssetDownloadDelegate_Extensions {

		public static void DidFinishDownloadingToUrl (this IAVAssetDownloadDelegate This, NSUrlSession session, AVAssetDownloadTask assetDownloadTask, NSUrl location)
		{
			throw new NotImplementedException ();
		}

		public static void DidLoadTimeRange (this IAVAssetDownloadDelegate This, NSUrlSession session, AVAssetDownloadTask assetDownloadTask, CMTimeRange timeRange, NSValue[] loadedTimeRanges, CMTimeRange timeRangeExpectedToLoad)
		{
			throw new NotImplementedException ();
		}

		public static void DidResolveMediaSelection (this IAVAssetDownloadDelegate This, NSUrlSession session, AVAssetDownloadTask assetDownloadTask, AVMediaSelection resolvedMediaSelection)
		{
			throw new NotImplementedException ();
		}
	}

	[Obsolete ("Removed in tvOS 10.")]
	[Deprecated (PlatformName.TvOS, 10, 0, PlatformArchitecture.None)]
	public class AVAssetDownloadTask : NSUrlSessionTask {

		public override IntPtr ClassHandle {
			get {
				throw new NotImplementedException ();
			}
		}

		public override NSUrlRequest CurrentRequest {
			get {
				throw new NotImplementedException ();
			}
		}

		public virtual NSUrl DestinationUrl {
			get {
				throw new NotImplementedException ();
			}
		}

		public virtual NSValue[] LoadedTimeRanges {
			get {
				throw new NotImplementedException ();
			}
		}

		public virtual NSDictionary<NSString, NSObject> Options {
			get {
				throw new NotImplementedException ();
			}
		}

		public override NSUrlRequest OriginalRequest {
			get {
				throw new NotImplementedException ();
			}
		}

		public override NSUrlResponse Response {
			get {
				throw new NotImplementedException ();
			}
		}

		public virtual AVUrlAsset UrlAsset {
			get {
				throw new NotImplementedException ();
			}
		}

		[EditorBrowsable (EditorBrowsableState.Advanced),]
		protected AVAssetDownloadTask (NSObjectFlag t)
		{
			throw new NotImplementedException ();
		}

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		protected internal AVAssetDownloadTask (IntPtr handle)
		{
			throw new NotImplementedException ();
		}
	}

	[Obsolete ("Removed in tvOS 10.")]
	[Deprecated (PlatformName.TvOS, 10, 0, PlatformArchitecture.None)]
	public class AVAssetDownloadUrlSession : NSUrlSession {

		public new static NSUrlSession SharedSession {
			get {
				throw new NotImplementedException ();
			}
		}

		public override IntPtr ClassHandle {
			get {
				throw new NotImplementedException ();
			}
		}

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		protected internal AVAssetDownloadUrlSession (IntPtr handle) : base (handle)
		{
			throw new NotImplementedException ();
		}

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		protected AVAssetDownloadUrlSession (NSObjectFlag t) : base (t)
		{
			throw new NotImplementedException ();
		}

		public static AVAssetDownloadUrlSession CreateSession (NSUrlSessionConfiguration configuration, IAVAssetDownloadDelegate @delegate, NSOperationQueue delegateQueue)
		{
			throw new NotImplementedException ();
		}

		public new static NSUrlSession FromConfiguration (NSUrlSessionConfiguration configuration)
		{
			throw new NotImplementedException ();
		}

		[Obsolete ("Use the overload with a 'INSUrlSessionDelegate' parameter.")]
		public new static NSUrlSession FromConfiguration (NSUrlSessionConfiguration configuration, NSUrlSessionDelegate sessionDelegate, NSOperationQueue delegateQueue)
		{
			throw new NotImplementedException ();
		}

		public new static NSUrlSession FromWeakConfiguration (NSUrlSessionConfiguration configuration, NSObject weakDelegate, NSOperationQueue delegateQueue)
		{
			throw new NotImplementedException ();
		}

		public override NSUrlSessionDataTask CreateDataTask (NSUrlRequest request)
		{
			throw new NotImplementedException ();
		}

		public override NSUrlSessionDataTask CreateDataTask (NSUrl url)
		{
			throw new NotImplementedException ();
		}

		public override NSUrlSessionDataTask CreateDataTask (NSUrlRequest request, NSUrlSessionResponse completionHandler)
		{
			throw new NotImplementedException ();
		}

		public override NSUrlSessionDataTask CreateDataTask (NSUrl url, NSUrlSessionResponse completionHandler)
		{
			throw new NotImplementedException ();
		}

		public override NSUrlSessionDownloadTask CreateDownloadTask (NSUrlRequest request, NSUrlDownloadSessionResponse completionHandler)
		{
			throw new NotImplementedException ();
		}

		public override NSUrlSessionDownloadTask CreateDownloadTask (NSUrl url, NSUrlDownloadSessionResponse completionHandler)
		{
			throw new NotImplementedException ();
		}

		public override NSUrlSessionDownloadTask CreateDownloadTask (NSData resumeData)
		{
			throw new NotImplementedException ();
		}

		public override NSUrlSessionDownloadTask CreateDownloadTask (NSUrl url)
		{
			throw new NotImplementedException ();
		}

		public override NSUrlSessionDownloadTask CreateDownloadTask (NSUrlRequest request)
		{
			throw new NotImplementedException ();
		}

		public override NSUrlSessionDownloadTask CreateDownloadTaskFromResumeData (NSData resumeData, NSUrlDownloadSessionResponse completionHandler)
		{
			throw new NotImplementedException ();
		}

		public override NSUrlSessionUploadTask CreateUploadTask (NSUrlRequest request, NSUrl fileURL)
		{
			throw new NotImplementedException ();
		}

		public override NSUrlSessionUploadTask CreateUploadTask (NSUrlRequest request, NSData bodyData)
		{
			throw new NotImplementedException ();
		}

		public override NSUrlSessionUploadTask CreateUploadTask (NSUrlRequest request, NSUrl fileURL, NSUrlSessionResponse completionHandler)
		{
			throw new NotImplementedException ();
		}

		public override NSUrlSessionUploadTask CreateUploadTask (NSUrlRequest request, NSData bodyData, NSUrlSessionResponse completionHandler)
		{
			throw new NotImplementedException ();
		}

		public override NSUrlSessionUploadTask CreateUploadTask (NSUrlRequest request)
		{
			throw new NotImplementedException ();
		}

		public virtual AVAssetDownloadTask GetAssetDownloadTask (AVUrlAsset urlAsset, NSUrl destinationUrl, NSDictionary options)
		{
			throw new NotImplementedException ();
		}

		public AVAssetDownloadTask GetAssetDownloadTask (AVUrlAsset urlAsset, NSUrl destinationUrl, AVAssetDownloadOptions options)
		{
			throw new NotImplementedException ();
		}

		public virtual AVAssetDownloadTask GetAssetDownloadTask (AVUrlAsset urlAsset, string title, NSData artworkData, NSDictionary options)
		{
			throw new NotImplementedException ();
		}

		public AVAssetDownloadTask GetAssetDownloadTask (AVUrlAsset urlAsset, string title, NSData artworkData, AVAssetDownloadOptions options)
		{
			throw new NotImplementedException ();
		}
	}

#endif // TVOS
#endif // !XAMCORE_4_0
}

#endif
