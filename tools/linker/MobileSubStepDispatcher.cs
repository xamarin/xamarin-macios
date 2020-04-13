using System.Collections.Generic;

using Mono.Tuner;

namespace Xamarin.Linker {


	public class MobileSubStepDispatcher : SubStepsDispatcher {
		public MobileSubStepDispatcher ()
			: base (MonoTouch.Tuner.Linker.GetSubSteps ())
		{
		}
	}

	public class SubStepsDispatcher : SubStepDispatcher {
		public SubStepsDispatcher (IEnumerable<ISubStep> subSteps)
		{
			foreach (var step in subSteps)
				Add (step);
		}
	}
}
