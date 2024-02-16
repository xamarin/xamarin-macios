using System;

#nullable enable

namespace Xamarin.Tests {
	public static class Extensions {
		public static void AssertNoWarnings (this ExecutionResult result, Func<BuildLogEvent, bool>? filter = null)
		{
			var warnings = BinLog.GetBuildLogWarnings (result.BinLogPath);
			if (filter is not null)
				warnings = warnings.Where (filter);
			if (!warnings.Any ())
				return;

			Assert.Fail ($"No warnings expected, but got:\n\t{string.Join ("\n\t", warnings.Select (v => v.ToString ()))}");
		}

		public static void AssertWarnings (this IEnumerable<BuildLogEvent> actualWarnings, IEnumerable<ExpectedBuildMessage> expectedWarnings)
		{
			// Source paths may be full (and local) paths. So make full paths relative to the root of the repository.
			actualWarnings = actualWarnings.Select (w => {
				if (w.File?.StartsWith (Configuration.SourceRoot) == true) {
					var rv = w.Clone ();
					rv.File = w.File [(Configuration.SourceRoot.Length + 1)..];
					return rv;
				}
				return w;
			});

			var newWarnings = actualWarnings.Where (v => !expectedWarnings.Any (x => x.IsMatch (v))).ToArray ();
			var missingWarnings = expectedWarnings.Where (v => !actualWarnings.Any (x => v.IsMatch (x))).ToArray ();

			if (newWarnings.Length == 0 && missingWarnings.Length == 0)
				return;

			var sb = new StringBuilder ();
			sb.AppendLine ($"\t\t\t\texpectedWarnings = new ExpectedBuildMessage [] {{");
			foreach (var w in actualWarnings.OrderBy (v => v.File).ThenBy (v => v.Message)) {
				sb.AppendLine ($"\t\t\t\t\tnew ExpectedBuildMessage (\"{w.File}\" /* line {w.LineNumber} */, \"{w.Message}\"),");
			}
			sb.AppendLine ($"\t\t\t\t}};");
			if (newWarnings.Length > 0) {
				Console.WriteLine ($"Got {newWarnings.Length} new warnings:");
				Console.WriteLine ();
				foreach (var evt in newWarnings)
					Console.WriteLine ($"    {evt.File}:{evt.LineNumber} {evt.Message}");
			}
			if (missingWarnings.Length > 0) {
				Console.WriteLine ($"Did not get {missingWarnings.Length} missing warnings:");
				Console.WriteLine ();
				foreach (var evt in missingWarnings)
					Console.WriteLine ($"    {evt.File}: {evt.Message}");
			}
			Console.WriteLine ($"If this is expected, here's the updated list of expected warnings:");
			Console.WriteLine (sb);

			// Rather than doing an Assert.IsEmpty, which produces a horrendous error message, we'll do an Assert.Multiple which generates a 
			// nice enumerated output of all the failures.
			Assert.Multiple (() => {
				// fail for each of the new warnings
				foreach (var evt in newWarnings)
					Assert.Fail ($"Unexpected warning: {evt.File}:{evt.LineNumber} {evt.Message}");

				// fail for each of the missing warnings
				foreach (var evt in missingWarnings)
					Assert.Fail ($"Missing warning: {evt.File}: {evt.Message}");
			});
		}
	}

	public class ExpectedBuildMessage {
		public string File;
		public string Message;

		public ExpectedBuildMessage (string file, string message)
		{
			File = file;
			Message = message;
		}

		public bool IsMatch (BuildLogEvent evt)
		{
			if (evt.Message != Message)
				return false;
			if (evt.File != File)
				return false;
			return true;
		}

		public override string? ToString ()
		{
			return $"{File}: {Message}";
		}
	}
}
