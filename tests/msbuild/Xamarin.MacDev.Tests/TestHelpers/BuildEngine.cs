using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Xamarin.Utils;
using PathUtils = Xamarin.Utils.PathUtils;

using Microsoft.Build.Framework;
using Microsoft.Build.Logging.StructuredLogger;

#nullable enable

namespace Xamarin.Tests {
	public class BuildEngine {
		public Dictionary<string, string> Properties { get; private set; } = new Dictionary<string, string> ();

		public void SetGlobalProperty (string name, string value)
		{
			Properties [name] = value;
		}

		public List<LogEvent> ErrorEvents = new List<LogEvent> ();
		public List<LogEvent> WarningsEvents = new List<LogEvent> ();
		public List<LogEvent> MessageEvents = new List<LogEvent> ();

		public BuildEngine Logger => this;
		public BuildEngine ProjectCollection => this;

		ExecutionResult? executionResult;

		public ExecutionResult RunTarget (ApplePlatform platform, ExecutionMode mode, string project, string target, Dictionary<string, string>? properties = null)
		{
			ExecutionResult rv;

			var props = new Dictionary<string, string> (Properties);
			if (properties is not null) {
				foreach (var kvp in properties)
					props [kvp.Key] = kvp.Value;
			}

			switch (mode) {
			case ExecutionMode.MSBuild:
				rv = MSBuild (platform, project, target, props);
				break;
			case ExecutionMode.DotNet:
				rv = Dotnet (project, "Build", target, props);
				break;
			default:
				throw new InvalidOperationException ($"Unknown execution mode: {mode}");
			}

			executionResult = rv;

			Console.WriteLine ($"Binlog: {rv.BinLogPath}");

			if (File.Exists (rv.BinLogPath))
				ParseBinLog (rv.BinLogPath);

			return rv;
		}

		MSBuildItem CreateItem (Item item)
		{
			var msbuildItem = new MSBuildItem {
				EvaluatedInclude = item.Text,
			};
			var metadata = item.Children.Where (v => v is Metadata).Cast<Metadata> ();
			foreach (var m in metadata) {
				msbuildItem.Metadata [m.Name] = m.Value;
			}
			return msbuildItem;
		}

		public MSBuildItem [] GetItems (ProjectPaths project, string name)
		{
			if (executionResult is null)
				throw new InvalidOperationException ($"Must build something first");

			var build = BinaryLog.ReadBuild (executionResult.BinLogPath);

			var rv = new List<MSBuildItem> ();

			// Items inside ItemGroups
			var items = build.FindChildrenRecursive<Item> (v => {
				var parent = v.Parent as NamedNode;
				if (parent?.Name != name)
					return false;

				if (!(parent is Folder || (parent is AddItem && parent.Parent is Folder)))
					return false;

				parent = parent.Parent as NamedNode;
				if (parent?.Name == "OutputItems")
					return false;

				return true;
			});
			foreach (var item in items) {
				rv.Add (CreateItem (item));
			}

			// Items as output from tasks
			var resolvedProjectPath = PathUtils.ResolveSymbolicLinks (project.ProjectCSProjPath);
			var outputItems = build.FindChildrenRecursive<Item> (v => {
				var parent = v.Parent as NamedNode;
				if (parent?.Name != name)
					return false;

				if (!(parent is Parameter || parent is AddItem))
					return false;

				parent = parent.Parent as NamedNode;
				if (parent?.Name != "OutputItems" || !(parent is Folder))
					return false;

				// There can be output from multiple projects, make sure we filter to the
				// project we're interested in.
				var target = parent.GetNearestParent<Target> ();
				var projectPath = PathUtils.ResolveSymbolicLinks (target.Project.ProjectFile);
				return projectPath == resolvedProjectPath;
			});

			foreach (var item in outputItems) {
				rv.Add (CreateItem (item));
			}

			return rv.ToArray ();
		}

		public string GetPropertyValue (string name)
		{
			if (executionResult is null)
				throw new InvalidOperationException ($"Must build something first");

			var build = BinaryLog.ReadBuild (executionResult.BinLogPath);
			var props = build.FindChildrenRecursive<Property> (v => v.Name == name);
			return props.LastOrDefault ()?.Value ?? string.Empty;
		}

		public void ParseBinLog (string log)
		{
			ErrorEvents.Clear ();
			WarningsEvents.Clear ();
			MessageEvents.Clear ();

			foreach (var args in BinLog.ReadBuildEvents (log)) {
				bool verbose = false;
				if (args is TaskStartedEventArgs tsea) {
					if (verbose)
						Console.WriteLine ($"TaskStartedEventArgs: {tsea.TaskName}");
				} else if (args is TaskFinishedEventArgs tfea) {
					if (verbose)
						Console.WriteLine ($"TaskFinished: {tfea.TaskName}");
				} else if (args is TargetStartedEventArgs targetStarted) {
					if (verbose)
						Console.WriteLine ($"TargetStartedEventArgs: {targetStarted.TargetName}");
				} else if (args is TargetFinishedEventArgs targetFinished) {
					if (verbose)
						Console.WriteLine ($"TargetFinishedEventArgs: {targetFinished.TargetName} Outputs: {targetFinished.TargetOutputs}");
				} else if (args is BuildStartedEventArgs buildStarted) {
					if (verbose)
						Console.WriteLine ($"BuildStarted: {buildStarted.Message}");
				} else if (args is BuildFinishedEventArgs buildFinished) {
					if (verbose)
						Console.WriteLine ($"BuildFinished: {buildFinished.Message}");
				} else if (args is ProjectEvaluationStartedEventArgs projectEvaluationStarted) {
					if (verbose)
						Console.WriteLine ($"ProjectEvaluationStarted: {projectEvaluationStarted.ProjectFile}");
				} else if (args.GetType ().Name == "ProjectEvaluationFinishedEventArgs") {
					if (verbose)
						Console.WriteLine ($"ProjectEvaluationFinished: {args}");
				} else if (args is ProjectStartedEventArgs projectStarted) {
					if (verbose)
						Console.WriteLine ($"ProjectStarted: {projectStarted.ProjectFile}");
				} else if (args is ProjectFinishedEventArgs projectFinished) {
					if (verbose)
						Console.WriteLine ($"ProjectFinished: {projectFinished.ProjectFile}");
				} else if (args is BuildErrorEventArgs buildError) {
					if (verbose)
						Console.WriteLine ($"❌ BuildError: {buildError.Message}");
					var ea = buildError;
					ErrorEvents.Add (new LogEvent {
						File = ea.File,
						LineNumber = ea.LineNumber,
						EndLineNumber = ea.EndLineNumber,
						ColumnNumber = ea.ColumnNumber,
						EndColumnNumber = ea.EndColumnNumber,
						Message = ea.Message,
						ProjectFile = ea.ProjectFile,
						Code = ea.Code,
						SubCategory = ea.Subcategory,
					});
				} else if (args is BuildWarningEventArgs buildWarning) {
					if (verbose)
						Console.WriteLine ($"⚠️ BuildWarning: {buildWarning.Message}");
					var ea = buildWarning;
					WarningsEvents.Add (new LogEvent {
						File = ea.File,
						LineNumber = ea.LineNumber,
						EndLineNumber = ea.EndLineNumber,
						ColumnNumber = ea.ColumnNumber,
						EndColumnNumber = ea.EndColumnNumber,
						Message = ea.Message,
						ProjectFile = ea.ProjectFile,
						Code = ea.Code,
						SubCategory = ea.Subcategory,
					});
				} else if (args is BuildMessageEventArgs buildMessage) {
					if (verbose)
						Console.WriteLine ($"ℹ️ BuildMessage: {buildMessage.Message}");
					var ea = buildMessage;
					MessageEvents.Add (new LogEvent {
						File = ea.File,
						LineNumber = ea.LineNumber,
						EndLineNumber = ea.EndLineNumber,
						ColumnNumber = ea.ColumnNumber,
						EndColumnNumber = ea.EndColumnNumber,
						Message = ea.Message,
						ProjectFile = ea.ProjectFile,
						Code = ea.Code,
						SubCategory = ea.Subcategory,
					});
				} else {
					Console.WriteLine ($"‼️ Unknown record type: {args.GetType ().FullName}");
				}
			}
		}

		static ExecutionResult Dotnet (string project, string command, string target, Dictionary<string, string> properties)
		{
			return DotNet.Execute (command, project, properties, assert_success: false, target: target);
		}

		static ExecutionResult MSBuild (ApplePlatform platform, string project, string target, Dictionary<string, string> properties)
		{
			if (!File.Exists (project))
				throw new FileNotFoundException ($"The project file '{project}' does not exist.");

			var args = new List<string> ();
			args.Add ("--");
			args.Add ($"/t:{target}");
			args.Add (project);
			if (properties is not null) {
				foreach (var prop in properties)
					args.Add ($"/p:{prop.Key}={prop.Value}");
			}
			var binlog = Path.Combine (Path.GetDirectoryName (project), $"log-{target}-{DateTime.Now:yyyyMMdd_HHmmss}.binlog");
			args.Add ($"/bl:{binlog}");

			var output = new StringBuilder ();
			var executable = Configuration.XIBuildPath;
			var rv = Execution.RunWithStringBuildersAsync (executable, args, Configuration.GetBuildEnvironment (platform), output, output, Console.Out, workingDirectory: Path.GetDirectoryName (project), timeout: TimeSpan.FromMinutes (10)).Result;
			return new ExecutionResult (output, output, rv.ExitCode) {
				BinLogPath = binlog,
			};
		}
	}
}
