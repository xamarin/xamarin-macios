//
// SoundAnalysis C# bindings
//
// Authors:
//	TJ Lambert  <t-anlamb@microsoft.com>
//
// Copyright 2019 Microsoft Corporation All rights reserved.
//

using System;
using AVFoundation;
using CoreML;
using Foundation;
using ObjCRuntime;
using CoreMedia;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace SoundAnalysis {

	[ErrorDomain ("SNErrorDomain")]
	[Watch (6, 0), TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	enum SNErrorCode : long {
		UnknownError = 1,
		OperationFailed,
		InvalidFormat,
		InvalidModel,
		InvalidFile,
	}

	[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[Native]
	public enum SNTimeDurationConstraintType : long {
		Enumerated = 1,
		Range = 2,
	}

	[Watch (6, 0), TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SNAudioStreamAnalyzer {

		[DesignatedInitializer]
		[Export ("initWithFormat:")]
		NativeHandle Constructor (AVAudioFormat format);

		[Export ("addRequest:withObserver:error:")]
		bool AddRequest (ISNRequest request, ISNResultsObserving observer, [NullAllowed] out NSError error);

		[Export ("removeRequest:")]
		void RemoveRequest (ISNRequest request);

		[Export ("removeAllRequests")]
		void RemoveAllRequests ();

		[Export ("analyzeAudioBuffer:atAudioFramePosition:")]
		void Analyze (AVAudioBuffer audioBuffer, long audioFramePosition);

		[Export ("completeAnalysis")]
		void CompleteAnalysis ();
	}

	delegate void SNAudioFileAnalyzerAnalyzeHandler (bool didReachEndOfFile);

	[Watch (6, 0), TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SNAudioFileAnalyzer {

		[DesignatedInitializer]
		[Export ("initWithURL:error:")]
		NativeHandle Constructor (NSUrl url, [NullAllowed] out NSError error);

		[Export ("addRequest:withObserver:error:")]
		bool AddRequest (ISNRequest request, ISNResultsObserving observer, [NullAllowed] out NSError error);

		[Export ("removeRequest:")]
		void RemoveRequest (ISNRequest request);

		[Export ("removeAllRequests")]
		void RemoveAllRequests ();

		[Export ("analyze")]
		void Analyze ();

		[Async]
		[Export ("analyzeWithCompletionHandler:")]
		void Analyze (SNAudioFileAnalyzerAnalyzeHandler completionHandler);

		[Export ("cancelAnalysis")]
		void CancelAnalysis ();
	}

	[Watch (6, 0), TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SNClassification {

		[Export ("identifier")]
		string Identifier { get; }

		[Export ("confidence")]
		double Confidence { get; }
	}

	[Watch (6, 0), TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SNClassificationResult : SNResult {

		[Export ("classifications")]
		SNClassification [] Classifications { get; }

		[Export ("timeRange")]
		CMTimeRange TimeRange { get; }

		[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("classificationForIdentifier:")]
		[return: NullAllowed]
		SNClassification GetClassification (string identifier);
	}

	[Watch (6, 0), TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SNClassifySoundRequest : SNRequest {

		[Export ("overlapFactor")]
		double OverlapFactor { get; set; }

		[Export ("initWithMLModel:error:")]
		NativeHandle Constructor (MLModel mlModel, [NullAllowed] out NSError error);

		[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("initWithClassifierIdentifier:error:")]
		NativeHandle Constructor (string classifierIdentifier, [NullAllowed] out NSError error);

		[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("knownClassifications", ArgumentSemantic.Copy)]
		string [] KnownClassifications { get; }

		[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("windowDuration", ArgumentSemantic.Assign)]
		CMTime WindowDuration { get; set; }

		[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
		[Export ("windowDurationConstraint", ArgumentSemantic.Strong)]
		SNTimeDurationConstraint WindowDurationConstraint { get; }
	}

	interface ISNRequest { }

	[Watch (6, 0), TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface SNRequest { }

	interface ISNResult { }

	[Watch (6, 0), TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface SNResult { }

	interface ISNResultsObserving { }

	[Watch (6, 0), TV (13, 0), iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface SNResultsObserving {

		[Abstract]
		[Export ("request:didProduceResult:")]
		void DidProduceResult (ISNRequest request, ISNResult result);

		[Export ("request:didFailWithError:")]
		void DidFail (ISNRequest request, NSError error);

		[Export ("requestDidComplete:")]
		void DidComplete (ISNRequest request);
	}

	[iOS (15, 0), Mac (12, 0), Watch (8, 0), TV (15, 0), MacCatalyst (15, 0)]
	enum SNClassifierIdentifier {
		[Field ("SNClassifierIdentifierVersion1")]
		Version1,
	}

	[Watch (8, 0), TV (15, 0), Mac (12, 0), iOS (15, 0), MacCatalyst (15, 0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface SNTimeDurationConstraint /* privately conforms to NSCoding, NSCopying, and NSSecureCoding */
	{
		[Export ("initWithEnumeratedDurations:")]
		NativeHandle Constructor ([BindAs (typeof (CMTime []))] NSValue [] enumeratedDurations);

		[Export ("initWithDurationRange:")]
		NativeHandle Constructor (CMTimeRange durationRange);

		[Export ("type", ArgumentSemantic.Assign)]
		SNTimeDurationConstraintType Type { get; }

#if NET
		[BindAs (typeof (CMTime[]))]
#endif
		[Export ("enumeratedDurations", ArgumentSemantic.Strong)]
		NSValue [] EnumeratedDurations { get; }

		[Export ("durationRange", ArgumentSemantic.Assign)]
		CMTimeRange DurationRange { get; }
	}
}
