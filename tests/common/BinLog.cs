using System.Collections.Generic;
using System.Text;

using Microsoft.Build.Logging.StructuredLogger;

#nullable enable

namespace Xamarin.Tests
{
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

				foreach (var line in args.Message.Split (eols, System.StringSplitOptions.RemoveEmptyEntries))
					yield return line;
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