using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace xharness.Jenkins
{
    // This is a very simple class to manage the general concept of 'resource'.
    // Performance isn't important, so this is very simple.
    // Currently it's only used to make sure everything that happens on the desktop
    // is serialized (Jenkins.DesktopResource), but in the future the idea is to
    // make each connected device a separate resource, which will make it possible
    // to run tests in parallel across devices (and at the same time use the desktop
    // to build the next test project).
    class Resource
	{
		public string Name;
		public string Description;
		ConcurrentQueue<TaskCompletionSource<IAcquiredResource>> queue = new ConcurrentQueue<TaskCompletionSource<IAcquiredResource>>();
		ConcurrentQueue<TaskCompletionSource<IAcquiredResource>> exclusive_queue = new ConcurrentQueue<TaskCompletionSource<IAcquiredResource>>();
		int users;
		int max_concurrent_users = 1;
		bool exclusive;

		public int Users => users;
		public int QueuedUsers => queue.Count + exclusive_queue.Count;
		public int MaxConcurrentUsers
		{
			get
			{
				return max_concurrent_users;
			}
			set
			{
				max_concurrent_users = value;
			}
		}

		public Resource(string name, int max_concurrent_users = 1, string description = null)
		{
			this.Name = name;
			this.max_concurrent_users = max_concurrent_users;
			this.Description = description ?? name;
		}

		public Task<IAcquiredResource> AcquireConcurrentAsync()
		{
			lock (queue)
			{
				if (!exclusive && users < max_concurrent_users)
				{
					users++;
					return Task.FromResult<IAcquiredResource>(new AcquiredResource(this));
				}
				else
				{
					var tcs = new TaskCompletionSource<IAcquiredResource>(new AcquiredResource(this));
					queue.Enqueue(tcs);
					return tcs.Task;
				}
			}
		}

		public Task<IAcquiredResource> AcquireExclusiveAsync()
		{
			lock (queue)
			{
				if (users == 0)
				{
					users++;
					exclusive = true;
					return Task.FromResult<IAcquiredResource>(new AcquiredResource(this));
				}
				else
				{
					var tcs = new TaskCompletionSource<IAcquiredResource>(new AcquiredResource(this));
					exclusive_queue.Enqueue(tcs);
					return tcs.Task;
				}
			}
		}

		void Release()
		{
			TaskCompletionSource<IAcquiredResource> tcs;

			lock (queue)
			{
				users--;
				exclusive = false;
				if (queue.TryDequeue(out tcs))
				{
					users++;
					tcs.SetResult((IAcquiredResource)tcs.Task.AsyncState);
				}
				else if (users == 0 && exclusive_queue.TryDequeue(out tcs))
				{
					users++;
					exclusive = true;
					tcs.SetResult((IAcquiredResource)tcs.Task.AsyncState);
				}
			}
		}

		class AcquiredResource : IAcquiredResource
		{
			Resource resource;

			public AcquiredResource(Resource resource)
			{
				this.resource = resource;
			}

			void IDisposable.Dispose()
			{
				resource.Release();
			}

			public Resource Resource { get { return resource; } }
		}
	}
}
