//
// CGImageMetadata.cs
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2013-2014, Xamarin Inc.
//

using System;
using System.Runtime.InteropServices;

using ObjCRuntime;
using Foundation;
using CoreFoundation;

namespace ImageIO {

	[iOS (7,0), Mac (10,8)]
	public partial class CGImageMetadataEnumerateOptions {

		public bool Recursive { get; set; }

		internal NSMutableDictionary ToDictionary ()
		{
			var dict = new NSMutableDictionary ();

			if (Recursive)
				dict.LowlevelSetObject (CFBoolean.True.Handle, kCGImageMetadataEnumerateRecursively);

			return dict;
		}
	}

	public delegate bool CGImageMetadataTagBlock (NSString path, CGImageMetadataTag tag);

	// CGImageMetadata.h
	[iOS (7,0), Mac (10,8)]
	public partial class CGImageMetadata : INativeObject, IDisposable {

		public CGImageMetadata (IntPtr handle)
		{
			Handle = handle;
		}

		[Preserve (Conditional = true)]
		internal CGImageMetadata (IntPtr handle, bool owns)
		{
			Handle = handle;
			if (!owns)
				CFObject.CFRetain (Handle);
		}

		[DllImport (Constants.ImageIOLibrary)]
		static extern /* CGImageMetadataRef __nullable */ IntPtr CGImageMetadataCreateFromXMPData (
			/* CFDataRef __nonnull */ IntPtr data);

		public CGImageMetadata (NSData data)
		{
			if (data == null)
				throw new ArgumentNullException ("data");

			Handle = CGImageMetadataCreateFromXMPData (data.Handle);
			if (Handle == IntPtr.Zero)
				throw new ArgumentException ("data");
		}

		public IntPtr Handle { get; internal set; }

		~CGImageMetadata ()
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
				CFObject.CFRelease (Handle);
				Handle = IntPtr.Zero;
			}
		}

		[DllImport (Constants.ImageIOLibrary, EntryPoint="CGImageMetadataGetTypeID")]
		public extern static /* CFTypeID */ nint GetTypeID ();


		[DllImport (Constants.ImageIOLibrary)]
		extern static /* CFStringRef __nullable */ IntPtr CGImageMetadataCopyStringValueWithPath (
			/* CGImageMetadataRef __nonnull */ IntPtr metadata, /* CGImageMetadataTagRef __nullable */ IntPtr parent,
			/* CFStringRef __nonnull*/ IntPtr path);

		public NSString GetStringValue (CGImageMetadata parent, NSString path)
		{
			// parent may be null
			if (path == null)
				throw new ArgumentNullException ("path");
			IntPtr p = parent == null ? IntPtr.Zero : parent.Handle;
			IntPtr result = CGImageMetadataCopyStringValueWithPath (Handle, p, path.Handle);
			return (result == IntPtr.Zero) ? null : new NSString (result);
		}

		[DllImport (Constants.ImageIOLibrary)]
		extern static /* CFArrayRef __nullable */ IntPtr CGImageMetadataCopyTags (
			/* CGImageMetadataRef __nonnull */ IntPtr metadata);

		public CGImageMetadataTag [] GetTags ()
		{
			IntPtr result = CGImageMetadataCopyTags (Handle);
			if (result == IntPtr.Zero)
				return null;
			using (var a = new CFArray (result)) {
				CGImageMetadataTag[] tags = new CGImageMetadataTag [a.Count];
				for (int i = 0; i < a.Count; i++)
					tags [i] = new CGImageMetadataTag (a.GetValue (i));
				return tags;
			}
		}

		[DllImport (Constants.ImageIOLibrary)]
		extern static /* CGImageMetadataTagRef __nullable */ IntPtr CGImageMetadataCopyTagWithPath (
			/* CGImageMetadataRef __nonnull */ IntPtr metadata, /* CGImageMetadataTagRef __nullable */ IntPtr parent,
			/* CFStringRef __nonnull */ IntPtr path);

		public CGImageMetadataTag GetTag (CGImageMetadata parent, NSString path)
		{
			// parent may be null
			if (path == null)
				throw new ArgumentNullException ("path");
			IntPtr p = parent == null ? IntPtr.Zero : parent.Handle;
			IntPtr result = CGImageMetadataCopyTagWithPath (Handle, p, path.Handle);
			return (result == IntPtr.Zero) ? null : new CGImageMetadataTag (result);
		}

		[DllImport (Constants.ImageIOLibrary)]
		extern static void CGImageMetadataEnumerateTagsUsingBlock (/* CGImageMetadataRef __nonnull */ IntPtr metadata,
			/* CFStringRef __nullable */ IntPtr rootPath, /* CFDictionaryRef __nullable */ IntPtr options,
			/* __nonnull */ CGImageMetadataTagBlock block);

		public void EnumerateTags (NSString rootPath, CGImageMetadataEnumerateOptions options, CGImageMetadataTagBlock block)
		{
			IntPtr r = rootPath == null ? IntPtr.Zero : rootPath.Handle;
			NSDictionary o = null;
			if (options != null)
				o = options.ToDictionary ();
			CGImageMetadataEnumerateTagsUsingBlock (Handle, r, o == null ? IntPtr.Zero : o.Handle, block);
			if (options != null)
				o.Dispose ();
		}

		[DllImport (Constants.ImageIOLibrary)]
		extern static /* CFDataRef __nullable */ IntPtr CGImageMetadataCreateXMPData (
			/* CGImageMetadataRef __nonnull */ IntPtr metadata, /* CFDictionaryRef __nullable */ IntPtr options);

		public NSData CreateXMPData ()
		{
			// note: there's no options defined for iOS7 (needs to be null)
			// we'll need to add an overload if this change in the future
			IntPtr result = CGImageMetadataCreateXMPData (Handle, IntPtr.Zero);
			return result == IntPtr.Zero ? null : new NSData (result);
		}

		[DllImport (Constants.ImageIOLibrary)]
		extern static /* CGImageMetadataTagRef __nullable */ IntPtr CGImageMetadataCopyTagMatchingImageProperty (
			/* CGImageMetadataRef __nonnull */ IntPtr metadata, /* CFStringRef __nonnull */ IntPtr dictionaryName,
			/* CFStringRef __nonnull */ IntPtr propertyName);

		public CGImageMetadataTag CopyTagMatchingImageProperty (NSString dictionaryName, NSString propertyName)
		{
			if (dictionaryName == null)
				throw new ArgumentNullException ("dictionaryName");
			if (propertyName == null)
				throw new ArgumentNullException ("propertyName");
			IntPtr result = CGImageMetadataCopyTagMatchingImageProperty (Handle, dictionaryName.Handle, propertyName.Handle);
			return result == IntPtr.Zero ? null : new CGImageMetadataTag (result);
		}
	}
}
