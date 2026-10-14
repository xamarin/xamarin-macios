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

using System.ComponentModel;

using CoreFoundation;
using CoreGraphics;

namespace ImageIO {

	public partial class CGImageDestinationOptions {
		CGColor? destinationBackgroundColor;
		/// <summary>The background color used during image compositing for transparent regions of the image.</summary>
		///         <value>The color used for background compositing.</value>
		///         <remarks>To be added.</remarks>
		public CGColor? DestinationBackgroundColor {
			get { return destinationBackgroundColor; }
			set {
				destinationBackgroundColor = value;
				(Dictionary as NSMutableDictionary)?.LowlevelSetObject (destinationBackgroundColor!, CGImageDestinationOptionsKeys.BackgroundColor);
			}
		}

		internal NSMutableDictionary ToDictionary ()
		{
			return (NSMutableDictionary) Dictionary;
		}
	}

	/// <summary>To be added.</summary>
	///     <remarks>To be added.</remarks>
	public partial class CGImageAuxiliaryDataInfo {

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public CGImageMetadata? Metadata {
			get {
				return GetNativeValue<CGImageMetadata> (CGImageAuxiliaryDataInfoKeys.MetadataKey);
			}
			set {
				SetNativeValue (CGImageAuxiliaryDataInfoKeys.MetadataKey, value);
			}
		}
	}

	/// <include file="../../docs/api/ImageIO/CGImageDestination.xml" path="/Documentation/Docs[@DocId='T:ImageIO.CGImageDestination']/*" />
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public class CGImageDestination : NativeObject {
		[Preserve (Conditional = true)]
		internal CGImageDestination (NativeHandle handle, bool owns)
			: base (handle, owns)
		{
		}

		/// <summary>Type identifier for the ImageIO.CGImageDestination type.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>
		///           <para>The returned token is the CoreFoundation type identifier (CFType) that has been assigned to this class.</para>
		///           <para>This can be used to determine type identity between different CoreFoundation objects.</para>
		///           <para>You can retrieve the type of a CoreFoundation object by invoking the <see cref="CoreFoundation.CFType.GetTypeID(System.IntPtr)" /> on the native handle of the object</para>
		///           <example>
		///             <code lang="csharp lang-csharp"><![CDATA[bool isCGImageDestination = (CFType.GetTypeID (foo.Handle) == CGImageDestination.GetTypeID ());]]></code>
		///           </example>
		///         </remarks>
		[DllImport (Constants.ImageIOLibrary, EntryPoint = "CGImageDestinationGetTypeID")]
		public extern static /* CFTypeID */ nint GetTypeID ();

		[DllImport (Constants.ImageIOLibrary)]
		extern static /* CFArrayRef __nonnull */ IntPtr CGImageDestinationCopyTypeIdentifiers ();

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
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

		/// <param name="consumer">To be added.</param>
		///         <param name="typeIdentifier">To be added.</param>
		///         <param name="imageCount">To be added.</param>
		///         <param name="options">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
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
				GC.KeepAlive (consumer);
				GC.KeepAlive (dict);
				return p == IntPtr.Zero ? null : new CGImageDestination (p, true);
			} finally {
				CFString.ReleaseNative (typeId);
			}
		}

		[DllImport (Constants.ImageIOLibrary)]
		extern static /* CGImageDestinationRef __nullable */ IntPtr CGImageDestinationCreateWithData (
			/* CFMutableDataRef __nonnull */ IntPtr data, /* CFStringRef __nonnull */ IntPtr stringType,
			/* size_t */ nint count, /* CFDictionaryRef __nullable */ IntPtr options);

		/// <param name="data">To be added.</param>
		///         <param name="typeIdentifier">To be added.</param>
		///         <param name="imageCount">To be added.</param>
		///         <param name="options">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
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
				GC.KeepAlive (data);
				GC.KeepAlive (dict);
				return p == IntPtr.Zero ? null : new CGImageDestination (p, true);
			} finally {
				CFString.ReleaseNative (typeId);
			}
		}

		[DllImport (Constants.ImageIOLibrary)]
		extern static /* CGImageDestinationRef __nullable */ IntPtr CGImageDestinationCreateWithURL (
			/* CFURLRef __nonnull */ IntPtr url, /* CFStringRef __nonnull */ IntPtr stringType,
			/* size_t */ nint count, /* CFDictionaryRef __nullable */ IntPtr options);

		/// <param name="url">To be added.</param>
		///         <param name="typeIdentifier">To be added.</param>
		///         <param name="imageCount">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		public static CGImageDestination? Create (NSUrl url, string typeIdentifier, int imageCount)
		{
			if (url is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (url));
			if (typeIdentifier is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (typeIdentifier));

			var typeId = CFString.CreateNative (typeIdentifier);
			try {
				IntPtr p = CGImageDestinationCreateWithURL (url.Handle, typeId, imageCount, IntPtr.Zero);
				GC.KeepAlive (url);
				return p == IntPtr.Zero ? null : new CGImageDestination (p, true);
			} finally {
				CFString.ReleaseNative (typeId);
			}
		}

		[DllImport (Constants.ImageIOLibrary)]
		extern static void CGImageDestinationSetProperties (/* CGImageDestinationRef __nonnull */ IntPtr idst,
			/* CFDictionaryRef __nullable */ IntPtr properties);

		/// <param name="properties">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public void SetProperties (NSDictionary? properties)
		{
			CGImageDestinationSetProperties (Handle, properties.GetHandle ());
			GC.KeepAlive (properties);
		}

		[DllImport (Constants.ImageIOLibrary)]
		extern static void CGImageDestinationAddImage (/* CGImageDestinationRef __nonnull */ IntPtr idst,
			/* CGImageRef __nonnull */ IntPtr image,
			/* CFDictionaryRef __nullable */ IntPtr properties);

		/// <param name="image">To be added.</param>
		///         <param name="options">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public void AddImage (CGImage image, CGImageDestinationOptions? options = null)
		{
			if (image is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (image));

			using (var dict = options?.ToDictionary ()) {
				CGImageDestinationAddImage (Handle, image.Handle, dict.GetHandle ());
				GC.KeepAlive (image);
			}
		}

		/// <param name="image">To be added.</param>
		///         <param name="properties">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public void AddImage (CGImage image, NSDictionary? properties)
		{
			if (image is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (image));

			CGImageDestinationAddImage (Handle, image.Handle, properties.GetHandle ());
			GC.KeepAlive (image);
			GC.KeepAlive (properties);
		}

		[DllImport (Constants.ImageIOLibrary)]
		extern static void CGImageDestinationAddImageFromSource (/* CGImageDestinationRef __nonnull */ IntPtr idst,
			/* CGImageSourceRef __nonnull */ IntPtr sourceHandle, /* size_t */ nint index,
			/* CFDictionaryRef __nullable */ IntPtr properties);

		/// <param name="source">To be added.</param>
		///         <param name="index">To be added.</param>
		///         <param name="options">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public void AddImage (CGImageSource source, int index, CGImageDestinationOptions? options = null)
		{
			if (source is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (source));

			using var dict = options?.ToDictionary ();
			CGImageDestinationAddImageFromSource (Handle, source.Handle, index, dict.GetHandle ());
			GC.KeepAlive (source);
			GC.KeepAlive (dict);
		}

		/// <param name="source">To be added.</param>
		///         <param name="index">To be added.</param>
		///         <param name="properties">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public void AddImage (CGImageSource source, int index, NSDictionary? properties)
		{
			if (source is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (source));

			CGImageDestinationAddImageFromSource (Handle, source.Handle, index, properties.GetHandle ());
			GC.KeepAlive (source);
			GC.KeepAlive (properties);
		}

		[DllImport (Constants.ImageIOLibrary)]
		extern static byte CGImageDestinationFinalize (/* CGImageDestinationRef __nonnull */ IntPtr idst);

		/// <summary>Writes the images to the destination and disposes the object.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		public bool Close ()
		{
			var success = CGImageDestinationFinalize (Handle);
			Dispose ();
			return success != 0;
		}

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[DllImport (Constants.ImageIOLibrary)]
		extern static void CGImageDestinationAddImageAndMetadata (/* CGImageDestinationRef __nonnull */ IntPtr idst,
			/* CGImageRef __nonnull */ IntPtr image, /* CGImageMetadataRef __nullable */ IntPtr metadata,
			/* CFDictionaryRef __nullable */ IntPtr options);

		/// <param name="image">To be added.</param>
		///         <param name="meta">To be added.</param>
		///         <param name="options">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		public void AddImageAndMetadata (CGImage image, CGImageMetadata meta, NSDictionary? options)
		{
			if (image is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (image));
			CGImageDestinationAddImageAndMetadata (Handle, image.Handle, meta.GetHandle (), options.GetHandle ());
			GC.KeepAlive (image);
			GC.KeepAlive (meta);
			GC.KeepAlive (options);
		}

		/// <param name="image">To be added.</param>
		///         <param name="meta">To be added.</param>
		///         <param name="options">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		public void AddImageAndMetadata (CGImage image, CGImageMetadata meta, CGImageDestinationOptions? options)
		{
			using var o = options?.ToDictionary ();
			AddImageAndMetadata (image, meta, o);
		}

		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[DllImport (Constants.ImageIOLibrary)]
		unsafe extern static byte CGImageDestinationCopyImageSource (/* CGImageDestinationRef __nonnull */ IntPtr idst,
			/* CGImageSourceRef __nonnull */ IntPtr image, /* CFDictionaryRef __nullable */ IntPtr options,
			/* CFErrorRef* */ IntPtr* err);

		/// <param name="image">To be added.</param>
		///         <param name="options">To be added.</param>
		///         <param name="error">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[EditorBrowsable (EditorBrowsableState.Advanced)]
		public bool CopyImageSource (CGImageSource image, NSDictionary? options, out NSError? error)
		{
			if (image is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (image));
			byte result;
			IntPtr err;
			unsafe {
				result = CGImageDestinationCopyImageSource (Handle, image.Handle, options.GetHandle (), &err);
				GC.KeepAlive (image);
				GC.KeepAlive (options);
			}
			error = Runtime.GetNSObject<NSError> (err);
			return result != 0;
		}

		/// <param name="image">To be added.</param>
		///         <param name="options">To be added.</param>
		///         <param name="error">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		public bool CopyImageSource (CGImageSource image, CGCopyImageSourceOptions? options, out NSError? error)
		{
			using var o = options?.Dictionary;
			return CopyImageSource (image, o, out error);
		}

		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[DllImport (Constants.ImageIOLibrary)]
		static extern void CGImageDestinationAddAuxiliaryDataInfo (IntPtr /* CGImageDestinationRef* */ idst, IntPtr /* CFStringRef* */ auxiliaryImageDataType, IntPtr /* CFDictionaryRef* */ auxiliaryDataInfoDictionary);

		/// <param name="auxiliaryImageDataType">To be added.</param>
		///         <param name="auxiliaryDataInfo">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		[SupportedOSPlatform ("tvos")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		public void AddAuxiliaryDataInfo (CGImageAuxiliaryDataType auxiliaryImageDataType, CGImageAuxiliaryDataInfo? auxiliaryDataInfo)
		{
			using (var dict = auxiliaryDataInfo?.Dictionary) {
				var constant = auxiliaryImageDataType.GetConstant ();
				CGImageDestinationAddAuxiliaryDataInfo (Handle, constant.GetHandle (), dict.GetHandle ());
				GC.KeepAlive (constant);
			}
		}
	}
}
