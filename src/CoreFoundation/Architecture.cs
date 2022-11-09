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
			I386 = 0x00000007,
			X86_64 = 0x01000007,
			ARM = 0x0000000c,
			ARM64 = 0x01000000 | ARM,
			PPC = 0x00000012,
			PPC64 = 0x01000000 | PPC,
		}
	}
}
