using Mono.Linker.Steps;

#nullable enable

namespace Xamarin.Linker.Steps {
	// SubStepsDispatcher is abstract, so create a subclass we can instantiate
	class DotNetSubStepDispatcher : SubStepsDispatcher {
		public DotNetSubStepDispatcher ()
		{
		}

		public DotNetSubStepDispatcher (params BaseSubStep [] subSteps)
		{
			foreach (var ss in subSteps)
				Add (ss);
		}
	}
}
