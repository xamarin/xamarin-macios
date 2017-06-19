// Copyright 2016 Xamarin Inc.

using System;
using Mono.Cecil;
using Mono.Tuner;
using Xamarin.Bundler;

using Xamarin.Tuner;

namespace Xamarin.Linker {

	public abstract class ExceptionalSubStep : BaseSubStep {

		protected DerivedLinkContext LinkContext {
			get {
				return (DerivedLinkContext) base.context;
			}
		}

		public override sealed void ProcessAssembly (AssemblyDefinition assembly)
		{
			try {
				Process (assembly);
			} catch (Exception e) {
				throw Fail (assembly, e);
			}
		}

		public override sealed void ProcessType (TypeDefinition type)
		{
			try {
				Process (type);
			} catch (Exception e) {
				throw Fail (type, e);
			}
		}

		public override sealed void ProcessField (FieldDefinition field)
		{
			try {
				Process (field);
			} catch (Exception e) {
				throw Fail (field, e);
			}
		}

		public override sealed void ProcessMethod (MethodDefinition method)
		{
			try {
				Process (method);
			} catch (Exception e) {
				throw Fail (method, e);
			}
		}

		public override sealed void ProcessProperty (PropertyDefinition property)
		{
			try {
				Process (property);
			} catch (Exception e) {
				throw Fail (property, e);
			}
		}

		public override sealed void ProcessEvent (EventDefinition @event)
		{
			try {
				Process (@event);
			} catch (Exception e) {
				throw Fail (@event, e);
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
			var message = $"{Name} failed processing `{assembly?.FullName}`.";
			return ErrorHelper.CreateError (ErrorCode, e, message);
		}

		protected virtual Exception Fail (TypeDefinition type, Exception e)
		{
			var message = $"{Name} failed processing {type?.FullName}.";
			return ErrorHelper.CreateError (ErrorCode | 1, e, message);
		}

		protected virtual Exception Fail (FieldDefinition field, Exception e)
		{
			var message = $"{Name} failed processing `{field?.FullName}`.";
			return ErrorHelper.CreateError (ErrorCode | 2, e, message);
		}

		protected virtual Exception Fail (MethodDefinition method, Exception e)
		{
			var message = $"{Name} failed processing `{method?.FullName}`.";
			return ErrorHelper.CreateError (ErrorCode | 3, e, message);
		}

		protected virtual Exception Fail (PropertyDefinition property, Exception e)
		{
			var message = $"{Name} failed processing {property?.FullName}.";
			return ErrorHelper.CreateError (ErrorCode | 4, e, message);
		}

		protected virtual Exception Fail (EventDefinition @event, Exception e)
		{
			var message = $"{Name} failed processing {@event?.FullName}.";
			return ErrorHelper.CreateError (ErrorCode | 5, e, message);
		}

		// abstracts

		protected abstract string Name { get; }

		protected abstract int ErrorCode { get; }
	}
}
