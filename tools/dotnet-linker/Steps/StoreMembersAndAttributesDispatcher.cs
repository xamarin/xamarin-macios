using Mono.Linker.Steps;
using Xamarin.Linker;

namespace Xamarin.Linker.Steps {
	class StoreMembersAndAttributesDispatcher : SubStepsDispatcher {
		public StoreMembersAndAttributesDispatcher ()
			: base (new BaseSubStep [] {
				new CollectUnmarkedMembersSubStep (),
				new StoreAttributesStep ()
				})
		{
		}
	}
}
