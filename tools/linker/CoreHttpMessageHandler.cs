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

		public CoreHttpMessageHandler ()
		{
		}

		public override SubStepTargets Targets {
			get { return SubStepTargets.Type; }
		}

		protected override string Name { get; } = "Default HttpMessageHandler setter";
		protected override int ErrorCode { get; } = 2040;

		Application App {
			get {
				return LinkContext.App;
			}
		}

		public override bool IsActiveFor (AssemblyDefinition assembly)
		{
			switch (assembly.Name.Name) {
#if MONOMAC
			case "Xamarin.Mac":
				return context.Annotations.GetAction (assembly) == AssemblyAction.Link;
#else
			case "Xamarin.iOS":
			case "Xamarin.TVOS":
			case "Xamarin.WatchOS":
				return context.Annotations.GetAction (assembly) == AssemblyAction.Link;
#endif
			default:
				return false;
			}
		}

		protected override void Process (TypeDefinition type)
		{
			if (!type.Is ("ObjCRuntime", "RuntimeOptions"))
				return;

			MethodDefinition method = type.Methods.First (x => x.Name == "GetHttpMessageHandler" && !x.HasParameters);

			AssemblyDefinition systemNetHTTPAssembly = context.GetAssemblies ().First (x => x.Name.Name == "System.Net.Http");
			TypeDefinition handler = RuntimeOptions.GetHttpMessageHandler (App, LinkContext.Target.LinkerOptions.RuntimeOptions, systemNetHTTPAssembly.MainModule, type.Module);
			MethodReference handler_ctor = handler.Methods.First (x => x.IsConstructor && !x.HasParameters && !x.IsStatic);

			// HttpClientHandler is defined in System.Net.Http.dll so we need to import
			if (handler.Name.Contains ("HttpClientHandler"))
				handler_ctor = type.Module.ImportReference (handler_ctor);

			var body = new MethodBody (method);
			var il = body.GetILProcessor ();
			il.Emit (OpCodes.Newobj, handler_ctor);
			il.Emit (OpCodes.Ret);
			method.Body = body;
		}
	}
}
