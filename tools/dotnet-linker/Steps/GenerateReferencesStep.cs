using System.Collections.Generic;
using System.IO;
using System.Text;

using Mono.Cecil;

using Xamarin.Bundler;
using Xamarin.Linker;

namespace Xamarin {

	public class GenerateReferencesStep : ConfigurationAwareStep {
		private Symbols required_symbols = new Symbols ();
		
		protected override string Name { get; } = "Generate References";
		protected override int ErrorCode { get; } = 2320;

		protected override void TryEndProcess ()
		{
			base.TryEndProcess ();

			var items = new List<MSBuildItem> ();
			var file = Path.Combine (Configuration.CacheDirectory, $"references.mm");
			if (Configuration.Target.GenerateReferencingSource (file, required_symbols) != null) {
				var item = new MSBuildItem { Include = file };
				items.Add (item);
			}
			Configuration.WriteOutputForMSBuild ("_ReferencesFile", items);
		}

		protected override void TryProcessAssembly (AssemblyDefinition assembly)
		{
			base.TryProcessAssembly (assembly);

			if (!assembly.MainModule.HasTypes)
				return;

			var hasSymbols = false;
			if (assembly.MainModule.HasModuleReferences) {
				hasSymbols = true;
			} else if (assembly.MainModule.HasTypeReference ("Foundation.FieldAttribute")) {
				hasSymbols = true;
			}
			if (!hasSymbols)
				return;

			foreach (var type in assembly.MainModule.Types)
				ProcessType (type);
		}

		void ProcessType (TypeDefinition type)
		{
			if (type.HasNestedTypes) {
				foreach (var nested in type.NestedTypes)
					ProcessType (nested);
			}

			if (type.HasMethods) {
				foreach (var method in type.Methods)
					ProcessMethod (method);
			}
		}

		void ProcessMethod (MethodDefinition method)
		{
			if (method.IsPInvokeImpl && method.HasPInvokeInfo && method.PInvokeInfo != null) {
				var pinfo = method.PInvokeInfo;
				if (pinfo.Module.Name == "__Internal") {
					required_symbols.AddFunction (pinfo.EntryPoint).AddMember (method);
				}
			}
		}
	}
}
