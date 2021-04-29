using Mono.Linker.Steps;
using Xamarin.Linker;

namespace Xamarin.Linker.Steps {
	class MarkNSObjectsDispatcher : MarkSubStepsDispatcher {
		public MarkNSObjectsDispatcher ()
			: base (new [] { new MarkNSObjects () })
		{
		}
	}
}
