//
// MonoMac.CFNetwork.WebResponseStream
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
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CoreFoundation;
using CoreServices;
using Foundation;
using CFNetwork;

#nullable enable

namespace CFNetwork {
	/*
	 * For optimal performance and reliability, either only access the
	 * public System.IO.Stream methods from the application's main thread
	 * or schedule the MessageHandler on a WorkerThread.
	 * 
	 * If is permitted to use the public Stream methods from a ThreadPool
	 * thread, though this scenario hasn't been very carefully tested for
	 * race conditions yet.
	 */

	class WebResponseStream : Stream, IDisposable {
		CFHTTPStream? stream;
		WorkerThread? worker;
		WebRequestStream? body;
		CancellationTokenSource? openCts;
		TaskCompletionSource<CFHTTPMessage?>? openTcs;
		IOperation? currentOperation;
		bool bytesAvailable;
		bool busy;
		Thread? mainThread;
		Thread? workerThread;
		volatile bool crossThreadAccess;
		object syncRoot;
		bool open;
		bool canceled;
		bool completed;
		Exception? lastError;

		WebResponseStream (CFHTTPStream stream, WebRequestStream? body)
		{
			this.stream = stream;
			this.body = body;
			syncRoot = new object ();
		}

		public static WebResponseStream? Create (CFHTTPMessage request)
		{
			var stream = CFStream.CreateForHTTPRequest (request);
			if (stream is null)
				return null;

			return new WebResponseStream (stream, null);
		}

		public static WebResponseStream? Create (CFHTTPMessage request, WebRequestStream body)
		{
			var stream = CFStream.CreateForStreamedHTTPRequest (request, body.ReadStream);
			if (stream is null)
				return null;

			return new WebResponseStream (stream, body);
		}

		public static WebResponseStream? Create (Uri uri, HttpMethod method, Version version)
		{
			using (var req = CFHTTPMessage.CreateRequest (uri, method.Method, version))
				return Create (req);
		}

		public CFHTTPStream? Stream {
			get { return stream; }
		}

		~WebResponseStream ()
		{
			Dispose (false);
		}

		protected override void Dispose (bool disposing)
		{
			if (disposing) {
				OnCanceled ();
				if (stream is not null) {
					stream.Dispose ();
					stream = null;
				}
				if (openCts is not null) {
					openCts.Dispose ();
					openCts = null;
				}
			}
			base.Dispose (disposing);
		}

		void OnError (Exception? error)
		{
			if (error is null)
				error = new InvalidOperationException ("Unknown error.");

			if (completed)
				return;
			lastError = error;
			completed = true;

			stream?.Close ();

			if (!open)
				openTcs?.SetException (error);

			var operation = Interlocked.Exchange (ref currentOperation, null);
			if (operation is not null)
				operation.SetException (error);
		}

		void OnCanceled ()
		{
			if (completed)
				return;
			completed = canceled = true;

			stream?.Close ();

			if (!open)
				openTcs?.SetCanceled ();

			var operation = Interlocked.Exchange (ref currentOperation, null);
			if (operation is not null)
				operation.SetCanceled ();
		}

		void OnCompleted ()
		{
			if (completed)
				return;
			completed = true;

			stream?.Close ();

			if (!open) {
				openTcs?.SetException (new InvalidOperationException ());
				return;
			}

			var operation = Interlocked.Exchange (ref currentOperation, null);
			if (operation is not null)
				operation.SetCompleted ();
		}

		/*
		 * Under normal circumstances, we're running on the main thread,
		 * so we could do without all the locking.
		 * 
		 * However, we're exposing a System.IO.Stream to the user, who
		 * might be using ConfigureAwait (false).
		 * 
		 * Ideally, consumers of the API should either only access the
		 * stream from the main thread or use a 'WorkerThread'.
		 * 
		 */

		void StartOperation (IOperation operation)
		{
			bool isCrossThread;
			lock (syncRoot) {
				if (!open || (currentOperation is not null))
					throw new InvalidOperationException ();

				if (canceled) {
					operation.SetCanceled ();
					return;
				}
				if (lastError is not null) {
					operation.SetException (lastError);
					return;
				}
				if (completed) {
					operation.SetCompleted ();
					return;
				}
				currentOperation = operation;

				isCrossThread = CheckCrossThreadAccess ();

				if (!bytesAvailable)
					return;
			}

			/*
			 * The server already sent us the OnBytesAvailable() event
			 * before the operation started.
			 * 
			 * If we have a worker thread, we simply handle it there and
			 * don't have to worry about any locking or anything.
			 * 
			 */
			if ((worker is not null) && !Thread.CurrentThread.Equals (workerThread)) {
				worker.Post (() => {
					if (bytesAvailable)
						OnBytesAvailable (false);
				}
				);
				return;
			}

			/*
			 * We're on the main / worker thread, so we don't need any locking.
			 */
			if (!isCrossThread) {
				OnBytesAvailable (false);
				return;
			}

			/*
			 * Ok, now it's getting complicated: we're neither on the main nor on
			 * the worker thread, so we need to do some locking here.
			 * 
			 */
			Monitor.Enter (syncRoot);
			if (!bytesAvailable) {
				Monitor.Exit (syncRoot);
				return;
			}

			OnBytesAvailable (true);
		}

		bool CheckCrossThreadAccess ()
		{
			if (crossThreadAccess)
				return true;
			if (Thread.CurrentThread.Equals (mainThread))
				return false;
			if (Thread.CurrentThread.Equals (workerThread))
				return false;
			crossThreadAccess = true;
			return true;
		}

		void HasBytesAvailable ()
		{
			/*
			 * We're always on the main / worker thread here.
			 * 
			 * As long as nobody accesses the Stream API from another thread,
			 * we don't need any of the locking.
			 * 
			 */
			if (!crossThreadAccess) {
				if ((currentOperation is null) || busy) {
					bytesAvailable = true;
					return;
				}

				if (!crossThreadAccess) {
					OnBytesAvailable (false);
					return;
				}
			}

			/*
			 * Acquire and keep the lock until OnBytesAvailable()
			 * releases it.
			 */

			Monitor.Enter (syncRoot);

			if ((currentOperation is null) || busy) {
				bytesAvailable = true;
				Monitor.Exit (syncRoot);
				return;
			}

			OnBytesAvailable (true);
		}

		async Task OnBytesAvailable (bool exitContext)
		{
			bool keepGoing;
			do {
				bytesAvailable = false;
				try {
					keepGoing = await ReadFromServer (exitContext);
				} catch (Exception ex) {
					OnError (ex);
					break;
				}

				/*
				 * 'bytesAvailable' is true here if the server sent us another
				 * OnBytesAvailable event while we were sending the data to
				 * the client.
				 *
				 */
			} while (bytesAvailable && keepGoing);
			if (exitContext)
				Monitor.Exit (syncRoot);
		}

		async Task<bool> ReadFromServer (bool exitContext)
		{
			int index, count;
			var buffer = currentOperation!.GetBuffer (out index, out count);

			nint ret;
			try {
				ret = stream!.Read (buffer, index, count);
			} catch (Exception ex) {
				OnError (ex);
				return false;
			}

			/*
			 * If there are still bytes available to be read, then we'll immediately
			 * get another BytesAvailable event, whereas calling stream.Read() again
			 * could block.
			 */

			if (ret < 0) {
				OnError (stream?.GetError ());
				return false;
			} else if (ret == 0) {
				OnCompleted ();
				return false;
			}

			/*
			 * We're normally called from the CFReadStream's OnBytesAvailableEvent
			 * on the main thread, though OperationStarted() may also call us from
			 * a ThreadPool thread.
			 * 
			 * Release the lock while we're writing the data and re-acquire it when
			 * done with that.  The server may send us a OnBytesAvailableEvent while
			 * we're await'ing - if that happens, 'onBytesAvailable' will be set.
			 */

			busy = true;
			if (exitContext)
				Monitor.Exit (syncRoot);

			bool keepGoing;
			try {
				keepGoing = await currentOperation.Write ((int) ret);
			} finally {
				if (exitContext)
					Monitor.Enter (syncRoot);
				busy = false;
			}

			/*
			 * 'keepGoing' specifies whether the client wants more data from us.
			 */

			if (keepGoing)
				return true;

			var operation = Interlocked.Exchange<IOperation?> (ref currentOperation, null);
			operation?.SetCompleted ();
			return false;
		}

		interface IOperation : IDisposable {
			bool IsCompleted {
				get;
			}

			void SetCompleted ();

			void SetCanceled ();

			void SetException (Exception error);

			byte [] GetBuffer (out int index, out int count);

			Task<bool> Write (int count);
		}

		abstract class Operation<T> : IOperation, IDisposable {
			CancellationTokenSource? cts;
			TaskCompletionSource<T?> tcs;
			bool completed;

			protected Operation (WebResponseStream parent,
								 CancellationToken cancellationToken)
			{
				cts = CancellationTokenSource.CreateLinkedTokenSource (
					cancellationToken);
				cts.Token.Register (() => parent.OnCanceled ());
				tcs = new TaskCompletionSource<T?> ();
			}

			public Task<T?> Task {
				get { return tcs.Task; }
			}

			public bool IsCompleted {
				get { return completed; }
			}

			protected TaskCompletionSource<T?> TaskCompletionSource {
				get { return tcs; }
			}

			protected CancellationToken? CancellationToken {
				get { return cts?.Token; }
			}

			public void SetCanceled ()
			{
				if (completed)
					return;
				completed = true;
				tcs.SetCanceled ();
			}

			public void SetException (Exception error)
			{
				if (completed)
					return;
				completed = true;
				tcs.SetException (error);
			}

			public void SetCompleted ()
			{
				if (completed)
					return;
				completed = true;
				OnCompleted ();
			}

			protected abstract void OnCompleted ();

			public abstract byte [] GetBuffer (out int index, out int count);

			public abstract Task<bool> Write (int count);

			~Operation ()
			{
				Dispose (false);
			}

			public void Dispose ()
			{
				Dispose (true);
				GC.SuppressFinalize (this);
			}

			protected virtual void Dispose (bool disposing)
			{
				if (disposing) {
					SetCanceled ();
					if (cts is not null) {
						cts.Dispose ();
						cts = null;
					}
				}
			}
		}

		class CopyToAsyncOperation : Operation<object> {
			Stream destination;
			byte [] buffer;

			public CopyToAsyncOperation (WebResponseStream parent,
										 Stream destination, int bufferSize,
										 CancellationToken cancellationToken)
				: base (parent, cancellationToken)
			{
				this.destination = destination;
				buffer = new byte [bufferSize];
			}

			public override byte [] GetBuffer (out int index, out int count)
			{
				index = 0;
				count = buffer.Length;
				return buffer;
			}

			public override async Task<bool> Write (int count)
			{
				await destination.WriteAsync (buffer, 0, count, CancellationToken ?? System.Threading.CancellationToken.None);
				return true;
			}

			protected override void OnCompleted ()
			{
				TaskCompletionSource.SetResult (null);
			}
		}

		class ReadAsyncOperation : Operation<int> {
			byte [] buffer;
			int bufferIndex;
			int bufferCount;
			int successfullyWritten;

			public ReadAsyncOperation (WebResponseStream parent,
									   byte [] buffer, int offset, int count,
									   CancellationToken cancellationToken)
				: base (parent, cancellationToken)
			{
				this.buffer = buffer;
				this.bufferIndex = offset;
				this.bufferCount = count;
			}

			public override byte [] GetBuffer (out int index, out int count)
			{
				index = bufferIndex;
				count = bufferCount;
				return buffer;
			}

			public override Task<bool> Write (int count)
			{
				bufferIndex += count;
				successfullyWritten += count;
				bool keepGoing = bufferIndex < bufferCount;
				return System.Threading.Tasks.Task.FromResult (keepGoing);
			}

			protected override void OnCompleted ()
			{
				TaskCompletionSource.SetResult (successfullyWritten);
			}
		}

		public async Task<CFHTTPMessage?> Open (WorkerThread worker,
											   CancellationToken cancellationToken)
		{
			this.worker = worker;
			openTcs = new TaskCompletionSource<CFHTTPMessage?> ();
			openCts = CancellationTokenSource.CreateLinkedTokenSource (cancellationToken);

			openCts.Token.Register (() => OnCanceled ());

			mainThread = Thread.CurrentThread;

			try {
				if (worker is not null)
					await worker.Post (c => DoOpen (), openCts.Token);
				else
					DoOpen ();
				var result = await openTcs.Task;
				return result;
			} finally {
				openCts.Dispose ();
				openCts = null;
			}
		}

		void DoOpen ()
		{
			if (lastError is not null) {
				openTcs!.SetException (lastError);
				return;
			}

			/*
			 * We must wait until the HasBytesAvailableEvent has been fired
			 * before we can access the result.
			 *
			 */

			stream!.ErrorEvent += (sender, e) => {
				OnError (stream!.GetError ());
			};
			stream.ClosedEvent += (sender, e) => {
				if (!open) {
					open = true;
					openTcs!.SetResult (stream.GetResponseHeader ());
				}
				OnCompleted ();
			};
			stream.HasBytesAvailableEvent += (sender, e) => {
				if (!open) {
					open = true;
					openTcs!.SetResult (stream.GetResponseHeader ());
				}

				HasBytesAvailable ();
			};
			stream.OpenCompletedEvent += (sender, e) => {
				if (body is null)
					return;
				body.Open ();
			};

			workerThread = Thread.CurrentThread;

			stream.EnableEvents (CFRunLoop.Current, CFRunLoop.ModeDefault);
			stream.Open ();
		}

		#region implemented abstract members of System.IO.Stream

		public override Task CopyToAsync (Stream destination, int bufferSize,
										  CancellationToken cancellationToken)
		{
			var operation = new CopyToAsyncOperation (
				this, destination, bufferSize, cancellationToken);
			StartOperation (operation);
			return operation.Task;
		}

		public override void Flush ()
		{
			;
		}

		public override Task<int> ReadAsync (byte [] buffer, int offset, int count,
											 CancellationToken cancellationToken)
		{
			var operation = new ReadAsyncOperation (
				this, buffer, offset, count, cancellationToken);
			StartOperation (operation);
			return operation.Task;
		}

		public override int Read (byte [] buffer, int offset, int count)
		{
			if (Thread.CurrentThread.Equals (mainThread) ||
				Thread.CurrentThread.Equals (workerThread))
				throw new InvalidOperationException (
					"You must not use synchronous Read() from the main thread.");

			return ReadAsync (buffer, offset, count, CancellationToken.None).Result;
		}

		public override long Seek (long offset, SeekOrigin origin)
		{
			throw new NotSupportedException ();
		}

		public override void SetLength (long value)
		{
			throw new NotSupportedException ();
		}

		public override void Write (byte [] buffer, int offset, int count)
		{
			throw new NotSupportedException ();
		}

		public override bool CanRead {
			get { return true; }
		}

		public override bool CanSeek {
			get { return false; }
		}

		public override bool CanWrite {
			get { return false; }
		}

		public override long Length {
			get { throw new NotSupportedException (); }
		}

		public override long Position {
			get { throw new NotSupportedException (); }
			set { throw new NotSupportedException (); }
		}

		#endregion
	}
}
#endif // !NET
