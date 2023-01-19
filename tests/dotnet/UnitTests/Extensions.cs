using System;

#nullable enable

namespace Xamarin.Tests {
	public static class Extensions {
		public static void AssertNoWarnings (this ExecutionResult result)
		{
			var warnings = BinLog.GetBuildLogWarnings (result.BinLogPath).ToArray ();
			if (!warnings.Any ())
				return;

			Assert.Fail ($"No warnings expected, but got:\n\t{string.Join ("\n\t", warnings.Select (v => v.ToString ()))}");
		}
	}
}
