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
			Configuration.Report (exception);
		}

		protected void Report (List<Exception> exceptions)
		{
			// Maybe there's a better way to show errors that integrates with the linker?
			// We can't just throw an exception or exit here, since there might be only warnings in the list of exceptions.
			ErrorHelper.Show (exceptions);
		}

		protected sealed override void Process ()
		{
			try {
				TryProcess ();
			} catch (Exception e) {
				Report (Fail (e));
			}
		}

		protected sealed override void ProcessAssembly (AssemblyDefinition assembly)
		{
			try {
				TryProcessAssembly (assembly);
			} catch (Exception e) {
				Report (Fail (assembly, e));
			}
		}

		protected sealed override void EndProcess ()
		{
			try {
				TryEndProcess ();
			} catch (Exception e) {
				Report (FailEnd (e));
			}
		}

		// state-aware versions to be subclassed
		protected virtual void TryProcess ()
		{
		}

		protected virtual void TryProcessAssembly (AssemblyDefinition assembly)
		{
		}

		protected virtual void TryEndProcess ()
		{
		}

		// failure overrides, with defaults

		protected virtual Exception Fail (AssemblyDefinition assembly, Exception e)
		{
			return ErrorHelper.CreateError (ErrorCode, e, Errors.MX_ConfigurationAwareStepWithAssembly, Name, assembly?.FullName, e.Message);
		}

		protected virtual Exception Fail (Exception e)
		{
			return ErrorHelper.CreateError (ErrorCode | 1, e, Errors.MX_ConfigurationAwareStep, Name, e.Message);
		}

		protected virtual Exception FailEnd (Exception e)
		{
			return ErrorHelper.CreateError (ErrorCode | 2, e, Errors.MX_ConfigurationAwareStep, Name, e.Message);
		}

		// abstracts

		protected abstract string Name { get; }

		protected abstract int ErrorCode { get; }
	}
}
