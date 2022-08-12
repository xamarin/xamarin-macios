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

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Runtime.InteropServices;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.Diagnostics.CodeAnalysis;

using CoreFoundation;
using Foundation;
using Security;

#if !MONOMAC
using UIKit;
#endif

#nullable enable

#if !MONOMAC
namespace System.Net.Http {
#else
namespace Foundation {
#endif

#if !NET
	public delegate bool NSUrlSessionHandlerTrustOverrideCallback (NSUrlSessionHandler sender, SecTrust trust);
#endif
	public delegate bool NSUrlSessionHandlerTrustOverrideForUrlCallback (NSUrlSessionHandler sender, string url, SecTrust trust);

	public partial class NSUrlSessionHandler : HttpMessageHandler
	{
		const string Cookie = "Cookie";
		NSUrlSession session;
		readonly NSUrlSessionConfiguration.SessionConfigurationType sessionType;
		readonly Dictionary<string, string> headerSeparators = new Dictionary<string, string> {
			["User-Agent"] = " ",
			["Server"] = " "
		};

		internal CookieContainer? cookieContainer;
		internal readonly NSUrlSessionHandlerInflightData inflightRequests;

		static NSUrlSessionConfiguration CreateConfig ()
		{
			// modifying the configuration does not affect future calls
			var config = NSUrlSessionConfiguration.DefaultSessionConfiguration;
			// but we want, by default, the timeout from HttpClient to have precedence over the one from NSUrlSession
			// Double.MaxValue does not work, so default to 24 hours
			config.TimeoutIntervalForRequest = 24 * 60 * 60;
			config.TimeoutIntervalForResource = 24 * 60 * 60;
			return config;
		}

		public NSUrlSessionHandler () : this (CreateConfig ())
		{
		}

		[CLSCompliant (false)]
		public NSUrlSessionHandler (NSUrlSessionConfiguration configuration)
		{
			if (configuration is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (configuration));

			// HACK: we need to store the following because session.Configuration gets a copy of the object and the value gets lost
			sessionType = configuration.SessionType;
			allowsCellularAccess = configuration.AllowsCellularAccess;
			AllowAutoRedirect = true;

			var sp = ServicePointManager.SecurityProtocol;
			if ((sp & SecurityProtocolType.Ssl3) != 0)
				configuration.TLSMinimumSupportedProtocol = SslProtocol.Ssl_3_0;
			else if ((sp & SecurityProtocolType.Tls) != 0)
				configuration.TLSMinimumSupportedProtocol = SslProtocol.Tls_1_0;
			else if ((sp & SecurityProtocolType.Tls11) != 0)
				configuration.TLSMinimumSupportedProtocol = SslProtocol.Tls_1_1;
			else if ((sp & SecurityProtocolType.Tls12) != 0)
				configuration.TLSMinimumSupportedProtocol = SslProtocol.Tls_1_2;
			else if ((sp & (SecurityProtocolType) 12288) != 0) // Tls13 value not yet in monno
				configuration.TLSMinimumSupportedProtocol = SslProtocol.Tls_1_3;

			session = NSUrlSession.FromConfiguration (configuration, (INSUrlSessionDelegate) new NSUrlSessionHandlerDelegate (this), null);
			inflightRequests = new (this);
		}

#if !MONOMAC  && !__WATCHOS__

		NSObject? notificationToken;  // needed to make sure we do not hang if not using a background session
		readonly object notificationTokenLock = new object (); // need to make sure that threads do no step on each other with a dispose and a remove  inflight data

		internal void AddNotification ()
		{
			lock (notificationTokenLock) {
				if (!bypassBackgroundCheck && sessionType != NSUrlSessionConfiguration.SessionConfigurationType.Background && notificationToken is null)
					notificationToken = NSNotificationCenter.DefaultCenter.AddObserver (UIApplication.WillResignActiveNotification, BackgroundNotificationCb);
			} // lock
		}

		internal void RemoveNotification ()
		{
			NSObject? localNotificationToken;
			lock (notificationTokenLock) {
				localNotificationToken = notificationToken;
				notificationToken = null;
			}
			if (localNotificationToken is not null)
				NSNotificationCenter.DefaultCenter.RemoveObserver (localNotificationToken);
		}

		void BackgroundNotificationCb (NSNotification _) => inflightRequests.CancelAll ();

#endif

		public long MaxInputInMemory { get; set; } = long.MaxValue;

		protected override void Dispose (bool disposing)
		{
			if (disposing) {
				inflightRequests.Dispose ();
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

		bool allowAutoRedirect;

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

		public ICredentials? Credentials {
			get {
				return credentials;
			}
			set {
				EnsureModifiability ();
				credentials = value;
			}
		}

#if !NET
		NSUrlSessionHandlerTrustOverrideCallback? trustOverride;

		[Obsolete ("Use the 'TrustOverrideForUrl' property instead.")]
		public NSUrlSessionHandlerTrustOverrideCallback? TrustOverride {
			get {
				return trustOverride;
			}
			set {
				EnsureModifiability ();
				trustOverride = value;
			}
		}
#endif

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
		// we do check if a user does a request and the application goes to the background, but
		// in certain cases the user does that on purpose (BeingBackgroundTask) and wants to be able
		// to use the network. In those cases, which are few, we want the developer to explicitly 
		// bypass the check when there are not request in flight 
		bool bypassBackgroundCheck = true;

		public bool BypassBackgroundSessionCheck {
			get {
				return bypassBackgroundCheck;
			}
			set {
				EnsureModifiability ();
				bypassBackgroundCheck = value;
			}
		}

		public CookieContainer? CookieContainer {
			get {
				return cookieContainer;
			}
			set {
				EnsureModifiability ();
				cookieContainer = value;
			}
		}

		public bool UseCookies {
			get {
				return session.Configuration.HttpCookieStorage is not null;
			}
			set {
				EnsureModifiability ();
				if (sessionType == NSUrlSessionConfiguration.SessionConfigurationType.Ephemeral)
					throw new InvalidOperationException ("Cannot set the use of cookies in Ephemeral sessions.");
				// we have to consider the following table of cases:
				// 1. Value is set to true and cookie storage is not null -> we do nothing
				// 2. Value is set to true and cookie storage is null -> we create/set the storage.
				// 3. Value is false and cookie container is not null -> we clear the cookie storage
				// 4. Value is false and cookie container is null -> we do nothing
				var oldSession = session;
				var configuration = session.Configuration;
				if (value && configuration.HttpCookieStorage is null) {
					// create storage because the user wants to use it. Things are not that easy, we have to 
					// consider the following:
					// 1. Default Session -> uses sharedHTTPCookieStorage
					// 2. Background Session -> uses sharedHTTPCookieStorage
					// 3. Ephemeral Session -> no allowed. apple does not provide a way to access to the private implementation of the storage class :/
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
				// HttpContent.TryComputeLength is `protected internal` :-( but it's indirectly called by headers
				var length = request.Content?.Headers?.ContentLength;
				if (length.HasValue && (length <= MaxInputInMemory))
					nsrequest.Body = NSData.FromStream (stream);
				else
					nsrequest.BodyStream = new WrappedNSInputStream (stream);
			}
			return nsrequest;
		}

#if (SYSTEM_NET_HTTP || MONOMAC) && !NET
		internal
#endif
		protected override async Task<HttpResponseMessage> SendAsync (HttpRequestMessage request, CancellationToken cancellationToken)
		{
			Volatile.Write (ref sentRequest, true);

			var nsrequest = await CreateRequest (request).ConfigureAwait(false);
			var dataTask = session.CreateDataTask (nsrequest);
			var inflightData = inflightRequests.Create (dataTask, request.RequestUri?.AbsoluteUri!, request, cancellationToken);

			if (dataTask.State == NSUrlSessionTaskState.Suspended)
				dataTask.Resume ();

			return await inflightData.CompletionSource.Task.ConfigureAwait (false);
		}

#if NET
		// Properties that will be called by the default HttpClientHandler

		// NSUrlSession handler automatically handles decompression, and there doesn't seem to be a way to turn it off.
		// The available decompression algorithms depend on the OS version we're running on, and maybe the target OS version as well,
		// so just say we're doing them all, and not do anything in the setter (it doesn't seem to be configurable in NSUrlSession anyways).
		public DecompressionMethods AutomaticDecompression {
			get => DecompressionMethods.All;
			set { }
		}

		// We're ignoring this property, just like Xamarin.Android does:
		// https://github.com/xamarin/xamarin-android/blob/09e8cb5c07ea6c39383185a3f90e53186749b802/src/Mono.Android/Xamarin.Android.Net/AndroidMessageHandler.cs#L158
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("macos")]
		public bool CheckCertificateRevocationList { get; set; } = false;

		// We're ignoring this property, just like Xamarin.Android does:
		// https://github.com/xamarin/xamarin-android/blob/09e8cb5c07ea6c39383185a3f90e53186749b802/src/Mono.Android/Xamarin.Android.Net/AndroidMessageHandler.cs#L150
		// Note: we can't return null (like Xamarin.Android does), because the return type isn't nullable.
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("macos")]
		public X509CertificateCollection ClientCertificates { get { return new X509CertificateCollection (); } }

		// We're ignoring this property, just like Xamarin.Android does:
		// https://github.com/xamarin/xamarin-android/blob/09e8cb5c07ea6c39383185a3f90e53186749b802/src/Mono.Android/Xamarin.Android.Net/AndroidMessageHandler.cs#L148
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("macos")]
		public ClientCertificateOption ClientCertificateOptions { get; set; }

		// We're ignoring this property, just like Xamarin.Android does:
		// https://github.com/xamarin/xamarin-android/blob/09e8cb5c07ea6c39383185a3f90e53186749b802/src/Mono.Android/Xamarin.Android.Net/AndroidMessageHandler.cs#L152
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("macos")]
		public ICredentials? DefaultProxyCredentials { get; set; }

		public int MaxAutomaticRedirections {
			get => int.MaxValue;
			set {
				// I believe it's possible to implement support for MaxAutomaticRedirections (it just has to be done)
				if (value != int.MaxValue)
					ObjCRuntime.ThrowHelper.ThrowArgumentOutOfRangeException (nameof (value), value, "It's not possible to lower the max number of automatic redirections.");;
			}
		}

		// We're ignoring this property, just like Xamarin.Android does:
		// https://github.com/xamarin/xamarin-android/blob/09e8cb5c07ea6c39383185a3f90e53186749b802/src/Mono.Android/Xamarin.Android.Net/AndroidMessageHandler.cs#L154
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("macos")]
		public int MaxConnectionsPerServer { get; set; } = int.MaxValue;

		// We're ignoring this property, just like Xamarin.Android does:
		// https://github.com/xamarin/xamarin-android/blob/09e8cb5c07ea6c39383185a3f90e53186749b802/src/Mono.Android/Xamarin.Android.Net/AndroidMessageHandler.cs#L156
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("macos")]
		public int MaxResponseHeadersLength { get; set; } = 64; // Units in K (1024) bytes.

		// We don't support PreAuthenticate, so always return false, and ignore any attempts to change it.
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("macos")]
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
		public IDictionary<string, object>? Properties { get { return null; } }

		// We dont support any custom proxies, and don't let anybody wonder why their proxy isn't
		// being used if they try to assign one (in any case we also return false from 'SupportsProxy').
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("macos")]
		public IWebProxy? Proxy {
			get => null;
			set => throw new PlatformNotSupportedException ();
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
		public SslProtocols SslProtocols { get; set; } = SslProtocols.Tls12 | SslProtocols.Tls13;

		// We're ignoring this property, just like Xamarin.Android does:
		// https://github.com/xamarin/xamarin-android/blob/09e8cb5c07ea6c39383185a3f90e53186749b802/src/Mono.Android/Xamarin.Android.Net/AndroidMessageHandler.cs#L160
		[UnsupportedOSPlatform ("ios")]
		[UnsupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("macos")]
		public Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool>? ServerCertificateCustomValidationCallback { get; set; }

		// There's no way to turn off automatic decompression, so yes, we support it
		public bool SupportsAutomaticDecompression {
			get => true;
		}

		// We don't support using custom proxies, but NSUrlSession will automatically use any proxies configured in the OS.
		public bool SupportsProxy {
			get => false;
		}

		// We support the AllowAutoRedirect property, but we don't support changing the MaxAutomaticRedirections value,
		// so be safe here and say we don't support redirect configuration.
		public bool SupportsRedirectConfiguration {
			get => false;
		}

		// NSUrlSession will automatically use any proxies configured in the OS (so always return true in the getter).
		// There doesn't seem to be a way to turn this off, so throw if someone attempts to disable this.
		public bool UseProxy {
			get => true;
			set {
				if (!value)
					ObjCRuntime.ThrowHelper.ThrowArgumentOutOfRangeException (nameof (value), value, "It's not possible to disable the use of system proxies.");;
			}
		}
#endif // NET

	}
}
