using System;

using Mono.Cecil;
using Mono.Linker;
using Mono.Linker.Steps;

#nullable enable

namespace Xamarin.Linker {
	public class MarkIProtocolHandler : ConfigurationAwareMarkHandler {

		protected override string Name { get; } = "IProtocol Marker";
		protected override int ErrorCode { get; } = 2420;

		public override void Initialize (LinkContext context, MarkContext markContext)
		{
			base.Initialize (context);

			if (LinkContext.App.Registrar == Bundler.RegistrarMode.Dynamic) {
				markContext.RegisterMarkTypeAction (ProcessType);
			}
		}

		protected override void Process (TypeDefinition type)
		{
			if (!type.HasInterfaces)
				return;

			foreach (var iface in type.Interfaces) {
				var resolvedInterfaceType = iface.InterfaceType.Resolve ();
				// If we're using the dynamic registrar, we need to mark interfaces that represent protocols
				// even if it doesn't look like the interfaces are used, since we need them at runtime.
				var isProtocol = type.IsNSObject (LinkContext) && resolvedInterfaceType.HasCustomAttribute (LinkContext, Namespaces.Foundation, "ProtocolAttribute");
				if (isProtocol) {
					// Mark only if not already marked.
					// otherwise we might enqueue something everytime and never get an empty queue
					if (!LinkContext.Annotations.IsMarked (resolvedInterfaceType)) {
						LinkContext.Annotations.Mark (resolvedInterfaceType);
					}
				}
			}
		}
	}
}
