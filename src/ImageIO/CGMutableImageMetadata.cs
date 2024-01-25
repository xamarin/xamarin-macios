//
// CGMutableImageMetadata.cs
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2013-2014, Xamarin Inc.
//

#nullable enable

using System;
using System.Runtime.InteropServices;

using CoreFoundation;

using Foundation;

using ObjCRuntime;

namespace ImageIO {

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class CGMutableImageMetadata : CGImageMetadata {

		[DllImport (Constants.ImageIOLibrary)]
		extern static /* CGMutableImageMetadataRef __nonnull */ IntPtr CGImageMetadataCreateMutable ();

		public CGMutableImageMetadata ()
			: base (CGImageMetadataCreateMutable (), true)
		{
		}

		[DllImport (Constants.ImageIOLibrary)]
		extern static /* CGMutableImageMetadataRef __nullable */ IntPtr CGImageMetadataCreateMutableCopy (
			/* CGImageMetadataRef __nonnull */ IntPtr metadata);

		public CGMutableImageMetadata (CGImageMetadata metadata)
			: base (CGImageMetadataCreateMutableCopy (Runtime.ThrowOnNull (metadata, nameof (metadata)).Handle), true)
		{
			if (metadata is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (metadata));
		}

		[DllImport (Constants.ImageIOLibrary)]
		unsafe extern static byte CGImageMetadataRegisterNamespaceForPrefix (
			/* CGMutableImageMetadataRef __nonnull */ IntPtr metadata, /* CFStringRef __nonnull */ IntPtr xmlns,
			/* CFStringRef __nonnull */ IntPtr prefix, /* CFErrorRef __nullable */ IntPtr* error);

		public bool RegisterNamespace (NSString xmlns, NSString prefix, out NSError? error)
		{
			if (xmlns is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (xmlns));
			if (prefix is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (prefix));
			byte result;
			IntPtr err;
			unsafe {
				result = CGImageMetadataRegisterNamespaceForPrefix (Handle, xmlns.Handle, prefix.Handle, &err);
			}
			error = Runtime.GetNSObject<NSError> (err);
			return result != 0;
		}

		[DllImport (Constants.ImageIOLibrary)]
		extern static byte CGImageMetadataSetTagWithPath (/* CGMutableImageMetadataRef __nonnull */ IntPtr metadata,
			/* CGImageMetadataTagRef __nullable */ IntPtr parent, /* CFStringRef __nonnull */ IntPtr path,
			/* CGImageMetadataTagRef __nonnull */ IntPtr tag);

		public bool SetTag (CGImageMetadataTag? parent, NSString path, CGImageMetadataTag tag)
		{
			if (path is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (path));
			if (tag is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (tag));
			return CGImageMetadataSetTagWithPath (Handle, parent.GetHandle (), path.Handle, tag.Handle) != 0;
		}

		[DllImport (Constants.ImageIOLibrary)]
		extern static byte CGImageMetadataSetValueWithPath (/* CGMutableImageMetadataRef __nonnull */ IntPtr metadata,
			/* CGImageMetadataTagRef __nullable */ IntPtr parent, /* CFStringRef __nonnull */ IntPtr path,
			/* CFTypeRef __nonnull */ IntPtr value);

		public bool SetValue (CGImageMetadataTag? parent, NSString path, NSObject value)
		{
			if (value is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));
			return SetValue (parent, path, value.Handle);
		}

		public bool SetValue (CGImageMetadataTag? parent, NSString path, bool value)
		{
			return SetValue (parent, path, value ? CFBoolean.TrueHandle : CFBoolean.FalseHandle);
		}

		bool SetValue (CGImageMetadataTag? parent, NSString path, IntPtr value)
		{
			if (path is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (path));
			return CGImageMetadataSetValueWithPath (Handle, parent.GetHandle (), path.Handle, value) != 0;
		}

		[DllImport (Constants.ImageIOLibrary)]
		extern static byte CGImageMetadataRemoveTagWithPath (/* CGMutableImageMetadataRef __nonnull */ IntPtr metadata,
			/* CGImageMetadataTagRef __nullable */ IntPtr parent, /* CFStringRef __nonnull */ IntPtr path);

		public bool RemoveTag (CGImageMetadataTag? parent, NSString path)
		{
			if (path is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (path));
			return CGImageMetadataRemoveTagWithPath (Handle, parent.GetHandle (), path.Handle) != 0;
		}

		[DllImport (Constants.ImageIOLibrary)]
		extern static byte CGImageMetadataSetValueMatchingImageProperty (
			/* CGMutableImageMetadataRef __nonnull */ IntPtr metadata,
			/* CFStringRef __nonnull */ IntPtr dictionaryName, /* CFStringRef __nonnull */ IntPtr propertyName,
			/* CFTypeRef __nonnull */ IntPtr value);

		public bool SetValueMatchingImageProperty (NSString dictionaryName, NSString propertyName, NSObject value)
		{
			if (value is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));
			return SetValueMatchingImageProperty (dictionaryName, propertyName, value.Handle);
		}

		public bool SetValueMatchingImageProperty (NSString dictionaryName, NSString propertyName, bool value)
		{
			return SetValueMatchingImageProperty (dictionaryName, propertyName, value ? CFBoolean.TrueHandle : CFBoolean.FalseHandle);
		}

		bool SetValueMatchingImageProperty (NSString dictionaryName, NSString propertyName, IntPtr value)
		{
			if (dictionaryName is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (dictionaryName));
			if (propertyName is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (propertyName));
			return CGImageMetadataSetValueMatchingImageProperty (Handle, dictionaryName.Handle, propertyName.Handle, value) != 0;
		}
	}
}
