using System;
using System.Linq;

using Mono.Cecil;

using Mono.Linker.Steps;
using Mono.Tuner;

using Xamarin.Bundler;

namespace Xamarin.Linker.Steps {
	public class PreserveBlockCodeSubStep : ConfigurationAwareSubStep {
		MethodDefinition ctor_string_def;
		MethodReference ctor_string_ref;

		public override SubStepTargets Targets {
			get {
				return SubStepTargets.Assembly |
						SubStepTargets.Field |
						SubStepTargets.Type;
			}
		}

		MethodReference GetConstructorReference (AssemblyDefinition assembly)
		{
			if (ctor_string_def == null) {
				// Find the method definition for the constructor we want to use
				foreach (var asm in Configuration.Assemblies) {
					var dependencyAttribute = asm.MainModule.GetType ("System.Diagnostics.CodeAnalysis.DynamicDependencyAttribute");
					if (dependencyAttribute == null)
						continue;

					foreach (var method in dependencyAttribute.Methods) {
						if (!method.HasParameters)
							continue;

						if (method.Parameters.Count == 1 && method.Parameters [0].ParameterType.Is ("System", "String")) {
							ctor_string_def = method;
							break;
						}
					}

					break;
				}

				if (ctor_string_def == null)
					throw ErrorHelper.CreateError (99, Errors.MX0099, "Could not find the constructor 'System.Diagnostics.CodeAnalysis.DynamicDependencyAttribute..ctor(System.String)'");
			}

			// Import the constructor into the current assembly if it hasn't already been imported
			ctor_string_ref ??= assembly.MainModule.ImportReference (ctor_string_def);

			return ctor_string_ref;
		}

		protected override void TryProcessAssembly (AssemblyDefinition assembly)
		{
			// Clear out the method reference we have, so that we import the method definition again
			ctor_string_ref = null;

			base.TryProcessAssembly (assembly);
		}

		protected override void TryProcessField (FieldDefinition field)
		{
			base.TryProcessField (field);

			PreserveBlockField (field);
		}

		void PreserveBlockField (FieldDefinition field)
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
			and the Invoke method. Unfortunately there's no programmatic way
			to preserve a field dependent upon the preservation of the
			containing type, so we have to inject a DynamicDependency
			attribute. And since we can't add a DynamicDependency attribute on
			the type itself, we add it to the Invoke method. We also need to
			preserve the Invoke method (which is done programmatically). Our
			generator generates the required attributes, but since we have to
			work with existing assemblies, we detect the scenario here as well
			and inject the attributes manually if they're not already there.

			*/

			// First make sure we got the right field
			// The containing type for the field we're looking for is abstract, sealed and nested and contains exactly 1 field.
			var td = field.DeclaringType;
			if (!td.IsAbstract || !td.IsSealed || !td.IsNested)
				return;
			if (td.Fields.Count != 1)
				return;

			// The containing type is also nested inside ObjCRuntime.Trampolines class)
			var nestingType = td.DeclaringType;
			if (!nestingType.Is ("ObjCRuntime", "Trampolines"))
				return;

			// The field itself is a readonly field named 'Handler'
			if (!field.IsInitOnly)
				return;
			if (field.Name != "Handler")
				return;

			// One problem is that we can't add the DynamicDependency attribute to the type, nor the field itself,
			// so we add it to the Invoke method in the same type.
			if (!td.HasMethods)
				return;

			var method = td.Methods.SingleOrDefault (v => {
				if (v.Name != "Invoke")
					return false;
				if (v.Parameters.Count == 0)
					return false;
				if (!v.HasCustomAttributes)
					return false;
				if (!v.CustomAttributes.Any (v => v.AttributeType.Name == "MonoPInvokeCallbackAttribute"))
					return false;
				return true;
			});

			if (method == null)
				return;

			// We need to preserve the method, if the type is used (unless it's already preserved)
			if (!method.CustomAttributes.Any (v => v.AttributeType.Name == "PreserveAttribute"))
				Annotations.AddPreservedMethod (method.DeclaringType, method);

			// Does the method already have a DynamicDependency attribute? If so, no need to add another one
			if (method.CustomAttributes.Any (v => v.AttributeType.Is ("System.Diagnostics.CodeAnalysis", "DynamicDependencyAttribute")))
				return;

			// Create and add the DynamicDependency attribute to the method
			var ctor = GetConstructorReference (field.DeclaringType.Module.Assembly);
			var attrib = new CustomAttribute (ctor);
			attrib.ConstructorArguments.Add (new CustomAttributeArgument (ctor.Parameters [0].ParameterType, "Handler"));
			method.CustomAttributes.Add (attrib);
		}
	}
}
