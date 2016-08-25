// Copyright 2015 Xamarin Inc. All rights reserved.

using System;
using Mono.Linker;
using Mono.Tuner;
using Mono.Cecil;
using Mono.Cecil.Cil;

#if MTOUCH
using MonoTouch;
using MonoTouch.Tuner;
using Xamarin.Bundler;
#elif MMP || MMP_TEST
using MonoMac;
using MonoMac.Tuner;
using Xamarin.Bundler;
#else
using XamCore.ObjCRuntime;
#endif

namespace Xamarin.Linker.Steps {

	public class CoreTlsProviderStep : BaseSubStep {

		public CoreTlsProviderStep (LinkerOptions options)
		{
			Options = options;
		}

		public LinkerOptions Options { get; private set; }

		public override SubStepTargets Targets {
			get { return SubStepTargets.Type; }
		}

		public override bool IsActiveFor (AssemblyDefinition assembly)
		{
#if XAMARIN_NO_TLS
			return false;
#else
#if MONOMAC
			// this is only supported on the profiles where we ship mono (not classic with a system mono)
			if (!(Profile.Current is MacMobileProfile))
				return false;
#endif
			if (assembly.Name.Name != (Profile.Current as BaseProfile).ProductAssembly)
				return false;

			// process only assemblies where the linker is enabled (e.g. --linksdk, --linkskip)
			return Annotations.GetAction (assembly) == AssemblyAction.Link;
#endif
		}

		static MethodDefinition FindDefaultCtor (TypeDefinition type)
		{
			foreach (var m in type.Methods) {
				if (m.IsStatic || !m.IsConstructor || m.HasParameters)
					continue;
				return m;
			}
			return null;
		}

		MethodReference FindProviderConstructor (ModuleDefinition module)
		{
			var providerType = RuntimeOptions.GetTlsProvider (Options.RuntimeOptions, module);
			if (providerType == null)
				return null;

			var ctor = FindDefaultCtor (providerType);
			if (ctor == null)
				throw new InvalidOperationException ();

			return module.Import (ctor);
		}

		public override void ProcessType (TypeDefinition type)
		{
#if XAMARIN_NO_TLS
			return;
#else
			if (!type.Is (Namespaces.ObjCRuntime, "Runtime"))
				return;

			MethodDefinition callbackMethod = null;

			foreach (var m in type.Methods) {
				if (!m.IsStatic || m.HasParameters)
					continue;
				if (m.Name.Equals ("TlsProviderFactoryCallback", StringComparison.Ordinal)) {
					callbackMethod = m;
					break;
				}
			}

			if (callbackMethod == null)
				throw new Exception ("Could not set the default TlsProvider");

			var providerCtor = FindProviderConstructor (type.Module);
			if (providerCtor == null)
				return;

			// re-write TlsProviderFactoryCallback()
			var body = new MethodBody (callbackMethod);
			var il = body.GetILProcessor ();
			if (providerCtor != null)
				il.Emit (OpCodes.Newobj, providerCtor);
			else
				il.Emit (OpCodes.Ldnull);
			il.Emit (OpCodes.Ret);
			callbackMethod.Body = body;
#endif		
		}
	}
}
