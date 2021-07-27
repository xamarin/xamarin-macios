using CoreFoundation;
using ObjCRuntime;
using Foundation;

using System;

namespace DataDetection {

	[iOS (15,0), MacCatalyst (15,0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface DDDetectedValue
	{
		[Export ("matchedString")]
		string MatchedString { get; }

		[Export ("matchedRange")]
		NSRange MatchedRange { get; }
	}

	[iOS (15,0), MacCatalyst (15,0)]
	[BaseType (typeof (DDDetectedValue))]
	[DisableDefaultCtor]
	interface DDDetectedValueLink
	{
		[Export ("URL")]
		NSUrl Url { get; }
	}

	[iOS (15,0), MacCatalyst (15,0)]
	[BaseType (typeof (DDDetectedValue))]
	[DisableDefaultCtor]
	interface DDDetectedValuePhoneNumber
	{
		[Export ("phoneNumber")]
		string PhoneNumber { get; }

		[NullAllowed, Export ("label")]
		string Label { get; }
	}

	[iOS (15,0), MacCatalyst (15,0)]
	[BaseType (typeof (DDDetectedValue))]
	[DisableDefaultCtor]
	interface DDDetectedValueEmailAddress
	{
		[Export ("emailAddress")]
		string EmailAddress { get; }

		[NullAllowed, Export ("label")]
		string Label { get; }
	}

	[iOS (15,0), MacCatalyst (15,0)]
	[BaseType (typeof (DDDetectedValue))]
	[DisableDefaultCtor]
	interface DDDetectedValuePostalAddress
	{
		[NullAllowed, Export ("street")]
		string Street { get; }

		[NullAllowed, Export ("city")]
		string City { get; }

		[NullAllowed, Export ("state")]
		string State { get; }

		[NullAllowed, Export ("postalCode")]
		string PostalCode { get; }

		[NullAllowed, Export ("country")]
		string Country { get; }
	}

	[iOS (15,0), MacCatalyst (15,0)]
	[BaseType (typeof (DDDetectedValue))]
	[DisableDefaultCtor]
	interface DDDetectedValueCalendarEvent
	{
		[Export ("allDay")]
		bool AllDay { [Bind ("isAllDay")] get; }

		[NullAllowed, Export ("startDate")]
		NSDate StartDate { get; }

		[NullAllowed, Export ("startTimeZone")]
		NSTimeZone StartTimeZone { get; }

		[NullAllowed, Export ("endDate")]
		NSDate EndDate { get; }

		[NullAllowed, Export ("endTimeZone")]
		NSTimeZone EndTimeZone { get; }
	}

	[iOS (15,0), MacCatalyst (15,0)]
	[BaseType (typeof (DDDetectedValue))]
	[DisableDefaultCtor]
	interface DDDetectedValueShipmentTrackingNumber
	{
		[Export ("carrier")]
		string Carrier { get; }

		[Export ("trackingNumber")]
		string TrackingNumber { get; }
	}

	[iOS (15,0), MacCatalyst (15,0)]
	[BaseType (typeof (DDDetectedValue))]
	[DisableDefaultCtor]
	interface DDDetectedValueFlightNumber
	{
		[Export ("airline")]
		string Airline { get; }

		[Export ("flightNumber")]
		string FlightNumber { get; }
	}

	[iOS (15,0), MacCatalyst (15,0)]
	[BaseType (typeof (DDDetectedValue))]
	[DisableDefaultCtor]
	interface DDDetectedValueMoneyAmount
	{
		[Export ("currency")]
		string Currency { get; }

		[Export ("amount")]
		double Amount { get; }
	}
}
