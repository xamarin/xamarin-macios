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

#nullable enable

namespace Foundation {

	public partial class NSUrlConnection {
		const string selSendSynchronousRequestReturningResponseError = "sendSynchronousRequest:returningResponse:error:";

		public unsafe static NSData? SendSynchronousRequest (NSUrlRequest request, out NSUrlResponse? response, out NSError? error)
		{
			var responseHandle = IntPtr.Zero;
			var errorHandle = IntPtr.Zero;

			IntPtr res;
			unsafe {
				res = objc_msgSend (
					class_ptr,
					Selector.GetHandle (selSendSynchronousRequestReturningResponseError),
					request.Handle,
					&responseHandle,
					&errorHandle);
			}

			response = (NSUrlResponse?) Runtime.GetNSObject (responseHandle);
			error = (NSError?) Runtime.GetNSObject (errorHandle);

			GC.KeepAlive (request);

			return (NSData?) Runtime.GetNSObject (res);
		}

		[DllImport (Messaging.LIBOBJC_DYLIB, EntryPoint="objc_msgSend")]
		public unsafe extern static IntPtr objc_msgSend (IntPtr receiver, IntPtr selector, IntPtr arg1, IntPtr* arg2, IntPtr* arg3);
	}
}
