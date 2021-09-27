#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Build.Framework;
using Microsoft.Build.Logging;
using Microsoft.Build.Logging.StructuredLogger;

#nullable enable

namespace Xamarin.Tests
{
	public class BuildLogEvent {
		public BuildLogEventType Type;

		public int ColumnNumber;
		public int EndColumnNumber;
		public int LineNumber;
		public int EndLineNumber;
		public string? Code;
		public string? SubCategory;
		public string? File;
		public string? ProjectFile;
		public string? Message;
	}

	public enum BuildLogEventType {
		Message,
		Warning,
		Error,
	}

	public class TargetExecutionResult {
		public string TargetName;
		public bool Skipped;
		public TargetSkipReason SkipReason;

		public TargetExecutionResult (string targetName, bool skipped, TargetSkipReason skipReason)
		{
			TargetName = targetName;
			Skipped = skipped;
			SkipReason = skipReason;
		}
	}

	public class BinLog {

		public static IEnumerable<TargetExecutionResult> GetAllTargets (string path)
		{
			var buildEvents = ReadBuildEvents (path).ToArray ();
			var targetsStarted = buildEvents.OfType<TargetStartedEventArgs> ();
			foreach (var target in targetsStarted) {
				var id = target.BuildEventContext.TargetId;
				if (id == -1)
					throw new InvalidOperationException ($"Target '{target.TargetName}' started but no id?");
				var eventsForTarget = buildEvents.Where (v => v.BuildEventContext?.TargetId == id);
				var skippedEvent = eventsForTarget.OfType<TargetSkippedEventArgs> ().FirstOrDefault ();
				var skipReason = (skippedEvent as TargetSkippedEventArgs2)?.SkipReason ?? TargetSkipReason.None;
				yield return new TargetExecutionResult (target.TargetName, skippedEvent != null, skipReason);
			}
		}

		public static IEnumerable<BuildEventArgs> ReadBuildEvents (string path)
		{
			var reader = new BinLogReader ();
			foreach (var record in reader.ReadRecords (path)) {
				if (record is null)
					continue;

				if (record.Args is null)
					continue;

				yield return record.Args;
			}
		}

		// Returns a diagnostic build log as an enumeration of lines
		public static IEnumerable<string> PrintToLines (string path)
		{
			var eols = new char [] { '\n', '\r' };
			foreach (var args in ReadBuildEvents (path)) {
				if (args.Message == null)
					continue;

				if (args is ProjectStartedEventArgs psea) {
					if (psea.Properties != null) {
						yield return "Initial Properties";
						var dict =  psea.Properties as IDictionary<string, string>;
						if (dict == null) {
							yield return $"Unknown property dictionary type: {psea.Properties.GetType ().FullName}";
						} else { 
							foreach (var prop in dict.OrderBy (v => v.Key))
								yield return $"{prop.Key} = {prop.Value}";
						}
					}
				}

				if (args is TaskParameterEventArgs tpea) {
					switch (tpea.Kind) {
					case TaskParameterMessageKind.AddItem:
						yield return "Added Item(s)";
						break;
					case TaskParameterMessageKind.RemoveItem:
						yield return "Removed Item(s)";
						break;
					case TaskParameterMessageKind.TaskInput:
						yield return "Task Parameter";
						break;
					case TaskParameterMessageKind.TaskOutput:
						yield return "Output Item(s)";
						break;
					default:
						yield return $"Unknown Kind ({tpea.Kind})";
						break;
					}
					foreach (var item in tpea.Items) {
						var taskItem = item as ITaskItem;
						yield return $"\t{tpea.ItemType}=";
						if (taskItem != null) {
							yield return $"\t\t{taskItem.ItemSpec}";
							foreach (var metadataName in taskItem.MetadataNames) {
								yield return $"\t\t\t{metadataName}={taskItem.GetMetadata (metadataName?.ToString ())}";
							}
						} else {
							yield return $"\t{item}";
						}
					}
					continue;
				}

				foreach (var line in args.Message.Split (eols, System.StringSplitOptions.RemoveEmptyEntries))
					yield return line;
			}
		}

		public static IEnumerable<BuildLogEvent> GetBuildLogWarnings (string path)
		{
			return GetBuildMessages (path).Where (v => v.Type == BuildLogEventType.Warning);
		}

		public static IEnumerable<BuildLogEvent> GetBuildLogErrors (string path)
		{
			return GetBuildMessages (path).Where (v => v.Type == BuildLogEventType.Error);
		}

		public static IEnumerable<BuildLogEvent> GetBuildMessages (string path)
		{
			var reader = new BinLogReader ();
			var eols = new char [] { '\n', '\r' };
			foreach (var record in reader.ReadRecords (path)) {
				if (record == null)
					continue;
				var args = record.Args;
				if (args == null)
					continue;

				if (args is BuildErrorEventArgs buildError) {
					var ea = buildError;
					yield return new BuildLogEvent {
						Type = BuildLogEventType.Error,
						File = ea.File,
						LineNumber = ea.LineNumber,
						EndLineNumber = ea.EndLineNumber,
						ColumnNumber = ea.ColumnNumber,
						EndColumnNumber = ea.EndColumnNumber,
						Message = ea.Message,
						ProjectFile = ea.ProjectFile,
						Code = ea.Code,
						SubCategory = ea.Subcategory,
					};
				} else if (args is BuildWarningEventArgs buildWarning) {
					var ea = buildWarning;
					yield return new BuildLogEvent {
						Type = BuildLogEventType.Warning,
						File = ea.File,
						LineNumber = ea.LineNumber,
						EndLineNumber = ea.EndLineNumber,
						ColumnNumber = ea.ColumnNumber,
						EndColumnNumber = ea.EndColumnNumber,
						Message = ea.Message,
						ProjectFile = ea.ProjectFile,
						Code = ea.Code,
						SubCategory = ea.Subcategory,
					};
				} else if (args is BuildMessageEventArgs buildMessage) {
					var ea = buildMessage;
					yield return new BuildLogEvent {
						Type = BuildLogEventType.Message,
						File = ea.File,
						LineNumber = ea.LineNumber,
						EndLineNumber = ea.EndLineNumber,
						ColumnNumber = ea.ColumnNumber,
						EndColumnNumber = ea.EndColumnNumber,
						Message = ea.Message,
						ProjectFile = ea.ProjectFile,
						Code = ea.Code,
						SubCategory = ea.Subcategory,
					};
				}
			}
		}

		// Returns a diagnostic build log as a string
		public static string PrintToString (string path)
		{
			var sb = new StringBuilder ();
			foreach (var line in PrintToLines (path))
				sb.AppendLine (line);
			return sb.ToString ();
		}
	}
}
