using System;
using System.Collections.Generic;

using Xamarin.Bundler;

namespace Xamarin.Linker {
	public abstract class ConfigurationAwareSubStep : ExceptionalSubStep {
		protected override void Report (Exception exception)
		{
			LinkerConfiguration.Report (Context, exception);
		}

		protected void Report (List<Exception> exceptions)
		{
			LinkerConfiguration.Report (Context, exceptions);
		}
	}
}
