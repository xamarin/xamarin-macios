using Mono.Linker.Steps;
using Xamarin.Linker;

namespace Xamarin.Linker.Steps {
	class PreOutputDispatcher : SubStepsDispatcher {
		public PreOutputDispatcher ()
			: base (new BaseSubStep [] {
				new RemoveUserResourcesSubStep (),
				new BackingFieldReintroductionSubStep (),
				})
		{
		}
	}
}
