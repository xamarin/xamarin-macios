//
// MonoMac.CFNetwork/WebRequestStream
//
// Authors:
//      Martin Baulig (martin.baulig@gmail.com)
//
// Copyright 2012 Xamarin Inc. (http://www.xamarin.com)
//
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

#if !NET
using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using CoreFoundation;

using CoreServices;

using Foundation;

#nullable enable

namespace CFNetwork {

	class WebRequestStream {
		Stream stream;
		CFReadStream readStream;
		CFWriteStream writeStream;
		TaskCompletionSource<object?> openTcs;
		CancellationTokenSource cts;

		byte [] buffer;
		bool canWrite;
		bool open, completed, closed;
		int start, position;

		const int BufferSize = 65536;
		const int BufferThreshold = 49152;

		public WebRequestStream (Stream stream, CancellationToken cancellationToken)
		{
			this.stream = stream;

			buffer = new byte [BufferSize];
			cts = CancellationTokenSource.CreateLinkedTokenSource (cancellationToken);

			openTcs = new TaskCompletionSource<object?> ();

			cts.Token.Register (() => Close ());

			CFStream.CreateBoundPair (out readStream, out writeStream, BufferSize);
		}

		public CFReadStream ReadStream {
			get { return readStream; }
		}

		public async Task Open ()
		{
			writeStream.OpenCompletedEvent += (sender, e) => {
				open = true;
				openTcs.SetResult (null);
				if (canWrite)
					Write ();
			};
			writeStream.CanAcceptBytesEvent += (sender, e) => {
				if (!open) {
					canWrite = true;
					return;
				}
				Write ();
			};
			writeStream.ErrorEvent += (sender, e) => {
				Close ();
			};

			writeStream.EnableEvents (CFRunLoop.Current, CFRunLoop.ModeDefault);
			writeStream.Open ();

			await openTcs.Task;
			open = true;
		}

		bool busy;

		/*
		 * IMPORTANT:
		 * 
		 * This method assumes that we're either running on the application's main
		 * thread or a worker thread that has setup the SynchronizationContext like
		 * the 'WorkerThread' class does.
		 * 
		 * In this scenario, both CFStream callbacks and await's continuations will
		 * both run on the same thread, so we don't need any locking here.
		 * 
		 */

		async Task Write ()
		{
			if (busy)
				return;
			busy = true;

			int available = position - start;
			int remainingSpace = BufferSize - position;

			if (!completed && ((available == 0) || (remainingSpace >= BufferThreshold))) {
				var ret = await stream.ReadAsync (buffer, position, remainingSpace, cts.Token);
				if (ret < 0) {
					Close ();
					return;
				} else if (ret == 0) {
					completed = true;
				} else {
					position += ret;
					available += ret;
					remainingSpace -= ret;
				}
			}

			if (available == 0) {
				Close ();
				return;
			}

			var ret2 = writeStream.Write (buffer, start, available);
			if (ret2 <= 0) {
				Close ();
				return;
			}

			start += ret2;
			if (start == position) {
				start = position = 0;
			} else if (start >= BufferThreshold) {
				available = position - start;
				Buffer.BlockCopy (buffer, start, buffer, 0, available);
				start = 0;
				position = available;
			}

			busy = false;
		}

		void Close ()
		{
			if (closed)
				return;
			if (!open)
				openTcs.SetCanceled ();
			closed = completed = true;
			if (writeStream.GetStatus () != CFStreamStatus.Closed)
				writeStream.Close ();
			writeStream.Dispose ();
			stream.Dispose ();
		}
	}
}
#endif // !NET
