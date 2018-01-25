// 
// CGPDFScanner.cs: Implement the managed CGPDFScanner binding
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//     
// Copyright 2014 Xamarin Inc. All rights reserved.

using System;
using System.Runtime.InteropServices;
using System.Text;
using Foundation;
using ObjCRuntime;
using CoreFoundation;

namespace CoreGraphics {

	public class CGPDFScanner : INativeObject, IDisposable {

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGPDFScannerRef */ IntPtr CGPDFScannerCreate (/* CGPDFContentStreamRef */ IntPtr cs,
			/* CGPDFOperatorTableRef */ IntPtr table, /* void* */ IntPtr info);

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGPDFScannerRef */ IntPtr CGPDFScannerRetain (/* CGPDFScannerRef */ IntPtr scanner);

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGPDFScannerRelease (/* CGPDFScannerRef */ IntPtr scanner);

		object info;
		internal GCHandle gch;

		public CGPDFScanner (CGPDFContentStream cs, CGPDFOperatorTable table, object userInfo)
		{
			if (cs == null)
				throw new ArgumentNullException ("cs");
			if (table == null)
				throw new ArgumentNullException ("table");

			info = userInfo;
			gch = GCHandle.Alloc (this);
			Handle = CGPDFScannerCreate (cs.Handle, table.Handle, GCHandle.ToIntPtr (gch));
		}

		public CGPDFScanner (IntPtr handle)
		{
			CGPDFScannerRetain (handle);
			Handle = handle;
		}

		[Preserve (Conditional=true)]
		internal CGPDFScanner (IntPtr handle, bool owns)
		{
			if (!owns)
				CGPDFScannerRetain (handle);

			Handle = handle;
		}

		~CGPDFScanner ()
		{
			Dispose (false);
		}

		public object UserInfo {
			get { return info; }
		}

		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		protected virtual void Dispose (bool disposing)
		{
			if (Handle != IntPtr.Zero) {
				CGPDFScannerRelease (Handle);
				Handle = IntPtr.Zero;
			}
			if (gch.IsAllocated)
				gch.Free ();
		}

		public IntPtr Handle { get; private set; }


		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGPDFContentStreamRef */ IntPtr CGPDFScannerGetContentStream (/* CGPDFScannerRef */ IntPtr scanner);

		public CGPDFContentStream GetContentStream ()
		{
			return new CGPDFContentStream (CGPDFScannerGetContentStream (Handle));
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static bool CGPDFScannerScan (/* CGPDFScannerRef */ IntPtr scanner);

		public bool Scan ()
		{
			return CGPDFScannerScan (Handle);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static bool CGPDFScannerPopObject (/* CGPDFScannerRef */ IntPtr scanner, /* CGPDFObjectRef* */ out IntPtr value);

		public bool TryPop (out CGPDFObject value)
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
		extern static bool CGPDFScannerPopBoolean (/* CGPDFScannerRef */ IntPtr scanner, /* CGPDFBoolean* */ out bool value);

		public bool TryPop (out bool value)
		{
			return CGPDFScannerPopBoolean (Handle, out value);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static bool CGPDFScannerPopInteger (/* CGPDFScannerRef */ IntPtr scanner, /* CGPDFInteger* */ out nint value);

		public bool TryPop (out nint value)
		{
			return CGPDFScannerPopInteger (Handle, out value);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static bool CGPDFScannerPopNumber (/* CGPDFScannerRef */ IntPtr scanner, /* CGPDFReal* */ out nfloat value);

		public bool TryPop (out nfloat value)
		{
			return CGPDFScannerPopNumber (Handle, out value);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static bool CGPDFScannerPopName (/* CGPDFScannerRef */ IntPtr scanner, /* const char** */ out IntPtr value);
		// note: that string is not ours to free

		// not to be confusing with CGPDFScannerPopString (value)
		public bool TryPopName (out string name)
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
		extern static bool CGPDFScannerPopString (/* CGPDFScannerRef */ IntPtr scanner, /* CGPDFStringRef* */ out IntPtr value);

		public bool TryPop (out string value)
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
		extern static bool CGPDFScannerPopArray (/* CGPDFScannerRef */ IntPtr scanner, /* CGPDFArrayRef* */ out IntPtr value);

		public bool TryPop (out CGPDFArray value)
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
		extern static bool CGPDFScannerPopDictionary (/* CGPDFScannerRef */ IntPtr scanner, /* CGPDFDictionaryRef* */ out IntPtr value);

		public bool TryPop (out CGPDFDictionary value)
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
		extern static bool CGPDFScannerPopStream (/* CGPDFScannerRef */ IntPtr scanner, /* CGPDFStreamRef* */ out IntPtr value);

		public bool TryPop (out CGPDFStream value)
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
	}
}