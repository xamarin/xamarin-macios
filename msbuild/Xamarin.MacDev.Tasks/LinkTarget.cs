using System;

namespace Xamarin.MacDev.Tasks {
	[Flags]
	enum LinkTarget {
		Simulator = 1,
		i386 = Simulator,
		ArmV6 = 2,
		ArmV7 = 4,
		Thumb = 8,
		ArmV7s = 16,
		Arm64 = 32,
		Simulator64 = 64,
		x86_64 = Simulator64
	}
}
