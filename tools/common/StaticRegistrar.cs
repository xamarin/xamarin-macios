//
// StaticRegistrar.cs: The static registrar
// 
// Authors:
//   Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2013 Xamarin Inc. 
//

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;

using Xamarin.Bundler;
using Xamarin.Linker;
using Xamarin.Tuner;
using Xamarin.Utils;

#if MONOTOUCH
using PlatformResolver = MonoTouch.Tuner.MonoTouchResolver;
#elif MMP
using PlatformResolver = Xamarin.Bundler.MonoMacResolver;
#elif NET
using PlatformResolver = Xamarin.Linker.DotNetResolver;
#else
#error Invalid defines
#endif

using Registrar;
using Foundation;
using ObjCRuntime;
using Mono.Cecil;
using Mono.Linker;
using Mono.Tuner;
using ClassRedirector;

// Disable warnings about nullability attributes in code until we've reviewed this file for nullability (and enabled it).
// This way we can add nullability attributes to new code in this file without getting warnings about these attributes.
#pragma warning disable 8632 // warning CS8632: The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.

namespace Registrar {
	/*
	 * This class will automatically detect lines starting/ending with curly braces,
	 * and indent/unindent accordingly.
	 * 
	 * It doesn't cope with indentation due to other circumstances than
	 * curly braces (such as one-line if statements for instance). In this case
	 * call Indent/Unindent manually.
	 * 
	 * Also don't try to print '\n' directly, it'll get confused. Use
	 * the AppendLine/WriteLine family of methods instead.
	 */
	class AutoIndentStringBuilder : IDisposable {
		StringBuilder sb = new StringBuilder ();
		int indent;
		bool was_last_newline = true;

		public int Indentation {
			get { return indent; }
			set { indent = value; }
		}

		public AutoIndentStringBuilder ()
		{
		}

		public AutoIndentStringBuilder (int indentation)
		{
			indent = indentation;
		}

		public StringBuilder StringBuilder {
			get {
				return sb;
			}
		}

		void OutputIndentation ()
		{
			if (!was_last_newline)
				return;
			sb.Append ('\t', indent);
			was_last_newline = false;
		}

		void Output (string a)
		{
			if (a.StartsWith ("}", StringComparison.Ordinal))
				Unindent ();
			OutputIndentation ();
			sb.Append (a);
			if (a.EndsWith ("{", StringComparison.Ordinal))
				Indent ();
		}

		public AutoIndentStringBuilder AppendLine (AutoIndentStringBuilder isb)
		{
			if (isb.Length > 0) {
				sb.Append (isb.ToString ());
				AppendLine ();
			}

			return this;
		}

		public AutoIndentStringBuilder Append (AutoIndentStringBuilder isb)
		{
			if (isb.Length > 0)
				sb.Append (isb.ToString ());

			return this;
		}

		public AutoIndentStringBuilder Append (string value)
		{
			Output (value);
			return this;
		}

		public AutoIndentStringBuilder AppendFormat (string format, params object [] args)
		{
			Output (string.Format (format, args));
			return this;
		}

		public AutoIndentStringBuilder AppendLine (string value)
		{
			Output (value);
			sb.AppendLine ();
			was_last_newline = true;
			return this;
		}

		public AutoIndentStringBuilder AppendLine ()
		{
			sb.AppendLine ();
			was_last_newline = true;
			return this;
		}

		public AutoIndentStringBuilder AppendLine (string format, params object [] args)
		{
			Output (string.Format (format, args));
			sb.AppendLine ();
			was_last_newline = true;
			return this;
		}

		public AutoIndentStringBuilder Write (string value)
		{
			return Append (value);
		}

		public AutoIndentStringBuilder Write (string format, params object [] args)
		{
			return AppendFormat (format, args);
		}

		public AutoIndentStringBuilder WriteLine (string format, params object [] args)
		{
			return AppendLine (format, args);
		}

		public AutoIndentStringBuilder WriteLine (string value)
		{
			return AppendLine (value);
		}

		public AutoIndentStringBuilder WriteLine ()
		{
			return AppendLine ();
		}

		public AutoIndentStringBuilder WriteLine (AutoIndentStringBuilder isb)
		{
			return AppendLine (isb);
		}

		public void Replace (string find, string replace)
		{
			sb.Replace (find, replace);
		}

		public AutoIndentStringBuilder Indent ()
		{
			indent++;
			if (indent > 100)
				throw new ArgumentOutOfRangeException ("indent");
			return this;
		}

		public AutoIndentStringBuilder Unindent ()
		{
			indent--;
			return this;
		}

		public int Length {
			get { return sb.Length; }
		}

		public void Clear ()
		{
			sb.Clear ();
			was_last_newline = true;
		}

		public void Dispose ()
		{
			Clear ();
		}
		public override string ToString ()
		{
			return sb.ToString (0, sb.Length);
		}
	}

	class StaticRegistrar : Registrar {
		static string NFloatTypeName { get => Driver.IsDotNet ? "System.Runtime.InteropServices.NFloat" : "System.nfloat"; }
		const uint INVALID_TOKEN_REF = 0xFFFFFFFF;

		Dictionary<ICustomAttribute, MethodDefinition> protocol_member_method_map;

		public Dictionary<ICustomAttribute, MethodDefinition> ProtocolMemberMethodMap {
			get {
				if (protocol_member_method_map is null) {
					if (App.Platform != ApplePlatform.MacOSX && App.IsExtension && !App.IsWatchExtension && App.IsCodeShared) {
						protocol_member_method_map = Target.ContainerTarget.StaticRegistrar.ProtocolMemberMethodMap;
					} else {
						protocol_member_method_map = new Dictionary<ICustomAttribute, MethodDefinition> ();
					}
				}
				return protocol_member_method_map;
			}
		}

		public static bool IsPlatformType (TypeReference type, string @namespace, string name)
		{
			return type.Is (@namespace, name);
		}

		public static bool ParametersMatch (IList<ParameterDefinition> a, TypeReference [] b)
		{
			if (a is null && b is null)
				return true;
			if (a is null ^ b is null)
				return false;

			if (a.Count != b.Length)
				return false;

			for (var i = 0; i < b.Length; i++)
				if (!TypeMatch (a [i].ParameterType, b [i]))
					return false;

			return true;
		}

		public static bool TypeMatch (IModifierType a, IModifierType b)
		{
			if (!TypeMatch (a.ModifierType, b.ModifierType))
				return false;

			return TypeMatch (a.ElementType, b.ElementType);
		}

		public static bool TypeMatch (TypeSpecification a, TypeSpecification b)
		{
			if (a is GenericInstanceType)
				return TypeMatch ((GenericInstanceType) a, (GenericInstanceType) b);

			if (a is IModifierType)
				return TypeMatch ((IModifierType) a, (IModifierType) b);

			return TypeMatch (a.ElementType, b.ElementType);
		}

		public static bool TypeMatch (GenericInstanceType a, GenericInstanceType b)
		{
			if (!TypeMatch (a.ElementType, b.ElementType))
				return false;

			if (a.GenericArguments.Count != b.GenericArguments.Count)
				return false;

			if (a.GenericArguments.Count == 0)
				return true;

			for (int i = 0; i < a.GenericArguments.Count; i++)
				if (!TypeMatch (a.GenericArguments [i], b.GenericArguments [i]))
					return false;

			return true;
		}

		public static bool TypeMatch (TypeReference a, TypeReference b)
		{
			if (a is GenericParameter)
				return true;

			if (a is TypeSpecification || b is TypeSpecification) {
				if (a.GetType () != b.GetType ())
					return false;

				return TypeMatch ((TypeSpecification) a, (TypeSpecification) b);
			}

			return a.FullName == b.FullName;
		}

		public static bool MethodMatch (MethodDefinition candidate, MethodDefinition method)
		{
			if (!candidate.IsVirtual)
				return false;

			if (candidate.Name != method.Name)
				return false;

			if (!TypeMatch (candidate.ReturnType, method.ReturnType))
				return false;

			if (!candidate.HasParameters)
				return !method.HasParameters;

			if (candidate.Parameters.Count != method.Parameters.Count)
				return false;

			for (int i = 0; i < candidate.Parameters.Count; i++)
				if (!TypeMatch (candidate.Parameters [i].ParameterType, method.Parameters [i].ParameterType))
					return false;

			return true;
		}

		void CollectInterfaces (ref List<TypeDefinition> ifaces, TypeDefinition type)
		{
			if (type is null)
				return;

			if (type.BaseType is not null)
				CollectInterfaces (ref ifaces, type.BaseType.Resolve ());

			if (!type.HasInterfaces)
				return;

			foreach (var iface in type.Interfaces) {
				var itd = iface.InterfaceType.Resolve ();
				CollectInterfaces (ref ifaces, itd);

				if (!HasAttribute (itd, Registrar.Foundation, Registrar.StringConstants.ProtocolAttribute))
					continue;

				if (ifaces is null) {
					ifaces = new List<TypeDefinition> ();
				} else if (ifaces.Contains (itd)) {
					continue;
				}

				ifaces.Add (itd);
			}
		}

		public Dictionary<MethodDefinition, List<MethodDefinition>> PrepareInterfaceMethodMapping (TypeReference type)
		{
			TypeDefinition td = type.Resolve ();
			List<TypeDefinition> ifaces = null;
			List<MethodDefinition> iface_methods;
			Dictionary<MethodDefinition, List<MethodDefinition>> rv = null;

			CollectInterfaces (ref ifaces, td);

			if (ifaces is null)
				return null;

			iface_methods = new List<MethodDefinition> ();
			foreach (var iface in ifaces) {
				var storedMethods = LinkContext?.GetProtocolMethods (iface.Resolve ());
				if (storedMethods?.Count > 0) {
					foreach (var imethod in storedMethods)
						if (!iface_methods.Contains (imethod))
							iface_methods.Add (imethod);
				}
				if (!iface.HasMethods)
					continue;

				foreach (var imethod in iface.Methods) {
					if (!iface_methods.Contains (imethod))
						iface_methods.Add (imethod);
				}
			}

			// We only care about implementators declared in 'type'.

			// First find all explicitly implemented methods.
			foreach (var impl in td.Methods) {
				if (!impl.HasOverrides)
					continue;

				foreach (var ifaceMethod in impl.Overrides) {
					var ifaceMethodDef = ifaceMethod.Resolve ();
					if (!iface_methods.Contains (ifaceMethodDef)) {
						// The type may implement interfaces which aren't protocol interfaces, so this is OK.
					} else {
						iface_methods.Remove (ifaceMethodDef);

						List<MethodDefinition> list;
						if (rv is null) {
							rv = new Dictionary<MethodDefinition, List<MethodDefinition>> ();
							rv [impl] = list = new List<MethodDefinition> ();
						} else if (!rv.TryGetValue (impl, out list)) {
							rv [impl] = list = new List<MethodDefinition> ();
						}
						list.Add (ifaceMethodDef);
					}
				}
			}

			// Next find all implicitly implemented methods.
			foreach (var ifaceMethod in iface_methods) {
				foreach (var impl in td.Methods) {
					if (impl.Name != ifaceMethod.Name)
						continue;

					if (!MethodMatch (impl, ifaceMethod))
						continue;

					List<MethodDefinition> list;
					if (rv is null) {
						rv = new Dictionary<MethodDefinition, List<MethodDefinition>> ();
						rv [impl] = list = new List<MethodDefinition> ();
					} else if (!rv.TryGetValue (impl, out list)) {
						rv [impl] = list = new List<MethodDefinition> ();
					}
					list.Add (ifaceMethod);
				}
			}

			return rv;
		}

		public static TypeReference GetEnumUnderlyingType (TypeDefinition type)
		{
			foreach (FieldDefinition field in type.Fields)
				if (field.IsSpecialName && field.Name == "value__")
					return field.FieldType;

			return null;
		}

		public string ToObjCType (TypeReference type)
		{
			var definition = type as TypeDefinition;
			if (definition is not null)
				return ToObjCType (definition);

			if (type is TypeSpecification)
				return ToObjCType (((TypeSpecification) type).ElementType) + " *";

			definition = type.Resolve ();
			if (definition is not null)
				return ToObjCType (definition);

			return "void *";
		}

		public string ToObjCType (TypeDefinition type, bool delegateToBlockType = false, bool cSyntaxForBlocks = false)
		{
			switch (type.FullName) {
			case "System.IntPtr": return "void *";
			case "System.SByte": return "unsigned char";
			case "System.Byte": return "signed char";
			case "System.Char": return "signed short";
			case "System.Int16": return "short";
			case "System.UInt16": return "unsigned short";
			case "System.Int32": return "int";
			case "System.UInt32": return "unsigned int";
			case "System.Int64": return "long long";
			case "System.UInt64": return "unsigned long long";
			case "System.Single": return "float";
			case "System.Double": return "double";
			case "System.Boolean": return "BOOL";
			case "System.Void": return "void";
			case "System.String": return "NSString *";
			case "ObjCRuntime.Selector": return "SEL";
			case "ObjCRuntime.Class": return "Class";
			}

			if (IsNativeObject (type))
				return "id";

			if (IsDelegate (type)) {
				if (!delegateToBlockType)
					return "id";

				MethodDefinition invokeMethod = type.Methods.SingleOrDefault (method => method.Name == "Invoke");
				if (invokeMethod is null)
					return "id";

				StringBuilder builder = new StringBuilder ();
				builder.Append (ToObjCType (invokeMethod.ReturnType));
				builder.Append (" (^");
				if (cSyntaxForBlocks)
					builder.Append ("%PARAMETERNAME%");
				builder.Append (")(");

				var argumentTypes = invokeMethod.Parameters.Select (param => ToObjCType (param.ParameterType));
				builder.Append (string.Join (", ", argumentTypes));

				builder.Append (")");
				return builder.ToString ();
			}

			if (type.IsEnum)
				return ToObjCType (GetEnumUnderlyingType (type));

			if (type.IsValueType) {
				return "void *";
			}

			throw ErrorHelper.CreateError (4108, Errors.MT4108, type.FullName);
		}

		public static bool IsDelegate (TypeDefinition type)
		{
			while (type is not null) {
				if (type.FullName == "System.Delegate")
					return true;

				type = type.BaseType is not null ? type.BaseType.Resolve () : null;
			}

			return false;
		}

		TypeDefinition ResolveType (TypeReference tr)
		{
			return ResolveType (LinkContext, tr);
		}

		public static TypeDefinition ResolveType (Xamarin.Tuner.DerivedLinkContext context, TypeReference tr)
		{
			// The static registrar might sometimes deal with types that have been linked away
			// It's not always possible to call .Resolve () on types that have been linked away,
			// it might result in a NotSupportedException, or just a null value, so here we
			// manually replicate some resolution logic to try to avoid those NotSupportedExceptions.
			// The static registrar calls .Resolve () in a lot of places; we'll only call
			// this method if there's an actual need.
			if (tr is ArrayType arrayType) {
				return arrayType.ElementType.Resolve ();
			} else if (tr is GenericInstanceType git) {
				return ResolveType (context, git.ElementType);
			} else {
				var td = tr.Resolve ();
				if (td is null)
					td = context?.GetLinkedAwayType (tr, out _);
				return td;
			}
		}

		public bool IsNativeObject (TypeReference tr)
		{
			return IsNativeObject (LinkContext, tr);
		}

		public static bool IsNativeObject (Xamarin.Tuner.DerivedLinkContext context, TypeReference tr)
		{
			var gp = tr as GenericParameter;
			if (gp is not null) {
				if (gp.HasConstraints) {
					foreach (var constraint in gp.Constraints) {
						if (IsNativeObject (context, constraint.ConstraintType))
							return true;
					}
				}
				return false;
			}

			var type = ResolveType (context, tr);

			while (type is not null) {
				if (type.HasInterfaces) {
					foreach (var iface in type.Interfaces)
						if (iface.InterfaceType.Is (Registrar.ObjCRuntime, Registrar.StringConstants.INativeObject))
							return true;
				}

				type = type.BaseType is not null ? type.BaseType.Resolve () : null;
			}

			return tr.Is (Registrar.ObjCRuntime, Registrar.StringConstants.INativeObject);
		}

		public Target Target { get; private set; }
		public bool IsSingleAssembly { get { return !string.IsNullOrEmpty (single_assembly); } }

		string single_assembly;
		IEnumerable<AssemblyDefinition> input_assemblies;
		Dictionary<IMetadataTokenProvider, object> availability_annotations;

		PlatformResolver resolver;
		PlatformResolver Resolver { get { return resolver ?? Target.Resolver; } }

		readonly Version MacOSTenTwelveVersion = new Version (10, 12);

		public Xamarin.Tuner.DerivedLinkContext LinkContext {
			get {
				return Target?.GetLinkContext ();
			}
		}

		Dictionary<IMetadataTokenProvider, object> AvailabilityAnnotations {
			get {
				if (availability_annotations is null)
					availability_annotations = LinkContext?.GetAllCustomAttributes ("Availability");
				return availability_annotations;
			}
		}

		// Look for linked away attributes as well as attributes on the attribute provider.
		IEnumerable<ICustomAttribute> GetCustomAttributes (ICustomAttributeProvider provider, string @namespace, string name, bool inherits = false)
		{
			var dict = LinkContext?.Annotations?.GetCustomAnnotations (name);
			object annotations = null;

			if (dict?.TryGetValue (provider, out annotations) == true) {
				var attributes = (IEnumerable<ICustomAttribute>) annotations;
				foreach (var attrib in attributes) {
					if (IsAttributeMatch (attrib, @namespace, name, inherits))
						yield return attrib;
				}
			}

			if (provider.HasCustomAttributes) {
				foreach (var attrib in provider.CustomAttributes) {
					if (IsAttributeMatch (attrib, @namespace, name, inherits))
						yield return attrib;
				}
			}
		}

		public bool TryGetAttribute (ICustomAttributeProvider provider, string @namespace, string attributeName, out ICustomAttribute attribute)
		{
			attribute = null;

			foreach (var custom_attribute in GetCustomAttributes (provider, @namespace, attributeName)) {
				attribute = custom_attribute;
				return true;
			}

			return false;
		}

		public bool HasAttribute (ICustomAttributeProvider provider, string @namespace, string name, bool inherits = false)
		{
			return GetCustomAttributes (provider, @namespace, name, inherits).Any ();
		}

		bool IsAttributeMatch (ICustomAttribute attribute, string @namespace, string name, bool inherits)
		{
			if (inherits)
				return attribute.AttributeType.Inherits (@namespace, name);
			return attribute.AttributeType.Is (@namespace, name);
		}

		void Init (Application app)
		{
			this.App = app;
			trace = !LaxMode && (app.RegistrarOptions & RegistrarOptions.Trace) == RegistrarOptions.Trace;
		}

		public StaticRegistrar (Application app)
		{
			Init (app);
		}

		public StaticRegistrar (Target target)
		{
			Init (target.App);
			this.Target = target;
		}

		protected override PropertyDefinition FindProperty (TypeReference type, string name)
		{
			var td = type.Resolve ();
			if (td?.HasProperties != true)
				return null;

			foreach (var prop in td.Properties) {
				if (prop.Name == name)
					return prop;
			}

			return null;
		}

		protected override IEnumerable<MethodDefinition> FindMethods (TypeReference type, string name)
		{
			var td = type.Resolve ();
			if (td?.HasMethods != true)
				return null;

			List<MethodDefinition> list = null;
			foreach (var method in td.Methods) {
				if (method.Name != name)
					continue;

				if (list is null)
					list = new List<MethodDefinition> ();

				list.Add (method);
			}
			return list;
		}

		public override TypeReference FindType (TypeReference relative, string @namespace, string name)
		{
			return relative.Resolve ().Module.GetType (@namespace, name);
		}

		protected override bool LaxMode {
			get {
				return IsSingleAssembly;
			}
		}

		protected override void ReportError (int code, string message, params object [] args)
		{
			throw ErrorHelper.CreateError (code, message, args);
		}

		protected override void ReportWarning (int code, string message, params object [] args)
		{
			ErrorHelper.Show (ErrorHelper.CreateWarning (code, message, args));
		}

		public static int GetValueTypeSize (TypeDefinition type, bool is_64_bits)
		{
			switch (type.FullName) {
			case "System.Char": return 2;
			case "System.Boolean":
			case "System.SByte":
			case "System.Byte": return 1;
			case "System.Int16":
			case "System.UInt16": return 2;
			case "System.Single":
			case "System.Int32":
			case "System.UInt32": return 4;
			case "System.Double":
			case "System.Int64":
			case "System.UInt64": return 8;
			case "System.IntPtr":
			case "System.nuint":
			case "System.nint": return is_64_bits ? 8 : 4;
			default:
				if (type.FullName == NFloatTypeName)
					return is_64_bits ? 8 : 4;
				int size = 0;
				foreach (FieldDefinition field in type.Fields) {
					if (field.IsStatic)
						continue;
					int s = GetValueTypeSize (field.FieldType.Resolve (), is_64_bits);
					if (s == -1)
						return -1;
					size += s;
				}
				return size;
			}
		}

		protected override int GetValueTypeSize (TypeReference type)
		{
			return GetValueTypeSize (type.Resolve (), Is64Bits);
		}

		public override bool HasReleaseAttribute (MethodDefinition method)
		{
			method = GetBaseMethodInTypeHierarchy (method);
			return HasAttribute (method.MethodReturnType, ObjCRuntime, StringConstants.ReleaseAttribute);
		}

		public override bool HasThisAttribute (MethodDefinition method)
		{
			return HasAttribute (method, "System.Runtime.CompilerServices", "ExtensionAttribute");
		}

		public bool IsSimulator {
			get { return App.IsSimulatorBuild; }
		}

		protected override bool IsSimulatorOrDesktop {
			get {
				return App.Platform == ApplePlatform.MacOSX || App.IsSimulatorBuild;
			}
		}

		protected override bool Is64Bits {
			get {
				if (IsSingleAssembly)
					return App.Is64Build;

				// Target can be null when mmp is run for multiple assemblies
				return Target is not null ? Target.Is64Build : App.Is64Build;
			}
		}

		protected override bool IsARM64 {
			get {
				return Application.IsArchEnabled (Target?.Abis ?? App.Abis, Xamarin.Abi.ARM64);
			}
		}

		protected override Exception CreateExceptionImpl (int code, bool error, Exception innerException, MethodDefinition method, string message, params object [] args)
		{
			return ErrorHelper.Create (App, code, error, innerException, method, message, args);
		}

		protected override Exception CreateExceptionImpl (int code, bool error, Exception innerException, TypeReference type, string message, params object [] args)
		{
			return ErrorHelper.Create (App, code, error, innerException, type, message, args);
		}

		protected override bool ContainsPlatformReference (AssemblyDefinition assembly)
		{
			if (assembly.Name.Name == PlatformAssembly)
				return true;

			if (!assembly.MainModule.HasAssemblyReferences)
				return false;

			foreach (var ar in assembly.MainModule.AssemblyReferences) {
				if (ar.Name == PlatformAssembly)
					return true;
			}

			return false;
		}

		protected override IEnumerable<TypeReference> CollectTypes (AssemblyDefinition assembly)
			=> GetAllTypes (assembly);

		internal static IEnumerable<TypeReference> GetAllTypes (AssemblyDefinition assembly)
		{
			var queue = new Queue<TypeDefinition> ();

			foreach (TypeDefinition type in assembly.MainModule.Types) {
				if (type.HasNestedTypes)
					queue.Enqueue (type);
				else
					yield return type;
			}

			while (queue.Count > 0) {
				var nt = queue.Dequeue ();
				foreach (var tp in nt.NestedTypes) {
					if (tp.HasNestedTypes)
						queue.Enqueue (tp);
					else
						yield return tp;
				}

				yield return nt;
			}
		}

		protected override IEnumerable<MethodDefinition> CollectMethods (TypeReference tr)
		{
			var type = tr.Resolve ();
			if (!type.HasMethods)
				yield break;

			foreach (MethodDefinition method in type.Methods) {
				if (method.IsConstructor)
					continue;

				yield return method;
			}
		}

		protected override IEnumerable<MethodDefinition> CollectConstructors (TypeReference tr)
		{
			var type = tr.Resolve ();
			if (!type.HasMethods)
				yield break;

			foreach (MethodDefinition ctor in type.Methods) {
				if (ctor.IsConstructor)
					yield return ctor;
			}
		}

		protected override IEnumerable<PropertyDefinition> CollectProperties (TypeReference tr)
		{
			var type = tr.Resolve ();
			if (!type.HasProperties)
				yield break;

			foreach (PropertyDefinition property in type.Properties)
				yield return property;
		}

		protected override string GetAssemblyName (AssemblyDefinition assembly)
		{
			return assembly.Name.Name;
		}

		protected override string GetTypeFullName (TypeReference type)
		{
			return type.FullName;
		}

		protected override string GetMethodName (MethodDefinition method)
		{
			return method.Name;
		}

		protected override string GetPropertyName (PropertyDefinition property)
		{
			return property.Name;
		}

		protected override TypeReference GetPropertyType (PropertyDefinition property)
		{
			return property.PropertyType;
		}

		protected override string GetTypeName (TypeReference type)
		{
			return type.Name;
		}

		protected override string GetFieldName (FieldDefinition field)
		{
			return field.Name;
		}

		protected override IEnumerable<FieldDefinition> GetFields (TypeReference tr)
		{
			var type = tr.Resolve ();
			foreach (FieldDefinition field in type.Fields)
				yield return field;
		}

		protected override TypeReference GetFieldType (FieldDefinition field)
		{
			return field.FieldType;
		}

		protected override bool IsByRef (TypeReference type)
		{
			return type.IsByReference;
		}

		protected override TypeReference MakeByRef (TypeReference type)
		{
			return new ByReferenceType (type);
		}

		protected override bool IsStatic (FieldDefinition field)
		{
			return field.IsStatic;
		}

		protected override bool IsStatic (MethodDefinition method)
		{
			return method.IsStatic;
		}

		protected override bool IsStatic (PropertyDefinition property)
		{
			if (property.GetMethod is not null)
				return property.GetMethod.IsStatic;
			if (property.SetMethod is not null)
				return property.SetMethod.IsStatic;
			return false;
		}

		public override TypeReference GetElementType (TypeReference type)
		{
			var ts = type as TypeSpecification;
			if (ts is not null) {
				// TypeSpecification.GetElementType calls GetElementType on the element type, thus unwinding multiple element types (which we don't want).
				// By fetching the ElementType property we only unwind one level.
				// This matches what the dynamic registrar (System.Reflection) does.
				return ts.ElementType;
			}

			if (type is ByReferenceType brt) {
				// ByReferenceType.GetElementType also calls GetElementType recursively.
				return brt.ElementType;
			}

			return type.GetElementType ();
		}

		protected override TypeReference GetReturnType (MethodDefinition method)
		{
			return method.ReturnType;
		}

		TypeReference system_void;
		protected override TypeReference GetSystemVoidType ()
		{
			if (system_void is not null)
				return system_void;

			// find corlib
			var corlib_name = Driver.CorlibName;
			AssemblyDefinition corlib = null;
			var candidates = new List<AssemblyDefinition> ();

			foreach (var assembly in input_assemblies) {
				if (corlib_name == assembly.Name.Name) {
					corlib = assembly;
					break;
				}
			}

			if (corlib is null)
				corlib = Resolver.Resolve (AssemblyNameReference.Parse (corlib_name), new ReaderParameters ());

			if (corlib is not null)
				candidates.Add (corlib);

			if (Resolver is not null)
				candidates.AddRange (Resolver.ToResolverCache ().Values.Cast<AssemblyDefinition> ());

			foreach (var candidate in candidates) {
				foreach (var type in candidate.MainModule.Types) {
					if (type.Namespace == "System" && type.Name == "Void")
						return system_void = type;
				}
			}

			throw ErrorHelper.CreateError (4165, Errors.MT4165);
		}

		protected override bool IsVirtual (MethodDefinition method)
		{
			return method.IsVirtual;
		}

		bool IsOverride (MethodDefinition method)
		{
			return method.IsVirtual && !method.IsNewSlot;
		}

		bool IsOverride (PropertyDefinition property)
		{
			if (property.GetMethod is not null)
				return IsOverride (property.GetMethod);
			return IsOverride (property.SetMethod);
		}

		protected override bool IsConstructor (MethodDefinition method)
		{
			return method.IsConstructor;
		}

		protected override bool IsDelegate (TypeReference tr)
		{
			var type = tr.Resolve ();
			return IsDelegate (type);
		}

		protected override bool IsValueType (TypeReference type)
		{
			var td = type.Resolve ();
			return td is not null && td.IsValueType;
		}

		bool IsNativeEnum (TypeDefinition td)
		{
			return HasAttribute (td, ObjCRuntime, StringConstants.NativeAttribute);
		}

		public override bool IsNullable (TypeReference type)
		{
			return GetNullableType (type) is not null;
		}

		protected override bool IsEnum (TypeReference tr, out bool isNativeEnum)
		{
			var type = tr.Resolve ();
			isNativeEnum = false;
			if (type is null)
				return false;
			if (type.IsEnum)
				isNativeEnum = IsNativeEnum (type);
			return type.IsEnum;
		}

		public override bool IsArray (TypeReference type, out int rank)
		{
			var arrayType = type as ArrayType;
			if (arrayType is null) {
				rank = 0;
				return false;
			}

			rank = arrayType.Rank;
			return true;
		}

		protected override bool IsGenericType (TypeReference type)
		{
			return type is GenericInstanceType || type.HasGenericParameters || type is GenericParameter;
		}

		protected override bool IsGenericMethod (MethodDefinition method)
		{
			return method.HasGenericParameters;
		}

		protected override bool IsInterface (TypeReference type)
		{
			if (type.IsArray)
				return false;
			return type.Resolve ()?.IsInterface == true;
		}

		protected override bool IsAbstract (TypeReference type)
		{
			if (type.IsArray)
				return false;
			return type.Resolve ()?.IsAbstract == true;
		}

		protected override bool IsPointer (TypeReference type)
		{
			return type is PointerType;
		}

		protected override TypeReference [] GetInterfaces (TypeReference type)
		{
			var td = type.Resolve ();
			if (!td.HasInterfaces || td.Interfaces.Count == 0)
				return null;
			var rv = new TypeReference [td.Interfaces.Count];
			for (int i = 0; i < td.Interfaces.Count; i++)
				rv [i] = td.Interfaces [i].InterfaceType.Resolve ();
			return rv;
		}

		protected override TypeReference [] GetLinkedAwayInterfaces (TypeReference type)
		{
			if (LinkContext is null)
				return null;

			if (LinkContext.ProtocolImplementations.TryGetValue (type.Resolve (), out var linkedAwayInterfaces) != true)
				return null;

			if (linkedAwayInterfaces.Count == 0)
				return null;

			return linkedAwayInterfaces.ToArray ();
		}

		protected override TypeReference GetGenericTypeDefinition (TypeReference type)
		{
			var git = type as GenericInstanceType;
			if (git is not null)
				return git.ElementType;
			return type;
		}

		protected override bool AreEqual (TypeReference a, TypeReference b)
		{
			if (a == b)
				return true;

			if (a is null ^ b is null)
				return false;

			return TypeMatch (a, b);
		}

		public override bool VerifyIsConstrainedToNSObject (TypeReference type, out TypeReference constrained_type)
		{
			constrained_type = null;

			var gp = type as GenericParameter;
			if (gp is not null) {
				if (!gp.HasConstraints)
					return false;
				foreach (var c in gp.Constraints) {
					if (IsNSObject (c.ConstraintType)) {
						constrained_type = c.ConstraintType;
						return true;
					}
				}
				return false;
			}

			var git = type as GenericInstanceType;
			if (git is not null) {
				var rv = true;
				if (git.HasGenericArguments) {
					var newGit = new GenericInstanceType (git.ElementType);
					for (int i = 0; i < git.GenericArguments.Count; i++) {
						TypeReference constr;
						rv &= VerifyIsConstrainedToNSObject (git.GenericArguments [i], out constr);
						newGit.GenericArguments.Add (constr ?? git.GenericArguments [i]);
					}
					constrained_type = newGit;
				}
				return rv;
			}

			var el = type as ArrayType;
			if (el is not null) {
				var rv = VerifyIsConstrainedToNSObject (el.ElementType, out constrained_type);
				if (constrained_type is null)
					return rv;
				constrained_type = new ArrayType (constrained_type, el.Rank);
				return rv;
			}

			var rt = type as ByReferenceType;
			if (rt is not null) {
				var rv = VerifyIsConstrainedToNSObject (rt.ElementType, out constrained_type);
				if (constrained_type is null)
					return rv;
				constrained_type = new ByReferenceType (constrained_type);
				return rv;
			}

			var tr = type as PointerType;
			if (tr is not null) {
				var rv = VerifyIsConstrainedToNSObject (tr.ElementType, out constrained_type);
				if (constrained_type is null)
					return rv;
				constrained_type = new PointerType (constrained_type);
				return rv;
			}

			return true;
		}

		protected override bool IsINativeObject (TypeReference tr)
		{
			return IsNativeObject (tr);
		}

		protected override TypeReference GetBaseType (TypeReference tr)
		{
			var gp = tr as GenericParameter;
			if (gp is not null) {
				foreach (var constr in gp.Constraints) {
					if (constr.ConstraintType.Resolve ().IsClass) {
						return constr.ConstraintType;
					}
				}
				return null;
			}
			var type = ResolveType (tr);
			if (type.BaseType is null)
				return null;

			return type.BaseType.Resolve ();
		}

		protected override MethodDefinition GetBaseMethod (MethodDefinition method)
		{
			return GetBaseMethodInTypeHierarchy (method);
		}

		protected override TypeReference GetEnumUnderlyingType (TypeReference tr)
		{
			var type = tr.Resolve ();
			return GetEnumUnderlyingType (type);
		}

		protected override TypeReference [] GetParameters (MethodDefinition method)
		{
			if (!method.HasParameters || method.Parameters.Count == 0)
				return null;

			var types = new TypeReference [method.Parameters.Count];
			for (int i = 0; i < types.Length; i++)
				types [i] = method.Parameters [i].ParameterType;
			return types;
		}

		protected override string GetParameterName (MethodDefinition method, int parameter_index)
		{
			if (method is null)
				return "?";
			return method.Parameters [parameter_index].Name;
		}

		protected override MethodDefinition GetGetMethod (PropertyDefinition property)
		{
			return property.GetMethod;
		}

		protected override MethodDefinition GetSetMethod (PropertyDefinition property)
		{
			return property.SetMethod;
		}

		protected override void GetNamespaceAndName (TypeReference type, out string @namespace, out string name)
		{
			name = type.Name;
			@namespace = type.Namespace;
		}

		protected override bool TryGetAttribute (TypeReference type, string attributeNamespace, string attributeType, out object attribute)
		{
			bool res = TryGetAttribute (type.Resolve (), attributeNamespace, attributeType, out var attrib);
			attribute = attrib;
			return res;
		}

		public override RegisterAttribute GetRegisterAttribute (TypeReference type)
		{
			RegisterAttribute rv = null;

			if (!TryGetAttribute (type.Resolve (), Foundation, StringConstants.RegisterAttribute, out var attrib))
				return null;

			if (!attrib.HasConstructorArguments) {
				rv = new RegisterAttribute ();
			} else {
				switch (attrib.ConstructorArguments.Count) {
				case 0:
					rv = new RegisterAttribute ();
					break;
				case 1:
					rv = new RegisterAttribute ((string) attrib.ConstructorArguments [0].Value);
					break;
				case 2:
					rv = new RegisterAttribute ((string) attrib.ConstructorArguments [0].Value, (bool) attrib.ConstructorArguments [1].Value);
					break;
				default:
					throw ErrorHelper.CreateError (4124, Errors.MT4124, "RegisterAttribute", type.FullName);
				}
			}

			if (attrib.HasProperties) {
				foreach (var prop in attrib.Properties) {
					switch (prop.Name) {
					case "IsWrapper":
						rv.IsWrapper = (bool) prop.Argument.Value;
						break;
					case "Name":
						rv.Name = (string) prop.Argument.Value;
						break;
					case "SkipRegistration":
						rv.SkipRegistration = (bool) prop.Argument.Value;
						break;
					default:
						throw ErrorHelper.CreateError (4124, Errors.MT4124_A, type.FullName, prop.Name);
					}
				}
			}

			return rv;
		}

		public override CategoryAttribute GetCategoryAttribute (TypeReference type)
		{
			string name = null;

			if (!TryGetAttribute (type.Resolve (), ObjCRuntime, StringConstants.CategoryAttribute, out var attrib))
				return null;

			if (!attrib.HasConstructorArguments)
				throw ErrorHelper.CreateError (4124, Errors.MT4124, "CategoryAttribute", type.FullName);

			if (attrib.HasProperties) {
				foreach (var prop in attrib.Properties) {
					if (prop.Name == "Name") {
						name = (string) prop.Argument.Value;
						break;
					}
				}
			}

			switch (attrib.ConstructorArguments.Count) {
			case 1:
				var t1 = (TypeReference) attrib.ConstructorArguments [0].Value;
				return new CategoryAttribute (t1 is not null ? t1.Resolve () : null) { Name = name };
			default:
				throw ErrorHelper.CreateError (4124, Errors.MT4124, "CategoryAttribute", type.FullName);
			}
		}

		protected override ExportAttribute GetExportAttribute (MethodDefinition method)
		{
			return CreateExportAttribute (GetBaseMethodInTypeHierarchy (method) ?? method);
		}

		protected override ExportAttribute GetExportAttribute (PropertyDefinition property)
		{
			return CreateExportAttribute (GetBasePropertyInTypeHierarchy (property) ?? property);
		}

		public override ProtocolAttribute GetProtocolAttribute (TypeReference type)
		{
			if (!TryGetAttribute (type.Resolve (), Foundation, StringConstants.ProtocolAttribute, out var attrib))
				return null;

			if (!attrib.HasProperties)
				return new ProtocolAttribute ();

			var rv = new ProtocolAttribute ();
			foreach (var prop in attrib.Properties) {
				switch (prop.Name) {
				case "Name":
					rv.Name = (string) prop.Argument.Value;
					break;
				case "WrapperType":
					rv.WrapperType = ((TypeReference) prop.Argument.Value).Resolve ();
					break;
				case "IsInformal":
					rv.IsInformal = (bool) prop.Argument.Value;
					break;
				case "FormalSince":
					Version version;
					if (!Version.TryParse ((string) prop.Argument.Value, out version))
						throw ErrorHelper.CreateError (4147, Errors.MT4147, "ProtocolAttribute", type.FullName);
					rv.FormalSinceVersion = version;
					break;
				default:
					throw ErrorHelper.CreateError (4147, Errors.MT4147, "ProtocolAttribute", type.FullName);
				}
			}

			return rv;
		}

		public BlockProxyAttribute GetBlockProxyAttribute (ParameterDefinition parameter)
		{
			if (!TryGetAttribute (parameter, ObjCRuntime, "BlockProxyAttribute", out var attrib))
				return null;

			var rv = new BlockProxyAttribute ();

			switch (attrib.ConstructorArguments.Count) {
			case 1:
				rv.Type = ((TypeReference) attrib.ConstructorArguments [0].Value).Resolve ();
				break;
			default:
				throw ErrorHelper.CreateError (4124, Errors.MT4124, "BlockProxyAttribute", ((MethodReference) parameter.Method)?.FullName);
			}

			return rv;
		}

		public DelegateProxyAttribute GetDelegateProxyAttribute (MethodDefinition method)
		{
			if (!TryGetAttribute (method.MethodReturnType, ObjCRuntime, "DelegateProxyAttribute", out var attrib))
				return null;

			var rv = new DelegateProxyAttribute ();

			switch (attrib.ConstructorArguments.Count) {
			case 1:
				rv.DelegateType = ((TypeReference) attrib.ConstructorArguments [0].Value).Resolve ();
				break;
			default:
				throw ErrorHelper.CreateError (4124, Errors.MT4124, "DelegateProxyAttribute", ((MethodReference) method)?.FullName);
			}

			return rv;
		}

		protected override string PlatformName {
			get {
				return App.PlatformName;
			}
		}

		ProtocolMemberAttribute GetProtocolMemberAttribute (TypeReference type, string selector, ObjCMethod obj_method, MethodDefinition method)
		{
			var memberAttributes = GetProtocolMemberAttributes (type);
			foreach (var attrib in memberAttributes) {
				if (attrib.IsStatic != method.IsStatic)
					continue;

				if (attrib.IsProperty) {
					if (method.IsSetter && attrib.SetterSelector != selector)
						continue;
					else if (method.IsGetter && attrib.GetterSelector != selector)
						continue;
				} else {
					if (attrib.Selector != selector)
						continue;
				}

				if (!obj_method.IsPropertyAccessor) {
					var attribParameters = new TypeReference [attrib.ParameterType?.Length ?? 0];
					for (var i = 0; i < attribParameters.Length; i++) {
						attribParameters [i] = attrib.ParameterType [i];
						if (attrib.ParameterByRef [i])
							attribParameters [i] = new ByReferenceType (attribParameters [i]);
					}
					if (!ParametersMatch (method.Parameters, attribParameters))
						continue;
				}

				return attrib;
			}

			return null;
		}

		protected override IEnumerable<ProtocolMemberAttribute> GetProtocolMemberAttributes (TypeReference type)
		{
			var td = type.Resolve ();

			foreach (var ca in GetCustomAttributes (td, Foundation, StringConstants.ProtocolMemberAttribute)) {
				var rv = new ProtocolMemberAttribute ();

				MethodDefinition implMethod = null;
				if (ProtocolMemberMethodMap.TryGetValue (ca, out implMethod) == true)
					rv.Method = implMethod;

				foreach (var prop in ca.Properties) {
					switch (prop.Name) {
					case "IsRequired":
						rv.IsRequired = (bool) prop.Argument.Value;
						break;
					case "IsProperty":
						rv.IsProperty = (bool) prop.Argument.Value;
						break;
					case "IsStatic":
						rv.IsStatic = (bool) prop.Argument.Value;
						break;
					case "Name":
						rv.Name = (string) prop.Argument.Value;
						break;
					case "Selector":
						rv.Selector = (string) prop.Argument.Value;
						break;
					case "ReturnType":
						rv.ReturnType = (TypeReference) prop.Argument.Value;
						break;
					case "ReturnTypeDelegateProxy":
						rv.ReturnTypeDelegateProxy = (TypeReference) prop.Argument.Value;
						break;
					case "ParameterType":
						if (prop.Argument.Value is not null) {
							var arr = (CustomAttributeArgument []) prop.Argument.Value;
							rv.ParameterType = new TypeReference [arr.Length];
							for (int i = 0; i < arr.Length; i++) {
								rv.ParameterType [i] = (TypeReference) arr [i].Value;
							}
						}
						break;
					case "ParameterByRef":
						if (prop.Argument.Value is not null) {
							var arr = (CustomAttributeArgument []) prop.Argument.Value;
							rv.ParameterByRef = new bool [arr.Length];
							for (int i = 0; i < arr.Length; i++) {
								rv.ParameterByRef [i] = (bool) arr [i].Value;
							}
						}
						break;
					case "ParameterBlockProxy":
						if (prop.Argument.Value is not null) {
							var arr = (CustomAttributeArgument []) prop.Argument.Value;
							rv.ParameterBlockProxy = new TypeReference [arr.Length];
							for (int i = 0; i < arr.Length; i++) {
								rv.ParameterBlockProxy [i] = (TypeReference) arr [i].Value;
							}
						}
						break;
					case "IsVariadic":
						rv.IsVariadic = (bool) prop.Argument.Value;
						break;
					case "PropertyType":
						rv.PropertyType = (TypeReference) prop.Argument.Value;
						break;
					case "GetterSelector":
						rv.GetterSelector = (string) prop.Argument.Value;
						break;
					case "SetterSelector":
						rv.SetterSelector = (string) prop.Argument.Value;
						break;
					case "ArgumentSemantic":
						rv.ArgumentSemantic = (ArgumentSemantic) prop.Argument.Value;
						break;
					}
				}
				yield return rv;
			}
		}

#if !NET
		PlatformName AsPlatformName (ApplePlatform platform)
		{
			switch (platform) {
			case ApplePlatform.iOS:
				return global::ObjCRuntime.PlatformName.iOS;
			case ApplePlatform.TVOS:
				return global::ObjCRuntime.PlatformName.TvOS;
			case ApplePlatform.WatchOS:
				return global::ObjCRuntime.PlatformName.WatchOS;
			case ApplePlatform.MacOSX:
				return global::ObjCRuntime.PlatformName.MacOSX;
			case ApplePlatform.MacCatalyst:
				return global::ObjCRuntime.PlatformName.MacCatalyst;
			default:
				throw ErrorHelper.CreateError (99, Errors.MX0099, $"unknown platform: {platform}");
			}
		}

		bool GetLegacyAvailabilityAttribute (ICustomAttribute ca, ApplePlatform platform, out Version sdkVersion, out string message)
		{
			var caType = ca.AttributeType;
			var currentPlatform = AsPlatformName (platform);
			AvailabilityKind kind;
			PlatformName platformName = global::ObjCRuntime.PlatformName.None;
			PlatformArchitecture architecture = PlatformArchitecture.All;
			int majorVersion = 0, minorVersion = 0, subminorVersion = 0;
			bool shorthand = false;

			sdkVersion = null;
			message = null;

			switch (caType.Name) {
			case "MacAttribute":
				shorthand = true;
				platformName = global::ObjCRuntime.PlatformName.MacOSX;
				goto case "IntroducedAttribute";
			case "iOSAttribute":
				shorthand = true;
				platformName = global::ObjCRuntime.PlatformName.iOS;
				goto case "IntroducedAttribute";
			case "IntroducedAttribute":
				kind = AvailabilityKind.Introduced;
				break;
			default:
				return false;
			}

			switch (ca.ConstructorArguments.Count) {
			case 2:
				if (!shorthand)
					throw ErrorHelper.CreateError (4163, Errors.MT4163, caType.Name, ca.ConstructorArguments.Count);
				majorVersion = (byte) ca.ConstructorArguments [0].Value;
				minorVersion = (byte) ca.ConstructorArguments [1].Value;
				break;
			case 3:
				if (!shorthand) {
					platformName = (PlatformName) ca.ConstructorArguments [0].Value;
					architecture = (PlatformArchitecture) ca.ConstructorArguments [1].Value;
					message = (string) ca.ConstructorArguments [2].Value;
				} else {
					majorVersion = (byte) ca.ConstructorArguments [0].Value;
					minorVersion = (byte) ca.ConstructorArguments [1].Value;
					if (ca.ConstructorArguments [2].Type.Name == "Boolean") {
						var onlyOn64 = (bool) ca.ConstructorArguments [2].Value;
						architecture = onlyOn64 ? PlatformArchitecture.Arch64 : PlatformArchitecture.All;
					} else if (ca.ConstructorArguments [2].Type.Name == "Byte") {
						minorVersion = (byte) ca.ConstructorArguments [2].Value;
					} else {
						throw ErrorHelper.CreateError (4163, Errors.MT4163, caType.Name, ca.ConstructorArguments.Count);
					}
				}
				break;
			case 4:
				if (!shorthand)
					throw ErrorHelper.CreateError (4163, Errors.MT4163, caType.Name, ca.ConstructorArguments.Count);

				majorVersion = (byte) ca.ConstructorArguments [0].Value;
				minorVersion = (byte) ca.ConstructorArguments [1].Value;
				minorVersion = (byte) ca.ConstructorArguments [2].Value;
				if (ca.ConstructorArguments [3].Type.Name == "Boolean") {
					var onlyOn64 = (bool) ca.ConstructorArguments [3].Value;
					architecture = onlyOn64 ? PlatformArchitecture.Arch64 : PlatformArchitecture.All;
				} else if (ca.ConstructorArguments [3].Type.Name == "PlatformArchitecture") {
					architecture = (PlatformArchitecture) (byte) ca.ConstructorArguments [3].Value;
				} else {
					throw ErrorHelper.CreateError (4163, Errors.MT4163, caType.Name, ca.ConstructorArguments.Count);
				}
				break;
			case 5:
				platformName = (PlatformName) ca.ConstructorArguments [0].Value;
				majorVersion = (int) ca.ConstructorArguments [1].Value;
				minorVersion = (int) ca.ConstructorArguments [2].Value;
				architecture = (PlatformArchitecture) ca.ConstructorArguments [3].Value;
				message = (string) ca.ConstructorArguments [4].Value;
				break;
			case 6:
				platformName = (PlatformName) ca.ConstructorArguments [0].Value;
				majorVersion = (int) ca.ConstructorArguments [1].Value;
				minorVersion = (int) ca.ConstructorArguments [2].Value;
				subminorVersion = (int) ca.ConstructorArguments [3].Value;
				architecture = (PlatformArchitecture) ca.ConstructorArguments [4].Value;
				message = (string) ca.ConstructorArguments [5].Value;
				break;
			default:
				throw ErrorHelper.CreateError (4163, Errors.MT4163, caType.Name, ca.ConstructorArguments.Count);
			}

			if (platformName != currentPlatform)
				return false;

			sdkVersion = new Version (majorVersion, minorVersion, subminorVersion);
			switch (kind) {
			case AvailabilityKind.Introduced:
				if (shorthand) {
					sdkVersion = new Version (majorVersion, minorVersion, subminorVersion);
				} else {
					switch (ca.ConstructorArguments.Count) {
					case 5:
						sdkVersion = new Version (majorVersion, minorVersion);
						break;
					case 6:
						sdkVersion = new Version (majorVersion, minorVersion, subminorVersion);
						break;
					default:
						throw ErrorHelper.CreateError (4163, Errors.MT4163, caType.Name, ca.ConstructorArguments.Count);
					}
				}
				break;
			default:
				throw ErrorHelper.CreateError (4163, Errors.MT4163_A, kind);
			}

			return true;
		}
#endif // !NET

#if NET
		bool GetDotNetAvailabilityAttribute (ICustomAttribute ca, ApplePlatform currentPlatform, out Version sdkVersion, out string message)
		{
			var caType = ca.AttributeType;

			sdkVersion = null;
			message = null;

			string supportedPlatformAndVersion;
			switch (ca.ConstructorArguments.Count) {
			case 1:
				supportedPlatformAndVersion = (string) ca.ConstructorArguments [0].Value;
				break;
			default:
				throw ErrorHelper.CreateError (4163, Errors.MT4163, caType.Name, ca.ConstructorArguments.Count);
			}

			if (!OSPlatformAttributeExtensions.TryParse (supportedPlatformAndVersion, out ApplePlatform? platformName, out sdkVersion))
				return false;

			if (platformName != currentPlatform)
				return false;

			return true;
		}
#endif // NET

		bool CollectAvailabilityAttributes (IEnumerable<ICustomAttribute> attributes, out Version sdkVersion, out string message)
		{
			sdkVersion = null;
			message = null;

			ApplePlatform currentPlatform;
			switch (App.Platform) {
			case ApplePlatform.iOS:
				currentPlatform = ApplePlatform.iOS;
				break;
			case ApplePlatform.TVOS:
				currentPlatform = ApplePlatform.TVOS;
				break;
			case ApplePlatform.WatchOS:
				currentPlatform = ApplePlatform.WatchOS;
				break;
			case ApplePlatform.MacOSX:
				currentPlatform = ApplePlatform.MacOSX;
				break;
			case ApplePlatform.MacCatalyst:
				currentPlatform = ApplePlatform.MacCatalyst;
				break;
			default:
				throw ErrorHelper.CreateError (71, Errors.MX0071, App.Platform, App.ProductName);
			}

			ApplePlatform [] platforms;

#if !NET
			if (currentPlatform == ApplePlatform.MacCatalyst) {
				// Fall back to any iOS attributes if we can't find something for Mac Catalyst
				platforms = new ApplePlatform [] {
					currentPlatform,
					ApplePlatform.iOS,
				};

			} else {
				platforms = new ApplePlatform [] {
					currentPlatform,
				};
			}
#else
			platforms = new ApplePlatform [] {
				currentPlatform,
			};
#endif

			foreach (var platform in platforms) {
				foreach (var ca in attributes) {
					var caType = ca.AttributeType;
#if NET
					if (!caType.Is ("System.Runtime.Versioning", "SupportedOSPlatformAttribute"))
						continue;
					if (GetDotNetAvailabilityAttribute (ca, platform, out sdkVersion, out message))
						return true;
#else
					if (caType.Namespace != ObjCRuntime && !string.IsNullOrEmpty (caType.Namespace))
						continue;
					if (GetLegacyAvailabilityAttribute (ca, platform, out sdkVersion, out message))
						return true;
#endif
				}
			}

			return false;
		}

		protected override Version GetSdkIntroducedVersion (TypeReference obj, out string message)
		{
			TypeDefinition td = obj.Resolve ();

			message = null;

			if (td is null)
				return null;

			if (td.HasCustomAttributes) {
				if (CollectAvailabilityAttributes (td.CustomAttributes, out var sdkVersion, out message))
					return sdkVersion;
			}

			if (AvailabilityAnnotations is not null && AvailabilityAnnotations.TryGetValue (td, out var attribObjects)) {
				if (CollectAvailabilityAttributes ((IEnumerable<ICustomAttribute>) attribObjects, out var sdkVersion, out message))
					return sdkVersion;
			}

			return null;
		}

		protected override Version GetSDKVersion ()
		{
			return App.SdkVersion;
		}

		protected override Dictionary<MethodDefinition, List<MethodDefinition>> PrepareMethodMapping (TypeReference type)
		{
			return PrepareInterfaceMethodMapping (type);
		}

		public TypeReference GetProtocolAttributeWrapperType (TypeDefinition type)
		{
			return GetProtocolAttributeWrapperType ((TypeReference) type);
		}

		public static TypeReference GetProtocolAttributeWrapperType (ICustomAttribute attrib)
		{
			if (!attrib.HasProperties)
				return null;

			foreach (var prop in attrib.Properties) {
				if (prop.Name == "WrapperType")
					return (TypeReference) prop.Argument.Value;
			}

			return null;
		}

		protected override TypeReference GetProtocolAttributeWrapperType (TypeReference type)
		{
			if (!TryGetAttribute (type.Resolve (), Foundation, StringConstants.ProtocolAttribute, out var attrib))
				return null;

			return GetProtocolAttributeWrapperType (attrib);
		}

		protected override IList<AdoptsAttribute> GetAdoptsAttributes (TypeReference type)
		{
			var attributes = GetCustomAttributes (type.Resolve (), ObjCRuntime, "AdoptsAttribute");
			if (attributes is null || !attributes.Any ())
				return null;

			var rv = new List<AdoptsAttribute> ();
			foreach (var ca in attributes) {
				var attrib = new AdoptsAttribute ();
				switch (ca.ConstructorArguments.Count) {
				case 1:
					attrib.ProtocolType = (string) ca.ConstructorArguments [0].Value;
					break;
				default:
					throw ErrorHelper.CreateError (4124, Errors.MT4124_B, type.FullName, 1, ca.ConstructorArguments.Count);
				}
				rv.Add (attrib);
			}

			return rv;
		}

		NativeNameAttribute GetNativeNameAttribute (TypeReference type)
		{
			if (!TryGetAttribute ((ICustomAttributeProvider) type, ObjCRuntime, "NativeNameAttribute", out var attrib))
				return null;

			return CreateNativeNameAttribute (attrib, type);
		}

		static NativeNameAttribute CreateNativeNameAttribute (ICustomAttribute attrib, TypeReference type)
		{
			if (attrib.HasFields)
				throw ErrorHelper.CreateError (4124, Errors.MT4124_I, "NativeNameAttribute", type.FullName);

			switch (attrib.ConstructorArguments.Count) {
			case 1:
				var t1 = (string) attrib.ConstructorArguments [0].Value;
				return new NativeNameAttribute (t1);
			default:
				throw ErrorHelper.CreateError (4124, Errors.MT4124_I, "NativeNameAttribute", type.FullName);
			}
		}

		public override BindAsAttribute GetBindAsAttribute (PropertyDefinition property)
		{
			if (property is null)
				return null;

			property = GetBasePropertyInTypeHierarchy (property);

			if (!TryGetAttribute (property, ObjCRuntime, "BindAsAttribute", out var attrib))
				return null;

			return CreateBindAsAttribute (attrib, property);
		}

		public override BindAsAttribute GetBindAsAttribute (MethodDefinition method, int parameter_index)
		{
			if (method is null)
				return null;

			method = GetBaseMethodInTypeHierarchy (method);

			if (!TryGetAttribute (parameter_index == -1 ? (ICustomAttributeProvider) method.MethodReturnType : method.Parameters [parameter_index], ObjCRuntime, "BindAsAttribute", out var attrib))
				return null;

			return CreateBindAsAttribute (attrib, method);
		}

		static BindAsAttribute CreateBindAsAttribute (ICustomAttribute attrib, IMemberDefinition member)
		{
			TypeReference originalType = null;
			if (attrib.HasFields) {
				foreach (var field in attrib.Fields) {
					switch (field.Name) {
					case "OriginalType":
						originalType = ((TypeReference) field.Argument.Value);
						break;
					default:
						throw ErrorHelper.CreateError (4124, Errors.MT4124_C, member.DeclaringType.FullName, member.Name, field.Name);
					}
				}
			}

			switch (attrib.ConstructorArguments.Count) {
			case 1:
				var t1 = (TypeReference) attrib.ConstructorArguments [0].Value;
				return new BindAsAttribute (t1) { OriginalType = originalType };
			default:
				throw ErrorHelper.CreateError (4124, Errors.MT4124_D, "BindAsAttribute", member.DeclaringType.FullName, member.Name);
			}
		}

		public override TypeReference GetNullableType (TypeReference type)
		{
			var git = type as GenericInstanceType;
			if (git is null)
				return null;
			if (!git.GetElementType ().Is ("System", "Nullable`1"))
				return null;
			return git.GenericArguments [0];
		}

		protected override ConnectAttribute GetConnectAttribute (PropertyDefinition property)
		{
			ICustomAttribute attrib;

			if (!TryGetAttribute (property, Foundation, StringConstants.ConnectAttribute, out attrib))
				return null;

			if (!attrib.HasConstructorArguments)
				return new ConnectAttribute ();

			switch (attrib.ConstructorArguments.Count) {
			case 0: return new ConnectAttribute ();
			case 1: return new ConnectAttribute (((string) attrib.ConstructorArguments [0].Value));
			default:
				throw ErrorHelper.CreateError (4124, Errors.MT4124_D, "ConnectAttribute", property.DeclaringType.FullName, property.Name);
			}
		}

		public static ExportAttribute CreateExportAttribute (IMemberDefinition candidate)
		{
			bool is_variadic = false;
			var attribute = GetExportAttribute (candidate);

			if (attribute is null)
				return null;

			if (attribute.HasProperties) {
				foreach (var prop in attribute.Properties) {
					if (prop.Name == "IsVariadic") {
						is_variadic = (bool) prop.Argument.Value;
						break;
					}
				}
			}

			if (!attribute.HasConstructorArguments)
				return new ExportAttribute (null) { IsVariadic = is_variadic };

			switch (attribute.ConstructorArguments.Count) {
			case 0:
				return new ExportAttribute (null) { IsVariadic = is_variadic };
			case 1:
				return new ExportAttribute ((string) attribute.ConstructorArguments [0].Value) { IsVariadic = is_variadic };
			case 2:
				return new ExportAttribute ((string) attribute.ConstructorArguments [0].Value, (ArgumentSemantic) attribute.ConstructorArguments [1].Value) { IsVariadic = is_variadic };
			default:
				throw ErrorHelper.CreateError (4124, Errors.MT4124, "ExportAttribute", $"{candidate.DeclaringType.FullName}.{candidate.Name}");
			}
		}

		// [Export] is not sealed anymore - so we cannot simply compare strings
		public static ICustomAttribute GetExportAttribute (ICustomAttributeProvider candidate)
		{
			if (!candidate.HasCustomAttributes)
				return null;

			foreach (CustomAttribute ca in candidate.CustomAttributes) {
				if (ca.Constructor.DeclaringType.Inherits (Foundation, StringConstants.ExportAttribute))
					return ca;
			}
			return null;
		}

		PropertyDefinition GetBasePropertyInTypeHierarchy (PropertyDefinition property)
		{
			if (!IsOverride (property))
				return property;

			var @base = GetBaseType (property.DeclaringType);
			while (@base is not null) {
				PropertyDefinition base_property = TryMatchProperty (@base.Resolve (), property);
				if (base_property is not null)
					return GetBasePropertyInTypeHierarchy (base_property) ?? base_property;

				@base = GetBaseType (@base);
			}

			return property;
		}

		static PropertyDefinition TryMatchProperty (TypeDefinition type, PropertyDefinition property)
		{
			if (!type.HasProperties)
				return null;

			foreach (PropertyDefinition candidate in type.Properties)
				if (PropertyMatch (candidate, property))
					return candidate;

			return null;
		}

		static bool PropertyMatch (PropertyDefinition candidate, PropertyDefinition property)
		{
			if (candidate.Name != property.Name)
				return false;

			if (candidate.GetMethod is not null) {
				if (property.GetMethod is null)
					return false;
				if (!MethodMatch (candidate.GetMethod, property.GetMethod))
					return false;
			} else if (property.GetMethod is not null) {
				return false;
			}

			if (candidate.SetMethod is not null) {
				if (property.SetMethod is null)
					return false;
				if (!MethodMatch (candidate.SetMethod, property.SetMethod))
					return false;
			} else if (property.SetMethod is not null) {
				return false;
			}

			return true;
		}

		public MethodDefinition GetBaseMethodInTypeHierarchy (MethodDefinition method)
		{
			if (!IsOverride (method))
				return method;

			var @base = GetBaseType (method.DeclaringType);
			while (@base is not null) {
				MethodDefinition base_method = TryMatchMethod (@base.Resolve (), method);
				if (base_method is not null)
					return GetBaseMethodInTypeHierarchy (base_method) ?? base_method;

				@base = GetBaseType (@base);
			}

			return method;
		}

		static MethodDefinition TryMatchMethod (TypeDefinition type, MethodDefinition method)
		{
			if (!type.HasMethods)
				return null;

			foreach (MethodDefinition candidate in type.Methods)
				if (MethodMatch (candidate, method))
					return candidate;

			return null;
		}

		// here we try to create a specialized trampoline for the specified method.
		static int counter = 0;
		static bool trace = false;
		AutoIndentStringBuilder header;
		AutoIndentStringBuilder declarations; // forward declarations, struct definitions
		AutoIndentStringBuilder methods; // c methods that contain the actual implementations
		AutoIndentStringBuilder interfaces; // public objective-c @interface declarations
		AutoIndentStringBuilder nslog_start = new AutoIndentStringBuilder ();
		AutoIndentStringBuilder nslog_end = new AutoIndentStringBuilder ();

		AutoIndentStringBuilder comment = new AutoIndentStringBuilder ();
		AutoIndentStringBuilder copyback = new AutoIndentStringBuilder ();
		AutoIndentStringBuilder invoke = new AutoIndentStringBuilder ();
		AutoIndentStringBuilder setup_call_stack = new AutoIndentStringBuilder ();
		AutoIndentStringBuilder setup_return = new AutoIndentStringBuilder ();
		AutoIndentStringBuilder cleanup = new AutoIndentStringBuilder ();
		AutoIndentStringBuilder body = new AutoIndentStringBuilder ();
		AutoIndentStringBuilder body_setup = new AutoIndentStringBuilder ();

		HashSet<string> trampoline_names = new HashSet<string> ();
		HashSet<string> namespaces = new HashSet<string> ();
		HashSet<string> structures = new HashSet<string> ();

		Dictionary<Body, Body> bodies = new Dictionary<Body, Body> ();

		AutoIndentStringBuilder full_token_references = new AutoIndentStringBuilder ();
		uint full_token_reference_count;
		List<(AssemblyDefinition Assembly, string Name)> registered_assemblies = new List<(AssemblyDefinition Assembly, string Name)> ();

		bool IsPlatformType (TypeReference type)
		{
			if (type.IsNested)
				return false;

			string aname;
			if (type.Module is null) {
				// This type was probably linked away
				if (LinkContext.GetLinkedAwayType (type, out var module) is not null) {
					aname = module.Assembly.Name.Name;
				} else {
					aname = string.Empty;
				}
			} else {
				aname = type.Module.Assembly.Name.Name;
			}

			if (aname != PlatformAssembly)
				return false;

			return Driver.GetFrameworks (App).ContainsKey (type.Namespace);
		}

		static bool IsLinkedAway (TypeReference tr)
		{
			return tr.Module is null;
		}

		void CheckNamespace (ObjCType objctype, List<Exception> exceptions)
		{
			CheckNamespace (objctype.Type, exceptions);
		}

		HashSet<string> reported_frameworks;
		void CheckNamespace (TypeReference type, List<Exception> exceptions)
		{
			if (!IsPlatformType (type))
				return;

			var ns = type.Namespace;

#if !XAMCORE_5_0
			// AVCustomRoutingControllerDelegate was incorrectly placed in AVKit
			if (type.Is ("AVKit", "AVCustomRoutingControllerDelegate"))
				ns = "AVRouting";
#endif
			Framework framework;
			if (Driver.GetFrameworks (App).TryGetValue (ns, out framework)) {
				if (framework.Version > App.SdkVersion) {
					if (reported_frameworks is null)
						reported_frameworks = new HashSet<string> ();
					if (!reported_frameworks.Contains (framework.Name)) {
						exceptions.Add (ErrorHelper.CreateError (4134,
							App.Platform == ApplePlatform.MacOSX ? Errors.MM4134 : Errors.MT4134,
							framework.Name, App.SdkVersion, framework.Version, App.PlatformName));
						reported_frameworks.Add (framework.Name);
					}
					return;
				}
			}

			CheckNamespace (ns, exceptions);
		}

		void CheckNamespace (string ns, List<Exception> exceptions)
		{
			if (string.IsNullOrEmpty (ns))
				return;

			if (namespaces.Contains (ns))
				return;

			namespaces.Add (ns);

			if (App.IsSimulatorBuild && !App.IsFrameworkAvailableInSimulator (ns)) {
				Driver.Log (5, "Not importing the framework {0} in the generated registrar code because it's not available in the simulator.", ns);
				return;
			} else if (Frameworks.GetFrameworks (App.Platform, false).TryGetValue (ns, out var fw) && fw.Unavailable) {
				Driver.Log (5, "Not importing the framework {0} in the generated registrar code because it's not available in the current platform.", ns);
				return;
			}

			string h;
			switch (ns) {
			case "CallKit":
				if (App.Platform == ApplePlatform.MacOSX) {
					// AVFoundation can't be imported before CallKit on macOS
					// Ref: https://github.com/xamarin/maccore/issues/2301
					// Ref: https://github.com/xamarin/maccore/issues/2257
					// The fun part is that other frameworks can import AVFoundation, so we can't check for AVFoundation specifically.
					// Instead add CallKit before any other imports.
					var firstImport = header.StringBuilder.ToString ().IndexOf ("#import <");
					if (firstImport >= 0) {
						header.StringBuilder.Insert (firstImport, "#import <CallKit/CallKit.h>\n");
						return;
					}
				}
				goto default;
#if !NET
			case "Chip":
				switch (App.Platform) {
				case ApplePlatform.iOS when App.SdkVersion.Major <= 15:
				case ApplePlatform.TVOS when App.SdkVersion.Major <= 15:
				case ApplePlatform.MacOSX when App.SdkVersion.Major <= 12:
				case ApplePlatform.WatchOS when App.SdkVersion.Major <= 8:
					h = "<CHIP/CHIP.h>";
					break;
				default:
					// The framework has been renamed.
					header.WriteLine ("@protocol CHIPDevicePairingDelegate <NSObject>");
					header.WriteLine ("@end");
					header.WriteLine ("@protocol CHIPKeypair <NSObject>");
					header.WriteLine ("@end");
					header.WriteLine ("@protocol CHIPPersistentStorageDelegate <NSObject>");
					header.WriteLine ("@end");
					break;
				}
				return;
#endif
			case "GLKit":
				// This prevents this warning:
				//     /Applications/Xcode83.app/Contents/Developer/Platforms/MacOSX.platform/Developer/SDKs/MacOSX10.12.sdk/System/Library/Frameworks/OpenGL.framework/Headers/gl.h:5:2: warning: gl.h and gl3.h are both
				//     included. Compiler will not invoke errors if using removed OpenGL functionality. [-W#warnings]
				// This warning shows up when both GLKit/GLKit.h and Quartz/Quartz.h are included.
				if (App.Platform == ApplePlatform.MacOSX)
					header.WriteLine ("#define GL_DO_NOT_WARN_IF_MULTI_GL_VERSION_HEADERS_INCLUDED 1");
				goto default;
			case "CoreBluetooth":
				if (App.Platform == ApplePlatform.MacOSX) {
					header.WriteLine ("#import <IOBluetooth/IOBluetooth.h>");
					header.WriteLine ("#import <CoreBluetooth/CoreBluetooth.h>");
					return;
				}
				goto default;
			case "ImageKit":
			case "QuartzComposer":
			case "QuickLookUI":
				h = "<Quartz/Quartz.h>";
				break;
			case "Phase":
				h = "<PHASE/PHASE.h>";
				break;
			case "PdfKit":
				h = App.Platform == ApplePlatform.MacOSX ? "<Quartz/Quartz.h>" : "<PDFKit/PDFKit.h>";
				break;
			case "OpenGLES":
				return;
			case "CoreAnimation":
				header.WriteLine ("#import <QuartzCore/QuartzCore.h>");
				switch (App.Platform) {
				case ApplePlatform.iOS:
				case ApplePlatform.TVOS:
					if (App.SdkVersion.Major > 7 && App.SdkVersion.Major < 11)
						header.WriteLine ("#import <QuartzCore/CAEmitterBehavior.h>");
					break;
				}
				return;
			case "CoreMidi":
				h = "<CoreMIDI/CoreMIDI.h>";
				break;
			case "CoreTelephony":
				// Grr, why doesn't CoreTelephony have one header that includes the rest !?
				header.WriteLine ("#import <CoreTelephony/CoreTelephonyDefines.h>");
				header.WriteLine ("#import <CoreTelephony/CTCall.h>");
				header.WriteLine ("#import <CoreTelephony/CTCallCenter.h>");
				header.WriteLine ("#import <CoreTelephony/CTCarrier.h>");
				header.WriteLine ("#import <CoreTelephony/CTTelephonyNetworkInfo.h>");
				if (App.SdkVersion.Major >= 7) {
					header.WriteLine ("#import <CoreTelephony/CTSubscriber.h>");
					header.WriteLine ("#import <CoreTelephony/CTSubscriberInfo.h>");
				}
				return;
			case "Accounts":
				if (App.Platform != ApplePlatform.MacOSX) {
					var compiler = Path.GetFileName (App.CompilerPath);
					if (compiler == "gcc" || compiler == "g++") {
						exceptions.Add (new ProductException (4121, true, "Cannot use GCC/G++ to compile the generated code from the static registrar when using the Accounts framework (the header files provided by Apple used during the compilation require Clang). Either use Clang (--compiler:clang) or the dynamic registrar (--registrar:dynamic)."));
						return;
					}
				}
				goto default;
#if !NET
			case "QTKit":
				if (App.Platform == ApplePlatform.MacOSX && App.SdkVersion >= MacOSTenTwelveVersion)
					return; // 10.12 removed the header files for QTKit
				goto default;
#endif
			case "IOSurface": // There is no IOSurface.h
				h = "<IOSurface/IOSurfaceObjC.h>";
				break;
			case "CoreImage":
				header.WriteLine ("#import <CoreImage/CoreImage.h>");
				header.WriteLine ("#import <CoreImage/CIFilterBuiltins.h>");
				return;
#if !NET
			case "iAd":
				if (App.SdkVersion.Major >= 13) {
					// most of the framework has been obliterated from the headers
					header.WriteLine ("@class ADBannerView;");
					header.WriteLine ("@class ADInterstitialAd;");
					header.WriteLine ("@protocol ADBannerViewDelegate <NSObject>");
					header.WriteLine ("@end");
					header.WriteLine ("@protocol ADInterstitialAdDelegate <NSObject>");
					header.WriteLine ("@end");
					return;
				}
				goto default;
#endif
			case "ThreadNetwork":
				h = "<ThreadNetwork/THClient.h>";
				break;
			default:
				h = string.Format ("<{0}/{0}.h>", ns);
				break;
			}
			header.WriteLine ("#import {0}", h);
		}

		string CheckStructure (TypeDefinition structure, string descriptiveMethodName, MemberReference inMember)
		{
			string n;
			StringBuilder name = new StringBuilder ();
			var body = new AutoIndentStringBuilder (1);
			int size = 0;

			ProcessStructure (name, body, structure, ref size, descriptiveMethodName, structure, inMember);

			n = "struct trampoline_struct_" + name.ToString ();
			if (!structures.Contains (n)) {
				structures.Add (n);
				declarations.WriteLine ($"// {structure.FullName} (+other structs with same layout)");
				declarations.WriteLine ("{0} {{\n{1}}};", n, body.ToString ());
			}

			return n;
		}

		void ProcessStructure (StringBuilder name, AutoIndentStringBuilder body, TypeDefinition structure, ref int size, string descriptiveMethodName, TypeDefinition root_structure, MemberReference inMember)
		{
			switch (structure.FullName) {
			case "System.Char":
				name.Append ('s');
				body.AppendLine ("short v{0};", size);
				size += 1;
				break;
			case "System.Boolean": // map managed 'bool' to ObjC BOOL
				name.Append ('B');
				body.AppendLine ("BOOL v{0};", size);
				size += 1;
				break;
			case "System.Byte":
			case "System.SByte":
				name.Append ('b');
				body.AppendLine ("char v{0};", size);
				size += 1;
				break;
			case "System.UInt16":
			case "System.Int16":
				name.Append ('s');
				body.AppendLine ("short v{0};", size);
				size += 2;
				break;
			case "System.UInt32":
			case "System.Int32":
				name.Append ('i');
				body.AppendLine ("int v{0};", size);
				size += 4;
				break;
			case "System.Int64":
			case "System.UInt64":
				name.Append ('l');
				body.AppendLine ("long long v{0};", size);
				size += 8;
				break;
			case "System.Single":
				name.Append ('f');
				body.AppendLine ("float v{0};", size);
				size += 4;
				break;
			case "System.Double":
				name.Append ('d');
				body.AppendLine ("double v{0};", size);
				size += 8;
				break;
			case "System.IntPtr":
			case "System.UIntPtr":
				name.Append ('p');
				body.AppendLine ("void *v{0};", size);
				size += Is64Bits ? 8 : 4;
				break;
			default:
				bool found = false;
				foreach (FieldDefinition field in structure.Fields) {
					if (field.IsStatic)
						continue;
					var fieldType = field.FieldType.Resolve ();
					if (fieldType is null)
						throw ErrorHelper.CreateError (App, 4111, inMember, Errors.MT4111, structure.FullName, descriptiveMethodName);
					if (!fieldType.IsValueType)
						throw ErrorHelper.CreateError (App, 4161, inMember, Errors.MT4161, root_structure.FullName, field.Name, fieldType.FullName);
					found = true;
					ProcessStructure (name, body, fieldType, ref size, descriptiveMethodName, root_structure, inMember);
				}
				if (!found)
					throw ErrorHelper.CreateError (App, 4111, inMember, Errors.MT4111, structure.FullName, descriptiveMethodName);
				break;
			}
		}

		string GetUniqueTrampolineName (string suggestion)
		{
			char [] fixup = suggestion.ToCharArray ();
			for (int i = 0; i < fixup.Length; i++) {
				char c = fixup [i];
				if (c >= 'a' && c <= 'z')
					continue;
				if (c >= 'A' && c <= 'Z')
					continue;
				if (c >= '0' && c <= '9')
					continue;
				fixup [i] = '_';
			}
			suggestion = new string (fixup);

			if (trampoline_names.Contains (suggestion)) {
				string tmp;
				int counter = 0;
				do {
					tmp = suggestion + (++counter).ToString ();
				} while (trampoline_names.Contains (tmp));
				suggestion = tmp;
			}

			trampoline_names.Add (suggestion);

			return suggestion;
		}

		// Some declarations can be generalized to NSObject for its subclasses
		// (and System.String too as we convert it into an NSString)
		// since the generated code, except the function signature, is identical anyway
		string ToSimpleObjCParameterType (TypeReference type, string descriptiveMethodName, List<Exception> exceptions, MemberReference inMethod)
		{
			var byref = type.IsByReference;
			var t = byref ? type.GetElementType () : type;
			if (t.Inherits ("Foundation", "NSObject") || t.Is ("System", "String"))
				return byref ? "id*" : "id";

			return ToObjCParameterType (type, descriptiveMethodName, exceptions, inMethod);
		}

		string ToObjCParameterType (TypeReference type, string descriptiveMethodName, List<Exception> exceptions, MemberReference inMethod, bool delegateToBlockType = false, bool cSyntaxForBlocks = false)
		{
			GenericParameter gp = type as GenericParameter;
			if (gp is not null)
				return "id";

			var reftype = type as ByReferenceType;
			if (reftype is not null) {
				string res = ToObjCParameterType (GetElementType (reftype), descriptiveMethodName, exceptions, inMethod);
				if (res is null)
					return null;
				return res + "*";
			}

			if (type is PointerType pt)
				return ToObjCParameterType (pt.ElementType, descriptiveMethodName, exceptions, inMethod, delegateToBlockType) + "*";

			ArrayType arrtype = type as ArrayType;
			if (arrtype is not null)
				return "NSArray *";

			var git = type as GenericInstanceType;
			if (git is not null && IsNSObject (type)) {
				var sb = new StringBuilder ();
				var elementType = git.GetElementType ();

				sb.Append (ToObjCParameterType (elementType, descriptiveMethodName, exceptions, inMethod));

				if (sb [sb.Length - 1] != '*') {
					// I'm not sure if this is possible to hit (I couldn't come up with a test case), but better safe than sorry.
					AddException (ref exceptions, CreateException (4166, inMethod.Resolve () as MethodDefinition, "Cannot register the method '{0}' because the signature contains a type ({1}) that isn't a reference type.", descriptiveMethodName, GetTypeFullName (elementType)));
					return "id";
				}

				sb.Length--; // remove the trailing * of the element type

				sb.Append ('<');
				for (int i = 0; i < git.GenericArguments.Count; i++) {
					if (i > 0)
						sb.Append (", ");
					var argumentType = git.GenericArguments [i];
					if (!IsINativeObject (argumentType)) {
						// I believe the generic constraints we have should make this error impossible to hit, but better safe than sorry.
						AddException (ref exceptions, CreateException (4167, inMethod.Resolve () as MethodDefinition, "Cannot register the method '{0}' because the signature contains a generic type ({1}) with a generic argument type that doesn't implement INativeObject ({2}).", descriptiveMethodName, GetTypeFullName (type), GetTypeFullName (argumentType)));
						return "id";
					}
					sb.Append (ToObjCParameterType (argumentType, descriptiveMethodName, exceptions, inMethod));
				}
				sb.Append ('>');

				sb.Append ('*'); // put back the * from the element type

				return sb.ToString ();
			}

			switch (type.FullName) {
			case "System.Drawing.RectangleF": return App.Platform == ApplePlatform.MacOSX ? "NSRect" : "CGRect";
			case "System.Drawing.PointF": return App.Platform == ApplePlatform.MacOSX ? "NSPoint" : "CGPoint";
			case "System.Drawing.SizeF": return App.Platform == ApplePlatform.MacOSX ? "NSSize" : "CGSize";
			case "System.String": return "NSString *";
			case "System.UIntPtr":
			case "System.IntPtr": return "void *";
			case "System.SByte": return "signed char";
			case "System.Byte": return "unsigned char";
			case "System.Char": return "signed short";
			case "System.Int16": return "short";
			case "System.UInt16": return "unsigned short";
			case "System.Int32": return "int";
			case "System.UInt32": return "unsigned int";
			case "System.Int64": return "long long";
			case "System.UInt64": return "unsigned long long";
			case "System.Single": return "float";
			case "System.Double": return "double";
			case "System.Boolean": return "BOOL"; // map managed 'bool' to ObjC BOOL = unsigned char
			case "System.Void": return "void";
			case "System.nint":
				CheckNamespace ("Foundation", exceptions);
				return "NSInteger";
			case "System.nuint":
				CheckNamespace ("Foundation", exceptions);
				return "NSUInteger";
			case "System.DateTime":
				throw ErrorHelper.CreateError (4102, Errors.MT4102, "System.DateTime", "Foundation.NSDate", descriptiveMethodName);
			case "ObjCRuntime.Selector": return "SEL";
			case "ObjCRuntime.Class": return "Class";
			case "ObjCRuntime.NativeHandle": return "void *";
			default:
				if (type.FullName == NFloatTypeName) {
					CheckNamespace ("CoreGraphics", exceptions);
					return "CGFloat";
				}
				TypeDefinition td = ResolveType (type);
				if (IsNSObject (td)) {
					if (!IsPlatformType (td))
						return "id";

					if (HasProtocolAttribute (td)) {
						return "id<" + GetExportedTypeName (td) + ">";
					} else {
						return GetExportedTypeName (td) + " *";
					}
				} else if (td.IsEnum) {
					if (HasAttribute (td, ObjCRuntime, StringConstants.NativeAttribute)) {
						switch (GetEnumUnderlyingType (td).FullName) {
						case "System.Int64":
							return "NSInteger";
						case "System.UInt64":
							return "NSUInteger";
						default:
							exceptions.Add (ErrorHelper.CreateError (4145, Errors.MT4145, td.FullName));
							return "NSInteger";
						}
					}

					return ToObjCParameterType (GetEnumUnderlyingType (td), descriptiveMethodName, exceptions, inMethod);
				} else if (td.IsValueType) {
					if (IsPlatformType (td)) {
						CheckNamespace (td, exceptions);
						return GetNativeName (td);
					}
					return CheckStructure (td, descriptiveMethodName, inMethod);
				} else {
					return ToObjCType (td, delegateToBlockType: delegateToBlockType, cSyntaxForBlocks: cSyntaxForBlocks);
				}
			}
		}

		string GetNativeName (TypeDefinition type)
		{
			var attrib = GetNativeNameAttribute (type);
			if (attrib is null)
				return type.Name;

			return attrib.NativeName;
		}

		string GetPrintfFormatSpecifier (TypeDefinition type, out bool unknown)
		{
			unknown = false;
			if (type.IsValueType) {
				switch (type.FullName) {
				case "System.Char": return "c";
				case "System.Boolean":
				case "System.SByte":
				case "System.Int16":
				case "System.Int32": return "i";
				case "System.Byte":
				case "System.UInt16":
				case "System.UInt32": return "u";
				case "System.Int64": return "lld";
				case "System.UInt64": return "llu";
				case "System.nint": return "zd";
				case "System.nuint": return "tu";
				case "System.Single":
				case "System.Double": return "f";
				default:
					if (type.FullName == NFloatTypeName)
						return "f";
					unknown = true;
					return "p";
				}
			} else if (IsNSObject (type)) {
				return "@";
			} else {
				unknown = true;
				return "p";
			}
		}

		string GetObjCSignature (ObjCMethod method, List<Exception> exceptions)
		{
			if (method.CurrentTrampoline == Trampoline.Retain)
				return "-(id) retain";
			else if (method.CurrentTrampoline == Trampoline.Release)
				return "-(void) release";
			else if (method.CurrentTrampoline == Trampoline.GetGCHandle)
				return "-(GCHandle) xamarinGetGCHandle";
			else if (method.CurrentTrampoline == Trampoline.GetFlags)
				return "-(enum XamarinGCHandleFlags) xamarinGetFlags";
			else if (method.CurrentTrampoline == Trampoline.SetFlags)
				return "-(void) xamarinSetFlags: (enum XamarinGCHandleFlags) flags";
			else if (method.CurrentTrampoline == Trampoline.SetGCHandle)
				return "-(bool) xamarinSetGCHandle: (GCHandle) gchandle flags: (enum XamarinGCHandleFlags) flags";
			else if (method.CurrentTrampoline == Trampoline.CopyWithZone1 || method.CurrentTrampoline == Trampoline.CopyWithZone2)
				return "-(id) copyWithZone: (NSZone *)zone";

			var sb = new StringBuilder ();
			var isCtor = method.CurrentTrampoline == Trampoline.Constructor;

			sb.Append ((method.IsStatic && !method.IsCategoryInstance) ? '+' : '-');
			sb.Append ('(');
			sb.Append (isCtor ? "id" : this.ToObjCParameterType (method.NativeReturnType, GetDescriptiveMethodName (method.Method), exceptions, method.Method));
			sb.Append (')');

			var split = method.Selector.Split (':');

			if (split.Length == 1) {
				sb.Append (' ');
				sb.Append (split [0]);
			} else {
				var indexOffset = method.IsCategoryInstance ? 1 : 0;
				for (int i = 0; i < split.Length - 1; i++) {
					sb.Append (' ');
					sb.Append (split [i]);
					sb.Append (':');
					sb.Append ('(');
					sb.Append (ToObjCParameterType (method.NativeParameters [i + indexOffset], method.DescriptiveMethodName, exceptions, method.Method, delegateToBlockType: true));
					sb.Append (')');
					sb.AppendFormat ("p{0}", i);
				}
			}

			if (method.IsVariadic)
				sb.Append (", ...");

			return sb.ToString ();
		}

		void WriteFullName (StringBuilder sb, TypeReference type)
		{
			if (type.DeclaringType is not null) {
				WriteFullName (sb, type.DeclaringType);
				sb.Append ('+');
			} else if (!string.IsNullOrEmpty (type.Namespace)) {
				sb.Append (type.Namespace);
				sb.Append ('.');
			}
			sb.Append (type.Name);
		}

		protected override string GetAssemblyQualifiedName (TypeReference type)
		{
			var gp = type as GenericParameter;
			if (gp is not null)
				return gp.Name;

			var sb = new StringBuilder ();

			WriteFullName (sb, type);

			var git = type as GenericInstanceType;
			if (git is not null) {
				sb.Append ('[');
				for (int i = 0; i < git.GenericArguments.Count; i++) {
					if (i > 0)
						sb.Append (',');
					sb.Append ('[');
					sb.Append (GetAssemblyQualifiedName (git.GenericArguments [i]));
					sb.Append (']');
				}
				sb.Append (']');
			}

			var td = type.Resolve ();
			if (td is not null)
				sb.Append (", ").Append (td.Module.Assembly.Name.Name);

			return sb.ToString ();
		}

		public static string EncodeNonAsciiCharacters (string value)
		{
			StringBuilder sb = null;
			for (int i = 0; i < value.Length; i++) {
				char c = value [i];
				if (c > 127) {
					if (sb is null) {
						sb = new StringBuilder (value.Length);
						sb.Append (value, 0, i);
					}
					sb.Append ("\\u");
					sb.Append (((int) c).ToString ("x4"));
				} else if (sb is not null) {
					sb.Append (c);
				}
			}
			return sb is not null ? sb.ToString () : value;
		}

		static bool IsTypeCore (ObjCType type, string nsToMatch)
		{
			var ns = type.Type.Namespace;

			var t = type.Type;
			while (string.IsNullOrEmpty (ns) && t.DeclaringType is not null) {
				t = t.DeclaringType;
				ns = t.Namespace;
			}

			return ns == nsToMatch;
		}

#if !NET
		static bool IsQTKitType (ObjCType type) => IsTypeCore (type, "QTKit");
#endif
		static bool IsMapKitType (ObjCType type) => IsTypeCore (type, "MapKit");
		static bool IsIntentsType (ObjCType type) => IsTypeCore (type, "Intents");
		static bool IsExternalAccessoryType (ObjCType type) => IsTypeCore (type, "ExternalAccessory");

		bool IsTypeAllowedInSimulator (ObjCType type)
		{
			var ns = type.Type.Namespace;
			return App.IsFrameworkAvailableInSimulator (ns);
		}

		class ProtocolInfo {
			public uint TokenReference;
			public ObjCType Protocol;
		}

		public class SkippedType {
			public TypeReference Skipped;
			public ObjCType Actual;
			public uint SkippedTokenReference;
			public uint ActualTokenReference;
		}
		List<SkippedType> skipped_types = new List<SkippedType> ();
		protected override void OnSkipType (TypeReference type, ObjCType registered_type)
		{
			base.OnSkipType (type, registered_type);
			skipped_types.Add (new SkippedType { Skipped = type, Actual = registered_type });
		}

		public List<SkippedType> SkippedTypes {
			get => skipped_types;
		}

		public string GetInitializationMethodName (string single_assembly)
		{
			if (!string.IsNullOrEmpty (single_assembly)) {
				return "xamarin_create_classes_" + single_assembly.Replace ('.', '_').Replace ('-', '_');
			} else {
				return "xamarin_create_classes";
			}
		}

		List<ObjCType>? all_types = null;
		List<ObjCType> GetAllTypes (List<Exception> exceptions)
		{
			if (all_types is not null)
				return all_types;
			var allTypes = new List<ObjCType> ();
			foreach (var @class in Types.Values) {
				if (!string.IsNullOrEmpty (single_assembly) && single_assembly != @class.Type.Module.Assembly.Name.Name)
					continue;

				if (App.Platform != ApplePlatform.MacOSX) {
					var isPlatformType = IsPlatformType (@class.Type);
					if (isPlatformType && IsSimulatorOrDesktop && !IsTypeAllowedInSimulator (@class)) {
						Driver.Log (5, "The static registrar won't generate code for {0} because its framework is not supported in the simulator.", @class.ExportedName);
						continue; // Some types are not supported in the simulator.
					}
				} else {
#if !NET
					if (IsQTKitType (@class) && App.SdkVersion >= MacOSTenTwelveVersion)
						continue; // QTKit header was removed in 10.12 SDK
#endif
				}

#if !NET
				// Xcode 11 removed WatchKit for iOS!
				if (IsTypeCore (@class, "WatchKit") && App.Platform == Xamarin.Utils.ApplePlatform.iOS) {
					exceptions.Add (ErrorHelper.CreateWarning (4178, $"The class '{@class.Type.FullName}' will not be registered because the WatchKit framework has been removed from the iOS SDK."));
					continue;
				}
#endif

				// Xcode 15 removed NewsstandKit
				if (Driver.XcodeVersion.Major >= 15) {
					if (IsTypeCore (@class, "NewsstandKit")) {
						exceptions.Add (ErrorHelper.CreateWarning (4178, $"The class '{@class.Type.FullName}' will not be registered because the NewsstandKit framework has been removed from the {App.Platform} SDK."));
						continue;
					}

					if (@class.Type.Is ("PassKit", "PKDisbursementAuthorizationControllerDelegate") || @class.Type.Is ("PassKit", "IPKDisbursementAuthorizationControllerDelegate")) {
						exceptions.Add (ErrorHelper.CreateWarning (4189, $"The class '{@class.Type.FullName}' will not be registered it has been removed from the {App.Platform} SDK."));
						continue;
					}

					if (@class.Type.Is ("PassKit", "PKDisbursementAuthorizationController")) {
						exceptions.Add (ErrorHelper.CreateWarning (4189, $"The class '{@class.Type.FullName}' will not be registered it has been removed from the {App.Platform} SDK."));
						continue;
					}
				}

				if (@class.IsFakeProtocol)
					continue;

				allTypes.Add (@class);
			}
			all_types = allTypes;
			return all_types;
		}

		CSToObjCMap type_map_dictionary;
		public CSToObjCMap GetTypeMapDictionary (List<Exception> exceptions)
		{
			if (type_map_dictionary is not null)
				return type_map_dictionary;

			var allTypes = GetAllTypes (exceptions);
			var map_dict = new CSToObjCMap ();

			foreach (var @class in allTypes) {
				if (!@class.IsProtocol && !@class.IsCategory) {
					var name = GetAssemblyQualifiedName (@class.Type);
					@class.ClassMapIndex = map_dict.Count;
					map_dict [name] = new ObjCNameIndex (@class.ExportedName, @class.ClassMapIndex);
				}
			}

			type_map_dictionary = map_dict;
			return type_map_dictionary;
		}

		public void Rewrite ()
		{
#if NET
			if (App.Optimizations.RedirectClassHandles == true) {
				var exceptions = new List<Exception> ();
				var map_dict = GetTypeMapDictionary (exceptions);
				var rewriter = new Rewriter (map_dict, GetAssemblies (), LinkContext);
				var result = rewriter.Process ();
				if (!string.IsNullOrEmpty (result)) {
					Driver.Log (5, $"Not redirecting class handles because {result}");
				}
				ErrorHelper.ThrowIfErrors (exceptions);
			}
#endif
		}

		void Specialize (AutoIndentStringBuilder sb, out string initialization_method)
		{
			List<Exception> exceptions = new List<Exception> ();
			List<ObjCMember> skip = new List<ObjCMember> ();

			var map = new AutoIndentStringBuilder (1);
			var map_init = new AutoIndentStringBuilder ();
			var map_dict = new CSToObjCMap (); // maps CS type to ObjC type name and index
			var protocol_wrapper_map = new Dictionary<uint, Tuple<ObjCType, uint>> ();
			var protocols = new List<ProtocolInfo> ();

			var i = 0;

			bool needs_protocol_map = false;
			// Check if we need the protocol map.
			// We don't need it if the linker removed the method ObjCRuntime.Runtime.GetProtocolForType,
			// or if we're not registering protocols.
			if (App.Optimizations.RegisterProtocols == true) {
				var asm = input_assemblies.FirstOrDefault ((v) => v.Name.Name == PlatformAssembly);
				needs_protocol_map = asm?.MainModule.GetType ("ObjCRuntime", "Runtime")?.Methods.Any ((v) => v.Name == "GetProtocolForType") == true;
			}

			map.AppendLine ("static MTClassMap __xamarin_class_map [] = {");

			initialization_method = GetInitializationMethodName (single_assembly);
			map_init.AppendLine ($"void {initialization_method} () {{");

			// Select the types that needs to be registered.
			var allTypes = GetAllTypes (exceptions);

			if (string.IsNullOrEmpty (single_assembly)) {
				foreach (var assembly in GetAssemblies ())
					registered_assemblies.Add (new (assembly, GetAssemblyName (assembly)));
			} else {
				registered_assemblies.Add (new (GetAssemblies ().Single (v => GetAssemblyName (v) == single_assembly), single_assembly));
			}

			// Don't need this dictionary, but do need ClassMapIndex
			GetTypeMapDictionary (exceptions);

			foreach (var @class in allTypes) {
				var isPlatformType = IsPlatformType (@class.Type);
				var flags = MTTypeFlags.None;

				skip.Clear ();

				uint token_ref = uint.MaxValue;
				if (!@class.IsProtocol && !@class.IsCategory) {
					if (!isPlatformType)
						flags |= MTTypeFlags.CustomType;

					if (!@class.IsWrapper && !@class.IsModel)
						flags |= MTTypeFlags.UserType;

					CheckNamespace (@class, exceptions);
					if (!TryCreateTokenReference (@class.Type, TokenType.TypeDef, out token_ref, exceptions))
						continue;
					map.AppendLine ("{{ NULL, 0x{1:X} /* #{3} '{0}' => '{2}' */, (MTTypeFlags) ({4}) /* {5} */ }},",
									@class.ExportedName,
									token_ref,
									GetAssemblyQualifiedName (@class.Type), @class.ClassMapIndex,
									(int) flags, flags);

					bool use_dynamic;

					if (@class.Type.Resolve ().Module.Assembly.Name.Name == PlatformAssembly) {
						// we don't need to use the static ref to prevent the linker from removing (otherwise unreferenced) code for monotouch.dll types.
						use_dynamic = true;
						// be smarter: we don't need to use dynamic refs for types available in the lowest version (target deployment) we building for.
						// We do need to use dynamic class lookup when the following conditions are all true:
						// * The class is not available in the target deployment version.
						// * The class is not in a weakly linked framework (for instance if an existing framework introduces a new class, we don't
						//   weakly link the framework because it already exists in the target deployment version - but since the class doesn't, we
						//   must use dynamic class lookup to determine if it's available or not.
					} else {
						use_dynamic = false;
					}

					switch (@class.ExportedName) {
					case "EKObject":
						// EKObject's class is a private symbol, so we can't link with it...
						use_dynamic = true;
						break;
					}

					string get_class;
					if (use_dynamic) {
						get_class = string.Format ("objc_getClass (\"{0}\")", @class.ExportedName);
					} else {
						get_class = string.Format ("[{0} class]", EncodeNonAsciiCharacters (@class.ExportedName));
					}

					map_init.AppendLine ("__xamarin_class_map [{1}].handle = {0};", get_class, @class.ClassMapIndex);
					if (App.Optimizations.RedirectClassHandles == true)
						map_init.AppendLine ("__xamarin_class_handles [{0}] = __xamarin_class_map [{0}].handle;", @class.ClassMapIndex);
					i++;
				}


				if (@class.IsProtocol && @class.ProtocolWrapperType is not null) {
					if (token_ref == INVALID_TOKEN_REF && !TryCreateTokenReference (@class.Type, TokenType.TypeDef, out token_ref, exceptions))
						continue;
					if (!TryCreateTokenReference (@class.ProtocolWrapperType, TokenType.TypeDef, out var protocol_wrapper_type_ref, exceptions))
						continue;
					protocol_wrapper_map.Add (token_ref, new Tuple<ObjCType, uint> (@class, protocol_wrapper_type_ref));
					if (needs_protocol_map || TryGetAttribute (@class.Type, "Foundation", "XpcInterfaceAttribute", out var xpcAttr)) {
						protocols.Add (new ProtocolInfo { TokenReference = token_ref, Protocol = @class });
						CheckNamespace (@class, exceptions);
					}
				}
				if (@class.IsWrapper && isPlatformType)
					continue;

				if (@class.Methods is null && isPlatformType && !@class.IsProtocol && !@class.IsCategory)
					continue;

				CheckNamespace (@class, exceptions);
				if (@class.BaseType is not null)
					CheckNamespace (@class.BaseType, exceptions);

				var class_name = EncodeNonAsciiCharacters (@class.ExportedName);
				var is_protocol = @class.IsProtocol;

				// Publicly visible types should go into the header, private types go into the .m
				var td = @class.Type.Resolve ();
				AutoIndentStringBuilder iface;
				if (td.IsNotPublic || td.IsNestedPrivate || td.IsNestedAssembly || td.IsNestedFamilyAndAssembly) {
					iface = sb;
				} else {
					iface = interfaces;
				}

				if (@class.IsCategory) {
					var exportedName = EncodeNonAsciiCharacters (@class.BaseType.ExportedName);
					iface.Write ("@interface {0} ({1})", exportedName, @class.CategoryName);
					declarations.AppendFormat ("@class {0};\n", exportedName);
				} else if (is_protocol) {
					var exportedName = EncodeNonAsciiCharacters (@class.ProtocolName);
					iface.Write ("@protocol ").Write (exportedName);
					declarations.AppendFormat ("@protocol {0};\n", exportedName);
				} else {
					iface.Write ("@interface {0} : {1}", class_name, EncodeNonAsciiCharacters (@class.SuperType.ExportedName));
					declarations.AppendFormat ("@class {0};\n", class_name);
				}
				var implementedProtocols = new HashSet<string> ();
				ObjCType tp = @class;
				while (tp is not null && tp != tp.BaseType) {
					if (tp.IsWrapper)
						break; // no need to declare protocols for wrapper types, they do it already in their headers.
					if (tp.Protocols is not null) {
						for (int p = 0; p < tp.Protocols.Length; p++) {
							implementedProtocols.Add (tp.Protocols [p].ProtocolName);
							var proto = tp.Protocols [p].Type;
							CheckNamespace (proto, exceptions);
						}
					}
					if (App.Optimizations.RegisterProtocols == true && tp.AdoptedProtocols is not null)
						implementedProtocols.UnionWith (tp.AdoptedProtocols);
					tp = tp.BaseType;
				}
				implementedProtocols.Remove ("UIAppearance"); // This is not a real protocol
				if (implementedProtocols.Count > 0) {
					iface.Append ("<");
					var firstProtocol = true;
					foreach (var ip in implementedProtocols.OrderBy (v => v)) {
						if (!firstProtocol)
							iface.Append (", ");
						firstProtocol = false;
						iface.Append (ip);
					}
					iface.Append (">");
				}

				AutoIndentStringBuilder implementation_fields = null;
				if (is_protocol) {
					iface.WriteLine ();
				} else {
					iface.WriteLine (" {");

					if (@class.Fields is not null) {
						foreach (var field in @class.Fields.Values) {
							AutoIndentStringBuilder fields = null;
							if (field.IsPrivate) {
								// Private fields go in the @implementation section.
								if (implementation_fields is null)
									implementation_fields = new AutoIndentStringBuilder (1);
								fields = implementation_fields;
							} else {
								// Public fields go in the header.
								fields = iface;
							}
							try {
								switch (field.FieldType) {
								case "@":
									fields.Write ("id ");
									break;
								case "^v":
									fields.Write ("void *");
									break;
								case "XamarinObject":
									fields.Write ("XamarinObject ");
									break;
								default:
									throw ErrorHelper.CreateError (4120, Errors.MT4120, field.FieldType, field.DeclaringType.Type.FullName, field.Name);
								}
								fields.Write (field.Name);
								fields.WriteLine (";");
							} catch (Exception ex) {
								exceptions.Add (ex);
							}
						}
					}
					iface.WriteLine ("}");
				}

				iface.Indent ();
				if (@class.Properties is not null) {
					foreach (var property in @class.Properties) {
						try {
							if (is_protocol)
								iface.Write (property.IsOptional ? "@optional " : "@required ");
							iface.Write ("@property (nonatomic");
							switch (property.ArgumentSemantic) {
							case ArgumentSemantic.Copy:
								iface.Write (", copy");
								break;
							case ArgumentSemantic.Retain:
								iface.Write (", retain");
								break;
							case ArgumentSemantic.Assign:
							case ArgumentSemantic.None:
							default:
								iface.Write (", assign");
								break;
							}
							if (property.IsReadOnly)
								iface.Write (", readonly");

							if (property.Selector is not null) {
								if (property.GetterSelector is not null && property.Selector != property.GetterSelector)
									iface.Write (", getter = ").Write (property.GetterSelector);
								if (property.SetterSelector is not null) {
									var setterSel = string.Format ("set{0}{1}:", char.ToUpperInvariant (property.Selector [0]), property.Selector.Substring (1));
									if (setterSel != property.SetterSelector)
										iface.Write (", setter = ").Write (property.SetterSelector);
								}
							}

							iface.Write (") ");
							try {
								iface.Write (ToObjCParameterType (property.PropertyType, property.DeclaringType.Type.FullName, exceptions, property.Property));
							} catch (ProductException mte) {
								exceptions.Add (CreateException (4138, mte, property.Property, "The registrar cannot marshal the property type '{0}' of the property '{1}.{2}'.",
									GetTypeFullName (property.PropertyType), property.DeclaringType.Type.FullName, property.Name));
							}
							iface.Write (" ").Write (property.Selector);
							iface.WriteLine (";");
						} catch (Exception ex) {
							exceptions.Add (ex);
						}
					}
				}

				if (@class.Methods is not null) {
					foreach (var method in @class.Methods) {
						try {
							if (is_protocol)
								iface.Write (method.IsOptional ? "@optional " : "@required ");
							iface.WriteLine ("{0};", GetObjCSignature (method, exceptions));
						} catch (ProductException ex) {
							skip.Add (method);
							exceptions.Add (ex);
						} catch (Exception ex) {
							skip.Add (method);
							exceptions.Add (ErrorHelper.CreateError (4114, ex, Errors.MT4114, method.DeclaringType.Type.FullName, method.Method.Name));
						}
					}
				}
				iface.Unindent ();
				iface.WriteLine ("@end");
				iface.WriteLine ();

				if (!is_protocol && !@class.IsWrapper) {
					var hasClangDiagnostic = @class.IsModel;
					if (hasClangDiagnostic)
						sb.WriteLine ("#pragma clang diagnostic push");
					if (@class.IsModel) {
						sb.WriteLine ("#pragma clang diagnostic ignored \"-Wprotocol\"");
						sb.WriteLine ("#pragma clang diagnostic ignored \"-Wobjc-protocol-property-synthesis\"");
						sb.WriteLine ("#pragma clang diagnostic ignored \"-Wobjc-property-implementation\"");
					}
					if (@class.IsCategory) {
						sb.WriteLine ("@implementation {0} ({1})", EncodeNonAsciiCharacters (@class.BaseType.ExportedName), @class.CategoryName);
					} else {
						sb.WriteLine ("@implementation {0} {{", class_name);
						if (implementation_fields is not null) {
							sb.Indent ();
							sb.Append (implementation_fields);
							sb.Unindent ();
						}
						sb.WriteLine ("}");
					}
					sb.Indent ();
					if (@class.Methods is not null) {
						foreach (var method in @class.Methods) {
							if (skip.Contains (method))
								continue;

							try {
								Specialize (sb, method, exceptions);
							} catch (Exception ex) {
								exceptions.Add (ex);
							}
						}
					}
					sb.Unindent ();
					sb.WriteLine ("@end");
					if (hasClangDiagnostic)
						sb.AppendLine ("#pragma clang diagnostic pop");
				}
				sb.WriteLine ();
			}

			map.AppendLine ("{ NULL, 0 },");
			map.AppendLine ("};");
			map.AppendLine ();

			if (App.Optimizations.RedirectClassHandles == true)
				map.AppendLine ("static void *__xamarin_class_handles [{0}];", i);
			if (skipped_types.Count > 0) {
				map.AppendLine ("static const MTManagedClassMap __xamarin_skipped_map [] = {");
				foreach (var skipped in skipped_types) {
					if (!TryCreateTokenReference (skipped.Skipped, TokenType.TypeDef, out var skipped_ref, exceptions))
						continue;
					if (!TryCreateTokenReference (skipped.Actual.Type, TokenType.TypeDef, out var actual_ref, exceptions))
						continue;
					skipped.SkippedTokenReference = skipped_ref;
					skipped.ActualTokenReference = actual_ref;
				}

				foreach (var skipped in skipped_types.OrderBy ((v) => v.SkippedTokenReference))
					map.AppendLine ("{{ 0x{0:X}, 0x{1:X} /* '{2}' => '{3}' */ }},", skipped.SkippedTokenReference, skipped.ActualTokenReference, skipped.Skipped.FullName, skipped.Actual.Type.FullName);
				map.AppendLine ("};");
				map.AppendLine ();
			}

			map.AppendLine ("static const MTAssembly __xamarin_registration_assemblies [] = {");
			int count = 0;
			foreach (var assembly in registered_assemblies) {
				count++;
				if (count > 1)
					map.AppendLine (", ");
				map.Append ("{ \"");
				map.Append (assembly.Name);
				map.Append ("\", \"");
				map.Append (assembly.Assembly.MainModule.Mvid.ToString ());
				map.Append ("\" }");
			}
			map.AppendLine ();
			map.AppendLine ("};");
			map.AppendLine ();

			if (full_token_reference_count > 0) {
				map.AppendLine ("static const MTFullTokenReference __xamarin_token_references [] = {");
				map.AppendLine (full_token_references);
				map.AppendLine ("};");
				map.AppendLine ();
			}

			if (protocol_wrapper_map.Count > 0) {
				var ordered = protocol_wrapper_map.OrderBy ((v) => v.Key);
				map.AppendLine ("static const MTProtocolWrapperMap __xamarin_protocol_wrapper_map [] = {");
				foreach (var p in ordered) {
					map.AppendLine ("{{ 0x{0:X} /* {1} */, 0x{2:X} /* {3} */ }},", p.Key, p.Value.Item1.Name, p.Value.Item2, p.Value.Item1.ProtocolWrapperType.Name);
				}
				map.AppendLine ("};");
				map.AppendLine ();
			}
			if (protocols.Count > 0) {
				var ordered = protocols.OrderBy ((v) => v.TokenReference);
				map.AppendLine ("static const uint32_t __xamarin_protocol_tokens [] = {");
				foreach (var p in ordered)
					map.AppendLine ("0x{0:X}, /* {1} */", p.TokenReference, p.Protocol.Type.FullName);
				map.AppendLine ("};");
				map.AppendLine ("static const Protocol* __xamarin_protocols [] = {");
				foreach (var p in ordered) {
					bool use_dynamic = false;
					if (App.Platform != ApplePlatform.MacOSX) {
						switch (p.Protocol.ProtocolName) {
						case "CAMetalDrawable": // The header isn't available for the simulator.
							use_dynamic = IsSimulator;
							break;
						}
					}
					if (use_dynamic) {
						map.AppendLine ("objc_getProtocol (\"{0}\"), /* {1} */", p.Protocol.ProtocolName, p.Protocol.Type.FullName);
					} else {
						map.AppendLine ("@protocol ({0}), /* {1} */", p.Protocol.ProtocolName, p.Protocol.Type.FullName);
					}
				}
				map.AppendLine ("};");
			}
			map.AppendLine ("static struct MTRegistrationMap __xamarin_registration_map = {");
			map.AppendLine ($"\"{Xamarin.ProductConstants.Hash}\",");
			map.AppendLine ("__xamarin_registration_assemblies,");
			map.AppendLine ("__xamarin_class_map,");
			map.AppendLine (full_token_reference_count == 0 ? "NULL," : "__xamarin_token_references,");
			map.AppendLine (skipped_types.Count == 0 ? "NULL," : "__xamarin_skipped_map,");
			map.AppendLine (protocol_wrapper_map.Count == 0 ? "NULL," : "__xamarin_protocol_wrapper_map,");
			if (needs_protocol_map && protocols.Count > 0) {
				map.AppendLine ("{ __xamarin_protocol_tokens, __xamarin_protocols },");
			} else {
				map.AppendLine ("{ NULL, NULL },");
			}
			map.AppendLine ("{0},", count);
			map.AppendLine ("{0},", i);
			map.AppendLine ("{0},", full_token_reference_count);
			map.AppendLine ("{0},", skipped_types.Count);
			map.AppendLine ("{0},", protocol_wrapper_map.Count);
			map.AppendLine ("{0},", needs_protocol_map ? protocols.Count : 0);
			if (App.Optimizations.RedirectClassHandles == true)
				map.AppendLine ("&__xamarin_class_handles [0]");
			else
				map.AppendLine ("(void **)0");
			map.AppendLine ("};");


			map_init.AppendLine ("xamarin_add_registration_map (&__xamarin_registration_map, {0});", string.IsNullOrEmpty (single_assembly) ? "false" : "true");
			map_init.AppendLine ("}");

			sb.WriteLine (map.ToString ());
			sb.WriteLine (map_init.ToString ());
			ErrorHelper.ThrowIfErrors (exceptions);
		}

		bool TryGetIntPtrBoolCtor (TypeDefinition type, List<Exception> exceptions, [NotNullWhen (true)] out MethodDefinition? ctor)
		{
			ctor = null;
			if (!type.HasMethods)
				return false;
			foreach (var method in type.Methods) {
				if (!method.IsConstructor || !method.HasParameters)
					continue;
				if (method.Parameters.Count != 2)
					continue;
				if (!method.Parameters [1].ParameterType.Is ("System", "Boolean"))
					continue;
				if (Driver.IsDotNet) {
					if (method.Parameters [0].ParameterType.Is ("System", "IntPtr")) {
						// The registrar found a non-optimal type `{0}`: the type does not have a constructor that takes two (ObjCRuntime.NativeHandle, bool) arguments. However, a constructor that takes two (System.IntPtr, bool) arguments was found (and will be used instead). It's highly recommended to change the signature of the (System.IntPtr, bool) constructor to be (ObjCRuntime.NativeHandle, bool).
						exceptions.Add (ErrorHelper.CreateWarning (App, 4186, method, Errors.MT4186, type.FullName));
						return true;
					}
					if (!method.Parameters [0].ParameterType.Is ("ObjCRuntime", "NativeHandle"))
						continue;
				} else {
					if (!method.Parameters [0].ParameterType.Is ("System", "IntPtr"))
						continue;
				}
				ctor = method;
				return true;
			}
			return false;
		}

		bool SpecializeTrampoline (AutoIndentStringBuilder sb, ObjCMethod method, List<Exception> exceptions)
		{
			var isGeneric = method.DeclaringType.IsGeneric;

			switch (method.CurrentTrampoline) {
			case Trampoline.Retain:
				sb.WriteLine ("-(id) retain");
				sb.WriteLine ("{");
				sb.WriteLine ("return xamarin_retain_trampoline (self, _cmd);");
				sb.WriteLine ("}");
				sb.WriteLine ();
				return true;
			case Trampoline.Release:
				sb.WriteLine ("-(void) release");
				sb.WriteLine ("{");
				sb.WriteLine ("xamarin_release_trampoline (self, _cmd);");
				sb.WriteLine ("}");
				sb.WriteLine ();
				return true;
			case Trampoline.GetGCHandle:
				sb.WriteLine ("-(GCHandle) xamarinGetGCHandle");
				sb.WriteLine ("{");
				sb.WriteLine ("return __monoObjectGCHandle.gc_handle;");
				sb.WriteLine ("}");
				sb.WriteLine ();
				return true;
			case Trampoline.SetGCHandle:
				sb.WriteLine ("-(bool) xamarinSetGCHandle: (GCHandle) gc_handle flags: (enum XamarinGCHandleFlags) flags");
				sb.WriteLine ("{");
				sb.WriteLine ("if (((flags & XamarinGCHandleFlags_InitialSet) == XamarinGCHandleFlags_InitialSet) && __monoObjectGCHandle.gc_handle != INVALID_GCHANDLE) {");
				sb.WriteLine ("return false;");
				sb.WriteLine ("}");
				sb.WriteLine ("flags = (enum XamarinGCHandleFlags) (flags & ~XamarinGCHandleFlags_InitialSet);"); // Remove the InitialSet flag, we don't want to store it.
				sb.WriteLine ("__monoObjectGCHandle.gc_handle = gc_handle;");
				sb.WriteLine ("__monoObjectGCHandle.flags = flags;");
				sb.WriteLine ("__monoObjectGCHandle.native_object = self;");
				sb.WriteLine ("return true;");
				sb.WriteLine ("}");
				sb.WriteLine ();
				return true;
			case Trampoline.GetFlags:
				sb.WriteLine ("-(enum XamarinGCHandleFlags) xamarinGetFlags");
				sb.WriteLine ("{");
				sb.WriteLine ("return __monoObjectGCHandle.flags;");
				sb.WriteLine ("}");
				sb.WriteLine ();
				return true;
			case Trampoline.SetFlags:
				sb.WriteLine ("-(void) xamarinSetFlags: (enum XamarinGCHandleFlags) flags");
				sb.WriteLine ("{");
				sb.WriteLine ("__monoObjectGCHandle.flags = flags;");
				sb.WriteLine ("}");
				sb.WriteLine ();
				return true;
			case Trampoline.Constructor:
				if (isGeneric) {
					sb.WriteLine (GetObjCSignature (method, exceptions));
					sb.WriteLine ("{");
					sb.WriteLine ("xamarin_throw_product_exception (4126, \"Cannot construct an instance of the type '{0}' from Objective-C because the type is generic.\");\n", method.DeclaringType.Type.FullName.Replace ("/", "+"));
					sb.WriteLine ("return self;");
					sb.WriteLine ("}");
					return true;
				}
				break;
			case Trampoline.CopyWithZone1:
				sb.AppendLine ("-(id) copyWithZone: (NSZone *) zone");
				sb.AppendLine ("{");
				sb.AppendLine ("id rv;");
				sb.AppendLine ("GCHandle gchandle;");
				sb.AppendLine ("enum XamarinGCHandleFlags flags = XamarinGCHandleFlags_None;");
				sb.AppendLine ();
				sb.AppendLine ("gchandle = xamarin_get_gchandle_with_flags (self, &flags);");
				sb.AppendLine ("if (gchandle != INVALID_GCHANDLE)");
				sb.Indent ().AppendLine ("xamarin_set_gchandle_with_flags (self, INVALID_GCHANDLE, XamarinGCHandleFlags_None);").Unindent ();
				// Call the base class implementation
				sb.AppendLine ("rv = [super copyWithZone: zone];");
				sb.AppendLine ();
				sb.AppendLine ("if (gchandle != INVALID_GCHANDLE)");
				sb.Indent ().AppendLine ("xamarin_set_gchandle_with_flags (self, gchandle, flags);").Unindent ();
				sb.AppendLine ();
				sb.AppendLine ("return rv;");
				sb.AppendLine ("}");
				return true;
			case Trampoline.CopyWithZone2:
#if NET
				// Managed Static Registrar handles CopyWithZone2 in GenerateCallToUnmanagedCallersOnlyMethod
				if (LinkContext.App.Registrar == RegistrarMode.ManagedStatic) {
					return false;
				}
#endif
				sb.AppendLine ("-(id) copyWithZone: (NSZone *) zone");
				sb.AppendLine ("{");
				sb.AppendLine ("return xamarin_copyWithZone_trampoline2 (self, _cmd, zone);");
				sb.AppendLine ("}");
				return true;
			}

			var customConformsToProtocol = method.Selector == "conformsToProtocol:" && method.Method.DeclaringType.Is ("Foundation", "NSObject") && method.Method.Name == "InvokeConformsToProtocol" && method.Parameters.Length == 1;
			if (customConformsToProtocol) {
				if (Driver.IsDotNet) {
					customConformsToProtocol &= method.Parameters [0].Is ("ObjCRuntime", "NativeHandle");
				} else {
					customConformsToProtocol &= method.Parameters [0].Is ("System", "IntPtr");
				}
				if (customConformsToProtocol) {
					sb.AppendLine ("-(BOOL) conformsToProtocol: (void *) protocol");
					sb.AppendLine ("{");
					sb.AppendLine ("GCHandle exception_gchandle;");
					sb.AppendLine ("BOOL rv = xamarin_invoke_conforms_to_protocol (self, (Protocol *) protocol, &exception_gchandle);");
					sb.AppendLine ("xamarin_process_managed_exception_gchandle (exception_gchandle);");
					sb.AppendLine ("return rv;");
					sb.AppendLine ("}");
					return true;
				}
			}

			return false;
		}

		bool TryGetReturnType (ObjCMethod method, string descriptiveMethodName, List<Exception> exceptions, out string rettype, out bool isCtor)
		{
			rettype = string.Empty;
			isCtor = false;

			switch (method.CurrentTrampoline) {
			case Trampoline.None:
			case Trampoline.Normal:
			case Trampoline.Static:
			case Trampoline.Single:
			case Trampoline.Double:
			case Trampoline.Long:
			case Trampoline.StaticLong:
			case Trampoline.StaticDouble:
			case Trampoline.StaticSingle:
			case Trampoline.X86_DoubleABI_StaticStretTrampoline:
			case Trampoline.X86_DoubleABI_StretTrampoline:
			case Trampoline.StaticStret:
			case Trampoline.Stret:
			case Trampoline.CopyWithZone2:
				switch (method.NativeReturnType.FullName) {
				case "System.Int64":
					rettype = "long long";
					return true;
				case "System.UInt64":
					rettype = "unsigned long long";
					return true;
				case "System.Single":
					rettype = "float";
					return true;
				case "System.Double":
					rettype = "double";
					return true;
				default:
					rettype = ToSimpleObjCParameterType (method.NativeReturnType, descriptiveMethodName, exceptions, method.Method);
					return true;
				}
			case Trampoline.Constructor:
				rettype = "id";
				isCtor = true;
				return true;
			default:
				return false;
			}
		}


		void SpecializePrepareParameters (AutoIndentStringBuilder sb, ObjCMethod method, int num_arg, string descriptiveMethodName, List<Exception> exceptions)
		{
			// prepare the parameters
			var baseMethod = GetBaseMethodInTypeHierarchy (method.Method);
			for (int i = 0; i < num_arg; i++) {
				var param = method.Method.Parameters [i];
				var paramBase = baseMethod.Parameters [i];
				var type = method.Parameters [i];
				var nativetype = method.NativeParameters [i];
				var objctype = ToObjCParameterType (nativetype, descriptiveMethodName, exceptions, method.Method);
				var original_objctype = objctype;
				var isRef = type.IsByReference;
				var isOut = param.IsOut || paramBase.IsOut;
				var isArray = type is ArrayType;
				var isByRefArray = isRef && GetElementType (type) is ArrayType;
				var isNativeEnum = false;
				var td = ResolveType (type);
				var isVariadic = i + 1 == num_arg && method.IsVariadic;

				if (type != nativetype) {
					body_setup.AppendLine ("MonoClass *paramclass{0} = NULL;", i);
					cleanup.AppendLine ("xamarin_mono_object_release (&paramclass{0});", i);
					body_setup.AppendLine ("MonoType *paramtype{0} = NULL;", i);
					cleanup.AppendLine ("xamarin_mono_object_release (&paramtype{0});", i);
					setup_call_stack.AppendLine ("paramtype{0} = xamarin_get_parameter_type (managed_method, {0});", i);
					setup_call_stack.AppendLine ("paramclass{0} = mono_class_from_mono_type (paramtype{0});", i);
					GenerateConversionToManaged (nativetype, type, setup_call_stack, descriptiveMethodName, ref exceptions, method, $"p{i}", $"arg_ptrs [{i}]", $"paramclass{i}", i);
					if (isRef || isOut)
						throw ErrorHelper.CreateError (4163, Errors.MT4163_B, descriptiveMethodName);
					continue;
				} else if (isRef) {
					type = GetElementType (type);
					td = type.Resolve ();
					original_objctype = ToObjCParameterType (type, descriptiveMethodName, exceptions, method.Method);
					objctype = ToObjCParameterType (type, descriptiveMethodName, exceptions, method.Method) + "*";
				} else if (td.IsEnum) {
					type = GetEnumUnderlyingType (td);
					isNativeEnum = HasAttribute (td, ObjCRuntime, StringConstants.NativeAttribute);
					td = type.Resolve ();
				}

				switch (type.FullName) {
				case "System.Int64":
				case "System.UInt64":
					// We already show MT4145 if the underlying enum type isn't a long or ulong
					if (isNativeEnum) {
						string tp;
						string ntp;
						if (type.FullName == "System.UInt64") {
							tp = "unsigned long long";
							ntp = "NSUInteger";
						} else {
							tp = "long long";
							ntp = "NSInteger";
						}

						if (isRef || isOut) {
							body_setup.AppendLine ("{1} nativeEnum{0} = 0;", i, tp);
							setup_call_stack.AppendLine ("arg_ptrs [{0}] = &nativeEnum{0};", i);
							copyback.AppendLine ("*p{0} = ({1}) nativeEnum{0};", ntp);
						} else {
							body_setup.AppendLine ("{1} nativeEnum{0} = p{0};", i, tp);
							setup_call_stack.AppendLine ("arg_ptrs [{0}] = &nativeEnum{0};", i);
						}
						break;
					}
					goto case "System.SByte";
				case "System.SByte":
				case "System.Byte":
				case "System.Char":
				case "System.Int16":
				case "System.UInt16":
				case "System.Int32":
				case "System.UInt32":
				case "System.Single":
				case "System.Double":
				case "System.Boolean":
					if (isRef || isOut) {
						// The isOut semantics isn't quite correct here: we pass the actual input value to managed code.
						// In theory we should create a temp location and then use a writeback when done instead.
						// This should be safe though, since managed code (at least C#) can't actually observe the value.
						setup_call_stack.AppendLine ("arg_ptrs [{0}] = p{0};", i);
					} else {
						setup_call_stack.AppendLine ("arg_ptrs [{0}] = &p{0};", i);
					}
					break;
				case "System.IntPtr":
					if (isVariadic) {
						body_setup.AppendLine ("va_list a{0};", i);
						setup_call_stack.AppendLine ("va_start (a{0}, p{1});", i, i - 1);
						setup_call_stack.AppendLine ("arg_ptrs [{0}] = &a{0};", i);
						copyback.AppendLine ("va_end (a{0});", i);
					} else if (isOut) {
						body_setup.AppendLine ("{1} a{0} = 0;", i, objctype);
						setup_call_stack.AppendLine ("arg_ptrs [{0}] = &a{0};", i);
						copyback.AppendLine ("*p{0} = a{0};", i);
					} else if (isRef) {
						setup_call_stack.AppendLine ("arg_ptrs [{0}] = p{0};", i);
					} else {
						body_setup.AppendLine ("{1} a{0} = p{0};", i, original_objctype);
						setup_call_stack.AppendLine ("arg_ptrs [{0}] = &a{0};", i);
					}
					break;
				case "ObjCRuntime.Selector":
					body_setup.AppendLine ("MonoObject *a{0} = NULL;", i);
					cleanup.AppendLine ("xamarin_mono_object_release (&a{0});", i);
					if (isRef) {
						body_setup.AppendLine ("MonoObject *a_copy{0} = NULL;", i);
						cleanup.AppendLine ($"xamarin_mono_object_release (&a_copy{i});");
						if (!isOut) {
							setup_call_stack.AppendLine ("a{0} = *p{0} ? xamarin_get_selector (*p{0}, &exception_gchandle) : NULL;", i);
							setup_call_stack.AppendLine ("if (exception_gchandle != INVALID_GCHANDLE) goto exception_handling;");
						}
						setup_call_stack.AppendLine ("a_copy{0} = a{0};", i);
						setup_call_stack.AppendLine ("arg_ptrs [{0}] = &a_copy{0};", i);
						copyback.AppendLine ("*p{0} = a_copy{0} ? (SEL) xamarin_get_handle_for_inativeobject (a_copy{0}, &exception_gchandle) : NULL;", i);
						copyback.AppendLine ("if (exception_gchandle != INVALID_GCHANDLE) goto exception_handling;");
					} else {
						setup_call_stack.AppendLine ("a{0} = p{0} ? xamarin_get_selector (p{0}, &exception_gchandle) : NULL;", i);
						setup_call_stack.AppendLine ("arg_ptrs [{0}] = a{0};", i);
						setup_call_stack.AppendLine ("if (exception_gchandle != INVALID_GCHANDLE) goto exception_handling;");
					}
					break;
				case "ObjCRuntime.Class":
					body_setup.AppendLine ("MonoObject *a{0} = NULL;", i);
					cleanup.AppendLine ("xamarin_mono_object_release (&a{0});", i);
					if (isRef) {
						body_setup.AppendLine ("MonoObject *a_copy{0} = NULL;", i);
						cleanup.AppendLine ($"xamarin_mono_object_release (&a_copy{i});");
						if (!isOut) {
							setup_call_stack.AppendLine ("a{0} = *p{0} ? xamarin_get_class (*p{0}, &exception_gchandle) : NULL;", i);
							setup_call_stack.AppendLine ("if (exception_gchandle != INVALID_GCHANDLE) goto exception_handling;");
						}
						setup_call_stack.AppendLine ("a_copy{0} = a{0};", i);
						setup_call_stack.AppendLine ("arg_ptrs [{0}] = &a_copy{0};", i);
						copyback.AppendLine ("*p{0} = a_copy{0} ? (Class) xamarin_get_handle_for_inativeobject (a_copy{0}, &exception_gchandle) : NULL;", i);
						copyback.AppendLine ("if (exception_gchandle != INVALID_GCHANDLE) goto exception_handling;");
					} else {
						setup_call_stack.AppendLine ("a{0} = p{0} ? xamarin_get_class (p{0}, &exception_gchandle) : NULL;", i);
						setup_call_stack.AppendLine ("arg_ptrs [{0}] = a{0};", i);
						setup_call_stack.AppendLine ("if (exception_gchandle != INVALID_GCHANDLE) goto exception_handling;");
					}
					break;
				case "System.String":
					// This should always be an NSString and never char*
					body_setup.AppendLine ("MonoString *a{0} = NULL;", i);
					cleanup.AppendLine ("xamarin_mono_object_release (&a{0});", i);
					if (isRef) {
						// Need to create a copy of the input argument, because the managed method may change it, and we'll still need to release it afterwards
						body_setup.AppendLine ("MonoString *a_copy{0} = NULL;", i);
						cleanup.AppendLine ($"xamarin_mono_object_release (&a_copy{i});");
						if (!isOut)
							setup_call_stack.AppendLine ("a{0} = xamarin_nsstring_to_string (NULL, *p{0});", i);
						setup_call_stack.AppendLine ("a_copy{0} = a{0};", i);
						setup_call_stack.AppendLine ("arg_ptrs [{0}] = &a_copy{0};", i);

						copyback.AppendLine ("*p{0} = xamarin_string_to_nsstring (a_copy{0}, false);", i);
					} else {
						setup_call_stack.AppendLine ("a{0} = xamarin_nsstring_to_string (NULL, p{0});", i);
						setup_call_stack.AppendLine ("arg_ptrs [{0}] = a{0};", i);
					}
					break;
				default:
					if (isArray || isByRefArray) {
						var elementType = ((ArrayType) type).ElementType;

						body_setup.AppendLine ("MonoArray *marr{0} = NULL;", i);
						body_setup.AppendLine ("NSArray *arr{0} = NULL;", i);
						if (isByRefArray) {
							body_setup.AppendLine ("MonoArray *original_marr{0} = NULL;", i);
							cleanup.AppendLine ("xamarin_mono_object_release (&original_marr{0});", i);
							setup_call_stack.AppendLine ("if (p{0} == NULL) {{", i);
							setup_call_stack.AppendLine ("arg_ptrs [{0}] = NULL;", i);
							setup_call_stack.AppendLine ("} else {");
							setup_call_stack.AppendLine ("if (*p{0} != NULL) {{", i);
							setup_call_stack.AppendLine ("arr{0} = *(NSArray **) p{0};", i);
						} else {
							setup_call_stack.AppendLine ("arr{0} = p{0};", i);
							if (App.EnableDebug)
								setup_call_stack.AppendLine ("xamarin_check_objc_type (p{0}, [NSArray class], _cmd, self, {0}, managed_method);", i);
						}

						var isString = elementType.Is ("System", "String");
						var isNSObject = !isString && IsNSObject (elementType);
						var isINativeObject = !isString && !isNSObject && IsNativeObject (elementType);

						if (isString) {
							setup_call_stack.AppendLine ("marr{0} = xamarin_nsarray_to_managed_string_array (arr{0}, &exception_gchandle);", i);
							setup_call_stack.AppendLine ("if (exception_gchandle != INVALID_GCHANDLE) goto exception_handling;");
							cleanup.AppendLine ("xamarin_mono_object_release (&marr{0});", i);
						} else if (isNSObject) {
							body_setup.AppendLine ("MonoType *paramtype{0} = NULL;", i);
							cleanup.AppendLine ("xamarin_mono_object_release (&paramtype{0});", i);
							setup_call_stack.AppendLine ("paramtype{0} = xamarin_get_parameter_type (managed_method, {0});", i);
							setup_call_stack.AppendLine ("marr{0} = xamarin_nsarray_to_managed_nsobject_array (arr{0}, paramtype{0}, NULL, &exception_gchandle);", i);
							setup_call_stack.AppendLine ("if (exception_gchandle != INVALID_GCHANDLE) goto exception_handling;");
							cleanup.AppendLine ("xamarin_mono_object_release (&marr{0});", i);
						} else if (isINativeObject) {
							TypeDefinition nativeObjType = elementType.Resolve ();
							var isNativeObjectInterface = nativeObjType.IsInterface;
							nativeObjType = GetInstantiableType (nativeObjType, exceptions, descriptiveMethodName);

							body_setup.AppendLine ("MonoType *paramtype{0} = NULL;", i);
							cleanup.AppendLine ("xamarin_mono_object_release (&paramtype{0});", i);
							setup_call_stack.AppendLine ("paramtype{0} = xamarin_get_parameter_type (managed_method, {0});", i);
							if (isNativeObjectInterface) {
								var resolvedElementType = ResolveType (elementType);
								if (TryCreateTokenReference (resolvedElementType, TokenType.TypeDef, out var iface_token_ref, out _) && TryCreateTokenReference (nativeObjType, TokenType.TypeDef, out var implementation_token_ref, out _)) {
									var iface_token_ref_str = $"0x{iface_token_ref:X} /* {resolvedElementType} */ ";
									var implementation_token_ref_str = $"0x{implementation_token_ref:X} /* {nativeObjType} */ ";
									setup_call_stack.AppendLine ("marr{0} = xamarin_nsarray_to_managed_inativeobject_array_static (arr{0}, paramtype{0}, NULL, {1}, {2}, &exception_gchandle);", i, iface_token_ref_str, implementation_token_ref_str);
								} else {
									setup_call_stack.AppendLine ("marr{0} = xamarin_nsarray_to_managed_inativeobject_array (arr{0}, paramtype{0}, NULL, &exception_gchandle);", i);
								}
							} else {
								setup_call_stack.AppendLine ("marr{0} = xamarin_nsarray_to_managed_inativeobject_array (arr{0}, paramtype{0}, NULL, &exception_gchandle);", i);
							}
							cleanup.AppendLine ("xamarin_mono_object_release (&marr{0});", i);
							setup_call_stack.AppendLine ("if (exception_gchandle != INVALID_GCHANDLE) goto exception_handling;");
						} else {
							throw ErrorHelper.CreateError (App, 4111, method.Method, Errors.MT4111, type.FullName, descriptiveMethodName);
						}
						if (isByRefArray) {
							setup_call_stack.AppendLine ("}");
							setup_call_stack.AppendLine ("original_marr{0} = marr{0};", i);
							setup_call_stack.AppendLine ("arg_ptrs [{0}] = &marr{0};", i);
							setup_call_stack.AppendLine ("}");
						} else {
							setup_call_stack.AppendLine ("arg_ptrs [{0}] = marr{0};", i);
						}
						if (isByRefArray) {
							copyback.AppendLine ("if (p{0} && original_marr{0} != marr{0}) {{", i);
							if (isString) {
								copyback.AppendLine ("*p{0} = xamarin_managed_string_array_to_nsarray (marr{0}, &exception_gchandle);", i);
							} else if (isNSObject) {
								copyback.AppendLine ("*p{0} = xamarin_managed_nsobject_array_to_nsarray (marr{0}, &exception_gchandle);", i);
							} else if (isINativeObject) {
								copyback.AppendLine ("*p{0} = xamarin_managed_inativeobject_array_to_nsarray (marr{0}, &exception_gchandle);", i);
							} else {
								throw ErrorHelper.CreateError (99, Errors.MX0099, "byref array is neither string, NSObject or INativeObject");
							}
							copyback.AppendLine ("}");
						}
					} else if (IsNSObject (type)) {
						if (isRef) {
							body_setup.AppendLine ("MonoObject *mobj{0} = NULL;", i);
							body_setup.AppendLine ("MonoObject *mobj_out{0} = NULL;", i);
							if (!isOut) {
								body_setup.AppendLine ("NSObject *nsobj{0} = NULL;", i);
								setup_call_stack.AppendLine ("if (p{0} != NULL)", i).Indent ();
								setup_call_stack.AppendLine ("nsobj{0} = *(NSObject **) p{0};", i).Unindent ();
								setup_call_stack.AppendLine ("if (nsobj{0}) {{", i);
								body_setup.AppendLine ("MonoType *paramtype{0} = NULL;", i);
								cleanup.AppendLine ("xamarin_mono_object_release (&paramtype{0});", i);
								setup_call_stack.AppendLine ("paramtype{0} = xamarin_get_parameter_type (managed_method, {0});", i);
								setup_call_stack.AppendLine ("mobj{0} = xamarin_get_nsobject_with_type_for_ptr (nsobj{0}, false, paramtype{0}, &exception_gchandle);", i);
								cleanup.AppendLine ("xamarin_mono_object_release (&mobj{0});", i);
								setup_call_stack.AppendLine ("if (exception_gchandle != INVALID_GCHANDLE) {");
								setup_call_stack.AppendLine ("exception_gchandle = xamarin_get_exception_for_parameter (8029, exception_gchandle, \"Unable to marshal the byref parameter\", _cmd, managed_method, paramtype{0}, {0}, true);", i);
								setup_call_stack.AppendLine ("goto exception_handling;");
								setup_call_stack.AppendLine ("}");
								if (App.EnableDebug) {
									body_setup.AppendLine ("MonoClass *paramclass{0} = NULL;", i);
									cleanup.AppendLine ("xamarin_mono_object_release (&paramclass{0});", i);
									setup_call_stack.AppendLine ("paramclass{0} = mono_class_from_mono_type (paramtype{0});", i);
									setup_call_stack.AppendLine ("xamarin_verify_parameter (mobj{0}, _cmd, self, nsobj{0}, {0}, paramclass{0}, managed_method);", i);
								}
								setup_call_stack.AppendLine ("}");
							}

							// argument semantics?
							setup_call_stack.AppendLine ("mobj_out{0} = mobj{0};", i);
							setup_call_stack.AppendLine ("arg_ptrs [{0}] = (int *) &mobj_out{0};", i);
							body_setup.AppendLine ("void * handle{0} = NULL;", i);
							copyback.AppendLine ("if (mobj_out{0} != NULL) {{", i);
							copyback.AppendLine ("handle{0} = xamarin_get_nsobject_handle (mobj_out{0});", i);
							copyback.AppendLine ("xamarin_mono_object_release (&mobj_out{0});", i);
							copyback.AppendLine ("}");
							copyback.AppendLine ("if (p{0} != NULL)", i).Indent ();
							copyback.AppendLine ("*p{0} = (id) handle{0};", i).Unindent ();
						} else {
							body_setup.AppendLine ("NSObject *nsobj{0} = NULL;", i);
							setup_call_stack.AppendLine ("nsobj{0} = (NSObject *) p{0};", i);
							if (method.ArgumentSemantic == ArgumentSemantic.Copy) {
								setup_call_stack.AppendLine ("nsobj{0} = [nsobj{0} copy];", i);
								setup_call_stack.AppendLine ("[nsobj{0} autorelease];", i);
							}
							body_setup.AppendLine ("MonoObject *mobj{0} = NULL;", i);
							body_setup.AppendLine ("int32_t created{0} = false;", i);
							setup_call_stack.AppendLine ("if (nsobj{0}) {{", i);
							body_setup.AppendLine ("MonoType *paramtype{0} = NULL;", i);
							cleanup.AppendLine ("xamarin_mono_object_release (&paramtype{0});", i);
							setup_call_stack.AppendLine ("paramtype{0} = xamarin_get_parameter_type (managed_method, {0});", i);
							setup_call_stack.AppendLine ("mobj{0} = xamarin_get_nsobject_with_type_for_ptr_created (nsobj{0}, false, paramtype{0}, &created{0}, &exception_gchandle);", i);
							cleanup.AppendLine ("xamarin_mono_object_release (&mobj{0});", i);
							setup_call_stack.AppendLine ("if (exception_gchandle != INVALID_GCHANDLE) {");
							setup_call_stack.AppendLine ("exception_gchandle = xamarin_get_exception_for_parameter (8029, exception_gchandle, \"Unable to marshal the parameter\", _cmd, managed_method, paramtype{0}, {0}, true);", i);
							setup_call_stack.AppendLine ("goto exception_handling;");
							setup_call_stack.AppendLine ("}");
							if (App.EnableDebug) {
								body_setup.AppendLine ("MonoClass *paramclass{0} = NULL;", i);
								cleanup.AppendLine ("xamarin_mono_object_release (&paramclass{0});", i);
								setup_call_stack.AppendLine ("paramclass{0} = mono_class_from_mono_type (paramtype{0});", i);
								setup_call_stack.AppendLine ("xamarin_verify_parameter (mobj{0}, _cmd, self, nsobj{0}, {0}, paramclass{0}, managed_method);", i);
							}
							setup_call_stack.AppendLine ("}");
							setup_call_stack.AppendLine ("arg_ptrs [{0}] = mobj{0};", i);

							if (HasAttribute (paramBase, ObjCRuntime, StringConstants.TransientAttribute)) {
								copyback.AppendLine ("if (created{0}) {{", i);
								copyback.AppendLine ("xamarin_dispose (mobj{0}, &exception_gchandle);", i);
								copyback.AppendLine ("if (exception_gchandle != INVALID_GCHANDLE) goto exception_handling;");
								copyback.AppendLine ("}");
							}
						}
					} else if (IsNativeObject (td)) {
						var nativeObjType = GetInstantiableType (td, exceptions, descriptiveMethodName);
						var findMonoClass = false;
						var tdTokenRef = INVALID_TOKEN_REF;
						var nativeObjectTypeTokenRef = INVALID_TOKEN_REF;
						if (!td.IsInterface) {
							findMonoClass = true;
						} else if (!(isRef && isOut) && (!TryCreateTokenReference (td, TokenType.TypeDef, out tdTokenRef, out _) || !TryCreateTokenReference (nativeObjType, TokenType.TypeDef, out nativeObjectTypeTokenRef, out _))) {
							findMonoClass = true;
						}
						if (findMonoClass) {
							// find the MonoClass for this parameter
							body_setup.AppendLine ("MonoType *type{0};", i);
							cleanup.AppendLine ("xamarin_mono_object_release (&type{0});", i);
							setup_call_stack.AppendLine ("type{0} = xamarin_get_parameter_type (managed_method, {0});", i);
						}

						body_setup.AppendLine ("MonoObject *inobj{0} = NULL;", i);
						cleanup.AppendLine ($"xamarin_mono_object_release (&inobj{i});");

						if (isRef) {
							if (isOut) {
								// Do nothing
							} else if (td.IsInterface && tdTokenRef != INVALID_TOKEN_REF && nativeObjectTypeTokenRef != INVALID_TOKEN_REF) {
								setup_call_stack.AppendLine ("inobj{0} = xamarin_get_inative_object_static (*p{0}, false, 0x{1:X} /* {2} */, 0x{3:X} /* {4} */, &exception_gchandle);", i, tdTokenRef, td.FullName, nativeObjectTypeTokenRef, nativeObjType.FullName);
								setup_call_stack.AppendLine ("if (exception_gchandle != INVALID_GCHANDLE) goto exception_handling;");
							} else {
								body_setup.AppendLine ("MonoReflectionType *reflectiontype{0} = NULL;", i);
								cleanup.AppendLine ("xamarin_mono_object_release (&reflectiontype{0});", i);
								setup_call_stack.AppendLine ("reflectiontype{0} = mono_type_get_object (mono_domain_get (), type{0});", i);
								setup_call_stack.AppendLine ("inobj{0} = xamarin_get_inative_object_dynamic (*p{0}, false, reflectiontype{0}, &exception_gchandle);", i);
								setup_call_stack.AppendLine ("if (exception_gchandle != INVALID_GCHANDLE) goto exception_handling;");
							}
							// We need to keep a copy of the inobj argument, because it may change during the call to managed code, and we still have to release the original value.
							body_setup.AppendLine ("MonoObject *inobj_copy{0} = NULL;", i);
							setup_call_stack.AppendLine ("inobj_copy{0} = inobj{0};", i);
							setup_call_stack.AppendLine ("arg_ptrs [{0}] = &inobj_copy{0};", i);
							body_setup.AppendLine ("id handle{0} = nil;", i);
							copyback.AppendLine ("if (inobj_copy{0} != NULL) {{", i);
							copyback.AppendLine ("handle{0} = xamarin_get_handle_for_inativeobject (inobj_copy{0}, &exception_gchandle);", i);
							copyback.AppendLine ("if (exception_gchandle != INVALID_GCHANDLE) goto exception_handling;");
							copyback.AppendLine ("xamarin_mono_object_release (&inobj_copy{0});", i);
							copyback.AppendLine ("}");
							copyback.AppendLine ("*p{0} = (id) handle{0};", i);
						} else {
							if (td.IsInterface && tdTokenRef != INVALID_TOKEN_REF && nativeObjectTypeTokenRef != INVALID_TOKEN_REF) {
								setup_call_stack.AppendLine ($"inobj{i} = xamarin_get_inative_object_static (p{i}, false, 0x{tdTokenRef:X} /* {td.FullName} */, 0x{nativeObjectTypeTokenRef:X} /* {nativeObjType.FullName} */, &exception_gchandle);");
							} else {
								body_setup.AppendLine ("MonoReflectionType *reflectiontype{0} = NULL;", i);
								cleanup.AppendLine ("xamarin_mono_object_release (&reflectiontype{0});", i);
								setup_call_stack.AppendLine ("reflectiontype{0} = mono_type_get_object (mono_domain_get (), type{0});", i);
								setup_call_stack.AppendLine ("inobj{0} = xamarin_get_inative_object_dynamic (p{0}, false, reflectiontype{0}, &exception_gchandle);", i);
							}
							setup_call_stack.AppendLine ("if (exception_gchandle != INVALID_GCHANDLE) goto exception_handling;");
							setup_call_stack.AppendLine ("arg_ptrs [{0}] = inobj{0};", i);
						}
					} else if (type.IsValueType) {
						if (isRef || isOut) {
							// The isOut semantics isn't quite correct here: we pass the actual input value to managed code.
							// In theory we should create a temp location and then use a writeback when done instead.
							// This should be safe though, since managed code (at least C#) can't actually observe the value.
							setup_call_stack.AppendLine ("arg_ptrs [{0}] = p{0};", i);
						} else {
							setup_call_stack.AppendLine ("arg_ptrs [{0}] = &p{0};", i);
						}
					} else if (type.IsPointer) {
						setup_call_stack.AppendLine ("arg_ptrs [{0}] = p{0};", i);
					} else if (td.BaseType.FullName == "System.MulticastDelegate") {
						if (isRef) {
							throw ErrorHelper.CreateError (4110, Errors.MT4110, type.FullName, descriptiveMethodName);
						} else {
							// Bug #4858 (also related: #4718)
							var token = "INVALID_TOKEN_REF";
							if (App.Optimizations.StaticBlockToDelegateLookup == true) {
								var creatorMethod = GetBlockWrapperCreator (method, i);
								if (creatorMethod is null) {
									exceptions.Add (ErrorHelper.CreateWarning (App, 4174, method.Method, Errors.MT4174, method.DescriptiveMethodName, i + 1));
								} else if (TryCreateTokenReference (creatorMethod, TokenType.Method, out var creator_method_token_ref, out _)) {
									token = $"0x{creator_method_token_ref:X} /* {creatorMethod.FullName} */ ";
								}
							}
							body_setup.AppendLine ("MonoObject *del{0} = NULL;", i);
							cleanup.AppendLine ($"xamarin_mono_object_release (&del{i});");
							setup_call_stack.AppendLine ("if (p{0}) {{", i);
							setup_call_stack.AppendLine ("del{0} = xamarin_get_delegate_for_block_parameter (managed_method, {1}, {0}, p{0}, &exception_gchandle);", i, token);
							setup_call_stack.AppendLine ("arg_ptrs [{0}] = del{0};", i);
							setup_call_stack.AppendLine ("if (exception_gchandle != INVALID_GCHANDLE) goto exception_handling;");
							setup_call_stack.AppendLine ("} else {");
							setup_call_stack.AppendLine ("arg_ptrs [{0}] = NULL;", i);
							setup_call_stack.AppendLine ("}");
						}
					} else {
						throw ErrorHelper.CreateError (App, 4105, method.Method, Errors.MT4105, type.FullName, descriptiveMethodName);
					}
					break;
				}
			}
		}

		void Specialize (AutoIndentStringBuilder sb, ObjCMethod method, List<Exception> exceptions)
		{
			if (SpecializeTrampoline (sb, method, exceptions))
				return;

			var isGeneric = method.DeclaringType.IsGeneric;
			var returntype = method.ReturnType;
			var isStatic = method.IsStatic;
			var isInstanceCategory = method.IsCategoryInstance;
			var num_arg = method.Method.HasParameters ? method.Method.Parameters.Count : 0;
			var descriptiveMethodName = method.DescriptiveMethodName;
			var name = GetUniqueTrampolineName ("native_to_managed_trampoline_" + descriptiveMethodName);
			var isVoid = returntype.FullName == "System.Void";
			var merge_bodies = true;

			if (!TryGetReturnType (method, descriptiveMethodName, exceptions, out var rettype, out var isCtor))
				return;

			comment.Clear ();
			nslog_start.Clear ();
			nslog_end.Clear ();
			copyback.Clear ();
			invoke.Clear ();
			setup_call_stack.Clear ();
			body.Clear ();
			body_setup.Clear ();
			setup_return.Clear ();
			cleanup.Clear ();

			counter++;

			body.WriteLine ("{");

			var indent = merge_bodies ? sb.Indentation : sb.Indentation + 1;
			body.Indentation = indent;
			body_setup.Indentation = indent;
			copyback.Indentation = indent;
			invoke.Indentation = indent;
			setup_call_stack.Indentation = indent;
			setup_return.Indentation = indent;
			cleanup.Indentation = indent;

			// A comment describing the managed signature
			if (trace) {
				nslog_start.Indentation = sb.Indentation;
				comment.Indentation = sb.Indentation;
				nslog_end.Indentation = sb.Indentation;

				comment.AppendFormat ("// {2} {0}.{1} (", method.Method.DeclaringType.FullName, method.Method.Name, method.Method.ReturnType.FullName);
				for (int i = 0; i < num_arg; i++) {
					var param = method.Method.Parameters [i];
					if (i > 0)
						comment.Append (", ");
					comment.AppendFormat ("{0} {1}", param.ParameterType.FullName, param.Name);
				}
				comment.AppendLine (")");
				comment.AppendLine ("// ArgumentSemantic: {0} IsStatic: {1} Selector: '{2}' Signature: '{3}'", method.ArgumentSemantic, method.IsStatic, method.Selector, method.Signature);
			}

			// a couple of debug printfs
			if (trace) {
				StringBuilder args = new StringBuilder ();
				nslog_start.AppendFormat ("NSLog (@\"{0} (this: %@, sel: %@", name);
				for (int i = 0; i < num_arg; i++) {
					var type = method.Method.Parameters [i].ParameterType;
					bool isRef = type.IsByReference;
					if (isRef)
						type = type.GetElementType ();
					var td = type.Resolve ();

					nslog_start.AppendFormat (", {0}: ", method.Method.Parameters [i].Name);
					args.Append (", ");
					switch (type.FullName) {
					case "System.Drawing.RectangleF":
						var rectFunc = App.Platform == ApplePlatform.MacOSX ? "NSStringFromRect" : "NSStringFromCGRect";
						if (isRef) {
							nslog_start.Append ("%p : %@");
							args.AppendFormat ("p{0}, p{0} ? {1} (*p{0}) : @\"NULL\"", i, rectFunc);
						} else {
							nslog_start.Append ("%@");
							args.AppendFormat ("{1} (p{0})", i, rectFunc);
						}
						break;
					case "System.Drawing.PointF":
						var pointFunc = App.Platform == ApplePlatform.MacOSX ? "NSStringFromPoint" : "NSStringFromCGPoint";
						if (isRef) {
							nslog_start.Append ("%p: %@");
							args.AppendFormat ("p{0}, p{0} ? {1} (*p{0}) : @\"NULL\"", i, pointFunc);
						} else {
							nslog_start.Append ("%@");
							args.AppendFormat ("{1} (p{0})", i, pointFunc);
						}
						break;
					default:
						bool unknown;
						var spec = GetPrintfFormatSpecifier (td, out unknown);
						if (unknown) {
							nslog_start.AppendFormat ("%{0}", spec);
							args.AppendFormat ("&p{0}", i);
						} else if (isRef) {
							nslog_start.AppendFormat ("%p *= %{0}", spec);
							args.AppendFormat ("p{0}, *p{0}", i);
						} else {
							nslog_start.AppendFormat ("%{0}", spec);
							args.AppendFormat ("p{0}", i);
						}
						break;
					}
				}

				string ret_arg = string.Empty;
				nslog_end.Append (nslog_start.ToString ());
				if (!isVoid) {
					bool unknown;
					var spec = GetPrintfFormatSpecifier (method.Method.ReturnType.Resolve (), out unknown);
					if (!unknown) {
						nslog_end.Append (" ret: %");
						nslog_end.Append (spec);
						ret_arg = ", res";
					}
				}
				nslog_end.Append (") END\", self, NSStringFromSelector (_cmd)");
				nslog_end.Append (args.ToString ());
				nslog_end.Append (ret_arg);
				nslog_end.AppendLine (");");

				nslog_start.Append (") START\", self, NSStringFromSelector (_cmd)");
				nslog_start.Append (args.ToString ());
				nslog_start.AppendLine (");");
			}

#if NET
			// Generate the native trampoline to call the generated UnmanagedCallersOnly method if we're using the managed static registrar.
			if (LinkContext.App.Registrar == RegistrarMode.ManagedStatic) {
				GenerateCallToUnmanagedCallersOnlyMethod (sb, method, isCtor, isVoid, num_arg, descriptiveMethodName, exceptions);
				return;
			}
#endif

			if (!TryCreateTokenReference (method.Method, TokenType.Method, out var token_ref, exceptions))
				return;

			SpecializePrepareParameters (sb, method, num_arg, descriptiveMethodName, exceptions);

			// the actual invoke
			if (isCtor) {
				body_setup.AppendLine ("MonoClass *declaring_type = NULL;");
				invoke.AppendLine ("declaring_type = mono_method_get_class (managed_method);");
				invoke.AppendLine ("mthis = xamarin_new_nsobject (self, declaring_type, &exception_gchandle);");
				invoke.AppendLine ("xamarin_mono_object_release (&declaring_type);");
				cleanup.AppendLine ("xamarin_mono_object_release (&mthis);");
				invoke.AppendLine ("if (exception_gchandle != INVALID_GCHANDLE) goto exception_handling;");
			}

			var marshal_exception = "NULL";
			var post_invoke_check = string.Empty;
			if (App.MarshalManagedExceptions != MarshalManagedExceptionMode.Disable) {
				body_setup.AppendLine ("MonoObject *exception = NULL;");
				if (App.EnableDebug && App.IsDefaultMarshalManagedExceptionMode) {
					body_setup.AppendLine ("MonoObject **exception_ptr = xamarin_is_managed_exception_marshaling_disabled () ? NULL : &exception;");
					marshal_exception = "exception_ptr";
				} else {
					marshal_exception = "&exception";
				}
				post_invoke_check = "if (exception != NULL) goto exception_handling;";
			}

			if (!isVoid) {
				body_setup.AppendLine ("MonoObject *retval = NULL;");
				cleanup.AppendLine ("xamarin_mono_object_release (&retval);");
				invoke.Append ("retval = ");
			}

			invoke.AppendLine ("mono_runtime_invoke (managed_method, {0}, arg_ptrs, {1});", isStatic ? "NULL" : "mthis", marshal_exception);

			if (!string.IsNullOrEmpty (post_invoke_check))
				invoke.AppendLine (post_invoke_check);

			body_setup.AppendLine ("GCHandle exception_gchandle = INVALID_GCHANDLE;");

			SpecializePrepareReturnValue (sb, method, descriptiveMethodName, rettype, exceptions);

			if (App.Embeddinator)
				body.WriteLine ("xamarin_embeddinator_initialize ();");

			body.WriteLine ("MONO_ASSERT_GC_SAFE_OR_DETACHED;");
			body.WriteLine ("MONO_THREAD_ATTACH;"); // COOP: this will switch to GC_UNSAFE
			body.WriteLine ();

			// Write out everything
			if (merge_bodies) {
				body_setup.WriteLine ("MonoMethod *managed_method = *managed_method_ptr;");
			} else {
				if (!isGeneric)
					body.Write ("static ");
				body.WriteLine ("MonoMethod *managed_method = NULL;");
			}

			if (comment.Length > 0)
				body.WriteLine (comment.ToString ());
			if (isInstanceCategory)
				body.WriteLine ("id p0 = self;");
			body_setup.WriteLine ("void *arg_ptrs [{0}];", num_arg);
			if (!isStatic || isInstanceCategory)
				body.WriteLine ("MonoObject *mthis = NULL;");

			if (isCtor) {
				body.WriteLine ("bool has_nsobject = xamarin_has_nsobject (self, &exception_gchandle);");
				body.WriteLine ("if (exception_gchandle != INVALID_GCHANDLE) goto exception_handling;");
				body.WriteLine ("if (has_nsobject) {");
				body.WriteLine ("*call_super = true;");
				body.WriteLine ("goto exception_handling;");
				body.WriteLine ("}");
			}

			if ((!isStatic || isInstanceCategory) && !isCtor) {
				body.WriteLine ("if (self) {");
				body.WriteLine ("mthis = xamarin_get_managed_object_for_ptr_fast (self, &exception_gchandle);");
				cleanup.AppendLine ($"xamarin_mono_object_release (&mthis);");
				body.WriteLine ("if (exception_gchandle != INVALID_GCHANDLE) goto exception_handling;");
				body.WriteLine ("}");
			}

			// no locking should be required here, it doesn't matter if we overwrite the field (it'll be the same value).
			body.WriteLine ("if (!managed_method) {");
			body.Write ("GCHandle reflection_method_handle = ");
			if (isGeneric)
				body.Write ("xamarin_get_generic_method_from_token (mthis, ");
			else
				body.Write ("xamarin_get_method_from_token (");

			if (merge_bodies) {
				body.WriteLine ("token_ref, &exception_gchandle);");
			} else {
				body.WriteLine ("0x{0:X}, &exception_gchandle);", token_ref);
			}
			body_setup.AppendLine ("MonoReflectionMethod *reflection_method = NULL;");
			body.WriteLine ("reflection_method = (MonoReflectionMethod *) xamarin_gchandle_unwrap (reflection_method_handle);");
			cleanup.AppendLine ($"xamarin_mono_object_release (&reflection_method);");

			body.WriteLine ("if (exception_gchandle != INVALID_GCHANDLE) goto exception_handling;");
			body.WriteLine ("managed_method = xamarin_get_reflection_method_method (reflection_method);");
			if (merge_bodies)
				body.WriteLine ("*managed_method_ptr = managed_method;");

			// If the managed_method instance is stored in a static variable, we can't release it until process exit.
			if (merge_bodies || !isGeneric) {
				body.WriteLine ("xamarin_mono_object_release_at_process_exit (managed_method);");
			} else {
				cleanup.AppendLine ("xamarin_mono_object_release (&managed_method);");
			}

			body.WriteLine ("}");

			if (!isStatic && !isInstanceCategory && !isCtor) {
				body.WriteLine ("xamarin_check_for_gced_object (mthis, _cmd, self, managed_method, &exception_gchandle);");
				body.WriteLine ("if (exception_gchandle != INVALID_GCHANDLE) goto exception_handling;");
			}

			if (trace)
				body.AppendLine (nslog_start);

			body.AppendLine (setup_call_stack);
			body.AppendLine (invoke);
			body.AppendLine (copyback);
			body.AppendLine (setup_return);

			if (trace)
				body.AppendLine (nslog_end);

			body.StringBuilder.AppendLine ("exception_handling:");

			body.AppendLine (cleanup);

			body.WriteLine ("MONO_THREAD_DETACH;"); // COOP: this will switch to GC_SAFE

			body.AppendLine ("if (exception_gchandle != INVALID_GCHANDLE)");
			body.Indent ().WriteLine ("xamarin_process_managed_exception_gchandle (exception_gchandle);").Unindent ();

			if (App.MarshalManagedExceptions != MarshalManagedExceptionMode.Disable)
				body.WriteLine ("xamarin_process_managed_exception (exception);");

			if (isCtor) {
				body.WriteLine ("return self;");
			} else if (isVoid) {
				body.WriteLine ("return;");
			} else {
				body.WriteLine ("return res;");
			}

			body.WriteLine ("}");

			body.StringBuilder.Insert (2, body_setup);

			/* We merge duplicated bodies (based on the signature of the method and the entire body) */

			var objc_signature = new StringBuilder ().Append (rettype).Append (":");
			if (method.Method.HasParameters) {
				for (int i = 0; i < method.NativeParameters.Length; i++)
					objc_signature.Append (ToSimpleObjCParameterType (method.NativeParameters [i], descriptiveMethodName, exceptions, method.Method)).Append (":");
			}

			Body existing;
			Body b = new Body () {
				Code = body.ToString (),
				Signature = objc_signature.ToString (),
			};

			if (merge_bodies && bodies.TryGetValue (b, out existing)) {
				/* We already have an identical trampoline, use it instead */
				b = existing;
			} else {
				/* Need to create a new trampoline */
				if (merge_bodies)
					bodies [b] = b;
				b.Name = "native_to_managed_trampoline_" + bodies.Count.ToString ();

				if (merge_bodies) {
					methods.Append ("static ");
					methods.Append (rettype).Append (" ").Append (b.Name).Append (" (id self, SEL _cmd, MonoMethod **managed_method_ptr");
					var pcount = method.Method.HasParameters ? method.NativeParameters.Length : 0;
					for (int i = (isInstanceCategory ? 1 : 0); i < pcount; i++) {
						methods.Append (", ").Append (ToSimpleObjCParameterType (method.NativeParameters [i], descriptiveMethodName, exceptions, method.Method));
						methods.Append (" ").Append ("p").Append (i.ToString ());
					}
					if (isCtor)
						methods.Append (", bool* call_super");
					methods.Append (", uint32_t token_ref");
					methods.AppendLine (")");
					methods.AppendLine (body);
					methods.AppendLine ();
				}
			}
			b.Count++;

			sb.WriteLine ();
			sb.WriteLine (GetObjCSignature (method, exceptions));
			if (merge_bodies) {
				sb.WriteLine ("{");
				if (!isGeneric)
					sb.Write ("static ");
				sb.WriteLine ("MonoMethod *managed_method = NULL;");
				if (isCtor) {
					sb.WriteLine ("bool call_super = false;");
					sb.Write ("id rv = ");
				} else if (!isVoid) {
					sb.Write ("return ");
				}
				sb.Write (b.Name);
				sb.Write (" (self, _cmd, &managed_method");
				var paramCount = method.Method.HasParameters ? method.Method.Parameters.Count : 0;
				if (isInstanceCategory)
					paramCount--;
				for (int i = 0; i < paramCount; i++)
					sb.Write (", p{0}", i);
				if (isCtor)
					sb.Write (", &call_super");
				sb.Write (", 0x{0:X}", token_ref);
				sb.WriteLine (");");
				if (isCtor) {
					GenerateCallToSuperForConstructor (sb, method, exceptions);
					sb.WriteLine ("return rv;");
				}
				sb.WriteLine ("}");
			} else {
				sb.WriteLine (body);
			}
		}

#if NET
		void GenerateCallToUnmanagedCallersOnlyMethod (AutoIndentStringBuilder sb, ObjCMethod method, bool isCtor, bool isVoid, int num_arg, string descriptiveMethodName, List<Exception> exceptions)
		{
			// Generate the native trampoline to call the generated UnmanagedCallersOnly method.
			// We try to do as little as possible in here, and instead do the work in managed code.

			// If we're AOT-compiled, we don't need to look for the UnmanagedCallersOnly method,
			// we can just call the corresponding entry point directly. Otherwise we'll have to
			// call into managed code to find the function pointer for the UnmanagedCallersOnly
			// method (we store the result in a static variable, so that we only do this once
			// per method, the first time it's called).
			var staticCall = App.IsAOTCompiled (method.DeclaringType.Type.Module.Assembly.Name.Name);
			if (!App.Configuration.AssemblyTrampolineInfos.TryFindInfo (method.Method, out var pinvokeMethodInfo)) {
				exceptions.Add (ErrorHelper.CreateError (99, "Could not find the managed callback for {0}", descriptiveMethodName));
				return;
			}
			var ucoEntryPoint = pinvokeMethodInfo.UnmanagedCallersOnlyEntryPoint;
			sb.AppendLine ();
			if (!staticCall)
				sb.Append ("typedef ");

			var callbackReturnType = string.Empty;
			var hasReturnType = true;
			if (isCtor) {
				callbackReturnType = "id";
			} else if (isVoid) {
				callbackReturnType = "void";
				hasReturnType = false;
			} else {
				callbackReturnType = ToObjCParameterType (method.NativeReturnType, descriptiveMethodName, exceptions, method.Method);
			}

			sb.Append (callbackReturnType);

			sb.Append (" ");
			if (staticCall) {
				sb.Append (ucoEntryPoint);
			} else {
				sb.Append ("(*");
				sb.Append (ucoEntryPoint);
				sb.Append ("_function)");
			}
			sb.Append (" (id self, SEL sel");
			var indexOffset = method.IsCategoryInstance ? 1 : 0;
			for (var i = indexOffset; i < num_arg; i++) {
				sb.Append (", ");
				var parameterType = ToObjCParameterType (method.NativeParameters [i], method.DescriptiveMethodName, exceptions, method.Method, delegateToBlockType: true, cSyntaxForBlocks: true);
				var containsBlock = parameterType.Contains ("%PARAMETERNAME%");
				parameterType = parameterType.Replace ("%PARAMETERNAME%", $"p{i - indexOffset}");
				sb.Append (parameterType);
				if (!containsBlock)
					sb.AppendFormat (" p{0}", i - indexOffset);
			}
			if (isCtor)
				sb.Append (", bool* call_super");
			sb.Append (", GCHandle* exception_gchandle");

			if (method.IsVariadic)
				sb.Append (", ...");
			sb.Append (");");

			sb.WriteLine ();
			sb.WriteLine (GetObjCSignature (method, exceptions));
			sb.WriteLine ("{");
			sb.WriteLine ("GCHandle exception_gchandle = INVALID_GCHANDLE;");
			if (isCtor)
				sb.WriteLine ($"bool call_super = false;");
			if (hasReturnType)
				sb.WriteLine ($"{callbackReturnType} rv = {{ 0 }};");
			if (method.CurrentTrampoline == Trampoline.CopyWithZone2) {
				sb.WriteLine ("id p0 = (id)zone;");
				sb.WriteLine ("GCHandle gchandle;");
				sb.WriteLine ("enum XamarinGCHandleFlags flags = XamarinGCHandleFlags_None;");
				sb.WriteLine ("gchandle = xamarin_get_gchandle_with_flags (self, &flags);");
				sb.WriteLine ("if (gchandle != INVALID_GCHANDLE)");
				sb.Indent ().WriteLine ("xamarin_set_gchandle_with_flags (self, INVALID_GCHANDLE, XamarinGCHandleFlags_None);").Unindent ();
			}

			if (!staticCall) {
				sb.WriteLine ($"static {ucoEntryPoint}_function {ucoEntryPoint};");
				sb.WriteLine ($"xamarin_registrar_dlsym ((void **) &{ucoEntryPoint}, \"{method.Method.Module.Assembly.Name.Name}\", \"{ucoEntryPoint}\", {pinvokeMethodInfo.Id});");
			}
			if (hasReturnType)
				sb.Write ("rv = ");
			sb.Write (ucoEntryPoint);
			sb.Write (" (self, _cmd");
			for (var i = indexOffset; i < num_arg; i++) {
				sb.AppendFormat (", p{0}", i - indexOffset);
			}
			if (isCtor)
				sb.Write (", &call_super");
			sb.Write (", &exception_gchandle");
			sb.WriteLine (");");

			sb.WriteLine ("xamarin_process_managed_exception_gchandle (exception_gchandle);");

			if (isCtor) {
				GenerateCallToSuperForConstructor (sb, method, exceptions);
			}

			if (method.CurrentTrampoline == Trampoline.CopyWithZone2) {
				sb.WriteLine ("if (gchandle != INVALID_GCHANDLE)");
				sb.Indent ().WriteLine ("xamarin_set_gchandle_with_flags (self, gchandle, flags);").Unindent ();
			}

			if (hasReturnType)
				sb.WriteLine ("return rv;");

			sb.WriteLine ("}");
		}
#endif

		void SpecializePrepareReturnValue (AutoIndentStringBuilder sb, ObjCMethod method, string descriptiveMethodName, string rettype, List<Exception> exceptions)
		{
			var returntype = method.ReturnType;
			var isVoid = returntype.FullName == "System.Void";

			if (isVoid)
				return;

			switch (rettype) {
			case "CGRect":
				body_setup.AppendLine ("{0} res = {{{{0}}}};", rettype);
				break;
			default:
				body_setup.AppendLine ("{0} res = {{0}};", rettype);
				break;
			}
			var isArray = returntype is ArrayType;
			var type = returntype.Resolve () ?? returntype;
			var retain = method.RetainReturnValue;

			if (returntype != method.NativeReturnType) {
				body_setup.AppendLine ("MonoClass *retparamclass = NULL;");
				cleanup.AppendLine ("xamarin_mono_object_release (&retparamclass);");
				body_setup.AppendLine ("MonoType *retparamtype = NULL;");
				cleanup.AppendLine ("xamarin_mono_object_release (&retparamtype);");
				setup_call_stack.AppendLine ("retparamtype = xamarin_get_parameter_type (managed_method, -1);");
				setup_call_stack.AppendLine ("retparamclass = mono_class_from_mono_type (retparamtype);");
				GenerateConversionToNative (returntype, method.NativeReturnType, setup_return, descriptiveMethodName, ref exceptions, method, "retval", "res", "retparamclass");
			} else if (returntype.IsValueType) {
				setup_return.AppendLine ("res = *({0} *) mono_object_unbox ((MonoObject *) retval);", rettype);
			} else if (isArray) {
				var elementType = ((ArrayType) returntype).ElementType;
				var conversion_func = string.Empty;
				if (elementType.FullName == "System.String") {
					conversion_func = "xamarin_managed_string_array_to_nsarray";
				} else if (IsNSObject (elementType)) {
					conversion_func = "xamarin_managed_nsobject_array_to_nsarray";
				} else if (IsINativeObject (elementType)) {
					conversion_func = "xamarin_managed_inativeobject_array_to_nsarray";
				} else {
					throw ErrorHelper.CreateError (App, 4111, method.Method, Errors.MT4111, method.NativeReturnType.FullName, descriptiveMethodName);
				}
				setup_return.AppendLine ("res = {0} ((MonoArray *) retval, &exception_gchandle);", conversion_func);
				if (retain)
					setup_return.AppendLine ("[res retain];");
				setup_return.AppendLine ("if (exception_gchandle != INVALID_GCHANDLE) goto exception_handling;");
				setup_return.AppendLine ("xamarin_framework_peer_waypoint ();");
				setup_return.AppendLine ("mt_dummy_use (retval);");
			} else {
				setup_return.AppendLine ("if (!retval) {");
				setup_return.AppendLine ("res = NULL;");
				setup_return.AppendLine ("} else {");

				if (IsNSObject (type)) {
					setup_return.AppendLine ("id retobj;");
					setup_return.AppendLine ("retobj = xamarin_get_nsobject_handle (retval);");
					setup_return.AppendLine ("xamarin_framework_peer_waypoint ();");
					setup_return.AppendLine ("[retobj retain];");
					if (!retain)
						setup_return.AppendLine ("[retobj autorelease];");
					setup_return.AppendLine ("mt_dummy_use (retval);");
					setup_return.AppendLine ("res = retobj;");
				} else if (IsPlatformType (type, "ObjCRuntime", "Selector")) {
					setup_return.AppendLine ("res = (SEL) xamarin_get_handle_for_inativeobject (retval, &exception_gchandle);");
					setup_return.AppendLine ("if (exception_gchandle != INVALID_GCHANDLE) goto exception_handling;");
				} else if (IsPlatformType (type, "ObjCRuntime", "Class")) {
					setup_return.AppendLine ("res = (Class) xamarin_get_handle_for_inativeobject (retval, &exception_gchandle);");
					setup_return.AppendLine ("if (exception_gchandle != INVALID_GCHANDLE) goto exception_handling;");
				} else if (IsNativeObject (type)) {
					setup_return.AppendLine ("{0} retobj;", rettype);
					setup_return.AppendLine ("retobj = xamarin_get_handle_for_inativeobject ((MonoObject *) retval, &exception_gchandle);");
					setup_return.AppendLine ("if (exception_gchandle != INVALID_GCHANDLE) goto exception_handling;");
					setup_return.AppendLine ("xamarin_framework_peer_waypoint ();");
					setup_return.AppendLine ("if (retobj != NULL) {");
					if (retain) {
						setup_return.AppendLine ("xamarin_retain_nativeobject (retval, &exception_gchandle);");
						setup_return.AppendLine ("if (exception_gchandle != INVALID_GCHANDLE) goto exception_handling;");
					} else {
						// If xamarin_attempt_retain_nsobject returns true, the input is an NSObject, so it's safe to call the 'autorelease' selector on it.
						// We don't retain retval if it's not an NSObject, because we'd have to immediately release it,
						// and that serves no purpose.
						setup_return.AppendLine ("bool retained = xamarin_attempt_retain_nsobject (retval, &exception_gchandle);");
						setup_return.AppendLine ("if (exception_gchandle != INVALID_GCHANDLE) goto exception_handling;");
						setup_return.AppendLine ("if (retained) {");
						setup_return.AppendLine ("[retobj autorelease];");
						setup_return.AppendLine ("}");
					}
					setup_return.AppendLine ("mt_dummy_use (retval);");
					setup_return.AppendLine ("res = retobj;");
					setup_return.AppendLine ("} else {");
					setup_return.AppendLine ("res = NULL;");
					setup_return.AppendLine ("}");
				} else if (type.FullName == "System.String") {
					// This should always be an NSString and never char*
					setup_return.AppendLine ("res = xamarin_string_to_nsstring ((MonoString *) retval, {0});", retain ? "true" : "false");
				} else if (IsDelegate (type.Resolve ())) {
					var signature = "NULL";
					var token = "INVALID_TOKEN_REF";
					if (App.Optimizations.OptimizeBlockLiteralSetupBlock == true) {
						if (type.Is ("System", "Delegate") || type.Is ("System", "MulticastDelegate")) {
							ErrorHelper.Show (ErrorHelper.CreateWarning (App, 4173, method.Method, Errors.MT4173, type.FullName, descriptiveMethodName));
						} else {
							var delegateMethod = type.Resolve ().GetMethods ().FirstOrDefault ((v) => v.Name == "Invoke");
							if (delegateMethod is null) {
								ErrorHelper.Show (ErrorHelper.CreateWarning (App, 4173, method.Method, Errors.MT4173_A, type.FullName, descriptiveMethodName));
							} else {
								signature = "\"" + ComputeSignature (method.DeclaringType.Type, null, method, isBlockSignature: true) + "\"";
							}
						}
						var delegateProxyType = GetDelegateProxyType (method);
						if (delegateProxyType is null) {
							exceptions.Add (ErrorHelper.CreateWarning (App, 4176, method.Method, "Unable to locate the delegate to block conversion type for the return value of the method {0}.", method.DescriptiveMethodName));
						} else if (TryCreateTokenReference (delegateProxyType, TokenType.TypeDef, out var delegate_proxy_type_token_ref, out _)) {
							token = $"0x{delegate_proxy_type_token_ref:X} /* {delegateProxyType.FullName} */ ";
						}
					}
					setup_return.AppendLine ("res = xamarin_get_block_for_delegate (managed_method, retval, {0}, {1}, &exception_gchandle);", signature, token);
					setup_return.AppendLine ("if (exception_gchandle != INVALID_GCHANDLE) goto exception_handling;");
				} else {
					throw ErrorHelper.CreateError (4104, Errors.MT4104, returntype.FullName, descriptiveMethodName);
				}

				setup_return.AppendLine ("}");
			}
		}

		void GenerateCallToSuperForConstructor (AutoIndentStringBuilder sb, ObjCMethod method, List<Exception> exceptions)
		{
			sb.WriteLine ("if (call_super && rv) {");
			sb.Write ("struct objc_super super = {  rv, [").Write (method.DeclaringType.SuperType.ExportedName).WriteLine (" class] };");
			sb.Write ("rv = ((id (*)(objc_super*, SEL");

			if (method.Parameters is not null) {
				for (int i = 0; i < method.Parameters.Length; i++)
					sb.Append (", ").Append (ToObjCParameterType (method.Parameters [i], method.DescriptiveMethodName, exceptions, method.Method));
			}
			if (method.IsVariadic)
				sb.Append (", ...");

			sb.Write (")) objc_msgSendSuper) (&super, @selector (");
			sb.Write (method.Selector);
			sb.Write (")");
			var split = method.Selector.Split (':');
			for (int i = 0; i < split.Length - 1; i++) {
				sb.Append (", ");
				sb.AppendFormat ("p{0}", i);
			}
			sb.WriteLine (");");
			sb.WriteLine ("}");
		}

		public TypeDefinition GetInstantiableType (TypeDefinition td, List<Exception> exceptions, string descriptiveMethodName)
		{
			return GetInstantiableType (td, exceptions, descriptiveMethodName, out var _);
		}

		public TypeDefinition GetInstantiableType (TypeDefinition td, List<Exception> exceptions, string descriptiveMethodName, out MethodDefinition ctor)
		{
			TypeDefinition nativeObjType = td;

			if (td.IsInterface) {
				var wrapper_type = GetProtocolAttributeWrapperType (td);
				if (wrapper_type is null)
					throw ErrorHelper.CreateError (4125, Errors.MT4125, td.FullName, descriptiveMethodName);

				nativeObjType = wrapper_type.Resolve ();
			}

			// verify that the type has a ctor with two parameters
			if (!TryGetIntPtrBoolCtor (nativeObjType, exceptions, out ctor))
				throw ErrorHelper.CreateError (4103, Errors.MT4103, nativeObjType.FullName, descriptiveMethodName);

			return nativeObjType;
		}

		// This method finds the CreateBlock method generated by the generator.
		public MethodDefinition GetCreateBlockMethod (TypeDefinition delegateProxyType)
		{
			if (!delegateProxyType.HasMethods)
				return null;

			foreach (var method in delegateProxyType.Methods) {
				if (method.Name != "CreateBlock")
					continue;
				if (!method.ReturnType.Is ("ObjCRuntime", "BlockLiteral"))
					continue;
				if (!method.HasParameters)
					continue;
				if (method.Parameters.Count != 1)
					continue;
				if (!IsDelegate (method.Parameters [0].ParameterType))
					continue;

				return method;
			}

			return null;
		}

		public TypeDefinition GetDelegateProxyType (ObjCMethod obj_method)
		{
			// A mirror of this method is also implemented in BlockLiteral:GetDelegateProxyType
			// If this method is changed, that method will probably have to be updated too (tests!!!)
			MethodDefinition method = obj_method.Method;
			MethodDefinition first = method;
			MethodDefinition last = null;
			while (method != last) {
				last = method;
				var delegateProxyType = GetDelegateProxyAttribute (method);
				if (delegateProxyType?.DelegateType is not null)
					return delegateProxyType.DelegateType;

				method = GetBaseMethodInTypeHierarchy (method);
			}

			// Might be the implementation of an interface method, so find the corresponding
			// MethodDefinition for the interface, and check for DelegateProxy attributes there as well.
			var map = PrepareMethodMapping (first.DeclaringType);
			if (map is not null && map.TryGetValue (first, out var list)) {
				if (list.Count != 1)
					throw new AggregateException (Shared.GetMT4127 (first, list));
				var delegateProxyType = GetDelegateProxyAttribute (list [0]);
				if (delegateProxyType?.DelegateType is not null)
					return delegateProxyType.DelegateType;
			}

			// Might be an implementation of an optional protocol member.
			var allProtocols = obj_method.DeclaringType.AllProtocolsInHierarchy;
			if (allProtocols is not null) {
				string selector = null;

				foreach (var proto in allProtocols) {
					// We store the DelegateProxy type in the ProtocolMemberAttribute, so check those.
					if (selector is null)
						selector = obj_method.Selector ?? string.Empty;
					if (selector is not null) {
						var attrib = GetProtocolMemberAttribute (proto.Type, selector, obj_method, method);
						if (attrib?.ReturnTypeDelegateProxy is not null)
							return attrib.ReturnTypeDelegateProxy.Resolve ();
					}
				}
			}

			return null;
		}

		//
		// Returns a MethodInfo that represents the method that can be used to turn
		// a the block in the given method at the given parameter into a strongly typed
		// delegate
		//
		public MethodDefinition GetBlockWrapperCreator (ObjCMethod obj_method, int parameter)
		{
			// A mirror of this method is also implemented in Runtime:GetBlockWrapperCreator
			// If this method is changed, that method will probably have to be updated too (tests!!!)
			MethodDefinition method = obj_method.Method;
			MethodDefinition first = method;
			MethodDefinition last = null;
			while (method != last) {
				last = method;
				var createMethod = GetBlockProxyAttributeMethod (method, parameter);
				if (createMethod is not null)
					return createMethod;

				method = GetBaseMethodInTypeHierarchy (method);
			}

			// Might be the implementation of an interface method, so find the corresponding
			// MethodDefinition for the interface, and check for BlockProxy attributes there as well.
			var map = PrepareMethodMapping (first.DeclaringType);
			if (map is not null && map.TryGetValue (first, out var list)) {
				if (list.Count != 1)
					throw new AggregateException (Shared.GetMT4127 (first, list));
				var createMethod = GetBlockProxyAttributeMethod (list [0], parameter);
				if (createMethod is not null)
					return createMethod;
			}

			// Might be an implementation of an optional protocol member.
			var allProtocols = obj_method.DeclaringType.AllProtocolsInHierarchy;
			if (allProtocols is not null) {
				string selector = null;

				foreach (var proto in allProtocols) {
					// We store the BlockProxy type in the ProtocolMemberAttribute, so check those.
					// We may run into binding assemblies built with earlier versions of the generator,
					// which means we can't rely on finding the BlockProxy attribute in the ProtocolMemberAttribute.
					if (selector is null)
						selector = obj_method.Selector ?? string.Empty;
					if (selector is not null) {
						var attrib = GetProtocolMemberAttribute (proto.Type, selector, obj_method, method);
						if (attrib?.ParameterBlockProxy?.Length > parameter && attrib.ParameterBlockProxy [parameter] is not null)
							return attrib.ParameterBlockProxy [parameter].Resolve ().Methods.First ((v) => v.Name == "Create");
					}

					if (proto.Methods is not null) {
						foreach (var pMethod in proto.Methods) {
							if (!pMethod.IsOptional)
								continue;
							if (pMethod.Name != method.Name)
								continue;
							if (!TypeMatch (pMethod.ReturnType, method.ReturnType))
								continue;
							if (!ParametersMatch (method.Parameters, pMethod.Parameters))
								continue;

							MethodDefinition extensionMethod = pMethod.Method;
							if (extensionMethod is null) {
								MapProtocolMember (obj_method.Method, out extensionMethod);
								if (extensionMethod is null)
									return null;
							}

							var createMethod = GetBlockProxyAttributeMethod (extensionMethod, parameter + 1);
							if (createMethod is not null)
								return createMethod;
						}
					}

				}
			}

			return null;
		}

		public bool TryFindType (TypeDefinition type, [NotNullWhen (true)] out ObjCType? objcType)
		{
			return Types.TryGetValue (type, out objcType);
		}

		public bool TryFindMethod (MethodDefinition method, [NotNullWhen (true)] out ObjCMethod? objcMethod)
		{
			if (TryFindType (method.DeclaringType, out var type)) {
				if (type.Methods is not null) {
					foreach (var m in type.Methods) {
						if ((object) m.Method == (object) method) {
							objcMethod = m;
							return true;
						}
					}
				}
			}
			objcMethod = null;
			return false;
		}

		MethodDefinition GetBlockProxyAttributeMethod (MethodDefinition method, int parameter)
		{
			var param = method.Parameters [parameter];
			var attrib = GetBlockProxyAttribute (param);
			if (attrib is null)
				return null;

			var createMethod = attrib.Type.Methods.FirstOrDefault ((v) => v.Name == "Create");
			if (createMethod is null) {
				// This may happen if users add their own BlockProxy attributes and don't know which types to pass.
				// One common variation is that the IDE will add the BlockProxy attribute found in base methods when the user overrides those methods,
				// which unfortunately doesn't compile (because the type passed to the BlockProxy attribute is internal), and then
				// the user just modifies the attribute to something that compiles.
				ErrorHelper.Show (ErrorHelper.CreateWarning (App, 4175, method, $"{(string.IsNullOrEmpty (param.Name) ? $"Parameter #{param.Index + 1}" : $"The parameter '{param.Name}'")} in the method '{GetTypeFullName (method.DeclaringType)}.{GetDescriptiveMethodName (method)}' has an invalid BlockProxy attribute (the type passed to the attribute does not have a 'Create' method)."));
				// Returning null will make the caller look for the attribute in the base implementation.
			}
			return createMethod;
		}

		public bool MapProtocolMember (MethodDefinition method, out MethodDefinition extensionMethod)
		{
			// Given 'method', finds out if it's the implementation of an optional protocol method,
			// and if so, return the corresponding IProtocol_Extensions method.
			extensionMethod = null;

			if (!method.HasCustomAttributes)
				return false;

			var type = method.DeclaringType;
			while (type is not null && (object) type != (object) type.BaseType) {
				if (MapProtocolMember (type, method, out extensionMethod))
					return true;

				type = type.BaseType?.Resolve ();
			}

			return false;
		}

		public bool MapProtocolMember (TypeDefinition t, MethodDefinition method, out MethodDefinition extensionMethod)
		{
			extensionMethod = null;

			if (!t.HasInterfaces)
				return false;

			// special processing to find [BlockProxy] attributes in _Extensions types
			// ref: https://bugzilla.xamarin.com/show_bug.cgi?id=23540
			string selector = null;
			foreach (var r in t.Interfaces) {
				var i = r.InterfaceType.Resolve ();
				if (i is null || !HasAttribute (i, Namespaces.Foundation, "ProtocolAttribute"))
					continue;
				if (selector is null) {
					// delay and don't compute each time
					var ea = CreateExportAttribute (method);
					selector = ea?.Selector;
				}
				string name = null;
				bool match = false;
				ICustomAttribute protocolMemberAttribute = null;
				foreach (var ca in GetCustomAttributes (i, Foundation, StringConstants.ProtocolMemberAttribute)) {
					foreach (var p in ca.Properties) {
						switch (p.Name) {
						case "Selector":
							match = (p.Argument.Value as string == selector);
							break;
						case "Name":
							name = p.Argument.Value as string;
							break;
						}
					}
					if (match) {
						protocolMemberAttribute = ca;
						break;
					}
				}
				if (!match || name is null)
					continue;
				// _Extensions time...
				var td = i.Module.GetType (i.Namespace, i.Name.Substring (1) + "_Extensions");
				if (td is not null && td.HasMethods) {
					foreach (var m in td.Methods) {
						if (!m.HasParameters || (m.Name != name) || !m.IsOptimizableCode (LinkContext))
							continue;
						bool proxy = false;
						match = method.Parameters.Count == m.Parameters.Count - 1;
						if (match) {
							for (int n = 1; n < m.Parameters.Count; n++) {
								var p = m.Parameters [n];
								var pt = p.ParameterType;
								match &= method.Parameters [n - 1].ParameterType.Is (pt.Namespace, pt.Name);
								proxy |= p.HasCustomAttribute (Namespaces.ObjCRuntime, "BlockProxyAttribute");
							}
						}
						if (match && proxy) {
							ProtocolMemberMethodMap [protocolMemberAttribute] = m;
							extensionMethod = m;
							return true;
						}
					}
				}
			}

			return false;
		}

		public string GetManagedToNSNumberFunc (TypeReference managedType, TypeReference inputType, TypeReference outputType, string descriptiveMethodName)
		{
			var typeName = managedType.FullName;
			switch (typeName) {
			case "System.SByte": return "xamarin_sbyte_to_nsnumber";
			case "System.Byte": return "xamarin_byte_to_nsnumber";
			case "System.Int16": return "xamarin_short_to_nsnumber";
			case "System.UInt16": return "xamarin_ushort_to_nsnumber";
			case "System.Int32": return "xamarin_int_to_nsnumber";
			case "System.UInt32": return "xamarin_uint_to_nsnumber";
			case "System.Int64": return "xamarin_long_to_nsnumber";
			case "System.UInt64": return "xamarin_ulong_to_nsnumber";
			case "System.IntPtr":
			case "System.nint": return "xamarin_nint_to_nsnumber";
			case "System.UIntPtr":
			case "System.nuint": return "xamarin_nuint_to_nsnumber";
			case "System.Single": return "xamarin_float_to_nsnumber";
			case "System.Double": return "xamarin_double_to_nsnumber";
			case "System.Boolean": return "xamarin_bool_to_nsnumber";
			default:
				if (typeName == NFloatTypeName)
					return "xamarin_nfloat_to_nsnumber";
				if (IsEnum (managedType))
					return GetManagedToNSNumberFunc (GetEnumUnderlyingType (managedType), inputType, outputType, descriptiveMethodName);
				throw ErrorHelper.CreateError (99, Errors.MX0099, $"can't convert from '{inputType.FullName}' to '{outputType.FullName}' in {descriptiveMethodName}");
			}
		}

		public string GetNSNumberToManagedFunc (TypeReference managedType, TypeReference inputType, TypeReference outputType, string descriptiveMethodName, out string nativeType)
		{
			var typeName = managedType.FullName;
			switch (typeName) {
			case "System.SByte": nativeType = "int8_t"; return "xamarin_nsnumber_to_sbyte";
			case "System.Byte": nativeType = "uint8_t"; return "xamarin_nsnumber_to_byte";
			case "System.Int16": nativeType = "int16_t"; return "xamarin_nsnumber_to_short";
			case "System.UInt16": nativeType = "uint16_t"; return "xamarin_nsnumber_to_ushort";
			case "System.Int32": nativeType = "int32_t"; return "xamarin_nsnumber_to_int";
			case "System.UInt32": nativeType = "uint32_t"; return "xamarin_nsnumber_to_uint";
			case "System.Int64": nativeType = "int64_t"; return "xamarin_nsnumber_to_long";
			case "System.UInt64": nativeType = "uint64_t"; return "xamarin_nsnumber_to_ulong";
			case "System.IntPtr":
			case "System.nint": nativeType = "NSInteger"; return "xamarin_nsnumber_to_nint";
			case "System.UIntPtr":
			case "System.nuint": nativeType = "NSUInteger"; return "xamarin_nsnumber_to_nuint";
			case "System.Single": nativeType = "float"; return "xamarin_nsnumber_to_float";
			case "System.Double": nativeType = "double"; return "xamarin_nsnumber_to_double";
			case "System.Boolean": nativeType = "BOOL"; return "xamarin_nsnumber_to_bool";
			default:
				if (typeName == NFloatTypeName) {
					nativeType = "CGFloat";
					return "xamarin_nsnumber_to_nfloat";
				}
				if (IsEnum (managedType))
					return GetNSNumberToManagedFunc (GetEnumUnderlyingType (managedType), inputType, outputType, descriptiveMethodName, out nativeType);
				throw ErrorHelper.CreateError (99, Errors.MX0099, $"can't convert from '{inputType.FullName}' to '{outputType.FullName}' in {descriptiveMethodName}");
			}
		}

		public string GetNSValueToManagedFunc (TypeReference managedType, TypeReference inputType, TypeReference outputType, string descriptiveMethodName, out string nativeType)
		{
			var underlyingTypeName = managedType.FullName;

			switch (underlyingTypeName) {
			case "Foundation.NSRange": nativeType = "NSRange"; return "xamarin_nsvalue_to_nsrange";
			case "CoreGraphics.CGAffineTransform": nativeType = "CGAffineTransform"; return "xamarin_nsvalue_to_cgaffinetransform";
			case "CoreGraphics.CGPoint": nativeType = "CGPoint"; return "xamarin_nsvalue_to_cgpoint";
			case "CoreGraphics.CGRect": nativeType = "CGRect"; return "xamarin_nsvalue_to_cgrect";
			case "CoreGraphics.CGSize": nativeType = "CGSize"; return "xamarin_nsvalue_to_cgsize";
			case "CoreGraphics.CGVector": nativeType = "CGVector"; return "xamarin_nsvalue_to_cgvector";
			case "CoreAnimation.CATransform3D": nativeType = "CATransform3D"; return "xamarin_nsvalue_to_catransform3d";
			case "CoreLocation.CLLocationCoordinate2D": nativeType = "CLLocationCoordinate2D"; return "xamarin_nsvalue_to_cllocationcoordinate2d";
			case "CoreMedia.CMTime": nativeType = "CMTime"; return "xamarin_nsvalue_to_cmtime";
			case "CoreMedia.CMTimeMapping": nativeType = "CMTimeMapping"; return "xamarin_nsvalue_to_cmtimemapping";
			case "CoreMedia.CMTimeRange": nativeType = "CMTimeRange"; return "xamarin_nsvalue_to_cmtimerange";
			case "CoreMedia.CMVideoDimensions": nativeType = "CMVideoDimensions"; return "xamarin_nsvalue_to_cmvideodimensions";
			case "MapKit.MKCoordinateSpan": nativeType = "MKCoordinateSpan"; return "xamarin_nsvalue_to_mkcoordinatespan";
			case "SceneKit.SCNMatrix4": nativeType = "SCNMatrix4"; return "xamarin_nsvalue_to_scnmatrix4";
			case "SceneKit.SCNVector3": nativeType = "SCNVector3"; return "xamarin_nsvalue_to_scnvector3";
			case "SceneKit.SCNVector4": nativeType = "SCNVector4"; return "xamarin_nsvalue_to_scnvector4";
			case "UIKit.UIEdgeInsets": nativeType = "UIEdgeInsets"; return "xamarin_nsvalue_to_uiedgeinsets";
			case "UIKit.UIOffset": nativeType = "UIOffset"; return "xamarin_nsvalue_to_uioffset";
			case "UIKit.NSDirectionalEdgeInsets": nativeType = "NSDirectionalEdgeInsets"; return "xamarin_nsvalue_to_nsdirectionaledgeinsets";
			default:
				throw ErrorHelper.CreateError (99, Errors.MX0099, $"can't convert from '{inputType.FullName}' to '{outputType.FullName}' in {descriptiveMethodName}");
			}
		}

		public string GetManagedToNSValueFunc (TypeReference managedType, TypeReference inputType, TypeReference outputType, string descriptiveMethodName)
		{
			var underlyingTypeName = managedType.FullName;

			switch (underlyingTypeName) {
			case "Foundation.NSRange": return "xamarin_nsrange_to_nsvalue";
			case "CoreGraphics.CGAffineTransform": return "xamarin_cgaffinetransform_to_nsvalue";
			case "CoreGraphics.CGPoint": return "xamarin_cgpoint_to_nsvalue";
			case "CoreGraphics.CGRect": return "xamarin_cgrect_to_nsvalue";
			case "CoreGraphics.CGSize": return "xamarin_cgsize_to_nsvalue";
			case "CoreGraphics.CGVector": return "xamarin_cgvector_to_nsvalue";
			case "CoreAnimation.CATransform3D": return "xamarin_catransform3d_to_nsvalue";
			case "CoreLocation.CLLocationCoordinate2D": return "xamarin_cllocationcoordinate2d_to_nsvalue";
			case "CoreMedia.CMTime": return "xamarin_cmtime_to_nsvalue";
			case "CoreMedia.CMTimeMapping": return "xamarin_cmtimemapping_to_nsvalue";
			case "CoreMedia.CMTimeRange": return "xamarin_cmtimerange_to_nsvalue";
			case "CoreMedia.CMVideoDimensions": return "xamarin_cmvideodimensions_to_nsvalue";
			case "MapKit.MKCoordinateSpan": return "xamarin_mkcoordinatespan_to_nsvalue";
			case "SceneKit.SCNMatrix4": return "xamarin_scnmatrix4_to_nsvalue";
			case "SceneKit.SCNVector3": return "xamarin_scnvector3_to_nsvalue";
			case "SceneKit.SCNVector4": return "xamarin_scnvector4_to_nsvalue";
			case "UIKit.UIEdgeInsets": return "xamarin_uiedgeinsets_to_nsvalue";
			case "UIKit.UIOffset": return "xamarin_uioffset_to_nsvalue";
			case "UIKit.NSDirectionalEdgeInsets": return "xamarin_nsdirectionaledgeinsets_to_nsvalue";
			default:
				throw ErrorHelper.CreateError (99, Errors.MX0099, $"can't convert from '{inputType.FullName}' to '{outputType.FullName}' in {descriptiveMethodName}");
			}
		}

		string GetNSStringToSmartEnumFunc (TypeReference managedType, TypeReference inputType, TypeReference outputType, string descriptiveMethodName, string parameterClass, out string nativeType)
		{
			nativeType = "NSString *";
			return $"xamarin_get_nsstring_to_smart_enum_func ({parameterClass}, managed_method, &exception_gchandle)";
		}

		string GetSmartEnumToNSStringFunc (TypeReference managedType, TypeReference inputType, TypeReference outputType, string descriptiveMethodName, string parameterClass)
		{
			return $"xamarin_get_smart_enum_to_nsstring_func ({parameterClass}, managed_method, &exception_gchandle)";
		}

		void GenerateConversionToManaged (TypeReference inputType, TypeReference outputType, AutoIndentStringBuilder sb, string descriptiveMethodName, ref List<Exception> exceptions, ObjCMethod method, string inputName, string outputName, string managedClassExpression, int parameter)
		{
			// This is a mirror of the native method xamarin_generate_conversion_to_managed (for the dynamic registrar).
			// It's also a mirror of the method ManagedRegistrarStep.GenerateConversionToManaged.
			// These methods must be kept in sync.
			var managedType = outputType;
			var nativeType = inputType;

			var isManagedNullable = IsNullable (managedType);

			var underlyingManagedType = managedType;
			var underlyingNativeType = nativeType;

			var isManagedArray = IsArray (managedType);
			var isNativeArray = IsArray (nativeType);

			if (isManagedArray != isNativeArray)
				throw ErrorHelper.CreateError (99, Errors.MX0099, $"can't convert from '{inputType.FullName}' to '{outputType.FullName}' in {descriptiveMethodName}");

			var classVariableName = $"{inputName}_conv_class";
			body_setup.AppendLine ($"MonoClass *{classVariableName} = NULL;");
			if (isManagedArray) {
				if (isManagedNullable)
					throw ErrorHelper.CreateError (99, Errors.MX0099, $"can't convert from '{inputType.FullName}' to '{outputType.FullName}' in {descriptiveMethodName}");
				underlyingNativeType = GetElementType (nativeType);
				underlyingManagedType = GetElementType (managedType);
				sb.AppendLine ($"{classVariableName} = mono_class_get_element_class ({managedClassExpression});");
				cleanup.AppendLine ($"xamarin_mono_object_release (&{classVariableName});");
			} else if (isManagedNullable) {
				underlyingManagedType = GetNullableType (managedType);
				sb.AppendLine ($"{classVariableName} = xamarin_get_nullable_type ({managedClassExpression}, &exception_gchandle);");
				sb.AppendLine ($"if (exception_gchandle != INVALID_GCHANDLE) goto exception_handling;");
				cleanup.AppendLine ($"xamarin_mono_object_release (&{classVariableName});");
			} else {
				sb.AppendLine ($"{classVariableName} = {managedClassExpression};");
			}

			CheckNamespace (underlyingNativeType.Resolve (), exceptions);
			CheckNamespace (underlyingManagedType.Resolve (), exceptions);

			if (isManagedNullable || isManagedArray)
				sb.AppendLine ($"if ({inputName}) {{");

			string func;
			string nativeTypeName;
			string token = "0";
			if (underlyingNativeType.Is (Foundation, "NSNumber")) {
				func = GetNSNumberToManagedFunc (underlyingManagedType, inputType, outputType, descriptiveMethodName, out nativeTypeName);
			} else if (underlyingNativeType.Is (Foundation, "NSValue")) {
				func = GetNSValueToManagedFunc (underlyingManagedType, inputType, outputType, descriptiveMethodName, out nativeTypeName);
			} else if (underlyingNativeType.Is (Foundation, "NSString")) {
				func = GetNSStringToSmartEnumFunc (underlyingManagedType, inputType, outputType, descriptiveMethodName, managedClassExpression, out nativeTypeName);
				MethodDefinition getConstantMethod, getValueMethod;
				if (!IsSmartEnum (underlyingManagedType, out getConstantMethod, out getValueMethod)) {
					// method linked away!? this should already be verified
					ErrorHelper.Show (ErrorHelper.CreateWarning (99, Errors.MX0099, $"the smart enum {underlyingManagedType.FullName} doesn't seem to be a smart enum after all"));
					token = "INVALID_TOKEN_REF";
				} else if (TryCreateTokenReference (getValueMethod, TokenType.Method, out var get_value_method_token_ref, out _)) {
					token = $"0x{get_value_method_token_ref:X} /* {getValueMethod.FullName} */";
				}
			} else {
				throw ErrorHelper.CreateError (99, Errors.MX0099, $"can't convert from '{inputType.FullName}' to '{outputType.FullName}' in {descriptiveMethodName}");
			}
			if (isManagedArray) {
				sb.AppendLine ($"xamarin_id_to_managed_func {inputName}_conv_func = (xamarin_id_to_managed_func) {func};");
				body_setup.AppendLine ("MonoArray *arr_convert_{0} = NULL;", parameter);
				cleanup.AppendLine ("xamarin_mono_object_release (&arr_convert_{0});", parameter);
				sb.AppendLine ("if (exception_gchandle != INVALID_GCHANDLE) goto exception_handling;");
				sb.AppendLine ($"arr_convert_{parameter} = xamarin_convert_nsarray_to_managed_with_func ({inputName}, {classVariableName}, {inputName}_conv_func, GINT_TO_POINTER ({token}), &exception_gchandle);");
				sb.AppendLine ($"{outputName} = arr_convert_{parameter};");
				sb.AppendLine ("if (exception_gchandle != INVALID_GCHANDLE) goto exception_handling;");
			} else {
				var tmpName = $"{inputName}_conv_tmp";
				body_setup.AppendLine ($"{nativeTypeName} {tmpName};");
				if (isManagedNullable) {
					var tmpName2 = $"{inputName}_conv_ptr";
					body_setup.AppendLine ($"void *{tmpName2} = NULL;");
					sb.AppendLine ($"{tmpName2} = {func} ({inputName}, &{tmpName}, {classVariableName}, GINT_TO_POINTER ({token}), &exception_gchandle);");
					sb.AppendLine ("if (exception_gchandle != INVALID_GCHANDLE) goto exception_handling;");
					body_setup.AppendLine ($"MonoObject *mobj_conversion_{parameter} = NULL;");
					sb.AppendLine ($"mobj_conversion_{parameter} = mono_value_box (mono_domain_get (), {classVariableName}, {tmpName2});");
					sb.AppendLine ($"{outputName} = mobj_conversion_{parameter};");
					cleanup.AppendLine ($"xamarin_mono_object_release (&mobj_conversion_{parameter});");
				} else {
					sb.AppendLine ($"{outputName} = {func} ({inputName}, &{tmpName}, {classVariableName}, GINT_TO_POINTER ({token}), &exception_gchandle);");
					sb.AppendLine ("if (exception_gchandle != INVALID_GCHANDLE) goto exception_handling;");
				}
			}

			if (isManagedNullable || isManagedArray) {
				sb.AppendLine ($"}} else {{");
				sb.AppendLine ($"{outputName} = NULL;");
				sb.AppendLine ($"}}");
			}
		}

		void GenerateConversionToNative (TypeReference inputType, TypeReference outputType, AutoIndentStringBuilder sb, string descriptiveMethodName, ref List<Exception> exceptions, ObjCMethod method, string inputName, string outputName, string managedClassExpression)
		{
			// This is a mirror of the native method xamarin_generate_conversion_to_native (for the dynamic registrar).
			// It's also a mirror of the method ManagedRegistrarStep.GenerateConversionToNative.
			// These methods must be kept in sync.
			var managedType = inputType;
			var nativeType = outputType;

			var isManagedNullable = IsNullable (managedType);

			var underlyingManagedType = managedType;
			var underlyingNativeType = nativeType;

			var isManagedArray = IsArray (managedType);
			var isNativeArray = IsArray (nativeType);

			if (isManagedArray != isNativeArray)
				throw ErrorHelper.CreateError (99, Errors.MX0099, $"can't convert from '{inputType.FullName}' to '{outputType.FullName}' in {descriptiveMethodName}");

			var classVariableName = $"{inputName}_conv_class";
			body_setup.AppendLine ($"MonoClass *{classVariableName} = NULL;");
			if (isManagedArray) {
				if (isManagedNullable)
					throw ErrorHelper.CreateError (99, Errors.MX0099, $"can't convert from '{inputType.FullName}' to '{outputType.FullName}' in {descriptiveMethodName}");
				underlyingNativeType = GetElementType (nativeType);
				underlyingManagedType = GetElementType (managedType);
				sb.AppendLine ($"{classVariableName} = mono_class_get_element_class ({managedClassExpression});");
				cleanup.AppendLine ($"xamarin_mono_object_release (&{classVariableName});");
			} else if (isManagedNullable) {
				underlyingManagedType = GetNullableType (managedType);
				sb.AppendLine ($"{classVariableName} = xamarin_get_nullable_type ({managedClassExpression}, &exception_gchandle);");
				sb.AppendLine ($"if (exception_gchandle != INVALID_GCHANDLE) goto exception_handling;");
				cleanup.AppendLine ($"xamarin_mono_object_release (&{classVariableName});");
			} else {
				sb.AppendLine ($"{classVariableName} = {managedClassExpression};");
			}

			CheckNamespace (underlyingNativeType.Resolve (), exceptions);
			CheckNamespace (underlyingManagedType.Resolve (), exceptions);

			if (isManagedNullable || isManagedArray)
				sb.AppendLine ($"if ({inputName}) {{");

			string func;
			string token = "0";
			if (underlyingNativeType.Is (Foundation, "NSNumber")) {
				func = GetManagedToNSNumberFunc (underlyingManagedType, inputType, outputType, descriptiveMethodName);
			} else if (underlyingNativeType.Is (Foundation, "NSValue")) {
				func = GetManagedToNSValueFunc (underlyingManagedType, inputType, outputType, descriptiveMethodName);
			} else if (underlyingNativeType.Is (Foundation, "NSString")) {
				func = GetSmartEnumToNSStringFunc (underlyingManagedType, inputType, outputType, descriptiveMethodName, classVariableName);
				MethodDefinition getConstantMethod, getValueMethod;
				if (!IsSmartEnum (underlyingManagedType, out getConstantMethod, out getValueMethod)) {
					// method linked away!? this should already be verified
					ErrorHelper.Show (ErrorHelper.CreateWarning (99, Errors.MX0099, $"the smart enum {underlyingManagedType.FullName} doesn't seem to be a smart enum after all"));
					token = "INVALID_TOKEN_REF";
				} else if (TryCreateTokenReference (getConstantMethod, TokenType.Method, out var get_constant_method_token_ref, out _)) {
					token = $"0x{get_constant_method_token_ref:X} /* {getConstantMethod.FullName} */";
				}
			} else {
				throw ErrorHelper.CreateError (99, Errors.MX0099, $"can't convert from '{inputType.FullName}' to '{outputType.FullName}' in {descriptiveMethodName}");
			}

			if (isManagedArray) {
				sb.AppendLine ($"{outputName} = xamarin_convert_managed_to_nsarray_with_func ((MonoArray *) {inputName}, (xamarin_managed_to_id_func) {func}, GINT_TO_POINTER ({token}), &exception_gchandle);");
			} else {
				sb.AppendLine ($"{outputName} = {func} ({inputName}, GINT_TO_POINTER ({token}), &exception_gchandle);");
			}
			sb.AppendLine ($"if (exception_gchandle != INVALID_GCHANDLE) goto exception_handling;");

			if (isManagedNullable || isManagedArray) {
				sb.AppendLine ($"}} else {{");
				sb.AppendLine ($"{outputName} = NULL;");
				sb.AppendLine ($"}}");
			}
		}

		class Body {
			public string Code;
			public string Signature;
			public string Name;
			public int Count;

			public override int GetHashCode ()
			{
				return Code.GetHashCode () ^ Signature.GetHashCode ();
			}
			public override bool Equals (object obj)
			{
				var other = obj as Body;
				if (other is null)
					return false;
				return Code == other.Code && Signature == other.Signature;
			}
		}

		bool TryCreateFullTokenReference (MemberReference member, out uint token_ref, out Exception exception)
		{
			switch (member.MetadataToken.TokenType) {
			case TokenType.TypeDef:
			case TokenType.Method:
				break; // OK
			default:
				exception = ErrorHelper.CreateError (99, Errors.MX0099, $"unsupported tokentype ({member.MetadataToken.TokenType}) for {member.FullName}");
				token_ref = INVALID_TOKEN_REF;
				return false;
			}
			var moduleToken = member.Module.MetadataToken.ToUInt32 ();
			var moduleName = member.Module.Name;
			var memberToken = member.MetadataToken.ToUInt32 ();
			var memberName = member.FullName;
			return WriteFullTokenReference (member.Module.Assembly, moduleToken, moduleName, memberToken, memberName, out token_ref, out exception);
		}

		bool WriteFullTokenReference (AssemblyDefinition assembly, uint moduleToken, string moduleName, uint memberToken, string memberName, out uint token_ref, out Exception exception)
		{
			token_ref = (full_token_reference_count++ << 1) + 1;
			var assemblyIndex = registered_assemblies.FindIndex (v => v.Assembly == assembly);
			if (assemblyIndex == -1) {
				exception = ErrorHelper.CreateError (99, Errors.MX0099, $"Could not find {assembly.Name.Name} in the list of registered assemblies when processing {memberName}:\n\t{string.Join ("\n\t", registered_assemblies.Select (v => v.Assembly.Name.Name))}");
				return false;
			}
			var assemblyName = registered_assemblies [assemblyIndex].Name;
			exception = null;
			full_token_references.Append ($"\t\t{{ /* #{full_token_reference_count} = 0x{token_ref:X} */ {assemblyIndex} /* {assemblyName} */, 0x{moduleToken:X} /* {moduleName} */, 0x{memberToken:X} /* {memberName} */ }},\n");
			return true;
		}

		Dictionary<Tuple<MemberReference, TokenType>, uint> token_ref_cache = new Dictionary<Tuple<MemberReference, TokenType>, uint> ();
		bool TryCreateTokenReference (MemberReference member, TokenType implied_type, out uint token_ref, List<Exception> exceptions)
		{
			var rv = TryCreateTokenReference (member, implied_type, out token_ref, out var ex);
			if (!rv)
				exceptions.Add (ex);
			return rv;
		}

		bool TryCreateTokenReference (MemberReference member, TokenType implied_type, out uint token_ref, out Exception exception)
		{
			var key = new Tuple<MemberReference, TokenType> (member, implied_type);
			exception = null;
			if (!token_ref_cache.TryGetValue (key, out token_ref)) {
				if (!TryCreateTokenReferenceUncached (member, implied_type, out token_ref, out exception))
					return false;
				token_ref_cache [key] = token_ref;
			}
			return true;
		}

		bool TryCreateTokenReferenceUncached (MemberReference member, TokenType implied_type, out uint token_ref, out Exception exception)
		{
			var token = member.MetadataToken;

#if NET
			if (App.Registrar == RegistrarMode.ManagedStatic) {
				if (implied_type == TokenType.TypeDef && member is TypeDefinition td) {
					if (App.Configuration.AssemblyTrampolineInfos.TryGetValue (td.Module.Assembly, out var infos) && infos.TryGetRegisteredTypeIndex (td, out var id)) {
						id = id | (uint) TokenType.TypeDef;
						return WriteFullTokenReference (member.Module.Assembly, INVALID_TOKEN_REF, member.Module.Name, id, member.FullName, out token_ref, out exception);
					}
					throw ErrorHelper.CreateError (99, $"Can't create a token reference to an unregistered type when using the managed static registrar: {member.FullName}");
				}
				if (implied_type == TokenType.Method) {
					throw ErrorHelper.CreateError (99, $"Can't create a token reference to a method when using the managed static registrar: {member.FullName}");
				}
				throw ErrorHelper.CreateError (99, "Can't create a token reference to a token type {0} when using the managed static registrar.", implied_type.ToString ());
			}
#endif

			/* We can't create small token references if we're in partial mode, because we may have multiple arrays of registered assemblies, and no way of saying which one we refer to with the assembly index */
			if (IsSingleAssembly)
				return TryCreateFullTokenReference (member, out token_ref, out exception);

			/* If the implied token type doesn't match, we need a full token */
			if (implied_type != token.TokenType)
				return TryCreateFullTokenReference (member, out token_ref, out exception);

			/* For small token references the only valid module is the first one */
			if (member.Module.MetadataToken.ToInt32 () != 1)
				return TryCreateFullTokenReference (member, out token_ref, out exception);

			/* The assembly must be a registered one, and only within the first 128 assemblies */
			var assembly_name = GetAssemblyName (member.Module.Assembly);
			var index = registered_assemblies.FindIndex (v => v.Name == assembly_name);
			if (index < 0 || index > 127)
				return TryCreateFullTokenReference (member, out token_ref, out exception);

			token_ref = (token.RID << 8) + ((uint) index << 1);
			exception = null;
			return true;
		}

		public void GeneratePInvokeWrappersStart (AutoIndentStringBuilder hdr, AutoIndentStringBuilder decls, AutoIndentStringBuilder mthds, AutoIndentStringBuilder ifaces)
		{
			header = hdr;
			declarations = decls;
			methods = mthds;
			interfaces = ifaces;
		}

		public void GeneratePInvokeWrappersEnd ()
		{
			header = null;
			declarations = null;
			methods = null;
			interfaces = null;
			namespaces.Clear ();
			structures.Clear ();

			FlushTrace ();
		}

		static string GetParamName (MethodDefinition method, int i)
		{
			var p = method.Parameters [i];
			if (p.Name is not null)
				return p.Name;
			return "__p__" + i.ToString ();
		}

		string TryGeneratePInvokeWrapper (PInvokeWrapperGenerator state, MethodDefinition method)
		{
			var signatures = state.signatures;
			var exceptions = state.exceptions;
			var signature = state.signature;
			var names = state.names;
			var sb = state.sb;
			var pinfo = method.PInvokeInfo;
			var is_stret = pinfo.EntryPoint.EndsWith ("_stret", StringComparison.Ordinal);
			var isVoid = method.ReturnType.FullName == "System.Void";
			var descriptiveMethodName = method.DeclaringType.FullName + "." + method.Name;

			signature.Clear ();

			string native_return_type;
			int first_parameter = 0;

			if (is_stret) {
				native_return_type = ToObjCParameterType (method.Parameters [0].ParameterType.GetElementType (), descriptiveMethodName, exceptions, method);
				first_parameter = 1;
			} else {
				native_return_type = ToObjCParameterType (method.ReturnType, descriptiveMethodName, exceptions, method);
			}

			signature.Append (native_return_type);
			signature.Append (" ");
			signature.Append (pinfo.EntryPoint);
			signature.Append (" (");
			for (int i = 0; i < method.Parameters.Count; i++) {
				if (i > 0)
					signature.Append (", ");
				signature.Append (ToObjCParameterType (method.Parameters [i].ParameterType, descriptiveMethodName, exceptions, method));
			}
			signature.Append (")");

			string wrapperName;
			if (!signatures.TryGetValue (signature.ToString (), out wrapperName)) {
				var methodName = method.Name.Replace ('<', '_').Replace ('>', '_').Replace ('|', '_');
				var baseName = "xamarin_pinvoke_wrapper_" + methodName;
				var name = baseName;
				var counter = 0;
				while (names.Contains (name)) {
					name = baseName + (++counter).ToString ();
				}
				names.Add (name);
				signatures [signature.ToString ()] = wrapperName = name;

				sb.WriteLine ("// EntryPoint: {0}", pinfo.EntryPoint);
				sb.WriteLine ("// Managed method: {0}.{1}", method.DeclaringType.FullName, method.Name);
				sb.WriteLine ("// Signature: {0}", signature.ToString ());

				sb.Write ("typedef ");
				sb.Write (native_return_type);
				sb.Write ("(*func_");
				sb.Write (name);
				sb.Write (") (");
				for (int i = first_parameter; i < method.Parameters.Count; i++) {
					if (i > first_parameter)
						sb.Write (", ");
					sb.Write (ToObjCParameterType (method.Parameters [i].ParameterType, descriptiveMethodName, exceptions, method));
					sb.Write (" ");
					sb.Write (GetParamName (method, i));
				}
				sb.WriteLine (");");

				sb.WriteLine (native_return_type);
				sb.Write (name);
				sb.Write (" (");
				for (int i = first_parameter; i < method.Parameters.Count; i++) {
					if (i > first_parameter)
						sb.Write (", ");
					sb.Write (ToObjCParameterType (method.Parameters [i].ParameterType, descriptiveMethodName, exceptions, method));
					sb.Write (" ");
					sb.Write (GetParamName (method, i));
				}
				sb.WriteLine (")");
				sb.WriteLine ("{");
				if (is_stret) {
					sb.StringBuilder.AppendLine ("#if defined (__arm64__)");
					sb.WriteLine ("xamarin_process_managed_exception ((MonoObject *) xamarin_create_system_entry_point_not_found_exception (\"{0}\"));", pinfo.EntryPoint);
					sb.StringBuilder.AppendLine ("#else");
				}
				sb.WriteLine ("@try {");
				if (!isVoid || is_stret)
					sb.Write ("return ");
				sb.Write ("((func_{0}) {1}) (", name, pinfo.EntryPoint);
				for (int i = first_parameter; i < method.Parameters.Count; i++) {
					if (i > first_parameter)
						sb.Write (", ");
					sb.Write (GetParamName (method, i));
				}
				sb.WriteLine (");");
				sb.WriteLine ("} @catch (NSException *exc) {");
				sb.WriteLine ("xamarin_process_nsexception (exc);");
				sb.WriteLine ("}");
				if (is_stret)
					sb.StringBuilder.AppendLine ("#endif /* defined (__arm64__) */");
				sb.WriteLine ("}");
				sb.WriteLine ();
			} else {
				// Console.WriteLine ("Signature already processed: {0} for {1}.{2}", signature.ToString (), method.DeclaringType.FullName, method.Name);
			}

			return wrapperName;
		}

		public void GeneratePInvokeWrapper (PInvokeWrapperGenerator state, MethodDefinition method)
		{
			string wrapperName;
			try {
				wrapperName = TryGeneratePInvokeWrapper (state, method);
			} catch (Exception e) {
				throw ErrorHelper.CreateError (App, 4169, e, method, Errors.MT4169, GetDescriptiveMethodName (method), e.Message);
			}

			// find the module reference to __Internal
			ModuleReference mr = null;
			foreach (var mref in method.Module.ModuleReferences) {
				if (mref.Name == "__Internal") {
					mr = mref;
					break;
				}
			}
			if (mr is null)
				method.Module.ModuleReferences.Add (mr = new ModuleReference ("__Internal"));

			var pinfo = method.PInvokeInfo;
			pinfo.Module = mr;
			pinfo.EntryPoint = wrapperName;
		}

		public void Register (IEnumerable<AssemblyDefinition> assemblies)
		{
			Register (null, assemblies);
		}

		public void Register (PlatformResolver resolver, IEnumerable<AssemblyDefinition> assemblies)
		{
			this.resolver = resolver;

			if (Target?.CachedLink == true)
				throw ErrorHelper.CreateError (99, Errors.MX0099, "the static registrar should not execute unless the linker also executed (or was disabled). A potential workaround is to pass '-f' as an additional " + Driver.NAME + " argument to force a full build");

			this.input_assemblies = assemblies;

			foreach (var assembly in assemblies) {
				Driver.Log (3, "Generating static registrar for {0}", assembly.Name);
				RegisterAssembly (assembly);
			}
		}

		static bool IsPropertyTrimmed (PropertyDefinition pd, AnnotationStore annotations)
		{
			if (pd is null)
				return false;

			if (!IsTrimmed (pd, annotations))
				return false;
			if (pd.GetMethod is not null && !IsTrimmed (pd.GetMethod, annotations))
				return false;
			if (pd.SetMethod is not null && !IsTrimmed (pd.SetMethod, annotations))
				return false;
			return true;
		}

		public static bool IsTrimmed (MemberReference tr, AnnotationStore annotations)
		{
			if (tr is null)
				return false;

			var assembly = tr.Module?.Assembly;
			if (assembly is null) {
				// Trimmed away
				return true;
			}

			var action = annotations.GetAction (assembly);
			switch (action) {
			case AssemblyAction.Skip:
			case AssemblyAction.Copy:
			case AssemblyAction.CopyUsed:
			case AssemblyAction.Save:
				return false;
			case AssemblyAction.Link:
				break;
			case AssemblyAction.Delete:
				return true;
			case AssemblyAction.AddBypassNGen:
			case AssemblyAction.AddBypassNGenUsed:
			default:
				throw ErrorHelper.CreateError (99, $"Unknown linker action: {action}");
			}

			if (annotations.IsMarked (tr))
				return false;

			if (annotations.IsMarked (tr.Resolve ()))
				return false;

			return true;
		}

		public void FilterTrimmedApi (AnnotationStore annotations)
		{
			var trimmedAway = Types.Where (kvp => IsTrimmed (kvp.Value.Type, annotations)).ToArray ();
			foreach (var trimmed in trimmedAway)
				Types.Remove (trimmed.Key);

			var skippedTrimmedAway = skipped_types.Where (v => IsTrimmed (v.Skipped, annotations)).ToArray ();
			foreach (var trimmed in skippedTrimmedAway)
				skipped_types.Remove (trimmed);

			foreach (var kvp in Types) {
				var methods = kvp.Value.Methods;
				if (methods is not null) {
					for (var i = methods.Count - 1; i >= 0; i--) {
						var method = methods [i].Method;
						if (IsTrimmed (method, annotations))
							methods.RemoveAt (i);
					}
				}
				var properties = kvp.Value.Properties;
				if (properties is not null) {
					for (var i = properties.Count - 1; i >= 0; i--) {
						var property = properties [i].Property;
						if (IsPropertyTrimmed (property, annotations))
							properties.RemoveAt (i);
					}
				}
				var fields = kvp.Value.Fields;
				if (fields is not null) {
					foreach (var fieldName in fields.Keys.ToArray ()) {
						var property = fields [fieldName].Property;
						if (IsPropertyTrimmed (property, annotations))
							fields.Remove (fieldName);
					}
				}
			}
		}

		public void GenerateSingleAssembly (PlatformResolver resolver, IEnumerable<AssemblyDefinition> assemblies, string header_path, string source_path, string assembly, out string initialization_method)
		{
			single_assembly = assembly;
			Generate (resolver, assemblies, header_path, source_path, out initialization_method);
		}

		public void Generate (IEnumerable<AssemblyDefinition> assemblies, string header_path, string source_path, out string initialization_method)
		{
			Generate (null, assemblies, header_path, source_path, out initialization_method);
		}

		public void Generate (PlatformResolver resolver, IEnumerable<AssemblyDefinition> assemblies, string header_path, string source_path, out string initialization_method)
		{
			Register (resolver, assemblies);
			Generate (header_path, source_path, out initialization_method);
		}

		public void Generate (string header_path, string source_path, out string initialization_method)
		{
			var sb = new AutoIndentStringBuilder ();
			header = new AutoIndentStringBuilder ();
			declarations = new AutoIndentStringBuilder ();
			methods = new AutoIndentStringBuilder ();
			interfaces = new AutoIndentStringBuilder ();

			header.WriteLine ("#pragma clang diagnostic ignored \"-Wdeprecated-declarations\"");
			header.WriteLine ("#pragma clang diagnostic ignored \"-Wtypedef-redefinition\""); // temporary hack until we can stop including glib.h
			header.WriteLine ("#pragma clang diagnostic ignored \"-Wobjc-designated-initializers\"");
			header.WriteLine ("#pragma clang diagnostic ignored \"-Wunguarded-availability-new\"");

			if (App.EnableDebug) {
				header.WriteLine ("#define DEBUG 1");
				methods.WriteLine ("#define DEBUG 1");
			}

			if (App.XamarinRuntime == XamarinRuntime.CoreCLR) {
				header.WriteLine ("#define CORECLR_RUNTIME");
				methods.WriteLine ("#define CORECLR_RUNTIME");
			}

			header.WriteLine ("#include <stdarg.h>");
			methods.WriteLine ("#include <xamarin/xamarin.h>");
			header.WriteLine ("#include <objc/objc.h>");
			header.WriteLine ("#include <objc/runtime.h>");
			header.WriteLine ("#include <objc/message.h>");

			methods.WriteLine ($"#include \"{Path.GetFileName (header_path)}\"");
			methods.StringBuilder.AppendLine ("extern \"C\" {");

			if (App.Embeddinator)
				methods.WriteLine ("void xamarin_embeddinator_initialize ();");

			Specialize (sb, out initialization_method);

			methods.WriteLine ();
			methods.AppendLine ();
			methods.AppendLine (sb);

			methods.StringBuilder.AppendLine ("} /* extern \"C\" */");

			FlushTrace ();

			Driver.WriteIfDifferent (source_path, methods.ToString (), true);

			header.AppendLine ();
			header.AppendLine (declarations);
			header.AppendLine (interfaces);
			Driver.WriteIfDifferent (header_path, header.ToString (), true);

			header.Dispose ();
			header = null;
			declarations.Dispose ();
			declarations = null;
			methods.Dispose ();
			methods = null;
			interfaces.Dispose ();
			interfaces = null;
			sb.Dispose ();
		}

		protected override bool SkipRegisterAssembly (AssemblyDefinition assembly)
		{
			if (assembly.HasCustomAttributes) {
				foreach (var ca in assembly.CustomAttributes) {
					var t = ca.AttributeType.Resolve ();
					while (t is not null) {
						if (t.Is ("ObjCRuntime", "DelayedRegistrationAttribute"))
							return true;
						t = t.BaseType?.Resolve ();
					}
				}
			}

			return base.SkipRegisterAssembly (assembly);
		}

		// Find the value of the [UserDelegateType] attribute on the specified delegate
		TypeReference GetUserDelegateType (TypeReference delegateType)
		{
			var delegateTypeDefinition = delegateType.Resolve ();
			foreach (var attrib in delegateTypeDefinition.CustomAttributes) {
				var attribType = attrib.AttributeType;
				if (!attribType.Is (Namespaces.ObjCRuntime, "UserDelegateTypeAttribute"))
					continue;
				return attrib.ConstructorArguments [0].Value as TypeReference;
			}
			return null;
		}

		MethodDefinition GetDelegateInvoke (TypeReference delegateType)
		{
			var td = delegateType.Resolve ();
			foreach (var method in td.Methods) {
				if (method.Name == "Invoke")
					return method;
			}
			return null;
		}

		MethodReference InflateMethod (TypeReference inflatedDeclaringType, MethodDefinition openMethod)
		{
			if (inflatedDeclaringType is not GenericInstanceType git)
				return openMethod;

			var inflatedReturnType = TypeReferenceExtensions.InflateGenericType (git, openMethod.ReturnType);
			var mr = new MethodReference (openMethod.Name, inflatedReturnType, git);
			if (openMethod.HasParameters) {
				for (int i = 0; i < openMethod.Parameters.Count; i++) {
					var inflatedParameterType = TypeReferenceExtensions.InflateGenericType (git, openMethod.Parameters [i].ParameterType);
					var p = new ParameterDefinition (openMethod.Parameters [i].Name, openMethod.Parameters [i].Attributes, inflatedParameterType);
					mr.Parameters.Add (p);
				}
			}
			return mr;
		}

		public bool TryComputeBlockSignature (ICustomAttributeProvider codeLocation, TypeReference trampolineDelegateType, out Exception exception, out string signature)
		{
			signature = null;
			exception = null;
			try {
				// Calculate the block signature.
				var blockSignature = false;
				MethodReference userMethod = null;

				// First look for any [UserDelegateType] attributes on the trampoline delegate type.
				var userDelegateType = GetUserDelegateType (trampolineDelegateType);
				if (userDelegateType is not null) {
					var userMethodDefinition = GetDelegateInvoke (userDelegateType);
					userMethod = InflateMethod (userDelegateType, userMethodDefinition);
					blockSignature = true;
				} else {
					// Couldn't find a [UserDelegateType] attribute, use the type of the actual trampoline instead.
					var userMethodDefinition = GetDelegateInvoke (trampolineDelegateType);
					userMethod = InflateMethod (trampolineDelegateType, userMethodDefinition);
					blockSignature = false;
				}

				// No luck finding the signature, so give up.
				if (userMethod is null) {
					exception = ErrorHelper.CreateError (App, 4187 /* Could not find a [UserDelegateType] attribute on the type '{0}'. */, codeLocation, Errors.MX4187, trampolineDelegateType.FullName);
					return false;
				}

				var parameters = new TypeReference [userMethod.Parameters.Count];
				for (int p = 0; p < parameters.Length; p++)
					parameters [p] = userMethod.Parameters [p].ParameterType;
				signature = LinkContext.Target.StaticRegistrar.ComputeSignature (userMethod.DeclaringType, false, userMethod.ReturnType, parameters, userMethod.Resolve (), isBlockSignature: blockSignature);
				return true;
			} catch (Exception e) {
				exception = ErrorHelper.CreateError (App, 4188 /* Unable to compute the block signature for the type '{0}': {1} */, e, codeLocation, Errors.MX4188, trampolineDelegateType.FullName, e.Message);
				return false;
			}
		}

	}

	// Replicate a few attribute types here, with TypeDefinition instead of Type

	class ProtocolAttribute : Attribute {
		public TypeDefinition WrapperType { get; set; }
		public string Name { get; set; }
		public bool IsInformal { get; set; }
		public Version FormalSinceVersion { get; set; }
	}

	class BlockProxyAttribute : Attribute {
		public TypeDefinition Type { get; set; }
	}

	class DelegateProxyAttribute : Attribute {
		public TypeDefinition DelegateType { get; set; }
	}

	class BindAsAttribute : Attribute {
		public BindAsAttribute (TypeReference type)
		{
			this.Type = type;
		}

		public TypeReference Type { get; set; }
		public TypeReference OriginalType { get; set; }
	}

	public sealed class ProtocolMemberAttribute : Attribute {
		public ProtocolMemberAttribute () { }

		public bool IsRequired { get; set; }
		public bool IsProperty { get; set; }
		public bool IsStatic { get; set; }
		public string Name { get; set; }
		public string Selector { get; set; }
		public TypeReference ReturnType { get; set; }
		public TypeReference ReturnTypeDelegateProxy { get; set; }
		public TypeReference [] ParameterType { get; set; }
		public bool [] ParameterByRef { get; set; }
		public TypeReference [] ParameterBlockProxy { get; set; }
		public bool IsVariadic { get; set; }

		public TypeReference PropertyType { get; set; }
		public string GetterSelector { get; set; }
		public string SetterSelector { get; set; }
		public ArgumentSemantic ArgumentSemantic { get; set; }

		public MethodDefinition Method { get; set; } // not in the API, used to find the original method in the static registrar
	}

	class CategoryAttribute : Attribute {
		public CategoryAttribute (TypeDefinition type)
		{
			Type = type;
		}

		public TypeDefinition Type { get; set; }
		public string Name { get; set; }
	}

	class RegisterAttribute : Attribute {
		public RegisterAttribute () { }
		public RegisterAttribute (string name)
		{
			this.Name = name;
		}

		public RegisterAttribute (string name, bool isWrapper)
		{
			this.Name = name;
			this.IsWrapper = isWrapper;
		}

		public string Name { get; set; }
		public bool IsWrapper { get; set; }
		public bool SkipRegistration { get; set; }
	}

	class AdoptsAttribute : Attribute {
		public string ProtocolType { get; set; }
	}

	[Flags]
	internal enum MTTypeFlags : uint {
		None = 0,
		CustomType = 1,
		UserType = 2,
	}
}
