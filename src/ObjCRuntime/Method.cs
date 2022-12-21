using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace ObjCRuntime {
	internal static class Method {
#if !COREBUILD
		public unsafe static IntPtr Trampoline {
			get {
				return Runtime.options->Trampolines->tramp;
			}
		}

		public unsafe static IntPtr SingleTrampoline {
			get {
				return Runtime.options->Trampolines->fpret_single_tramp;
			}
		}

		public unsafe static IntPtr StaticSingleTrampoline {
			get {
				return Runtime.options->Trampolines->static_fpret_single_tramp;
			}
		}

		public unsafe static IntPtr DoubleTrampoline {
			get {
				return Runtime.options->Trampolines->fpret_double_tramp;
			}
		}

		public unsafe static IntPtr StaticDoubleTrampoline {
			get {
				return Runtime.options->Trampolines->static_fpret_double_tramp;
			}
		}

		public unsafe static IntPtr StretTrampoline {
			get {
				return Runtime.options->Trampolines->stret_tramp;
			}
		}

		public unsafe static IntPtr StaticStretTrampoline {
			get {
				return Runtime.options->Trampolines->static_stret_tramp;
			}
		}

		public unsafe static IntPtr StaticTrampoline {
			get {
				return Runtime.options->Trampolines->static_tramp;
			}
		}

		public unsafe static IntPtr ConstructorTrampoline {
			get {
				return Runtime.options->Trampolines->ctor_tramp;
			}
		}

		internal unsafe static IntPtr ReleaseTrampoline {
			get {
				return Runtime.options->Trampolines->release_tramp;
			}
		}

		internal unsafe static IntPtr RetainTrampoline {
			get {
				return Runtime.options->Trampolines->retain_tramp;
			}
		}

		internal unsafe static IntPtr X86_DoubleABI_StretTrampoline {
			get {
				return Runtime.options->Trampolines->x86_double_abi_stret_tramp;
			}
		}

		internal unsafe static IntPtr X86_DoubleABI_StaticStretTrampoline {
			get {
				return Runtime.options->Trampolines->x86_double_abi_static_stret_tramp;
			}
		}

		internal unsafe static IntPtr LongTrampoline {
			get {
				return Runtime.options->Trampolines->long_tramp;
			}
		}

		internal unsafe static IntPtr StaticLongTrampoline {
			get {
				return Runtime.options->Trampolines->static_long_tramp;
			}
		}

#if MONOMAC
		internal unsafe static IntPtr CopyWithZone1 {
			get { return Runtime.options->Trampolines->copy_with_zone_1; }
		}

		internal unsafe static IntPtr CopyWithZone2 {
			get { return Runtime.options->Trampolines->copy_with_zone_2; }
		}
#endif

		internal unsafe static IntPtr GetGCHandleTrampoline {
			get { return Runtime.options->Trampolines->get_gchandle_tramp; }
		}

		internal unsafe static IntPtr SetGCHandleTrampoline {
			get { return Runtime.options->Trampolines->set_gchandle_tramp; }
		}

		internal unsafe static IntPtr GetFlagsTrampoline {
			get { return Runtime.options->Trampolines->get_flags_tramp; }
		}

		internal unsafe static IntPtr SetFlagsTrampoline {
			get { return Runtime.options->Trampolines->set_flags_tramp; }
		}
#endif // !COREBUILD
	}
}
