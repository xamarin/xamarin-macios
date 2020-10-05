using System.Collections.Generic;
using System.Linq;

using Mono.Cecil;

using Xamarin.Linker;

namespace Xamarin {

	public class GatherFrameworksStep : ConfigurationAwareStep {
		HashSet<string> Frameworks = new HashSet<string> ();
		HashSet<string> WeakFrameworks = new HashSet<string> ();

		protected override void ProcessAssembly (AssemblyDefinition assembly)
		{
			base.ProcessAssembly (assembly);

			if (Configuration.PlatformAssembly != assembly.Name.Name)
				return;

			global::Frameworks.Gather (Configuration.Application, assembly, Frameworks, WeakFrameworks);
		}

		protected override void EndProcess ()
		{
			base.EndProcess ();

			// Remove duplicates. WeakFrameworks takes precedence
			Frameworks.ExceptWith (WeakFrameworks);

			// Write out the frameworks we found and pass them to the MSBuild tasks
			var items = new List<MSBuildItem> ();
			foreach (var fw in Frameworks.OrderBy (v => v)) {
				items.Add (new MSBuildItem {
					Include = fw,
					Metadata = {
						{ "IsWeak", "false" },
					},
				});
			}
			foreach (var fw in WeakFrameworks.OrderBy (v => v)) {
				items.Add (new MSBuildItem {
					Include = fw,
					Metadata = {
						{ "IsWeak", "true" },
					},
				});
			}

			Configuration.WriteOutputForMSBuild ("_LinkerFrameworks", items);
		}
	}
}

