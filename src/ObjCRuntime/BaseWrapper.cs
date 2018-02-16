// Copyright 2014, Xamarin Inc. All rights reserved.

#if !COREBUILD

using System;
using System.Runtime.InteropServices;
using Foundation;
using CoreFoundation;

namespace ObjCRuntime {

	public abstract class BaseWrapper : INativeObject, IDisposable {

		public BaseWrapper (IntPtr handle, bool owns)
		{
			Handle = handle;
			if (!owns)
				Messaging.void_objc_msgSend (Handle, Selector.GetHandle ("retain"));
		}

		~BaseWrapper ()
		{
			Dispose (false);
		}

		public IntPtr Handle { get; protected set; }

		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		protected virtual void Dispose (bool disposing)
		{
			if (Handle != IntPtr.Zero) {
				Messaging.void_objc_msgSend (Handle, Selector.GetHandle ("release"));
				Handle = IntPtr.Zero;
			}
		}
	}
}

#endif