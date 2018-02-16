// 
// CMFormatDescription.cs: Implements the managed CMFormatDescription
//
// Authors:
//   Miguel de Icaza (miguel@xamarin.com)
//   Frank Krueger
//   Mono Team
//   Marek Safar (marek.safar@gmail.com)	
//     
// Copyright 2010 Novell, Inc
// Copyright 2012-2014 Xamarin Inc
//
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;

using Foundation;
using CoreFoundation;
using CoreGraphics;
using ObjCRuntime;
using CoreVideo;
using AudioToolbox;

#if !XAMCORE_2_0
using System.Drawing;
using CMVideoDimensions = System.Drawing.Size;
#endif

namespace CoreMedia {

	// untyped enum (uses as OSStatus) -> CMFormatDescription.h
	public enum CMFormatDescriptionError : int {
		None				= 0,
		InvalidParameter	= -12710,
		AllocationFailed	= -12711,
		ValueNotAvailable   = -12718,
	}

	public class CMFormatDescription : INativeObject, IDisposable {
		internal IntPtr handle;

		internal CMFormatDescription (IntPtr handle)
			: this (handle, false)
		{
		}

		[Preserve (Conditional=true)]
		internal CMFormatDescription (IntPtr handle, bool owns)
		{
			if (!owns)
				CFObject.CFRetain (handle);

			this.handle = handle;
		}
		
		~CMFormatDescription ()
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
		
		[DllImport(Constants.CoreMediaLibrary)]
		extern static /* CFDictionaryRef */ IntPtr CMFormatDescriptionGetExtensions (/* CMFormatDescriptionRef */ IntPtr desc);

#if !COREBUILD
		
		public NSDictionary GetExtensions ()
		{
			var cfDictRef = CMFormatDescriptionGetExtensions (handle);
			if (cfDictRef == IntPtr.Zero)
				return null;
			return Runtime.GetNSObject<NSDictionary> (cfDictRef);
		}

		[DllImport(Constants.CoreMediaLibrary)]
		extern static /* CFPropertyListRef */ IntPtr CMFormatDescriptionGetExtension (/* CMFormatDescriptionRef */ IntPtr desc, /* CFStringRef */ IntPtr extensionkey);

		public NSObject GetExtension (string extensionKey)
		{
			using (var ns = new NSString (extensionKey)){
				var r =  CMFormatDescriptionGetExtension (handle, ns.Handle);
				if (r == IntPtr.Zero)
					return null;
				return Runtime.GetNSObject<NSObject> (r);
			}
		}

#endif
		
		[DllImport(Constants.CoreMediaLibrary)]
		extern static /* FourCharCode */ uint CMFormatDescriptionGetMediaSubType (/* CMFormatDescriptionRef */ IntPtr desc);

		public uint MediaSubType
		{
			get
			{
				return CMFormatDescriptionGetMediaSubType (handle);
			}
		}

		public AudioFormatType AudioFormatType {
			get {
				return MediaType == CMMediaType.Audio ? (AudioFormatType) MediaSubType : 0;
			}
		}

		public CMSubtitleFormatType SubtitleFormatType {
			get {
				return MediaType == CMMediaType.Subtitle ? (CMSubtitleFormatType) MediaSubType : 0;
			}
		}

		public CMClosedCaptionFormatType ClosedCaptionFormatType {
			get {
				return MediaType == CMMediaType.ClosedCaption ? (CMClosedCaptionFormatType) MediaSubType : 0;				
			}
		}

		public CMMuxedStreamType MuxedStreamType {
			get {
				return MediaType == CMMediaType.Muxed ? (CMMuxedStreamType) MediaSubType : 0;	
			}
		}

		public CMVideoCodecType VideoCodecType {
			get {
				return MediaType == CMMediaType.Video ? (CMVideoCodecType) MediaSubType : 0;
			}
		}

		public CMMetadataFormatType MetadataFormatType {
			get {
				return MediaType == CMMediaType.Metadata ? (CMMetadataFormatType) MediaSubType : 0;
			}
		}

		public CMTimeCodeFormatType TimeCodeFormatType {
			get {
				return MediaType == CMMediaType.TimeCode ? (CMTimeCodeFormatType) MediaSubType : 0;				
			}
		}

		[DllImport(Constants.CoreMediaLibrary)]
		extern static CMMediaType CMFormatDescriptionGetMediaType (/* CMFormatDescriptionRef */ IntPtr desc);
		
		public CMMediaType MediaType
		{
			get
			{
				return CMFormatDescriptionGetMediaType (handle);
			}
		}
		
		[DllImport(Constants.CoreMediaLibrary)]
		extern static /* CFTypeID */ nint CMFormatDescriptionGetTypeID ();
		
		public static nint GetTypeID ()
		{
			return CMFormatDescriptionGetTypeID ();
		}

#if !COREBUILD

		[DllImport (Constants.CoreMediaLibrary)]
		extern static /* OSStatus */ CMFormatDescriptionError CMFormatDescriptionCreate (/* CFAllocatorRef */ IntPtr allocator, CMMediaType mediaType, /* FourCharCode */ uint mediaSubtype, /* CFDictionaryRef */ IntPtr extensions, /* CMFormatDescriptionRef* */ out IntPtr descOut);

		public static CMFormatDescription Create (CMMediaType mediaType, uint mediaSubtype, out CMFormatDescriptionError error)
		{
			IntPtr handle;
			error = CMFormatDescriptionCreate (IntPtr.Zero, mediaType, mediaSubtype, IntPtr.Zero, out handle);
			if (error != CMFormatDescriptionError.None)
				return null;

			return Create (mediaType, handle, true);
		}

		public static CMFormatDescription Create (IntPtr handle, bool owns)
		{
			return Create (CMFormatDescriptionGetMediaType (handle), handle, owns);
		}

		public static CMFormatDescription Create (IntPtr handle)
		{
			return Create (handle, false);
		}

		static CMFormatDescription Create (CMMediaType type, IntPtr handle, bool owns)
		{		
			switch (type) {
			case CMMediaType.Video:
				return new CMVideoFormatDescription (handle, owns);
			case CMMediaType.Audio:
				return new CMAudioFormatDescription (handle, owns);
			default:
				return new CMFormatDescription (handle, owns);
			}
		}

		[DllImport (Constants.CoreMediaLibrary)]
		extern static /* AudioStreamBasicDescription */ IntPtr CMAudioFormatDescriptionGetStreamBasicDescription (/* CMAudioFormatDescriptionRef */ IntPtr desc);

		public AudioStreamBasicDescription? AudioStreamBasicDescription {
			get {
				var ret = CMAudioFormatDescriptionGetStreamBasicDescription (handle);
				if (ret != IntPtr.Zero){
					unsafe {
						return *((AudioStreamBasicDescription *) ret);
					}
				}
				return null;
			}
		}

		[DllImport (Constants.CoreMediaLibrary)]
		extern static /* AudioChannelLayout* */ IntPtr CMAudioFormatDescriptionGetChannelLayout (/* CMAudioFormatDescriptionRef */ IntPtr desc, /* size_t* */ out nint size);
			
		public AudioChannelLayout AudioChannelLayout {
			get {
				nint size;
				var res = CMAudioFormatDescriptionGetChannelLayout (handle, out size);
				if (res == IntPtr.Zero || size == 0)
					return null;
				return AudioChannelLayout.FromHandle (res);
			}
		}

		[DllImport (Constants.CoreMediaLibrary)]
		extern static /* AudioFormatListItem* */ IntPtr CMAudioFormatDescriptionGetFormatList (/* CMAudioFormatDescriptionRef */ IntPtr desc, /* size_t* */ out nint size);

		public AudioFormat [] AudioFormats {
			get {
				unsafe {
					nint size;
					var v = CMAudioFormatDescriptionGetFormatList (handle, out size);
					if (v == IntPtr.Zero)
						return null;
					var items = size / sizeof (AudioFormat);
					var ret = new AudioFormat [items];
					var ptr = (AudioFormat *) v;
					for (int i = 0; i < items; i++)
						ret [i] = ptr [i];
					return ret;
				}
			}
		}

		[DllImport (Constants.CoreMediaLibrary)]
		extern static /* const void* */ IntPtr CMAudioFormatDescriptionGetMagicCookie (/* CMAudioFormatDescriptionRef */ IntPtr desc, /* size_t* */ out nint size);

		public byte [] AudioMagicCookie {
			get {
				nint size;
				var h = CMAudioFormatDescriptionGetMagicCookie (handle, out size);
				if (h == IntPtr.Zero)
					return null;

				var result = new byte [size];
				Marshal.Copy (h, result, 0, result.Length);
				return result;
			}
		}

		[DllImport (Constants.CoreMediaLibrary)]
		extern static /* AudioFormatListItem* */ IntPtr CMAudioFormatDescriptionGetMostCompatibleFormat (/* CMAudioFormatDescriptionRef */ IntPtr desc);

		public AudioFormat AudioMostCompatibleFormat {
			get {
				unsafe {
					var ret = (AudioFormat *) CMAudioFormatDescriptionGetMostCompatibleFormat (handle);
					if (ret == null)
						return new AudioFormat ();
					return *ret;
				}
			}
		}

		[DllImport (Constants.CoreMediaLibrary)]
		extern static /* AudioFormatListItem* */ IntPtr CMAudioFormatDescriptionGetRichestDecodableFormat (/* CMAudioFormatDescriptionRef */ IntPtr desc);

		public AudioFormat AudioRichestDecodableFormat {
			get {
				unsafe {
					var ret = (AudioFormat *) CMAudioFormatDescriptionGetRichestDecodableFormat (handle);
					if (ret == null)
						return new AudioFormat ();
					return *ret;
				}
			}
		}

		// CMVideoDimensions => int32_t width + int32_t height

		[DllImport (Constants.CoreMediaLibrary)]
		internal extern static CMVideoDimensions CMVideoFormatDescriptionGetDimensions (/* CMVideoFormatDescriptionRef */ IntPtr videoDesc);

		[DllImport (Constants.CoreMediaLibrary)]
		internal extern static CGRect CMVideoFormatDescriptionGetCleanAperture (/* CMVideoFormatDescriptionRef */ IntPtr videoDesc, /* Boolean */ bool originIsAtTopLeft);

		[DllImport (Constants.CoreMediaLibrary)]
		internal extern static /* CFArrayRef */ IntPtr CMVideoFormatDescriptionGetExtensionKeysCommonWithImageBuffers ();

		[DllImport (Constants.CoreMediaLibrary)]
		internal extern static CGSize CMVideoFormatDescriptionGetPresentationDimensions (/* CMVideoFormatDescriptionRef */ IntPtr videoDesc, /* Boolean */ bool usePixelAspectRatio, /* Boolean */ bool useCleanAperture);

		[DllImport (Constants.CoreMediaLibrary)]
		internal extern static /* Boolean */ bool CMVideoFormatDescriptionMatchesImageBuffer (/* CMVideoFormatDescriptionRef */ IntPtr videoDesc, /* CVImageBufferRef */ IntPtr imageBuffer);

#if !XAMCORE_2_0
		[Advice ("Use 'CMVideoFormatDescription'.")]
		public Size  VideoDimensions {
			get {
				return CMVideoFormatDescriptionGetDimensions (handle);
			}
		}

		[Advice ("Use 'CMVideoFormatDescription'.")]
		public CGRect GetVideoCleanAperture (bool originIsAtTopLeft)
		{
			return CMVideoFormatDescriptionGetCleanAperture (handle, originIsAtTopLeft);
		}

		// Belongs to CMVideoFormatDescription
		public static NSObject [] GetExtensionKeysCommonWithImageBuffers ()
		{
			var arr = CMVideoFormatDescriptionGetExtensionKeysCommonWithImageBuffers ();
			return NSArray.ArrayFromHandle<NSString> (arr);
		}

		[Advice ("Use 'CMVideoFormatDescription'.")]
		public CGSize GetVideoPresentationDimensions (bool usePixelAspectRatio, bool useCleanAperture)
		{
			return CMVideoFormatDescriptionGetPresentationDimensions (handle, usePixelAspectRatio, useCleanAperture);
		}

		// Belongs to CMVideoFormatDescription
		public bool VideoMatchesImageBuffer (CVImageBuffer imageBuffer)
		{
			if (imageBuffer == null)
				throw new ArgumentNullException ("imageBuffer");
			return CMVideoFormatDescriptionMatchesImageBuffer (handle, imageBuffer.Handle);
		}
#endif
#endif
	}

	public class CMAudioFormatDescription : CMFormatDescription {
		
		internal CMAudioFormatDescription (IntPtr handle)
			: base (handle)
		{
		}

		internal CMAudioFormatDescription (IntPtr handle, bool owns)
			: base (handle, owns)
		{
		}

		// TODO: Move more audio specific methods here
	}

	public partial class CMVideoFormatDescription : CMFormatDescription {
		
		internal CMVideoFormatDescription (IntPtr handle)
			: base (handle)
		{
		}

		internal CMVideoFormatDescription (IntPtr handle, bool owns)
			: base (handle, owns)
		{
		}

		[DllImport (Constants.CoreMediaLibrary)]
		static extern /* OSStatus */ CMFormatDescriptionError CMVideoFormatDescriptionCreate (
			/* CFAllocatorRef */ IntPtr allocator,
			CMVideoCodecType codecType,
			/* int32_t */ int width, 
			/* int32_t */ int height,
			/* CFDictionaryRef */ IntPtr extensions,
			/* CMVideoFormatDescriptionRef* */ out IntPtr outDesc);

		static IntPtr CreateCMVideoFormatDescription (CMVideoCodecType codecType, CMVideoDimensions size)
		{
			IntPtr handle;
			var error = CMVideoFormatDescriptionCreate (IntPtr.Zero, codecType, size.Width, size.Height, IntPtr.Zero, out handle);
			if (error != CMFormatDescriptionError.None)
				throw new ArgumentException (error.ToString ());
			return handle;
		}

		public CMVideoFormatDescription (CMVideoCodecType codecType, CMVideoDimensions size)
			: base (CreateCMVideoFormatDescription (codecType, size), true)
		{
		}

#if !COREBUILD
		public CMVideoDimensions Dimensions {
			get {
				return CMVideoFormatDescriptionGetDimensions (handle);
			}
		}

		[DllImport (Constants.CoreMediaLibrary)]
		static extern /* OSStatus */ CMFormatDescriptionError CMVideoFormatDescriptionCreateForImageBuffer (
			/* CFAllocatorRef */ IntPtr allocator, 
			/* CVImageBufferRef */ IntPtr imageBuffer,
			/* CMVideoFormatDescriptionRef* */ out IntPtr outDesc);

		public static CMVideoFormatDescription CreateForImageBuffer (CVImageBuffer imageBuffer, out CMFormatDescriptionError error)
		{
			if (imageBuffer == null)
				throw new ArgumentNullException ("imageBuffer");

			IntPtr desc;
			error = CMVideoFormatDescriptionCreateForImageBuffer (IntPtr.Zero, imageBuffer.handle, out desc);
			if (error != CMFormatDescriptionError.None)
				return null;

			return new CMVideoFormatDescription (desc, true);
		}

		[iOS (7,0), Mac (10,9)]
		[DllImport (Constants.CoreMediaLibrary)]
		static extern /* OSStatus */ CMFormatDescriptionError CMVideoFormatDescriptionCreateFromH264ParameterSets (
			/* CFAllocatorRef */ IntPtr allocator, 
			/* size_t  */ nuint parameterSetCount,
			/* const uint8_t* const* */ IntPtr[] parameterSetPointers,
			/* size_t*  */ nuint[] parameterSetSizes,
			/* int */ int NALUnitHeaderLength,
			/* CMFormatDescriptionRef* */ out IntPtr formatDescriptionOut);

		[iOS (7,0), Mac (10,9)]
		public static CMVideoFormatDescription FromH264ParameterSets (List<byte[]> parameterSets, int nalUnitHeaderLength, out CMFormatDescriptionError error)
		{
			if (parameterSets == null)
				throw new ArgumentNullException ("parameterSets");

			if (parameterSets.Count < 2)
				throw new ArgumentException ("parameterSets must contain at least two elements");

			if (nalUnitHeaderLength != 1 && nalUnitHeaderLength != 2 && nalUnitHeaderLength != 4)
				throw new ArgumentOutOfRangeException ("nalUnitHeaderLength", "must be 1, 2 or 4");

			var handles = new GCHandle [parameterSets.Count];
			try {
				var parameterSetSizes = new nuint [parameterSets.Count];
				var parameterSetPtrs = new IntPtr [parameterSets.Count];

				for (int i = 0; i < parameterSets.Count; i++) {
					handles [i] = GCHandle.Alloc (parameterSets [i], GCHandleType.Pinned); // This can't use unsafe code because we need to get the pointer for an unbound number of objects.
					parameterSetPtrs [i] = handles [i].AddrOfPinnedObject ();
					parameterSetSizes [i] = (nuint)parameterSets [i].Length;
				}

				IntPtr desc;
				error = CMVideoFormatDescriptionCreateFromH264ParameterSets (IntPtr.Zero, (nuint)parameterSets.Count, parameterSetPtrs, parameterSetSizes, nalUnitHeaderLength, out desc);
				if (error != CMFormatDescriptionError.None)
					return null;

				return new CMVideoFormatDescription (desc, true);
			} finally {
				for (int i = 0; i < parameterSets.Count; i++) {
					if (handles [i].IsAllocated)
						handles [i].Free ();
				}
			}
		}

		[iOS (7,0), Mac (10,9)]
		[DllImport (Constants.CoreMediaLibrary)]
		static extern /* OSStatus */ CMFormatDescriptionError CMVideoFormatDescriptionGetH264ParameterSetAtIndex (
			/* CMFormatDescriptionRef */ IntPtr videoDesc, 
			/* size_t  */ nuint parameterSetIndex,
			/* const uint8_t** */ out IntPtr parameterSetPointerOut,
			/* size_t* */ out nuint parameterSetSizeOut,
			/* size_t* */ out nuint parameterSetCountOut,
			/* int* */ out int nalUnitHeaderLengthOut);

		[iOS (7,0), Mac (10,9)]
		public byte[] GetH264ParameterSet (nuint index, out nuint parameterSetCount, out int nalUnitHeaderLength, out CMFormatDescriptionError error)
		{
			if (Handle == IntPtr.Zero)
				throw new ObjectDisposedException ("VideoFormatDescription");

			IntPtr ret;
			nuint parameterSetSizeOut;
			error = CMVideoFormatDescriptionGetH264ParameterSetAtIndex (Handle, index, out ret, out parameterSetSizeOut, out parameterSetCount, out nalUnitHeaderLength);
			if (error != CMFormatDescriptionError.None)
				return null;

			var arr = new byte[(int)parameterSetSizeOut];
			Marshal.Copy (ret, arr, 0, (int)parameterSetSizeOut);

			return arr;
		}

		public CGRect GetCleanAperture (bool originIsAtTopLeft)
		{
			return CMVideoFormatDescriptionGetCleanAperture (handle, originIsAtTopLeft);
		}

		public CGSize GetPresentationDimensions (bool usePixelAspectRatio, bool useCleanAperture)
		{
			return CMVideoFormatDescriptionGetPresentationDimensions (handle, usePixelAspectRatio, useCleanAperture);
		}

#if XAMCORE_2_0
		public static NSObject [] GetExtensionKeysCommonWithImageBuffers ()
		{
			var arr = CMVideoFormatDescriptionGetExtensionKeysCommonWithImageBuffers ();
			return NSArray.ArrayFromHandle<NSString> (arr);
		}

		public bool VideoMatchesImageBuffer (CVImageBuffer imageBuffer)
		{
			if (imageBuffer == null)
				throw new ArgumentNullException ("imageBuffer");
			return CMVideoFormatDescriptionMatchesImageBuffer (handle, imageBuffer.Handle);
		}
#endif

		[iOS (11,0), Mac (10,13), TV (11,0)]
		[DllImport (Constants.CoreMediaLibrary)]
		static extern /* OSStatus */ CMFormatDescriptionError CMVideoFormatDescriptionCreateFromHEVCParameterSets (
			/* CFAllocatorRef */ IntPtr allocator, 
			/* size_t  */ nuint parameterSetCount,
			/* const uint8_t* const* */ IntPtr [] parameterSetPointers,
			/* size_t*  */ nuint[] parameterSetSizes,
			/* int */ int NALUnitHeaderLength,
			/* CFDictionaryRef */ IntPtr extensions,
			/* CMFormatDescriptionRef* */ out IntPtr formatDescriptionOut);

		[iOS (11,0), Mac (10,13), TV (11,0)]
		public static CMVideoFormatDescription FromHevcParameterSets (List<byte[]> parameterSets, int nalUnitHeaderLength, NSDictionary extensions, out CMFormatDescriptionError error)
		{
			if (parameterSets == null)
				throw new ArgumentNullException (nameof (parameterSets));

			if (parameterSets.Count < 3)
				throw new ArgumentException ($"{nameof (parameterSets)} must contain at least three elements");

			if (nalUnitHeaderLength != 1 && nalUnitHeaderLength != 2 && nalUnitHeaderLength != 4)
				throw new ArgumentOutOfRangeException (nameof (nalUnitHeaderLength), "must be 1, 2 or 4");

			var handles = new GCHandle [parameterSets.Count];
			try {
				var parameterSetSizes = new nuint [parameterSets.Count];
				var parameterSetPtrs = new IntPtr [parameterSets.Count];

				for (int i = 0; i < parameterSets.Count; i++) {
					handles [i] = GCHandle.Alloc (parameterSets [i], GCHandleType.Pinned); // This can't use unsafe code because we need to get the pointer for an unbound number of objects.
					parameterSetPtrs [i] = handles [i].AddrOfPinnedObject ();
					parameterSetSizes [i] = (nuint) parameterSets [i].Length;
				}

				IntPtr desc;
				error = CMVideoFormatDescriptionCreateFromHEVCParameterSets (IntPtr.Zero, (nuint) parameterSets.Count, parameterSetPtrs, parameterSetSizes, nalUnitHeaderLength, extensions.GetHandle (), out desc);
				if (error != CMFormatDescriptionError.None || desc == IntPtr.Zero)
					return null;

				return new CMVideoFormatDescription (desc, true);
			} finally {
				for (int i = 0; i < handles.Length; i++) {
					if (handles [i].IsAllocated)
						handles [i].Free ();
				}
			}
		}

		[iOS (11,0), Mac (10,13), TV (11,0)]
		[DllImport (Constants.CoreMediaLibrary)]
		static extern /* OSStatus */ CMFormatDescriptionError CMVideoFormatDescriptionGetHEVCParameterSetAtIndex (
			/* CMFormatDescriptionRef */ IntPtr videoDesc, 
			/* size_t  */ nuint parameterSetIndex,
			/* const uint8_t** */ out IntPtr parameterSetPointerOut,
			/* size_t* */ out nuint parameterSetSizeOut,
			/* size_t* */ out nuint parameterSetCountOut,
			/* int* */ out int nalUnitHeaderLengthOut);

		[iOS (11,0), Mac (10,13), TV (11,0)]
		public byte [] GetHevcParameterSet (nuint index, out nuint parameterSetCount, out int nalUnitHeaderLength, out CMFormatDescriptionError error)
		{
			if (Handle == IntPtr.Zero)
				throw new ObjectDisposedException ("VideoFormatDescription");

			IntPtr ret;
			nuint parameterSetSizeOut;
			error = CMVideoFormatDescriptionGetHEVCParameterSetAtIndex (Handle, index, out ret, out parameterSetSizeOut, out parameterSetCount, out nalUnitHeaderLength);
			if (error != CMFormatDescriptionError.None)
				return null;

			var arr = new byte [(int) parameterSetSizeOut];
			Marshal.Copy (ret, arr, 0, (int) parameterSetSizeOut);

			return arr;
		}
#endif
	}
}
