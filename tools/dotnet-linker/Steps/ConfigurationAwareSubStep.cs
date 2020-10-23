using System;
using System.Collections.Generic;
using Mono.Cecil;
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

		protected virtual bool TryIsActiveFor (AssemblyDefinition assembly)
		{
			return base.IsActiveFor (assembly);
		}

		public sealed override bool IsActiveFor (AssemblyDefinition assembly)
		{
			try {
				return TryIsActiveFor (assembly);
			} catch (Exception e) {
				Report (e);
				throw;
			}
		}

		protected virtual void TryProcessAssembly (AssemblyDefinition assembly)
		{
			base.ProcessAssembly (assembly);
		}

		public sealed override void ProcessAssembly (AssemblyDefinition assembly)
		{
			try {
				TryProcessAssembly (assembly);
			} catch (Exception e) {
				Report (e);
				throw;
			}
		}

		protected virtual void TryProcessEvent (EventDefinition @event)
		{
			base.ProcessEvent (@event);
		}

		public sealed override void ProcessEvent (EventDefinition @event)
		{
			try {
				TryProcessEvent (@event);
			} catch (Exception e) {
				Report (e);
				throw;
			}
		}

		protected virtual void TryProcessField (FieldDefinition field)
		{
			base.ProcessField (field);
		}

		public sealed override void ProcessField (FieldDefinition field)
		{
			try {
				TryProcessField (field);
			} catch (Exception e) {
				Report (e);
				throw;
			}
		}

		protected virtual void TryProcessMethod (MethodDefinition method)
		{
			base.ProcessMethod (method);
		}

		public sealed override void ProcessMethod (MethodDefinition method)
		{
			try {
				TryProcessMethod (method);
			} catch (Exception e) {
				Report (e);
				throw;
			}
		}

		protected virtual void TryProcessProperty (PropertyDefinition property)
		{
			base.ProcessProperty (property);
		}

		public sealed override void ProcessProperty (PropertyDefinition property)
		{
			try {
				TryProcessProperty (property);
			} catch (Exception e) {
				Report (e);
				throw;
			}
		}

		protected virtual void TryProcessType (TypeDefinition type)
		{
			base.ProcessType (type);
		}

		public sealed override void ProcessType (TypeDefinition type)
		{
			try {
				TryProcessType (type);
			} catch (Exception e) {
				Report (e);
				throw;
			}
		}
	}
}

