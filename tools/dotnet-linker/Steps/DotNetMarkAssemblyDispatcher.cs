using Mono.Linker.Steps;

namespace Xamarin.Linker.Steps {
	// MarkSubStepsDispatcher is abstract, so create a subclass we can instantiate.
	// Can be removed when we update to the preview4 linker, which makes MarkSubStepsDispatcher non-abstract.
	class DotNetMarkAssemblySubStepDispatcher : MarkSubStepsDispatcher {
		public DotNetMarkAssemblySubStepDispatcher (params BaseSubStep[] subSteps) : base (subSteps)
		{
		}
	}
}
