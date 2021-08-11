using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Xharness.Jenkins.TestTasks {
	// This is a very simple class to manage the general concept of 'resource'.
	// Performance isn't important, so this is very simple.
	// Currently it's only used to make sure everything that happens on the desktop
	// is serialized (Jenkins.DesktopResource), but in the future the idea is to
	// make each connected device a separate resource, which will make it possible
	// to run tests in parallel across devices (and at the same time use the desktop
	// to build the next test project).
	public class Resource {
		public string Name;
		public string Description;
		ConcurrentQueue<TaskCompletionSource<IAcquiredResource>> queue = new ConcurrentQueue<TaskCompletionSource<IAcquiredResource>> ();
		ConcurrentQueue<TaskCompletionSource<IAcquiredResource>> exclusive_queue = new ConcurrentQueue<TaskCompletionSource<IAcquiredResource>> ();
		bool exclusive;

		public int Users { get; private set; }
		public int QueuedUsers => queue.Count + exclusive_queue.Count;
		public int MaxConcurrentUsers { get; set; } = 1;

		public Resource (string name, int max_concurrent_users = 1, string description = null)
		{
			this.Name = name;
			this.MaxConcurrentUsers = max_concurrent_users;
			this.Description = description ?? name;
		}

		public Task<IAcquiredResource> AcquireConcurrentAsync ()
		{
			lock (queue) {
				if (!exclusive && Users < MaxConcurrentUsers) {
					Users++;
					return Task.FromResult<IAcquiredResource> (new AcquiredResource (this));
				} else {
					var tcs = new TaskCompletionSource<IAcquiredResource> (new AcquiredResource (this));
					queue.Enqueue (tcs);
					return tcs.Task;
				}
			}
		}

		public Task<IAcquiredResource> AcquireExclusiveAsync ()
		{
			lock (queue) {
				if (Users == 0) {
					Users++;
					exclusive = true;
					return Task.FromResult<IAcquiredResource> (new AcquiredResource (this));
				} else {
					var tcs = new TaskCompletionSource<IAcquiredResource> (new AcquiredResource (this));
					exclusive_queue.Enqueue (tcs);
					return tcs.Task;
				}
			}
		}

		void Release ()
		{
			lock (queue) {
				Users--;
				exclusive = false;
				if (queue.TryDequeue (out TaskCompletionSource<IAcquiredResource> tcs)) {
					Users++;
					tcs.SetResult ((IAcquiredResource) tcs.Task.AsyncState);
				} else if (Users == 0 && exclusive_queue.TryDequeue (out tcs)) {
					Users++;
					exclusive = true;
					tcs.SetResult ((IAcquiredResource) tcs.Task.AsyncState);
				}
			}
		}

		class AcquiredResource : IAcquiredResource {
			public AcquiredResource (Resource resource)
			{
				this.Resource = resource;
			}

			void IDisposable.Dispose ()
			{
				Resource.Release ();
			}

			public Resource Resource { get; }
		}
	}
}
