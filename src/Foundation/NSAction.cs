//
// Copyright 2009-2010, Novell, Inc.
// Copyright 2012-2014 Xamarin Inc.
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
using System.Runtime.InteropServices;
using System.Threading;
using ObjCRuntime;

// Disable until we get around to enable + fix any issues.
#nullable disable

namespace Foundation {

#if !COREBUILD
	// Use this for synchronous operations
	internal abstract class NSDispatcher : NSObject {
		public const string SelectorName = "xamarinApplySelector";
		public static readonly Selector Selector = new Selector (SelectorName);

		protected NSDispatcher ()
		{
			IsDirectBinding = false;
		}

		[Export (SelectorName)]
		[Preserve (Conditional = true)]
		public abstract void Apply ();
	}

	// Use this for synchronous operations
	[Register ("__MonoMac_NSActionDispatcher")]
	internal sealed class NSActionDispatcher : NSDispatcher {
		readonly Action action;

		public NSActionDispatcher (Action action)
		{
			if (action is null)
				throw new ArgumentNullException ("action");

			this.action = action;
		}

		public override void Apply () => action ();
	}

	// Use this for synchronous operations
	[Register ("__MonoMac_NSSynchronizationContextDispatcher")]
	internal sealed class NSSynchronizationContextDispatcher : NSDispatcher {
		readonly SendOrPostCallback d;
		readonly object state;

		public NSSynchronizationContextDispatcher (SendOrPostCallback d, object state)
		{
			if (d is null)
				throw new ArgumentNullException (nameof (d));

			this.d = d;
			this.state = state;
		}

		public override void Apply () => d (state);
	}

	// Used this for NSTimer support
	[Register ("__Xamarin_NSTimerActionDispatcher")]
	internal sealed class NSTimerActionDispatcher : NSObject {
		public const string SelectorName = "xamarinFireSelector:";
		public static readonly Selector Selector = new Selector (SelectorName);

		readonly Action<NSTimer> action;

		public NSTimerActionDispatcher (Action<NSTimer> action)
		{
			if (action is null)
				throw new ArgumentNullException ("action");

			this.action = action;
			IsDirectBinding = false;
		}

		[Export (SelectorName)]
		[Preserve (Conditional = true)]
		public void Fire (NSTimer timer)
		{
			action (timer);
		}
	}

	abstract class NSAsyncDispatcher : NSDispatcher {
		readonly GCHandle gch;

		protected NSAsyncDispatcher ()
		{
			gch = GCHandle.Alloc (this);
		}

		public override void Apply ()
		{
			gch.Free ();

			//
			// Although I would like to call Dispose here, to
			// reduce the load on the GC, we have some useful diagnostic
			// code in our runtime that is useful to track down
			// problems, so we are removing the Dispose and letting
			// the GC and our pipeline do their job.
			// 
#if MONOTOUCH
			// MonoTouch has fixed the above problems, and we can call
			// Dispose here.
			Dispose ();
#endif
		}
	}

	// Use this for asynchronous operations
	[Register ("__MonoMac_NSAsyncActionDispatcher")]
	internal sealed class NSAsyncActionDispatcher : NSAsyncDispatcher {
		Action action;

		public NSAsyncActionDispatcher (Action action)
		{
			if (action is null)
				throw new ArgumentNullException (nameof (action));

			this.action = action;
		}

		public override void Apply ()
		{
			try {
				action ();
			} finally {
				action = null;
				base.Apply ();
			}
		}
	}

	// Use this for asynchronous operations
	[Register ("__MonoMac_NSAsyncSynchronizationContextDispatcher")]
	internal sealed class NSAsyncSynchronizationContextDispatcher : NSAsyncDispatcher {
		SendOrPostCallback d;
		object state;

		public NSAsyncSynchronizationContextDispatcher (SendOrPostCallback d, object state)
		{
			if (d is null)
				throw new ArgumentNullException (nameof (d));

			this.d = d;
			this.state = state;
		}

		public override void Apply ()
		{
			try {
				d (state);
			} finally {
				d = null; // this is a one-shot dispatcher
				state = null;
				base.Apply ();
			}
		}
	}
#endif // !COREBUILD
}
