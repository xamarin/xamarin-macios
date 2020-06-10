using System;

namespace Xamarin.MacDev.Tasks {
	[Flags]
	public enum TargetArchitecture {
		Default = 0,

		i386 = 1,
		x86_64 = 2,

		ARMv6 = 4,
		ARMv7 = 8,
		ARMv7s = 16,
		ARMv7k = 32,
		ARM64 = 64,
		ARM64_32 = 128,

		// Note: needed for backwards compatability
		ARMv6_ARMv7 = ARMv6 | ARMv7,
	}
}
