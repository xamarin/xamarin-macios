using XamCore.Foundation;
using XamCore.ObjCRuntime;
using System;

namespace XamCore.CoreTelephony {
	[Since (4,0)]
	[BaseType (typeof (NSObject))]
	interface CTCall {
		[Export ("callID")]
		string CallID { get;  }

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
	[Since (7,0)]
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
	[Since (4,0)]
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

		[Since (7,0), Export ("currentRadioAccessTechnology")]
		NSString CurrentRadioAccessTechnology { get; }
	}

#if !XAMCORE_2_0
	delegate void CTCallEventHandler (CTCall call);
#endif

	[BaseType (typeof (NSObject))]
	[Since (4,0)]
	interface CTCallCenter {
		[NullAllowed] // by default this property is null
		[Export ("callEventHandler")]
#if XAMCORE_2_0
		Action<CTCall> CallEventHandler { get; set; }
#else
		CTCallEventHandler CallEventHandler { get; set; }
#endif

		[Export ("currentCalls")]
		NSSet CurrentCalls { get; }

	}

	[BaseType (typeof (NSObject))]
	[Since (4,0)]
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
	[Since (7,0)]
	public partial interface CTSubscriber {
		[Since (7,0), Export ("carrierToken")]
		NSData CarrierToken { get; }
	}

#if !XAMCORE_2_0
	public delegate void SimAuthenticationCallback (NSDictionary dictionary);
#endif

	[Since (6,0), BaseType (typeof (NSObject))]
	public partial interface CTSubscriberInfo {
		[Static]
		[Export ("subscriber")]
		CTSubscriber Subscriber { get; }
	}
}
