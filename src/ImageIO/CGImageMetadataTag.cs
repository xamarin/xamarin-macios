//
// CGImageMetadataTag.cs
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
using ObjCRuntime;
using Foundation;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace ImageIO {

	// CGImageMetadata.h
#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class CGImageMetadataTag : NativeObject {

		// note: CGImageMetadataType is always an int (4 bytes) so it's ok to use in the pinvoke declaration
		[DllImport (Constants.ImageIOLibrary)]
		extern static /* CGImageMetadataTagRef __nullable */ IntPtr CGImageMetadataTagCreate (
			/* CFStringRef __nonnull */ IntPtr xmlns, /* CFStringRef __nullable */ IntPtr prefix,
			/* CFStringRef __nonnull */ IntPtr name, CGImageMetadataType type, /* CFTypeRef __nonnull */ IntPtr value);

#if !NET
		public CGImageMetadataTag (NativeHandle handle)
			: base (handle, false)
		{
		}
#endif

		[Preserve (Conditional = true)]
		internal CGImageMetadataTag (NativeHandle handle, bool owns)
			: base (handle, owns)
		{
		}

		// According to header file the CFType value can be:
		// CFStringRef	-> NSString (NSObject)
		// CFNumberRef	-> NSNumber (NSObject)
		// CFBooleanRef	-> no direct mapping
		// CFArrayRef	-> NSArray (NSObject)
		// CFDictionary	-> NSDictionary (NSObject)

		public CGImageMetadataTag (NSString xmlns, NSString? prefix, NSString name, CGImageMetadataType type, NSObject? value) :
			this (xmlns, prefix, name, type, value.GetHandle ())
		{
		}

		// CFBoolean support
		public CGImageMetadataTag (NSString xmlns, NSString? prefix, NSString name, CGImageMetadataType type, bool value) :
			this (xmlns, prefix, name, type, value ? CFBoolean.TrueHandle : CFBoolean.FalseHandle)
		{
		}

		CGImageMetadataTag (NSString xmlns, NSString? prefix, NSString name, CGImageMetadataType type, IntPtr value)
		{
			if (xmlns is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (xmlns));
			if (name is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (name));
			// it won't crash - but the instance is invalid (null handle)
			if (value == IntPtr.Zero)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (value));

			InitializeHandle (CGImageMetadataTagCreate (xmlns.Handle, prefix.GetHandle (), name.Handle, type, value));
		}

		[DllImport (Constants.ImageIOLibrary, EntryPoint = "CGImageMetadataTagGetTypeID")]
		public extern static nint GetTypeID ();


		[DllImport (Constants.ImageIOLibrary)]
		extern static /* CFStringRef __nullable */ IntPtr CGImageMetadataTagCopyNamespace (
			/* CGImageMetadataTagRef __nonnull */ IntPtr tag);

		public NSString? Namespace {
			get {
				var result = CGImageMetadataTagCopyNamespace (Handle);
				return Runtime.GetNSObject<NSString> (result, true);
			}
		}

		[DllImport (Constants.ImageIOLibrary)]
		extern static /* CFStringRef __nullable */ IntPtr CGImageMetadataTagCopyPrefix (
			/* CGImageMetadataTagRef __nonnull */ IntPtr tag);

		public NSString? Prefix {
			get {
				var result = CGImageMetadataTagCopyPrefix (Handle);
				return Runtime.GetNSObject<NSString> (result, true);
			}
		}

		[DllImport (Constants.ImageIOLibrary)]
		extern static /* CFStringRef __nullable */ IntPtr CGImageMetadataTagCopyName (
			/* CGImageMetadataTagRef __nonnull */ IntPtr tag);

		public NSString? Name {
			get {
				var result = CGImageMetadataTagCopyName (Handle);
				return Runtime.GetNSObject<NSString> (result, true);
			}
		}

		[DllImport (Constants.ImageIOLibrary)]
		extern static /* CFTypeRef __nullable */ IntPtr CGImageMetadataTagCopyValue (
			/* CGImageMetadataTagRef __nonnull */ IntPtr tag);

		// a boolean is returned as a NSString, i.e. type CGImageMetadataType.String, so NSObject is fine
		public NSObject? Value {
			get { return Runtime.GetNSObject<NSObject> (CGImageMetadataTagCopyValue (Handle), true); }
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

		public CGImageMetadataTag? []? GetQualifiers ()
		{
			IntPtr result = CGImageMetadataTagCopyQualifiers (Handle);
			return CFArray.ArrayFromHandle<CGImageMetadataTag> (result, true);
		}
	}
}
