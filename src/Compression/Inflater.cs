// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;

using ObjCRuntime;

namespace Compression {

	internal sealed class Inflater : IDisposable {
		private CompressionStreamStruct _compression_struct;
		private bool _finished; // Whether the end of the stream has been reached
		private bool _shouldFinalize; // Whether we should end the stream
		private bool _isDisposed; // Prevents multiple disposals
		private GCHandle _inputBufferHandle; // The handle to the buffer that provides input to _zlibStream

		private object SyncLock => this; // Used to make writing to unmanaged structures atomic

		/// <summary>
		/// Initialized the Inflater with the given windowBits size
		/// </summary>
		internal Inflater (CompressionAlgorithm algorithm)
		{
			_finished = false;
			_isDisposed = false;
			InflateInit (algorithm);
		}

		public int AvailableOutput => (int) _compression_struct.DestinationSize;

		/// <summary>
		/// Returns true if the end of the stream has been reached.
		/// </summary>
		public bool Finished () => _finished;

		public unsafe bool Inflate (out byte b)
		{
			fixed (byte* bufPtr = &b) {
				int bytesRead = InflateVerified (bufPtr, 1);
				return bytesRead != 0;
			}
		}

		public unsafe int Inflate (byte [] bytes, int offset, int length)
		{
			if (bytes is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (bytes));
			// If Inflate is called on an invalid or unready inflater, return 0 to indicate no bytes have been read.
			if (length == 0)
				return 0;

			fixed (byte* bufPtr = bytes) {
				return InflateVerified (bufPtr + offset, length);
			}
		}

		public unsafe int Inflate (Span<byte> destination)
		{
			// If Inflate is called on an invalid or unready inflater, return 0 to indicate no bytes have been read.
			if (destination.Length == 0)
				return 0;

			fixed (byte* bufPtr = &MemoryMarshal.GetReference (destination!)) {
				return InflateVerified (bufPtr, destination.Length);
			}
		}

		public unsafe int InflateVerified (byte* bufPtr, int length)
		{
			// State is valid; attempt inflation
			try {
				var errCode = ReadInflateOutput (bufPtr, length, out var bytesRead);
				if (errCode == CompressionStatus.End) {
					_finished = true;
				}
				return bytesRead;
			} finally {
				// Before returning, make sure to release input buffer if necessary:
				if (0 == _compression_struct.SourceSize && _inputBufferHandle.IsAllocated) {
					DeallocateInputBufferHandle ();
				}
			}
		}

		public bool NeedsInput () => _compression_struct.SourceSize == 0;

		public void SetInput (byte [] inputBuffer, int startIndex, int count)
		{
			if (inputBuffer is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (inputBuffer));
			if (!NeedsInput ())
				throw new InvalidOperationException ("We have something left in previous input!");
			if (_inputBufferHandle.IsAllocated)
				throw new InvalidOperationException ("inputBuggerHandler should not be allocated");
			if (!(startIndex >= 0 && count >= 0 && count + startIndex <= inputBuffer.Length))
				throw new ArgumentOutOfRangeException ("count and start index are out of range.");

			if (startIndex + count < inputBuffer.Length) {
				_shouldFinalize = true;
			}

			if (0 == count)
				return;

			lock (SyncLock) {
				_inputBufferHandle = GCHandle.Alloc (inputBuffer, GCHandleType.Pinned);
				_compression_struct.Source = _inputBufferHandle.AddrOfPinnedObject () + startIndex;
				_compression_struct.SourceSize = count;
				_finished = false;
			}
		}

		private void Dispose (bool disposing)
		{
			if (!_isDisposed) {
				if (_inputBufferHandle.IsAllocated)
					DeallocateInputBufferHandle ();

				CompressionStreamStruct.compression_stream_destroy (ref _compression_struct);
				_isDisposed = true;
			}
		}

		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		~Inflater ()
		{
			Dispose (false);
		}

		/// <summary>
		/// Creates the Compression stream that will handle inflation.
		/// </summary>
		private void InflateInit (CompressionAlgorithm algorithm)
		{
			_compression_struct = new CompressionStreamStruct ();

			var status = CompressionStreamStruct.compression_stream_init (ref _compression_struct, StreamOperation.Decode, algorithm);
			if (status != CompressionStatus.Ok)
				throw new InvalidOperationException (status.ToString ());
			_compression_struct.Source = IntPtr.Zero;
			_compression_struct.SourceSize = 0;
		}

		/// <summary>
		/// Wrapper around the ZLib inflate function, configuring the stream appropriately.
		/// </summary>
		private unsafe CompressionStatus ReadInflateOutput (byte* bufPtr, int length, out int bytesRead)
		{
			lock (SyncLock) {
				_compression_struct.Destination = (IntPtr) bufPtr;
				_compression_struct.DestinationSize = length;
				// source is set in SetInput, nothing to be done
				var readStatus = CompressionStreamStruct.compression_stream_process (ref _compression_struct, (_shouldFinalize) ? StreamFlag.Finalize : StreamFlag.Continue);
				switch (readStatus) {
				case CompressionStatus.Ok:
				case CompressionStatus.End:
					bytesRead = length - (int) _compression_struct.DestinationSize;
					break;
				default:
					bytesRead = 0;
					break;
				}
				return readStatus;
			}
		}

		/// <summary>
		/// Frees the GCHandle being used to store the input buffer
		/// </summary>
		private void DeallocateInputBufferHandle ()
		{
			if (!_inputBufferHandle.IsAllocated)
				throw new InvalidOperationException ("Inputbufferhandler should be allocated.");

			lock (SyncLock) {
				_compression_struct.SourceSize = 0;
				_compression_struct.Source = IntPtr.Zero;
				_inputBufferHandle.Free ();
			}
		}
	}
}
