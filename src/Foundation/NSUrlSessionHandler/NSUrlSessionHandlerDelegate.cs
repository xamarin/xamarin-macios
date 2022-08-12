using System;
using System.Net;
using System.Net.Http;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

using CoreFoundation;
using Foundation;
using Security;

#nullable enable

#if !MONOMAC
namespace System.Net.Http {
#else
namespace Foundation {
#endif

	partial class NSUrlSessionHandlerDelegate : NSUrlSessionDataDelegate
	{
		const string SetCookie = "Set-Cookie";
		readonly NSUrlSessionHandler sessionHandler;

		public NSUrlSessionHandlerDelegate (NSUrlSessionHandler handler)
		{
			sessionHandler = handler;
		}

		static Exception createExceptionForNSError(NSError error)
		{
			var innerException = new NSErrorException(error);

			// errors that exists in both share the same error code, so we can use a single switch/case
			// this also ease watchOS integration as if does not expose CFNetwork but (I would not be 
			// surprised if it)could return some of it's error codes
#if __WATCHOS__
			if (error.Domain == NSError.NSUrlErrorDomain) {
#else
			if ((error.Domain == NSError.NSUrlErrorDomain) || (error.Domain == NSError.CFNetworkErrorDomain)) {
#endif
				// Apple docs: https://developer.apple.com/library/mac/documentation/Cocoa/Reference/Foundation/Miscellaneous/Foundation_Constants/index.html#//apple_ref/doc/constant_group/URL_Loading_System_Error_Codes
				// .NET docs: http://msdn.microsoft.com/en-us/library/system.net.webexceptionstatus(v=vs.110).aspx
				switch ((NSUrlError) (long) error.Code) {
				case NSUrlError.Cancelled:
				case NSUrlError.UserCancelledAuthentication:
#if !__WATCHOS__
				case (NSUrlError) NSNetServicesStatus.CancelledError:
#endif
					// No more processing is required so just return.
					return new OperationCanceledException(error.LocalizedDescription, innerException);
				}
			}

			return new HttpRequestException (error.LocalizedDescription, innerException);
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

		void UpdateManagedCookieContainer (NSUrl url, NSHttpCookie[] cookies)
		{
			var uri = new Uri (url.AbsoluteString);
			if (sessionHandler.cookieContainer is not null && cookies.Length > 0)
				lock (sessionHandler.inflightRequestsLock) { // ensure we lock when writing to the collection
					var cookiesContents = new string [cookies.Length];
					for (var index = 0; index < cookies.Length; index++)
						cookiesContents [index] = cookies [index].GetHeaderValue ();
					sessionHandler.cookieContainer.SetCookies (uri, string.Join (',', cookiesContents)); //  as per docs: The contents of an HTTP set-cookie header as returned by a HTTP server, with Cookie instances delimited by commas.
				}
		}

		[Preserve (Conditional = true)]
		public override void DidReceiveResponse (NSUrlSession session, NSUrlSessionDataTask dataTask, NSUrlResponse response, Action<NSUrlSessionResponseDisposition> completionHandler)
		{
			var inflight = GetInflightData (dataTask);

			if (inflight is null)
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
				}, inflight.CancellationTokenSource.Token);

				// NB: The double cast is because of a Xamarin compiler bug
				var httpResponse = new HttpResponseMessage ((HttpStatusCode)status) {
					Content = content,
					RequestMessage = inflight.Request
				};
				httpResponse.RequestMessage.RequestUri = new Uri (urlResponse.Url.AbsoluteString);

				foreach (var v in urlResponse.AllHeaderFields) {
					// NB: Cocoa trolling us so hard by giving us back dummy dictionary entries
					if (v.Key is null || v.Value is null) continue;
					// NSUrlSession tries to be smart with cookies, we will not use the raw value but the ones provided by the cookie storage
					if (v.Key.ToString () == SetCookie) continue;

					httpResponse.Headers.TryAddWithoutValidation (v.Key.ToString (), v.Value.ToString ());
					httpResponse.Content.Headers.TryAddWithoutValidation (v.Key.ToString (), v.Value.ToString ());
				}

				// it might be confusing that we are not using the managed CookieStore here, this is ONLY for those cookies that have been retrieved from
				// the server via a Set-Cookie header, the managed container does not know a thing about this and apple is storing them in the native
				// cookie container. Once we have the cookies from the response, we need to update the managed cookie container
				if (session.Configuration.HttpCookieStorage is not null) {
					var cookies = session.Configuration.HttpCookieStorage.CookiesForUrl (response.Url);
					UpdateManagedCookieContainer (response.Url, cookies);
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
				// set the stream as finished
				inflight.Stream.TrySetReceivedAllData ();

				// send the error or send the response back
				if (error is not null || serverError is not null) {
					// got an error, cancel the stream operatios before we do anything
					inflight.CancellationTokenSource.Cancel (); 
					inflight.Errored = true;

					var exc = inflight.Exception ?? createExceptionForNSError (error ?? serverError!);  // client errors wont happen if we get server errors
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

				if (inflight.CancellationTokenSource.Token.IsCancellationRequested)
					return;

				if (inflight.CompletionSource.Task.IsCompleted)
					return;

				var httpResponse = inflight.Response;

				inflight.ResponseSent = true;

				// EVIL HACK: having TrySetResult inline was blocking the request from completing
				Task.Run (() => inflight.CompletionSource.TrySetResult (httpResponse!));
			}
		}

		[Preserve (Conditional = true)]
		public override void WillCacheResponse (NSUrlSession session, NSUrlSessionDataTask dataTask, NSCachedUrlResponse proposedResponse, Action<NSCachedUrlResponse> completionHandler)
		{
			var inflight = GetInflightData (dataTask);

			if (inflight is null)
				return;
			// apple caches post request with a body, which should not happen. https://github.com/xamarin/maccore/issues/2571 
			var disableCache = sessionHandler.DisableCaching || (inflight.Request.Method == HttpMethod.Post && inflight.Request.Content is not null);
			completionHandler (disableCache ? null! : proposedResponse);
		}

		[Preserve (Conditional = true)]
		public override void WillPerformHttpRedirection (NSUrlSession session, NSUrlSessionTask task, NSHttpUrlResponse response, NSUrlRequest newRequest, Action<NSUrlRequest> completionHandler)
		{
			completionHandler (sessionHandler.AllowAutoRedirect ? newRequest : null!);
		}

		[Preserve (Conditional = true)]
		public override void DidReceiveChallenge (NSUrlSession session, NSUrlSessionTask task, NSUrlAuthenticationChallenge challenge, Action<NSUrlSessionAuthChallengeDisposition, NSUrlCredential> completionHandler)
		{
			var inflight = GetInflightData (task);

			if (inflight is null)
				return;

			// ToCToU for the callback
#if !NET
			var trustCallback = sessionHandler.TrustOverride;
#endif
			var trustCallbackForUrl = sessionHandler.TrustOverrideForUrl;
#if NET
			var hasCallBack = trustCallbackForUrl is not null;
#else
			var hasCallBack = trustCallback is not null || trustCallbackForUrl is not null;
#endif
			if (hasCallBack && challenge.ProtectionSpace.AuthenticationMethod == NSUrlProtectionSpace.AuthenticationMethodServerTrust) {
#if NET
				// if the trust delegate allows to ignore the cert, do it. Since we are using nullables, if the delegate is not present, by default is false
				var trustSec = (trustCallbackForUrl?.Invoke (sessionHandler, inflight.RequestUrl, challenge.ProtectionSpace.ServerSecTrust) ?? false);
#else
				// if one of the delegates allows to ignore the cert, do it. We check first the one that takes the url because is more precisse, later the
				// more general one. Since we are using nullables, if the delegate is not present, by default is false
				var trustSec = (trustCallbackForUrl?.Invoke (sessionHandler, inflight.RequestUrl, challenge.ProtectionSpace.ServerSecTrust) ?? false) || 
					(trustCallback?.Invoke (sessionHandler, challenge.ProtectionSpace.ServerSecTrust) ?? false);
#endif

				if (trustSec) {
					var credential = new NSUrlCredential (challenge.ProtectionSpace.ServerSecTrust);
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
				var authHeader = inflight.Request.Headers?.Authorization;
				if (!(string.IsNullOrEmpty (authHeader?.Scheme) && string.IsNullOrEmpty (authHeader?.Parameter))) {
					completionHandler (NSUrlSessionAuthChallengeDisposition.RejectProtectionSpace, null!);
					return;
				}
			}

			if (sessionHandler.Credentials is not null && TryGetAuthenticationType (challenge.ProtectionSpace, out var authType)) {
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
						var uri = inflight.Request.RequestUri!;
						credentialsToUse = sessionHandler.Credentials.GetCredential (uri, authType);
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

		static readonly string RejectProtectionSpaceAuthType = "reject";

		static bool TryGetAuthenticationType (NSUrlProtectionSpace protectionSpace, [NotNullWhen (true)] out string? authenticationType)
		{
			if (protectionSpace.AuthenticationMethod == NSUrlProtectionSpace.AuthenticationMethodNTLM) {
				authenticationType = "NTLM";
			} else if (protectionSpace.AuthenticationMethod == NSUrlProtectionSpace.AuthenticationMethodHTTPBasic) {
				authenticationType = "basic";
			} else if (protectionSpace.AuthenticationMethod == NSUrlProtectionSpace.AuthenticationMethodNegotiate ||
				protectionSpace.AuthenticationMethod == NSUrlProtectionSpace.AuthenticationMethodHTMLForm ||
				protectionSpace.AuthenticationMethod == NSUrlProtectionSpace.AuthenticationMethodHTTPDigest) {
				// Want to reject this authentication type to allow the next authentication method in the request to
				// be used.
				authenticationType = RejectProtectionSpaceAuthType;
			} else {
				// ServerTrust, ClientCertificate or Default.
				authenticationType = null;
				return false;
			}
			return true;
		}
	}

}
