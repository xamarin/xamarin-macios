#nullable enable

using System;
using System.Runtime.InteropServices;
using ObjCRuntime;
using Foundation;
using CoreFoundation;

namespace CoreFoundation {
	public partial class CFBundle {

		// from machine.h
		// #define CPU_ARCH_ABI64       0x01000000
		// #define CPU_TYPE_X86        ((cpu_type_t) 7)
		// #define CPU_TYPE_X86_64     (CPU_TYPE_X86 | CPU_ARCH_ABI64)
		// #define CPU_TYPE_ARM        ((cpu_type_t) 12)
		// #define CPU_TYPE_ARM64      (CPU_TYPE_ARM | CPU_ARCH_ABI64)
		// #define CPU_TYPE_POWERPC    ((cpu_type_t) 18)
		// #define CPU_TYPE_POWERPC64  (CPU_TYPE_POWERPC | CPU_ARCH_ABI64)
		public enum Architecture {
			/// <summary>To be added.</summary>
			I386 = 0x00000007,
			/// <summary>To be added.</summary>
			X86_64 = 0x01000007,
			/// <summary>To be added.</summary>
			ARM = 0x0000000c,
			/// <summary>To be added.</summary>
			ARM64 = 0x01000000 | ARM,
			/// <summary>To be added.</summary>
			PPC = 0x00000012,
			/// <summary>To be added.</summary>
			PPC64 = 0x01000000 | PPC,
		}
	}
}
