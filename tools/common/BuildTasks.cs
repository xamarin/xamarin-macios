using System.Collections.Generic;
using System.Linq;
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
		
		// A list of input files (not a list of all the dependencies that would make this task rebuild).
		public abstract IEnumerable<string> Inputs { get; }

		public IEnumerable<BuildTask> Run ()
		{
			Execute ();
			return NextTasks;
		}

		public virtual bool IsUptodate {
			get {
				return Application.IsUptodate (FileDependencies, Outputs);
			}
		}

		// A list of all the files that causes the task to rebuild.
		// This should at least include all the 'Inputs', and potentially other files as well.
		public virtual IEnumerable<string> FileDependencies {
			get {
				return Inputs;
			}
		}

		// A list of files that this task outputs.
		public abstract IEnumerable<string> Outputs { get; }

		public bool CheckIsUptodate ()
		{
			if (!IsUptodate)
				return false;
			var outputs = Outputs;
			if (outputs.Count () > 1) {
				Driver.Log (3, "Targets '{0}' are up-to-date.", string.Join ("', '", outputs));
			} else {
				Driver.Log (3, "Target '{0}' is up-to-date.", outputs.First ());
			}
			return true;
		}
	}
}
