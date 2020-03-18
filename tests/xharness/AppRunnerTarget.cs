using System;

namespace Xharness {
	public enum AppRunnerTarget {
		None,
		Simulator_iOS,
		Simulator_iOS32,
		Simulator_iOS64,
		Simulator_tvOS,
		Simulator_watchOS,
		Device_iOS,
		Device_tvOS,
		Device_watchOS,
	}

	public static class AppRunnerTargetExtensions {
		public static RunMode ToRunMode (this AppRunnerTarget target) => target switch
		{
			AppRunnerTarget.Simulator_iOS => RunMode.Classic,
			AppRunnerTarget.Simulator_iOS32 => RunMode.Sim32,
			AppRunnerTarget.Simulator_iOS64 => RunMode.Sim64,
			AppRunnerTarget.Simulator_tvOS => RunMode.TvOS,
			AppRunnerTarget.Simulator_watchOS => RunMode.WatchOS,
			AppRunnerTarget.Device_iOS => RunMode.iOS,
			AppRunnerTarget.Device_tvOS => RunMode.TvOS,
			AppRunnerTarget.Device_watchOS => RunMode.WatchOS,
			_ => throw new ArgumentOutOfRangeException ($"Unknown target: {target}"),
		};

		public static bool IsSimulator (this AppRunnerTarget target) => target switch
		{
			AppRunnerTarget.Simulator_iOS => true,
			AppRunnerTarget.Simulator_iOS32 => true,
			AppRunnerTarget.Simulator_iOS64 => true,
			AppRunnerTarget.Simulator_tvOS => true,
			AppRunnerTarget.Simulator_watchOS => true,
			AppRunnerTarget.Device_iOS => false,
			AppRunnerTarget.Device_tvOS => false,
			AppRunnerTarget.Device_watchOS => false,
			_ => throw new ArgumentOutOfRangeException ($"Unknown target: {target}"),
		};
	}
}
