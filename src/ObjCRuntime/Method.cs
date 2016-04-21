using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace XamCore.ObjCRuntime {
	internal static class Method {
#if !COREBUILD
		static Runtime.Trampolines tramps;
		
		internal static void Initialize (ref Runtime.InitializationOptions options)
		{
			tramps = options.Trampolines;
		}

#if !XAMCORE_2_0
		public static string Signature (MethodBase minfo) {
			string signature = null;

			if (minfo.IsConstructor)
				signature = "@";
			else if (minfo is MethodInfo)
				signature = TypeConverter.ToNative ((minfo as MethodInfo).ReturnType);

			signature += "@:";
			foreach (ParameterInfo param in minfo.GetParameters ()) {
				signature += TypeConverter.ToNative (param.ParameterType);
			}

			return signature;
		}
#endif

		public static IntPtr Trampoline {
			get {
				return tramps.tramp;
			}
		}

		public static IntPtr SingleTrampoline {
			get {
				return tramps.fpret_single_tramp;
			}
		}
		
		public static IntPtr StaticSingleTrampoline {
			get {
				return tramps.static_fpret_single_tramp;
			}
		}

		public static IntPtr DoubleTrampoline {
			get {
				return tramps.fpret_double_tramp;
			}
		}
		
		public static IntPtr StaticDoubleTrampoline {
			get {
				return tramps.static_fpret_double_tramp;
			}
		}

		public static IntPtr StretTrampoline {
			get {
				return tramps.stret_tramp;
			}
		}
		
		public static IntPtr StaticStretTrampoline {
			get {
				return tramps.static_stret_tramp;
			}
		}

		public static IntPtr StaticTrampoline {
			get {
				return tramps.static_tramp;
			}
		}

		public static IntPtr ConstructorTrampoline {
			get {
				return tramps.ctor_tramp;
			}
		}

		internal static IntPtr ReleaseTrampoline {
			get {
				return tramps.release_tramp;
			}
		}

		internal static IntPtr RetainTrampoline {
			get {
				return tramps.retain_tramp;
			}
		}
			
		internal static IntPtr X86_DoubleABI_StretTrampoline {
			get {
				return tramps.x86_double_abi_stret_tramp;
			}
		}
		
		internal static IntPtr X86_DoubleABI_StaticStretTrampoline {
			get {
				return tramps.x86_double_abi_static_stret_tramp;
			}
		}
		
		internal static IntPtr LongTrampoline {
			get {
				return tramps.long_tramp;
			}
		}
		
		internal static IntPtr StaticLongTrampoline {
			get {
				return tramps.static_long_tramp;
			}
		}

#if MONOMAC
		internal static IntPtr CopyWithZone1 {
			get { return tramps.copy_with_zone_1; }
		}

		internal static IntPtr CopyWithZone2 {
			get { return tramps.copy_with_zone_2; }
		}
#endif

		internal static IntPtr GetGCHandleTrampoline {
			get { return tramps.get_gchandle_tramp; }
		}

		internal static IntPtr SetGCHandleTrampoline {
			get { return tramps.set_gchandle_tramp; }
		}
#endif // !COREBUILD
	}
}
