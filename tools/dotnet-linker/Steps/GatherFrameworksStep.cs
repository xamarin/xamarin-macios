using System.Collections.Generic;
using System.Linq;

using Mono.Cecil;

using Xamarin.Linker;

#nullable enable

namespace Xamarin {

	public class GatherFrameworksStep : ConfigurationAwareStep {
		protected override string Name { get; } = "Gather Frameworks";
		protected override int ErrorCode { get; } = 2310;

		HashSet<string> Frameworks = new HashSet<string> ();
		HashSet<string> WeakFrameworks = new HashSet<string> ();

		protected override void TryProcessAssembly (AssemblyDefinition assembly)
		{
			base.TryProcessAssembly (assembly);

			if (Configuration.PlatformAssembly != assembly.Name.Name)
				return;

			global::Frameworks.Gather (Configuration.Application, assembly, Frameworks, WeakFrameworks);
		}

		protected override void TryEndProcess ()
		{

			Configuration.Target.ComputeLinkerFlags ();

			foreach (var asm in Configuration.Target.Assemblies) {
				Frameworks.UnionWith (asm.Frameworks);
				WeakFrameworks.UnionWith (asm.WeakFrameworks);
			}

			// Remove duplicates. WeakFrameworks takes precedence
			Frameworks.ExceptWith (WeakFrameworks);

			// Write out the frameworks we found and pass them to the MSBuild tasks
			var items = new List<MSBuildItem> ();
			foreach (var fw in Frameworks.OrderBy (v => v)) {
				items.Add (new MSBuildItem (
					fw,
					new Dictionary<string, string> {
						{ "IsWeak", "false" },
					}
				));
			}
			foreach (var fw in WeakFrameworks.OrderBy (v => v)) {
				items.Add (new MSBuildItem (
					fw,
					new Dictionary<string, string> {
						{ "IsWeak", "true" },
					}
				));
			}

			Configuration.WriteOutputForMSBuild ("_LinkerFrameworks", items);
		}
	}
}
