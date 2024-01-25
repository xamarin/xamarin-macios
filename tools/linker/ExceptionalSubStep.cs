// Copyright 2016 Xamarin Inc.

using System;

using Mono.Cecil;
using Mono.Tuner;

using Xamarin.Bundler;

using Xamarin.Tuner;

#if NET
using Mono.Linker;
using Mono.Linker.Steps;
#endif

namespace Xamarin.Linker {

	public abstract class ExceptionalSubStep : BaseSubStep {

		protected DerivedLinkContext LinkContext {
			get {
#if NET
				return Configuration.DerivedLinkContext;
#else
				return (DerivedLinkContext) base.context;
#endif
			}
		}

#if NET
		protected LinkContext context {
			get { return Context; }
		}

		protected LinkerConfiguration Configuration {
			get { return LinkerConfiguration.GetInstance (Context); }
		}

		protected Profile Profile {
			get {
				return Configuration.Profile;
			}
		}
#endif

		public override sealed void ProcessAssembly (AssemblyDefinition assembly)
		{
			try {
				Process (assembly);
			} catch (Exception e) {
				Report (Fail (assembly, e));
			}
		}

		public override sealed void ProcessType (TypeDefinition type)
		{
			try {
				Process (type);
			} catch (Exception e) {
				Report (Fail (type, e));
			}
		}

		public override sealed void ProcessField (FieldDefinition field)
		{
			try {
				Process (field);
			} catch (Exception e) {
				Report (Fail (field, e));
			}
		}

		public override sealed void ProcessMethod (MethodDefinition method)
		{
			try {
				Process (method);
			} catch (Exception e) {
				Report (Fail (method, e));
			}
		}

		public override sealed void ProcessProperty (PropertyDefinition property)
		{
			try {
				Process (property);
			} catch (Exception e) {
				Report (Fail (property, e));
			}
		}

		public override sealed void ProcessEvent (EventDefinition @event)
		{
			try {
				Process (@event);
			} catch (Exception e) {
				Report (Fail (@event, e));
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

		protected virtual void Process (PropertyDefinition property)
		{
		}

		protected virtual void Process (EventDefinition @event)
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

		protected virtual Exception Fail (PropertyDefinition property, Exception e)
		{
			return ErrorHelper.CreateError (ErrorCode | 4, e, Errors.MX_ExceptionalSubSteps, Name, property?.FullName);
		}

		protected virtual Exception Fail (EventDefinition @event, Exception e)
		{
			return ErrorHelper.CreateError (ErrorCode | 5, e, Errors.MX_ExceptionalSubSteps, Name, @event?.FullName);
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
