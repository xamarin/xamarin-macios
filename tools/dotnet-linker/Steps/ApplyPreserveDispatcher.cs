using Mono.Linker.Steps;
using Xamarin.Linker;

namespace Xamarin.Linker.Steps {
	class ApplyPreserveDispatcher : MarkSubStepsDispatcher {
		public ApplyPreserveDispatcher ()
			: base (new [] { new ApplyPreserveAttribute () })
		{
		}
	}
}
