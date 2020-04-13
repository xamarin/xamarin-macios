using System.Collections.Generic;

#if NET
using Mono.Linker.Steps;
#else
using Mono.Tuner;
#endif

namespace Xamarin.Linker {


	public class MobileSubStepDispatcher : SubStepsDispatcher {
		public MobileSubStepDispatcher ()
#if NET
			: base ()
#else
			: base (MonoTouch.Tuner.Linker.GetSubSteps ())
#endif
		{
		}
	}

#if !NET
	public class SubStepsDispatcher : SubStepDispatcher {
		public SubStepsDispatcher (IEnumerable<ISubStep> subSteps)
		{
			foreach (var step in subSteps)
				Add (step);
		}
	}
#endif
}
