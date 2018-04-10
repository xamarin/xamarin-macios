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
		readonly Dictionary<string, string> headerSeparators = new Dictionary<string, string> {
			["User-Agent"] = " ",
			["Server"] = " "
		};

		readonly NSUrlSession session;
		readonly Dictionary<NSUrlSessionTask, InflightData> inflightRequests;
		readonly object inflightRequestsLock = new object ();

		public NSUrlSessionHandler () : this (NSUrlSessionConfiguration.DefaultSessionConfiguration)
		{
		}

		public NSUrlSessionHandler (NSUrlSessionConfiguration configuration)
		{
			if (configuration == null)
				throw new ArgumentNullException (nameof (configuration));

			AllowAutoRedirect = true;

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

			session = NSUrlSession.FromConfiguration (configuration, new NSUrlSessionHandlerDelegate (this), null);
			inflightRequests = new Dictionary<NSUrlSessionTask, InflightData> ();
		}

		public long MaxInputInMemory { get; set; } = long.MaxValue;

		void RemoveInflightData (NSUrlSessionTask task, bool cancel = true)
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

		bool disableCaching;

		public bool DisableCaching {
			get {
				return disableCaching;
			}
			set {
				EnsureModifiability ();
				disableCaching = value;
			}
		}

		string GetHeaderSeparator (string name)
		{
			string value;
			if (!headerSeparators.TryGetValue (name, out value))
				value = ",";
			return value;
		}

		async Task<NSUrlRequest> CreateRequest (HttpRequestMessage request)
		{
			var stream = Stream.Null;
			var headers = request.Headers as IEnumerable<KeyValuePair<string, IEnumerable<string>>>;

			if (request.Content != null) {
				stream = await request.Content.ReadAsStreamAsync ().ConfigureAwait (false);
				headers = headers.Union (request.Content.Headers).ToArray ();
			}

			var nsrequest = new NSMutableUrlRequest {
				AllowsCellularAccess = true,
				CachePolicy = DisableCaching ? NSUrlRequestCachePolicy.ReloadIgnoringCacheData : NSUrlRequestCachePolicy.UseProtocolCachePolicy,
				HttpMethod = request.Method.ToString ().ToUpperInvariant (),
				Url = NSUrl.FromString (request.RequestUri.AbsoluteUri),
				Headers = headers.Aggregate (new NSMutableDictionary (), (acc, x) => {
					acc.Add (new NSString (x.Key), new NSString (string.Join (GetHeaderSeparator (x.Key), x.Value)));
					return acc;
				})
			};
			if (stream != Stream.Null) {
				// HttpContent.TryComputeLength is `protected internal` :-( but it's indirectly called by headers
				var length = request.Content.Headers.ContentLength;
				if (length.HasValue && (length <= MaxInputInMemory))
					nsrequest.Body = NSData.FromStream (stream);
				else
					nsrequest.BodyStream = new WrappedNSInputStream (stream);
			}
			return nsrequest;
		}

#if SYSTEM_NET_HTTP || MONOMAC
		internal
#endif
		protected override async Task<HttpResponseMessage> SendAsync (HttpRequestMessage request, CancellationToken cancellationToken)
		{
			Volatile.Write (ref sentRequest, true);

			var nsrequest = await CreateRequest (request).ConfigureAwait(false);
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
		partial class NSUrlSessionHandlerDelegate : NSUrlSessionDataDelegate
		{
			readonly NSUrlSessionHandler sessionHandler;

			public NSUrlSessionHandlerDelegate (NSUrlSessionHandler handler)
			{
				sessionHandler = handler;
			}

			InflightData GetInflightData (NSUrlSessionTask task)
			{
				var inflight = default (InflightData);

				lock (sessionHandler.inflightRequestsLock)
					if (sessionHandler.inflightRequests.TryGetValue (task, out inflight)) {
						// ensure that we did not cancel the request, if we did, do cancel the task
						if (inflight.CancellationToken.IsCancellationRequested)
							task?.Cancel ();
						return inflight;
					}

				// if we did not manage to get the inflight data, we either got an error or have been canceled, lets cancel the task, that will execute DidCompleteWithError
				task?.Cancel ();
				return null;
			}

			public override void DidReceiveResponse (NSUrlSession session, NSUrlSessionDataTask dataTask, NSUrlResponse response, Action<NSUrlSessionResponseDisposition> completionHandler)
			{
				var inflight = GetInflightData (dataTask);

				if (inflight == null)
					return;

				try {
					var urlResponse = (NSHttpUrlResponse)response;
					var status = (int)urlResponse.StatusCode;

					var content = new NSUrlSessionDataTaskStreamContent (inflight.Stream, () => {
						if (!inflight.Completed) {
							dataTask.Cancel ();
						}

						inflight.Disposed = true;
						inflight.Stream.TrySetException (new ObjectDisposedException ("The content stream was disposed."));

						sessionHandler.RemoveInflightData (dataTask);
					}, inflight.CancellationToken);

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

				if (inflight == null)
					return;

				inflight.Stream.Add (data);
				SetResponse (inflight);
			}

			public override void DidCompleteWithError (NSUrlSession session, NSUrlSessionTask task, NSError error)
			{
				var inflight = GetInflightData (task);

				// this can happen if the HTTP request times out and it is removed as part of the cancellation process
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

			void SetResponse (InflightData inflight)
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
				completionHandler (sessionHandler.DisableCaching ? null : proposedResponse);
			}

			public override void WillPerformHttpRedirection (NSUrlSession session, NSUrlSessionTask task, NSHttpUrlResponse response, NSUrlRequest newRequest, Action<NSUrlRequest> completionHandler)
			{
				completionHandler (sessionHandler.AllowAutoRedirect ? newRequest : null);
			}

			public override void DidReceiveChallenge (NSUrlSession session, NSUrlSessionTask task, NSUrlAuthenticationChallenge challenge, Action<NSUrlSessionAuthChallengeDisposition, NSUrlCredential> completionHandler)
			{
				var inflight = GetInflightData (task);

				if (inflight == null)
					return;

				// case for the basic auth failing up front. As per apple documentation:
				// The URL Loading System is designed to handle various aspects of the HTTP protocol for you. As a result, you should not modify the following headers using
				// the addValue(_:forHTTPHeaderField:) or setValue(_:forHTTPHeaderField:) methods:
				// 	Authorization
				// 	Connection
				// 	Host
				// 	Proxy-Authenticate
				// 	Proxy-Authorization
				// 	WWW-Authenticate
				// but we are hiding such a situation from our users, we can nevertheless know if the header was added and deal with it. The idea is as follows,
				// check if we are in the first attempt, if we are (PreviousFailureCount == 0), we check the headers of the request and if we do have the Auth 
				// header, it means that we do not have the correct credentials, in any other case just do what it is expected.
				
				if (challenge.PreviousFailureCount == 0) {
					var authHeader = inflight.Request?.Headers?.Authorization;
					if (!(string.IsNullOrEmpty (authHeader?.Scheme) && string.IsNullOrEmpty (authHeader?.Parameter))) {
						completionHandler (NSUrlSessionAuthChallengeDisposition.RejectProtectionSpace, null);
						return;
					}
				}

				if (challenge.ProtectionSpace.AuthenticationMethod == NSUrlProtectionSpace.AuthenticationMethodNTLM && sessionHandler.Credentials != null) {
					var credentialsToUse = sessionHandler.Credentials as NetworkCredential;
					if (credentialsToUse == null) {
						var uri = inflight.Request.RequestUri;
						credentialsToUse = sessionHandler.Credentials.GetCredential (uri, "NTLM");
					}
					NSUrlCredential credential = new NSUrlCredential (credentialsToUse.UserName, credentialsToUse.Password, NSUrlCredentialPersistence.ForSession);
					completionHandler (NSUrlSessionAuthChallengeDisposition.UseCredential, credential);
				} else {
					completionHandler (NSUrlSessionAuthChallengeDisposition.PerformDefaultHandling, challenge.ProposedCredential);
				}
			}
		}

#if MONOMAC
		// Needed since we strip during linking since we're inside a product assembly.
		[Preserve (AllMembers = true)]
#endif
		class InflightData
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
		class NSUrlSessionDataTaskStreamContent : StreamContent
		{
			Action disposed;

			public NSUrlSessionDataTaskStreamContent (NSUrlSessionDataTaskStream source, Action onDisposed, CancellationToken token) : base (source, token)
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
		class NSUrlSessionDataTaskStream : Stream
		{
			readonly Queue<NSData> data;
			readonly object dataLock = new object ();

			long position;
			long length;

			bool receivedAllData;
			Exception exc;

			NSData current;
			Stream currentStream;

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

					await Task.Delay (50).ConfigureAwait (false);
				}

				// try to throw again before read
				ThrowIfNeeded (cancellationToken);

				var d = currentStream;
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

#if MONOMAC
		// Needed since we strip during linking since we're inside a product assembly.
		[Preserve (AllMembers = true)]
#endif
		class WrappedNSInputStream : NSInputStream
		{
			NSStreamStatus status;
			CFRunLoopSource source;
			readonly Stream stream;
			bool notifying;

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

			// NSInvalidArgumentException Reason: *** -propertyForKey: only defined for abstract class.  Define -[System_Net_Http_NSUrlSessionHandler_WrappedNSInputStream propertyForKey:]!
			protected override NSObject GetProperty (NSString key)
			{
				return null;
			}

			protected override bool SetProperty (NSObject property, NSString key)
			{
				return false;
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
