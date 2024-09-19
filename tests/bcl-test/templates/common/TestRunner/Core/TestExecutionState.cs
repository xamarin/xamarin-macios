using System;

namespace Xamarin.iOS.UnitTests {
	public class TestExecutionState {
		public string TestName { get; internal set; }
		public TimeSpan Started { get; private set; } = TimeSpan.MinValue;
		public TimeSpan Finished { get; private set; } = TimeSpan.MinValue;
		public TestCompletionStatus CompletionStatus { get; set; } = TestCompletionStatus.Undefined;

		internal TestExecutionState ()
		{ }

		internal void Start ()
		{
			Started = new TimeSpan (DateTime.Now.Ticks);
		}

		internal void Finish ()
		{
			Finished = new TimeSpan (DateTime.Now.Ticks);
		}
	}
}
