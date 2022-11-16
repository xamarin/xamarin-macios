#nullable enable

using System;
using System.Runtime.InteropServices;

using Foundation;
using CoreFoundation;
using ObjCRuntime;

namespace CoreMedia {

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#else
	[Watch (6, 0)]
#endif
	public static class CMAttachmentBearer {

		[DllImport (Constants.CoreMediaLibrary)]
		extern static /* CFDictionaryRef */ IntPtr CMCopyDictionaryOfAttachments (/* CFAllocatorRef */ IntPtr allocator, /* CMAttachmentBearerRef */ IntPtr target,
			/* CMAttachmentMode */ CMAttachmentMode attachmentMode);

		public static NSDictionary? GetAttachments (this ICMAttachmentBearer target, CMAttachmentMode attachmentMode)
		{
			if (target is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (target));
			var attachments = CMCopyDictionaryOfAttachments (IntPtr.Zero, target.Handle, attachmentMode);
			if (attachments == IntPtr.Zero)
				return null;
			return Runtime.GetNSObject<NSDictionary> (attachments, true);
		}

		// There is some API that needs a more strongly typed version of a NSDictionary
		// and there is no easy way to downcast from NSDictionary to NSDictionary<TKey, TValue>
		public static NSDictionary<TKey, TValue>? GetAttachments<TKey, TValue> (this ICMAttachmentBearer target, CMAttachmentMode attachmentMode)
			where TKey : class, INativeObject
			where TValue : class, INativeObject
		{
			if (target is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (target));
			var attachments = CMCopyDictionaryOfAttachments (IntPtr.Zero, target.Handle, attachmentMode);
			if (attachments == IntPtr.Zero)
				return null;

			return Runtime.GetNSObject<NSDictionary<TKey, TValue>> (attachments, true);
		}

		[DllImport (Constants.CoreMediaLibrary)]
		extern static /* CFTypeRef */ IntPtr CMGetAttachment (/* CMAttachmentBearerRef */ IntPtr target, /* CFStringRef */ IntPtr key,
			/* CMAttachmentMode */ out CMAttachmentMode attachmentModeOut);
		public static T? GetAttachment<T> (this ICMAttachmentBearer target, string key, out CMAttachmentMode attachmentModeOut) where T : class, INativeObject
		{
			if (target is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (target));
			if (key is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (key));
			var nsKey = CFString.CreateNative (key);
			var attchm = CMGetAttachment (target.Handle, nsKey, out attachmentModeOut);
			CFString.ReleaseNative (nsKey);
			if (attchm != IntPtr.Zero)
				return Runtime.GetINativeObject<T> (attchm, false);
			return default (T);
		}
#if !WATCH
		public static T? GetAttachment<T> (this ICMAttachmentBearer target, CMSampleBufferAttachmentKey key, out CMAttachmentMode attachmentModeOut) where T : class, INativeObject
		{
			return GetAttachment<T> (target, key.GetConstant (), out attachmentModeOut);
		}
#endif

		[DllImport (Constants.CoreMediaLibrary)]
		extern static void CMPropagateAttachments (/* CMAttachmentBearerRef */ IntPtr source, /* CMAttachmentBearerRef */ IntPtr destination);
		public static void PropagateAttachments (this ICMAttachmentBearer source, ICMAttachmentBearer destination)
		{
			if (source is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (source));
			if (destination is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (destination));
			CMPropagateAttachments (source.Handle, destination.Handle);
		}

		[DllImport (Constants.CoreMediaLibrary)]
		extern static void CMRemoveAllAttachments (/*CMAttachmentBearerRef*/ IntPtr target);
		public static void RemoveAllAttachments (this ICMAttachmentBearer target)
		{
			if (target is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (target));
			CMRemoveAllAttachments (target.Handle);
		}

		[DllImport (Constants.CoreMediaLibrary)]
		extern static void CMRemoveAttachment (/* CMAttachmentBearerRef */ IntPtr target, /* CFStringRef */ IntPtr key);
		public static void RemoveAttachment (this ICMAttachmentBearer target, string key)
		{
			if (target is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (target));
			if (key is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (key));
			var nsKey = CFString.CreateNative (key);
			CMRemoveAttachment (target.Handle, nsKey);
			CFString.ReleaseNative (nsKey);
		}

		[DllImport (Constants.CoreMediaLibrary)]
		extern static void CMSetAttachment (/* CMAttachmentBearerRef */ IntPtr target, /* CFStringRef */ IntPtr key, /* CFTypeRef */ IntPtr value,
			/* CMAttachmentMode */ CMAttachmentMode attachmentMode);
		public static void SetAttachment (this ICMAttachmentBearer target, string key, INativeObject value, CMAttachmentMode attachmentMode)
		{
			if (target is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (target));
			if (value is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));
			if (key is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (key));
			var nsKey = CFString.CreateNative (key);
			CMSetAttachment (target.Handle, nsKey, value.Handle, attachmentMode);
			CFString.ReleaseNative (nsKey);
		}

		[DllImport (Constants.CoreMediaLibrary)]
		extern static void CMSetAttachments (/* CMAttachmentBearerRef */ IntPtr target, /* CFDictionaryRef */ IntPtr theAttachments,
			/* CMAttachmentMode */ CMAttachmentMode attachmentMode);
		public static void SetAttachments (this ICMAttachmentBearer target, NSDictionary theAttachments, CMAttachmentMode attachmentMode)
		{
			if (target is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (target));
			if (theAttachments is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (theAttachments));
			CMSetAttachments (target.Handle, theAttachments.Handle, attachmentMode);
		}
	}
}
