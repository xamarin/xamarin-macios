// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using System;
using System.Buffers;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;

using ObjCRuntime;

namespace Compression {

	internal sealed class Deflater : IDisposable {
		private CompressionStreamStruct _compression_struct;
		private bool _finished; // Whether the end of the stream has been reached
		private MemoryHandle _inputBufferHandle;
		private bool _isDisposed;

		// Note, DeflateStream or the deflater do not try to be thread safe.
		// The lock is just used to make writing to unmanaged structures atomic to make sure
		// that they do not get inconsistent fields that may lead to an unmanaged memory violation.
		// To prevent *managed* buffer corruption or other weird behaviour users need to synchronise
		// on the stream explicitly.
		private object SyncLock => this;

		/// <summary>
		/// Returns true if the end of the stream has been reached.
		/// </summary>
		public bool Finished () => _finished;

		internal Deflater (CompressionAlgorithm algorithm)
		{
			DeflateInit (algorithm);
		}

		~Deflater ()
		{
			Dispose (false);
		}

		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		private void Dispose (bool disposing)
		{
			if (!_isDisposed) {
				CompressionStreamStruct.compression_stream_destroy (ref _compression_struct);
				DeallocateInputBufferHandle ();
				_isDisposed = true;
			}
		}

		public bool NeedsInput () => _compression_struct.SourceSize == 0;

		internal unsafe void SetInput (ReadOnlyMemory<byte> inputBuffer)
		{
			if (!NeedsInput ())
				throw new InvalidOperationException ("We have something left in previous input!");
			if (_inputBufferHandle.Pointer is not null)
				throw new InvalidOperationException ("Unexpected input buffer handler found.");

			if (0 == inputBuffer.Length) {
				return;
			}

			lock (SyncLock) {
				_inputBufferHandle = inputBuffer.Pin ();

				_compression_struct.Source = (IntPtr) _inputBufferHandle.Pointer;
				_compression_struct.SourceSize = inputBuffer.Length;
			}
		}

		internal unsafe void SetInput (byte* inputBufferPtr, int count)
		{
			if (!NeedsInput ())
				throw new InvalidOperationException ("We have something left in previous input!");
			if (inputBufferPtr is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (inputBufferPtr));
			if (_inputBufferHandle.Pointer is not null)
				throw new InvalidOperationException ("Unexpected input buffer handler found.");

			if (count == 0) {
				return;
			}

			lock (SyncLock) {
				_compression_struct.Source = (IntPtr) inputBufferPtr;
				_compression_struct.SourceSize = count;
			}
		}

		internal int GetDeflateOutput (byte [] outputBuffer)
		{
			if (outputBuffer is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (outputBuffer));
			if (NeedsInput ())
				throw new InvalidOperationException ("GetDeflateOutput should only be called after providing input");

			try {
				ReadDeflateOutput (outputBuffer, StreamFlag.Continue, out var bytesRead);
				return bytesRead;
			} finally {
				// Before returning, make sure to release input buffer if necessary:
				if (0 == _compression_struct.SourceSize) {
					DeallocateInputBufferHandle ();
				}
			}
		}

		private unsafe CompressionStatus ReadDeflateOutput (byte [] outputBuffer, StreamFlag flushCode, out int bytesRead)
		{
			if (outputBuffer is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (outputBuffer));

			if (outputBuffer.Length < 0)
				throw new ArgumentException ("outputbuffer length must be bigger than 0");
			lock (SyncLock) {
				fixed (byte* bufPtr = &outputBuffer [0]) {
					_compression_struct.Destination = (IntPtr) bufPtr;
					_compression_struct.DestinationSize = outputBuffer.Length;

					var readStatus = CompressionStreamStruct.compression_stream_process (ref _compression_struct, flushCode);
					switch (readStatus) {
					case CompressionStatus.Ok:
					case CompressionStatus.End:
						bytesRead = outputBuffer.Length - (int) _compression_struct.DestinationSize;
						break;
					default:
						bytesRead = 0;
						break;
					}
					_finished = readStatus == CompressionStatus.End;
					return readStatus;
				}
			}
		}

		internal bool Finish (byte [] outputBuffer, out int bytesRead)
		{
			if (outputBuffer is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (outputBuffer));
			if (outputBuffer.Length < 0)
				throw new ArgumentException ("Can't pass in an empty output buffer!");

			var errC = ReadDeflateOutput (outputBuffer, StreamFlag.Finalize, out bytesRead);
			return errC == CompressionStatus.End;
		}

		/// <summary>
		/// Returns true if there was something to flush. Otherwise False.
		/// </summary>
		internal unsafe bool Flush (byte [] outputBuffer, out int bytesRead)
		{
			if (outputBuffer is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (outputBuffer));
			if (outputBuffer.Length < 0)
				throw new ArgumentException ("Can't pass in an empty output buffer!");
			if (!NeedsInput ())
				throw new InvalidOperationException ("We have something left in previous input!");
			if (_inputBufferHandle.Pointer is not null)
				throw new InvalidOperationException ("InputHandler should not be set");

			// Note: we require that NeedsInput() == true, i.e. that 0 == _zlibStream.AvailIn.
			// If there is still input left we should never be getting here; instead we
			// should be calling GetDeflateOutput.

			if (_finished)
				return ReadDeflateOutput (outputBuffer, StreamFlag.Finalize, out bytesRead) == CompressionStatus.Ok;
			bytesRead = 0;
			return false;
		}

		private void DeallocateInputBufferHandle ()
		{
			lock (SyncLock) {
				_compression_struct.SourceSize = 0;
				_compression_struct.Source = IntPtr.Zero;
				_inputBufferHandle.Dispose ();
			}
		}

		private void DeflateInit (CompressionAlgorithm algorithm)
		{
			_finished = false;
			_compression_struct = new CompressionStreamStruct ();

			var status = CompressionStreamStruct.compression_stream_init (ref _compression_struct, StreamOperation.Encode, algorithm);
			if (status != CompressionStatus.Ok)
				throw new InvalidOperationException (status.ToString ());
		}
	}
}
