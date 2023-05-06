// #define LOG_TASK

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Xamarin.Bundler {
	// This contains all the tasks that has to be done to create the final output.
	// Intermediate tasks do not have to be in this list (as long as they're in another task's dependencies),
	// but it doesn't hurt if they're here either.
	// This is a directed graph, where the top nodes represent the input, and the leaf nodes the output.
	// Each node (BuildTask) will build its dependencies before building itself, so at build time
	// we only have to iterate over the leaf nodes to build the whole graph.
	public class BuildTasks : List<BuildTask> {
		SemaphoreSlim semaphore;

		public BuildTasks ()
		{
			semaphore = new SemaphoreSlim (Driver.Concurrency, Driver.Concurrency);
			Driver.Log (2, $"Created task scheduler with concurrency {Driver.Concurrency}.");
		}

		public async Task AcquireSemaphore ()
		{
			await semaphore.WaitAsync ();
		}

		public void ReleaseSemaphore ()
		{
			semaphore.Release ();
		}

		void ExecuteBuildTasks (SingleThreadedSynchronizationContext context, List<Exception> exceptions)
		{
			Task [] tasks = new Task [Count];

			for (int i = 0; i < Count; i++)
				tasks [i] = this [i].Execute (this);

			Task.Factory.StartNew (async () => {
				try {
					await Task.WhenAll (tasks);
				} catch (Exception e) {
					exceptions.Add (e);
				} finally {
					context.SetCompleted ();
				}
			}, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext ());
		}

		public void Execute ()
		{
			if (Count == 0)
				return;

			var savedContext = SynchronizationContext.Current;
			var exceptions = new List<Exception> ();
			try {
				var context = new SingleThreadedSynchronizationContext ();
				SynchronizationContext.SetSynchronizationContext (context);
				ExecuteBuildTasks (context, exceptions);
				context.Run ();
			} finally {
				SynchronizationContext.SetSynchronizationContext (savedContext);
			}
			if (exceptions.Count > 0)
				throw new AggregateException (exceptions);
		}

		public void Dot (Application app, string file)
		{
			var nodes = new HashSet<string> ();
			var queue = new Queue<BuildTask> (this);
			var input_nodes = new HashSet<string> ();
			var action_nodes = new HashSet<string> ();
			var output_nodes = new HashSet<string> ();
			var all_nodes = new HashSet<string> ();
			var all_files = new HashSet<string> ();
			var circular_ref_nodes = new HashSet<string> ();

			var render_file = new Func<string, string> ((v) => {
				if (Path.GetDirectoryName (v).EndsWith (".framework", StringComparison.Ordinal))
					v = Path.GetDirectoryName (v);
				var cache = v.IndexOf (app.Cache.Location, StringComparison.Ordinal);
				if (cache >= 0)
					return v.Substring (app.Cache.Location.Length).TrimStart (Path.DirectorySeparatorChar);

				return Path.GetFileName (v);
			});

			var processed = new HashSet<BuildTask> ();
			while (queue.Count > 0) {
				var task = queue.Dequeue ();
				var action_node = $"\"{task.GetType ().Name}{task.ID}\"";

				if (processed.Contains (task)) {
					Console.WriteLine ($"Already processed: {action_node}");
					continue;
				}

				processed.Add (task);
				foreach (var d in task.Dependencies)
					queue.Enqueue (d);

				nodes.Add ($"{action_node} [label=\"{task.GetType ().Name.Replace ("Task", "")}\", shape=box]");
				all_nodes.Add ($"X{task.ID}");
				action_nodes.Add (action_node);

				var inputs = task.Inputs.ToArray ();
				all_files.UnionWith (inputs);
				for (int i = 0; i < inputs.Length; i++) {
					var node = $"\"{inputs [i]}\"";
					all_nodes.Add (node);
					input_nodes.Add (node);
					nodes.Add ($"{node} -> {action_node}");
				}

				var outputs = task.Outputs.ToArray ();
				all_files.UnionWith (outputs);
				for (int i = 0; i < outputs.Length; i++) {
					var node = $"\"{outputs [i]}\"";
					all_nodes.Add (node);
					output_nodes.Add (node);
					nodes.Add ($"{action_node} -> {node}");
				}
			}

			foreach (var af in all_files) {
				nodes.Add ($"\"{af}\" [label=\"{render_file (af)}\"]");
			}

			using (var writer = new StreamWriter (file)) {
				writer.WriteLine ("digraph build {");
				writer.WriteLine ("\trankdir=LR;");
				foreach (var node in nodes)
					writer.WriteLine ("\t{0};", node);

				// make all the final nodes a different color
				foreach (var end_node in output_nodes.Except (input_nodes))
					writer.WriteLine ($"\t{end_node} [fillcolor = \"lightblue\"; style = \"filled\"; ];");

				foreach (var node in circular_ref_nodes)
					writer.WriteLine ($"\t{node} [fillcolor = \"red\"; style = \"filled\"; ];");

				writer.WriteLine ("}");
			}
			Driver.Log ("Created dot file: {0}", file);
		}
	}

	public abstract class BuildTask {
		static int counter;
		public readonly int ID = counter++;

		TaskCompletionSource<bool> started_task = new TaskCompletionSource<bool> ();
		TaskCompletionSource<bool> completed_task = new TaskCompletionSource<bool> (); // The value determines whether the target was rebuilt (not up-to-date) or not.
		List<BuildTask> dependencies = new List<BuildTask> ();

		[System.Diagnostics.Conditional ("LOG_TASK")]
		void Log (string format, params object [] args)
		{
			Console.WriteLine ($"{ID} {GetType ().Name}: {string.Format (format, args)}");
		}

		// A list of input files (not a list of all the dependencies that would make this task rebuild).
		public abstract IEnumerable<string> Inputs { get; }

		public bool Rebuilt {
			get {
				if (!completed_task.Task.IsCompleted)
					throw ErrorHelper.CreateError (99, Errors.MX0099, "Can't rebuild a task that hasn't completed");
				return completed_task.Task.Result;
			}
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

		public IEnumerable<BuildTask> Dependencies {
			get {
				return dependencies;
			}
		}

		public Task CompletedTask {
			get {
				return completed_task.Task;
			}
		}

		public void AddDependency (params BuildTask [] dependencies)
		{
			if (dependencies is null)
				return;
			this.dependencies.AddRange (dependencies.Where ((v) => v is not null));
		}

		public void AddDependency (IEnumerable<BuildTask> dependencies)
		{
			if (dependencies is null)
				return;
			this.dependencies.AddRange (dependencies.Where ((v) => v is not null));
		}

		public async Task Execute (BuildTasks build_tasks)
		{
			if (started_task.TrySetResult (true)) {
				var watch = new System.Diagnostics.Stopwatch ();
				try {
					Log ("Launching task");
					var deps = Dependencies.ToArray ();
					var dep_tasks = new Task [deps.Length];
					for (int i = 0; i < deps.Length; i++)
						dep_tasks [i] = deps [i].Execute (build_tasks);

					Log ("Waiting for dependencies to complete.");
					await Task.WhenAll (dep_tasks);
					Log ("Done waiting for dependencies.");

					// We can only check if we're up-to-date after executing dependencies.
					if (IsUptodate) {
						if (Outputs.Count () > 1) {
							Driver.Log (3, "Targets '{0}' are up-to-date.", string.Join ("', '", Outputs.ToArray ()));
						} else {
							Driver.Log (3, "Target '{0}' is up-to-date.", Outputs.First ());
						}
						completed_task.SetResult (false);
					} else {
						Driver.Log (3, "Target(s) {0} must be rebuilt.", string.Join (", ", Outputs.ToArray ()));
						Log ("Dependencies are complete.");
						await build_tasks.AcquireSemaphore ();
						try {
							Log ("Executing task");
							watch.Start ();
							await ExecuteAsync ();
							watch.Stop ();
							Log ("Completed task {0} s", watch.Elapsed.TotalSeconds);
							completed_task.SetResult (true);
						} finally {
							build_tasks.ReleaseSemaphore ();
						}
					}
				} catch (Exception e) {
					Log ("Completed task in {0} s with exception: {1}", watch.Elapsed.TotalSeconds, e.Message);
					completed_task.SetException (e);
					throw;
				}
			} else {
				Log ("Waiting for started task");
				await completed_task.Task;
				Log ("Waited for started task");
			}
		}

		// Derived tasks must override either ExecuteAsync or Execute.
		// If ExecuteAsync is not overridden, then Execute is called on
		// a background thread.

		protected virtual Task ExecuteAsync ()
		{
			return Task.Run (() => Execute ());
		}

		protected virtual void Execute ()
		{
			throw ErrorHelper.CreateError (99, Errors.MX0099, "'Either Execute or ExecuteAsync must be overridden'");
		}

		public override string ToString ()
		{
			return GetType ().Name;
		}

		bool reported_5107;
		protected void CheckFor5107 (string assembly_name, string line, List<Exception> exceptions)
		{
			if (line.Contains ("can not encode offset") && line.Contains ("in resulting scattered relocation")) {
				if (!reported_5107) {
					// There can be thousands of these, but we only need one.
					reported_5107 = true;
					exceptions.Add (ErrorHelper.CreateError (5107, Errors.MT5107, assembly_name));
				}
			}
		}

		// Writes a list of lines to stderr, writing only a limited number of lines if there are too many of them.
		protected void WriteLimitedOutput (string first, IEnumerable<string> lines, List<Exception> exceptions)
		{
			if ((first is null || first.Length == 0) && !lines.Any ())
				return;

			if (Driver.Verbosity < 6 && lines.Count () > 1000) {
				lines = lines.Take (1000); // Limit the output so that we don't overload VSfM.
				exceptions.Add (ErrorHelper.CreateWarning (5108, Errors.MT5108));
			}

			// Construct the entire message before writing anything, so that there's a better chance the message isn't
			// mixed up with output from other threads.
			var sb = new StringBuilder ();
			if (first is not null && first.Length > 0)
				sb.AppendLine (first);
			foreach (var line in lines)
				sb.AppendLine (line);
			sb.Length -= Environment.NewLine.Length; // strip off the last newline, since we're adding it in the next line
			Console.Error.WriteLine (sb);
		}
	}

	class SingleThreadedSynchronizationContext : SynchronizationContext {
		readonly BlockingCollection<Tuple<SendOrPostCallback, object>> queue = new BlockingCollection<Tuple<SendOrPostCallback, object>> ();

		public override void Post (SendOrPostCallback d, object state)
		{
			queue.Add (new Tuple<SendOrPostCallback, object> (d, state));
		}

		public override void Send (SendOrPostCallback d, object state)
		{
			d (state);
		}

		public int Run ()
		{
			int counter = 0;

			while (!queue.IsCompleted) {
				var item = queue.Take ();
				counter++;
				item.Item1 (item.Item2);
			}

			return counter;
		}

		public void SetCompleted ()
		{
			queue.CompleteAdding ();
		}
	}
}
