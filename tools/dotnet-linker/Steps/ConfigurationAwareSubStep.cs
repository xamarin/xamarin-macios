using System;
using System.Collections.Generic;

using Xamarin.Bundler;

namespace Xamarin.Linker {
	public abstract class ConfigurationAwareSubStep : ExceptionalSubStep {
		protected override void Report (Exception exception)
		{
			Configuration.Report (exception);
		}

		protected void Report (List<Exception> exceptions)
		{
			Configuration.Report (exceptions);
		}
	}
}
