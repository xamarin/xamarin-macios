using Foundation;
using ObjCRuntime;
using System;

namespace CoreTelephony {
	[BaseType (typeof (NSObject))]
	interface CTCall {
		[Availability (Deprecated = Platform.iOS_10_0, Message = "Use 'CallKit' instead.")]
		[Export ("callID")]
		string CallID { get;  }

		[Availability (Deprecated = Platform.iOS_10_0, Message = "Use 'CallKit' instead.")]
		[Export ("callState")]
		string CallState { get; }

	}

	[iOS (9,0)]
	[BaseType (typeof (NSObject))]
	interface CTCellularData {
		[NullAllowed, Export ("cellularDataRestrictionDidUpdateNotifier", ArgumentSemantic.Copy)]
		Action<CTCellularDataRestrictedState> RestrictionDidUpdateNotifier { get; set; }
	
		[Export ("restrictedState")]
		CTCellularDataRestrictedState RestrictedState { get; }
	}
	

#if !XAMCORE_2_0
	delegate void CTCarrierEventHandler (CTCarrier call);
#endif

	[Static]
	[iOS (7,0)]
	interface CTRadioAccessTechnology {
		[Field ("CTRadioAccessTechnologyGPRS")]
		NSString GPRS { get; }

		[Field ("CTRadioAccessTechnologyEdge")]
		NSString Edge { get; }

		[Field ("CTRadioAccessTechnologyWCDMA")]
		NSString WCDMA { get; }

		[Field ("CTRadioAccessTechnologyHSDPA")]
		NSString HSDPA { get; }

		[Field ("CTRadioAccessTechnologyHSUPA")]
		NSString HSUPA { get; }

		[Field ("CTRadioAccessTechnologyCDMA1x")]
		NSString CDMA1x { get; }

		[Field ("CTRadioAccessTechnologyCDMAEVDORev0")]
		NSString CDMAEVDORev0 { get; }

		[Field ("CTRadioAccessTechnologyCDMAEVDORevA")]
		NSString CDMAEVDORevA { get; }

		[Field ("CTRadioAccessTechnologyCDMAEVDORevB")]
		NSString CDMAEVDORevB { get; }

		[Field ("CTRadioAccessTechnologyeHRPD")]
		NSString EHRPD { get; }

		[Field ("CTRadioAccessTechnologyLTE")]
		NSString LTE { get; }
	}

	[BaseType (typeof (NSObject))]
	interface CTTelephonyNetworkInfo {
		[Export ("subscriberCellularProvider", ArgumentSemantic.Retain)]
		CTCarrier SubscriberCellularProvider { get; }

		[NullAllowed] // by default this property is null
		[Export ("subscriberCellularProviderDidUpdateNotifier")]
#if XAMCORE_2_0
		Action<CTCarrier> CellularProviderUpdatedEventHandler { get; set; }
#else
		CTCarrierEventHandler CellularProviderUpdatedEventHandler { get; set; }
#endif

		[iOS (7,0), Export ("currentRadioAccessTechnology")]
		NSString CurrentRadioAccessTechnology { get; }
	}

#if !XAMCORE_2_0
	delegate void CTCallEventHandler (CTCall call);
#endif

	[Deprecated (PlatformName.iOS, 10, 0, message: "Replaced by 'CXCallObserver' from 'CallKit'.")]
	[BaseType (typeof (NSObject))]
	interface CTCallCenter {
		[Availability (Deprecated = Platform.iOS_10_0, Message = "Use 'CallKit' instead.")]
		[NullAllowed] // by default this property is null
		[Export ("callEventHandler")]
#if XAMCORE_2_0
		Action<CTCall> CallEventHandler { get; set; }
#else
		CTCallEventHandler CallEventHandler { get; set; }
#endif

		[Availability (Deprecated = Platform.iOS_10_0, Message = "Use 'CallKit' instead.")]
		[Export ("currentCalls")]
		NSSet CurrentCalls { get; }

	}

	[BaseType (typeof (NSObject))]
	interface CTCarrier {
		[Export ("mobileCountryCode")]
		string MobileCountryCode { get;  }

		[Export ("mobileNetworkCode")]
		string MobileNetworkCode { get;  }

		[Export ("isoCountryCode")]
		string IsoCountryCode { get;  }

		[Export ("allowsVOIP")]
		bool AllowsVoip { get;  }

		[Export ("carrierName")]
		string CarrierName { get; }
	}

	[BaseType (typeof (NSObject))]
	[iOS (7,0)]
	partial interface CTSubscriber {
		[iOS (7,0), Export ("carrierToken")]
		NSData CarrierToken { get; }
	}

#if !XAMCORE_2_0
	delegate void SimAuthenticationCallback (NSDictionary dictionary);
#endif

	[iOS (6,0), BaseType (typeof (NSObject))]
	partial interface CTSubscriberInfo {
		[Static]
		[Export ("subscriber")]
		CTSubscriber Subscriber { get; }
	}
}
