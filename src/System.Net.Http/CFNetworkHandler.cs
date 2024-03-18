//
// CFNetworkHandler.cs
//
// Authors:
//	Marek Safar  <marek.safar@gmail.com>
//
// Copyright (C) 2013 Xamarin Inc (http://www.xamarin.com)
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

using System.Threading;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;
using System.Net;

#if NET
using CFNetwork;
using CoreFoundation;
using CF=CoreFoundation;
#else
using CoreServices;
using CoreFoundation;
using CF = CoreFoundation;
#endif

// Disable until we get around to enable + fix any issues.
#nullable disable

namespace System.Net.Http {
	public class CFNetworkHandler : HttpMessageHandler {
		class StreamBucket {
			public TaskCompletionSource<HttpResponseMessage> Response;
			public HttpRequestMessage Request;
			public CancellationTokenRegistration CancellationTokenRegistration;
			public CFContentStream ContentStream;
			public bool StreamCanBeDisposed;

			public void Close ()
			{
				CancellationTokenRegistration.Dispose ();
				if (ContentStream is not null) {
					// The Close method of the CFContentStream blocks as you can see:
					// public void Close ()
					// {
					//	data_read_event.WaitOne (); // make sure there's no pending data
					//
					//	data_mutex.WaitOne ();
					//	data = null;
					//	this.http_stream.ErrorEvent -= HandleErrorEvent;
					//	data_mutex.ReleaseMutex ();
					//
					//	data_event.Set ();
					// }
					// This means. that when we want to ignore the data of the content, which happens
					// in the first request of a redirect, if we want to close, ignoring the content
					// we will have a deadlock.
					// Dispose will clean all the data, without waiting for the read, which by design in the
					// CFContentStream, blocks the thread. This happens ONLY after the first request that gets
					// a redirect status code. All other cases should call close.
					if (StreamCanBeDisposed)
						ContentStream.Dispose ();
					else
						ContentStream.Close ();
				}
			}
		}

		bool allowAutoRedirect;
		bool sentRequest;
		bool useSystemProxy;
		CookieContainer cookies;

		Dictionary<IntPtr, StreamBucket> streamBuckets;

		public CFNetworkHandler ()
		{
			allowAutoRedirect = true;
			streamBuckets = new Dictionary<IntPtr, StreamBucket> ();
		}

		void EnsureModifiability ()
		{
			if (sentRequest)
				throw new InvalidOperationException (
					"This instance has already started one or more requests. " +
					"Properties can only be modified before sending the first request.");
		}

		public bool AllowAutoRedirect {
			get {
				return allowAutoRedirect;
			}
			set {
				EnsureModifiability ();
				allowAutoRedirect = value;
			}
		}

		public CookieContainer CookieContainer {
			get {
				return cookies ?? (cookies = new CookieContainer ());
			}
			set {
				EnsureModifiability ();
				cookies = value;
			}
		}

		public bool UseSystemProxy {
			get {
				return useSystemProxy;
			}
			set {
				EnsureModifiability ();
				useSystemProxy = value;
			}
		}

		// TODO: Add more properties

		protected override void Dispose (bool disposing)
		{
			// TODO: CloseStream remaining stream buckets if there are any

			base.Dispose (disposing);
		}

		CFHTTPMessage CreateWebRequestAsync (HttpRequestMessage request)
		{
			var req = CFHTTPMessage.CreateRequest (request.RequestUri, request.Method.Method, request.Version);

			// TODO:
			/*
						if (wr.ProtocolVersion == HttpVersion.Version10) {
							wr.KeepAlive = request.Headers.ConnectionKeepAlive;
						} else {
							wr.KeepAlive = request.Headers.ConnectionClose != true;
						}

						if (useDefaultCredentials) {
							wr.UseDefaultCredentials = true;
						} else {
							wr.Credentials = credentials;
						}

						if (useProxy) {
							wr.Proxy = proxy;
						}
			*/
			if (cookies is not null) {
				string cookieHeader = cookies.GetCookieHeader (request.RequestUri);
				if (cookieHeader != "")
					req.SetHeaderFieldValue ("Cookie", cookieHeader);
			}

			foreach (var header in request.Headers) {
				foreach (var value in header.Value) {
					req.SetHeaderFieldValue (header.Key, value);
				}
			}

			if (request.Content is not null) {
				foreach (var header in request.Content.Headers) {
					foreach (var value in header.Value) {
						req.SetHeaderFieldValue (header.Key, value);
					}
				}
			}

			return req;
		}

#if !NET
		internal
#endif
		protected override async Task<HttpResponseMessage> SendAsync (HttpRequestMessage request, CancellationToken cancellationToken)
		{
			return await SendAsync (request, cancellationToken, true).ConfigureAwait (false);
		}

		internal async Task<HttpResponseMessage> SendAsync (HttpRequestMessage request, CancellationToken cancellationToken, bool isFirstRequest)
		{
			sentRequest = true;

			CFHTTPStream stream;
			using (var message = CreateWebRequestAsync (request)) {
				if (request.Content is not null) {
					var data = await request.Content.ReadAsByteArrayAsync ().ConfigureAwait (false);
					message.SetBody (data);
				}

				stream = CFHTTPStream.CreateForHTTPRequest (message);
			}

			if (useSystemProxy) {
				var proxies = CF.CFNetwork.GetSystemProxySettings ();
				if (proxies.HTTPEnable) {
					stream.SetProxy (proxies);
				}
			}

			if (!isFirstRequest && allowAutoRedirect)
				stream.ShouldAutoredirect = allowAutoRedirect;
			stream.HasBytesAvailableEvent += HandleHasBytesAvailableEvent;
			stream.ErrorEvent += HandleErrorEvent;
			stream.ClosedEvent += HandleClosedEvent;

			var response = new TaskCompletionSource<HttpResponseMessage> ();

			if (cancellationToken.IsCancellationRequested) {
				response.SetCanceled ();
				return await response.Task;
			}

			var bucket = new StreamBucket () {
				Request = request,
				Response = response,
			};

			streamBuckets.Add (stream.Handle, bucket);

			//
			// Always schedule stream events handling on main-loop. Due to ConfigureAwait (false) we may end up
			// on any thread-pool thread which may not have run-loop running
			//
			stream.EnableEvents (CF.CFRunLoop.Main, CF.CFRunLoop.ModeCommon);

			stream.Open ();

			bucket.CancellationTokenRegistration = cancellationToken.Register (() => {
				StreamBucket bucket2;
				if (!streamBuckets.TryGetValue (stream.Handle, out bucket2))
					return;

				bucket2.Response.TrySetCanceled ();
				CloseStream (stream);
			});

			if (isFirstRequest) {
				var initialRequest = await response.Task;
				var status = initialRequest.StatusCode;
				if (IsRedirect (status) && allowAutoRedirect) {
					bucket.StreamCanBeDisposed = true;
					// we do not care about the first stream cbs
					stream.HasBytesAvailableEvent -= HandleHasBytesAvailableEvent;
					stream.ErrorEvent -= HandleErrorEvent;
					stream.ClosedEvent -= HandleClosedEvent;
					// remove headers in a redirect for Authentication.
					request.Headers.Authorization = null;
					return await SendAsync (request, cancellationToken, false).ConfigureAwait (false);
				}
				return initialRequest;
			}
			return await response.Task;
		}

		// Decide if we redirect or not, similar to what is done in the managed handler
		// https://github.com/mono/mono/blob/eca15996c7163f331c9f2cd0a17b63e8f92b1d55/mcs/class/referencesource/System/net/System/Net/HttpWebRequest.cs#L5681
		static bool IsRedirect (HttpStatusCode status)
		{
			return status == HttpStatusCode.Ambiguous || // 300
				status == HttpStatusCode.Moved || // 301
				status == HttpStatusCode.Redirect || // 302
				status == HttpStatusCode.RedirectMethod || // 303
				status == HttpStatusCode.RedirectKeepVerb; // 307
		}

		void HandleErrorEvent (object sender, CFStream.StreamEventArgs e)
		{
			var stream = (CFHTTPStream) sender;

			StreamBucket bucket;
			if (!streamBuckets.TryGetValue (stream.Handle, out bucket))
				return;

			var ex = stream.GetError ();
			bucket.Response.TrySetException (new HttpRequestException (ex.FailureReason, ex));
			CloseStream (stream);
		}

		void HandleClosedEvent (object sender, CFStream.StreamEventArgs e)
		{
			var stream = (CFHTTPStream) sender;
			// might not have been called (e.g. no data) but initialize critical data
			HandleHasBytesAvailableEvent (sender, e);
			CloseStream (stream);
		}

		void CloseStream (CFHTTPStream stream)
		{
			lock (streamBuckets) {
				if (streamBuckets.TryGetValue (stream.Handle, out var bucket)) {
					bucket.Close ();
					streamBuckets.Remove (stream.Handle);
				}
			}
			stream.Close ();
		}

		void HandleHasBytesAvailableEvent (object sender, CFStream.StreamEventArgs e)
		{
			var stream = (CFHTTPStream) sender;

			StreamBucket bucket;
			if (!streamBuckets.TryGetValue (stream.Handle, out bucket))
				return;

			if (bucket.Response.Task.IsCompleted) {
				bucket.ContentStream.ReadStreamData ();
				return;
			}

			var header = stream.GetResponseHeader ();

			// Is this possible?
			if (!header.IsHeaderComplete)
				throw new NotImplementedException ();

			bucket.ContentStream = new CFContentStream (stream);

			var response_msg = new HttpResponseMessage (header.ResponseStatusCode);
			response_msg.RequestMessage = bucket.Request;
			response_msg.ReasonPhrase = header.ResponseStatusLine;
			response_msg.Content = bucket.ContentStream;

			var fields = header.GetAllHeaderFields ();
			if (fields is not null) {
				foreach (var entry in fields) {
					if (entry.Key is null)
						continue;

					var key = entry.Key.ToString ();
					var value = entry.Value is null ? string.Empty : entry.Value.ToString ();
					HttpHeaders item_headers;
					if (IsContentHeader (key)) {
						item_headers = response_msg.Content.Headers;
					} else {
						item_headers = response_msg.Headers;

						if (cookies is not null && (key == "Set-Cookie" || key == "Set-Cookie2"))
							AddCookie (value, bucket.Request.RequestUri, key);
					}

					item_headers.TryAddWithoutValidation (key, value);
				}
			}

			// cancellation (CancellationTokenRegistration) can happen in parallel
			if (!bucket.Response.Task.IsCanceled) {
				bucket.Response.TrySetResult (response_msg);

				bucket.ContentStream.ReadStreamData ();
			}
		}

		void AddCookie (string value, Uri uri, string header)
		{
#if NET
			// .NET: CookieCollection.CookieCutter is internal to mscorlib:
			// https://github.com/microsoft/referencesource/blob/a7bd3242bd7732dec4aebb21fbc0f6de61c2545e/System/net/System/Net/cookiecontainer.cs#L632
			// https://github.com/xamarin/xamarin-macios/issues/8072
			// so use the public CookieContainer.SetCookies instead.
			try {
				cookies.SetCookies (uri, value);
			} catch {
			}
#else
			CookieCollection cookies1 = null;
			try {
				cookies1 = cookies.CookieCutter (uri, header, value, false);
			} catch {
			}

			if (cookies1 is not null && cookies1.Count != 0)
				cookies.Add (cookies1);
#endif
		}

		static bool IsContentHeader (string header)
		{
			// The headers here which have HttpHeaderType.Content: https://github.com/dotnet/runtime/blob/e9853d4baa4c9510dc62ed5852f8381141f3c87e/src/libraries/System.Net.Http/src/System/Net/Http/Headers/KnownHeaders.cs#L15-L101
			switch (header) {
			case "Allow":
			case "ContentDisposition":
			case "ContentEncoding":
			case "ContentLanguage":
			case "ContentLength":
			case "ContentLocation":
			case "ContentMD5":
			case "ContentRange":
			case "ContentType":
			case "Expires":
			case "LastModified":
				return true;
			}
			return false;
		}
	}
}
