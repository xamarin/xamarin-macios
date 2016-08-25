// Copyright 2014-2015 Xamarin Inc. All rights reserved.

#if !WATCH

using System;
using XamCore.CoreMedia;
using XamCore.Foundation;

namespace XamCore.AVFoundation {

#if !XAMCORE_2_0
	partial class AVAssetResourceLoadingDataRequest {

		[Obsolete ("Type is not meant to be created by user code")]
		public AVAssetResourceLoadingDataRequest ()
		{
		}
	}
#endif

#if !XAMCORE_3_0
	partial class AVAsset {

		[Obsolete ("Use GetChapterMetadataGroups")]
		public virtual AVMetadataItem[] ChapterMetadataGroups (NSLocale forLocale, AVMetadataItem[] commonKeys)
		{
			return null;
		}
	}

	partial class AVAssetTrack {

		[Obsolete ("Use GetAssociatedTracks")]
		public virtual NSString GetAssociatedTracksOfType (NSString avAssetTrackTrackAssociationType)
		{
			return null;
		}
	}

	partial class AVMutableCompositionTrack {

		[Obsolete ("Use InsertTimeRanges overload accepting an NSValue array")]
		public virtual bool InsertTimeRanges (NSValue cmTimeRanges, AVAssetTrack[] tracks, CMTime startTime, out NSError error)
		{
			return InsertTimeRanges (new NSValue [] { cmTimeRanges }, tracks, startTime, out error);
		}
	}


	partial class AVCaptureAudioDataOutputSampleBufferDelegate {

		[Obsolete ("This member only exists for AVCaptureVideoDataOutputSampleBufferDelegate")]
		public virtual void DidDropSampleBuffer (AVCaptureOutput captureOutput, CMSampleBuffer sampleBuffer, AVCaptureConnection connection)
		{
		}
	}

	static partial class AVCaptureAudioDataOutputSampleBufferDelegate_Extensions {

		[Obsolete ("This member only exists for AVCaptureVideoDataOutputSampleBufferDelegate")]
		public static void DidDropSampleBuffer (IAVCaptureAudioDataOutputSampleBufferDelegate This, AVCaptureOutput captureOutput, CMSampleBuffer sampleBuffer, AVCaptureConnection connection)
		{
		}
	}
#endif

#if !XAMCORE_4_0
	partial class AVAudioChannelLayout {

		[Obsolete ("Valid instance of this type cannot be directly created")]
		public AVAudioChannelLayout ()
		{
		}
	}

	partial class AVAudioConnectionPoint {

		[Obsolete ("Valid instance of this type cannot be directly created")]
		public AVAudioConnectionPoint ()
		{
		}
	}
#endif
}

#endif
