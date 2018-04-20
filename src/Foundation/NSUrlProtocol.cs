//
// NSUrlProtocol support
//
// Author:
//   Rolf Bjarne Kvinge
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
using ObjCRuntime;

namespace Foundation {
	public abstract partial class NSUrlProtocol : NSObject {
#if !XAMCORE_2_0
		NSUrlProtocolClient client;

		public NSUrlProtocolClient Client {
			get {
				if (client == null) {
					client = new NSUrlProtocolClient (WeakClient.Handle);
					MarkDirty ();
				}
				return client;
			}
		}
#endif
#if XAMCORE_2_0 && MONOMAC && !XAMCORE_4_0
		[BindingImpl (BindingImplOptions.Optimizable)]
		[Obsolete ("Use the overload that takes an 'INSUrlProtocolClient' instead.")]
		public NSUrlProtocol (NSUrlRequest request, NSCachedUrlResponse cachedResponse, NSUrlProtocolClient client)
			: base (NSObjectFlag.Empty)
		{
			if (request == null)
				throw new ArgumentNullException ("request");
			if (client == null)
				throw new ArgumentNullException ("client");
			if (IsDirectBinding) {
				InitializeHandle (global::ObjCRuntime.Messaging.IntPtr_objc_msgSend_IntPtr_IntPtr_IntPtr (this.Handle, selInitWithRequest_CachedResponse_Client_Handle, request.Handle, cachedResponse == null ? IntPtr.Zero : cachedResponse.Handle, client.Handle), "initWithRequest:cachedResponse:client:");
			} else {
				InitializeHandle (global::ObjCRuntime.Messaging.IntPtr_objc_msgSendSuper_IntPtr_IntPtr_IntPtr (this.SuperHandle, selInitWithRequest_CachedResponse_Client_Handle, request.Handle, cachedResponse == null ? IntPtr.Zero : cachedResponse.Handle, client.Handle), "initWithRequest:cachedResponse:client:");
			}
		}

		[Obsolete ("Use 'Client' instead.")]
		public virtual NSObject WeakClient {
			get {
				var client = Client;
				if (client == null)
					return null;
				var nsclient = client as NSObject;
				if (nsclient != null)
					return nsclient;
				return Runtime.GetNSObject (client.Handle);
			}
		}
#endif
	}
}
