//
// MonoMac.CFNetwork.MessageHandler
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
using System.Net;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using CFNetwork;
using CoreFoundation;
using CoreServices;
using Foundation;

#nullable enable

namespace CFNetwork {

	[Obsolete ("Use 'System.Net.Http.CFNetworkHandler' or the more recent 'Foundation.NSUrlSessionHandler' instead.")]
	public class MessageHandler : HttpClientHandler {
		public MessageHandler ()
		{
		}

		public MessageHandler (WorkerThread worker)
		{
			WorkerThread = worker;
		}

		public WorkerThread? WorkerThread {
			get;
			private set;
		}

		/*
		 * CFNetwork supports two ways of authentication:
		 * 
		 * a) You send a normal request to the server and when it responds with
		 *    a 401 or 407, then you call CFHTTPMessageAddAuthentication on the
		 *    returned response CFHTTPMessage.  When the call succeeds, you resend
		 *    the request.
		 * 
		 * b) You do the same thing for the first request, but call
		 *    CFHTTPAuthenticationCreateFromResponse on the returned response to
		 *    get a CFHTTPAuthentication object which can persist multiple requests.
		 * 
		 *    On subsequent requests, you can then resue that object by calling
		 *    CFHTTPAuthenticationAppliesToRequest, CFHTTPAuthenticationIsValid and
		 *    (if both succeed) CFHTTPMessageApplyCredentials prior to sending the
		 *    request.
		 * 
		 */
		CFHTTPAuthentication? auth;

		#region implemented abstract members of HttpMessageHandler
#if MONOMAC && !NET
		internal
#endif
		protected override async Task<HttpResponseMessage> SendAsync (HttpRequestMessage request,
																	  CancellationToken cancellationToken)
		{
			if (!request.RequestUri.IsAbsoluteUri)
				throw new InvalidOperationException ();

			using (var message = CreateRequest (request, true)) {
				var body = await CreateBody (request, message, cancellationToken);
				return await ProcessRequest (request, message, body, true, cancellationToken, true);
			}
		}
		#endregion

		CFHTTPMessage CreateRequest (HttpRequestMessage request, bool isFirstRequest)
		{
			var message = CFHTTPMessage.CreateRequest (
				request.RequestUri, request.Method.Method, request.Version);

			SetupRequest (request, message, isFirstRequest);

			if ((auth is null) || (Credentials is null) || !PreAuthenticate)
				return message;

			if (!auth.AppliesToRequest (message))
				return message;

			var method = auth.GetMethod ();
			var credential = Credentials.GetCredential (request.RequestUri, method);
			if (credential is null)
				return message;

			if (isFirstRequest)
				message.ApplyCredentials (auth, credential);
			return message;
		}

		void SetupRequest (HttpRequestMessage request, CFHTTPMessage message, bool isFirstRequest)
		{
			string? accept_encoding = null;
			if ((AutomaticDecompression & DecompressionMethods.GZip) != 0)
				accept_encoding = "gzip";
			if ((AutomaticDecompression & DecompressionMethods.Deflate) != 0)
				accept_encoding = accept_encoding is not null ? "gzip, deflate" : "deflate";
			if (accept_encoding is not null)
				message.SetHeaderFieldValue ("Accept-Encoding", accept_encoding);

			if (request.Content is not null) {
				foreach (var header in request.Content.Headers) {
					if (!isFirstRequest && header.Key == "Authorization")
						continue;
					var value = string.Join (",", header.Value);
					message.SetHeaderFieldValue (header.Key, value);
				}
			}

			foreach (var header in request.Headers) {
				if ((accept_encoding is not null) && header.Key.Equals ("Accept-Encoding"))
					continue;
				var value = string.Join (",", header.Value);
				message.SetHeaderFieldValue (header.Key, value);
			}

			if (UseCookies && (CookieContainer is not null)) {
				string cookieHeader = CookieContainer.GetCookieHeader (request.RequestUri);
				if (cookieHeader != "")
					message.SetHeaderFieldValue ("Cookie", cookieHeader);
			}
		}

		async Task<WebRequestStream?> CreateBody (HttpRequestMessage request, CFHTTPMessage message,
												 CancellationToken cancellationToken)
		{
			if (request.Content is null)
				return null;

			/*
			 * There are two ways of sending the body:
			 * 
			 * - CFHTTPMessageSetBody() sets the full body contents
			 *   We use this by default.
			 *
			 * - CFReadStreamCreateForStreamedHTTPRequest() should be used
			 *   if the body is too large to fit in memory.  It also uses
			 *   chunked transfer encoding.
			 *
			 *   We use this if the user either gave us a StreamContent, or we
			 *   don't have any Content-Length, so we'll have to use chunked
			 *   transfer anyways.
			 *
			 */
			var length = request.Content.Headers.ContentLength;
			if ((request.Content is StreamContent) || (length is null)) {
				var stream = await request.Content.ReadAsStreamAsync ().ConfigureAwait (false);
				return new WebRequestStream (stream, cancellationToken);
			}

			var text = await request.Content.ReadAsByteArrayAsync ().ConfigureAwait (false);
			message.SetBody (text);
			return null;
		}

		bool GetKeepAlive (HttpRequestMessage request)
		{
			if (request.Version != HttpVersion.Version10)
				return request.Headers.ConnectionClose != true;

			foreach (var header in request.Headers.Connection) {
				if (string.Equals (header, "Keep-Alive", StringComparison.OrdinalIgnoreCase))
					return true;
			}

			return request.Headers.Contains ("Keep-Alive");
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

		async Task<HttpResponseMessage> ProcessRequest (HttpRequestMessage request,
														CFHTTPMessage message,
														WebRequestStream? body,
														bool retryWithCredentials,
														CancellationToken cancellationToken, bool isFirstRequest)
		{
			cancellationToken.ThrowIfCancellationRequested ();

			WebResponseStream? stream;
			if (body is not null)
				stream = WebResponseStream.Create (message, body);
			else
				stream = WebResponseStream.Create (message);
			if (stream is null)
				throw new HttpRequestException (string.Format (
					"Failed to create web request for '{0}'.",
					request.RequestUri)
				);

			if (stream.Stream is not null) {
				if (!isFirstRequest)
					stream.Stream.ShouldAutoredirect = AllowAutoRedirect;
				stream.Stream.AttemptPersistentConnection = GetKeepAlive (request);
			}

			var response = await stream.Open (
				WorkerThread!, cancellationToken).ConfigureAwait (true); // with false, we will have a deadlock.

			var status = (HttpStatusCode) response!.ResponseStatusCode;

			if (IsRedirect (status)) {
				request.Headers.Authorization = null;
				stream.Dispose ();
				// we cannot reuse the message, will deadlock and also the message.ApplyCredentials (auth, credential); 
				// was called the first time
				using (var retryMsg = CreateRequest (request, false)) {
					return await ProcessRequest (
						request, retryMsg, null, false, cancellationToken, false);
				}

			}
			if (retryWithCredentials && (body is null) &&
				(status == HttpStatusCode.Unauthorized) ||
				(status == HttpStatusCode.ProxyAuthenticationRequired)) {
				if (HandleAuthentication (request.RequestUri, message, response)) {
					stream.Dispose ();
					using (var retryMsg = CreateRequest (request, true)) { // behave as if it was the first attempt
						return await ProcessRequest (
							request, message, null, false, cancellationToken, true); // behave as if it was the first attempt
					}
				}
			}

			// The Content object takes ownership of the stream, so we don't
			// dispose it here.

			var retval = new HttpResponseMessage ();
			retval.StatusCode = response.ResponseStatusCode;
			retval.ReasonPhrase = GetReasonPhrase (response);
			retval.Version = response.Version;

			var content = new Content (stream);
			retval.Content = content;

			DecodeHeaders (response, retval, content);
			return retval;
		}

		string? GetReasonPhrase (CFHTTPMessage response)
		{
			var line = response.ResponseStatusLine;
			var match = Regex.Match (line, "HTTP/1.(0|1) (\\d+) (.*)");
			if (!match.Success)
				return line;

			return match.Groups [3].Value;
		}

		bool HandleAuthentication (Uri uri, CFHTTPMessage request, CFHTTPMessage response)
		{
			if (Credentials is null)
				return false;

			if (PreAuthenticate) {
				FindAuthenticationObject (response);
				return HandlePreAuthentication (uri, request);
			}

			var basic = Credentials.GetCredential (uri, "Basic");
			var digest = Credentials.GetCredential (uri, "Digest");

			bool ok = false;
			if ((basic is not null) && (digest is null))
				ok = HandleAuthentication (
					request, response, CFHTTPMessage.AuthenticationScheme.Basic, basic);
			if ((digest is not null) && (basic is null))
				ok = HandleAuthentication (
					request, response, CFHTTPMessage.AuthenticationScheme.Digest, digest);
			if (ok)
				return true;

			FindAuthenticationObject (response);
			return HandlePreAuthentication (uri, request);
		}

		bool HandlePreAuthentication (Uri uri, CFHTTPMessage message)
		{
			var method = auth?.GetMethod ();
			var credential = Credentials.GetCredential (uri, method);
			if (credential is null)
				return false;

			message.ApplyCredentials (auth, credential);
			return true;
		}

		bool HandleAuthentication (CFHTTPMessage request, CFHTTPMessage response,
								   CFHTTPMessage.AuthenticationScheme scheme,
								   NetworkCredential credential)
		{
			bool forProxy = response.ResponseStatusCode == HttpStatusCode.ProxyAuthenticationRequired;

			return request.AddAuthentication (
				response, (NSString) credential.UserName, (NSString) credential.Password,
				scheme, forProxy);
		}

		void FindAuthenticationObject (CFHTTPMessage response)
		{
			if (auth is not null) {
				if (auth.IsValid)
					return;
				auth.Dispose ();
				auth = null;
			}

			if (auth is null) {
				auth = CFHTTPAuthentication.CreateFromResponse (response);
				if (auth is null)
					throw new HttpRequestException ("Failed to create CFHTTPAuthentication");
			}

			if (!auth.IsValid)
				throw new HttpRequestException ("Failed to validate CFHTTPAuthentication");
		}

		void DecodeHeaders (CFHTTPMessage message, HttpResponseMessage response, Content content)
		{
			using var dict = message.GetAllHeaderFields ();
			if (dict is null)
				return;
			foreach (var entry in dict) {
				DecodeHeader (response, content, entry);
			}
		}

		void DecodeHeader (HttpResponseMessage response, Content content,
						   KeyValuePair<NSObject, NSObject> entry)
		{
			string key = (NSString) entry.Key;
			string value = (NSString) entry.Value;

			try {
				if (content.DecodeHeader (key, value))
					return;

				response.Headers.Add (key, value);
				return;
			} catch {
				;
			}

			/*
			 * FIXME: .NET automatically fixes an invalid date header
			 *        by setting it to the current time.  Mono does not.
			 */
			if (key.Equals ("Date"))
				response.Headers.Date = DateTime.Now;
		}

		protected override void Dispose (bool disposing)
		{
			if (disposing) {
				if (auth is not null) {
					auth.Dispose ();
					auth = null;
				}
			}

			base.Dispose (disposing);
		}
	}
}
#endif // !NET
