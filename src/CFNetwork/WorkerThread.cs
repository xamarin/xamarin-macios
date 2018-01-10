//
// MonoMac.CFNetwork.WorkerThread
//
// Authors:
//      Martin Baulig (martin.baulig@gmail.com)
//
// Copyright 2012 Xamarin Inc. (http://www.xamarin.com)
//
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
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;
using Foundation;
using CoreFoundation;

#if XAMCORE_2_0
using CFRunLoopModeString = global::Foundation.NSString;
#else
using CFRunLoopModeString = global::System.String;
#endif

namespace CFNetwork {

	public class WorkerThread {
		CFRunLoop loop;
		Source source;
		Context context;
		CancellationTokenSource cts;
		ManualResetEventSlim readyEvent;
		ConcurrentQueue<Event> eventQueue = new ConcurrentQueue<Event> ();

		public WorkerThread ()
		{
			cts = new CancellationTokenSource ();
			readyEvent = new ManualResetEventSlim (false);

			var thread = new Thread (() => Main ());
			thread.Start ();

			readyEvent.Wait ();
		}

		[Conditional ("DEBUG")]
		static void Log (string message, params object[] args)
		{
			Debug.WriteLine (string.Format (message, args), "WorkerThread");
		}

		public void Main ()
		{
			source = new Source (this);

			loop = CFRunLoop.Current;
			loop.AddSource (source, CFRunLoop.ModeDefault);

			context = new Context (this);
			SynchronizationContext.SetSynchronizationContext (context);

			readyEvent.Set ();

			loop.Run ();

			cts.Dispose ();
			source.Dispose ();
		}

		public void Stop ()
		{
			cts.Cancel ();
			loop.RemoveSource (source, CFRunLoop.ModeDefault);
			loop.Stop ();
		}

		protected void PostNoResult (Action callback)
		{
			var ev = new Event ();
			ev.Callback = c => {
				callback ();
				return null;
			};
			eventQueue.Enqueue (ev);
			source.Signal ();
			loop.WakeUp ();
		}

		public Task Post (Action callback)
		{
			return Post (c => callback (), CancellationToken.None);
		}

		public async Task Post (Action<CancellationToken> callback, CancellationToken cancellationToken)
		{
			var ev = new Event ();
			ev.Callback = c => {
				callback (c);
				return null;
			};
			ev.Tcs = new TaskCompletionSource<object> ();
			ev.Cts = CancellationTokenSource.CreateLinkedTokenSource (cts.Token, cancellationToken);
			eventQueue.Enqueue (ev);
			source.Signal ();
			loop.WakeUp ();

			try {
				await ev.Tcs.Task;
			} finally {
				ev.Cts.Dispose ();
			}
		}

		public Task<T> Post<T> (Func<T> callback)
		{
			return Post (c => callback (), CancellationToken.None);
		}

		public async Task<T> Post<T> (Func<CancellationToken, T> callback,
		                              CancellationToken cancellationToken)
		{
			var ev = new Event ();
			ev.Callback = c => callback (c);
			ev.Tcs = new TaskCompletionSource<object> ();
			ev.Cts = CancellationTokenSource.CreateLinkedTokenSource (cts.Token, cancellationToken);
			eventQueue.Enqueue (ev);
			source.Signal ();
			loop.WakeUp ();

			try {
				var result = await ev.Tcs.Task;
				if (result != null)
					return (T)result;
				else
					return default(T);
			} finally {
				ev.Cts.Dispose ();
			}
		}

		protected void Perform ()
		{
			Event ev;
			if (!eventQueue.TryDequeue (out ev))
				return;

			if ((ev.Cts != null) && ev.Cts.IsCancellationRequested) {
				ev.Tcs.SetCanceled ();
				return;
			}

			var effectiveCts = ev.Cts ?? cts;

			try {
				var result = ev.Callback (effectiveCts.Token);
				if (ev.Tcs != null)
					ev.Tcs.SetResult (result);
			} catch (TaskCanceledException) {
				if (ev.Tcs != null)
					ev.Tcs.SetCanceled ();
			} catch (Exception ex) {
				if (ev.Tcs != null)
					ev.Tcs.SetException (ex);
			}
		}

		struct Event {
			public Func<CancellationToken, object> Callback;
			public TaskCompletionSource<object> Tcs;
			public CancellationTokenSource Cts;
		}

		class Context : SynchronizationContext {
			WorkerThread worker;

			public Context (WorkerThread worker)
			{
				this.worker = worker;
			}

			public override SynchronizationContext CreateCopy ()
			{
				return new Context (worker);
			}

			public override void Post (SendOrPostCallback d, object state)
			{
				worker.PostNoResult (() => d (state));
			}

			public override void Send (SendOrPostCallback d, object state)
			{
				worker.Post (() => d (state)).Wait ();
			}
		}

		class Source : CFRunLoopSourceCustom {
			WorkerThread worker;

			public Source (WorkerThread worker)
			{
				this.worker = worker;
			}

			protected override void OnSchedule (CFRunLoop loop, CFRunLoopModeString mode)
			{
				;
			}

			protected override void OnCancel (CFRunLoop loop, CFRunLoopModeString mode)
			{
				;
			}

			protected override void OnPerform ()
			{
				worker.Perform ();
			}
		}
	}
}

