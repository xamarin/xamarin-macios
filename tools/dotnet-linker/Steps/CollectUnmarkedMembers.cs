using System.Collections.Generic;

using Mono.Cecil;
using Mono.Linker.Steps;

#nullable enable

namespace Xamarin.Linker {
	// The static registrar may need access to information that has been linked away,
	// in particular types and interfaces, so we need to store those somewhere
	// so that the static registrar can access them.
	public class CollectUnmarkedMembersSubStep : ConfigurationAwareSubStep {
		Dictionary<TypeDefinition, List<TypeDefinition>> ProtocolImplementations => Configuration.DerivedLinkContext.ProtocolImplementations;

		protected override string Name { get; } = "Collect Unmarked Members";
		protected override int ErrorCode { get; } = 2230;

		public override SubStepTargets Targets {
			get {
				return SubStepTargets.Type;
			}
		}

		protected override void Process (TypeDefinition type)
		{
			if (!Annotations.IsMarked (type))
				LinkContext.AddLinkedAwayType (type);

			if (type.HasInterfaces) {
				foreach (var iface in type.Interfaces) {
					if (Annotations.IsMarked (iface))
						continue;

					// This interface might be removed, so save it
					if (!ProtocolImplementations.TryGetValue (type, out var list))
						ProtocolImplementations [type] = list = new List<TypeDefinition> ();
					list.Add (iface.InterfaceType.Resolve ());
				}
			}
		}
	}
}
