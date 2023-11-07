using System;
using System.Linq;

using Mono.Cecil;

using Mono.Linker;
using Mono.Linker.Steps;
using Mono.Tuner;

using Xamarin.Bundler;

#nullable enable

namespace Xamarin.Linker.Steps {

	public class PreserveBlockCodeHandler : ConfigurationAwareMarkHandler {

		protected override string Name { get; } = "Preserve Block Code";
		protected override int ErrorCode { get; } = 2240;

		public override void Initialize (LinkContext context, MarkContext markContext)
		{
			base.Initialize (context);
			markContext.RegisterMarkTypeAction (ProcessType);
		}

		protected override void Process (TypeDefinition type)
		{
			/* For the following class:

			static internal class SDInnerBlock {
				// this field is not preserved by other means, but it must not be linked away
				static internal readonly DInnerBlock Handler = Invoke;

				[MonoPInvokeCallback (typeof (DInnerBlock))]
				static internal void Invoke (IntPtr block, int magic_number)
				{
				}
			}

			We need to make sure the linker doesn't remove the Handler field
			and the Invoke method. 
			*/

			// First make sure we got the right class
			// The type for the field we're looking for is abstract, sealed and nested and contains exactly 1 field.
			if (!type.HasFields || !type.IsAbstract || !type.IsSealed || !type.IsNested)
				return;
			if (type.Fields.Count != 1)
				return;

			// The type is also nested inside ObjCRuntime.Trampolines class)
			var nestingType = type.DeclaringType;
			if (!nestingType.Is ("ObjCRuntime", "Trampolines"))
				return;

			// The class has a readonly field named 'Handler'
			var field = type.Fields [0];
			if (!field.IsInitOnly)
				return;
			if (field.Name != "Handler")
				return;

			// The class has a parameterless 'Invoke' method with a 'MonoPInvokeCallback' attribute
			if (!type.HasMethods)
				return;
			var method = type.Methods.SingleOrDefault (v => {
				if (v.Name != "Invoke")
					return false;
				if (!v.HasParameters)
					return false;
				if (!v.HasCustomAttributes)
					return false;
				if (!v.CustomAttributes.Any (v => v.AttributeType.Name == "MonoPInvokeCallbackAttribute"))
					return false;
				return true;
			});

			if (method is null)
				return;

			// The type was used, so preserve the method and field
			Context.Annotations.Mark (method);
			Context.Annotations.Mark (field);
		}
	}
}
