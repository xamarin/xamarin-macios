// 
// CGContextPDF.cs: Implements the managed CGContextPDF
//
// Authors: Mono Team
//     
// Copyright 2009-2010 Novell, Inc
// Copyright 2011-2014 Xamarin Inc
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

using ObjCRuntime;
using Foundation;
using CoreFoundation;

namespace CoreGraphics {

	public partial class CGPDFPageInfo {

		public CGRect? MediaBox { get; set; }
		public CGRect? CropBox { get; set; }
		public CGRect? BleedBox { get; set; }
		public CGRect? TrimBox { get; set; }
		public CGRect? ArtBox { get; set; }

		static void Add (NSMutableDictionary dict, IntPtr key, CGRect? val)
		{
			if (!val.HasValue)
				return;
			NSData data;
			unsafe {
				CGRect f = val.Value;
				CGRect *pf = &f;
				data = NSData.FromBytes ((IntPtr) pf, 16);
			}
			dict.LowlevelSetObject (data, key);
		}
		
		internal virtual NSMutableDictionary ToDictionary ()
		{
			var ret = new NSMutableDictionary ();
			Add (ret, kCGPDFContextMediaBox, MediaBox);
			Add (ret, kCGPDFContextCropBox, CropBox);
			Add (ret, kCGPDFContextBleedBox, BleedBox);
			Add (ret, kCGPDFContextTrimBox, TrimBox);
			Add (ret, kCGPDFContextArtBox, ArtBox);
			return ret;
		}
	}

	public partial class CGPDFInfo : CGPDFPageInfo {

		public string Title { get; set; }
		public string Author { get; set; }
		public string Subject { get; set; }
		public string [] Keywords { get; set; }
		public string Creator { get; set; }
		public string OwnerPassword { get; set; }
		public string UserPassword { get; set; }
		public int? EncryptionKeyLength { get; set; }
		public bool? AllowsPrinting { get; set; }
		public bool? AllowsCopying { get; set; }
		public CGPDFAccessPermissions? AccessPermissions { get; set; }
		//public NSDictionary OutputIntent { get; set; }

		internal override NSMutableDictionary ToDictionary ()
		{
			var ret = base.ToDictionary ();

			if (Title != null)
				ret.LowlevelSetObject ((NSString) Title, kCGPDFContextTitle);
			if (Author != null)
				ret.LowlevelSetObject ((NSString) Author, kCGPDFContextAuthor);
			if (Subject != null)
				ret.LowlevelSetObject ((NSString) Subject, kCGPDFContextSubject);
			if (Keywords != null && Keywords.Length > 0){
				if (Keywords.Length == 1)
					ret.LowlevelSetObject ((NSString) Keywords [0], kCGPDFContextKeywords);
				else
					ret.LowlevelSetObject (NSArray.FromStrings (Keywords), kCGPDFContextKeywords);
			}
			if (Creator != null)
				ret.LowlevelSetObject ((NSString) Creator, kCGPDFContextCreator);
			if (OwnerPassword != null)
				ret.LowlevelSetObject ((NSString) OwnerPassword, kCGPDFContextOwnerPassword);
			if (UserPassword != null)
				ret.LowlevelSetObject ((NSString) UserPassword, kCGPDFContextUserPassword);
			if (EncryptionKeyLength.HasValue)
				ret.LowlevelSetObject (NSNumber.FromInt32 (EncryptionKeyLength.Value), kCGPDFContextEncryptionKeyLength);
			if (AllowsPrinting.HasValue && AllowsPrinting.Value == false)
				ret.LowlevelSetObject (CFBoolean.FalseHandle, kCGPDFContextAllowsPrinting);
			if (AllowsCopying.HasValue && AllowsCopying.Value == false)
				ret.LowlevelSetObject (CFBoolean.FalseHandle, kCGPDFContextAllowsCopying);
			if (AccessPermissions.HasValue)
				ret.LowlevelSetObject (NSNumber.FromInt32 ((int) AccessPermissions.Value), kCGPDFContextAccessPermissions);
			return ret;
		}
	}
	
	public class CGContextPDF : CGContext {
		bool closed;
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		unsafe extern static /* CGContextRef */ IntPtr CGPDFContextCreateWithURL (/* CFURLRef */ IntPtr url, CGRect *mediaBox, /* CFDictionaryRef */ IntPtr auxiliaryInfo);

		[DllImport (Constants.CoreGraphicsLibrary)]
		unsafe extern static /* CGContextRef */ IntPtr CGPDFContextCreate (/* CGDataConsumerRef */ IntPtr consumer, CGRect *mediaBox, /* CFDictionaryRef */ IntPtr auxiliaryInfo);

		unsafe CGContextPDF (CGDataConsumer dataConsumer, CGRect *mediaBox, CGPDFInfo info)
		{
			using (var dict = info == null ? null : info.ToDictionary ())
				Handle = CGPDFContextCreate (dataConsumer.GetHandle (), mediaBox, dict.GetHandle ());
		}

		public unsafe CGContextPDF (CGDataConsumer dataConsumer, CGRect mediaBox, CGPDFInfo info) :
			this (dataConsumer, &mediaBox, info)
		{
		}

		public unsafe CGContextPDF (CGDataConsumer dataConsumer, CGRect mediaBox) :
			this (dataConsumer, &mediaBox, null)
		{
		}

		public unsafe CGContextPDF (CGDataConsumer dataConsumer, CGPDFInfo info) :
			this (dataConsumer, null, info)
		{
		}

		public unsafe CGContextPDF (CGDataConsumer dataConsumer) :
			this (dataConsumer, null, null)
		{
		}

		unsafe CGContextPDF (NSUrl url, CGRect *mediaBox, CGPDFInfo info)
		{
			using (var dict = info == null ? null : info.ToDictionary ())
				Handle = CGPDFContextCreateWithURL (url.GetHandle (), mediaBox, dict.GetHandle ());
		}

		public unsafe CGContextPDF (NSUrl url, CGRect mediaBox, CGPDFInfo info) :
			this (url, &mediaBox, info)
		{
		}

		public unsafe CGContextPDF (NSUrl url, CGRect mediaBox) :
			this (url, &mediaBox, null)
		{
		}

		public unsafe CGContextPDF (NSUrl url, CGPDFInfo info) :
			this (url, null, info)
		{
		}

		public unsafe CGContextPDF (NSUrl url) :
			this (url, null, null)
		{
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGPDFContextClose (/* CGContextRef */ IntPtr context);

		public void Close ()
		{
			if (closed)
				return;
			CGPDFContextClose (Handle);
			closed = true;
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGPDFContextBeginPage (/* CGContextRef */ IntPtr context, /* CFDictionaryRef */ IntPtr pageInfo);
		
		public void BeginPage (CGPDFPageInfo info)
		{
			using (var dict = info == null ? null : info.ToDictionary ())
				CGPDFContextBeginPage (Handle, dict == null ? IntPtr.Zero : dict.Handle);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGPDFContextEndPage (/* CGContextRef */ IntPtr context);

		public void EndPage ()
		{
			CGPDFContextEndPage (Handle);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGPDFContextAddDocumentMetadata (/* CGContextRef */ IntPtr context, /* CFDataRef */ IntPtr metadata);

		public void AddDocumentMetadata (NSData data)
		{
			if (data == null)
				return;
			CGPDFContextAddDocumentMetadata (Handle, data.Handle);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGPDFContextSetURLForRect (/* CGContextRef */ IntPtr context, /* CFURLRef */ IntPtr url, CGRect rect);

		public void SetUrl (NSUrl url, CGRect region)
		{
			if (url == null)
				throw new ArgumentNullException ("url");
			CGPDFContextSetURLForRect (Handle, url.Handle, region);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGPDFContextAddDestinationAtPoint (/* CGContextRef */ IntPtr context, /* CFStringRef */ IntPtr name, CGPoint point);

		public void AddDestination (string name, CGPoint point)
		{
			if (name == null)
				throw new ArgumentNullException ("name");

			using (var s = new CFString (name))
				CGPDFContextAddDestinationAtPoint (Handle, s.Handle, point);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGPDFContextSetDestinationForRect (/* CGContextRef */ IntPtr context, /* CFStringRef */ IntPtr name, CGRect rect);

		public void SetDestination (string name, CGRect rect)
		{
			if (name == null)
				throw new ArgumentNullException ("name");

			using (var s = new CFString (name))
				CGPDFContextSetDestinationForRect (Handle, s.Handle, rect);
		}
		
		protected override void Dispose (bool disposing)
		{
			if (disposing)
				Close ();

			base.Dispose (disposing);
		}
	}
}
