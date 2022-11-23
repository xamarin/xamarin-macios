// 
// CGPDFScanner.cs: Implement the managed CGPDFScanner binding
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//     
// Copyright 2014 Xamarin Inc. All rights reserved.

#nullable enable

using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
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
	public class CGPDFScanner : NativeObject {

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGPDFScannerRef */ IntPtr CGPDFScannerCreate (/* CGPDFContentStreamRef */ IntPtr cs,
			/* CGPDFOperatorTableRef */ IntPtr table, /* void* */ IntPtr info);

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGPDFScannerRef */ IntPtr CGPDFScannerRetain (/* CGPDFScannerRef */ IntPtr scanner);

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGPDFScannerRelease (/* CGPDFScannerRef */ IntPtr scanner);

		object? info;
		GCHandle gch;

		public CGPDFScanner (CGPDFContentStream cs, CGPDFOperatorTable table, object userInfo)
		{
			if (cs is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (cs));
			if (table is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (table));

			info = userInfo;
			gch = GCHandle.Alloc (this);
			InitializeHandle (CGPDFScannerCreate (cs.Handle, table.Handle, GCHandle.ToIntPtr (gch)));
		}

#if !NET
		public CGPDFScanner (NativeHandle handle)
			: base (handle, false)
		{
		}
#endif

		[Preserve (Conditional = true)]
		internal CGPDFScanner (NativeHandle handle, bool owns)
			: base (handle, owns)
		{
		}

		public object? UserInfo {
			get { return info; }
		}

		protected internal override void Retain ()
		{
			CGPDFScannerRetain (GetCheckedHandle ());
		}

		protected internal override void Release ()
		{
			CGPDFScannerRelease (GetCheckedHandle ());
		}

		protected override void Dispose (bool disposing)
		{
			if (gch.IsAllocated)
				gch.Free ();
			base.Dispose (disposing);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGPDFContentStreamRef */ IntPtr CGPDFScannerGetContentStream (/* CGPDFScannerRef */ IntPtr scanner);

		public CGPDFContentStream GetContentStream ()
		{
			return new CGPDFContentStream (CGPDFScannerGetContentStream (Handle), false);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static bool CGPDFScannerScan (/* CGPDFScannerRef */ IntPtr scanner);

		public bool Scan ()
		{
			return CGPDFScannerScan (Handle);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static bool CGPDFScannerPopObject (/* CGPDFScannerRef */ IntPtr scanner, /* CGPDFObjectRef* */ out IntPtr value);

		public bool TryPop (out CGPDFObject? value)
		{
			IntPtr ip;
			if (CGPDFScannerPopObject (Handle, out ip)) {
				value = new CGPDFObject (ip);
				return true;
			} else {
				value = null;
				return false;
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static bool CGPDFScannerPopBoolean (/* CGPDFScannerRef */ IntPtr scanner, /* CGPDFBoolean* */ [MarshalAs (UnmanagedType.I1)] out bool value);

		public bool TryPop (out bool value)
		{
			return CGPDFScannerPopBoolean (Handle, out value);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static bool CGPDFScannerPopInteger (/* CGPDFScannerRef */ IntPtr scanner, /* CGPDFInteger* */ out nint value);

		public bool TryPop (out nint value)
		{
			return CGPDFScannerPopInteger (Handle, out value);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static bool CGPDFScannerPopNumber (/* CGPDFScannerRef */ IntPtr scanner, /* CGPDFReal* */ out nfloat value);

		public bool TryPop (out nfloat value)
		{
			return CGPDFScannerPopNumber (Handle, out value);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static bool CGPDFScannerPopName (/* CGPDFScannerRef */ IntPtr scanner, /* const char** */ out IntPtr value);
		// note: that string is not ours to free

		// not to be confusing with CGPDFScannerPopString (value)
		public bool TryPopName (out string? name)
		{
			IntPtr ip;
			if (CGPDFScannerPopName (Handle, out ip)) {
				name = Marshal.PtrToStringAnsi (ip);
				return true;
			} else {
				name = null;
				return false;
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static bool CGPDFScannerPopString (/* CGPDFScannerRef */ IntPtr scanner, /* CGPDFStringRef* */ out IntPtr value);

		public bool TryPop (out string? value)
		{
			IntPtr ip;
			if (CGPDFScannerPopString (Handle, out ip)) {
				value = CGPDFString.ToString (ip);
				return true;
			} else {
				value = null;
				return false;
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static bool CGPDFScannerPopArray (/* CGPDFScannerRef */ IntPtr scanner, /* CGPDFArrayRef* */ out IntPtr value);

		public bool TryPop (out CGPDFArray? value)
		{
			IntPtr ip;
			if (CGPDFScannerPopArray (Handle, out ip)) {
				value = new CGPDFArray (ip);
				return true;
			} else {
				value = null;
				return false;
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static bool CGPDFScannerPopDictionary (/* CGPDFScannerRef */ IntPtr scanner, /* CGPDFDictionaryRef* */ out IntPtr value);

		public bool TryPop (out CGPDFDictionary? value)
		{
			IntPtr ip;
			if (CGPDFScannerPopDictionary (Handle, out ip)) {
				value = new CGPDFDictionary (ip);
				return true;
			} else {
				value = null;
				return false;
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static bool CGPDFScannerPopStream (/* CGPDFScannerRef */ IntPtr scanner, /* CGPDFStreamRef* */ out IntPtr value);

		public bool TryPop (out CGPDFStream? value)
		{
			IntPtr ip;
			if (CGPDFScannerPopStream (Handle, out ip)) {
				value = new CGPDFStream (ip);
				return true;
			} else {
				value = null;
				return false;
			}
		}

#if NET
		[SupportedOSPlatform ("ios16.0")]
		[SupportedOSPlatform ("maccatalyst16.0")]
		[SupportedOSPlatform ("macos13.0")]
		[SupportedOSPlatform ("tvos16.0")]
#else
		[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0), Watch (9, 0)]
#endif
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGPDFScannerStop (/* CGPDFScannerRef */ IntPtr scanner);

#if NET
		[SupportedOSPlatform ("ios16.0")]
		[SupportedOSPlatform ("maccatalyst16.0")]
		[SupportedOSPlatform ("macos13.0")]
		[SupportedOSPlatform ("tvos16.0")]
#else
		[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0), Watch (9, 0)]
#endif
		public void Stop ()
		{
			CGPDFScannerStop (Handle);
		}
	}
}
