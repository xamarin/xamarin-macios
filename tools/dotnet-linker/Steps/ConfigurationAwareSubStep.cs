using System;
using System.Collections.Generic;

using Xamarin.Bundler;

namespace Xamarin.Linker {
	public abstract class ConfigurationAwareSubStep : ExceptionalSubStep {
		protected override void Report (Exception exception)
		{
			ErrorHelper.Show (exception);
		}

		protected void Report (List<Exception> exceptions)
		{
			// Maybe there's a better way to show errors that integrates with the linker?
			// We can't just throw an exception or exit here, since there might be only warnings in the list of exceptions.
			ErrorHelper.Show (exceptions);
		}
	}
}
