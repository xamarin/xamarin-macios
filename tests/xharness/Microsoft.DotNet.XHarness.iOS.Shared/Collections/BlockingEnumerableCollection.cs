using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.DotNet.XHarness.iOS.Shared.Collections {
	// This is a collection whose enumerator will wait enumerating until 
	// the collection has been marked as completed (but the enumerator can still
	// be created; this allows the creation of linq queries whose execution is
	// delayed until later).
	public class BlockingEnumerableCollection<T> : IEnumerable<T> where T : class {
		List<T> list = new List<T> ();
		TaskCompletionSource<bool> completed = new TaskCompletionSource<bool> ();

		public int Count {
			get {
				WaitForCompletion ();
				return list.Count;
			}
		}

		public void Add (T device)
		{
			if (completed.Task.IsCompleted)
				Console.WriteLine ("Adding to completed collection!");
			list.Add (device);
		}

		public void SetCompleted ()
		{
			completed.TrySetResult (true);
		}

		void WaitForCompletion ()
		{
			completed.Task.Wait ();
		}

		public void Reset ()
		{
			completed = new TaskCompletionSource<bool> ();
			list.Clear ();
		}

		public IEnumerator<T> GetEnumerator ()
		{
			return new Enumerator (this);
		}

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return GetEnumerator ();
		}

		class Enumerator : IEnumerator<T> {
			BlockingEnumerableCollection<T> collection;
			IEnumerator<T> enumerator;

			public Enumerator (BlockingEnumerableCollection<T> collection)
			{
				this.collection = collection;
			}

			public T Current {
				get {
					return enumerator.Current;
				}
			}

			object IEnumerator.Current {
				get {
					return enumerator.Current;
				}
			}

			public void Dispose ()
			{
				enumerator.Dispose ();
			}

			public bool MoveNext ()
			{
				collection.WaitForCompletion ();
				if (enumerator == null)
					enumerator = collection.list.GetEnumerator ();
				return enumerator.MoveNext ();
			}

			public void Reset ()
			{
				enumerator.Reset ();
			}
		}
	}
}
