using System;
using Mono.Cecil;
using Mono.Linker;
using Mono.Tuner;
using Xamarin.Linker;

namespace MonoMac.Tuner {

	public class MonoMacPreserveCode : CorePreserveCode	{

		public MonoMacPreserveCode (LinkerOptions options) : 
			base (options.I18nAssemblies)
		{
		}

		public override void Process (LinkContext context)
		{
			base.Process (context);

			ProcessNativeBuilder ();
		}

		void ProcessNativeBuilder ()
		{
			// NativeImplementationBuilder.cs emits code (post-linking) using [MarshalAs] attribute
			// * .ctor(UnmanagedType)
			// * MarshalTypeRef field - but since it's decorated with [StructLayout (LayoutKind.Sequential)] 
			//   the linker will keep every fields
			var marshalas = Corlib.MainModule.GetType ("System.Runtime.InteropServices", "MarshalAsAttribute");
			if (marshalas == null || !marshalas.HasMethods)
				return;
			foreach (MethodDefinition ctor in marshalas.Methods) {
				if (ctor.IsConstructor && ctor.HasParameters && ctor.Parameters [0].ParameterType.Is ("System.Runtime.InteropServices", "UnmanagedType"))
					Context.Annotations.AddPreservedMethod (marshalas, ctor);
			}
		}
	}
}