using System;
using UIKit;
using Foundation;
using ObjCRuntime;
using CoreGraphics;
using CoreLocation;
using Contacts;

namespace Bug57531 {

	[Category]
	[BaseType (typeof (CLGeocoder))]
	interface CLGeocoder_ContactsAdditions {

		[Export ("geocodePostalAddress:completionHandler:")]
		[Async]
		void GeocodePostalAddress (CNPostalAddress postalAddress, CLGeocodeCompletionHandler completionHandler);

		[Export ("geocodePostalAddress:preferredLocale:completionHandler:")]
		[Async]
		void GeocodePostalAddress (CNPostalAddress postalAddress, [NullAllowed] NSLocale locale, CLGeocodeCompletionHandler completionHandler);
	}
}
