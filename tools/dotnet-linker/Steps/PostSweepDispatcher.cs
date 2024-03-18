using Mono.Linker.Steps;
using Xamarin.Linker;

#nullable enable

namespace Xamarin.Linker.Steps {
	class PostSweepDispatcher : SubStepsDispatcher {
		public PostSweepDispatcher ()
			: base (new [] { new RemoveAttributesStep () })
		{
		}
	}
}
