//
// CoreNFC C# bindings
//
// Authors:
//	Alex Soto  <alexsoto@microsoft.com>
//
// Copyright 2017 Xamarin Inc. All rights reserved.
//

using System;
using ObjCRuntime;
using CoreFoundation;
using Foundation;

#if !NET
using NFCFeliCaEncryptionId = CoreNFC.EncryptionId;
using NFCFeliCaPollingRequestCode = CoreNFC.PollingRequestCode;
using NFCFeliCaPollingTimeSlot = CoreNFC.PollingTimeSlot;
using NFCIso15693RequestFlag = CoreNFC.RequestFlag;
using NFCVasErrorCode = CoreNFC.VasErrorCode;
using NFCVasMode = CoreNFC.VasMode;

using NativeHandle = System.IntPtr;
#endif

namespace CoreNFC {

	[MacCatalyst (13, 1)]
	[ErrorDomain ("NFCErrorDomain")]
	[Native]
	public enum NFCReaderError : long {
		UnsupportedFeature = 1,
		SecurityViolation,
		InvalidParameter,
		InvalidParameterLength,
		ParameterOutOfBound,
		RadioDisabled = 6,

		ReaderTransceiveErrorTagConnectionLost = 100,
		ReaderTransceiveErrorRetryExceeded,
		ReaderTransceiveErrorTagResponseError,
		ReaderTransceiveErrorSessionInvalidated,
		ReaderTransceiveErrorTagNotConnected,
		ReaderTransceiveErrorPacketTooLong = 105,

		ReaderSessionInvalidationErrorUserCanceled = 200,
		ReaderSessionInvalidationErrorSessionTimeout,
		ReaderSessionInvalidationErrorSessionTerminatedUnexpectedly,
		ReaderSessionInvalidationErrorSystemIsBusy,
		ReaderSessionInvalidationErrorFirstNDEFTagRead,

		TagCommandConfigurationErrorInvalidParameters = 300,

		NdefReaderSessionErrorTagNotWritable = 400,
		NdefReaderSessionErrorTagUpdateFailure = 401,
		NdefReaderSessionErrorTagSizeTooSmall = 402,
		NdefReaderSessionErrorZeroLengthMessage = 403,
	}

	//[NoTV, NoWatch, NoMac]
	[MacCatalyst (13, 1)]
	[Native]
	public enum NFCTagType : ulong {
		Iso15693 = 1,
		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		FeliCa = 2,
		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		Iso7816Compatible = 3,
		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		MiFare = 4,
	}

	//[NoTV, NoWatch, NoMac]
	[MacCatalyst (13, 1)]
	public enum NFCTypeNameFormat : byte { // uint8_t
		Empty = 0x00,
		NFCWellKnown = 0x01,
		Media = 0x02,
		AbsoluteUri = 0x03,
		NFCExternal = 0x04,
		Unknown = 0x05,
		Unchanged = 0x06,
	}

	//[NoTV, NoWatch, NoMac]
	[Deprecated (PlatformName.iOS, 17, 0)]
	[Deprecated (PlatformName.MacCatalyst, 17, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NFCReaderSession), Name = "NFCISO15693ReaderSession")]
	[DisableDefaultCtor]
	interface NFCIso15693ReaderSession {

		[Field ("NFCISO15693TagResponseErrorKey")]
		NSString TagResponseErrorKey { get; }

		[Export ("initWithDelegate:queue:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INFCReaderSessionDelegate @delegate, [NullAllowed] DispatchQueue queue);

		[Static]
		[Export ("readingAvailable")]
		bool ReadingAvailable { get; }

		[Export ("restartPolling")]
		void RestartPolling ();
	}

	//[NoTV, NoWatch, NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NFCTagCommandConfiguration), Name = "NFCISO15693CustomCommandConfiguration")]
	interface NFCIso15693CustomCommandConfiguration {

		[Export ("manufacturerCode")]
		nuint ManufacturerCode { get; set; }

		[Export ("customCommandCode")]
		nuint CustomCommandCode { get; set; }

		[Export ("requestParameters", ArgumentSemantic.Copy)]
		NSData RequestParameters { get; set; }

		[Export ("initWithManufacturerCode:customCommandCode:requestParameters:")]
		NativeHandle Constructor (nuint manufacturerCode, nuint customCommandCode, [NullAllowed] NSData requestParameters);

		[Export ("initWithManufacturerCode:customCommandCode:requestParameters:maximumRetries:retryInterval:")]
		NativeHandle Constructor (nuint manufacturerCode, nuint customCommandCode, [NullAllowed] NSData requestParameters, nuint maximumRetries, double retryInterval);
	}

	//[NoTV, NoWatch, NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NFCTagCommandConfiguration), Name = "NFCISO15693ReadMultipleBlocksConfiguration")]
	interface NFCIso15693ReadMultipleBlocksConfiguration {

		[Export ("range", ArgumentSemantic.Assign)]
		NSRange Range { get; set; }

		[Export ("chunkSize")]
		nuint ChunkSize { get; set; }

		[Export ("initWithRange:chunkSize:")]
		NativeHandle Constructor (NSRange range, nuint chunkSize);

		[Export ("initWithRange:chunkSize:maximumRetries:retryInterval:")]
		NativeHandle Constructor (NSRange range, nuint chunkSize, nuint maximumRetries, double retryInterval);
	}

	delegate void NFCGetSystemInfoCompletionHandler (nint dsfid, nint afi, nint blockSize, nint blockCount, nint icReference, NSError error);

	interface INFCIso15693Tag { }

	delegate void NFCIso15693TagReadMultipleBlocksCallback (NSData [] dataBlocks, NSError error);
	delegate void NFCIso15693TagResponseCallback (NFCIso15693ResponseFlag responseFlag, NSData response, NSError error);
	delegate void NFCIso15693TagGetMultipleBlockSecurityStatusCallback (NSNumber [] securityStatus, NSError error);
	delegate void NFCIso15693TagGetSystemInfoAndUidCallback (NSData uid, nint dsfid, nint afi, nint blockSize, nint blockCount, nint icReference, NSError error);

	//[NoTV, NoWatch, NoMac]
	[MacCatalyst (13, 1)]
	[Protocol (Name = "NFCISO15693Tag")]
	interface NFCIso15693Tag : NFCTag, NFCNdefTag {

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

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("stayQuietWithCompletionHandler:")]
		void StayQuiet (Action<NSError> completionHandler);

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("readSingleBlockWithRequestFlags:blockNumber:completionHandler:")]
		void ReadSingleBlock (NFCIso15693RequestFlag flags, byte blockNumber, Action<NSData, NSError> completionHandler);

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("writeSingleBlockWithRequestFlags:blockNumber:dataBlock:completionHandler:")]
		void WriteSingleBlock (NFCIso15693RequestFlag flags, byte blockNumber, NSData dataBlock, Action<NSError> completionHandler);

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("lockBlockWithRequestFlags:blockNumber:completionHandler:")]
		void LockBlock (NFCIso15693RequestFlag flags, byte blockNumber, Action<NSError> completionHandler);

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("readMultipleBlocksWithRequestFlags:blockRange:completionHandler:")]
		void ReadMultipleBlocks (NFCIso15693RequestFlag flags, NSRange blockRange, Action<NSData [], NSError> completionHandler);

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("writeMultipleBlocksWithRequestFlags:blockRange:dataBlocks:completionHandler:")]
		void WriteMultipleBlocks (NFCIso15693RequestFlag flags, NSRange blockRange, NSData [] dataBlocks, Action<NSError> completionHandler);

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("selectWithRequestFlags:completionHandler:")]
		void Select (NFCIso15693RequestFlag flags, Action<NSError> completionHandler);

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("resetToReadyWithRequestFlags:completionHandler:")]
		void ResetToReady (NFCIso15693RequestFlag flags, Action<NSError> completionHandler);

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("writeAFIWithRequestFlag:afi:completionHandler:")]
		void WriteAfi (NFCIso15693RequestFlag flags, byte afi, Action<NSError> completionHandler);

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("lockAFIWithRequestFlag:completionHandler:")]
		void LockAfi (NFCIso15693RequestFlag flags, Action<NSError> completionHandler);

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("writeDSFIDWithRequestFlag:dsfid:completionHandler:")]
		void WriteDsfi (NFCIso15693RequestFlag flags, byte dsfid, Action<NSError> completionHandler);

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("lockDFSIDWithRequestFlag:completionHandler:")]
		void LockDfsi (NFCIso15693RequestFlag flags, Action<NSError> completionHandler);

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("getSystemInfoWithRequestFlag:completionHandler:")]
		void GetSystemInfo (NFCIso15693RequestFlag flags, NFCGetSystemInfoCompletionHandler completionHandler);

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("getMultipleBlockSecurityStatusWithRequestFlag:blockRange:completionHandler:")]
		void GetMultipleBlockSecurityStatus (NFCIso15693RequestFlag flags, NSRange blockRange, Action<NSNumber [], NSError> completionHandler);

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("customCommandWithRequestFlag:customCommandCode:customRequestParameters:completionHandler:")]
		void CustomCommand (NFCIso15693RequestFlag flags, nint customCommandCode, NSData customRequestParameters, Action<NSData, NSError> completionHandler);

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("extendedReadSingleBlockWithRequestFlags:blockNumber:completionHandler:")]
		void ExtendedReadSingleBlock (NFCIso15693RequestFlag flags, nint blockNumber, Action<NSData, NSError> completionHandler);

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("extendedWriteSingleBlockWithRequestFlags:blockNumber:dataBlock:completionHandler:")]
		void ExtendedWriteSingleBlock (NFCIso15693RequestFlag flags, nint blockNumber, NSData dataBlock, Action<NSError> completionHandler);

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("extendedLockBlockWithRequestFlags:blockNumber:completionHandler:")]
		void ExtendedLockBlock (NFCIso15693RequestFlag flags, nint blockNumber, Action<NSError> completionHandler);

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("extendedReadMultipleBlocksWithRequestFlags:blockRange:completionHandler:")]
		void ExtendedReadMultipleBlocks (NFCIso15693RequestFlag flags, NSRange blockRange, Action<NSData [], NSError> completionHandler);

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
#if NET
		[Abstract]
#endif
		[Export ("extendedWriteMultipleBlocksWithRequestFlags:blockRange:dataBlocks:completionHandler:")]
		void ExtendedWriteMultipleBlocks (NFCIso15693RequestFlag flags, NSRange blockRange, NSData [] dataBlocks, Action<NSError> completionHandler);

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
#if NET
		[Abstract]
#endif
		[Export ("authenticateWithRequestFlags:cryptoSuiteIdentifier:message:completionHandler:")]
		void Authenticate (NFCIso15693RequestFlag flags, nint cryptoSuiteIdentifier, NSData message, NFCIso15693TagResponseCallback completionHandler);

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
#if NET
		[Abstract]
#endif
		[Export ("keyUpdateWithRequestFlags:keyIdentifier:message:completionHandler:")]
		void KeyUpdate (NFCIso15693RequestFlag flags, nint keyIdentifier, NSData message, NFCIso15693TagResponseCallback completionHandler);

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
#if NET
		[Abstract]
#endif
		[Export ("challengeWithRequestFlags:cryptoSuiteIdentifier:message:completionHandler:")]
		void Challenge (NFCIso15693RequestFlag flags, nint cryptoSuiteIdentifier, NSData message, Action<NSError> completionHandler);

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
#if NET
		[Abstract]
#endif
		[Export ("readBufferWithRequestFlags:completionHandler:")]
		void ReadBuffer (NFCIso15693RequestFlag flags, NFCIso15693TagResponseCallback completionHandler);

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
#if NET
		[Abstract]
#endif
		[Export ("extendedGetMultipleBlockSecurityStatusWithRequestFlag:blockRange:completionHandler:")]
		void ExtendedGetMultipleBlockSecurityStatus (NFCIso15693RequestFlag flags, NSRange blockRange, NFCIso15693TagGetMultipleBlockSecurityStatusCallback completionHandler);

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
#if NET
		[Abstract]
#endif
		[Export ("extendedFastReadMultipleBlocksWithRequestFlag:blockRange:completionHandler:")]
		void ExtendedFastReadMultipleBlocks (NFCIso15693RequestFlag flags, NSRange blockRange, NFCIso15693TagReadMultipleBlocksCallback completionHandler);

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
#if NET
		[Abstract]
#endif
		[Export ("sendRequestWithFlag:commandCode:data:completionHandler:")]
		void SendRequest (nint flags, nint commandCode, [NullAllowed] NSData data, NFCIso15693TagResponseCallback completionHandler);

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
#if NET
		[Abstract]
#endif
		[Export ("getSystemInfoAndUIDWithRequestFlag:completionHandler:")]
		void GetSystemInfoAndUid (NFCIso15693RequestFlag flags, NFCIso15693TagGetSystemInfoAndUidCallback completionHandler);

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
#if NET
		[Abstract]
#endif
		[Export ("fastReadMultipleBlocksWithRequestFlag:blockRange:completionHandler:")]
		void FastReadMultipleBlocks (NFCIso15693RequestFlag flags, NSRange blockRange, NFCIso15693TagReadMultipleBlocksCallback completionHandler);

		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
#if NET
		[Abstract]
#endif
		[Export ("lockDSFIDWithRequestFlag:completionHandler:")]
		void LockDsfId (NFCIso15693RequestFlag flags, Action<NSError> completionHandler);

	}

	[MacCatalyst (13, 1)]
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

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("wellKnownTypeURIPayloadWithString:")]
		[return: NullAllowed]
		NFCNdefPayload CreateWellKnownTypePayload (string uri);

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("wellKnownTypeURIPayloadWithURL:")]
		[return: NullAllowed]
		NFCNdefPayload CreateWellKnownTypePayload (NSUrl url);

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("wellKnownTypeTextPayloadWithString:locale:")]
		[return: NullAllowed]
		NFCNdefPayload CreateWellKnownTypePayload (string text, NSLocale locale);

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[NullAllowed, Export ("wellKnownTypeURIPayload")]
		NSUrl WellKnownTypeUriPayload { get; }

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("wellKnownTypeTextPayloadWithLocale:")]
		[return: NullAllowed]
		string GetWellKnownTypeTextPayload (out NSLocale locale);

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("initWithFormat:type:identifier:payload:")]
		NativeHandle Constructor (NFCTypeNameFormat format, NSData type, NSData identifier, NSData payload);

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("initWithFormat:type:identifier:payload:chunkSize:")]
		NativeHandle Constructor (NFCTypeNameFormat format, NSData type, NSData identifier, NSData payload, nuint chunkSize);
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject), Name = "NFCNDEFMessage")]
	[DisableDefaultCtor]
	interface NFCNdefMessage : NSSecureCoding {

		[Export ("records", ArgumentSemantic.Copy)]
		NFCNdefPayload [] Records { get; set; }

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("ndefMessageWithData:")]
		[return: NullAllowed]
		NFCNdefMessage Create (NSData data);

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("initWithNDEFRecords:")]
		NativeHandle Constructor (NFCNdefPayload [] records);

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("length")]
		nuint Length { get; }
	}

	interface INFCNdefReaderSessionDelegate { }

	[MacCatalyst (13, 1)]
	[Protocol, Model]
	[BaseType (typeof (NSObject), Name = "NFCNDEFReaderSessionDelegate")]
	interface NFCNdefReaderSessionDelegate {

		[Abstract]
		[Export ("readerSession:didInvalidateWithError:")]
		void DidInvalidate (NFCNdefReaderSession session, NSError error);

		[Abstract]
		[Export ("readerSession:didDetectNDEFs:")]
		void DidDetect (NFCNdefReaderSession session, NFCNdefMessage [] messages);

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("readerSession:didDetectTags:")]
		void DidDetectTags (NFCNdefReaderSession session, INFCNdefTag [] tags);

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("readerSessionDidBecomeActive:")]
		void DidBecomeActive (NFCNdefReaderSession session);
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NFCReaderSession), Name = "NFCNDEFReaderSession")]
	[DisableDefaultCtor]
	interface NFCNdefReaderSession {

		[Export ("initWithDelegate:queue:invalidateAfterFirstRead:")]
		[DesignatedInitializer]
		NativeHandle Constructor (INFCNdefReaderSessionDelegate @delegate, [NullAllowed] DispatchQueue queue, bool invalidateAfterFirstRead);

		[Static]
		[Export ("readingAvailable")]
		bool ReadingAvailable { get; }

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("restartPolling")]
		void RestartPolling ();

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("connectToTag:completionHandler:")]
		[Async]
		void ConnectToTag (INFCNdefTag tag, Action<NSError> completionHandler);
	}

	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface NFCReaderSession : NFCReaderSessionContract {

		[Export ("delegate", ArgumentSemantic.Weak), NullAllowed]
		INFCReaderSessionDelegate Delegate { get; }

		[Export ("sessionQueue")]
		DispatchQueue SessionQueue { get; }

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Static]
		[Export ("readingAvailable")]
		bool ReadingAvailable { get; }
	}

	interface INFCReaderSessionContract { }

	[MacCatalyst (13, 1)]
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

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
#if NET
		[Abstract]
#endif
		[Export ("invalidateSessionWithErrorMessage:")]
		void InvalidateSession (string errorMessage);
	}

	interface INFCReaderSessionDelegate { }

	//[NoTV, NoWatch, NoMac]
	[MacCatalyst (13, 1)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface NFCReaderSessionDelegate {

		[Abstract]
		[Export ("readerSessionDidBecomeActive:")]
		void DidBecomeActive (NFCReaderSession session);

#if !NET
		[Abstract]
#endif
		[Export ("readerSession:didDetectTags:")]
		void DidDetectTags (NFCReaderSession session, INFCTag [] tags);

		[Abstract]
		[Export ("readerSession:didInvalidateWithError:")]
		void DidInvalidate (NFCReaderSession session, NSError error);
	}

	interface INFCTag { }

	[MacCatalyst (13, 1)]
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

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("asNFCISO15693Tag")]
#if NET
		[Abstract]
		[NullAllowed]
		INFCIso15693Tag AsNFCIso15693Tag { get; }
#else
		[return: NullAllowed]
		INFCIso15693Tag GetNFCIso15693Tag ();
#endif

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("asNFCISO7816Tag")]
#if NET
		[Abstract]
		[NullAllowed]
		INFCIso7816Tag AsNFCIso7816Tag { get; }
#else
		[return: NullAllowed]
		INFCIso7816Tag GetNFCIso7816Tag ();
#endif

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("asNFCFeliCaTag")]
#if NET
		[Abstract]
		[NullAllowed]
		INFCFeliCaTag AsNFCFeliCaTag { get; }
#else
		[return: NullAllowed]
		INFCFeliCaTag GetNFCFeliCaTag ();
#endif

		[iOS (13, 0)]
		[MacCatalyst (13, 1)]
		[Export ("asNFCMiFareTag")]
#if NET
		[Abstract]
		[NullAllowed]
		INFCMiFareTag AsNFCMiFareTag { get; }
#else
		[return: NullAllowed]
		INFCMiFareTag GetNFCMiFareTag ();
#endif
	}

	//[NoTV, NoWatch, NoMac]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	interface NFCTagCommandConfiguration : NSCopying {

		[Export ("maximumRetries")]
		nuint MaximumRetries { get; set; }

		[Export ("retryInterval")]
		double RetryInterval { get; set; }
	}

	[iOS (12, 0)]
	[MacCatalyst (13, 1)]
	[Category]
	[BaseType (typeof (NSUserActivity))]
	interface NSUserActivity_CoreNFC {

		[Export ("ndefMessagePayload")]
		NFCNdefMessage GetNdefMessagePayload ();
	}

	[iOS (13, 0)]
	[MacCatalyst (13, 1)]
#if NET
	[Native]
	enum NFCFeliCaEncryptionId
#else
	[Native ("NFCFeliCaEncryptionId")]
	enum EncryptionId
#endif
		: long {
		Aes = 79,
		Des = 65,
	}

	[iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	enum NFCMiFareFamily : long {
		Unknown = 1,
		Ultralight = 2,
		Plus = 3,
		DesFire = 4,
	}

	[iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Native]
	enum NFCNdefStatus : ulong {
		NotSupported = 1,
		ReadWrite = 2,
		ReadOnly = 3,
	}

	[iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Flags]
	[Native]
	enum NFCPollingOption : ulong {
		Iso14443 = 0x1,
		Iso15693 = 0x2,
		Iso18092 = 0x4,
		Pace = 0x8,
	}

	[iOS (13, 0)]
	[MacCatalyst (13, 1)]
#if NET
	[Native]
	enum NFCFeliCaPollingRequestCode
#else
	[Native ("NFCFeliCaPollingRequestCode")]
	enum PollingRequestCode
#endif
		: long {
		NoRequest = 0,
		SystemCode = 1,
		CommunicationPerformance = 2,
	}

	[iOS (13, 0)]
	[MacCatalyst (13, 1)]
#if NET
	[Native]
	enum NFCFeliCaPollingTimeSlot
#else
	[Native ("NFCFeliCaPollingTimeSlot")]
	enum PollingTimeSlot
#endif
		: long {
		Max1 = 0,
		Max2 = 1,
		Max4 = 3,
		Max8 = 7,
		Max16 = 15,
	}

	[iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Flags]
	[NativeName ("NFCISO15693RequestFlag")]
#if NET
	enum NFCIso15693RequestFlag
#else
	enum RequestFlag
#endif
		: byte {
		DualSubCarriers = (1 << 0),
		HighDataRate = (1 << 1),
		ProtocolExtension = (1 << 3),
		Select = (1 << 4),
		Address = (1 << 5),
		Option = (1 << 6),
		[iOS (14, 0)]
		[MacCatalyst (14, 0)]
		CommandSpecificBit8 = (1 << 7),
	}

	[Flags, iOS (14, 0)]
	[MacCatalyst (14, 0)]
	public enum NFCIso15693ResponseFlag : byte {
		Error = (1 << 0),
		ResponseBufferValid = (1 << 1),
		FinalResponse = (1 << 2),
		ProtocolExtension = (1 << 3),
		BlockSecurityStatusBit5 = (1 << 4),
		BlockSecurityStatusBit6 = (1 << 5),
		WaitTimeExtension = (1 << 6),
	}

	[iOS (13, 0)]
	[MacCatalyst (13, 1)]
#if NET
	[Native]
	enum NFCVasErrorCode
#else
	[Native ("NFCVASErrorCode")]
	enum VasErrorCode
#endif
		: long {
		Success = 36864,
		DataNotFound = 27267,
		DataNotActivated = 25223,
		WrongParameters = 27392,
		WrongLCField = 26368,
		UserIntervention = 27012,
		IncorrectData = 27264,
		UnsupportedApplicationVersion = 25408,
	}

	[iOS (13, 0)]
	[MacCatalyst (13, 1)]
#if NET
	[Native]
	enum NFCVasMode 
#else
	[Native ("NFCVASMode")]
	enum VasMode
#endif
		: long {
		UrlOnly = 0,
		Normal = 1,
	}

	interface INFCNdefTag { }

	delegate void NFCQueryNdefStatusCompletionHandler (NFCNdefStatus status, nuint capacity, NSError error);

	[iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Protocol (Name = "NFCNDEFTag")]
	interface NFCNdefTag : NSSecureCoding, NSCopying {

		[Abstract]
		[Export ("available")]
		bool Available { [Bind ("isAvailable")] get; }

		[Abstract]
		[Export ("queryNDEFStatusWithCompletionHandler:")]
		void QueryNdefStatus (NFCQueryNdefStatusCompletionHandler completionHandler);

		[Abstract]
		[Export ("readNDEFWithCompletionHandler:")]
		void ReadNdef (Action<NFCNdefMessage, NSError> completionHandler);

		[Abstract]
		[Export ("writeNDEF:completionHandler:")]
		void WriteNdef (NFCNdefMessage ndefMessage, Action<NSError> completionHandler);

		[Abstract]
		[Export ("writeLockWithCompletionHandler:")]
		void WriteLock (Action<NSError> completionHandler);
	}

	interface INFCFeliCaTag { }

	delegate void NFCFeliCaPollingCompletionHandler (NSData pmm, NSData requestData, NSError error);
	delegate void NFCFeliCaReadWithoutEncryptionCompletionHandler (nint statusFlag1, nint statusFlag2, NSData [] blockData, NSError error);
	delegate void NFCFeliCaStatus1Status2CompletionHandler (nint statusFlag1, nint statusFlag2, NSError error);
	delegate void NFCFeliCaRequestServiceV2CompletionHandler (nint statusFlag1, nint statusFlag2, NFCFeliCaEncryptionId encryptionIdentifier, NSData [] nodeKeyVersionListAes, NSData [] nodeKeyVersionListDes, NSError error);
	delegate void NFCFeliCaRequestSpecificationVersionCompletionHandler (nint statusFlag1, nint statusFlag2, NSData basicVersion, NSData optionVersion, NSError error);

	[iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface NFCFeliCaTag : NFCTag, NFCNdefTag {

		[Abstract]
		[Export ("currentSystemCode", ArgumentSemantic.Retain)]
		NSData CurrentSystemCode { get; }

		[Abstract]
		[Export ("currentIDm", ArgumentSemantic.Retain)]
		NSData CurrentIdm { get; }

		[Abstract]
		[Export ("pollingWithSystemCode:requestCode:timeSlot:completionHandler:")]
		void Polling (NSData systemCode, NFCFeliCaPollingRequestCode requestCode, NFCFeliCaPollingTimeSlot timeSlot, NFCFeliCaPollingCompletionHandler completionHandler);

		[Abstract]
		[Export ("requestServiceWithNodeCodeList:completionHandler:")]
		void RequestService (NSData [] nodeCodeList, Action<NSData [], NSError> completionHandler);

		[Abstract]
		[Export ("requestResponseWithCompletionHandler:")]
		void RequestResponse (Action<nint, NSError> completionHandler);

		[Abstract]
		[Export ("readWithoutEncryptionWithServiceCodeList:blockList:completionHandler:")]
		void ReadWithoutEncryption (NSData [] serviceCodeList, NSData [] blockList, NFCFeliCaReadWithoutEncryptionCompletionHandler completionHandler);

		[Abstract]
		[Export ("writeWithoutEncryptionWithServiceCodeList:blockList:blockData:completionHandler:")]
		void WriteWithoutEncryption (NSData [] serviceCodeList, NSData [] blockList, NSData [] blockData, NFCFeliCaStatus1Status2CompletionHandler completionHandler);

		[Abstract]
		[Export ("requestSystemCodeWithCompletionHandler:")]
		void RequestSystemCode (Action<NSData [], NSError> completionHandler);

		[Abstract]
		[Export ("requestServiceV2WithNodeCodeList:completionHandler:")]
		void RequestServiceV2 (NSData [] nodeCodeList, NFCFeliCaRequestServiceV2CompletionHandler completionHandler);

		[Abstract]
		[Export ("requestSpecificationVersionWithCompletionHandler:")]
		void RequestSpecificationVersion (NFCFeliCaRequestSpecificationVersionCompletionHandler completionHandler);

		[Abstract]
		[Export ("resetModeWithCompletionHandler:")]
		void ResetMode (NFCFeliCaStatus1Status2CompletionHandler completionHandler);

		[Abstract]
		[Export ("sendFeliCaCommandPacket:completionHandler:")]
		void Send (NSData commandPacket, Action<NSData, NSError> completionHandler);
	}

	interface INFCIso7816Tag { }

	[iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Protocol (Name = "NFCISO7816Tag")]
	interface NFCIso7816Tag : NFCTag, NFCNdefTag {

		[Abstract]
		[Export ("initialSelectedAID", ArgumentSemantic.Retain)]
		string InitialSelectedAid { get; }

		[Abstract]
		[Export ("identifier", ArgumentSemantic.Copy)]
		NSData Identifier { get; }

		[Abstract]
		[NullAllowed, Export ("historicalBytes", ArgumentSemantic.Copy)]
		NSData HistoricalBytes { get; }

		[Abstract]
		[NullAllowed, Export ("applicationData", ArgumentSemantic.Copy)]
		NSData ApplicationData { get; }

		[Abstract]
		[Export ("proprietaryApplicationDataCoding")]
		bool ProprietaryApplicationDataCoding { get; }

		[Abstract]
		[Export ("sendCommandAPDU:completionHandler:")]
		void SendCommand (NFCIso7816Apdu apdu, NFCIso7816SendCompletionHandler completionHandler);
	}

	[iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject), Name = "NFCISO7816APDU")]
	[DisableDefaultCtor]
	interface NFCIso7816Apdu : NSCopying {

		[Export ("initWithInstructionClass:instructionCode:p1Parameter:p2Parameter:data:expectedResponseLength:")]
		NativeHandle Constructor (byte instructionClass, byte instructionCode, byte p1Parameter, byte p2Parameter, NSData data, nint expectedResponseLength);

		[Export ("initWithData:")]
		NativeHandle Constructor (NSData data);

		[Export ("instructionClass")]
		byte InstructionClass { get; }

		[Export ("instructionCode")]
		byte InstructionCode { get; }

		[Export ("p1Parameter")]
		byte P1Parameter { get; }

		[Export ("p2Parameter")]
		byte P2Parameter { get; }

		[NullAllowed, Export ("data", ArgumentSemantic.Copy)]
		NSData Data { get; }

		[Export ("expectedResponseLength")]
		nint ExpectedResponseLength { get; }
	}

	interface INFCMiFareTag { }

	delegate void NFCIso7816SendCompletionHandler (NSData responseData, byte sw1, byte sw2, NSError error);

	[iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[Protocol]
	interface NFCMiFareTag : NFCTag, NFCNdefTag {
#if NET
		[Abstract]
#endif
		[Export ("mifareFamily", ArgumentSemantic.Assign)]
		NFCMiFareFamily MifareFamily { get; }

		[Abstract]
		[Export ("identifier", ArgumentSemantic.Copy)]
		NSData Identifier { get; }

		[Abstract]
		[NullAllowed, Export ("historicalBytes", ArgumentSemantic.Copy)]
		NSData HistoricalBytes { get; }

		[Abstract]
		[Export ("sendMiFareCommand:completionHandler:")]
		void SendMiFareCommand (NSData command, Action<NSData, NSError> completionHandler);

		[Abstract]
		[Export ("sendMiFareISO7816Command:completionHandler:")]
		void SendMiFareIso7816Command (NFCIso7816Apdu apdu, NFCIso7816SendCompletionHandler completionHandler);
	}

	interface INFCTagReaderSessionDelegate { }

	[iOS (13, 0)]
	[MacCatalyst (13, 1)]
#if NET
	[Protocol][Model]
#else
	[Protocol]
	[Model (AutoGeneratedName = true)]
#endif
	[BaseType (typeof (NSObject))]
	interface NFCTagReaderSessionDelegate {

		[Abstract]
		[Export ("tagReaderSession:didInvalidateWithError:")]
		void DidInvalidate (NFCTagReaderSession session, NSError error);

		[Export ("tagReaderSessionDidBecomeActive:")]
		void DidBecomeActive (NFCTagReaderSession session);

		[Export ("tagReaderSession:didDetectTags:")]
		void DidDetectTags (NFCTagReaderSession session, INFCTag [] tags);
	}

	[iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NFCReaderSession))]
	[DisableDefaultCtor]
	[Advice ("Not available for application extensions.")]
	interface NFCTagReaderSession {

		[Export ("initWithPollingOption:delegate:queue:")]
		NativeHandle Constructor (NFCPollingOption pollingOption, INFCTagReaderSessionDelegate @delegate, [NullAllowed] DispatchQueue queue);

		[NullAllowed, Export ("connectedTag", ArgumentSemantic.Retain)]
		INFCTag ConnectedTag { get; }

		[Export ("restartPolling")]
		void RestartPolling ();

		[Export ("connectToTag:completionHandler:")]
		[Async]
		void ConnectTo (INFCTag tag, Action<NSError> completionHandler);

		[Field ("NFCTagResponseUnexpectedLengthErrorKey")]
		NSString UnexpectedLengthErrorKey { get; }
	}

	interface INFCVasReaderSessionDelegate { }

	[iOS (13, 0)]
	[MacCatalyst (13, 1)]
#if NET
	[Protocol][Model]
#else
	[Protocol]
	[Model (AutoGeneratedName = true)]
#endif
	[BaseType (typeof (NSObject), Name = "NFCVASReaderSessionDelegate")]
	interface NFCVasReaderSessionDelegate {

		[Export ("readerSessionDidBecomeActive:")]
		void DidBecomeActive (NFCVasReaderSession session);

		[Abstract]
		[Export ("readerSession:didInvalidateWithError:")]
		void DidInvalidate (NFCVasReaderSession session, NSError error);

		[Abstract]
		[Export ("readerSession:didReceiveVASResponses:")]
		void DidReceiveVasResponses (NFCVasReaderSession session, NFCVasResponse [] responses);
	}

	[iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject), Name = "NFCVASResponse")]
	[DisableDefaultCtor] // implied
	interface NFCVasResponse : NSCopying {

		[Export ("status", ArgumentSemantic.Assign)]
		NFCVasErrorCode Status { get; }

		[Export ("vasData", ArgumentSemantic.Retain)]
		NSData VasData { get; }

		[Export ("mobileToken", ArgumentSemantic.Retain)]
		NSData MobileToken { get; }
	}

	[iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject), Name = "NFCVASCommandConfiguration")]
	[DisableDefaultCtor] // implied (designated initializer)
	interface NFCVasCommandConfiguration : NSCopying {

		[Export ("initWithVASMode:passTypeIdentifier:url:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NFCVasMode mode, string passTypeIdentifier, [NullAllowed] NSUrl url);

		[Export ("mode", ArgumentSemantic.Assign)]
		NFCVasMode Mode { get; set; }

		[Export ("passTypeIdentifier", ArgumentSemantic.Retain)]
		string PassTypeIdentifier { get; set; }

		[NullAllowed, Export ("url", ArgumentSemantic.Retain)]
		NSUrl Url { get; set; }
	}

	[iOS (13, 0)]
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NFCReaderSession), Name = "NFCVASReaderSession")]
	[DisableDefaultCtor]
	[Advice ("Not available for application extensions.")]
	interface NFCVasReaderSession {

		[Export ("initWithVASCommandConfigurations:delegate:queue:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NFCVasCommandConfiguration [] commandConfigurations, INFCVasReaderSessionDelegate @delegate, [NullAllowed] DispatchQueue queue);
	}
}
