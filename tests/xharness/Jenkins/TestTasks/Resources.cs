using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Xharness.Jenkins.TestTasks {
	public class Resources : IEnumerable<Resource> {
		readonly Resource [] resources;

		public Resources (IEnumerable<Resource> resources)
		{
			this.resources = resources.ToArray ();
		}

		public Task<IAcquiredResource> AcquireAnyConcurrentAsync ()
		{
			if (resources.Length == 0)
				throw new Exception ("No resources");

			if (resources.Length == 1)
				return resources [0].AcquireConcurrentAsync ();

			// We try to acquire every resource
			// When the first one succeeds, we set the result to true
			// We immediately release any other resources we acquire.
			var tcs = new TaskCompletionSource<IAcquiredResource> ();
			for (int i = 0; i < resources.Length; i++) {
				resources [i].AcquireConcurrentAsync ().ContinueWith ((v) => {
					var ar = v.Result;
					if (!tcs.TrySetResult (ar))
						ar.Dispose ();
				});
			}

			return tcs.Task;
		}

		public IEnumerator<Resource> GetEnumerator ()
		{
			foreach (var r in resources)
				yield return r;
		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}
}
