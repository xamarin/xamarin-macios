using System.Collections.Generic;
using System.Linq;

namespace Xharness.Jenkins.TestTasks {
	public static class ITestTaskExtensions {

		public static string GetTestColor (this IEnumerable<ITestTask> tests)
		{
			if (!tests.Any ())
				return "black";

			var first = tests.First ();
			if (tests.All ((v) => v.ExecutionResult == first.ExecutionResult))
				return first.GetTestColor ();
			if (tests.Any ((v) => v.Crashed))
				return "maroon";
			else if (tests.Any ((v) => v.TimedOut))
				return "purple";
			else if (tests.Any ((v) => v.BuildFailure))
				return "darkred";
			else if (tests.Any ((v) => v.Failed))
				return "red";
			else if (tests.Any ((v) => v.NotStarted))
				return "black";
			else if (tests.Any ((v) => v.Ignored))
				return "gray";
			else if (tests.Any ((v) => v.DeviceNotFound))
				return "orangered";
			else if (tests.All ((v) => v.BuildSucceeded))
				return "lightgreen";
			else if (tests.All ((v) => v.Succeeded))
				return "green";
			else
				return "black";
		}

		public static string GetTestColor (this ITestTask test)
		{
			if (test.NotStarted) {
				return "black";
			} else if (test.InProgress) {
				if (test.Building) {
					return "darkblue";
				} else if (test.Running) {
					return "lightblue";
				} else {
					return "blue";
				}
			} else {
				if (test.Crashed) {
					return "maroon";
				} else if (test.HarnessException) {
					return "orange";
				} else if (test.TimedOut) {
					return "purple";
				} else if (test.BuildFailure) {
					return "darkred";
				} else if (test.Failed) {
					return "red";
				} else if (test.BuildSucceeded) {
					return "lightgreen";
				} else if (test.Succeeded) {
					return "green";
				} else if (test.Ignored) {
					return "gray";
				} else if (test.Waiting) {
					return "darkgray";
				} else if (test.DeviceNotFound) {
					return "orangered";
				} else {
					return "pink";
				}
			}
		}
	}
}
