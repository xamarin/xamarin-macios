//
// AdSupport bindings.
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

using ObjCRuntime;
using Foundation;
using System;

namespace AdSupport {

	/// <summary>Provides a device identifier, only to be used for serving advertisements.</summary>
	///     <remarks>
	///       <para>This class provides a device identifier that is shared by all vendors. This identifier may change over time, for example if the user erases the device, and applications should not cache it.</para>
	///       <para>This identifier should only be used for the purpose of serving advertisements.</para>
	///       <para>Application developers should use the static <see cref="P:AdSupport.ASIdentifierManager.SharedManager" /> in order to gain access to the <see cref="P:AdSupport.ASIdentifierManager.AdvertisingIdentifier" />. Application developers must respect the <see cref="P:AdSupport.ASIdentifierManager.IsAdvertisingTrackingEnabled" /> value.</para>
	///     </remarks>
	///     <related type="externalDocumentation" href="https://developer.apple.com/library/ios/documentation/AdSupport/Reference/ASIdentifierManager_Ref/index.html">Apple documentation for <c>ASIdentifierManager</c></related>
	[MacCatalyst (13, 1)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ASIdentifierManager {

		[Export ("sharedManager")]
		[Static]
		ASIdentifierManager SharedManager { get; }

		/// <summary>Whether advertising tracking is allowed by the user.</summary>
		///         <value>The value is determined by the user in the system Privacy settings.</value>
		///         <remarks>
		///           <para>Unlike most other privacy settings, there is not a system-provided dialog that application's can call to ask user's to switch this value. If an application wishes to switch this value, they must provide the user directions on manipulating the system Privacy settings. (Settings application: Privacy / Advertising dialogs)</para>
		///         </remarks>
		[Deprecated (PlatformName.iOS, 14, 0, message: "Use 'ATTrackingManager.AppTrackingTransparency' instead.")]
		[Deprecated (PlatformName.MacOSX, 11, 0, message: "Use 'ATTrackingManager.AppTrackingTransparency' instead.")]
		[Deprecated (PlatformName.TvOS, 14, 0, message: "Use 'ATTrackingManager.AppTrackingTransparency' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 14, 0, message: "Use 'ATTrackingManager.AppTrackingTransparency' instead.")]
		[Export ("advertisingTrackingEnabled")]
		bool IsAdvertisingTrackingEnabled { [Bind ("isAdvertisingTrackingEnabled")] get; }

		[Export ("advertisingIdentifier")]
		NSUuid AdvertisingIdentifier { get; }

		[NoTV]
		[NoiOS]
		[NoMac] // unclear when that was changed (xcode 12 GM allowed it)
		[NoMacCatalyst]
		[Export ("clearAdvertisingIdentifier")]
		void ClearAdvertisingIdentifier ();
	}
}
