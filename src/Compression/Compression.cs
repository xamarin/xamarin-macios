using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using System.Threading.Tasks;

using ObjCRuntime;

namespace Compression {

	[iOS (9,0), TV (9,0), Mac (10,11)]
	public class CompressionStream : Stream
	{
		delegate int ReadMethod (byte[] array, int offset, int count);
		delegate void WriteMethod (byte[] array, int offset, int count);

		Stream base_stream;
		CompressionMode mode;
		CompressionAlgorithm algorithm;
		bool leaveOpen;
		bool disposed;
		DeflateStreamNative native;

		public CompressionStream (Stream stream, CompressionMode mode) :
			this (stream, mode, false)
		{
		}

		public CompressionStream (Stream stream, CompressionMode mode, bool leaveOpen) :
			this (stream, mode, CompressionAlgorithm.Zlib, leaveOpen)
		{
		}

		public CompressionStream (Stream stream, CompressionMode mode, CompressionAlgorithm algorithm) :
			this (stream, mode, algorithm, false)
		{
		}

		public CompressionStream (Stream compressedStream, CompressionMode mode, CompressionAlgorithm algorithm, bool leaveOpen)
		{
			if (compressedStream == null)
				throw new ArgumentNullException ("compressedStream");

			if (mode != CompressionMode.Compress && mode != CompressionMode.Decompress)
				throw new ArgumentException ("mode");

			this.base_stream = compressedStream;

			this.native = DeflateStreamNative.Create (compressedStream, mode, algorithm);
			if (this.native == null) {
				throw new NotImplementedException ("Failed to initialize internal compression structure.");
			}
			this.mode = mode;
			this.algorithm = algorithm;
			this.leaveOpen = leaveOpen;
		}
		
		protected override void Dispose (bool disposing)
		{
			native.Dispose (disposing);

			if (disposing && !disposed) {
				disposed = true;

				if (!leaveOpen) {
					Stream st = base_stream;
					if (st != null)
						st.Close ();
					base_stream = null;
				}
			}

			base.Dispose (disposing);
		}

		unsafe int ReadInternal (byte[] array, int offset, int count)
		{
			if (count == 0)
				return 0;

			return native.ReadStream (array, offset, count);
		}

		internal ValueTask<int> ReadAsyncMemory (Memory<byte> destination, CancellationToken cancellationToken)
		{
			throw new NotImplementedException ();
		}

		internal int ReadCore (Span<byte> destination)
		{
			throw new NotImplementedException ();
		}

		public override int Read (byte[] array, int offset, int count)
		{
			if (disposed)
				throw new ObjectDisposedException (GetType ().FullName);
			if (array == null)
				throw new ArgumentNullException ("Destination array is null.");
			if (!CanRead)
				throw new InvalidOperationException ("Stream does not support reading.");
			int len = array.Length;
			if (offset < 0 || count < 0)
				throw new ArgumentException ("Dest or count is negative.");
			if (offset > len)
				throw new ArgumentException ("destination offset is beyond array size");
			if ((offset + count) > len)
				throw new ArgumentException ("Reading would overrun buffer");

			return ReadInternal (array, offset, count);
		}

		unsafe void WriteInternal (byte[] array, int offset, int count)
		{
			if (count == 0)
				return;

			native.WriteStream (array, offset, count);
		}

		public override void Write (byte[] array, int offset, int count)
		{
			if (disposed)
				throw new ObjectDisposedException (GetType ().FullName);

			if (array == null)
				throw new ArgumentNullException ("array");

			if (offset < 0)
				throw new ArgumentOutOfRangeException ("offset");

			if (count < 0)
				throw new ArgumentOutOfRangeException ("count");

			if (!CanWrite)
				throw new NotSupportedException ("Stream does not support writing");

			if (offset > array.Length - count)
				throw new ArgumentException ("Buffer too small. count/offset wrong.");

			WriteInternal (array, offset, count);
		}

		public override void Flush ()
		{
			if (disposed)
				throw new ObjectDisposedException (GetType ().FullName);

			if (CanWrite) {
				native.Flush ();
			}
		}

		public override IAsyncResult BeginRead (byte [] array, int offset, int count,
							AsyncCallback asyncCallback, object asyncState)
		{
			if (disposed)
				throw new ObjectDisposedException (GetType ().FullName);

			if (!CanRead)
				throw new NotSupportedException ("This stream does not support reading");

			if (array == null)
				throw new ArgumentNullException ("array");

			if (count < 0)
				throw new ArgumentOutOfRangeException ("count", "Must be >= 0");

			if (offset < 0)
				throw new ArgumentOutOfRangeException ("offset", "Must be >= 0");

			if (count + offset > array.Length)
				throw new ArgumentException ("Buffer too small. count/offset wrong.");

			ReadMethod r = new ReadMethod (ReadInternal);
			return r.BeginInvoke (array, offset, count, asyncCallback, asyncState);
		}

		public override IAsyncResult BeginWrite (byte [] array, int offset, int count,
							AsyncCallback asyncCallback, object asyncState)
		{
			if (disposed)
				throw new ObjectDisposedException (GetType ().FullName);

			if (!CanWrite)
				throw new InvalidOperationException ("This stream does not support writing");

			if (array == null)
				throw new ArgumentNullException ("array");

			if (count < 0)
				throw new ArgumentOutOfRangeException ("count", "Must be >= 0");

			if (offset < 0)
				throw new ArgumentOutOfRangeException ("offset", "Must be >= 0");

			if (count + offset > array.Length)
				throw new ArgumentException ("Buffer too small. count/offset wrong.");

			WriteMethod w = new WriteMethod (WriteInternal);
			return w.BeginInvoke (array, offset, count, asyncCallback, asyncState);			
		}

		public override int EndRead(IAsyncResult asyncResult)
		{
			if (asyncResult == null)
				throw new ArgumentNullException ("asyncResult");

			AsyncResult ares = asyncResult as AsyncResult;
			if (ares == null)
				throw new ArgumentException ("Invalid IAsyncResult", "asyncResult");

			ReadMethod r = ares.AsyncDelegate as ReadMethod;
			if (r == null)
				throw new ArgumentException ("Invalid IAsyncResult", "asyncResult");

			return r.EndInvoke (asyncResult);
		}

		public override void EndWrite (IAsyncResult asyncResult)
		{
			if (asyncResult == null)
				throw new ArgumentNullException ("asyncResult");

			AsyncResult ares = asyncResult as AsyncResult;
			if (ares == null)
				throw new ArgumentException ("Invalid IAsyncResult", "asyncResult");

			WriteMethod w = ares.AsyncDelegate as WriteMethod;
			if (w == null)
				throw new ArgumentException ("Invalid IAsyncResult", "asyncResult");

			w.EndInvoke (asyncResult);
			return;
		}

		public override long Seek (long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
		}

		public override void SetLength (long value)
		{
			throw new NotSupportedException();
		}

		public Stream BaseStream {
			get { return base_stream; }
		}

		public override bool CanRead {
			get { return !disposed && mode == CompressionMode.Decompress && base_stream.CanRead; }
		}

		public override bool CanSeek {
			get { return false; }
		}

		public override bool CanWrite {
			get { return !disposed && mode == CompressionMode.Compress && base_stream.CanWrite; }
		}

		public override long Length {
			get { throw new NotSupportedException(); }
		}

		public override long Position {
			get { throw new NotSupportedException(); }
			set { throw new NotSupportedException(); }
		}
	}

	class DeflateStreamNative
	{

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

		Stream base_stream;
		CompressionStreamStruct compression_struct;
		StreamOperation operation;
		CompressionAlgorithm algorithm;
		bool disposed;

		private DeflateStreamNative ()
		{
		}

		public static DeflateStreamNative Create (Stream compressedStream, CompressionMode mode, CompressionAlgorithm algorithm)
		{
			var dsn = new DeflateStreamNative ();
			dsn.operation = (mode == CompressionMode.Compress) ? StreamOperation.Encode : StreamOperation.Decode;
			dsn.algorithm = algorithm;
			dsn.base_stream = compressedStream;
			dsn.compression_struct = new CompressionStreamStruct ();
			
			var status = compression_stream_init (ref dsn.compression_struct, dsn.operation, dsn.algorithm);
			if (status != CompressionStatus.Ok)
				return null;
			return dsn;
		}

		~DeflateStreamNative ()
		{
			Dispose (false);
		}

		public void Dispose()
		{ 
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		public void Dispose (bool disposing)
		{
			if (disposed)
				return;

			compression_stream_destroy (ref compression_struct);
			disposed = true;
		}

		public void Flush ()
		{
			if (compression_struct.SourceSize != 0) {
				var closeStatus = compression_stream_process (ref compression_struct, StreamFlag.Finalize);
				switch (closeStatus) {
				case CompressionStatus.End:
					break;
				case CompressionStatus.Ok:
					// as per the docs, we should never get here, lets throw an exception so that we know it happened.
					throw new IOException ("Unexpected CompressionStatus.Ok received.");
				default:
					throw new IOException ($"An error occurred when performing the operation: {closeStatus}");
				}
			}
		}

		unsafe public int ReadStream (byte[] array, int offset, int count)
		{

			var srcManagedBuffer = new byte [count - offset];
			var dstManagedBuffer = new byte [count - offset];
			fixed (byte *srcBuf = srcManagedBuffer)
			fixed (byte *dstBuf = dstManagedBuffer) {
				var read = base_stream.Read (srcManagedBuffer, offset, count); // we are taking care of the offset in this call

				compression_struct.Destination = (IntPtr) dstBuf;
				compression_struct.Source = (IntPtr) srcBuf;

				// copy the data to the source buffer from the internal stream
				compression_struct.DestinationSize = read;
				compression_struct.SourceSize = read;
				var readStatus = compression_stream_process (ref compression_struct, StreamFlag.Continue);
				switch (readStatus) {
				case CompressionStatus.Ok:
				case CompressionStatus.End:
						// copy the data to the passed array, no need to deal with the offset, read took care, count should be read
						Array.Copy (dstManagedBuffer, 0, array, 0, read);
					break;
				default:
					throw new IOException ($"An error occurred when performing the operation: {readStatus}");
				}
				return read;
			}
		}

		unsafe public void WriteStream (byte[] buffer, int offset, int count)
		{
			var srcManagedBuffer = new byte [count - offset];
			var dstManagedBuffer = new byte [count - offset];

			fixed (byte *srcBuf = srcManagedBuffer)
			fixed (byte *dstBuf = dstManagedBuffer) {
				compression_struct.Destination = (IntPtr) dstBuf;
				compression_struct.DestinationSize = count - offset;
				compression_struct.Source = (IntPtr) srcBuf;
				compression_struct.SourceSize = count - offset;
				// we need to copy the data from the passed buffer to the source buffer
				Array.Copy (buffer, offset, srcManagedBuffer, 0, count);
				var writeStatus = compression_stream_process (ref compression_struct, StreamFlag.Continue);
				switch (writeStatus) {
				case CompressionStatus.Ok:
				case CompressionStatus.End:
					if (compression_struct.DestinationSize == 0) {
						// we are happy, and we need to write to the stream all the data from the destination
						// buffer
						base_stream.Write (dstManagedBuffer, 0, count); // no need to deal with offset, the copy did it
					}
					break;
				default:
					throw new InvalidOperationException ($"An error occurred when performing the operation: {writeStatus}");
				}
			}
		}

	}
}
