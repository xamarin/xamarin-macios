//
// CGImageMetadataTag.cs
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2013-2014, Xamarin Inc.
//

using System;
using System.Runtime.InteropServices;

using CoreFoundation;
using ObjCRuntime;
using Foundation;

namespace ImageIO {

	// CGImageMetadata.h
	[iOS (7,0), Mac (10,8)]
	public class CGImageMetadataTag : INativeObject, IDisposable {

		// note: CGImageMetadataType is always an int (4 bytes) so it's ok to use in the pinvoke declaration
		[DllImport (Constants.ImageIOLibrary)]
		extern static /* CGImageMetadataTagRef __nullable */ IntPtr CGImageMetadataTagCreate (
			/* CFStringRef __nonnull */ IntPtr xmlns, /* CFStringRef __nullable */ IntPtr prefix,
			/* CFStringRef __nonnull */ IntPtr name, CGImageMetadataType type, /* CFTypeRef __nonnull */ IntPtr value);

		public CGImageMetadataTag (IntPtr handle)
			: this (handle, false)
		{
		}

		[Preserve (Conditional=true)]
		internal CGImageMetadataTag (IntPtr handle, bool owns)
		{
			if (handle == IntPtr.Zero)
				throw new Exception ("Invalid handle");

			Handle = handle;
			if (!owns)
				CFObject.CFRetain (handle);
		}

		// According to header file the CFType value can be:
		// CFStringRef	-> NSString (NSObject)
		// CFNumberRef	-> NSNumber (NSObject)
		// CFBooleanRef	-> no direct mapping
		// CFArrayRef	-> NSArray (NSObject)
		// CFDictionary	-> NSDictionary (NSObject)

		public CGImageMetadataTag (NSString xmlns, NSString prefix, NSString name, CGImageMetadataType type, NSObject value) :
			this (xmlns, prefix, name, type, value == null ? IntPtr.Zero : value.Handle)
		{
		}

		// CFBoolean support
		public CGImageMetadataTag (NSString xmlns, NSString prefix, NSString name, CGImageMetadataType type, bool value) :
			this (xmlns, prefix, name, type, value ? CFBoolean.TrueHandle : CFBoolean.FalseHandle)
		{
		}

		CGImageMetadataTag (NSString xmlns, NSString prefix, NSString name, CGImageMetadataType type, IntPtr value)
		{
			if (xmlns == null)
				throw new ArgumentNullException ("xmlns");
			if (name == null)
				throw new ArgumentNullException ("name");
			// it won't crash - but the instance is invalid (null handle)
			if (value == IntPtr.Zero)
				throw new ArgumentNullException ("value");

			var p = (prefix == null) ? IntPtr.Zero : prefix.Handle;
			Handle = CGImageMetadataTagCreate (xmlns.Handle, p, name.Handle, type, value);
		}

		public IntPtr Handle { get; internal set; }

		~CGImageMetadataTag ()
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

		[DllImport (Constants.ImageIOLibrary, EntryPoint="CGImageMetadataTagGetTypeID")]
		public extern static nint GetTypeID ();


		[DllImport (Constants.ImageIOLibrary)]
		extern static /* CFStringRef __nullable */ IntPtr CGImageMetadataTagCopyNamespace (
			/* CGImageMetadataTagRef __nonnull */ IntPtr tag);

		public NSString Namespace {
			get {
				var result = CGImageMetadataTagCopyNamespace (Handle);
				return result == IntPtr.Zero ? null : new NSString (result, true);
			}
		}

		[DllImport (Constants.ImageIOLibrary)]
		extern static /* CFStringRef __nullable */ IntPtr CGImageMetadataTagCopyPrefix (
			/* CGImageMetadataTagRef __nonnull */ IntPtr tag);

		public NSString Prefix {
			get {
				var result = CGImageMetadataTagCopyPrefix (Handle);
				return result == IntPtr.Zero ? null : new NSString (result, true);
			}
		}

		[DllImport (Constants.ImageIOLibrary)]
		extern static /* CFStringRef __nullable */ IntPtr CGImageMetadataTagCopyName (
			/* CGImageMetadataTagRef __nonnull */ IntPtr tag);

		public NSString Name {
			get {
				var result = CGImageMetadataTagCopyName (Handle);
				return result == IntPtr.Zero ? null : new NSString (result, true);
			}
		}

		[DllImport (Constants.ImageIOLibrary)]
		extern static /* CFTypeRef __nullable */ IntPtr CGImageMetadataTagCopyValue (
			/* CGImageMetadataTagRef __nonnull */ IntPtr tag);

		// a boolean is returned as a NSString, i.e. type CGImageMetadataType.String, so NSObject is fine
		public NSObject Value {
			get { return Runtime.GetNSObject (CGImageMetadataTagCopyValue (Handle)); }
		}

		// note: CGImageMetadataType is always an int (4 bytes) so it's ok to use in the pinvoke declaration
		[DllImport (Constants.ImageIOLibrary)]
		extern static CGImageMetadataType CGImageMetadataTagGetType (/* CGImageMetadataTagRef __nonnull */ IntPtr tag);

		public CGImageMetadataType Type {
			get { return CGImageMetadataTagGetType (Handle); }
		}

		[DllImport (Constants.ImageIOLibrary)]
		extern static /* CFArrayRef __nullable */ IntPtr CGImageMetadataTagCopyQualifiers (
			/* CGImageMetadataTagRef __nonnull */ IntPtr tag);

		public CGImageMetadataTag[] GetQualifiers ()
		{
			IntPtr result = CGImageMetadataTagCopyQualifiers (Handle);
			if (result == IntPtr.Zero)
				return null;
			using (var a = new CFArray (result)) {
				CGImageMetadataTag[] tags = new CGImageMetadataTag [a.Count];
				for (int i = 0; i < a.Count; i++)
					tags [i] = new CGImageMetadataTag (a.GetValue (i), true);
				return tags;
			}
		}
	}
}
