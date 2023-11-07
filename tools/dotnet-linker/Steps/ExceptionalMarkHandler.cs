// Copyright 2016 Xamarin Inc.

using System;
using Mono.Cecil;
using Xamarin.Bundler;

using Xamarin.Tuner;

using Mono.Linker;
using Mono.Linker.Steps;

#nullable enable

namespace Xamarin.Linker {

	// Similar to ExceptionalSubStep, but this only runs for marked members
	// that were registered for handling by the subclass.
	public abstract class ExceptionalMarkHandler : IMarkHandler {
		LinkContext? context;

		public abstract void Initialize (LinkContext context, MarkContext markContext);

		public virtual void Initialize (LinkContext context)
		{
			this.context = context;
		}

		protected DerivedLinkContext LinkContext => Configuration.DerivedLinkContext;

		protected LinkContext Context { get { return context!; } }

		protected AnnotationStore Annotations => Context.Annotations;
		protected LinkerConfiguration Configuration => LinkerConfiguration.GetInstance (Context);

		protected Profile Profile => Configuration.Profile;

		public void ProcessAssembly (AssemblyDefinition assembly)
		{
			try {
				Process (assembly);
			} catch (Exception e) {
				Report (Fail (assembly, e));
			}
		}

		public void ProcessType (TypeDefinition type)
		{
			try {
				Process (type);
			} catch (Exception e) {
				Report (Fail (type, e));
			}
		}

		public void ProcessField (FieldDefinition field)
		{
			try {
				Process (field);
			} catch (Exception e) {
				Report (Fail (field, e));
			}
		}

		public void ProcessMethod (MethodDefinition method)
		{
			try {
				Process (method);
			} catch (Exception e) {
				Report (Fail (method, e));
			}
		}

		// state-aware versions to be subclassed

		protected virtual void Process (AssemblyDefinition assembly)
		{
		}

		protected virtual void Process (TypeDefinition type)
		{
		}

		protected virtual void Process (FieldDefinition field)
		{
		}

		protected virtual void Process (MethodDefinition method)
		{
		}

		// failure overrides, with defaults

		protected virtual Exception Fail (AssemblyDefinition assembly, Exception e)
		{
			return ErrorHelper.CreateError (ErrorCode, e, Errors.MX_ExceptionalSubSteps, Name, assembly?.FullName);
		}

		protected virtual Exception Fail (TypeDefinition type, Exception e)
		{
			return ErrorHelper.CreateError (ErrorCode | 1, e, Errors.MX_ExceptionalSubSteps, Name, type?.FullName);
		}

		protected virtual Exception Fail (FieldDefinition field, Exception e)
		{
			return ErrorHelper.CreateError (ErrorCode | 2, e, Errors.MX_ExceptionalSubSteps, Name, field?.FullName);
		}

		protected virtual Exception Fail (MethodDefinition method, Exception e)
		{
			return ErrorHelper.CreateError (ErrorCode | 3, e, Errors.MX_ExceptionalSubSteps, Name, method?.FullName);
		}
		protected virtual void Report (Exception e)
		{
			throw e;
		}

		// abstracts

		protected abstract string Name { get; }

		protected abstract int ErrorCode { get; }
	}
}
