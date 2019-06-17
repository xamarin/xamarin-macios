//
// DeviceCheck C# bindings
//
// Authors:
//	TJ Lambert  <t-anlamb@microsoft.com>
//
// Copyright 2019 Xamarin Inc. All rights reserved.
//

using System;
using AVFoundation;
using CoreMedia;
using CoreML;
using Foundation;
using ObjCRuntime;

namespace SoundAnalysis {

    //TODO FIX
    //[ErrorDomain ("SNErrorDomain")]
    [Watch (6, 0), TV (13, 0), Mac (10, 15, onlyOn64: true), iOS (13, 0)]
    enum SNErrorCode : long {
        UnknownError = 1,
        OperationFailed,
        InvalidFormat,
        InvalidModel,    
    }

    [Watch (6, 0), TV (13, 0), Mac (10, 15, onlyOn64: true), iOS (13, 0)]
    [BaseType (typeof (NSObject))]
    [DisableDefaultCtor]
    interface SNAudioStreamAnalyzer {

        [DesignatedInitializer]
        [Export ("initWithFormat:")]
        IntPtr Constructor (AVAudioFormat format);
        
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

    [Watch (6, 0), TV (13, 0), Mac (10, 15, onlyOn64: true), iOS (13, 0)]
    [BaseType (typeof (NSObject))]
    [DisableDefaultCtor]
    interface SNAudioFileAnalyzer {

        [DesignatedInitializer]
        [Export ("initWithURL:error:")]
        IntPtr Constructor (NSUrl url, [NullAllowed] out NSError error);

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
        
        [Export ("cancelAnalyze")]
        void CancelAnalyze ();
    }

    [Watch (6, 0), TV (13, 0), Mac (10, 15, onlyOn64: true), iOS (13, 0)]
    [BaseType (typeof (NSObject))]
    [DisableDefaultCtor]
    interface SNClassification {

        [Export ("identifier")]
        string Identifier { get; }
        
        [Export ("confidence")]
        double Confidence { get; }
    }
    
    [Watch (6, 0), TV (13, 0), Mac (10, 15, onlyOn64: true), iOS (13, 0)]
    [BaseType (typeof (NSObject))]
    [DisableDefaultCtor]
    interface SNClassificationResult : SNResult {

        [Export ("classifications")]
        SNClassification [] Classifications { get; }
        
        [Export ("timeRange")]
        CMTimeRange TimeRange { get; }
    }

    [Watch (6, 0), TV (13, 0), Mac (10, 15, onlyOn64: true), iOS (13, 0)]
    [BaseType (typeof (NSObject))]
    [DisableDefaultCtor]
    interface SNClassifySoundRequest : SNRequest {

        [Export ("overlapFactor")]
        double OverlapFactor { get; set; }
        
        [return: NullAllowed]
        [Export ("initWithMLModel:error:")]
        IntPtr Constructor (MLModel mlModel, [NullAllowed] out NSError error);
    }
    
    interface ISNRequest { }

    [Watch (6, 0), TV (13, 0), Mac (10, 15, onlyOn64: true), iOS (13, 0)]
    [Protocol]
    interface SNRequest {

    }

    interface ISNResult { }
    
    [Watch (6, 0), TV (13, 0), Mac (10, 15, onlyOn64: true), iOS (13, 0)]
    [Protocol]
    interface SNResult {

    }
    
    interface ISNResultsObserving { }
    
    [Watch (6, 0), TV (13, 0), Mac (10, 15, onlyOn64: true), iOS (13, 0)]
    [Protocol]
    interface SNResultsObserving {

        [Abstract]
        [Export ("request:didProduceResult:")]
        void RequestDidProduceResult (ISNRequest request, ISNResult result);
        
        [Export ("request:didFailWithError:")]
        void RequestDidFailWithError (ISNRequest request, NSError error);
        
        [Export ("requestDidComplete:")]
        void RequestDidComplete (ISNRequest request);
    }
}
