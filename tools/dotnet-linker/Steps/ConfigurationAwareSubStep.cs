using System;
using System.Collections.Generic;
using Mono.Linker;
using Mono.Linker.Steps;

using Xamarin.Bundler;
using Xamarin.Tuner;

namespace Xamarin.Linker {
	public abstract class ConfigurationAwareSubStep : BaseSubStep {
		public LinkerConfiguration Configuration { get; private set; }

		public DerivedLinkContext LinkContext {
			get { return Configuration.DerivedLinkContext; }
		}

		public override sealed void Initialize (LinkContext context)
		{
			base.Initialize (context);

			Configuration = LinkerConfiguration.GetInstance (context);
		}

		protected void Report (Exception exception)
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

