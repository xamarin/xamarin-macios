// 
// CVPixelBuffer.cs: Implements the managed CVPixelBuffer
//
// Authors: Mono Team
//     
// Copyright 2010 Novell, Inc
// Copyright 2011-2014 Xamarin Inc
//
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CoreFoundation;
using ObjCRuntime;
using Foundation;

using CFDictionaryRef = System.IntPtr;
using CVPixelBufferRef = System.IntPtr;

#nullable enable

namespace CoreVideo {

	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public partial class CVPixelBuffer : CVImageBuffer {
#if !COREBUILD
		[DllImport (Constants.CoreVideoLibrary, EntryPoint = "CVPixelBufferGetTypeID")]
		public extern static /* CFTypeID */ nint GetTypeID ();

		[Preserve (Conditional = true)]
		internal CVPixelBuffer (NativeHandle handle, bool owns) : base (handle, owns)
		{
		}

#if !COREBUILD
		[DllImport (Constants.CoreVideoLibrary)]
		unsafe extern static CVReturn CVPixelBufferCreate (
			/* CFAllocatorRef __nullable */ IntPtr allocator, /* size_t */ nint width, /* size_t */ nint height,
			/* OSType */ CVPixelFormatType pixelFormatType,
			/* CFDictionaryRef __nullable */ IntPtr pixelBufferAttributes,
			/* CVPixelBufferRef __nullable * __nonnull */ IntPtr* pixelBufferOut);

		public CVPixelBuffer (nint width, nint height, CVPixelFormatType pixelFormat)
			: this (width, height, pixelFormat, (NSDictionary?) null)
		{
		}

		public CVPixelBuffer (nint width, nint height, CVPixelFormatType pixelFormatType, CVPixelBufferAttributes? attributes)
			: this (width, height, pixelFormatType, attributes?.Dictionary)
		{
		}

		internal static CVPixelBuffer? Create (NativeHandle handle, bool owns)
		{
			if (handle == IntPtr.Zero)
				return null;
			return new CVPixelBuffer (handle, owns);
		}

		static IntPtr Create (nint width, nint height, CVPixelFormatType pixelFormatType, NSDictionary? pixelBufferAttributes)
		{
			if (width <= 0)
				throw new ArgumentOutOfRangeException (nameof (width));

			if (height <= 0)
				throw new ArgumentOutOfRangeException (nameof (height));

			CVReturn ret;
			IntPtr handle;
			unsafe {
				ret = CVPixelBufferCreate (IntPtr.Zero, width, height, pixelFormatType, pixelBufferAttributes.GetHandle (), &handle);
			}

			if (ret != CVReturn.Success)
				throw new ArgumentException (ret.ToString ());

			return handle;
		}

		[Advice ("Use constructor with CVPixelBufferAttributes")]
		CVPixelBuffer (nint width, nint height, CVPixelFormatType pixelFormatType, NSDictionary? pixelBufferAttributes)
			: base (Create (width, height, pixelFormatType, pixelBufferAttributes), true)
		{
		}

		[DllImport (Constants.CoreVideoLibrary)]
		unsafe extern static CVReturn CVPixelBufferCreateResolvedAttributesDictionary (
			/* CFAllocatorRef __nullable */ IntPtr allocator,
			/* CFArrayRef __nullable */ IntPtr attributes,
			/* CFDictionaryRef __nullable * __nonnull */ IntPtr* resolvedDictionaryOut);

		public NSDictionary? GetAttributes (NSDictionary []? attributes)
		{
			CVReturn ret;
			IntPtr resolvedDictionaryOut;
			var nsarray = NSArray.FromNSObjects (attributes);
			unsafe {
				ret = CVPixelBufferCreateResolvedAttributesDictionary (IntPtr.Zero, nsarray.GetHandle (), &resolvedDictionaryOut);
			}
			GC.KeepAlive (nsarray);
			if (ret != CVReturn.Success)
				throw new ArgumentException (ret.ToString ());
			return Runtime.GetNSObject<NSDictionary> (resolvedDictionaryOut);
		}

		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[DllImport (Constants.CoreVideoLibrary)]
		static extern CFDictionaryRef CVPixelBufferCopyCreationAttributes (CVPixelBufferRef pixelBuffer);

		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("maccatalyst")]
		public CVPixelBufferAttributes? GetPixelBufferCreationAttributes ()
		{
			var attrs = Runtime.GetNSObject<NSDictionary> (CVPixelBufferCopyCreationAttributes (Handle), true);
			if (attrs is null)
				return null;
			return new CVPixelBufferAttributes (attrs);
		}
#endif

		/* CVPixelBufferCreateWithBytes */

		delegate void CVPixelBufferReleaseBytesCallback (
			/* void * CV_NULLABLE */ IntPtr releaseRefCon,
			/* const void * CV_NULLABLE */ IntPtr baseAddress);

		[UnmanagedCallersOnly]
		static void ReleaseBytesCallback (IntPtr releaseRefCon, IntPtr baseAddress)
		{
			GCHandle handle = GCHandle.FromIntPtr (releaseRefCon);
			handle.Free ();
		}

		[DllImport (Constants.CoreVideoLibrary)]
		static unsafe extern CVReturn CVPixelBufferCreateWithBytes (
			/* CFAllocatorRef CV_NULLABLE */ IntPtr allocator,
			/* size_t */ nint width,
			/* size_t */ nint height,
			/* OSType */ CVPixelFormatType pixelFormatType,
			/* void * CV_NONNULL */ IntPtr baseAddress,
			/* size_t */ nint bytesPerRow,
			delegate* unmanaged<IntPtr, IntPtr, void> releaseCallback,
			/* void * CV_NULLABLE */ IntPtr releaseRefCon,
			/* CFDictionaryRef CV_NULLABLE */ IntPtr pixelBufferAttributes,
			/* CV_RETURNS_RETAINED_PARAMETER CVPixelBufferRef CV_NULLABLE * CV_NONNULL */ IntPtr* pixelBufferOut);// __OSX_AVAILABLE_STARTING(__MAC_10_4,__IPHONE_4_0);

		public static CVPixelBuffer? Create (nint width, nint height, CVPixelFormatType pixelFormatType, byte [] data, nint bytesPerRow, CVPixelBufferAttributes pixelBufferAttributes)
		{
			CVReturn status;
			return Create (width, height, pixelFormatType, data, bytesPerRow, pixelBufferAttributes, out status);
		}

		public static CVPixelBuffer? Create (nint width, nint height, CVPixelFormatType pixelFormatType, byte [] data, nint bytesPerRow, CVPixelBufferAttributes pixelBufferAttributes, out CVReturn status)
		{
			IntPtr handle;
			GCHandle gchandle;

			if (data is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (data));

			if (data.Length < height * bytesPerRow)
				throw new ArgumentOutOfRangeException (nameof (data), "The length of data is smaller than height * bytesPerRow");

			gchandle = GCHandle.Alloc (data, GCHandleType.Pinned); // This requires a pinned GCHandle, because unsafe code is scoped to the current block, and the address of the byte array will be used after this function returns.

			unsafe {
				status = CVPixelBufferCreateWithBytes (
					IntPtr.Zero,
					width, height,
					pixelFormatType,
					gchandle.AddrOfPinnedObject (),
					bytesPerRow,
					&ReleaseBytesCallback,
					GCHandle.ToIntPtr (gchandle),
					DictionaryContainerHelper.GetHandle (pixelBufferAttributes),
					&handle);
			}

			if (status != CVReturn.Success) {
				gchandle.Free ();
				return null;
			}

			return new CVPixelBuffer (handle, true);
		}

		/* CVPixelBufferCreateWithPlanarBytes */

		class PlaneData {
			public GCHandle [] dataHandles = Array.Empty<GCHandle> ();
		}

		delegate void CVPixelBufferReleasePlanarBytesCallback (
			/* void* */ IntPtr releaseRefCon,
			/* const void*/ IntPtr dataPtr,
			/* size_t */ nint dataSize,
			/* size_t */ nint numberOfPlanes,
			/* const void* */IntPtr planeAddresses);

		static void ReleasePlanarBytesCallbackImpl (IntPtr releaseRefCon, IntPtr dataPtr, nint dataSize, nint numberOfPlanes, IntPtr planeAddresses)
		{
			GCHandle handle = GCHandle.FromIntPtr (releaseRefCon);
			PlaneData data = (PlaneData) handle.Target!;
			for (int i = 0; i < data.dataHandles.Length; i++)
				data.dataHandles [i].Free ();
			handle.Free ();
		}

		[UnmanagedCallersOnly]
		static void ReleasePlanarBytesCallback (IntPtr releaseRefCon, IntPtr dataPtr, nint dataSize, nint numberOfPlanes, IntPtr planeAddresses)
		{
			ReleasePlanarBytesCallbackImpl (releaseRefCon, dataPtr, dataSize, numberOfPlanes, planeAddresses);
		}

		[DllImport (Constants.CoreVideoLibrary)]
		static unsafe extern CVReturn CVPixelBufferCreateWithPlanarBytes (
			/* CFAllocatorRef CV_NULLABLE */ IntPtr allocator,
			/* size_t */ nint width,
			/* size_t */ nint height,
			/* OSType */ CVPixelFormatType pixelFormatType,
			/* void * CV_NULLABLE */ IntPtr dataPtr, /* pass a pointer to a plane descriptor block, or NULL /*
			/* size_t */ nint dataSize, /* pass size if planes are contiguous, NULL if not */
			/* size_t */ nint numberOfPlanes,
			/* void *[] CV_NULLABLE */ IntPtr* planeBaseAddress,
			/* size_t[] */ nint* planeWidth,
			/* size_t[] */ nint* planeHeight,
			/* size_t[] */ nint* planeBytesPerRow,
			delegate* unmanaged<IntPtr, IntPtr, nint, nint, IntPtr, void>/* CV_NULLABLE */ releaseCallback,
			/* void * CV_NULLABLE */ IntPtr releaseRefCon,
			/* CFDictionaryRef CV_NULLABLE */ IntPtr pixelBufferAttributes,
			/* CV_RETURNS_RETAINED_PARAMETER CVPixelBufferRef CV_NULLABLE * CV_NONNULL */ IntPtr* pixelBufferOut); // __OSX_AVAILABLE_STARTING(__MAC_10_4,__IPHONE_4_0);

		public static CVPixelBuffer? Create (nint width, nint height, CVPixelFormatType pixelFormatType, byte [] [] planes, nint [] planeWidths, nint [] planeHeights, nint [] planeBytesPerRow, CVPixelBufferAttributes pixelBufferAttributes)
		{
			CVReturn status;
			return Create (width, height, pixelFormatType, planes, planeWidths, planeHeights, planeBytesPerRow, pixelBufferAttributes, out status);
		}

		public static CVPixelBuffer? Create (nint width, nint height, CVPixelFormatType pixelFormatType, byte [] [] planes, nint [] planeWidths, nint [] planeHeights, nint [] planeBytesPerRow, CVPixelBufferAttributes pixelBufferAttributes, out CVReturn status)
		{
			IntPtr handle;
			IntPtr [] addresses;
			PlaneData data;
			GCHandle data_handle;

			if (planes is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (planes));

			if (planeWidths is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (planeWidths));

			if (planeHeights is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (planeHeights));

			if (planeBytesPerRow is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (planeBytesPerRow));

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
				data.dataHandles [i] = GCHandle.Alloc (planes [i], GCHandleType.Pinned); // This can't use unsafe code because we need to get the pointer for an unbound number of objects.
				addresses [i] = data.dataHandles [i].AddrOfPinnedObject ();
			}
			data_handle = GCHandle.Alloc (data);

			IntPtr data_handle_ptr = GCHandle.ToIntPtr (data_handle);
			unsafe {
				fixed (IntPtr* addressesPtr = addresses) {
					fixed (nint* planeWidthsPtr = planeWidths) {
						fixed (nint* planeHeightsPtr = planeHeights) {
							fixed (nint* planeBytesPerRowPtr = planeBytesPerRow) {
								status = CVPixelBufferCreateWithPlanarBytes (
											IntPtr.Zero,
											width, height, pixelFormatType, IntPtr.Zero, 0,
											planeCount,
											addressesPtr,
											planeWidthsPtr,
											planeHeightsPtr,
											planeBytesPerRowPtr,
											&ReleasePlanarBytesCallback,
											data_handle_ptr,
											DictionaryContainerHelper.GetHandle (pixelBufferAttributes),
											&handle);
							}
						}
					}
				}
			}

			if (status != CVReturn.Success) {
				ReleasePlanarBytesCallbackImpl (data_handle_ptr, IntPtr.Zero, 0, 0, IntPtr.Zero);
				return null;
			}

			return new CVPixelBuffer (handle, true);
		}

		[DllImport (Constants.CoreVideoLibrary)]
		unsafe static extern void CVPixelBufferGetExtendedPixels (/* CVPixelBufferRef __nonnull */ IntPtr pixelBuffer,
			/* size_t* */ nuint* extraColumnsOnLeft, /* size_t* */ nuint* extraColumnsOnRight,
			/* size_t* */ nuint* extraRowsOnTop, /* size_t* */ nuint* extraRowsOnBottom);

		public void GetExtendedPixels (ref nuint extraColumnsOnLeft, ref nuint extraColumnsOnRight,
			ref nuint extraRowsOnTop, ref nuint extraRowsOnBottom)
		{
			unsafe {
				CVPixelBufferGetExtendedPixels (Handle,
					(nuint*) Unsafe.AsPointer<nuint> (ref extraColumnsOnLeft),
					(nuint*) Unsafe.AsPointer<nuint> (ref extraColumnsOnRight),
					(nuint*) Unsafe.AsPointer<nuint> (ref extraRowsOnTop),
					(nuint*) Unsafe.AsPointer<nuint> (ref extraRowsOnBottom));
			}
		}

		[DllImport (Constants.CoreVideoLibrary)]
		extern static CVReturn CVPixelBufferFillExtendedPixels (/* CVPixelBufferRef __nonnull */ IntPtr pixelBuffer);

		public CVReturn FillExtendedPixels ()
		{
			return CVPixelBufferFillExtendedPixels (Handle);
		}

		[DllImport (Constants.CoreVideoLibrary)]
		extern static /* void* __nullable */ IntPtr CVPixelBufferGetBaseAddress (
			/* CVPixelBufferRef __nonnull */ IntPtr pixelBuffer);

		public IntPtr BaseAddress {
			get {
				return CVPixelBufferGetBaseAddress (Handle);
			}
		}

		[DllImport (Constants.CoreVideoLibrary)]
		extern static /* size_t */ nint CVPixelBufferGetBytesPerRow (
			/* CVPixelBufferRef __nonnull */ IntPtr pixelBuffer);

		public nint BytesPerRow {
			get {
				return CVPixelBufferGetBytesPerRow (Handle);
			}
		}

		[DllImport (Constants.CoreVideoLibrary)]
		extern static /* size_t */ nint CVPixelBufferGetDataSize (
			/* CVPixelBufferRef __nonnull */ IntPtr pixelBuffer);

		public nint DataSize {
			get {
				return CVPixelBufferGetDataSize (Handle);
			}
		}

		[DllImport (Constants.CoreVideoLibrary)]
		extern static /* size_t */ nint CVPixelBufferGetHeight (/* CVPixelBufferRef __nonnull */ IntPtr pixelBuffer);

		public nint Height {
			get {
				return CVPixelBufferGetHeight (Handle);
			}
		}

		[DllImport (Constants.CoreVideoLibrary)]
		extern static /* size_t */ nint CVPixelBufferGetWidth (/* CVPixelBufferRef __nonnull */ IntPtr pixelBuffer);

		public nint Width {
			get {
				return CVPixelBufferGetWidth (Handle);
			}
		}

		[DllImport (Constants.CoreVideoLibrary)]
		extern static /* size_t */ nint CVPixelBufferGetPlaneCount (
			/* CVPixelBufferRef __nonnull */ IntPtr pixelBuffer);

		public nint PlaneCount {
			get {
				return CVPixelBufferGetPlaneCount (Handle);
			}
		}

		[DllImport (Constants.CoreVideoLibrary)]
		extern static /* Boolean */ byte CVPixelBufferIsPlanar (/* CVPixelBufferRef __nonnull */ IntPtr pixelBuffer);

		public bool IsPlanar {
			get {
				return CVPixelBufferIsPlanar (Handle) != 0;
			}
		}

		[DllImport (Constants.CoreVideoLibrary)]
		extern static CVPixelFormatType CVPixelBufferGetPixelFormatType (
			/* CVPixelBufferRef __nonnull */ IntPtr pixelBuffer);

		public CVPixelFormatType PixelFormatType {
			get {
				return CVPixelBufferGetPixelFormatType (Handle);
			}
		}

		[DllImport (Constants.CoreVideoLibrary)]
		extern static /* void * __nullable */ IntPtr CVPixelBufferGetBaseAddressOfPlane (
			/* CVPixelBufferRef __nonnull */ IntPtr pixelBuffer, /* size_t */ nint planeIndex);

		public IntPtr GetBaseAddress (nint planeIndex)
		{
			return CVPixelBufferGetBaseAddressOfPlane (Handle, planeIndex);
		}

		[DllImport (Constants.CoreVideoLibrary)]
		extern static /* size_t */ nint CVPixelBufferGetBytesPerRowOfPlane (
			/* CVPixelBufferRef __nonnull */ IntPtr pixelBuffer, /* size_t */ nint planeIndex);

		public nint GetBytesPerRowOfPlane (nint planeIndex)
		{
			return CVPixelBufferGetBytesPerRowOfPlane (Handle, planeIndex);
		}

		[DllImport (Constants.CoreVideoLibrary)]
		extern static /* size_t */ nint CVPixelBufferGetHeightOfPlane (
			/* CVPixelBufferRef __nonnull */ IntPtr pixelBuffer, /* size_t */ nint planeIndex);

		public nint GetHeightOfPlane (nint planeIndex)
		{
			return CVPixelBufferGetHeightOfPlane (Handle, planeIndex);
		}

		[DllImport (Constants.CoreVideoLibrary)]
		extern static /* size_t */ nint CVPixelBufferGetWidthOfPlane (
			/* CVPixelBufferRef __nonnull */ IntPtr pixelBuffer, /* size_t */ nint planeIndex);

		public nint GetWidthOfPlane (nint planeIndex)
		{
			return CVPixelBufferGetWidthOfPlane (Handle, planeIndex);
		}

		[DllImport (Constants.CoreVideoLibrary)]
		extern static CVReturn CVPixelBufferLockBaseAddress (
			/* CVPixelBufferRef __nonnull */ IntPtr pixelBuffer, CVPixelBufferLock lockFlags);

		public CVReturn Lock (CVPixelBufferLock lockFlags)
		{
			return CVPixelBufferLockBaseAddress (Handle, lockFlags);
		}

		[DllImport (Constants.CoreVideoLibrary)]
		extern static CVReturn CVPixelBufferUnlockBaseAddress (
			/* CVPixelBufferRef __nonnull */ IntPtr pixelBuffer, CVPixelBufferLock unlockFlags);

		public CVReturn Unlock (CVPixelBufferLock unlockFlags)
		{
			return CVPixelBufferUnlockBaseAddress (Handle, unlockFlags);
		}
#endif // !COREBUILD
	}
}
