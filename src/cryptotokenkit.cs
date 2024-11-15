#if NET

// Let's hope that by .NET 10 we've ironed out all the bugs in the API.
// This can of course be adjusted as needed (until we've released as stable).
#if NET10_0_OR_GREATER
#define STABLE_CRYPTOTOKENKIT
#endif

using System;
using System.Diagnostics.CodeAnalysis;

using Foundation;
using ObjCRuntime;
using Security;

namespace CryptoTokenKit {
#if !STABLE_CRYPTOTOKENKIT
	[Experimental ("APL0001")]
#endif
	[Native]
	[ErrorDomain ("TKErrorDomain")]
	[iOS (9, 0), Mac (10, 10), MacCatalyst (13, 1), TV (9, 0), Watch (2, 0)]
	public enum TKErrorCode : long
	{
		NotImplemented = -1,
		CommunicationError = -2,
		CorruptedData = -3,
		CanceledByUser = -4,
		AuthenticationFailed = -5,
		ObjectNotFound = -6,
		TokenNotFound = -7,
		BadParameter = -8,
		AuthenticationNeeded = -9,
	}

#if !STABLE_CRYPTOTOKENKIT
	[Experimental ("APL0001")]
#endif
	[iOS (13, 0), Mac (10, 10), MacCatalyst (13, 1), TV (13, 0), Watch (8, 0)]
	[Native]
	public enum TKSmartCardSlotState : long
	{
		Missing = 0,
		Empty = 1,
		Probing = 2,
		MuteCard = 3,
		ValidCard = 4,
	}

#if !STABLE_CRYPTOTOKENKIT
	[Experimental ("APL0001")]
#endif
	[iOS (13, 0), Mac (10, 11), MacCatalyst (13, 1), TV (13, 0), Watch (8, 0)]
	[Native ("TKSmartCardPINCharset")]
	public enum TKSmartCardPinCharset : long
	{
		Numeric = 0,
		Alphanumeric = 1,
		UpperAlphanumeric = 2,
	}

#if !STABLE_CRYPTOTOKENKIT
	[Experimental ("APL0001")]
#endif
	[iOS (13, 0), Mac (10, 11), MacCatalyst (13, 1), TV (13, 0), Watch (8, 0)]
	[Native ("TKSmartCardPINEncoding")]
	public enum TKSmartCardPinEncoding : long
	{
		Binary = 0,
		Ascii = 1,
		Bcd = 2,
	}

#if !STABLE_CRYPTOTOKENKIT
	[Experimental ("APL0001")]
#endif
	[iOS (13, 0), Mac (10, 11), MacCatalyst (13, 1), TV (13, 0), Watch (8, 0)]
	[Native ("TKSmartCardPINJustification")]
	public enum TKSmartCardPinJustification : long
	{
		Left = 0,
		Right = 1,
	}

#if !STABLE_CRYPTOTOKENKIT
	[Experimental ("APL0001")]
#endif
	[Flags]
	[iOS (13, 0), Mac (10, 11), MacCatalyst (13, 1), TV (13, 0), Watch (8, 0)]
	[Native ("TKSmartCardPINCompletion")]
	public enum TKSmartCardPinCompletion : ulong
	{
		MaxLength = (1uL << 0),
		Key = (1uL << 1),
		Timeout = (1uL << 2),
	}

#if !STABLE_CRYPTOTOKENKIT
	[Experimental ("APL0001")]
#endif
	[Flags]
	[iOS (13, 0), Mac (10, 11), MacCatalyst (13, 1), TV (13, 0), Watch (8, 0)]
	[Native ("TKSmartCardPINConfirmation")]
	public enum TKSmartCardPinConfirmation : ulong
	{
		None = 0x0,
		New = (1uL << 0),
		Current = (1uL << 1),
	}

#if !STABLE_CRYPTOTOKENKIT
	[Experimental ("APL0001")]
#endif
	[Flags]
	[iOS (13, 0), Mac (10, 10), MacCatalyst (13, 1), TV (13, 0), Watch (8, 0)]
	[Native]
	public enum TKSmartCardProtocol : ulong
	{
		None = 0x0,
		T0 = (1uL << 0),
		T1 = (1uL << 1),
		T15 = (1uL << 15),
		Any = (1uL << 16) - 0x1,
	}

#if !STABLE_CRYPTOTOKENKIT
	[Experimental ("APL0001")]
#endif
	[iOS (10, 0), Mac (10, 12), MacCatalyst (13, 1), TV (11, 0), Watch (4, 0)]
	[Native]
	public enum TKTokenOperation : long
	{
		None = 0,
		ReadData = 1,
		SignData = 2,
		DecryptData = 3,
		PerformKeyExchange = 4,
	}

#if !STABLE_CRYPTOTOKENKIT
	[Experimental ("APL0001")]
#endif
	[iOS (13, 0), Mac (10, 12), MacCatalyst (13, 1), TV (13, 0), Watch (8, 0)]
	[Protocol]
	interface TKSmartCardTokenDriverDelegate : TKTokenDriverDelegate
	{
		[Abstract]
		[Export ("tokenDriver:createTokenForSmartCard:AID:error:")]
		[return: NullAllowed]
		TKSmartCardToken CreateToken (TKSmartCardTokenDriver driver, TKSmartCard smartCard, [NullAllowed] NSData aid, [NullAllowed] out NSError error);
	}

#if !STABLE_CRYPTOTOKENKIT
	[Experimental ("APL0001")]
#endif
	interface ITKSmartCardTokenDriverDelegate {}

#if !STABLE_CRYPTOTOKENKIT
	[Experimental ("APL0001")]
#endif
	[iOS (13, 0), Mac (10, 11), MacCatalyst (13, 1), TV (13, 0), Watch (8, 0)]
	[Protocol]
	interface TKSmartCardUserInteractionDelegate
	{
		[Export ("characterEnteredInUserInteraction:")]
		void CharacterEntered (TKSmartCardUserInteraction interaction);

		[Export ("correctionKeyPressedInUserInteraction:")]
		void CorrectionKeyPressed (TKSmartCardUserInteraction interaction);

		[Export ("validationKeyPressedInUserInteraction:")]
		void ValidationKeyPressed (TKSmartCardUserInteraction interaction);

		[Export ("invalidCharacterEnteredInUserInteraction:")]
		void InvalidCharacterEntered (TKSmartCardUserInteraction interaction);

		[Export ("oldPINRequestedInUserInteraction:")]
		void OldPinRequested (TKSmartCardUserInteraction interaction);

		[Export ("newPINRequestedInUserInteraction:")]
		void NewPinRequested (TKSmartCardUserInteraction interaction);

		[Export ("newPINConfirmationRequestedInUserInteraction:")]
		void NewPinConfirmationRequested (TKSmartCardUserInteraction interaction);
	}

#if !STABLE_CRYPTOTOKENKIT
	[Experimental ("APL0001")]
#endif
	interface ITKSmartCardUserInteractionDelegate {}

#if !STABLE_CRYPTOTOKENKIT
	[Experimental ("APL0001")]
#endif
	[iOS (10, 0), Mac (10, 12), MacCatalyst (13, 1), TV (11, 0), Watch (4, 0)]
	[Protocol, Model]
	[BaseType (typeof(NSObject))]
	interface TKTokenDelegate
	{
		[Abstract]
		[Export ("token:createSessionWithError:")]
		[return: NullAllowed]
		TKTokenSession CreateSession (TKToken token, [NullAllowed] out NSError error);

		[Export ("token:terminateSession:")]
		void TerminateSession (TKToken token, TKTokenSession session);
	}

#if !STABLE_CRYPTOTOKENKIT
	[Experimental ("APL0001")]
#endif
	interface ITKTokenDelegate {}

#if !STABLE_CRYPTOTOKENKIT
	[Experimental ("APL0001")]
#endif
	[iOS (10, 0), Mac (10, 12), MacCatalyst (13, 1), TV (11, 0), Watch (4, 0)]
	[Protocol, Model]
	[BaseType (typeof(NSObject))]
	interface TKTokenDriverDelegate
	{
		[Watch (7,0), TV (14,0), Mac (10,15), iOS (14,0)]
		[Export ("tokenDriver:tokenForConfiguration:error:")]
		[return: NullAllowed]
		TKToken GetToken (TKTokenDriver driver, TKTokenConfiguration configuration, [NullAllowed] out NSError error);

		[Export ("tokenDriver:terminateToken:")]
		void TerminateToken (TKTokenDriver driver, TKToken token);
	}

#if !STABLE_CRYPTOTOKENKIT
	[Experimental ("APL0001")]
#endif
	interface ITKTokenDriverDelegate {}

#if !STABLE_CRYPTOTOKENKIT
	[Experimental ("APL0001")]
#endif
	[iOS (10, 0), Mac (10, 12), MacCatalyst (13, 1), TV (11, 0), Watch (4, 0)]
	[Protocol, Model]
	[BaseType (typeof(NSObject))]
	interface TKTokenSessionDelegate
	{
		[Export ("tokenSession:beginAuthForOperation:constraint:error:")]
		[return: NullAllowed]
		TKTokenAuthOperation BeginAuthentication (TKTokenSession session, TKTokenOperation operation, NSObject constraint, [NullAllowed] out NSError error);

		[Export ("tokenSession:supportsOperation:usingKey:algorithm:")]
		bool SupportsOperation (TKTokenSession session, TKTokenOperation operation, NSObject keyObjectId, TKTokenKeyAlgorithm algorithm);

		[Export ("tokenSession:signData:usingKey:algorithm:error:")]
		[return: NullAllowed]
		NSData SignData (TKTokenSession session, NSData dataToSign, NSObject keyObjectId, TKTokenKeyAlgorithm algorithm, [NullAllowed] out NSError error);

		[Export ("tokenSession:decryptData:usingKey:algorithm:error:")]
		[return: NullAllowed]
		NSData DecryptData (TKTokenSession session, NSData ciphertext, NSObject keyObjectId, TKTokenKeyAlgorithm algorithm, [NullAllowed] out NSError error);

		[Export ("tokenSession:performKeyExchangeWithPublicKey:usingKey:algorithm:parameters:error:")]
		[return: NullAllowed]
		NSData PerformKeyExchange (TKTokenSession session, NSData otherPartyPublicKeyData, NSObject objectId, TKTokenKeyAlgorithm algorithm, TKTokenKeyExchangeParameters parameters, [NullAllowed] out NSError error);
	}

#if !STABLE_CRYPTOTOKENKIT
	[Experimental ("APL0001")]
#endif
	interface ITKTokenSessionDelegate {}

#if !STABLE_CRYPTOTOKENKIT
	[Experimental ("APL0001")]
#endif
	[iOS (10, 0), Mac (10, 12), MacCatalyst (13, 1), TV (11, 0), Watch (4, 0)]
	[BaseType (typeof(TKTlvRecord), Name = "TKBERTLVRecord")]
	interface TKBerTlvRecord
	{
		[Static]
		[Export ("dataForTag:")]
		NSData GetData (ulong tag);

		[Export ("initWithTag:value:")]
		NativeHandle Constructor (ulong tag, NSData value);

		[Export ("initWithTag:records:")]
		NativeHandle Constructor (ulong tag, TKTlvRecord[] records);
	}

#if !STABLE_CRYPTOTOKENKIT
	[Experimental ("APL0001")]
#endif
	[iOS (10, 0), Mac (10, 12), MacCatalyst (13, 1), TV (11, 0), Watch (4, 0)]
	[BaseType (typeof(TKTlvRecord), Name = "TKCompactTLVRecord")]
	interface TKCompactTlvRecord
	{
		[Export ("initWithTag:value:")]
		NativeHandle Constructor (byte tag, NSData value);
	}

#if !STABLE_CRYPTOTOKENKIT
	[Experimental ("APL0001")]
#endif
	[iOS (10, 0), Mac (10, 12), MacCatalyst (13, 1), TV (11, 0), Watch (4, 0)]
	[BaseType (typeof(TKTlvRecord), Name = "TKSimpleTLVRecord")]
	interface TKSimpleTlvRecord
	{
		[Export ("initWithTag:value:")]
		NativeHandle Constructor (byte tag, NSData value);
	}

#if !STABLE_CRYPTOTOKENKIT
	[Experimental ("APL0001")]
#endif
	delegate void TKSmartCardSendCallback ([NullAllowed] NSData replyData, ushort sq, [NullAllowed] NSError error);

#if !STABLE_CRYPTOTOKENKIT
	[Experimental ("APL0001")]
#endif
	delegate bool TKSmartCardExecuteCallback ([NullAllowed] out NSError error);

#if !STABLE_CRYPTOTOKENKIT
	[Experimental ("APL0001")]
#endif
	[iOS (13, 0), Mac (10, 10), MacCatalyst (13, 1), TV (13, 0), Watch (8, 0)]
	[BaseType (typeof(NSObject))]
	interface TKSmartCard
	{
		[Export ("slot")]
		TKSmartCardSlot Slot { get; }

		[Export ("valid")]
		bool Valid { get; }

		[Export ("allowedProtocols", ArgumentSemantic.Assign)]
		TKSmartCardProtocol AllowedProtocols { get; set; }

		[Export ("currentProtocol")]
		TKSmartCardProtocol CurrentProtocol { get; }

		[Export ("sensitive")]
		bool Sensitive { get; set; }

		[NullAllowed, Export ("context", ArgumentSemantic.Assign)]
		NSObject Context { get; set; }

		[Export ("beginSessionWithReply:")]
		[Async]
		void BeginSession (Action<bool, NSError> reply);

		[Export ("transmitRequest:reply:")]
		[Async]
		void TransmitRequest (NSData request, Action<NSData, NSError> reply);

		[Export ("endSession")]
		void EndSession ();

		[Mac (10, 11)]
		[Export ("userInteractionForSecurePINVerificationWithPINFormat:APDU:PINByteOffset:")]
		[return: NullAllowed]
		TKSmartCardUserInteractionForSecurePinVerification CreateUserInteractionForSecurePinVerification (TKSmartCardPinFormat pinFormat, NSData apdu, nint pinByteOffset);

		[Mac (10, 11)]
		[Export ("userInteractionForSecurePINChangeWithPINFormat:APDU:currentPINByteOffset:newPINByteOffset:")]
		[return: NullAllowed]
		TKSmartCardUserInteractionForSecurePinChange CreateUserInteractionForSecurePinChange (TKSmartCardPinFormat pinFormat, NSData apdu, nint currentPinByteOffset, nint newPinByteOffset);

		// This is from the APDULevelTransmit category
		[Mac (10, 10)]
		[Export ("cla")]
		byte Cla { get; set; }

		[Export ("useExtendedLength")]
		bool UseExtendedLength { get; set; }

		[Mac (10, 12)]
		[Export ("useCommandChaining")]
		bool UseCommandChaining { get; set; }

		[Export ("sendIns:p1:p2:data:le:reply:")]
		void Send (byte ins, byte p1, byte p2, [NullAllowed] NSData requestData, [NullAllowed] NSNumber le, TKSmartCardSendCallback reply);

		[Mac (10,12)]
		[Export ("inSessionWithError:executeBlock:")]
		bool ExecuteBlock ([NullAllowed] out NSError error, TKSmartCardExecuteCallback block);

		[Mac (10,12)]
		[Export ("sendIns:p1:p2:data:le:sw:error:")]
		[return: NullAllowed]
		NSData Send (byte ins, byte p1, byte p2, [NullAllowed] NSData requestData, [NullAllowed] NSNumber le, out ushort sw, [NullAllowed] out NSError error);
	}

#if !STABLE_CRYPTOTOKENKIT
	[Experimental ("APL0001")]
#endif
	[iOS (13, 0), Mac (10, 10), MacCatalyst (13, 1), TV (13, 0), Watch (8, 0)]
	[BaseType (typeof(NSObject), Name = "TKSmartCardATR")]
	[DisableDefaultCtor]
	interface TKSmartCardAtr
	{
		[Export ("initWithBytes:")]
		NativeHandle Constructor (NSData bytes);

		[Export ("initWithSource:")]
		NativeHandle Constructor (Func<int> source);

		[Export ("bytes")]
		NSData Bytes { get; }

		[Export ("protocols")]
		[BindAs (typeof (TKSmartCardProtocol[]))]
		NSNumber[] Protocols { get; }

		// -(TKSmartCardATRInterfaceGroup * _Nullable)interfaceGroupAtIndex:(NSInteger)index;
		[Export ("interfaceGroupAtIndex:")]
		[return: NullAllowed]
		TKSmartCardAtrInterfaceGroup GetInterfaceGroup (nint index);

		[Export ("interfaceGroupForProtocol:")]
		[return: NullAllowed]
		TKSmartCardAtrInterfaceGroup GetInterfaceGroup (TKSmartCardProtocol protocol);

		[Export ("historicalBytes")]
		NSData HistoricalBytes { get; }

		[Mac (10, 12)]
		[Export ("historicalRecords")]
		[NullAllowed]
		TKCompactTlvRecord[] HistoricalRecords { get; }
	}

#if !STABLE_CRYPTOTOKENKIT
	[Experimental ("APL0001")]
#endif
	[iOS (13, 0), Mac (10, 10), MacCatalyst (13, 1), TV (13, 0), Watch (8, 0)]
	[BaseType (typeof(NSObject), Name = "TKSmartCardATRInterfaceGroup")]
	interface TKSmartCardAtrInterfaceGroup
	{
		[NullAllowed, Export ("TA")]
		NSNumber TA { get; }

		// not clear from the documentation if we can BindAs this to something or not
		[NullAllowed, Export ("TB")]
		NSNumber TB { get; }

		// not clear from the documentation if we can BindAs this to something or not
		[NullAllowed, Export ("TC")]
		NSNumber TC { get; }

		// not clear from the documentation if we can BindAs this to something or not
		[NullAllowed, Export ("protocol")]
		NSNumber Protocol { get; }
	}

#if !STABLE_CRYPTOTOKENKIT
	[Experimental ("APL0001")]
#endif
	[iOS (10, 0), Mac (10, 12), MacCatalyst (13, 1), TV (11, 0), Watch (4, 0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface TKTokenSession
	{
		[Export ("initWithToken:")]
		[DesignatedInitializer]
		NativeHandle Constructor (TKToken token);

		[Export ("token")]
		TKToken Token { get; }

		[Wrap ("WeakDelegate")]
		[NullAllowed]
		ITKTokenSessionDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }
	}

#if !STABLE_CRYPTOTOKENKIT
	[Experimental ("APL0001")]
#endif
	[iOS (13, 0), Mac (10, 11), MacCatalyst (13, 1), TV (13, 0), Watch (8, 0)]
	[BaseType (typeof(NSObject), Name = "TKSmartCardPINFormat")]
	interface TKSmartCardPinFormat
	{
		[Export ("charset", ArgumentSemantic.Assign)]
		TKSmartCardPinCharset Charset { get; set; }

		[Export ("encoding", ArgumentSemantic.Assign)]
		TKSmartCardPinEncoding Encoding { get; set; }

		[Export ("minPINLength")]
		nint MinPinLength { get; set; }

		[Export ("maxPINLength")]
		nint MaxPinLength { get; set; }

		[Export ("PINBlockByteLength")]
		nint PinBlockByteLength { get; set; }

		[Export ("PINJustification", ArgumentSemantic.Assign)]
		TKSmartCardPinJustification PinJustification { get; set; }

		[Export ("PINBitOffset")]
		nint PinBitOffset { get; set; }

		[Export ("PINLengthBitOffset")]
		nint PinLengthBitOffset { get; set; }

		[Export ("PINLengthBitSize")]
		nint PinLengthBitSize { get; set; }
	}

#if !STABLE_CRYPTOTOKENKIT
	[Experimental ("APL0001")]
#endif
	[iOS (13, 0), Mac (10, 10), MacCatalyst (13, 1), TV (13, 0), Watch (8, 0)]
	[BaseType (typeof(NSObject))]
	interface TKSmartCardSlot
	{
		[Export ("state")]
		TKSmartCardSlotState State { get; }

		[NullAllowed, Export ("ATR")]
		TKSmartCardAtr Atr { get; }

		[Export ("name")]
		string Name { get; }

		[Export ("maxInputLength")]
		nint MaxInputLength { get; }

		[Export ("maxOutputLength")]
		nint MaxOutputLength { get; }

		[return: NullAllowed]
		[Export ("makeSmartCard")]
		TKSmartCard MakeSmartCard ();
	}

#if !STABLE_CRYPTOTOKENKIT
	[Experimental ("APL0001")]
#endif
	delegate void TKGetSlotCallback ([NullAllowed] TKSmartCardSlot slot);

#if !STABLE_CRYPTOTOKENKIT
	[Experimental ("APL0001")]
#endif
	[iOS (13, 0), Mac (10, 10), MacCatalyst (13, 1), TV (13, 0), Watch (8, 0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface TKSmartCardSlotManager
	{
		[Static]
		[NullAllowed, Export ("defaultManager")]
		TKSmartCardSlotManager DefaultManager { get; }

		[Export ("slotNames")]
		string[] SlotNames { get; }

		[Export ("getSlotWithName:reply:")]
		[Async]
		void GetSlot (string name, TKGetSlotCallback reply);

		[Mac (10,13)]
		[Export ("slotNamed:")]
		[return: NullAllowed]
		TKSmartCardSlot GetSlot (string name);
	}

#if !STABLE_CRYPTOTOKENKIT
	[Experimental ("APL0001")]
#endif
	[iOS (10, 0), Mac (10, 12), MacCatalyst (13, 1), TV (11, 0), Watch (4, 0)]
	[BaseType (typeof(TKToken))]
	interface TKSmartCardToken
	{
		[Export ("initWithSmartCard:AID:instanceID:tokenDriver:")]
		[DesignatedInitializer]
		NativeHandle Constructor (TKSmartCard smartCard, [NullAllowed] NSData aid, string instanceId, TKSmartCardTokenDriver tokenDriver);

		[NullAllowed, Export ("AID")]
		NSData Aid { get; }
	}

#if !STABLE_CRYPTOTOKENKIT
	[Experimental ("APL0001")]
#endif
	[iOS (10, 0), Mac (10, 12), MacCatalyst (13, 1), TV (11, 0), Watch (4, 0)]
	[BaseType (typeof (TKTokenDriver))]
	interface TKSmartCardTokenDriver
	{
	}

#if !STABLE_CRYPTOTOKENKIT
	[Experimental ("APL0001")]
#endif
	[iOS (10, 0), Mac (10, 12), MacCatalyst (13, 1), TV (11, 0), Watch (4, 0)]
	[BaseType (typeof(TKTokenSession))]
	interface TKSmartCardTokenSession
	{
		// Re-implemented from base class.
		[Export ("initWithToken:")]
		[DesignatedInitializer]
		NativeHandle Constructor (TKToken token);

		[Export ("smartCard")]
		TKSmartCard SmartCard { get; }
	}

#if !STABLE_CRYPTOTOKENKIT
	[Experimental ("APL0001")]
#endif
	[iOS (13, 0), Mac (10, 11), MacCatalyst (13, 1), TV (13, 0), Watch (8, 0)]
	[BaseType (typeof(NSObject))]
	interface TKSmartCardUserInteraction
	{
		[Wrap ("WeakDelegate")]
		[NullAllowed]
		ITKSmartCardUserInteractionDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		[Export ("initialTimeout")]
		double InitialTimeout { get; set; }

		[Export ("interactionTimeout")]
		double InteractionTimeout { get; set; }

		[Export ("runWithReply:")]
		[Async]
		void Run (Action<bool, NSError> reply);

		[Export ("cancel")]
		bool Cancel ();
	}

#if !STABLE_CRYPTOTOKENKIT
	[Experimental ("APL0001")]
#endif
	[iOS (13, 0), Mac (10, 11), MacCatalyst (13, 1), TV (13, 0), Watch (8, 0)]
	[BaseType (typeof(TKSmartCardUserInteraction), Name = "TKSmartCardUserInteractionForPINOperation")]
	interface TKSmartCardUserInteractionForPinOperation
	{
		[Export ("PINCompletion", ArgumentSemantic.Assign)]
		TKSmartCardPinCompletion PinCompletion { get; set; }

		// not clear from the documentation if we can BindAs this to something or not
		[NullAllowed, Export ("PINMessageIndices", ArgumentSemantic.Assign)]
		NSNumber[] PinMessageIndices { get; set; }

		[NullAllowed, Export ("locale", ArgumentSemantic.Assign)]
		NSLocale Locale { get; set; }

		[Export ("resultSW")]
		ushort ResultSW { get; set; }

		[NullAllowed, Export ("resultData", ArgumentSemantic.Assign)]
		NSData ResultData { get; set; }
	}

#if !STABLE_CRYPTOTOKENKIT
	[Experimental ("APL0001")]
#endif
	[iOS (13, 0), Mac (10, 11), MacCatalyst (13, 1), TV (13, 0), Watch (8, 0)]
	[BaseType (typeof(TKSmartCardUserInteractionForPinOperation), Name = "TKSmartCardUserInteractionForSecurePINChange")]
	interface TKSmartCardUserInteractionForSecurePinChange
	{
		[Export ("PINConfirmation", ArgumentSemantic.Assign)]
		TKSmartCardPinConfirmation PinConfirmation { get; set; }
	}

#if !STABLE_CRYPTOTOKENKIT
	[Experimental ("APL0001")]
#endif
	[iOS (13, 0), Mac (10, 11), MacCatalyst (13, 1), TV (13, 0), Watch (8, 0)]
	[BaseType (typeof (TKSmartCardUserInteractionForPinOperation), Name = "TKSmartCardUserInteractionForSecurePINVerification")]
	interface TKSmartCardUserInteractionForSecurePinVerification
	{
	}

#if !STABLE_CRYPTOTOKENKIT
	[Experimental ("APL0001")]
#endif
	[iOS (10, 0), Mac (10, 12), MacCatalyst (13, 1), TV (11, 0), Watch (4, 0)]
	[BaseType (typeof(NSObject), Name = "TKTLVRecord")]
	[DisableDefaultCtor]
	interface TKTlvRecord
	{
		[Export ("tag")]
		ulong Tag { get; }

		[Export ("value")]
		NSData Value { get; }

		[Export ("data")]
		NSData Data { get; }

		[Static]
		[Export ("recordFromData:")]
		[return: NullAllowed]
		TKTlvRecord CreateRecord (NSData data);

		[Static]
		[Export ("sequenceOfRecordsFromData:")]
		[return: NullAllowed]
		TKTlvRecord[] CreateSequenceOfRecords (NSData data);
	}

#if !STABLE_CRYPTOTOKENKIT
	[Experimental ("APL0001")]
#endif
	[iOS (10, 0), Mac (10, 12), MacCatalyst (13, 1), TV (11, 0), Watch (4, 0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface TKToken
	{
		[Export ("initWithTokenDriver:instanceID:")]
		[DesignatedInitializer]
		NativeHandle Constructor (TKTokenDriver tokenDriver, string instanceId);

		[Export ("tokenDriver")]
		TKTokenDriver TokenDriver { get; }

		[Wrap ("WeakDelegate")]
		[NullAllowed]
		ITKTokenDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		[iOS (14, 0), Mac (10, 15), MacCatalyst (14, 0), TV (14, 0), Watch (7, 0)]
		[Export ("configuration")]
		TKTokenConfiguration Configuration { get; }

		[NullAllowed, Export ("keychainContents")]
		TKTokenKeychainContents KeychainContents { get; }
	}

#if !STABLE_CRYPTOTOKENKIT
	[Experimental ("APL0001")]
#endif
	[iOS (10, 0), Mac (10, 12), MacCatalyst (13, 1), TV (11, 0), Watch (4, 0)]
	[BaseType (typeof(NSObject))]
	interface TKTokenAuthOperation : NSSecureCoding
	{
		[Export ("finishWithError:")]
		bool Finish ([NullAllowed] out NSError error);
	}

#if !STABLE_CRYPTOTOKENKIT
	[Experimental ("APL0001")]
#endif
	[iOS (14, 0), Mac (10, 15), MacCatalyst (14, 0), TV (14, 0), Watch (7, 0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface TKTokenConfiguration
	{
		[Export ("instanceID")]
		string InstanceId { get; }

		[NullAllowed, Export ("configurationData", ArgumentSemantic.Copy)]
		NSData ConfigurationData { get; set; }

		[Export ("keychainItems", ArgumentSemantic.Copy)]
		TKTokenKeychainItem[] KeychainItems { get; set; }

		[Export ("keyForObjectID:error:")]
		[return: NullAllowed]
		TKTokenKeychainKey GetKey (NSObject objectId, [NullAllowed] out NSError error);

		[Export ("certificateForObjectID:error:")]
		[return: NullAllowed]
		TKTokenKeychainCertificate GetCertificate (NSObject objectId, [NullAllowed] out NSError error);
	}

#if !STABLE_CRYPTOTOKENKIT
	[Experimental ("APL0001")]
#endif
	[iOS (10, 0), Mac (10, 12), MacCatalyst (13, 1), TV (11, 0), Watch (4, 0)]
	[BaseType (typeof(NSObject))]
	interface TKTokenDriver
	{
		[Wrap ("WeakDelegate")]
		[NullAllowed]
		ITKTokenDriverDelegate Delegate { get; set; }

		[NullAllowed, Export ("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }
	}

#if !STABLE_CRYPTOTOKENKIT
	[Experimental ("APL0001")]
#endif
	[iOS (14, 0), Mac (10, 15), MacCatalyst (14, 0), TV (14, 0), Watch (7, 0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface TKTokenDriverConfiguration
	{
		[Static]
		[Export ("driverConfigurations")]
		NSDictionary<NSString, TKTokenDriverConfiguration> DriverConfigurations { get; }

		[Export ("classID")]
		string ClassId { get; }

		[Export ("tokenConfigurations", ArgumentSemantic.Copy)]
		NSDictionary<NSString, TKTokenConfiguration> TokenConfigurations { get; }

		[Export ("addTokenConfigurationForTokenInstanceID:")]
		TKTokenConfiguration AddTokenConfiguration (string instanceId);

		[Export ("removeTokenConfigurationForTokenInstanceID:")]
		void RemoveTokenConfiguration (string instanceId);
	}

#if !STABLE_CRYPTOTOKENKIT
	[Experimental ("APL0001")]
#endif
	[iOS (10, 0), Mac (10, 12), MacCatalyst (13, 1), TV (11, 0), Watch (4, 0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface TKTokenKeyAlgorithm
	{
		[Export ("isAlgorithm:")]
		bool IsAlgorithm ([BindAs (typeof (SecKeyAlgorithm))] NSString algorithm);

		[Export ("supportsAlgorithm:")]
		bool SupportsAlgorithm ([BindAs (typeof (SecKeyAlgorithm))] NSString algorithm);
	}

#if !STABLE_CRYPTOTOKENKIT
	[Experimental ("APL0001")]
#endif
	[iOS (10, 0), Mac (10, 12), MacCatalyst (13, 1), TV (11, 0), Watch (4, 0)]
	[BaseType (typeof(TKTokenKeychainItem))]
	interface TKTokenKeychainCertificate
	{
		[Export ("initWithCertificate:objectID:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([BindAs (typeof (SecKeyAlgorithm))] NSString certificateRef, NSObject objectId);

		[Export ("data", ArgumentSemantic.Copy)]
		NSData Data { get; }
	}

#if !STABLE_CRYPTOTOKENKIT
	[Experimental ("APL0001")]
#endif
	[iOS (10, 0), Mac (10, 12), MacCatalyst (13, 1), TV (11, 0), Watch (4, 0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface TKTokenKeychainContents
	{
		[Export ("fillWithItems:")]
		void Fill (TKTokenKeychainItem[] items);

		[Export ("items", ArgumentSemantic.Copy)]
		TKTokenKeychainItem[] Items { get; }

		[Export ("keyForObjectID:error:")]
		[return: NullAllowed]
		TKTokenKeychainKey GetKey (NSObject objectId, [NullAllowed] out NSError error);

		[Export ("certificateForObjectID:error:")]
		[return: NullAllowed]
		TKTokenKeychainCertificate GetCertificate (NSObject objectId, [NullAllowed] out NSError error);
	}

#if !STABLE_CRYPTOTOKENKIT
	[Experimental ("APL0001")]
#endif
	[iOS (10, 0), Mac (10, 12), MacCatalyst (13, 1), TV (11, 0), Watch (4, 0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface TKTokenKeychainItem
	{
		[Export ("initWithObjectID:")]
		[DesignatedInitializer]
		NativeHandle Constructor (NSObject objectId);

		[Export ("objectID", ArgumentSemantic.Copy)]
		NSObject ObjectId { get; }

		[NullAllowed, Export ("label")]
		string Label { get; set; }

		[NullAllowed, Export ("constraints", ArgumentSemantic.Copy)]
		NSDictionary<NSNumber, NSObject> Constraints { get; set; }
	}

#if !STABLE_CRYPTOTOKENKIT
	[Experimental ("APL0001")]
#endif
	[iOS (10, 0), Mac (10, 12), MacCatalyst (13, 1), TV (11, 0), Watch (4, 0)]
	[BaseType (typeof(TKTokenKeychainItem))]
	interface TKTokenKeychainKey
	{
		[Export ("initWithCertificate:objectID:")]
		[DesignatedInitializer]
		NativeHandle Constructor (IntPtr certificate, NSObject objectId);

		[Wrap ("this (certificate.GetHandle (), objectId)")]
		NativeHandle Constructor ([NullAllowed] SecCertificate certificate, NSObject objectId);

		[Export ("keyType")]
		string KeyType { get; set; }

		[NullAllowed, Export ("applicationTag", ArgumentSemantic.Copy)]
		NSData ApplicationTag { get; set; }

		[Export ("keySizeInBits")]
		nint KeySizeInBits { get; set; }

		[NullAllowed, Export ("publicKeyData", ArgumentSemantic.Copy)]
		NSData PublicKeyData { get; set; }

		[NullAllowed, Export ("publicKeyHash", ArgumentSemantic.Copy)]
		NSData PublicKeyHash { get; set; }

		[Export ("canDecrypt")]
		bool CanDecrypt { get; set; }

		[Export ("canSign")]
		bool CanSign { get; set; }

		[Export ("canPerformKeyExchange")]
		bool CanPerformKeyExchange { get; set; }

		[Export ("suitableForLogin")]
		bool SuitableForLogin { [Bind ("isSuitableForLogin")] get; set; }
	}

#if !STABLE_CRYPTOTOKENKIT
	[Experimental ("APL0001")]
#endif
	[iOS (10, 0), Mac (10, 12), MacCatalyst (13, 1), TV (11, 0), Watch (4, 0)]
	[BaseType (typeof(NSObject))]
	interface TKTokenKeyExchangeParameters
	{
		[Export ("requestedSize")]
		nint RequestedSize { get; }

		[NullAllowed, Export ("sharedInfo", ArgumentSemantic.Copy)]
		NSData SharedInfo { get; }
	}

#if !STABLE_CRYPTOTOKENKIT
	[Experimental ("APL0001")]
#endif
	[iOS (10, 0), Mac (10, 12), MacCatalyst (13, 1), TV (11, 0), Watch (4, 0)]
	[BaseType (typeof(TKTokenAuthOperation))]
	interface TKTokenPasswordAuthOperation
	{
		[NullAllowed, Export ("password")]
		string Password { get; set; }
	}

#if !STABLE_CRYPTOTOKENKIT
	[Experimental ("APL0001")]
#endif
	[iOS (10, 0), Mac (10, 12), MacCatalyst (13, 1), TV (11, 0), Watch (4, 0)]
	[BaseType (typeof(TKTokenAuthOperation), Name = "TKTokenSmartCardPINAuthOperation")]
	interface TKTokenSmartCardPinAuthOperation
	{
		[Export ("PINFormat", ArgumentSemantic.Assign)]
		TKSmartCardPinFormat PinFormat { get; set; }

		[NullAllowed, Export ("APDUTemplate", ArgumentSemantic.Copy)]
		NSData ApduTemplate { get; set; }

		[Export ("PINByteOffset")]
		nint PinByteOffset { get; set; }

		[NullAllowed, Export ("smartCard", ArgumentSemantic.Assign)]
		TKSmartCard SmartCard { get; set; }

		[NullAllowed, Export ("PIN")]
		string Pin { get; set; }
	}

#if !STABLE_CRYPTOTOKENKIT
	[Experimental ("APL0001")]
#endif
	[iOS (10, 0), Mac (10, 12), MacCatalyst (13, 1), TV (11, 0), Watch (4, 0)]
	[BaseType (typeof(NSObject))]
	interface TKTokenWatcher
	{
		[Export ("tokenIDs")]
		string[] TokenIds { get; }

		// -(instancetype _Nonnull)initWithInsertionHandler:(void (^ _Nonnull)(NSString * _Nonnull))insertionHandler __attribute__((availability(macos, introduced=10.12, deprecated=10.13))) __attribute__((availability(ios, introduced=10.0, deprecated=11.0)));
		[Deprecated (PlatformName.MacOSX, 10, 13, "Use the default constructor followed by a call to the 'SetInsertionHandler' method.")]
		[Deprecated (PlatformName.iOS, 11, 0, "Use the default constructor followed by a call to the 'SetInsertionHandler' method.")]
		[Deprecated (PlatformName.TvOS, 11, 0, "Use the default constructor followed by a call to the 'SetInsertionHandler' method.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, "Use the default constructor followed by a call to the 'SetInsertionHandler' method.")]
		[Deprecated (PlatformName.WatchOS, 4, 0, "Use the default constructor followed by a call to the 'SetInsertionHandler' method.")]
		[Export ("initWithInsertionHandler:")]
		NativeHandle Constructor (Action<NSString> insertionHandler);

		[iOS (11, 0), Mac (10, 13), MacCatalyst (13, 1), TV (11, 0), Watch (4, 0)]
		[Export ("setInsertionHandler:")]
		void SetInsertionHandler (Action<NSString> insertionHandler);

		[Export ("addRemovalHandler:forTokenID:")]
		void AddRemovalHandler (Action<NSString> removalHandler, string tokenId);

		[iOS (15, 0), Mac (12, 0), MacCatalyst (15, 0), TV (15, 0), Watch (8, 0)]
		[Export ("tokenInfoForTokenID:")]
		[return: NullAllowed]
		TKTokenWatcherTokenInfo GetTokenInfo (string tokenId);
	}

#if !STABLE_CRYPTOTOKENKIT
	[Experimental ("APL0001")]
#endif
	[iOS (15, 0), Mac (12, 0), MacCatalyst (15, 0), TV (15, 0), Watch (8, 0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface TKTokenWatcherTokenInfo
	{
		[Export ("tokenID")]
		string TokenId { get; }

		[NullAllowed, Export ("slotName")]
		string SlotName { get; }

		[NullAllowed, Export ("driverName")]
		string DriverName { get; }
	}

}
#endif // NET
