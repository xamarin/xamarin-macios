using Mono.Linker.Steps;
using Xamarin.Linker;

using Mono.Cecil;
using Mono.Tuner;

#nullable enable

namespace Xamarin.Linker.Steps {
	public class SetBeforeFieldInitStep : ConfigurationAwareSubStep {
		protected override string Name { get; } = "Set BeforeFieldInit";
		protected override int ErrorCode { get; } = 2380;

		public override SubStepTargets Targets {
			get {
				return SubStepTargets.Type;
			}
		}

		protected override void Process (TypeDefinition type)
		{
			// If we're registering protocols, we want to remove the static
			// constructor on the protocol interface, because it's not needed
			// (because we've removing all the DynamicDependency attributes
			// from the cctor).
			//
			// However, just removing the static constructor from the type
			// causes problems later on in the trimming process, so we want
			// the trimmer to just not mark it.
			//
			// The trimmer marks it, because it has a static constructor, so
			// we're in a bit of a cyclic dependency here.
			//
			// This is complicated by a few facts:
			// - When we optimize the cctor (i.e. removing the
			//   DynamicDependency attributes), the cctor is already marked.
			// - Adding a MarkHandler that processes types doesn't work
			//   either, because it may be called after the cctor is marked:
			//   https://github.com/dotnet/runtime/blob/6177a9f920861900681cfda2b6cc66ac3557e93b/src/tools/illink/src/linker/Linker.Steps/MarkStep.cs#L1928-L1952
			//
			// So this is a pre-mark step that just sets
			// IsBeforeFieldInit=true for interfaces we want trimmed away by
			// the linker.

			if (Configuration.DerivedLinkContext.App.Optimizations.RegisterProtocols != true)
				return;

			if (!type.IsBeforeFieldInit && type.IsInterface && type.HasMethods) {
				var cctor = type.GetTypeConstructor ();
				if (cctor is not null && cctor.IsBindingImplOptimizableCode (LinkContext))
					type.IsBeforeFieldInit = true;
			}
		}
	}
}
