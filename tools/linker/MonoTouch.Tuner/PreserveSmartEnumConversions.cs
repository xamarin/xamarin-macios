// Copyright 2017 Xamarin Inc.

using System;
using System.Collections.Generic;

using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Linker;
using Mono.Tuner;
#if NET
using Mono.Linker.Steps;
#else
using MonoTouch.Tuner;
#endif

using Xamarin.Bundler;

namespace Xamarin.Linker.Steps {
#if NET
	public class PreserveSmartEnumConversionsHandler : ExceptionalMarkHandler
#else
	public class PreserveSmartEnumConversionsSubStep : ExceptionalSubStep
#endif
	{
		Dictionary<TypeDefinition, Tuple<MethodDefinition, MethodDefinition>> cache;
		protected override string Name { get; } = "Smart Enum Conversion Preserver";
		protected override int ErrorCode { get; } = 2200;

#if NET
		public override void Initialize (LinkContext context, MarkContext markContext)
		{
			base.Initialize (context);
			markContext.RegisterMarkMethodAction (ProcessMethod);
		}
#else
		public override SubStepTargets Targets {
			get {
				return SubStepTargets.Method | SubStepTargets.Property;
			}
		}
#endif

#if NET
		bool IsActiveFor (AssemblyDefinition assembly)
#else
		public override bool IsActiveFor (AssemblyDefinition assembly)
#endif
		{
			if (Profile.IsProductAssembly (assembly))
				return true;

			// We don't need to process assemblies that don't reference ObjCRuntime.BindAsAttribute.
			foreach (var tr in assembly.MainModule.GetTypeReferences ()) {
				if (tr.Is ("ObjCRuntime", "BindAsAttribute"))
					return true;
			}

			return false;
		}

#if NET
		void Mark (Tuple<MethodDefinition, MethodDefinition> pair)
		{
			Context.Annotations.Mark (pair.Item1);
			Context.Annotations.Mark (pair.Item2);
		}
#else
		void Preserve (Tuple<MethodDefinition, MethodDefinition> pair, MethodDefinition conditionA, MethodDefinition conditionB = null)
		{
			if (conditionA is not null) {
				context.Annotations.AddPreservedMethod (conditionA, pair.Item1);
				context.Annotations.AddPreservedMethod (conditionA, pair.Item2);
			}
			if (conditionB is not null) {
				context.Annotations.AddPreservedMethod (conditionB, pair.Item1);
				context.Annotations.AddPreservedMethod (conditionB, pair.Item2);
			}
		}
#endif

#if NET
		void ProcessAttributeProvider (ICustomAttributeProvider provider)
#else
		void ProcessAttributeProvider (ICustomAttributeProvider provider, MethodDefinition conditionA, MethodDefinition conditionB = null)
#endif
		{
			if (provider?.HasCustomAttributes != true)
				return;

			foreach (var ca in provider.CustomAttributes) {
				var tr = ca.Constructor.DeclaringType;

				if (!tr.Is ("ObjCRuntime", "BindAsAttribute"))
					continue;

				if (ca.ConstructorArguments.Count != 1) {
					ErrorHelper.Show (ErrorHelper.CreateWarning (LinkContext.Target.App, 4124, provider, Errors.MT4124_E, provider.AsString (), ca.ConstructorArguments.Count));
					continue;
				}

				var managedType = ca.ConstructorArguments [0].Value as TypeReference;
				var managedEnumType = managedType?.GetElementType ().Resolve ();
				if (managedEnumType is null) {
					ErrorHelper.Show (ErrorHelper.CreateWarning (LinkContext.Target.App, 4124, provider, Errors.MT4124_H, provider.AsString (), managedType?.FullName));
					continue;
				}

				// We only care about enums, BindAs attributes can be used for other types too.
				if (!managedEnumType.IsEnum)
					continue;

				Tuple<MethodDefinition, MethodDefinition> pair;
				if (cache is not null && cache.TryGetValue (managedEnumType, out pair)) {
#if NET
					// The pair was already marked if it was cached.
#else
					Preserve (pair, conditionA, conditionB);
#endif
					continue;
				}

				// Find the Extension type
				TypeDefinition extensionType = null;
				var extensionName = managedEnumType.Name + "Extensions";
				foreach (var type in managedEnumType.Module.Types) {
					if (type.Namespace != managedEnumType.Namespace)
						continue;
					if (type.Name != extensionName)
						continue;
					extensionType = type;
					break;
				}
				if (extensionType is null) {
					Driver.Log (1, $"Could not find a smart extension type for the enum {managedEnumType.FullName} (due to BindAs attribute on {provider.AsString ()}): most likely this is because the enum isn't a smart enum.");
					continue;
				}

				// Find the GetConstant/GetValue methods
				MethodDefinition getConstant = null;
				MethodDefinition getValue = null;

				foreach (var method in extensionType.Methods) {
					if (!method.IsStatic)
						continue;
					if (!method.HasParameters || method.Parameters.Count != 1)
						continue;
					if (method.Name == "GetConstant") {
						if (!method.ReturnType.Is ("Foundation", "NSString"))
							continue;
						if (method.Parameters [0].ParameterType != managedEnumType)
							continue;
						getConstant = method;
					} else if (method.Name == "GetValue") {
						if (!method.Parameters [0].ParameterType.Is ("Foundation", "NSString"))
							continue;
						if (method.ReturnType != managedEnumType)
							continue;
						getValue = method;
					}
				}

				if (getConstant is null) {
					Driver.Log (1, $"Could not find the GetConstant method on the supposedly smart extension type {extensionType.FullName} for the enum {managedEnumType.FullName} (due to BindAs attribute on {provider.AsString ()}): most likely this is because the enum isn't a smart enum.");
					continue;
				}

				if (getValue is null) {
					Driver.Log (1, $"Could not find the GetValue method on the supposedly smart extension type {extensionType.FullName} for the enum {managedEnumType.FullName} (due to BindAs attribute on {provider.AsString ()}): most likely this is because the enum isn't a smart enum.");
					continue;
				}

				pair = new Tuple<MethodDefinition, MethodDefinition> (getConstant, getValue);
				if (cache is null)
					cache = new Dictionary<TypeDefinition, Tuple<MethodDefinition, MethodDefinition>> ();
				cache.Add (managedEnumType, pair);
#if NET
				Mark (pair);
#else
				Preserve (pair, conditionA, conditionB);
#endif
			}
		}

		protected override void Process (MethodDefinition method)
		{
#if NET
			static bool IsPropertyMethod (MethodDefinition method)
			{
				return method.IsGetter || method.IsSetter;
			}

			ProcessAttributeProvider (method);
			ProcessAttributeProvider (method.MethodReturnType);

			if (method.HasParameters) {
				foreach (var p in method.Parameters)
					ProcessAttributeProvider (p);
			}
			if (IsPropertyMethod (method)) {
				foreach (PropertyDefinition property in method.DeclaringType.Properties)
					if (property.GetMethod == method || property.SetMethod == method) {
						ProcessAttributeProvider (property);
						break;
					}
			}
#else
			ProcessAttributeProvider (method, method);
			ProcessAttributeProvider (method.MethodReturnType, method);
			if (method.HasParameters) {
				foreach (var p in method.Parameters)
					ProcessAttributeProvider (p, method);
			}
#endif
		}

#if !NET
		protected override void Process (PropertyDefinition property)
		{
			ProcessAttributeProvider (property, property.GetMethod, property.SetMethod);
			if (property.GetMethod is not null)
				Process (property.GetMethod);
			if (property.SetMethod is not null)
				Process (property.SetMethod);
		}
#endif
	}
}
