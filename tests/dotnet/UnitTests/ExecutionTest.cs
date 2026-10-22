#nullable enable

using System.Diagnostics;
using System.Threading.Tasks;

namespace Xamarin.Tests {
	[TestFixture]
	[Platform (Exclude = "Win")]
	public class ExecutionTest : TestBaseClass {
		Execution? executionResult;

		protected override Execution Execute (string executable, out string output, out string magicWord, Dictionary<string, string?>? environment = null)
		{
			var result = executionResult ?? throw new InvalidOperationException ("No execution result was provided.");
			output = result.Output.MergedOutput;
			magicWord = "magic";
			return result;
		}

		[TestCase (false)]
		[TestCase (true)]
		public async Task CompletionDoesNotInvokeTimeoutCallback (bool setTimeout)
		{
			var invoked = false;
			var execution = new Execution {
				FileName = "/usr/bin/true",
				Arguments = [],
				Timeout = setTimeout ? TimeSpan.FromSeconds (30) : null,
				TimeoutCallback = pid => invoked = true,
			};
			var result = await execution.RunAsync ();
			Assert.That (result.ExitCode, Is.EqualTo (0));
			Assert.That (result.TimedOut, Is.False);
			Assert.That (invoked, Is.False);
		}

		[TestCase (false)]
		[TestCase (true)]
		public async Task TimeoutKillsProcess (bool setCallback)
		{
			var observedAlive = false;
			var execution = new Execution {
				FileName = "/bin/sleep",
				Arguments = ["30"],
				Timeout = TimeSpan.FromMilliseconds (100),
			};
			if (setCallback) {
				execution.TimeoutCallback = pid => {
					using var process = Process.GetProcessById (pid);
					observedAlive = !process.HasExited;
				};
			}
			var result = await execution.RunAsync ();
			Assert.That (result.TimedOut, Is.True);
			Assert.That (result.ExitCode, Is.Not.EqualTo (0));
			Assert.That (observedAlive, Is.EqualTo (setCallback));
		}

		[Test]
		public void ThrowingTimeoutCallbackStillKillsProcess ()
		{
			Process? process = null;
			try {
				var execution = new Execution {
					FileName = "/bin/sleep",
					Arguments = ["30"],
					Timeout = TimeSpan.FromMilliseconds (100),
					TimeoutCallback = pid => {
						process = Process.GetProcessById (pid);
						throw new InvalidOperationException ("Timeout callback failed.");
					},
				};
				var exception = Assert.ThrowsAsync<InvalidOperationException> (async () => await execution.RunAsync ());
				Assert.That (exception?.Message, Is.EqualTo ("Timeout callback failed."));
				Assert.That (execution.TimedOut, Is.True);
				Assert.That (process, Is.Not.Null);
				if (process is not null)
					Assert.That (process.WaitForExit (5000), Is.True, "The child must be killed even when diagnostics fail.");
			} finally {
				if (process is not null) {
					if (!process.HasExited)
						process.Kill (true);
					process.Dispose ();
				}
			}
		}

		[Test]
		public async Task LateSuccessStillFailsTheAppDeadline ()
		{
			var signal = Path.Combine (Path.GetTempPath (), $"execution-timeout-{Guid.NewGuid ():N}");
			try {
				var execution = new Execution {
					FileName = "/bin/sh",
					Arguments = ["-c", "while [ ! -f \"$1\" ]; do /bin/sleep 0.01; done; printf magic", "execution-timeout", signal],
					Timeout = TimeSpan.FromMilliseconds (100),
					TimeoutCallback = pid => {
						using var process = Process.GetProcessById (pid);
						File.WriteAllText (signal, "");
						Assert.That (process.WaitForExit (5000), Is.True, "The child should exit during diagnostics.");
					},
				};
				executionResult = await execution.RunAsync ();
				Assert.That (executionResult.ExitCode, Is.EqualTo (0));
				Assert.That (executionResult.TimedOut, Is.True);
				Assert.That (executionResult.Output.MergedOutput, Does.Contain ("magic"));
				Assert.Throws<AssertionException> (() => ExecuteWithMagicWordAndAssert ("late-success"));
			} finally {
				executionResult = null;
				File.Delete (signal);
			}
		}
	}
}
