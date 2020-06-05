using System;

using Mono.Linker.Steps;

using Xamarin.Bundler;
using Xamarin.Linker;

namespace Xamarin {

	public class SetupStep : ConfigurationAwareStep {

		protected override void Process ()
		{
			// This will be replaced with something more useful later.
			Console.WriteLine ("Hello SetupStep");
			Configuration.Write ();

			ErrorHelper.Platform = Configuration.Platform;
		}
	}
}
