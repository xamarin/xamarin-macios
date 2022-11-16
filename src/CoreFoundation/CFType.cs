//
// Copyright 2012-2014 Xamarin
//

#nullable enable

using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

using CoreFoundation;
using Foundation;
using ObjCRuntime;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace CoreFoundation {
#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class CFType : NativeObject, ICFType {
		[DllImport (Constants.CoreFoundationLibrary, EntryPoint = "CFGetTypeID")]
		public static extern nint GetTypeID (IntPtr typeRef);

		[DllImport (Constants.CoreFoundationLibrary)]
		extern static IntPtr CFCopyDescription (IntPtr ptr);

#if NET
		internal CFType ()
#else
		public CFType ()
#endif
		{
		}

		[Preserve (Conditional = true)]
		internal CFType (NativeHandle handle, bool owns)
			: base (handle, owns)
		{
		}

		public string? GetDescription (IntPtr handle)
		{
			if (handle == IntPtr.Zero)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (handle));

			return CFString.FromHandle (CFCopyDescription (handle));
		}

		[DllImport (Constants.CoreFoundationLibrary, EntryPoint = "CFEqual")]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static bool CFEqual (/*CFTypeRef*/ IntPtr cf1, /*CFTypeRef*/ IntPtr cf2);

		public static bool Equal (IntPtr cf1, IntPtr cf2)
		{
			// CFEqual is not happy (but crashy) when it receive null
			if (cf1 == IntPtr.Zero)
				return cf2 == IntPtr.Zero;
			else if (cf2 == IntPtr.Zero)
				return false;
			return CFEqual (cf1, cf2);
		}
	}

	public interface ICFType : INativeObject {
	}
}
