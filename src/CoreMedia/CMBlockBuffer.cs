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
using System;
using System.Runtime.InteropServices;

using Foundation;
using CoreFoundation;
using ObjCRuntime;

namespace CoreMedia {

	// untyped enum (used as OSStatus) -> CMBlockBuffer.h
	public enum CMBlockBufferError : int {
		None						= 0,
		StructureAllocationFailed	= -12700,
		BlockAllocationFailed		= -12701,
		BadCustomBlockSource		= -12702,
		BadOffsetParameter			= -12703,
		BadLengthParameter			= -12704,
		BadPointerParameter			= -12705,
		EmptyBlockBuffer			= -12706,
		UnallocatedBlock			= -12707,
		InsufficientSpace			= -12708,
	}

	// uint32_t -> CMBlockBuffer.h
	[Flags]
	public enum CMBlockBufferFlags : uint {
		AssureMemoryNow			= (1<<0),
		AlwaysCopyData			= (1<<1),
		DontOptimizeDepth		= (1<<2),
		PermitEmptyReference	= (1<<3)
	}

	[Mac (10,7)]
	public class CMBlockBuffer : ICMAttachmentBearer, IDisposable {
		internal IntPtr handle;
		internal CMCustomBlockAllocator customAllocator;

		internal CMBlockBuffer (IntPtr handle)
		{
			this.handle = handle;
		}

		[Preserve (Conditional=true)]
		internal CMBlockBuffer (IntPtr handle, bool owns)
		{
			if (!owns)
				CFObject.CFRetain (handle);

			this.handle = handle;
		}
		
		~CMBlockBuffer ()
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
		extern static /* OSStatus */ CMBlockBufferError CMBlockBufferCreateEmpty (/* CFAllocatorRef */ IntPtr allocator, /* uint32_t */ uint subBlockCapacity, CMBlockBufferFlags flags, /* CMBlockBufferRef* */ out IntPtr output);

		public static CMBlockBuffer CreateEmpty (uint subBlockCapacity, CMBlockBufferFlags flags, out CMBlockBufferError error)
		{
			IntPtr buffer;
			error = CMBlockBufferCreateEmpty (IntPtr.Zero, subBlockCapacity, flags, out buffer);
			if (error != CMBlockBufferError.None)
				return null;

			return new CMBlockBuffer (buffer, true);
		}

		[DllImport(Constants.CoreMediaLibrary)]
		extern static /* OSStatus */ CMBlockBufferError CMBlockBufferCreateWithBufferReference (
			/* CFAllocatorRef */ IntPtr structureAllocator,
			/* CMBlockBufferRef */ IntPtr targetBuffer,
			/* size_t */ nuint offsetToData,
			/* size_t */ nuint dataLength,
			CMBlockBufferFlags flags,
			/* CMBlockBufferRef* */ out IntPtr newBlockBuffer);

		public static CMBlockBuffer FromBuffer (CMBlockBuffer targetBuffer, nuint offsetToData, nuint dataLength, CMBlockBufferFlags flags, out CMBlockBufferError error)
		{
			// From docs targetBuffer must not be null unless the PermitEmptyReference flag is set
			if (!flags.HasFlag (CMBlockBufferFlags.PermitEmptyReference))
				if (targetBuffer == null)
					throw new ArgumentNullException ("targetBuffer");
			
			IntPtr buffer;
			error = CMBlockBufferCreateWithBufferReference (IntPtr.Zero, targetBuffer == null ? IntPtr.Zero : targetBuffer.handle, offsetToData, dataLength, flags, out buffer);
			if (error != CMBlockBufferError.None)
				return null;

			return new CMBlockBuffer (buffer, true);
		}

		[DllImport(Constants.CoreMediaLibrary)]
		extern static /* OSStatus */ CMBlockBufferError CMBlockBufferAppendBufferReference (
			/* CMBlockBufferRef */ IntPtr buffer,
			/* CMBlockBufferRef */ IntPtr targetBBuf,
			/* size_t */ nuint offsetToData,
			/* size_t */ nuint dataLength,
			CMBlockBufferFlags flags);
		
		public CMBlockBufferError AppendBuffer (CMBlockBuffer targetBuffer, nuint offsetToData, nuint dataLength, CMBlockBufferFlags flags)
		{
			if (Handle == IntPtr.Zero)
				throw new ObjectDisposedException ("BlockBuffer");
			
			// From docs targetBuffer must not be null unless the PermitEmptyReference flag is set
			if (!flags.HasFlag (CMBlockBufferFlags.PermitEmptyReference)) {
				if (targetBuffer == null)
					throw new ArgumentNullException ("targetBuffer");
			}

			return CMBlockBufferAppendBufferReference (Handle, targetBuffer == null ? IntPtr.Zero : targetBuffer.handle, offsetToData, dataLength, flags);
		}

		[DllImport(Constants.CoreMediaLibrary)]
		extern static /* OSStatus */ CMBlockBufferError CMBlockBufferAssureBlockMemory (/* CMBlockBufferRef */ IntPtr buffer);

		public CMBlockBufferError AssureBlockMemory ()
		{
			if (Handle == IntPtr.Zero)
				throw new ObjectDisposedException ("BlockBuffer");

			return CMBlockBufferAssureBlockMemory (Handle);
		}

		[DllImport(Constants.CoreMediaLibrary)]
		extern static /* OSStatus */ CMBlockBufferError CMBlockBufferAccessDataBytes (
			/* CMBlockBufferRef */ IntPtr buffer,
			/* size_t */ nuint offset,
			/* size_t */ nuint length,
			/* void* */ IntPtr temporaryBlock,
			/* char ** */ ref IntPtr returnedPointer);

		//FIXME: can we expose better API here?
		public CMBlockBufferError AccessDataBytes (nuint offset, nuint length, IntPtr temporaryBlock, ref IntPtr returnedPointer)
		{
			if (Handle == IntPtr.Zero)
				throw new ObjectDisposedException ("BlockBuffer");

			return CMBlockBufferAccessDataBytes (Handle, offset, length, temporaryBlock, ref returnedPointer);
		}

		[DllImport(Constants.CoreMediaLibrary)]
		extern static /* OSStatus */ CMBlockBufferError CMBlockBufferCopyDataBytes (
			/* CMBlockBufferRef */ IntPtr theSourceBuffer,
			/* size_t */ nuint offsetToData,
			/* size_t */ nuint dataLength,
			/* void* */ IntPtr destination);
		
		public CMBlockBufferError CopyDataBytes (nuint offsetToData, nuint dataLength, IntPtr destination)
		{
			if (Handle == IntPtr.Zero)
				throw new ObjectDisposedException ("BlockBuffer");

			return CMBlockBufferCopyDataBytes (handle, offsetToData, dataLength, destination);
		}

		public unsafe CMBlockBufferError CopyDataBytes (nuint offsetToData, nuint dataLength, out byte [] destination)
		{
			if (Handle == IntPtr.Zero)
				throw new ObjectDisposedException ("BlockBuffer");

			destination = new byte [dataLength];

			CMBlockBufferError error;
			fixed (byte* ptr = destination)
				error = CMBlockBufferCopyDataBytes (handle, offsetToData, dataLength, (IntPtr) ptr);
			if (error != CMBlockBufferError.None)
				destination = default (byte []);
			return error;
		}

		[DllImport(Constants.CoreMediaLibrary)]
		extern static /* OSStatus */ CMBlockBufferError CMBlockBufferReplaceDataBytes (
			/* void* */ IntPtr sourceBytes,
			/* CMBlockBufferRef */ IntPtr destinationBuffer,
			/* size_t */ nuint offsetIntoDestination,
			/* size_t */ nuint dataLength);

		public CMBlockBufferError ReplaceDataBytes (IntPtr sourceBytes, nuint offsetIntoDestination, nuint dataLength)
		{
			if (Handle == IntPtr.Zero)
				throw new ObjectDisposedException ("BlockBuffer");
			
			return CMBlockBufferReplaceDataBytes (sourceBytes, handle, offsetIntoDestination, dataLength);
		}

		public unsafe CMBlockBufferError ReplaceDataBytes (byte [] sourceBytes, nuint offsetIntoDestination)
		{
			if (Handle == IntPtr.Zero)
				throw new ObjectDisposedException ("BlockBuffer");
			if (sourceBytes == null)
				throw new ArgumentNullException (nameof (sourceBytes));

			fixed (byte* ptr = sourceBytes)
				return ReplaceDataBytes ((IntPtr) ptr, offsetIntoDestination, (nuint) sourceBytes.Length);
		}

		[DllImport(Constants.CoreMediaLibrary)]
		extern static /* OSStatus */ CMBlockBufferError CMBlockBufferFillDataBytes (
			/* char */ byte fillByte,
			/* CMBlockBufferRef */ IntPtr destinationBuffer,
			/* size_t */ nuint offsetIntoDestination,
			/* size_t */ nuint dataLength);

		public CMBlockBufferError FillDataBytes (byte fillByte, nuint offsetIntoDestination, nuint dataLength)
		{
			if (Handle == IntPtr.Zero)
				throw new ObjectDisposedException ("BlockBuffer");
			
			return CMBlockBufferFillDataBytes (fillByte, handle, offsetIntoDestination, dataLength);
		}

		[DllImport(Constants.CoreMediaLibrary)]
		extern static /* OSStatus */ CMBlockBufferError CMBlockBufferGetDataPointer (
			/* CMBlockBufferRef */ IntPtr buffer,
			/* size_t */ nuint offset,
			/* size_t* */ out nuint lengthAtOffset,
			/* size_t* */ out nuint totalLength,
			/* char ** */ ref IntPtr dataPointer);

		public CMBlockBufferError GetDataPointer (nuint offset, out nuint lengthAtOffset, out nuint totalLength, ref IntPtr dataPointer)
		{
			if (Handle == IntPtr.Zero)
				throw new ObjectDisposedException ("BlockBuffer");
			
			return CMBlockBufferGetDataPointer (Handle, offset, out lengthAtOffset, out totalLength, ref dataPointer);
		}
		
		[DllImport(Constants.CoreMediaLibrary)]
		extern static /* size_t */ nuint CMBlockBufferGetDataLength (/* CMBlockBufferRef */ IntPtr theBuffer);
		
		public nuint DataLength
		{
			get
			{
				return CMBlockBufferGetDataLength (handle);
			}
		}

		[DllImport(Constants.CoreMediaLibrary)]
		[return: MarshalAs (UnmanagedType.U1)]
		extern static /* Boolean */ bool CMBlockBufferIsRangeContiguous (
			/* CMBlockBufferRef */ IntPtr buffer,
			/* size_t */ nuint offset,
			/* size_t */ nuint length);

		public bool IsRangeContiguous (nuint offset, nuint length)
		{
			if (Handle == IntPtr.Zero)
				throw new ObjectDisposedException ("BlockBuffer");
			
			return CMBlockBufferIsRangeContiguous (Handle, offset, length);
		}

		[DllImport(Constants.CoreMediaLibrary)]
		[return: MarshalAs (UnmanagedType.U1)]
		extern static /* Boolean */ bool CMBlockBufferIsEmpty (/* CMBlockBufferRef */ IntPtr theBuffer);

		public bool IsEmpty {
			get {
				if (Handle == IntPtr.Zero)
					throw new ObjectDisposedException ("BlockBuffer");
				
				return CMBlockBufferIsEmpty (handle);
			}
		}

		[DllImport(Constants.CoreMediaLibrary)]
		extern static /* OSStatus */ CMBlockBufferError CMBlockBufferCreateWithMemoryBlock (
			/* CFAllocatorRef */ IntPtr structureAllocator,
			/* void * */ IntPtr memoryBlock,
			/* size_t */ nuint blockLength,
			/* CFAllocatorRef */ IntPtr blockAllocator,
			/* CMBlockBufferCustomBlockSource* */ ref CMCustomBlockAllocator.CMBlockBufferCustomBlockSource customBlockSource,
			/* size_t */ nuint offsetToData,
			/* size_t */ nuint dataLength,
			CMBlockBufferFlags flags,
			/* CMBlockBufferRef* */ out IntPtr newBlockBuffer);

		[DllImport(Constants.CoreMediaLibrary)]
		extern static /* OSStatus */ CMBlockBufferError CMBlockBufferCreateWithMemoryBlock (
			/* CFAllocatorRef */ IntPtr structureAllocator,
			/* void * */ IntPtr memoryBlock,
			/* size_t */ nuint blockLength,
			/* CFAllocatorRef */ IntPtr blockAllocator,
			/* CMBlockBufferCustomBlockSource* */ IntPtr customBlockSource, // Can be null
			/* size_t */ nuint offsetToData,
			/* size_t */ nuint dataLength,
			CMBlockBufferFlags flags,
			/* CMBlockBufferRef* */ out IntPtr newBlockBuffer);

		public static CMBlockBuffer FromMemoryBlock (IntPtr memoryBlock, nuint blockLength, CMCustomBlockAllocator customBlockSource, nuint offsetToData, nuint dataLength, CMBlockBufferFlags flags, out CMBlockBufferError error)
		{
			var blockAllocator = memoryBlock == IntPtr.Zero ? IntPtr.Zero : CFAllocator.Null.Handle;
			IntPtr buffer;
			if (customBlockSource == null)
				error = CMBlockBufferCreateWithMemoryBlock (IntPtr.Zero, memoryBlock, blockLength, blockAllocator, IntPtr.Zero, offsetToData, dataLength, flags, out buffer);
			else
				error = CMBlockBufferCreateWithMemoryBlock (IntPtr.Zero, memoryBlock, blockLength, blockAllocator, ref customBlockSource.Cblock, offsetToData, dataLength, flags, out buffer);

			if (error != CMBlockBufferError.None)
				return null;

			var block = new CMBlockBuffer (buffer, true);
			block.customAllocator = customBlockSource;
			return block;
		}

		public static CMBlockBuffer FromMemoryBlock (byte [] data, nuint offsetToData, CMBlockBufferFlags flags, out CMBlockBufferError error)
		{
			if (data == null)
				throw new ArgumentNullException (nameof (data));

			var allocator = new CMManagedArrayBlockAllocator (data);
			return FromMemoryBlock (IntPtr.Zero, (uint) data.Length, allocator, offsetToData, (uint) data.Length, flags, out error);
		}

		[DllImport(Constants.CoreMediaLibrary)]
		extern static /* OSStatus */ CMBlockBufferError CMBlockBufferCreateContiguous (
			/* CFAllocatorRef */ IntPtr structureAllocator,
			/* CMBlockBufferRef */ IntPtr sourceBuffer,
			/* CFAllocatorRef */ IntPtr blockAllocator,
			/* CMBlockBufferCustomBlockSource* */ ref CMCustomBlockAllocator.CMBlockBufferCustomBlockSource customBlockSource,
			/* size_t */ nuint offsetToData,
			/* size_t */ nuint dataLength,
			CMBlockBufferFlags flags,
			/* CMBlockBufferRef* */ out IntPtr newBlockBuffer);

		[DllImport(Constants.CoreMediaLibrary)]
		extern static /* OSStatus */ CMBlockBufferError CMBlockBufferCreateContiguous (
			/* CFAllocatorRef */ IntPtr structureAllocator,
			/* CMBlockBufferRef */ IntPtr sourceBuffer,
			/* CFAllocatorRef */ IntPtr blockAllocator,
			/* CMBlockBufferCustomBlockSource */ IntPtr customBlockSource, // Can be null
			/* size_t */ nuint offsetToData,
			/* size_t */ nuint dataLength,
			CMBlockBufferFlags flags,
			/* CMBlockBufferRef* */ out IntPtr newBlockBuffer);

		public static CMBlockBuffer CreateContiguous (CMBlockBuffer sourceBuffer, CMCustomBlockAllocator customBlockSource, nuint offsetToData, nuint dataLength, CMBlockBufferFlags flags, out CMBlockBufferError error)
		{
			if (sourceBuffer == null)
				throw new ArgumentNullException ("sourceBuffer");

			IntPtr buffer;
			if (customBlockSource == null)
				error = CMBlockBufferCreateContiguous (IntPtr.Zero, sourceBuffer.handle, IntPtr.Zero, IntPtr.Zero, offsetToData, dataLength, flags, out buffer);
			else
				error = CMBlockBufferCreateContiguous (IntPtr.Zero, sourceBuffer.handle, IntPtr.Zero, ref customBlockSource.Cblock, offsetToData, dataLength, flags, out buffer);
			
			if (error != CMBlockBufferError.None)
				return null;

			var block = new CMBlockBuffer (buffer, true);
			block.customAllocator = customBlockSource;
			return block;
		}

		[DllImport(Constants.CoreMediaLibrary)]
		extern static /* OSStatus */ CMBlockBufferError CMBlockBufferAppendMemoryBlock (
			/* CMBlockBufferRef */ IntPtr buffer,
			/* void * */IntPtr memoryBlock,
			/* size_t */nuint blockLength,
			/* CFAllocatorRef */IntPtr blockAllocator,
			/* CMBlockBufferCustomBlockSource */ ref CMCustomBlockAllocator.CMBlockBufferCustomBlockSource customBlockSource,
			/* size_t */nuint offsetToData,
			/* size_t */nuint dataLength,
		    CMBlockBufferFlags flags);
		
		[DllImport(Constants.CoreMediaLibrary)]
		extern static /* OSStatus */ CMBlockBufferError CMBlockBufferAppendMemoryBlock (
			/* CMBlockBufferRef */ IntPtr buffer,
			/* void * */IntPtr memoryBlock,
			/* size_t */nuint blockLength,
			/* CFAllocatorRef */IntPtr blockAllocator,
			/* CMBlockBufferCustomBlockSource */ IntPtr customBlockSource, // can be null
			/* size_t */nuint offsetToData,
			/* size_t */nuint dataLength,
			CMBlockBufferFlags flags);
		
		public CMBlockBufferError AppendMemoryBlock (IntPtr memoryBlock, nuint blockLength, CMCustomBlockAllocator customBlockSource, nuint offsetToData, nuint dataLength, CMBlockBufferFlags flags)
		{
			if (Handle == IntPtr.Zero)
				throw new ObjectDisposedException ("BlockBuffer");

			var blockAllocator = memoryBlock == IntPtr.Zero ? IntPtr.Zero : CFAllocator.Null.Handle;
			if (customBlockSource == null)
				return CMBlockBufferAppendMemoryBlock (Handle, memoryBlock, blockLength, blockAllocator, IntPtr.Zero, offsetToData, dataLength, flags);
			else
				return CMBlockBufferAppendMemoryBlock (Handle, memoryBlock, blockLength, blockAllocator, ref customBlockSource.Cblock, offsetToData, dataLength, flags);
		}

		public CMBlockBufferError AppendMemoryBlock (byte [] data, nuint offsetToData, CMBlockBufferFlags flags)
		{
			if (Handle == IntPtr.Zero)
				throw new ObjectDisposedException ("BlockBuffer");
			if (data == null)
				throw new ArgumentNullException (nameof (data));

			var allocator = new CMManagedArrayBlockAllocator (data);
			return AppendMemoryBlock (IntPtr.Zero, (uint) data.Length, allocator, offsetToData, (uint) data.Length, flags);
		}
	}
}
