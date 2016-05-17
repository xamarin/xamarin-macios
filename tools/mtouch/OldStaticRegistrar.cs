// OldStaticRegistrar.cs: The old static registrar. This is obsolete code, and will be removed once the bugs have been ironed out of the new static registrar.
// #define VERBOSE_REGISTRAR

using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;

using Mono.Cecil;
using Mono.Tuner;

using MonoTouch;
using Xamarin.Bundler;
using XamCore.Registrar;
using XamCore.ObjCRuntime;
using Xamarin.Linker;
using Mono.Linker;

namespace XamCore.Registrar {

	class InstanceVariable {
		public string Name { get; set; }
		public string Type { get; set; }
		public int Size { get; set; }
		public int Align { get; set; }
	}

	class Method {
		public string Selector { get; set; }
		public string Signature { get; set; }
		public bool IsStatic { get; set; }
		public Trampoline Trampoline { get; set; }
		public MethodDefinition MethodDefinition { get; set; }
		public ArgumentSemantic ArgumentSemantic { get; set; }
		public string SpecializedTrampoline { get; set; }
		public MethodDefinition Implementation { get; set; }
		
		public string GetTrampoline ()
		{
			string str = "&";
			
			if (!string.IsNullOrEmpty (SpecializedTrampoline))
				return str + SpecializedTrampoline;
			
			switch (Trampoline) {
				case Trampoline.Normal:
					str += "xamarin_trampoline";
					break;
				case Trampoline.Stret:
					str += "xamarin_stret_trampoline";
					break;
				case Trampoline.Single:
					str += "xamarin_fpret_single_trampoline";
					break;
				case Trampoline.Double:
					str += "xamarin_fpret_double_trampoline";
					break;
				case Trampoline.Release:
					str += "xamarin_release_trampoline";
					break;
				case Trampoline.Retain:
					str += "xamarin_retain_trampoline";
					break;
				case Trampoline.Static:
					str += "xamarin_static_trampoline";
					break;
				case Trampoline.StaticStret:
					str += "xamarin_static_stret_trampoline";
					break;
				case Trampoline.StaticSingle:
					str += "xamarin_static_fpret_single_trampoline";
					break;
				case Trampoline.StaticDouble:
					str += "xamarin_static_fpret_double_trampoline";
					break;
				case Trampoline.Constructor:
					str += "xamarin_ctor_trampoline";
					break;
				case Trampoline.Long:
					str += "xamarin_longret_trampoline";
					break;
				case Trampoline.StaticLong:
					str += "xamarin_static_longret_trampoline";
					break;
			}
	
			return str;
		}
	}

	class Property {
		public string Name { get; set; }
		public string Type { get; set; }
		public string ArgumentSemantic { get; set; }
	}

	class Class {
		public string Name { get; set; }
		public string SuperName { get; set; }
		public string TypeName { get; set; }
		public bool IsInternal { get; set; }
		public bool IsModel { get; set; }
		public List<InstanceVariable> InstanceVariables { get; private set; }
		public List<Method> Methods { get; private set; }
		public List<Property> Properties { get; private set; }

		public Class ()
		{
			InstanceVariables = new List<InstanceVariable> ();
			Methods = new List<Method> ();
			Properties = new List<Property> ();
		}

		public void AddInstanceVariable (string name, string type)
		{
			var size_of_intptr = Marshal.SizeOf (typeof (IntPtr));

			AddInstanceVariable (name, type, size_of_intptr, (int) Math.Log (size_of_intptr, 2));
		}

		public void AddInstanceVariable (string name, string type, int size, int align)
		{
			var ivar = new InstanceVariable {
				Name = name,
				Type = type,
				Size = size,
				Align = align,
			};

			InstanceVariables.Add (ivar);
		}

		public void AddMethod (string selector, string signature, bool isstatic, Trampoline trampoline, MethodDefinition methoddef, ArgumentSemantic semantic, MethodDefinition implementation)
		{
			var method = new Method {
				Selector = selector,
				Signature = signature,
				IsStatic = isstatic,
				Trampoline = trampoline,
				MethodDefinition = methoddef,
				ArgumentSemantic = semantic,
				Implementation = implementation,
			};
			Methods.Add (method);
		}

		public void AddProperty (string name, string type, string semantic)
		{
			Properties.Add (new Property {
				Name = name,
				Type = type,
				ArgumentSemantic = semantic,
			});
		}
	}

	struct Pair<T1, T2> {
		public T1 Left;
		public T2 Right;

		public Pair (T1 left, T2 right)
		{
			Left = left;
			Right = right;
		}
	}

	class TypeSystemDescriptor {
		const string ExportAttribute =		"ExportAttribute";
		const string ModelAttribute =		"ModelAttribute";
		const string RegisterAttribute =	"RegisterAttribute";
		const string ConnectAttribute =		"ConnectAttribute";
		const string ProtocolAttribute =    "ProtocolAttribute";
		public const string TransientAttribute ="TransientAttribute";
		const string INativeObject =		"INativeObject";

		List<Pair<Class, TypeDefinition>> classes;

		[System.Diagnostics.Conditional ("VERBOSE_REGISTRAR")]
		static void Trace (string format, params object [] args )
		{
			Console.WriteLine (format, args);
		}

		TypeSystemDescriptor ()
		{
			this.classes = new List<Pair<Class, TypeDefinition>> ();
		}

		void Process (IEnumerable <AssemblyDefinition> assemblies)
		{
			foreach (AssemblyDefinition asm in assemblies)
				ProcessAssembly (asm);
		}

		void ProcessAssembly (AssemblyDefinition assembly)
		{
			List<Exception> exceptions = null;

			Trace ("Processing {0}", assembly.Name.FullName);
			foreach (TypeDefinition type in assembly.MainModule.Types) {
				try {
					ProcessType (type);
				} catch (MonoTouchException mex) {
					if (exceptions == null)
						exceptions = new List<Exception> ();
					exceptions.Add (mex);
				}
			}

			if (exceptions != null)
				throw new AggregateException (exceptions);
		}
		
		public static string GetAssemblyQualifiedName (TypeReference type)
		{
			return string.Format ("{0}, {1}", type.FullName.Replace ('/', '+'), type.Resolve ().Module.Assembly.Name.Name);
		}

		void ProcessType (TypeDefinition type)
		{
			// note: new cecil does not include nested type in the module's type collection!
			if (type.HasNestedTypes) {
				foreach (var nested in type.NestedTypes)
					ProcessType (nested);
			}

			if (!type.IsNSObject ())
				return;

			if (type.HasGenericParameters)
				ErrorHelper.Show (new MonoTouchException (4112, false, "The registrar found a generic type: {0}. Registering generic types with ObjectiveC is not supported with the legacy registrar, and will lead to random behavior and/or crashes. " +
					"Please remove any --registrar arguments passed as additional mtouch arguments in your project's iOS Build options. " +
					"See http://docs.xamarin.com/guides/ios/advanced_topics/registrar for more information.", type.FullName));

			Trace ("    {0}", type.FullName);

			var @class = new Class {
				Name = GetClassName (type),
				SuperName = GetSuperClassName (type),
				TypeName = GetAssemblyQualifiedName (type),
				IsInternal = type.Module.Assembly.Name.Name == Registrar.PlatformAssembly,
				IsModel = HasModelAttribute (type.Module.Assembly.Name.Name == Registrar.PlatformAssembly ? type : GetSuperClass (type)),
			};

			if (!IsWrapperType (type)) {
				ProcessInstanceVariables (type, @class);
				ProcessMethods (type, @class);
			}

			classes.Add (new Pair<Class, TypeDefinition> (@class, type));
		}

		void ProcessInstanceVariables (TypeDefinition type, Class @class)
		{
			@class.AddInstanceVariable ("__monoObjectGCHandle", "i", Marshal.SizeOf (typeof (Int32)), 4);

			if (!type.HasProperties)
				return;

			foreach (PropertyDefinition property in type.Properties) {
				CustomAttribute connect_attribute;
				if (!TryGetAttribute (property, Registrar.Foundation, ConnectAttribute, out connect_attribute))
					continue;

				@class.AddInstanceVariable (GetInstanceVariableName (property, connect_attribute), "@");
			}
		}

		static string GetInstanceVariableName (PropertyDefinition property, CustomAttribute connect_attribute)
		{
			if (connect_attribute.HasConstructorArguments)
				return (string) connect_attribute.ConstructorArguments [0].Value;

			return property.Name;
		}

		void ProcessMethods (TypeDefinition type, Class @class)
		{
			if (type.HasProperties) {
				foreach (var pair in CollectCandidateProperties (type))
					ProcessProperty (pair.Left, pair.Right, @class);
			}

			if (!type.HasMethods)
				return;

			ProcessConstructors (type,  @class);

			foreach (var tuple in CollectCandidateMethods (type, @class))
				ProcessMethod (tuple.Item1, tuple.Item2, tuple.Item3, @class);
		}
		
		void ProcessProperty (PropertyDefinition property, CustomAttribute export_attribute, Class @class)
		{
			string get_name = export_attribute.HasConstructorArguments
				? (string) export_attribute.ConstructorArguments [0].Value
				: property.Name;
			ArgumentSemantic argument_semantic = GetArgumentSemantic (export_attribute);

			if (property.GetMethod != null)
				ProcessMethod (property.GetMethod, get_name, property.GetMethod, @class, argument_semantic);

			if (property.SetMethod != null) {
				string set_name = String.Format ("set{0}{1}:", char.ToUpper (get_name [0]), get_name.Substring (1));
				ProcessMethod (property.SetMethod, set_name, property.SetMethod, @class, argument_semantic);
			}

			if (export_attribute.HasConstructorArguments) {
				string name = (string) export_attribute.ConstructorArguments [0].Value;
				string type = ToSignature (null, property.PropertyType);
				string semantic = string.Empty;
				switch (argument_semantic) {
				case ArgumentSemantic.Copy:
					semantic = "C";
					break;
				case ArgumentSemantic.Retain:
					semantic = "&";
					break;
				}
				@class.AddProperty (name, type, semantic);
			}
		}

		void ProcessMethod (MethodDefinition method, CustomAttribute export_attribute, MethodDefinition implementation, Class @class)
		{
			var name = export_attribute.HasConstructorArguments
				? (string) export_attribute.ConstructorArguments [0].Value
				: method.Name;

			ProcessMethod (method, name, implementation, @class, GetArgumentSemantic (export_attribute));
		}
		
		void ProcessMethod (MethodDefinition method, string name, MethodDefinition implementation, Class @class, ArgumentSemantic semantic)
		{
			var signature = ToSignature (method);
			var trampoline = method.IsStatic ? Trampoline.Static : Trampoline.Normal;

			// TODO: needs fixup?
			switch (signature [0]) {
			case 'Q':
			case 'q':
				trampoline = method.IsStatic ? Trampoline.StaticLong : Trampoline.Long;
				break;
			case 'f':
				trampoline = method.IsStatic ? Trampoline.StaticSingle : Trampoline.Single;
				break;
			case 'd':
				trampoline = method.IsStatic ? Trampoline.StaticDouble : Trampoline.Double;
				break;
			default:
				if (method.ReturnType.IsValueType && ToSignature (method, method.ReturnType).StartsWith ("{"))
					trampoline = method.IsStatic ? Trampoline.StaticStret : Trampoline.Stret;
				break;
			}

			Trace ("        Method: {0}::{1} Signature: {2} Trampoline: {3}", method.DeclaringType, method.Name, signature, trampoline);
			@class.AddMethod (name, signature, method.IsStatic, trampoline, method, semantic, implementation);
		}
		
		static ArgumentSemantic GetArgumentSemantic (CustomAttribute export_attribute)
		{
			if (!export_attribute.HasConstructorArguments || export_attribute.ConstructorArguments.Count <= 1)
				return ArgumentSemantic.None;
			
			return (ArgumentSemantic) export_attribute.ConstructorArguments [1].Value;
		}
		
		// [Export] is not sealed anymore - so we cannot simply compare strings
		static CustomAttribute GetExportAttribute (ICustomAttributeProvider candidate)
		{
			if (!candidate.HasCustomAttributes)
				return null;
			
			foreach (CustomAttribute ca in candidate.CustomAttributes) {
				if (ca.Constructor.DeclaringType.Inherits (Registrar.Foundation, ExportAttribute))
					return ca;
			}
			return null;
		}

		static IEnumerable<Pair<PropertyDefinition, CustomAttribute>> CollectCandidateProperties (TypeDefinition type)
		{
			foreach (PropertyDefinition property in type.Properties) {
				var candidate = GetBasePropertyInTypeHierarchy (property) ?? property;

				CustomAttribute export_attribute = GetExportAttribute (candidate);
				if (export_attribute == null)
					continue;

				// TODO: Check that accessors have no Export attribute and ignore the property
				// in such case
				
				yield return new Pair<PropertyDefinition, CustomAttribute> {
					Left = candidate,
					Right = export_attribute,
				};
			}
		}
		
		static IEnumerable<Tuple<MethodDefinition, CustomAttribute, MethodDefinition>> CollectCandidateMethods (TypeDefinition type, Class @class)
		{
			var method_map = SharedStatic.PrepareInterfaceMethodMapping (type);

			foreach (MethodDefinition method in type.Methods) {
				if (method.IsConstructor)
					continue;

				var candidate = GetBaseMethodInTypeHierarchy (method) ?? method;

				CustomAttribute export_attribute = GetExportAttribute (candidate);

				if (export_attribute == null) {
					List<MethodDefinition> impls;
					if (method_map != null && method_map.TryGetValue (method, out impls)) {
						if (impls.Count != 1)
							Shared.GetMT4127 (method, impls);

						export_attribute = GetExportAttribute (impls [0]);
					}
				}

				if (export_attribute == null)
					continue;

				if (method.IsVirtual && @class.IsModel) {
					Trace ("        Method: Discarded. IsVirtual: {0} IsModel: {1}", method.IsVirtual, @class.IsModel);
					continue;
				}

				yield return new Tuple<MethodDefinition, CustomAttribute, MethodDefinition> (candidate, export_attribute, method);
			}
		}

		void ProcessConstructors (TypeDefinition type, Class @class)
		{
			foreach (MethodDefinition constructor in type.Methods) {
				if (!constructor.IsConstructor || constructor.IsStatic)
					continue;

				if (!constructor.HasParameters) {
					ProcessDefaultConstructor (constructor, @class);
					continue;
				}

				ProcessConstructor (constructor, @class);
			}
		}

		void ProcessDefaultConstructor (MethodDefinition constructor, Class @class)
		{
			Trace ("        Method: .ctor() Signature: {0} Trampoline: {1}", ToSignature (constructor), Trampoline.Constructor);
			@class.AddMethod ("init", ToSignature (constructor), false, Trampoline.Constructor, constructor, ArgumentSemantic.None, constructor);
		}

		void ProcessConstructor (MethodDefinition constructor, Class @class)
		{
			CustomAttribute export_attribute = GetExportAttribute (constructor);
			if (export_attribute == null)
				return;

			if (!export_attribute.HasConstructorArguments || export_attribute.ConstructorArguments.Count != 1)
				return;

			string selector = (string) export_attribute.ConstructorArguments [0].Value;

			Trace ("        Method: .ctor Signature: {0} Trampoline: {1}", ToSignature (constructor), Trampoline.Constructor);
			@class.AddMethod (selector, ToSignature (constructor), false, Trampoline.Constructor, constructor, ArgumentSemantic.None, constructor);
		}
		
		string GetClassName (TypeDefinition type)
		{
			CustomAttribute register_attribute;

			if (!TryGetAttribute (type, Registrar.Foundation, RegisterAttribute, out register_attribute))
				return NormalizeFullName (type);

			if (!register_attribute.HasConstructorArguments)
				return NormalizeFullName (type);

			return (string) register_attribute.ConstructorArguments [0].Value;
		}

		string GetSuperClassName (TypeDefinition type)
		{
			var base_type = GetSuperClass (type);

			CustomAttribute register_attribute;

			if (!TryGetAttribute (base_type, Registrar.Foundation, RegisterAttribute, out register_attribute))
				return NormalizeFullName (base_type);

			if (!register_attribute.HasConstructorArguments)
				return NormalizeFullName (base_type);

			return (string) register_attribute.ConstructorArguments [0].Value;
		}
		
		public static TypeReference GetProtocolAttributeWrapperType (TypeReference type)
		{
			CustomAttribute attrib;

			if (!TryGetAttribute (type.Resolve (), Registrar.Foundation, ProtocolAttribute, out attrib))
				return null;

			if (attrib.HasProperties) {
				foreach (var prop in attrib.Properties) {
					if (prop.Name == "WrapperType")
						return (TypeReference) prop.Argument.Value;
				}
			}

			return null;
		}

		static bool IsWrapperType (TypeDefinition type)
		{
			CustomAttribute register_attribute;
			
			if (!TryGetAttribute (type, Registrar.Foundation, RegisterAttribute, out register_attribute))
				return false;
			
			if (!register_attribute.HasConstructorArguments || register_attribute.ConstructorArguments.Count < 2)
				return false;
			
			return (bool) register_attribute.ConstructorArguments [1].Value;
		}

		static TypeDefinition GetSuperClass (TypeDefinition type)
		{
			if (type.BaseType == null)
				return null;

			var base_type = type.BaseType.Resolve ();
			if (base_type == null)
				return null;

			if (HasModelAttribute (base_type))
				return GetSuperClass (base_type);

			return base_type;
		}

		static bool HasModelAttribute (ICustomAttributeProvider provider)
		{
			CustomAttribute model_attribute;
			return TryGetAttribute (provider, Registrar.Foundation, ModelAttribute, out model_attribute);
		}

		static bool TryGetAttribute (ICustomAttributeProvider provider, string @namespace, string attributeName, out CustomAttribute attribute)
		{
			attribute = null;
			if (!provider.HasCustomAttributes)
				return false;

			foreach (CustomAttribute custom_attribute in provider.CustomAttributes) {
				if (!custom_attribute.Constructor.DeclaringType.Is (@namespace, attributeName))
					continue;

				attribute = custom_attribute;
				return true;
			}

			return false;
		}

		static string NormalizeFullName (TypeDefinition type)
		{
			return type.FullName.Replace ('/', '+');
		}

		public static string ToSignature (MethodDefinition method, TypeReference type)
		{
			var definition = type as TypeDefinition;
			if (definition != null)
				return ToSignature (method, definition);

			if (type is TypeSpecification)
				return "@";

			definition = type.Resolve ();
			if (definition != null)
				return ToSignature (method, definition);

			return "@";
		}

		static string ValueTypeSignature (TypeDefinition type)
		{
			var signature = new StringBuilder ();
			signature.Append ("{");
			signature.AppendFormat ("{0}=", type.Name);
			foreach (FieldDefinition field in type.Fields) {
				if (field.IsStatic)
					continue;

				signature.Append (ToSignature (null, field.FieldType));
			}

			signature.Append ("}");
			return signature.ToString ();
		}

		public static string ToSignature (MethodDefinition method)
		{
			var signature = new StringBuilder ();
			signature.Append (method.IsConstructor ? "@" : ToSignature (method, method.ReturnType));
			signature.Append ("@:");
			foreach (ParameterDefinition parameter in method.Parameters){
				if (parameter.ParameterType.IsByReference) {
					signature.Append ("^");
					signature.Append (ToSignature (method, parameter.ParameterType.GetElementType ()));
				} else {
					signature.Append (ToSignature (method, parameter.ParameterType));
				}
			}
			return signature.ToString ();
		}

		public static string ToSignature (MethodDefinition method, TypeDefinition type)
		{
			switch (type.FullName) {
			case "System.IntPtr": return "^v";
			case "System.SByte": return "c";
			case "System.Byte": return "c";
			case "System.Char": return "c";
			case "System.Int16": return "s";
			case "System.UInt16": return "S";
			case "System.Int32": return "i";
			case "System.UInt32": return "I";
			case "System.Int64": return "q";
			case "System.UInt64": return "Q";
			case "System.Single": return "f";
			case "System.Double": return "d";
			case "System.Boolean": return "B";
			case "System.Void": return "v";
			case "System.String": return "@";
			case "MonoTouch.ObjCRuntime.Selector": return ":";
			case "MonoTouch.ObjCRuntime.Class": return "#";
			}

			if (SharedStatic.IsNativeObject (type))
				return "@";

			if (SharedStatic.IsDelegate (type))
				return "^v";

			if (type.IsEnum)
				return ToSignature (method, SharedStatic.GetEnumUnderlyingType (type));

			if (type.IsValueType) {
				return ValueTypeSignature (type);
			}

			if (method != null)
				throw new MonoTouchException (4111, true, "The registrar cannot build a signature for type `{0}' in method `{1}`.", type.FullName, method.DeclaringType.FullName + "." + method.Name);

			throw new MonoTouchException (4101, true, "The registrar cannot build a signature for type `{0}`.", type.FullName);
		}

		static PropertyDefinition GetBasePropertyInTypeHierarchy (PropertyDefinition property)
		{
			TypeDefinition @base = GetBaseType (property.DeclaringType);
			while (@base != null) {
				PropertyDefinition base_property = TryMatchProperty (@base, property);
				if (base_property != null) {
					var accessor = base_property.GetMethod ?? base_property.SetMethod;
					if (!accessor.IsVirtual || accessor.IsNewSlot)
						return base_property;

					return GetBasePropertyInTypeHierarchy (base_property) ?? base_property;
				}

				@base = GetBaseType (@base);
			}

			return null;
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
			
			if (candidate.GetMethod != null) {
				if (property.GetMethod == null)
					return false;
				if (!SharedStatic.MethodMatch (candidate.GetMethod, property.GetMethod))
					return false;
			} else if (property.GetMethod != null) {
					return false;
			}

			if (candidate.SetMethod != null) {
				if (property.SetMethod == null)
					return false;
				if (!SharedStatic.MethodMatch (candidate.SetMethod, property.SetMethod))
					return false;
			} else if (property.SetMethod != null) {
					return false;
			}

			return true;
		}

		public static MethodDefinition GetBaseMethodInTypeHierarchy (MethodDefinition method)
		{
			TypeDefinition @base = GetBaseType (method.DeclaringType);
			while (@base != null) {
				MethodDefinition base_method = TryMatchMethod (@base, method);
				if (base_method != null) {
					if (!base_method.IsVirtual || base_method.IsNewSlot)
						return base_method;

					return GetBaseMethodInTypeHierarchy (base_method) ?? base_method;
				}

				@base = GetBaseType (@base);
			}

			return null;
		}

		static MethodDefinition TryMatchMethod (TypeDefinition type, MethodDefinition method)
		{
			if (!type.HasMethods)
				return null;

			foreach (MethodDefinition candidate in type.Methods)
				if (SharedStatic.MethodMatch (candidate, method))
					return candidate;

			return null;
		}

		static TypeDefinition GetBaseType (TypeDefinition type)
		{
			if (type == null || type.BaseType == null)
				return null;

			return type.BaseType.Resolve ();
		}

		static int Depth (TypeDefinition type)
		{
			int depth = 0;
			while (type != null) {
				depth++;
				type = type.BaseType != null ? type.BaseType.Resolve () : null;
			}

			return depth;
		}

		public static IEnumerable<Class> Describe (IEnumerable <AssemblyDefinition> assemblies)
		{
			var descriptor = new TypeSystemDescriptor ();
			descriptor.Process (assemblies);

			descriptor.classes.Sort ((x, y) => Depth (x.Right) - Depth (y.Right));

			return descriptor.classes.Select (pair => pair.Left);
		}
	}

	class OldStaticRegistrar : IStaticRegistrar {
		static StringBuilder headers = new StringBuilder ();
		static StringBuilder structs = new StringBuilder ();
		static bool verbose = false;
		
		static string ToObjCParameterType (TypeReference type, string descriptiveMethodName)
		{
			TypeDefinition td = type.Resolve ();

			if (td == null)
				throw new MonoTouchException (4105, true, "The registrar cannot marshal the parameter of type `{0}` in signature for method `{1}`.", type.FullName, descriptiveMethodName);

			if (type.IsByReference) {
				string res = ToObjCParameterType (type.GetElementType (), descriptiveMethodName);
				if (res == null)
					return null;
				return res + "*";
			}
			
			switch (td.FullName) {
			case "System.Drawing.RectangleF": return "CGRect";
			case "System.Drawing.PointF": return "CGPoint";
			case "System.Drawing.SizeF": return "CGSize";
			case "System.String": return "NSString *";
			case "System.IntPtr": return "void *";
			case "System.SByte": return "unsigned char";
			case "System.Byte": return "signed char";
			case "System.Char": return "signed char";
			case "System.Int16": return "short";
			case "System.UInt16": return "unsigned short";
			case "System.Int32": return "int";
			case "System.UInt32": return "unsigned int";
			case "System.Int64": return "long long";
			case "System.UInt64": return "unsigned long long";
			case "System.Single": return "float";
			case "System.Double": return "double";
			case "System.Boolean": return "bool";
			case "System.DateTime":
				throw new MonoTouchException (4102, true, "The registrar found an invalid type `{0}` in signature for method `{2}`. Use `{1}` instead.", "System.DateTime", "MonoTouch.Foundation.NSDate", descriptiveMethodName);
			case "MonoTouch.ObjCRuntime.Selector": return "SEL";
			case "MonoTouch.ObjCRuntime.Class": return "Class";
			default:
				if (td.IsNSObject ()) {
					return "id";
				} else if (td.IsEnum) {
					return ToObjCParameterType (SharedStatic.GetEnumUnderlyingType (td), descriptiveMethodName);
				} else if (td.IsValueType) {
					if (td.Namespace.StartsWith ("MonoTouch")) {
						CheckNamespace (td.Namespace);
						return td.Name;
					}
					return CheckStructure (td, descriptiveMethodName);
				} else {
					return SharedStatic.ToObjCType (td);
				}
			}
		}
		
		static string GetPrintfFormatSpecifier (TypeDefinition type, out bool unknown)
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
				case "System.Single":
				case "System.Double": return "f";
				default:
					unknown = true;
					return "p";
				}
			} else if (type.IsNSObject ()) {
				return "@";
			} else {
				unknown = true;
				return "p";
			}
		}
		
		// here we try to create a specialized trampoline for the specified method.
		static int counter = 0;
		static StringBuilder nslog_start = new StringBuilder ();
		static StringBuilder nslog_end = new StringBuilder ();
		
		static StringBuilder comment = new StringBuilder ();
		static StringBuilder copyback = new StringBuilder ();
		static StringBuilder signature = new StringBuilder ();
		static StringBuilder invoke = new StringBuilder ();
		static StringBuilder setup_call_stack = new StringBuilder ();
		static StringBuilder setup_return = new StringBuilder ();
		
		static HashSet<string> trampoline_names = new HashSet<string> ();
		static HashSet<string> namespaces = new HashSet<string> ();
		static HashSet<string> structures = new HashSet<string> ();

		public LinkContext LinkContext {
			get { throw new NotImplementedException (); }
			set { throw new NotImplementedException (); }
		}

		static void CheckNamespace (string ns)
		{
			if (!ns.StartsWith ("MonoTouch."))
				return;
			ns = ns.Substring (10);
			if (namespaces.Contains (ns))
				return;

			namespaces.Add (ns);		

			switch (ns) {
			case "CoreAnimation":
				headers.Append ("#include <QuartzCore/QuartzCore.h>\n");
				break;
			case "CoreMidi":
				headers.Append ("#include <CoreMIDI/CoreMIDI.h>\n");
				break;
			default:
				headers.AppendFormat ("#include <{0}/{0}.h>\n", ns);
				break;
			}
		}
		
		static string CheckStructure (TypeDefinition structure, string descriptiveMethodName)
		{
			string n;
			StringBuilder name = new StringBuilder ();
			StringBuilder body = new StringBuilder ();
			int size = 0;
			
			ProcessStructure (name, body, structure, ref size, descriptiveMethodName);
			
			n = "struct trampoline_struct_" + name.ToString ();
			if (!structures.Contains (n)) {
				structures.Add (n);
				structs.AppendFormat ("{0} {{\n{1}}};\n\n", n, body.ToString ());
			}

			return n;
		}
		
		static void ProcessStructure (StringBuilder name, StringBuilder body, TypeDefinition structure, ref int size, string descriptiveMethodName)
		{
			switch (structure.FullName) {
			case "System.Char":
				name.Append ('c');
				body.AppendFormat ("\tchar v{0};\n", size);
				size += 1;
				break;
			case "System.Boolean": 
				name.Append ('B');
				body.AppendFormat ("\tbool v{0};\n", size);
				size += 1;
				break;
			case "System.Byte":
			case "System.SByte":
				name.Append ('b');
				body.AppendFormat ("\tchar v{0};\n", size);
				size += 1;
				break;
			case "System.UInt16":
			case "System.Int16":
				name.Append ('s');
				body.AppendFormat ("\tshort v{0};\n", size);
				size += 2;
				break;
			case "System.UInt32":
			case "System.Int32":
				name.Append ('i');
				body.AppendFormat ("\tint v{0};\n", size);
				size += 4;
				break;
			case "System.Int64":
			case "System.UInt64":
				name.Append ('l');
				body.AppendFormat ("\tlong long v{0};\n", size);
				size += 8;
				break;
			case "System.Single":
				name.Append ('f');
				body.AppendFormat ("\tfloat v{0};\n", size);
				size += 4;
				break;
			case "System.Double":
				name.Append ('d');
				body.AppendFormat ("\tdouble v{0};\n", size);
				size += 8;
				break;
			case "System.IntPtr":
				name.Append ('p');
				body.AppendFormat ("\tvoid *v{0};\n", size);
				size += 4; // for now at least...
				break;
			default:
				bool found = false;
				foreach (FieldDefinition field in structure.Fields) {
					if (field.IsStatic)
						continue;
					found = true;
					var fd = field.FieldType.Resolve ();
					if (fd == null)
						throw new MonoTouchException (4105, true, "The registrar cannot marshal the parameter of type `{0}` in signature for method `{1}`.", structure.FullName, descriptiveMethodName);
					ProcessStructure (name, body, fd, ref size, descriptiveMethodName);
				}
				if (!found)
					throw new MonoTouchException (4111, true, "The registrar cannot build a signature for type `{0}' in method `{1}`.", structure.FullName, descriptiveMethodName);
				break;
			}
		}
		
		static string GetUniqueTrampolineName (string suggestion)
		{
			char []fixup = suggestion.ToCharArray ();
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

		static bool IsGeneric (TypeReference tr)
		{
			if (tr is GenericParameter || tr is GenericInstanceType)
				return true;

			if (tr.HasGenericParameters)
				return true;

			return false;
		}

		static void WarnIfGeneric (MethodDefinition method)
		{
			bool warn = false;

			if (method.HasGenericParameters)
				warn = true;
			else if (method.HasParameters) {
				foreach (ParameterDefinition p in method.Parameters) {
					if (!IsGeneric (p.ParameterType))
						continue;

					if (SharedStatic.IsDelegate (p.ParameterType.Resolve ()))
						continue;

					warn = true;
					break;
				}
			}

			if (!warn && IsGeneric (method.ReturnType))
				warn = true;

			if (warn)
				ErrorHelper.Show (new MonoTouchException (4113, false, "The registrar found a generic method: '{0}.{1}'. Exporting generic methods is not supported with the legacy registrar, and will lead to random behavior and/or crashes. " +
					"Please remove any --registrar arguments passed as additional mtouch arguments in your project's iOS Build options. " +
					"See http://docs.xamarin.com/guides/ios/advanced_topics/registrar for more information.", method.DeclaringType.FullName, method.Name));
		}

		static bool HasIntPtrBoolCtor (TypeDefinition type)
		{
			if (!type.HasMethods)
				return false;
			foreach (var method in type.Methods) {
				if (!method.IsConstructor || !method.HasParameters)
					continue;
				if (method.Parameters.Count != 2)
					continue;
				if (!method.Parameters [0].ParameterType.Is ("System", "IntPtr"))
					continue;
				if (method.Parameters [1].ParameterType.Is ("System", "Boolean"))
					return true;
			}
			return false;
		}
		
		static void Specialize (StringBuilder sb, Method method)
		{
			if (method == null || method.MethodDefinition == null)
				return;

			WarnIfGeneric (method.MethodDefinition);

			var rettype = string.Empty;
			var returntype = method.MethodDefinition.ReturnType;
			var isStatic = method.IsStatic;
			var isStret = false;
			var isCtor = false;
			var num_arg = method.MethodDefinition.Parameters.Count;
			var descriptiveMethodName = method.Implementation.DeclaringType.FullName + "." + method.Implementation.Name;
			var name = GetUniqueTrampolineName ("native_to_managed_trampoline_" + descriptiveMethodName);
			var isVoid = returntype.Is ("System", "Void");
			
			switch (method.Trampoline) {
			case Trampoline.Normal:
			case Trampoline.Static:
			case Trampoline.Single:
			case Trampoline.Double:
			case Trampoline.StaticDouble:
			case Trampoline.StaticSingle:
				switch (returntype.FullName) {
				case "System.Int64":
					rettype = "long long";
					break;
				case "System.UInt64":
					rettype = "unsigned long long";
					break;
				case "System.Single":
					rettype = "float";
					break;
				case "System.Double":
					rettype = "double";
					break;
				default:
					rettype = "void *";
					break;
				}
				break;
			case Trampoline.StaticStret:
			case Trampoline.Stret:
				rettype = "void";
				isStret = true;
				break;
			case Trampoline.Constructor:
				rettype = "void *";
				isCtor = true;
				break;
			case Trampoline.Release:
			case Trampoline.Retain:
			case Trampoline.GetGCHandle:
			case Trampoline.SetGCHandle:
				// Release & Retain & Dealloc don't contain anything that can be specialized.
				return;
			default:
				return;
			}
			
			comment.Clear ();
			nslog_start.Clear ();
			nslog_end.Clear ();
			copyback.Clear ();
			signature.Clear ();
			invoke.Clear ();
			setup_call_stack.Clear ();
			setup_return.Clear ();
			
			counter++;
			
			// A comment describing the managed signature
			if (verbose) {
				comment.AppendFormat ("\t// {2} {0}.{1} (", method.MethodDefinition.DeclaringType.FullName, method.MethodDefinition.Name, method.MethodDefinition.ReturnType.FullName);
				for (int i = 0; i < num_arg; i++) {
					var param = method.MethodDefinition.Parameters [i];
					if (i > 0)
						comment.Append (", ");
					comment.AppendFormat ("{0} {1}", param.ParameterType.FullName, param.Name);
				}
				comment.AppendFormat (")\n");
				comment.AppendFormat ("\t// ArgumentSemantic: {0} IsStatic: {1} IsStret: {4} Selector: '{2}' Signature: '{3}'\n", method.ArgumentSemantic, method.IsStatic, method.Selector, method.Signature, isStret);
			}
			
			// the native signature
			signature.AppendFormat ("{0}\n", rettype);
			signature.Append (name);
			signature.Append (" (");
			if (isStret)
				signature.Append ("void *buffer, ");
			signature.Append ("id self, SEL sel");
			for (int i = 0; i < num_arg; i++) {
				var param = method.MethodDefinition.Parameters [i];
				var objcparamtype = ToObjCParameterType (param.ParameterType, descriptiveMethodName);
				if (objcparamtype == null)
					throw new MonoTouchException (4107, true, 
					                              "The registrar cannot marshal the parameter of type `{0}` in signature for method `{1}`.",
					                              param.ParameterType.FullName, descriptiveMethodName);
				signature.Append (", ");
				if (verbose)
					signature.AppendFormat ("/*{0} {1}*/ ", param.ParameterType, param.Name);
				signature.Append (objcparamtype);
				signature.AppendFormat (" p{0}", i);
			}
			signature.AppendLine (")");
			
			// a couple of debug printfs
			if (verbose) {
				StringBuilder args = new StringBuilder ();
				nslog_start.AppendFormat ("\tNSLog (@\"{0} (self: %@, sel: %@", name);
				for (int i = 0; i < num_arg; i++) {
					var type = method.MethodDefinition.Parameters [i].ParameterType;
					bool isOut = type.IsByReference;
					if (isOut)
						type = type.GetElementType ();
					var td = type.Resolve ();
					
					nslog_start.AppendFormat (", {0}: ", method.MethodDefinition.Parameters [i].Name);
					args.Append (", ");
					switch (type.FullName) {
					case "System.Drawing.RectangleF":
						if (isOut) {
							nslog_start.Append ("%p : %@");
							args.AppendFormat ("p{0}, p{0} ? NSStringFromCGRect (*p{0}) : @\"NULL\"", i);
						} else {
							nslog_start.Append ("%@"); 
							args.AppendFormat ("NSStringFromCGRect (p{0})", i);
						}
						break;
					case "System.Drawing.PointF":
						if (isOut) {
							nslog_start.Append ("%p: %@"); 
							args.AppendFormat ("p{0}, p{0} ? NSStringFromCGPoint (*p{0}) : @\"NULL\"", i);
						} else {
							nslog_start.Append ("%@"); 
							args.AppendFormat ("NSStringFromCGPoint (p{0})", i);
						}
						break;
					default:
						bool unknown;
						var spec = GetPrintfFormatSpecifier (td, out unknown);
						if (unknown) {
							nslog_start.AppendFormat ("%{0}", spec);
							args.AppendFormat ("&p{0}", i);
						} else if (isOut) {
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
					var spec = GetPrintfFormatSpecifier (method.MethodDefinition.ReturnType.Resolve (), out unknown);
					if (!unknown) {
						nslog_end.Append (", ret: %");
						nslog_end.Append (spec);
						ret_arg = ", res";
					}
				}
				nslog_end.Append (") END\", self, NSStringFromSelector (sel)");
				nslog_end.Append (args.ToString ());
				nslog_end.Append (ret_arg);
				nslog_end.Append (");\n");
				
				nslog_start.Append (") START\", self, NSStringFromSelector (sel)");
				nslog_start.Append (args.ToString ());
				nslog_start.Append (");\n");
			}
			
			// prepare the parameters
			var baseMethod = TypeSystemDescriptor.GetBaseMethodInTypeHierarchy (method.MethodDefinition) ?? method.MethodDefinition;
			for (int i = 0; i < num_arg; i++) {
				var param = method.MethodDefinition.Parameters [i];
				var paramBase = baseMethod.Parameters [i];
				var type = param.ParameterType;
				var objctype = ToObjCParameterType (type, descriptiveMethodName);
				var original_objctype = objctype;
				var isOut = type.IsByReference;
				var isArray = type is ArrayType;
				var td = type.Resolve ();
				if (isOut) {
					type = type.GetElementType ();
					td = type.Resolve ();
					original_objctype = ToObjCParameterType (type, descriptiveMethodName);
					objctype = ToObjCParameterType (type, descriptiveMethodName);
				} else if (td.IsEnum) {
					type = SharedStatic.GetEnumUnderlyingType (td);
					td = type.Resolve ();
				}
				
				switch (type.FullName) {
				case "System.SByte":
				case "System.Byte":
				case "System.Char":
				case "System.Int16":
				case "System.UInt16":
				case "System.Int32":
				case "System.UInt32":
				case "System.Int64":
				case "System.UInt64":
				case "System.Single":
				case "System.Double":
				case "System.Boolean":
					if (isOut) {
						setup_call_stack.AppendFormat ("\targ_ptrs [{0}] = p{0};\n", i);
					} else {
						setup_call_stack.AppendFormat ("\targ_ptrs [{0}] = &p{0};\n", i);
					}
					break;
				case "System.IntPtr":
					// no copy-back here
						if (isOut) {
						setup_call_stack.AppendFormat ("\t{1} a{0} = *p{0};\n", i, objctype);
						setup_call_stack.AppendFormat ("\targ_ptrs [{0}] = &a{0};\n", i);
					} else {
						setup_call_stack.AppendFormat ("\t{1} a{0} = p{0};\n", i, original_objctype);
						setup_call_stack.AppendFormat ("\targ_ptrs [{0}] = &a{0};\n", i);
					}
					break;
				case "MonoTouch.ObjCRuntime.Selector":
					if (isOut) {
						setup_call_stack.AppendFormat ("\tvoid *a{0} = *p{0} ? xamarin_get_selector (*p{0}) : NULL;\n", i);
						setup_call_stack.AppendFormat ("\targ_ptrs [{0}] = &a{0};\n", i);
						copyback.AppendFormat ("\t*p{0} = a{0};\n", i);
					} else {
						setup_call_stack.AppendFormat ("\targ_ptrs [{0}] = p{0} ? xamarin_get_selector (p{0}) : NULL;\n", i);
					}
					break;
				case "MonoTouch.ObjCRuntime.Class":
					if (isOut) {
						setup_call_stack.AppendFormat ("\tvoid *a{0} = *p{0} ? xamarin_get_class (*p{0}) : NULL;\n", i);
						setup_call_stack.AppendFormat ("\targ_ptrs [{0}] = &a{0};\n", i);
						copyback.AppendFormat ("\t*p{0} = a{0};\n", i);
					} else {
						setup_call_stack.AppendFormat ("\targ_ptrs [{0}] = p{0} ?  xamarin_get_class (p{0}) : NULL;\n", i);
					}
					break;
				case "System.String":
					// This should always be an NSString and never char*
					if (isOut) {
						setup_call_stack.AppendFormat ("\tMonoString *a{0} = *p{0} ? mono_string_new (mono_domain_get (), [(*p{0}) UTF8String]) : NULL;\n", i);
						setup_call_stack.AppendFormat ("\targ_ptrs [{0}] = &a{0};\n", i);
						copyback.AppendFormat ("\tchar *str{0} = mono_string_to_utf8 (a{0});\n", i);
						copyback.AppendFormat ("\t*p{0} = [[NSString alloc] initWithUTF8String:str{0}];\n", i);
						copyback.AppendFormat ("\t[*p{0} autorelease];\n", i);
						copyback.AppendFormat ("\tmono_free (str{0});\n", i);
					} else {
						setup_call_stack.AppendFormat ("\targ_ptrs [{0}] = p{0} ? mono_string_new (mono_domain_get (), [p{0} UTF8String]) : NULL;\n", i);
					}
					break;
				default:
					if (isArray) {
						var elementType = ((ArrayType) type).ElementType;
						
						setup_call_stack.AppendFormat ("\tif (p{0}) {{\n", i);
						setup_call_stack.AppendFormat ("\t\tNSArray *arr = (NSArray *) p{0};\n", i);
						setup_call_stack.AppendFormat ("\t\tMonoClass *e_class;\n");
						setup_call_stack.AppendFormat ("\t\tMonoArray *marr;\n");
						setup_call_stack.AppendFormat ("\t\tMonoType *p;\n");
						setup_call_stack.AppendFormat ("\t\tint j;\n", i);
						setup_call_stack.AppendFormat ("\t\t\tMonoType *p_class;\n");	
						setup_call_stack.AppendFormat ("\t\tp = xamarin_get_parameter_type (method_{1}, {0});\n", i, counter);
						setup_call_stack.AppendFormat ("\t\te_class = mono_class_get_element_class (mono_class_from_mono_type (p));\n");
						setup_call_stack.AppendFormat ("\t\tmarr = mono_array_new (mono_domain_get (), e_class, [arr count]);\n", i);
						setup_call_stack.AppendFormat ("\t\tfor (j = 0; j < [arr count]; j++) {{\n", i);
						if (elementType.FullName == "System.String") {
							setup_call_stack.AppendFormat ("\t\t\tNSString *sv = (NSString *) [arr objectAtIndex: j];\n", i);
							setup_call_stack.AppendFormat ("\t\t\tmono_array_set (marr, MonoString *, j, mono_string_new (mono_domain_get (), [sv UTF8String]));\n", i);
						} else if (elementType.IsNSObject ()) {
							setup_call_stack.AppendFormat ("\t\t\tNSObject *nobj = [arr objectAtIndex: j];\n");
							setup_call_stack.AppendFormat ("\t\t\tMonoObject *mobj{0} = NULL;\n", i);
							setup_call_stack.AppendFormat ("\t\t\tp_class = mono_class_get_type (e_class);\n");
							setup_call_stack.AppendFormat ("\t\t\tif (nobj) {{\n");
	//						if (MTouch.EnableDebug) {
	//							setup_call_stack.AppendFormat ("\t\t\t\tmobj{0} = get_managed_object_for_ptr_fast (nobj, false);\n", i);
	//							setup_call_stack.AppendFormat ("\t\t\t\txamarin_verify_parameter (mobj{0}, sel, self, nobj, {0}, e_class, method_{1});\n", i, counter);
	//						} else {
								setup_call_stack.AppendFormat ("\t\t\t\tmobj{0} = xamarin_get_nsobject_with_type_for_ptr (nobj, false, p_class);\n", i);
	//						}
							setup_call_stack.AppendFormat ("\t\t\t}}\n");
							setup_call_stack.AppendFormat ("\t\t\tmono_array_set (marr, MonoObject *, j, mobj{0});\n", i);
						} else {
							throw new MonoTouchException (4111, true, "The registrar cannot build a signature for type `{0}' in method `{1}`.", type.FullName, descriptiveMethodName);
						}
						setup_call_stack.AppendFormat ("\t\t}}\n");
						setup_call_stack.AppendFormat ("\t\targ_ptrs [{0}] = marr;\n", i);
						setup_call_stack.AppendFormat ("\t}} else {{\n");
						setup_call_stack.AppendFormat ("\t\targ_ptrs [{0}] = NULL;\n", i);
						setup_call_stack.AppendFormat ("\t}}\n");
					} else if (type.IsNSObject ()) {
						if (isOut) {
							setup_call_stack.AppendFormat ("\tNSObject *nsobj{0} = *(NSObject **) p{0};\n", i);
							setup_call_stack.AppendFormat ("\t\tMonoObject *mobj{0} = NULL;\n", i);
							setup_call_stack.AppendFormat ("\t\tif (nsobj{0}) {{\n", i);
	//						if (MTouch.EnableDebug) {
	//							setup_call_stack.AppendFormat ("\t\t\tmobj{0} = get_managed_object_for_ptr_fast (nsobj{0}, false);\n", i);
	//							setup_call_stack.AppendFormat ("\t\t\txamarin_verify_parameter (mobj{0}, sel, self, nsobj{0}, {0}, NULL, method_{1});\n", i, counter);
	//						} else {
								setup_call_stack.AppendFormat ("\t\t\tmobj{0} = xamarin_get_nsobject_with_type_for_ptr (nsobj{0}, false, xamarin_get_parameter_type (method_{1}, {0}));\n", i, counter);
	//						}
							setup_call_stack.AppendFormat ("\t\t}}\n");
							// argument semantics?
							setup_call_stack.AppendFormat ("\targ_ptrs [{0}] = (int *) &mobj{0};\n", i);
							copyback.AppendFormat ("\tint handle{0} = 0;\n", i);
							copyback.AppendFormat ("\tif (mobj{0} != NULL)\n", i);
							copyback.AppendFormat ("\t\thandle{0} = (int) xamarin_get_nsobject_handle (mobj{0});\n", i);
							copyback.AppendFormat ("\t*p{0} = (id) handle{0};\n", i);
						} else {
							setup_call_stack.AppendFormat ("\tNSObject *nsobj{0} = (NSObject *) p{0};\n", i);
							if (method.ArgumentSemantic == ArgumentSemantic.Copy) {
								setup_call_stack.AppendFormat ("\t\tnsobj{0} = [nsobj{0} copy];\n", i);
								setup_call_stack.AppendFormat ("\t\t[nsobj{0} autorelease];\n", i);
							}
							setup_call_stack.AppendFormat ("\t\tMonoObject *mobj{0} = NULL;\n", i);
							setup_call_stack.AppendFormat ("\t\tint32_t created{0} = false;\n", i);
							setup_call_stack.AppendFormat ("\t\tif (nsobj{0}) {{\n", i);
	//						if (MTouch.EnableDebug) {
	//							setup_call_stack.AppendFormat ("\t\t\tmobj{0} = get_managed_object_for_ptr_fast (nsobj{0}, false);\n", i);
	//							setup_call_stack.AppendFormat ("\t\t\txamarin_verify_parameter (mobj{0}, sel, self, nsobj{0}, {0}, NULL, method_{1});\n", i, counter);
	//						} else {
								setup_call_stack.AppendFormat ("\t\t\tmobj{0} = xamarin_get_nsobject_with_type_for_ptr_created (nsobj{0}, false, xamarin_get_parameter_type (method_{1}, {0}), &created{0});\n", i, counter);
	//						}
							setup_call_stack.AppendFormat ("\t\t}}\n");
							setup_call_stack.AppendFormat ("\t\targ_ptrs [{0}] = mobj{0};\n", i);

							if (SharedStatic.HasAttribute (paramBase, Registrar.ObjCRuntime, TypeSystemDescriptor.TransientAttribute)) {
								copyback.AppendFormat ("\t\tif (created{0})\n", i);
								copyback.AppendFormat ("\t\t\txamarin_dispose (mobj{0});\n", i);
							}
						}
					} else if (SharedStatic.IsNativeObject (td)) {
						TypeDefinition nativeObjType = td;

						if (td.IsInterface) {
							var wrapper_type = TypeSystemDescriptor.GetProtocolAttributeWrapperType (td);
							if (wrapper_type == null)
								throw new MonoTouchException (4125, true, "The registrar found an invalid type '{0}' in signature for method '{1}': " +
								                              "The interface must have a Protocol attribute specifying its wrapper type.",
								                              td.FullName, descriptiveMethodName);

							nativeObjType = wrapper_type.Resolve ();
						}

						// verify that the type has a ctor with two parameters
						if (!HasIntPtrBoolCtor (nativeObjType))
							throw new MonoTouchException (4103, true, 
							                              "The registrar found an invalid type `{0}` in signature for method `{1}`: " + 
							                              "The type implements INativeObject, but does not have a constructor that takes " +
							                              "two (IntPtr, bool) arguments.", td.FullName, descriptiveMethodName);

						if (!td.IsInterface) {
							// find the MonoClass for this parameter
							setup_call_stack.AppendFormat ("\tMonoType *type{0};\n", i);
							setup_call_stack.AppendFormat ("\t\ttype{0} = xamarin_get_parameter_type (method_{1}, {0});\n", i, counter);
						}
						if (isOut) {
							setup_call_stack.AppendFormat ("\tMonoObject *inobj{0};\n", i);
							if (td.IsInterface) {
								setup_call_stack.AppendFormat ("\tinobj{0} = xamarin_get_inative_object_static (*p{0}, false, \"{1}\", \"{2}\");\n", i, TypeSystemDescriptor.GetAssemblyQualifiedName (nativeObjType), TypeSystemDescriptor.GetAssemblyQualifiedName (td));
							} else {
								setup_call_stack.AppendFormat ("\tinobj{0} = xamarin_get_inative_object_dynamic (*p{0}, false, mono_type_get_object (mono_domain_get (), type{0}));\n", i);
							}
							setup_call_stack.AppendFormat ("\targ_ptrs [{0}] = &inobj{0};\n", i);
							copyback.AppendFormat ("\tid handle{0} = nil;\n", i);
							copyback.AppendFormat ("\tif (inobj{0} != NULL)\n", i);
							copyback.AppendFormat ("\t\thandle{0} = xamarin_get_handle_for_inativeobject (inobj{0});\n", i);
							copyback.AppendFormat ("\t*p{0} = (id) handle{0};\n", i);
						} else {
							if (td.IsInterface) {
								setup_call_stack.AppendFormat ("\targ_ptrs [{0}] = xamarin_get_inative_object_static (p{0}, false, \"{1}\", \"{2}\");\n", i, TypeSystemDescriptor.GetAssemblyQualifiedName (nativeObjType), TypeSystemDescriptor.GetAssemblyQualifiedName (td));
							} else {
								setup_call_stack.AppendFormat ("\targ_ptrs [{0}] = xamarin_get_inative_object_dynamic (p{0}, false, mono_type_get_object (mono_domain_get (), type{0}));\n", i);
							}

						}
					} else if (type.IsValueType) {
						if (isOut) {
							setup_call_stack.AppendFormat ("\targ_ptrs [{0}] = p{0};\n", i);
						} else {
							setup_call_stack.AppendFormat ("\targ_ptrs [{0}] = &p{0};\n", i);
						}
					} else if (td.BaseType.FullName == "System.MulticastDelegate") {
						if (isOut) {
							throw new MonoTouchException (4110, true,
							                              "The registrar cannot marshal the out parameter of type `{0}` in signature for method `{1}`.",
							                              type.FullName, descriptiveMethodName);
						} else {
							// Bug #4858 (also related: #4718)
							setup_call_stack.AppendFormat ("\tif (p{0})\n\t\targ_ptrs [{0}] = (void*) xamarin_get_delegate_for_block_parameter (method_{1}, {0}, p{0});\n", i, counter);
							setup_call_stack.AppendFormat ("\telse\n\t\targ_ptrs [{0}] = NULL;\n", i);
						}
					} else {
						throw new MonoTouchException (4105, true,
						                              "The registrar cannot marshal the parameter of type `{0}` in signature for method `{1}`.",
						                              type.FullName, descriptiveMethodName);
					}
					break;
				}
			}
			
			// the actual invoke
			if (isCtor) {
				invoke.AppendLine ("uint8_t flags = NSObjectFlagsNativeRef;");
				invoke.AppendFormat ("\tmthis = mono_object_new (mono_domain_get (), mono_method_get_class (method_{0}));\n", counter);
				invoke.AppendLine ("\txamarin_set_nsobject_handle (mthis, self);");
				invoke.AppendLine ("\txamarin_set_nsobject_flags (mthis, flags);");
				invoke.AppendFormat ("\tmono_runtime_invoke (method_{0}, mthis, arg_ptrs, NULL);\n", counter);
				invoke.AppendLine ("\txamarin_create_managed_ref (self, mthis, true);");
			} else {
				invoke.Append ("\t");
				if (!isVoid)
					invoke.Append ("MonoObject* retval = ");
				invoke.AppendFormat ("mono_runtime_invoke (method_{1}, {0}, arg_ptrs, NULL);\n", isStatic ? "NULL" : "mthis", counter);
			}
			
			// prepare the return value
			if (!isVoid) {
				var retain = method.MethodDefinition.MethodReturnType.HasCustomAttribute (Registrar.ObjCRuntime, Registrar.StringConstants.ReleaseAttribute);
				if (isStret) {
					int size = StaticRegistrar.GetValueTypeSize (returntype.Resolve (), false);
					if (size <= 0)
						throw new MonoTouchException (4106, true, "The registrar cannot marshal the return value for structure `{0}` in signature for method `{1}`.",
						                              returntype.FullName, descriptiveMethodName);
					setup_return.AppendFormat ("\tmemcpy (buffer, mono_object_unbox (retval), {0});\n", size);
				} else {
					setup_return.AppendFormat ("\t{0} res;\n", rettype);
					var isArray = returntype is ArrayType;
					var type = returntype.Resolve ();

					if (type == null)
						throw new MonoTouchException (4104, true, "The registrar cannot marshal the return value for type `{0}` in signature for method `{1}`.", returntype.FullName, descriptiveMethodName);

					if (returntype.IsValueType) {
						setup_return.AppendFormat ("\tres = *({0} *) mono_object_unbox (retval);\n", rettype);
					} else if (isArray) {
						var elementType = ((ArrayType) returntype).ElementType;

						setup_return.AppendFormat ("\tif (retval) {{\n");
						setup_return.AppendFormat ("\t\tint length = mono_array_length ((MonoArray *) retval);\n");
						setup_return.AppendFormat ("\t\tint i;\n");
						setup_return.AppendFormat ("\t\tid* buf = (id *) malloc (sizeof (id) * length);\n");
						setup_return.AppendFormat ("\t\tfor (i = 0; i < length; i++) {{\n");
						setup_return.AppendFormat ("\t\t\tMonoObject *value = mono_array_get ((MonoArray *) retval, MonoObject *, i);\n");
						
						if (elementType.FullName == "System.String") {
							setup_return.AppendFormat ("\t\t\tchar *str = mono_string_to_utf8 ((MonoString *) value);\n");
							setup_return.AppendFormat ("\t\t\tNSString *sv = [[NSString alloc] initWithUTF8String:str];\n");
							setup_return.AppendFormat ("\t\t\t[sv autorelease];\n");
							setup_return.AppendFormat ("\t\t\tmono_free (str);\n");
							setup_return.AppendFormat ("\t\t\tbuf [i] = sv;\n");
						} else if (elementType.IsNSObject ()) {
							setup_return.AppendFormat ("\t\t\tbuf [i] = xamarin_get_nsobject_handle (value);\n");
						} else {
							throw new MonoTouchException (4111, true, "The registrar cannot build a signature for type `{0}' in method `{1}`.", returntype.FullName, descriptiveMethodName);
						}
						
						setup_return.AppendFormat ("\t\t}}\n");

						setup_return.AppendFormat ("\t\tNSArray *arr = [[NSArray alloc] initWithObjects: buf count: length];\n");
						setup_return.AppendFormat ("\t\tfree (buf);\n");
						if (!retain)
							setup_return.AppendFormat ("\t\t[arr autorelease];\n");
						setup_return.AppendFormat ("\t\tres = arr;\n");
						setup_return.AppendFormat ("\t}} else {{\n");
						setup_return.AppendFormat ("\t\tres = NULL;\n");
						setup_return.AppendFormat ("\t}}\n");
					} else {
						setup_return.AppendFormat ("\tif (!retval) {{\n");
						setup_return.AppendFormat ("\t\tres = NULL;\n");
						setup_return.AppendFormat ("\t}} else {{\n");
						switch (type.FullName) {
						case "MonoTouch.ObjCRuntime.Selector":
							setup_return.AppendFormat ("\t\tres = xamarin_get_selector_handle (retval);\n");
							break;
						case "MonoTouch.ObjCRuntime.Class":
							setup_return.AppendFormat ("\t\tres = xamarin_get_class_handle (retval);\n");
							break;
						case "System.String":
							// This should always be an NSString and never char*
							setup_return.AppendFormat ("\t\tchar *str = mono_string_to_utf8 ((MonoString *) retval);\n");
							setup_return.AppendFormat ("\t\tNSString *nsstr = [[NSString alloc] initWithUTF8String:str];\n");
							if (!retain)
								setup_return.AppendFormat ("\t\t[nsstr autorelease];\n");
							setup_return.AppendFormat ("\t\tmono_free (str);\n");
							setup_return.AppendFormat ("\t\tres = nsstr;\n");
							break;
						default:
							if (type.IsNSObject ()) {
								setup_return.AppendFormat ("\t\tid retobj;\n");
								setup_return.AppendFormat ("\t\tretobj = xamarin_get_nsobject_handle (retval);\n");
								setup_return.AppendFormat ("\t\t[retobj retain];\n");
								if (!retain)
									setup_return.AppendFormat ("\t\t[retobj autorelease];\n");
								setup_return.AppendFormat ("\t\tres = retobj;\n");
							} else if (SharedStatic.IsNativeObject (type)) {
								setup_return.AppendFormat ("\t\tid retobj;\n");
								setup_return.AppendFormat ("\t\tretobj = xamarin_get_handle_for_inativeobject (retval);\n");
								setup_return.AppendFormat ("\t\t[retobj retain];\n");
								if (!retain)
									setup_return.AppendFormat ("\t\t[retobj autorelease];\n");
								setup_return.AppendFormat ("\t\tres = retobj;\n");
							} else {
								throw new MonoTouchException (4104, true, 
								                              "The registrar cannot marshal the return value for type `{0}` in signature for method `{1}`.",
								                              returntype.FullName, descriptiveMethodName);
							}
							break;
						}
						setup_return.AppendFormat ("\t}}\n");
					}
				}
			}

			// Write out everything
			
			sb.AppendFormat ("static MonoMethod *method_{0} = NULL;\n", counter);
			
			sb.Append (signature.ToString ());
			sb.AppendLine ("{");
			
			sb.Append (comment.ToString ());
			sb.AppendFormat ("\tvoid *arg_ptrs [{0}];\n", num_arg);
			if (!isStatic)
				sb.AppendLine ("\tMonoObject *mthis;");
			
			sb.Append ("\tif (mono_domain_get () == NULL)\n");
			sb.Append ("\t\tmono_jit_thread_attach (NULL);\n");
			
			if (isCtor) {
				sb.AppendLine ("\tif (xamarin_try_get_nsobject (self))");
				sb.AppendLine ("\t\treturn self;");
			}
			
			// no locking should be required here, it doesn't matter if we overwrite the field (it'll be the same value).
			sb.AppendFormat ("\tif (!method_{0})\n", counter);
			sb.AppendFormat ("\t\tmethod_{0} = xamarin_get_reflection_method_method (xamarin_get_method_for_selector ([self class], sel).method);\n", counter);
				
			if (!isStatic && !isCtor) {
				sb.AppendLine ("\t\tmthis = NULL;");
				sb.AppendLine ("\t\tif (self) {");
	//			if (MTouch.EnableDebug) {
	//				sb.AppendLine ("\t\t\tmthis = get_managed_object_for_ptr_fast (self, false);");
	//				sb.AppendFormat ("\t\t\tcheck_for_gced_object (mthis, sel, self, method_{0});\n", counter);
	//			} else {
					sb.AppendFormat ("\t\t\tmthis = xamarin_get_nsobject_with_type_for_ptr (self, false, mono_class_get_type (mono_method_get_class (method_{0})));\n", counter);
	//			}
				sb.AppendLine ("\t\t}");
			}

			if (verbose)
				sb.Append (nslog_start.ToString ());
			
			sb.Append (setup_call_stack.ToString ());
			sb.Append (invoke.ToString ());
			sb.Append (copyback.ToString ());
			sb.Append (setup_return.ToString ());
			
			if (verbose)
				sb.Append (nslog_end.ToString ());
			
			if (isCtor) {
				sb.AppendLine ("\treturn self;");
			} else if (isStret) {
				// nothing to do
			} else if (isVoid) {
				sb.AppendLine ("\treturn NULL;");
			} else {
				sb.AppendLine ("\treturn res;");
			}
			
			sb.AppendLine ("}");
			sb.AppendLine ();
			
			method.SpecializedTrampoline = name;
		}

		public void GeneratePInvokeWrappers (List<MethodDefinition> marshal_exception_pinvokes, string header_path, string source_path)
		{
			throw new NotSupportedException ();
		}

		public void GenerateSingleAssembly (IEnumerable<AssemblyDefinition> assemblies, string header_path, string source_path, string single_assembly)
		{
			throw new NotSupportedException ();
		}

		public void Generate (IEnumerable<AssemblyDefinition> assemblies, string header_path, string source_path)
		{
			Driver.WriteIfDifferent (header_path, string.Empty);
			Driver.WriteIfDifferent (source_path, Generate (assemblies));
		}

		public static string Generate (IEnumerable<AssemblyDefinition> assemblies)
		{
			var classes = TypeSystemDescriptor.Describe (assemblies);
			var sb = new StringBuilder ();
			var specialized_trampolines = new StringBuilder ();
			int count = 0;
			int map_count = 0;
			List<Exception> exceptions = null;

			foreach (var @class in classes) {
				foreach (var method in @class.Methods) {
					try {
						Specialize (specialized_trampolines, method);
					} catch (Exception ex) {
						if (exceptions == null)
							exceptions = new List<Exception> ();
						if (!(ex is MonoTouchException))
							ex = new MonoTouchException (4114, true, 
							                             "Unexpected error in the registrar for the method '{0}.{1}' - Please file a bug report at http://bugzilla.xamarin.com", 
							                             method.MethodDefinition.DeclaringType.FullName, 
							                             method.MethodDefinition.Name);
						exceptions.Add (ex);
					}
				}
			}
			if (exceptions != null)
				throw new AggregateException (exceptions);
			
			sb.AppendLine ("#include <xamarin/xamarin.h>");
			sb.AppendLine ("#include <objc/objc.h>");
			sb.AppendLine ("#include <objc/runtime.h>");
			sb.AppendLine ("#include <UIKit/UIKit.h>");
			sb.AppendLine (headers.ToString ());
			sb.AppendLine (structs.ToString ());
			sb.AppendLine (specialized_trampolines.ToString ());
			sb.AppendLine ();

			sb.AppendLine ("static MTClassMap __xamarin_class_map [] = {");
			foreach (var @class in classes) {
				if (@class.IsModel)
					continue;
				sb.AppendFormat ("\t{{\"{0}\", \"{1}\", {2}}},\n", @class.Name, @class.TypeName, 0);
				map_count++;
			}
			sb.AppendLine ("};");
			sb.AppendLine ();

			sb.AppendLine ("static MTClass __xamarin_classes [] = {");
			foreach (var @class in classes) {
				if (@class.InstanceVariables.Count == 0 && @class.Methods.Count == 0 && @class.Properties.Count == 0)
					continue;
				sb.AppendFormat ("\t{{\"{0}\", \"{1}\", {2}, {3}, {4}}},\n", @class.Name, @class.SuperName, @class.InstanceVariables.Count, @class.Methods.Count, @class.Properties.Count);
				count++;
			}
			sb.AppendLine ("};");
			sb.AppendLine ();
			
			sb.AppendLine ("static MTIvar __xamarin_ivars [] = {");
			foreach (var @class in classes) {
				foreach (var ivar in @class.InstanceVariables) 
					sb.AppendFormat ("\t{{\"{0}\", \"{1}\", {2}, {3}}},\n", ivar.Name, ivar.Type, ivar.Size, ivar.Align);
			}
			sb.AppendLine ("};");
			sb.AppendLine ();
			
			sb.AppendLine ("static MTMethod __xamarin_methods [] = {");
			foreach (var @class in classes) {
				foreach (var method in @class.Methods)
					sb.AppendFormat ("\t{{\"{0}\",\"{1}\",{2}, (void *) {3}}},\n", method.Selector, method.Signature, method.IsStatic ? "1" : "0", method.GetTrampoline ());
			}
			sb.AppendLine ("};");
			sb.AppendLine ();

			sb.AppendLine ("static MTProperty __xamarin_properties [] = {");
			foreach (var @class in classes) {
				foreach (var property in @class.Properties) {
					sb.AppendFormat ("\t{{\"{0}\", \"{1}\", \"{2}\"}},\n", property.Name, property.Type, property.ArgumentSemantic);
				}
			}
			sb.AppendLine ("};");
			sb.AppendLine ();

			sb.AppendLine ("static const char *__xamarin_registration_assemblies []= {");
			int assembly_count = 0;
			foreach (var @assembly in assemblies) {
				if (assembly_count > 0)
					sb.Append (", ");
				sb.AppendFormat ("\t\"{0}\"\n", assembly.Name.Name);
				assembly_count++;
			}
			sb.AppendLine ("};");

			sb.AppendFormat ("const int __xamarin_map_count = {0};\n", map_count);
			sb.AppendFormat ("const int __xamarin_class_count = {0};\n", count);


			sb.AppendLine ("static struct MTRegistrationMap __xamarin_registration_map = {");
			sb.AppendLine ("NULL,");
			sb.AppendLine ("__xamarin_registration_assemblies,");
			sb.AppendLine ("__xamarin_class_map,");
			sb.AppendFormat ("{0},\n", assembly_count);
			sb.AppendLine ("__xamarin_map_count,");
			sb.AppendLine ("0");
			sb.AppendLine ("};");

			sb.AppendLine ();
			sb.AppendLine ("void xamarin_create_classes (void) {");
			sb.AppendLine ("	int i,j;");
			sb.AppendLine ("	int ivar_offset = 0;");
			sb.AppendLine ("	int method_offset = 0;");
			sb.AppendLine ("	int prop_offset = 0;");
			sb.AppendLine ();
			sb.AppendLine ("	for (i = 0; i < __xamarin_class_count; i++) {");
			sb.AppendLine ("		Class handle = objc_allocateClassPair (objc_getClass (__xamarin_classes [i].supername), __xamarin_classes [i].name, 0);");
			sb.AppendLine ("		if (handle == NULL) {");
			sb.AppendLine ("			ivar_offset += __xamarin_classes [i].ivar_count;");
			sb.AppendLine ("			method_offset += __xamarin_classes [i].method_count;");
			sb.AppendLine ("			prop_offset += __xamarin_classes [i].prop_count;");
			sb.AppendLine ("			continue;");
			sb.AppendLine ("		}");
			sb.AppendLine ("		for (j = 0; j < __xamarin_classes [i].ivar_count; j++, ivar_offset++)");
			sb.AppendLine ("			class_addIvar (handle, __xamarin_ivars [ivar_offset].name, __xamarin_ivars [ivar_offset].size, __xamarin_ivars [ivar_offset].align, __xamarin_ivars [ivar_offset].type);");
			sb.AppendLine ("		class_addMethod (handle, sel_registerName (\"release\"), (IMP) &xamarin_release_trampoline, \"v@:\");");
			sb.AppendLine ("		class_addMethod (handle, sel_registerName (\"retain\"), (IMP) &xamarin_retain_trampoline, \"@@:\");");
			sb.AppendLine ("		class_addMethod (handle, sel_registerName (\"conformsToProtocol:\"), (IMP) &xamarin_trampoline, \"B@:^v\");");
			sb.AppendLine ("		for (j = 0; j < __xamarin_classes [i].method_count; j++, method_offset++) {");
			sb.AppendLine ("			Class h = (__xamarin_methods [method_offset].isstatic ? object_getClass (handle) : handle);");
			sb.AppendLine ("			class_addMethod (h, sel_registerName (__xamarin_methods [method_offset].selector), (IMP) __xamarin_methods [method_offset].trampoline, __xamarin_methods [method_offset].signature);");
			sb.AppendLine ("		}");
			sb.AppendLine ("		for (j = 0; j < __xamarin_classes [i].prop_count; j++, prop_offset++) {");
			sb.AppendLine ("			int count = 0;");
			sb.AppendLine ("			objc_property_attribute_t props[3];");
			sb.AppendLine ("			props [count].name = \"T\";");
			sb.AppendLine ("			props [count++].value = __xamarin_properties [prop_offset].type;");
			sb.AppendLine ("			if (*__xamarin_properties [prop_offset].argument_semantic != 0) {");
			sb.AppendLine ("				props [count].name = __xamarin_properties [prop_offset].argument_semantic;");
			sb.AppendLine ("				props [count++].value = \"\";");
			sb.AppendLine ("			}");
			sb.AppendLine ("			props [count].name = \"V\";");
			sb.AppendLine ("			props [count++].value = __xamarin_properties [prop_offset].name;");
			sb.AppendLine ("			class_addProperty (handle, __xamarin_properties [prop_offset].name, props, count);;");
			sb.AppendLine ("		}");
			sb.AppendLine ("		objc_registerClassPair (handle);");
			sb.AppendLine ("	}");
			sb.AppendLine ("	for (i = 0; i < __xamarin_map_count; i++) {");
			sb.AppendLine ("		__xamarin_class_map [i].handle = objc_getClass (__xamarin_class_map [i].name);");	
			sb.AppendLine ("	}");
			sb.AppendLine ("}");

			return sb.ToString ();
		}
	}
		
}