// 
// CGPDFContentStream.cs: Implement the managed CGPDFContentStream bindings
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//     
// Copyright 2014 Xamarin Inc. All rights reserved.

using System;
using System.Runtime.InteropServices;
using Foundation;
using ObjCRuntime;
using CoreFoundation;

namespace CoreGraphics {

	// CGPDFContentStream.h
	public class CGPDFContentStream : INativeObject, IDisposable {

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGPDFContentStreamRef */ IntPtr CGPDFContentStreamCreateWithPage (/* CGPDFPageRef */ IntPtr page);

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGPDFContentStreamRef */ IntPtr CGPDFContentStreamCreateWithStream (/* CGPDFStreamRef */ IntPtr stream, 
			/* CGPDFDictionaryRef */ IntPtr streamResources, /* CGPDFContentStreamRef */ IntPtr parent);

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGPDFContentStreamRef */ IntPtr CGPDFContentStreamRetain (/* CGPDFContentStreamRef */ IntPtr cs);

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGPDFContentStreamRelease (/* CGPDFContentStreamRef */ IntPtr cs);

		public CGPDFContentStream (IntPtr handle)
		{
			CGPDFContentStreamRetain (handle);
			Handle = handle;
		}

		[Preserve (Conditional=true)]
		internal CGPDFContentStream (IntPtr handle, bool owns)
		{
			if (!owns)
				CGPDFContentStreamRetain (handle);

			Handle = handle;
		}

		public CGPDFContentStream (CGPDFPage page)
		{
			if (page == null)
				throw new ArgumentNullException ("page");
			Handle = CGPDFContentStreamCreateWithPage (page.Handle);
		}

		public CGPDFContentStream (CGPDFStream stream, NSDictionary streamResources = null, CGPDFContentStream parent = null)
		{
			if (stream == null)
				throw new ArgumentNullException ("stream");

			var dh = streamResources == null ? IntPtr.Zero : streamResources.Handle;
			var ph = parent == null ? IntPtr.Zero : parent.Handle;
			Handle = CGPDFContentStreamCreateWithStream (stream.Handle, dh, ph);
		}

		~CGPDFContentStream ()
		{
			Dispose (false);
		}

		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		protected virtual void Dispose (bool disposing)
		{
			if (Handle != IntPtr.Zero){
				CGPDFContentStreamRelease (Handle);
				Handle = IntPtr.Zero;
			}
		}

		public IntPtr Handle { get; private set; }

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CFArrayRef */ IntPtr CGPDFContentStreamGetStreams (/* CGPDFContentStreamRef */ IntPtr cs);

		public CGPDFStream[] GetStreams ()
		{
			using (CFArray a = new CFArray (CGPDFContentStreamGetStreams (Handle))) {
				var streams = new CGPDFStream [a.Count];
				for (int i = 0; i < a.Count; i++)
					streams [i] = new CGPDFStream (a.GetValue (i));
				return streams;
				// note: CGPDFStreamRef is weird because it has no retain/release calls unlike other CGPDF* types
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGPDFObjectRef */ IntPtr CGPDFContentStreamGetResource (/* CGPDFContentStreamRef */ IntPtr cs, /* const char* */ string category, /* const char* */ string name);

		public CGPDFObject GetResource (string category, string name)
		{
			if (category == null)
				throw new ArgumentNullException ("category");
			if (name == null)
				throw new ArgumentNullException ("name");

			var h = CGPDFContentStreamGetResource (Handle, category, name);
			return (h == IntPtr.Zero) ? null : new CGPDFObject (h);
		}
	}
}