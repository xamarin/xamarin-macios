// 
// CVBuffer.cs: Implements the managed CVBuffer
//
// Authors: Mono Team
//     
// Copyright 2010 Novell, Inc
// Copyright 2014 Xamarin Inc
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
using CoreFoundation;
using ObjCRuntime;
using Foundation;

namespace CoreVideo {

	// CVBuffer.h
	[Watch (4,0)]
	public partial class CVBuffer : INativeObject
#if !COREBUILD
		, IDisposable
#endif
		{
#if !COREBUILD
#if !XAMCORE_2_0
		public static readonly NSString MovieTimeKey;
		public static readonly NSString TimeValueKey;
		public static readonly NSString TimeScaleKey;
		public static readonly NSString PropagatedAttachmentsKey;
		public static readonly NSString NonPropagatedAttachmentsKey;

		static CVBuffer ()
		{
			var hlib = Dlfcn.dlopen (Constants.CoreVideoLibrary, 0);
			if (hlib == IntPtr.Zero)
				return;
			try {
				MovieTimeKey = Dlfcn.GetStringConstant (hlib, "kCVBufferMovieTimeKey");
				TimeValueKey = Dlfcn.GetStringConstant (hlib, "kCVBufferTimeValueKey");
				TimeScaleKey = Dlfcn.GetStringConstant (hlib, "kCVBufferTimeScaleKey");
				PropagatedAttachmentsKey = Dlfcn.GetStringConstant (hlib, "kCVBufferPropagatedAttachmentsKey");
				NonPropagatedAttachmentsKey = Dlfcn.GetStringConstant (hlib, "kCVBufferNonPropagatedAttachmentsKey");
			}
			finally {
				Dlfcn.dlclose (hlib);
			}
		}
#endif
		internal IntPtr handle;

		internal CVBuffer ()
		{
		}

		internal CVBuffer (IntPtr handle)
		{
			if (handle == IntPtr.Zero)
				throw new Exception ("Invalid parameters to context creation");

			CVBufferRetain (handle);
			this.handle = handle;
		}

		[Preserve (Conditional=true)]
		internal CVBuffer (IntPtr handle, bool owns)
		{
			if (!owns)
				CVBufferRetain (handle);

			this.handle = handle;
		}

		~CVBuffer ()
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
	
		[DllImport (Constants.CoreVideoLibrary)]
		extern static void CVBufferRelease (/* CVBufferRef */ IntPtr buffer);
		
		[DllImport (Constants.CoreVideoLibrary)]
		extern static /* CVBufferRef */ IntPtr CVBufferRetain (/* CVBufferRef */ IntPtr buffer);
		
		protected virtual void Dispose (bool disposing)
		{
			if (handle != IntPtr.Zero){
				CVBufferRelease (handle);
				handle = IntPtr.Zero;
			}
		}

		[DllImport (Constants.CoreVideoLibrary)]
		extern static void CVBufferRemoveAllAttachments (/* CVBufferRef */ IntPtr buffer);

		public void RemoveAllAttachments ()
		{
			CVBufferRemoveAllAttachments (handle);
		}

		[DllImport (Constants.CoreVideoLibrary)]
		extern static void CVBufferRemoveAttachment (/* CVBufferRef */ IntPtr buffer, /* CFStringRef */ IntPtr key);

		public void RemoveAttachment (NSString key)
		{
			if (key == null)
				throw new ArgumentNullException ("key");

			CVBufferRemoveAttachment (handle, key.Handle);
		}

		[DllImport (Constants.CoreVideoLibrary)]
		extern static /* CFTypeRef */ IntPtr CVBufferGetAttachment (/* CVBufferRef */ IntPtr buffer, /* CFStringRef */ IntPtr key, out CVAttachmentMode attachmentMode);

// FIXME: we need to bring the new API to xamcore
#if XAMCORE_2_0 && !MONOMAC
		// any CF object can be attached
		public T GetAttachment<T> (NSString key, out CVAttachmentMode attachmentMode) where T : class, INativeObject
		{
			if (key == null)
				throw new ArgumentNullException ("key");
			return Runtime.GetINativeObject<T> (CVBufferGetAttachment (handle, key.Handle, out attachmentMode), false);
		}
#else
		public NSObject GetAttachment (NSString key, out CVAttachmentMode attachmentMode)
		{
			if (key == null)
				throw new ArgumentNullException ("key");
			return Runtime.GetNSObject (CVBufferGetAttachment (handle, key.Handle, out attachmentMode));
		}
#endif

		[DllImport (Constants.CoreVideoLibrary)]
		extern static /* CFDictionaryRef */ IntPtr CVBufferGetAttachments (/* CVBufferRef */ IntPtr buffer, CVAttachmentMode attachmentMode);

		public NSDictionary GetAttachments (CVAttachmentMode attachmentMode)
		{
			return (NSDictionary) Runtime.GetNSObject (CVBufferGetAttachments (handle, attachmentMode));
		}

#if XAMCORE_2_0
		// There is some API that needs a more strongly typed version of a NSDictionary
		// and there is no easy way to downcast from NSDictionary to NSDictionary<TKey, TValue>
		public NSDictionary<TKey, TValue> GetAttachments<TKey, TValue> (CVAttachmentMode attachmentMode)
			where TKey : class, INativeObject
			where TValue : class, INativeObject
		{
			return Runtime.GetNSObject<NSDictionary<TKey, TValue>> (CVBufferGetAttachments (handle, attachmentMode));
		}
#endif

		[DllImport (Constants.CoreVideoLibrary)]
		extern static void CVBufferPropagateAttachments (/* CVBufferRef */ IntPtr sourceBuffer, /* CVBufferRef */ IntPtr destinationBuffer);

		public void PropogateAttachments (CVBuffer destinationBuffer)
		{
			if (destinationBuffer == null)
				throw new ArgumentNullException ("destinationBuffer");

			CVBufferPropagateAttachments (handle, destinationBuffer.Handle);
		}

		[DllImport (Constants.CoreVideoLibrary)]
		extern static void CVBufferSetAttachment (/* CVBufferRef */ IntPtr buffer, /* CFStringRef */ IntPtr key, /* CFTypeRef */ IntPtr @value, CVAttachmentMode attachmentMode);

#if XAMCORE_2_0
		public void SetAttachment (NSString key, INativeObject @value, CVAttachmentMode attachmentMode)
		{
			if (key == null)
				throw new ArgumentNullException ("key");
			if (@value == null)
				throw new ArgumentNullException ("value");
			CVBufferSetAttachment (handle, key.Handle, @value.Handle, attachmentMode);
		}
#else
		public void SetAttachment (NSString key, NSObject @value, CVAttachmentMode attachmentMode)
		{
			if (key == null)
				throw new ArgumentNullException ("key");
			if (@value == null)
				throw new ArgumentNullException ("value");
			CVBufferSetAttachment (handle, key.Handle, @value.Handle, attachmentMode);
		}
#endif

		[DllImport (Constants.CoreVideoLibrary)]
		extern static void CVBufferSetAttachments (/* CVBufferRef */ IntPtr buffer, /* CFDictionaryRef */ IntPtr theAttachments, CVAttachmentMode attachmentMode);

		public void SetAttachments (NSDictionary theAttachments, CVAttachmentMode attachmentMode)
		{
			if (theAttachments == null)
				throw new ArgumentNullException ("theAttachments");
			CVBufferSetAttachments (handle, theAttachments.Handle, attachmentMode);
		}
#endif // !COREBUILD
	}
}
