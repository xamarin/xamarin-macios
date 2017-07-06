// Copyright 2017 Xamarin Inc.

using System;
using System.Collections.Generic;

using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Linker;
using Mono.Tuner;
using MonoTouch.Tuner;

namespace Xamarin.Linker.Steps
{
	public class PreserveSmartEnumConversionsSubStep : ExceptionalSubStep
	{
		Dictionary<TypeDefinition, Tuple<MethodDefinition, MethodDefinition>> cache;
		protected override string Name { get; } = "Smart Enum Conversion Preserver";
		protected override int ErrorCode { get; } = 2100;

		public override SubStepTargets Targets {
			get {
				return SubStepTargets.Method | SubStepTargets.Property;
			}
		}

		public override bool IsActiveFor (AssemblyDefinition assembly)
		{
			// we need to process all assemblies, because the functions we want to
			// preserve are not necessarily in the assembly we're processing.
			return true;
		}

		void Preserve (Tuple<MethodDefinition, MethodDefinition> pair, MethodDefinition conditionA, MethodDefinition conditionB = null)
		{
			if (conditionA != null) {
				context.Annotations.AddPreservedMethod (conditionA, pair.Item1);
				context.Annotations.AddPreservedMethod (conditionA, pair.Item2);
			}
			if (conditionB != null) {
				context.Annotations.AddPreservedMethod (conditionB, pair.Item1);
				context.Annotations.AddPreservedMethod (conditionB, pair.Item2);
			}
		}

		void ProcessAttributeProvider (ICustomAttributeProvider provider, MethodDefinition conditionA, MethodDefinition conditionB = null)
		{
			if (provider?.HasCustomAttributes != true)
				return;

			foreach (var ca in provider.CustomAttributes) {
				var tr = ca.Constructor.DeclaringType;

				if (!tr.IsPlatformType ("ObjCRuntime", "BindAsAttribute"))
					continue;

				if (ca.ConstructorArguments.Count != 1) {
					Console.WriteLine ("WARNING");
					continue;
				}

				var managedType = ca.ConstructorArguments [0].Value as TypeReference;
				var managedEnumType = managedType?.GetElementType ().Resolve ();
				if (managedEnumType == null) {
					Console.WriteLine ("WARNING");
					continue;
				}

				Tuple<MethodDefinition, MethodDefinition> pair;
				if (cache != null && cache.TryGetValue (managedEnumType, out pair)) {
					Preserve (pair, conditionA, conditionB);
					continue;
				}

				Console.WriteLine (managedEnumType);
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
				if (extensionType == null) {
					Console.WriteLine ("WARNING");
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
						if (!method.ReturnType.IsPlatformType ("Foundation", "NSString"))
							continue;
						if (method.Parameters [0].ParameterType != managedEnumType)
							continue;
						getConstant = method;
					} else if (method.Name == "GetValue") {
						if (!method.Parameters [0].ParameterType.IsPlatformType ("Foundation", "NSString"))
							continue;
						if (method.ReturnType != managedEnumType)
							continue;
						getValue = method;
					}
				}
				if (getConstant == null || getValue == null) {
					Console.WriteLine ("WARNING");
					continue;
				}

				pair = new Tuple<MethodDefinition, MethodDefinition> (getConstant, getValue);
				if (cache == null)
					cache = new Dictionary<TypeDefinition, Tuple<MethodDefinition, MethodDefinition>> ();
				cache.Add (managedEnumType, pair);
				Preserve (pair, conditionA, conditionB);
			}
		}

		protected override void Process (MethodDefinition method)
		{
			ProcessAttributeProvider (method, method);
			ProcessAttributeProvider (method.MethodReturnType, method);
			if (method.HasParameters) {
				foreach (var p in method.Parameters)
					ProcessAttributeProvider (p, method);
			}
		}

		protected override void Process (PropertyDefinition property)
		{
			ProcessAttributeProvider (property, property.GetMethod, property.SetMethod);
			if (property.GetMethod != null)
				Process (property.GetMethod);
			if (property.SetMethod != null)
				Process (property.SetMethod);
		}
	}
}
