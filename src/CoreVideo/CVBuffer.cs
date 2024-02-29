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

#nullable enable

using System;
using System.Runtime.InteropServices;
using CoreFoundation;
using ObjCRuntime;
using Foundation;

#if !NET
using NativeHandle = System.IntPtr;
#endif

#nullable enable

namespace CoreVideo {

	// CVBuffer.h
#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public partial class CVBuffer : NativeObject {
#if !COREBUILD
		[Preserve (Conditional = true)]
		internal CVBuffer (NativeHandle handle, bool owns)
			: base (handle, owns, verify: true)
		{
		}

		[DllImport (Constants.CoreVideoLibrary)]
		extern static void CVBufferRelease (/* CVBufferRef */ IntPtr buffer);

		[DllImport (Constants.CoreVideoLibrary)]
		extern static /* CVBufferRef */ IntPtr CVBufferRetain (/* CVBufferRef */ IntPtr buffer);

		protected internal override void Retain ()
		{
			CVBufferRetain (GetCheckedHandle ());
		}

		protected internal override void Release ()
		{
			CVBufferRelease (GetCheckedHandle ());
		}

		[DllImport (Constants.CoreVideoLibrary)]
		extern static void CVBufferRemoveAllAttachments (/* CVBufferRef */ IntPtr buffer);

		public void RemoveAllAttachments ()
		{
			CVBufferRemoveAllAttachments (Handle);
		}

		[DllImport (Constants.CoreVideoLibrary)]
		extern static void CVBufferRemoveAttachment (/* CVBufferRef */ IntPtr buffer, /* CFStringRef */ IntPtr key);

		public void RemoveAttachment (NSString key)
		{
			if (key is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (key));

			CVBufferRemoveAttachment (Handle, key.Handle);
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("macos12.0")]
		[ObsoletedOSPlatform ("tvos15.0")]
		[ObsoletedOSPlatform ("maccatalyst15.0")]
		[ObsoletedOSPlatform ("ios15.0")]
#else
		[Deprecated (PlatformName.MacOSX, 12, 0)]
		[Deprecated (PlatformName.iOS, 15, 0)]
		[Deprecated (PlatformName.TvOS, 15, 0)]
		[Deprecated (PlatformName.MacCatalyst, 15, 0)]
		[Deprecated (PlatformName.WatchOS, 8, 0)]
#endif
		[DllImport (Constants.CoreVideoLibrary)]
		extern static /* CFTypeRef */ IntPtr CVBufferGetAttachment (/* CVBufferRef */ IntPtr buffer, /* CFStringRef */ IntPtr key, out CVAttachmentMode attachmentMode);

		// The new method is the same as the old one but changing the ownership from Get to Copy, so we will use the new version if possible since the
		// older method has been deprecatd.
#if NET
		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[Watch (8, 0)]
		[TV (15, 0)]
		[iOS (15, 0)]
		[MacCatalyst (15, 0)]
#endif
		[DllImport (Constants.CoreVideoLibrary)]
		extern static /* CFTypeRef */ IntPtr CVBufferCopyAttachment (/* CVBufferRef */ IntPtr buffer, /* CFStringRef */ IntPtr key, out CVAttachmentMode attachmentMode);

		// FIXME: we need to bring the new API to xamcore
#if !MONOMAC
		// any CF object can be attached
		public T? GetAttachment<T> (NSString key, out CVAttachmentMode attachmentMode) where T : class, INativeObject
		{
			if (key is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (key));
#if IOS || __MACCATALYST__ || TVOS
			if (SystemVersion.CheckiOS (15, 0))
#elif WATCH
			if (SystemVersion.CheckwatchOS (8, 0))
#endif
			return Runtime.GetINativeObject<T> (CVBufferCopyAttachment (Handle, key.Handle, out attachmentMode), true);
			return Runtime.GetINativeObject<T> (CVBufferGetAttachment (Handle, key.Handle, out attachmentMode), false);
		}
#else
		public NSObject? GetAttachment (NSString key, out CVAttachmentMode attachmentMode)
		{
			if (key is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (key));
			if (SystemVersion.CheckmacOS (12, 0))
				return Runtime.GetNSObject<NSObject> (CVBufferCopyAttachment (Handle, key.Handle, out attachmentMode), true);
			else
				return Runtime.GetNSObject<NSObject> (CVBufferGetAttachment (Handle, key.Handle, out attachmentMode), false);
		}
#endif

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[ObsoletedOSPlatform ("macos12.0")]
		[ObsoletedOSPlatform ("tvos15.0")]
		[ObsoletedOSPlatform ("maccatalyst15.0")]
		[ObsoletedOSPlatform ("ios15.0")]
#else
		[Deprecated (PlatformName.MacOSX, 12, 0)]
		[Deprecated (PlatformName.iOS, 15, 0)]
		[Deprecated (PlatformName.TvOS, 15, 0)]
		[Deprecated (PlatformName.MacCatalyst, 15, 0)]
		[Deprecated (PlatformName.WatchOS, 8, 0)]
#endif
		[DllImport (Constants.CoreVideoLibrary)]
		extern static /* CFDictionaryRef */ IntPtr CVBufferGetAttachments (/* CVBufferRef */ IntPtr buffer, CVAttachmentMode attachmentMode);

#if NET
		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[Watch (8, 0)]
		[TV (15, 0)]
		[iOS (15, 0)]
		[MacCatalyst (15, 0)]
#endif
		[DllImport (Constants.CoreVideoLibrary)]
		extern static /* CFDictionaryRef */ IntPtr CVBufferCopyAttachments (/* CVBufferRef */ IntPtr buffer, CVAttachmentMode attachmentMode);

		public NSDictionary? GetAttachments (CVAttachmentMode attachmentMode)
		{
#if IOS || __MACCATALYST__ || TVOS
			if (SystemVersion.CheckiOS (15, 0))
#elif WATCH
			if (SystemVersion.CheckwatchOS (8, 0))
#elif MONOMAC
			if (SystemVersion.CheckmacOS (12, 0))
#endif
			return Runtime.GetINativeObject<NSDictionary> (CVBufferCopyAttachments (Handle, attachmentMode), true);
			return Runtime.GetNSObject<NSDictionary> (CVBufferGetAttachments (Handle, attachmentMode), false);
		}

		// There is some API that needs a more strongly typed version of a NSDictionary
		// and there is no easy way to downcast from NSDictionary to NSDictionary<TKey, TValue>
		public NSDictionary<TKey, TValue>? GetAttachments<TKey, TValue> (CVAttachmentMode attachmentMode)
			where TKey : class, INativeObject
			where TValue : class, INativeObject
		{
			return Runtime.GetNSObject<NSDictionary<TKey, TValue>> (CVBufferGetAttachments (Handle, attachmentMode));
		}

		[DllImport (Constants.CoreVideoLibrary)]
		extern static void CVBufferPropagateAttachments (/* CVBufferRef */ IntPtr sourceBuffer, /* CVBufferRef */ IntPtr destinationBuffer);

		public void PropogateAttachments (CVBuffer destinationBuffer)
		{
			if (destinationBuffer is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (destinationBuffer));

			CVBufferPropagateAttachments (Handle, destinationBuffer.Handle);
		}

		[DllImport (Constants.CoreVideoLibrary)]
		extern static void CVBufferSetAttachment (/* CVBufferRef */ IntPtr buffer, /* CFStringRef */ IntPtr key, /* CFTypeRef */ IntPtr @value, CVAttachmentMode attachmentMode);

		public void SetAttachment (NSString key, INativeObject @value, CVAttachmentMode attachmentMode)
		{
			if (key is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (key));
			if (@value is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));
			CVBufferSetAttachment (Handle, key.Handle, @value.Handle, attachmentMode);
		}

		[DllImport (Constants.CoreVideoLibrary)]
		extern static void CVBufferSetAttachments (/* CVBufferRef */ IntPtr buffer, /* CFDictionaryRef */ IntPtr theAttachments, CVAttachmentMode attachmentMode);

		public void SetAttachments (NSDictionary theAttachments, CVAttachmentMode attachmentMode)
		{
			if (theAttachments is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (theAttachments));
			CVBufferSetAttachments (Handle, theAttachments.Handle, attachmentMode);
		}

#if NET
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
#else
		[iOS (15, 0)]
		[TV (15, 0)]
		[MacCatalyst (15, 0)]
		[Watch (8, 0)]
#endif
		[DllImport (Constants.CoreVideoLibrary)]
		[return: MarshalAs (UnmanagedType.U1)]
		static extern bool CVBufferHasAttachment (/* CVBufferRef */ IntPtr buffer, /* CFStringRef */ IntPtr key);

#if NET
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
#else
		[iOS (15, 0)]
		[TV (15, 0)]
		[MacCatalyst (15, 0)]
		[Watch (8, 0)]
#endif
		public bool HasAttachment (NSString key)
		{
			if (key is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (key));
			return CVBufferHasAttachment (Handle, key.Handle);
		}

#endif // !COREBUILD
	}
}
