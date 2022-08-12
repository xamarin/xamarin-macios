using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Foundation;

#nullable enable

#if !MONOMAC
namespace System.Net.Http {
#else
namespace Foundation {
#endif

	class NSUrlSessionHandlerInflightData : IDisposable {
		readonly NSUrlSessionHandler sessionHandler;
		readonly Dictionary<NSUrlSessionTask, (InflightData data, CancellationData cancellation)> inflightRequests = new ();

		public object Lock { get; } = new object ();

		public NSUrlSessionHandlerInflightData (NSUrlSessionHandler handler) {
			sessionHandler = handler;
		}

		public void CancelAll () {
			// the cancelation task of each of the sources will clean the different resources. Each removal is done
			// inside a lock, but of course, the .Values collection will not like that because it is modified during the
			// iteration. We split the operation in two, get all the diff cancelation sources, then try to cancel each of them
			// which will do the correct lock dance. Note that we could be tempted to do a RemoveAll, that will yield the same
			// runtime issue, this is dull but safe. 
			List<TaskCompletionSource<HttpResponseMessage>> sources;
			lock (Lock) { // just lock when we iterate
				sources = new (inflightRequests.Count);
				foreach (var (_, cancellation) in inflightRequests.Values) {
					sources.Add (cancellation.CompletionSource);
				}
			}
			sources.ForEach (source => { source.TrySetCanceled (); });
		}

		public (InflightData inflight, CancellationData cancellation) Create (NSUrlSessionTask dataTask, string requestUrl, HttpRequestMessage request, CancellationToken cancellationToken) {
			var inflightData = new InflightData (request.RequestUri?.AbsoluteUri!, request);
			var cancellationData = new CancellationData (cancellationToken);

			lock (Lock) {
#if !MONOMAC  && !__WATCHOS__
				// Add the notification whenever needed
				sessionHandler.AddNotification ();
#endif
				inflightRequests.Add (dataTask, new (inflightData, cancellationData));
			}

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
				Remove (dataTask);
				cancellationData.CompletionSource.TrySetCanceled ();
			});

			return (inflightData, cancellationData);
		}

		public void Get (NSUrlSessionTask task, out InflightData? inflightData, out CancellationData? cancellationData)
		{
			lock (Lock) {
				if (inflightRequests.TryGetValue (task, out var inflight)) {
					// ensure that we did not cancel the request, if we did, do cancel the task, if we 
					// cancel the task it means that we are not interested in any of the delegate methods:
					// 
					// DidReceiveResponse     We might have received a response, but either the user cancelled or a 
					//                        timeout did, if that is the case, we do not care about the response.
					// DidReceiveData         Of buffer has a partial response ergo garbage and there is not real 
					//                        reason we would like to add more data.
					// DidCompleteWithError - We are not changing a behaviour compared to the case in which 
					//                        we did not find the data.
					(inflightData, cancellationData) = inflight;
					if (cancellationData.CancellationToken.IsCancellationRequested) {
						task?.Cancel ();
						// return null so that we break out of any delegate method, but we do have the cancellation data
						inflightData = null;
					}
				} else {
					// if we did not manage to get the inflight data, we either got an error or have been canceled,
					// lets cancel the task, that will execute DidCompleteWithError
					task?.Cancel ();
					inflightData = null;
					cancellationData = null;
				}
			}
		}

		public void Remove (NSUrlSessionTask task, bool cancel = true)
		{
			lock (Lock) {
				if (inflightRequests.TryGetValue (task, out var inflight)) {
					var (inflightData, cancellationData) = inflight;
					if (cancel)
						cancellationData.CancellationTokenSource.Cancel ();
					cancellationData.Dispose ();
					inflightRequests.Remove (task);
				}
#if !MONOMAC  && !__WATCHOS__
				// do we need to be notified? If we have not inflightData, we do not
				if (inflightRequests.Count == 0)
					sessionHandler.RemoveNotification ();
#endif
				if (cancel)
					task?.Cancel ();

				task?.Dispose ();
			}

		}

		protected void Dispose (bool disposing)
		{
			lock (Lock) {
#if !MONOMAC  && !__WATCHOS__
			// remove the notification if present, method checks against null
			sessionHandler.RemoveNotification ();
#endif
				foreach (var pair in inflightRequests) {
					pair.Key?.Cancel ();
					pair.Key?.Dispose ();
					var (_, cancellation) = pair.Value;
					cancellation.Dispose ();
				}

				inflightRequests.Clear ();
			}
		}

		public void Dispose()
		{
			Dispose (true);
			GC.SuppressFinalize(this);
		}

	}

	class CancellationData : IDisposable {
		public TaskCompletionSource<HttpResponseMessage> CompletionSource { get; } = new TaskCompletionSource<HttpResponseMessage> (TaskCreationOptions.RunContinuationsAsynchronously);
		public CancellationToken CancellationToken { get; set; }
		public CancellationTokenSource CancellationTokenSource { get; } = new CancellationTokenSource ();

		public CancellationData (CancellationToken cancellationToken) {
			CancellationToken = cancellationToken;
		}

		public void Dispose()
		{
			Dispose (true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose (bool disposing)
		{
			if (disposing) {
				CancellationTokenSource.Dispose ();
			}
		}
	}

	// Contains the data of the infligh requests. Should not contain any reference to the cancellation objects, thos
	// are shared by the managed code and we want to make sure that we have a clear separation between the managed and the
	// unmanaged worlds.
	class InflightData
	{
		public readonly object Lock = new object ();
		public string RequestUrl { get; set; }

		public NSUrlSessionDataTaskStream Stream { get; } = new NSUrlSessionDataTaskStream ();
		public HttpRequestMessage Request { get; set; }
		public HttpResponseMessage? Response { get; set; }

		public Exception? Exception { get; set; }
		public bool ResponseSent { get; set; }
		public bool Errored { get; set; }
		public bool Disposed { get; set; }
		public bool Completed { get; set; }
		// CancellationToken.IsCancellationRequested
		public bool Done { get { return Errored || Disposed || Completed; } }

		public InflightData (string requestUrl, HttpRequestMessage request)
		{
			RequestUrl = requestUrl;
			Request = request;
		}
	}

}
