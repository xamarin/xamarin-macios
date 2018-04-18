using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

using ObjCRuntime;

namespace Compression {

	[iOS (9,0), TV (9,0), Mac (10,11)]
	public partial class CompressionStream : IDisposable {

		[StructLayout(LayoutKind.Sequential)]
		struct CompressionStreamStruct {
			public IntPtr Destination; // uint8_t * dst_ptr
			public nint DestinationSize; // size_t dst_size;
			public IntPtr Source; // const uint8_t * src_ptr;
			public nint SourceSize; // size_t src_size;
			public IntPtr State; // void * __nullable state;
		}

		[DllImport (Constants.libcompression)]
		static extern int /* CompressionStatus */ compression_stream_init (ref CompressionStreamStruct stream, int /* StreamOperation */ operation, int /* CompressionAlgorithm */ algorithm);

		[DllImport (Constants.libcompression)]
		static extern int /* CompressionStatus */ compression_stream_process (ref CompressionStreamStruct stream, int /* StreamFlag */ flags);

		[DllImport (Constants.libcompression)]
		static extern int /* CompressionStatus */ compression_stream_destroy (ref CompressionStreamStruct stream);

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
			status = (CompressionStatus) compression_stream_init (ref internalStream, (int) operation, (int) algorithm);
			if (status != CompressionStatus.Ok)
				throw new InvalidOperationException ("Internal stream could not be created.");

			// set the different required values
			inSize = inBufferSize;
			outSize = outBufferSize;
			dstBuf = Marshal.AllocHGlobal (outSize);
			srcBuf = Marshal.AllocHGlobal (inBufferSize);

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
						byte[] data = new byte[inSize];
						var read = await sourceStream.ReadAsync (data, 0, inSize).ConfigureAwait (false);
						var dataRead = (read == 0)? data.Length : read;
						Marshal.Copy (data, 0, srcBuf, dataRead);
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
					byte[] data  = new byte[outSize];
					Marshal.Copy (dstBuf, data, 0, outSize);
					await destinationStream.WriteAsync (data, 0, outSize).ConfigureAwait (false); // we should be appending
					totalOutputSize += outSize;

					internalStream.Destination = dstBuf;
					internalStream.DestinationSize = outSize;
				}

				// stream should be set up, perform the action
				status = (CompressionStatus) compression_stream_process (ref internalStream, finalizeStream? (int)StreamFlag.Finalize:0); // TODO, use flags instead!
				switch (status) {
				case CompressionStatus.Ok:
					// we will do the loop at least once more, lets be ready
					if (internalStream.DestinationSize == 0) {
						byte[] data  = new byte[outSize];
						Marshal.Copy (dstBuf, data, 0, outSize);
						await destinationStream.WriteAsync (data, 0, outSize).ConfigureAwait (false); // we should be appending
						totalOutputSize += outSize;

						internalStream.Destination = dstBuf;
						internalStream.DestinationSize = outSize;
					}
					break;
				case CompressionStatus.End:
					// we are done, write whatever we have in the buffer
					if ((long)internalStream.Destination > (long)dstBuf) {
						var size = (int)((long)internalStream.Destination - (long)dstBuf);
						byte[] data  = new byte[size];
						Marshal.Copy (dstBuf, data, 0, size);
						await destinationStream.WriteAsync (data, 0, size).ConfigureAwait (false); // we should be appending
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
					Marshal.FreeHGlobal (dstBuf);
					dstBuf = IntPtr.Zero;
				}
				if (srcBuf != IntPtr.Zero) {
					Marshal.FreeHGlobal (srcBuf);
					srcBuf = IntPtr.Zero;
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
