//
// Authors:
//   Miguel de Icaza (miguel@gnome.org)
//   Aaron Bockover (abock@xamarin.com)
//
// Copyright 2011-2014 Xamarin Inc
//
// The class can be either constructed from a string (from user code)
// or from a handle (from iphone-sharp.dll internal calls).  This
// delays the creation of the actual managed string until actually
// required
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
using System;
using ObjCRuntime;

#nullable enable

namespace CoreLocation {

	// NSInteger -> CLError.h
	/// <summary>Errors returned by the CLLocationManager.</summary>
	[Native]
	public enum CLError : long {
		/// <summary>The location could not be determined.</summary>
		LocationUnknown = 0,
		/// <summary>Access to the location service was denied by the user.</summary>
		Denied,
		/// <summary>The network is unavailable.</summary>
		Network,
		/// <summary>The heading could not be determined.</summary>
		HeadingFailure,
		/// <summary>Region monitoring was disallowed by the user.</summary>
		RegionMonitoringDenied,
		/// <summary>Region monitoring failed.</summary>
		RegionMonitoringFailure,
		/// <summary>Region monitoring could not be configured immediately.</summary>
		RegionMonitoringSetupDelayed,

		// ios5 osx10.8
		/// <summary>Although region monitoring is available, events may be delayed.</summary>
		RegionMonitoringResponseDelayed,
		// ios5 osx10.7
		/// <summary>The geocoding was unsuccessful.</summary>
		GeocodeFoundNoResult,
		// ios5 osx10.8
		/// <summary>The geocoding was only partially successful.</summary>
		GeocodeFoundPartialResult,
		// ios5 osx10.7
		/// <summary>The geocoding request was canceled.</summary>
		GeocodeCanceled,

		// ios6
		/// <summary>The attempt to switch to deferred mode failed. Application developers may try again on devices that have a GPS device.</summary>
		DeferredFailed,
		/// <summary>The <see cref="T:CoreLocation.CLLocationManager" /> did not enter deferred mode because location updates were already paused or disabled.</summary>
		DeferredNotUpdatingLocation,
		/// <summary>Deferred mode is not available for the requested accuracy.</summary>
		///         <remarks>
		///           <para>For deferred mode, the accuracy must be <see cref="P:CoreLocation.CLLocation.AccuracyBest" /> or <see cref="P:CoreLocation.CLLocation.AccurracyBestForNavigation" />.</para>
		///         </remarks>
		DeferredAccuracyTooLow,
		/// <summary>Deferred mode does not allow distance filters. The <see cref="P:CoreLocation.CLLocationManager.DistanceFilter" /> must be set to <see cref="P:CoreLocation.CLLocationDistance.FilterNone" />.</summary>
		DeferredDistanceFiltered,
		/// <summary>The application's request for deferred location notices has been canceled.</summary>
		DeferredCanceled,

		// ios7
		/// <summary>An error occurred during ranging.</summary>
		RangingFailure,
		/// <summary>Ranging is not available.</summary>
		RangingUnavailable,

		// ios14
		PromptDeclined = 18,

		// ios16
		HistoricalLocationError,
	}

	// untyped enum -> CLLocationManager.h
	/// <summary>An enumeration whose values represent the device's physical orientation.</summary>
	public enum CLDeviceOrientation : uint {
		/// <summary>The device's orientation is unavailable.</summary>
		Unknown,
		/// <summary>The device is in an upright position, with the home button towards the ground.</summary>
		Portrait,
		/// <summary>The device is in an upright position, with the home button towards the sky.</summary>
		PortraitUpsideDown,
		/// <summary>The device is in an upright position, with the home button to the right.</summary>
		LandscapeLeft,
		/// <summary>The device is in an upright position, with the home button to the left.</summary>
		LandscapeRight,
		/// <summary>The device is parallel to the ground and the face is pointing towards the sky.</summary>
		FaceUp,
		/// <summary>The device is parallel to the ground and the face is pointing towards the ground.</summary>
		FaceDown
	}

	// untyped enum -> CLLocationManager.h
	/// <summary>An enumeration whose values specify the current status of authorization to use location services.</summary>
	public enum CLAuthorizationStatus : uint {
		/// <summary>The user has not yet chosen whether to allow location services.</summary>
		NotDetermined = 0,
		/// <summary>Location services are not available and the user cannot change the authorization (e.g., constrained by parental controls).</summary>
		Restricted,
		/// <summary>The app is not allowed to use location services.</summary>
		Denied,

		/// <summary>Developers should not use this deprecated field. Developers should use 'AuthorizedAlways' instead.</summary>
		[Deprecated (PlatformName.iOS, 8, 0, message: "Use 'AuthorizedAlways' instead.")]
		[Deprecated (PlatformName.MacCatalyst, 13, 1, message: "Use 'AuthorizedAlways' instead.")]
		Authorized,
		/// <summary>To be added.</summary>
		AuthorizedAlways = Authorized,
		/// <summary>To be added.</summary>
		[Deprecated (PlatformName.MacOSX, 13, 0)]
		AuthorizedWhenInUse,
	}

	// NSInteger -> CLLocationManager.h
	/// <summary>An enumeration whose values specify different types of activity.</summary>
	///     <remarks>
	///       <para>By assigning <see cref="P:CoreLocation.CLLocationManager.ActivityType" />, the system can make intelligent choices regarding location update frequency vs. power consumption. </para>
	///     </remarks>
	[Native]
	public enum CLActivityType : long {
		/// <summary>Indicates that the activity type is unknown.</summary>
		Other = 1,
		/// <summary>Indicates that the app is engaged in navigating an automobile. (Use <see cref="F:CoreLocation.CLActivityType.OtherNavigation" /> for other vehicle types.)</summary>
		AutomotiveNavigation,
		/// <summary>Indicates fitness and all walking activities.</summary>
		Fitness,
		/// <summary>Indicates that the app is involved in navigation, but not in a car.</summary>
		///         <remarks>
		///           <para>This value should be used for tracking motion in, e.g., trains, planes, and boats.</para>
		///         </remarks>
		OtherNavigation,
		/// <summary>To be added.</summary>
		[MacCatalyst (13, 1)]
		Airborne,
	}

	[Native]
	[TV (14, 0), iOS (14, 0)]
	[MacCatalyst (14, 0)]
	public enum CLAccuracyAuthorization : long {
		FullAccuracy,
		ReducedAccuracy,
	}

}
