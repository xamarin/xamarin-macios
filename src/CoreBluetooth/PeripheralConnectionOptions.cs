// 
// PeripheralConnectionOptions.cs: Implements strongly typed access for CBConnectPeripheralOption
//
// Authors: Marek Safar (marek.safar@gmail.com)
//     
// Copyright 2013, Xamarin Inc.
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

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class PeripheralConnectionOptions : DictionaryContainer {
#if !COREBUILD
		public PeripheralConnectionOptions ()
			: base (new NSMutableDictionary ())
		{
		}

		public PeripheralConnectionOptions (NSDictionary dictionary)
			: base (dictionary)
		{
		}

#if !MONOMAC
		public bool? NotifyOnConnection {
			get {
				return GetBoolValue (CBConnectPeripheralOptionsKeys.NotifyOnConnectionKey);
			}

			set {
				if (!value.HasValue) {
					RemoveValue (CBConnectPeripheralOptionsKeys.NotifyOnConnectionKey);
					return;
				}
				SetNumberValue (CBConnectPeripheralOptionsKeys.NotifyOnConnectionKey, value.Value ? 1 : 0);
			}
		}

		public bool? NotifyOnNotification {
			get {
				return GetBoolValue (CBConnectPeripheralOptionsKeys.NotifyOnNotificationKey);

			}

			set {
				if (!value.HasValue) {
					RemoveValue (CBConnectPeripheralOptionsKeys.NotifyOnNotificationKey);
					return;
				}

				SetNumberValue (CBConnectPeripheralOptionsKeys.NotifyOnNotificationKey, value.Value ? 1 : 0);
			}
		}
#endif

		public bool? NotifyOnDisconnection {
			get {
				return GetBoolValue (CBConnectPeripheralOptionsKeys.NotifyOnDisconnectionKey);
			}
			set {
				if (!value.HasValue) {
					RemoveValue (CBConnectPeripheralOptionsKeys.NotifyOnDisconnectionKey);
					return;
				}
				SetNumberValue (CBConnectPeripheralOptionsKeys.NotifyOnDisconnectionKey, value.Value ? 1 : 0);
			}
		}
#endif
	}
}
