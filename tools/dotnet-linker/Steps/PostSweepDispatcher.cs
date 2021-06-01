using Mono.Linker.Steps;
using Xamarin.Linker;

namespace Xamarin.Linker.Steps {
	class PostSweepDispatcher : SubStepsDispatcher {
		public PostSweepDispatcher ()
			: base (new [] { new RemoveAttributesStep () })
		{
		}
	}
}
