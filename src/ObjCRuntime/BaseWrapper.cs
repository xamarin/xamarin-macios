// Copyright 2014, Xamarin Inc. All rights reserved.

#nullable enable

#if !COREBUILD

using System;
using System.Runtime.InteropServices;
using Foundation;
using CoreFoundation;

namespace ObjCRuntime {

	public abstract class BaseWrapper : NativeObject {

		public BaseWrapper (IntPtr handle, bool owns)
			: base (handle, owns)
		{
		}

		protected override void Retain ()
		{
			if (Handle != IntPtr.Zero)
				Messaging.void_objc_msgSend (Handle, Selector.GetHandle ("retain"));
		}

		protected override void Release ()
		{
			if (Handle != IntPtr.Zero)
				Messaging.void_objc_msgSend (Handle, Selector.GetHandle ("release"));
		}
	}
}

#endif
