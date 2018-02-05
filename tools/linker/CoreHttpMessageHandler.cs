// Copyright 2015-2017 Xamarin Inc. All rights reserved.

using System;
using System.Linq;
using Mono.Linker;
using Mono.Tuner;
using Mono.Cecil;
using Mono.Cecil.Cil;

#if MTOUCH
using MonoTouch;
using MonoTouch.Tuner;
using Xamarin.Bundler;
#elif MMP || MMP_TEST
using MonoMac;
using MonoMac.Tuner;
using Xamarin.Bundler;
#else
using ObjCRuntime;
#endif

namespace Xamarin.Linker.Steps {

	public class CoreHttpMessageHandler : ExceptionalSubStep {
		
		public CoreHttpMessageHandler (LinkerOptions options)
		{
			Options = options;
		}

		public LinkerOptions Options { get; private set; }

		public override SubStepTargets Targets {
			get { return SubStepTargets.Type; }
		}

		protected override string Name { get; } = "Default HttpMessageHandler setter";
		protected override int ErrorCode { get; } = 2040;

		public override bool IsActiveFor (AssemblyDefinition assembly)
		{
			switch (assembly.Name.Name) {
#if MONOMAC
			case "Xamarin.Mac":
				return context.Annotations.GetAction (assembly) == AssemblyAction.Link;
#else
			case "System.Net.Http":
				return context.Annotations.GetAction (assembly) == AssemblyAction.Link;
#endif
			default:
				return false;
			}
		}

#if MONOMAC
		protected override void Process (TypeDefinition type)
		{
			if (!type.Is ("ObjCRuntime", "RuntimeOptions"))
				return;

			MethodDefinition method = type.Methods.First (x => x.Name == "GetHttpMessageHandler" && !x.HasParameters);

			AssemblyDefinition systemNetHTTPAssembly = context.GetAssemblies ().First (x => x.Name.Name == "System.Net.Http");
			TypeDefinition handler = RuntimeOptions.GetHttpMessageHandler (Driver.App, Options.RuntimeOptions, systemNetHTTPAssembly.MainModule, type.Module);
			MethodReference handler_ctor = handler.Methods.First (x => x.IsConstructor && !x.HasParameters && !x.IsStatic);

			// HttpClientHandler is defined not in Xamarin.Mac.dll so we need to import
			if (handler.Name.Contains ("HttpClientHandler"))
				handler_ctor = type.Module.ImportReference (handler_ctor);

			var body = new MethodBody (method);
			var il = body.GetILProcessor ();
			il.Emit (OpCodes.Newobj, handler_ctor);
			il.Emit (OpCodes.Ret);
			method.Body = body;
		}
#else
		protected override void Process (TypeDefinition type)
		{
			if (!type.Is ("System.Net.Http", "HttpClient"))
				return;
			
			MethodDefinition default_ctor = null;
			MethodDefinition full_ctor = null;
			foreach (var m in type.Methods) {
				if (m.IsStatic || !m.IsConstructor)
					continue;
				if (!m.HasParameters) {
					default_ctor = m;
				} else if (m.Parameters.Count == 2) {
					full_ctor = m;
				}
			}

			if (default_ctor == null || full_ctor == null)
				throw new Exception ("Could not set the default HttpMessageHandler");

			var handler = RuntimeOptions.GetHttpMessageHandler (Options.Application, Options.RuntimeOptions, type.Module);

			MethodDefinition handler_ctor = null;
			foreach (var m in handler.Methods) {
				if (m.IsStatic || !m.IsConstructor || m.HasParameters)
					continue;
				handler_ctor = m;
				break;
			}
			// re-write default ctor
			var body = new MethodBody (default_ctor);
			var il = body.GetILProcessor ();
			il.Emit (OpCodes.Ldarg_0);
			il.Emit (OpCodes.Newobj, handler_ctor);
			il.Emit (OpCodes.Ldc_I4_1);
			il.Emit (OpCodes.Call, full_ctor);
			il.Emit (OpCodes.Ret);
			default_ctor.Body = body;
		}
#endif
	}
}
