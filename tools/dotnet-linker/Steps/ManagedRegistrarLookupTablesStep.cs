using System;
using System.Collections.Generic;

#nullable enable

namespace Xamarin.Linker {
	public class ManagedRegistrarLookupTablesStep : ConfigurationAwareStep {
		protected override string Name { get; } = "ManagedRegistrarLookupTables";
		protected override int ErrorCode { get; } = 2440;

		protected override void TryProcessAssembly (AssemblyDefinition assembly)
		{
			base.TryProcessAssembly (assembly);

			if (App.Registrar != RegistrarMode.ManagedStatic)
				return;
		}
	}
}

