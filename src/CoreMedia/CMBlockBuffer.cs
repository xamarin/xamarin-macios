// 
// CMBlockBuffer.cs: Implements the managed CMBlockBuffer
//
// Authors: Mono Team
//          Marek Safar	(marek.safar@gmail.com)
//          Alex Soto		(alex.soto@xamarin.com)
//     
// Copyright 2010 Novell, Inc
// Copyright 2012-2015 Xamarin Inc
//

#nullable enable

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using Foundation;
using CoreFoundation;
using ObjCRuntime;

namespace CoreMedia {

	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	public class CMBlockBuffer : NativeObject, ICMAttachmentBearer {
		CMCustomBlockAllocator? customAllocator;

		[Preserve (Conditional = true)]
		internal CMBlockBuffer (NativeHandle handle, bool owns)
			: base (handle, owns)
		{
		}

		[DllImport (Constants.CoreMediaLibrary)]
		unsafe extern static /* OSStatus */ CMBlockBufferError CMBlockBufferCreateEmpty (/* CFAllocatorRef */ IntPtr allocator, /* uint32_t */ uint subBlockCapacity, CMBlockBufferFlags flags, /* CMBlockBufferRef* */ IntPtr* output);

		public static CMBlockBuffer? CreateEmpty (uint subBlockCapacity, CMBlockBufferFlags flags, out CMBlockBufferError error)
		{
			IntPtr buffer;
			unsafe {
				error = CMBlockBufferCreateEmpty (IntPtr.Zero, subBlockCapacity, flags, &buffer);
			}
			if (error != CMBlockBufferError.None)
				return null;

			return new CMBlockBuffer (buffer, true);
		}

		[DllImport (Constants.CoreMediaLibrary)]
		unsafe extern static /* OSStatus */ CMBlockBufferError CMBlockBufferCreateWithBufferReference (
			/* CFAllocatorRef */ IntPtr structureAllocator,
			/* CMBlockBufferRef */ IntPtr targetBuffer,
			/* size_t */ nuint offsetToData,
			/* size_t */ nuint dataLength,
			CMBlockBufferFlags flags,
			/* CMBlockBufferRef* */ IntPtr* newBlockBuffer);

		public static CMBlockBuffer? FromBuffer (CMBlockBuffer? targetBuffer, nuint offsetToData, nuint dataLength, CMBlockBufferFlags flags, out CMBlockBufferError error)
		{
			// From docs targetBuffer must not be null unless the PermitEmptyReference flag is set
			if (!flags.HasFlag (CMBlockBufferFlags.PermitEmptyReference))
				if (targetBuffer is null)
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (targetBuffer));

			IntPtr buffer;
			unsafe {
				error = CMBlockBufferCreateWithBufferReference (IntPtr.Zero, targetBuffer.GetHandle (), offsetToData, dataLength, flags, &buffer);
			}
			if (error != CMBlockBufferError.None)
				return null;

			return new CMBlockBuffer (buffer, true);
		}

		[DllImport (Constants.CoreMediaLibrary)]
		extern static /* OSStatus */ CMBlockBufferError CMBlockBufferAppendBufferReference (
			/* CMBlockBufferRef */ IntPtr buffer,
			/* CMBlockBufferRef */ IntPtr targetBBuf,
			/* size_t */ nuint offsetToData,
			/* size_t */ nuint dataLength,
			CMBlockBufferFlags flags);

		public CMBlockBufferError AppendBuffer (CMBlockBuffer? targetBuffer, nuint offsetToData, nuint dataLength, CMBlockBufferFlags flags)
		{
			// From docs targetBuffer must not be null unless the PermitEmptyReference flag is set
			if (!flags.HasFlag (CMBlockBufferFlags.PermitEmptyReference)) {
				if (targetBuffer is null)
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (targetBuffer));
			}

			return CMBlockBufferAppendBufferReference (GetCheckedHandle (), targetBuffer.GetHandle (), offsetToData, dataLength, flags);
		}

		[DllImport (Constants.CoreMediaLibrary)]
		extern static /* OSStatus */ CMBlockBufferError CMBlockBufferAssureBlockMemory (/* CMBlockBufferRef */ IntPtr buffer);

		public CMBlockBufferError AssureBlockMemory ()
		{
			return CMBlockBufferAssureBlockMemory (GetCheckedHandle ());
		}

		[DllImport (Constants.CoreMediaLibrary)]
		unsafe extern static /* OSStatus */ CMBlockBufferError CMBlockBufferAccessDataBytes (
			/* CMBlockBufferRef */ IntPtr buffer,
			/* size_t */ nuint offset,
			/* size_t */ nuint length,
			/* void* */ IntPtr temporaryBlock,
			/* char ** */ IntPtr* returnedPointer);

		//FIXME: can we expose better API here?
		public CMBlockBufferError AccessDataBytes (nuint offset, nuint length, IntPtr temporaryBlock, ref IntPtr returnedPointer)
		{
			unsafe {
				return CMBlockBufferAccessDataBytes (GetCheckedHandle (), offset, length, temporaryBlock, (IntPtr*) Unsafe.AsPointer<IntPtr> (ref returnedPointer));
			}
		}

		[DllImport (Constants.CoreMediaLibrary)]
		extern static /* OSStatus */ CMBlockBufferError CMBlockBufferCopyDataBytes (
			/* CMBlockBufferRef */ IntPtr theSourceBuffer,
			/* size_t */ nuint offsetToData,
			/* size_t */ nuint dataLength,
			/* void* */ IntPtr destination);

		public CMBlockBufferError CopyDataBytes (nuint offsetToData, nuint dataLength, IntPtr destination)
		{
			return CMBlockBufferCopyDataBytes (GetCheckedHandle (), offsetToData, dataLength, destination);
		}

		public unsafe CMBlockBufferError CopyDataBytes (nuint offsetToData, nuint dataLength, out byte []? destination)
		{
			destination = new byte [dataLength];

			CMBlockBufferError error;
			fixed (byte* ptr = destination)
				error = CMBlockBufferCopyDataBytes (GetCheckedHandle (), offsetToData, dataLength, (IntPtr) ptr);
			if (error != CMBlockBufferError.None)
				destination = default (byte []);
			return error;
		}

		[DllImport (Constants.CoreMediaLibrary)]
		extern static /* OSStatus */ CMBlockBufferError CMBlockBufferReplaceDataBytes (
			/* void* */ IntPtr sourceBytes,
			/* CMBlockBufferRef */ IntPtr destinationBuffer,
			/* size_t */ nuint offsetIntoDestination,
			/* size_t */ nuint dataLength);

		public CMBlockBufferError ReplaceDataBytes (IntPtr sourceBytes, nuint offsetIntoDestination, nuint dataLength)
		{
			return CMBlockBufferReplaceDataBytes (sourceBytes, GetCheckedHandle (), offsetIntoDestination, dataLength);
		}

		public unsafe CMBlockBufferError ReplaceDataBytes (byte [] sourceBytes, nuint offsetIntoDestination)
		{
			if (sourceBytes is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (sourceBytes));

			fixed (byte* ptr = sourceBytes)
				return ReplaceDataBytes ((IntPtr) ptr, offsetIntoDestination, (nuint) sourceBytes.Length);
		}

		[DllImport (Constants.CoreMediaLibrary)]
		extern static /* OSStatus */ CMBlockBufferError CMBlockBufferFillDataBytes (
			/* char */ byte fillByte,
			/* CMBlockBufferRef */ IntPtr destinationBuffer,
			/* size_t */ nuint offsetIntoDestination,
			/* size_t */ nuint dataLength);

		public CMBlockBufferError FillDataBytes (byte fillByte, nuint offsetIntoDestination, nuint dataLength)
		{
			return CMBlockBufferFillDataBytes (fillByte, GetCheckedHandle (), offsetIntoDestination, dataLength);
		}

		[DllImport (Constants.CoreMediaLibrary)]
		unsafe extern static /* OSStatus */ CMBlockBufferError CMBlockBufferGetDataPointer (
			/* CMBlockBufferRef */ IntPtr buffer,
			/* size_t */ nuint offset,
			/* size_t* */ nuint* lengthAtOffset,
			/* size_t* */ nuint* totalLength,
			/* char ** */ IntPtr* dataPointer);

		public CMBlockBufferError GetDataPointer (nuint offset, out nuint lengthAtOffset, out nuint totalLength, ref IntPtr dataPointer)
		{
			lengthAtOffset = default;
			totalLength = default;
			unsafe {
				return CMBlockBufferGetDataPointer (GetCheckedHandle (),
													offset,
													(nuint*) Unsafe.AsPointer<nuint> (ref lengthAtOffset),
													(nuint*) Unsafe.AsPointer<nuint> (ref totalLength),
													(IntPtr*) Unsafe.AsPointer<IntPtr> (ref dataPointer));
			}
		}

		[DllImport (Constants.CoreMediaLibrary)]
		extern static /* size_t */ nuint CMBlockBufferGetDataLength (/* CMBlockBufferRef */ IntPtr theBuffer);

		public nuint DataLength {
			get {
				return CMBlockBufferGetDataLength (Handle);
			}
		}

		[DllImport (Constants.CoreMediaLibrary)]
		extern static /* Boolean */ byte CMBlockBufferIsRangeContiguous (
			/* CMBlockBufferRef */ IntPtr buffer,
			/* size_t */ nuint offset,
			/* size_t */ nuint length);

		public bool IsRangeContiguous (nuint offset, nuint length)
		{
			return CMBlockBufferIsRangeContiguous (GetCheckedHandle (), offset, length) != 0;
		}

		[DllImport (Constants.CoreMediaLibrary)]
		extern static /* Boolean */ byte CMBlockBufferIsEmpty (/* CMBlockBufferRef */ IntPtr theBuffer);

		public bool IsEmpty {
			get {
				return CMBlockBufferIsEmpty (GetCheckedHandle ()) != 0;
			}
		}

		[DllImport (Constants.CoreMediaLibrary)]
		unsafe extern static /* OSStatus */ CMBlockBufferError CMBlockBufferCreateWithMemoryBlock (
			/* CFAllocatorRef */ IntPtr structureAllocator,
			/* void * */ IntPtr memoryBlock,
			/* size_t */ nuint blockLength,
			/* CFAllocatorRef */ IntPtr blockAllocator,
			/* CMBlockBufferCustomBlockSource* */ CMCustomBlockAllocator.CMBlockBufferCustomBlockSource* customBlockSource,
			/* size_t */ nuint offsetToData,
			/* size_t */ nuint dataLength,
			CMBlockBufferFlags flags,
			/* CMBlockBufferRef* */ IntPtr* newBlockBuffer);

		public static CMBlockBuffer? FromMemoryBlock (IntPtr memoryBlock, nuint blockLength, CMCustomBlockAllocator? customBlockSource, nuint offsetToData, nuint dataLength, CMBlockBufferFlags flags, out CMBlockBufferError error)
		{
			var blockAllocator = memoryBlock == IntPtr.Zero ? NativeHandle.Zero : CFAllocator.Null.Handle;
			IntPtr buffer;
			unsafe {
				if (customBlockSource is null) {
					error = CMBlockBufferCreateWithMemoryBlock (IntPtr.Zero, memoryBlock, blockLength, blockAllocator, null, offsetToData, dataLength, flags, &buffer);
				} else {
					fixed (CMCustomBlockAllocator.CMBlockBufferCustomBlockSource* cblock = &customBlockSource.Cblock) {
						error = CMBlockBufferCreateWithMemoryBlock (IntPtr.Zero, memoryBlock, blockLength, blockAllocator, cblock, offsetToData, dataLength, flags, &buffer);
					}
				}
			}

			if (error != CMBlockBufferError.None)
				return null;

			var block = new CMBlockBuffer (buffer, true);
			block.customAllocator = customBlockSource;
			return block;
		}

		public static CMBlockBuffer? FromMemoryBlock (byte [] data, nuint offsetToData, CMBlockBufferFlags flags, out CMBlockBufferError error)
		{
			if (data is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (data));

			var allocator = new CMManagedArrayBlockAllocator (data);
			return FromMemoryBlock (IntPtr.Zero, (uint) data.Length, allocator, offsetToData, (uint) data.Length, flags, out error);
		}

		[DllImport (Constants.CoreMediaLibrary)]
		unsafe extern static /* OSStatus */ CMBlockBufferError CMBlockBufferCreateContiguous (
			/* CFAllocatorRef */ IntPtr structureAllocator,
			/* CMBlockBufferRef */ IntPtr sourceBuffer,
			/* CFAllocatorRef */ IntPtr blockAllocator,
			/* CMBlockBufferCustomBlockSource* */ CMCustomBlockAllocator.CMBlockBufferCustomBlockSource* customBlockSource,
			/* size_t */ nuint offsetToData,
			/* size_t */ nuint dataLength,
			CMBlockBufferFlags flags,
			/* CMBlockBufferRef* */ IntPtr* newBlockBuffer);

		public static CMBlockBuffer? CreateContiguous (CMBlockBuffer sourceBuffer, CMCustomBlockAllocator? customBlockSource, nuint offsetToData, nuint dataLength, CMBlockBufferFlags flags, out CMBlockBufferError error)
		{
			if (sourceBuffer is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (sourceBuffer));

			IntPtr buffer;
			unsafe {
				if (customBlockSource is null) {
					error = CMBlockBufferCreateContiguous (IntPtr.Zero, sourceBuffer.Handle, IntPtr.Zero, null, offsetToData, dataLength, flags, &buffer);
				} else {
					fixed (CMCustomBlockAllocator.CMBlockBufferCustomBlockSource* cblock = &customBlockSource.Cblock) {
						error = CMBlockBufferCreateContiguous (IntPtr.Zero, sourceBuffer.Handle, IntPtr.Zero, cblock, offsetToData, dataLength, flags, &buffer);
					}
				}
			}

			if (error != CMBlockBufferError.None)
				return null;

			var block = new CMBlockBuffer (buffer, true);
			block.customAllocator = customBlockSource;
			return block;
		}

		[DllImport (Constants.CoreMediaLibrary)]
		unsafe extern static /* OSStatus */ CMBlockBufferError CMBlockBufferAppendMemoryBlock (
			/* CMBlockBufferRef */ IntPtr buffer,
			/* void * */IntPtr memoryBlock,
			/* size_t */nuint blockLength,
			/* CFAllocatorRef */IntPtr blockAllocator,
			/* CMBlockBufferCustomBlockSource */ CMCustomBlockAllocator.CMBlockBufferCustomBlockSource* customBlockSource,
			/* size_t */nuint offsetToData,
			/* size_t */nuint dataLength,
			CMBlockBufferFlags flags);

		public CMBlockBufferError AppendMemoryBlock (IntPtr memoryBlock, nuint blockLength, CMCustomBlockAllocator customBlockSource, nuint offsetToData, nuint dataLength, CMBlockBufferFlags flags)
		{
			var blockAllocator = memoryBlock == IntPtr.Zero ? NativeHandle.Zero : CFAllocator.Null.Handle;
			unsafe {
				if (customBlockSource is null) {
					return CMBlockBufferAppendMemoryBlock (GetCheckedHandle (), memoryBlock, blockLength, blockAllocator, null, offsetToData, dataLength, flags);
				} else {
					fixed (CMCustomBlockAllocator.CMBlockBufferCustomBlockSource* cblock = &customBlockSource.Cblock) {
						return CMBlockBufferAppendMemoryBlock (GetCheckedHandle (), memoryBlock, blockLength, blockAllocator, cblock, offsetToData, dataLength, flags);
					}
				}
			}
		}

		public CMBlockBufferError AppendMemoryBlock (byte [] data, nuint offsetToData, CMBlockBufferFlags flags)
		{
			if (data is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (data));

			var allocator = new CMManagedArrayBlockAllocator (data);
			return AppendMemoryBlock (IntPtr.Zero, (uint) data.Length, allocator, offsetToData, (uint) data.Length, flags);
		}
	}
}
