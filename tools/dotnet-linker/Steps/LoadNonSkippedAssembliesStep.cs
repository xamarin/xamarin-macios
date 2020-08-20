using System;

using Mono.Cecil;
using Mono.Linker;

namespace Xamarin.Linker {
	// List all the assemblies we care about (i.e. the ones that have not been linked away)
	public class LoadNonSkippedAssembliesStep : ConfigurationAwareStep {

		protected override void ProcessAssembly (AssemblyDefinition assembly)
		{
			base.ProcessAssembly (assembly);

			// Figure out if an assembly is linked away or not
			if (Context.Annotations.HasAction (assembly)) {
				var action = Context.Annotations.GetAction (assembly);
				switch (action) {
				case AssemblyAction.Delete:
				case AssemblyAction.Skip:
					break;
				case AssemblyAction.Copy:
				case AssemblyAction.CopyUsed:
				case AssemblyAction.Link:
				case AssemblyAction.Save:
					var ad = Configuration.Target.AddAssembly (assembly);
					var assemblyFileName = Configuration.GetAssemblyFileName (assembly);
					ad.FullPath = assemblyFileName;
					break;
				case AssemblyAction.AddBypassNGen: // This should be turned into Save or Delete
				case AssemblyAction.AddBypassNGenUsed: // This should be turned into Save or Delete
					// Log this?
					break;
				default:
					// Log this?
					break;
				}
			} else {
				// Log this?
			}
		}
	}
}
