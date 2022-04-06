// Copyright 2014, Xamarin Inc. All rights reserved.

#nullable enable

#if !COREBUILD

using System;
using System.Runtime.InteropServices;
using Foundation;
using CoreFoundation;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace ObjCRuntime {

	public abstract class BaseWrapper : NativeObject {

#if NET
		protected BaseWrapper (NativeHandle handle, bool owns)
#else
		public BaseWrapper (NativeHandle handle, bool owns)
#endif
			: base (handle, owns)
		{
		}

		protected internal override void Retain ()
		{
			if (Handle != IntPtr.Zero)
				Messaging.void_objc_msgSend (Handle, Selector.GetHandle ("retain"));
		}

		protected internal override void Release ()
		{
			if (Handle != IntPtr.Zero)
				Messaging.void_objc_msgSend (Handle, Selector.GetHandle ("release"));
		}
	}
}

#endif
