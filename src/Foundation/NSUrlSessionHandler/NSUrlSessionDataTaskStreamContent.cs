using System;
using System.Threading;

#nullable enable

#if !MONOMAC
namespace System.Net.Http {
#else
namespace Foundation {
#endif

	class NSUrlSessionDataTaskStreamContent : MonoStreamContent
	{
		Action? disposed;

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
}
