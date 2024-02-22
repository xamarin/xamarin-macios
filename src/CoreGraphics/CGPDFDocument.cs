// 
// CGPDFDocument.cs: Implements the managed CGPDFDocument
//
// Authors: Mono Team
//     
// Copyright 2009 Novell, Inc
// Copyright 2011, 2012 Xamarin Inc
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

#nullable enable

using System;
using System.Runtime.CompilerServices;
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
	public class CGPDFDocument : NativeObject {
#if !COREBUILD
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGPDFDocumentRelease (/* CGPDFDocumentRef */ IntPtr document);

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGPDFDocumentRef */ IntPtr CGPDFDocumentRetain (/* CGPDFDocumentRef */ IntPtr document);

#if !NET
		public CGPDFDocument (NativeHandle handle)
			: base (handle, false)
		{
		}
#endif

		[Preserve (Conditional = true)]
		internal CGPDFDocument (NativeHandle handle, bool owns)
			: base (handle, owns)
		{
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGPDFDocumentRef */ IntPtr CGPDFDocumentCreateWithProvider (/* CGDataProviderRef */ IntPtr provider);

		public CGPDFDocument (CGDataProvider provider)
			: base (CGPDFDocumentCreateWithProvider (Runtime.ThrowOnNull (provider, nameof (provider)).Handle), true)
		{
		}

		protected internal override void Retain ()
		{
			CGPDFDocumentRetain (GetCheckedHandle ());
		}

		protected internal override void Release ()
		{
			CGPDFDocumentRelease (GetCheckedHandle ());
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGPDFDocumentRef */ IntPtr CGPDFDocumentCreateWithURL (/* CFURLRef */ IntPtr url);

		public static CGPDFDocument? FromFile (string str)
		{
			using (var url = CFUrl.FromFile (str)) {
				if (url is null)
					return null;
				IntPtr handle = CGPDFDocumentCreateWithURL (url.Handle);
				if (handle == IntPtr.Zero)
					return null;
				return new CGPDFDocument (handle, true);
			}

		}

		public static CGPDFDocument? FromUrl (string str)
		{
			using (var url = CFUrl.FromUrlString (str, null)) {
				if (url is null)
					return null;
				IntPtr handle = CGPDFDocumentCreateWithURL (url.Handle);
				if (handle == IntPtr.Zero)
					return null;
				return new CGPDFDocument (handle, true);
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* size_t */ nint CGPDFDocumentGetNumberOfPages (/* CGPDFDocumentRef */ IntPtr document);

		public nint Pages {
			get {
				return CGPDFDocumentGetNumberOfPages (Handle);
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGPDFPageRef */ IntPtr CGPDFDocumentGetPage (/* CGPDFDocumentRef */ IntPtr document, /* size_t */ nint page);

		public CGPDFPage? GetPage (nint page)
		{
			var h = CGPDFDocumentGetPage (Handle, page);
			return h == IntPtr.Zero ? null : new CGPDFPage (h, false);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		unsafe extern static void CGPDFDocumentGetVersion (/* CGPDFDocumentRef */ IntPtr document, /* int* */ int* majorVersion, /* int* */ int* minorVersion);

		public void GetVersion (out int major, out int minor)
		{
			major = default;
			minor = default;
			unsafe {
				CGPDFDocumentGetVersion (Handle, (int *) Unsafe.AsPointer<int> (ref major), (int *) Unsafe.AsPointer<int> (ref minor));
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static byte CGPDFDocumentIsEncrypted (/* CGPDFDocumentRef */ IntPtr document);

		public bool IsEncrypted {
			get {
				return CGPDFDocumentIsEncrypted (Handle) != 0;
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static byte CGPDFDocumentUnlockWithPassword (/* CGPDFDocumentRef */ IntPtr document, /* const char* */ IntPtr password);

		public bool Unlock (string password)
		{
			using var passwordPtr = new TransientString (password);
			return CGPDFDocumentUnlockWithPassword (Handle, passwordPtr) != 0;
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static byte CGPDFDocumentIsUnlocked (/* CGPDFDocumentRef */ IntPtr document);

		public bool IsUnlocked {
			get {
				return CGPDFDocumentIsUnlocked (Handle) != 0;
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static byte CGPDFDocumentAllowsPrinting (/* CGPDFDocumentRef */ IntPtr document);

		public bool AllowsPrinting {
			get {
				return CGPDFDocumentAllowsPrinting (Handle) != 0;
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static byte CGPDFDocumentAllowsCopying (/* CGPDFDocumentRef */ IntPtr document);

		public bool AllowsCopying {
			get {
				return CGPDFDocumentAllowsCopying (Handle) != 0;
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGPDFDictionaryRef */ IntPtr CGPDFDocumentGetCatalog (/* CGPDFDocumentRef */ IntPtr document);
		public CGPDFDictionary GetCatalog ()
		{
			return new CGPDFDictionary (CGPDFDocumentGetCatalog (Handle));
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGPDFDictionaryRef */ IntPtr CGPDFDocumentGetInfo (/* CGPDFDocumentRef */ IntPtr document);

		public CGPDFDictionary GetInfo ()
		{
			return new CGPDFDictionary (CGPDFDocumentGetInfo (Handle));
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGPDFContextSetOutline (/* CGPDFDocumentRef */ IntPtr document, IntPtr /* dictionary */ outline);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		public void SetOutline (CGPDFOutlineOptions? options)
		{
			CGPDFContextSetOutline (Handle, options.GetHandle ());
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CFDictionaryPtry */ IntPtr CGPDFDocumentGetOutline (/* CGPDFDocumentRef */ IntPtr document);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		public CGPDFOutlineOptions GetOutline ()
		{
			var ptr = CGPDFDocumentGetOutline (Handle);
			return new CGPDFOutlineOptions (Runtime.GetNSObject<NSDictionary> (ptr));
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static CGPDFAccessPermissions CGPDFDocumentGetAccessPermissions (IntPtr document);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		public CGPDFAccessPermissions GetAccessPermissions ()
		{
			return CGPDFDocumentGetAccessPermissions (Handle);
		}

#endif // !COREBUILD
	}
}
