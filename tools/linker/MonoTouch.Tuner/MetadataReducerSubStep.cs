// Copyright 2016 Xamarin Inc. All right reserved.

using System;
using Mono.Cecil;
using Mono.Linker;
using Mono.Tuner;

namespace Xamarin.Linker.Steps {

	// The value of some metadata is very low, some is useful only when debugging
	// and other can be replaced with different information
	public class MetadataReducerSubStep : ExceptionalSubStep {

		protected override string Name { get; } = "Metadata Reducer";
		protected override int ErrorCode { get; } = 2070;

		public override SubStepTargets Targets {
			get {
				return SubStepTargets.Type | SubStepTargets.Method;
			}
		}

		bool ReflectedParameterNames { get; set; }

		public override void Initialize (LinkContext context)
		{
			base.Initialize (context);
			// is some user code depending (or not) on reflection to get method's parameters names ?
			// note: member references will still exists (even if not marked) until the assembly is saved
			// so we need this to be pre-computed at the marking stage
			ReflectedParameterNames = Annotations.GetCustomAnnotations ("ParameterInfo").Count > 0;
		}

		public override bool IsActiveFor (AssemblyDefinition assembly)
		{
			return Annotations.GetAction (assembly) == AssemblyAction.Link;
		}

		protected override void Process (MethodDefinition method)
		{
			if (method.IsPInvokeImpl) {
				// note: ObjCRuntime.Messaging - but also 3rd parties bindings (normally ApiDefinitions.Messaging)
				// to avoid possible out-of-assembly usage (eg. classic) we only apply the optimization on non-public methods
				var dt = method.DeclaringType;
				if (!method.IsPublic || !dt.IsPublic) {
					// Avoid hundreds of different strings for msgSend[Super] overloads
					// C# requires different names (for methods with different return values)
					// but it's not an issue in IL
					if ((dt.Name == "Messaging") && (method.PInvokeInfo.Module.Name == "/usr/lib/libobjc.dylib"))
						method.Name = method.PInvokeInfo.EntryPoint;
				}
			}

			// Parameter names are rather safe to remove, i.e. unlikely to be used in reflection
			// but their removal (from the string table) can save a lot of space in the .app
			// https://trello.com/c/zYFcYmuM/34-linker-support-for-removing-parameter-names

			// Still unlikely != impossible, e.g. some user code can reflect them
			// https://bugzilla.xamarin.com/show_bug.cgi?id=40458
			if (ReflectedParameterNames)
				return;

			// The limit of this optimization is that parameter validation (e.g. null checks)
			// often use the parameter name anyway (no saving in those cases). However this
			// is something that can be solved elsewhere...
			if (!method.HasParameters)
				return;

			foreach (var p in method.Parameters)
				p.Name = null;
		}
	}
}
