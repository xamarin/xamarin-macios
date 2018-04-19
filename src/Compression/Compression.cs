using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

using ObjCRuntime;

namespace Compression {

	[iOS (9,0), TV (9,0), Mac (10,11)]
	public partial class CompressionStream : /* Stream */ IDisposable {

		[StructLayout(LayoutKind.Sequential)]
		struct CompressionStreamStruct {
			public IntPtr Destination; // uint8_t * dst_ptr
			public nint DestinationSize; // size_t dst_size;
			public IntPtr Source; // const uint8_t * src_ptr;
			public nint SourceSize; // size_t src_size;
			public IntPtr State; // void * __nullable state;
		}

		[DllImport (Constants.libcompression)]
		static extern CompressionStatus compression_stream_init (ref CompressionStreamStruct stream, StreamOperation operation, CompressionAlgorithm algorithm);

		[DllImport (Constants.libcompression)]
		static extern CompressionStatus compression_stream_process (ref CompressionStreamStruct stream, StreamFlag flags);

		[DllImport (Constants.libcompression)]
		static extern CompressionStatus compression_stream_destroy (ref CompressionStreamStruct stream);

		const int defaultInBufSize = 1024;
		const int defaultOutBufSize = 1024;

		int inSize;
		int outSize;
		bool disposedValue = false;
		bool finalizeStream = false;
		CompressionStatus status;
		StreamOperation operation;
		CompressionStreamStruct internalStream;
		Stream sourceStream;
		Stream destinationStream;
		int totalInputSize = 0;
		int totalOutputSize = 0;
		IntPtr dstBuf = IntPtr.Zero;
		IntPtr srcBuf = IntPtr.Zero;
		byte [] srcManagedBuffer;
		byte [] dstManagedBuffer;
		GCHandle srcHandle;
		GCHandle dstHandle;

		public CompressionStream (Stream inSourceStream, Stream inDestinationStream, StreamOperation inOperation, CompressionAlgorithm algorithm)
			: this (inSourceStream, inDestinationStream, inOperation, algorithm, defaultInBufSize, defaultOutBufSize) { }

		public CompressionStream (Stream inSourceStream, Stream inDestinationStream, StreamOperation inOperation, CompressionAlgorithm algorithm, int inBufferSize, int outBufferSize)
		{
			if (inSourceStream == null)
				throw new ArgumentNullException (nameof (inSourceStream));
			if (inDestinationStream == null)
				throw new ArgumentNullException (nameof (inDestinationStream));
			if (inBufferSize <= 0)
				throw new ArgumentException ("In buffer size cannot be 0 or smaller than 0");
			if (outBufferSize <= 0)
				throw new ArgumentException ("Out buffer size cannot be 0 or smaller than 0");

			operation = inOperation;
			sourceStream = inSourceStream;
			destinationStream = inDestinationStream;
			status = compression_stream_init (ref internalStream, operation, algorithm);
			if (status != CompressionStatus.Ok)
				throw new InvalidOperationException ("Internal stream could not be created.");

			// set the different required values
			inSize = inBufferSize;
			srcManagedBuffer = new byte [inSize];
			outSize = outBufferSize;
			dstManagedBuffer = new byte [outSize];

			// pin the objects and pass them to the struct
			srcHandle = GCHandle.Alloc (srcManagedBuffer, GCHandleType.Pinned);
			dstHandle = GCHandle.Alloc (dstManagedBuffer, GCHandleType.Pinned);
			srcBuf = srcHandle.AddrOfPinnedObject ();
			dstBuf = dstHandle.AddrOfPinnedObject ();

			internalStream.Destination = dstBuf;
			internalStream.DestinationSize = outSize;
			internalStream.Source = srcBuf;
			internalStream.SourceSize = 0;
		}

		public float Process ()
		{
			return ProcessAsync ().Result;
		}

		public async Task<float> ProcessAsync ()
		{
			do {
				if (internalStream.SourceSize == 0) {
					if (!finalizeStream || operation == StreamOperation.Encode) {
						// Refill source buffer
						var read = await sourceStream.ReadAsync (srcManagedBuffer, 0, inSize).ConfigureAwait (false);
						var dataRead = (read == 0)? srcManagedBuffer.Length : read;
						totalInputSize += dataRead;
						internalStream.Source = srcBuf;
						internalStream.SourceSize = dataRead;

						if (dataRead < inSize) {
							// Reached end of data.
							finalizeStream = true;
						}
					}
				}
				if (internalStream.DestinationSize == 0) {
					// output buffer is full, copy from managed to unmanaged and write
					await destinationStream.WriteAsync (dstManagedBuffer, 0, outSize).ConfigureAwait (false); // we should be appending
					totalOutputSize += outSize;

					internalStream.Destination = dstBuf;
					internalStream.DestinationSize = outSize;
				}

				// stream should be set up, perform the action
				status = compression_stream_process (ref internalStream, finalizeStream? StreamFlag.Finalize:StreamFlag.Continue); // TODO, use flags instead!
				switch (status) {
				case CompressionStatus.Ok:
					// we will do the loop at least once more, lets be ready
					if (internalStream.DestinationSize == 0) {
						await destinationStream.WriteAsync (dstManagedBuffer, 0, outSize).ConfigureAwait (false); // we should be appending
						totalOutputSize += outSize;

						internalStream.Destination = dstBuf;
						internalStream.DestinationSize = outSize;
					}
					break;
				case CompressionStatus.End:
					// we are done, write whatever we have in the buffer
					if ((long)internalStream.Destination > (long)dstBuf) {
						var size = (int)((long)internalStream.Destination - (long)dstBuf);
						await destinationStream.WriteAsync (dstManagedBuffer, 0, size).ConfigureAwait (false); // we should be appending
						totalOutputSize += size;
					}
					break;
				default:
					throw new InvalidOperationException ($"An error occurred when performing the operation: {status}");
				}

			} while (status == CompressionStatus.Ok);
			return totalInputSize / totalOutputSize;
		}

		protected virtual void Dispose (bool disposing)
		{
			if (!disposedValue) {
				if (disposing) {
				}
				compression_stream_destroy (ref internalStream);
				if (dstBuf != IntPtr.Zero) {
					dstBuf = IntPtr.Zero;
					dstHandle.Free ();
				}
				if (srcBuf != IntPtr.Zero) {
					srcBuf = IntPtr.Zero;
					srcHandle.Free ();
				}
				disposedValue = true;
			}
		}

		public void Dispose ()
		{
			Dispose (true);
		}

		~CompressionStream ()
		{
			Dispose (false);
		}
	}

}
