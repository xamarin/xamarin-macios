using Mono.Linker.Steps;
using Xamarin.Linker;

namespace Xamarin.Linker.Steps {
	class RemoveUserResourcesDispatcher : SubStepsDispatcher {
		public RemoveUserResourcesDispatcher ()
			: base (new [] { new RemoveUserResourcesSubStep () })
		{
		}
	}
}
