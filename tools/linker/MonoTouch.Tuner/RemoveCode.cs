using System;
using Mono.Linker;
using Mono.Tuner;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Xamarin.Linker;

namespace MonoTouch.Tuner {

	public class RemoveCode : BaseSubStep {

		MethodDefinition get_nse_def;
		MethodReference get_nse;
		bool product;

		public RemoveCode (LinkerOptions options)
		{
			Device = options.Device;
			Debug = options.DebugBuild;
		}

		public bool Device { get; set; }

		public bool Debug { get; set; }

		public override SubStepTargets Targets {
			get { return SubStepTargets.Assembly | SubStepTargets.Type; }
		}

		public override bool IsActiveFor (AssemblyDefinition assembly)
		{
			switch (assembly.Name.Name) {
			case "mscorlib":
				product = false;
				return context.Annotations.GetAction (assembly) == AssemblyAction.Link;
			case "monotouch":
			case "Xamarin.iOS":
			case "Xamarin.TVOS":
			case "Xamarin.WatchOS":
				product = true;
				return context.Annotations.GetAction (assembly) == AssemblyAction.Link;
			default:
				return false;
			}
		}

		public override void ProcessAssembly (AssemblyDefinition assembly)
		{
			if (get_nse_def == null) {
				var corlib = context.GetAssembly ("mscorlib");
				var nse = corlib.MainModule.GetType ("System", "NotSupportedException");
				foreach (var m in nse.Methods) {
					// no need to check HasMethods because we know there are (and nothing is removed at this stage)
					if (m.Name != "LinkedAway")
						continue;
					get_nse_def = m;
					break;
				}
			}

			// import the method into the current assembly
			get_nse = assembly.MainModule.Import (get_nse_def);
		}

		public override void ProcessType (TypeDefinition type)
		{
			// no code to remove in interfaces, skip processing
			if (type.IsInterface)
				return;
			
			// [MonoTouch.]ObjCRuntime.Runtime.RegisterEntryAssembly is needed only for the simulator 
			// and does not have to be preserved on devices
			if (product) {
				if (Device && type.Is (Namespaces.ObjCRuntime, "Runtime")) {
					foreach (var m in type.Methods) {
						if (m.Name == "RegisterEntryAssembly") {
							ProcessMethod (m);
							type.Module.Import (get_nse);
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

		void ProcessMethods (TypeDefinition type)
		{
			if (type.HasMethods) {
				MethodDefinition static_ctor = null;
				foreach (MethodDefinition method in type.Methods) {
					if (method.IsConstructor && method.IsStatic)
						static_ctor = method;
					else
						ProcessMethod (method);
				}
				if (static_ctor != null)
					type.Methods.Remove (static_ctor);
			}
		}

		new void ProcessMethod (MethodDefinition method)
		{
			ProcessParameters (method);

			if (!method.HasBody)
				return;

			var body = new MethodBody (method);

			var il = body.GetILProcessor ();
			il.Emit (OpCodes.Call, get_nse);
			il.Emit (OpCodes.Throw);

			method.Body = body;
		}

		static void ProcessParameters (MethodDefinition method)
		{
			if (!method.HasParameters)
				return;

			foreach (ParameterDefinition parameter in method.Parameters)
				parameter.Name = string.Empty;
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
				return type.Name != "AsyncResult" && type.Name != "LogicalCallContext";
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
