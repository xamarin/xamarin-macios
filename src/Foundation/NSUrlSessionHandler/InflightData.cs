using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

#if !MONOMAC
namespace System.Net.Http {
#else
namespace Foundation {
#endif

	class InflightData : IDisposable
	{
		public readonly object Lock = new object ();
		public string RequestUrl { get; set; }

		public TaskCompletionSource<HttpResponseMessage> CompletionSource { get; } = new TaskCompletionSource<HttpResponseMessage> ();
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
		// CancellationToken.IsCancellationRequested
		public bool Done { get { return Errored || Disposed || Completed; } }

		public InflightData (string requestUrl, CancellationToken cancellationToken, HttpRequestMessage request)
		{
			RequestUrl = requestUrl;
			CancellationToken = cancellationToken;
			Request = request;
		}

		public void Dispose()
		{
			Dispose (true);
			GC.SuppressFinalize(this);
		}

		// The bulk of the clean-up code is implemented in Dispose(bool)
		protected virtual void Dispose (bool disposing)
		{
			if (disposing) {
				CancellationTokenSource.Dispose ();
			}
		}

	}

}
