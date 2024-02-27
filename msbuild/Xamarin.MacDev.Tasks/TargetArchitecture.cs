using System;
using System.Collections.Generic;

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

	public static class TargetArchitectureExtensions {
		public static IList<TargetArchitecture> ToArray (this TargetArchitecture self)
		{
			if (self == TargetArchitecture.Default)
				return Array.Empty<TargetArchitecture> ();

			var rv = new List<TargetArchitecture> ();

#if NET
			var simpleValues = new List<TargetArchitecture> (Enum.GetValues<TargetArchitecture> ());
#else
			var simpleValues = new List<TargetArchitecture> ((TargetArchitecture []) Enum.GetValues (typeof (TargetArchitecture)));
#endif
			simpleValues.Remove (TargetArchitecture.Default);
			simpleValues.Remove (TargetArchitecture.ARMv6_ARMv7);

			foreach (var arch in simpleValues) {
				if ((self & arch) == arch)
					rv.Add (arch);
			}

			return rv;
		}

		public static string ToNativeArchitecture (this TargetArchitecture self)
		{
			switch (self) {
			case TargetArchitecture.i386:
			case TargetArchitecture.x86_64:
				return self.ToString ();
			case TargetArchitecture.ARMv6:
				return "armv6";
			case TargetArchitecture.ARMv7:
				return "armv7";
			case TargetArchitecture.ARMv7s:
				return "armv7s";
			case TargetArchitecture.ARMv7k:
				return "armv7k";
			case TargetArchitecture.ARM64:
				return "arm64";
			case TargetArchitecture.ARM64_32:
				return "arm64_32";
			default:
				throw new ArgumentOutOfRangeException (nameof (self), $"The value '{self}' does not represent a single architecture.");
			}
		}
	}
}
