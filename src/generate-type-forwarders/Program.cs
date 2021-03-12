using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Mono.Cecil;

/*
 * Given two assemblies, this tool will generate a third assembly, where:
 * * Any type that exists in both the first and the second assembly is a typeforwarder to the second assembly
 * * Any type that does not exist in the first assembly, is re-generated with implementations that throw PlatformNotSupportedExceptions.
 * 
 * The re-generation is not 100% identical to the input, because the third assembly is only
 * to be used as an implementation assembly (it will never be used at compile time), which means
 * that minor differences such as incorrect visibility are insignificant.
 * 
 */

namespace GenerateTypeForwarders {
	class MainClass {
		public static int Main (string [] args)
		{
			var forwardFrom = args [0];
			var forwardTo = args [1];
			var output = args [2];
			return Fix (forwardFrom, forwardTo, output);
		}

		static void EmitTypeName (StringBuilder sb, TypeReference type)
		{
			if (type.FullName == "System.Void") {
				sb.Append ("void");
				return;
			}

			if (type.IsNested) {
				EmitTypeName (sb, type.DeclaringType);
				sb.Append ('.');
			} else {
				if (!string.IsNullOrEmpty (type.Namespace)) {
					sb.Append (type.Namespace);
					sb.Append ('.');
				}
			}

			var arrayType = type as ArrayType;
			if (arrayType != null) {
				sb.Append (arrayType.ElementType.Name);
				sb.Append ('[');
				sb.Append (new string (',', arrayType.Rank - 1));
				sb.Append (']');
				return;
			}

			var genericType = type as GenericInstanceType;
			if (genericType != null) {
				sb.Append (genericType.ElementType.Name.Substring (0, genericType.ElementType.Name.IndexOf ('`')));
				sb.Append ('<');
				for (var i = 0; i < genericType.GenericArguments.Count; i++) {
					var ga = genericType.GenericArguments [i];
					if (i > 0)
						sb.Append (", ");
					EmitTypeName (sb, ga);
				}
				sb.Append ('>');
			} else {
				sb.Append (type.Name);
			}
		}

		static void EmitParameters (StringBuilder sb, IList<ParameterDefinition> parameters)
		{
			for (var i = 0; i < parameters.Count; i++) {
				var param = parameters [i];
				var paramType = param.ParameterType;

				if (i > 0)
					sb.Append (", ");

				if (param.IsOut) {
					sb.Append ("out ");
					if (paramType.IsByReference)
						paramType = paramType.GetElementType ();
				} else if (paramType.IsByReference) {
					sb.Append ("ref ");
					paramType = paramType.GetElementType ();
				}

				EmitTypeName (sb, paramType);
				sb.Append (" @");
				sb.Append (param.Name);
			}
		}

		static void EmitPNSE (StringBuilder sb, MethodDefinition method, int indent, bool nsobject)
		{
			if (method.IsPrivate)
				return;

			if (method.IsConstructor) {
				// we can have an internal ctor using internal types that are not generated leading to uncompilable code
				// e.g. `internal DisplayedPropertiesCollection (ABFunc<NSNumber[]> g, Action<NSNumber[]> s)`
				if (method.IsAssembly && method.HasParameters) {
					foreach (var p in method.Parameters) {
						// check is the type is public - if not it won't be generated
						if (p.ParameterType.Resolve ().IsNotPublic) {
							// but we can't skip the generation as without any .ctor the compiler will add a default, public one
							// so we create a custom signature to ensure an (internal) ctor exists
							p.ParameterType = method.DeclaringType;
						}
					}
				}
			} else if (method.IsSpecialName)
				return;

			if (method.Name == "Finalize")
				return;

			var strIndent = new string ('\t', indent);
			sb.Append (strIndent);

			if (method.IsPublic)
				sb.Append ("public ");
			else if (method.IsFamilyOrAssembly)
				sb.Append ("protected internal ");
			else if (method.IsFamilyAndAssembly)
				sb.Append ("private protected ");
			else if (method.IsFamily)
				sb.Append ("protected ");
			else if (method.IsAssembly)
				sb.Append ("internal ");

			sb.Append ("unsafe ");

			if (method.IsStatic) {
				sb.Append ("static ");
				if (AnyMethodWithSignatureInHierarchy (method.DeclaringType.BaseType.Resolve (), method, out var matchingMethod)) {
					sb.Append ("new ");
				}
			}
			if (method.IsAbstract && !method.DeclaringType.IsInterface)
				sb.Append ("abstract ");
			if (method.IsVirtual && !method.DeclaringType.IsInterface && !method.IsFinal && !method.IsAbstract) {
				if (AnyMethodWithSignatureInHierarchy (method.DeclaringType.BaseType?.Resolve (), method, out var matchingMethod) && matchingMethod.IsVirtual) {
					sb.Append ("override ");
				} else {
					sb.Append ("virtual ");
				}
			}
			if (method.IsFinal && !method.IsVirtual)
				sb.Append ("sealed ");
			if (method.IsConstructor) {
				sb.Append (method.DeclaringType.Name);
			} else {
				if (method.ReturnType.FullName == "System.Void") {
					sb.Append ("void ");
				} else {
					EmitTypeName (sb, method.ReturnType);
					sb.Append (' ');
				}
				sb.Append (method.Name);
			}

			StringBuilder constraints = null;
			if (method.HasGenericParameters) {
				PrintGenericParameters (sb, method.GenericParameters, true);
				for (var i = 0; i < method.GenericParameters.Count; i++) {
					var gp = method.GenericParameters [i];
					if (gp.HasConstraints || gp.HasDefaultConstructorConstraint || gp.HasNotNullableValueTypeConstraint || gp.HasReferenceTypeConstraint) {
						if (constraints == null) {
							constraints = new StringBuilder (" where ");
						} else {
							constraints.Append (", ");
						}
						constraints.Append (gp.Name);
						constraints.Append (" : ");

						var list = new List<string> ();

						if (gp.HasReferenceTypeConstraint)
							list.Add ("class");

						if (gp.HasNotNullableValueTypeConstraint) {
							list.Add ("struct");
						}

						foreach (var c in gp.Constraints) {
							var cstr = new StringBuilder ();
							EmitTypeName (cstr, c.ConstraintType);
							list.Add (cstr.ToString ());
						}

						if (gp.HasDefaultConstructorConstraint && !gp.HasNotNullableValueTypeConstraint)
							list.Add ("new ()");

						if (gp.HasNotNullableValueTypeConstraint)
							list.Remove ("System.ValueType");

						constraints.Append (string.Join (", ", list));
					}
				}
			}

			sb.Append (" (");
			EmitParameters (sb, method.Parameters);
			sb.Append (")");
			if (constraints != null)
				sb.Append (constraints);
			if (method.IsAbstract) {
				sb.AppendLine (";");
			} else {
				sb.AppendLine ();
				// use .ctor(IntPtr) for NSObject subclasses - it's simpler and won't refer to multiple .ctor needlessly
				if (nsobject && method.IsConstructor && !method.IsStatic)
					sb.Append ('\t', indent + 1).AppendLine (" : base (System.IntPtr.Zero)");
				// if there is a base constructor with the same signature, call it
				else if (method.IsConstructor && method.HasParameters) {
					var baseConstructors = method.DeclaringType.BaseType?.Resolve ()?.Methods?.Where (v => v.IsConstructor && v.HasParameters);
					if (StartParameterTypeMatch (baseConstructors, method.Parameters, out var baseCtor)) {
						sb.Append ($"{strIndent}\t: base (");
						for (var i = 0; i < baseCtor.Parameters.Count; i++) {
							if (i > 0)
								sb.Append (", ");
							sb.Append ($"@{method.Parameters [i].Name}"); // need the original name
						}
						sb.AppendLine (")");
					}
				}
				sb.AppendLine ($"{strIndent}{{");
				sb.AppendLine ($"{strIndent}\tthrow new global::System.PlatformNotSupportedException (global::Constants.UnavailableOnMacCatalyst);");
				sb.AppendLine ($"{strIndent}}}");
			}
		}

		static bool AnyMethodWithSignatureInHierarchy (TypeDefinition type, MethodDefinition method, out MethodDefinition matchingMethod)
		{
			matchingMethod = null;

			while (type != null) {
				if (AllParameterTypeMatch (type.Methods, method, out matchingMethod))
					return true;

				type = type.BaseType?.Resolve ();
			}

			return false;
		}

		static bool AllParameterTypeMatch (IList<MethodDefinition> methods, MethodDefinition method, out MethodDefinition matchingMethod)
		{
			matchingMethod = null;

			return AllParameterTypeMatch (methods.Where (v =>
				v.Name == method.Name &&
				v.IsStatic == method.IsStatic &&
				v.Parameters.Count == method.Parameters.Count &&
				v.GenericParameters.Count == method.GenericParameters.Count
			), method.Parameters, out matchingMethod);
		}

		static bool AllParameterTypeMatch (IEnumerable<MethodDefinition> methods, IList<ParameterDefinition> parameters, out MethodDefinition matchingMethod)
		{
			matchingMethod = null;

			foreach (var method in methods) {
				var parameterCount = method.HasParameters ? method.Parameters.Count : 0;
				if (parameterCount != parameters.Count)
					continue;
				if (parameters.Count == 0) {
					matchingMethod = method;
					return true;
				}
				bool match = true;
				for (var i  = 0; i < parameters.Count; i++) {
					var a = method.Parameters [i];
					var b = parameters [i];
					match &= a.ParameterType.FullName == b.ParameterType.FullName;
				}
				if (match) {
					matchingMethod = method;
					return true;
				}
			}
			return false;
		}

		static bool StartParameterTypeMatch (IEnumerable<MethodDefinition> methods, IList<ParameterDefinition> parameters, out MethodDefinition matchingMethod)
		{
			matchingMethod = null;

			foreach (var method in methods) {
				if (parameters.Count == 0) {
					matchingMethod = method;
					return true;
				}
				var parameterCount = method.HasParameters ? method.Parameters.Count : 0;
				for (var i  = 0; i < parameters.Count; i++) {
					var a = method.Parameters [i];
					var b = parameters [i];
					if (a.ParameterType.FullName == b.ParameterType.FullName) {
						matchingMethod = method;
						return true;
					}
				}
			}
			return false;
		}

		static void EmitField (StringBuilder sb, FieldDefinition fd, int indent)
		{
			var strIndent = new string ('\t', indent);
			if (fd.DeclaringType.IsEnum) {
				if (!fd.IsStatic)
					return;
				sb.Append (strIndent);
				sb.Append (fd.Name);
				sb.AppendLine (",");
			} else {
				sb.Append (strIndent);
				sb.Append ("public ");
				if (fd.IsStatic)
					sb.Append ("static ");
				EmitTypeName (sb, fd.FieldType);
				sb.Append (' ');
				sb.Append (fd.Name);
				sb.Append (';');
				sb.AppendLine ();
			}
		}

		static bool HasPropertyInTypeHierarchy (TypeDefinition type, MethodDefinition accessor, out bool valid)
		{
			type = type?.BaseType?.Resolve ();

			valid = false;

			if (type == null)
				return false;

			var first = type.Properties.Select (v => v.GetMethod ?? v.SetMethod).FirstOrDefault (v =>
					v.IsVirtual == accessor.IsVirtual &&
					v.IsStatic == accessor.IsStatic &&
					v.Name == accessor.Name);
			if (first != null) {
				if (accessor.Name.StartsWith ("set_", StringComparison.Ordinal)) {
					valid = accessor.Parameters [0].ParameterType.FullName == first.Parameters [0].ParameterType.FullName;
				} else {
					valid = accessor.ReturnType.FullName == first.ReturnType.FullName;
				}
				return true;
			}

			return HasPropertyInTypeHierarchy (type, accessor, out valid);
		}

		static void EmitProperty (StringBuilder sb, PropertyDefinition pd, int indent)
		{
			var strIndent = new string ('\t', indent);
			sb.Append (strIndent);
			sb.Append ("public ");
			var m = pd.GetMethod ?? pd.SetMethod;
			if (m.IsStatic) {
				sb.Append ("static ");
				if (HasPropertyInTypeHierarchy (pd.DeclaringType, m, out var _))
					sb.Append ("new ");
			} else if (m.IsVirtual) {
				if (HasPropertyInTypeHierarchy (pd.DeclaringType, m, out var valid)) {
					if (valid) {
						sb.Append ("override ");
					} else {
						sb.Append ("new ");
					}
				} else if (!pd.DeclaringType.IsSealed) {
					sb.Append ("virtual ");
				}
			}
			EmitTypeName (sb, pd.PropertyType);
			sb.Append (' ');
			sb.Append (pd.Name);
			sb.AppendLine (" {");
			if (pd.GetMethod != null) {
				sb.AppendLine ($"{strIndent}\tget {{ throw new global::System.PlatformNotSupportedException (global::Constants.UnavailableOnMacCatalyst); }}");
			}
			if (pd.SetMethod != null) {
				sb.AppendLine ($"{strIndent}\tset {{ throw new global::System.PlatformNotSupportedException (global::Constants.UnavailableOnMacCatalyst); }}");
			}
			sb.AppendLine ($"{strIndent}}}");
		}

		static void EmitEvent (StringBuilder sb, EventDefinition ed, int indent)
		{
			sb.AppendLine ("#pragma warning disable CS0067");
			sb.Append ('\t', indent);
			sb.Append ("public ");
			var e = ed.AddMethod ?? ed.InvokeMethod;
			if (e.IsStatic)
				sb.Append ("static ");
			sb.Append ("event ");
			EmitTypeName (sb, ed.EventType);
			sb.Append (' ');
			var dot = ed.Name.LastIndexOf ('.');
			if (dot >= 0) {
				sb.Append (ed.Name.Substring (dot + 1));
			} else {
				sb.Append (ed.Name);
			}
			sb.Append (';');
			sb.AppendLine ();
		}

		static void EmitPNSE (StringBuilder sb, TypeDefinition type, int indent)
		{
			if (!type.IsNested)
				sb.AppendLine ($"namespace {type.Namespace} {{");
			var strIndent = new string ('\t', indent);
			if (type.BaseType?.FullName == "System.MulticastDelegate") {
				sb.Append ($"{strIndent}public delegate ");
				var invoke = type.Methods.First (v => v.Name == "Invoke");
				EmitTypeName (sb, invoke.ReturnType);
				sb.Append (' ');
				sb.Append (type.Name);
				sb.Append (" (");
				EmitParameters (sb, invoke.Parameters);
				sb.AppendLine (");");
			} else {
				sb.Append ($"{strIndent}public ");
				if (type.IsInterface) {
					sb.Append ("interface ");
				} else if (type.IsEnum) {
					sb.Append ("enum ");
				} else if (type.IsValueType) {
					sb.Append ("struct ");
				} else {
					if (type.IsAbstract) {
						if (type.IsSealed)
							sb.Append ("static ");
						else
							sb.Append ("abstract ");
					} else if (type.IsSealed)
						sb.Append ("sealed ");
					sb.Append ("class ");
				}

				if (type.HasGenericParameters) {
					sb.Append (type.Name.Substring (0, type.Name.IndexOf ('`')));
					PrintGenericParameters (sb, type.GenericParameters, true);
				} else {
					sb.Append (type.Name);
				}

				if (type.IsEnum) {
					sb.Append (" : ");
					var enumType = type.Fields.First (v => v.Name == "value__").FieldType;
					switch (enumType.FullName) {
					case "System.Byte":
						sb.Append ("byte");
						break;
					case "System.SByte":
						sb.Append ("sbyte");
						break;
					case "System.Int32":
						sb.Append ("int");
						break;
					case "System.UInt32":
						sb.Append ("uint");
						break;
					case "System.Int64":
						sb.Append ("long");
						break;
					case "System.UInt64":
						sb.Append ("ulong");
						break;
					default:
						throw new NotImplementedException (enumType.FullName);
					}
				} else if (type.IsValueType) {
					// nothing to do
				} else if (type.BaseType != null && type.BaseType.FullName != "System.Object") {
					sb.Append (" : ");
					EmitTypeName (sb, type.BaseType);
				}
				sb.Append (" {");
				sb.AppendLine ();
				foreach (var nestedType in type.NestedTypes) {
					if (nestedType.IsNestedPrivate)
						continue;
					EmitPNSE (sb, nestedType, indent + 1);
				}

				bool nsobject = false;
				var bt = type.BaseType?.Resolve ();
				while (bt != null) {
					nsobject = bt.FullName == "Foundation.NSObject";
					if (nsobject)
						break;
					bt = bt.BaseType?.Resolve ();
				}
				foreach (var method in type.Methods) {
					EmitPNSE (sb, method, indent + 1, nsobject);
				}

				foreach (var field in type.Fields) {
					if (field.IsPrivate)
						continue;

					EmitField (sb, field, indent + 1);
				}

				foreach (var prop in type.Properties) {
					if (prop.GetMethod?.IsPrivate != false && prop.SetMethod?.IsPrivate != false)
						continue;
					EmitProperty (sb, prop, indent + 1);
				}

				foreach (var ev in type.Events)
					EmitEvent (sb, ev, indent + 1);

				sb.AppendLine ($"{strIndent}}}");
			}
			if (!type.IsNested)
				sb.AppendLine ("}");
		}

		static void PrintGenericParameters (StringBuilder sb, IList<GenericParameter> genericParameters, bool printName)
		{
			if (genericParameters?.Any () != true)
				return;

			sb.Append ('<');
			for (var i = 0; i < genericParameters.Count; i++) {
				if (i > 0)
					sb.Append (',');
				if (printName) {
					if (i > 0)
						sb.Append (' ');
					sb.Append (genericParameters [i].Name);
				}
			}
			sb.Append ('>');
		}

		static int Fix (string forwardFrom, string forwardTo, string output)
		{
			var rp = new ReaderParameters (ReadingMode.Deferred);
			var resolver = new DefaultAssemblyResolver ();
			resolver.AddSearchDirectory (Path.GetDirectoryName (forwardFrom));
			rp.AssemblyResolver = resolver;
			var from = AssemblyDefinition.ReadAssembly (forwardFrom, rp);
			var to = AssemblyDefinition.ReadAssembly (forwardTo, rp);
			var sb = new StringBuilder ();

			sb.AppendLine ("using System.Runtime.CompilerServices;");

			foreach (var exportedType in from.MainModule.ExportedTypes) {
				sb.AppendLine ($"[assembly: TypeForwardedToAttribute (typeof ({exportedType.Namespace}.{exportedType.Name}))]");
			}

			var pnseTypes = new List<TypeDefinition> ();

			foreach (var type in from.MainModule.Types.OrderBy (v => v.FullName)) {
				if (type.IsNotPublic)
					continue;

				var toType = to.MainModule.GetType (type.FullName);
				if (toType == null) {
					pnseTypes.Add (type);
					continue;
				} else if (toType.IsNotPublic) {
					pnseTypes.Add (type);
					continue;
				}

				sb.Append ("[assembly: TypeForwardedToAttribute (typeof (");
				sb.Append (type.Namespace);
				sb.Append (".");

				if (type.HasGenericParameters) {
					var nonGeneric = type.Name.Substring (0, type.Name.IndexOf ('`'));
					sb.Append (nonGeneric);
					PrintGenericParameters (sb, type.GenericParameters, false);
				} else {
					sb.Append (type.Name);
				}
				sb.AppendLine ("))]");
			}

			sb.AppendLine (@"
// We can't use the ObjCRuntime.Constants class, because this assembly already has a type forwarder for that class to Xamarin.MacCatalyst.dll
class Constants {
	internal const string UnavailableOnMacCatalyst = ""This API is not available on Mac Catalyst"";
}
");

			// Generate implementations that throw PlatformNotSupportedException for types that don't exist or aren't public in Xamarin.MacCatalyst.dll
			foreach (var type in pnseTypes) {
				EmitPNSE (sb, type, 1);
			}

			File.WriteAllText (output, sb.ToString ());
			Console.WriteLine ($"Created type forwarders: {output}");
			return 0;
		}
	}
}
