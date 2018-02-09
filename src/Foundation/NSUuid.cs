//
// NSUuid.cs: support code for the NSUUID binding
//
// Authors:
//    MIguel de Icaza (miguel@xamarin.com)
//
// Copyright 2012-2013 Xamarin Inc
//
using System;
using System.Runtime.InteropServices;
using ObjCRuntime;

namespace Foundation {
	partial class NSUuid {
		static unsafe IntPtr GetIntPtr (byte [] bytes)
		{
			if (bytes == null)
				throw new ArgumentNullException ("bytes");
			if (bytes.Length < 16)
				throw new ArgumentException ("length must be at least 16 bytes");
			
			IntPtr ret;
			fixed (byte *p = &bytes [0]){
				ret = (IntPtr) p;
			}
			return ret;
		}

		unsafe public NSUuid (byte [] bytes) : base (NSObjectFlag.Empty)
		{
			IntPtr ptr = GetIntPtr (bytes);

			if (IsDirectBinding) {
				Handle = Messaging.IntPtr_objc_msgSend_IntPtr (this.Handle, Selector.GetHandle ("initWithUUIDBytes:"), ptr);
			} else {
				Handle = Messaging.IntPtr_objc_msgSendSuper_IntPtr (this.SuperHandle, Selector.GetHandle ("initWithUUIDBytes:"), ptr);
			}
		}

		public byte [] GetBytes ()
		{
			byte [] ret = new byte [16];
			
			IntPtr buf = Marshal.AllocHGlobal (16);
			GetUuidBytes (buf);
			Marshal.Copy (buf, ret, 0, 16);
			Marshal.FreeHGlobal (buf);

			return ret;
		}
	}
}
