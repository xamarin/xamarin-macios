//
// Registrar.cs: Common code for the static and dynamic registrars.
// 
// Authors:
//   Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2013 Xamarin Inc. 
//

// #define VERBOSE_REGISTRAR

using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

using Xamarin.Bundler;

#if LEGACY_TOOLS || BUNDLER
using Xamarin.Utils;
using TAssembly = Mono.Cecil.AssemblyDefinition;
using TType = Mono.Cecil.TypeReference;
using TMethod = Mono.Cecil.MethodDefinition;
using TProperty = Mono.Cecil.PropertyDefinition;
using TField = Mono.Cecil.FieldDefinition;
using R = Registrar.Registrar;
#else
using TAssembly = System.Reflection.Assembly;
using TType = System.Type;
using TMethod = System.Reflection.MethodBase;
using TProperty = System.Reflection.PropertyInfo;
using TField = System.Reflection.FieldInfo;
using R = ObjCRuntime.Runtime;
#endif

#if !(LEGACY_TOOLS || BUNDLER)
using ProductException = ObjCRuntime.RuntimeException;
#endif

#if !LEGACY_TOOLS && !BUNDLER
// static registrar needs them but they might not be marked (e.g. if System.Console is not used)
[assembly: Preserve (typeof (System.Action))]
[assembly: Preserve (typeof (System.Action<string>))]
#endif

#nullable enable

//
// This file cannot use any cecil code, since it's also compiled into Microsoft.[iOS|macOS|tvOS|MacCatalyst].dll
//

#if MONOMAC
namespace ObjCRuntime {
	/// <param name="sender">To be added.</param>
	///     <param name="args">To be added.</param>
	///     <summary>To be added.</summary>
	///     <remarks>To be added.</remarks>
	public delegate void AssemblyRegistrationHandler (object? sender, AssemblyRegistrationEventArgs args);

	/// <summary>To be added.</summary>
	///     <remarks>To be added.</remarks>
	public class AssemblyRegistrationEventArgs : EventArgs {
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public bool Register { get; set; }
		/// <summary>To be added.</summary>
		///         <value>To be added.</value>
		///         <remarks>To be added.</remarks>
		public System.Reflection.AssemblyName? AssemblyName { get; internal set; }
	}
}
#endif

namespace Registrar {
	static class Shared {

		public static List<ProductException> GetMT4127 (TMethod impl, List<TMethod> ifaceMethods)
		{
			var exceptions = new List<ProductException> ();
			exceptions.Add (ErrorHelper.CreateError (4127, Errors.MT4127, impl.DeclaringType?.FullName, impl.Name));
			for (int i = 0; i < ifaceMethods.Count; i++) {
				var ifaceM = ifaceMethods [i];
				exceptions.Add (ErrorHelper.CreateError (4137, Errors.MT4137, impl.DeclaringType?.FullName, impl.Name, ifaceM.DeclaringType?.FullName, ifaceM.Name));
			}
			return exceptions;
		}
	}

	abstract partial class Registrar {
#if LEGACY_TOOLS || BUNDLER
		public Application App { get; private set; }
#endif

		const string NFloatTypeName = "System.Runtime.InteropServices.NFloat";

		Dictionary<TAssembly, object?> assemblies = new (); // Use Dictionary instead of HashSet to avoid pulling in System.Core.dll.
															// locking: all accesses must lock 'types'.
		Dictionary<TType, ObjCType> types = new Dictionary<TType, ObjCType> ();
		// this is used to check if multiple types are registered with the same name.
		// locking: all accesses must lock 'type_map'.
		Dictionary<string, TType> type_map = new Dictionary<string, TType> ();
		// this is used to check if multiple protocols are registered with the same name.
		// locking: all accesses must lock 'protocol_map'.
		Dictionary<string, TType> protocol_map = new Dictionary<string, TType> ();
		// this is used to check if multiple categories are registered with the same name.
		// locking: all accesses must lock 'categories_map'.
		Dictionary<string, TType> categories_map = new Dictionary<string, TType> ();
		TMethod? conforms_to_protocol;
		TMethod? invoke_conforms_to_protocol;

		public IEnumerable<TAssembly> GetAssemblies ()
		{
			return assemblies.Keys;
		}


		internal class ObjCType {
			public readonly Registrar Registrar;
			public RegisterAttribute? RegisterAttribute;
			public CategoryAttribute? CategoryAttribute;
			public readonly TType Type;
			public ObjCType? BaseType;
			// This only contains the leaf protocols implemented by this type.
			public ObjCType []? Protocols;
			public string []? AdoptedProtocols;
			public bool IsModel;
			// if this type represents an ObjC protocol (!= has the protocol attribute, since that can be applied to all kinds of things).
			public bool IsProtocol;
			public bool IsInformalProtocol;
			public bool IsWrapper;
			public bool IsGeneric;
#if !LEGACY_TOOLS && !BUNDLER
			public IntPtr Handle;
#else
			public TType? ProtocolWrapperType;
			public int ClassMapIndex;
#endif

			public Dictionary<string, ObjCField>? Fields;
			public List<ObjCMethod>? Methods;
			public List<ObjCProperty>? Properties;

			Dictionary<string, ObjCMember> Map = new Dictionary<string, ObjCMember> ();

			ObjCType? superType;

			public bool IsCategory { get { return CategoryAttribute is not null; } }

			public bool IsStubClass {
				get => RegisterAttribute?.IsStubClass == true;
			}

			public ObjCType (Registrar registrar, TType type)
			{
				this.Registrar = registrar;
				this.Type = type;
			}

#if LEGACY_TOOLS || BUNDLER
			HashSet<ObjCType>? all_protocols;
			// This contains all protocols in the type hierarchy.
			// Given a type T that implements a protocol with super protocols:
			// class T : NSObject, IProtocol2 {}
			// [Protocol]
			// interface IP1 {}
			// [Protocol]
			// interface IP2 : IP1 {}
			// This property will contain both IP1 and IP2. The Protocols property only contains IP2.
			public IEnumerable<ObjCType>? AllProtocols {
				get {
					if (Protocols is null || Protocols.Length == 0)
						return null;

					if (all_protocols is null) {
						var queue = new Queue<ObjCType> (Protocols);
						var rv = new HashSet<ObjCType> ();
						while (queue.Count > 0) {
							var type = queue.Dequeue ();
							if (rv.Add (type)) {
								foreach (var iface in type.Type!.Resolve ().Interfaces) {
									if (!Registrar.Types.TryGetValue (iface.InterfaceType, out var superIface)) {
										// This is not an interface that corresponds to a protocol.
										continue;
									}
									queue.Enqueue (superIface);
								}
							}
						}
						all_protocols = rv;
					}

					return all_protocols;
				}
			}

			HashSet<ObjCType>? all_protocols_in_hierarchy;
			public IEnumerable<ObjCType> AllProtocolsInHierarchy {
				get {
					if (all_protocols_in_hierarchy is null) {
						all_protocols_in_hierarchy = new HashSet<ObjCType> ();
						var type = this;
						while (type is not null && (object) type != (object?) type.BaseType) {
							var allProtocols = type.AllProtocols;
							if (allProtocols is not null)
								all_protocols_in_hierarchy.UnionWith (allProtocols);
							type = type.BaseType;
						}
					}

					return all_protocols_in_hierarchy;
				}
			}
#endif

			public void VerifyRegisterAttribute ([NotNullIfNotNull (nameof (exceptions))] ref List<Exception>? exceptions)
			{
				if (RegisterAttribute is null)
					return;

				var name = RegisterAttribute.Name;
#if NET
				if (string.IsNullOrEmpty (name))
#else
				if (string.IsNullOrEmpty (name) || name is null)
#endif
					return;

				for (int i = 0; i < name.Length; i++) {
					if (!char.IsWhiteSpace (name [i]))
						continue;
					// There is no Objective-C spec, and the C/C++ spec mentions "implementation-defined characters",
					// so afaict there's no list of valid identifier characters. However we do know that whitespace
					// is not allowed, so notify the developer about that. We're only emitting an error if we can detect
					// that it's a name that will cause the static registrar output to fail to compile
					// (currently if the whitespace is not at the start or end of the name), since otherwise it's
					// a breaking change.
					// See also bug #25441.
					var showError = false;
					var trimmedName = name.Trim (); // Trim removes any characters that returns true for 'char.IsWhiteSpace', so it matches our condition above
					for (int k = 0; k < trimmedName.Length; k++) {
						if (char.IsWhiteSpace (trimmedName [k])) {
							showError = true;
							break;
						}
					}
					AddException (ref exceptions, new ProductException (4146, showError, Errors.MT4146, Registrar.GetTypeFullName (Type), name [i], ((int) name [i]).ToString ("x"), name));
					break;
				}
			}

			// This list is duplicated in tests/mtouch/RegistrarTest.cs.
			// Update that list whenever this list is updated.
			static readonly char [] invalidSelectorCharacters = { ' ', '\t', '?', '\\', '!', '|', '@', '"', '\'', '%', '&', '/', '(', ')', '=', '^', '[', ']', '{', '}', ',', '.', ';', '-', '\n', '<', '>' };
			void VerifySelector (ObjCMethod method, [NotNullIfNotNull (nameof (exceptions))] ref List<Exception>? exceptions)
			{
				if (method.Method is null)
					return;

				var split = method.Selector.Split (':');
				var nativeParamCount = split.Length - 1;

				if (method.IsVariadic)
					nativeParamCount++;

				var pars = method.Parameters;
				var paramCount = pars is null ? 0 : pars.Length;
				if (method.IsCategoryInstance)
					paramCount--;

				if (nativeParamCount != paramCount) {
					Exception ex;

					if (method.IsVariadic) {
						ex = Registrar.CreateException (4140, method, Errors.MT4140,
							method.Method.DeclaringType?.FullName, method.MethodName, nativeParamCount, paramCount, method.Selector);
					} else {
						ex = Registrar.CreateException (4117, method, Errors.MT4117,
							method.Method.DeclaringType?.FullName, method.MethodName, nativeParamCount, paramCount, method.Selector);
					}

					Registrar.AddException (ref exceptions, ex);
				}

				if (method.IsVariadic && pars is not null && Registrar.GetTypeFullName (pars [paramCount - 1]) != "System.IntPtr")
					Registrar.AddException (ref exceptions, Registrar.CreateException (4123, method, Errors.MT4123, Registrar.GetDescriptiveMethodName (method.Method)));

				char ch;
				var idx = method.Selector.IndexOfAny (invalidSelectorCharacters);
				if (idx != -1) {
					ch = method.Selector [idx];
					Registrar.AddException (ref exceptions, Registrar.CreateException (4160, method, Errors.MT4160,
						ch, ((int) ch).ToString ("x"), method.Selector, Registrar.GetTypeFullName (Type), Registrar.GetDescriptiveMethodName (method.Method)));
				}
			}

			public void VerifyAdoptedProtocolsNames ([NotNullIfNotNull (nameof (exceptions))] ref List<Exception>? exceptions)
			{
				if (AdoptedProtocols is null)
					return;

				foreach (var adoptedProtocol in AdoptedProtocols) {
					// Tested all of the current 'invalidSelectorCharacters' for protocol names and all of them are invalid.
					char ch;
					var idx = adoptedProtocol.IndexOfAny (invalidSelectorCharacters);
					var ap = adoptedProtocol;
					if (idx != -1) {
						ch = adoptedProtocol [idx];
						var str = ch.ToString ();
						if (ch == '{') {
							str = "{{";
							ap = ap.Insert (idx, "{");
						} else if (ch == '}') {
							str = "}}";
							ap = ap.Insert (idx, "}");
						}

						var m4177 = String.Format (Errors.MT4177, Registrar.GetTypeFullName (Type), ap, str);
						AddException (ref exceptions, new ProductException (4177, true, m4177, new string [0]));

					}
				}
			}

			public void Add (ObjCField field, [NotNullIfNotNull (nameof (exceptions))] ref List<Exception>? exceptions)
			{
				// Check if there are any base types with the same property.
				var base_type = BaseType;
				var fieldNameInDictionary = (field.IsStatic ? "+" : "-") + field.Name;
				while (base_type is not null) {
					if (base_type.Fields is not null && base_type.Fields.ContainsKey (fieldNameInDictionary)) {
						// Maybe we should warn here? Not sure if this is something bad or not.
						return;
					}
					if (base_type == base_type.BaseType)
						break;
					base_type = base_type.BaseType;
				}

				if (Fields is null)
					Fields = new Dictionary<string, ObjCField> ();
				// Do fields and methods live in the same objc namespace? // AddToMap (field, errorIfExists);
				Fields.Add (fieldNameInDictionary, field);
			}

			public bool Add (ObjCMethod method, [NotNullIfNotNull (nameof (exceptions))] ref List<Exception>? exceptions)
			{
				bool rv;

				if (Methods is null)
					Methods = new List<ObjCMethod> ();

				VerifySelector (method, ref exceptions);
				method.ValidateSignature (ref exceptions);
				// Protocol members don't show up in the generated output, so it doesn't matter if we run into those.
				// If a class implements a protocol, it will still hit this check on the implemented members.
				if (!method.IsPropertyAccessor && !method.DeclaringType.IsProtocol)
					Registrar.VerifyInSdk (ref exceptions, method);

				rv = AddToMap (method, ref exceptions, out var alreadyAdded);
				if (!alreadyAdded)
					Methods.Add (method);
				return rv;
			}

			public void Add (ObjCProperty property, [NotNullIfNotNull (nameof (exceptions))] ref List<Exception>? exceptions)
			{
				if (Properties is null)
					Properties = new List<ObjCProperty> ();
				// Do properties and methods live in the same objc namespace? // AddToMap (property, errorIfExists);
				Properties.Add (property);
				// Protocol members don't show up in the generated output, so it doesn't matter if we run into those.
				// If a class implements a protocol, it will still hit this check on the implemented members.
				if (!property.DeclaringType.IsProtocol)
					Registrar.VerifyInSdk (ref exceptions, property);
				VerifyIsNotKeyword (ref exceptions, property);
			}

			public static bool IsObjectiveCKeyword (string? name)
			{
				switch (name) {
				case "auto":
				case "break":
				case "case":
				case "char":
				case "const":
				case "continue":
				case "default":
				case "do":
				case "double":
				case "else":
				case "enum":
				case "export":
				case "extern":
				case "float":
				case "for":
				case "goto":
				case "if":
				case "inline":
				case "int":
				case "long":
				case "register":
				case "restrict":
				case "return":
				case "short":
				case "signed":
				case "sizeof":
				case "static":
				case "struct":
				case "switch":
				case "template":
				case "typedef":
				case "union":
				case "unsigned":
				case "void":
				case "volatile":
				case "while":
				case "_Bool":
				case "_Complex":
				case "_Imaginery":
					return true;
				default:
					return false;
				}
			}

			void VerifyIsNotKeyword ([NotNullIfNotNull (nameof (exceptions))] ref List<Exception>? exceptions, ObjCProperty property)
			{
				if (IsObjectiveCKeyword (property.Selector))
					AddException (ref exceptions, CreateException (4164, property, Errors.MT4164, property.Name, property.Selector));
			}

			public bool TryGetMember (string selector, bool is_static, [NotNullWhen (true)] out ObjCMember? member)
			{
				if (is_static)
					selector = "+" + selector;
				else
					selector = "-" + selector;
				return Map.TryGetValue (selector, out member);
			}

			bool AddToMap (ObjCMember member, [NotNullIfNotNull (nameof (exceptions))] ref List<Exception>? exceptions, out bool alreadyAdded)
			{
				bool rv = true;

				alreadyAdded = false;

				if (TryGetMember (member.Selector, member.IsNativeStatic, out var existing)) {
					if (existing is ObjCMethod existingMethod && member is ObjCMethod method && object.ReferenceEquals (method.Method, existingMethod.Method)) {
						// This can happen if we try to register:
						// * A property declared on a type (we register the getter + setter)
						// * The getter or setter matches the signature of an exported method from a protocol interface, in which case we'll try to export again.
						alreadyAdded = true;
						return true;
					}
					if (existing.IsImplicit) {
						AddException (ref exceptions, CreateException (4141, member, Errors.MT4141, member.Selector, Registrar.GetTypeFullName (Type), Registrar.GetMemberName (member)));
					} else {
						AddException (ref exceptions, CreateException (4119, member, Errors.MT4119, member.Selector, Registrar.GetTypeFullName (Type), Registrar.GetMemberName (member), Registrar.GetMemberName (existing)));
					}
					rv = false;
				}

				Map [(member.IsNativeStatic ? "+" : "-") + member.Selector] = member;
				return rv;
			}

			Exception CreateException (int code, ObjCMember member, string message, params object? [] args)
			{
				var method = member as ObjCMethod;
				if (method is not null)
					return Registrar.CreateException (code, method.Method, message, args);
				var property = member as ObjCProperty;
				if (property is not null && property.Property is not null)
					return Registrar.CreateException (code, property.Property.GetMethod ?? property.Property.SetMethod, message, args);
				return Registrar.CreateException (code, message, args);
			}

			public string? Name {
				get { return RegisterAttribute is not null && RegisterAttribute.Name is not null ? RegisterAttribute.Name : Registrar.GetTypeFullName (Type); }
			}

			public string? CategoryName {
				get {
					if (!IsCategory)
						throw new InvalidOperationException ();
					var attrib = CategoryAttribute;
					var name = attrib?.Name ?? Registrar.GetTypeFullName (Type);
					return SanitizeObjectiveCName (name);
				}
			}

			public string ProtocolName {
				get {
					if (!IsProtocol)
						throw new InvalidOperationException ();
					var attrib = Registrar.GetProtocolAttribute (Type!);
					var name = attrib?.Name ?? Registrar.GetTypeFullName (Type!);
					return SanitizeObjectiveCName (name);
				}
			}

			public string ExportedName {
				get {
					return Registrar.GetExportedTypeName (Type!, RegisterAttribute);
				}
			}

			public bool IsFakeProtocol {
				get {
					if (RegisterAttribute is null || IsProtocol || IsModel || Type is null)
						return false;

					return Registrar.HasProtocolAttribute (Type);
				}
			}

			/// <summary>
			/// This is the first parent type which is not a model.
			/// </summary>
			public ObjCType? SuperType {
				get {
					if (superType is not null)
						return superType;

					var super = BaseType;
					while (super is not null && (super.IsModel || super.IsFakeProtocol))
						super = super.BaseType;
					superType = super;
					return superType;
				}
			}
		}

		abstract internal class ObjCMember {
			public readonly Registrar Registrar;
			public readonly ObjCType DeclaringType;
			public string? Name;
			public ObjCType? CategoryType;
			public ArgumentSemantic ArgumentSemantic = ArgumentSemantic.None;
			public bool IsVariadic;
			public bool IsOptional;

			public bool SetExportAttribute (ExportAttribute ea, [NotNullIfNotNull (nameof (exceptions))] ref List<Exception>? exceptions)
			{
#if NET
				if (string.IsNullOrEmpty (ea.Selector)) {
#else
				if (string.IsNullOrEmpty (ea.Selector) || ea.Selector is null) {
#endif
					AddException (ref exceptions, Registrar.CreateException (4135, this, Errors.MT4135, FullName));
					return false;
				}
				Selector = ea.Selector;
				ArgumentSemantic = ea.ArgumentSemantic;
				IsVariadic = ea.IsVariadic;
				return true;
			}

			// public ObjCMember (Registrar registrar)
			// {
			// 	Registrar = registrar;
			// }

			public ObjCMember (Registrar registrar, ObjCType declaringType)
			{
				Registrar = registrar;
				DeclaringType = declaringType;
			}

			string? selector;
			public string Selector {
				get { return selector!; }
				set {
					if (string.IsNullOrEmpty (value))
						throw Registrar.CreateException (4135, this, Errors.MT4135, FullName);
					selector = value;
				}
			}

			public abstract string FullName { get; }
			public abstract bool IsNativeStatic { get; }
			public virtual bool IsImplicit { get { return false; } }

			protected string ToSignature (TType type, ref bool success)
			{
				return Registrar.ToSignature (type, this, ref success);
			}
		}

		internal class ObjCMethod : ObjCMember {
			public readonly TMethod? Method;
			string? signature;
			Trampoline trampoline;
			bool? is_static;
			bool? is_ctor;
			TType []? parameters;
			TType []? native_parameters;
			TType? return_type;
			TType? native_return_type;

			public ObjCMethod (Registrar registrar, ObjCType declaringType, TMethod? method)
				: base (registrar, declaringType)
			{
				Method = method;
			}

			public string MethodName {
				get {
					return Name ?? Registrar.GetMethodName (Method!);
				}
			}

			public override bool IsImplicit {
				get {
					if (Method is not null)
						return false;

					switch (trampoline) {
					case Trampoline.Release:
					case Trampoline.Retain:
					case Trampoline.GetGCHandle:
					case Trampoline.SetGCHandle:
					case Trampoline.GetGCHandleFlags:
					case Trampoline.SetGCHandleFlags:
					case Trampoline.RetainWeakReference:
					case Trampoline.GetNSObjectData:
						return true;
					default:
						return false;
					}
				}
			}

			public bool IsConstructor {
				get {
					if (!is_ctor.HasValue)
						is_ctor = Registrar.IsConstructor (Method!);
					return is_ctor.Value;
				}
				set {
					is_ctor = value;
				}
			}

#if !LEGACY_TOOLS && !BUNDLER
			// The ArgumentSemantic enum is public, and
			// I don't want to add another enum value there which
			// is just an internal implementation detail, so just
			// use a constant instead. Eventually we'll use an internal
			// enum instead.
			const int RetainReturnValueFlag = 1 << 10;
			const int InstanceCategoryFlag = 1 << 11;

			internal bool IsInstanceCategory {
				get {
					return DynamicRegistrar.HasThisAttributeImpl (Method);
				}
			}

			internal void WriteUnmanagedDescription (IntPtr desc)
			{
				WriteUnmanagedDescription (desc, (System.Reflection.MethodBase) (object) Method!);
			}

			internal void WriteUnmanagedDescription (IntPtr desc, System.Reflection.MethodBase method_base)
			{
				var semantic = ArgumentSemantic;
				var minfo = method_base as System.Reflection.MethodInfo;
				var retainReturnValue = minfo is not null && minfo.GetBaseDefinition ().ReturnTypeCustomAttributes.IsDefined (typeof (ReleaseAttribute), false);
				var instanceCategory = minfo is not null && DynamicRegistrar.HasThisAttributeImpl (minfo);

				// bitfields and a default value of -1 don't go very well together.
				if (semantic == ArgumentSemantic.None)
					semantic = ArgumentSemantic.Assign;

				if (retainReturnValue)
					semantic |= (ArgumentSemantic) (RetainReturnValueFlag);
				if (instanceCategory)
					semantic |= (ArgumentSemantic) (InstanceCategoryFlag);

				var bindas_count = Marshal.ReadInt32 (desc + IntPtr.Size + 4);
				if (bindas_count < 1 + Parameters!.Length)
					throw ErrorHelper.CreateError (8018, $"Internal consistency error: BindAs array is not big enough (expected at least {1 + parameters!.Length} elements, got {bindas_count} elements) for {method_base.DeclaringType?.FullName + "." + method_base.Name}. Please file a bug report at https://github.com/dotnet/macios/issues/new.");

				Marshal.WriteIntPtr (desc, Runtime.AllocGCHandle (method_base));
				Marshal.WriteInt32 (desc + IntPtr.Size, (int) semantic);

				if (!IsConstructor && ReturnType != NativeReturnType)
					Marshal.WriteIntPtr (desc + IntPtr.Size + 8, Runtime.AllocGCHandle (NativeReturnType));
				for (int i = 0; i < NativeParameters!.Length; i++) {
					if (parameters! [i] == native_parameters! [i])
						continue;
					Marshal.WriteIntPtr (desc + IntPtr.Size + 8 + IntPtr.Size * (i + 1), Runtime.AllocGCHandle (native_parameters [i]));
				}
			}
#endif

			public bool HasParameters {
				get {
					return parameters is not null;
				}
			}

			public TType []? Parameters {
				get {
					if (parameters is null)
						parameters = Registrar.GetParameters (Method!);
					return parameters;
				}
				set {
					parameters = value;
					native_parameters = null;
				}
			}

			public TType []? NativeParameters {
				get {
					PrepareNativeParameters ();
					return native_parameters;
				}
			}

			void PrepareNativeParameters ()
			{
				if (native_parameters is not null || Parameters is null)
					return;

				// Put the parameters in a temporary variable, and only store them in the instance field once done,
				// so that if an exception occurs, the same exception will be raised the next time too.
				var computedNativeParameters = new TType [parameters!.Length];
				for (int i = 0; i < parameters.Length; i++) {
					var originalType = Registrar.GetBindAsAttribute (this, i)?.OriginalType;
					if (originalType is not null) {
						if (!IsValidToManagedTypeConversion (originalType, parameters [i]))
							throw Registrar.CreateException (4172, Method, Errors.MT4172, Registrar.GetTypeFullName (parameters [i]), originalType.FullName, Registrar.GetParameterName (Method, i), DescriptiveMethodName);
						computedNativeParameters [i] = originalType;
					} else {
						computedNativeParameters [i] = parameters [i];
					}
				}
				native_parameters = computedNativeParameters;
			}

			bool IsValidToManagedTypeConversion (TType inputType, TType outputType)
			{
				var nullableType = Registrar.GetNullableType (outputType);
				var isArray = Registrar.IsArray (outputType, out var arrayRank);

				TType underlyingOutputType = outputType;
				TType underlyingInputType = inputType;
				if (nullableType is not null) {
					underlyingOutputType = nullableType;
				} else if (isArray) {
					if (arrayRank != 1)
						return false;
					if (!Registrar.IsArray (inputType))
						return false;
					underlyingOutputType = Registrar.GetElementType (outputType)!;
					underlyingInputType = Registrar.GetElementType (inputType)!;
				}
				var outputTypeName = Registrar.GetTypeFullName (underlyingOutputType);

				if (Registrar.Is (underlyingInputType, Foundation, "NSNumber")) {
					switch (outputTypeName) {
					case "System.Byte":
					case "System.SByte":
					case "System.Int16":
					case "System.UInt16":
					case "System.Int32":
					case "System.UInt32":
					case "System.Int64":
					case "System.UInt64":
					case "System.IntPtr":
					case "System.UIntPtr":
					case "System.nint":
					case "System.nuint":
					case "System.Single":
					case "System.Double":
					case "System.Boolean":
						return true;
					default:
						if (outputTypeName == NFloatTypeName)
							return true;
						return Registrar.IsEnum (underlyingOutputType);
					}
				} else if (Registrar.Is (underlyingInputType, Foundation, "NSValue")) {
					switch (outputTypeName) {
					case "CoreAnimation.CATransform3D":
					case "CoreGraphics.CGAffineTransform":
					case "CoreGraphics.CGPoint":
					case "CoreGraphics.CGRect":
					case "CoreGraphics.CGSize":
					case "CoreGraphics.CGVector":
					case "CoreLocation.CLLocationCoordinate2D":
					case "CoreMedia.CMTime":
					case "CoreMedia.CMTimeMapping":
					case "CoreMedia.CMTimeRange":
					case "CoreMedia.CMVideoDimensions":
					case "MapKit.MKCoordinateSpan":
					case "Foundation.NSRange":
					case "SceneKit.SCNMatrix4":
					case "SceneKit.SCNVector3":
					case "SceneKit.SCNVector4":
					case "UIKit.UIEdgeInsets":
					case "UIKit.UIOffset":
					case "UIKit.NSDirectionalEdgeInsets":
						return true;
					default:
						return false;
					}
				} else if (Registrar.Is (underlyingInputType, Foundation, "NSString")) {
					return Registrar.IsSmartEnum (underlyingOutputType);
				} else {
					return false;
				}
			}

			public bool HasReturnType {
				get {
					return return_type is not null;
				}
			}

			public TType? ReturnType {
				get {
					if (return_type is null)
						return_type = Registrar.GetReturnType (Method!);
					return return_type;
				}
				set {
					return_type = value;
					native_return_type = null;
				}
			}

			public TType NativeReturnType {
				get {
					if (native_return_type is null) {
						if (Registrar.Is (ReturnType!, "System", "Void")) {
							native_return_type = ReturnType!;
						} else {
							var originalType = Registrar.GetBindAsAttribute (this, -1)?.OriginalType;
							if (originalType is not null) {
								if (!IsValidToManagedTypeConversion (originalType, ReturnType!))
									throw Registrar.CreateException (4170, Method, Errors.MT4170, Registrar.GetTypeFullName (ReturnType), originalType.FullName, DescriptiveMethodName);
								native_return_type = originalType;
							} else {
								native_return_type = ReturnType!;
							}
						}
					}
					return native_return_type;
				}
			}

			// If the managed method is static.
			public bool IsStatic {
				get { return is_static.HasValue ? is_static.Value : Registrar.IsStatic (Method!); }
				set { is_static = value; }
			}

			public override bool IsNativeStatic {
				get {
					return IsStatic && !IsCategoryInstance;
				}
			}

			public bool IsCategoryInstance {
				get {
					return IsCategory && Registrar.HasThisAttribute (Method!);
				}
			}

			public bool IsCategory {
				get { return CategoryType is not null; }
			}

			public bool RetainReturnValue {
				get {
					return Registrar.HasReleaseAttribute (Method!);
				}
			}

			public Trampoline CurrentTrampoline {
				get {
					return trampoline;
				}
			}

			public Trampoline Trampoline {
				get {
					if (trampoline != Trampoline.None)
						return trampoline;

#if LEGACY_TOOLS || BUNDLER
					throw ErrorHelper.CreateError (8018, Errors.MT8018);
#else
					var mi = (System.Reflection.MethodInfo?) Method;
					bool is_stret;
#if MONOMAC || __MACCATALYST__
					if (Runtime.IsARM64CallingConvention) {
						is_stret = false;
					} else {
						is_stret = Stret.X86_64NeedStret (NativeReturnType, null);
					}
#elif __IOS__
					if (Runtime.Arch == Arch.DEVICE) {
						is_stret = false;
					} else {
						is_stret = Stret.X86_64NeedStret (NativeReturnType, null);
					}
#elif __TVOS__
					is_stret = Runtime.Arch == Arch.SIMULATOR && Stret.X86_64NeedStret (NativeReturnType, null);
#else
#error unknown architecture
#endif
					var is_static_trampoline = IsStatic && !IsCategoryInstance;
					var is_value_type = Registrar.IsValueType (NativeReturnType) && !Registrar.IsEnum (NativeReturnType);

					if (is_value_type && Registrar.IsGenericType (NativeReturnType))
						throw Registrar.CreateException (4104, Method, "The registrar cannot marshal the return value of type `{0}` in the method `{1}.{2}`.", Registrar.GetTypeFullName (NativeReturnType), Registrar.GetTypeFullName (DeclaringType.Type), Registrar.GetDescriptiveMethodName (Method));

					if (is_stret) {
						trampoline = is_static_trampoline ? Trampoline.StaticStret : Trampoline.Stret;
					} else {
						switch (Signature [0]) {
						case 'Q':
						case 'q':
							trampoline = is_static_trampoline ? Trampoline.StaticLong : Trampoline.Long;
							break;
						case 'f':
							trampoline = is_static_trampoline ? Trampoline.StaticSingle : Trampoline.Single;
							break;
						case 'd':
							trampoline = is_static_trampoline ? Trampoline.StaticDouble : Trampoline.Double;
							break;
						default:
							trampoline = is_static_trampoline ? Trampoline.Static : Trampoline.Normal;
							break;
						}
					}

					return trampoline;
#endif
				}
				set {
					trampoline = value;
				}
			}

			public string Signature {
				get {
					if (signature is null)
						signature = ComputeSignature ();
					return signature;
				}
				set {
					signature = value;
				}
			}

			public bool ValidateSignature ([NotNullWhen (false)][NotNullIfNotNull (nameof (exceptions))] ref List<Exception>? exceptions)
			{
				if (Registrar.LaxMode)
					return true;

				if (signature is null) {
					try {
						PrepareNativeParameters ();
						signature = ComputeSignature ();
					} catch (ProductException mte) {
						AddException (ref exceptions, mte);
						return false;
					}
				}

				return true;
			}

			string ComputeSignature ()
			{
				return Registrar.ComputeSignature (DeclaringType.Type, null, this, IsCategoryInstance);
			}

			public override string ToString ()
			{
				return string.Format ("[ObjCMethod: Name={0}, IsStatic={1}, Trampoline={2}, Signature={3}]", Method is not null ? Registrar.GetMethodName (Method) : "null", IsStatic, Trampoline, ""); //Signature);
			}

			public string DescriptiveMethodName {
				get {
					return Registrar.GetTypeFullName (DeclaringType.Type) + "." + (Method is null ? Name : Registrar.GetMethodName (Method));
				}
			}

			public override string FullName {
				get {
					return DescriptiveMethodName;
				}
			}

			public bool IsPropertyAccessor {
				get {
					return Registrar.IsPropertyAccessor (Method!);
				}
			}
		}

		internal class ObjCProperty : ObjCMember {
			bool? is_static;
			public TProperty? Property;
			TType? property_type;
			public TType PropertyType {
				get {
					if (property_type is null)
						property_type = Property?.PropertyType;
					return property_type!;
				}
				set {
					property_type = value;
				}
			}
			bool? is_read_only;

			public string? GetterSelector;
			public string? SetterSelector;

			public bool IsReadOnly {
				get {
					if (!is_read_only.HasValue)
						is_read_only = Registrar.GetSetMethod (Property!) is null;
					return is_read_only.Value;
				}
				set {
					is_read_only = value;
				}
			}

			public bool IsStatic {
				get { return is_static.HasValue ? is_static.Value : Registrar.IsStatic (Property!); }
				set { is_static = value; }
			}

			public override bool IsNativeStatic {
				get { return IsStatic; }
			}

			public ObjCProperty (Registrar registrar, ObjCType declaringType) : base (registrar, declaringType)
			{
			}

			public override string FullName {
				get {
					return Registrar.GetTypeFullName (DeclaringType.Type) + "." + (Property is not null ? Registrar.GetPropertyName (Property) : Name);
				}
			}
		}

		internal class ObjCField : ObjCMember {
#if !LEGACY_TOOLS && !BUNDLER
			public int Size;
			public byte Alignment;
#else
			public bool IsPrivate;
			public TProperty? Property;
#endif
			public string FieldType;
			public bool IsProperty;
			bool is_static;

			public ObjCField (Registrar registrar, ObjCType declaringType, string fieldType) : base (registrar, declaringType)
			{
				FieldType = fieldType;
			}

			public override string FullName {
				get {
					return Registrar.GetTypeFullName (DeclaringType.Type) + "." + Name;
				}
			}

			public bool IsStatic {
				get { return is_static; }
				set { is_static = value; }
			}

			public override bool IsNativeStatic {
				get { return IsStatic; }
			}
		}

		protected virtual void OnRegisterType (ObjCType type) { }
		protected virtual void OnSkipType (TType type, ObjCType registered_type) { }
		protected virtual void OnReloadType (ObjCType type) { }
		protected virtual void OnRegisterProtocol (ObjCType type) { }
		protected virtual void OnRegisterCategory (ObjCType type, [NotNullIfNotNull (nameof (exceptions))] ref List<Exception>? exceptions) { }

		protected virtual bool SkipRegisterAssembly (TAssembly assembly) { return false; }

		protected abstract IEnumerable<TType> CollectTypes (TAssembly assembly);
		protected abstract IEnumerable<TMethod> CollectMethods (TType type); // Must return all methods defined on the type. Must not return methods from base classes.
		protected abstract IEnumerable<TProperty> CollectProperties (TType type); // Must return all properties defined on the type. Must not return properties from base classes.
		protected abstract IEnumerable<TMethod> CollectConstructors (TType type); // Must return all instance ctors. May return static ctors too (they're automatically filtered out).

		protected abstract bool ContainsPlatformReference (TAssembly assembly); // returns true if the assembly is monotouch.dll too.
		protected abstract TType? GetBaseType (TType? type); // for generic parameters it returns the first specific class constraint.
		protected abstract TType []? GetInterfaces (TType type); // may return interfaces from base classes as well. May return null if no interfaces found.
		protected virtual TType []? GetLinkedAwayInterfaces (TType type) { return null; } // may NOT return interfaces from base classes as well. May return null if no interfaces found.
		protected abstract TMethod GetBaseMethod (TMethod method);
		protected abstract TType []? GetParameters (TMethod method);
		protected abstract string GetParameterName (TMethod? method, int parameter_index);
		protected abstract TMethod? GetGetMethod (TProperty property);
		protected abstract TMethod? GetSetMethod (TProperty property);
		protected abstract TType GetSystemVoidType ();
		protected abstract bool IsVirtual (TMethod method);
		protected abstract bool IsByRef (TType type);
		protected abstract bool IsStatic (TProperty property);
		protected abstract bool IsStatic (TField field);
		protected abstract bool IsStatic (TMethod method);
		protected abstract TType MakeByRef (TType type);
		public abstract bool HasThisAttribute (TMethod method);
		protected abstract bool IsConstructor (TMethod method);
		public abstract TType? GetElementType (TType type);
		protected abstract TType GetReturnType (TMethod method);
		protected abstract void GetNamespaceAndName (TType type, out string? @namespace, out string name);
		protected abstract bool TryGetAttribute (TType type, string attributeNamespace, string attributeType, [NotNullWhen (true)] out object? attribute);
		protected abstract ExportAttribute? GetExportAttribute (TProperty property); // Return null if no attribute is found. Must check the base property (i.e. if property is overriding a property in a base class, must check the overridden property for the attribute).
		public abstract ExportAttribute? GetExportAttribute (TMethod method); // Return null if no attribute is found. Must check the base method (i.e. if method is overriding a method in a base class, must check the overridden method for the attribute).
		protected abstract Dictionary<TMethod, List<TMethod>>? PrepareMethodMapping (TType type);
		public abstract RegisterAttribute? GetRegisterAttribute (TType type); // Return null if no attribute is found. Do not consider base types.
		public abstract CategoryAttribute? GetCategoryAttribute (TType type); // Return null if no attribute is found. Do not consider base types.
		protected abstract ConnectAttribute? GetConnectAttribute (TProperty property); // Return null if no attribute is found. Do not consider inherited properties.
		public abstract ProtocolAttribute? GetProtocolAttribute (TType type); // Return null if no attribute is found. Do not consider base types.
		protected abstract IEnumerable<ProtocolMemberAttribute> GetProtocolMemberAttributes (TType type); // Return null if no attributes found. Do not consider base types.
		public virtual Version? GetSdkIntroducedVersion (TType obj, out string? message) { message = null; return null; } // returns the sdk version when the type was introduced for the current platform (null if all supported versions)
		protected abstract Version GetSDKVersion ();
		protected abstract TType? GetProtocolAttributeWrapperType (TType type); // Return null if no attribute is found. Do not consider base types.
		public abstract BindAsAttribute? GetBindAsAttribute (TMethod method, int parameter_index); // If parameter_index = -1 then get the attribute for the return type. Return null if no attribute is found. Must consider base method.
		public abstract BindAsAttribute? GetBindAsAttribute (TProperty? property);
		protected abstract IList<AdoptsAttribute>? GetAdoptsAttributes (TType type);
		public abstract TType? GetNullableType (TType type); // For T? returns T. For T returns null.
		public abstract bool HasReleaseAttribute (TMethod method); // Returns true of the method's return type/value has a [Release] attribute.
		protected abstract bool IsINativeObject (TType type);
		protected abstract bool IsValueType (TType type);
		public abstract bool IsArray (TType type, out int rank);
		protected abstract bool IsEnum (TType type, out bool isNativeEnum);
		public abstract bool IsNullable (TType type);
		protected abstract bool IsDelegate (TType type);
		protected abstract bool IsGenericType (TType type);
		protected abstract bool IsGenericMethod (TMethod method);
		protected abstract bool IsInterface (TType type);
		protected abstract bool IsAbstract (TType type);
		protected abstract bool IsPointer (TType type);
		protected abstract TType GetGenericTypeDefinition (TType type);
		protected abstract IEnumerable<TType> GetGenericArguments (TType type);
		public abstract bool VerifyIsConstrainedToNSObject (TType type, out TType? constrained_type);
		protected abstract TType? GetEnumUnderlyingType (TType type);
		protected abstract bool TryGetEnumUnderlyingType ([NotNullWhen (true)] TType? tr, [NotNullWhen (true)] out TType? underlyingType);
		protected abstract IEnumerable<TField> GetFields (TType type); // Must return all instance fields. May return static fields (they are filtered out automatically).
		protected abstract TType GetFieldType (TField field);
		protected abstract int GetValueTypeSize (TType type);
		protected abstract bool IsSimulatorOrDesktop { get; }
		protected abstract bool IsARM64 { get; }
		protected abstract Exception CreateExceptionImpl (int code, bool error, Exception? innerException, TMethod? method, string message, params object? [] args);
		protected abstract Exception CreateExceptionImpl (int code, bool error, Exception? innerException, TType? type, string message, params object? [] args);
		protected abstract string PlatformName { get; }
		public abstract TType? FindType (TType relative, string? @namespace, string name);
		protected abstract IEnumerable<TMethod>? FindMethods (TType type, string name); // will return null if nothing was found
		protected abstract TProperty? FindProperty (TType type, string name); // will return null if nothing was found

		protected abstract string GetAssemblyName (TAssembly assembly);
		[return: NotNullIfNotNull (nameof (type))]
		protected abstract string? GetTypeFullName (TType? type);
		protected abstract string GetAssemblyQualifiedName (TType type);
		protected abstract string GetTypeName (TType type);
		protected abstract string GetPropertyName (TProperty property);
		protected abstract TType GetPropertyType (TProperty property);
		protected abstract string GetMethodName (TMethod method);
		protected abstract string GetFieldName (TField field);

		// This is just to support the single-file/partial build registration
		// used for Xamarin.iOS.dll.
		protected virtual bool LaxMode { get { return false; } }

#if LEGACY_TOOLS || BUNDLER
		public Registrar (Application app)
		{
			App = app;
		}
#else
		public Registrar ()
		{
		}
#endif

		public static bool IsPropertyAccessor (TMethod method)
		{
			if (method is null)
				return false;

			if (!method.IsSpecialName)
				return false;

			var name = method.Name;
			if (!name.StartsWith ("get_", StringComparison.Ordinal) && !name.StartsWith ("set_", StringComparison.Ordinal))
				return false;

			return true;
		}

		public bool IsPropertyAccessor (TMethod method, [NotNullWhen (true)] out TProperty? property)
		{
			property = null;

			if (!IsPropertyAccessor (method))
				return false;

			property = FindProperty (method.DeclaringType!, method.Name.Substring (4));
			return property is not null;
		}

		public bool IsArray (TType type)
		{
			int rank;
			return IsArray (type, out rank);
		}

		protected bool IsEnum (TType type)
		{
			bool dummy;
			return IsEnum (type, out dummy);
		}

		public BindAsAttribute? GetBindAsAttribute (ObjCMethod method, int parameter_index)
		{
			var mthd = method.Method!;
			var attrib = GetBindAsAttribute (mthd, parameter_index);
			if (attrib is null && parameter_index >= 0 && !method.IsPropertyAccessor && !method.IsConstructor && !method.IsCategory && !IsInterface (method.DeclaringType.Type)) {
				// Parameter attributes aren't inherited from protocol methods, so look up the mapped interface method.
				var methodMap = PrepareMethodMapping (method.DeclaringType.Type);
				if (methodMap is not null && methodMap.TryGetValue (mthd, out var interfaceMethods)) {
					List<TMethod>? bindAsInterfaceMethods = null;
					foreach (var interfaceMethod in interfaceMethods) {
						var interfaceAttribute = GetBindAsAttribute (interfaceMethod, parameter_index);
						if (interfaceAttribute is null)
							continue;

						bindAsInterfaceMethods ??= new List<TMethod> ();
						bindAsInterfaceMethods.Add (interfaceMethod);
						attrib = interfaceAttribute;
					}

					if (bindAsInterfaceMethods is not null && interfaceMethods.Count != 1)
						throw new AggregateException (Shared.GetMT4127 (mthd, interfaceMethods));
				}
			}
			if (attrib is not null) {
				var type = parameter_index == -1 ? GetReturnType (mthd) : GetParameters (mthd)! [parameter_index];
				if (parameter_index == -1) {
					var returnType = GetReturnType (mthd);
					if (!AreEqual (returnType, attrib.Type))
						throw CreateException (4171, method.Method, Errors.MT4171, method.DescriptiveMethodName, GetTypeFullName (attrib.Type), GetTypeFullName (returnType));
				} else {
					var parameterType = GetParameters (mthd)! [parameter_index];
					if (IsByRef (parameterType))
						parameterType = GetElementType (parameterType);
					if (!AreEqual (parameterType, attrib.Type))
						throw CreateException (4171, method.Method, Errors.MT4171_A, parameter_index + 1, GetTypeFullName (attrib.Type), GetTypeFullName (parameterType));
				}

				return attrib;
			}

			if (!method.IsPropertyAccessor)
				return null;

			var property = FindProperty (method.DeclaringType.Type, method.MethodName.Substring (4));
			attrib = GetBindAsAttribute (property);
			if (attrib is not null && property is not null) {
				var propertyType = GetPropertyType (property);
				if (!AreEqual (propertyType, attrib.Type))
					throw CreateException (4171, property, Errors.MT4171_B, GetTypeFullName (method.DeclaringType.Type), GetPropertyName (property), GetTypeFullName (attrib.Type), GetTypeFullName (propertyType));
			}
			return attrib;
		}

		bool IsSmartEnum (TType type)
		{
			return IsSmartEnum (type, out var _, out var _);
		}

		public bool IsSmartEnum (TType type, [NotNullWhen (true)] out TMethod? getConstantMethod, [NotNullWhen (true)] out TMethod? getValueMethod)
		{
			getConstantMethod = null;
			getValueMethod = null;

			if (!IsEnum (type))
				return false;

			var extension = FindType (type, type.Namespace, type.Name + "Extensions");
			if (extension is null)
				return false;

			var getConstantMethods = FindMethods (extension, "GetConstant");
			if (getConstantMethods is null)
				return false;
			foreach (var m in getConstantMethods) {
				if (!Is (GetReturnType (m), Foundation, "NSString"))
					continue;
				var parameters = GetParameters (m);
				if (parameters?.Length != 1)
					continue;
				if (!AreEqual (parameters [0], type))
					continue;
				getConstantMethod = m;
				break;
			}
			if (getConstantMethod is null)
				return false;

			var getValueMethods = FindMethods (extension, "GetValue");
			if (getValueMethods is null)
				return false;
			foreach (var m in getValueMethods) {
				if (!AreEqual (GetReturnType (m), type))
					continue;
				var parameters = GetParameters (m);
				if (parameters?.Length != 1)
					continue;
				if (!Is (parameters [0], Foundation, "NSString"))
					continue;
				getValueMethod = m;
				break;
			}
			if (getValueMethod is null)
				return false;

			return true;
		}

		protected string GetMemberName (ObjCMember member)
		{
			var method = member as ObjCMethod;
			if (method is not null) {
				if (method.Method is not null)
					return GetMethodName (method.Method);
				return method.MethodName ?? "<implicit>";
			}
			var property = member as ObjCProperty;
			if (property is not null)
				return GetPropertyName (property.Property!);
			return ((ObjCField) member).Name!;
		}

		internal static string Foundation {
			get {
				return "Foundation";
			}
		}

		internal static string ObjCRuntime {
			get {
				return "ObjCRuntime";
			}
		}

		internal static string CoreAnimation {
			get {
				return "CoreAnimation";
			}
		}

#if MONOMAC || BUNDLER
		internal static string AppKit {
			get {
				return "AppKit";
			}
		}
#endif

#if LEGACY_TOOLS || BUNDLER
		internal string AssemblyName {
			get {
				switch (App.Platform) {
				case ApplePlatform.iOS:
					return "Microsoft.iOS";
				case ApplePlatform.TVOS:
					return "Microsoft.tvOS";
				case ApplePlatform.MacOSX:
					return "Microsoft.macOS";
				case ApplePlatform.MacCatalyst:
					return "Microsoft.MacCatalyst";
				default:
					throw ErrorHelper.CreateError (71, Errors.MX0071, App.Platform, App.ProductName);
				}
			}
		}
#elif MONOMAC
		internal const string AssemblyName = "Microsoft.macOS";
#elif TVOS
		internal const string AssemblyName = "Microsoft.tvOS";
#elif __MACCATALYST__
		internal const string AssemblyName = "Microsoft.MacCatalyst";
#elif IOS
		internal const string AssemblyName = "Microsoft.iOS";
#else
#error Unknown platform
#endif
		internal static class StringConstants {
			internal const string ExportAttribute = "ExportAttribute";
			internal const string ModelAttribute = "ModelAttribute";
			internal const string RegisterAttribute = "RegisterAttribute";
			internal const string ConnectAttribute = "ConnectAttribute";
			internal const string ProtocolAttribute = "ProtocolAttribute";
			internal const string ProtocolMemberAttribute = "ProtocolMemberAttribute";
			internal const string TransientAttribute = "TransientAttribute";
			internal const string ReleaseAttribute = "ReleaseAttribute";
			internal const string NativeAttribute = "NativeAttribute";
			internal const string CategoryAttribute = "CategoryAttribute";
			internal const string INativeObject = "INativeObject";
		}

		public string PlatformAssembly {
			get {
				return AssemblyName;
			}
		}

#if LEGACY_TOOLS || BUNDLER
		// "#if MTOUCH" code does not need locking when accessing 'types', because mtouch is single-threaded.
		public Dictionary<TType, ObjCType> Types {
			get { return types; }
		}
#endif

		protected Exception CreateException (int code, string message, params object? [] args)
		{
			return CreateExceptionImpl (code, true, message, args);
		}

		protected Exception CreateException (int code, TMethod? method, string message, params object? [] args)
		{
			return CreateExceptionImpl (code, true, method, message, args);
		}

		protected Exception CreateException (int code, TProperty? property, string message, params object? [] args)
		{
			return CreateExceptionImpl (code, true, property, message, args);
		}

		protected Exception CreateException (int code, TType? type, string message, params object? [] args)
		{
			return CreateExceptionImpl (code, true, type, message, args);
		}

		protected Exception CreateException (int code, Exception? innerException, TProperty? property, string message, params object? [] args)
		{
			return CreateExceptionImpl (code, true, innerException, property, message, args);
		}

		Exception CreateExceptionImpl (int code, bool error, string message, params object? [] args)
		{
			return CreateExceptionImpl (code, error, (TMethod?) null, message, args);
		}

		Exception CreateExceptionImpl (int code, bool error, TMethod? method, string message, params object? [] args)
		{
			return CreateExceptionImpl (code, error, null, method, message, args);
		}

		Exception CreateExceptionImpl (int code, bool error, TProperty? property, string message, params object? [] args)
		{
			return CreateExceptionImpl (code, error, null, property, message, args);
		}

		Exception CreateExceptionImpl (int code, bool error, TType? type, string message, params object? [] args)
		{
			return CreateExceptionImpl (code, error, null, type, message, args);
		}

		Exception CreateExceptionImpl (int code, bool error, Exception? innerException, TProperty? property, string message, params object? [] args)
		{
			if (property is null)
				return CreateExceptionImpl (code, error, innerException, (TMethod?) null, message, args);
			var getter = GetGetMethod (property);
			if (getter is not null)
				return CreateExceptionImpl (code, error, innerException, getter, message, args);
			return CreateExceptionImpl (code, error, innerException, GetSetMethod (property), message, args);
		}

		Exception CreateException (int code, ObjCMember? member, string message, params object? [] args)
		{
			return CreateExceptionImpl (code, true, member, message, args);
		}

		Exception CreateExceptionImpl (int code, bool error, ObjCMember? member, string message, params object? [] args)
		{
			var method = member as ObjCMethod;
			if (method is not null)
				return CreateExceptionImpl (code, error, method.Method, message, args);
			var property = member as ObjCProperty;
			if (property is not null)
				return CreateExceptionImpl (code, error, property.Property, message, args);
			return CreateExceptionImpl (code, error, message, args);
		}

		Exception CreateWarning (int code, ObjCMember? member, string message, params object? [] args)
		{
			return CreateExceptionImpl (code, false, member, message, args);
		}

		protected string GetDescriptiveMethodName (TMethod? method)
		{
			if (method is null)
				return string.Empty;

			var sb = new StringBuilder ();

			sb.Append (GetMethodName (method));
			sb.Append ("(");
			var pars = GetParameters (method);
			if (pars is not null && pars.Length > 0) {
				for (int i = 0; i < pars.Length; i++) {
					if (i > 0)
						sb.Append (",");
					sb.Append (GetTypeFullName (pars [i]));
				}
			}
			sb.Append (")");
			return sb.ToString ();
		}

		string GetDescriptiveMethodName (TType type, TMethod? method)
		{
			return GetTypeFullName (type) + "." + GetDescriptiveMethodName (method);
		}

		// overridable so that descendant classes can provide faster implementations
		protected virtual bool IsNSObject (TType type)
		{
			GetNamespaceAndName (type, out var @namespace, out var name);

			if (@namespace == Foundation && name == "NSObject")
				return true;

			var baseType = GetBaseType (type);
			if (baseType is not null)
				return IsNSObject (baseType);

			return false;
		}

		protected virtual bool AreEqual (TType? a, TType? b)
		{
			return a == b;
		}

		protected bool Is (TType type, string @namespace, string name)
		{
			GetNamespaceAndName (type, out var ns, out var n);
			return ns == @namespace && n == name;
		}

		// overridable so that descendant classes can provide faster implementation
		// do not check base types.
		protected virtual bool HasModelAttribute (TType type)
		{
			return TryGetAttribute (type, Foundation, StringConstants.ModelAttribute, out var _);
		}

		// overridable so that descendant classes can provide a faster implementation
		// do not check base types.
		public virtual bool HasProtocolAttribute (TType type)
		{
			return TryGetAttribute (type, Foundation, StringConstants.ProtocolAttribute, out var _);
		}

		// This method is thread-safe (locks 'types').
		public ObjCType? RegisterType (TType type)
		{
			ObjCType? rv;
			List<Exception>? exceptions = null;

			lock (types) {
				if (types.TryGetValue (type, out rv))
					return rv;

				rv = RegisterTypeUnsafe (type, ref exceptions);
			}

			if (exceptions is not null && exceptions.Count > 0)
				throw new AggregateException (exceptions);

			return rv;
		}

		// This method is thread-safe (locks 'types').
		public ObjCType? RegisterType (TType type, [NotNullIfNotNull (nameof (exceptions))] ref List<Exception>? exceptions)
		{
			lock (types)
				return RegisterTypeUnsafe (type, ref exceptions);
		}

		bool VerifyNonGenericMethod ([NotNullIfNotNull (nameof (exceptions))] ref List<Exception>? exceptions, TType declaringType, TMethod method)
		{
			if (!IsGenericMethod (method))
				return true;

			AddException (ref exceptions, CreateException (4113, method, Errors.MT4113,
				GetDescriptiveMethodName (declaringType, method)));
			return false;
		}

		void VerifyInSdk ([NotNullIfNotNull (nameof (exceptions))] ref List<Exception>? exceptions, ObjCMethod method)
		{
			if (method.HasReturnType || (method.Method is not null && !method.IsConstructor && method.NativeReturnType is not null))
				VerifyTypeInSDK (ref exceptions, method.NativeReturnType, returnTypeOf: method);

			if (method.HasParameters || (method.Method is not null && method.Parameters is not null)) {
				foreach (var p in method.Parameters!)
					VerifyTypeInSDK (ref exceptions, p, parameterIn: method);
			}
		}

		void VerifyInSdk ([NotNullIfNotNull (nameof (exceptions))] ref List<Exception>? exceptions, ObjCProperty property)
		{
			VerifyTypeInSDK (ref exceptions, property.PropertyType, propertyTypeOf: property);
		}

		void VerifyTypeInSDK ([NotNullIfNotNull (nameof (exceptions))] ref List<Exception>? exceptions, TType type, ObjCMethod? parameterIn = null, ObjCMethod? returnTypeOf = null, ObjCProperty? propertyTypeOf = null, TType? baseTypeOf = null)
		{
			var sdkVersion = GetSdkIntroducedVersion (type, out var message);
			if (sdkVersion is null)
				return;

			Version sdk = GetSDKVersion ();
			// Version treats unspecified build/revision components as -1, but SDK versions use zero.
			var introducedVersion = new Version (sdkVersion.Major, sdkVersion.Minor, Math.Max (sdkVersion.Build, 0), Math.Max (sdkVersion.Revision, 0));
			var currentVersion = new Version (sdk.Major, sdk.Minor, Math.Max (sdk.Build, 0), Math.Max (sdk.Revision, 0));
			if (introducedVersion <= currentVersion)
				return;

			string msg;
			string zero = GetTypeFullName (type);
			string one = string.Empty;
			string two = PlatformName;
			string three = sdk.ToString ();
			string four = sdkVersion.ToString ();
			string five = string.IsNullOrEmpty (message) ? "." : ": '" + message + "'.";
			if (baseTypeOf is not null) {
				msg = Errors.MT4162_BaseType;
				one = GetTypeFullName (baseTypeOf);
			} else if (parameterIn is not null) {
				msg = Errors.MT4162_Parameter;
				one = parameterIn.DescriptiveMethodName;
			} else if (returnTypeOf is not null) {
				msg = Errors.MT4162_ReturnType;
				one = returnTypeOf.DescriptiveMethodName;
			} else if (propertyTypeOf is not null) {
				msg = Errors.MT4162_PropertyType;
				one = propertyTypeOf.FullName;
			} else {
				msg = Errors.MT4162_A;
			}

			msg = string.Format (msg, zero, one, two, three, four, five);

			Exception ex;

			if (baseTypeOf is not null) {
				ex = CreateException (4162, baseTypeOf, msg);
			} else if (parameterIn is not null) {
				ex = CreateException (4162, parameterIn, msg);
			} else if (returnTypeOf is not null) {
				ex = CreateException (4162, returnTypeOf, msg);
			} else if (propertyTypeOf is not null) {
				ex = CreateException (4162, propertyTypeOf, msg);
			} else {
				ex = CreateException (4162, msg);
			}

			AddException (ref exceptions, ex);
		}

		protected static void AddException ([NotNull] ref List<Exception>? exceptions, Exception mex)
		{
			if (exceptions is null)
				exceptions = new List<Exception> ();
			exceptions.Add (mex);
		}

		bool IsSubClassOf (TType? type, string @namespace, string name)
		{
			while ((type = GetBaseType (type)) is not null) {
				GetNamespaceAndName (type, out var ns, out var n);
				if (ns == @namespace && n == name)
					return true;
			}

			return false;
		}

		bool VerifyIsConstrainedToNSObject ([NotNullIfNotNull (nameof (exceptions))] ref List<Exception>? exceptions, TType type, ObjCMethod method)
		{
			if (!VerifyIsConstrainedToNSObject (method.ReturnType!, out var constrained_type)) {
				AddException (ref exceptions, CreateException (4129, method.Method, Errors.MT4129, GetTypeFullName (method.ReturnType), GetDescriptiveMethodName (type, method.Method)));
				return false;
			}
			if (constrained_type is not null)
				method.ReturnType = constrained_type;

			var pars = method.Parameters;
			if (pars is null)
				return true;

			List<TType>? types = null;
			for (int i = 0; i < pars.Length; i++) {
				var p = pars [i];
				if (!VerifyIsConstrainedToNSObject (p, out constrained_type)) {
					AddException (ref exceptions, CreateException (4128, method.Method, Errors.MT4128, GetTypeFullName (p), GetDescriptiveMethodName (type, method.Method), GetParameterName (method.Method, i)));
					return false;
				}
				if (constrained_type is not null) {
					if (types is null) {
						types = new List<TType> ();
						for (int j = 0; j < i; j++)
							types.Add (pars [j]);
					}
					types.Add (constrained_type);
				} else if (types is not null) {
					types.Add (p);
				}
			}
			if (types is not null)
				method.Parameters = types.ToArray ();

			return true;
		}

		// Null out interfaces in the list already implemented in 
		// another interface in the list.
		// Example:
		//     interface A : B {} interface B {}
		// If the list contains {A,B}, this function will
		// null out the B entry to {A,null}, since A already
		// implements B.
		void FlattenInterfaces (TType? [] ifaces)
		{
			if (ifaces.Length == 1)
				return;

			for (int i = 0; i < ifaces.Length; i++) {
				var iface = ifaces [i];
				if (iface is null)
					continue;
				var ii = GetInterfaces (iface);
				if (ii is null)
					continue;
				for (int j = 0; j < ii.Length; j++) {
					for (int k = 0; k < ifaces.Length; k++) {
						if (k == i)
							continue;
						if (ifaces [k] is null)
							continue;
						if (ifaces [k] == ii [j])
							ifaces [k] = null;
					}
				}
			}
		}

		TType? []? GetInterfacesImpl (ObjCType objcType)
		{
			// Both the dynamic and static registrars (i.e both Cecil and SRE)
			// return interfaces from all base classes as well.
			// We only want the interfaces declared on this type.

			// Additionally it may return interface implementations that were linked away.

			// This function will return arrays with null entries.

			var type = objcType.Type;
			var allI = GetInterfaces (type);
			if (allI is null || allI.Length == 0)
				return allI;
			FlattenInterfaces (allI);

			var baseType = objcType.SuperType;
			if (baseType is null || baseType.Type is null)
				return allI;

			var baseI = GetInterfaces (baseType.Type);
			if (baseI is null || baseI.Length == 0)
				return allI;
			FlattenInterfaces (baseI);

			// Remove all interfaces the base type implements.
			var c = 0;
			for (int i = 0; i < allI.Length; i++) {
				if (Array.IndexOf (baseI, allI [i]) < 0) {
					allI [c++] = allI [i];
				}
			}
			Array.Resize (ref allI, c);
			return allI;
		}

		ObjCType []? GetProtocols (ObjCType type, [NotNullIfNotNull (nameof (exceptions))] ref List<Exception>? exceptions)
		{
			var interfaces = GetInterfacesImpl (type);
			List<ObjCType>? protocolList = null;
			if (interfaces?.Length > 0) {
				var ifaceList = new List<TType?> (interfaces);
				protocolList = new List<ObjCType> (interfaces.Length);
				for (int i = 0; i < ifaceList.Count; i++) {
					var iface = ifaceList [i];
					if (iface is null)
						continue;
					var baseP = RegisterTypeUnsafe (iface, ref exceptions);
					if (baseP is not null) {
						if (baseP.IsInformalProtocol) {
							var ifaces = GetInterfacesImpl (baseP);
							if (ifaces is not null)
								ifaceList.AddRange (ifaces);
						} else {
							protocolList.Add (baseP);
						}
					}
				}
			}

			// The interface might have been linked away.
			var linkedAwayInterfaces = new List<TType> ();
			var laifaces = GetLinkedAwayInterfaces (type.Type);
			var baseType = type.BaseType;

			if (laifaces is not null)
				linkedAwayInterfaces.AddRange (laifaces);

			while (baseType?.IsModel == true) {
				laifaces = GetLinkedAwayInterfaces (baseType.Type);
				if (laifaces is not null)
					linkedAwayInterfaces.AddRange (laifaces);
				baseType = baseType.BaseType;
			}
			if (linkedAwayInterfaces.Count > 0) {
				if (protocolList is null)
					protocolList = new List<ObjCType> ();
				for (int i = 0; i < linkedAwayInterfaces.Count; i++) {
					var iface = linkedAwayInterfaces [i];
					var proto = GetProtocolAttribute (iface);
					if (proto is null)
						continue;
					if (proto.IsInformal) {
						laifaces = GetLinkedAwayInterfaces (iface);
						if (laifaces is not null)
							linkedAwayInterfaces.AddRange (laifaces);
						continue;
					}
					// We can't register this interface (because it's been linked away),
					// but we still need to return information about it.
					// So create an ObjCType with just the required information,
					// and add that to what we return.
					var objcType = new ObjCType (this, iface) {
						IsProtocol = true,
					};
#if LEGACY_TOOLS || BUNDLER
					objcType.ProtocolWrapperType = GetProtocolAttributeWrapperType (objcType.Type);
					objcType.IsWrapper = objcType.ProtocolWrapperType is not null;
#endif

					protocolList.Add (objcType);
				}
			}
			if (protocolList is null || protocolList.Count == 0)
				return null;
			return protocolList.ToArray ();
		}

		string []? GetAdoptedProtocols (ObjCType type)
		{
			var attribs = GetAdoptsAttributes (type.Type);
			if (attribs is null || attribs.Count == 0)
				return null;
			var rv = new string [attribs.Count];
			for (var i = 0; i < attribs.Count; i++)
				rv [i] = attribs [i].ProtocolType;
			return rv;
		}

		ObjCType? RegisterCategory (TType type, CategoryAttribute attrib, [NotNullIfNotNull (nameof (exceptions))] ref List<Exception>? exceptions)
		{
			if (IsINativeObject (type)) {
				AddException (ref exceptions, ErrorHelper.CreateError (4152, Errors.MT4152, GetTypeFullName (type)));
				return null;
			}

			if (IsGenericType (type)) {
				AddException (ref exceptions, ErrorHelper.CreateError (4153, Errors.MT4153, GetTypeFullName (type)));
				return null;
			}

			if (attrib.Type is null) {
				AddException (ref exceptions, ErrorHelper.CreateError (4151, Errors.MT4151, GetTypeFullName (type)));
				return null;
			}

			var declaringType = RegisterType (attrib.Type, ref exceptions);
			if (declaringType is null) {
				AddException (ref exceptions, ErrorHelper.CreateError (4150, Errors.MT4150, GetTypeFullName (type), GetTypeFullName (attrib.Type)));
				return null;
			}

			var objcType = new ObjCType (this, type) {
				BaseType = declaringType,
				CategoryAttribute = attrib,
			};

			var categoryName = objcType.CategoryName!;
			lock (categories_map) {
				if (categories_map.TryGetValue (categoryName, out var previous_type)) {
					AddException (ref exceptions, ErrorHelper.CreateError (4156, Errors.MT4156,
						GetAssemblyQualifiedName (type), GetAssemblyQualifiedName (previous_type), categoryName));
					return null;
				}
				categories_map.Add (categoryName, type);
			}

			types.Add (type, objcType);

			Trace ("    [CATEGORY] Registering {0} on {1}", type.ToString ().Replace ('+', '/'), attrib.Type.FullName);

			foreach (var ctor in CollectConstructors (type)) {
				var ea = GetExportAttribute (ctor);
				if (ea is null)
					continue;

				AddException (ref exceptions, CreateException (4158, ctor, Errors.MT4158, GetTypeFullName (type), GetDescriptiveMethodName (ctor)));
			}

			foreach (var method in CollectMethods (type)) {
				var ea = GetExportAttribute (method);

				if (ea is null)
					continue;

				if (!IsStatic (method)) {
					AddException (ref exceptions, CreateException (4159, method, Errors.MT4159, GetTypeFullName (type), GetMethodName (method)));
					return null;
				}

				if (HasThisAttribute (method)) {
					var parameters = GetParameters (method);
					if (parameters is null || parameters.Length == 0) {
						AddException (ref exceptions, CreateException (4157, method, Errors.MT4157,
							GetTypeFullName (type), GetMethodName (method), GetTypeFullName (declaringType.Type)));
						continue;
					} else if (GetTypeFullName (parameters [0]) != GetTypeFullName (declaringType.Type)) {
						AddException (ref exceptions, CreateException (4149, method, Errors.MT4149,
							GetTypeFullName (type), GetMethodName (method), GetTypeFullName (parameters [0]), GetTypeFullName (declaringType.Type)));
						continue;
					}
				}

				if (IsGenericMethod (method)) {
					AddException (ref exceptions, CreateException (4154, method, Errors.MT4154, GetTypeFullName (type), GetMethodName (method)));
					continue;
				}

				Trace ("        [METHOD] {0} => {1}", method, ea.Selector);

				var category_method = new ObjCMethod (this, declaringType, method) {
					CategoryType = objcType,
				};
				if (category_method.SetExportAttribute (ea, ref exceptions)) {
					objcType.Add (category_method, ref exceptions);
					declaringType.Add (category_method, ref exceptions);
				}
			}

			// TODO: properties

			OnRegisterCategory (objcType, ref exceptions);

			return objcType;
		}

		// This method is not thread-safe wrt 'types', and must be called with
		// a lock held on 'types'.
		ObjCType? RegisterTypeUnsafe (TType type, [NotNullIfNotNull (nameof (exceptions))] ref List<Exception>? exceptions)
		{
			bool isGenericType = false;
			bool isProtocol = false;
			bool isInformalProtocol = false;

			if (IsGenericType (type)) {
				type = GetGenericTypeDefinition (type);
				isGenericType = true;
			}

			if (types.TryGetValue (type, out var objcType)) {
				OnReloadType (objcType);
				return objcType;
			}

			var categoryAttribute = GetCategoryAttribute (type);
			if (categoryAttribute is not null)
				return RegisterCategory (type, categoryAttribute, ref exceptions);

			if (!IsNSObject (type)) {
				//Trace ("{0} is not derived from NSObject", GetTypeFullName (type));
				if (!IsInterface (type))
					return null;

				if (!HasProtocolAttribute (type))
					return null;

				if (isGenericType) {
					AddException (ref exceptions, ErrorHelper.CreateError (4148, Errors.MT4148, GetTypeFullName (type)));
					return null;
				}

				// This is a protocol
				var pAttr = GetProtocolAttribute (type)!;
				isInformalProtocol = pAttr.IsInformal;
				isProtocol = true;

#if LEGACY_TOOLS || BUNDLER
				if (pAttr.FormalSinceVersion is not null && pAttr.FormalSinceVersion > App.SdkVersion)
					isInformalProtocol = !isInformalProtocol;
#endif
			}

			// make sure the base type is already registered
			var baseType = GetBaseType (type);
			ObjCType? baseObjCType = null;
			if (baseType is not null) {
				Trace ("Registering base type {0} of {1}", GetTypeName (baseType), GetTypeName (type));
				baseObjCType = RegisterTypeUnsafe (baseType, ref exceptions);
			}

			var register_attribute = GetRegisterAttribute (type);
			if (register_attribute is not null && register_attribute.SkipRegistration) {
				if (baseObjCType is not null)
					OnSkipType (type, baseObjCType);
				return baseObjCType;
			}

			objcType = new ObjCType (this, type) {
				RegisterAttribute = GetRegisterAttribute (type),
				IsModel = HasModelAttribute (type),
				IsProtocol = isProtocol,
				IsInformalProtocol = isInformalProtocol,
				IsGeneric = isGenericType,
			};
			objcType.VerifyRegisterAttribute (ref exceptions);
			objcType.AdoptedProtocols = GetAdoptedProtocols (objcType);
			objcType.VerifyAdoptedProtocolsNames (ref exceptions);
			objcType.BaseType = isProtocol ? null : (baseObjCType ?? objcType);
			objcType.Protocols = GetProtocols (objcType, ref exceptions);
#if LEGACY_TOOLS || BUNDLER
			objcType.ProtocolWrapperType = (isProtocol && !isInformalProtocol) ? GetProtocolAttributeWrapperType (objcType.Type) : null;
#endif
			objcType.IsWrapper = (isProtocol && !isInformalProtocol) ? (GetProtocolAttributeWrapperType (objcType.Type) is not null) : (objcType.RegisterAttribute is not null && objcType.RegisterAttribute.IsWrapper);

			if (!objcType.IsWrapper && objcType.BaseType is not null)
				VerifyTypeInSDK (ref exceptions, objcType.BaseType.Type, baseTypeOf: objcType.Type);

			if (ObjCType.IsObjectiveCKeyword (objcType.ExportedName))
				AddException (ref exceptions, ErrorHelper.CreateError (4168, Errors.MT4168, GetTypeFullName (type), objcType.ExportedName));

			// make sure all the protocols this type implements are registered
			if (objcType.Protocols is not null) {
				foreach (var p in objcType.Protocols) {
					Trace ("Registering implemented protocol {0} of {1}", p is null ? "null" : p.ProtocolName, GetTypeName (type));
					if (p is null)
						continue;
					OnRegisterProtocol (p);
				}
			}

			TType? previous_type;
			if (objcType.IsProtocol) {
				lock (protocol_map) {
					if (protocol_map.TryGetValue (objcType.ExportedName, out previous_type))
						throw ErrorHelper.CreateError (4126, Errors.MT4126,
							GetAssemblyQualifiedName (type), GetAssemblyQualifiedName (previous_type), objcType.ExportedName);
					protocol_map.Add (objcType.ExportedName, type);
				}
			} else {
				lock (type_map) {
					if (type_map.TryGetValue (objcType.ExportedName, out previous_type))
						throw ErrorHelper.CreateError (4118, Errors.MT4118,
							GetAssemblyQualifiedName (type), GetAssemblyQualifiedName (previous_type), objcType.ExportedName);
					type_map.Add (objcType.ExportedName, type);
				}
			}

			types.Add (type, objcType);

			Trace ("    [TYPE] Registering {0} => {1} IsWrapper: {2} BaseType: {3} IsModel: {4} IsProtocol: {5}", type.ToString ().Replace ('+', '/'), objcType.ExportedName, objcType.IsWrapper, objcType.BaseType is null ? "null" : objcType.BaseType.Name, objcType.IsModel, objcType.IsProtocol);

			// Special methods
			bool is_first_nonWrapper = false;
			var methods = new List<TMethod> (CollectMethods (type));
			if (!isProtocol) {
				is_first_nonWrapper = !(objcType.IsWrapper || objcType.IsModel) && (objcType.BaseType!.IsWrapper || objcType.BaseType.IsModel);
				if (is_first_nonWrapper) {
					bool isCalayerSubclass = IsSubClassOf (objcType.Type, CoreAnimation, "CALayer");
					if (!isCalayerSubclass) {
						objcType.Add (new ObjCMethod (this, objcType, null) {
							Selector = "release",
							Trampoline = Trampoline.Release,
							Signature = "v@:",
							IsStatic = false,
						}, ref exceptions);

						objcType.Add (new ObjCMethod (this, objcType, null) {
							Selector = "retain",
							Trampoline = Trampoline.Retain,
							Signature = "@@:",
							IsStatic = false,
						}, ref exceptions);
					}

					objcType.Add (new ObjCMethod (this, objcType, null) {
						Selector = "xamarinGetGCHandle",
						Trampoline = Trampoline.GetGCHandle,
						Signature = "i@:",
						IsStatic = false,
					}, ref exceptions);

					objcType.Add (new ObjCMethod (this, objcType, null) {
						Selector = "xamarinSetGCHandle:flags:data:",
						Trampoline = Trampoline.SetGCHandle,
						Signature = "v@:^vi^v",
						IsStatic = false,
					}, ref exceptions);

					objcType.Add (new ObjCMethod (this, objcType, null) {
						Selector = "xamarinGetGCHandleFlags",
						Trampoline = Trampoline.GetGCHandleFlags,
						Signature = "i@:",
						IsStatic = false,
					}, ref exceptions);

					objcType.Add (new ObjCMethod (this, objcType, null) {
						Selector = "xamarinSetGCHandleFlags:",
						Trampoline = Trampoline.SetGCHandleFlags,
						Signature = "v@:i",
						IsStatic = false,
					}, ref exceptions);

					objcType.Add (new ObjCMethod (this, objcType, null) {
						Selector = "retainWeakReference",
						Trampoline = Trampoline.RetainWeakReference,
						Signature = $"{GetBoolEncoding ()}@:",
						IsStatic = false,
					}, ref exceptions);

					objcType.Add (new ObjCMethod (this, objcType, null) {
						Selector = "xamarinGetNSObjectData",
						Trampoline = Trampoline.GetNSObjectData,
						Signature = "^{NSObjectData=@I}:",
						IsStatic = false,
					}, ref exceptions);
				}

				// Find conform_to_protocol
				if (conforms_to_protocol is null && Is (type, Foundation, "NSObject")) {
					foreach (var method in methods) {
						switch (GetMethodName (method)) {
						case "InvokeConformsToProtocol":
							invoke_conforms_to_protocol = method;
							break;
						case "ConformsToProtocol":
							conforms_to_protocol = method;
							break;
						}

						if (invoke_conforms_to_protocol is not null && conforms_to_protocol is not null)
							break;
					}
				}

#if LEGACY_TOOLS || BUNDLER
				// Special fields
				if (is_first_nonWrapper) {
					// static registrar
					objcType.Add (new ObjCField (this, objcType, "XamarinObject") {
						Name = "__monoObjectGCHandle",
						IsPrivate = true,
						IsStatic = false,
					}, ref exceptions);
				}
#endif
			}

			var properties = new List<TProperty> (CollectProperties (type));
			var hasProtocolMemberAttributes = false;
			if (isProtocol && !isInformalProtocol) {
				var attribs = GetProtocolMemberAttributes (type);
				foreach (var attrib in attribs) {
					hasProtocolMemberAttributes = true;
					if (attrib.IsProperty) {
						if (attrib.IsStatic) {
							// There is no such things as a static property in ObjC, so export this as just the getter[+setter].
							var objcGetter = new ObjCMethod (this, objcType, null) {
								Name = attrib.Name,
								Selector = attrib.GetterSelector!,
								Parameters = new TType [] { },
								ReturnType = attrib.PropertyType,
								IsStatic = attrib.IsStatic,
								IsOptional = !attrib.IsRequired,
								IsConstructor = false,
							};

							objcType.Add (objcGetter, ref exceptions);

#if NET
							if (!string.IsNullOrEmpty (attrib.SetterSelector)) {
#else
							if (!string.IsNullOrEmpty (attrib.SetterSelector) && attrib.SetterSelector is not null) {
#endif
								var objcSetter = new ObjCMethod (this, objcType, null) {
									Name = attrib.Name,
									Selector = attrib.SetterSelector,
									Parameters = new TType [] { attrib.PropertyType! },
									ReturnType = GetSystemVoidType (),
									IsStatic = attrib.IsStatic,
									IsOptional = !attrib.IsRequired,
									IsConstructor = false,
								};
								objcType.Add (objcSetter, ref exceptions);
							}
						} else {
							var objcProperty = new ObjCProperty (this, objcType) {
								Property = null,
								Name = attrib.Name,
								Selector = attrib.Selector!,
								ArgumentSemantic = attrib.ArgumentSemantic,
								IsReadOnly = string.IsNullOrEmpty (attrib.SetterSelector),
								IsStatic = attrib.IsStatic,
								IsOptional = !attrib.IsRequired,
								GetterSelector = attrib.GetterSelector,
								SetterSelector = attrib.SetterSelector,
								PropertyType = attrib.PropertyType!,
							};
							objcType.Add (objcProperty, ref exceptions);
						}
					} else {
						TMethod? method = null;
#if LEGACY_TOOLS || BUNDLER
						method = attrib.Method;
#endif
						var objcMethod = new ObjCMethod (this, objcType, method) {
							Name = attrib.Name,
							Selector = attrib.Selector!,
							ArgumentSemantic = attrib.ArgumentSemantic,
							IsVariadic = attrib.IsVariadic,
							ReturnType = attrib.ReturnType ?? GetSystemVoidType (),
							IsStatic = attrib.IsStatic,
							IsOptional = !attrib.IsRequired,
							IsConstructor = false,
						};

						if (attrib.ParameterType is not null) {
							var parameters = new TType [attrib.ParameterType.Length];
							for (int i = 0; i < parameters.Length; i++) {
								if (attrib.ParameterByRef! [i]) {
									parameters [i] = MakeByRef (attrib.ParameterType [i]);
								} else {
									parameters [i] = attrib.ParameterType [i];
								}
							}
							objcMethod.Parameters = parameters;
						} else {
							objcMethod.Parameters = new TType [] { };
						}

						objcType.Add (objcMethod, ref exceptions);
					}
				}
			}

			foreach (TProperty property in properties) {
				if (hasProtocolMemberAttributes)
					continue;

				if (!isProtocol) {
					var ca = GetConnectAttribute (property);
					if (ca is not null) {
						if (!IsINativeObject (GetPropertyType (property))) {
							AddException (ref exceptions, CreateException (4139, property, Errors.MT4139,
								GetTypeFullName (GetPropertyType (property)), GetTypeFullName (type), GetPropertyName (property)));
							continue;
						}

						objcType.Add (new ObjCField (this, objcType, "@") {
							Name = ca.Name ?? GetPropertyName (property),
#if !LEGACY_TOOLS && !BUNDLER
							Size = 8,
							Alignment = (byte) 3,
#endif
							IsProperty = true,
							IsStatic = IsStatic (property),
#if LEGACY_TOOLS || BUNDLER
							Property = property,
#endif
						}, ref exceptions);
					}
				}

				var ea = GetExportAttribute (property);

				if (ea is null)
					continue;

				if (IsStatic (property) && (objcType.IsWrapper || objcType.IsModel)) {
					// This is useless to export, since the user can't actually do anything with it,
					// it'll just call back into the base implementation.
					continue;
				}

				if (IsStatic (property) && isGenericType) {
					AddException (ref exceptions, CreateException (4131, property, Errors.MT4131, GetTypeFullName (type), GetPropertyName (property)));
					continue;
				}

				TType? property_type = null;
				if (isGenericType && !VerifyIsConstrainedToNSObject (GetPropertyType (property), out property_type)) {
					AddException (ref exceptions, CreateException (4132, property, Errors.MT4132, GetTypeFullName (GetPropertyType (property)), GetTypeFullName (type), GetPropertyName (property)));
					continue;
				}
				if (property_type is null)
					property_type = GetPropertyType (property);

				Trace ("        [PROPERTY] {0} => {1}", property, ea.Selector);

				var objcProperty = new ObjCProperty (this, objcType) {
					Property = property,
					Name = property.Name,
					Selector = ea.Selector ?? GetPropertyName (property),
					ArgumentSemantic = ea.ArgumentSemantic,
					PropertyType = property_type,
				};

				TMethod? getter = GetGetMethod (property);
				TMethod? setter = GetSetMethod (property);

				if (getter is not null && VerifyNonGenericMethod (ref exceptions, type, getter)) {
					var method = new ObjCMethod (this, objcType, getter) {
						Selector = ea.Selector ?? GetPropertyName (property),
						ArgumentSemantic = ea.ArgumentSemantic,
						ReturnType = property_type,
					};

					List<Exception>? excs = null;
					if (!method.ValidateSignature (ref excs)) {
						AddException (ref exceptions, CreateException (4138, excs [0], property, Errors.MT4138,
							GetTypeFullName (property.PropertyType), property.DeclaringType?.FullName, property.Name));
						continue;
					}

					if (!objcType.Add (method, ref exceptions))
						continue;

					Trace ("            [GET] {0}", objcType.Methods? [objcType.Methods.Count - 1]?.Name);
				}

				if (setter is not null && VerifyNonGenericMethod (ref exceptions, type, setter)) {
					string setterName = ea.Selector ?? GetPropertyName (property);

					var method = new ObjCMethod (this, objcType, setter) {
						Selector = CreateSetterSelector (setterName),
						ArgumentSemantic = ea.ArgumentSemantic,
						Parameters = new TType [] { property_type },
					};

					List<Exception>? excs = null;
					if (!method.ValidateSignature (ref excs)) {
						AddException (ref exceptions, CreateException (4138, excs [0], property, Errors.MT4138,
							GetTypeFullName (property.PropertyType), property.DeclaringType?.FullName, property.Name));
						continue;
					}

					if (!objcType.Add (method, ref exceptions))
						continue;

					Trace ("            [SET] {0}", objcType.Methods? [objcType.Methods.Count - 1]?.Name);
				}

				objcType.Add (objcProperty, ref exceptions);
			}

			var custom_conforms_to_protocol = !is_first_nonWrapper; // we only have to generate the conformsToProtocol method for the first non-wrapper type.

#if MONOMAC || BUNDLER
			ObjCMethod? custom_copy_with_zone = null;
			var isNSCellSubclass = false;
#if BUNDLER
			var isMac = App.Platform == ApplePlatform.MacOSX;
#elif MONOMAC
			const bool isMac = true;
#endif
			if (isMac)
				isNSCellSubclass = IsSubClassOf (type, AppKit, "NSCell");
#endif
			Dictionary<TMethod, List<TMethod>>? method_map = null;

			if (!isProtocol)
				method_map = PrepareMethodMapping (type);

			foreach (TMethod method in methods) {
				if (hasProtocolMemberAttributes)
					continue;

				var ea = GetExportAttribute (method);

				if (ea is null) {
					if (method_map is not null && method_map.TryGetValue (method, out var impls)) {
						if (impls.Count != 1) {
							foreach (var err in Shared.GetMT4127 (method, impls))
								AddException (ref exceptions, err);
							continue;
						}

						ea = GetExportAttribute (impls [0]);
					}
				}

				if (ea is null)
					continue;

				if (IsStatic (method) && (objcType.IsWrapper || objcType.IsModel) && !(objcType.IsProtocol && !objcType.IsFakeProtocol)) {
					// This is useless to export, since the user can't actually do anything with it,
					// it'll just call back into the base implementation.
					continue;
				}

				if (objcType.IsModel && IsVirtual (method))
					continue;

				Trace ("        [METHOD] {0} => {1}", method, ea.Selector);

				if (!custom_conforms_to_protocol && method.DeclaringType == type && GetBaseMethod (method) == conforms_to_protocol)
					custom_conforms_to_protocol = true;

				if (!VerifyNonGenericMethod (ref exceptions, type, method))
					continue;

				var objcMethod = new ObjCMethod (this, objcType, method);
				if (!objcMethod.SetExportAttribute (ea, ref exceptions))
					continue;

#if MONOMAC || BUNDLER
				if (objcMethod.Selector == "copyWithZone:")
					custom_copy_with_zone = objcMethod;
#endif

				if (IsStatic (method) && isGenericType) {
					AddException (ref exceptions, CreateException (4130, method, Errors.MT4130, GetDescriptiveMethodName (type, method)));
					continue;
				} else if (isGenericType && !VerifyIsConstrainedToNSObject (ref exceptions, type, objcMethod)) {
					continue;
				}

				try {
					objcType.Add (objcMethod, ref exceptions);
				} catch (Exception ex) {
					AddException (ref exceptions, ex);
				}
			}

			if (!isProtocol && !custom_conforms_to_protocol) {
				objcType.Add (new ObjCMethod (this, objcType, invoke_conforms_to_protocol) {
					Selector = "conformsToProtocol:",
					Trampoline = Trampoline.Normal,
					Signature = "B@:^v",
					IsStatic = false,
				}, ref exceptions);
			}

#if MONOMAC || BUNDLER
			if (isNSCellSubclass) {
				if (custom_copy_with_zone is not null) {
					custom_copy_with_zone.Trampoline = Trampoline.CopyWithZone2;
				} else {
					objcType.Add (new ObjCMethod (this, objcType, null) {
						Selector = "copyWithZone:",
						Trampoline = Trampoline.CopyWithZone1,
						Signature = "@@:^v",
						IsStatic = false,
					}, ref exceptions);
				}
			}
#endif

			foreach (TMethod ctor in CollectConstructors (type)) {
				if (IsStatic (ctor))
					continue;

				var parameters = GetParameters (ctor);
				if (parameters is null || parameters.Length == 0) {
					Trace ("        [CTOR] {0} default => init", GetTypeName (type));

					objcType.Add (new ObjCMethod (this, objcType, ctor) {
						Selector = "init",
						Trampoline = Trampoline.Constructor,
					}, ref exceptions);
					continue;
				}

				var ea = GetExportAttribute (ctor);

				if (ea is null)
					continue;

				Trace ("        [CTOR] {2} {0} => {1}", GetMethodName (ctor), ea.Selector, GetTypeName (type));

				if (!VerifyNonGenericMethod (ref exceptions, type, ctor))
					continue;

				var method = new ObjCMethod (this, objcType, ctor) {
					Trampoline = Trampoline.Constructor,
				};
				if (method.SetExportAttribute (ea, ref exceptions))
					objcType.Add (method, ref exceptions);
			}

			if (objcType.IsProtocol) {
				OnRegisterProtocol (objcType);
			} else {
				OnRegisterType (objcType);
			}

			return objcType;
		}

		public void RegisterAssembly (TAssembly assembly)
		{
			if (assembly is null)
				throw new ArgumentNullException ("assembly");

			if (assemblies.ContainsKey (assembly))
				return;

			if (SkipRegisterAssembly (assembly)) {
				Trace ("[ASSEMBLY] Skipped registration for {0}", GetAssemblyName (assembly));
				return;
			}

			if (!ContainsPlatformReference (assembly)) {
				assemblies.Add (assembly, null);
				return;
			}

			var exceptions = new List<Exception> ();

			try {
				var types = CollectTypes (assembly);

				Trace ("[ASSEMBLY] Registering {0}", assembly.ToString ());

				// Normally this method is only called at startup, when we're still
				// single-threaded. In this case it makes sense to lock as seldom as
				// possible, so instead of locking 'types' once for every type, we
				// lock it once for all of them.
				lock (this.types) {
					foreach (TType type in types) {
						//Trace (" Checking {0}", GetTypeFullName (type));
						RegisterTypeUnsafe (type, ref exceptions);
					}
				}

				assemblies.Add (assembly, null);
			} catch (Exception e) {
				ReportError (4116, Errors.MT4116, GetAssemblyName (assembly), e);
			}

			FlushTrace ();

			if (exceptions.Count > 0) {
				Exception ae = exceptions.Count == 1 ? exceptions [0] : new AggregateException (exceptions);
#if !LEGACY_TOOLS && !BUNDLER
				Runtime.NSLog (ae.ToString ());
#endif
				throw ae;
			}
		}

		public string ComputeSignature (TType? DeclaringType, TMethod? Method, ObjCMember? member = null, bool isCategoryInstance = false, bool isBlockSignature = false)
		{
			bool is_ctor;
			var method = member as ObjCMethod;
			TType? return_type = null;

			if (Method is not null) {
				is_ctor = IsConstructor (Method);
				if (!is_ctor)
					return_type = GetReturnType (Method);
			} else if (method is null) {
				throw ErrorHelper.CreateError (99, Errors.MX0099, "ComputeSignature requires either a Method or an ObjCMethod.");
			} else {
				is_ctor = method.IsConstructor;
				if (!is_ctor)
					return_type = method.NativeReturnType;
			}

			TType []? parameters;
			if (Method is not null) {
				parameters = GetParameters (Method);
			} else {
				parameters = method!.NativeParameters;
			}

			return ComputeSignature (DeclaringType, is_ctor, return_type, parameters, Method, member, isCategoryInstance, isBlockSignature);
		}

		public string ComputeSignature (TType? declaring_type, bool is_ctor, TType? return_type, TType []? parameters, TMethod? mi = null, ObjCMember? member = null, bool isCategoryInstance = false, bool isBlockSignature = false)
		{
			var success = true;
			var signature = new StringBuilder ();
			if (mi is null)
				mi = (member as ObjCMethod)?.Method;

			if (is_ctor) {
				signature.Append ('@');
			} else {
				signature.Append (ToSignature (return_type!, member, ref success));
				if (!success)
					throw CreateException (4104, mi, Errors.MT4104_A, GetTypeFullName (return_type), GetTypeFullName (declaring_type), GetDescriptiveMethodName (mi));
			}

			signature.Append (isBlockSignature ? "@?" : "@:");

			if (parameters is not null) {
				for (int i = 0; i < parameters.Length; i++) {
					if (i == 0 && isCategoryInstance)
						continue;
					var type = parameters [i];
					if (IsByRef (type)) {
						signature.Append ("^");
						var elementType = GetElementType (type)!;
						if (IsNullable (elementType)) {
							signature.Append (ToSignature (GetNullableType (elementType)!, member, ref success));
						} else {
							signature.Append (ToSignature (elementType, member, ref success));
						}
					} else {
						signature.Append (ToSignature (type, member, ref success));
					}
					if (!success) {
						if (mi is not null) {
							// The input 'parameters' might be closed generic types, and thus might not correspond exactly with the source code.
							// By fetching the parameters from the method again, we attempt to report the parameter type as close as possible
							// to the source code.
							parameters = GetParameters (mi)!;
						}
						throw CreateException (4136, mi, Errors.MT4136,
							GetTypeFullName (parameters [i]), GetParameterName (mi, i), GetTypeFullName (declaring_type), GetDescriptiveMethodName (mi));
					}
				}
			}
			return signature.ToString ();
		}

		protected string ToSignature (TType type, ObjCMember member, bool forProperty = false)
		{
			bool success = true;
			var rv = ToSignature (type, member, ref success, forProperty);
			if (success)
				return rv;

			var objcMethod = member as ObjCMethod;
			if (objcMethod is not null)
				throw ErrorHelper.CreateError (4111, Errors.MT4111, GetTypeFullName (type), GetTypeFullName (objcMethod.DeclaringType.Type) + "." + objcMethod.MethodName);

			throw ErrorHelper.CreateError (4101, Errors.MT4101, GetTypeFullName (type));
		}

		// Gets the Objective-C name for the given type.
		// Returns false if the type in question isn't exported to Objective-C, and thus doesn't have an Objective-C name.
		public bool TryGetExportedTypeName (TType type, [NotNullWhen (true)] out string? name)
		{
			name = null;

			var registerAttribute = GetRegisterAttribute (type);
			if (registerAttribute is null)
				return false;

			if (!registerAttribute.IsWrapper)
				return false;

			if (HasProtocolAttribute (type))
				return false;

			name = GetExportedTypeName (type, registerAttribute);
			return true;
		}

		public string GetExportedTypeName (TType type, RegisterAttribute? register_attribute)
		{
			string? name = null;
			if (register_attribute is not null) {
				if (register_attribute.SkipRegistration)
					return GetExportedTypeName (GetBaseType (type)!);
				name = register_attribute.Name;
			}
			if (name is null)
				name = GetTypeFullName (type);
			return SanitizeObjectiveCName (name);
		}

		protected string GetExportedTypeName (TType type)
		{
			return GetExportedTypeName (type, GetRegisterAttribute (type));
		}

		string GetBoolEncoding ()
		{
			// map managed 'bool' to ObjC BOOL = 'unsigned char' in OSX and 32bit iOS architectures and 'bool' in 64bit iOS architectures
#if LEGACY_TOOLS || BUNDLER
			switch (App.Platform) {
			case ApplePlatform.iOS:
			case ApplePlatform.TVOS:
				return "B";
			case ApplePlatform.MacOSX:
			case ApplePlatform.MacCatalyst:
				return IsARM64 ? "B" : "c";
			default:
				throw ErrorHelper.CreateError (71, Errors.MX0071, App.Platform, App.ProductName);
			}
#else
#if MONOMAC || __MACCATALYST__
			return IsARM64 ? "B" : "c";
#else
			return "B";
#endif
#endif
		}

		// 'member' is only used for diagnostic purposes.
		protected string ToSignature (TType type, ObjCMember? member, ref bool success, bool forProperty = false)
		{
			var typeFullName = GetTypeFullName (type);

			switch (typeFullName) {
			case "System.UIntPtr":
			case "System.IntPtr": return "^v";
			case "System.SByte": return "c";
			case "System.Byte": return "C";
			case "System.Char": return "s";
			case "System.Int16": return "s";
			case "System.UInt16": return "S";
			case "System.Int32": return "i";
			case "System.UInt32": return "I";
			case "System.Int64": return "q";
			case "System.UInt64": return "Q";
			case "System.Single": return "f";
			case "System.Double": return "d";
			case "System.Boolean": return GetBoolEncoding ();
			case "System.Void": return "v";
			case "System.String":
				return forProperty ? "@\"NSString\"" : "@";
			case "System.nint":
				return "q";
			case "System.nuint":
				return "Q";
			case "System.DateTime":
				throw CreateException (4102, member, Errors.MT4102, "System.DateTime", "Foundation.NSDate", member?.FullName);
			}

			if (typeFullName == NFloatTypeName)
				return "d";

			if (Is (type, ObjCRuntime, "Selector"))
				return ":";

			if (Is (type, ObjCRuntime, "Class"))
				return "#";

			if (IsINativeObject (type)) {
				if (IsGenericType (type))
					VerifyGenericArguments (type, member);

				if (!IsGenericType (type) && !IsInterface (type) && !IsNSObject (type) && IsAbstract (type)) {
					ReportWarning (4179, Errors.MT4179, type.FullName, member?.FullName);
				}
				if (IsNSObject (type) && forProperty) {
					return "@\"" + GetExportedTypeName (type) + "\"";
				} else {
					return "@";
				}
			}

			if (IsDelegate (type))
				return "^v";

			if (TryGetEnumUnderlyingType (type, out var underlyingType))
				return ToSignature (underlyingType, member, ref success);

			if (IsValueType (type))
				return ValueTypeSignature (type, member, ref success);

			if (IsArray (type)) {
				ToSignature (GetElementType (type)!, member, ref success); // this validates that the element type is a type we support.
				return "@"; // But we don't care about the actual type, we'll just return '@'. We only support NSArrays of the element type, so '@' is always right.
			}

			if (IsPointer (type))
				return "^" + ToSignature (GetElementType (type)!, member, ref success);

			success = false;
			return string.Empty;
		}

		void VerifyGenericArguments (TType type, ObjCMember? member)
		{
			foreach (var argument in GetGenericArguments (type)) {
				if (HasModelAttribute (argument))
					throw CreateException (4192, member, Errors.MT4192, GetTypeFullName (argument), GetTypeFullName (type));

				if (IsGenericType (argument))
					VerifyGenericArguments (argument, member);
			}
		}

		string ValueTypeSignature (TType type, ObjCMember member)
		{
			bool success = true;
			return ValueTypeSignature (type, member, ref success);
		}

		// 'member' is only used for diagnostic purposes.
		string ValueTypeSignature (TType type, ObjCMember? member, ref bool success)
		{
			var signature = new StringBuilder ();
			signature.Append ("{");
			signature.AppendFormat ("{0}=", GetTypeName (type));
			foreach (TField field in GetFields (type)) {
				if (IsStatic (field))
					continue;

				signature.Append (ToSignature (GetFieldType (field), member, ref success));
			}

			signature.Append ("}");
			return signature.ToString ();
		}

		protected void LockRegistrar (ref bool lockTaken)
		{
			System.Threading.Monitor.Enter (types, ref lockTaken);
		}

		protected void UnlockRegistrar ()
		{
			System.Threading.Monitor.Exit (types);
		}

#if LEGACY_TOOLS || BUNDLER
		internal static void NSLog (string format, params object [] args)
		{
			Console.WriteLine (format, args);
		}
#endif

		protected virtual void ReportError (int code, string message, params object? [] args)
		{
			// Using Console.WriteLine here is error prone, since if we get an early error
			// we'll end up crashing/infinite recursion since Console.WriteLine is redirected
			// to NSLog and is using NSString (and we haven't necessarily finished registering
			// everything yet).
			R.NSLog (String.Format (message, args));
		}

		protected virtual void ReportWarning (int code, string message, params object? [] args)
		{
			// Using Console.WriteLine here is error prone, since if we get an early error
			// we'll end up crashing/infinite recursion since Console.WriteLine is redirected
			// to NSLog and is using NSString (and we haven't necessarily finished registering
			// everything yet).
			R.NSLog (ErrorHelper.CreateWarning (code, message, args).ToString ());
		}

		static StringBuilder? trace;

		[Conditional ("VERBOSE_REGISTRAR")]
		public static void FlushTrace ()
		{
			if (trace is not null) {
				trace.Insert (0, '\n');
				R.NSLog (trace.ToString ());
#if DEV
				Console.WriteLine (trace.ToString ());
#endif
				trace.Clear ();
			}
		}

		[Conditional ("VERBOSE_REGISTRAR")]
		public static void Trace (string msg, params object? [] args)
		{
			if (trace is null)
				trace = new StringBuilder ();
			trace.AppendFormat (msg, args);
			trace.AppendLine ();
		}
	}

	enum Trampoline {
		None,
		Normal,
		Stret,
		Single,
		Double,
		Release,
		Retain,
		Static,
		StaticStret,
		StaticSingle,
		StaticDouble,
		Constructor,
		Long,
		StaticLong,
		CopyWithZone1,
		CopyWithZone2,
		GetGCHandle,
		SetGCHandle,
		GetGCHandleFlags,
		SetGCHandleFlags,
		RetainWeakReference,
		GetNSObjectData,
	}
}
