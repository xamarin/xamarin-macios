using System;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Mono.Cecil;

using ObjCRuntime;

namespace Xamarin.Utils {
	public static partial class CecilExtensions {

		// note: direct check, no inheritance
		public static bool Is (this TypeReference type, string @namespace, string name)
		{
			return (type is not null) && (type.Name == name) && (type.Namespace == @namespace);
		}


		public static string AsOSPlatformAttributeString (this CustomAttribute ca)
		{
			if (ca.AttributeType.Namespace != "System.Runtime.Versioning")
				throw new ArgumentOutOfRangeException ($"Not an OSPlatformAttribute: {ca.AttributeType.FullName}");

			switch (ca.AttributeType.Name) {
			case "UnsupportedOSPlatformAttribute":
				return $"[UnsupportedOSPlatform (\"{(string) ca.ConstructorArguments [0].Value}\")]";
			case "SupportedOSPlatformAttribute":
				return $"[SupportedOSPlatform (\"{(string) ca.ConstructorArguments [0].Value}\")]";
			case "ObsoletedOSPlatformAttribute":
				return $"[ObsoletedOSPlatformAttribute (\"{(string) ca.ConstructorArguments [0].Value}\")]";
			default:
				throw new NotImplementedException (ca.AttributeType.FullName);
			}
		}

		public static string AsString (this IMemberDefinition member)
		{
			if (member is MethodDefinition md) {
				var rv = new StringBuilder ();
				rv.Append (md.Name);
				rv.Append ('(');
				if (md.HasParameters) {
					for (var i = 0; i < md.Parameters.Count; i++) {
						if (i > 0)
							rv.Append (", ");
						var p = md.Parameters [0];
						if (p.IsOut)
							rv.Append ("out ");
						rv.Append (p.ParameterType.FullName);
					}
				}
				rv.Append (')');
				return rv.ToString ();
			}

			if (member is FieldDefinition fd)
				return fd.Name;

			if (member is PropertyDefinition pd)
				return pd.Name;

			return member.ToString ()!;
		}

		public static string AsFullName (this ICustomAttributeProvider member)
		{
			if (member is MethodDefinition md) {
				var rv = new StringBuilder ();
				rv.Append (AsFullName (md.DeclaringType));
				rv.Append ('.');
				rv.Append (md.Name);
				if (md.HasGenericParameters) {
					rv.Append ("`");
					rv.Append (md.GenericParameters.Count.ToString ());
				}
				rv.Append ('(');
				if (md.HasParameters) {
					for (var i = 0; i < md.Parameters.Count; i++) {
						if (i > 0)
							rv.Append (", ");
						var p = md.Parameters [i];
						if (p.IsOut)
							rv.Append ("out ");
						rv.Append (p.ParameterType.FullName);
					}
				}
				rv.Append (')');

				if (md.IsSpecialName && (md.Name == "op_Explicit" || md.Name == "op_Implicit")) {
					rv.Append (" => ");
					rv.Append (md.ReturnType.FullName);
				}

				return rv.ToString ();
			}

			if (member is FieldDefinition fd)
				return fd.FullName;

			if (member is PropertyDefinition pd)
				return pd.FullName;

			if (member is EventDefinition ed)
				return ed.FullName + " (event)";

			if (member is IMemberDefinition imd)
				return imd.FullName;

			return member.ToString ()!;
		}

		public static string GetOSPlatformAttributePlatformName (this CustomAttribute ca)
		{
			return (string) ca.ConstructorArguments [0].Value;
		}

		public static bool IsObsolete (this ICustomAttributeProvider? provider)
		{
			if (provider?.HasCustomAttributes != true)
				return false;

			foreach (var attrib in provider.CustomAttributes)
				if (IsObsoleteAttribute (attrib))
					return true;

			return false;
		}

		public static bool IsObsoleteAttribute (this CustomAttribute attribute)
		{
			return attribute.AttributeType.Is ("System", "ObsoleteAttribute");
		}


		public static bool HasEditorBrowseableNeverAttribute (this ICustomAttributeProvider? provider)
		{
			if (provider?.HasCustomAttributes != true)
				return false;

			foreach (var attr in provider.CustomAttributes) {
				if (!attr.AttributeType.Is ("System.ComponentModel", "EditorBrowsableAttribute"))
					continue;
				var state = (EditorBrowsableState) attr.ConstructorArguments [0].Value;
				if (state == EditorBrowsableState.Never)
					return true;
			}

			return false;
		}

		public static bool HasBindingImplAttribute (this ICustomAttributeProvider? provider, out BindingImplOptions options)
		{
			options = BindingImplOptions.None;
			if (provider is null)
				return false;

			if (provider.HasCustomAttributes) {
				foreach (var attr in provider.CustomAttributes) {
					if (!attr.AttributeType.Is ("ObjCRuntime", "BindingImplAttribute"))
						continue;
					options = (BindingImplOptions) attr.ConstructorArguments [0].Value;
					return true;
				}
			}

			var property = FindProperty (provider as MethodDefinition);
			if (property is not null)
				return HasBindingImplAttribute (property, out options);

			return false;
		}

		public static PropertyDefinition? FindProperty (this MethodDefinition? accessor)
		{
			if (accessor is null)
				return null;

			if (!accessor.IsSpecialName)
				return null;

			if (!accessor.DeclaringType.HasProperties)
				return null;

			if (!accessor.Name.StartsWith ("get_", StringComparison.Ordinal) && !accessor.Name.StartsWith ("set_", StringComparison.Ordinal))
				return null;

			var propertyName = accessor.Name.Substring (4);
			var properties = accessor.DeclaringType.Properties.Where (v => v.Name == propertyName);
			foreach (var property in properties) {
				if (property.GetMethod == accessor || property.SetMethod == accessor)
					return property;
			}

			return null;
		}
	}
}

namespace ObjCRuntime {
	[Flags]
	public enum BindingImplOptions {
		None = 0,
		GeneratedCode = 1,
		Optimizable = 2,
	}
}
