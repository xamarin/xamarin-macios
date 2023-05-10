using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Mono.Cecil;
using Mono.Linker.Steps;
using Xamarin.Tuner;

using Xamarin.Bundler;

#nullable enable

namespace Xamarin.Linker {
	public abstract class ConfigurationAwareStep : BaseStep {
		public LinkerConfiguration Configuration {
			get { return LinkerConfiguration.GetInstance (Context); }
		}

		public DerivedLinkContext DerivedLinkContext {
			get { return Configuration.DerivedLinkContext; }
		}

		public Application App {
			get { return DerivedLinkContext.App; }
		}

		protected void Report (params Exception [] exceptions)
		{
			Report ((IList<Exception>) exceptions);
		}

		protected void Report (IList<Exception>? exceptions)
		{
			if (exceptions is null)
				return;

			LinkerConfiguration.Report (Context, exceptions);
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
				TryEndProcess (out var exceptions);
				Report (exceptions);
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

		protected virtual void TryEndProcess (out List<Exception>? exceptions)
		{
			exceptions = null;
			TryEndProcess ();
		}

		// failure overrides, with defaults

		bool CollectProductExceptions (Exception e, [NotNullWhen (true)] out List<ProductException>? productExceptions)
		{
			if (e is ProductException pe) {
				productExceptions = new List<ProductException> ();
				productExceptions.Add (pe);
				return true;
			}

			if (e is AggregateException ae && ae.InnerExceptions.All (v => v is ProductException)) {
				productExceptions = new List<ProductException> (ae.InnerExceptions.Cast<ProductException> ());
				return true;
			}

			productExceptions = null;
			return false;
		}

		protected virtual Exception [] Fail (AssemblyDefinition assembly, Exception e)
		{
			return CollectExceptions (e, () => ErrorHelper.CreateError (ErrorCode, Errors.MX_ConfigurationAwareStepWithAssembly, Name, assembly?.FullName, e.Message));
		}

		protected virtual Exception [] Fail (Exception e)
		{
			return CollectExceptions (e, () => ErrorHelper.CreateError (ErrorCode | 1, Errors.MX_ConfigurationAwareStep, Name, e.Message));
		}

		protected virtual Exception [] FailEnd (Exception e)
		{
			return CollectExceptions (e, () => ErrorHelper.CreateError (ErrorCode | 2, Errors.MX_ConfigurationAwareStep, Name, e.Message));
		}

		Exception [] CollectExceptions (Exception e, Func<ProductException> createException)
		{
			// Detect if we're reporting one or more ProductExceptions (and no other exceptions), and in that case
			// report the product exceptions as top-level exceptions + the step-specific exception at the end,
			// instead of the step-specific exception with all the other exceptions as an inner exception.
			// This makes the errors show up nicer in the output.
			// If we're only reporting warnings, then don't add the step-specific exception at all.
			if (CollectProductExceptions (e, out var productExceptions)) {
				// don't add inner exception
				if (productExceptions.Any (v => v.Error)) {
					var ex = createException ();
					// instead return an aggregate exception with the original exception and all the ProductExceptions we're reporting.
					productExceptions.Add (ex);
				}
				return productExceptions.ToArray ();
			}

			return new Exception [] { createException () };
		}

		// abstracts

		protected abstract string Name { get; }

		protected abstract int ErrorCode { get; }
	}
}
