//
// CoreNFC C# bindings
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2017 Xamarin Inc. All rights reserved.
//

#if XAMCORE_2_0

using System;
using ObjCRuntime;
using CoreFoundation;
using Foundation;

namespace CoreNFC {
	// Note on Availability:
	// This framework says it is available on tvOS but
	// nothing else but NFCReaderError is available on it in beta 1
	// keeping the original Availability attributes commented just in case
	// this changes in a future beta and will just be enabled in iOS for now.

	//[iOS (11, 0), TV (11, 0), NoWatch, NoMac]
	[iOS (11,0)]
	[ErrorDomain ("NFCErrorDomain")]
	[Native]
	public enum NFCReaderError : long {
		UnsupportedFeature = 1,
		SecurityViolation,
		ReaderTransceiveErrorTagConnectionLost = 100,
		ReaderTransceiveErrorRetryExceeded,
		ReaderTransceiveErrorTagResponseError,
		ReaderSessionInvalidationErrorUserCanceled = 200,
		ReaderSessionInvalidationErrorSessionTimeout,
		ReaderSessionInvalidationErrorSessionTerminatedUnexpectedly,
		ReaderSessionInvalidationErrorSystemIsBusy,
		ReaderSessionInvalidationErrorFirstNDEFTagRead,
		TagCommandConfigurationErrorInvalidParameters = 300,
	}

	//[iOS (11,0), NoTV, NoWatch, NoMac]
	[iOS (11,0)]
	[Native]
	public enum NFCTagType : ulong {
		Iso15693 = 1,
	}

	//[iOS (11,0), NoTV, NoWatch, NoMac]
	[iOS (11,0)]
	public enum NFCTypeNameFormat : byte { // uint8_t
		Empty = 0x00,
		NFCWellKnown = 0x01,
		Media = 0x02,
		AbsoluteUri = 0x03,
		NFCExternal = 0x04,
		Unknown = 0x05,
		Unchanged = 0x06,
	}

	//[iOS (11,0), NoTV, NoWatch, NoMac]
	[iOS (11,0)]
	[BaseType (typeof (NFCReaderSession), Name = "NFCISO15693ReaderSession")]
	[DisableDefaultCtor]
	interface NFCIso15693ReaderSession {

		[Field ("NFCISO15693TagResponseErrorKey")]
		NSString TagResponseErrorKey { get; }

		[Export ("initWithDelegate:queue:")]
		[DesignatedInitializer]
		IntPtr Constructor (INFCReaderSessionDelegate @delegate, [NullAllowed] DispatchQueue queue);

		[Static]
		[Export ("readingAvailable")]
		bool ReadingAvailable { get; }

		[Export ("restartPolling")]
		void RestartPolling ();
	}

	//[iOS (11,0), NoTV, NoWatch, NoMac]
	[iOS (11,0)]
	[BaseType (typeof (NFCTagCommandConfiguration), Name = "NFCISO15693CustomCommandConfiguration")]
	interface NFCIso15693CustomCommandConfiguration {

		[Export ("manufacturerCode")]
		nuint ManufacturerCode { get; set; }

		[Export ("customCommandCode")]
		nuint CustomCommandCode { get; set; }

		[Export ("requestParameters", ArgumentSemantic.Copy)]
		NSData RequestParameters { get; set; }

		[Export ("initWithManufacturerCode:customCommandCode:requestParameters:")]
		IntPtr Constructor (nuint manufacturerCode, nuint customCommandCode, [NullAllowed] NSData requestParameters);

		[Export ("initWithManufacturerCode:customCommandCode:requestParameters:maximumRetries:retryInterval:")]
		IntPtr Constructor (nuint manufacturerCode, nuint customCommandCode, [NullAllowed] NSData requestParameters, nuint maximumRetries, double retryInterval);
	}

	//[iOS (11,0), NoTV, NoWatch, NoMac]
	[iOS (11,0)]
	[BaseType (typeof (NFCTagCommandConfiguration), Name = "NFCISO15693ReadMultipleBlocksConfiguration")]
	interface NFCIso15693ReadMultipleBlocksConfiguration {

		[Export ("range", ArgumentSemantic.Assign)]
		NSRange Range { get; set; }

		[Export ("chunkSize")]
		nuint ChunkSize { get; set; }

		[Export ("initWithRange:chunkSize:")]
		IntPtr Constructor (NSRange range, nuint chunkSize);

		[Export ("initWithRange:chunkSize:maximumRetries:retryInterval:")]
		IntPtr Constructor (NSRange range, nuint chunkSize, nuint maximumRetries, double retryInterval);
	}

	interface INFCIso15693Tag { }

	//[iOS (11,0), NoTV, NoWatch, NoMac]
	[iOS (11,0)]
	[Protocol (Name = "NFCISO15693Tag")]
	interface NFCIso15693Tag : NFCTag {

		[Abstract]
		[Export ("identifier", ArgumentSemantic.Copy)]
		NSData Identifier { get; }

		[Abstract]
		[Export ("icManufacturerCode")]
		nuint IcManufacturerCode { get; }

		[Abstract]
		[Export ("icSerialNumber", ArgumentSemantic.Copy)]
		NSData IcSerialNumber { get; }

		[Abstract]
		[Export ("sendCustomCommandWithConfiguration:completionHandler:")]
		void SendCustomCommand (NFCIso15693CustomCommandConfiguration commandConfiguration, Action<NSData, NSError> completionHandler);

		[Abstract]
		[Export ("readMultipleBlocksWithConfiguration:completionHandler:")]
		void ReadMultipleBlocks (NFCIso15693ReadMultipleBlocksConfiguration readConfiguration, Action<NSData, NSError> completionHandler);
	}

	//[iOS (11,0), NoTV, NoWatch, NoMac]
	[iOS (11,0)]
	[BaseType (typeof (NSObject), Name = "NFCNDEFPayload")]
	[DisableDefaultCtor]
	interface NFCNdefPayload : NSSecureCoding {

		[Export ("typeNameFormat", ArgumentSemantic.Assign)]
		NFCTypeNameFormat TypeNameFormat { get; set; }

		[Export ("type", ArgumentSemantic.Copy)]
		NSData Type { get; set; }

		[Export ("identifier", ArgumentSemantic.Copy)]
		NSData Identifier { get; set; }

		[Export ("payload", ArgumentSemantic.Copy)]
		NSData Payload { get; set; }
	}

	//[iOS (11,0), NoTV, NoWatch, NoMac]
	[iOS (11,0)]
	[BaseType (typeof (NSObject), Name = "NFCNDEFMessage")]
	[DisableDefaultCtor]
	interface NFCNdefMessage : NSSecureCoding {

		[Export ("records", ArgumentSemantic.Copy)]
		NFCNdefPayload [] Records { get; set; }
	}

	interface INFCNdefReaderSessionDelegate { }

	//[iOS (11,0), NoTV, NoWatch, NoMac]
	[iOS (11,0)]
	[Protocol, Model]
	[BaseType (typeof (NSObject), Name = "NFCNDEFReaderSessionDelegate")]
	interface NFCNdefReaderSessionDelegate {

		[Abstract]
		[Export ("readerSession:didInvalidateWithError:")]
		void DidInvalidate (NFCNdefReaderSession session, NSError error);

		[Abstract]
		[Export ("readerSession:didDetectNDEFs:")]
		void DidDetect (NFCNdefReaderSession session, NFCNdefMessage [] messages);
	}

	//[iOS (11,0), NoTV, NoWatch, NoMac]
	[iOS (11,0)]
	[BaseType (typeof (NFCReaderSession), Name = "NFCNDEFReaderSession")]
	[DisableDefaultCtor]
	interface NFCNdefReaderSession {

		[Export ("initWithDelegate:queue:invalidateAfterFirstRead:")]
		[DesignatedInitializer]
		IntPtr Constructor (INFCNdefReaderSessionDelegate @delegate, [NullAllowed] DispatchQueue queue, bool invalidateAfterFirstRead);

		[Static]
		[Export ("readingAvailable")]
		bool ReadingAvailable { get; }
	}

	//[iOS (11,0), NoTV, NoWatch, NoMac]
	[iOS (11,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NFCReaderSession : NFCReaderSessionContract {

		[Export ("delegate", ArgumentSemantic.Weak), NullAllowed]
		INFCReaderSessionDelegate Delegate { get; }

		[Export ("sessionQueue")]
		DispatchQueue SessionQueue { get; }
	}

	interface INFCReaderSessionContract { }

	//[iOS (11,0), NoTV, NoWatch, NoMac]
	[iOS (11,0)]
	[Protocol (Name = "NFCReaderSession")]
	interface NFCReaderSessionContract {

		[Abstract]
		[Export ("ready")]
		bool Ready { [Bind ("isReady")] get; }

		[Abstract]
		[Export ("alertMessage")]
		string AlertMessage { get; set; }

		[Abstract]
		[Export ("beginSession")]
		void BeginSession ();

		[Abstract]
		[Export ("invalidateSession")]
		void InvalidateSession ();
	}

	interface INFCReaderSessionDelegate { }

	//[iOS (11,0), NoTV, NoWatch, NoMac]
	[iOS (11,0)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface NFCReaderSessionDelegate {

		[Abstract]
		[Export ("readerSessionDidBecomeActive:")]
		void DidBecomeActive (NFCReaderSession session);

		[Abstract]
		[Export ("readerSession:didDetectTags:")]
		void DidDetectTags (NFCReaderSession session, INFCTag [] tags);

		[Abstract]
		[Export ("readerSession:didInvalidateWithError:")]
		void DidInvalidate (NFCReaderSession session, NSError error);
	}

	interface INFCTag { }

	//[iOS (11,0), NoTV, NoWatch, NoMac]
	[iOS (11,0)]
	[Protocol]
	interface NFCTag : NSSecureCoding, NSCopying {

		[Abstract]
		[Export ("type", ArgumentSemantic.Assign)]
		NFCTagType Type { get; }

		[Abstract]
		[NullAllowed, Export ("session", ArgumentSemantic.Weak)]
		NFCReaderSession Session { get; }

		[Abstract]
		[Export ("available")]
		bool Available { [Bind ("isAvailable")] get; }
	}

	//[iOS (11,0), NoTV, NoWatch, NoMac]
	[iOS (11,0)]
	[BaseType (typeof (NSObject))]
	interface NFCTagCommandConfiguration : NSCopying {

		[Export ("maximumRetries")]
		nuint MaximumRetries { get; set; }

		[Export ("retryInterval")]
		double RetryInterval { get; set; }
	}

	[iOS (12,0)]
	[Category]
	[BaseType (typeof (NSUserActivity))]
	interface NSUserActivity_CoreNFC {

		[Export ("ndefMessagePayload")]
		NFCNdefMessage GetNdefMessagePayload ();
	}
}
#endif
