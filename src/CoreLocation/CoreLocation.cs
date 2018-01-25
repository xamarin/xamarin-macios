//
// Authors:
//   Miguel de Icaza (miguel@gnome.org)
//   Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2012-2014 Xamarin Inc
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
using System.Runtime.InteropServices;

using ObjCRuntime;
#if IOS && !COREBUILD && XAMCORE_2_0
using Contacts;
using Intents;
#endif

namespace CoreLocation {

	// CLLocationDegrees -> double -> CLLocation.h

	// CLLocation.h
	[StructLayout (LayoutKind.Sequential)]
	public struct CLLocationCoordinate2D {
		public /* CLLocationDegrees */ double Latitude;
		public /* CLLocationDegrees */ double Longitude;

		public CLLocationCoordinate2D (double latitude, double longitude)
		{
			Latitude = latitude;
			Longitude = longitude;
		}

		[DllImport (Constants.CoreLocationLibrary)]
		static extern /* BOOL */ bool CLLocationCoordinate2DIsValid (CLLocationCoordinate2D cord);
		
		public bool IsValid ()
		{
			return CLLocationCoordinate2DIsValid (this);
		}

		public override string ToString ()
		{
			return $"(Latitude={Latitude}, Longitude={Longitude}";
		}
	}
	
#if !MONOMAC && !COREBUILD && !XAMCORE_2_0
	public partial class CLHeading {

		[Obsolete ("This type is not meant to be created by application code")]
		public CLHeading () : base (IntPtr.Zero)
		{
			// calling ToString, 'description' selector, would crash the application
			IsDirectBinding = GetType () == typeof (CLHeading);
		}
	}

	public partial class CLRegion {

		[Obsolete ("This type is not meant to be created by application code")]
		public CLRegion () : base (IntPtr.Zero)
		{
			// calling ToString, 'description' selector, would crash the application
			IsDirectBinding = GetType () == typeof (CLRegion);
		}
	}

	public partial class CLPlacemark {

		[Obsolete ("This type is not meant to be created by application code")]
		public CLPlacemark () : base (IntPtr.Zero)
		{
			// calling ToString, 'description' selector, or disposing the instance would crash the application
			IsDirectBinding = GetType () == typeof (CLPlacemark);
		}
	}
#endif

#if IOS && !COREBUILD && XAMCORE_2_0 // This code comes from Intents.CLPlacemark_INIntentsAdditions Category
	public partial class CLPlacemark {
		[iOS (10, 0)]
		static public CLPlacemark GetPlacemark (CLLocation location, string name, CNPostalAddress postalAddress)
		{
			return (null as CLPlacemark)._GetPlacemark (location, name, postalAddress);
		}
	}
#endif
}
