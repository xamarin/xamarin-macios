// Copyright 2016-2017 Xamarin Inc.

using System;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Linker;
using Mono.Tuner;

namespace Xamarin.Linker.Steps {

	// This inlining is done, almost exclusively, as a metadata reduction step that
	// occurs before linking so some code is not marked (and shipped in final apps).
	//
	// In many case the AOT'ed native code won't be affected (same size) but the
	// *.aotdata files will be smaller. In some cases the AOT compiler does not 
	// inline some _simple_ cases (cross assemblies) so it can reduce the native 
	// executable size too (but this is not the step main goal)
	public class InlinerSubStep : ExceptionalSubStep {

		protected override string Name { get; } = "Inliner";
		protected override int ErrorCode { get; } = 2090;

		public override SubStepTargets Targets {
			get {
				return SubStepTargets.Type | SubStepTargets.Method;
			}
		}

		public override bool IsActiveFor (AssemblyDefinition assembly)
		{
			return Annotations.GetAction (assembly) == AssemblyAction.Link;
		}

		protected override void Process (MethodDefinition method)
		{
			if (!method.HasBody)
				return;

			foreach (var il in method.Body.Instructions) {
				if (il.OpCode.Code == Code.Call) {
					var mr = il.Operand as MethodReference;
					if (mr is null)
						continue;
					// this removes type System.Security.SecurityManager (unless referenced by user code)
					if (!mr.HasParameters && mr.DeclaringType.Is ("System.Security", "SecurityManager")) {
						switch (mr.Name) {
						case "EnsureElevatedPermissions":
							il.OpCode = OpCodes.Nop;
							break;
						case "CheckElevatedPermissions":
							// always positive (no security manager)
							il.OpCode = OpCodes.Ldc_I4_1;
							break;
						}
					}
				}
			}
		}
	}
}
