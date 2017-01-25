using System.Collections.Generic;
using System.Threading.Tasks;

namespace Xamarin.Bundler
{
	public class BuildTasks : List<BuildTask>
	{
		static void Execute (List<BuildTask> added, BuildTask v)
		{
			var next = v.Run ();
			if (next != null) {
				lock (added)
					added.AddRange (next);
			}
		}

		public void ExecuteInParallel ()
		{
			if (Count == 0)
				return;

			var build_list = new List<BuildTask> (this);
			var added = new List<BuildTask> ();
			while (build_list.Count > 0) {
				added.Clear ();
				Parallel.ForEach (build_list, new ParallelOptions () { MaxDegreeOfParallelism = Driver.Concurrency }, (v) =>
				{
					Execute (added, v);
				});
				build_list.Clear ();
				build_list.AddRange (added);
			}

			Clear ();
		}
	}

	public abstract class BuildTask
	{
		public IEnumerable<BuildTask> NextTasks;

		protected abstract void Execute ();

		public IEnumerable<BuildTask> Run ()
		{
			Execute ();
			return NextTasks;
		}

		public virtual bool IsUptodate ()
		{
			return false;
		}
	}
}
