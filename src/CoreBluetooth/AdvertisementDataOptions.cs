// 
// AdvertisementDataOptions.cs: Implements strongly typed access for CBAdvertisementData
//
// Authors: Marek Safar (marek.safar@gmail.com)
//     
// Copyright 2012, Xamarin Inc.
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

using Foundation;
using CoreFoundation;
using ObjCRuntime;

#nullable enable

namespace CoreBluetooth {

	//
	// It's intentionally not called AdvertisementDataOptions because different options
	// are valid in different contexts
	//
#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class StartAdvertisingOptions : DictionaryContainer {
#if !COREBUILD
		public StartAdvertisingOptions ()
			: base (new NSMutableDictionary ())
		{
		}

		public StartAdvertisingOptions (NSDictionary dictionary)
			: base (dictionary)
		{
		}

		/// <summary>Represents the local name of a peripheral</summary>
		///         <value>
		///         </value>
		///         <remarks>The property uses constant CBAdvertisementDataLocalNameKey value to access the underlying dictionary.</remarks>
		public string? LocalName {
			set {
				SetStringValue (CBAdvertisement.DataLocalNameKey, value);
			}
			get {
				return GetStringValue (CBAdvertisement.DataLocalNameKey);
			}
		}

		/// <summary>One or more <see cref="T:CoreBluetooth.CBUUID" /> objects.</summary>
		///         <value>
		///         </value>
		///         <remarks>The property uses constant CBAdvertisementDataServiceUUIDsKey value to access the underlying dictionary.</remarks>
		public CBUUID []? ServicesUUID {
			get {
				return GetArray<CBUUID> (CBAdvertisement.DataServiceUUIDsKey);
			}
			set {
				SetArrayValue (CBAdvertisement.DataServiceUUIDsKey, value);
			}
		}
#endif
	}
}
