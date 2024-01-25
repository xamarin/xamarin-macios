using Mono.Linker.Steps;

using Xamarin.Linker;

#nullable enable

namespace Xamarin.Linker.Steps {
	class MarkDispatcher : MarkSubStepsDispatcher {
		public MarkDispatcher ()
			: base (new BaseSubStep [] {
				new ApplyPreserveAttribute (),
				new MarkNSObjects ()
			})
		{
		}
	}
}
