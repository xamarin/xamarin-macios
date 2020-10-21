using Mono.Linker.Steps;

namespace Xamarin.Linker {
	public abstract class ConfigurationAwareStep : BaseStep {
		public LinkerConfiguration Configuration {
			get { return LinkerConfiguration.GetInstance (Context); }
		}
	}
}
