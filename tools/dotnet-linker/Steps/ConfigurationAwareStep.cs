using System;
using System.Collections.Generic;

using Mono.Cecil;
using Mono.Linker.Steps;

using Xamarin.Bundler;

namespace Xamarin.Linker {
	public abstract class ConfigurationAwareStep : BaseStep {
		public LinkerConfiguration Configuration {
			get { return LinkerConfiguration.GetInstance (Context); }
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

		protected virtual void TryProcess ()
		{
			base.Process ();
		}

		protected sealed override void Process ()
		{
			try {
				TryProcess ();
			} catch (Exception e) {
				Report (e);
				throw;
			}
		}

		protected virtual void TryProcessAssembly (AssemblyDefinition assembly)
		{
			base.ProcessAssembly (assembly);
		}

		protected sealed override void ProcessAssembly (AssemblyDefinition assembly)
		{
			try {
				TryProcessAssembly (assembly);
			} catch (Exception e) {
				Report (e);
				throw;
			}
		}

		protected virtual void TryEndProcess ()
		{
			base.EndProcess ();
		}

		protected sealed override void EndProcess ()
		{
			try {
				TryEndProcess ();
			} catch (Exception e) {
				Report (e);
				throw;
			}
		}
	}
}
