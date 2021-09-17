using Mono.Linker.Steps;
using Xamarin.Linker;

namespace Xamarin.Linker.Steps {
	class PreMarkDispatcher : SubStepsDispatcher {
		public PreMarkDispatcher ()
			: base (new BaseSubStep [] {
				new CollectUnmarkedMembersSubStep (),
				new StoreAttributesStep ()
				})
		{
		}
	}
}
