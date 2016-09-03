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

using XamCore.Foundation;
using XamCore.CoreFoundation;
using XamCore.CoreGraphics;
using XamCore.ObjCRuntime;
using XamCore.CoreVideo;
using XamCore.AudioToolbox;

#if !XAMCORE_2_0
using System.Drawing;
using CMVideoDimensions = System.Drawing.Size;
#endif

namespace XamCore.CoreMedia {

	// untyped enum (uses as OSStatus) -> CMFormatDescription.h
	public enum CMFormatDescriptionError : int {
		None				= 0,
		InvalidParameter	= -12710,
		AllocationFailed	= -12711,
		ValueNotAvailable   = -12718,
	}

	[iOS (4,0)]
	public class CMFormatDescription : INativeObject, IDisposable {
		internal IntPtr handle;

		internal CMFormatDescription ()
		{
		}

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
		protected internal extern static /* AudioStreamBasicDescription */ IntPtr CMAudioFormatDescriptionGetStreamBasicDescription (/* CMAudioFormatDescriptionRef */ IntPtr desc);

		[DllImport (Constants.CoreMediaLibrary)]
		protected internal extern static /* AudioChannelLayout* */ IntPtr CMAudioFormatDescriptionGetChannelLayout (/* CMAudioFormatDescriptionRef */ IntPtr desc, /* size_t* */ out nint size);

		[DllImport (Constants.CoreMediaLibrary)]
		protected internal extern static /* AudioFormatListItem* */ IntPtr CMAudioFormatDescriptionGetFormatList (/* CMAudioFormatDescriptionRef */ IntPtr desc, /* size_t* */ out nint size);

		[DllImport (Constants.CoreMediaLibrary)]
		protected internal extern static /* const void* */ IntPtr CMAudioFormatDescriptionGetMagicCookie (/* CMAudioFormatDescriptionRef */ IntPtr desc, /* size_t* */ out nint size);

		[DllImport (Constants.CoreMediaLibrary)]
		protected internal extern static /* AudioFormatListItem* */ IntPtr CMAudioFormatDescriptionGetMostCompatibleFormat (/* CMAudioFormatDescriptionRef */ IntPtr desc);

		[DllImport (Constants.CoreMediaLibrary)]
		protected internal extern static /* AudioFormatListItem* */ IntPtr CMAudioFormatDescriptionGetRichestDecodableFormat (/* CMAudioFormatDescriptionRef */ IntPtr desc);

#if !XAMCORE_4_0  // These should all be part of the CMAudioFormatDescription subclass
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

		public AudioChannelLayout AudioChannelLayout {
			get {
				nint size;
				var res = CMAudioFormatDescriptionGetChannelLayout (handle, out size);
				if (res == IntPtr.Zero || size == 0)
					return null;
				return AudioChannelLayout.FromHandle (res);
			}
		}

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
	#endif
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
		[Advice ("Use CMVideoFormatDescription")]
		public Size  VideoDimensions {
			get {
				return CMVideoFormatDescriptionGetDimensions (handle);
			}
		}

		[Advice ("Use CMVideoFormatDescription")]
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

		[Advice ("Use CMVideoFormatDescription")]
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

		[Mac (10,7)]
		[DllImport (Constants.CoreMediaLibrary)]
		static extern CMFormatDescriptionError CMTextFormatDescriptionGetDisplayFlags (
			/* CMFormatDescriptionRef* */ IntPtr desc, 
			out uint outDisplayFlags);

		public uint GetTextDisplayFlags ()
		{
			uint outDisplayFlags;
			var error = CMTextFormatDescriptionGetDisplayFlags (handle, out outDisplayFlags);
			if (error != CMFormatDescriptionError.None)
				throw new ArgumentException (error.ToString ());

			return outDisplayFlags;
		}

		[Mac (10,7)]
		[DllImport (Constants.CoreMediaLibrary)]
		static extern CMFormatDescriptionError CMTextFormatDescriptionGetJustification (
			/* CMFormatDescriptionRef* */ IntPtr desc,  
			out bool outHorizontalJust, 
			out bool outVerticalJust);

		public void GetTextJustification (out bool outHorizontalJust, out bool outVerticalJust)
		{
			var error = CMTextFormatDescriptionGetJustification (handle, out outHorizontalJust, out outVerticalJust);
			if (error != CMFormatDescriptionError.None)
				throw new ArgumentException (error.ToString ());
		}

		[Mac (10,7)]
		[DllImport (Constants.CoreMediaLibrary)]
		static extern CMFormatDescriptionError CMTextFormatDescriptionGetDefaultTextBox (
			/* CMFormatDescriptionRef* */ IntPtr desc, 
			bool originIsAtTopLeft, 
			nfloat heightOfTextTrack, 
			out CGRect outDefaultTextBox);

		public CGRect GetDefaultTextBox (bool originIsAtTopLeft, nfloat heightOfTextTrack)
		{
			CGRect outDefaultTextBox;
			var error = CMTextFormatDescriptionGetDefaultTextBox (handle, originIsAtTopLeft, heightOfTextTrack, out outDefaultTextBox);
			if (error != CMFormatDescriptionError.None)
				throw new ArgumentException (error.ToString ());

			return outDefaultTextBox;
		}

		[Mac (10,7)]
		[DllImport (Constants.CoreMediaLibrary)]
		static extern CMFormatDescriptionError CMTextFormatDescriptionGetDefaultStyle (
			/* CMFormatDescriptionRef* */ IntPtr desc, 
			out ushort outLocalFontID, 
			out bool outBold, 
			out bool outItalic, 
			out bool outUnderline, 
			out nfloat outFontSize, 
			nfloat[] outColorComponents);

		public void GetTextDefaultStyle (out ushort outLocalFontId, out bool outBold, out bool outItalic, out bool outUnderline, out nfloat outFontSize, nfloat[] outColorComponents)
		{
			var error = CMTextFormatDescriptionGetDefaultStyle (handle, out outLocalFontId, out outBold, out outItalic, out outUnderline, out outFontSize, outColorComponents);
			if (error != CMFormatDescriptionError.None)
				throw new ArgumentException (error.ToString ());
		}

		[Mac (10,7)]
		[DllImport (Constants.CoreMediaLibrary)]
		static extern CMFormatDescriptionError CMTextFormatDescriptionGetFontName (
			/* CMFormatDescriptionRef* */ IntPtr desc, 
			ushort localFontID, 
			/* CFStringRef** */ out string outFontName);

		public string GetTextFontName (ushort localFontId)
		{
			string outFontName;
			var error = CMTextFormatDescriptionGetFontName (handle, localFontId, out outFontName);
			if (error != CMFormatDescriptionError.None)
				throw new ArgumentException (error.ToString ());

			return outFontName;
		}
#endif
#endif
	}

	[iOS (4,0)]
	public class CMAudioFormatDescription : CMFormatDescription {
		internal CMAudioFormatDescription (IntPtr handle)
			: base (handle)
		{
		}

		internal CMAudioFormatDescription (IntPtr handle, bool owns)
			: base (handle, owns)
		{
		}

#if !COREBUILD
		[DllImport (Constants.CoreMediaLibrary)]
		static unsafe extern /* OSStatus */ CMFormatDescriptionError CMAudioFormatDescriptionCreate (
			/* CFAllocatorRef */ IntPtr allocator, 
			ref AudioStreamBasicDescription description,
			nuint layoutSize, 
			/* AudioChannelLayout* */ IntPtr layout, 
			nuint magicCookieSize, 
			/* void* */ IntPtr magicCookie, 
			/* CFDictionaryRef* */ IntPtr extensions,
			out IntPtr outDesc);

		public CMAudioFormatDescription (ref AudioStreamBasicDescription asbd) : this (ref asbd, null, 0, IntPtr.Zero, null)
		{
		}

		public CMAudioFormatDescription (ref AudioStreamBasicDescription asbd, AudioChannelLayout layout) : this (ref asbd, layout, 0, IntPtr.Zero, null)
		{
		}

		public CMAudioFormatDescription (ref AudioStreamBasicDescription asbd, uint magicCookieSize, IntPtr magicCookie) : this (ref asbd, null, magicCookieSize, magicCookie, null)
		{
		}

		public CMAudioFormatDescription (ref AudioStreamBasicDescription asbd, NSDictionary extensions) : this (ref asbd, null, 0, IntPtr.Zero, extensions)
		{
		}

		public CMAudioFormatDescription (ref AudioStreamBasicDescription asbd, AudioChannelLayout layout, uint magicCookieSize, IntPtr magicCookie) : this (ref asbd, layout, magicCookieSize, magicCookie, null)
		{
		}

		public CMAudioFormatDescription (ref AudioStreamBasicDescription asbd, AudioChannelLayout layout, NSDictionary extensions) : this (ref asbd, layout, 0, IntPtr.Zero, extensions)
		{
		}

		public CMAudioFormatDescription (ref AudioStreamBasicDescription asbd, uint magicCookieSize, IntPtr magicCookie, NSDictionary extensions) : this (ref asbd, null, magicCookieSize, magicCookie, extensions)
		{
		}

		public CMAudioFormatDescription (ref AudioStreamBasicDescription asbd, AudioChannelLayout layout, uint magicCookieSize, IntPtr magicCookie, NSDictionary extensions)
		{
			IntPtr ptr = IntPtr.Zero;
			uint size = 0;
			if (layout != null) {
				size = (uint)Marshal.SizeOf (layout.NativeStruct);
				Marshal.StructureToPtr (layout.NativeStruct, ptr, false);
			}
			var error = CMAudioFormatDescriptionCreate (IntPtr.Zero, ref asbd, size, ptr, magicCookieSize, magicCookie, extensions != null ? extensions.Handle : IntPtr.Zero, out handle);
			
			if (error != CMFormatDescriptionError.None)
				throw new ArgumentException (error.ToString ());
		}

		public unsafe AudioStreamBasicDescription? StreamBasicDescription {
			get {
				var ret = CMAudioFormatDescriptionGetStreamBasicDescription (handle);
				if (ret != IntPtr.Zero) {
					return *((AudioStreamBasicDescription*)ret);
				}
				return null;
			}
		}

		public AudioChannelLayout ChannelLayout {
			get {
				nint size;
				var res = CMAudioFormatDescriptionGetChannelLayout (handle, out size);
				if (res == IntPtr.Zero || size == 0)
					return null;
				return AudioChannelLayout.FromHandle (res);
			}
		}

		public AudioFormat [] Formats {
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

		public byte [] MagicCookie {
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

		public AudioFormat MostCompatibleFormat {
			get {
				unsafe {
					var ret = (AudioFormat *) CMAudioFormatDescriptionGetMostCompatibleFormat (handle);
					if (ret == null)
						return new AudioFormat ();
					return *ret;
				}
			}
		}

		public AudioFormat RichestDecodableFormat {
			get {
				unsafe {
					var ret = (AudioFormat *) CMAudioFormatDescriptionGetRichestDecodableFormat (handle);
					if (ret == null)
						return new AudioFormat ();
					return *ret;
				}
			}
		}
#endif
	}

	[iOS (4,0)]
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
		
#if !COREBUILD
		public CMVideoFormatDescription (CMVideoCodecType codecType, CMVideoDimensions size) : this (codecType, size.Width, size.Height, null)
		{
		}

		public CMVideoFormatDescription (CMVideoCodecType codecType, CMVideoDimensions size, NSDictionary extensions) : this (codecType, size.Width, size.Height, extensions)
		{
		}

		public CMVideoFormatDescription (CMVideoCodecType codecType, int width, int height, NSDictionary extensions)
		{
			var error = CMVideoFormatDescriptionCreate (IntPtr.Zero, codecType, width, height, extensions != null ? extensions.Handle : IntPtr.Zero, out handle);
			if (error != CMFormatDescriptionError.None)
				throw new ArgumentException (error.ToString ());
		}

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

		public CMVideoFormatDescription (CVImageBuffer imageBuffer)
		{
			if (imageBuffer == null)
				throw new ArgumentNullException ("imageBuffer");

			var error = CMVideoFormatDescriptionCreateForImageBuffer (IntPtr.Zero, imageBuffer.handle, out handle);
			if (error != CMFormatDescriptionError.None)
				throw new ArgumentException (error.ToString ());
		}

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
					handles [i] = GCHandle.Alloc (parameterSets [i], GCHandleType.Pinned);
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
#endif
	}

	public partial class CMMuxedFormatDescription : CMFormatDescription {
		internal CMMuxedFormatDescription (IntPtr handle)
			: base (handle)
		{
		}

		internal CMMuxedFormatDescription (IntPtr handle, bool owns)
			: base (handle, owns)
		{
		}

#if !COREBUILD
		[Mac (10,7)]
		[DllImport (Constants.CoreMediaLibrary)]
		static extern CMFormatDescriptionError CMMuxedFormatDescriptionCreate (
			/* CFAllocatorRef */ IntPtr allocator, 
			uint muxType, 
			/* CFDictionaryRef */ IntPtr extensions, 
			/* CMMuxedFormatDescriptionRef */ out IntPtr outDesc);
		
		public CMMuxedFormatDescription (uint muxType, NSDictionary extensions = null)
		{
			var error = CMMuxedFormatDescriptionCreate (IntPtr.Zero, muxType, extensions != null ? extensions.Handle : IntPtr.Zero, out handle);
			if (error != CMFormatDescriptionError.None)
				throw new ArgumentException (error.ToString ());
		}
#endif
	}

	public partial class CMTimeCodeFormatDescription : CMFormatDescription {
		internal CMTimeCodeFormatDescription (IntPtr handle)
			: base (handle)
		{
		}

		internal CMTimeCodeFormatDescription (IntPtr handle, bool owns)
			: base (handle, owns)
		{
		}

#if !COREBUILD
		[Mac (10,7)]
		[DllImport (Constants.CoreMediaLibrary)]
		static extern CMFormatDescriptionError CMTimeCodeFormatDescriptionCreate (
			/* CFAllocatorRef* */ IntPtr allocator, 
			uint timeCodeFormatType, 
			CMTime frameDuration, 
			uint frameQuanta, 
			uint tcFlags, 
			/* CFDictionaryRef* */ IntPtr extensions, 
			/* CMTimeCodeFormatDescriptionRef* */ out IntPtr descOut);

		public CMTimeCodeFormatDescription (uint timeCodeFormatType, CMTime frameDuration, uint frameQuanta, uint tcFlags, NSDictionary extensions = null)
		{
			var error = CMTimeCodeFormatDescriptionCreate (IntPtr.Zero, timeCodeFormatType, frameDuration, frameQuanta, tcFlags, extensions != null ? extensions.Handle : IntPtr.Zero, out handle);
			if (error != CMFormatDescriptionError.None)
				throw new ArgumentException (error.ToString ());
		}

		[Mac (10,7)]
		[DllImport (Constants.CoreMediaLibrary)]
		static extern CMTime CMTimeCodeFormatDescriptionGetFrameDuration (
			/*CMTimeCodeFormatDescriptionRef* */ IntPtr timeCodeFormatDescription);

		public CMTime GetFrameDuration ()
		{
			return CMTimeCodeFormatDescriptionGetFrameDuration (handle);
		}

		[Mac (10,7)]
		[DllImport (Constants.CoreMediaLibrary)]
		static extern uint CMTimeCodeFormatDescriptionGetFrameQuanta (
			/*CMTimeCodeFormatDescriptionRef* */ IntPtr timeCodeFormatDescription);

		public uint GetFrameQuanta ()
		{
			return CMTimeCodeFormatDescriptionGetFrameQuanta (handle);
		}

		[Mac (10,7)]
		[DllImport (Constants.CoreMediaLibrary)]
		static extern uint CMTimeCodeFormatDescriptionGetTimeCodeFlags (
			/*CMTimeCodeFormatDescriptionRef* */ IntPtr desc);

		public uint GetTimeCodeFlags ()
		{
			return CMTimeCodeFormatDescriptionGetTimeCodeFlags (handle);
		}
#endif
	}

	public partial class CMMetadataFormatDescription : CMFormatDescription {
		internal CMMetadataFormatDescription (IntPtr handle)
			: base (handle)
		{
		}

		internal CMMetadataFormatDescription (IntPtr handle, bool owns)
			: base (handle, owns)
		{
		}

#if !COREBUILD
		[Mac (10,7)]
		[DllImport (Constants.CoreMediaLibrary)]
		static extern CMFormatDescriptionError CMMetadataFormatDescriptionCreateWithKeys (
			/* CFAllocatorRef* */ IntPtr allocator, 
			CMMetadataFormatType metadataType, 
			/* CFArrayRef */ IntPtr keys, 
			/* CMMetadataFormatDescriptionRef* */ out IntPtr outDesc);

		public CMMetadataFormatDescription (CMMetadataFormatType metadataType, NSArray keys = null)
		{
			var error = CMMetadataFormatDescriptionCreateWithKeys (IntPtr.Zero, metadataType, keys != null ? keys.Handle : IntPtr.Zero, out handle);
			if (error != CMFormatDescriptionError.None)
				throw new ArgumentException (error.ToString ());
		}

		[Mac (10,10)]
		[DllImport (Constants.CoreMediaLibrary)]
		static extern CMFormatDescriptionError CMMetadataFormatDescriptionCreateWithMetadataSpecifications (
			/* CFAllocatorRef* */ IntPtr allocator,
			CMMetadataFormatType metadataType,
			/* CFArrayRef* */ IntPtr metadataSpecifications, 
			/* CMMetadataFormatDescriptionRef* */ out IntPtr outDesc);

		public static CMMetadataFormatDescription FromMetadataSpecifications (CMMetadataFormatType metadataType, NSArray metadataSpecifications)
		{
			IntPtr handle;
			var error = CMMetadataFormatDescriptionCreateWithMetadataSpecifications (IntPtr.Zero, metadataType, metadataSpecifications != null ? metadataSpecifications.Handle : IntPtr.Zero, out handle);
			if (error != CMFormatDescriptionError.None)
				throw new ArgumentException (error.ToString ());

			return new CMMetadataFormatDescription (handle, true);
		}

		[Mac (10,10)]
		[DllImport (Constants.CoreMediaLibrary)]
		static extern CMFormatDescriptionError CMMetadataFormatDescriptionCreateWithMetadataFormatDescriptionAndMetadataSpecifications (
			/* CFAllocatorRef */ IntPtr allocator, 
			/* CMMetadataFormatDescriptionRef */ IntPtr srcDesc, 
			/* CFArrayRef */ IntPtr metadataSpecifications, 
			/* CMMetadataFormatDescriptionRef* */ out IntPtr outDesc);

		public CMMetadataFormatDescription (CMFormatDescription srcDesc, NSArray metadataSpecifications)
		{
			var error = CMMetadataFormatDescriptionCreateWithMetadataFormatDescriptionAndMetadataSpecifications (IntPtr.Zero, srcDesc != null ? srcDesc.Handle : IntPtr.Zero, metadataSpecifications != null ? metadataSpecifications.Handle : IntPtr.Zero, out handle);
			if (error != CMFormatDescriptionError.None)
				throw new ArgumentException (error.ToString ());
		}

		[Mac (10,10)]
		[DllImport (Constants.CoreMediaLibrary)]
		static extern CMFormatDescriptionError CMMetadataFormatDescriptionCreateByMergingMetadataFormatDescriptions (
			/* CFAllocatorRef */ IntPtr allocator, 
			/* CMMetadataFormatDescriptionRef */ IntPtr srcDesc1, 
			/* CMMetadataFormatDescriptionRef */ IntPtr srcDesc2, 
			/* CMMetadataFormatDescriptionRef* */ out IntPtr outDesc);

		public CMMetadataFormatDescription (CMMetadataFormatDescription srcDesc1, CMMetadataFormatDescription srcDesc2)
		{
			var error = CMMetadataFormatDescriptionCreateByMergingMetadataFormatDescriptions (IntPtr.Zero, srcDesc1 != null ? srcDesc1.Handle : IntPtr.Zero, srcDesc2 != null ? srcDesc2.Handle : IntPtr.Zero, out handle);
			if (error != CMFormatDescriptionError.None)
				throw new ArgumentException (error.ToString ());
		}

		[Mac (10,7)]
		[DllImport (Constants.CoreMediaLibrary)]
		static extern /* CFDictionaryRef* */ IntPtr CMMetadataFormatDescriptionGetKeyWithLocalID (
			/* CMMetadataFormatDescriptionRef */ IntPtr desc, 
			uint localKeyID);

		public NSDictionary GetKey (uint localKeyId)
		{
			IntPtr handle = CMMetadataFormatDescriptionGetKeyWithLocalID (Handle, localKeyId);
			if (handle == IntPtr.Zero)
				return null;

			return new NSDictionary (handle, false);
		}

		[Mac (10,10)]
		[DllImport (Constants.CoreMediaLibrary)]
		static extern /* CFArrayRef */ IntPtr CMMetadataFormatDescriptionGetIdentifiers (
			/* CMMetadataFormatDescriptionRef*/ IntPtr desc);

		public NSString [] GetIdentifiers ()
		{
			IntPtr handle = CMMetadataFormatDescriptionGetIdentifiers (Handle);
			return NSArray.ArrayFromHandle <NSString> (handle);
		}
#endif
	}
}
