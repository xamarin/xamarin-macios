using System;

namespace Microsoft.DotNet.XHarness.iOS.Shared {
	[Flags]
	public enum TestExecutingResult {
		NotStarted = 0,
		InProgress = 0x1,
		Finished = 0x2,
		Waiting = 0x4,
		StateMask = NotStarted + InProgress + Waiting + Finished,

		// In progress state
		Building = 0x10 + InProgress,
		BuildQueued = 0x10 + InProgress + Waiting,
		Built = 0x20 + InProgress,
		Running = 0x40 + InProgress,
		RunQueued = 0x40 + InProgress + Waiting,
		InProgressMask = 0x10 + 0x20 + 0x40,

		// Finished results
		Succeeded = 0x100 + Finished,
		Failed = 0x200 + Finished,
		Ignored = 0x400 + Finished,
		DeviceNotFound = 0x800 + Finished,

		// Finished & Failed results
		Crashed = 0x1000 + Failed,
		TimedOut = 0x2000 + Failed,
		HarnessException = 0x4000 + Failed,
		BuildFailure = 0x8000 + Failed,

		// Other results
		BuildSucceeded = 0x10000 + Succeeded,
	}
}
