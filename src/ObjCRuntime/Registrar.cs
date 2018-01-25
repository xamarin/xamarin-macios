//
// Registrar.cs: Common code for the static and dynamic registrars.
// 
// Authors:
//   Rolf Bjarne Kvinge <rolf@xamarin.com>
//
// Copyright 2013 Xamarin Inc. 
//

// #define VERBOSE_REGISTRAR

#if IPHONE
#define MONOTOUCH
#endif

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

using XamCore.Foundation;
using XamCore.ObjCRuntime;

#if MTOUCH || MMP
using TAssembly=Mono.Cecil.AssemblyDefinition;
using TType=Mono.Cecil.TypeReference;
using TMethod=Mono.Cecil.MethodDefinition;
using TProperty=Mono.Cecil.PropertyDefinition;
using TField=Mono.Cecil.FieldDefinition;
using R=XamCore.Registrar.Registrar;
#else
using TAssembly=System.Reflection.Assembly;
using TType=System.Type;
using TMethod=System.Reflection.MethodBase;
using TProperty=System.Reflection.PropertyInfo;
using TField=System.Reflection.FieldInfo;
using R=XamCore.ObjCRuntime.Runtime;
#endif

#if MONOTOUCH
#if MTOUCH
using Xamarin.Bundler;
using ProductException=Xamarin.Bundler.MonoTouchException;
#else
#if XAMCORE_2_0
using ProductException=ObjCRuntime.RuntimeException;
#else
using ProductException=MonoTouch.RuntimeException;
#endif
#endif
#elif MONOMAC
#if MMP
using Xamarin.Bundler;
using ProductException=Xamarin.Bundler.MonoMacException;
#elif XAMCORE_2_0
using ProductException=ObjCRuntime.RuntimeException;
#else
using ProductException=MonoMac.RuntimeException;
#endif
#else
#error Only XI and XM
#endif

//
// This file cannot use any cecil code, since it's also compiled into monotouch.dll
//

#if MONOMAC
namespace XamCore.ObjCRuntime
{
	public delegate void AssemblyRegistrationHandler (object sender, AssemblyRegistrationEventArgs args);

	public class AssemblyRegistrationEventArgs : EventArgs
	{
		public bool Register { get; set; }
		public System.Reflection.AssemblyName AssemblyName { get; internal set; }
	}
}
#endif

namespace XamCore.Registrar {
	static class Shared {
		
		public static ProductException GetMT4127 (TMethod impl, List<TMethod> ifaceMethods)
		{
			var msg = new System.Text.StringBuilder ();
			msg.Append ("Cannot register more than one interface method for the method '");
			msg.Append (impl.DeclaringType.FullName).Append ('.').Append (impl.Name);
			msg.Append ("' (which is implementing '");
			for (int i = 0; i < ifaceMethods.Count; i++) {
				if (i > 0)
					msg.Append (i < ifaceMethods.Count - 1 ? "', '" : "' and '");
				var ifaceM = ifaceMethods [i];
				msg.Append (ifaceM.DeclaringType.FullName).Append ('.').Append (ifaceM.Name);
			}
			msg.Append ("').");
			return ErrorHelper.CreateError (4127, msg.ToString ());
		}
	}

	abstract partial class Registrar {
#if MTOUCH || MMP
		public Application App { get; protected set; }
#endif

		Dictionary<TAssembly, object> assemblies = new Dictionary<TAssembly, object> (); // Use Dictionary instead of HashSet to avoid pulling in System.Core.dll.
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
		TMethod conforms_to_protocol;
		TMethod invoke_conforms_to_protocol;

		public static bool IsDualBuild { get; private set; }

		public IEnumerable<TAssembly> GetAssemblies ()
		{
			return assemblies.Keys;
		}


		internal class ObjCType {
			public Registrar Registrar;
			public RegisterAttribute RegisterAttribute;
			public CategoryAttribute CategoryAttribute;
			public TType Type;
			public ObjCType BaseType;
			public ObjCType [] Protocols;
			public bool IsModel;
			// if this type represents an ObjC protocol (!= has the protocol attribute, since that can be applied to all kinds of things).
			public bool IsProtocol;
			public bool IsWrapper;
			public bool IsGeneric;
#if !MTOUCH && !MMP
			public IntPtr Handle;
#endif

			public Dictionary<string, ObjCField> Fields;
			public List<ObjCMethod> Methods;
			public List<ObjCProperty> Properties;

			Dictionary<string, ObjCMember> Map = new Dictionary<string, ObjCMember> ();

			ObjCType superType;

			public bool IsCategory { get { return CategoryAttribute != null; } }

			public void VerifyRegisterAttribute (ref List<Exception> exceptions)
			{
				if (RegisterAttribute == null)
					return;

				var name = RegisterAttribute.Name;
				if (string.IsNullOrEmpty (name))
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
					AddException (ref exceptions, new ProductException (4146, showError, "The Name parameter of the Registrar attribute on the class '{0}' ('{3}') contains an invalid character: '{1}' (0x{2}).", Registrar.GetTypeFullName (Type), name [i], ((int) name [i]).ToString ("x"), name));
					break;
				}
			}

			// This list is duplicated in tests/mtouch/RegistrarTest.cs.
			// Update that list whenever this list is updated.
			static char[] invalidSelectorCharacters = new char[] { ' ', '\t', '?', '\\', '!', '|', '@', '"', '\'', '%', '&', '/', '(', ')', '=', '^', '[', ']', '{', '}', ',', '.', ';', '-', '\n' };
			void VerifySelector (ObjCMethod method, ref List<Exception> exceptions)
			{
				if (method.Method == null)
					return;

				var split = method.Selector.Split (':');
				var nativeParamCount = split.Length - 1;

				if (method.IsVariadic)
					nativeParamCount++;

				var pars = method.Parameters;
				var paramCount = pars == null ? 0 : pars.Length;
				if (method.IsCategoryInstance)
					paramCount--;
				
				if (nativeParamCount != paramCount) {
					Exception ex;

					if (method.IsVariadic) {
						ex = Registrar.CreateException (4140, method, "The registrar found a signature mismatch in the method '{0}.{1}' - the selector '{4}' indicates the variadic method takes {2} parameters, while the managed method has {3} parameters.",
							method.Method.DeclaringType.FullName, method.MethodName, nativeParamCount, paramCount, method.Selector);
					} else {
						ex = Registrar.CreateException (4117, method, "The registrar found a signature mismatch in the method '{0}.{1}' - the selector '{4}' indicates the method takes {2} parameters, while the managed method has {3} parameters.",
							method.Method.DeclaringType.FullName, method.MethodName, nativeParamCount, paramCount, method.Selector);
					}

					Registrar.AddException (ref exceptions, ex);
				}

				if (method.IsVariadic && pars != null && Registrar.GetTypeFullName (pars [paramCount - 1]) != "System.IntPtr")
					Registrar.AddException (ref exceptions, Registrar.CreateException (4123, method, "The type of the variadic parameter in the variadic function '{0}' must be System.IntPtr.", Registrar.GetDescriptiveMethodName (method.Method)));
				
				char ch;
				var idx = method.Selector.IndexOfAny (invalidSelectorCharacters);
				if (idx != -1) {
					ch = method.Selector [idx];
					Registrar.AddException (ref exceptions, Registrar.CreateException (4160, method, "Invalid character '{0}' (0x{1}) found in selector '{2}' for '{3}.{4}'",
						ch, ((int) ch).ToString ("x"), method.Selector, Registrar.GetTypeFullName (Type), Registrar.GetDescriptiveMethodName (method.Method)));
				}
			}

			public void Add (ObjCField field, ref List<Exception> exceptions)
			{
				// Check if there are any base types with the same property.
				var base_type = BaseType;
				var fieldNameInDictionary = (field.IsStatic ? "+" : "-") + field.Name;
				while (base_type != null) {
					if (base_type.Fields != null && base_type.Fields.ContainsKey (fieldNameInDictionary)) {
						// Maybe we should warn here? Not sure if this is something bad or not.
						return;
					}
					if (base_type == base_type.BaseType)
						break;
					base_type = base_type.BaseType;
				}

				if (Fields == null)
					Fields = new Dictionary<string, ObjCField> ();
				// Do fields and methods live in the same objc namespace? // AddToMap (field, errorIfExists);
				Fields.Add (fieldNameInDictionary, field);
			}

			public bool Add (ObjCMethod method, ref List<Exception> exceptions)
			{
				bool rv;

				if (Methods == null)
					Methods = new List<ObjCMethod> ();

				VerifySelector (method, ref exceptions);
				method.ValidateSignature (ref exceptions);
				// Protocol members don't show up in the generated output, so it doesn't matter if we run into those.
				// If a class implements a protocol, it will still hit this check on the implemented members.
				if (!method.IsPropertyAccessor && !method.DeclaringType.IsProtocol)
					Registrar.VerifyInSdk (ref exceptions, method);

				rv = AddToMap (method, ref exceptions);
				Methods.Add (method);
				return rv;
			}

			public void Add (ObjCProperty property, ref List<Exception> exceptions)
			{
				if (Properties == null)
					Properties = new List<ObjCProperty> ();
				// Do properties and methods live in the same objc namespace? // AddToMap (property, errorIfExists);
				Properties.Add (property);
				// Protocol members don't show up in the generated output, so it doesn't matter if we run into those.
				// If a class implements a protocol, it will still hit this check on the implemented members.
				if (!property.DeclaringType.IsProtocol)
					Registrar.VerifyInSdk (ref exceptions, property);
				VerifyIsNotKeyword (ref exceptions, property);
			}

			public static bool IsObjectiveCKeyword (string name)
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

			void VerifyIsNotKeyword (ref List<Exception> exceptions, ObjCProperty property)
			{
				if (IsObjectiveCKeyword (property.Selector))
					AddException (ref exceptions, CreateException (4164, property, "Cannot export the property '{0}' because its selector '{1}' is an Objective-C keyword. Please use a different name.", property.Name, property.Selector));
			}

			public bool TryGetMember (string selector, bool is_static, out ObjCMember member)
			{
				if (is_static)
					selector = "+" + selector;
				else
					selector = "-" + selector;
				return Map.TryGetValue (selector, out member);
			}

			bool AddToMap (ObjCMember member, ref List<Exception> exceptions)
			{
				ObjCMember existing;
				bool rv = true;
				if (TryGetMember (member.Selector, member.IsNativeStatic, out existing)) {
					if (existing.IsImplicit) {
						AddException (ref exceptions, CreateException (4141, member, "Cannot register the selector '{0}' on the member '{1}.{2}' because Xamarin.iOS implicitly registers this selector.", member.Selector, Registrar.GetTypeFullName (Type), Registrar.GetMemberName (member)));
					} else {
						AddException (ref exceptions, CreateException (4119, member, "Could not register the selector '{0}' of the member '{1}.{2}' because the selector is already registered on the member '{3}'.", member.Selector, Registrar.GetTypeFullName (Type), Registrar.GetMemberName (member), Registrar.GetMemberName (existing)));
					}
					rv = false;
				}
				
				Map [(member.IsNativeStatic ? "+" : "-") + member.Selector] = member;
				return rv;
			}

			Exception CreateException (int code, ObjCMember member, string message, params object[] args)
			{
				var method = member as ObjCMethod;
				if (method != null)
					return Registrar.CreateException (code, method.Method, message, args);
				var property = member as ObjCProperty;
				if (property != null && property.Property != null)
					return Registrar.CreateException (code, property.Property.GetMethod ?? property.Property.SetMethod, message, args);
				return Registrar.CreateException (code, message, args);
			}

			public string Name {
				get { return RegisterAttribute != null && RegisterAttribute.Name != null ? RegisterAttribute.Name : Registrar.GetTypeFullName (Type); }
			}

			public string CategoryName {
				get {
					if (!IsCategory)
						throw new InvalidOperationException ();
					var attrib = CategoryAttribute;
					var name = attrib.Name ?? Registrar.GetTypeFullName (Type);
					return Registrar.SanitizeName (name);
				}
			}

			public string ProtocolName {
				get {
					if (!IsProtocol)
						throw new InvalidOperationException ();
					var attrib = Registrar.GetProtocolAttribute (Type);
					var name = attrib.Name ?? Registrar.GetTypeFullName (Type);
					return Registrar.SanitizeName (name);
				}
			}

			public string ExportedName {
				get {
					return Registrar.GetExportedTypeName (Type, RegisterAttribute);
				}
			}

			public bool IsFakeProtocol {
				get {
					if (RegisterAttribute == null || IsProtocol || IsModel)
						return false;

					return Registrar.HasProtocolAttribute (Type);
				}
			}

			/// <summary>
			/// This is the first parent type which is not a model.
			/// </summary>
			public ObjCType SuperType {
				get {
					if (superType != null)
						return superType;

					var super = BaseType;
					while (super != null && (super.IsModel || super.IsFakeProtocol))
						super = super.BaseType;
					superType = super;
					return superType;
				}
			}
		}

		abstract internal class ObjCMember {
			public Registrar Registrar;
			public ObjCType DeclaringType;
			public string Name;
			public ObjCType CategoryType;
			public ArgumentSemantic ArgumentSemantic = ArgumentSemantic.None;
			public bool IsVariadic;
			public bool IsOptional;

			public bool SetExportAttribute (ExportAttribute ea, ref List<Exception> exceptions)
			{
				if (string.IsNullOrEmpty (ea.Selector)) {
					AddException (ref exceptions, Registrar.CreateException (4135, this, "The member '{0}' has an Export attribute without a selector. A selector is required.", FullName));
					return false;
				}
				Selector = ea.Selector;
				ArgumentSemantic = ea.ArgumentSemantic;
				IsVariadic = ea.IsVariadic;
				return true;
			}

			public ObjCMember ()
			{
			}

			public ObjCMember (Registrar registrar, ObjCType declaringType)
			{
				Registrar = registrar;
				DeclaringType = declaringType;
			}

			string selector;
			public string Selector {
				get { return selector; }
				set {
					if (string.IsNullOrEmpty (value))
						throw Registrar.CreateException (4135, this, "The member '{0}' has an Export attribute without a selector. A selector is required.", FullName);
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
			public readonly TMethod Method;
			string signature;
			Trampoline trampoline;
			bool? is_static;
			bool? is_ctor;
			TType[] parameters;
			TType[] native_parameters;
			TType return_type;
			TType native_return_type;

			public ObjCMethod (Registrar registrar, ObjCType declaringType, TMethod method)
				: base (registrar, declaringType)
			{
				Method = method;
			}

			public string MethodName {
				get {
					return Name ?? Registrar.GetMethodName (Method);
				}
			}

			public override bool IsImplicit {
				get {
					if (Method != null)
						return false;

					switch (trampoline) {
					case Trampoline.Release:
					case Trampoline.Retain:
					case Trampoline.GetGCHandle:
					case Trampoline.SetGCHandle:
						return true;
					default:
						return false;
					}
				}
			}

			public bool IsConstructor {
				get {
					if (!is_ctor.HasValue)
						is_ctor = Registrar.IsConstructor (Method);
					return is_ctor.Value;
				}
				set {
					is_ctor = false;
				}
			}

#if !MMP && !MTOUCH
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
				WriteUnmanagedDescription (desc, (System.Reflection.MethodBase) (object) Method);
			}

			internal void WriteUnmanagedDescription (IntPtr desc, System.Reflection.MethodBase method_base)
			{
				var semantic = ArgumentSemantic;
				var minfo = method_base as System.Reflection.MethodInfo;
				var retainReturnValue = minfo != null && minfo.GetBaseDefinition ().ReturnTypeCustomAttributes.IsDefined (typeof (ReleaseAttribute), false);
				var instanceCategory = minfo != null && DynamicRegistrar.HasThisAttributeImpl (minfo);

				// bitfields and a default value of -1 don't go very well together.
				if (semantic == ArgumentSemantic.None)
					semantic = ArgumentSemantic.Assign;

				if (retainReturnValue)
					semantic |= (ArgumentSemantic) (RetainReturnValueFlag);
				if (instanceCategory)
					semantic |= (ArgumentSemantic) (InstanceCategoryFlag);

				var bindas_count = Marshal.ReadInt32 (desc + IntPtr.Size + 4);
				if (bindas_count < 1 + Parameters.Length)
					throw ErrorHelper.CreateError (8018, $"Internal consistency error: BindAs array is not big enough (expected at least {1 + parameters.Length} elements, got {bindas_count} elements) for {method_base.DeclaringType.FullName + "." + method_base.Name}. Please file a bug report at https://bugzilla.xamarin.com.");

				Marshal.WriteIntPtr (desc, ObjectWrapper.Convert (method_base));
				Marshal.WriteInt32 (desc + IntPtr.Size, (int) semantic);

				if (!IsConstructor && ReturnType != NativeReturnType)
					Marshal.WriteIntPtr (desc + IntPtr.Size + 8, ObjectWrapper.Convert (NativeReturnType));
				for (int i = 0; i < NativeParameters.Length; i++) {
					if (parameters [i] == native_parameters [i])
						continue;
					Marshal.WriteIntPtr (desc + IntPtr.Size + 8 + IntPtr.Size * (i + 1), ObjectWrapper.Convert (native_parameters [i]));
				}
			}
#endif

			public bool HasParameters {
				get {
					return parameters != null;
				}
			}

			public TType [] Parameters {
				get {
					if (parameters == null)
						parameters = Registrar.GetParameters (Method);
					return parameters;
				}
				set {
					parameters = value;
					native_parameters = null;
				}
			}

			public TType [] NativeParameters {
				get {
					if (native_parameters == null && Parameters != null) {
						// Put the parameters in a temporary variable, and only store them in the instance field once done,
						// so that if an exception occurs, the same exception will be raised the next time too.
						var native_parameters = new TType [parameters.Length];
						for (int i = 0; i < parameters.Length; i++) {
							var originalType = Registrar.GetBindAsAttribute (this, i)?.OriginalType;
							if (originalType != null) {
								if (!IsValidToManagedTypeConversion (originalType, parameters [i]))
									throw Registrar.CreateException (4172, Method, $"The registrar can't convert from '{Registrar.GetTypeFullName (parameters [i])}' to '{originalType.FullName}' for the parameter '{Registrar.GetParameterName (Method, i)}' in the method {DescriptiveMethodName}.");
								native_parameters [i] = originalType;
							} else {
								native_parameters [i] = parameters [i];
							}
						}
						this.native_parameters = native_parameters;
					}
					return native_parameters;
				}
			}

			bool IsValidToManagedTypeConversion (TType inputType, TType outputType)
			{
				var nullableType = Registrar.GetNullableType (outputType);
				var isNullable = nullableType != null;
				var arrayRank = 0;
				var isArray = Registrar.IsArray (outputType, out arrayRank);

				TType underlyingOutputType = outputType;
				TType underlyingInputType = inputType;
				if (isNullable) {
					underlyingOutputType = nullableType;
				} else if (isArray) {
					if (arrayRank != 1)
						return false;
					if (!Registrar.IsArray (inputType))
						return false;
					underlyingOutputType = Registrar.GetElementType (outputType);
					underlyingInputType = Registrar.GetElementType (inputType);
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
					case "System.nint":
					case "System.nuint":
					case "System.Single":
					case "System.Double":
					case "System.nfloat":
					case "System.Boolean":
						return true;
					default:
						return Registrar.IsEnum (underlyingOutputType);
					}
				} else if (Registrar.Is (underlyingInputType, Foundation, "NSValue")) {
#if MMP || MONOMAC
					// Remove 'MonoMac.' namespace prefix to make switch smaller
					if (!Registrar.IsDualBuild && outputTypeName.StartsWith ("MonoMac.", StringComparison.Ordinal))
						outputTypeName = outputTypeName.Substring ("MonoMac.".Length);
#endif

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

			bool IsValidToNativeTypeConversion (TType inputType, TType outputType)
			{
				return IsValidToManagedTypeConversion (inputType: outputType, outputType: inputType);
			}

			public bool HasReturnType {
				get {
					return return_type != null;
				}
			}

			public TType ReturnType {
				get {
					if (return_type == null)
						return_type = Registrar.GetReturnType (Method);
					return return_type;
				}
				set {
					return_type = value;
					native_return_type = null;
				}
			}

			public TType NativeReturnType {
				get {
					if (native_return_type == null) {
						if (Registrar.Is (ReturnType, "System", "Void")) {
							native_return_type = ReturnType;
						} else {
							var originalType = Registrar.GetBindAsAttribute (this, -1)?.OriginalType;
							if (originalType != null) {
								if (!IsValidToManagedTypeConversion (originalType, ReturnType))
									throw Registrar.CreateException (4170, Method, $"The registrar can't convert from '{Registrar.GetTypeFullName (ReturnType)}' to '{originalType.FullName}' for the return value in the method {DescriptiveMethodName}.");
								native_return_type = originalType;
							} else {
								native_return_type = ReturnType;
							}
						}
					}
					return native_return_type;
				}
			}

			// If the managed method is static.
			public bool IsStatic {
				get { return is_static.HasValue ? is_static.Value : Registrar.IsStatic (Method); }
				set { is_static = value; }
			}

			public override bool IsNativeStatic {
				get {
					return IsStatic && !IsCategoryInstance;
				}
			}

			public bool IsCategoryInstance {
				get {
					return IsCategory && Registrar.HasThisAttribute (Method);
				}
			}

			public bool IsCategory {
				get { return CategoryType != null; }
			}

			public bool RetainReturnValue {
				get {
					return Registrar.HasReleaseAttribute (Method);
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

#if MTOUCH || MMP
					throw ErrorHelper.CreateError (8018, "Internal consistency error. Please file a bug report at http://bugzilla.xamarin.com.");
#else
					var mi = (System.Reflection.MethodInfo) Method;
					bool is_stret;
#if __WATCHOS__
					is_stret = Runtime.Arch == Arch.DEVICE ? Stret.ArmNeedStret (NativeReturnType) : Stret.X86NeedStret (NativeReturnType);
#elif MONOMAC
					is_stret = IntPtr.Size == 8 ? Stret.X86_64NeedStret (NativeReturnType) : Stret.X86NeedStret (NativeReturnType);
#elif __IOS__
					if (Runtime.Arch == Arch.DEVICE) {
						is_stret = IntPtr.Size == 4 && Stret.ArmNeedStret (NativeReturnType);
					} else {
						is_stret = IntPtr.Size == 4 ? Stret.X86NeedStret (NativeReturnType) : Stret.X86_64NeedStret (NativeReturnType);
					}
#elif __TVOS__
					is_stret = Runtime.Arch == Arch.SIMULATOR && Stret.X86_64NeedStret (NativeReturnType);
#else
	#error unknown architecture
#endif
					var is_static_trampoline = IsStatic && !IsCategoryInstance;
					var is_value_type = Registrar.IsValueType (NativeReturnType) && !Registrar.IsEnum (NativeReturnType);

					if (is_value_type && Registrar.IsGenericType (NativeReturnType))
						throw Registrar.CreateException (4104, Method, "The registrar cannot marshal the return value of type `{0}` in the method `{1}.{2}`.", Registrar.GetTypeFullName (NativeReturnType), Registrar.GetTypeFullName (DeclaringType.Type), Registrar.GetDescriptiveMethodName (Method));
					
					if (is_stret) {
						if (Registrar.IsSimulatorOrDesktop && !Registrar.Is64Bits) {
							trampoline = is_static_trampoline ? Trampoline.X86_DoubleABI_StaticStretTrampoline : Trampoline.X86_DoubleABI_StretTrampoline;
						} else {
							trampoline = is_static_trampoline ? Trampoline.StaticStret : Trampoline.Stret;
						}
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
					if (signature == null)
						signature = ComputeSignature ();
					return signature;
				}
				set {
					signature = value;
				}
			}

			public bool ValidateSignature (ref List<Exception> exceptions)
			{
				if (Registrar.LaxMode)
					return true;

				if (signature == null) {
					try {
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
				return string.Format ("[ObjCMethod: Name={0}, IsStatic={1}, Trampoline={2}, Signature={3}]", Method != null ? Registrar.GetMethodName (Method) : "null", IsStatic, Trampoline, ""); //Signature);
			}

			public string DescriptiveMethodName {
				get {
					return Registrar.GetTypeFullName (DeclaringType.Type) + "." + (Method == null ? Name : Registrar.GetMethodName (Method));
				}
			}

			public override string FullName {
				get {
					return DescriptiveMethodName;
				}
			}

			public bool IsPropertyAccessor {
				get {
					if (Method == null)
						return false;
					
					return Method.IsSpecialName && (Method.Name.StartsWith ("get_", StringComparison.Ordinal) || Method.Name.StartsWith ("set_", StringComparison.Ordinal));
				}
			}
		}

		internal class ObjCProperty : ObjCMember {
			bool? is_static;
			public TProperty Property;
			TType property_type;
			public TType PropertyType {
				get {
					if (property_type == null)
						property_type = Property.PropertyType;
					return property_type;
				}
				set {
					property_type = value;
				}
			}
			bool? is_read_only;

			public string GetterSelector;
			public string SetterSelector;

			public bool IsReadOnly {
				get {
					if (!is_read_only.HasValue)
						is_read_only = Registrar.GetSetMethod (Property) == null;
					return is_read_only.Value;
				}
				set {
					is_read_only = value;
				}
			}

			public bool IsStatic {
				get { return is_static.HasValue ? is_static.Value : Registrar.IsStatic (Property); }
				set { is_static = value; }
			}

			public override bool IsNativeStatic {
				get { return IsStatic; }
			}

			public ObjCProperty () : base ()
			{
			}

			public override string FullName {
				get {
					return Registrar.GetTypeFullName (DeclaringType.Type) + "." + (Property != null ? Registrar.GetPropertyName (Property) : Name);
				}
			}
		}

		internal class ObjCField : ObjCMember {
#if !MTOUCH && !MMP
			public int Size;
			public byte Alignment;
#else
			public bool IsPrivate;
#endif
			public string FieldType;
			public bool IsProperty;
			bool is_static;

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

		protected virtual void OnRegisterType (ObjCType type) {}
		protected virtual void OnReloadType (ObjCType type) {}
		protected virtual void OnRegisterProtocol (ObjCType type) {}
		protected virtual void OnRegisterCategory (ObjCType type, ref List<Exception> exceptions) {}

		protected virtual bool SkipRegisterAssembly (TAssembly assembly) { return false; }

		protected abstract IEnumerable<TType> CollectTypes (TAssembly assembly);
		protected abstract IEnumerable<TMethod> CollectMethods (TType type); // Must return all methods defined on the type. Must not return methods from base classes.
		protected abstract IEnumerable<TProperty> CollectProperties (TType type); // Must return all properties defined on the type. Must not return properties from base classes.
		protected abstract IEnumerable<TMethod> CollectConstructors (TType type); // Must return all instance ctors. May return static ctors too (they're automatically filtered out).

		protected abstract bool ContainsPlatformReference (TAssembly assembly); // returns true if the assembly is monotouch.dll too.
		protected abstract TType GetBaseType (TType type); // for generic parameters it returns the first specific class constraint.
		protected abstract TType[] GetInterfaces (TType type); // may return interfaces from base classes as well. May return null if no interfaces found.
		protected abstract TMethod GetBaseMethod (TMethod method);
		protected abstract TType[] GetParameters (TMethod method);
		protected abstract string GetParameterName (TMethod method, int parameter_index);
		protected abstract TMethod GetGetMethod (TProperty property);
		protected abstract TMethod GetSetMethod (TProperty property);
		protected abstract TType GetSystemVoidType ();
		protected abstract bool IsVirtual (TMethod method);
		protected abstract bool IsByRef (TType type);
		protected abstract bool IsStatic (TProperty property);
		protected abstract bool IsStatic (TField field);
		protected abstract bool IsStatic (TMethod method);
		protected abstract TType MakeByRef (TType type);
		protected abstract bool HasThisAttribute (TMethod method);
		protected abstract bool IsConstructor (TMethod method);
		protected abstract TType GetElementType (TType type);
		protected abstract TType GetReturnType (TMethod method);
		protected abstract void GetNamespaceAndName (TType type, out string @namespace, out string name);
		protected abstract bool TryGetAttribute (TType type, string attributeNamespace, string attributeType, out object attribute);
		protected abstract ExportAttribute GetExportAttribute (TProperty property); // Return null if no attribute is found. Must check the base property (i.e. if property is overriding a property in a base class, must check the overridden property for the attribute).
		protected abstract ExportAttribute GetExportAttribute (TMethod method); // Return null if no attribute is found. Must check the base method (i.e. if method is overriding a method in a base class, must check the overridden method for the attribute).
		protected abstract Dictionary<TMethod, List<TMethod>> PrepareMethodMapping (TType type);
		public abstract RegisterAttribute GetRegisterAttribute (TType type); // Return null if no attribute is found. Do not consider base types.
		protected abstract CategoryAttribute GetCategoryAttribute (TType type); // Return null if no attribute is found. Do not consider base types.
		protected abstract ConnectAttribute GetConnectAttribute (TProperty property); // Return null if no attribute is found. Do not consider inherited properties.
		protected abstract ProtocolAttribute GetProtocolAttribute (TType type); // Return null if no attribute is found. Do not consider base types.
		protected abstract IEnumerable<ProtocolMemberAttribute> GetProtocolMemberAttributes (TType type); // Return null if no attributes found. Do not consider base types.
		protected abstract List<AvailabilityBaseAttribute> GetAvailabilityAttributes (TType obj); // must only return attributes for the current platform.
		protected abstract Version GetSDKVersion ();
		protected abstract TType GetProtocolAttributeWrapperType (TType type); // Return null if no attribute is found. Do not consider base types.
		protected abstract BindAsAttribute GetBindAsAttribute (TMethod method, int parameter_index); // If parameter_index = -1 then get the attribute for the return type. Return null if no attribute is found. Must consider base method.
		protected abstract BindAsAttribute GetBindAsAttribute (TProperty property);
		public abstract TType GetNullableType (TType type); // For T? returns T. For T returns null.
		protected abstract bool HasReleaseAttribute (TMethod method); // Returns true of the method's return type/value has a [Release] attribute.
		protected abstract bool IsINativeObject (TType type);
		protected abstract bool IsValueType (TType type);
		protected abstract bool IsArray (TType type, out int rank);
		protected abstract bool IsEnum (TType type, out bool isNativeEnum);
		protected abstract bool IsNullable (TType type);
		protected abstract bool IsDelegate (TType type);
		protected abstract bool IsGenericType (TType type);
		protected abstract bool IsGenericMethod (TMethod method);
		protected abstract bool IsInterface (TType type);
		protected abstract TType GetGenericTypeDefinition (TType type);
		protected abstract bool VerifyIsConstrainedToNSObject (TType type, out TType constrained_type);
		protected abstract TType GetEnumUnderlyingType (TType type);
		protected abstract IEnumerable<TField> GetFields (TType type); // Must return all instance fields. May return static fields (they are filtered out automatically).
		protected abstract TType GetFieldType (TField field);
		protected abstract int GetValueTypeSize (TType type);
		protected abstract bool IsCorlibType (TType type);
		protected abstract bool IsSimulatorOrDesktop { get; }
		protected abstract bool Is64Bits { get; }
		protected abstract bool IsDualBuildImpl { get; }
		protected abstract Exception CreateException (int code, Exception innerException, TMethod method, string message, params object[] args);
		protected abstract Exception CreateException (int code, Exception innerException, TType type, string message, params object [] args);
		protected abstract string PlatformName { get; }
		public abstract TType FindType (TType relative, string @namespace, string name);
		protected abstract IEnumerable<TMethod> FindMethods (TType type, string name); // will return null if nothing was found
		protected abstract TProperty FindProperty (TType type, string name); // will return null if nothing was found

		protected abstract string GetAssemblyName (TAssembly assembly);
		protected abstract string GetTypeFullName (TType type);
		protected abstract string GetAssemblyQualifiedName (TType type);
		protected abstract string GetTypeName (TType type);
		protected abstract string GetPropertyName (TProperty property);
		protected abstract TType GetPropertyType (TProperty property);
		protected abstract string GetMethodName (TMethod method);
		protected abstract string GetFieldName (TField field);

		// This is just to support the single-file/partial build registration
		// used for Xamarin.iOS.dll.
		protected virtual bool LaxMode { get { return false; } }

		public Registrar ()
		{
			IsDualBuild = IsDualBuildImpl;
		}

		protected bool IsArray (TType type)
		{
			int rank;
			return IsArray (type, out rank);
		}

		protected bool IsEnum (TType type)
		{
			bool dummy;
			return IsEnum (type, out dummy);
		}

		public BindAsAttribute GetBindAsAttribute (ObjCMethod method, int parameter_index)
		{
			var attrib = GetBindAsAttribute (method.Method, parameter_index);
			if (attrib != null) {
				var type = parameter_index == -1 ? GetReturnType (method.Method) : GetParameters (method.Method) [parameter_index];
				if (parameter_index == -1) {
					var returnType = GetReturnType (method.Method);
					if (!AreEqual (returnType, attrib.Type))
						throw CreateException (4171, method.Method, $"The BindAs attribute on the return value of the method {method.DescriptiveMethodName} is invalid: the BindAs type {GetTypeFullName (attrib.Type)} is different from the return type {GetTypeFullName (returnType)}.");
				} else {
					var parameterType = GetParameters (method.Method) [parameter_index];
					if (IsByRef (parameterType))
						parameterType = GetElementType (parameterType);
					if (!AreEqual (parameterType, attrib.Type))
						throw CreateException (4171, method.Method, $"The BindAs attribute on the parameter #{parameter_index + 1} is invalid: the BindAs type {GetTypeFullName (attrib.Type)} is different from the parameter type {GetTypeFullName (parameterType)}.");
				}

				return attrib;
			}

			if (!method.IsPropertyAccessor)
				return null;

			var property = FindProperty (method.DeclaringType.Type, method.MethodName.Substring (4));
			attrib = GetBindAsAttribute (property);
			if (attrib != null) {
				var propertyType = GetPropertyType (property);
				if (!AreEqual (propertyType, attrib.Type))
					throw CreateException (4171, property, $"The BindAs attribute on the property {GetTypeFullName (method.DeclaringType.Type)}.{GetPropertyName (property)} is invalid: the BindAs type {GetTypeFullName (attrib.Type)} is different from the property type {GetTypeFullName (propertyType)}.");
			}
			return attrib;
		}

		bool IsSmartEnum (TType type)
		{
			TMethod getConstant, getValue;
			return IsSmartEnum (type, out getConstant, out getValue);
		}

		public bool IsSmartEnum (TType type, out TMethod getConstantMethod, out TMethod getValueMethod)
		{
			getConstantMethod = null;
			getValueMethod = null;

			if (!IsEnum (type))
				return false;

			var extension = FindType (type, type.Namespace, type.Name + "Extensions");
			if (extension == null)
				return false;

			var getConstantMethods = FindMethods (extension, "GetConstant");
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
			if (getConstantMethod == null)
				return false;

			var getValueMethods = FindMethods (extension, "GetValue");
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
			if (getValueMethod == null)
				return false;

			return true;
		}

		protected string GetMemberName (ObjCMember member)
		{
			var method = member as ObjCMethod;
			if (method != null) {
				if (method.Method != null)
					return GetMethodName (method.Method);
				return method.MethodName ?? "<implicit>";
			}
			var property = member as ObjCProperty;
			if (property != null)
				return GetPropertyName (property.Property);
			return ((ObjCField)member).Name;
		}

		internal static string Foundation {
			get {
				return IsDualBuild ? "Foundation" : CompatNamespace + ".Foundation";
			}
		}

		internal static string ObjCRuntime {
			get {
				return IsDualBuild ? "ObjCRuntime" : CompatNamespace + ".ObjCRuntime";
			}
		}

		internal static string CoreAnimation {
			get {
				return IsDualBuild ? "CoreAnimation" : CompatNamespace + ".CoreAnimation";
			}
		}

#if MONOMAC
		internal static string AppKit {
			get {
				return IsDualBuild ? "AppKit" : CompatNamespace + ".AppKit";
			}
		}
#endif

#if MONOMAC
		internal const string CompatNamespace = "MonoMac";
		internal const string CompatAssemblyName = "XamMac";
		internal const string DualAssemblyName = "Xamarin.Mac";
#else
		internal const string CompatNamespace = "MonoTouch";
		internal const string CompatAssemblyName = "monotouch";
#if MTOUCH
		internal string DualAssemblyName
		{
			get {
				switch (App.Platform) {
				case Xamarin.Utils.ApplePlatform.iOS:
					return "Xamarin.iOS";
				case Xamarin.Utils.ApplePlatform.WatchOS:
					return "Xamarin.WatchOS";
				case Xamarin.Utils.ApplePlatform.TVOS:
					return "Xamarin.TVOS";
				default:
					throw ErrorHelper.CreateError (71, "Unknown platform: {0}. This usually indicates a bug in Xamarin.iOS; please file a bug report at http://bugzilla.xamarin.com with a test case.", App.Platform);
				}
			}
		}
#elif WATCH
		internal const string DualAssemblyName = "Xamarin.WatchOS";
#elif TVOS
		internal const string DualAssemblyName = "Xamarin.TVOS";
#elif IOS
		internal const string DualAssemblyName = "Xamarin.iOS";
#else
#error Unknown platform
#endif
#endif
		internal static class StringConstants {
				internal const string ExportAttribute         =	"ExportAttribute";
				internal const string ModelAttribute          =	"ModelAttribute";
				internal const string RegisterAttribute       =	"RegisterAttribute";
				internal const string ConnectAttribute        =	"ConnectAttribute";
				internal const string ProtocolAttribute       =	"ProtocolAttribute";
				internal const string ProtocolMemberAttribute = "ProtocolMemberAttribute";
				internal const string TransientAttribute      =   "TransientAttribute";
				internal const string ReleaseAttribute        =	"ReleaseAttribute";
				internal const string NativeAttribute         =   "NativeAttribute";
				internal const string CategoryAttribute       =   "CategoryAttribute";
				internal const string INativeObject           =	"INativeObject";
		}

		internal string PlatformAssembly {
			get {
				return IsDualBuild ? DualAssemblyName : CompatAssemblyName;
			}
		}

#if MTOUCH || MMP
		// "#if MTOUCH" code does not need locking when accessing 'types', because mtouch is single-threaded.
		public Dictionary<TType, ObjCType> Types {
			get { return types; }
		}
#endif

		protected Exception CreateException (int code, string message, params object[] args)
		{
			return CreateException (code, (TMethod)null, message, args);
		}

		protected Exception CreateException (int code, TMethod method, string message, params object[] args)
		{
			return CreateException (code, null, method, message, args);
		}

		protected Exception CreateException (int code, TProperty property, string message, params object[] args)
		{
			return CreateException (code, null, property, message, args);
		}

		protected Exception CreateException (int code, TType type, string message, params object [] args)
		{
			return CreateException (code, null, type, message, args);
		}

		protected Exception CreateException (int code, Exception innerException, TProperty property, string message, params object[] args)
		{
			if (property == null)
				return CreateException (code, innerException, (TMethod) null, message, args);
			var getter = GetGetMethod (property);
			if (getter != null)
				return CreateException (code, innerException, getter, message, args);
			return CreateException (code, innerException, GetSetMethod (property), message, args);
		}

		Exception CreateException (int code, ObjCMember member, string message, params object[] args)
		{
			var method = member as ObjCMethod;
			if (method != null)
				return CreateException (code, method.Method, message, args);
			var property = member as ObjCProperty;
			if (property != null)
				return CreateException (code, property.Property, message, args);
			return CreateException (code, message, args);
		}

		protected string GetDescriptiveMethodName (TMethod method)
		{
			if (method == null)
				return string.Empty;
			
			var sb = new StringBuilder ();

			sb.Append (GetMethodName (method));
			sb.Append ("(");
			var pars = GetParameters (method);
			if (pars != null && pars.Length > 0) {
				for (int i = 0; i < pars.Length; i++) {
					if (i > 0)
						sb.Append (",");
					sb.Append (GetTypeFullName (pars [i]));
				}
			}
			sb.Append (")");
			return sb.ToString ();
		}

		string GetDescriptiveMethodName (TType type, TMethod method)
		{
			return GetTypeFullName (type) + "." + GetDescriptiveMethodName (method);
		}

		// overridable so that descendant classes can provide faster implementations
		protected virtual bool IsNSObject (TType type)
		{
			string @namespace;
			string name;

			GetNamespaceAndName (type, out @namespace, out name);

			if (@namespace == Foundation && name == "NSObject")
				return true;

			var baseType = GetBaseType (type);
			if (baseType != null)
				return IsNSObject (baseType);

			return false;
		}

		protected virtual bool AreEqual (TType a, TType b)
		{
			return a == b;
		}

		protected bool Is (TType type, string @namespace, string name)
		{
			string ns, n;
			GetNamespaceAndName (type, out ns, out n);
			return ns == @namespace && n == name;
		}

		// overridable so that descendant classes can provide faster implementation
		// do not check base types.
		protected virtual bool HasModelAttribute (TType type)
		{
			object dummy;
			return TryGetAttribute (type, Foundation, StringConstants.ModelAttribute, out dummy);
		}
		
		// overridable so that descendant classes can provide a faster implementation
		// do not check base types.
		public virtual bool HasProtocolAttribute (TType type)
		{
			object dummy;
			return TryGetAttribute (type, Foundation, StringConstants.ProtocolAttribute, out dummy);
		}

		// This method is thread-safe (locks 'types').
		public ObjCType RegisterType (TType type)
		{
			ObjCType rv;
			List<Exception> exceptions = null;

			lock (types) {
				if (types.TryGetValue (type, out rv))
					return rv;

				rv = RegisterTypeUnsafe (type, ref exceptions);
			}

			if (exceptions != null && exceptions.Count > 0)
				throw new AggregateException (exceptions);

			return rv;
		}

		// This method is thread-safe (locks 'types').
		public ObjCType RegisterType (TType type, ref List<Exception> exceptions)
		{
			lock (types)
				return RegisterTypeUnsafe (type, ref exceptions);
		}

		bool VerifyNonGenericMethod (ref List<Exception> exceptions, TType declaringType, TMethod method)
		{
			if (!IsGenericMethod (method))
				return true;

			AddException (ref exceptions, CreateException (4113, method, 
				"The registrar found a generic method: '{0}'. Exporting generic methods is not supported, and will lead to random behavior and/or crashes", 
				GetDescriptiveMethodName (declaringType, method)));
			return false;
		}

		void VerifyInSdk (ref List<Exception> exceptions, ObjCMethod method)
		{
			if (method.HasReturnType || (method.Method != null && !method.IsConstructor && method.NativeReturnType != null))
				VerifyTypeInSDK (ref exceptions, method.NativeReturnType, returnTypeOf: method);

			if (method.HasParameters || (method.Method != null && method.Parameters != null)) {
				foreach (var p in method.Parameters)
					VerifyTypeInSDK (ref exceptions, p, parameterIn: method);
			}
		}

		void VerifyInSdk (ref List<Exception> exceptions, ObjCProperty property)
		{
			VerifyTypeInSDK (ref exceptions, property.PropertyType, propertyTypeOf: property);
		}

		void VerifyTypeInSDK (ref List<Exception> exceptions, TType type, ObjCMethod parameterIn = null, ObjCMethod returnTypeOf = null, ObjCProperty propertyTypeOf = null, TType baseTypeOf = null)
		{
			var attribs = GetAvailabilityAttributes (type);
			if (attribs == null || attribs.Count == 0)
				return;

			Version sdk = GetSDKVersion ();
			foreach (var attrib in attribs) {
				// The attributes are already filtered to the current platform.
				switch (attrib.AvailabilityKind) {
				case AvailabilityKind.Introduced:
					if (attrib.Version <= sdk)
						break;

					string msg = "The type '{0}' (used as {1} {2}) is not available in {3} {4} (it was introduced in {3} {5}){6} Please build with a newer {3} SDK (usually done by using the most recent version of Xcode).";
					string zero = GetTypeFullName (type);
					string one = string.Empty;
					string two = string.Empty;
					string three = PlatformName;
					string four = sdk.ToString ();
					string five = attrib.Version.ToString ();
					string six = string.IsNullOrEmpty (attrib.Message) ? "." : ": '" + attrib.Message + "'.";
					if (baseTypeOf != null) {
						one = "a base type of";
						two = GetTypeFullName (baseTypeOf);
					} else if (parameterIn != null) {
						one = "a parameter in";
						two = parameterIn.DescriptiveMethodName;
					} else if (returnTypeOf != null) {
						one = "a return type in";
						two = returnTypeOf.DescriptiveMethodName;
					} else if (propertyTypeOf != null) {
						one = "the property type of";
						two = propertyTypeOf.FullName;
					} else {
						msg = "The type '{0}' is not available in {3} {4} (it was introduced in {3} {5}){6} Please build with a newer {3} SDK (usually done by using the most recent version of Xcode).";
					}

					msg = string.Format (msg, zero, one, two, three, four, five, six);

					Exception ex;

					if (baseTypeOf != null) {
						ex = CreateException (4162, baseTypeOf, msg);
					} else if (parameterIn != null) {
						ex = CreateException (4162, parameterIn, msg);
					} else if (returnTypeOf != null) {
						ex = CreateException (4162, returnTypeOf, msg);
					} else if (propertyTypeOf != null) {
						ex = CreateException (4162, propertyTypeOf, msg);
					} else {
						ex = CreateException (4162, msg);
					}

					AddException (ref exceptions, ex);

					break;
				}
			}
		}

		protected static void AddException (ref List<Exception> exceptions, Exception mex)
		{
			if (exceptions == null)
				exceptions = new List<Exception> ();
			exceptions.Add (mex);
		}

		bool IsSubClassOf (TType type, string @namespace, string name)
		{
			while ((type = GetBaseType (type)) != null) {
				string ns, n;
				GetNamespaceAndName (type, out ns, out n);
				if (ns == @namespace && n == name)
					return true;
			}

			return false;
		}

		bool VerifyIsConstrainedToNSObject (ref List<Exception> exceptions, TType type, ObjCMethod method)
		{
			TType constrained_type = null;

			if (!VerifyIsConstrainedToNSObject (method.ReturnType, out constrained_type)) {
				AddException (ref exceptions, CreateException (4129, method.Method, "The registrar found an invalid generic return type '{0}' in the method '{1}'. The generic return type must have an 'NSObject' constraint.", GetTypeFullName (method.ReturnType), GetDescriptiveMethodName (type, method.Method)));
				return false;
			}
			if (constrained_type != null)
				method.ReturnType = constrained_type;

			var pars = method.Parameters;
			if (pars == null)
				return true;

			List<TType> types = null;
			for (int i = 0; i < pars.Length; i++) {
				var p = pars [i];
				if (!VerifyIsConstrainedToNSObject (p, out constrained_type)) {
					AddException (ref exceptions, CreateException (4128, method.Method, "The registrar found an invalid generic parameter type '{0}' in the parameter {2} of the method '{1}'. The generic parameter must have an 'NSObject' constraint.", GetTypeFullName (p), GetDescriptiveMethodName (type, method.Method), GetParameterName (method.Method, i)));
					return false;
				}
				if (constrained_type != null) {
					if (types == null) {
						types = new List<TType> ();
						for (int j = 0; j < i; j++)
							types.Add (pars [j]);
					}
					types.Add (constrained_type);
				} else if (types != null) {
					types.Add (p);
				}
			}
			if (types != null)
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
		void FlattenInterfaces (TType [] ifaces)
		{
			if (ifaces.Length == 1)
				return;

			for (int i = 0; i < ifaces.Length; i++) {
				var iface = ifaces [i];
				if (iface == null)
					continue;
				var ii = GetInterfaces (iface);
				if (ii == null)
					continue;
				for (int j = 0; j < ii.Length; j++) {
					for (int k = 0; k < ifaces.Length; k++) {
						if (k == i)
							continue;
						if (ifaces [k] == null)
							continue;
						if (ifaces [k] == ii [j])
							ifaces [k] = null;
					}
				}
			}
		}

		TType [] GetInterfacesImpl (ObjCType objcType)
		{
			// Both the dynamic and static registrars (i.e both Cecil and SRE)
			// return interfaces from all base classes as well.
			// We only want the interfaces declared on this type.

			// This function will return arrays with null entries.

			var type = objcType.Type;
			var allI = GetInterfaces (type);
			if (allI == null || allI.Length == 0)
				return allI;
			FlattenInterfaces (allI);

			var baseType = objcType.SuperType;
			if (baseType == null || baseType.Type == null)
				return allI;
				
			var baseI = GetInterfaces (baseType.Type);
			if (baseI == null || baseI.Length == 0)
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

		ObjCType [] GetProtocols (ObjCType type, ref List<Exception> exceptions)
		{
			var interfaces = GetInterfacesImpl (type);
			if (interfaces == null || interfaces.Length == 0)
				return null;

			var protocolList = new List<ObjCType> (interfaces.Length);
			for (int i = 0; i < interfaces.Length; i++) {
				if (interfaces [i] == null)
					continue;
				var baseP = RegisterTypeUnsafe (interfaces [i], ref exceptions);
				if (baseP != null)
					protocolList.Add (baseP);
			}
			if (protocolList.Count == 0)
				return null;
			return protocolList.ToArray ();
		}

		ObjCType RegisterCategory (TType type, CategoryAttribute attrib, ref List<Exception> exceptions)
		{
			if (IsINativeObject (type)) {
				AddException (ref exceptions, ErrorHelper.CreateError (4152, "Cannot register the type '{0}' as a category because it implements INativeObject or subclasses NSObject.", GetTypeFullName (type)));
				return null;
			}

			if (IsGenericType (type)) {
				AddException (ref exceptions, ErrorHelper.CreateError (4153, "Cannot register the type '{0}' as a category because it's generic.", GetTypeFullName (type)));
				return null;
			}

			if (attrib.Type == null) {
				AddException (ref exceptions, ErrorHelper.CreateError (4151, "Cannot register the type '{0}' because the Type property in its Category attribute isn't set.", GetTypeFullName (type)));
				return null;
			}

			var declaringType = RegisterType (attrib.Type, ref exceptions);
			if (declaringType == null) {
				AddException (ref exceptions, ErrorHelper.CreateError (4150, "Cannot register the type '{0}' because the category type '{1}' in its Category attribute does not inherit from NSObject.", GetTypeFullName (type), GetTypeFullName (attrib.Type)));
				return null;
			}

			var objcType = new ObjCType () {
				Registrar = this,
				Type = type,
				BaseType = declaringType,
				CategoryAttribute = attrib,
			};

			lock (categories_map) {
				TType previous_type;
				if (categories_map.TryGetValue (objcType.CategoryName, out previous_type)) {
					AddException (ref exceptions, ErrorHelper.CreateError (4156, "Cannot register two categories ('{0}' and '{1}') with the same native name ('{2}')",
						GetAssemblyQualifiedName (type), GetAssemblyQualifiedName (previous_type), objcType.CategoryName));
					return null;
				}
				categories_map.Add (objcType.CategoryName, type);
			}

			types.Add (type, objcType);

			Trace ("    [CATEGORY] Registering {0} on {1}", type.ToString ().Replace ('+', '/'), attrib.Type.FullName);

			foreach (var ctor in CollectConstructors (type)) {
				var ea = GetExportAttribute (ctor);
				if (ea == null)
					continue;

				AddException (ref exceptions, CreateException (4158, ctor, "Cannot register the constructor {0}.{1} in the category {0} because constructors in categories are not supported.", GetTypeFullName (type), GetDescriptiveMethodName (ctor)));
			}

			foreach (var method in CollectMethods (type)) {
				var ea = GetExportAttribute (method);

				if (ea == null)
					continue;
					
				if (!IsStatic (method)) {
					AddException (ref exceptions, CreateException (4159, method, "Cannot register the method '{0}.{1}' as a category method because category methods must be static.", GetTypeFullName (type), GetMethodName (method)));
					return null;
				}

				if (HasThisAttribute (method)) {
					var parameters = GetParameters (method);
					if (parameters == null || parameters.Length == 0) {
						AddException (ref exceptions, CreateException (4157, method, "Cannot register the category method '{0}.{1}' because at least one parameter is required for extension methods (and its type must match the category type '{2}').",
							GetTypeFullName (type), GetMethodName (method), GetTypeFullName (declaringType.Type)));
						continue;
					} else if (GetTypeFullName (parameters [0]) != GetTypeFullName (declaringType.Type)) {
						AddException (ref exceptions, CreateException (4149, method, "Cannot register the extension method '{0}.{1}' because the type of the first parameter ('{2}') does not match the category type ('{3}').",
							GetTypeFullName (type), GetMethodName (method), GetTypeFullName (parameters [0]), GetTypeFullName (declaringType.Type)));
						continue;
					}
				}

				if (IsGenericMethod (method)) {
					AddException (ref exceptions, CreateException (4154, method, "Cannot register the method '{0}.{1}' as a category method because it's generic.", GetTypeFullName (type), GetMethodName (method)));
					continue;
				}

				Trace ("        [METHOD] {0} => {1}", method, ea.Selector);

				var category_method = new ObjCMethod (this, declaringType, method)
				{
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
			
		protected bool SupportsModernObjectiveC {
			get {
#if MTOUCH || MONOTOUCH
				return true;
#elif MMP
				return App.Is64Build;
#elif MONOMAC
				return IntPtr.Size == 8;
#endif
			}
		}

		// This method is not thread-safe wrt 'types', and must be called with
		// a lock held on 'types'.
		ObjCType RegisterTypeUnsafe (TType type, ref List<Exception> exceptions)
		{
			ObjCType objcType;
			bool isGenericType = false;
			bool isProtocol = false;
			bool isInformalProtocol = false;

			if (IsGenericType (type)) {
				type = GetGenericTypeDefinition (type);
				isGenericType = true;
			}

			if (types.TryGetValue (type, out objcType)) {
				OnReloadType (objcType);
				return objcType;
			}

			var categoryAttribute = GetCategoryAttribute (type);
			if (categoryAttribute != null)
				return RegisterCategory (type, categoryAttribute, ref exceptions);

			if (!IsNSObject (type)) {
				//Trace ("{0} is not derived from NSObject", GetTypeFullName (type));
				if (!IsInterface (type))
					return null;

				if (!HasProtocolAttribute (type))
					return null;

				if (isGenericType) {
					exceptions.Add (ErrorHelper.CreateError (4148, "The registrar found a generic protocol: '{0}'. Exporting generic protocols is not supported.", GetTypeFullName (type)));
					return null;
				}

				// This is a protocol
				var pAttr = GetProtocolAttribute (type);
				isInformalProtocol = pAttr.IsInformal;
				isProtocol = true;

#if MMP || MTOUCH
				if (pAttr.FormalSinceVersion != null && pAttr.FormalSinceVersion > App.SdkVersion)
					isInformalProtocol = !isInformalProtocol;
#endif
			}

			// make sure the base type is already registered
			var baseType = GetBaseType (type);
			ObjCType baseObjCType = null;
			if (baseType != null) {
				Trace ("Registering base type {0} of {1}", GetTypeName (baseType), GetTypeName (type));
				baseObjCType = RegisterTypeUnsafe (baseType, ref exceptions);
			}

			var register_attribute = GetRegisterAttribute (type);
			if (register_attribute != null && register_attribute.SkipRegistration)
				return baseObjCType;

			objcType = new ObjCType () {
				Registrar = this,
				RegisterAttribute = GetRegisterAttribute (type),
				Type = type,
				IsModel = HasModelAttribute (type),
				IsProtocol = isProtocol,
				IsGeneric = isGenericType,
			};
			objcType.VerifyRegisterAttribute (ref exceptions);
			objcType.Protocols = GetProtocols (objcType, ref exceptions);
			objcType.BaseType = isProtocol ? null : (baseObjCType ?? objcType);
			objcType.IsWrapper = (isProtocol && !isInformalProtocol) ? (GetProtocolAttributeWrapperType (objcType.Type) != null) : (objcType.RegisterAttribute != null && objcType.RegisterAttribute.IsWrapper);

			if (!objcType.IsWrapper && objcType.BaseType != null)
				VerifyTypeInSDK (ref exceptions, objcType.BaseType.Type, baseTypeOf: objcType.Type);

			if (ObjCType.IsObjectiveCKeyword (objcType.ExportedName))
				AddException (ref exceptions, ErrorHelper.CreateError (4168, $"Cannot register the type '{GetTypeFullName (type)}' because its Objective-C name '{objcType.ExportedName}' is an Objective-C keyword. Please use a different name."));

			// make sure all the protocols this type implements are registered
			if (objcType.Protocols != null) {
				foreach (var p in objcType.Protocols) {
					Trace ("Registering implemented protocol {0} of {1}", p == null ? "null" : p.ProtocolName, GetTypeName (type));
					OnRegisterProtocol (p);
				}
			}

			TType previous_type;
			if (objcType.IsProtocol) {
				lock (protocol_map) {
					if (protocol_map.TryGetValue (objcType.ExportedName, out previous_type))
						throw ErrorHelper.CreateError (4126, "Cannot register two managed protocols ('{0}' and '{1}') with the same native name ('{2}').",
							GetAssemblyQualifiedName (type), GetAssemblyQualifiedName (previous_type), objcType.ExportedName);
					protocol_map.Add (objcType.ExportedName, type);
				}
			} else {
				lock (type_map) {
					if (type_map.TryGetValue (objcType.ExportedName, out previous_type))
						throw ErrorHelper.CreateError (4118, "Cannot register two managed types ('{0}' and '{1}') with the same native name ('{2}').",
							GetAssemblyQualifiedName (type), GetAssemblyQualifiedName (previous_type), objcType.ExportedName);
					type_map.Add (objcType.ExportedName, type);
				}
			}
		
			types.Add (type, objcType);
			
			Trace ("    [TYPE] Registering {0} => {1} IsWrapper: {2} BaseType: {3} IsModel: {4} IsProtocol: {5}", type.ToString ().Replace ('+', '/'), objcType.ExportedName, objcType.IsWrapper, objcType.BaseType == null ? "null" : objcType.BaseType.Name, objcType.IsModel, objcType.IsProtocol);

			// Special methods
			bool is_first_nonWrapper = false;
			var methods = new List<TMethod> (CollectMethods (type));
			if (!isProtocol) {
				is_first_nonWrapper = !(objcType.IsWrapper || objcType.IsModel) && (objcType.BaseType.IsWrapper || objcType.BaseType.IsModel);
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
						Selector = "xamarinSetGCHandle:",
						Trampoline = Trampoline.SetGCHandle,
						Signature = "v@:i",
						IsStatic = false,
					}, ref exceptions);
				}

				// Find conform_to_protocol
				if (conforms_to_protocol == null && Is (type, Foundation, "NSObject")) {
					foreach (var method in methods) {
						switch (GetMethodName (method)) {
						case "InvokeConformsToProtocol":
							invoke_conforms_to_protocol = method;
							break;
						case "ConformsToProtocol":
							conforms_to_protocol = method;
							break;
						}

						if (invoke_conforms_to_protocol != null && conforms_to_protocol != null)
							break;
					}
				}

#if MMP || MTOUCH
				// Special fields
				if (is_first_nonWrapper) {
					// static registrar
						objcType.Add (new ObjCField () {
							DeclaringType = objcType,
							FieldType = "XamarinObject",// "^v", // void*
							Name = "__monoObjectGCHandle",
							IsPrivate = SupportsModernObjectiveC,
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
								Selector = attrib.GetterSelector,
								Parameters = new TType[] { },
								ReturnType = attrib.PropertyType,
								IsStatic = attrib.IsStatic,
								IsOptional = !attrib.IsRequired,
								IsConstructor = false,
							};

							objcType.Add (objcGetter, ref exceptions);

							if (!string.IsNullOrEmpty (attrib.SetterSelector)) {
								var objcSetter = new ObjCMethod (this, objcType, null) {
									Name = attrib.Name,
									Selector = attrib.SetterSelector,
									Parameters = new TType[] { attrib.PropertyType },
									ReturnType = GetSystemVoidType (),
									IsStatic = attrib.IsStatic,
									IsOptional = !attrib.IsRequired,
									IsConstructor = false,
								};
								objcType.Add (objcSetter, ref exceptions);
							}
						} else {
							var objcProperty = new ObjCProperty () {
								Registrar = this,
								DeclaringType = objcType,
								Property = null,
								Name = attrib.Name,
								Selector = attrib.Selector,
								ArgumentSemantic = attrib.ArgumentSemantic,
								IsReadOnly = string.IsNullOrEmpty (attrib.SetterSelector),
								IsStatic = attrib.IsStatic,
								IsOptional = !attrib.IsRequired,
								GetterSelector = attrib.GetterSelector,
								SetterSelector = attrib.SetterSelector,
								PropertyType = attrib.PropertyType,
							};
							objcType.Add (objcProperty, ref exceptions);
						}
					} else {
						var objcMethod = new ObjCMethod (this, objcType, null) {
							Name = attrib.Name,
							Selector = attrib.Selector,
							ArgumentSemantic = attrib.ArgumentSemantic,
							IsVariadic = attrib.IsVariadic,
							ReturnType = attrib.ReturnType ?? GetSystemVoidType (),
							IsStatic = attrib.IsStatic,
							IsOptional = !attrib.IsRequired,
							IsConstructor = false,
						};

						if (attrib.ParameterType != null) {
							var parameters = new TType [attrib.ParameterType.Length];
							for (int i = 0; i < parameters.Length; i++) {
								if (attrib.ParameterByRef [i]) {
									parameters [i] = MakeByRef (attrib.ParameterType [i]);
								} else {
									parameters [i] = attrib.ParameterType [i];
								}
							}
							objcMethod.Parameters = parameters;
						} else {
							objcMethod.Parameters = new TType[] { };
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
					if (ca != null) {
						if (!IsINativeObject (GetPropertyType (property))) {
							AddException (ref exceptions, CreateException (4139, property,
								"The registrar cannot marshal the property type '{0}' of the property '{1}.{2}'. Properties with the [Connect] attribute must have a property type of NSObject (or a subclass of NSObject).",
								GetTypeFullName (GetPropertyType (property)), GetTypeFullName (type), GetPropertyName (property)));
							continue;
						}

						objcType.Add (new ObjCField () {
							DeclaringType = objcType,
							Name = ca.Name ?? GetPropertyName (property),
#if !MTOUCH && !MMP
							Size = Is64Bits ? 8 : 4,
							Alignment = (byte) (Is64Bits ? 3 : 2),
#endif
							FieldType = "@",
							IsProperty = true,
							IsStatic = IsStatic (property),
						}, ref exceptions);
					}
				}

				var ea = GetExportAttribute (property);

				if (ea == null)
					continue;

				if (IsStatic (property) && (objcType.IsWrapper || objcType.IsModel)) {
					// This is useless to export, since the user can't actually do anything with it,
					// it'll just call back into the base implementation.
					continue;
				}

				if (IsStatic (property) && isGenericType) {
					AddException (ref exceptions, CreateException (4131, property, "The registrar cannot export static properties in generic classes ('{0}.{1}').", GetTypeFullName (type), GetPropertyName (property)));
					continue;
				}

				TType property_type = null;
				if (isGenericType && !VerifyIsConstrainedToNSObject (GetPropertyType (property), out property_type)) {
					AddException (ref exceptions, CreateException (4132, property, "The registrar found an invalid generic return type '{0}' in the property '{1}.{2}'. The return type must have an 'NSObject' constraint.", GetTypeFullName (GetPropertyType (property)), GetTypeFullName (type), GetPropertyName (property)));
					continue;
				}
				if (property_type == null)
					property_type = GetPropertyType (property);

				Trace ("        [PROPERTY] {0} => {1}", property, ea.Selector);

				var objcProperty = new ObjCProperty ()
				{
					Registrar = this,
					DeclaringType = objcType,
					Property = property,
					Name = property.Name,
					Selector = ea.Selector ?? GetPropertyName (property),
					ArgumentSemantic = ea.ArgumentSemantic,
					PropertyType = property_type,
				};

				TMethod getter = GetGetMethod (property);
				TMethod setter = GetSetMethod (property);

				if (getter != null && VerifyNonGenericMethod (ref exceptions, type, getter)) {
					var method = new ObjCMethod (this, objcType, getter) {
						Selector = ea.Selector ?? GetPropertyName (property),
						ArgumentSemantic = ea.ArgumentSemantic,
						ReturnType = property_type,
					};

					List<Exception> excs = null;
					if (!method.ValidateSignature (ref excs)) {
						exceptions.Add (CreateException (4138, excs [0], property, "The registrar cannot marshal the property type '{0}' of the property '{1}.{2}'.",
							GetTypeFullName (property.PropertyType), property.DeclaringType.FullName, property.Name));
						continue;
					}

					if (!objcType.Add (method, ref exceptions))
						continue;

					Trace ("            [GET] {0}", objcType.Methods [objcType.Methods.Count - 1].Name);
				}

				if (setter != null && VerifyNonGenericMethod (ref exceptions, type, setter)) {
					string setterName = ea.Selector ?? GetPropertyName (property);

					var method = new ObjCMethod (this, objcType, setter) {
						Selector = CreateSetterSelector (setterName),
						ArgumentSemantic = ea.ArgumentSemantic,
						Parameters = new TType[] { property_type },
					};

					List<Exception> excs = null;
					if (!method.ValidateSignature (ref excs)) {
						exceptions.Add (CreateException (4138, excs [0], property, "The registrar cannot marshal the property type '{0}' of the property '{1}.{2}'.",
							GetTypeFullName (property.PropertyType), property.DeclaringType.FullName, property.Name));
						continue;
					}

					if (!objcType.Add (method, ref exceptions))
						continue;

					Trace ("            [SET] {0}", objcType.Methods [objcType.Methods.Count - 1].Name);
				}

				objcType.Add (objcProperty, ref exceptions);
			}

			var custom_conforms_to_protocol = !is_first_nonWrapper; // we only have to generate the conformsToProtocol method for the first non-wrapper type.

#if MONOMAC
			ObjCMethod custom_copy_with_zone = null;
			var isNSCellSubclass = IsSubClassOf (type, AppKit, "NSCell");
#endif
			Dictionary<TMethod, List<TMethod>> method_map = null;

			if (!isProtocol)
				method_map = PrepareMethodMapping (type);

			foreach (TMethod method in methods) {
				if (hasProtocolMemberAttributes)
					continue;
				
				var ea = GetExportAttribute (method);

				if (ea == null) {
					List<TMethod> impls;
					if (method_map != null && method_map.TryGetValue (method, out impls)) {
						if (impls.Count != 1) {
							AddException (ref exceptions, Shared.GetMT4127 (method, impls));
							continue;
						}

						ea = GetExportAttribute (impls [0]);
					}
				}

				if (ea == null)
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

#if MONOMAC
				if (objcMethod.Selector == "copyWithZone:")
					custom_copy_with_zone = objcMethod;
#endif

				if (IsStatic (method) && isGenericType) {
					AddException (ref exceptions, CreateException (4130, method, "The registrar cannot export static methods in generic classes ('{0}').", GetDescriptiveMethodName (type, method)));
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

#if MONOMAC
			if (isNSCellSubclass) {
				if (custom_copy_with_zone != null) {
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
				if (parameters == null || parameters.Length == 0) {
					Trace ("        [CTOR] {0} default => init", GetTypeName (type));

					objcType.Add (new ObjCMethod (this, objcType, ctor)
					{
						Selector = "init",
						Trampoline = Trampoline.Constructor,
					}, ref exceptions);
					continue;
				}

				var ea = GetExportAttribute (ctor);

				if (ea == null)
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
			if (assembly == null)
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
				ReportError (4116, "Could not register the assembly '{0}': {1}", GetAssemblyName (assembly), e);
			}

			FlushTrace ();

			if (exceptions.Count > 0) {
				Exception ae = exceptions.Count == 1 ? exceptions [0] : new AggregateException (exceptions);
#if !MTOUCH
				Console.WriteLine (ae);
#endif
				throw ae;
			}
		}

		public string ComputeSignature (TType DeclaringType, TMethod Method, ObjCMember member = null, bool isCategoryInstance = false, bool isBlockSignature = false)
		{
			var success = true;
			var signature = new StringBuilder ();
			bool is_ctor;
			var method = member as ObjCMethod;

			if (Method != null) {
				is_ctor = IsConstructor (Method);
			} else {
				is_ctor = method.IsConstructor;
			}

			if (is_ctor) {
				signature.Append ('@');
			} else {
				var ReturnType = Method != null ? GetReturnType (Method) : method.NativeReturnType;
				signature.Append (ToSignature (ReturnType, member, ref success));
				if (!success)
					throw CreateException (4104, Method ?? method.Method, "The registrar cannot marshal the return value of type `{0}` in the method `{1}.{2}`.", GetTypeFullName (ReturnType), GetTypeFullName (DeclaringType), GetDescriptiveMethodName (Method ?? method.Method));
			}

			signature.Append (isBlockSignature ? "@?" : "@:");

			TType[] parameters;
			if (Method != null) {
				parameters = GetParameters (Method);
			} else {
				parameters = method.NativeParameters;
			}

			if (parameters != null) {
				for (int i = 0; i < parameters.Length; i++) {
					if (i == 0 && isCategoryInstance)
						continue;
					var type = parameters [i];
					if (IsByRef (type)) {
						signature.Append ("^");
						signature.Append (ToSignature (GetElementType (type), member, ref success));
					} else {
						signature.Append (ToSignature (type, member, ref success));
					}
					if (!success) {
						var mi = Method ?? method.Method;
						throw CreateException (4136, mi, "The registrar cannot marshal the parameter type '{0}' of the parameter '{1}' in the method '{2}.{3}'",
							GetTypeFullName (GetParameters (mi) [i]), GetParameterName (mi, i), GetTypeFullName (DeclaringType), GetDescriptiveMethodName (mi));
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
			if (objcMethod != null)
				throw ErrorHelper.CreateError (4111, "The registrar cannot build a signature for type `{0}' in method `{1}`.", GetTypeFullName (type), GetTypeFullName (objcMethod.DeclaringType.Type) + "." + objcMethod.MethodName);

			throw ErrorHelper.CreateError (4101, "The registrar cannot build a signature for type `{0}`.", GetTypeFullName (type));
		}

		public static string SanitizeName (string name)
		{
			StringBuilder sb = null;

			for (int i = 0; i < name.Length; i++) {
				var ch = name [i];
				switch (ch) {
				case '.':
				case '+':
				case '/':
				case '`':
				case '@':
				case '<':
				case '>':
				case '$':
				case '-':
					if (sb == null)
						sb = new StringBuilder (name, 0, i, name.Length);
					sb.Append ('_');
					break;
				default:
					if (sb != null)
						sb.Append (ch);
					break;
				}
			}

			if (sb != null)
				return sb.ToString ();
			
			return name;
		}

		public string GetExportedTypeName (TType type, RegisterAttribute register_attribute)
		{
			string name = null;
			if (register_attribute != null) {
				if (register_attribute.SkipRegistration)
					return GetExportedTypeName (GetBaseType (type));
				name = register_attribute.Name;
			}
			if (name == null)
				name = GetTypeFullName (type);
			return SanitizeName (name);
		}

		protected string GetExportedTypeName (TType type)
		{
			return GetExportedTypeName (type, GetRegisterAttribute (type));
		}

		protected string ToSignature (TType type, ObjCMember member, ref bool success, bool forProperty = false)
		{
			bool isNativeEnum;

			var typeFullName = GetTypeFullName (type);

			switch (typeFullName) {
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
			case "System.Boolean":
				// map managed 'bool' to ObjC BOOL = 'unsigned char' in OSX and 32bit iOS architectures and 'bool' in 64bit iOS architectures
				#if MONOMAC
				return "c";
				#else
				return Is64Bits ? "B" : "c";
				#endif
			case "System.Void": return "v";
			case "System.String":
				return forProperty ? "@\"NSString\"" : "@";
			case "System.nint":
				return Is64Bits ? "q" : "i";
			case "System.nuint":
				return Is64Bits ? "Q" : "I";
			case "System.nfloat":
				return Is64Bits ? "d" : "f";
			case "System.DateTime":
				throw CreateException (4102, member, "The registrar found an invalid type `{0}` in signature for method `{2}`. Use `{1}` instead.", "System.DateTime", IsDualBuild ? "Foundation.NSDate" : CompatNamespace + ".Foundation.NSDate", member.FullName);
			}

			if (Is (type, ObjCRuntime, "Selector"))
				return ":";

			if (Is (type, ObjCRuntime, "Class"))
				return "#";

			if (IsINativeObject (type)) {
				if (IsNSObject (type) && forProperty) {
					return "@\"" + GetExportedTypeName (type) + "\"";
				} else {
					return "@";
				}
			}

			if (IsDelegate (type))
				return "^v";

			if (IsEnum (type, out isNativeEnum)) {
				if (isNativeEnum && !Is64Bits) {
					switch (GetEnumUnderlyingType (type).FullName) {
					case "System.Int64":
						return "i";
					case "System.UInt64":
						return "I";
					default:
						throw CreateException (4145, "Invalid enum '{0}': enums with the [Native] attribute must have a underlying enum type of either 'long' or 'ulong'.", GetTypeFullName (type));
					}
				} else {
					return ToSignature (GetEnumUnderlyingType (type), member, ref success);
				}
			}

			if (IsValueType (type))
				return ValueTypeSignature (type, member, ref success);

			if (IsArray (type)) {
				ToSignature (GetElementType (type), member, ref success); // this validates that the element type is a type we support.
				return "@"; // But we don't care about the actual type, we'll just return '@'. We only support NSArrays of the element type, so '@' is always right.
			}

			success = false;
			return string.Empty;
		}

		string ValueTypeSignature (TType type, ObjCMember member)
		{
			bool success = true;
			return ValueTypeSignature (type, member, ref success);
		}

		string ValueTypeSignature (TType type, ObjCMember member, ref bool success)
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

#if MTOUCH || MMP
		internal static void NSLog (string format, params object [] args)
		{
			Console.WriteLine (format, args);
		}
#endif

		protected virtual void ReportError (int code, string message, params object [] args)
		{
			// Using Console.WriteLine here is error prone, since if we get an early error
			// we'll end up crashing/infinite recursion since Console.WriteLine is redirected
			// to NSLog and is using NSString (and we haven't necessarily finished registering
			// everything yet).
			R.NSLog (message, args);
		}

		protected virtual void ReportWarning (int code, string message, params object[] args)
		{
			// Using Console.WriteLine here is error prone, since if we get an early error
			// we'll end up crashing/infinite recursion since Console.WriteLine is redirected
			// to NSLog and is using NSString (and we haven't necessarily finished registering
			// everything yet).
			R.NSLog (ErrorHelper.CreateWarning (code, message, args).ToString ());
		}

		static StringBuilder trace;

		[Conditional ("VERBOSE_REGISTRAR")]
		public static void FlushTrace ()
		{
			if (trace != null) {
				trace.Insert (0, '\n');
				R.NSLog (trace.ToString ());
#if DEV
				Console.WriteLine (trace.ToString ());
#endif
				trace.Clear ();
			}
		}

		[Conditional ("VERBOSE_REGISTRAR")]
		public static void Trace (string msg, params object [] args)
		{
			if (trace == null)
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
		X86_DoubleABI_StaticStretTrampoline,
		X86_DoubleABI_StretTrampoline,
#if MONOMAC
		CopyWithZone1,
		CopyWithZone2,
#endif
		GetGCHandle,
		SetGCHandle,
	}
}

