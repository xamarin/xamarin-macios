//
// Speech bindings
//
// Authors:
//	Alex Soto  <alex.soto@xamarin.com>
//	TJ Lambert  <t-anlamb@microsoft.com>
//
// Copyright 2016 Xamarin Inc. All rights reserved.
// Copyright 2019 Microsoft Corporation All rights reserved.
//

using System;
using AVFoundation;
using CoreMedia;
using Foundation;
using ObjCRuntime;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace Speech {

	[Native]
	[MacCatalyst (13, 1)]
	public enum SFSpeechRecognitionTaskState : long {
		Starting = 0,
		Running = 1,
		Finishing = 2,
		Canceling = 3,
		Completed = 4,
	}

	[Native]
	[MacCatalyst (13, 1)]
	public enum SFSpeechRecognitionTaskHint : long {
		Unspecified = 0,
		Dictation = 1,
		Search = 2,
		Confirmation = 3,
	}

	[Native]
	[MacCatalyst (13, 1)]
	public enum SFSpeechRecognizerAuthorizationStatus : long {
		NotDetermined,
		Denied,
		Restricted,
		Authorized,
	}

	[Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[Native]
	[ErrorDomain ("SFSpeechErrorDomain")]
	public enum SFSpeechErrorCode : long {
		InternalServiceError = 1,
		UndefinedTemplateClassName = 7,
		MalformedSupplementalModel = 8,
	}

	[MacCatalyst (13, 1)]
	[DisableDefaultCtor]
	[Abstract] // no docs (yet) but it has no means (init*) to create it, unlike its subclasses
	[BaseType (typeof (NSObject))]
	interface SFSpeechRecognitionRequest {

		[Export ("taskHint", ArgumentSemantic.Assign)]
		SFSpeechRecognitionTaskHint TaskHint { get; set; }

		[Export ("shouldReportPartialResults", ArgumentSemantic.Assign)]
		bool ShouldReportPartialResults { get; set; }

		[Export ("contextualStrings", ArgumentSemantic.Copy)]
		string [] ContextualStrings { get; set; }

		[Deprecated (PlatformName.iOS, 15, 0)]
		[Deprecated (PlatformName.MacCatalyst, 15, 0)]
		[NullAllowed, Export ("interactionIdentifier")]
		string InteractionIdentifier { get; set; }

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("requiresOnDeviceRecognition")]
		bool RequiresOnDeviceRecognition { get; set; }

		[Mac (13, 0), iOS (16, 0), MacCatalyst (16, 0)]
		[Export ("addsPunctuation")]
		bool AddsPunctuation { get; set; }

		[Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
		[NullAllowed, Export ("customizedLanguageModel", ArgumentSemantic.Copy)]
		SFSpeechLanguageModelConfiguration CustomizedLanguageModel { get; set; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (SFSpeechRecognitionRequest), Name = "SFSpeechURLRecognitionRequest")]
	[DisableDefaultCtor]
	interface SFSpeechUrlRecognitionRequest {

		[Export ("initWithURL:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSUrl url);

		[Export ("URL", ArgumentSemantic.Copy)]
		NSUrl Url { get; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (SFSpeechRecognitionRequest))]
	interface SFSpeechAudioBufferRecognitionRequest {

		[Export ("nativeAudioFormat")]
		AVAudioFormat NativeAudioFormat { get; }

		[Export ("appendAudioPCMBuffer:")]
		void Append (AVAudioPcmBuffer audioPcmBuffer);

		[Export ("appendAudioSampleBuffer:")]
		void Append (CMSampleBuffer sampleBuffer);

		[Export ("endAudio")]
		void EndAudio ();
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface SFSpeechRecognitionResult : NSCopying, NSSecureCoding {

		[Export ("bestTranscription", ArgumentSemantic.Copy)]
		SFTranscription BestTranscription { get; }

		[Export ("transcriptions", ArgumentSemantic.Copy)]
		SFTranscription [] Transcriptions { get; }

		[Export ("final")]
		bool Final { [Bind ("isFinal")] get; }

		[iOS (14, 5), Mac (11, 3)]
		[MacCatalyst (14, 5)]
		[NullAllowed, Export ("speechRecognitionMetadata")]
		SFSpeechRecognitionMetadata SpeechRecognitionMetadata { get; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface SFSpeechRecognitionTask {

		[Export ("state")]
		SFSpeechRecognitionTaskState State { get; }

		[Export ("finishing")]
		bool Finishing { [Bind ("isFinishing")] get; }

		[Export ("finish")]
		void Finish ();

		[Export ("cancelled")]
		bool Cancelled { [Bind ("isCancelled")] get; }

		[Export ("cancel")]
		void Cancel ();

		[NullAllowed, Export ("error", ArgumentSemantic.Copy)]
		NSError Error { get; }
	}

	interface ISFSpeechRecognitionTaskDelegate { }

	[MacCatalyst (13, 1)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface SFSpeechRecognitionTaskDelegate {

		[Export ("speechRecognitionDidDetectSpeech:")]
		void DidDetectSpeech (SFSpeechRecognitionTask task);

		[Export ("speechRecognitionTask:didHypothesizeTranscription:")]
		void DidHypothesizeTranscription (SFSpeechRecognitionTask task, SFTranscription transcription);

		[Export ("speechRecognitionTask:didFinishRecognition:")]
		void DidFinishRecognition (SFSpeechRecognitionTask task, SFSpeechRecognitionResult recognitionResult);

		[Export ("speechRecognitionTaskFinishedReadingAudio:")]
		void FinishedReadingAudio (SFSpeechRecognitionTask task);

		[Export ("speechRecognitionTaskWasCancelled:")]
		void WasCancelled (SFSpeechRecognitionTask task);

		[Export ("speechRecognitionTask:didFinishSuccessfully:")]
		void DidFinishSuccessfully (SFSpeechRecognitionTask task, bool successfully);
	}

	interface ISFSpeechRecognizerDelegate { }

	[MacCatalyst (13, 1)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface SFSpeechRecognizerDelegate {

		[Export ("speechRecognizer:availabilityDidChange:")]
		void AvailabilityDidChange (SFSpeechRecognizer speechRecognizer, bool available);
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface SFSpeechRecognizer {

		[Static]
		[Export ("supportedLocales")]
		NSSet<NSLocale> SupportedLocales { get; }

		[Static]
		[Export ("authorizationStatus")]
		SFSpeechRecognizerAuthorizationStatus AuthorizationStatus { get; }

		[Static]
		[Export ("requestAuthorization:")]
		void RequestAuthorization (Action<SFSpeechRecognizerAuthorizationStatus> handler);

		[Export ("initWithLocale:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSLocale locale);

		[Export ("available")]
		bool Available { [Bind ("isAvailable")] get; }

		[Export ("locale", ArgumentSemantic.Copy)]
		NSLocale Locale { get; }

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("supportsOnDeviceRecognition")]
		bool SupportsOnDeviceRecognition { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		ISFSpeechRecognizerDelegate Delegate { get; set; }

		[Export ("defaultTaskHint", ArgumentSemantic.Assign)]
		SFSpeechRecognitionTaskHint DefaultTaskHint { get; set; }

		// no [Async] as this gets called multiple times, leading to an exception
		[Export ("recognitionTaskWithRequest:resultHandler:")]
		SFSpeechRecognitionTask GetRecognitionTask (SFSpeechRecognitionRequest request, Action<SFSpeechRecognitionResult, NSError> resultHandler);

		[Export ("recognitionTaskWithRequest:delegate:")]
		SFSpeechRecognitionTask GetRecognitionTask (SFSpeechRecognitionRequest request, ISFSpeechRecognitionTaskDelegate @delegate);

		[Export ("queue", ArgumentSemantic.Strong)]
		NSOperationQueue Queue { get; set; }
	}

	[iOS (14, 5), Mac (11, 3)]
	[MacCatalyst (14, 5)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SFSpeechRecognitionMetadata : NSCopying, NSSecureCoding {

		[Export ("speakingRate")]
		double SpeakingRate { get; }

		[Export ("averagePauseDuration")]
		double AveragePauseDuration { get; }

		[Export ("speechStartTimestamp")]
		double SpeechStartTimestamp { get; }

		[Export ("speechDuration")]
		double SpeechDuration { get; }

		[NullAllowed, Export ("voiceAnalytics")]
		SFVoiceAnalytics VoiceAnalytics { get; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface SFTranscription : NSCopying, NSSecureCoding {

		[Export ("formattedString")]
		string FormattedString { get; }

		[Export ("segments", ArgumentSemantic.Copy)]
		SFTranscriptionSegment [] Segments { get; }

		[iOS (13, 0)]
		[Export ("speakingRate")]
		[Deprecated (PlatformName.iOS, 14, 5)]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 14, 5)]
		[Advice ("Use 'SpeakingRate' from 'SFSpeechRecognitionMetadata' instead.")]
		double SpeakingRate { get; }

		[iOS (13, 0)]
		[Export ("averagePauseDuration")]
		[Deprecated (PlatformName.iOS, 14, 5)]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 14, 5)]
		[Advice ("Use 'AveragePauseDuration' from 'SFSpeechRecognitionMetadata' instead.")]
		double AveragePauseDuration { get; }
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface SFTranscriptionSegment : NSCopying, NSSecureCoding {

		[Export ("substring")]
		string Substring { get; }

		[Export ("substringRange")]
		NSRange SubstringRange { get; }

		[Export ("timestamp")]
		double Timestamp { get; }

		[Export ("duration")]
		double Duration { get; }

		[Export ("confidence")]
		float Confidence { get; }

		[Export ("alternativeSubstrings")]
		string [] AlternativeSubstrings { get; }

		[iOS (13, 0)]
		[NullAllowed, Export ("voiceAnalytics")]
		[Deprecated (PlatformName.iOS, 14, 5)]
		[MacCatalyst (13, 1)]
		[Deprecated (PlatformName.MacCatalyst, 14, 5)]
		[Advice ("Use 'VoiceAnalytics' from 'SFSpeechRecognitionMetadata' instead.")]
		SFVoiceAnalytics VoiceAnalytics { get; }
	}

	[iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SFAcousticFeature : NSCopying, NSSecureCoding {

		[Export ("acousticFeatureValuePerFrame", ArgumentSemantic.Copy)]
		NSNumber [] AcousticFeatureValuePerFrame { get; }

		[Export ("frameDuration")]
		double FrameDuration { get; }
	}

	[iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SFVoiceAnalytics : NSCopying, NSSecureCoding {

		[Export ("jitter", ArgumentSemantic.Copy)]
		SFAcousticFeature Jitter { get; }

		[Export ("shimmer", ArgumentSemantic.Copy)]
		SFAcousticFeature Shimmer { get; }

		[Export ("pitch", ArgumentSemantic.Copy)]
		SFAcousticFeature Pitch { get; }

		[Export ("voicing", ArgumentSemantic.Copy)]
		SFAcousticFeature Voicing { get; }
	}

	[Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	interface SFSpeechLanguageModelConfiguration : NSCopying {
		[Export ("initWithLanguageModel:")]
		NativeHandle Constructor (NSUrl languageModel);

		[Export ("initWithLanguageModel:vocabulary:")]
		NativeHandle Constructor (NSUrl languageModel, [NullAllowed] NSUrl vocabulary);

		[Export ("languageModel", ArgumentSemantic.Copy)]
		NSUrl LanguageModel { get; }

		[NullAllowed, Export ("vocabulary", ArgumentSemantic.Copy)]
		NSUrl Vocabulary { get; }
	}

	[Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SFSpeechLanguageModel {
		[Static]
		[Export ("prepareCustomLanguageModelForUrl:clientIdentifier:configuration:completion:")]
		[Async]
		void PrepareCustomModel (NSUrl asset, string clientIdentifier, SFSpeechLanguageModelConfiguration configuration, Action<NSError> completion);

		[Static]
		[Export ("prepareCustomLanguageModelForUrl:clientIdentifier:configuration:ignoresCache:completion:")]
		[Async]
		void PrepareCustomModel (NSUrl asset, string clientIdentifier, SFSpeechLanguageModelConfiguration configuration, bool ignoresCache, Action<NSError> completion);
	}

	[Partial]
	[Mac (14, 0), iOS (17, 0), MacCatalyst (17, 0)]
	interface SFAnalysisContextTag {
		[Field ("SFAnalysisContextTagLeftContext")]
		NSString LeftContext { get; }

		[Field ("SFAnalysisContextTagRightContext")]
		NSString RightContext { get; }

		[Field ("SFAnalysisContextTagSelectedText")]
		NSString SelectedText { get; }
	}
}
