using System.Linq;
using System.Text;

using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Linker;
using Mono.Tuner;

using Xamarin.Bundler;
using Xamarin.Utils;

#nullable enable

namespace Xamarin.Linker.Steps;

// See docs/code/class-handles.md for an overview of class handle handling.

// Find all the references to Objective-C classes for each assembly.
// * If an assembly is trimmed, we replace calls to (inline) Class.GetHandle with a P/Invoke that fetches the class in question.
// * If an assembly is not trimmed, we do not inline Class.GetHandle, but we keep a list of all the Objective-C classes that are used, so that we can tell the native linker about them so they're not linked away by the native linker.
//
// The sticky problem is that we can't inspect the managed output from NativeAOT, which means
// we can't list the Objective-C classes NativeAOT-compiled assemblies using. To get around this
// problem, we convert calls to Class.GetHandle to a P/Invoke which fetches the native Objective-C
// class handle directly - because we _can_ list the P/Invoke calls from NativeAOT-compiled code.
//
// For non-trimmed assemblies, we don't need to do this, because we know nothing from the assembly
// will be trimmed away. This has the added advantage of being Hot Reload compatible, because we
// can't modify assemblies when we're doing Hot Reload.

public class InlineClassGetHandleStep : AssemblyModifierStep {

	protected override string Name { get; } = "Inline Class GetHandle";
	protected override int ErrorCode { get; } = 2262;

	bool strictMode;
	bool? inlining_enabled;

	Dictionary<string, Registrar.Registrar.ObjCType> objectiveCTypeMap = new ();

	public const string PInvokePrefix = "xamarin_Class_GetHandle_";
	public const string PInvokeSuffix = "_Native";

	protected override bool ConditionToProcess ()
	{
		return Configuration.InlineClassGetHandle != InlineClassGetHandleMode.Disabled;
	}

	protected override void TryProcess ()
	{
		strictMode = Configuration.InlineClassGetHandle == InlineClassGetHandleMode.Strict;

		if (strictMode && Configuration.Application.Registrar == Bundler.RegistrarMode.Dynamic) {
			Report (ErrorHelper.CreateError (Configuration.Application, 2262, null, Errors.MX2262));
		}

		// The native static registrar otherwise registers types only during postprocessing.
		if (App.Registrar == RegistrarMode.Static)
			DerivedLinkContext.StaticRegistrar.Register (Configuration.GetNonDeletedAssemblies (this));

		objectiveCTypeMap = DerivedLinkContext.StaticRegistrar.Types.ToDictionary (v => v.Value.ExportedName, v => v.Value);

		if (!string.IsNullOrEmpty (Configuration.TypeMapFilePath)) {
			var sb = new StringBuilder ();
			foreach (var info in objectiveCTypeMap.Values.OrderBy (v => v.ExportedName)) {
				var td = info.Type.Resolve ();
				if (td is null)
					continue;
				var introduced = DerivedLinkContext.StaticRegistrar.GetSdkIntroducedVersion (td, out _);
				Frameworks.TryGetFramework (App, td, out string? framework);
				sb.AppendLine ($"Class={info.ExportedName}|Framework={framework}|Introduced={introduced}|IsWrapper={info.IsWrapper}|IsStubClass={info.IsStubClass}");
			}
			Driver.WriteIfDifferent (App, Configuration.TypeMapFilePath, sb.ToString ());
		}

		base.TryProcess ();
	}

	protected override bool IsActiveFor (AssemblyDefinition assembly)
	{
		if (!Configuration.Profile.IsOrReferencesProductAssembly (assembly))
			return false;

		// we have to process both trimmed and non-trimmed assemblies.

		return true;
	}

	protected override bool ModifyAssembly (AssemblyDefinition assembly)
	{
		inlining_enabled = Annotations.GetAction (assembly) == AssemblyAction.Link;
		var modified = base.ModifyAssembly (assembly);
		inlining_enabled = null;
		return modified;
	}

	protected override bool ProcessType (TypeDefinition type)
	{
		var modified = false;

		if (inlining_enabled == true) {
			modified |= ProcessMethods (type);
		} else {
			if (ListExportedSymbols.TryGetRequiredObjectiveCType (DerivedLinkContext, type, out var exportedName)) {
				DerivedLinkContext.RequiredSymbols.AddObjectiveCClass (exportedName).AddMember (type);
			}
		}
		return modified;
	}

	MethodDefinition GetOrCreatePInvokeMethod (MethodDefinition callingMethod, string objectiveCClassName)
	{
		// [DllImport ("__Internal")]
		// static extern IntPtr xamarin_Class_GetClassHandle_{objectiveCClassName}_Native ();

		return abr.CreateInternalPInvoke (callingMethod.Module, "ObjCRuntime", "Class", $"{PInvokePrefix}{objectiveCClassName}{PInvokeSuffix}", out _);
	}

	protected override bool ProcessMethod (MethodDefinition method)
	{
		var modified = false;

		if (!method.HasBody)
			return modified;

		if (method.DeclaringType.Name == "Class" && method.DeclaringType.Namespace == "ObjCRuntime")
			return modified; // don't process the Class methods themselves

		bool isOurOwnCode ()
		{
			// Don't show warnings for a few places in our own code where we call Class.GetHandle in un-inlinable ways.
			switch (method.DeclaringType.Namespace) {
			case "Registrar":
				switch (method.DeclaringType.Name) {
				case "DynamicRegistrar":
					switch (method.Name) {
					case "OnReloadType":
					case "OnRegisterType":
						return true;
					}
					break;
				}
				break;
			}
			return false;
		}

		foreach (var instr in method.Body.Instructions) {
			if (instr.Operand is not MethodReference mr)
				continue;
			if (mr.DeclaringType.Name != "Class" || mr.DeclaringType.Namespace != "ObjCRuntime")
				continue;
			if (mr.Name != "GetHandle" && mr.Name != "GetHandleIntrinsic")
				continue;
			if (mr.Parameters.Count != 1)
				continue;
			if (!mr.Parameters [0].ParameterType.Is ("System", "String"))
				continue;
			if (!mr.ReturnType.Is ("ObjCRuntime", "NativeHandle"))
				continue;

			var ldstr = instr.Previous;
			if (ldstr.OpCode != OpCodes.Ldstr) {
				if (!isOurOwnCode ())
					App.Log (3, "Unknown or unsupported pattern in call to Class.GetHandle in '{0}': {1}. The call will not be inlined.", FormatMethod (method), ldstr);
				continue;
			}
			if (ldstr.Operand is not string objectiveCClassName) {
				if (!isOurOwnCode ())
					App.Log (3, "Unknown or unsupported pattern in call to Class.GetHandle in '{0}': {1}. The call will not be inlined.", FormatMethod (method), ldstr.Operand);
				continue;
			}

			if (!objectiveCTypeMap.TryGetValue (objectiveCClassName, out var objCType)) {
				App.Log (3, "Could not find a managed type for the Objective-C type '{1}' in the call to Class.GetHandle in '{0}', assuming the Objective-C type is available in the simulator.", FormatMethod (method), objectiveCClassName);
			}

			if (!strictMode) {
				if (objCType is not null && objCType.Type == method.DeclaringType) {
					// This is a call to Class.GetHandle for the same class that it's being called from, this is OK.
				} else if (ListExportedSymbols.TryGetRequiredObjectiveCType (DerivedLinkContext, method.DeclaringType, out var exportedName)) {
					if (exportedName != objectiveCClassName) {
						App.Log (3, "The call to Class.GetHandle in '{0}' is trying to get the handle for the Objective-C class '{1}', but the declaring type's exported name is '{2}', not '{1}'. Since we're in compat mode, we're assuming the class should not be preserved.", FormatMethod (method), objectiveCClassName, exportedName);
						continue;
					}
				} else {
					if (App.StaticRegistrar.GetCategoryAttribute (method.DeclaringType) is not null)
						App.Log (3, "The call to Class.GetHandle in '{0}' is trying to get the handle for the Objective-C class '{1}', but we couldn't determine whether this class should be statically preserved or not. Since we're in compat mode, we're assuming the class should not be statically preserved.", FormatMethod (method), objectiveCClassName);
					continue;
				}
			}

			// Check if the Objective-C class is listed as a ReferenceNativeSymbol with Ignore mode, and if so, don't inline the call to Class.GetHandle (because the native symbol won't be available at link time)
			var existingSymbol = DerivedLinkContext.RequiredSymbols.Find (Symbol.ObjectiveCPrefix + objectiveCClassName);
			if (existingSymbol is not null && existingSymbol.Type == SymbolType.ObjectiveCClass && existingSymbol.Mode == SymbolMode.Ignore) {
				App.Log (3, "Not inlining the call to Class.GetHandle (\"{0}\") in method {1} because the class is listed as a ReferenceNativeSymbol with Ignore mode.", objectiveCClassName, FormatMethod (method));
				continue;
			}

			if (objCType is not null) {
				if (objCType.IsFakeProtocol) {
					// This is a class with a [Protocol] attribute (typically a binding that declares a
					// [BaseType (typeof (NSObject))] type for something that is a protocol - and not a class -
					// natively). The static registrar doesn't register these classes, so there's no native
					// Objective-C class to reference, which means inlining the call to Class.GetHandle would
					// turn into a hard link error ('Undefined symbols: _OBJC_CLASS_$_X'). Don't inline it, so
					// that the call falls back to a runtime lookup (which returns a zero handle for a missing class).
					App.Log (3, "Not inlining the call to Class.GetHandle (\"{0}\") in method {1} because the class has a [Protocol] attribute, so there's no native Objective-C class to reference.", objectiveCClassName, FormatMethod (method));
					continue;
				}

				if (DerivedLinkContext.App.IsSimulatorBuild) {
					if (DerivedLinkContext.HasAvailabilityAttributesShowingUnavailableInSimulator (objCType.Type.Resolve (), method)) {
						App.Log (3, "Not inlining the call to Class.GetHandle (\"{0}\") in method {1} because the type is marked with an attribute indicating it's not available in the simulator.", objectiveCClassName, FormatMethod (method));
						continue;
					}
				}

				if (IsUnsupported (objCType.Type.Resolve ())) {
					App.Log (3, "Not inlining the call to Class.GetHandle (\"{0}\") in method {1} because the type is marked with an [UnsupportedOSPlatform] attribute.", objectiveCClassName, FormatMethod (method));
					continue;
				}

				if (objCType.Methods is null && DerivedLinkContext.StaticRegistrar.IsPlatformType (objCType.Type) && !objCType.IsProtocol && !objCType.IsCategory && objCType.IsModel) {
					// The static registrar skips generating code for this type, so we shouldn't inline calls to Class.GetHandle for it, because the P/Invoke we generate won't be able to find the native symbol for it.
					continue;
				}

				if (Frameworks.TryGetFramework (App, objCType.Type.Resolve (), out Framework? framework) && framework.IsFrameworkUnavailable (App)) {
					App.Log (3, "Not inlining the call to Class.GetHandle (\"{0}\") in method {1} because the framework {2} is unavailable.", objectiveCClassName, FormatMethod (method), framework.Name);
					continue;
				}

				if (objCType.Type.Is ("UIKit", "UITitlebar")) {
					// UITitlebar is a weird special case, the class exists in the headers, and it's documented online, but it's not possible to link with it (not even in an Xcode project, it's not in any .tbd files).
					continue;
				}

				if (objCType.Type.Namespace == "BrowserEngineKit" || objCType.Type.Namespace == "BrowserEngineCore") {
					// Most apps do not use BrowserEngineKit, and linking with it when an app is not supposed to will prevent it from getting approved in the App Store.
					// So we treat these frameworks specially, where we don't link with these two frameworks by default, *even if they're detected as used*,
					// which means we shouldn't inline Class.GetHandle calls for any classes in these frameworks, since native linking will fail.
					continue;
				}
			}

			ldstr.OpCode = OpCodes.Call;
			ldstr.Operand = GetOrCreatePInvokeMethod (method, objectiveCClassName);

			instr.OpCode = OpCodes.Call;
			instr.Operand = abr.NativeObject_op_Implicit_NativeHandle;

			modified = true;
		}

		return modified;
	}

	bool IsUnsupported (TypeDefinition? type)
	{
		if (type is null)
			return false;

		if (!type.HasCustomAttributes)
			return false;

		foreach (var ca in type.CustomAttributes) {
			if (!ca.AttributeType.Is ("System.Runtime.Versioning", "UnsupportedOSPlatformAttribute"))
				continue;

			if (!DerivedLinkContext.StaticRegistrar.GetDotNetAvailabilityAttribute (ca, App.Platform, out var sdkVersion, out _))
				continue;

			if (sdkVersion is null)
				return true; // if there's no version, then it's always unavailable

			return sdkVersion <= App.SdkVersion;
		}

		return false;
	}

	static string FormatMethod (MethodReference method)
	{
		var rv = method.FullName;
		var idx = rv.IndexOf (' ');
		if (idx > 0)
			rv = rv.Substring (idx + 1);
		return rv;
	}
}
