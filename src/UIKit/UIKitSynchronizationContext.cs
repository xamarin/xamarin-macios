//
// UIKitSynchronizationContext.cs: a SynchronizationContext that works through the main UIKit NSRunLoop.
//
// Authors:
//   Chris Toshok
//
// Copyright 2011, Xamarin Inc.
//

using System;
using System.Threading;
using Foundation;

namespace UIKit {

	internal class UIKitSynchronizationContext : SynchronizationContext {
		public override SynchronizationContext CreateCopy ()
		{
			return new UIKitSynchronizationContext ();
		}

		public override void Post (SendOrPostCallback d, object state)
		{
			NSRunLoop.Main.BeginInvokeOnMainThread ( () => d (state) );
		}

		public override void Send (SendOrPostCallback d, object state)
		{
			NSRunLoop.Main.InvokeOnMainThread ( () => d (state) );
		}
	}

}