using Mono.Linker.Steps;
using Xamarin.Linker;

namespace Xamarin.Linker.Steps {
	class RemoveAttributesDispatcher : SubStepsDispatcher {
		public RemoveAttributesDispatcher ()
			: base (new [] { new RemoveAttributesStep () })
		{
		}
	}
}
