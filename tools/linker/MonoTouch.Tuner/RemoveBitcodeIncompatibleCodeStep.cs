// Copyright 2012-2014, 2016 Xamarin Inc. All rights reserved.
using System.Collections.Generic;
using Mono.Tuner;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Linker.Steps;

using Xamarin.Bundler;
using Xamarin.Linker;
using Mono.Linker;
using System;

namespace MonoTouch.Tuner {
	
	public class RemoveBitcodeIncompatibleCodeStep : BaseSubStep {

		LinkerOptions Options;
		MethodDefinition nse_ctor_def;
		Dictionary<ModuleDefinition, MethodReference> nse_ctors;

		public RemoveBitcodeIncompatibleCodeStep (LinkerOptions options)
		{
			Options = options;
		}

		public override SubStepTargets Targets {
			get {
				return SubStepTargets.Method | SubStepTargets.Type /* We don't care about types, but if not set a NullReferenceException occurs in BaseSubStep */;
			}
		}
		
		public override void ProcessMethod (MethodDefinition method)
		{
			if (!method.HasBody)
				return;

			var body = method.Body;
			if (!body.HasExceptionHandlers)
				return;

			var anyFilterClauses = false;
			foreach (var eh in body.ExceptionHandlers) {
				if (eh.HandlerType == ExceptionHandlerType.Filter) {
					anyFilterClauses = true;
					ErrorHelper.Show (ErrorHelper.CreateWarning (Options.Application, 2105, method, $"The method {method.DeclaringType.FullName}.{method.Name} contains a '{eh.HandlerType}' exception clause, which is currently not supported when compiling for bitcode. This method will throw an exception if called."));
					break;
				}
			}
			if (!anyFilterClauses)
				return;

			body = new MethodBody (method);
			var il = body.GetILProcessor ();
			il.Emit (OpCodes.Ldstr, "This method contains IL not supported when compiled to bitcode.");
			if (nse_ctor_def == null) {
				var corlib = context.GetAssembly ("mscorlib");
				var nse = corlib.MainModule.GetType ("System", "NotSupportedException");
				foreach (var ctor in nse.GetConstructors ()) {
					if (!ctor.HasParameters)
						continue;
					var parameters = ctor.Parameters;
					if (parameters.Count != 1)
						continue;
					if (!parameters [0].ParameterType.Is ("System", "String"))
						continue;
					nse_ctor_def = ctor;
					break;
				}
				nse_ctors = new Dictionary<ModuleDefinition, MethodReference> ();
			}
			MethodReference nse_ctor;
			if (!nse_ctors.TryGetValue (method.Module, out nse_ctor)) {
				nse_ctors [method.Module] = nse_ctor = method.Module.ImportReference (nse_ctor_def);

				// We're processing all assemblies, not linked assemblies, so
				// make sure we're saving any changes to non-linked assemblies as well.
				var assembly = method.Module.Assembly;
				var action = Annotations.GetAction (assembly);
				switch (action) {
				case AssemblyAction.Link:
				case AssemblyAction.Save:
					break;
				default:
					Annotations.SetAction (assembly, AssemblyAction.Save);
					break;
				}
			}
			il.Emit (OpCodes.Newobj, nse_ctor);
			il.Emit (OpCodes.Throw);
			method.Body = body;

		}
	}
}
