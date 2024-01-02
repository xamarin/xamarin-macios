//
// Copyright 2013-2014, Xamarin Inc.
//
// Authors:
//   Miguel de Icaza
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
using System.ComponentModel;
using System.Runtime.InteropServices;

using ObjCRuntime;
using Foundation;
using CoreFoundation;
using CoreGraphics;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace ImageIO {

	public partial class CGImageDestinationOptions {
		CGColor? destinationBackgroundColor;
		public CGColor? DestinationBackgroundColor {
			get { return destinationBackgroundColor; }
			set {
				destinationBackgroundColor = value;
				(Dictionary as NSMutableDictionary)?.LowlevelSetObject (destinationBackgroundColor.GetHandle (), CGImageDestinationOptionsKeys.BackgroundColor.Handle);
			}
		}

		internal NSMutableDictionary ToDictionary ()
		{
			return (NSMutableDictionary) Dictionary;
		}
	}

	public partial class CGCopyImageSourceOptions {
#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
#endif
		public CGImageMetadata? Metadata { get; set; }

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
#endif
		public bool MergeMetadata { get; set; }

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
#endif
		public bool ShouldExcludeXMP { get; set; }

#if NET
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		public bool ShouldExcludeGPS { get; set; }

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
#endif
		public DateTime? DateTime { get; set; }

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
#endif
		public int? Orientation { get; set; }

		internal NSMutableDictionary ToDictionary ()
		{
			var dict = new NSMutableDictionary ();

			// new in iOS 7 and 10.8
			if (Metadata is not null) {
				dict.LowlevelSetObject (Metadata.Handle, kMetadata);
				// default are false
				if (MergeMetadata)
					dict.LowlevelSetObject (CFBoolean.TrueHandle, kMergeMetadata);
				if (ShouldExcludeXMP)
					dict.LowlevelSetObject (CFBoolean.TrueHandle, kShouldExcludeXMP);
			} else {
				// DateTime is exclusive of metadata (which includes its own)
				if (DateTime.HasValue)
					dict.LowlevelSetObject ((NSDate) DateTime, kDateTime);
			}

			// new in iOS 8 and 10.10 - default is false
			if (ShouldExcludeGPS && (kShouldExcludeGPS != IntPtr.Zero))
				dict.LowlevelSetObject (CFBoolean.TrueHandle, kShouldExcludeGPS);

			if (Orientation.HasValue) {
				using (var n = new NSNumber (Orientation.Value))
					dict.LowlevelSetObject (n.Handle, kOrientation);
			}

			return dict;
		}
	}

	public partial class CGImageAuxiliaryDataInfo {

		public CGImageMetadata? Metadata {
			get {
				return GetNativeValue<CGImageMetadata> (CGImageAuxiliaryDataInfoKeys.MetadataKey);
			}
			set {
				SetNativeValue (CGImageAuxiliaryDataInfoKeys.MetadataKey, value);
			}
		}
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class CGImageDestination : NativeObject {
#if !NET
		internal CGImageDestination (NativeHandle handle)
			: base (handle, false)
		{
		}
#endif

		[Preserve (Conditional = true)]
		internal CGImageDestination (NativeHandle handle, bool owns)
			: base (handle, owns)
		{
		}

		[DllImport (Constants.ImageIOLibrary, EntryPoint = "CGImageDestinationGetTypeID")]
		public extern static /* CFTypeID */ nint GetTypeID ();

		[DllImport (Constants.ImageIOLibrary)]
		extern static /* CFArrayRef __nonnull */ IntPtr CGImageDestinationCopyTypeIdentifiers ();

		public static string? []? TypeIdentifiers {
			get {
				var handle = CGImageDestinationCopyTypeIdentifiers ();
				return CFArray.StringArrayFromHandle (handle, true);
			}
		}

		[DllImport (Constants.ImageIOLibrary)]
		extern static /* CGImageDestinationRef __nullable */ IntPtr CGImageDestinationCreateWithDataConsumer (
			/* CGDataConsumerRef __nonnull */ IntPtr consumer, /* CFStringRef __nonnull */ IntPtr type,
			/* size_t */ nint count, /* CFDictionaryRef __nullable */ IntPtr options);

		public static CGImageDestination? Create (CGDataConsumer consumer, string typeIdentifier, int imageCount, CGImageDestinationOptions? options = null)
		{
			if (consumer is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (consumer));
			if (typeIdentifier is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (typeIdentifier));

			using var dict = options?.ToDictionary ();
			var typeId = CFString.CreateNative (typeIdentifier);
			try {
				IntPtr p = CGImageDestinationCreateWithDataConsumer (consumer.Handle, typeId, imageCount, dict.GetHandle ());
				return p == IntPtr.Zero ? null : new CGImageDestination (p, true);
			} finally {
				CFString.ReleaseNative (typeId);
			}
		}

		[DllImport (Constants.ImageIOLibrary)]
		extern static /* CGImageDestinationRef __nullable */ IntPtr CGImageDestinationCreateWithData (
			/* CFMutableDataRef __nonnull */ IntPtr data, /* CFStringRef __nonnull */ IntPtr stringType,
			/* size_t */ nint count, /* CFDictionaryRef __nullable */ IntPtr options);

		public static CGImageDestination? Create (NSMutableData data, string typeIdentifier, int imageCount, CGImageDestinationOptions? options = null)
		{
			if (data is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (data));
			if (typeIdentifier is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (typeIdentifier));

			using var dict = options?.ToDictionary ();
			var typeId = CFString.CreateNative (typeIdentifier);
			try {
				IntPtr p = CGImageDestinationCreateWithData (data.Handle, typeId, imageCount, dict.GetHandle ());
				return p == IntPtr.Zero ? null : new CGImageDestination (p, true);
			} finally {
				CFString.ReleaseNative (typeId);
			}
		}

		[DllImport (Constants.ImageIOLibrary)]
		extern static /* CGImageDestinationRef __nullable */ IntPtr CGImageDestinationCreateWithURL (
			/* CFURLRef __nonnull */ IntPtr url, /* CFStringRef __nonnull */ IntPtr stringType,
			/* size_t */ nint count, /* CFDictionaryRef __nullable */ IntPtr options);

		public static CGImageDestination? Create (NSUrl url, string typeIdentifier, int imageCount)
		{
			if (url is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (url));
			if (typeIdentifier is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (typeIdentifier));

			var typeId = CFString.CreateNative (typeIdentifier);
			try {
				IntPtr p = CGImageDestinationCreateWithURL (url.Handle, typeId, imageCount, IntPtr.Zero);
				return p == IntPtr.Zero ? null : new CGImageDestination (p, true);
			} finally {
				CFString.ReleaseNative (typeId);
			}
		}

		[DllImport (Constants.ImageIOLibrary)]
		extern static void CGImageDestinationSetProperties (/* CGImageDestinationRef __nonnull */ IntPtr idst,
			/* CFDictionaryRef __nullable */ IntPtr properties);

		public void SetProperties (NSDictionary? properties)
		{
			CGImageDestinationSetProperties (Handle, properties.GetHandle ());
		}

		[DllImport (Constants.ImageIOLibrary)]
		extern static void CGImageDestinationAddImage (/* CGImageDestinationRef __nonnull */ IntPtr idst,
			/* CGImageRef __nonnull */ IntPtr image,
			/* CFDictionaryRef __nullable */ IntPtr properties);

		public void AddImage (CGImage image, CGImageDestinationOptions? options = null)
		{
			if (image is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (image));

			using var dict = options?.ToDictionary ();
			CGImageDestinationAddImage (Handle, image.Handle, dict.GetHandle ());
		}

		public void AddImage (CGImage image, NSDictionary? properties)
		{
			if (image is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (image));

			CGImageDestinationAddImage (Handle, image.Handle, properties.GetHandle ());
		}

		[DllImport (Constants.ImageIOLibrary)]
		extern static void CGImageDestinationAddImageFromSource (/* CGImageDestinationRef __nonnull */ IntPtr idst,
			/* CGImageSourceRef __nonnull */ IntPtr sourceHandle, /* size_t */ nint index,
			/* CFDictionaryRef __nullable */ IntPtr properties);

		public void AddImage (CGImageSource source, int index, CGImageDestinationOptions? options = null)
		{
			if (source is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (source));

			using var dict = options?.ToDictionary ();
			CGImageDestinationAddImageFromSource (Handle, source.Handle, index, dict.GetHandle ());
		}

		public void AddImage (CGImageSource source, int index, NSDictionary? properties)
		{
			if (source is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (source));

			CGImageDestinationAddImageFromSource (Handle, source.Handle, index, properties.GetHandle ());
		}

		[DllImport (Constants.ImageIOLibrary)]
		extern static byte CGImageDestinationFinalize (/* CGImageDestinationRef __nonnull */ IntPtr idst);

		public bool Close ()
		{
			var success = CGImageDestinationFinalize (Handle);
			Dispose ();
			return success != 0;
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
#endif
		[DllImport (Constants.ImageIOLibrary)]
		extern static void CGImageDestinationAddImageAndMetadata (/* CGImageDestinationRef __nonnull */ IntPtr idst,
			/* CGImageRef __nonnull */ IntPtr image, /* CGImageMetadataRef __nullable */ IntPtr metadata,
			/* CFDictionaryRef __nullable */ IntPtr options);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
#endif
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		public void AddImageAndMetadata (CGImage image, CGImageMetadata meta, NSDictionary? options)
		{
			if (image is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (image));
			CGImageDestinationAddImageAndMetadata (Handle, image.Handle, meta.GetHandle (), options.GetHandle ());
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
#endif
		public void AddImageAndMetadata (CGImage image, CGImageMetadata meta, CGImageDestinationOptions? options)
		{
			using var o = options?.ToDictionary ();
			AddImageAndMetadata (image, meta, o);
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
#endif
		[DllImport (Constants.ImageIOLibrary)]
		unsafe extern static byte CGImageDestinationCopyImageSource (/* CGImageDestinationRef __nonnull */ IntPtr idst,
			/* CGImageSourceRef __nonnull */ IntPtr image, /* CFDictionaryRef __nullable */ IntPtr options,
			/* CFErrorRef* */ IntPtr* err);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
#endif
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		public bool CopyImageSource (CGImageSource image, NSDictionary? options, out NSError? error)
		{
			if (image is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (image));
			byte result;
			IntPtr err;
			unsafe {
				result = CGImageDestinationCopyImageSource (Handle, image.Handle, options.GetHandle (), &err);
			}
			error = Runtime.GetNSObject<NSError> (err);
			return result != 0;
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
#endif
		public bool CopyImageSource (CGImageSource image, CGCopyImageSourceOptions? options, out NSError? error)
		{
			using var o = options?.ToDictionary ();
			return CopyImageSource (image, o, out error);
		}

#if NET
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		[DllImport (Constants.ImageIOLibrary)]
		static extern void CGImageDestinationAddAuxiliaryDataInfo (IntPtr /* CGImageDestinationRef* */ idst, IntPtr /* CFStringRef* */ auxiliaryImageDataType, IntPtr /* CFDictionaryRef* */ auxiliaryDataInfoDictionary);

#if NET
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		public void AddAuxiliaryDataInfo (CGImageAuxiliaryDataType auxiliaryImageDataType, CGImageAuxiliaryDataInfo? auxiliaryDataInfo)
		{
			using (var dict = auxiliaryDataInfo?.Dictionary) {
				CGImageDestinationAddAuxiliaryDataInfo (Handle, auxiliaryImageDataType.GetConstant ().GetHandle (), dict.GetHandle ());
			}
		}
	}
}
