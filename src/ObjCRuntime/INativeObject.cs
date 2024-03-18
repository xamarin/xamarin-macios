#nullable enable

using System;
using System.Runtime.CompilerServices;
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

#if NET
		// The method will be implemented via custom linker step if the managed static registrar is used
		// for classes which have an (NativeHandle, bool) or (IntPtr, bool) constructor.
		// This method will be made public when the managed static registrar is used.
		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static virtual INativeObject? _Xamarin_ConstructINativeObject (NativeHandle handle, bool owns) => null;
#endif
	}

#if !COREBUILD
	public static class NativeObjectExtensions {

		// help to avoid the (too common pattern)
		// 	var p = x is null ? IntPtr.Zero : x.Handle;
		static public NativeHandle GetHandle (this INativeObject? self)
		{
			return self is null ? NativeHandle.Zero : self.Handle;
		}

		static public NativeHandle GetNonNullHandle (this INativeObject self, string argumentName)
		{
			if (self is null)
				ThrowHelper.ThrowArgumentNullException (argumentName);
			if (self.Handle == NativeHandle.Zero)
				ThrowHelper.ThrowObjectDisposedException (self);
			return self.Handle;
		}

#if !NET
		public static NativeHandle GetCheckedHandle (this INativeObject self)
		{
			var h = self.Handle;
			if (h == NativeHandle.Zero)
				ObjCRuntime.ThrowHelper.ThrowObjectDisposedException (self);

			return h;
		}
#endif
	}
#endif
}
