//
// AppKitSynchronizationContext.cs: Default SynchronizationContext for the main UI thread
//
using System.Threading;
using System.Runtime.Versioning;

using Foundation;

namespace AppKit {
#if NET
	[SupportedOSPlatform ("macos")]
#endif
	class AppKitSynchronizationContext : SynchronizationContext {
		public override SynchronizationContext CreateCopy ()
		{
			return new AppKitSynchronizationContext ();
		}

		public override void Post (SendOrPostCallback d, object state)
		{
			NSRunLoop.Main.BeginInvokeOnMainThread (d, state);
		}

		public override void Send (SendOrPostCallback d, object state)
		{
			NSRunLoop.Main.InvokeOnMainThread (d, state);
		}
	}
}
