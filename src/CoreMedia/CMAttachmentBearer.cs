using System;
using System.Runtime.InteropServices;

using Foundation;
using CoreFoundation;
using ObjCRuntime;

namespace CoreMedia {

	public enum CMAttachmentMode : uint {
		ShouldNotPropagate    = 0,
		ShouldPropagate       = 1
	};

	public static class CMAttachmentBearer {

		[DllImport(Constants.CoreMediaLibrary)]
		extern static /* CFDictionaryRef */ IntPtr CMCopyDictionaryOfAttachments (/* CFAllocatorRef */ IntPtr allocator, /* CMAttachmentBearerRef */ IntPtr target,
			/* CMAttachmentMode */ CMAttachmentMode attachmentMode);

		public static NSDictionary GetAttachments (this ICMAttachmentBearer target, CMAttachmentMode attachmentMode)
		{
			if (target == null)
				throw new ArgumentNullException (nameof (target));
			var attachments = CMCopyDictionaryOfAttachments (IntPtr.Zero, target.Handle, attachmentMode);
			if (attachments == IntPtr.Zero)
				return null;
			return Runtime.GetNSObject<NSDictionary> (attachments, true);
		}

#if XAMCORE_2_0
		// There is some API that needs a more strongly typed version of a NSDictionary
		// and there is no easy way to downcast from NSDictionary to NSDictionary<TKey, TValue>
		public static NSDictionary<TKey, TValue> GetAttachments<TKey, TValue> (this ICMAttachmentBearer target, CMAttachmentMode attachmentMode)
			where TKey : class, INativeObject
			where TValue : class, INativeObject
		{
			if (target == null)
				throw new ArgumentNullException (nameof (target));
			var attachments = CMCopyDictionaryOfAttachments (IntPtr.Zero, target.Handle, attachmentMode);
			if (attachments == IntPtr.Zero)
				return null;

			return Runtime.GetNSObject<NSDictionary<TKey, TValue>> (attachments, true);
		}
#endif

		[DllImport(Constants.CoreMediaLibrary)]
		extern static /* CFTypeRef */ IntPtr CMGetAttachment (/* CMAttachmentBearerRef */ IntPtr target, /* CFStringRef */ IntPtr key,
			/* CMAttachmentMode */ out CMAttachmentMode attachmentModeOut);
		public static T GetAttachment<T> (this ICMAttachmentBearer target, string key, out CMAttachmentMode attachmentModeOut) where T: class, INativeObject
		{
			if (target == null)
				throw new ArgumentNullException (nameof (target));
			if (key == null)
				throw new ArgumentNullException (nameof (key));
			var nsKey = NSString.CreateNative (key);
			var attchm = CMGetAttachment (target.Handle, nsKey, out attachmentModeOut);
			NSString.ReleaseNative (nsKey);
			if (attchm != IntPtr.Zero)
				return Runtime.GetINativeObject<T> (attchm, false);
			return default (T);
		}

		public static T GetAttachment<T> (this ICMAttachmentBearer target, CMSampleBufferAttachmentKey key, out CMAttachmentMode attachmentModeOut) where T: class, INativeObject
		{
			return GetAttachment<T> (target, key.GetConstant (), out attachmentModeOut);
		}

		[DllImport(Constants.CoreMediaLibrary)]
		extern static void CMPropagateAttachments (/* CMAttachmentBearerRef */ IntPtr source, /* CMAttachmentBearerRef */ IntPtr destination);
		public static void PropagateAttachments (this ICMAttachmentBearer source, ICMAttachmentBearer destination)
		{
			if (source == null)
				throw new ArgumentNullException (nameof (source));
			if (destination == null)
				throw new ArgumentNullException (nameof (destination));
			CMPropagateAttachments (source.Handle, destination.Handle);
		}

		[DllImport(Constants.CoreMediaLibrary)]
		extern static void CMRemoveAllAttachments (/*CMAttachmentBearerRef*/ IntPtr target );
		public static void RemoveAllAttachments (this ICMAttachmentBearer target)
		{
			if (target == null)
				throw new ArgumentNullException (nameof (target));
			CMRemoveAllAttachments (target.Handle);
		}

		[DllImport(Constants.CoreMediaLibrary)]
		extern static void CMRemoveAttachment(/* CMAttachmentBearerRef */ IntPtr target, /* CFStringRef */ IntPtr key);
		public static void RemoveAttachment (this ICMAttachmentBearer target, string key)
		{
			if (target == null)
				throw new ArgumentNullException (nameof (target));
			if (key == null)
				throw new ArgumentNullException (nameof (key));
			var nsKey = NSString.CreateNative (key);
			CMRemoveAttachment (target.Handle, nsKey);
			NSString.ReleaseNative (nsKey);
		}

		[DllImport(Constants.CoreMediaLibrary)]
		extern static void CMSetAttachment (/* CMAttachmentBearerRef */ IntPtr target, /* CFStringRef */ IntPtr key, /* CFTypeRef */ IntPtr value,
			/* CMAttachmentMode */ CMAttachmentMode attachmentMode);
		public static void SetAttachment (this ICMAttachmentBearer target, string key, INativeObject value, CMAttachmentMode attachmentMode)
		{
			if (target == null)
				throw new ArgumentNullException (nameof (target));
			if (value == null)
				throw new ArgumentNullException (nameof (value));
			if (key == null)
				throw new ArgumentNullException (nameof (key));
			var nsKey = NSString.CreateNative (key);
			CMSetAttachment (target.Handle, nsKey, value.Handle, attachmentMode);
			NSString.ReleaseNative (nsKey);
		}

		[DllImport(Constants.CoreMediaLibrary)]
		extern static void CMSetAttachments (/* CMAttachmentBearerRef */ IntPtr target, /* CFDictionaryRef */ IntPtr theAttachments,
			/* CMAttachmentMode */ CMAttachmentMode attachmentMode );
		public static void SetAttachments (this ICMAttachmentBearer target, NSDictionary theAttachments, CMAttachmentMode attachmentMode)
		{
			if (target == null)
				throw new ArgumentNullException (nameof (target));
			if (theAttachments == null)
				throw new ArgumentNullException (nameof (theAttachments));
			CMSetAttachments (target.Handle, theAttachments.Handle, attachmentMode);
		}
	}
}
