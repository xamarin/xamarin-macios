// Copyright 2011-2013 Xamarin Inc., All rights reserved.
// adapted from xtouch/tools/mtouch/Touch.Tuner/ManualMarkStep.cs

using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Linker;
using Mono.Tuner;

using Xamarin.Bundler;
using Xamarin.Linker;
using Xamarin.Linker.Steps;

namespace MonoTouch.Tuner {

	// XML definition files have their limits, i.e. they are good to keep stuff around unconditionnally
	// e.g. we do not want to force all/most Socket code around (non-network apps) because some types have unmanaged representation
	//
	// Generated backend fields inside monotouch.dll are also removed if only used (i.e. set to null)
	// inside the Dispose method
	public class MonoTouchMarkStep : CoreMarkStep {
		List<Exception> list = new List<Exception> ();
		bool serialization_required = false;
		List<MethodDefinition> pending_serialization_constructors = new List<MethodDefinition> ();
		bool parameter_info;

		public MonoTouchMarkStep ()
		{
		}

		public override void Process (LinkContext context)
		{
			DebugBuild = context.GetParameter ("debug-build") == "True";

			base.Process (context);

			if (list.Count > 0)
				throw new AggregateException (list);

			if (!serialization_required) {
#if DEBUG
				Console.WriteLine ("{0} serialization constructors were removed because no SerializationInfo is created", pending_serialization_constructors.Count);
#endif
				pending_serialization_constructors.Clear ();
			}
		}

		protected override void MarkSerializable (TypeDefinition type)
		{
			if (!type.HasMethods)
				return;

			foreach (MethodDefinition method in type.Methods) {
				// skip non-constructors and static methods
				if (!method.IsConstructor || method.IsStatic)
					continue;

				// mark the default constructor for every serializable type
				if (!method.HasParameters)
					MarkMethod (method);

				// look for the special serialization constructor
				if (!method.HasParameters)
					continue;
				var parameters = method.Parameters;
				if (parameters.Count != 2)
					continue;
				if (parameters [0].ParameterType.Name != "SerializationInfo" || 
				    parameters [1].ParameterType.Name != "StreamingContext")
					continue;

				if (serialization_required) {
					MarkMethod (method);
				} else {
					// keep track of the serialization constructors until we know if we'll need them
					pending_serialization_constructors.Add (method);
				}
			}
		}

		protected override TypeDefinition MarkType (TypeReference reference)
		{
			TypeDefinition type = base.MarkType (reference);
			if (type == null)
				return null;

			switch (type.Module.Assembly.Name.Name) {
			case "System.Core":
				ProcessSystemCore (type);
				break;
			}
			return type;
		}

		protected override MethodDefinition MarkMethod (MethodReference reference)
		{
			var method = base.MarkMethod (reference);
			if (method == null)
				return null;

			// we need to track who's calling ParameterInfo.Name property getter, if it comes from
			// user code then it's not possible to remove the parameters from the assemblies metadata
			if (!parameter_info && (method.Name == "get_Name") && method.DeclaringType.Is ("System.Reflection", "ParameterInfo")) {
				if (current_method == null) {
					// This can happen if ParameterInfo.get_Name is preserved in an xml file
					Annotations.GetCustomAnnotations ("ParameterInfo").Add (method, null);
				} else {
					var a = current_method.DeclaringType.Module.Assembly;
					if (!Profile.IsSdkAssembly (a) && !Profile.IsProductAssembly (a)) {
						// see MetadataReducerSubStep for the consumer part of the data
						Annotations.GetCustomAnnotations ("ParameterInfo").Add (method, current_method);
						parameter_info = true;
					}
				}
			}

			// if the application creates instances of SerializationInfo then we assume serialization is likely
			// and include all the .ctor(SerializationInfo,StreamingContext) inside the application
			if (!serialization_required && method.IsConstructor && !method.IsStatic && 
			    method.DeclaringType.Is ("System.Runtime.Serialization", "SerializationInfo")) {
				serialization_required = true;
				// stop procrastinaing and mark them all
				foreach (var m in pending_serialization_constructors)
					MarkMethod (m);
				pending_serialization_constructors.Clear ();
			}

			// we want to avoid separate mscorlib.dll for 32/64 bits so the arch specific code for n[u]int and nfloat must be preserved in both cases
			// a single, slightly larger, assenmbly is much smaller thn two (slightly smaller) ones
			if (LinkContext.App.IsDualBuild) {
				switch (method.Name) {
				case "TryParse":
					switch (method.DeclaringType.Name) {
					case "nfloat":
						MarkNamedMethod (GetType ("mscorlib", "System.Double"), "TryParse");
						MarkNamedMethod (GetType ("mscorlib", "System.Single"), "TryParse");
						break;
					}
					break;
				case "Create":
					// ARCH_32 optimization
					if (!method.DeclaringType.Is (Namespaces.Foundation, "NSIndexPath"))
						break;
					MarkNamedMethod (GetType ("mscorlib", "System.Array"), "ConvertAll");
					break;
				}
			}

			return method;
		}

		MethodReference current_method;

		protected override void MarkMethodBody (MethodBody body)
		{
			// we need some context about the caller of methods and that's kind of
			// unavailable, by default, so we must keep that state around
			current_method = body.Method;
			base.MarkMethodBody (body);
			current_method = null;
		}

		// AOT (not MOBILE) specific
		void ProcessSystemCore (TypeDefinition type)
		{
			switch (type.Namespace) {
			case "System.Linq.Expressions":
				switch (type.Name) {
				case "LambdaExpression":
					var expr_t = type.Module.GetType ("System.Linq.Expressions.ExpressionCreator`1");
					if (expr_t != null) {
						MarkNamedMethod (expr_t, "CreateExpressionFunc");
					}
					break;
				}
				break;
			case "System.Linq.Expressions.Interpreter":
				switch (type.Name) {
				case "LightDelegateCreator":
					// TODO: Should be more selective
					MarkMethods (type.Module.GetType ("System.Linq.Expressions.Interpreter.LightLambda"));
					break;
				}
				break;
			case "System.Runtime.CompilerServices":
				switch (type.Name) {
				case "CallSite":
					var cs_ops = type.Module.GetType ("System.Runtime.CompilerServices.CallSiteOps");
					if (cs_ops != null)
						MarkMethods (ResolveTypeDefinition (cs_ops));

					break;
				case "CallSite`1":
					MarkNamedMethod (type, "get_Update");

					var ud = type.Module.GetType ("System.Dynamic.UpdateDelegates");
					if (ud != null)
						MarkMethods (ResolveTypeDefinition (ud));

					break;
				}
				break;
			}
		}
	}
}
