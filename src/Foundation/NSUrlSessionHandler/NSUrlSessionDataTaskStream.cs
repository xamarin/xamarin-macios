using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Foundation;

#nullable enable

#if !MONOMAC
namespace System.Net.Http {
#else
namespace Foundation {
#endif

	class NSUrlSessionDataTaskStream : Stream
	{
		readonly Queue<NSData> data;
		readonly object dataLock = new object ();

		long position;
		long length;

		bool receivedAllData;
		Exception? exc;

		NSData? current;
		Stream? currentStream;

		public NSUrlSessionDataTaskStream ()
		{
			data = new Queue<NSData> ();
		}

		public void Add (NSData d)
		{
			lock (dataLock) {
				data.Enqueue (d);
				length += (int)d.Length;
			}
		}

		public void TrySetReceivedAllData ()
		{
			receivedAllData = true;
		}

		public void TrySetException (Exception e)
		{
			exc = e;
			TrySetReceivedAllData ();
		}

		void ThrowIfNeeded (CancellationToken cancellationToken)
		{
			if (exc is not null)
				throw exc;

			cancellationToken.ThrowIfCancellationRequested ();
		}

		public override int Read (byte [] buffer, int offset, int count)
		{
			return ReadAsync (buffer, offset, count).Result;
		}

		public override async Task<int> ReadAsync (byte [] buffer, int offset, int count, CancellationToken cancellationToken)
		{
			// try to throw on enter
			ThrowIfNeeded (cancellationToken);

			while (current is null) {
				lock (dataLock) {
					if (data.Count == 0 && receivedAllData && position == length)
						return 0;

					if (data.Count > 0 && current is null) {
						current = data.Peek ();
						currentStream = current.AsStream ();
						break;
					}
				}

				try {
					await Task.Delay (50, cancellationToken).ConfigureAwait (false);
				} catch (TaskCanceledException ex) {
					// add a nicer exception for the user to catch, add the cancelation exception
					// to have a decent stack
					throw new TimeoutException ("The request timed out.", ex);
				}
			}

			// try to throw again before read
			ThrowIfNeeded (cancellationToken);

			var d = currentStream!;
			var bufferCount = Math.Min (count, (int)(d.Length - d.Position));
			var bytesRead = await d.ReadAsync (buffer, offset, bufferCount, cancellationToken).ConfigureAwait (false);

			// add the bytes read from the pointer to the position
			position += bytesRead;

			// remove the current primary reference if the current position has reached the end of the bytes
			if (d.Position == d.Length) {
				lock (dataLock) {
					// this is the same object, it was done to make the cleanup
					data.Dequeue ();
					currentStream?.Dispose ();
					// We cannot use current?.Dispose. The reason is the following one:
					// In the DidReceiveResponse, if iOS realizes that a buffer can be reused,
					// because the data is the same, it will do so. Such a situation does happen
					// between requests, that is, request A and request B will get the same NSData
					// (buffer) in the delegate. In this case, we cannot dispose the NSData because
					// it might be that a different request received it and it is present in
					// its NSUrlSessionDataTaskStream stream. We can only trust the gc to do the job
					// which is better than copying the data over. 
					current = null;
					currentStream = null;
				}
			}

			return bytesRead;
		}

		public override bool CanRead => true;

		public override bool CanSeek => false;

		public override bool CanWrite => false;

		public override bool CanTimeout => false;

		public override long Length => length;

		public override void SetLength (long value)
		{
			throw new InvalidOperationException ();
		}

		public override long Position {
			get { return position; }
			set { throw new InvalidOperationException (); }
		}

		public override long Seek (long offset, SeekOrigin origin)
		{
			throw new InvalidOperationException ();
		}

		public override void Flush ()
		{
			throw new InvalidOperationException ();
		}

		public override void Write (byte [] buffer, int offset, int count)
		{
			throw new InvalidOperationException ();
		}
	}
}
