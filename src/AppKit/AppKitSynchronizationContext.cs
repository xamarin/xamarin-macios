//
// AppKitSynchronizationContext.cs: Default SynchronizationContext for the main UI thread
//
using System.Threading;

using Foundation;

namespace AppKit {
	class AppKitSynchronizationContext : SynchronizationContext {
		public override SynchronizationContext CreateCopy ()
		{
			return new AppKitSynchronizationContext ();
		}

		public override void Post (SendOrPostCallback d, object state)
		{
			NSRunLoop.Main.BeginInvokeOnMainThread (() => d (state));
		}

		public override void Send (SendOrPostCallback d, object state)
		{
			NSRunLoop.Main.InvokeOnMainThread (() => d (state));
		}
	}
}