// 
// CGPDFOperatorTable.cs: Implement the managed CGPDFOperatorTable bindings
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//     
// Copyright 2014 Xamarin Inc. All rights reserved.

#nullable enable

using System;
using System.Collections.Generic;
using System.Runtime.Versioning;
using System.Runtime.InteropServices;
using Foundation;
using ObjCRuntime;
using CoreFoundation;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace CoreGraphics {


#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	// CGPDFOperatorTable.h
	public class CGPDFOperatorTable : NativeObject {

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGPDFOperatorTableRef */ IntPtr CGPDFOperatorTableCreate ();

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGPDFOperatorTableRef */ IntPtr CGPDFOperatorTableRetain (/* CGPDFOperatorTableRef */ IntPtr table);

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGPDFOperatorTableRelease (/* CGPDFOperatorTableRef */ IntPtr table);

		// CGPDFOperatorCallback
		delegate void CGPDFOperatorCallback (/* CGPDFScannerRef */ IntPtr scanner, /* void* */ IntPtr info);

		public CGPDFOperatorTable ()
			: base (CGPDFOperatorTableCreate (), true)
		{
		}

#if !NET
		public CGPDFOperatorTable (NativeHandle handle)
			: base (handle, false)
		{
		}
#endif

		[Preserve (Conditional = true)]
		internal CGPDFOperatorTable (NativeHandle handle, bool owns)
			 : base (handle, owns)
		{
		}

		protected internal override void Retain ()
		{
			CGPDFOperatorTableRetain (GetCheckedHandle ());
		}

		protected internal override void Release ()
		{
			CGPDFOperatorTableRelease (GetCheckedHandle ());
		}

#if !NET
		// We need this P/Invoke for legacy AOT scenarios (since we have public API taking a 'Action<IntPtr, IntPtr>', and with this particular native function we can't wrap the delegate)
		// Unfortunately CoreCLR doesn't support generic Action delegates in P/Invokes: https://github.com/dotnet/runtime/issues/32963
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGPDFOperatorTableSetCallback (/* CGPDFOperatorTableRef */ IntPtr table, /* const char */ IntPtr name, /* CGPDFOperatorCallback */ Action<IntPtr, IntPtr>? callback);
#endif

#if NET
		// This signature requires C# 9 (so .NET only).
		// The good part about this signature is that it enforces at compile time that 'callback' is callable from native code in a FullAOT scenario.
		// The bad part is that it's unsafe code (and callers must be in unsafe mode as well).
		[DllImport (Constants.CoreGraphicsLibrary)]
		unsafe extern static void CGPDFOperatorTableSetCallback (/* CGPDFOperatorTableRef */ IntPtr table, /* const char */ IntPtr name, /* CGPDFOperatorCallback */ delegate* unmanaged<IntPtr, IntPtr, void> callback);
#endif

#if MONOMAC && !NET
		// This signature can work everywhere, but we can't enforce at compile time that 'callback' is a delegate to a static function (which is required for FullAOT scenarios),
		// so limit it to non-FullAOT platforms (macOS)
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGPDFOperatorTableSetCallback (/* CGPDFOperatorTableRef */ IntPtr table, /* const char */ IntPtr name, /* CGPDFOperatorCallback */ CGPDFOperatorCallback? callback);

		// this won't work with AOT since the callback must be decorated with [MonoPInvokeCallback]
		public void SetCallback (string name, Action<CGPDFScanner?,object?>? callback)
		{
			if (name is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (name));

			using var namePtr = new TransientString (name);
			if (callback is null)
				CGPDFOperatorTableSetCallback (Handle, namePtr, (CGPDFOperatorCallback?) null);
			else
				CGPDFOperatorTableSetCallback (Handle, namePtr, new CGPDFOperatorCallback (delegate (IntPtr reserved, IntPtr gchandle) {
					// we could do 'new CGPDFScanner (reserved, true)' but we would not get `UserInfo` (managed state) back
					// we could GCHandle `userInfo` but that would (even more) diverge both code bases :(
					var scanner = GetScannerFromInfo (gchandle);
					callback (scanner, scanner?.UserInfo);
				}));
		}

		[Advice ("Use the nicer SetCallback(string,Action<CGPDFScanner,object>) API when possible.")]
#endif

#if !NET
		// this API is ugly - but I do not see a better way with the AOT limitation
		public void SetCallback (string name, Action<IntPtr, IntPtr>? callback)
		{
			if (name is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (name));

			using var namePtr = new TransientString (name);
			CGPDFOperatorTableSetCallback (Handle, namePtr, callback);
		}
#endif // !NET

#if NET
		// this signature requires C# 9 and unsafe code
		public unsafe void SetCallback (string name, delegate* unmanaged<IntPtr, IntPtr, void> callback)
		{
			if (name is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (name));

			using var namePtr = new TransientString (name);
			CGPDFOperatorTableSetCallback (Handle, namePtr, callback);
		}
#endif

		static public CGPDFScanner? GetScannerFromInfo (IntPtr gchandle)
		{
			return GCHandle.FromIntPtr (gchandle).Target as CGPDFScanner;
		}
	}
}
