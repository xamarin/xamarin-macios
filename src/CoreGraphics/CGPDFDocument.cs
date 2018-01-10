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
using System;
using System.Runtime.InteropServices;
using Foundation;
using ObjCRuntime;
using CoreFoundation;

namespace CoreGraphics {

	public class CGPDFDocument : INativeObject
#if !COREBUILD
		, IDisposable
#endif
	{
#if !COREBUILD
		internal IntPtr handle;

		~CGPDFDocument ()
		{
			Dispose (false);
		}
		
		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		public IntPtr Handle {
			get { return handle; }
		}
	
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGPDFDocumentRelease (/* CGPDFDocumentRef */ IntPtr document);

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGPDFDocumentRef */ IntPtr CGPDFDocumentRetain (/* CGPDFDocumentRef */ IntPtr document);
		
		protected virtual void Dispose (bool disposing)
		{
			if (handle != IntPtr.Zero){
				CGPDFDocumentRelease (handle);
				handle = IntPtr.Zero;
			}
		}

		/* invoked by marshallers */
		public CGPDFDocument (IntPtr handle)
		{
			this.handle = handle;
			CGPDFDocumentRetain (handle);
		}

		[Preserve (Conditional=true)]
		internal CGPDFDocument (IntPtr handle, bool owns)
		{
			this.handle = handle;
			if (!owns)
				CGPDFDocumentRetain (handle);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGPDFDocumentRef */ IntPtr CGPDFDocumentCreateWithProvider (/* CGDataProviderRef */ IntPtr provider);
		
		public CGPDFDocument (CGDataProvider provider)
		{
			if (provider == null)
				throw new ArgumentNullException ("provider");
			handle = CGPDFDocumentCreateWithProvider (provider.Handle);
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGPDFDocumentRef */ IntPtr CGPDFDocumentCreateWithURL (/* CFURLRef */ IntPtr url);

		public static CGPDFDocument FromFile (string str)
		{
			using (var url = CFUrl.FromFile (str)){
				if (url == null)
					return null;
				IntPtr handle = CGPDFDocumentCreateWithURL (url.Handle);
				if (handle == IntPtr.Zero)
					return null;
				return new CGPDFDocument (handle, true);
			}
			
		}
			
		public static CGPDFDocument FromUrl (string str)
		{
			using (var url = CFUrl.FromUrlString (str, null)){
				if (url == null)
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
				return CGPDFDocumentGetNumberOfPages (handle);
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGPDFPageRef */ IntPtr CGPDFDocumentGetPage (/* CGPDFDocumentRef */ IntPtr document, /* size_t */ nint page);
		
		public CGPDFPage GetPage (nint page)
		{
			var h = CGPDFDocumentGetPage (handle, page);
			return h == IntPtr.Zero ? null : new CGPDFPage (h);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGPDFDocumentGetVersion (/* CGPDFDocumentRef */ IntPtr document, /* int* */ out int majorVersion, /* int* */ out int minorVersion);

		public void GetVersion (out int major, out int minor)
		{
			CGPDFDocumentGetVersion (handle, out major, out minor);
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static bool CGPDFDocumentIsEncrypted (/* CGPDFDocumentRef */ IntPtr document);

		public bool IsEncrypted {
			get {
				return CGPDFDocumentIsEncrypted (handle);
			}
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static bool CGPDFDocumentUnlockWithPassword (/* CGPDFDocumentRef */ IntPtr document, /* const char* */ string password);

#if XAMCORE_2_0
		public bool Unlock (string password)
		{
			return CGPDFDocumentUnlockWithPassword (handle, password);
		}
#else
		public bool UnlockWithPassword (string pass)
		{
			return CGPDFDocumentUnlockWithPassword (handle, pass);
		}
#endif

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static bool CGPDFDocumentIsUnlocked (/* CGPDFDocumentRef */ IntPtr document);

		public bool IsUnlocked {
			get {
				return CGPDFDocumentIsUnlocked (handle);
			}
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static bool CGPDFDocumentAllowsPrinting (/* CGPDFDocumentRef */ IntPtr document);

		public bool AllowsPrinting {
			get {
				return CGPDFDocumentAllowsPrinting (handle);
			}
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static bool CGPDFDocumentAllowsCopying (/* CGPDFDocumentRef */ IntPtr document);

		public bool AllowsCopying {
			get {
				return CGPDFDocumentAllowsCopying (handle);
			}
		}

#if !XAMCORE_2_0
		// deprecated in OSX 10.5 in favor of CGPDFPage API and never part of iOS

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static CGRect CGPDFDocumentGetMediaBox (/* CGPDFDocumentRef */ IntPtr document, /* int */ int page);

		public CGRect GetMediaBox (int page)
		{
			return CGPDFDocumentGetMediaBox (handle, page);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static CGRect CGPDFDocumentGetCropBox (/* CGPDFDocumentRef */ IntPtr document, /* int */ int page);

		public CGRect GetCropBox (int page)
		{
			return CGPDFDocumentGetCropBox (handle, page);
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static CGRect CGPDFDocumentGetBleedBox (/* CGPDFDocumentRef */ IntPtr document, /* int */ int page);

		public CGRect GetBleedBox (int page)
		{
			return CGPDFDocumentGetBleedBox (handle, page);
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static CGRect CGPDFDocumentGetTrimBox (/* CGPDFDocumentRef */ IntPtr document, /* int */ int page);

		public CGRect GetTrimBox (int page)
		{
			return CGPDFDocumentGetTrimBox (handle, page);
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static CGRect CGPDFDocumentGetArtBox (/* CGPDFDocumentRef */ IntPtr document, /* int */ int page);

		public CGRect GetArtBox (int page)
		{
			return CGPDFDocumentGetArtBox (handle, page);
		}
#endif
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGPDFDictionaryRef */ IntPtr CGPDFDocumentGetCatalog (/* CGPDFDocumentRef */ IntPtr document);
		public CGPDFDictionary GetCatalog ()
		{
			return new CGPDFDictionary (CGPDFDocumentGetCatalog (handle));
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGPDFDictionaryRef */ IntPtr CGPDFDocumentGetInfo (/* CGPDFDocumentRef */ IntPtr document);

		public CGPDFDictionary GetInfo ()
		{
			return new CGPDFDictionary (CGPDFDocumentGetInfo (handle));
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		[iOS (11,0), Mac(10,13), TV(11,0), Watch(4,0)]
		extern static void CGPDFContextSetOutline (/* CGPDFDocumentRef */ IntPtr document, IntPtr /* dictionary */ outline);

		[iOS (11,0), Mac(10,13), TV(11,0), Watch(4,0)]
		public void SetOutline (CGPDFOutlineOptions options)
		{
			CGPDFContextSetOutline (handle, options == null ? IntPtr.Zero : options.Dictionary.Handle);
		}
					
		[DllImport (Constants.CoreGraphicsLibrary)]
		[iOS (11,0), Mac(10,13), TV(11,0), Watch(4,0)]
		extern static /* CFDictionaryPtry */ IntPtr CGPDFDocumentGetOutline (/* CGPDFDocumentRef */ IntPtr document);

		[iOS (11,0), Mac(10,13), TV(11,0), Watch(4,0)]
		public CGPDFOutlineOptions GetOutline ()
		{
			var ptr = CGPDFDocumentGetOutline (handle);
			return new CGPDFOutlineOptions (Runtime.GetNSObject<NSDictionary> (ptr));
		}

		[iOS (11,0), Mac(10,13), TV(11,0), Watch(4,0)]
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static CGPDFAccessPermissions CGPDFDocumentGetAccessPermissions (IntPtr document);

		[iOS (11,0), Mac(10,13), TV(11,0), Watch(4,0)]
		public CGPDFAccessPermissions GetAccessPermissions ()
		{
			return CGPDFDocumentGetAccessPermissions (handle);
		}
		
#endif // !COREBUILD
	}
}
