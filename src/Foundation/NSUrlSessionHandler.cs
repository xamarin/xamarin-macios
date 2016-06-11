//
// NSUrlSessionHandler.cs:
//
// Authors:
//     Paul Betts <paul@paulbetts.org>
//     Nick Berardi <nick@nickberardi.com>
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

#if UNIFIED
using CoreFoundation;
using Foundation;
using Security;
#else
using MonoTouch.CoreFoundation;
using MonoTouch.Foundation;
using MonoTouch.Security;
using System.Globalization;
using nint = System.Int32;
using nuint = System.UInt32;
#endif

#if SYSTEM_NET_HTTP
namespace System.Net.Http {
#else
namespace Foundation {
#endif
	public partial class NSUrlSessionHandler : HttpMessageHandler
	{
		private readonly Dictionary<string, string> headerSeparators = new Dictionary<string, string> {
			["User-Agent"] = " ",
			["Server"] = " "
		};

		private readonly NSUrlSession session;
		private readonly Dictionary<NSUrlSessionTask, InflightData> inflightRequests;
		private readonly object inflightRequestsLock = new object ();

		public NSUrlSessionHandler ()
		{
			var configuration = NSUrlSessionConfiguration.DefaultSessionConfiguration;

			// we cannot do a bitmask but we can set the minimum based on ServicePointManager.SecurityProtocol minimum
			var sp = ServicePointManager.SecurityProtocol;
			if ((sp & SecurityProtocolType.Ssl3) != 0)
				configuration.TLSMinimumSupportedProtocol = SslProtocol.Ssl_3_0;
			else if ((sp & SecurityProtocolType.Tls) != 0)
				configuration.TLSMinimumSupportedProtocol = SslProtocol.Tls_1_0;
			else if ((sp & SecurityProtocolType.Tls11) != 0)
				configuration.TLSMinimumSupportedProtocol = SslProtocol.Tls_1_1;
			else if ((sp & SecurityProtocolType.Tls12) != 0)
				configuration.TLSMinimumSupportedProtocol = SslProtocol.Tls_1_2;

			session = NSUrlSession.FromConfiguration (NSUrlSessionConfiguration.DefaultSessionConfiguration, new NSUrlSessionHandlerDelegate (this), null);
			inflightRequests = new Dictionary<NSUrlSessionTask, InflightData> ();
		}

		private void RemoveInflightData (NSUrlSessionTask task, bool cancel = true)
		{
			InflightData inflight;
			lock (inflightRequestsLock)
				if (inflightRequests.TryGetValue (task, out inflight))
					inflightRequests.Remove (task);

			if (cancel)
				task?.Cancel ();

			task?.Dispose ();
		}

		protected override void Dispose (bool disposing)
		{
			lock (inflightRequestsLock) {
				foreach (var pair in inflightRequests) {
					pair.Key?.Cancel ();
					pair.Key?.Dispose ();
				}

				inflightRequests.Clear ();
			}

			base.Dispose (disposing);
		}

		private string GetHeaderSeparator (string name)
		{
			if (headerSeparators.ContainsKey (name))
				return headerSeparators [name];

			return ",";
		}

		private async Task<NSUrlRequest> CreateRequest (HttpRequestMessage request)
		{
			var stream = Stream.Null;
			var headers = request.Headers as IEnumerable<KeyValuePair<string, IEnumerable<string>>>;

			if (request.Content != null) {
				stream = await request.Content.ReadAsStreamAsync ().ConfigureAwait (false);
				headers = headers.Union (request.Content.Headers).ToArray ();
			}

			var nsrequest = new NSMutableUrlRequest {
				AllowsCellularAccess = true,
				CachePolicy = NSUrlRequestCachePolicy.UseProtocolCachePolicy,
				HttpMethod = request.Method.ToString ().ToUpperInvariant (),
				Url = NSUrl.FromString (request.RequestUri.AbsoluteUri),
				Headers = headers.Aggregate (new NSMutableDictionary (), (acc, x) => {
					acc.Add (new NSString (x.Key), new NSString (string.Join (GetHeaderSeparator (x.Key), x.Value)));
					return acc;
				})
			};

			if (stream != Stream.Null)
				nsrequest.BodyStream = new WrappedNSInputStream (stream);

			return nsrequest;
		}

#if SYSTEM_NET_HTTP || MONOMAC
		internal
#endif
		protected override async Task<HttpResponseMessage> SendAsync (HttpRequestMessage request, CancellationToken cancellationToken)
		{
			var nsrequest = await CreateRequest (request);
			var dataTask = session.CreateDataTask (nsrequest);

			var tcs = new TaskCompletionSource<HttpResponseMessage> ();

			cancellationToken.Register (() => {
				RemoveInflightData (dataTask);
				tcs.TrySetCanceled ();
			});

			lock (inflightRequestsLock)
				inflightRequests.Add (dataTask, new InflightData {
					RequestUrl = request.RequestUri.AbsoluteUri,
					CompletionSource = tcs,
					CancellationToken = cancellationToken,
					Stream = new NSUrlSessionDataTaskStream (),
					Request = request
				});

			if (dataTask.State == NSUrlSessionTaskState.Suspended)
				dataTask.Resume ();

			return await tcs.Task.ConfigureAwait (false);
		}

#if MONOMAC
		// Needed since we strip during linking since we're inside a product assembly.
		[Preserve (AllMembers = true)]
#endif
		private class NSUrlSessionHandlerDelegate : NSUrlSessionDataDelegate
		{
			private readonly NSUrlSessionHandler sessionHandler;

			public NSUrlSessionHandlerDelegate (NSUrlSessionHandler handler)
			{
				sessionHandler = handler;
			}

			private InflightData GetInflightData (NSUrlSessionTask task)
			{
				var inflight = default (InflightData);

				lock (sessionHandler.inflightRequestsLock)
					if (sessionHandler.inflightRequests.TryGetValue (task, out inflight))
						return inflight;

				return null;
			}

			public override void DidReceiveResponse (NSUrlSession session, NSUrlSessionDataTask dataTask, NSUrlResponse response, Action<NSUrlSessionResponseDisposition> completionHandler)
			{
				var inflight = GetInflightData (dataTask);

				try {
					var urlResponse = (NSHttpUrlResponse)response;
					var status = (int)urlResponse.StatusCode;

					var content = new NSUrlSessionDataTaskStreamContent (inflight.Stream, () => {
						inflight.Disposed = true;
						inflight.Stream.TrySetException (new ObjectDisposedException ("The content stream was disposed."));

						sessionHandler.RemoveInflightData (dataTask);
					});

					// NB: The double cast is because of a Xamarin compiler bug
					var httpResponse = new HttpResponseMessage ((HttpStatusCode)status) {
						Content = content,
						RequestMessage = inflight.Request
					};
					httpResponse.RequestMessage.RequestUri = new Uri (urlResponse.Url.AbsoluteString);

					foreach (var v in urlResponse.AllHeaderFields) {
						// NB: Cocoa trolling us so hard by giving us back dummy dictionary entries
						if (v.Key == null || v.Value == null) continue;

						httpResponse.Headers.TryAddWithoutValidation (v.Key.ToString (), v.Value.ToString ());
						httpResponse.Content.Headers.TryAddWithoutValidation (v.Key.ToString (), v.Value.ToString ());
					}

					inflight.Response = httpResponse;

					// We don't want to send the response back to the task just yet.  Because we want to mimic .NET behavior
					// as much as possible.  When the response is sent back in .NET, the content stream is ready to read or the
					// request has completed, because of this we want to send back the response in DidReceiveData or DidCompleteWithError
					if (dataTask.State == NSUrlSessionTaskState.Suspended)
						dataTask.Resume ();

				} catch (Exception ex) {
					inflight.CompletionSource.TrySetException (ex);
					inflight.Stream.TrySetException (ex);

					sessionHandler.RemoveInflightData (dataTask);
				}

				completionHandler (NSUrlSessionResponseDisposition.Allow);
			}

			public override void DidReceiveData (NSUrlSession session, NSUrlSessionDataTask dataTask, NSData data)
			{
				var inflight = GetInflightData (dataTask);

				inflight.Stream.Add (data);
				SetResponse (inflight);
			}

			public override void DidCompleteWithError (NSUrlSession session, NSUrlSessionTask task, NSError error)
			{
				var inflight = GetInflightData (task);

				// this can happen if the HTTP request times out and it is removed as part of the cancelation process
				if (inflight != null) {
					// set the stream as finished
					inflight.Stream.TrySetReceivedAllData ();

					// send the error or send the response back
					if (error != null) {
						inflight.Errored = true;

						var exc = createExceptionForNSError (error);
						inflight.CompletionSource.TrySetException (exc);
						inflight.Stream.TrySetException (exc);
					} else {
						inflight.Completed = true;
						SetResponse (inflight);
					}

					sessionHandler.RemoveInflightData (task, cancel: false);
				}
			}

			private void SetResponse (InflightData inflight)
			{
				lock (inflight.Lock) {
					if (inflight.ResponseSent)
						return;

					if (inflight.CompletionSource.Task.IsCompleted)
						return;

					var httpResponse = inflight.Response;

					inflight.ResponseSent = true;

					// EVIL HACK: having TrySetResult inline was blocking the request from completing
					Task.Run (() => inflight.CompletionSource.TrySetResult (httpResponse));
				}
			}

			public override void WillCacheResponse (NSUrlSession session, NSUrlSessionDataTask dataTask, NSCachedUrlResponse proposedResponse, Action<NSCachedUrlResponse> completionHandler)
			{
				// never cache
				completionHandler (null);
			}

			public override void WillPerformHttpRedirection (NSUrlSession session, NSUrlSessionTask task, NSHttpUrlResponse response, NSUrlRequest newRequest, Action<NSUrlRequest> completionHandler)
			{
				// never redirect
				completionHandler (null);
			}
		}

#if MONOMAC
		// Needed since we strip during linking since we're inside a product assembly.
		[Preserve (AllMembers = true)]
#endif
		private class InflightData
		{
			public readonly object Lock = new object ();
			public string RequestUrl { get; set; }

			public TaskCompletionSource<HttpResponseMessage> CompletionSource { get; set; }
			public CancellationToken CancellationToken { get; set; }
			public NSUrlSessionDataTaskStream Stream { get; set; }
			public HttpRequestMessage Request { get; set; }
			public HttpResponseMessage Response { get; set; }

			public bool ResponseSent { get; set; }
			public bool Errored { get; set; }
			public bool Disposed { get; set; }
			public bool Completed { get; set; }
			public bool Done { get { return Errored || Disposed || Completed || CancellationToken.IsCancellationRequested; } }
		}

#if MONOMAC
		// Needed since we strip during linking since we're inside a product assembly.
		[Preserve (AllMembers = true)]
#endif
		private class NSUrlSessionDataTaskStreamContent : StreamContent
		{
			private Action disposed;

			public NSUrlSessionDataTaskStreamContent (NSUrlSessionDataTaskStream source, Action onDisposed) : base (source)
			{
				disposed = onDisposed;
			}

			protected override void Dispose (bool disposing)
			{
				var action = Interlocked.Exchange (ref disposed, null);
				action?.Invoke ();

				base.Dispose (disposing);
			}
		}

#if MONOMAC
		// Needed since we strip during linking since we're inside a product assembly.
		[Preserve (AllMembers = true)]
#endif
		private class NSUrlSessionDataTaskStream : Stream
		{
			private readonly Queue<NSData> data;
			private readonly object dataLock = new object ();

			private long position;
			private long length;

			private bool receivedAllData;
			private Exception exc;

			private NSData current;
			private Stream currentStream;

			public NSUrlSessionDataTaskStream ()
			{
				data = new Queue<NSData> ();
			}

			protected override void Dispose (bool disposing)
			{
				lock(dataLock) {
					foreach (var q in data)
						q?.Dispose ();
				}

				base.Dispose (disposing);
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

			private void ThrowIfNeeded (CancellationToken cancellationToken)
			{
				if (exc != null)
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

				while (current == null) {
					lock (dataLock) {
						if (data.Count == 0 && receivedAllData && position == length)
							return 0;

						if (data.Count > 0 && current == null) {
							current = data.Peek ();
							currentStream = current.AsStream ();
							break;
						}
					}

					await Task.Delay (50);
				}

				// try to throw again before read
				ThrowIfNeeded (cancellationToken);

				var d = currentStream;
				var bufferCount = Math.Min (count, (int)(d.Length - d.Position));
				var bytesRead = await d.ReadAsync (buffer, offset, bufferCount, cancellationToken);

				// add the bytes read from the pointer to the position
				position += bytesRead;

				// remove the current primary reference if the current position has reached the end of the bytes
				if (d.Position == d.Length) {
					lock (dataLock) {
						// this is the same object, it was done to make the cleanup
						data.Dequeue ();
						current?.Dispose ();
						currentStream?.Dispose ();
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

#if MONOMAC
		// Needed since we strip during linking since we're inside a product assembly.
		[Preserve (AllMembers = true)]
#endif
		private class WrappedNSInputStream : NSInputStream
		{
			private NSStreamStatus status;
			private CFRunLoopSource source;
			private readonly Stream stream;
			private bool notifying;

			public WrappedNSInputStream (Stream inputStream)
			{
				status = NSStreamStatus.NotOpen;
				stream = inputStream;
				source = new CFRunLoopSource (Handle);
			}

			public override NSStreamStatus Status => status;

			public override void Open ()
			{
				status = NSStreamStatus.Open;
				Notify (CFStreamEventType.OpenCompleted);
			}

			public override void Close ()
			{
				status = NSStreamStatus.Closed;
			}

			public override nint Read (IntPtr buffer, nuint len)
			{
				var sourceBytes = new byte [len];
				var read = stream.Read (sourceBytes, 0, (int)len);
				Marshal.Copy (sourceBytes, 0, buffer, (int)len);

				if (notifying)
					return read;

				notifying = true;
				if (stream.CanSeek && stream.Position == stream.Length) {
					Notify (CFStreamEventType.EndEncountered);
					status = NSStreamStatus.AtEnd;
				}
				notifying = false;

				return read;
			}

			public override bool HasBytesAvailable ()
			{
				return true;
			}

			protected override bool GetBuffer (out IntPtr buffer, out nuint len)
			{
				// Just call the base implemention (which will return false)
				return base.GetBuffer (out buffer, out len);
			}

			protected override bool SetCFClientFlags (CFStreamEventType inFlags, IntPtr inCallback, IntPtr inContextPtr)
			{
				// Just call the base implementation, which knows how to handle everything.
				return base.SetCFClientFlags (inFlags, inCallback, inContextPtr);
			}

			public override void Schedule (NSRunLoop aRunLoop, string mode)
			{
				var cfRunLoop = aRunLoop.GetCFRunLoop ();
				var nsMode = new NSString (mode);

				cfRunLoop.AddSource (source, nsMode);

				if (notifying)
					return;

				notifying = true;
				Notify (CFStreamEventType.HasBytesAvailable);
				notifying = false;
			}

			public override void Unschedule (NSRunLoop aRunLoop, string mode)
			{
				var cfRunLoop = aRunLoop.GetCFRunLoop ();
				var nsMode = new NSString (mode);

				cfRunLoop.RemoveSource (source, nsMode);
			}

			protected override void Dispose (bool disposing)
			{
				stream?.Dispose ();
			}
		}
	}
}