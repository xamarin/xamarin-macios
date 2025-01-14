// 
// CGPDFScanner.cs: Implement the managed CGPDFScanner binding
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//     
// Copyright 2014 Xamarin Inc. All rights reserved.

#nullable enable

using System;
using System.Runtime.CompilerServices;
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
		extern static byte CGPDFScannerScan (/* CGPDFScannerRef */ IntPtr scanner);

		public bool Scan ()
		{
			return CGPDFScannerScan (Handle) != 0;
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		unsafe extern static byte CGPDFScannerPopObject (/* CGPDFScannerRef */ IntPtr scanner, /* CGPDFObjectRef* */ IntPtr* value);

		public bool TryPop (out CGPDFObject? value)
		{
			IntPtr ip;
			bool rv;
			unsafe {
				rv = CGPDFScannerPopObject (Handle, &ip) != 0;
			}
			value = rv ? new CGPDFObject (ip) : null;
			return rv;
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		unsafe extern static byte CGPDFScannerPopBoolean (/* CGPDFScannerRef */ IntPtr scanner, /* CGPDFBoolean* */ byte* value);

		public unsafe bool TryPop (out bool value)
		{
			byte bytevalue;
			var rv = CGPDFScannerPopBoolean (Handle, &bytevalue) != 0;
			value = bytevalue != 0;
			return rv;
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		unsafe extern static byte CGPDFScannerPopInteger (/* CGPDFScannerRef */ IntPtr scanner, /* CGPDFInteger* */ nint* value);

		public bool TryPop (out nint value)
		{
			value = default;
			unsafe {
				return CGPDFScannerPopInteger (Handle, (nint*) Unsafe.AsPointer<nint> (ref value)) != 0;
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		unsafe extern static byte CGPDFScannerPopNumber (/* CGPDFScannerRef */ IntPtr scanner, /* CGPDFReal* */ nfloat* value);

		public bool TryPop (out nfloat value)
		{
			value = default;
			unsafe {
				return CGPDFScannerPopNumber (Handle, (nfloat*) Unsafe.AsPointer<nfloat> (ref value)) != 0;
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		unsafe extern static byte CGPDFScannerPopName (/* CGPDFScannerRef */ IntPtr scanner, /* const char** */ IntPtr* value);
		// note: that string is not ours to free

		// not to be confusing with CGPDFScannerPopString (value)
		public bool TryPopName (out string? name)
		{
			IntPtr ip;
			bool rv;
			unsafe {
				rv = CGPDFScannerPopName (Handle, &ip) != 0;
			}
			name = rv ? Marshal.PtrToStringAnsi (ip) : null;
			return rv;
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		unsafe extern static byte CGPDFScannerPopString (/* CGPDFScannerRef */ IntPtr scanner, /* CGPDFStringRef* */ IntPtr* value);

		public bool TryPop (out string? value)
		{
			IntPtr ip;
			bool rv;
			unsafe {
				rv = CGPDFScannerPopString (Handle, &ip) != 0;
			}
			value = rv ? CGPDFString.ToString (ip) : null;
			return rv;
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		unsafe extern static byte CGPDFScannerPopArray (/* CGPDFScannerRef */ IntPtr scanner, /* CGPDFArrayRef* */ IntPtr* value);

		public bool TryPop (out CGPDFArray? value)
		{
			IntPtr ip;
			bool rv;
			unsafe {
				rv = CGPDFScannerPopArray (Handle, &ip) != 0;
			}
			value = rv ? new CGPDFArray (ip) : null;
			return rv;
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		unsafe extern static byte CGPDFScannerPopDictionary (/* CGPDFScannerRef */ IntPtr scanner, /* CGPDFDictionaryRef* */ IntPtr* value);

		public bool TryPop (out CGPDFDictionary? value)
		{
			IntPtr ip;
			bool rv;
			unsafe {
				rv = CGPDFScannerPopDictionary (Handle, &ip) != 0;
			}
			value = rv ? new CGPDFDictionary (ip) : null;
			return rv;
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		unsafe extern static byte CGPDFScannerPopStream (/* CGPDFScannerRef */ IntPtr scanner, /* CGPDFStreamRef* */ IntPtr* value);

		public bool TryPop (out CGPDFStream? value)
		{
			IntPtr ip;
			bool rv;
			unsafe {
				rv = CGPDFScannerPopStream (Handle, &ip) != 0;
			}
			value = rv ? new CGPDFStream (ip) : null;
			return rv;
		}

#if NET
		[SupportedOSPlatform ("ios16.0")]
		[SupportedOSPlatform ("maccatalyst16.0")]
		[SupportedOSPlatform ("macos13.0")]
		[SupportedOSPlatform ("tvos16.0")]
#else
		[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
#endif
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGPDFScannerStop (/* CGPDFScannerRef */ IntPtr scanner);

#if NET
		[SupportedOSPlatform ("ios16.0")]
		[SupportedOSPlatform ("maccatalyst16.0")]
		[SupportedOSPlatform ("macos13.0")]
		[SupportedOSPlatform ("tvos16.0")]
#else
		[Mac (13, 0), iOS (16, 0), TV (16, 0), MacCatalyst (16, 0)]
#endif
		public void Stop ()
		{
			CGPDFScannerStop (Handle);
		}
	}
}
