//
// NSUrlSessionHandler.cs:
//
// Authors:
//     Ani Betts <anais@anaisbetts.org>
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

using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.Diagnostics.CodeAnalysis;

using CoreFoundation;
using Security;

#if !MONOMAC
using UIKit;
#endif

#nullable enable

#if !MONOMAC && !XAMCORE_5_0
namespace System.Net.Http {
#else
namespace Foundation {
#endif

	public delegate bool NSUrlSessionHandlerTrustOverrideForUrlCallback (NSUrlSessionHandler sender, string url, SecTrust trust);

	// useful extensions for the class in order to set it in a header
	static class NSHttpCookieExtensions {
		static void AppendSegment (StringBuilder builder, string name, string? value)
		{
			if (builder.Length > 0)
				builder.Append ("; ");

			builder.Append (name);
			if (value is not null)
				builder.Append ("=").Append (value);
		}

		// returns the header for a cookie
		public static string GetHeaderValue (this NSHttpCookie cookie)
		{
			var header = new StringBuilder ();
			AppendSegment (header, cookie.Name, cookie.Value);
			AppendSegment (header, NSHttpCookie.KeyPath.ToString (), cookie.Path.ToString ());
			AppendSegment (header, NSHttpCookie.KeyDomain.ToString (), cookie.Domain.ToString ());
			AppendSegment (header, NSHttpCookie.KeyVersion.ToString (), cookie.Version.ToString ());

			if (cookie.Comment is not null)
				AppendSegment (header, NSHttpCookie.KeyComment.ToString (), cookie.Comment.ToString ());

			if (cookie.CommentUrl is not null)
				AppendSegment (header, NSHttpCookie.KeyCommentUrl.ToString (), cookie.CommentUrl.ToString ());

			if (cookie.Properties.ContainsKey (NSHttpCookie.KeyDiscard))
				AppendSegment (header, NSHttpCookie.KeyDiscard.ToString (), null);

			if (cookie.ExpiresDate is not null) {
				// Format according to RFC1123; 'r' uses invariant info (DateTimeFormatInfo.InvariantInfo)
				var dateStr = ((DateTime) cookie.ExpiresDate).ToUniversalTime ().ToString ("r", CultureInfo.InvariantCulture);
				AppendSegment (header, NSHttpCookie.KeyExpires.ToString (), dateStr);
			}

			var timeStampStringValue = cookie.Properties [NSHttpCookie.KeyMaximumAge];
			if (timeStampStringValue is NSString timeStampString)
				AppendSegment (header, NSHttpCookie.KeyMaximumAge.ToString (), timeStampString);

			if (cookie.IsSecure)
				AppendSegment (header, NSHttpCookie.KeySecure.ToString (), null);

			if (cookie.IsHttpOnly)
				AppendSegment (header, "httponly", null); // Apple does not show the key for the httponly

			return header.ToString ();
		}
	}

	/// <summary>To be added.</summary>
	///     <remarks>To be added.</remarks>
	public partial class NSUrlSessionHandler : HttpMessageHandler {
		private const string SetCookie = "Set-Cookie";
		private const string Cookie = "Cookie";
		private const string ContentEncodingHeaderName = "Content-Encoding";
		private const string ContentLengthHeaderName = "Content-Length";
		private CookieContainer? cookieContainer;
		readonly Dictionary<string, string> headerSeparators = new Dictionary<string, string> {
			["User-Agent"] = " ",
			["Server"] = " ",
		};

		NSUrlSession session;
		readonly Dictionary<NSUrlSessionTask, InflightData> inflightRequests;
		readonly object inflightRequestsLock = new object ();
		readonly NSUrlSessionConfiguration.SessionConfigurationType sessionType;
		X509ChainPolicy? policy;

		static NSUrlSessionConfiguration CreateConfig ()
		{
			// modifying the configuration does not affect future calls
			var config = NSUrlSessionConfiguration.DefaultSessionConfiguration;
			// but we want, by default, the timeout from HttpClient to have precedence over the one from NSUrlSession
			// Double.MaxValue does not work, so default to 24 hours
			config.TimeoutIntervalForRequest = 24 * 60 * 60;
			config.TimeoutIntervalForResource = 24 * 60 * 60;

			// Disable shared credential storage so credentials we pass with UseCredential in DidReceiveChallenge dont get saved in
			// the SharedCredentialStorage so (native) NSUrlSession can't try to authenticate later requests by itself using old credentials
			// incluiding redirects, and then our managed DidReceiveChallenge delegate may not get called at all. We already manage 
			// the credential flow in DidReceiveChallenge and the Credentials property. The switch is just a compat in case we
			// someone needs to go back to the old behaviour.
			var useSharedCredentialStorage = AppContext.TryGetSwitch ("Foundation.NSUrlSessionHandler.UseSharedCredentialStorage", out var useSharedStorage) && useSharedStorage;
			if (!useSharedCredentialStorage)
				config.URLCredentialStorage = null;
			return config;
		}

		/// <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		public NSUrlSessionHandler () : this (CreateConfig ())
		{
		}

		/// <param name="configuration">To be added.</param>
		///         <summary>To be added.</summary>
		///         <remarks>To be added.</remarks>
		[CLSCompliant (false)]
		public NSUrlSessionHandler (NSUrlSessionConfiguration configuration)
		{
			if (configuration is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (configuration));

			// HACK: we need to store the following because session.Configuration gets a copy of the object and the value gets lost
			sessionType = configuration.SessionType;
			allowsCellularAccess = configuration.AllowsCellularAccess;
			AllowAutoRedirect = true;

			session = NSUrlSession.FromConfiguration (configuration, (INSUrlSessionDelegate) new NSUrlSessionHandlerDelegate (this), null);
			inflightRequests = new Dictionary<NSUrlSessionTask, InflightData> ();
		}

		/// <summary>The maximum amount of content to load into memory when sending content with a request.</summary>
		/// <value>The maximum size of content to load into memory.</value>
		/// <remarks>
		///   <para>When sending content with a request, the content can be provided either in memory, or in a streaming manner.</para>
		///   <para>If the content is provided in memory, the underlying NSURLSession will set the Content-Length header to the size of the content.</para>
		///   <para>If the content is provided in a streaming manner, the underlying NSURLSession will send the content using a chunked encoding, and the Content-Length header will not be set.</para>
		///   <para>This means that if a chunked encoding is not desirable, or a Content-Length header is required, then the content must be provided in memory.</para>
		///   <para>On the other hand, if upload progress is needed, it's required to provide the content in a streaming manner, and this can be forced by setting this property to 0.</para>
		///   <para>If the content to upload doesn't have a pre-determined length, then it will always be sent in a streaming manner.</para>
		/// </remarks>
		public long MaxInputInMemory { get; set; } = long.MaxValue;

		void RemoveInflightData (NSUrlSessionTask task, bool cancel = true)
		{
			lock (inflightRequestsLock) {
				if (inflightRequests.Remove (task, out var data)) {
					if (cancel)
						data.CancellationTokenSource.Cancel ();
				}
			}

			if (cancel)
				task?.Cancel ();

			task?.Dispose ();
		}

		/// <inheritdoc />
		protected override void Dispose (bool disposing)
		{
			var tasks = new List<NSUrlSessionTask> ();
			lock (inflightRequestsLock) {
				tasks.AddRange (inflightRequests.Keys);
				inflightRequests.Clear ();
			}
			foreach (var task in tasks) {
				task.Cancel ();
				task.Dispose ();
			}

			// Take the proxy configuration lock so we don't race with ConfigureSessionProxy: either we
			// invalidate the session it created (if it ran first), or it observes 'disposed' and doesn't
			// create a new session that would never be invalidated (if we ran first).
			lock (proxyConfigurationLock) {
				disposed = true;
				session.InvalidateAndCancel ();
			}
			base.Dispose (disposing);
		}

		bool disableCaching;

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public bool DisableCaching {
			get {
				return disableCaching;
			}
			set {
				EnsureModifiability ();
				disableCaching = value;
			}
		}

		bool allowAutoRedirect;

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public bool AllowAutoRedirect {
			get {
				return allowAutoRedirect;
			}
			set {
				EnsureModifiability ();
				allowAutoRedirect = value;
			}
		}

		bool allowsCellularAccess = true;

		public bool AllowsCellularAccess {
			get {
				return allowsCellularAccess;
			}
			set {
				EnsureModifiability ();
				allowsCellularAccess = value;
			}
		}

		ICredentials? credentials;

		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public ICredentials? Credentials {
			get {
				return credentials;
			}
			set {
				EnsureModifiability ();
				credentials = value;
			}
		}

		NSUrlSessionHandlerTrustOverrideForUrlCallback? trustOverrideForUrl;

		public NSUrlSessionHandlerTrustOverrideForUrlCallback? TrustOverrideForUrl {
			get {
				return trustOverrideForUrl;
			}
			set {
				EnsureModifiability ();
				trustOverrideForUrl = value;
			}
		}

#if !XAMCORE_5_0
		[EditorBrowsable (EditorBrowsableState.Never)]
		[Obsolete ("This property is ignored.")]
		public bool BypassBackgroundSessionCheck {
			get {
				return true;
			}
			set {
			}
		}
#endif // !XAMCORE_5_0

		public CookieContainer? CookieContainer {
			get {
				return cookieContainer;
			}
			set {
				EnsureModifiability ();
				cookieContainer = value;
			}
		}

		/// <summary>Enable or disable the use of cookies.</summary>
		/// <remarks>
		///   <para>
		///       For default and background sessions, the shared cookie storage (<see cref="NSHttpCookieStorage.SharedStorage" /> will be used.
		///       This shared cookie storage will persist beyond app restarts; to clear the cookies call <c>NSHttpCookieStorage.SharedStorage.RemoveCookiesSinceDate(NSDate.DistantPast)</c>.
		///       To use a custom cookie storage, use a custom <see cref="NSUrlSessionConfiguration" />, set the <see cref="NSUrlSessionConfiguration.HttpCookieStorage" /> property, and then pass in the custom session configuration when creating the <see cref="NSUrlSessionHandler(NSUrlSessionConfiguration)" />.
		///   </para>
		///   <para>Ephemeral sessions have by default a private cookie storage area. This private cookie storage area can't be recreated, which means that if the use of cookies is disabled, then it can't be re-enabled.</para>
		/// </remarks>
		public bool UseCookies {
			get {
				return session.Configuration.HttpCookieStorage is not null;
			}
			set {
				EnsureModifiability ();

				// first check if anything changed
				if (value == UseCookies)
					return;

				// we have to consider the following table of cases:
				// 1. Value is set to true and cookie storage is not null -> we do nothing (already handled above)
				// 2. Value is set to true and cookie storage is null -> we create/set the storage.
				// 3. Value is false and cookie container is not null -> we clear the cookie storage
				// 4. Value is false and cookie container is null -> we do nothing (already handled above)
				var oldSession = session;
				var configuration = session.Configuration;
				if (value && configuration.HttpCookieStorage is null) {
					// create storage because the user wants to use it. Things are not that easy, we have to 
					// consider the following:
					// 1. Default Session -> uses sharedHTTPCookieStorage
					// 2. Background Session -> uses sharedHTTPCookieStorage
					// 3. Ephemeral Session -> we can't create an instance of a private cookie storage. Note that ephemeral sessions have a private cookie storage by default, so this can only happen if the developer disables cookies, and then re-enables them.
					if (sessionType == NSUrlSessionConfiguration.SessionConfigurationType.Ephemeral)
						throw new InvalidOperationException ("Can't re-enable the use of cookies in Ephemeral sessions.");

					configuration.HttpCookieStorage = NSHttpCookieStorage.SharedStorage;
				}
				if (!value && configuration.HttpCookieStorage is not null) {
					// remove storage so that it is not used in any of the requests
					configuration.HttpCookieStorage = null;
				}
				session = NSUrlSession.FromConfiguration (configuration, (INSUrlSessionDelegate) new NSUrlSessionHandlerDelegate (this), null);
				oldSession.Dispose ();
			}
		}

		bool sentRequest;

		internal void EnsureModifiability ()
		{
			if (sentRequest)
				throw new InvalidOperationException (
					"This instance has already started one or more requests. " +
					"Properties can only be modified before sending the first request.");
		}

		static Exception createExceptionForNSError (NSError error)
		{
			var innerException = new NSErrorException (error);

			// errors that exists in both share the same error code, so we can use a single switch/case
			// this also ease watchOS integration as if does not expose CFNetwork but (I would not be 
			// surprised if it)could return some of it's error codes
			if ((error.Domain == NSError.NSUrlErrorDomain) || (error.Domain == NSError.CFNetworkErrorDomain)) {
				// Apple docs: https://developer.apple.com/library/mac/documentation/Cocoa/Reference/Foundation/Miscellaneous/Foundation_Constants/index.html#//apple_ref/doc/constant_group/URL_Loading_System_Error_Codes
				// .NET docs: http://msdn.microsoft.com/en-us/library/system.net.webexceptionstatus(v=vs.110).aspx
				switch ((NSUrlError) (long) error.Code) {
				case NSUrlError.Cancelled:
				case NSUrlError.UserCancelledAuthentication:
				case (NSUrlError) NSNetServicesStatus.CancelledError:
					// No more processing is required so just return.
					return new OperationCanceledException (error.LocalizedDescription, innerException);
				}
			}

			return new HttpRequestException (error.LocalizedDescription, innerException);
		}

		string GetHeaderSeparator (string name)
		{
			if (!headerSeparators.TryGetValue (name, out var value))
				value = ",";
			return value;
		}

		void AddManagedHeaders (NSMutableDictionary nativeHeaders, IEnumerable<KeyValuePair<string, IEnumerable<string>>> managedHeaders)
		{
			foreach (var keyValuePair in managedHeaders) {
				var keyPtr = NSString.CreateNative (keyValuePair.Key);
				var valuePtr = NSString.CreateNative (string.Join (GetHeaderSeparator (keyValuePair.Key), keyValuePair.Value));
				nativeHeaders.LowlevelSetObject (valuePtr, keyPtr);
				NSString.ReleaseNative (keyPtr);
				NSString.ReleaseNative (valuePtr);
			}
		}

		async Task<NSUrlRequest> CreateRequest (HttpRequestMessage request)
		{
			var stream = Stream.Null;
			var nativeHeaders = new NSMutableDictionary ();
			// set header cookies if needed from the managed cookie container if we do use Cookies
			if (session.Configuration.HttpCookieStorage is not null) {
				var cookies = cookieContainer?.GetCookieHeader (request.RequestUri!); // as per docs: An HTTP cookie header, with strings representing Cookie instances delimited by semicolons.
				if (!string.IsNullOrEmpty (cookies)) {
					var cookiePtr = NSString.CreateNative (Cookie);
					var cookiesPtr = NSString.CreateNative (cookies);
					nativeHeaders.LowlevelSetObject (cookiesPtr, cookiePtr);
					NSString.ReleaseNative (cookiePtr);
					NSString.ReleaseNative (cookiesPtr);
				}
			}

			AddManagedHeaders (nativeHeaders, request.Headers);

			if (request.Content is not null) {
				stream = await request.Content.ReadAsStreamAsync ().ConfigureAwait (false);
				AddManagedHeaders (nativeHeaders, request.Content.Headers);
			}

			var nsrequest = new NSMutableUrlRequest {
				AllowsCellularAccess = allowsCellularAccess,
				CachePolicy = DisableCaching ? NSUrlRequestCachePolicy.ReloadIgnoringCacheData : NSUrlRequestCachePolicy.UseProtocolCachePolicy,
				HttpMethod = request.Method.ToString ().ToUpperInvariant (),
				Url = NSUrl.FromString (request.RequestUri?.AbsoluteUri),
				Headers = nativeHeaders,
			};

			if (stream != Stream.Null) {
				// Rewind the stream to the beginning in case the HttpContent implementation
				// will be accessed again (e.g. for retry/redirect) and it keeps its stream open behind the scenes.
				if (stream.CanSeek)
					stream.Seek (0, SeekOrigin.Begin);

				// HttpContent.TryComputeLength is `protected internal` :-( but it's indirectly called by headers
				var length = request.Content?.Headers?.ContentLength;
				if (length.HasValue && (length <= MaxInputInMemory))
					nsrequest.Body = NSData.FromStream (stream);
				else
					nsrequest.BodyStream = new WrappedNSInputStream (stream);
			}
			return nsrequest;
		}

		readonly object proxyConfigurationLock = new object ();
		bool proxyConfigured;
		bool disposed;

		// NSUrlSession applies proxy settings per-session (via the configuration's connection proxy dictionary),
		// not per-request, so we compute the proxy configuration once (using the first request's destination) and
		// recreate the session with it before the first request is sent.
		void ConfigureSessionProxy (HttpRequestMessage request)
		{
			lock (proxyConfigurationLock) {
				if (proxyConfigured)
					return;

				// The handler has been disposed (and the session already invalidated); don't create a new
				// session, since it would never be invalidated.
				if (disposed)
					return;

				if (TryGetProxyDictionary (request.RequestUri, out var proxyDictionary)) {
					var oldSession = session;
					var configuration = session.Configuration;
					configuration.ConnectionProxyDictionary = proxyDictionary;
					session = NSUrlSession.FromConfiguration (configuration, (INSUrlSessionDelegate) new NSUrlSessionHandlerDelegate (this), null);
					// A session keeps a strong reference to its delegate until it's invalidated, so
					// invalidate the old session to release its delegate (which references this handler).
					// No requests have been sent on the old session yet, so there's nothing to cancel.
					oldSession.InvalidateAndCancel ();
					oldSession.Dispose ();
				}

				// Only mark the proxy as configured once we've successfully computed and applied the
				// configuration, so that a failure (e.g. a throwing IWebProxy) doesn't permanently leave the
				// session unconfigured while pretending otherwise (the next request will try again).
				proxyConfigured = true;
			}
		}

		// Computes the connection proxy dictionary to apply to the session.
		// Returns false (and a null dictionary) when the session's default proxy behavior (use the OS-configured proxies) should be used.
		bool TryGetProxyDictionary (Uri? destination, out NSDictionary? proxyDictionary)
		{
			proxyDictionary = null;

			// The developer explicitly asked us to not use any proxy: apply an empty dictionary to
			// override any proxy configured in the OS.
			if (!useProxy) {
				proxyDictionary = new NSDictionary ();
				return true;
			}

			// No custom proxy: let NSUrlSession use the OS-configured proxies (the default behavior).
			if (proxy is null || destination is null)
				return false;

			// The proxy says this destination should not be proxied: override any OS-configured proxy.
			if (proxy.IsBypassed (destination)) {
				proxyDictionary = new NSDictionary ();
				return true;
			}

			var proxyUri = proxy.GetProxy (destination);
			// A null proxy uri (or one that points back at the destination) means "no proxy for this destination".
			if (proxyUri is null || proxyUri == destination) {
				proxyDictionary = new NSDictionary ();
				return true;
			}

			// The proxy uri's scheme indicates how to connect to the proxy itself (not which requests to
			// proxy). NSUrlSession's connection proxy dictionary can only describe a plain-HTTP connection
			// to the proxy (the HTTP/HTTPS proxy keys select the destination scheme, not the protocol used
			// to reach the proxy, and there's no key to connect to the proxy over TLS or via SOCKS). Rather
			// than silently connecting over plain HTTP and doing the wrong thing for a secure ("https") or
			// SOCKS proxy, fail loudly for any scheme we can't honor.
			if (!string.Equals (proxyUri.Scheme, Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase))
				throw new NotSupportedException ($"The proxy scheme '{proxyUri.Scheme}' is not supported. Only 'http' proxies are supported.");

			var strongProxy = new ProxyConfigurationDictionary {
				HttpEnable = true,
				HttpProxyHost = proxyUri.DnsSafeHost,
				HttpProxyPort = proxyUri.Port,
				HttpsProxyHost = proxyUri.DnsSafeHost,
				HttpsProxyPort = proxyUri.Port,
#if MONOMAC
				HttpsEnable = true,
#endif
			};
			proxyDictionary = strongProxy.GetDictionary ();
#if !MONOMAC
			// The strongly-typed HttpsEnable property (kCFNetworkProxiesHTTPSEnable) is only exposed on
			// macOS, but CFNetwork honors the same "HTTPSEnable" key on the other platforms too, and it's
			// required for HTTPS proxying (CONNECT tunneling) to work. Add it via the literal key.
			if (proxyDictionary is not null) {
				var mutableProxyDictionary = new NSMutableDictionary (proxyDictionary);
				mutableProxyDictionary ["HTTPSEnable"] = NSNumber.FromBoolean (true);
				proxyDictionary = mutableProxyDictionary;
			}
#endif
			return true;
		}

		/// <param name="request">To be added.</param>
		///         <param name="cancellationToken">To be added.</param>
		///         <summary>To be added.</summary>
		///         <returns>To be added.</returns>
		///         <remarks>To be added.</remarks>
		protected override async Task<HttpResponseMessage> SendAsync (HttpRequestMessage request, CancellationToken cancellationToken)
		{
			// Mark the handler as non-modifiable before any per-first-request initialization (such as
			// configuring the session proxy), so other threads can't mutate handler properties once the
			// first request has started being processed.
			Volatile.Write (ref sentRequest, true);

			ConfigureSessionProxy (request);

			var nsrequest = await CreateRequest (request).ConfigureAwait (false);
			var dataTask = session.CreateDataTask (nsrequest);

			var inflightData = new InflightData (request.RequestUri?.AbsoluteUri!, cancellationToken, request);

			lock (inflightRequestsLock) {
				inflightRequests.Add (dataTask, inflightData);
			}

			if (dataTask.State == NSUrlSessionTaskState.Suspended)
				dataTask.Resume ();

			// as per documentation: 
			// If this token is already in the canceled state, the 
			// delegate will be run immediately and synchronously.
			// Any exception the delegate generates will be 
			// propagated out of this method call.
			//
			// The execution of the register ensures that if we 
			// receive a already cancelled token or it is cancelled
			// just before this call, we will cancel the task. 
			// Other approaches are harder, since querying the state
			// of the token does not guarantee that in the next
			// execution a threads cancels it.
			cancellationToken.Register (() => {
				RemoveInflightData (dataTask);
				inflightData.CompletionSource.TrySetCanceled ();
			});

			return await inflightData.CompletionSource.Task.ConfigureAwait (false);
		}

		// Properties that will be called by the default HttpClientHandler

		// NSUrlSession handler automatically handles decompression, and there doesn't seem to be a way to turn it off.
		// The available decompression algorithms depend on the OS version we're running on, and maybe the target OS version as well,
		// so just say we're doing them all, and not do anything in the setter (it doesn't seem to be configurable in NSUrlSession anyways).
		public DecompressionMethods AutomaticDecompression {
			get => DecompressionMethods.All;
			set { }
		}

		/// <summary>Gets or sets a value that indicates whether the certificate is checked against the certificate authority revocation list.</summary>
		/// <remarks>
		///   <para>This is the same as setting CertificateChainPolicy.RevocationMode = X509RevocationMode.Online (if enabling the check) or X509RevocationMode.NoCheck (if disabling the check).</para>
		///   <para>This only has an effect if a custom server certificate validation callback is being used ('ServerCertificateCustomValidationCallback' is set).</para>
		/// </remarks>
		[EditorBrowsable (EditorBrowsableState.Never)]
		public bool CheckCertificateRevocationList {
			// This implementation was mostly copied from https://github.com/dotnet/runtime/blob/0e3562e97c6db531f26a2ffe3e8084cf67ba8a93/src/libraries/System.Net.Http/src/System/Net/Http/HttpClientHandler.cs#L326-L335
			get => CertificateChainPolicy!.RevocationMode == X509RevocationMode.Online;
			set {
				EnsureModifiability ();

				CertificateChainPolicy!.RevocationMode = value ? X509RevocationMode.Online : X509RevocationMode.NoCheck;
			}
		}

		/// <summary>Gets or sets the custom chain policy to use when validating certificate chains.</summary>
		/// <remarks>
		///   <para>The getter will never return a <see langword="null" /> policy, it will return a policy configured with the default behavior.</para>
		///   <para>To select the default policy, call the setter with <see langword="null" /> value.</para>
		///   <para>This only has an effect if a custom server certificate validation callback is being used ('ServerCertificateCustomValidationCallback' is set).</para>
		/// </remarks>
		public X509ChainPolicy? CertificateChainPolicy {
			get {
				if (policy is null) {
					policy = new X509ChainPolicy () {
						RevocationMode = X509RevocationMode.Online,
						RevocationFlag = X509RevocationFlag.ExcludeRoot,
						// Ignore unknown revocation status, because Apple has a bug where revocation checks fail if the certificate(s)
						// in question don't support revocation checking via OCSP.
						// References:
						// * https://learn.microsoft.com/en-us/dotnet/core/compatibility/networking/10.0/ssl-certificate-revocation-check-default
						// * https://github.com/dotnet/macios/issues/23764#issuecomment-3264999234
						// * https://github.com/dotnet/runtime/issues/117195
						VerificationFlags = X509VerificationFlags.IgnoreEndRevocationUnknown,
					};
				}
				return policy;
			}
			set => policy = value;
		}

		X509CertificateCollection? _clientCertificates;

		/// <summary>Gets the collection of security certificates that are associated with requests to the server.</summary>
		/// <remarks>Client certificates are only supported when ClientCertificateOptions is set to ClientCertificateOptions.Manual.</remarks>
		public X509CertificateCollection ClientCertificates {
			get {
				if (ClientCertificateOptions != ClientCertificateOption.Manual) {
					throw new InvalidOperationException ($"Enable manual options first on {nameof (ClientCertificateOptions)}");
				}

				return _clientCertificates ?? (_clientCertificates = new X509CertificateCollection ());
			}
		}

		public ClientCertificateOption ClientCertificateOptions { get; set; }

		ICredentials? defaultProxyCredentials;

		/// <summary>The credentials to submit to the proxy server for authentication.</summary>
		/// <value>The credentials to use to authenticate with the proxy, or <see langword="null" /> to not provide any proxy credentials.</value>
		/// <remarks>
		///   <para>These credentials are only used when the proxy itself (<see cref="Proxy" />) doesn't provide its own credentials.</para>
		///   <para>Proxy authentication is only supported for HTTPS destinations (which are proxied using a CONNECT tunnel). For plain HTTP destinations NSUrlSession doesn't deliver a proxy authentication challenge, so these credentials aren't applied and an authenticating proxy will return an HTTP 407 response instead.</para>
		/// </remarks>
		public ICredentials? DefaultProxyCredentials {
			get {
				return defaultProxyCredentials;
			}
			set {
				EnsureModifiability ();
				defaultProxyCredentials = value;
			}
		}

		public int MaxAutomaticRedirections {
			get => int.MaxValue;
			set {
				// I believe it's possible to implement support for MaxAutomaticRedirections (it just has to be done)
				if (value != int.MaxValue)
					ObjCRuntime.ThrowHelper.ThrowArgumentOutOfRangeException (nameof (value), value, "It's not possible to lower the max number of automatic redirections."); ;
			}
		}

		// We're ignoring this property, just like Xamarin.Android does:
		// https://github.com/xamarin/xamarin-android/blob/09e8cb5c07ea6c39383185a3f90e53186749b802/src/Mono.Android/Xamarin.Android.Net/AndroidMessageHandler.cs#L154
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("macos")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public int MaxConnectionsPerServer { get; set; } = int.MaxValue;

		// We're ignoring this property, just like Xamarin.Android does:
		// https://github.com/xamarin/xamarin-android/blob/09e8cb5c07ea6c39383185a3f90e53186749b802/src/Mono.Android/Xamarin.Android.Net/AndroidMessageHandler.cs#L156
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("macos")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public int MaxResponseHeadersLength { get; set; } = 64; // Units in K (1024) bytes.

		// We don't support PreAuthenticate, so always return false, and ignore any attempts to change it.
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("macos")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public bool PreAuthenticate {
			get => false;
			set { }
		}

		// We're ignoring this property, just like Xamarin.Android does:
		// https://github.com/xamarin/xamarin-android/blob/09e8cb5c07ea6c39383185a3f90e53186749b802/src/Mono.Android/Xamarin.Android.Net/AndroidMessageHandler.cs#L167
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("macos")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public IDictionary<string, object>? Properties { get { return null; } }

		IWebProxy? proxy;

		/// <summary>The proxy to use for requests.</summary>
		/// <value>The custom proxy to use, or <see langword="null" /> to use the proxies configured in the operating system.</value>
		/// <remarks>
		///   <para>Setting this property only has an effect if <see cref="UseProxy" /> is <see langword="true" /> (which is the default value).</para>
		///   <para>NSUrlSession applies proxy settings per-session, not per-request, so the proxy is evaluated only once, using the destination of the first request sent by this handler, and the result is applied to every subsequent request made by this handler.</para>
		///   <para>
		///     This has important consequences when a single handler is reused for requests to multiple destinations: the proxy's per-destination decisions (<see cref="IWebProxy.IsBypassed(System.Uri)" /> and the proxy returned by <see cref="IWebProxy.GetProxy(System.Uri)" />) are computed from the first request's destination only.
		///     As a result a proxy's bypass list and any destination-dependent proxy selection are not honored for later requests to other destinations: requests that should have been proxied might be sent directly (or through the wrong proxy), and requests that should have bypassed the proxy might be sent through it.
		///   </para>
		///   <para>If per-destination proxy behavior is required, use a separate <see cref="NSUrlSessionHandler" /> (and thus a separate <see cref="System.Net.Http.HttpClient" />) for each destination or proxy configuration.</para>
		///   <para>Proxy authentication (using <see cref="Proxy" />'s own credentials or <see cref="DefaultProxyCredentials" />) is only supported for HTTPS destinations (which are proxied using a CONNECT tunnel). For plain HTTP destinations NSUrlSession doesn't deliver a proxy authentication challenge, so proxy credentials aren't applied and an authenticating proxy will return an HTTP 407 response instead.</para>
		/// </remarks>
		public IWebProxy? Proxy {
			get {
				return proxy;
			}
			set {
				EnsureModifiability ();
				proxy = value;
			}
		}

		// There doesn't seem to be a trivial way to specify the protocols to accept (or not)
		// It might be possible to reject some protocols in code during the challenge phase,
		// but accepting earlier (unsafe) protocols requires adding entires to the Info.plist,
		// which means it's not trivial to detect/accept/reject from code here.
		// Currently the default for Apple platforms is to accept TLS v1.2 and v1.3, so default
		// to that value, and ignore any changes to it.
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("macos")]
		[EditorBrowsable (EditorBrowsableState.Never)]
		public SslProtocols SslProtocols { get; set; } = SslProtocols.Tls12 | SslProtocols.Tls13;

		private ServerCertificateCustomValidationCallbackHelper? _serverCertificateCustomValidationCallbackHelper;
		public Func<HttpRequestMessage, X509Certificate2?, X509Chain?, SslPolicyErrors, bool>? ServerCertificateCustomValidationCallback {
			get => _serverCertificateCustomValidationCallbackHelper?.Callback;
			set {
				if (value is null) {
					_serverCertificateCustomValidationCallbackHelper = null;
				} else {
					_serverCertificateCustomValidationCallbackHelper = new ServerCertificateCustomValidationCallbackHelper (value, CertificateChainPolicy!);
				}
			}
		}

		// returns false if there's no callback
		internal bool TryInvokeServerCertificateCustomValidationCallback (HttpRequestMessage request, SecTrust secTrust, out bool trusted)
		{
			trusted = false;
			var helper = _serverCertificateCustomValidationCallbackHelper;
			if (helper is null)
				return false;
			trusted = helper.Invoke (request, secTrust);
			return true;
		}

		sealed class ServerCertificateCustomValidationCallbackHelper {
			X509ChainPolicy policy;
			public Func<HttpRequestMessage, X509Certificate2?, X509Chain?, SslPolicyErrors, bool> Callback { get; private set; }

			public ServerCertificateCustomValidationCallbackHelper (Func<HttpRequestMessage, X509Certificate2?, X509Chain?, SslPolicyErrors, bool> callback, X509ChainPolicy policy)
			{
				Callback = callback;
				this.policy = policy;
			}

			public bool Invoke (HttpRequestMessage request, SecTrust secTrust)
			{
				var certificates = ConvertCertificates (secTrust);
				var certificate = certificates.Length > 0 ? certificates [0] : null;
				using X509Chain chain = CreateChain (certificates);
				SslPolicyErrors sslPolicyErrors = EvaluateSslPolicyErrors (certificate, chain, secTrust);

				return Callback (request, certificate, chain, sslPolicyErrors);
			}

			X509Certificate2 [] ConvertCertificates (SecTrust secTrust)
			{
				var certificates = new X509Certificate2 [secTrust.Count];

				if (SystemVersion.IsAtLeastXcode13) {
					var originalChain = secTrust.GetCertificateChain ();
					if (originalChain is null)
						return Array.Empty<X509Certificate2> ();
					for (int i = 0; i < originalChain.Length; i++)
						certificates [i] = originalChain [i].ToX509Certificate2 ();
				} else {
					for (int i = 0; i < secTrust.Count; i++) {
						// The analyzer has a bug where SupportedOSPlatformGuard attributes don't work correctly (https://github.com/dotnet/roslyn-analyzers/issues/7665#issuecomment-2898275765), so ignore CA1422 here
#pragma warning disable CA1422 // This call site is reachable on: 'ios' 12.2 and later, 'maccatalyst' 12.2 and later, 'macOS/OSX' 12.0 and later, 'tvos' 12.2 and later. 'SecTrust.this[nint]' is obsoleted on: 'ios' 15.0 and later
						certificates [i] = secTrust [i].ToX509Certificate2 ();
#pragma warning restore CA1422
					}
				}

				return certificates;
			}

			X509Chain CreateChain (X509Certificate2 [] certificates)
			{
				// See https://github.com/dotnet/macios/issues/23764 for more information.
				var chain = new X509Chain ();
				chain.ChainPolicy = policy.Clone ();
				chain.ChainPolicy.ExtraStore.AddRange (certificates);
				return chain;
			}

			SslPolicyErrors EvaluateSslPolicyErrors (X509Certificate2? certificate, X509Chain chain, SecTrust secTrust)
			{
				var sslPolicyErrors = SslPolicyErrors.None;

				try {
					if (certificate is null) {
						sslPolicyErrors |= SslPolicyErrors.RemoteCertificateNotAvailable;
					} else if (!chain.Build (certificate)) {
						sslPolicyErrors |= SslPolicyErrors.RemoteCertificateChainErrors;
					}
				} catch {
					sslPolicyErrors |= SslPolicyErrors.RemoteCertificateChainErrors;
				}

				if (!secTrust.Evaluate (out _)) {
					sslPolicyErrors |= SslPolicyErrors.RemoteCertificateChainErrors;
				}

				return sslPolicyErrors;
			}
		}

		// There's no way to turn off automatic decompression, so yes, we support it
		public bool SupportsAutomaticDecompression {
			get => true;
		}

		// We support custom proxies (applied per-session via the connection proxy dictionary), and
		// NSUrlSession will also automatically use any proxies configured in the OS.
		public bool SupportsProxy {
			get => true;
		}

		// We support the AllowAutoRedirect property, but we don't support changing the MaxAutomaticRedirections value,
		// so be safe here and say we don't support redirect configuration.
		public bool SupportsRedirectConfiguration {
			get => false;
		}

		bool useProxy = true;

		// When true (the default), NSUrlSession uses either the custom 'Proxy' (if set) or the proxies configured in the OS.
		// When false, no proxy is used (this overrides any proxy configured in the OS).
		public bool UseProxy {
			get => useProxy;
			set {
				EnsureModifiability ();
				useProxy = value;
			}
		}

		static bool HasCompressedEncoding (string headerValue)
		{
			foreach (var encoding in headerValue.Split (',')) {
				if (IsCompressedEncoding (encoding.Trim ()))
					return true;
			}
			return false;
		}

		static bool IsCompressedEncoding (string encoding)
		{
			return string.Equals (encoding, "gzip", StringComparison.OrdinalIgnoreCase)
				|| string.Equals (encoding, "deflate", StringComparison.OrdinalIgnoreCase)
				|| string.Equals (encoding, "br", StringComparison.OrdinalIgnoreCase)
				|| string.Equals (encoding, "compress", StringComparison.OrdinalIgnoreCase)
				|| string.Equals (encoding, "zstd", StringComparison.OrdinalIgnoreCase);
		}

		partial class NSUrlSessionHandlerDelegate : NSUrlSessionDataDelegate {
			readonly NSUrlSessionHandler sessionHandler;

			public NSUrlSessionHandlerDelegate (NSUrlSessionHandler handler)
			{
				sessionHandler = handler;
			}

			InflightData? GetInflightData (NSUrlSessionTask task)
			{
				var inflight = default (InflightData);

				lock (sessionHandler.inflightRequestsLock)
					if (sessionHandler.inflightRequests.TryGetValue (task, out inflight)) {
						// ensure that we did not cancel the request, if we did, do cancel the task, if we 
						// cancel the task it means that we are not interested in any of the delegate methods:
						// 
						// DidReceiveResponse     We might have received a response, but either the user cancelled or a 
						//                        timeout did, if that is the case, we do not care about the response.
						// DidReceiveData         Of buffer has a partial response ergo garbage and there is not real 
						//                        reason we would like to add more data.
						// DidCompleteWithError - We are not changing a behaviour compared to the case in which 
						//                        we did not find the data.
						if (inflight.CancellationToken.IsCancellationRequested) {
							task?.Cancel ();
							// return null so that we break out of any delegate method.
							return null;
						}
						return inflight;
					}

				// if we did not manage to get the inflight data, we either got an error or have been canceled, lets cancel the task, that will execute DidCompleteWithError
				task?.Cancel ();
				return null;
			}

			void UpdateManagedCookieContainer (Uri absoluteUri, NSHttpCookie [] cookies)
			{
				if (sessionHandler.cookieContainer is not null && cookies.Length > 0)
					lock (sessionHandler.inflightRequestsLock) { // ensure we lock when writing to the collection
						var cookiesContents = Array.ConvertAll (cookies, static cookie => cookie.GetHeaderValue ());
						sessionHandler.cookieContainer.SetCookies (absoluteUri, string.Join (',', cookiesContents)); //  as per docs: The contents of an HTTP set-cookie header as returned by a HTTP server, with Cookie instances delimited by commas.
					}
			}

			[Preserve (Conditional = true)]
			public override void DidReceiveResponse (NSUrlSession session, NSUrlSessionDataTask dataTask, NSUrlResponse response, Action<NSUrlSessionResponseDisposition> completionHandler)
			{
				try {
					DidReceiveResponseImpl (session, dataTask, response, completionHandler);
				} catch {
					completionHandler (NSUrlSessionResponseDisposition.Cancel);
					throw;
				}
			}

			void DidReceiveResponseImpl (NSUrlSession session, NSUrlSessionDataTask dataTask, NSUrlResponse response, Action<NSUrlSessionResponseDisposition> completionHandler)
			{
				var inflight = GetInflightData (dataTask);

				if (inflight is null) {
					completionHandler (NSUrlSessionResponseDisposition.Cancel);
					return;
				}

				try {
					var urlResponse = (NSHttpUrlResponse) response;
					var status = (int) urlResponse.StatusCode;
					var absoluteUri = new Uri (urlResponse.Url.AbsoluteString!);

					var content = new NSUrlSessionDataTaskStreamContent (inflight.Stream, () => {
						if (!inflight.Completed) {
							dataTask.Cancel ();
						}

						inflight.Disposed = true;
						inflight.Stream.TrySetException (new ObjectDisposedException ("The content stream was disposed."));

						sessionHandler.RemoveInflightData (dataTask);
					}, inflight.CancellationTokenSource.Token);

					// NB: The double cast is because of a Xamarin compiler bug
					var httpResponse = new HttpResponseMessage ((HttpStatusCode) status) {
						Content = content,
						RequestMessage = inflight.Request,
					};
					var wasRedirected = dataTask.CurrentRequest?.Url?.AbsoluteString != dataTask.OriginalRequest?.Url?.AbsoluteString;
					if (wasRedirected)
						httpResponse.RequestMessage.RequestUri = absoluteUri;

					// NSURLSession automatically decompresses content for all supported
					// encodings (gzip, deflate, br, zstd, etc.), and there's no way to
					// turn it off. After decompression, Content-Encoding and Content-Length
					// are stale (Content-Length refers to compressed size), so we need to
					// remove them to match the behavior of other HTTP handlers:
					// - SocketsHttpHandler:    https://github.com/dotnet/runtime/blob/b2974279efd059efaa17f359ed4b266b1c705721/src/libraries/System.Net.Http/src/System/Net/Http/SocketsHttpHandler/DecompressionHandler.cs#L122-L123
					// - AndroidMessageHandler: https://github.com/dotnet/android/pull/7785
					// Ref: https://github.com/dotnet/macios/issues/23958
					// This behavior can be opted out of by setting the
					// Foundation.NSUrlSessionHandler.KeepHeadersAfterDecompression switch.
					var keepHeaders = AppContext.TryGetSwitch ("Foundation.NSUrlSessionHandler.KeepHeadersAfterDecompression", out var keepHeadersEnabled) && keepHeadersEnabled;
					string? contentEncodingValue = null;
					string? contentLengthValue = null;

					foreach (var v in urlResponse.AllHeaderFields) {
						var key = v.Key?.ToString ();
						var value = v.Value?.ToString ();

						// NB: Cocoa trolling us so hard by giving us back dummy dictionary entries
						if (key is null || value is null) continue;
						// NSUrlSession tries to be smart with cookies, we will not use the raw value but the ones provided by the cookie storage
						if (key == SetCookie) continue;

						if (!keepHeaders && string.Equals (key, ContentEncodingHeaderName, StringComparison.OrdinalIgnoreCase)) {
							contentEncodingValue = value;
							continue;
						}
						if (!keepHeaders && string.Equals (key, ContentLengthHeaderName, StringComparison.OrdinalIgnoreCase)) {
							contentLengthValue = value;
							continue;
						}

						httpResponse.Headers.TryAddWithoutValidation (key, value);
						httpResponse.Content.Headers.TryAddWithoutValidation (key, value);
					}

					var contentWasDecompressed = contentEncodingValue is not null && HasCompressedEncoding (contentEncodingValue);
					if (!contentWasDecompressed) {
						if (contentEncodingValue is not null) {
							httpResponse.Headers.TryAddWithoutValidation (ContentEncodingHeaderName, contentEncodingValue);
							httpResponse.Content.Headers.TryAddWithoutValidation (ContentEncodingHeaderName, contentEncodingValue);
						}
						if (contentLengthValue is not null) {
							httpResponse.Headers.TryAddWithoutValidation (ContentLengthHeaderName, contentLengthValue);
							httpResponse.Content.Headers.TryAddWithoutValidation (ContentLengthHeaderName, contentLengthValue);
						}
					}

					// it might be confusing that we are not using the managed CookieStore here, this is ONLY for those cookies that have been retrieved from
					// the server via a Set-Cookie header, the managed container does not know a thing about this and apple is storing them in the native
					// cookie container. Once we have the cookies from the response, we need to update the managed cookie container
					if (session.Configuration.HttpCookieStorage is not null) {
						var cookies = session.Configuration.HttpCookieStorage.CookiesForUrl (response.Url);
						UpdateManagedCookieContainer (absoluteUri, cookies);
						for (var index = 0; index < cookies.Length; index++) {
							httpResponse.Headers.TryAddWithoutValidation (SetCookie, cookies [index].GetHeaderValue ());
						}
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

			[Preserve (Conditional = true)]
			public override void DidReceiveData (NSUrlSession session, NSUrlSessionDataTask dataTask, NSData data)
			{
				var inflight = GetInflightData (dataTask);

				if (inflight is null)
					return;

				inflight.Stream.Add (data);
				SetResponse (inflight);
			}

			[Preserve (Conditional = true)]
			public override void DidCompleteWithError (NSUrlSession session, NSUrlSessionTask task, NSError? error)
			{
				var inflight = GetInflightData (task);
				var serverError = task.Error;

				// this can happen if the HTTP request times out and it is removed as part of the cancellation process
				if (inflight is not null) {
					// send the error or send the response back
					if (error is not null || serverError is not null) {
						// got an error, cancel the stream operatios before we do anything
						inflight.CancellationTokenSource.Cancel ();
						inflight.Errored = true;

						var exc = inflight.Exception ?? createExceptionForNSError (error ?? serverError!);  // client errors wont happen if we get server errors
						inflight.CompletionSource.TrySetException (exc);
						inflight.Stream.TrySetException (exc);
					} else {
						// set the stream as finished
						inflight.Stream.TrySetReceivedAllData ();

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

					if (inflight.CancellationTokenSource.Token.IsCancellationRequested)
						return;

					if (inflight.CompletionSource.Task.IsCompleted)
						return;

					var httpResponse = inflight.Response;

					inflight.ResponseSent = true;

					inflight.CompletionSource.TrySetResult (httpResponse!);
				}
			}

			[Preserve (Conditional = true)]
			public override void WillCacheResponse (NSUrlSession session, NSUrlSessionDataTask dataTask, NSCachedUrlResponse proposedResponse, Action<NSCachedUrlResponse> completionHandler)
			{
				try {
					WillCacheResponseImpl (session, dataTask, proposedResponse, completionHandler);
				} catch {
					completionHandler (null!);
					throw;
				}
			}

			void WillCacheResponseImpl (NSUrlSession session, NSUrlSessionDataTask dataTask, NSCachedUrlResponse proposedResponse, Action<NSCachedUrlResponse> completionHandler)
			{
				var inflight = GetInflightData (dataTask);

				if (inflight is null) {
					completionHandler (null!);
					return;
				}

				// apple caches post request with a body, which should not happen. https://github.com/xamarin/maccore/issues/2571 
				var disableCache = sessionHandler.DisableCaching || (inflight.Request.Method == HttpMethod.Post && inflight.Request.Content is not null);
				completionHandler (disableCache ? null! : proposedResponse);
			}

			[Preserve (Conditional = true)]
			public override void WillPerformHttpRedirection (NSUrlSession session, NSUrlSessionTask task, NSHttpUrlResponse response, NSUrlRequest newRequest, Action<NSUrlRequest?> completionHandler)
			{
				if (!sessionHandler.AllowAutoRedirect) {
					completionHandler (null);
					return;
				}

				var inflight = GetInflightData (task);

				if (inflight is null) {
					completionHandler (null);
					return;
				}

				inflight.HasRedirected = true;

				if (newRequest.Url?.AbsoluteString is string redirectUrl) {
					inflight.CurrentRequestUrl = redirectUrl;

					if (Uri.TryCreate (redirectUrl, UriKind.Absolute, out var redirectUri))
						inflight.HasCrossOriginRedirect |= IsCrossOriginRedirect (inflight.RequestUrl, redirectUri);
					else
						inflight.HasCrossOriginRedirect = true;
				}

				completionHandler (newRequest);
			}

			[Preserve (Conditional = true)]
			public override void DidReceiveChallenge (NSUrlSession session, NSUrlSessionTask task, NSUrlAuthenticationChallenge challenge, Action<NSUrlSessionAuthChallengeDisposition, NSUrlCredential> completionHandler)
			{
				try {
					DidReceiveChallengeImpl (session, task, challenge, completionHandler);
				} catch {
					completionHandler (NSUrlSessionAuthChallengeDisposition.CancelAuthenticationChallenge, null!);
					throw;
				}
			}

			void DidReceiveChallengeImpl (NSUrlSession session, NSUrlSessionTask task, NSUrlAuthenticationChallenge challenge, Action<NSUrlSessionAuthChallengeDisposition, NSUrlCredential> completionHandler)
			{
				var inflight = GetInflightData (task);

				if (inflight is null) {
					// Request was probably cancelled
					completionHandler (NSUrlSessionAuthChallengeDisposition.CancelAuthenticationChallenge, null!);
					return;
				}

				// ToCToU for the callback
				var trustCallbackForUrl = sessionHandler.TrustOverrideForUrl;
				var trustSec = false;
				var usedCallback = false;
				if (challenge.ProtectionSpace.AuthenticationMethod == NSUrlProtectionSpace.AuthenticationMethodServerTrust) {
					// if the trust delegate allows to ignore the cert, do it. Since we are using nullables, if the delegate is not present, by default is false
					if (trustCallbackForUrl is not null) {
						trustSec = trustCallbackForUrl (sessionHandler, inflight.RequestUrl, challenge.ProtectionSpace.ServerSecTrust!);
						usedCallback = true;
					} else if (sessionHandler.TryInvokeServerCertificateCustomValidationCallback (inflight.Request, challenge.ProtectionSpace.ServerSecTrust!, out trustSec)) {
						usedCallback = true;
					}
				}

				if (usedCallback) {
					if (trustSec) {
						var credential = new NSUrlCredential (challenge.ProtectionSpace.ServerSecTrust!);
						completionHandler (NSUrlSessionAuthChallengeDisposition.UseCredential, credential);
					} else {
						// user callback rejected the certificate, we want to set the exception, else the user will
						// see as if the request was cancelled.
						lock (inflight.Lock) {
							inflight.Exception = new HttpRequestException ("An error occurred while sending the request.", new WebException ("Error: TrustFailure"));
						}
						completionHandler (NSUrlSessionAuthChallengeDisposition.CancelAuthenticationChallenge, null!);
					}
					return;
				}

				if (sessionHandler.ClientCertificateOptions == ClientCertificateOption.Manual && challenge.ProtectionSpace.AuthenticationMethod == NSUrlProtectionSpace.AuthenticationMethodClientCertificate) {
					var certificate = CertificateHelper.GetEligibleClientCertificate (sessionHandler.ClientCertificates);
					if (certificate is not null) {
						var cert = new SecCertificate (certificate);
						var identity = SecIdentity.Import (certificate);
						var credential = new NSUrlCredential (identity, new SecCertificate [] { cert }, NSUrlCredentialPersistence.ForSession);
						completionHandler (NSUrlSessionAuthChallengeDisposition.UseCredential, credential);
						return;
					} else if (!AppContext.TryGetSwitch ("Foundation.NSUrlSessionHandler.NoMissingCertificateHandling", out bool enabled) || !enabled) {
						// The server requested a certificate, but we don't have one to provide. Fail the request with a meaningful exception
						// that allows the developer to identify this, ask the user for a certificate, add it to the ClientCertificates collection
						// and then re-try the request.
						lock (inflight.Lock) {
							inflight.Exception = new HttpRequestException ("An error occurred while sending the request.",
								new WebException ("Error: Certificate Required",
									new AuthenticationException ("Error: Certificate Required"),
								WebExceptionStatus.SecureChannelFailure, null));
						}
						// We will still continue with a null credential, since some services use optional client certificates and this will still let it succeed
						completionHandler (NSUrlSessionAuthChallengeDisposition.PerformDefaultHandling, challenge.ProposedCredential);
						return;
					}
				}

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
				// This only applies to server authentication challenges: a proxy authentication challenge is unrelated to
				// the request's (server-targeted) Authorization header, so it's handled separately below.

				if (challenge.PreviousFailureCount == 0 && !challenge.ProtectionSpace.IsProxy) {
					var authHeader = inflight.Request.Headers?.Authorization;
					if (!(string.IsNullOrEmpty (authHeader?.Scheme) && string.IsNullOrEmpty (authHeader?.Parameter))) {
						completionHandler (NSUrlSessionAuthChallengeDisposition.RejectProtectionSpace, null!);
						return;
					}
				}

				// Detect redirect from the task as a fallback in case
				// WillPerformHttpRedirection has not updated infligth state yet
				if (!inflight.HasRedirected) {
					var originalUrl = task.OriginalRequest?.Url?.AbsoluteString;
					var currentUrl = task.CurrentRequest?.Url?.AbsoluteString;
					if (originalUrl is not null && currentUrl is not null
						&& !string.Equals (originalUrl, currentUrl, StringComparison.Ordinal)) {
						inflight.HasRedirected = true;
						inflight.CurrentRequestUrl = currentUrl;
						if (Uri.TryCreate (currentUrl, UriKind.Absolute, out var redirectUri))
							inflight.HasCrossOriginRedirect |= IsCrossOriginRedirect (inflight.RequestUrl, redirectUri);
						else
							inflight.HasCrossOriginRedirect = true;
					}
				}

				// Proxy authentication challenges are handled separately from server authentication challenges:
				// the credentials come from the proxy configuration (the proxy's own credentials or the
				// DefaultProxyCredentials) rather than from the handler's Credentials property.
				if (challenge.ProtectionSpace.IsProxy) {
					var proxy = sessionHandler.Proxy;
					var proxyCredentials = proxy?.Credentials ?? sessionHandler.DefaultProxyCredentials;
					// Only provide the credentials for the first challenge; if they were rejected we let the request
					// fail instead of retrying the same (bad) credentials indefinitely.
					if (proxyCredentials is not null && challenge.PreviousFailureCount == 0) {
						var proxyUri = GetProxyLookupUri (challenge.ProtectionSpace);
						// Look up the credentials using the proxy's authentication method (Basic/Digest/NTLM), so
						// that a CredentialCache keyed by a specific scheme resolves correctly. Fall back to "basic"
						// for any authentication method we don't recognize: a plain NetworkCredential ignores the
						// authentication type anyway, so this preserves the behavior for the common case.
						if (!TryGetAuthenticationType (challenge.ProtectionSpace, out var proxyAuthType) || proxyAuthType == RejectProtectionSpaceAuthType)
							proxyAuthType = "basic";
						var proxyCredential = proxyCredentials.GetCredential (proxyUri, proxyAuthType);
						if (proxyCredential is not null) {
							var proxyNSCredential = new NSUrlCredential (proxyCredential.UserName, proxyCredential.Password, NSUrlCredentialPersistence.ForSession);
							completionHandler (NSUrlSessionAuthChallengeDisposition.UseCredential, proxyNSCredential);
							return;
						}
					}
					completionHandler (NSUrlSessionAuthChallengeDisposition.PerformDefaultHandling, challenge.ProposedCredential);
					return;
				}

				var challengeCredentials = sessionHandler.Credentials;
				if (challengeCredentials is not null && TryGetAuthenticationType (challenge.ProtectionSpace, out var authType)) {
					NetworkCredential? credentialsToUse = null;
					if (authType != RejectProtectionSpaceAuthType) {
						// interesting situation, when we use a credential that we created that is empty, we are not getting the RejectProtectionSpaceAuthType,
						// nevertheless, we need to check is not the first challenge we will continue trusting the 
						// TryGetAuthenticationType method, but we will also check that the status response in not a 401
						// look like we do get an exception from the credentials db:
						//  TestiOSHttpClient[28769:26371051] CredStore - performQuery - Error copying matching creds.  Error=-25300, query={
						// class = inet;
						// "m_Limit" = "m_LimitAll";
						// ptcl = htps;
						// "r_Attributes" = 1;
						// sdmn = test;
						// srvr = "jigsaw.w3.org";
						// sync = syna;
						// }
						// do remember that we ALWAYS get a challenge, so the failure count has to be 1 or more for this check, 1 would be the first time
						var nsurlRespose = challenge.FailureResponse as NSHttpUrlResponse;
						var responseIsUnauthorized = (nsurlRespose is null) ? false : nsurlRespose.StatusCode == (int) HttpStatusCode.Unauthorized && challenge.PreviousFailureCount > 0;
						if (!responseIsUnauthorized) {
							var uri = GetCredentialLookupUri (task, inflight);
							if (ShouldLookupCredentials (challengeCredentials, inflight))
								credentialsToUse = challengeCredentials.GetCredential (uri, authType);
						}
					}

					if (credentialsToUse is not null) {
						var credential = new NSUrlCredential (credentialsToUse.UserName, credentialsToUse.Password, NSUrlCredentialPersistence.ForSession);
						completionHandler (NSUrlSessionAuthChallengeDisposition.UseCredential, credential);
					} else {
						// Rejecting the challenge allows the next authentication method in the request to be delivered to
						// the DidReceiveChallenge method. Another authentication method may have credentials available.
						completionHandler (NSUrlSessionAuthChallengeDisposition.RejectProtectionSpace, null!);
					}
				} else {
					completionHandler (NSUrlSessionAuthChallengeDisposition.PerformDefaultHandling, challenge.ProposedCredential);
				}
			}

			static Uri GetProxyLookupUri (NSUrlProtectionSpace protectionSpace)
			{
				// Derive the scheme from the proxy type (HTTP vs HTTPS proxy), not from ReceivesCredentialSecurely
				// (which describes whether the authentication method protects the credentials, not the proxy protocol).
				var scheme = protectionSpace.ProxyType == (string) NSUrlProtectionSpace.HTTPSProxy ? "https" : "http";
				var builder = new UriBuilder (scheme, protectionSpace.Host, (int) protectionSpace.Port);
				return builder.Uri;
			}

			static Uri GetCredentialLookupUri (NSUrlSessionTask task, InflightData inflight)
			{
				var currentRequestUrl = task.CurrentRequest?.Url?.AbsoluteString;
				if (currentRequestUrl is not null && Uri.TryCreate (currentRequestUrl, UriKind.Absolute, out var currentRequestUri))
					return currentRequestUri;

				if (Uri.TryCreate (inflight.CurrentRequestUrl, UriKind.Absolute, out var inflightCurrentRequestUri))
					return inflightCurrentRequestUri;

				return inflight.Request.RequestUri!;
			}

			static bool ShouldLookupCredentials (ICredentials credentials, InflightData inflight)
			{
				if (credentials is CredentialCache)
					return true;

				if (!inflight.HasRedirected)
					return true;

				// We are now matching .NET handlers (SocketsHttpHandler and WinHttpHandler) redirect behavior by dropping non CredentialCache credentials after a redirect
				// Ref:
				//	https://github.com/dotnet/runtime/blob/eb5503a1f0dc40ee7b73eb79a039eb143ee25038/src/libraries/System.Net.Http/src/System/Net/Http/SocketsHttpHandler/SocketsHttpHandler.cs#L541-L547
				//	https://github.com/dotnet/runtime/blob/eb5503a1f0dc40ee7b73eb79a039eb143ee25038/src/libraries/System.Net.Http/src/System/Net/Http/SocketsHttpHandler/RedirectHandler.cs#L52-L87
				// Provide a way for customers to opt into the old behavior for same origin redirects.
				var allowSameOriginRedirectCredentials = AppContext.TryGetSwitch ("Foundation.NSUrlSessionHandler.AllowSameOriginRedirectCredentials", out var allowRedirectCred) && allowRedirectCred;
				return allowSameOriginRedirectCredentials && !inflight.HasCrossOriginRedirect;
			}

			static bool IsCrossOriginRedirect (string originalRequestUrl, Uri currentRequestUri)
			{
				if (!Uri.TryCreate (originalRequestUrl, UriKind.Absolute, out var originalRequestUri))
					return true;

				return !string.Equals (originalRequestUri.Scheme, currentRequestUri.Scheme, StringComparison.OrdinalIgnoreCase)
					|| !string.Equals (originalRequestUri.IdnHost, currentRequestUri.IdnHost, StringComparison.OrdinalIgnoreCase)
					|| originalRequestUri.Port != currentRequestUri.Port;
			}

			static readonly string RejectProtectionSpaceAuthType = "reject";

			static bool TryGetAuthenticationType (NSUrlProtectionSpace protectionSpace, [NotNullWhen (true)] out string? authenticationType)
			{
				if (protectionSpace.AuthenticationMethod == NSUrlProtectionSpace.AuthenticationMethodNTLM) {
					authenticationType = "NTLM";
				} else if (protectionSpace.AuthenticationMethod == NSUrlProtectionSpace.AuthenticationMethodHTTPBasic) {
					authenticationType = "basic";
				} else if (protectionSpace.AuthenticationMethod == NSUrlProtectionSpace.AuthenticationMethodHTTPDigest) {
					authenticationType = "digest";
				} else if (protectionSpace.AuthenticationMethod == NSUrlProtectionSpace.AuthenticationMethodServerTrust ||
					protectionSpace.AuthenticationMethod == NSUrlProtectionSpace.AuthenticationMethodClientCertificate) {
					// ServerTrust and ClientCertificate are handled earlier in DidReceiveChallengeImpl,
					// so we should not reach here for these types. Return false just in case.
					authenticationType = null;
					return false;
				} else {
					// For any other authentication method (Negotiate, HTMLForm, Bearer, etc.),
					// reject this protection space to allow the next authentication method in the
					// request to be tried. This is important when the server advertises multiple
					// WWW-Authenticate challenges (e.g. Bearer before Basic) - rejecting unsupported
					// methods allows fallback to one we can handle.
					authenticationType = RejectProtectionSpaceAuthType;
				}
				return true;
			}
		}

		class InflightData {
			public readonly object Lock = new object ();
			public string RequestUrl { get; set; }
			public string CurrentRequestUrl { get; set; }
			public bool HasRedirected { get; set; }
			public bool HasCrossOriginRedirect { get; set; }

			public TaskCompletionSource<HttpResponseMessage> CompletionSource { get; } = new TaskCompletionSource<HttpResponseMessage> (TaskCreationOptions.RunContinuationsAsynchronously);
			public CancellationToken CancellationToken { get; set; }
			public CancellationTokenSource CancellationTokenSource { get; } = new CancellationTokenSource ();
			public NSUrlSessionDataTaskStream Stream { get; } = new NSUrlSessionDataTaskStream ();
			public HttpRequestMessage Request { get; set; }
			public HttpResponseMessage? Response { get; set; }

			public Exception? Exception { get; set; }
			public bool ResponseSent { get; set; }
			public bool Errored { get; set; }
			public bool Disposed { get; set; }
			public bool Completed { get; set; }
			public bool Done { get { return Errored || Disposed || Completed || CancellationToken.IsCancellationRequested; } }

			public InflightData (string requestUrl, CancellationToken cancellationToken, HttpRequestMessage request)
			{
				RequestUrl = requestUrl;
				CurrentRequestUrl = requestUrl;
				CancellationToken = cancellationToken;
				Request = request;
			}
		}

		class NSUrlSessionDataTaskStreamContent : MonoStreamContent {
			Action? disposed;

			public NSUrlSessionDataTaskStreamContent (NSUrlSessionDataTaskStream source, Action onDisposed, CancellationToken token) : base (source, token)
			{
				disposed = onDisposed;
			}

			/// <inheritdoc />
			protected override void Dispose (bool disposing)
			{
				var action = Interlocked.Exchange (ref disposed, null);
				action?.Invoke ();

				base.Dispose (disposing);
			}
		}

		//
		// Copied from https://github.com/mono/mono/blob/2019-02/mcs/class/System.Net.Http/System.Net.Http/StreamContent.cs.
		//
		// This is not a perfect solution, but the most robust and risk-free approach.
		//
		// The implementation depends on Mono-specific behavior, which makes SerializeToStreamAsync() cancellable.
		// Unfortunately, the CoreFX implementation of HttpClient does not support this.
		//
		// By copying Mono's old implementation here, we ensure that we're compatible with both HttpClient implementations,
		// so when we eventually adopt the CoreFX version in all of Mono's profiles, we don't regress here.
		//
		class MonoStreamContent : HttpContent {
			readonly Stream content;
			readonly int bufferSize;
			readonly CancellationToken cancellationToken;
			readonly long startPosition;
			bool contentCopied;

			public MonoStreamContent (Stream content)
				: this (content, 16 * 1024)
			{
			}

			public MonoStreamContent (Stream content, int bufferSize)
			{
				if (content is null)
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (content));

				if (bufferSize <= 0)
					ObjCRuntime.ThrowHelper.ThrowArgumentOutOfRangeException (nameof (bufferSize), bufferSize, "Buffer size must be >0");

				this.content = content;
				this.bufferSize = bufferSize;

				if (content.CanSeek) {
					startPosition = content.Position;
				}
			}

			//
			// Workarounds for poor .NET API
			// Instead of having SerializeToStreamAsync with CancellationToken as public API. Only LoadIntoBufferAsync
			// called internally from the send worker can be cancelled and user cannot see/do it
			//
			internal MonoStreamContent (Stream content, CancellationToken cancellationToken)
				: this (content)
			{
				// We don't own the token so don't worry about disposing it
				this.cancellationToken = cancellationToken;
			}

			protected override Task<Stream> CreateContentReadStreamAsync ()
			{
				return Task.FromResult (content);
			}

			/// <inheritdoc />
			protected override void Dispose (bool disposing)
			{
				if (disposing) {
					content.Dispose ();
				}

				base.Dispose (disposing);
			}

			protected override Task SerializeToStreamAsync (Stream stream, TransportContext? context)
			{
				if (contentCopied) {
					if (!content.CanSeek) {
						throw new InvalidOperationException ("The stream was already consumed. It cannot be read again.");
					}

					content.Seek (startPosition, SeekOrigin.Begin);
				} else {
					contentCopied = true;
				}

				return content.CopyToAsync (stream, bufferSize, cancellationToken);
			}

			protected override bool TryComputeLength (out long length)
			{
				if (!content.CanSeek) {
					length = 0;
					return false;
				}
				length = content.Length - startPosition;
				return true;
			}
		}

		class NSUrlSessionDataTaskStream : Stream {
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
					length += (int) d.Length;
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
						if (data.Count == 0 && receivedAllData && position == length) {
							ThrowIfNeeded (cancellationToken);
							return 0;
						}

						if (data.Count > 0 && current is null) {
							current = data.Peek ();
							currentStream = current.AsStream ();
							break;
						}
					}

					await Task.Delay (50, cancellationToken).ConfigureAwait (false);
				}

				// try to throw again before read
				ThrowIfNeeded (cancellationToken);

				var d = currentStream!;
				var bufferCount = Math.Min (count, (int) (d.Length - d.Position));
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

		class WrappedNSInputStream : NSInputStream {
			NSStreamStatus status;
			CFRunLoopSource source;
			readonly Stream stream;
			bool notifying;
			NSError? error;

			public WrappedNSInputStream (Stream inputStream)
			{
				status = NSStreamStatus.NotOpen;
				stream = inputStream;
				source = new CFRunLoopSource (Handle, false);
				IsDirectBinding = false;
			}

			[Preserve (Conditional = true)]
			public override NSStreamStatus Status => status;

			[Preserve (Conditional = true)]
			public override void Open ()
			{
				status = NSStreamStatus.Open;
				Notify (CFStreamEventType.OpenCompleted);
			}

			[Preserve (Conditional = true)]
			public override void Close ()
			{
				status = NSStreamStatus.Closed;
			}

			[Preserve (Conditional = true)]
			public override nint Read (IntPtr buffer, nuint len)
			{
				try {
					var sourceBytes = new byte [len];
					var read = stream.Read (sourceBytes, 0, (int) len);
					Marshal.Copy (sourceBytes, 0, buffer, (int) len);

					if (notifying)
						return read;

					notifying = true;
					if (stream.CanSeek && stream.Position == stream.Length) {
						Notify (CFStreamEventType.EndEncountered);
						status = NSStreamStatus.AtEnd;
					}
					notifying = false;

					return read;
				} catch (Exception e) {
					// -1 means that the operation failed; more information about the error can be obtained with streamError.
					error = new NSExceptionError (e);
					return -1;
				}
			}

			[Preserve (Conditional = true)]
			public override NSError Error {
				get {
					return error ?? base.Error;
				}
			}

			[Preserve (Conditional = true)]
			public override bool HasBytesAvailable ()
			{
				return true;
			}

			[Preserve (Conditional = true)]
			protected override bool GetBuffer (out IntPtr buffer, out nuint len)
			{
				// Just call the base implemention (which will return false)
				return base.GetBuffer (out buffer, out len);
			}

			// NSInvalidArgumentException Reason: *** -propertyForKey: only defined for abstract class.  Define -[System_Net_Http_NSUrlSessionHandler_WrappedNSInputStream propertyForKey:]!
			[Preserve (Conditional = true)]
			protected override NSObject? GetProperty (NSString key)
			{
				return null;
			}

			[Preserve (Conditional = true)]
			protected override bool SetProperty (NSObject? property, NSString key)
			{
				return false;
			}

			[Preserve (Conditional = true)]
			protected override bool SetCFClientFlags (CFStreamEventType inFlags, IntPtr inCallback, IntPtr inContextPtr)
			{
				// Just call the base implementation, which knows how to handle everything.
				return base.SetCFClientFlags (inFlags, inCallback, inContextPtr);
			}

			[Preserve (Conditional = true)]
			public override void Schedule (NSRunLoop aRunLoop, NSString nsMode)
			{
				var cfRunLoop = aRunLoop.GetCFRunLoop ();

				cfRunLoop.AddSource (source, nsMode);

				if (notifying)
					return;

				notifying = true;
				Notify (CFStreamEventType.HasBytesAvailable);
				notifying = false;
			}

			[Preserve (Conditional = true)]
			public override void Unschedule (NSRunLoop aRunLoop, NSString nsMode)
			{
				var cfRunLoop = aRunLoop.GetCFRunLoop ();
				cfRunLoop.RemoveSource (source, nsMode);
			}

			/// <inheritdoc />
			protected override void Dispose (bool disposing)
			{
				stream?.Dispose ();
			}
		}

		static class CertificateHelper {
			// Based on https://github.com/dotnet/runtime/blob/c2848c582f5d6ae42c89f5bfe0818687ab3345f0/src/libraries/Common/src/System/Net/Security/CertificateHelper.cs
			// with the NetEventSource code removed and namespace changed.
			const string ClientAuthenticationOID = "1.3.6.1.5.5.7.3.2";

			internal static X509Certificate2? GetEligibleClientCertificate (X509CertificateCollection? candidateCerts)
			{
				if (candidateCerts is null || candidateCerts.Count == 0) {
					return null;
				}

				var certs = new X509Certificate2Collection ();
				certs.AddRange (candidateCerts);

				return GetEligibleClientCertificate (certs);
			}

			internal static X509Certificate2? GetEligibleClientCertificate (X509Certificate2Collection? candidateCerts)
			{
				if (candidateCerts is null || candidateCerts.Count == 0) {
					return null;
				}

				foreach (X509Certificate2 cert in candidateCerts) {
					if (!cert.HasPrivateKey) {
						continue;
					}

					if (IsValidClientCertificate (cert)) {
						return cert;
					}
				}
				return null;
			}

			static bool IsValidClientCertificate (X509Certificate2 cert)
			{
				foreach (X509Extension extension in cert.Extensions) {
					if ((extension is X509EnhancedKeyUsageExtension eku) && !IsValidForClientAuthenticationEKU (eku)) {
						return false;
					} else if ((extension is X509KeyUsageExtension ku) && !IsValidForDigitalSignatureUsage (ku)) {
						return false;
					}
				}

				return true;
			}

			static bool IsValidForClientAuthenticationEKU (X509EnhancedKeyUsageExtension eku)
			{
				foreach (System.Security.Cryptography.Oid oid in eku.EnhancedKeyUsages) {
					if (oid.Value == ClientAuthenticationOID) {
						return true;
					}
				}

				return false;
			}

			static bool IsValidForDigitalSignatureUsage (X509KeyUsageExtension ku)
			{
				const X509KeyUsageFlags RequiredUsages = X509KeyUsageFlags.DigitalSignature;
				return (ku.KeyUsages & RequiredUsages) == RequiredUsages;
			}
		}
	}
}
