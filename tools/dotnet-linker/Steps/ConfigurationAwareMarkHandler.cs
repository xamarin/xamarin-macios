using System;
using System.Collections.Generic;

using Xamarin.Bundler;

namespace Xamarin.Linker {
	public abstract class ConfigurationAwareMarkHandler : ExceptionalMarkHandler {
		protected override void Report (Exception exception)
		{
			LinkerConfiguration.Report (context, exception);
		}

		protected void Report (List<Exception> exceptions)
		{
			LinkerConfiguration.Report (context, exceptions);
		}
	}
}
