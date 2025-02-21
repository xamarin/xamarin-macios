//
// Authors:
//   Duane Wandless
//   Miguel de Icaza
//   Sebastien Pouliot
//
// Copyright 2011-2014, Xamarin Inc.
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

#if !COREBUILD
	// untyped enum -> CGImageSource.h
	public enum CGImageSourceStatus {
		/// <summary>The image loader has completed, the full set of images is loaded.</summary>
		Complete = 0,
		/// <summary>The CGImageSource is still expecting data.</summary>
		Incomplete = -1,
		/// <summary>The CGImageSource is reading the image header information.</summary>
		ReadingHeader = -2,
		/// <summary>The CGImageSource does not have a decoder for the image.</summary>
		UnknownType = -3,
		/// <summary>The data fed to the CGImageSource is invalid and does not represent an image that can be decoded.</summary>
		InvalidData = -4,
		/// <summary>The image loader detected a premature end-of-file condition.</summary>
		UnexpectedEOF = -5,
	}

	public partial class CGImageOptions {

		public CGImageOptions ()
		{
			ShouldCache = true;
		}

		/// <summary>Provides the best guess for the file format that is going to be loaded.</summary>
		///         <value>A Uniform Type Identifier (UTI).</value>
		///         <remarks>To learn more about UTIs, you can read:
		///
		/// https://developer.apple.com/library/mac/#documentation/FileManagement/Conceptual/understanding_utis/understand_utis_intro/understand_utis_intro.html</remarks>
		public string? BestGuessTypeIdentifier { get; set; }

		/// <summary>Determines whether the loaded image should be cached.</summary>
		///         <value />
		///         <remarks>To be added.</remarks>
		public bool ShouldCache { get; set; }

#if NET
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		public bool ShouldCacheImmediately { get; set; }

		/// <summary>Determines whether the image loaded will use floating point values for its components (if the source image has them).</summary>
		///         <value />
		///         <remarks>To be added.</remarks>
		public bool ShouldAllowFloat { get; set; }

		internal virtual NSMutableDictionary ToDictionary ()
		{
			var dict = new NSMutableDictionary ();

			if (BestGuessTypeIdentifier is not null)
				dict.LowlevelSetObject (BestGuessTypeIdentifier, kTypeIdentifierHint);
			if (!ShouldCache)
				dict.LowlevelSetObject (CFBoolean.FalseHandle, kShouldCache);
			if (ShouldAllowFloat)
				dict.LowlevelSetObject (CFBoolean.TrueHandle, kShouldAllowFloat);
			if (kShouldCacheImmediately != IntPtr.Zero && ShouldCacheImmediately)
				dict.LowlevelSetObject (CFBoolean.TrueHandle, kShouldCacheImmediately);

			return dict;
		}
	}

	public partial class CGImageThumbnailOptions : CGImageOptions {

		/// <summary>Determines whether to create a thumbnail if one is not found on the image source.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public bool CreateThumbnailFromImageIfAbsent { get; set; }
		/// <summary>Forces a thumbnail to be created, even if the source image has one.</summary>
		///         <value />
		///         <remarks>The thumbnail is created subject to the value set in the MaxPixelSize property.</remarks>
		public bool CreateThumbnailFromImageAlways { get; set; }
		/// <summary>Maximum width and height allowed for a thumbnail (in pixels).</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public int? MaxPixelSize { get; set; }
		/// <summary>Determines if the created thumbnail should be rotated and scaled to match the full image.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public bool CreateThumbnailWithTransform { get; set; }

#if NET
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#endif
		public int? SubsampleFactor { get; set; }

		internal override NSMutableDictionary ToDictionary ()
		{
			var dict = base.ToDictionary ();
			IntPtr thandle = CFBoolean.TrueHandle;

			if (CreateThumbnailFromImageIfAbsent)
				dict.LowlevelSetObject (thandle, kCreateThumbnailFromImageIfAbsent);
			if (CreateThumbnailFromImageAlways)
				dict.LowlevelSetObject (thandle, kCreateThumbnailFromImageAlways);
			if (MaxPixelSize.HasValue)
				dict.LowlevelSetObject (new NSNumber (MaxPixelSize.Value), kThumbnailMaxPixelSize);
			if (CreateThumbnailWithTransform)
				dict.LowlevelSetObject (thandle, kCreateThumbnailWithTransform);
			if (SubsampleFactor.HasValue)
				dict.LowlevelSetObject (new NSNumber (SubsampleFactor.Value), kCGImageSourceSubsampleFactor);

			return dict;
		}
	}
#endif

	public partial class CGImageSource : NativeObject {
#if !COREBUILD
		[DllImport (Constants.ImageIOLibrary, EntryPoint = "CGImageSourceGetTypeID")]
		public extern static nint GetTypeID ();

		[DllImport (Constants.ImageIOLibrary)]
		extern static /* CFArrayRef __nonnull */ IntPtr CGImageSourceCopyTypeIdentifiers ();

		/// <summary>The type identifiers for the formats supported by the image loader.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public static string? []? TypeIdentifiers {
			get {
				var handle = CGImageSourceCopyTypeIdentifiers ();
				return CFArray.StringArrayFromHandle (handle, true);
			}
		}
#endif
		[Preserve (Conditional = true)]
		internal CGImageSource (NativeHandle handle, bool owns)
			: base (handle, owns)
		{
		}

#if !COREBUILD
		[DllImport (Constants.ImageIOLibrary)]
		extern static /* CGImageSourceRef __nullable */ IntPtr CGImageSourceCreateWithURL (
			/* CFURLRef __nonnull */ IntPtr url, /* CFDictionaryRef __nullable */ IntPtr options);

		public static CGImageSource? FromUrl (NSUrl url)
		{
			return FromUrl (url, null);
		}

		public static CGImageSource? FromUrl (NSUrl url, CGImageOptions? options)
		{
			if (url is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (url));

			using (var dict = options?.ToDictionary ()) {
				var result = CGImageSourceCreateWithURL (url.Handle, dict.GetHandle ());
				return result == IntPtr.Zero ? null : new CGImageSource (result, true);
			}
		}

		[DllImport (Constants.ImageIOLibrary)]
		extern static /* CGImageSourceRef __nullable */ IntPtr CGImageSourceCreateWithDataProvider (
			/* CGDataProviderRef __nonnull */ IntPtr provider, /* CFDictionaryRef __nullable */ IntPtr options);

		public static CGImageSource? FromDataProvider (CGDataProvider provider)
		{
			return FromDataProvider (provider, null);
		}

		public static CGImageSource? FromDataProvider (CGDataProvider provider, CGImageOptions? options)
		{
			if (provider is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (provider));

			using (var dict = options?.ToDictionary ()) {
				var result = CGImageSourceCreateWithDataProvider (provider.Handle, dict.GetHandle ());
				return result == IntPtr.Zero ? null : new CGImageSource (result, true);
			}
		}

		[DllImport (Constants.ImageIOLibrary)]
		extern static /* CGImageSourceRef __nullable */ IntPtr CGImageSourceCreateWithData (
			/* CFDataRef __nonnull */ IntPtr data, /* CFDictionaryRef __nullable */ IntPtr options);

		public static CGImageSource? FromData (NSData data)
		{
			return FromData (data, null);
		}

		public static CGImageSource? FromData (NSData data, CGImageOptions? options)
		{
			if (data is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (data));

			using (var dict = options?.ToDictionary ()) {
				var result = CGImageSourceCreateWithData (data.Handle, dict.GetHandle ());
				return result == IntPtr.Zero ? null : new CGImageSource (result, true);
			}
		}

		[DllImport (Constants.ImageIOLibrary)]
		extern static /* CFStringRef __nullable */ IntPtr CGImageSourceGetType (
			/* CGImageSourceRef __nonnull */ IntPtr handle);

		/// <summary>The image type of the underling image.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public string? TypeIdentifier {
			get {
				return CFString.FromHandle (CGImageSourceGetType (Handle));
			}
		}

		[DllImport (Constants.ImageIOLibrary)]
		extern static /* size_t */ nint CGImageSourceGetCount (/* CGImageSourceRef __nonnull */ IntPtr handle);

		/// <summary>Number of images loaded (does not include the Thumbnail).</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public nint ImageCount {
			get {
				return CGImageSourceGetCount (Handle);
			}
		}

		[DllImport (Constants.ImageIOLibrary)]
		extern static /* CFDictionaryRef __nullable */ IntPtr CGImageSourceCopyProperties (
			/* CGImageSourceRef __nonnull */ IntPtr isrc, /* CFDictionaryRef __nullable */ IntPtr options);

		[Advice ("Use 'GetProperties'.")]
		public NSDictionary? CopyProperties (NSDictionary? dict)
		{
			var result = CGImageSourceCopyProperties (Handle, dict.GetHandle ());
			return Runtime.GetNSObject<NSDictionary> (result, true);
		}

		[Advice ("Use 'GetProperties'.")]
		public NSDictionary? CopyProperties (CGImageOptions options)
		{
			if (options is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (options));
			using var dict = options.ToDictionary ();
			return CopyProperties (dict);
		}

		[DllImport (Constants.ImageIOLibrary)]
		extern static /* CFDictionaryRef __nullable */ IntPtr CGImageSourceCopyPropertiesAtIndex (
			/* CGImageSourceRef __nonnull */ IntPtr isrc, /* size_t */ nint index,
			/* CFDictionaryRef __nullable */ IntPtr options);

		[Advice ("Use 'GetProperties'.")]
		public NSDictionary? CopyProperties (NSDictionary? dict, int imageIndex)
		{
			var result = CGImageSourceCopyPropertiesAtIndex (Handle, imageIndex, dict.GetHandle ());
			return Runtime.GetNSObject<NSDictionary> (result, true);
		}

		[Advice ("Use 'GetProperties'.")]
		public NSDictionary? CopyProperties (CGImageOptions options, int imageIndex)
		{
			if (options is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (options));
			using var dict = options.ToDictionary ();
			return CopyProperties (dict, imageIndex);
		}

		public CoreGraphics.CGImageProperties GetProperties (CGImageOptions? options = null)
		{
			using var dict = options?.ToDictionary ();
			return new CoreGraphics.CGImageProperties (CopyProperties (dict));
		}

		public CoreGraphics.CGImageProperties GetProperties (int index, CGImageOptions? options = null)
		{
			using var dict = options?.ToDictionary ();
			return new CoreGraphics.CGImageProperties (CopyProperties (dict, index));
		}

		[DllImport (Constants.ImageIOLibrary)]
		extern static /* CGImageRef __nullable */ IntPtr CGImageSourceCreateImageAtIndex (
			/* CGImageSourceRef __nonnull */ IntPtr isrc, /* size_t */ nint index,
			/* CFDictionaryRef __nullable */ IntPtr options);

		public CGImage? CreateImage (int index, CGImageOptions options)
		{
			using (var dict = options?.ToDictionary ()) {
				var ret = CGImageSourceCreateImageAtIndex (Handle, index, dict.GetHandle ());
				return ret == IntPtr.Zero ? null : new CGImage (ret, true);
			}
		}

		[DllImport (Constants.ImageIOLibrary)]
		extern static /* CGImageRef */ IntPtr CGImageSourceCreateThumbnailAtIndex (/* CGImageSourceRef */ IntPtr isrc, /* size_t */ nint index, /* CFDictionaryRef */ IntPtr options);

		public CGImage? CreateThumbnail (int index, CGImageThumbnailOptions? options)
		{
			using (var dict = options?.ToDictionary ()) {
				var ret = CGImageSourceCreateThumbnailAtIndex (Handle, index, dict.GetHandle ());
#if NET
				return CGImage.FromHandle (ret, true);
#else
				return new CGImage (ret, true);
#endif
			}
		}

		[DllImport (Constants.ImageIOLibrary)]
		extern static /* CGImageSourceRef __nonnull */ IntPtr CGImageSourceCreateIncremental (
			/* CFDictionaryRef __nullable */ IntPtr options);

		public static CGImageSource CreateIncremental (CGImageOptions? options)
		{
			using (var dict = options?.ToDictionary ())
				return new CGImageSource (CGImageSourceCreateIncremental (dict.GetHandle ()), true);
		}

		[DllImport (Constants.ImageIOLibrary)]
		extern static void CGImageSourceUpdateData (/* CGImageSourceRef __nonnull */ IntPtr isrc,
			/* CFDataRef __nonnull */ IntPtr data, byte final);

		public void UpdateData (NSData data, bool final)
		{
			if (data is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (data));
			CGImageSourceUpdateData (Handle, data.Handle, final ? (byte) 1 : (byte) 0);
		}

		[DllImport (Constants.ImageIOLibrary)]
		extern static void CGImageSourceUpdateDataProvider (/* CGImageSourceRef __nonnull */ IntPtr handle,
			/* CGDataProviderRef __nonnull */ IntPtr dataProvider,
			byte final);

		public void UpdateDataProvider (CGDataProvider provider, bool final)
		{
			if (provider is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (provider));
			CGImageSourceUpdateDataProvider (Handle, provider.Handle, final ? (byte) 1 : (byte) 0);
		}

		// note: CGImageSourceStatus is always an int (4 bytes) so it's ok to use in the pinvoke declaration
		[DllImport (Constants.ImageIOLibrary)]
		extern static CGImageSourceStatus CGImageSourceGetStatus (/* CGImageSourceRef __nonnull */ IntPtr isrc);

		public CGImageSourceStatus GetStatus ()
		{
			return CGImageSourceGetStatus (Handle);
		}

		// note: CGImageSourceStatus is always an int (4 bytes) so it's ok to use in the pinvoke declaration
		[DllImport (Constants.ImageIOLibrary)]
		extern static CGImageSourceStatus CGImageSourceGetStatusAtIndex (
			/* CGImageSourceRef __nonnull */ IntPtr handle, /* size_t */ nint idx);

		public CGImageSourceStatus GetStatus (int index)
		{
			return CGImageSourceGetStatusAtIndex (Handle, index);
		}

#if NET
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		[DllImport (Constants.ImageIOLibrary)]
		static extern IntPtr /* CFDictionaryRef* */ CGImageSourceCopyAuxiliaryDataInfoAtIndex (IntPtr /* CGImageSourceRef* */ isrc, nuint index, IntPtr /* CFStringRef* */ auxiliaryImageDataType);

#if NET
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		public CGImageAuxiliaryDataInfo? CopyAuxiliaryDataInfo (nuint index, CGImageAuxiliaryDataType auxiliaryImageDataType)
		{
			var ptr = CGImageSourceCopyAuxiliaryDataInfoAtIndex (Handle, index, auxiliaryImageDataType.GetConstant ().GetHandle ());
			if (ptr == IntPtr.Zero)
				return null;

			var dictionary = Runtime.GetNSObject<NSDictionary> (ptr, true);
			return new CGImageAuxiliaryDataInfo (dictionary);
		}

#if NET
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		[DllImport (Constants.ImageIOLibrary)]
		extern static nuint CGImageSourceGetPrimaryImageIndex (IntPtr /* CGImageSource */ src);

#if NET
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("maccatalyst")]
#endif
		public nuint GetPrimaryImageIndex ()
		{
			return CGImageSourceGetPrimaryImageIndex (Handle);
		}
#endif

#if !COREBUILD
#if NET
		[SupportedOSPlatform ("macos14.4")]
		[SupportedOSPlatform ("ios17.4")]
		[SupportedOSPlatform ("tvos17.4")]
		[SupportedOSPlatform ("maccatalyst17.4")]
#else
		[TV (17, 4), Mac (14, 4), iOS (17, 4)]
#endif
		[DllImport (Constants.ImageIOLibrary)]
		static extern OSStatus CGImageSourceSetAllowableTypes (IntPtr allowableTypes);

#if NET
		[SupportedOSPlatform ("macos14.4")]
		[SupportedOSPlatform ("ios17.4")]
		[SupportedOSPlatform ("tvos17.4")]
		[SupportedOSPlatform ("maccatalyst17.4")]
#else
		[TV (17, 4), Mac (14, 4), iOS (17, 4)]
#endif
		public static void SetAllowableTypes (string [] allowableTypes)
		{
			if (allowableTypes is null || allowableTypes.Length == 0)
				throw new ArgumentException ($"{nameof (allowableTypes)} cannot be null or empty.");

			using (var array = NSArray.FromStrings (allowableTypes)) {
				var errorCode = CGImageSourceSetAllowableTypes (array.Handle);
				if (errorCode != 0)
					throw new InvalidOperationException ($"Unable to set allowable types, error code: 0x{errorCode:x}");
			}
		}
#endif
	}
}
