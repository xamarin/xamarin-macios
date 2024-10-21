using System;
using System.IO;

using Mono.Cecil;
using Mono.Linker;
using Mono.Linker.Steps;

using Xamarin.Bundler;

#nullable enable

namespace Xamarin.Linker {
	public class MarkIProtocolHandler : ConfigurationAwareMarkHandler {

		bool mark_protocol_wrapper_type;
		bool mark_protocol_interface_type;

		protected override string Name { get; } = "IProtocol Marker";
		protected override int ErrorCode { get; } = 2420;

		public override void Initialize (LinkContext context, MarkContext markContext)
		{
			base.Initialize (context);

			switch (LinkContext.App.Registrar) {
			case Bundler.RegistrarMode.Dynamic:
				mark_protocol_interface_type = true;
				break;
			}

			mark_protocol_wrapper_type = LinkContext.App.Optimizations.RegisterProtocols == true;

			if (mark_protocol_wrapper_type || mark_protocol_interface_type)
				markContext.RegisterMarkTypeAction (ProcessType);
		}

		protected override void Process (TypeDefinition type)
		{
			if (mark_protocol_interface_type)
				MarkProtocolInterfaceType (type);

			if (mark_protocol_wrapper_type)
				MarkProtocolWrapperType (type);
		}

		void MarkProtocolInterfaceType (TypeDefinition type)
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

		void MarkProtocolWrapperType (TypeDefinition type)
		{
			var protocolAttributes = LinkContext.GetCustomAttributes (type, "Foundation", "ProtocolAttribute");
			if (protocolAttributes is null)
				return;

			foreach (var attrib in protocolAttributes) {
				foreach (var prop in attrib.Properties) {
					if (prop.Name != "WrapperType")
						continue;

					var wrapperType = ((TypeReference) prop.Argument.Value).Resolve ();
					if (!LinkContext.Annotations.IsMarked (wrapperType)) {
						LinkContext.Annotations.Mark (wrapperType);
					}

					return;
				}
			}
		}

	}
}
