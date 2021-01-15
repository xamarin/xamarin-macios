using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Build.Framework;
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

	public class BinLog {

		// Returns a diagnostic build log as an enumeration of lines
		public static IEnumerable<string> PrintToLines (string path)
		{
			var reader = new BinLogReader ();
			var eols = new char [] { '\n', '\r' };
			foreach (var record in reader.ReadRecords (path)) {
				if (record == null)
					continue;
				var args = record.Args;
				if (args == null)
					continue;

				if (args.Message == null)
					continue;

				if (args is ProjectStartedEventArgs psea) {
					if (psea.Properties != null) {
						yield return "Initial Properties";
						foreach (var prop in psea.Properties.Cast<System.Collections.DictionaryEntry> ().OrderBy (v => v.Key))
							yield return $"{prop.Key} = {prop.Value}";
					}
				}

				foreach (var line in args.Message.Split (eols, System.StringSplitOptions.RemoveEmptyEntries))
					yield return line;
			}
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
