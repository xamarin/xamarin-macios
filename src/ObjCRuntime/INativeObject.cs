#nullable enable

using System;
using Foundation;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace ObjCRuntime {

	public interface INativeObject {
#if !COREBUILD
		NativeHandle Handle {
			get;
		}
#endif
	}

#if !COREBUILD
	public static class NativeObjectExtensions {

		// help to avoid the (too common pattern)
		// 	var p = x == null ? IntPtr.Zero : x.Handle;
		static public NativeHandle GetHandle (this INativeObject? self)
		{
			return self is null ? NativeHandle.Zero : self.Handle;
		}

		static public NativeHandle GetNonNullHandle (this INativeObject self, string argumentName)
		{
			if (self is null)
				ThrowHelper.ThrowArgumentNullException (argumentName);
			if (self.Handle == IntPtr.Zero)
				ThrowHelper.ThrowObjectDisposedException (self);
			return self.Handle;
		}

		public static IntPtr GetCheckedHandle (this INativeObject self)
		{
			if (self.Handle == IntPtr.Zero)
				ObjCRuntime.ThrowHelper.ThrowObjectDisposedException (self);

			return self.Handle;
		}
	}
#endif
}
