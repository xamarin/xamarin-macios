#nullable enable

namespace Xamarin.Linker {
	public class DoneStep : ConfigurationAwareStep {
		protected override string Name { get; } = "Done";
		protected override int ErrorCode { get; } = 2350;

		protected override void TryEndProcess ()
		{
			base.TryEndProcess ();

			Configuration.FlushOutputForMSBuild ();
		}
	}
}
