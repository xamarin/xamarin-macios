using Mono.Linker.Steps;

using Xamarin.Linker;

#nullable enable

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
