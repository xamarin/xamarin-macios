using System;
using Mono.Cecil;

namespace Microsoft.MaciOS.Nnyeah {
	public class ModuleContainer {
		public ModuleContainer (ModuleDefinition moduleToEdit, ModuleDefinition xamarinModule, ModuleDefinition microsoftModule)
		{
			ModuleToEdit = moduleToEdit;
			XamarinModule = xamarinModule;
			MicrosoftModule = microsoftModule;
		}
		public ModuleDefinition ModuleToEdit { get; init; }
		public ModuleDefinition XamarinModule { get; init; }
		public ModuleDefinition MicrosoftModule { get; init; }
		public TypeSystem TypeSystem => ModuleToEdit.TypeSystem;
	}
}
