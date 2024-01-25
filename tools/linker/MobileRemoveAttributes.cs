using System;

using Mono.Cecil;
using Mono.Tuner;

namespace Xamarin.Linker {

	public class MobileRemoveAttributes : RemoveAttributesBase {

		protected virtual bool DebugBuild {
			get { return context.LinkSymbols; }
		}

		protected override bool IsRemovedAttribute (CustomAttribute attribute)
		{
			// note: this also avoid calling FullName (which allocates a string)
			var attr_type = attribute.Constructor.DeclaringType;
			switch (attr_type.Name) {
			case "ObsoleteAttribute":
			// System.Mono*Attribute from mono/mcs/build/common/MonoTODOAttribute.cs
			case "MonoLimitationAttribute":
			case "MonoNotSupportedAttribute":
			case "MonoTODOAttribute":
				return attr_type.Namespace == "System";
			// remove debugging-related attributes if we're not linking symbols (i.e. we're building release builds)
			case "DebuggableAttribute":
			case "DebuggerBrowsableAttribute":
			case "DebuggerDisplayAttribute":
			case "DebuggerHiddenAttribute":
			case "DebuggerNonUserCodeAttribute":
			case "DebuggerStepperBoundaryAttribute":
			case "DebuggerStepThroughAttribute":
			case "DebuggerTypeProxyAttribute":
			case "DebuggerVisualizerAttribute":
				return !DebugBuild && attr_type.Namespace == "System.Diagnostics";
			// compiler nullability attributes are not used at runtime so they can be removed by the linker
			case "NullableAttribute":
			case "NullableContextAttribute":
			case "NullablePublicOnlyAttribute":
				return attr_type.Namespace == "System.Runtime.CompilerServices";
			// _manual_ nullability attributes are not used at runtime so they can be removed by the linker
			case "AllowNullAttribute":
			case "DisallowNullAttribute":
			case "DoesNotReturnAttribute":
			case "DoesNotReturnIfAttribute":
			case "ExcludeFromCodeCoverageAttribute":
			case "MaybeNullAttribute":
			case "MaybeNullWhenAttribute":
			case "NotNullAttribute":
			case "NotNullIfNotNullAttribute":
			case "NotNullWhenAttribute":
				return attr_type.Namespace == "System.Diagnostics.CodeAnalysis";
			// decorate the internalattributes (like nullability) that Roslyn inject into assemblies
			case "EmbeddedAttribute":
				return attr_type.Namespace == "Microsoft.CodeAnalysis";
			default:
				return false;
			}
		}
	}
}
