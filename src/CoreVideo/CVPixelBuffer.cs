// 
// CVPixelBuffer.cs: Implements the managed CVPixelBuffer
//
// Authors: Mono Team
//     
// Copyright 2010 Novell, Inc
// Copyright 2011-2014 Xamarin Inc
//
using System;
using System.Runtime.InteropServices;
using CoreFoundation;
using ObjCRuntime;
using Foundation;

namespace CoreVideo {

	[Watch (4,0)]
	public partial class CVPixelBuffer : CVImageBuffer {
#if !COREBUILD
		[DllImport (Constants.CoreVideoLibrary, EntryPoint = "CVPixelBufferGetTypeID")]
		public extern static /* CFTypeID */ nint GetTypeID ();

#if !XAMCORE_2_0
		public static readonly NSString PixelFormatTypeKey;
		public static readonly NSString MemoryAllocatorKey;
		public static readonly NSString WidthKey;
		public static readonly NSString HeightKey;
		public static readonly NSString ExtendedPixelsLeftKey;
		public static readonly NSString ExtendedPixelsTopKey;
		public static readonly NSString ExtendedPixelsRightKey;
		public static readonly NSString ExtendedPixelsBottomKey;
		public static readonly NSString BytesPerRowAlignmentKey;
		public static readonly NSString CGBitmapContextCompatibilityKey;
		public static readonly NSString CGImageCompatibilityKey;
		public static readonly NSString OpenGLCompatibilityKey;
		public static readonly NSString IOSurfacePropertiesKey;
		public static readonly NSString PlaneAlignmentKey;
#if !MONOMAC || !XAMCORE_2_0
		public static readonly NSString MetalCompatibilityKey;
		public static readonly NSString OpenGLESCompatibilityKey;
#endif

		public static readonly nint CVImageBufferType;

		static CVPixelBuffer ()
		{
			var handle = Dlfcn.dlopen (Constants.CoreVideoLibrary, 0);
			if (handle == IntPtr.Zero)
				return;
			try {
				PixelFormatTypeKey = Dlfcn.GetStringConstant (handle, "kCVPixelBufferPixelFormatTypeKey");
				MemoryAllocatorKey = Dlfcn.GetStringConstant (handle, "kCVPixelBufferMemoryAllocatorKey");
				WidthKey = Dlfcn.GetStringConstant (handle, "kCVPixelBufferWidthKey");
				HeightKey = Dlfcn.GetStringConstant (handle, "kCVPixelBufferHeightKey");
				ExtendedPixelsLeftKey = Dlfcn.GetStringConstant (handle, "kCVPixelBufferExtendedPixelsLeftKey");
				ExtendedPixelsTopKey = Dlfcn.GetStringConstant (handle, "kCVPixelBufferExtendedPixelsTopKey");
				ExtendedPixelsRightKey = Dlfcn.GetStringConstant (handle, "kCVPixelBufferExtendedPixelsRightKey");
				ExtendedPixelsBottomKey = Dlfcn.GetStringConstant (handle, "kCVPixelBufferExtendedPixelsBottomKey");
				BytesPerRowAlignmentKey = Dlfcn.GetStringConstant (handle, "kCVPixelBufferBytesPerRowAlignmentKey");
				CGBitmapContextCompatibilityKey = Dlfcn.GetStringConstant (handle, "kCVPixelBufferCGBitmapContextCompatibilityKey");
				CGImageCompatibilityKey = Dlfcn.GetStringConstant (handle, "kCVPixelBufferCGImageCompatibilityKey");
				OpenGLCompatibilityKey = Dlfcn.GetStringConstant (handle, "kCVPixelBufferOpenGLCompatibilityKey");
				IOSurfacePropertiesKey = Dlfcn.GetStringConstant (handle, "kCVPixelBufferIOSurfacePropertiesKey");
				PlaneAlignmentKey = Dlfcn.GetStringConstant (handle, "kCVPixelBufferPlaneAlignmentKey");
				CVImageBufferType = GetTypeID ();
				MetalCompatibilityKey = Dlfcn.GetStringConstant (handle, "kCVPixelBufferMetalCompatibilityKey");
#if !MONOMAC
				OpenGLESCompatibilityKey = Dlfcn.GetStringConstant (handle, "kCVPixelBufferOpenGLESCompatibilityKey");
#endif
			}
			finally {
				Dlfcn.dlclose (handle);
			}
		}
#endif

		internal CVPixelBuffer (IntPtr handle) : base (handle)
		{
		}

		[Preserve (Conditional=true)]
		internal CVPixelBuffer (IntPtr handle, bool owns) : base (handle, owns)
		{
		}

#if !COREBUILD
		[DllImport (Constants.CoreVideoLibrary)]
		extern static CVReturn CVPixelBufferCreate (
			/* CFAllocatorRef __nullable */ IntPtr allocator, /* size_t */ nint width, /* size_t */ nint height, 
			/* OSType */ CVPixelFormatType pixelFormatType,
			/* CFDictionaryRef __nullable */ IntPtr pixelBufferAttributes,
			/* CVPixelBufferRef __nullable * __nonnull */ out IntPtr pixelBufferOut);

		public CVPixelBuffer (nint width, nint height, CVPixelFormatType pixelFormat)
			: this (width, height, pixelFormat, (NSDictionary) null)
		{
		}

		public CVPixelBuffer (nint width, nint height, CVPixelFormatType pixelFormatType, CVPixelBufferAttributes attributes)
			: this (width, height, pixelFormatType, attributes == null ? null : attributes.Dictionary)
		{
		}

#if !XAMCORE_2_0
		public CVPixelBuffer (System.Drawing.Size size, CVPixelFormatType pixelFormat)
			: this (size.Width, size.Height, pixelFormat, (NSDictionary) null)
		{
		}

		public CVPixelBuffer (System.Drawing.Size size, CVPixelFormatType pixelFormatType, CVPixelBufferAttributes attributes)
			: this (size.Width, size.Height, pixelFormatType, attributes == null ? null : attributes.Dictionary)
		{
		}
#endif

		[Advice ("Use constructor with CVPixelBufferAttributes")]
#if !XAMCORE_2_0
		public
#endif
		CVPixelBuffer (nint width, nint height, CVPixelFormatType pixelFormatType, NSDictionary pixelBufferAttributes)
		{
			if (width <= 0)
				throw new ArgumentOutOfRangeException ("width");

			if (height <= 0)
				throw new ArgumentOutOfRangeException ("height");

			CVReturn ret = CVPixelBufferCreate (IntPtr.Zero, width, height, pixelFormatType,
				pixelBufferAttributes == null ? IntPtr.Zero : pixelBufferAttributes.Handle, out handle);

			if (ret != CVReturn.Success)
				throw new ArgumentException (ret.ToString ());
		}

		[DllImport (Constants.CoreVideoLibrary)]
		extern static CVReturn CVPixelBufferCreateResolvedAttributesDictionary (
			/* CFAllocatorRef __nullable */ IntPtr allocator,
			/* CFArrayRef __nullable */ IntPtr attributes,
			/* CFDictionaryRef __nullable * __nonnull */ out IntPtr resolvedDictionaryOut);

		public NSDictionary GetAttributes (NSDictionary [] attributes)
		{
			CVReturn ret;
			IntPtr resolvedDictionaryOut;
			if (attributes == null) {
				ret = CVPixelBufferCreateResolvedAttributesDictionary (IntPtr.Zero, IntPtr.Zero, out resolvedDictionaryOut);
			} else {
				using (var array = NSArray.FromNSObjects (attributes)) {
					ret = CVPixelBufferCreateResolvedAttributesDictionary (IntPtr.Zero, array.Handle, out resolvedDictionaryOut);
				}
			}
			if (ret != CVReturn.Success)
				throw new ArgumentException (ret.ToString ());
			return Runtime.GetNSObject<NSDictionary> (resolvedDictionaryOut);
		}
#endif

		/* CVPixelBufferCreateWithBytes */

		delegate void CVPixelBufferReleaseBytesCallback (
			/* void * CV_NULLABLE */ IntPtr releaseRefCon, 
			/* const void * CV_NULLABLE */ IntPtr baseAddress);
		
		static CVPixelBufferReleaseBytesCallback releaseBytesCallback = new CVPixelBufferReleaseBytesCallback (ReleaseBytesCallback);

		[MonoPInvokeCallbackAttribute (typeof (CVPixelBufferReleaseBytesCallback))]
		static void ReleaseBytesCallback (IntPtr releaseRefCon, IntPtr baseAddress)
		{
			GCHandle handle = GCHandle.FromIntPtr (releaseRefCon);
			handle.Free ();
		}

		[DllImport (Constants.CoreVideoLibrary)]
		static extern CVReturn CVPixelBufferCreateWithBytes (
			/* CFAllocatorRef CV_NULLABLE */ IntPtr allocator,
			/* size_t */ nint width,
			/* size_t */ nint height,
			/* OSType */ CVPixelFormatType pixelFormatType,
			/* void * CV_NONNULL */ IntPtr baseAddress,
			/* size_t */ nint bytesPerRow,
			CVPixelBufferReleaseBytesCallback /* CV_NULLABLE */ releaseCallback,
			/* void * CV_NULLABLE */ IntPtr releaseRefCon,
			/* CFDictionaryRef CV_NULLABLE */ IntPtr pixelBufferAttributes,
			/* CV_RETURNS_RETAINED_PARAMETER CVPixelBufferRef CV_NULLABLE * CV_NONNULL */ out IntPtr pixelBufferOut);// __OSX_AVAILABLE_STARTING(__MAC_10_4,__IPHONE_4_0);

		public static CVPixelBuffer Create (nint width, nint height, CVPixelFormatType pixelFormatType, byte[] data, nint bytesPerRow, CVPixelBufferAttributes pixelBufferAttributes)
		{
			CVReturn status;
			return Create (width, height, pixelFormatType, data, bytesPerRow, pixelBufferAttributes, out status);
		}

		public static CVPixelBuffer Create (nint width, nint height, CVPixelFormatType pixelFormatType, byte[] data, nint bytesPerRow, CVPixelBufferAttributes pixelBufferAttributes, out CVReturn status)
		{
			IntPtr handle;
			GCHandle gchandle;

			if (data == null)
				throw new ArgumentNullException (nameof (data));

			if (data.Length < height * bytesPerRow)
				throw new ArgumentOutOfRangeException (nameof (data), "The length of data is smaller than height * bytesPerRow");

			gchandle = GCHandle.Alloc (data, GCHandleType.Pinned); // This requires a pinned GCHandle, because unsafe code is scoped to the current block, and the address of the byte array will be used after this function returns.

			status = CVPixelBufferCreateWithBytes (IntPtr.Zero, width, height, pixelFormatType, gchandle.AddrOfPinnedObject (), bytesPerRow, releaseBytesCallback, GCHandle.ToIntPtr (gchandle), DictionaryContainerHelper.GetHandle (pixelBufferAttributes), out handle);

			if (status != CVReturn.Success) {
				gchandle.Free ();
				return null;
			}

			return new CVPixelBuffer (handle, true);
		}

		/* CVPixelBufferCreateWithPlanarBytes */

		class PlaneData
		{
			public GCHandle[] dataHandles;
		}

		delegate void CVPixelBufferReleasePlanarBytesCallback (
			/* void* */ IntPtr releaseRefCon,
			/* const void*/ IntPtr dataPtr, 
			/* size_t */ nint dataSize,
			/* size_t */ nint numberOfPlanes,
			/* const void* */IntPtr planeAddresses);

		static CVPixelBufferReleasePlanarBytesCallback releasePlanarBytesCallback = new CVPixelBufferReleasePlanarBytesCallback (ReleasePlanarBytesCallback);

		[MonoPInvokeCallbackAttribute (typeof (CVPixelBufferReleasePlanarBytesCallback))]
		static void ReleasePlanarBytesCallback (IntPtr releaseRefCon, IntPtr dataPtr, nint dataSize, nint numberOfPlanes, IntPtr planeAddresses)
		{
			GCHandle handle = GCHandle.FromIntPtr (releaseRefCon);
			PlaneData data = (PlaneData) handle.Target;
			for (int i = 0; i < data.dataHandles.Length; i++)
				data.dataHandles[i].Free ();
			handle.Free ();
		}

		[DllImport (Constants.CoreVideoLibrary)]
		static extern CVReturn CVPixelBufferCreateWithPlanarBytes (
			/* CFAllocatorRef CV_NULLABLE */ IntPtr allocator,
			/* size_t */ nint width,
			/* size_t */ nint height,
			/* OSType */ CVPixelFormatType pixelFormatType,
			/* void * CV_NULLABLE */ IntPtr dataPtr, /* pass a pointer to a plane descriptor block, or NULL /*
			/* size_t */ nint dataSize, /* pass size if planes are contiguous, NULL if not */
			/* size_t */ nint numberOfPlanes,
			/* void *[] CV_NULLABLE */ IntPtr[] planeBaseAddress,
			/* size_t[] */ nint [] planeWidth,
			/* size_t[] */ nint [] planeHeight,
			/* size_t[] */ nint [] planeBytesPerRow,
			CVPixelBufferReleasePlanarBytesCallback /* CV_NULLABLE */ releaseCallback,
			/* void * CV_NULLABLE */ IntPtr releaseRefCon,
			/* CFDictionaryRef CV_NULLABLE */ IntPtr pixelBufferAttributes,
			/* CV_RETURNS_RETAINED_PARAMETER CVPixelBufferRef CV_NULLABLE * CV_NONNULL */ out IntPtr pixelBufferOut); // __OSX_AVAILABLE_STARTING(__MAC_10_4,__IPHONE_4_0);

		public static CVPixelBuffer Create (nint width, nint height, CVPixelFormatType pixelFormatType, byte[][] planes, nint[] planeWidths, nint[] planeHeights, nint[] planeBytesPerRow, CVPixelBufferAttributes pixelBufferAttributes)
		{
			CVReturn status;
			return Create (width, height, pixelFormatType, planes, planeWidths, planeHeights, planeBytesPerRow, pixelBufferAttributes, out status);
		}

		public static CVPixelBuffer Create (nint width, nint height, CVPixelFormatType pixelFormatType, byte[][] planes, nint[] planeWidths, nint[] planeHeights, nint[] planeBytesPerRow, CVPixelBufferAttributes pixelBufferAttributes, out CVReturn status)
		{
			IntPtr handle;
			IntPtr[] addresses;
			PlaneData data;
			GCHandle data_handle;

			if (planes == null)
				throw new ArgumentNullException (nameof (planes));

			if (planeWidths == null)
				throw new ArgumentNullException (nameof (planeWidths));

			if (planeHeights == null)
				throw new ArgumentNullException (nameof (planeHeights));

			if (planeBytesPerRow == null)
				throw new ArgumentNullException (nameof (planeBytesPerRow));

			var planeCount = planes.Length;

			if (planeWidths.Length != planeCount)
				throw new ArgumentOutOfRangeException (nameof (planeWidths), "The length of planeWidths does not match the number of planes");
			
			if (planeHeights.Length != planeCount)
				throw new ArgumentOutOfRangeException (nameof (planeWidths), "The length of planeHeights does not match the number of planes");

			if (planeBytesPerRow.Length != planeCount)
				throw new ArgumentOutOfRangeException (nameof (planeWidths), "The length of planeBytesPerRow does not match the number of planes");

			addresses = new IntPtr [planeCount];
			data = new PlaneData ();
			data.dataHandles = new GCHandle [planeCount];
			for (int i = 0; i < planeCount; i++) {
				data.dataHandles[i] = GCHandle.Alloc (planes [i], GCHandleType.Pinned); // This can't use unsafe code because we need to get the pointer for an unbound number of objects.
				addresses[i] = data.dataHandles[i].AddrOfPinnedObject ();
			}
			data_handle = GCHandle.Alloc (data);

			IntPtr data_handle_ptr = GCHandle.ToIntPtr (data_handle);
			status = CVPixelBufferCreateWithPlanarBytes (IntPtr.Zero, 
			                                             width, height, pixelFormatType, IntPtr.Zero, 0, 
			                                             planeCount, addresses, planeWidths, planeHeights, planeBytesPerRow, 
			                                             releasePlanarBytesCallback, data_handle_ptr,
			                                             DictionaryContainerHelper.GetHandle (pixelBufferAttributes), out handle);

			if (status != CVReturn.Success) {
				ReleasePlanarBytesCallback (data_handle_ptr, IntPtr.Zero, 0, 0, IntPtr.Zero);
				return null;
			}

			return new CVPixelBuffer (handle, true);
		}

		[DllImport (Constants.CoreVideoLibrary)]
		static extern void CVPixelBufferGetExtendedPixels (/* CVPixelBufferRef __nonnull */ IntPtr pixelBuffer,
			/* size_t* */ ref nuint extraColumnsOnLeft, /* size_t* */ ref nuint extraColumnsOnRight,
			/* size_t* */ ref nuint extraRowsOnTop, /* size_t* */ ref nuint extraRowsOnBottom);

		public void GetExtendedPixels (ref nuint extraColumnsOnLeft, ref nuint extraColumnsOnRight,
			ref nuint extraRowsOnTop, ref nuint extraRowsOnBottom)
		{
			CVPixelBufferGetExtendedPixels (handle, ref extraColumnsOnLeft, ref extraColumnsOnRight, 
				ref extraRowsOnTop, ref extraRowsOnBottom);
		}

		[DllImport (Constants.CoreVideoLibrary)]
		extern static CVReturn CVPixelBufferFillExtendedPixels (/* CVPixelBufferRef __nonnull */ IntPtr pixelBuffer);

		public CVReturn FillExtendedPixels ()
		{
			return CVPixelBufferFillExtendedPixels (handle);
		}

		[DllImport (Constants.CoreVideoLibrary)]
		extern static /* void* __nullable */ IntPtr CVPixelBufferGetBaseAddress (
			/* CVPixelBufferRef __nonnull */ IntPtr pixelBuffer);

		public IntPtr BaseAddress {
			get {
				return CVPixelBufferGetBaseAddress (handle);
			}
		}

		[DllImport (Constants.CoreVideoLibrary)]
		extern static /* size_t */ nint CVPixelBufferGetBytesPerRow (
			/* CVPixelBufferRef __nonnull */ IntPtr pixelBuffer);

		public nint BytesPerRow {
			get {
				return CVPixelBufferGetBytesPerRow (handle);
			}
		}

		[DllImport (Constants.CoreVideoLibrary)]
		extern static /* size_t */ nint CVPixelBufferGetDataSize (
			/* CVPixelBufferRef __nonnull */ IntPtr pixelBuffer);

		public nint DataSize {
			get {
				return CVPixelBufferGetDataSize (handle);
			}
		}

		[DllImport (Constants.CoreVideoLibrary)]
		extern static /* size_t */ nint CVPixelBufferGetHeight (/* CVPixelBufferRef __nonnull */ IntPtr pixelBuffer);

		public nint Height {
			get {
				return CVPixelBufferGetHeight (handle);
			}
		}

		[DllImport (Constants.CoreVideoLibrary)]
		extern static /* size_t */ nint CVPixelBufferGetWidth (/* CVPixelBufferRef __nonnull */ IntPtr pixelBuffer);

		public nint Width {
			get {
				return CVPixelBufferGetWidth (handle);
			}
		}

		[DllImport (Constants.CoreVideoLibrary)]
		extern static /* size_t */ nint CVPixelBufferGetPlaneCount (
			/* CVPixelBufferRef __nonnull */ IntPtr pixelBuffer);

		public nint PlaneCount {
			get {
				return CVPixelBufferGetPlaneCount (handle);
			}
		}

		[DllImport (Constants.CoreVideoLibrary)]
		extern static /* Boolean */ bool CVPixelBufferIsPlanar (/* CVPixelBufferRef __nonnull */ IntPtr pixelBuffer);

		public bool IsPlanar {
			get {
				return CVPixelBufferIsPlanar (handle);
			}
		}

		[DllImport (Constants.CoreVideoLibrary)]
		extern static CVPixelFormatType CVPixelBufferGetPixelFormatType (
			/* CVPixelBufferRef __nonnull */ IntPtr pixelBuffer);

		public CVPixelFormatType PixelFormatType {
			get {
				return CVPixelBufferGetPixelFormatType (handle);
			}
		}

		[DllImport (Constants.CoreVideoLibrary)]
		extern static /* void * __nullable */ IntPtr CVPixelBufferGetBaseAddressOfPlane (
			/* CVPixelBufferRef __nonnull */ IntPtr pixelBuffer, /* size_t */ nint planeIndex);

		public IntPtr GetBaseAddress (nint planeIndex)
		{
			return CVPixelBufferGetBaseAddressOfPlane (handle, planeIndex);
		}

		[DllImport (Constants.CoreVideoLibrary)]
		extern static /* size_t */ nint CVPixelBufferGetBytesPerRowOfPlane (
			/* CVPixelBufferRef __nonnull */ IntPtr pixelBuffer, /* size_t */ nint planeIndex);

		public nint GetBytesPerRowOfPlane (nint planeIndex)
		{
			return CVPixelBufferGetBytesPerRowOfPlane (handle, planeIndex);
		}

		[DllImport (Constants.CoreVideoLibrary)]
		extern static /* size_t */ nint CVPixelBufferGetHeightOfPlane (
			/* CVPixelBufferRef __nonnull */ IntPtr pixelBuffer, /* size_t */ nint planeIndex);

		public nint GetHeightOfPlane (nint planeIndex)
		{
			return CVPixelBufferGetHeightOfPlane (handle, planeIndex);
		}

		[DllImport (Constants.CoreVideoLibrary)]
		extern static /* size_t */ nint CVPixelBufferGetWidthOfPlane (
			/* CVPixelBufferRef __nonnull */ IntPtr pixelBuffer, /* size_t */ nint planeIndex);

		public nint GetWidthOfPlane (nint planeIndex)
		{
			return CVPixelBufferGetWidthOfPlane (handle, planeIndex);
		}

		[DllImport (Constants.CoreVideoLibrary)]
		extern static CVReturn CVPixelBufferLockBaseAddress (
			/* CVPixelBufferRef __nonnull */ IntPtr pixelBuffer, CVPixelBufferLock lockFlags);

#if !XAMCORE_3_0
		[Obsolete ("Use 'Lock (CVPixelBufferLock)' instead.")]
		public CVReturn Lock (CVOptionFlags lockFlags)
		{
			return CVPixelBufferLockBaseAddress (handle, (CVPixelBufferLock) lockFlags);
		}
#endif

		public CVReturn Lock (CVPixelBufferLock lockFlags)
		{
			return CVPixelBufferLockBaseAddress (handle, lockFlags);
		}

		[DllImport (Constants.CoreVideoLibrary)]
		extern static CVReturn CVPixelBufferUnlockBaseAddress (
			/* CVPixelBufferRef __nonnull */ IntPtr pixelBuffer, CVPixelBufferLock unlockFlags);

#if !XAMCORE_3_0
		[Obsolete ("Use 'Unlock (CVPixelBufferLock)'.")]
		public CVReturn Unlock (CVOptionFlags unlockFlags)
		{
			return CVPixelBufferUnlockBaseAddress (handle, (CVPixelBufferLock) unlockFlags);
		}
#endif

		public CVReturn Unlock (CVPixelBufferLock unlockFlags)
		{
			return CVPixelBufferUnlockBaseAddress (handle, unlockFlags);
		}
#endif // !COREBUILD
	}
}
