using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

using ObjCRuntime;

namespace Compression {


	public partial class CompressionStream : IDisposable {

		[StructLayout(LayoutKind.Sequential)]
		struct CompressionStreamStruct {
			public IntPtr Destination;
			public int DestinationLength;
			public IntPtr Source;
			public int SourceLength;
			public IntPtr State;
		}

		[DllImport (Constants.libcompression)]
		static extern int /* CompressionStatus */ compression_stream_init (ref CompressionStreamStruct stream, int /* StreamOperation */ operation, int /* CompressionAlgorithm */ algorithm);

		[DllImport (Constants.libcompression)]
		static extern int /* CompressionStatus */ compression_stream_process (ref CompressionStreamStruct stream, int /* StreamFlag */ flags);

		[DllImport (Constants.libcompression)]
		static extern int /* CompressionStatus */ compression_stream_destroy (ref CompressionStreamStruct stream);

		static int defaultInBufSize = 256;
		static int defaultOutBufSize = 1024;

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
		IntPtr dstBuf;
		IntPtr srcBuf;

		public CompressionStream (Stream inSourceStream, Stream inDestinationStream, StreamOperation inOperation, CompressionAlgorithm algorithm)
			: this (inSourceStream, inDestinationStream, inOperation, algorithm, defaultInBufSize, defaultOutBufSize) { }

		public CompressionStream (Stream inSourceStream, Stream inDestinationStream, StreamOperation inOperation, CompressionAlgorithm algorithm, int inBufferSize, int outBufferSize)
		{
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
			internalStream.DestinationLength = outSize;
			internalStream.Source = srcBuf;
			internalStream.SourceLength = 0;
		}

		public async Task<float> Process ()
		{
			do {
				if (internalStream.SourceLength == 0) {
					if (!finalizeStream || operation == StreamOperation.Encode) {
						// Refill source buffer
						byte[] data = new byte[inSize];
						await sourceStream.ReadAsync (data, 0, inSize);
						Marshal.Copy(data, 0, internalStream.Source, data.Length);
						totalInputSize += data.Length;
						internalStream.Source = srcBuf;
						internalStream.SourceLength = data.Length;

						if (data.Length < inSize) {
							// Reached end of data.
							finalizeStream = true;
						}
					}
				}
				if (internalStream.DestinationLength == 0) {
					// output buffer is full, copy from managed to unmanaged and write
					byte[] data  = new byte[outSize];
					Marshal.Copy (internalStream.Destination, data, 0, outSize);
					await destinationStream.WriteAsync (data, 0, outSize); // we should be appending
					totalOutputSize += outSize;

					internalStream.Destination = dstBuf;
					internalStream.DestinationLength = outSize;
				}

				// stream should be set up, perform the action
				status = (CompressionStatus) compression_stream_process (ref internalStream, 0); // TODO, use flags instead!
				switch (status) {
				case CompressionStatus.Ok:
					// we will do the loop at least once more, lets be ready
					if (internalStream.DestinationLength == 0) {
						byte[] data  = new byte[outSize];
						Marshal.Copy (internalStream.Destination, data, 0, outSize);
						await destinationStream.WriteAsync (data, 0, outSize); // we should be appending
						totalOutputSize += outSize;

						internalStream.Destination = dstBuf;
						internalStream.DestinationLength = outSize;
					}
					break;
				case CompressionStatus.End:
					// we are done, write whatever we have in the buffer
					if ((long)internalStream.Destination > (long)dstBuf) {
						var size = (int)((long)internalStream.Destination - (long)dstBuf);
						byte[] data  = new byte[size];
						Marshal.Copy (internalStream.Destination, data, 0, outSize);
						await destinationStream.WriteAsync (data, 0, size); // we should be appending
						totalOutputSize += size;
					}
					break;
				default:
					throw new InvalidOperationException ("An error occurred when performing the operation");
				}

			} while (status == CompressionStatus.Ok);

			return totalInputSize / totalOutputSize;
		}

		protected virtual void Dispose (bool disposing)
		{
			if (!disposedValue) {
				if (disposing) {
					// TODO: dispose managed state (managed objects).
				}
				compression_stream_destroy (ref internalStream);
				disposedValue = true;
			}
		}

		public void Dispose ()
		{
			Dispose (true);
		}
	}

}