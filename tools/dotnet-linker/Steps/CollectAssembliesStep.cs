using System;
using System.IO;
using Mono.Cecil;

namespace Xamarin.Linker {
	public class CollectAssembliesStep : ConfigurationAwareStep {
		protected override string Name { get; } = "Collect Assemblies";
		protected override int ErrorCode { get; } = 2330;

		protected override void TryProcess ()
		{
			base.TryProcess ();

			var tryResolve = Configuration.Context.GetType ().GetMethod ("TryResolve");
			foreach (var asm in Configuration.ManagedAssembliesToLink) {
				var ad = (AssemblyDefinition) tryResolve.Invoke (Configuration.Context, new object [] { Path.GetFileNameWithoutExtension (asm) });
				if (ad != null) {
					Console.WriteLine ($"Loaded {asm} => {ad.FullName} => {ad.MainModule.FileName}");
				} else {
					Console.WriteLine ($"Failed to load {asm}");
				}
			}
		}

		protected override void TryProcessAssembly (AssemblyDefinition assembly)
		{
			base.TryProcessAssembly (assembly);

			Configuration.Assemblies.Add (assembly);
		}

		protected override void TryEndProcess ()
		{
			foreach (var asm in Configuration.Assemblies)
				System.Console.WriteLine ($"Collected assembly: {asm.FullName}");

			base.TryEndProcess ();
		}
	}
}

