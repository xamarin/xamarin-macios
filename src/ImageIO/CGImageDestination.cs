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

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

using ObjCRuntime;
using Foundation;
using CoreFoundation;
using CoreGraphics;

namespace ImageIO {

#if !XAMCORE_2_0
	public partial class CGImageDestinationOptions {

		public float? LossyCompressionQuality { get; set; }
		public CGColor DestinationBackgroundColor { get; set; }

		// new in iOS 7 and 10.8
		[iOS (7,0)]
		public CGImageMetadata Metadata { get; set; }

		[iOS (7,0)]
		public bool MergeMetadata { get; set; }

		[iOS (7,0)]
		public bool ShouldExcludeXMP { get; set; }

		[iOS (7,0)]
		public int? Orientation { get; set; }

		[iOS (7,0)]
		public DateTime? DateTime { get; set; }

		[Mac (10, 10)]
		[iOS (8, 0)]
		public int? ImageMaxPixelSize { get; set; }

		[Mac (10, 10)]
		[iOS (8, 0)]
		public bool EmbedThumbnail { get; set; }

		[Mac (10, 10)]
		[iOS (8, 0)]
		public bool ShouldExcludeGPS { get; set; }

		[iOS (9,3)][Mac (10,12)]
		public bool OptimizeColorForSharing { get; set; }

		internal NSMutableDictionary ToDictionary ()
		{
			var dict = new NSMutableDictionary ();

			if (LossyCompressionQuality.HasValue)
				dict.LowlevelSetObject (new NSNumber (LossyCompressionQuality.Value), kLossyCompressionQuality);
			if (DestinationBackgroundColor != null)
				dict.LowlevelSetObject (DestinationBackgroundColor.Handle, kBackgroundColor);

			// new in iOS 7 and 10.8
			if (Metadata != null) {
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

			if (Orientation.HasValue) {
				using (var n = new NSNumber (Orientation.Value))
					dict.LowlevelSetObject (n.Handle, kOrientation);
			}

			// new in iOS 8 and 10.10 
			if (ImageMaxPixelSize.HasValue && (kImageMaxPixelSize != IntPtr.Zero)) {
				using (var n = new NSNumber (ImageMaxPixelSize.Value))
					dict.LowlevelSetObject (n.Handle, kOrientation);
			}

			// new in iOS 8 and 10.10 - default is false
			if (EmbedThumbnail && (kEmbedThumbnail != IntPtr.Zero))
				dict.LowlevelSetObject (CFBoolean.TrueHandle, kEmbedThumbnail);

			// new in iOS 8 and 10.10 - default is false
			if (ShouldExcludeGPS && (kShouldExcludeGPS != IntPtr.Zero))
				dict.LowlevelSetObject (CFBoolean.TrueHandle, kShouldExcludeGPS);

			if (OptimizeColorForSharing && (kOptimizeColorForSharing != IntPtr.Zero))
				dict.LowlevelSetObject (CFBoolean.TrueHandle, kOptimizeColorForSharing);

			return dict;
		}
	}

#else
	public partial class CGImageDestinationOptions
	{
		CGColor destinationBackgroundColor;
		public CGColor DestinationBackgroundColor {
			get { return destinationBackgroundColor; }
			set {
				destinationBackgroundColor = value;
				(Dictionary as NSMutableDictionary).LowlevelSetObject (destinationBackgroundColor.Handle, CGImageDestinationOptionsKeys.BackgroundColor.Handle);
			}
		}

		internal NSMutableDictionary ToDictionary ()
		{
			return (NSMutableDictionary) Dictionary;
		}
	}

	public partial class CGCopyImageSourceOptions
	{
		[iOS (7,0)]
		public CGImageMetadata Metadata { get; set; }

		[iOS (7,0)]
		public bool MergeMetadata { get; set; }

		[iOS (7,0)]
		public bool ShouldExcludeXMP { get; set; }

		[Mac (10, 10)]
		[iOS (8, 0)]
		public bool ShouldExcludeGPS { get; set; }

		[iOS (7,0)]
		public DateTime? DateTime { get; set; }

		[iOS (7,0)]
		public int? Orientation { get; set; }

		internal NSMutableDictionary ToDictionary ()
		{
			var dict = new NSMutableDictionary ();

			// new in iOS 7 and 10.8
			if (Metadata != null) {
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
#endif

	public partial class CGImageAuxiliaryDataInfo {

		public CGImageMetadata Metadata {
			get {
				return GetNativeValue<CGImageMetadata> (CGImageAuxiliaryDataInfoKeys.MetadataKey);
			}
			set {
				SetNativeValue (CGImageAuxiliaryDataInfoKeys.MetadataKey, value);
			}
		}
	}

	public class CGImageDestination : INativeObject, IDisposable {
		internal IntPtr handle;

		// invoked by marshallers
		internal CGImageDestination (IntPtr handle) : this (handle, false)
		{
			this.handle = handle;
		}

		[Preserve (Conditional=true)]
		internal CGImageDestination (IntPtr handle, bool owns)
		{
			this.handle = handle;
			if (!owns)
				CFObject.CFRetain (handle);
		}

		~CGImageDestination ()
		{
			Dispose (false);
		}
		
		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		public IntPtr Handle {
			get { return handle; }
		}
		
		protected virtual void Dispose (bool disposing)
		{
			if (handle != IntPtr.Zero){
				CFObject.CFRelease (handle);
				handle = IntPtr.Zero;
			}
		}
				
		[DllImport (Constants.ImageIOLibrary, EntryPoint="CGImageDestinationGetTypeID")]
		public extern static /* CFTypeID */ nint GetTypeID ();
		
		[DllImport (Constants.ImageIOLibrary)]
		extern static /* CFArrayRef __nonnull */ IntPtr CGImageDestinationCopyTypeIdentifiers ();

		public static string [] TypeIdentifiers {
			get {
				var handle = CGImageDestinationCopyTypeIdentifiers ();
				var array = NSArray.StringArrayFromHandle (handle);
				CFObject.CFRelease (handle);
				return array;
			}
		}

		[DllImport (Constants.ImageIOLibrary)]
		extern static /* CGImageDestinationRef __nullable */ IntPtr CGImageDestinationCreateWithDataConsumer (
			/* CGDataConsumerRef __nonnull */ IntPtr consumer, /* CFStringRef __nonnull */ IntPtr type,
			/* size_t */ nint count, /* CFDictionaryRef __nullable */ IntPtr options);

		public static CGImageDestination Create (CGDataConsumer consumer, string typeIdentifier, int imageCount, CGImageDestinationOptions options = null)
		{
			if (consumer == null)
				throw new ArgumentNullException ("consumer");
			if (typeIdentifier == null)
				throw new ArgumentNullException ("typeIdentifier");

			var dict = options == null ? null : options.ToDictionary ();
			var typeId = NSString.CreateNative (typeIdentifier);
			IntPtr p = CGImageDestinationCreateWithDataConsumer (consumer.Handle, typeId, imageCount, dict == null ? IntPtr.Zero : dict.Handle);
			NSString.ReleaseNative (typeId);
			var ret = p == IntPtr.Zero ? null : new CGImageDestination (p, true);
			if (dict != null)
				dict.Dispose ();
			return ret;
		}

		[DllImport (Constants.ImageIOLibrary)]
		extern static /* CGImageDestinationRef __nullable */ IntPtr CGImageDestinationCreateWithData (
			/* CFMutableDataRef __nonnull */ IntPtr data, /* CFStringRef __nonnull */ IntPtr stringType, 
			/* size_t */ nint count, /* CFDictionaryRef __nullable */ IntPtr options);

		// binding mistake -> NSMutableData, not NSData
		// naming mistake -> it's not From since it will write into (not read from) 'data'
#if XAMCORE_2_0
		public static CGImageDestination Create (NSMutableData data, string typeIdentifier, int imageCount, CGImageDestinationOptions options = null)
#else
		public static CGImageDestination FromData (NSData data, string typeIdentifier, int imageCount)
		{
			return FromData (data, typeIdentifier, imageCount, null);
		}

		public static CGImageDestination FromData (NSData data, string typeIdentifier, int imageCount, CGImageDestinationOptions options)
#endif
		{
			if (data == null)
				throw new ArgumentNullException ("data");
			if (typeIdentifier == null)
				throw new ArgumentNullException ("typeIdentifier");

			var dict = options == null ? null : options.ToDictionary ();
			var typeId = NSString.CreateNative (typeIdentifier);
			IntPtr p = CGImageDestinationCreateWithData (data.Handle, typeId, imageCount, dict == null ? IntPtr.Zero : dict.Handle);
			NSString.ReleaseNative (typeId);
			var ret = p == IntPtr.Zero ? null : new CGImageDestination (p, true);
			if (dict != null)
				dict.Dispose ();
			return ret;
		}

		[DllImport (Constants.ImageIOLibrary)]
		extern static /* CGImageDestinationRef __nullable */ IntPtr CGImageDestinationCreateWithURL (
			/* CFURLRef __nonnull */ IntPtr url, /* CFStringRef __nonnull */ IntPtr stringType,
			/* size_t */ nint count, /* CFDictionaryRef __nullable */ IntPtr options);

		// naming mistake -> it's not From since it will write into (not read from) 'url'
#if XAMCORE_2_0
		//
		// Dropped the CGImageDestinationOption parameter, as it turns out that for *creation* operations
		// it was never supported to begin with (it is expected to be null).   The CGImageDestinationOption
		// is actually just used for AddImage methods
		//
		public static CGImageDestination Create (NSUrl url, string typeIdentifier, int imageCount)
#else
		public static CGImageDestination FromUrl (NSUrl url, string typeIdentifier, int imageCount)
		{
			return FromUrl (url, typeIdentifier, imageCount, null);
		}
		
		public static CGImageDestination FromUrl (NSUrl url, string typeIdentifier, int imageCount, CGImageDestinationOptions options)
#endif
		{
			if (url == null)
				throw new ArgumentNullException ("url");
			if (typeIdentifier == null)
				throw new ArgumentNullException ("typeIdentifier");

			var typeId = NSString.CreateNative (typeIdentifier);
			IntPtr p = CGImageDestinationCreateWithURL (url.Handle, typeId, imageCount, IntPtr.Zero);
			NSString.ReleaseNative (typeId);
			var ret = p == IntPtr.Zero ? null : new CGImageDestination (p, true);
			return ret;
		}

		[DllImport (Constants.ImageIOLibrary)]
		extern static void CGImageDestinationSetProperties (/* CGImageDestinationRef __nonnull */ IntPtr idst,
			/* CFDictionaryRef __nullable */ IntPtr properties);

		public void SetProperties (NSDictionary properties)
		{
			CGImageDestinationSetProperties (handle, properties == null ? IntPtr.Zero : properties.Handle);
		}

		[DllImport (Constants.ImageIOLibrary)]
		extern static void CGImageDestinationAddImage (/* CGImageDestinationRef __nonnull */ IntPtr idst,
			/* CGImageRef __nonnull */ IntPtr image,
			/* CFDictionaryRef __nullable */ IntPtr properties);

		public void AddImage (CGImage image, CGImageDestinationOptions options = null)
		{
			if (image == null)
				throw new ArgumentNullException ("image");

			var dict = options == null ? null : options.ToDictionary ();
			CGImageDestinationAddImage (handle, image.Handle, dict == null ? IntPtr.Zero : dict.Handle);
			if (dict != null)
				dict.Dispose ();
		}

		public void AddImage (CGImage image, NSDictionary properties)
		{
			if (image == null)
				throw new ArgumentNullException ("image");
			
			CGImageDestinationAddImage (handle, image.Handle, properties == null ? IntPtr.Zero : properties.Handle);
		}

		[DllImport (Constants.ImageIOLibrary)]
		extern static void CGImageDestinationAddImageFromSource (/* CGImageDestinationRef __nonnull */ IntPtr idst,
			/* CGImageSourceRef __nonnull */ IntPtr sourceHandle, /* size_t */ nint index, 
			/* CFDictionaryRef __nullable */ IntPtr properties);

		public void AddImage (CGImageSource source, int index, CGImageDestinationOptions options = null)
		{
			if (source == null)
				throw new ArgumentNullException ("source");

			var dict = options == null ? null : options.ToDictionary ();
			CGImageDestinationAddImageFromSource (handle, source.Handle, index, dict == null ? IntPtr.Zero : dict.Handle);
			if (dict != null)
				dict.Dispose ();
		}

		public void AddImage (CGImageSource source, int index, NSDictionary properties)
		{
			if (source == null)
				throw new ArgumentNullException ("source");
			
			CGImageDestinationAddImageFromSource (handle, source.Handle, index, properties == null ? IntPtr.Zero : properties.Handle);
		}

		[DllImport (Constants.ImageIOLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static bool CGImageDestinationFinalize (/* CGImageDestinationRef __nonnull */ IntPtr idst);

		public bool Close ()
		{
			var success = CGImageDestinationFinalize (handle);
			Dispose ();
			return success;
		}

		[iOS (7,0)]
		[DllImport (Constants.ImageIOLibrary)]
		extern static void CGImageDestinationAddImageAndMetadata (/* CGImageDestinationRef __nonnull */ IntPtr idst,
			/* CGImageRef __nonnull */ IntPtr image, /* CGImageMetadataRef __nullable */ IntPtr metadata,
			/* CFDictionaryRef __nullable */ IntPtr options);

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[iOS (7,0)]
		public void AddImageAndMetadata (CGImage image, CGImageMetadata meta, NSDictionary options)
		{
			if (image == null)
				throw new ArgumentNullException ("image");
			IntPtr m = meta == null ? IntPtr.Zero : meta.Handle;
			IntPtr o = options == null ? IntPtr.Zero : options.Handle;
			CGImageDestinationAddImageAndMetadata (handle, image.Handle, m, o);
		}

		[iOS (7,0)]
		public void AddImageAndMetadata (CGImage image, CGImageMetadata meta, CGImageDestinationOptions options)
		{
			NSDictionary o = null;
			if (options != null)
				o = options.ToDictionary ();
			try {
				AddImageAndMetadata (image, meta, o);
			}
			finally {
				if (options != null)
					o.Dispose ();
			}
		}

		[iOS (7,0)]
		[DllImport (Constants.ImageIOLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static bool CGImageDestinationCopyImageSource (/* CGImageDestinationRef __nonnull */ IntPtr idst,
			/* CGImageSourceRef __nonnull */ IntPtr image, /* CFDictionaryRef __nullable */ IntPtr options,
			/* CFErrorRef* */ out IntPtr err);

		[EditorBrowsable (EditorBrowsableState.Advanced)]
		[iOS (7,0)]
		public bool CopyImageSource (CGImageSource image, NSDictionary options, out NSError error)
		{
			if (image == null)
				throw new ArgumentNullException ("image");
			IntPtr err;
			IntPtr o = options == null ? IntPtr.Zero : options.Handle;
			bool result = CGImageDestinationCopyImageSource (handle, image.Handle, o, out err);
			error = err == IntPtr.Zero ? null : new NSError (err);
			return result;
		}

		[iOS (7,0)]
#if XAMCORE_2_0
		public bool CopyImageSource (CGImageSource image, CGCopyImageSourceOptions options, out NSError error)
#else
		public bool CopyImageSource (CGImageSource image, CGImageDestinationOptions options, out NSError error)
#endif
		{
			NSDictionary o = null;
			if (options != null)
				o = options.ToDictionary ();
			try {
				return CopyImageSource (image, o, out error);
			}
			finally {
				if (options != null)
					o.Dispose ();
			}
		}

		[Watch (4, 0), TV (11, 0), Mac (10, 13), iOS (11, 0)]
		[DllImport (Constants.ImageIOLibrary)]
		static extern void CGImageDestinationAddAuxiliaryDataInfo (IntPtr /* CGImageDestinationRef* */ idst, IntPtr /* CFStringRef* */ auxiliaryImageDataType, IntPtr /* CFDictionaryRef* */ auxiliaryDataInfoDictionary);

		[Watch (4, 0), TV (11, 0), Mac (10, 13), iOS (11, 0)]
		public void AddAuxiliaryDataInfo (CGImageAuxiliaryDataType auxiliaryImageDataType, CGImageAuxiliaryDataInfo auxiliaryDataInfo)
		{
			using (var dict = auxiliaryDataInfo?.Dictionary) {
				CGImageDestinationAddAuxiliaryDataInfo (Handle, auxiliaryImageDataType.GetConstant ().GetHandle (), dict.GetHandle ());
			}
		}
	}
}