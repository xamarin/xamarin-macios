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

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

using ObjCRuntime;
using Foundation;
using CoreFoundation;
using CoreGraphics;

namespace ImageIO {

	// untyped enum -> CGImageSource.h
	public enum CGImageSourceStatus {
		Complete      = 0,
		Incomplete    = -1,
		ReadingHeader = -2,
		UnknownType   = -3,
		InvalidData   = -4,
		UnexpectedEOF = -5,
	}
	
	public partial class CGImageOptions {

		public CGImageOptions ()
		{
			ShouldCache = true;
		}
		
		public string BestGuessTypeIdentifier { get; set; }

		public bool ShouldCache { get; set; }

		[iOS (7,0)][Mac (10,9)]
		public bool ShouldCacheImmediately { get; set; }

		public bool ShouldAllowFloat { get; set; }
		
		internal virtual NSMutableDictionary ToDictionary ()
		{
			var dict = new NSMutableDictionary ();
			
			if (BestGuessTypeIdentifier != null)
				dict.LowlevelSetObject (new NSString (BestGuessTypeIdentifier), kTypeIdentifierHint);
			if (!ShouldCache)
				dict.LowlevelSetObject (CFBoolean.False.Handle, kShouldCache);
			if (ShouldAllowFloat)
				dict.LowlevelSetObject (CFBoolean.True.Handle, kShouldAllowFloat);
			if (kShouldCacheImmediately != IntPtr.Zero && ShouldCacheImmediately)
				dict.LowlevelSetObject (CFBoolean.True.Handle, kShouldCacheImmediately);

			return dict;
		}
	}

	public partial class CGImageThumbnailOptions : CGImageOptions {

		public bool CreateThumbnailFromImageIfAbsent { get; set; }
		public bool CreateThumbnailFromImageAlways { get; set; }
		public int? MaxPixelSize { get; set; }
		public bool CreateThumbnailWithTransform { get; set; }

		[iOS (9,0)][Mac (10,11)]
		public int? SubsampleFactor { get; set; }

		internal override NSMutableDictionary ToDictionary ()
		{
			var dict = base.ToDictionary ();
			IntPtr thandle = CFBoolean.True.Handle;

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
	
	public partial class CGImageSource : INativeObject, IDisposable
	{
		[DllImport (Constants.ImageIOLibrary, EntryPoint="CGImageSourceGetTypeID")]
		public extern static nint GetTypeID ();

		[DllImport (Constants.ImageIOLibrary)]
		extern static /* CFArrayRef __nonnull */ IntPtr CGImageSourceCopyTypeIdentifiers ();

		public static string [] TypeIdentifiers {
			get {
				var handle = CGImageSourceCopyTypeIdentifiers ();
				var array = NSArray.StringArrayFromHandle (handle);
				CFObject.CFRelease (handle);
				return array;
			}
		}
		
		internal IntPtr handle;

		// invoked by marshallers
		internal CGImageSource (IntPtr handle) : this (handle, false)
		{
			this.handle = handle;
		}

		[Preserve (Conditional=true)]
		internal CGImageSource (IntPtr handle, bool owns)
		{
			this.handle = handle;
			if (!owns)
				CFObject.CFRetain (handle);
		}

		~CGImageSource ()
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
				
		[DllImport (Constants.ImageIOLibrary)]
		extern static /* CGImageSourceRef __nullable */ IntPtr CGImageSourceCreateWithURL (
			/* CFURLRef __nonnull */ IntPtr url, /* CFDictionaryRef __nullable */ IntPtr options);

		public static CGImageSource FromUrl (NSUrl url)
		{
			return FromUrl (url, null);
		}
		
		public static CGImageSource FromUrl (NSUrl url, CGImageOptions options)
		{
			if (url == null)
				throw new ArgumentNullException ("url");

			using (var dict = options == null ? null : options.ToDictionary ()) {
				var result = CGImageSourceCreateWithURL (url.Handle, dict == null ? IntPtr.Zero : dict.Handle);
				return result == IntPtr.Zero ? null : new CGImageSource (result, true);
			}
		}

		[DllImport (Constants.ImageIOLibrary)]
		extern static /* CGImageSourceRef __nullable */ IntPtr CGImageSourceCreateWithDataProvider (
			/* CGDataProviderRef __nonnull */ IntPtr provider, /* CFDictionaryRef __nullable */ IntPtr options);

		public static CGImageSource FromDataProvider (CGDataProvider provider)
		{
			return FromDataProvider (provider, null);
		}
		
		public static CGImageSource FromDataProvider (CGDataProvider provider, CGImageOptions options)
		{
			if (provider == null)
				throw new ArgumentNullException ("provider");

			using (var dict = options == null ? null : options.ToDictionary ()) {
				var result = CGImageSourceCreateWithDataProvider (provider.Handle, dict == null ? IntPtr.Zero : dict.Handle);
				return result == IntPtr.Zero ? null : new CGImageSource (result, true);
			}
		}

		[DllImport (Constants.ImageIOLibrary)]
		extern static /* CGImageSourceRef __nullable */ IntPtr CGImageSourceCreateWithData (
			/* CFDataRef __nonnull */ IntPtr data, /* CFDictionaryRef __nullable */ IntPtr options);

		public static CGImageSource FromData (NSData data)
		{
			return FromData (data, null);
		}
		
		public static CGImageSource FromData (NSData data, CGImageOptions options)
		{
			if (data == null)
				throw new ArgumentNullException ("data");

			using (var dict = options == null ? null : options.ToDictionary ()) {
				var result = CGImageSourceCreateWithData (data.Handle, dict == null ? IntPtr.Zero : dict.Handle);
				return result == IntPtr.Zero ? null : new CGImageSource (result, true);
			}
		}

		[DllImport (Constants.ImageIOLibrary)]
		extern static /* CFStringRef __nullable */ IntPtr CGImageSourceGetType (
			/* CGImageSourceRef __nonnull */ IntPtr handle);
		
		public string TypeIdentifier {
			get {
				return NSString.FromHandle (CGImageSourceGetType (handle));
			}
		}

		[DllImport (Constants.ImageIOLibrary)]
		extern static /* size_t */ nint CGImageSourceGetCount (/* CGImageSourceRef __nonnull */ IntPtr handle);
		
		public nint ImageCount {
			get {
				return CGImageSourceGetCount (handle);
			}
		}

		[DllImport (Constants.ImageIOLibrary)]
		extern static /* CFDictionaryRef __nullable */ IntPtr CGImageSourceCopyProperties (
			/* CGImageSourceRef __nonnull */ IntPtr isrc, /* CFDictionaryRef __nullable */ IntPtr options);

		[Advice ("Use 'GetProperties'.")]
		public NSDictionary CopyProperties (NSDictionary dict)
		{
			var result = CGImageSourceCopyProperties (handle, dict == null ? IntPtr.Zero : dict.Handle);
			return result == IntPtr.Zero ? null : Runtime.GetNSObject<NSDictionary> (result);
		}

		[Advice ("Use 'GetProperties'.")]
		public NSDictionary CopyProperties (CGImageOptions options)
		{
			if (options == null)
				throw new ArgumentNullException ("options");
			return CopyProperties (options.ToDictionary ());
		}

		[DllImport (Constants.ImageIOLibrary)]
		extern static /* CFDictionaryRef __nullable */ IntPtr CGImageSourceCopyPropertiesAtIndex (
			/* CGImageSourceRef __nonnull */ IntPtr isrc, /* size_t */ nint index,
			/* CFDictionaryRef __nullable */ IntPtr options);

		[Advice ("Use 'GetProperties'.")]
		public NSDictionary CopyProperties (NSDictionary dict, int imageIndex)
		{
			var result = CGImageSourceCopyPropertiesAtIndex (handle, imageIndex, dict == null ? IntPtr.Zero : dict.Handle);
			return result == IntPtr.Zero ? null : Runtime.GetNSObject<NSDictionary> (result);
		}

		[Advice ("Use 'GetProperties'.")]
		public NSDictionary CopyProperties (CGImageOptions options, int imageIndex)
		{
			if (options == null)
				throw new ArgumentNullException ("options");
			return CopyProperties (options.ToDictionary (), imageIndex);
		}

		public CoreGraphics.CGImageProperties GetProperties (CGImageOptions options = null)
		{
			return new CoreGraphics.CGImageProperties (CopyProperties (options == null ? null : options.ToDictionary ()));
		}

		public CoreGraphics.CGImageProperties GetProperties (int index, CGImageOptions options = null)
		{
			return new CoreGraphics.CGImageProperties (CopyProperties (options == null ? null : options.ToDictionary (), index));
		}

		[DllImport (Constants.ImageIOLibrary)]
		extern static /* CGImageRef __nullable */ IntPtr CGImageSourceCreateImageAtIndex (
			/* CGImageSourceRef __nonnull */ IntPtr isrc, /* size_t */ nint index,
			/* CFDictionaryRef __nullable */ IntPtr options);

		public CGImage CreateImage (int index, CGImageOptions options)
		{
			using (var dict = options == null ? null : options.ToDictionary ()) {
				var ret = CGImageSourceCreateImageAtIndex (handle, index, dict == null ? IntPtr.Zero : dict.Handle);
				return ret == IntPtr.Zero ? null : new CGImage (ret, true);
			}
		}

		[DllImport (Constants.ImageIOLibrary)]
		extern static /* CGImageRef */ IntPtr CGImageSourceCreateThumbnailAtIndex (/* CGImageSourceRef */ IntPtr isrc, /* size_t */ nint index, /* CFDictionaryRef */ IntPtr options);

		public CGImage CreateThumbnail (int index, CGImageThumbnailOptions options)
		{
			using (var dict = options == null ? null : options.ToDictionary ()) {
				var ret = CGImageSourceCreateThumbnailAtIndex (handle, index, dict == null ? IntPtr.Zero : dict.Handle);
				return new CGImage (ret, true);
			}
		}

		[DllImport (Constants.ImageIOLibrary)]
		extern static /* CGImageSourceRef __nonnull */ IntPtr CGImageSourceCreateIncremental (
			/* CFDictionaryRef __nullable */ IntPtr options);

		public static CGImageSource CreateIncremental (CGImageOptions options)
		{
			using (var dict = options == null ? null : options.ToDictionary ())
				return new CGImageSource (CGImageSourceCreateIncremental (dict == null ? IntPtr.Zero : dict.Handle), true);
		}

		[DllImport (Constants.ImageIOLibrary)]
		extern static void CGImageSourceUpdateData (/* CGImageSourceRef __nonnull */ IntPtr isrc,
			/* CFDataRef __nonnull */ IntPtr data, [MarshalAs (UnmanagedType.I1)] bool final);
		
		public void UpdateData (NSData data, bool final)
		{
			if (data == null)
				throw new ArgumentNullException ("data");
			CGImageSourceUpdateData (handle, data.Handle, final);
		}

		[DllImport (Constants.ImageIOLibrary)]
		extern static void CGImageSourceUpdateDataProvider (/* CGImageSourceRef __nonnull */ IntPtr handle,
			/* CGDataProviderRef __nonnull */ IntPtr dataProvider,
			[MarshalAs (UnmanagedType.I1)] bool final);

#if !XAMCORE_2_0
		[Obsolete ("Use 'UpdateDataProvider(CGDataProvider,bool)'.")]
		public void UpdateDataProvider (CGDataProvider provider)
		{
			UpdateDataProvider (provider, true);
		}
#endif
		public void UpdateDataProvider (CGDataProvider provider, bool final)
		{
			if (provider == null)
				throw new ArgumentNullException ("provider");
			CGImageSourceUpdateDataProvider (handle, provider.Handle, final);
		}

		// note: CGImageSourceStatus is always an int (4 bytes) so it's ok to use in the pinvoke declaration
		[DllImport (Constants.ImageIOLibrary)]
		extern static CGImageSourceStatus CGImageSourceGetStatus (/* CGImageSourceRef __nonnull */ IntPtr isrc);
		
		public CGImageSourceStatus GetStatus ()
		{
			return CGImageSourceGetStatus (handle);
		}

		// note: CGImageSourceStatus is always an int (4 bytes) so it's ok to use in the pinvoke declaration
		[DllImport (Constants.ImageIOLibrary)]
		extern static CGImageSourceStatus CGImageSourceGetStatusAtIndex (
			/* CGImageSourceRef __nonnull */ IntPtr handle, /* size_t */ nint idx);		

		public CGImageSourceStatus GetStatus (int index)
		{
			return CGImageSourceGetStatusAtIndex (handle, index);
		}

		[Watch (4, 0), TV (11, 0), Mac (10, 13), iOS (11, 0)]
		[DllImport (Constants.ImageIOLibrary)]
		static extern IntPtr /* CFDictionaryRef* */ CGImageSourceCopyAuxiliaryDataInfoAtIndex (IntPtr /* CGImageSourceRef* */ isrc, nuint index, IntPtr /* CFStringRef* */ auxiliaryImageDataType);

		[Watch (4, 0), TV (11, 0), Mac (10, 13), iOS (11, 0)]
		public CGImageAuxiliaryDataInfo CopyAuxiliaryDataInfo (nuint index, CGImageAuxiliaryDataType auxiliaryImageDataType)
		{
			var ptr = CGImageSourceCopyAuxiliaryDataInfoAtIndex (Handle, index, auxiliaryImageDataType.GetConstant ().GetHandle ());
			if (ptr == IntPtr.Zero)
				return null;

			var dictionary = Runtime.GetNSObject<NSDictionary> (ptr);
			var info = new CGImageAuxiliaryDataInfo (dictionary);

			return info;
		}

		[Mac (10,14, onlyOn64: true), iOS (12,0), TV (12,0), Watch (5,0)]
		[DllImport (Constants.ImageIOLibrary)]
		extern static nuint CGImageSourceGetPrimaryImageIndex (IntPtr /* CGImageSource */ src);

		[Mac (10,14, onlyOn64: true), iOS (12,0), TV (12,0), Watch (5,0)]
		public nuint GetPrimaryImageIndex ()
		{
			return CGImageSourceGetPrimaryImageIndex (handle);
		}
	}
}
