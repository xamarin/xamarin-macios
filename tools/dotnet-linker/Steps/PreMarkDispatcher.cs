using Mono.Linker.Steps;
using Xamarin.Linker;

#nullable enable

namespace Xamarin.Linker.Steps {
	class PreMarkDispatcher : SubStepsDispatcher {
		public PreMarkDispatcher ()
			: base (new BaseSubStep [] {
				new SetBeforeFieldInitStep (),
				new CollectUnmarkedMembersSubStep (),
				new StoreAttributesStep ()
				})
		{
		}
	}
}
