using System;
using Mono.Linker;
using Mono.Tuner;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Xamarin.Linker;

namespace MonoTouch.Tuner {

	public class RemoveCode : RemoveCodeBase {

		bool product;

		protected override string Name { get; } = "Code Remover";
		protected override int ErrorCode { get; } = 2050;

		public bool Device { get { return LinkContext.App.IsDeviceBuild; } }

		public bool Debug { get { return LinkContext.App.EnableDebug; } }

		public override SubStepTargets Targets {
			get { return SubStepTargets.Assembly | SubStepTargets.Type; }
		}

		public override bool IsActiveFor (AssemblyDefinition assembly)
		{
			if (!LinkContext.Target.LinkerOptions.LinkAway)
				return false;

			switch (assembly.Name.Name) {
			case "mscorlib":
				product = false;
				return context.Annotations.GetAction (assembly) == AssemblyAction.Link;
			case "Xamarin.iOS":
			case "Xamarin.TVOS":
			case "Xamarin.WatchOS":
				product = true;
				return context.Annotations.GetAction (assembly) == AssemblyAction.Link;
			default:
				return false;
			}
		}

		protected override void Process (TypeDefinition type)
		{
			// no code to remove in interfaces, skip processing
			if (type.IsInterface)
				return;

			// [MonoTouch.]ObjCRuntime.Runtime.RegisterEntryAssembly is needed only for the simulator 
			// and does not have to be preserved on devices
			if (product) {
				if (LinkContext.Target.App.Optimizations.RemoveDynamicRegistrar == true && type.Is (Namespaces.ObjCRuntime, "Runtime")) {
					foreach (var m in type.Methods) {
						if (m.Name == "RegisterEntryAssembly") {
							ProcessMethod (m);
							type.Module.ImportReference (NotSupportedException);
						}
					}
				}
				// Remove the warning that we show (console) if someone subclass UIButton using the UIButtonType ctor
				// https://trello.com/c/Nf2B8mIM/484-remove-debug-code-in-the-linker
				if (!Debug && type.Is (Namespaces.UIKit, "UIButton")) {
					foreach (var m in type.Methods) {
						if (m.IsConstructor && m.HasParameters && m.Parameters [0].ParameterType.Is (Namespaces.UIKit, "UIButtonType"))
							ProcessUIButtonCtor (m);
					}
				}
				// all other candidates are in mscorlib.dll (not the product .dll)
				return;
			}

			if (!IsCandidate (type))
				return;

			ProcessMethods (type);
			if (type.HasNestedTypes) {
				foreach (TypeDefinition nested in type.NestedTypes)
					ProcessMethods (nested);
			}
		}

		// removing the call to VerifyIsUIButton ensure the code is unreachable and won't be part of the final app
		// even if the main optimization is to save execution time
		void ProcessUIButtonCtor (MethodDefinition ctor)
		{
			foreach (var ins in ctor.Body.Instructions) {
				if (ins.OpCode.Code != Code.Call)
					continue;
				var callee = ins.Operand as MethodReference;
				if (callee.Name != "VerifyIsUIButton")
					continue;
				// remove previous: ldarg.0 (instance method)
				ins.Previous.OpCode = OpCodes.Nop;
				// remove the call itself
				ins.OpCode = OpCodes.Nop;
				ins.Operand = null;
				break;
			}
		}

		static bool IsCandidate (TypeDefinition type)
		{
			switch (type.Namespace) {
			case "System.Runtime.Serialization.Formatters.Binary":
				return type.Name == "CodeGenerator";
			case "System.Runtime.Remoting":
				return type.Name != "ObjectHandle";
			case "System.Runtime.Remoting.Activation":
			case "System.Runtime.Remoting.Channels":
			case "System.Runtime.Remoting.Contexts":
			case "System.Runtime.Remoting.Proxies":
				return true;
			case "System.Runtime.Remoting.Messaging":
				switch (type.Name) {
				case "AsyncResult":
				case "CallContextSecurityData":
				case "LogicalCallContext":
				case "MonoMethodMessage":
					return false;
				}
				return true;
			case "System.Security.AccessControl":
			case "System.Security.Permissions":
			case "System.Security.Policy":
				return true;
			case "System.Security":
				switch (type.Name) {
				case "CodeAccessPermission":
				case "PermissionSet":
					return true;
				default:
					return false;
				}
			default:
				return false;
			}
		}
	}
}
