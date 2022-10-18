using System.Collections;
using System.Collections.Generic;
using Microsoft.DotNet.XHarness.iOS.Shared;

using Moq;
using NUnit.Framework;

using Xharness.Jenkins.TestTasks;

namespace Xharness.Tests.Jenkins {

	[TestFixture]
	public class ITestTaskExtensionsTests {

		public class TestCasesData {
			public static IEnumerable GetColorTestCases {
				get {
					var task = new Mock<ITestTask> ();
					task.Setup (t => t.NotStarted).Returns (true);
					yield return new TestCaseData (task.Object).Returns ("black");

					task = new Mock<ITestTask> ();
					task.Setup (t => t.InProgress).Returns (true);
					task.Setup (t => t.Building).Returns (true);
					yield return new TestCaseData (task.Object).Returns ("darkblue");

					task = new Mock<ITestTask> ();
					task.Setup (t => t.InProgress).Returns (true);
					task.Setup (t => t.Building).Returns (false);
					task.Setup (t => t.Running).Returns (true);
					yield return new TestCaseData (task.Object).Returns ("lightblue");

					task = new Mock<ITestTask> ();
					task.Setup (t => t.InProgress).Returns (true);
					task.Setup (t => t.Building).Returns (false);
					task.Setup (t => t.Running).Returns (false);
					yield return new TestCaseData (task.Object).Returns ("blue");

					task = new Mock<ITestTask> ();
					task.Setup (t => t.InProgress).Returns (false);
					task.Setup (t => t.Crashed).Returns (true);
					yield return new TestCaseData (task.Object).Returns ("maroon");

					task = new Mock<ITestTask> ();
					task.Setup (t => t.InProgress).Returns (false);
					task.Setup (t => t.HarnessException).Returns (true);
					yield return new TestCaseData (task.Object).Returns ("orange");

					task = new Mock<ITestTask> ();
					task.Setup (t => t.InProgress).Returns (false);
					task.Setup (t => t.TimedOut).Returns (true);
					yield return new TestCaseData (task.Object).Returns ("purple");

					task = new Mock<ITestTask> ();
					task.Setup (t => t.InProgress).Returns (false);
					task.Setup (t => t.BuildFailure).Returns (true);
					yield return new TestCaseData (task.Object).Returns ("darkred");

					task = new Mock<ITestTask> ();
					task.Setup (t => t.InProgress).Returns (false);
					task.Setup (t => t.Failed).Returns (true);
					yield return new TestCaseData (task.Object).Returns ("red");

					task = new Mock<ITestTask> ();
					task.Setup (t => t.InProgress).Returns (false);
					task.Setup (t => t.BuildSucceeded).Returns (true);
					yield return new TestCaseData (task.Object).Returns ("lightgreen");

					task = new Mock<ITestTask> ();
					task.Setup (t => t.InProgress).Returns (false);
					task.Setup (t => t.Succeeded).Returns (true);
					yield return new TestCaseData (task.Object).Returns ("green");

					task = new Mock<ITestTask> ();
					task.Setup (t => t.InProgress).Returns (false);
					task.Setup (t => t.Ignored).Returns (true);
					yield return new TestCaseData (task.Object).Returns ("gray");

					task = new Mock<ITestTask> ();
					task.Setup (t => t.InProgress).Returns (false);
					task.Setup (t => t.Waiting).Returns (true);
					yield return new TestCaseData (task.Object).Returns ("darkgray");

					task = new Mock<ITestTask> ();
					task.Setup (t => t.InProgress).Returns (false);
					task.Setup (t => t.DeviceNotFound).Returns (true);
					yield return new TestCaseData (task.Object).Returns ("orangered");

					task = new Mock<ITestTask> ();
					task.Setup (t => t.InProgress).Returns (false);
					yield return new TestCaseData (task.Object).Returns ("pink");
				}
			}

			public static IEnumerable GetColorCollectionTestCases {
				get {

					Mock<ITestTask> firstTask = new Mock<ITestTask> ();
					firstTask.Setup (t => t.Succeeded).Returns (true);
					firstTask.Setup (t => t.ExecutionResult).Returns (TestExecutingResult.Succeeded);
					Mock<ITestTask> secondTask = new Mock<ITestTask> ();
					secondTask.Setup (t => t.Crashed).Returns (true);
					secondTask.Setup (t => t.ExecutionResult).Returns (TestExecutingResult.Crashed);
					yield return new TestCaseData (new List<ITestTask> { firstTask.Object, secondTask.Object }).Returns ("maroon");

					firstTask = new Mock<ITestTask> ();
					firstTask.Setup (t => t.Succeeded).Returns (true);
					firstTask.Setup (t => t.ExecutionResult).Returns (TestExecutingResult.Succeeded);
					secondTask = new Mock<ITestTask> ();
					secondTask.Setup (t => t.TimedOut).Returns (true);
					secondTask.Setup (t => t.ExecutionResult).Returns (TestExecutingResult.TimedOut);
					yield return new TestCaseData (new List<ITestTask> { firstTask.Object, secondTask.Object }).Returns ("purple");

					firstTask = new Mock<ITestTask> ();
					firstTask.Setup (t => t.Succeeded).Returns (true);
					firstTask.Setup (t => t.ExecutionResult).Returns (TestExecutingResult.Succeeded);
					secondTask = new Mock<ITestTask> ();
					secondTask.Setup (t => t.BuildFailure).Returns (true);
					secondTask.Setup (t => t.ExecutionResult).Returns (TestExecutingResult.BuildFailure);
					yield return new TestCaseData (new List<ITestTask> { firstTask.Object, secondTask.Object }).Returns ("darkred");

					firstTask = new Mock<ITestTask> ();
					firstTask.Setup (t => t.Succeeded).Returns (true);
					firstTask.Setup (t => t.ExecutionResult).Returns (TestExecutingResult.Succeeded);
					secondTask = new Mock<ITestTask> ();
					secondTask.Setup (t => t.Failed).Returns (true);
					secondTask.Setup (t => t.ExecutionResult).Returns (TestExecutingResult.Failed);
					yield return new TestCaseData (new List<ITestTask> { firstTask.Object, secondTask.Object }).Returns ("red");

					firstTask = new Mock<ITestTask> ();
					firstTask.Setup (t => t.Succeeded).Returns (true);
					firstTask.Setup (t => t.ExecutionResult).Returns (TestExecutingResult.Succeeded);
					secondTask = new Mock<ITestTask> ();
					secondTask.Setup (t => t.NotStarted).Returns (true);
					secondTask.Setup (t => t.ExecutionResult).Returns (TestExecutingResult.NotStarted);
					yield return new TestCaseData (new List<ITestTask> { firstTask.Object, secondTask.Object }).Returns ("black");

					firstTask = new Mock<ITestTask> ();
					firstTask.Setup (t => t.Succeeded).Returns (true);
					firstTask.Setup (t => t.ExecutionResult).Returns (TestExecutingResult.Succeeded);
					secondTask = new Mock<ITestTask> ();
					secondTask.Setup (t => t.Ignored).Returns (true);
					secondTask.Setup (t => t.ExecutionResult).Returns (TestExecutingResult.Ignored);
					yield return new TestCaseData (new List<ITestTask> { firstTask.Object, secondTask.Object }).Returns ("gray");

					firstTask = new Mock<ITestTask> ();
					firstTask.Setup (t => t.Succeeded).Returns (true);
					firstTask.Setup (t => t.ExecutionResult).Returns (TestExecutingResult.Succeeded);
					secondTask = new Mock<ITestTask> ();
					secondTask.Setup (t => t.DeviceNotFound).Returns (true);
					secondTask.Setup (t => t.ExecutionResult).Returns (TestExecutingResult.DeviceNotFound);
					yield return new TestCaseData (new List<ITestTask> { firstTask.Object, secondTask.Object }).Returns ("orangered");

					firstTask = new Mock<ITestTask> ();
					firstTask.Setup (t => t.BuildSucceeded).Returns (true);
					firstTask.Setup (t => t.ExecutionResult).Returns (TestExecutingResult.BuildSucceeded);
					secondTask = new Mock<ITestTask> ();
					secondTask.Setup (t => t.BuildSucceeded).Returns (true);
					secondTask.Setup (t => t.ExecutionResult).Returns (TestExecutingResult.BuildSucceeded);
					yield return new TestCaseData (new List<ITestTask> { firstTask.Object, secondTask.Object }).Returns ("lightgreen");

					firstTask = new Mock<ITestTask> ();
					firstTask.Setup (t => t.Succeeded).Returns (true);
					firstTask.Setup (t => t.ExecutionResult).Returns (TestExecutingResult.Succeeded);
					secondTask = new Mock<ITestTask> ();
					secondTask.Setup (t => t.Succeeded).Returns (true);
					firstTask.Setup (t => t.ExecutionResult).Returns (TestExecutingResult.Succeeded);
					yield return new TestCaseData (new List<ITestTask> { firstTask.Object, secondTask.Object }).Returns ("green");

				}
			}
		}

		[Test, TestCaseSource (typeof (TestCasesData), "GetColorTestCases")]
		public string GetTestColorTest (ITestTask task) => task.GetTestColor ();

		[Test, TestCaseSource (typeof (TestCasesData), "GetColorCollectionTestCases")]
		public string GetTestColorCollectionTest (IEnumerable<ITestTask> tasks) => tasks.GetTestColor ();
	}
}
