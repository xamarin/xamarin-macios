//
// NSUrlConnection.cs:
// Author:
//   Miguel de Icaza
// Copyright 2011, 2012 Xamarin Inc
//

using System;
using System.Reflection;
using System.Collections;
using System.Runtime.InteropServices;

using ObjCRuntime;

namespace Foundation {

	public partial class NSUrlConnection {
		const string selSendSynchronousRequestReturningResponseError = "sendSynchronousRequest:returningResponse:error:";

		public unsafe static NSData SendSynchronousRequest (NSUrlRequest request, out NSUrlResponse response, out NSError error)
		{
			IntPtr responseStorage = IntPtr.Zero;
			IntPtr errorStorage = IntPtr.Zero;

			void* resp = &responseStorage;
			void* errp = &errorStorage;
			IntPtr rhandle = (IntPtr) resp;
			IntPtr ehandle = (IntPtr) errp;

#if NET
			var res = Messaging.NativeHandle_objc_msgSend_NativeHandle_NativeHandle_NativeHandle (
#else
			var res = Messaging.IntPtr_objc_msgSend_IntPtr_IntPtr_IntPtr (
#endif
				class_ptr,
				Selector.GetHandle (selSendSynchronousRequestReturningResponseError),
				request.Handle,
				rhandle,
				ehandle);

			if (responseStorage != IntPtr.Zero)
				response = (NSUrlResponse) Runtime.GetNSObject (responseStorage);
			else
				response = null;

			if (errorStorage != IntPtr.Zero)
				error = (NSError) Runtime.GetNSObject (errorStorage);
			else
				error = null;

			return (NSData) Runtime.GetNSObject (res);
		}
	}
}
