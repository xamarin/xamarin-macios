using System;

using Mono.Cecil;

using Mono.Linker;
using Mono.Linker.Steps;

namespace Xamarin.Tuner {

}

namespace Mono.Tuner {
	using MonoTouch.Tuner;

	public abstract class Profile {

		static Profile current;

		public static Profile Current {
			get {
				if (current != null)
					return current;

#if IOS || TVOS || WATCHOS
				current = new MonoTouchProfile ();
#elif MACOS
				current = new MonoMacProfile ();
#else
#error Invalid platform
#endif

				throw new NotSupportedException ("No active profile");
			}
			set {
				current = value;
			}
		}
		
		public static bool IsSdkAssembly (AssemblyDefinition assembly)
		{
			return Current.IsSdk (assembly);
		}

		public static bool IsSdkAssembly (string assemblyName)
		{
			return Current.IsSdk (assemblyName);
		}

		public static bool IsProductAssembly (AssemblyDefinition assembly)
		{
			return Current.IsProduct (assembly);
		}

		public static bool IsProductAssembly (string assemblyName)
		{
			return Current.IsProduct (assemblyName);
		}

		protected virtual bool IsSdk (AssemblyDefinition assembly)
		{
			return IsSdk (assembly.Name.Name);
		}

		protected virtual bool IsProduct (AssemblyDefinition assembly)
		{
			return IsProduct (assembly.Name.Name);
		}

		protected abstract bool IsSdk (string assemblyName);
		protected abstract bool IsProduct (string assemblyName);
	}
}
