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

			void *resp = &responseStorage;
			void *errp = &errorStorage;
			IntPtr rhandle = (IntPtr) resp;
			IntPtr ehandle = (IntPtr) errp;
			
			var res = Messaging.IntPtr_objc_msgSend_IntPtr_IntPtr_IntPtr (
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
#if !XAMCORE_2_0
		[Advice ("Use 'Schedule (NSRunLoop, NSString)' instead.")]
		public virtual void Schedule (NSRunLoop aRunLoop, string forMode)
		{
			if (aRunLoop == null)
				throw new ArgumentNullException ("aRunLoop");
			if (forMode == null)
				throw new ArgumentNullException ("forMode");
			var nsforMode = NSString.CreateNative (forMode);
			
			if (IsDirectBinding) {
				Messaging.void_objc_msgSend_IntPtr_IntPtr (this.Handle, Selector.GetHandle ("scheduleInRunLoop:forMode:"), aRunLoop.Handle, nsforMode);
			} else {
				Messaging.void_objc_msgSendSuper_IntPtr_IntPtr (this.SuperHandle, Selector.GetHandle ("scheduleInRunLoop:forMode:"), aRunLoop.Handle, nsforMode);
			}
			NSString.ReleaseNative (nsforMode);
		}

		[Advice ("Use 'Unschedule (NSRunLoop, NSString)' instead.")]
		public virtual void Unschedule (NSRunLoop aRunLoop, string forMode)
		{
			if (aRunLoop == null)
				throw new ArgumentNullException ("aRunLoop");
			if (forMode == null)
				throw new ArgumentNullException ("forMode");
			var nsforMode = NSString.CreateNative (forMode);
			
			if (IsDirectBinding) {
				Messaging.void_objc_msgSend_IntPtr_IntPtr (this.Handle, Selector.GetHandle ("unscheduleFromRunLoop:forMode:"), aRunLoop.Handle, nsforMode);
			} else {
				Messaging.void_objc_msgSendSuper_IntPtr_IntPtr (this.SuperHandle, Selector.GetHandle ("unscheduleFromRunLoop:forMode:"), aRunLoop.Handle, nsforMode);
			}
			NSString.ReleaseNative (nsforMode);
		}
#endif
	}
}
