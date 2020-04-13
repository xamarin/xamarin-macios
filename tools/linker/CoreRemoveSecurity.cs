using System;

using Mono.Linker;

using Mono.Cecil;

using Xamarin.Bundler;
using Xamarin.Tuner;

namespace Mono.Tuner {

	public class CoreRemoveSecurity : RemoveSecurity {

		protected DerivedLinkContext LinkContext {
			get {
				return (DerivedLinkContext) base.context;
			}
		}

		public override bool IsActiveFor (AssemblyDefinition assembly)
		{
#if MMP
			// CoreRemoveSecurity can modify non-linked assemblies
			// but the conditions for this cannot happen if only the platform assembly is linked
			if (LinkContext.App.LinkMode == LinkMode.Platform)
				return false;
#endif
			// if we run the linker then we can't ignore any assemblies since the security 
			// declarations can refers to types that would not be marked (and preserved)
			// leading to invalid binaries (that even Cecil won't be able to read back)
			return true;
		}

		AssemblyDefinition Current { get; set; }
		AssemblyAction Action { get; set; }

		public override void ProcessAssembly (AssemblyDefinition assembly)
		{
			Current = assembly;
			Action = Annotations.GetAction (assembly);
			ProcessSecurityProvider (assembly);
		}

		public override void ProcessType (TypeDefinition type)
		{
			ProcessSecurityProvider (type);
		}

		public override void ProcessMethod (MethodDefinition method)
		{
			ProcessSecurityProvider (method);
		}

		void ProcessSecurityProvider (ISecurityDeclarationProvider provider)
		{
			if (!provider.HasSecurityDeclarations)
				return;

			// for non-linked code we still need to remove the security declarations,
			// if any are present, are save back the assembly. Otherwise it might become
			// impossible to decode what we save #28918.
			switch (Action) {
			case AssemblyAction.Link:
			case AssemblyAction.Save:
				break;
			default:
				Annotations.SetAction (Current, AssemblyAction.Save);
				Action = AssemblyAction.Save;
				break;
			}

			provider.SecurityDeclarations.Clear ();
		}
	}
}
