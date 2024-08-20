using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using NUnit.Framework;
using Xamarin.Utils;

namespace Xamarin.Tests {
	class ToolMessage {
		public bool IsError;
		public bool IsWarning { get { return !IsError; } }
		public string Prefix;
		public int Number;
		public string PrefixedNumber { get { return Prefix + Number.ToString (); } }
		public string Message;
		public string FileName;
		public int LineNumber;

		public override string ToString ()
		{
			if (string.IsNullOrEmpty (FileName)) {
				return String.Format ("{0} {3}{1:0000}: {2}", IsError ? "error" : "warning", Number, Message, Prefix);
			} else {
				return String.Format ("{3}({4}): {0} {5}{1:0000}: {2}", IsError ? "error" : "warning", Number, Message, FileName, LineNumber, Prefix);
			}
		}
	}

	static class Extensions {
		public static IEnumerable<ToolMessage> FilterUnrelatedWarnings (this IEnumerable<ToolMessage> messages)
		{
			return messages.Where (msg => {
				if (!msg.IsWarning)
					return true;

				switch (msg.Number) {
				case 4189:
					switch (msg.Message) {
					case "The class 'PassKit.PKDisbursementAuthorizationController' will not be registered it has been removed from the iOS SDK.":
					case "The class 'PassKit.PKDisbursementAuthorizationControllerDelegate' will not be registered it has been removed from the iOS SDK.":
						return false;
					}
					break;
				case 4178:
					switch (msg.Message) {
					case "The class 'NewsstandKit.NKAssetDownload' will not be registered because the NewsstandKit framework has been removed from the iOS SDK.":
					case "The class 'NewsstandKit.NKLibrary' will not be registered because the NewsstandKit framework has been removed from the iOS SDK.":
					case "The class 'NewsstandKit.NKIssue' will not be registered because the NewsstandKit framework has been removed from the iOS SDK.":
					case "The class 'AssetsLibrary.ALAssetRepresentation' will not be registered because the AssetsLibrary framework has been removed from the iOS SDK.":
					case "The class 'AssetsLibrary.ALAssetsFilter' will not be registered because the AssetsLibrary framework has been removed from the iOS SDK.":
					case "The class 'AssetsLibrary.ALAssetsGroup' will not be registered because the AssetsLibrary framework has been removed from the iOS SDK.":
					case "The class 'AssetsLibrary.ALAssetsLibrary' will not be registered because the AssetsLibrary framework has been removed from the iOS SDK.":
					case "The class 'AssetsLibrary.ALAsset' will not be registered because the AssetsLibrary framework has been removed from the iOS SDK.":
						return false;
					}
					break;
				}

				return true;
			});
		}
	}

	abstract class Tool {
		StringBuilder output = new StringBuilder ();

		List<string> output_lines;

		List<ToolMessage> messages = new List<ToolMessage> ();

		public Dictionary<string, string> EnvironmentVariables { get; set; }
		public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds (60);
#pragma warning disable 0649 // Field 'X' is never assigned to, and will always have its default value Y
		public string WorkingDirectory;
#pragma warning restore 0649

		public IEnumerable<ToolMessage> Messages { get { return messages; } }
		public List<string> OutputLines {
			get {
				if (output_lines is null) {
					output_lines = new List<string> ();
					output_lines.AddRange (output.ToString ().Split ('\n'));
				}
				return output_lines;
			}
		}

		public StringBuilder Output {
			get {
				return output;
			}
			set {
				output = value;
			}
		}

		public int Execute (IList<string> arguments)
		{
			return Execute (ToolPath, arguments, false);
		}

		public int Execute (IList<string> arguments, bool always_show_output)
		{
			return Execute (ToolPath, arguments, always_show_output);
		}

		public int Execute (string toolPath, IList<string> arguments)
		{
			return Execute (toolPath, arguments, false);
		}

		public int Execute (string toolPath, IList<string> arguments, bool always_show_output)
		{
			output.Clear ();
			output_lines = null;

			var args = new List<string> ();
			args.Add ("-t");
			args.Add ("--");
			args.Add (toolPath);
			args.AddRange (arguments);
			var rv = ExecutionHelper.Execute (Configuration.XIBuildPath, args, EnvironmentVariables, output, output, workingDirectory: WorkingDirectory);

			if ((rv != 0 || always_show_output) && output.Length > 0)
				Console.WriteLine ("\t" + output.ToString ().Replace ("\n", "\n\t"));

			ParseMessages ();

			return rv;
		}

		static bool IndexOfAny (string line, out int start, out int end, params string [] values)
		{
			foreach (var value in values) {
				start = line.IndexOf (value, StringComparison.Ordinal);
				if (start >= 0) {
					end = start + value.Length;
					return true;
				}
			}
			start = -1;
			end = -1;
			return false;
		}

		static string RemovePathAtEnd (string line)
		{
			if (line.TrimEnd ().EndsWith ("]", StringComparison.Ordinal)) {
				var start = line.LastIndexOf ("[", StringComparison.Ordinal);
				if (start >= 0) {
					// we want to get the space before `[` too.
					if (start > 0 && line [start - 1] == ' ')
						start--;

					line = line.Substring (0, start);
					return line;
				}
			}

			return line;
		}

		public static List<ToolMessage> ParseMessages (string [] lines, string messageToolName)
		{
			var messages = new List<ToolMessage> ();
			ParseMessages (messages, lines, messageToolName);
			return messages;
		}

		public static void ParseMessages (List<ToolMessage> messages, string [] lines, string messageToolName)
		{
			foreach (var l in lines) {
				var line = l;
				var msg = new ToolMessage ();
				var origin = string.Empty;

				if (IndexOfAny (line, out var idxError, out var endError, ": error ", ":  error ")) {
					msg.IsError = true;
					origin = line.Substring (0, idxError);
					line = line.Substring (endError);
					line = RemovePathAtEnd (line);
				} else if (IndexOfAny (line, out var idxWarning, out var endWarning, ": warning ", ":  warning ")) {
					origin = line.Substring (0, idxWarning);
					line = line.Substring (endWarning);
					line = RemovePathAtEnd (line);
				} else if (line.StartsWith ("error ", StringComparison.Ordinal)) {
					msg.IsError = true;
					line = line.Substring (6);
				} else if (line.StartsWith ("warning ", StringComparison.Ordinal)) {
					msg.IsError = false;
					line = line.Substring (8);
				} else {
					// something else
					continue;
				}
				if (line.Length < 7)
					continue; // something else

				msg.Prefix = line.Substring (0, 2);
				if (!int.TryParse (line.Substring (2, 4), out msg.Number))
					continue; // something else

				line = line.Substring (8);
				var toolName = messageToolName;
				if (toolName is not null && line.StartsWith (toolName + ": ", StringComparison.Ordinal))
					line = line.Substring (toolName.Length + 2);

				msg.Message = line;

				if (!string.IsNullOrEmpty (origin)) {
					var idx = origin.IndexOf ('(');
					if (idx > 0) {
						var closing = origin.IndexOf (')');
						var number = 0;
						if (!int.TryParse (origin.Substring (idx + 1, closing - idx - 1), out number))
							continue;
						msg.LineNumber = number;
						msg.FileName = origin.Substring (0, idx);
					} else {
						msg.FileName = origin;
					}
				}

				messages.Add (msg);
			}
		}

		public void ParseMessages ()
		{
			messages.Clear ();
			ParseMessages (messages, output.ToString ().Split ('\n', '\r'), MessageToolName);
		}

		static bool TrySplitCode (string code, out string prefix, out int number)
		{
			prefix = null;
			number = -1;

			if (code is null)
				return false;

			for (var i = 0; i < code.Length; i++) {
				var c = code [i];
				if (c >= '0' && c <= '9') {
					prefix = code.Substring (0, i);
					return int.TryParse (code.Substring (i), out number);
				}
			}

			return false;
		}

		public void ParseBinLog (string binlog)
		{
			messages.Clear ();
			foreach (var buildLogEvent in BinLog.GetBuildMessages (binlog)) {
				// We're only interested in warnings and errors
				if (buildLogEvent.Type != BuildLogEventType.Error && buildLogEvent.Type != BuildLogEventType.Warning)
					continue;

				var msg = new ToolMessage ();

				if (TrySplitCode (buildLogEvent.Code, out var prefix, out var number)) {
					msg.Prefix = prefix;
					msg.Number = number;
				}

				msg.IsError = buildLogEvent.Type == BuildLogEventType.Error;
				msg.Message = buildLogEvent.Message;
				msg.LineNumber = buildLogEvent.LineNumber;
				msg.FileName = buildLogEvent.File;

				messages.Add (msg);
			}
		}

		public bool HasErrorPattern (string prefix, int number, string messagePattern)
		{
			foreach (var msg in messages) {
				if (msg.IsError && msg.Prefix == prefix && msg.Number == number && Regex.IsMatch (msg.Message, messagePattern))
					return true;
			}
			return false;
		}

		public int ErrorCount {
			get {
				return messages.Count ((v) => v.IsError);
			}
		}

		public int WarningCount {
			get {
				return GetWarningCount (messages);
			}
		}

		public static int GetWarningCount (IEnumerable<ToolMessage> messages)
		{
			return messages
				.FilterUnrelatedWarnings ()
				.Count ((v) => v.IsWarning);
		}

		public bool HasError (string prefix, int number, string message)
		{
			foreach (var msg in messages) {
				if (msg.IsError && msg.Prefix == prefix && msg.Number == number && msg.Message == message)
					return true;
			}
			return false;
		}

		public void AssertWarningCount (int count, string message = "warnings")
		{
			AssertWarningCount (messages, count, message);
		}

		public static void AssertWarningCount (IEnumerable<ToolMessage> messages, int count, string message = "warnings")
		{
			if (count != GetWarningCount (messages))
				Assert.Fail ($"{message}\nExpected: {count}\nBut was: {GetWarningCount (messages)}\nWarnings:\n\t{string.Join ("\n\t", messages.Where ((v) => v.IsWarning).Select ((v) => v.ToString ()))}");
		}

		public void AssertErrorCount (int count, string message = "errors")
		{
			Assert.AreEqual (count, ErrorCount, message);
		}

		public void AssertErrorPattern (int number, string messagePattern, string filename = null, int? linenumber = null, bool custom_pattern_syntax = false)
		{
			AssertErrorPattern (MessagePrefix, number, messagePattern, filename, linenumber, custom_pattern_syntax);
		}

		public void AssertErrorPattern (string prefix, int number, string messagePattern, string filename = null, int? linenumber = null, bool custom_pattern_syntax = false)
		{
			if (!messages.Any ((msg) => msg.Prefix == prefix && msg.Number == number))
				Assert.Fail (string.Format ("The error '{0}{1:0000}' was not found in the output.", prefix, number));

			// Custom pattern syntax: escape parenthesis and brackets so that they're treated like normal characters.
			var processedPattern = custom_pattern_syntax ? messagePattern.Replace ("(", "[(]").Replace (")", "[)]").Replace ("[]", "[[][]]") + "$" : messagePattern;
			var matches = messages.Where ((msg) => Regex.IsMatch (msg.Message, processedPattern));
			if (!matches.Any ()) {
				var details = messages.Where ((msg) => msg.Prefix == prefix && msg.Number == number && !Regex.IsMatch (msg.Message, processedPattern)).Select ((msg) => string.Format ("\tThe message '{0}' did not match the pattern '{1}'.", msg.Message, messagePattern));
				Assert.Fail (string.Format ("The error '{0}{1:0000}: {2}' was not found in the output:\n{3}", prefix, number, messagePattern, string.Join ("\n", details.ToArray ())));
			}

			AssertFilename (prefix, number, messagePattern, matches, filename, linenumber);
		}

		public void AssertError (int number, string message, string filename = null, int? linenumber = null)
		{
			AssertError (MessagePrefix, number, message, filename, linenumber);
		}

		public void AssertError (string prefix, int number, string message, string filename = null, int? linenumber = null)
		{
			if (!messages.Any ((msg) => msg.Prefix == prefix && msg.Number == number))
				Assert.Fail (string.Format ("The error '{0}{1:0000}' was not found in the output.\nFound {2}i:\n", prefix, number, string.Join ("\n", messages)));

			var matches = messages.Where ((msg) => msg.Message == message);
			if (!matches.Any ()) {
				var details = messages.
									  Where ((msg) => msg.Prefix == prefix && msg.Number == number && msg.Message != message).
									  Select ((msg) => string.Format ("\tMessage #{2} did not match:\n\t\tactual:   '{0}'\n\t\texpected: '{1}'", msg.Message, message, messages.IndexOf (msg) + 1));
				Assert.Fail (string.Format ("The error '{0}{1:0000}: {2}' was not found in the output:\n{3}", prefix, number, message, string.Join ("\n", details.ToArray ())));
			}

			AssertFilename (prefix, number, message, matches, filename, linenumber);
		}

		void AssertFilename (string prefix, int number, string message, IEnumerable<ToolMessage> matches, string filename, int? linenumber)
		{
			AssertFilename (messages, prefix, number, message, matches, filename, linenumber);
		}

		static void AssertFilename (IList<ToolMessage> messages, string prefix, int number, string message, IEnumerable<ToolMessage> matches, string filename, int? linenumber)
		{
			if (filename is not null) {
				var hasDirectory = filename.IndexOf (Path.DirectorySeparatorChar) > -1;
				if (!matches.Any ((v) => {
					if (hasDirectory) {
						// Check the entire path
						return filename == v.FileName;
					} else {
						// Don't compare the directory unless one was specified.
						return filename == Path.GetFileName (v.FileName);
					}
				})) {
					var details = matches.Select ((msg) => string.Format ("\tMessage #{2} did not contain expected filename:\n\t\tactual:   '{0}'\n\t\texpected: '{1}'", hasDirectory ? msg.FileName : Path.GetFileName (msg.FileName), filename, messages.IndexOf (msg) + 1));
					Assert.Fail (string.Format ($"The filename '{filename}' was not found in the output for the error {prefix}{number:X4}: {message}:\n{string.Join ("\n", details.ToArray ())}"));
				}
			}

			if (linenumber is not null) {
				if (!matches.Any ((v) => linenumber.Value == v.LineNumber)) {
					var details = matches.Select ((msg) => string.Format ("\tMessage #{2} did not contain expected line number:\n\t\tactual:   '{0}'\n\t\texpected: '{1}'", msg.LineNumber, linenumber, messages.IndexOf (msg) + 1));
					Assert.Fail (string.Format ($"The linenumber '{linenumber.Value}' was not found in the output for the error {prefix}{number:X4}: {message}:\n{string.Join ("\n", details.ToArray ())}"));
				}
			}
		}

		public void AssertWarningPattern (int number, string messagePattern)
		{
			AssertWarningPattern (MessagePrefix, number, messagePattern);
		}

		public void AssertWarningPattern (string prefix, int number, string messagePattern)
		{
			AssertWarningPattern (messages, prefix, number, messagePattern);
		}

		public static void AssertWarningPattern (IEnumerable<ToolMessage> messages, string prefix, int number, string messagePattern)
		{
			if (!messages.Any ((msg) => msg.Prefix == prefix && msg.Number == number))
				Assert.Fail (string.Format ("The warning '{0}{1:0000}' was not found in the output.", prefix, number));

			if (messages.Any ((msg) => Regex.IsMatch (msg.Message, messagePattern)))
				return;

			var details = messages.Where ((msg) => msg.Prefix == prefix && msg.Number == number && !Regex.IsMatch (msg.Message, messagePattern)).Select ((msg) => string.Format ("\tThe message '{0}' did not match the pattern '{1}'.", msg.Message, messagePattern));
			Assert.Fail (string.Format ("The warning '{0}{1:0000}: {2}' was not found in the output:\n{3}", prefix, number, messagePattern, string.Join ("\n", details.ToArray ())));
		}

		public void AssertWarning (int number, string message, string filename = null, int? linenumber = null)
		{
			AssertWarning (MessagePrefix, number, message, filename, linenumber);
		}

		public void AssertWarning (string prefix, int number, string message, string filename = null, int? linenumber = null)
		{
			AssertWarning (messages, prefix, number, message, filename, linenumber);
		}

		public static void AssertWarning (IList<ToolMessage> messages, string prefix, int number, string message, string filename = null, int? linenumber = null)
		{
			if (!messages.Any ((msg) => msg.Prefix == prefix && msg.Number == number))
				Assert.Fail (string.Format ("The warning '{0}{1:0000}' was not found in the output.", prefix, number));

			var matches = messages.Where ((msg) => msg.Message == message);
			if (!matches.Any ()) {
				var details = messages.Where ((msg) => msg.Prefix == prefix && msg.Number == number && msg.Message != message).Select ((msg) => string.Format ("\tMessage #{2} did not match:\n\t\tactual:   '{0}'\n\t\texpected: '{1}'", msg.Message, message, messages.IndexOf (msg) + 1));
				Assert.Fail (string.Format ("The warning '{0}{1:0000}: {2}' was not found in the output:\n{3}", prefix, number, message, string.Join ("\n", details.ToArray ())));
			}

			AssertFilename (messages, prefix, number, message, matches, filename, linenumber);
		}

		public void AssertNoWarnings ()
		{
			var warnings = messages
				.FilterUnrelatedWarnings ()
				.Where ((v) => v.IsWarning);
			if (!warnings.Any ())
				return;

			Assert.Fail ("No warnings expected, but got:\n{0}\t", string.Join ("\n\t", warnings.Select ((v) => v.Message).ToArray ()));
		}

		public void AssertNoMessage (int number)
		{
			var msgs = messages.Where ((v) => v.Number == number);
			if (!msgs.Any ())
				return;

			Assert.Fail ("No messages with number {0} expected, but got:\n{1}\t", number, string.Join ("\n\t", msgs.Select ((v) => v.Message).ToArray ()));
		}

		public bool HasOutput (string line)
		{
			return OutputLines.Contains (line);
		}

		public bool HasOutputPattern (string linePattern)
		{
			foreach (var line in OutputLines) {
				if (Regex.IsMatch (line, linePattern, RegexOptions.CultureInvariant))
					return true;
			}

			return false;
		}

		public void AssertOutputPattern (string linePattern)
		{
			if (!HasOutputPattern (linePattern))
				Assert.Fail (string.Format ("The output does not contain the line '{0}'", linePattern));
		}

		public void ForAllOutputLines (Action<string> action)
		{
			foreach (var line in OutputLines)
				action (line);
		}

		protected abstract string ToolPath { get; }
		protected abstract string MessagePrefix { get; }
		protected virtual string MessageToolName { get { return null; } }
	}
}
