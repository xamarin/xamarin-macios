//
// CGMutableImageMetadata.cs
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2013-2014, Xamarin Inc.
//

using System;
using System.Runtime.InteropServices;

using CoreFoundation;
using Foundation;
using ObjCRuntime;

namespace ImageIO {

	[iOS (7,0), Mac (10,8)]
	public class CGMutableImageMetadata : CGImageMetadata {

		[DllImport (Constants.ImageIOLibrary)]
		extern static /* CGMutableImageMetadataRef __nonnull */ IntPtr CGImageMetadataCreateMutable ();

		public CGMutableImageMetadata () : 
			base (CGImageMetadataCreateMutable ())
		{
		}

		[DllImport (Constants.ImageIOLibrary)]
		extern static /* CGMutableImageMetadataRef __nullable */ IntPtr CGImageMetadataCreateMutableCopy (
			/* CGImageMetadataRef __nonnull */ IntPtr metadata);

		public CGMutableImageMetadata (CGImageMetadata metadata) :
			base (CGImageMetadataCreateMutableCopy (metadata.Handle))
		{
			if (metadata == null)
				throw new ArgumentNullException ("metadata");
		}

		[DllImport (Constants.ImageIOLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static bool CGImageMetadataRegisterNamespaceForPrefix (
			/* CGMutableImageMetadataRef __nonnull */ IntPtr metadata, /* CFStringRef __nonnull */ IntPtr xmlns,
			/* CFStringRef __nonnull */ IntPtr prefix, /* CFErrorRef __nullable */ out IntPtr error);

		public bool RegisterNamespace (NSString xmlns, NSString prefix, out NSError error)
		{
			if (xmlns == null)
				throw new ArgumentNullException ("xmlns");
			if (prefix == null)
				throw new ArgumentNullException ("prefix");
			IntPtr err;
			bool result = CGImageMetadataRegisterNamespaceForPrefix (Handle, xmlns.Handle, prefix.Handle, out err);
			error = err == IntPtr.Zero ? null : new NSError (err);
			return result;
		}

		[DllImport (Constants.ImageIOLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static bool CGImageMetadataSetTagWithPath (/* CGMutableImageMetadataRef __nonnull */ IntPtr metadata,
			/* CGImageMetadataTagRef __nullable */ IntPtr parent, /* CFStringRef __nonnull */ IntPtr path,
			/* CGImageMetadataTagRef __nonnull */ IntPtr tag);

		public bool SetTag (CGImageMetadataTag parent, NSString path, CGImageMetadataTag tag)
		{
			IntPtr p = parent == null ? IntPtr.Zero : parent.Handle;
			if (path == null)
				throw new ArgumentNullException ("path");
			if (tag == null)
				throw new ArgumentNullException ("tag");
			return CGImageMetadataSetTagWithPath (Handle, p, path.Handle, tag.Handle);
		}

		[DllImport (Constants.ImageIOLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static bool CGImageMetadataSetValueWithPath (/* CGMutableImageMetadataRef __nonnull */ IntPtr metadata,
			/* CGImageMetadataTagRef __nullable */ IntPtr parent, /* CFStringRef __nonnull */ IntPtr path,
			/* CFTypeRef __nonnull */ IntPtr value);

		public bool SetValue (CGImageMetadataTag parent, NSString path, NSObject value)
		{
			if (value == null)
				throw new ArgumentNullException ("value");
			return SetValue (parent, path, value.Handle);
		}

		public bool SetValue (CGImageMetadataTag parent, NSString path, bool value)
		{
			return SetValue (parent, path, value ? CFBoolean.True.Handle : CFBoolean.False.Handle);
		}

		bool SetValue (CGImageMetadataTag parent, NSString path, IntPtr value)
		{
			IntPtr p = parent == null ? IntPtr.Zero : parent.Handle;
			if (path == null)
				throw new ArgumentNullException ("path");
			return CGImageMetadataSetValueWithPath (Handle, p, path.Handle, value);
		}

		[DllImport (Constants.ImageIOLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static bool CGImageMetadataRemoveTagWithPath (/* CGMutableImageMetadataRef __nonnull */ IntPtr metadata,
			/* CGImageMetadataTagRef __nullable */ IntPtr parent, /* CFStringRef __nonnull */ IntPtr path);

		public bool RemoveTag (CGImageMetadataTag parent, NSString path)
		{
			IntPtr p = parent == null ? IntPtr.Zero : parent.Handle;
			if (path == null)
				throw new ArgumentNullException ("path");
			return CGImageMetadataRemoveTagWithPath (Handle, p, path.Handle);
		}

		[DllImport (Constants.ImageIOLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static bool CGImageMetadataSetValueMatchingImageProperty (
			/* CGMutableImageMetadataRef __nonnull */ IntPtr metadata,
			/* CFStringRef __nonnull */ IntPtr dictionaryName, /* CFStringRef __nonnull */ IntPtr propertyName,
			/* CFTypeRef __nonnull */ IntPtr value);

		public bool SetValueMatchingImageProperty (NSString dictionaryName, NSString propertyName, NSObject value)
		{
			if (value == null)
				throw new ArgumentNullException ("value");
			return SetValueMatchingImageProperty (dictionaryName, propertyName, value.Handle);
		}

		public bool SetValueMatchingImageProperty (NSString dictionaryName, NSString propertyName, bool value)
		{
			return SetValueMatchingImageProperty (dictionaryName, propertyName, value ? CFBoolean.True.Handle : CFBoolean.False.Handle);
		}

		bool SetValueMatchingImageProperty (NSString dictionaryName, NSString propertyName, IntPtr value)
		{
			if (dictionaryName == null)
				throw new ArgumentNullException ("dictionaryName");
			if (propertyName == null)
				throw new ArgumentNullException ("propertyName");
			return CGImageMetadataSetValueMatchingImageProperty (Handle, dictionaryName.Handle, propertyName.Handle, value);
		}
	}
}