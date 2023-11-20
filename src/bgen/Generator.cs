//
// This is the binding generator for the MonoTouch API, it uses the
// contract in API.cs to generate the binding.
//
// Authors:
//   Geoff Norton
//   Miguel de Icaza
//   Marek Safar (marek.safar@gmail.com)
//
// Copyright 2009-2010, Novell, Inc.
// Copyright 2011-2015 Xamarin, Inc.
//
//
// This generator produces various */*.g.cs files based on the
// interface-based type description on this file, see the 
// embedded `MonoTouch.UIKit' namespace here for an example
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
// TODO:
//   * Add support for wrapping "ref" and "out" NSObjects (WrappedTypes)
//     Typically this is necessary for things like NSError.
//

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.ComponentModel;
using System.Reflection;
using ObjCRuntime;
using Foundation;
using Xamarin.Utils;

// Disable until we get around to enable + fix any issues.
#nullable disable

public partial class Generator : IMemberGatherer {
	internal bool IsPublicMode;
	internal static string NativeHandleType;
	BindingTouch BindingTouch;
	Frameworks Frameworks { get { return BindingTouch.Frameworks; } }
	public TypeManager TypeManager { get { return BindingTouch.TypeManager; } }
	public AttributeManager AttributeManager { get { return BindingTouch.AttributeManager; } }
	NamespaceCache NamespaceCache { get { return BindingTouch.NamespaceCache; } }
	public TypeCache TypeCache { get { return BindingTouch.TypeCache; } }

	Nomenclator nomenclator;
	Nomenclator Nomenclator {
		get {
			nomenclator ??= new (AttributeManager);
			return nomenclator;
		}
	}
	public GeneratedTypes GeneratedTypes;
	List<Exception> exceptions = new List<Exception> ();

	Dictionary<Type, IEnumerable<string>> selectors = new Dictionary<Type, IEnumerable<string>> ();
	Dictionary<Type, bool> need_abstract = new Dictionary<Type, bool> ();
	Dictionary<string, int> selector_use = new Dictionary<string, int> ();
	Dictionary<string, string> selector_names = new Dictionary<string, string> ();
	Dictionary<string, string> send_methods = new Dictionary<string, string> ();
	readonly MarshalTypeList marshalTypes = new ();
	Dictionary<Type, TrampolineInfo> trampolines = new Dictionary<Type, TrampolineInfo> ();
	Dictionary<Type, Type> notification_event_arg_types = new Dictionary<Type, Type> ();
	Dictionary<string, string> libraries = new Dictionary<string, string> (); // <LibraryName, libraryPath>

	List<Tuple<string, ParameterInfo []>> async_result_types = new List<Tuple<string, ParameterInfo []>> ();
	HashSet<string> async_result_types_emitted = new HashSet<string> ();

	//
	// This contains delegates that are referenced in the source and need to be generated.
	//
	Dictionary<string, MethodInfo> delegate_types = new Dictionary<string, MethodInfo> ();

	public PlatformName CurrentPlatform { get { return BindingTouch.CurrentPlatform; } }
	public int XamcoreVersion => CurrentPlatform.GetXamcoreVersion ();
	public string ApplicationClassName => CurrentPlatform.GetApplicationClassName ();
	public string CoreImageMap => CurrentPlatform.GetCoreImageMap ();
	public string CoreServicesMap => CurrentPlatform.GetCoreServicesMap ();
	public string PDFKitMap => CurrentPlatform.GetPDFKitMap ();

	Type [] types, strong_dictionaries;
	bool debug;
	bool external;
	StreamWriter sw, m;
	int indent;

	//
	// Properties and definitions to support binding third-party Objective-C libraries
	//
	string is_direct_binding_value; // An expression that calculates the IsDirectBinding value. Might not be a constant expression. This will be added to every constructor for a type.
	bool? is_direct_binding; // If a constant value for IsDirectBinding is known, it's stored here. Will be null if no constant value is known.

	// Whether to use ZeroCopy for strings, defaults to false
	public bool ZeroCopyStrings;

	public bool BindThirdPartyLibrary { get { return BindingTouch.BindThirdPartyLibrary; } }
	public bool InlineSelectors;
	public string BaseDir { get { return basedir; } set { basedir = value; } }
	string basedir;
	HashSet<string> generated_files = new HashSet<string> ();

	//
	// We inject thread checks to MonoTouch.UIKit types, unless there is a [ThreadSafe] attribuet on the type.
	// Set on every call to Generate
	//
	bool type_needs_thread_checks;

	//
	// If set, the members of this type will get zero copy
	// 
	internal bool type_wants_zero_copy;

	//
	// Used by the public binding generator to populate the
	// class with types that do not exist
	//
	public void RegisterMethodName (string method_name)
	{
		send_methods [method_name] = method_name;
	}

	//
	// Helpers
	//
	string MakeSig (MethodInfo mi, bool stret, bool aligned = false) { return MakeSig ("objc_msgSend", stret, mi, aligned); }
	string MakeSuperSig (MethodInfo mi, bool stret, bool aligned = false) { return MakeSig ("objc_msgSendSuper", stret, mi, aligned); }

	public IEnumerable<string> GeneratedFiles {
		get {
			return generated_files;
		}
	}

	public bool IsNSObject (Type type)
	{
		if (type == TypeCache.NSObject)
			return true;

		if (type.IsSubclassOf (TypeCache.NSObject))
			return true;

		if (BindThirdPartyLibrary) {
			var bta = ReflectionExtensions.GetBaseTypeAttribute (type, this);
			if (bta?.BaseType is not null)
				return IsNSObject (bta.BaseType);

			return false;
		}

		return type.IsInterface;
	}

	public string PrimitiveType (Type t, bool formatted = false)
	{
		if (t.IsEnum) {
			var enumType = t;

			t = t.GetEnumUnderlyingType ();

			if (IsNativeEnum (enumType, out var _, out var nativeType))
				return nativeType;
		}

		return TypeManager.PrimitiveType (t, formatted);
	}

	//
	// Returns the type that we use to marshal the given type as a string
	// for example "UIView" -> "IntPtr"
	string ParameterGetMarshalType (MarshalInfo mai, bool formatted = false)
	{
		// formatted: used as an actual type name
		// !formatted: used in the method name
		if (mai.IsAligned)
			return "IntPtr";

		if (mai.Type.IsEnum)
			return PrimitiveType (mai.Type, formatted);

		if (TypeManager.IsWrappedType (mai.Type)) {
			if (!mai.Type.IsByRef)
				return NativeHandleType;
			if (formatted)
				return NativeHandleType + "*";
			return "ref " + NativeHandleType;
		}

		if (mai.Type.Namespace == "System") {
			if (mai.Type.Name == "nint")
				return "IntPtr";
			if (mai.Type.Name == "nuint")
				return "UIntPtr";
		}

		if (TypeManager.IsNativeType (mai.Type))
			return PrimitiveType (mai.Type, formatted);

		if (mai.Type == TypeCache.System_String) {
			if (mai.PlainString)
				return "string";

			// We will do NSString
			return NativeHandleType;
		}

		MarshalType mt;
		if (marshalTypes.TryGetMarshalType (mai.Type, out mt))
			return mt.Encoding;

		if (mai.Type.IsValueType)
			return PrimitiveType (mai.Type, formatted);

		// Arrays are returned as NSArrays
		if (mai.Type.IsArray)
			return NativeHandleType;

		//
		// Pass "out ValueType" directly
		//
		if (mai.Type.IsByRef && mai.Type.GetElementType ().IsValueType) {
			Type elementType = mai.Type.GetElementType ();

			if (formatted) {
				if (elementType == TypeCache.System_Boolean)
					return "byte*";
				else if (elementType == TypeCache.System_Char)
					return "ushort*";
				return TypeManager.FormatType (null, elementType) + "*";
			}

			return (mai.IsOut ? "out " : "ref ") + elementType.Name;
		}

		// Pass "ValueType*" directly
		if (mai.Type.IsPointer) {
			var elementType = mai.Type.GetElementType ();
			if (elementType.IsValueType)
				return (formatted ? TypeManager.FormatType (null, elementType) : elementType.Name) + "*";
		}

		if (mai.Type.IsSubclassOf (TypeCache.System_Delegate)) {
			return NativeHandleType;
		}

		if (TypeManager.IsDictionaryContainerType (mai.Type)) {
			return NativeHandleType;
		}

		//
		// Edit the table in the "void Go ()" routine
		//

		if (mai.Type.IsByRef && mai.Type.GetElementType ().IsValueType == false) {
			if (formatted)
				return NativeHandleType + "*";
			return "ref " + NativeHandleType;
		}

		if (mai.Type.IsGenericParameter)
			return NativeHandleType;

		throw new BindingException (1017, true, mai.Type);
	}

	bool IsProtocolInterface (Type type, bool checkPrefix = true)
	{
		return IsProtocolInterface (type, checkPrefix, out var _);
	}

	bool IsProtocolInterface (Type type, bool checkPrefix, out Type protocol)
	{
		protocol = null;
		// for subclassing the type (from the binding files) is not yet prefixed by an `I`
		if (checkPrefix && type.Name [0] != 'I')
			return false;

		protocol = type;
		if (AttributeManager.HasAttribute<ProtocolAttribute> (type))
			return true;

		protocol = type.Assembly.GetType (type.Namespace + "." + type.Name.Substring (1), false);
		if (protocol is null)
			return false;

		return AttributeManager.HasAttribute<ProtocolAttribute> (protocol);
	}

	string FindProtocolInterface (Type type, MemberInfo pi)
	{
		var isArray = type.IsArray;
		var declType = isArray ? type.GetElementType () : type;

		if (!AttributeManager.HasAttribute<ProtocolAttribute> (declType))
			throw new BindingException (1034, true,
				pi.DeclaringType, pi.Name, declType);

		return "I" + type.Name;
	}

	public BindAsAttribute GetBindAsAttribute (ICustomAttributeProvider cu)
	{
		BindAsAttribute rv;
		if (cu is not null && (rv = AttributeManager.GetCustomAttribute<BindAsAttribute> (cu)) is not null)
			return rv;

		var minfo = cu as MethodInfo;
		if (minfo?.ReturnParameter is not null && (rv = AttributeManager.GetCustomAttribute<BindAsAttribute> (minfo.ReturnParameter)) is not null)
			return rv;

		return null;
	}
	public bool HasBindAsAttribute (ICustomAttributeProvider cu)
	{
		return GetBindAsAttribute (cu) is not null;
	}

	static bool IsSetter (MethodInfo mi) => mi.IsSpecialName && mi.Name.StartsWith ("set_", StringComparison.Ordinal);
	static BindingException GetBindAsException (string box, string retType, string containerType, string container, params ICustomAttributeProvider [] providers)
	{
		Type declaringType = null;
		string memberName = null;
		foreach (var provider in providers) {
			if (provider is MemberInfo member) {
				declaringType = member.DeclaringType;
				memberName = member.Name;
			} else if (provider is ParameterInfo parameter) {
				declaringType = parameter.Member.DeclaringType;
				memberName = parameter.Member.Name;
				break;
			}
		}

		if (declaringType is not null && memberName is not null)
			memberName = declaringType.FullName + "." + memberName;

		return new BindingException (1049, true, box, retType, containerType, container, memberName);
	}
	bool IsMemberInsideProtocol (Type type) => IsProtocol (type) || IsModel (type);

	bool IsSmartEnum (Type type)
	{
		if (!type.IsEnum)
			return false;
		// First check if the smart enum candidate still holds the FieldAtttribute data
		if (type.GetFields ().Any (f => AttributeManager.GetCustomAttribute<FieldAttribute> (f) is not null))
			return true;
		// If the above fails it's possible that it comes from another dll (like X.I.dll) so we look for the [Enum]Extensions class existence
		return type.Assembly.GetType (type.FullName + "Extensions") is not null;
	}
	
	string GetToBindAsWrapper (MethodInfo mi, MemberInformation minfo = null, ParameterInfo pi = null)
	{
		BindAsAttribute attrib = null;
		Type originalType = null;
		string temp = null;
		var declaringType = minfo?.mi?.DeclaringType ?? pi?.Member?.DeclaringType;

		if (IsMemberInsideProtocol (declaringType))
			throw new BindingException (1050, true, declaringType.Name);

		if (pi is null) {
			attrib = GetBindAsAttribute (minfo.mi);
			var property = minfo.mi as PropertyInfo;
			var method = minfo.mi as MethodInfo;
			originalType = method?.ReturnType ?? property?.PropertyType;
		} else {
			attrib = GetBindAsAttribute (pi);
			originalType = pi.ParameterType;
		}

		if (originalType.IsByRef)
			throw new BindingException (1080, true, originalType.Name.Replace ("&", string.Empty));

		var retType = TypeManager.GetUnderlyingNullableType (attrib.Type) ?? attrib.Type;
		var isNullable = attrib.IsNullable (this);
		var isValueType = retType.IsValueType;
		var isEnum = retType.IsEnum;
		var parameterName = pi is not null ? pi.Name.GetSafeParamName () : "value";
		var denullify = isNullable ? ".Value" : string.Empty;
		var nullCheck = isNullable ? $"{parameterName} is null ? null : " : string.Empty;

		if (isNullable || !isValueType)
			temp = string.Format ("{0} is null ? null : ", parameterName);

		if (originalType == TypeCache.NSNumber) {
			var enumCast = isEnum ? $"(int)" : string.Empty;
			temp = string.Format ("{3}new NSNumber ({2}{1}{0});", denullify, parameterName, enumCast, nullCheck);
		} else if (originalType == TypeCache.NSValue) {
			var typeStr = string.Empty;
			if (!TypeCache.NSValueCreateMap.TryGetValue (retType, out typeStr)) {
				// HACK: These are problematic for X.M due to we do not ship System.Drawing for Full profile
				if (retType.Name == "RectangleF" || retType.Name == "SizeF" || retType.Name == "PointF")
					typeStr = retType.Name;
				else
					throw GetBindAsException ("box", retType.Name, originalType.Name, "container", minfo?.mi, pi);
			}
			temp = string.Format ("{3}NSValue.From{0} ({2}{1});", typeStr, denullify, parameterName, nullCheck);
		} else if (originalType == TypeCache.NSString && IsSmartEnum (retType)) {
			temp = isNullable ? $"{parameterName} is null ? null : " : string.Empty;
			temp += $"{TypeManager.FormatType (retType.DeclaringType, retType)}Extensions.GetConstant ({parameterName}{denullify});";
		} else if (originalType.IsArray && originalType.GetArrayRank () == 1) {
			if (!retType.IsArray) {
				throw new BindingException (1072, true,
					parameterName, mi.DeclaringType.FullName, mi.Name);
			}
			var arrType = originalType.GetElementType ();
			var arrRetType = TypeManager.GetUnderlyingNullableType (retType.GetElementType ()) ?? retType.GetElementType ();
			var valueConverter = string.Empty;

			if (arrType == TypeCache.NSString && !isNullable) {
				valueConverter = isNullable ? "o is null ? null : " : string.Empty;
				valueConverter += $"{TypeManager.FormatType (retType.DeclaringType, arrRetType)}Extensions.GetConstant ({(isNullable ? "o.Value" : "o")}), {parameterName});";
			} else if (arrType == TypeCache.NSNumber && !isNullable) {
				var cast = arrRetType.IsEnum ? "(int)" : string.Empty;
				valueConverter = $"new NSNumber ({cast}o{denullify}), {parameterName});";
			} else if (arrType == TypeCache.NSValue && !isNullable) {
				var typeStr = string.Empty;
				if (!TypeCache.NSValueCreateMap.TryGetValue (arrRetType, out typeStr)) {
					if (arrRetType.Name == "RectangleF" || arrRetType.Name == "SizeF" || arrRetType.Name == "PointF")
						typeStr = retType.Name;
					else
						throw GetBindAsException ("box", arrRetType.Name, originalType.Name, "array", minfo?.mi, pi);
				}
				valueConverter = $"NSValue.From{typeStr} (o{denullify}), {parameterName});";
			} else
				throw new BindingException (1048, true, isNullable ? arrRetType.Name + "?[]" : retType.Name);
			temp = $"NSArray.FromNSObjects (o => {valueConverter}";
		} else
			throw new BindingException (1048, true, retType.Name);

		return temp;
	}

	string GetFromBindAsWrapper (MemberInformation minfo, out string suffix)
	{
		var declaringType = minfo.mi.DeclaringType;
		if (IsMemberInsideProtocol (declaringType))
			throw new BindingException (1050, true, declaringType.Name);

		suffix = string.Empty;

		var attrib = GetBindAsAttribute (minfo.mi);
		var nullableRetType = TypeManager.GetUnderlyingNullableType (attrib.Type);
		var isNullable = nullableRetType is not null;
		var retType = isNullable ? nullableRetType : attrib.Type;
		var isValueType = retType.IsValueType;
		var append = string.Empty;
		var property = minfo.mi as PropertyInfo;
		var method = minfo.mi as MethodInfo;
		var originalReturnType = method?.ReturnType ?? property?.PropertyType;

		if (originalReturnType == TypeCache.NSNumber) {
			if (!TypeManager.NSNumberReturnMap.TryGetValue (retType, out append)) {
				if (retType.IsEnum) {
					var enumType = retType.GetEnumUnderlyingType ();
					if (!TypeManager.NSNumberReturnMap.TryGetValue (enumType, out append))
						throw GetBindAsException ("unbox", retType.Name, originalReturnType.Name, "container", minfo.mi);
				} else
					throw GetBindAsException ("unbox", retType.Name, originalReturnType.Name, "container", minfo.mi);
			}
			if (isNullable)
				append = $"?{append}";
		} else if (originalReturnType == TypeCache.NSValue) {
			if (!TypeManager.NSValueReturnMap.TryGetValue (retType, out append)) {
				// HACK: These are problematic for X.M due to we do not ship System.Drawing for Full profile
				if (retType.Name == "RectangleF" || retType.Name == "SizeF" || retType.Name == "PointF")
					append = $".{retType.Name}Value";
				else
					throw GetBindAsException ("unbox", retType.Name, originalReturnType.Name, "container", minfo.mi);
			}
			if (isNullable)
				append = $"?{append}";
		} else if (originalReturnType == TypeCache.NSString && IsSmartEnum (retType)) {
			append = $"{TypeManager.FormatType (retType.DeclaringType, retType)}Extensions.GetValue (";
			suffix = ")";
		} else if (originalReturnType.IsArray && originalReturnType.GetArrayRank () == 1) {
			var arrType = originalReturnType.GetElementType ();
			var nullableElementType = TypeManager.GetUnderlyingNullableType (retType.GetElementType ());
			var arrIsNullable = nullableElementType is not null;
			var arrRetType = arrIsNullable ? nullableElementType : retType.GetElementType ();
			var valueFetcher = string.Empty;
			if (arrType == TypeCache.NSString && !arrIsNullable)
				append = $"ptr => {{\n\tusing (var str = Runtime.GetNSObject<NSString> (ptr)!) {{\n\t\treturn {TypeManager.FormatType (arrRetType.DeclaringType, arrRetType)}Extensions.GetValue (str);\n\t}}\n}}";
			else if (arrType == TypeCache.NSNumber && !arrIsNullable) {
				if (TypeManager.NSNumberReturnMap.TryGetValue (arrRetType, out valueFetcher) || arrRetType.IsEnum) {
					var getterStr = string.Format ("{0}{1}", arrIsNullable ? "?" : string.Empty, arrRetType.IsEnum ? ".Int32Value" : valueFetcher);
					append = string.Format ("ptr => {{\n\tusing (var num = Runtime.GetNSObject<NSNumber> (ptr)!) {{\n\t\treturn ({1}) num{0};\n\t}}\n}}", getterStr, TypeManager.FormatType (arrRetType.DeclaringType, arrRetType));
				} else
					throw GetBindAsException ("unbox", retType.Name, arrType.Name, "array", minfo.mi);
			} else if (arrType == TypeCache.NSValue && !arrIsNullable) {
				if (arrRetType.Name == "RectangleF" || arrRetType.Name == "SizeF" || arrRetType.Name == "PointF")
					valueFetcher = $"{(arrIsNullable ? "?" : string.Empty)}.{arrRetType.Name}Value";
				else if (!TypeManager.NSValueReturnMap.TryGetValue (arrRetType, out valueFetcher))
					throw GetBindAsException ("unbox", retType.Name, arrType.Name, "array", minfo.mi);

				append = string.Format ("ptr => {{\n\tusing (var val = Runtime.GetNSObject<NSValue> (ptr)!) {{\n\t\treturn val{0};\n\t}}\n}}", valueFetcher);
			} else
				throw new BindingException (1048, true, arrIsNullable ? arrRetType.Name + "?[]" : retType.Name);
		} else
			throw new BindingException (1048, true, retType.Name);
		return append;
	}

	public bool HasForcedAttribute (ICustomAttributeProvider cu, out string owns)
	{
		var att = AttributeManager.GetCustomAttribute<ForcedTypeAttribute> (cu);

		if (att is null) {
			var mi = cu as MethodInfo;
			if (mi is not null)
				att = AttributeManager.GetCustomAttribute<ForcedTypeAttribute> (mi.ReturnParameter);
		}

		if (att is null) {
			owns = "false";
			return false;
		}

		owns = att.Owns ? "true" : "false";
		return true;
	}

	//
	// MakeTrampoline: processes a delegate type and registers a TrampolineInfo with all the information
	// necessary to create trampolines that allow Objective-C blocks to call C# code, and C# code to call
	// Objective-C blocks.
	//
	// @t: A delegate type
	// 
	// This probably should use MarshalInfo to find the correct way of turning
	// the native types into managed types instead of hardcoding the limited
	// values we know about here
	//
	public TrampolineInfo MakeTrampoline (Type t)
	{
		if (trampolines.ContainsKey (t)) {
			return trampolines [t];
		}

		var mi = t.GetMethod ("Invoke");
		var pars = new List<TrampolineParameterInfo> ();
		var convert = new StringBuilder ();
		var invoke = new StringBuilder ();
		var clear = new StringBuilder ();
		var postConvert = new StringBuilder ();
		string returntype;
		var returnformat = "return {0};";

		if (mi.ReturnType.IsArray && TypeManager.IsWrappedType (mi.ReturnType.GetElementType ())) {
			returntype = NativeHandleType;
			returnformat = "return NSArray.FromNSObjects({0}).Handle;";
		} else if (TypeManager.IsWrappedType (mi.ReturnType)) {
			returntype = Generator.NativeHandleType;
			returnformat = "return {0}.GetHandle ();";
		} else if (mi.ReturnType == TypeCache.System_String) {
			returntype = Generator.NativeHandleType;
			returnformat = "return NSString.CreateNative ({0}, true);";
		} else if (GetNativeEnumToNativeExpression (mi.ReturnType, out var preExpression, out var postExpression, out var nativeType)) {
			returntype = nativeType;
			returnformat = "return " + preExpression + "{0}" + postExpression + ";";
		} else if (TypeCache.INativeObject.IsAssignableFrom (mi.ReturnType)) {
			returntype = Generator.NativeHandleType;
			returnformat = "return {0}.GetHandle ();";
		} else if (mi.ReturnType == TypeCache.System_Boolean) {
			returntype = "byte";
			returnformat = "return {0} ? (byte) 1 : (byte) 0;";
		} else {
			returntype = TypeManager.FormatType (mi.DeclaringType, mi.ReturnType);
		}

		pars.Add (new TrampolineParameterInfo ("IntPtr", "block"));
		var parameters = mi.GetParameters ();
		foreach (var pi in parameters) {
			string isForcedOwns;
			var isForced = HasForcedAttribute (pi, out isForcedOwns);

			if (pi != parameters [0])
				invoke.Append (", ");

			var safe_name = pi.Name.GetSafeParamName ();

			if (TypeManager.IsWrappedType (pi.ParameterType)) {
				pars.Add (new TrampolineParameterInfo (NativeHandleType, safe_name));
				if (IsProtocolInterface (pi.ParameterType)) {
					invoke.AppendFormat (" Runtime.GetINativeObject<{1}> ({0}, false)!", safe_name, pi.ParameterType);
				} else if (isForced) {
					invoke.AppendFormat (" Runtime.GetINativeObject<{1}> ({0}, true, {2})!", safe_name, TypeManager.RenderType (pi.ParameterType), isForcedOwns);
				} else {
					invoke.AppendFormat (" Runtime.GetNSObject<{1}> ({0})!", safe_name, TypeManager.RenderType (pi.ParameterType));
				}
				continue;
			}

			if (Frameworks.HaveCoreMedia) {
				// special case (false) so it needs to be before the _real_ INativeObject check
				if (pi.ParameterType == TypeCache.CMSampleBuffer) {
					pars.Add (new TrampolineParameterInfo (NativeHandleType, safe_name));
					if (BindThirdPartyLibrary)
						invoke.AppendFormat ("{0} == IntPtr.Zero ? null! : Runtime.GetINativeObject<CMSampleBuffer> ({0}, false)!", safe_name);
					else
						invoke.AppendFormat ("{0} == IntPtr.Zero ? null! : new CMSampleBuffer ({0}, false)", safe_name);
					continue;
				}
			}

			if (Frameworks.HaveAudioToolbox) {
				if (pi.ParameterType == TypeCache.AudioBuffers) {
					pars.Add (new TrampolineParameterInfo (NativeHandleType, safe_name));
					invoke.AppendFormat ("new global::AudioToolbox.AudioBuffers ({0})", safe_name);
					continue;
				}
			}

			if (TypeCache.INativeObject.IsAssignableFrom (pi.ParameterType)) {
				pars.Add (new TrampolineParameterInfo (NativeHandleType, safe_name));
				if (BindThirdPartyLibrary)
					invoke.AppendFormat ("Runtime.GetINativeObject<{0}> ({1}, false)!", pi.ParameterType, safe_name);
				else
					invoke.AppendFormat ("{1} == IntPtr.Zero ? null! : new {0} ({1}, false)", pi.ParameterType, safe_name);
				continue;
			}

			if (pi.ParameterType.IsByRef) {
				var nt = pi.ParameterType.GetElementType ();
				if (pi.IsOut) {
					if (nt == TypeCache.System_Boolean) {
						clear.AppendFormat ("*{0} = 0;", safe_name);
					} else {
						clear.AppendFormat ("*{0} = {1};", safe_name, nt.IsValueType ? "default (" + TypeManager.FormatType (null, nt) + ")" : "null");
					}
				}
				var outOrRef = pi.IsOut ? "out" : "ref";
				if (nt.IsValueType) {
					string fnt;
					string invoke_name;
					var nullable = TypeManager.GetUnderlyingNullableType (nt);
					if (nullable is not null) {
						fnt = TypeManager.FormatType (null, nullable);
						invoke_name = $"__xamarin_nullified__{pi.Position}";
						convert.AppendLine ($"{nullable.Name}? {invoke_name} = null;");
						convert.AppendLine ("if (value is not null)");
						convert.AppendLine ($"\t{invoke_name} =  *value;");
						postConvert.AppendLine ($"if (value is not null && {invoke_name}.HasValue)");
						postConvert.AppendLine ($"\t*value = {invoke_name}.Value;");
					} else if (nt == TypeCache.System_Boolean) {
						fnt = "byte";
						invoke_name = $"__xamarin_bool__{pi.Position}";
						convert.AppendLine ($"bool {invoke_name} = *{safe_name} != 0;");
						postConvert.AppendLine ($"*{safe_name} = {invoke_name} ? (byte) 1 : (byte) 0;");
					} else {
						fnt = TypeManager.FormatType (null, nt);
						invoke_name = $"global::System.Runtime.CompilerServices.Unsafe.AsRef<{fnt}> ({safe_name})";
					}
					pars.Add (new TrampolineParameterInfo (fnt + "*", safe_name));
					invoke.AppendFormat ("{0} {1}", outOrRef, invoke_name);
					continue;
				} else {
					var refname = $"__xamarin_pref{pi.Position}";
					convert.Append ($"var {refname} = Runtime.GetINativeObject<{TypeManager.RenderType (nt)}> ({safe_name} is not null ? *{safe_name} : NativeHandle.Zero, false)!;");
					pars.Add (new TrampolineParameterInfo ($"{NativeHandleType}*", safe_name));
					postConvert.AppendLine ($"if ({safe_name} is not null)");
					postConvert.Append ($"\t*{safe_name} = {refname}.GetHandle ();");
					invoke.Append (outOrRef);
					invoke.Append (" ");
					invoke.Append (refname);
					continue;
				}
			} else if (GetNativeEnumToManagedExpression (pi.ParameterType, out var preExpression, out var postExpression, out var nativeType)) {
				pars.Add (new TrampolineParameterInfo (nativeType, safe_name));
				invoke.Append (preExpression).Append (safe_name).Append (postExpression);
				continue;
			} else if (pi.ParameterType == TypeCache.System_Boolean) {
				pars.Add (new TrampolineParameterInfo ("byte", safe_name));
				invoke.AppendFormat ("{0} != 0", safe_name);
				continue;
			} else if (pi.ParameterType.IsValueType) {
				pars.Add (new TrampolineParameterInfo (TypeManager.FormatType (null, pi.ParameterType), safe_name));
				invoke.AppendFormat ("{0}", safe_name);
				continue;
			}

			if (pi.ParameterType == TypeCache.System_String_Array) {
				pars.Add (new TrampolineParameterInfo (NativeHandleType, safe_name));
				invoke.AppendFormat ("CFArray.StringArrayFromHandle ({0})!", safe_name);
				continue;
			}
			if (pi.ParameterType == TypeCache.System_String) {
				pars.Add (new TrampolineParameterInfo (NativeHandleType, safe_name));
				invoke.AppendFormat ("CFString.FromHandle ({0})!", safe_name);
				continue;
			}

			if (pi.ParameterType.IsArray) {
				Type et = pi.ParameterType.GetElementType ();
				if (TypeManager.IsWrappedType (et)) {
					pars.Add (new TrampolineParameterInfo (NativeHandleType, safe_name));
					invoke.AppendFormat ("CFArray.ArrayFromHandle<{0}> ({1})!", TypeManager.FormatType (null, et), safe_name);
					continue;
				}
			}

			if (pi.ParameterType.IsSubclassOf (TypeCache.System_Delegate)) {
				if (!delegate_types.ContainsKey (pi.ParameterType.Name)) {
					delegate_types [pi.ParameterType.FullName] = pi.ParameterType.GetMethod ("Invoke");
				}
				if (AttributeManager.HasAttribute<BlockCallbackAttribute> (pi)) {
					pars.Add (new TrampolineParameterInfo (NativeHandleType, safe_name));
					invoke.AppendFormat ("NID{0}.Create ({1})!", Nomenclator.GetTrampolineName (pi.ParameterType), safe_name);
					// The trampoline will eventually be generated in the final loop
				} else {
					if (!AttributeManager.HasAttribute<CCallbackAttribute> (pi)) {
						if (t.FullName.StartsWith ("System.Action`", StringComparison.Ordinal) || t.FullName.StartsWith ("System.Func`", StringComparison.Ordinal)) {
							ErrorHelper.Warning (1116, safe_name, t.FullName);
						} else {
							ErrorHelper.Warning (1115, safe_name, t.FullName);
						}
					}
					pars.Add (new TrampolineParameterInfo (NativeHandleType, safe_name));
					invoke.AppendFormat ("({0}) Marshal.GetDelegateForFunctionPointer ({1}, typeof ({0}))", pi.ParameterType, safe_name);
				}
				continue;
			}

			throw new BindingException (1001, true, pi.ParameterType.FullName);
		}

		var rt = mi.ReturnType;
		var rts = IsNativeEnum (rt) ? "var" : TypeManager.RenderType (rt);
		var trampoline_name = Nomenclator.GetTrampolineName (t);
		var functionPointerSignature = string.Join (", ", pars.Select (v => v.Type)) + ", " + returntype;
		var ti = new TrampolineInfo (userDelegate: TypeManager.FormatType (null, t),
						 delegateName: "D" + trampoline_name,
						 pars: string.Join (", ", pars.Select (v => v.Type + " " + v.ParameterName)),
									 convert: convert.ToString (),
						 invoke: invoke.ToString (),
						 returnType: returntype,
									 delegateReturnType: rts,
						 returnFormat: returnformat,
						 clear: clear.ToString (),
									 postConvert: postConvert.ToString (),
						 type: t,
						 functionPointerSignature: functionPointerSignature);


		ti.UserDelegateTypeAttribute = TypeManager.FormatType (null, t);
		trampolines [t] = ti;

		return ti;
	}

	//
	// Returns the actual way in which the type t must be marshalled
	// for example "UIView foo" is generated as  "foo.Handle"
	//
	public string MarshalParameter (MethodInfo mi, ParameterInfo pi, bool null_allowed_override, PropertyInfo propInfo = null, bool castEnum = true, StringBuilder convs = null)
	{
		if (pi.ParameterType.IsByRef && pi.ParameterType.GetElementType ().IsValueType == false)
			return "&" + pi.Name + "Value";

		if (HasBindAsAttribute (pi))
			return string.Format ("nsb_{0}.GetHandle ()", pi.Name);
		if (propInfo is not null && HasBindAsAttribute (propInfo))
			return string.Format ("nsb_{0}.GetHandle ()", propInfo.Name);

		var safe_name = pi.Name.GetSafeParamName ();

		if (TypeManager.IsWrappedType (pi.ParameterType))
			return safe_name + "__handle__";

		if (GetNativeEnumToNativeExpression (pi.ParameterType, out var preExpression, out var postExpression, out var nativeType))
			return preExpression + safe_name + postExpression;

		if (castEnum && pi.ParameterType.IsEnum)
			return "(" + PrimitiveType (pi.ParameterType) + ")" + safe_name;

		if (castEnum && pi.ParameterType.Namespace == "System") {
			if (pi.ParameterType.Name == "nint")
				return "(IntPtr) " + safe_name;
			else if (pi.ParameterType.Name == "nuint")
				return "(UIntPtr) " + safe_name;
		}

		if (TypeManager.IsNativeType (pi.ParameterType))
			return safe_name;

		if (pi.ParameterType == TypeCache.System_String) {
			var mai = new MarshalInfo (this, mi, pi);
			if (mai.PlainString)
				return safe_name;
			else {
				bool allow_null = null_allowed_override || AttributeManager.HasAttribute<NullAllowedAttribute> (pi);

				if (mai.ZeroCopyStringMarshal) {
					if (allow_null)
						return String.Format ("{0} is null ? IntPtr.Zero : (IntPtr)(&_s{0})", pi.Name);
					else
						return String.Format ("(IntPtr)(&_s{0})", pi.Name);
				} else {
					return "ns" + pi.Name;
				}
			}
		}

		if (pi.ParameterType.IsValueType) {
			if (pi.ParameterType == TypeCache.System_Boolean)
				return safe_name + " ? (byte) 1 : (byte) 0";
			return safe_name;
		}

		MarshalType mt;
		if (marshalTypes.TryGetMarshalType (pi.ParameterType, out mt)) {
			if (null_allowed_override || AttributeManager.HasAttribute<NullAllowedAttribute> (pi))
				return safe_name + "__handle__";
			return String.Format (mt.ParameterMarshal, safe_name);
		}

		if (pi.ParameterType.IsArray) {
			//Type etype = pi.ParameterType.GetElementType ();

			if (null_allowed_override || AttributeManager.HasAttribute<NullAllowedAttribute> (pi))
				return String.Format ("nsa_{0}.GetHandle ()", pi.Name);
			return "nsa_" + pi.Name + ".Handle";
		}

		//
		// Handle (out ValueType foo)
		//
		if (pi.ParameterType.IsByRef) {
			var et = pi.ParameterType.GetElementType ();
			var nullable = TypeManager.GetUnderlyingNullableType (et);
			if (nullable is not null) {
				return $"converted_{safe_name}";
			} else if (et.IsValueType) {
				if (et == TypeCache.System_Boolean) {
					if (pi.IsOut)
						convs!.Append ($"{safe_name} = false;");
					return $"(byte*) global::System.Runtime.CompilerServices.Unsafe.AsPointer<bool> (ref {safe_name})";
				}
				var blittableType = TypeManager.FormatType (mi.DeclaringType, et);
				if (pi.IsOut)
					convs!.Append ($"{safe_name} = default ({blittableType});");
				return $"({blittableType}*) global::System.Runtime.CompilerServices.Unsafe.AsPointer<{blittableType}> (ref {safe_name})";
			}
			return (pi.IsOut ? "out " : "ref ") + safe_name;
		}

		// Handle 'ValueType* foo'
		if (pi.ParameterType.IsPointer) {
			var et = pi.ParameterType.GetElementType ();
			if (et.IsValueType)
				return safe_name;
		}

		if (pi.ParameterType.IsSubclassOf (TypeCache.System_Delegate)) {
			return String.Format ("(IntPtr) block_ptr_{0}", pi.Name);
		}

		if (TypeManager.IsDictionaryContainerType (pi.ParameterType)) {
			if (null_allowed_override || AttributeManager.HasAttribute<NullAllowedAttribute> (pi))
				return String.Format ("{0} is null ? NativeHandle.Zero : {0}.Dictionary.Handle", safe_name);
			return safe_name + ".Dictionary.Handle";
		}

		if (pi.ParameterType.IsGenericParameter) {
			if (null_allowed_override || AttributeManager.HasAttribute<NullAllowedAttribute> (pi))
				return string.Format ("{0}.GetHandle ()", safe_name);
			return safe_name + ".Handle";
		}

		// This means you need to add a new MarshalType in the method "Go"
		throw new BindingException (1002, true, pi.ParameterType.FullName, mi.DeclaringType.FullName, mi.Name.GetSafeParamName ());
	}

	public bool ParameterNeedsNullCheck (ParameterInfo pi, MethodInfo mi, PropertyInfo propInfo = null)
	{
		if (pi.ParameterType.IsByRef)
			return false;

		if (AttributeManager.HasAttribute<NullAllowedAttribute> (pi))
			return false;

		if (IsSetter (mi)) {
			if (AttributeManager.HasAttribute<NullAllowedAttribute> (mi)) {
				return false;
			}
			if ((propInfo is not null) && AttributeManager.HasAttribute<NullAllowedAttribute> (propInfo)) {
				return false;
			}
		}

		var bindAsAtt = GetBindAsAttribute (pi) ?? GetBindAsAttribute (propInfo);
		if (bindAsAtt is not null)
			return bindAsAtt.IsNullable (this) || !bindAsAtt.IsValueType (this);

		if (TypeManager.IsWrappedType (pi.ParameterType))
			return true;

		return !pi.ParameterType.IsValueType;
	}

	public BindAttribute GetBindAttribute (MethodInfo mi)
	{
		return AttributeManager.GetCustomAttribute<BindAttribute> (mi);
	}

	public bool ShouldMarshalNativeExceptions (MethodInfo mi)
	{
		// [MarshalNativeExceptions] should work on a property and inside the get / set
		// If we have it directly on our method, good enough
		if (AttributeManager.HasAttribute<MarshalNativeExceptionsAttribute> (mi))
			return true;

		// Else look up to see if we are part of a property and look for the attribute there
		PropertyInfo owningProperty = mi.DeclaringType.GetProperties ()
			.FirstOrDefault (prop => prop.GetSetMethod () == mi ||
					prop.GetGetMethod () == mi);
		if (owningProperty is not null && AttributeManager.HasAttribute<MarshalNativeExceptionsAttribute> (owningProperty))
			return true;

		return false;
	}

	public bool IsTarget (ParameterInfo pi)
	{
		var is_target = AttributeManager.HasAttribute<TargetAttribute> (pi);
		if (is_target) {
			throw new BindingException (1031, true,
				pi.Member.DeclaringType.FullName, pi.Member.Name.GetSafeParamName ());
		}
		return is_target;
	}

	//
	// Makes the method name for a objcSend call
	//
	string MakeSig (string send, bool stret, MethodInfo mi, bool aligned)
	{
		var sb = new StringBuilder ();
		var shouldMarshalNativeExceptions = ShouldMarshalNativeExceptions (mi);
		var marshalDirective = AttributeManager.GetCustomAttribute<MarshalDirectiveAttribute> (mi);

		if (!string.IsNullOrEmpty (marshalDirective?.NativePrefix))
			sb.Append (marshalDirective.NativePrefix);
		else if (shouldMarshalNativeExceptions)
			sb.Append ("xamarin_");

		try {
			sb.Append (ParameterGetMarshalType (new MarshalInfo (this, mi) { IsAligned = aligned }));
		} catch (BindingException ex) {
			throw new BindingException (1078, ex.Error, ex, ex.Message, mi.Name);
		}

		sb.Append ("_");
		sb.Append (send);
		if (stret)
			sb.Append ("_stret");

		foreach (var pi in mi.GetParameters ()) {
			if (IsTarget (pi))
				continue;
			sb.Append ("_");
			try {
				sb.Append (ParameterGetMarshalType (new MarshalInfo (this, mi, pi)).Replace (' ', '_').Replace ('*', '_'));
			} catch (BindingException ex) {
				throw new BindingException (1079, ex.Error, ex, ex.Message, pi.Name.GetSafeParamName (), mi.DeclaringType, mi.Name);
			}
		}

		if (shouldMarshalNativeExceptions)
			sb.Append ("_exception");

		if (!string.IsNullOrEmpty (marshalDirective?.NativeSuffix))
			sb.Append (marshalDirective.NativeSuffix);

		return sb.ToString ();
	}

	void RegisterMethod (bool need_stret, MethodInfo mi, string method_name, bool aligned)
	{
		if (send_methods.ContainsKey (method_name))
			return;
		send_methods [method_name] = method_name;

		var b = new StringBuilder ();
		int n = 0;

		foreach (var pi in mi.GetParameters ()) {
			if (IsTarget (pi))
				continue;

			b.Append (", ");

			try {
				var parameterType = ParameterGetMarshalType (new MarshalInfo (this, mi, pi), true);
				if (parameterType == "bool") {
					b.Append ("byte");
				} else if (parameterType == "out bool" || parameterType == "ref bool") {
					b.Append ("byte* ");
				} else if (parameterType == "char") {
					b.Append ("ushort");
				} else if (parameterType == "out char" || parameterType == "ref char") {
					b.Append ("ushort*");
				} else {
					b.Append (parameterType);
				}
			} catch (BindingException ex) {
				throw new BindingException (1079, ex.Error, ex, ex.Message, pi.Name.GetSafeParamName (), mi.DeclaringType, mi.Name);
			}
			b.Append (" ");
			b.Append ("arg" + (++n));
		}

		if (ShouldMarshalNativeExceptions (mi))
			b.Append (", IntPtr* exception_gchandle");

		string entry_point;
		if (method_name.IndexOf ("objc_msgSendSuper", StringComparison.Ordinal) != -1) {
			entry_point = need_stret ? "objc_msgSendSuper_stret" : "objc_msgSendSuper";
		} else
			entry_point = need_stret ? "objc_msgSend_stret" : "objc_msgSend";

		var marshalDirective = AttributeManager.GetCustomAttribute<MarshalDirectiveAttribute> (mi);
		if (marshalDirective is not null && marshalDirective.Library is not null) {
			print (m, "\t\t[DllImport (\"{0}\", EntryPoint=\"{1}\")]", marshalDirective.Library, method_name);
		} else if (method_name.StartsWith ("xamarin_", StringComparison.Ordinal)) {
			print (m, "\t\t[DllImport (\"__Internal\", EntryPoint=\"{0}\")]", method_name);
		} else {
			print (m, "\t\t[DllImport (LIBOBJC_DYLIB, EntryPoint=\"{0}\")]", entry_point);
		}

		var returnType = need_stret ? "void" : ParameterGetMarshalType (new MarshalInfo (this, mi), true);
		if (returnType == "bool")
			returnType = "byte";
		else if (returnType == "char")
			returnType = "ushort";

		print (m, "\t\tpublic unsafe extern static {0} {1} ({3}IntPtr receiver, IntPtr selector{2});",
			   returnType, method_name, b.ToString (),
			   need_stret ? (aligned ? "IntPtr" : TypeManager.FormatTypeUsedIn ("ObjCRuntime", mi.ReturnType)) + "* retval, " : "");
	}

	bool IsNativeEnum (Type type)
	{
		return type.IsEnum && AttributeManager.HasAttribute<NativeAttribute> (type);
	}

	// Returns two strings that are used to convert from a native (nint/nuint) value to a managed ([Native]) enum value
	// nativeType: nint/nuint
	bool GetNativeEnumToManagedExpression (Type enumType, out string preExpression, out string postExpression, out string nativeType, StringBuilder postproc = null)
	{
		preExpression = null;
		postExpression = null;
		nativeType = null;

		if (!enumType.IsEnum)
			return false;

		var attrib = AttributeManager.GetCustomAttribute<NativeAttribute> (enumType);
		if (attrib is null)
			return false;

		var renderedEnumType = TypeManager.RenderType (enumType);
		var underlyingEnumType = enumType.GetEnumUnderlyingType ();
		var underlyingTypeName = TypeManager.RenderType (underlyingEnumType);
		string itype;
		string intermediateType;
		object maxValue;
		Func<FieldInfo, bool> isMaxDefinedFunc;
		Func<FieldInfo, bool> isMinDefinedFunc = null;
		if (TypeCache.System_Int64 == underlyingEnumType) {
			nativeType = "IntPtr";
			intermediateType = "nint";
			itype = "int";
			maxValue = long.MaxValue;
			isMaxDefinedFunc = (v) => (long) v.GetRawConstantValue () == long.MaxValue;
			isMinDefinedFunc = (v) => (long) v.GetRawConstantValue () == long.MinValue;
		} else if (TypeCache.System_UInt64 == underlyingEnumType) {
			nativeType = "UIntPtr";
			intermediateType = "nuint";
			itype = "uint";
			maxValue = ulong.MaxValue;
			isMaxDefinedFunc = (v) => (ulong) v.GetRawConstantValue () == ulong.MaxValue;
		} else {
			throw new BindingException (1029, enumType);
		}

		if (!string.IsNullOrEmpty (attrib.ConvertToManaged)) {
			preExpression = attrib.ConvertToManaged + " ((" + intermediateType + ") ";
			postExpression = ")";
		} else {
			preExpression = "(" + renderedEnumType + ") (" + TypeManager.RenderType (underlyingEnumType) + ") ";
			postExpression = string.Empty;
		}

		// Check if we got UInt32.MaxValue, which should probably be UInt64.MaxValue (if the enum
		// in question actually has that value at least). Same goes for Int32.MinValue/Int64.MinValue.
		// var isDefined = enumType.IsEnumDefined (maxValue);
		var definedMaxField = enumType.GetFields ().Where (v => v.IsLiteral).FirstOrDefault (isMaxDefinedFunc);
		if (definedMaxField is not null && postproc is not null) {
			postproc.AppendLine ("#if ARCH_32");
			postproc.AppendFormat ("if (({0}) ret == ({0}) {1}.MaxValue)\n", underlyingTypeName, itype);
			postproc.AppendFormat ("\tret = {0}.{1}; // = {2}.MaxValue\n", renderedEnumType, definedMaxField.Name, underlyingTypeName);
			if (underlyingEnumType == TypeCache.System_Int64) {
				var definedMinField = enumType.GetFields ().Where (v => v.IsLiteral).FirstOrDefault (isMinDefinedFunc);
				if (definedMinField is not null) {
					postproc.AppendFormat ("else if (({0}) ret == ({0}) {1}.MinValue)\n", underlyingTypeName, itype);
					postproc.AppendFormat ("\tret = {0}.{1}; // = {2}.MinValue\n", renderedEnumType, definedMinField.Name, underlyingTypeName);
				}
			}
			postproc.AppendLine ("#endif");
		}

		return true;
	}

	// Returns two strings that are used to convert from a managed enum value to a native nint/nuint.
	// nativeType: nint/nuint
	bool GetNativeEnumToNativeExpression (Type enumType, out string preExpression, out string postExpression, out string nativeType)
	{
		preExpression = null;
		postExpression = null;
		nativeType = null;

		if (!enumType.IsEnum)
			return false;

		var attrib = AttributeManager.GetCustomAttribute<NativeAttribute> (enumType);
		if (attrib is null)
			return false;

		var underlyingEnumType = enumType.GetEnumUnderlyingType ();
		if (TypeCache.System_Int64 == underlyingEnumType) {
			nativeType = "IntPtr";
		} else if (TypeCache.System_UInt64 == underlyingEnumType) {
			nativeType = "UIntPtr";
		} else {
			throw new BindingException (1029, enumType);
		}

		if (!string.IsNullOrEmpty (attrib.ConvertToNative)) {
			preExpression = "(" + nativeType + ") " + attrib.ConvertToNative + " (";
			postExpression = ")";
		} else {
			preExpression = "(" + nativeType + ") (" + TypeManager.RenderType (underlyingEnumType) + ") ";
			postExpression = string.Empty;
		}

		return true;
	}

	bool IsNativeEnum (Type enumType, out Type underlyingType, out string nativeType)
	{
		underlyingType = null;
		nativeType = null;

		if (!enumType.IsEnum)
			return false;

		var attrib = AttributeManager.GetCustomAttribute<NativeAttribute> (enumType);
		if (attrib is null)
			return false;

		underlyingType = enumType.GetEnumUnderlyingType ();
		if (underlyingType == TypeCache.System_Int64) {
			nativeType = "IntPtr";
		} else if (underlyingType == TypeCache.System_UInt64) {
			nativeType = "UIntPtr";
		} else {
			throw new BindingException (1026, true, enumType.FullName, "NativeAttribute");
		}

		return true;
	}

	void DeclareInvoker (MethodInfo mi)
	{
		if (AttributeManager.HasAttribute<WrapAttribute> (mi))
			return;

		// arm64 never requires stret, so we'll always need the non-stret variants
		RegisterMethod (false, mi, MakeSig (mi, false), false);
		RegisterMethod (false, mi, MakeSuperSig (mi, false), false);

		if (CheckNeedStret (mi)) {
			RegisterMethod (true, mi, MakeSig (mi, true), false);
			RegisterMethod (true, mi, MakeSuperSig (mi, true), false);

			if (AttributeManager.HasAttribute<AlignAttribute> (mi)) {
				RegisterMethod (true, mi, MakeSig (mi, true, true), true);
				RegisterMethod (true, mi, MakeSuperSig (mi, true, true), true);
			}
		}
	}
	static char [] invalid_selector_chars = new char [] { '*', '^', '(', ')' };

	public ExportAttribute GetExportAttribute (MemberInfo mo)
	{
		string dummy;
		return GetExportAttribute (mo, out dummy);
	}

	//
	// Either we have an [Export] attribute, or we have a [Wrap] one
	//
	public ExportAttribute GetExportAttribute (MemberInfo mo, out string wrap)
	{
		wrap = null;
		object [] attrs = AttributeManager.GetCustomAttributes<ExportAttribute> (mo);
		if (attrs.Length == 0) {
			attrs = AttributeManager.GetCustomAttributes<WrapAttribute> (mo);
			if (attrs.Length != 0) {
				wrap = ((WrapAttribute) attrs [0]).MethodName;
				return null;
			}
			PropertyInfo pi = mo as PropertyInfo;
			if (pi is not null && pi.CanRead) {
				var getter = pi.GetGetMethod (true);
				attrs = AttributeManager.GetCustomAttributes<ExportAttribute> (getter);
			}
			if (attrs.Length == 0)
				return null;
		}

		var export = (ExportAttribute) attrs [0];

		if (string.IsNullOrEmpty (export.Selector))
			throw new BindingException (1024, true, mo.DeclaringType.FullName, mo.Name);

		if (export.Selector.IndexOfAny (invalid_selector_chars) != -1) {
			Console.Error.WriteLine ("Export attribute contains invalid selector name: {0}", export.Selector);
			Environment.Exit (1);
		}

		return export;
	}

	public ExportAttribute GetSetterExportAttribute (PropertyInfo pinfo)
	{
		var ea = AttributeManager.GetCustomAttribute<ExportAttribute> (pinfo.GetSetMethod ());
		if (ea is not null && ea.Selector is not null)
			return ea;
		return AttributeManager.GetCustomAttribute<ExportAttribute> (pinfo)?.ToSetter (pinfo);
	}

	public ExportAttribute GetGetterExportAttribute (PropertyInfo pinfo)
	{
		var ea = AttributeManager.GetCustomAttribute<ExportAttribute> (pinfo.GetGetMethod ());
		if (ea is not null && ea.Selector is not null)
			return ea;
		return AttributeManager.GetCustomAttribute<ExportAttribute> (pinfo).ToGetter (pinfo);
	}

	public Generator (BindingTouch binding_touch, bool is_public_mode, bool external, bool debug, Type [] types, Type [] strong_dictionaries)
	{
		BindingTouch = binding_touch;
		IsPublicMode = is_public_mode;
		this.external = external;
		this.debug = debug;
		this.types = types;
		this.strong_dictionaries = strong_dictionaries;
		basedir = ".";
		NativeHandleType = binding_touch.IsDotNet ? "NativeHandle" : "IntPtr";
	}

	public void Go ()
	{
		GeneratedTypes = new GeneratedTypes (this);
		marshalTypes.Load (TypeCache, Frameworks);

		m = GetOutputStream ("ObjCRuntime", "Messaging");
		Header (m);
		print (m, "namespace {0} {{", NamespaceCache.ObjCRuntime);
		print (m, "\tstatic partial class Messaging {");

		if (BindThirdPartyLibrary) {
			print (m, "\t\tinternal const string LIBOBJC_DYLIB = \"/usr/lib/libobjc.dylib\";\n");
			print (m, "\t\tstatic internal System.Reflection.Assembly this_assembly = typeof (Messaging).Assembly;\n");
			// IntPtr_objc_msgSend[Super]: for init
			print (m, "\t\t[DllImport (LIBOBJC_DYLIB, EntryPoint=\"objc_msgSend\")]");
			print (m, "\t\tpublic extern static IntPtr IntPtr_objc_msgSend (IntPtr receiever, IntPtr selector);");
			send_methods ["IntPtr_objc_msgSend"] = "IntPtr_objc_msgSend";
			print (m, "\t\t[DllImport (LIBOBJC_DYLIB, EntryPoint=\"objc_msgSendSuper\")]");
			print (m, "\t\tpublic extern static IntPtr IntPtr_objc_msgSendSuper (IntPtr receiever, IntPtr selector);");
			send_methods ["IntPtr_objc_msgSendSuper"] = "IntPtr_objc_msgSendSuper";
			// IntPtr_objc_msgSendSuper_IntPtr: for initWithCoder:
			print (m, "\t\t[DllImport (LIBOBJC_DYLIB, EntryPoint=\"objc_msgSend\")]");
			print (m, "\t\tpublic extern static IntPtr IntPtr_objc_msgSend_IntPtr (IntPtr receiever, IntPtr selector, IntPtr arg1);");
			send_methods ["IntPtr_objc_msgSend_IntPtr"] = "IntPtr_objc_msgSend_IntPtr";
			print (m, "\t\t[DllImport (LIBOBJC_DYLIB, EntryPoint=\"objc_msgSendSuper\")]");
			print (m, "\t\tpublic extern static IntPtr IntPtr_objc_msgSendSuper_IntPtr (IntPtr receiever, IntPtr selector, IntPtr arg1);");
			send_methods ["IntPtr_objc_msgSendSuper_IntPtr"] = "IntPtr_objc_msgSendSuper_IntPtr";
		}

		Array.Sort (types, (a, b) => string.CompareOrdinal (a.FullName, b.FullName));
		TypeManager.SetTypesThatMustAlwaysBeGloballyNamed (types);

		foreach (Type t in types) {
			if (t.IsUnavailable (this))
				continue;

			// We call lookup to build the hierarchy graph
			GeneratedTypes.Lookup (t);

			var tselectors = new List<string> ();

			foreach (var pi in GetTypeContractProperties (t)) {
				if (pi.IsUnavailable (this))
					continue;

				if (AttributeManager.HasAttribute<IsThreadStaticAttribute> (pi) && !AttributeManager.HasAttribute<StaticAttribute> (pi))
					throw new BindingException (1008, true);

				string wrapname;
				var export = GetExportAttribute (pi, out wrapname);
				if (export is null) {
					if (wrapname is not null)
						continue;

					// Let properties with the [Field] attribute through as well.
					if (AttributeManager.HasAttribute<FieldAttribute> (pi))
						continue;


					if (AttributeManager.HasAttribute<CoreImageFilterPropertyAttribute> (pi))
						continue;

					// Ensure there's a [Wrap] on either (or both) the getter and setter - since we already know there's no [Export]
					var getMethod = pi.GetGetMethod ();
					var hasWrapGet = getMethod is not null && AttributeManager.HasAttribute<WrapAttribute> (getMethod);
					var setMethod = pi.GetSetMethod ();
					var hasWrapSet = setMethod is not null && AttributeManager.HasAttribute<WrapAttribute> (setMethod);
					if (hasWrapGet || hasWrapSet)
						continue;

					throw new BindingException (1018, true, t.FullName, pi.Name);
				}

#if NET
				var is_abstract = false;
#else
				bool is_abstract = AttributeManager.HasAttribute<AbstractAttribute> (pi) && pi.DeclaringType == t;
#endif

				if (pi.CanRead) {
					MethodInfo getter = pi.GetGetMethod ();
					BindAttribute ba = GetBindAttribute (getter);

					if (!is_abstract)
						tselectors.Add (ba is not null ? ba.Selector : export.Selector);
					DeclareInvoker (getter);
				}

				if (pi.CanWrite) {
					MethodInfo setter = pi.GetSetMethod ();
					BindAttribute ba = GetBindAttribute (setter);
					var notImpl = AttributeManager.HasAttribute<NotImplementedAttribute> (setter);

					if (!is_abstract && !notImpl)
						tselectors.Add (ba is not null ? ba.Selector : GetSetterExportAttribute (pi).Selector);
					DeclareInvoker (setter);
				}
			}

			foreach (var mi in GetTypeContractMethods (t)) {
				// Skip properties
				if (mi.IsSpecialName)
					continue;

				if (mi.IsUnavailable (this))
					continue;

				bool seenAbstract = false;
				bool seenDefaultValue = false;
				bool seenNoDefaultValue = false;

				foreach (var attr in AttributeManager.GetCustomAttributes<System.Attribute> (mi)) {
					string selector = null;
					ExportAttribute ea = attr as ExportAttribute;
					BindAttribute ba = attr as BindAttribute;
					if (ea is not null) {
						selector = ea.Selector;
					} else if (ba is not null) {
						selector = ba.Selector;
					} else if (attr is StaticAttribute) {
						continue;
					} else if (attr is InternalAttribute || attr is UnifiedInternalAttribute || attr is ProtectedAttribute) {
						continue;
					} else if (attr is NeedsAuditAttribute) {
						continue;
					} else if (attr is FactoryAttribute) {
						continue;
					} else if (attr is AbstractAttribute) {
						if (mi.DeclaringType == t)
							need_abstract [t] = true;
						seenAbstract = true;
						continue;
					} else if (attr is DefaultValueAttribute || attr is DefaultValueFromArgumentAttribute) {
						seenDefaultValue = true;
						continue;
					} else if (attr is NoDefaultValueAttribute) {
						seenNoDefaultValue = true;
						continue;
					} else if (attr is SealedAttribute || attr is EventArgsAttribute || attr is DelegateNameAttribute || attr is EventNameAttribute || attr is IgnoredInDelegateAttribute || attr is ObsoleteAttribute || attr is NewAttribute || attr is PostGetAttribute || attr is NullAllowedAttribute || attr is CheckDisposedAttribute || attr is SnippetAttribute || attr is AppearanceAttribute || attr is ThreadSafeAttribute || attr is AutoreleaseAttribute || attr is EditorBrowsableAttribute || attr is AdviceAttribute || attr is OverrideAttribute || attr is DelegateApiNameAttribute || attr is ForcedTypeAttribute)
						continue;
					else if (attr is MarshalNativeExceptionsAttribute)
						continue;
					else if (attr is WrapAttribute)
						continue;
					else if (attr is AsyncAttribute)
						continue;
					else if (attr is DesignatedInitializerAttribute)
						continue;
					else if (attr is DesignatedDefaultCtorAttribute)
						continue;
					else if (attr is AvailabilityBaseAttribute)
						continue;
					else if (attr is RequiresSuperAttribute)
						continue;
					else if (attr is NoMethodAttribute)
						continue;
					else {
						switch (attr.GetType ().Name) {
						case "PreserveAttribute":
						case "CompilerGeneratedAttribute":
						case "ManualAttribute":
						case "MarshalDirectiveAttribute":
						case "BindingImplAttribute":
						case "XpcInterfaceAttribute":
						case "NativeIntegerAttribute":
							continue;
						default:
							throw new BindingException (1007, true, attr.GetType (), mi.DeclaringType, mi.Name);
						}
					}

					if (selector is null)
						throw new BindingException (1009, true, mi.DeclaringType, mi.Name);

					tselectors.Add (selector);
					if (selector_use.ContainsKey (selector)) {
						selector_use [selector]++;
					} else
						selector_use [selector] = 1;
				}

				if (seenNoDefaultValue && seenAbstract)
					throw new BindingException (1019, true, mi.DeclaringType, mi.Name);
				else if (seenNoDefaultValue && seenDefaultValue)
					throw new BindingException (1019, true, mi.DeclaringType, mi.Name);

				DeclareInvoker (mi);
			}

			foreach (var pi in t.GatherProperties (BindingFlags.Instance | BindingFlags.Public, this)) {
				if (pi.IsUnavailable (this))
					continue;

				if (AttributeManager.HasAttribute<AbstractAttribute> (pi) && pi.DeclaringType == t)
					need_abstract [t] = true;
			}

			selectors [t] = tselectors.Distinct ();
		}

		foreach (Type t in types) {
			if (t.IsUnavailable (this))
				continue;

			Generate (t);
		}

		//DumpChildren (0, GeneratedType.Lookup (TypeCache.NSObject));

		print (m, "\t}\n}");
		m.Close ();

		// Generate strong argument types
		GenerateStrongDictionaryTypes ();

		// Generate the event arg mappings
		if (notification_event_arg_types.Count > 0)
			GenerateEventArgsFile ();

		if (delegate_types.Count > 0)
			GenerateIndirectDelegateFile ();

		if (trampolines.Count > 0)
			GenerateTrampolines ();

		if (libraries.Count > 0)
			GenerateLibraryHandles ();

		ThrowIfExceptions ();
	}

	void ThrowIfExceptions ()
	{
		if (exceptions.Count > 0)
			throw new AggregateException (exceptions);
	}

	static string GenerateNSValue (string propertyToCall)
	{
		return "using (var nsv = Runtime.GetNSObject<NSValue> (value)!)\n\treturn nsv." + propertyToCall + ";";
	}

	static string GenerateNSNumber (string cast, string propertyToCall)
	{
		return "using (var nsn = Runtime.GetNSObject<NSNumber> (value)!)\n\treturn " + cast + "nsn." + propertyToCall + ";";
	}

	void GenerateIndirectDelegateFile ()
	{
		sw = GetOutputStream (null, "SupportDelegates");
		Header (sw);
		RenderDelegates (delegate_types);
		sw.Close ();
	}

	void GenerateTrampolines ()
	{
		sw = GetOutputStream ("ObjCRuntime", "Trampolines");

		Header (sw);
		print ("namespace ObjCRuntime {"); indent++;
		print ("");
		print_generated_code ();
		print ("static partial class Trampolines {"); indent++;

		while (trampolines.Count > 0) {
			var queue = trampolines.Values.ToArray ();
			trampolines.Clear ();

			GenerateTrampolinesForQueue (queue);
		}
		indent--; print ("}");
		indent--; print ("}");
		sw.Close ();
	}

	Dictionary<Type, bool> generated_trampolines = new Dictionary<Type, bool> ();

	void GenerateTrampolinesForQueue (TrampolineInfo [] queue)
	{
		Array.Sort (queue, (a, b) => string.CompareOrdinal (a.Type.FullName, b.Type.FullName));
		foreach (var ti in queue) {
			if (generated_trampolines.ContainsKey (ti.Type))
				continue;
			generated_trampolines [ti.Type] = true;

			var mi = ti.Type.GetMethod ("Invoke");
			var parameters = mi.GetParameters ();

			print ("");
			print ("[UnmanagedFunctionPointerAttribute (CallingConvention.Cdecl)]");
			print ("[UserDelegateType (typeof ({0}))]", ti.UserDelegate);
			print ("unsafe internal delegate {0} {1} ({2});", ti.ReturnType, ti.DelegateName, ti.Parameters);
			print ("");
			print ("//\n// This class bridges native block invocations that call into C#\n//");
			print ("static internal class {0} {{", ti.StaticName); indent++;
			// it can't be conditional without fixing https://github.com/mono/linker/issues/516
			// but we have a workaround in place because we can't fix old, binary bindings so...
			// print ("[Preserve (Conditional=true)]");
			// For .NET we fix it using the DynamicDependency attribute below
#if !NET
			print ("unsafe static internal readonly {0} Handler = Invoke;", ti.DelegateName);
			print ("");
#endif
#if NET
			print ("[Preserve (Conditional = true)]");
			print ("[UnmanagedCallersOnly]");
#else
			print ("[MonoPInvokeCallback (typeof ({0}))]", ti.DelegateName);
#endif
			print ("internal static unsafe {0} Invoke ({1}) {{", ti.ReturnType, ti.Parameters);
			indent++;
			print ("var del = BlockLiteral.GetTarget<{0}> (block);", ti.UserDelegate);
			bool is_void = ti.ReturnType == "void";
			// FIXME: right now we only support 'null' when the delegate does not return a value
			// otherwise we will need to know the default value to be returned (likely uncommon)
			if (is_void) {
				print ("if (del is not null) {");
				indent++;
				if (ti.Convert.Length > 0)
					print (ti.Convert);
				print ("del ({0});", ti.Invoke);
				if (ti.PostConvert.Length > 0)
					print (ti.PostConvert);
				indent--;
				if (ti.Clear.Length > 0) {
					print ("} else {");
					indent++;
					print (ti.Clear);
					indent--;
				}
				print ("}");
			} else {
				if (ti.Convert.Length > 0)
					print (ti.Convert);
				print ("{0} retval = del ({1});", ti.DelegateReturnType, ti.Invoke);
				if (ti.PostConvert.Length > 0)
					print (ti.PostConvert);
				print (ti.ReturnFormat, "retval");
			}
			indent--;
			print ("}");
			print ("");
			print ("internal static unsafe BlockLiteral CreateNullableBlock ({0}? callback)", ti.UserDelegate);
			print ("{");
			indent++;
			print ("if (callback is null)");
			indent++;
			print ("return default (BlockLiteral);");
			indent--;
			print ("return CreateBlock (callback);");
			indent--;
			print ("}");
			print ("");
			print ("[BindingImpl (BindingImplOptions.GeneratedCode | BindingImplOptions.Optimizable)]");
			print ("internal static unsafe BlockLiteral CreateBlock ({0} callback)", ti.UserDelegate);
			print ("{");
			indent++;
#if NET
			print ("delegate* unmanaged<{0}> trampoline = &Invoke;", ti.FunctionPointerSignature);
			print ("return new BlockLiteral (trampoline, callback, typeof ({0}), nameof (Invoke));", ti.StaticName);
#else
			print ("var block = new BlockLiteral ();");
			print ("block.SetupBlockUnsafe (Handler, callback);");
			print ("return block;");
#endif
			indent--;
			print ("}");
			indent--;
			print ("}} /* class {0} */", ti.StaticName);

			//
			// Now generate the class that allows us to invoke a Objective-C block from C#
			//
			print ("");
			print ($"internal sealed class {ti.NativeInvokerName} : TrampolineBlockBase {{");
			indent++;
			print ("{0} invoker;", ti.DelegateName);
			print ("");
			print_generated_code ();
			print ("public unsafe {0} (BlockLiteral *block) : base (block)", ti.NativeInvokerName);
			print ("{"); indent++;
			print ("invoker = block->GetDelegateForBlock<{0}> ();", ti.DelegateName);
			indent--; print ("}");
			print ("");
			print ("[Preserve (Conditional=true)]");
			print_generated_code ();
			print ("public unsafe static {0}? Create (IntPtr block)\n{{", ti.UserDelegate); indent++;
			print ("if (block == IntPtr.Zero)"); indent++;
			print ("return null;"); indent--;
			print ($"var del = ({ti.UserDelegate}) GetExistingManagedDelegate (block);");
			print ($"return del ?? new {ti.NativeInvokerName} ((BlockLiteral *) block).Invoke;");
			indent--; print ("}");
			print ("");
			var string_pars = new StringBuilder ();
			MakeSignatureFromParameterInfo (false, string_pars, mi, declaringType: null, parameters: parameters);
			print_generated_code ();
			print ("unsafe {0} Invoke ({1})", TypeManager.FormatType (null, mi.ReturnType), string_pars.ToString ());
			print ("{"); indent++;
			string cast_a = "", cast_b = "";
			bool use_temp_return;

			GenerateArgumentChecks (mi, true);

			StringBuilder args, convs, disposes, by_ref_processing, by_ref_init;
			GenerateTypeLowering (mi,
						  null_allowed_override: true,
						  castEnum: false,
						  args: out args,
						  convs: out convs,
						  disposes: out disposes,
						  by_ref_processing: out by_ref_processing,
						  by_ref_init: out by_ref_init);

			if (by_ref_init.Length > 0)
				print (by_ref_init.ToString ());

			use_temp_return = mi.ReturnType != TypeCache.System_Void;
			if (use_temp_return)
				GetReturnsWrappers (mi, null, null, out cast_a, out cast_b);

			if (convs.Length > 0)
				print (convs.ToString ());
			print ("{0}{1}invoker (BlockPointer{2}){3};",
				   use_temp_return ? "var ret = " : "",
				   cast_a,
				   args.ToString (),
				   cast_b);
			if (disposes.Length > 0)
				print (disposes.ToString ());
			if (by_ref_processing.Length > 0)
				print (sw, by_ref_processing.ToString ());
			if (use_temp_return) {
				if (mi.ReturnType == TypeCache.System_Boolean) {
					print ("return ret != 0;");
				} else {
					print ("return ret!;");
				}
			}
			indent--; print ("}");
			indent--;
			print ("}} /* class {0} */", ti.NativeInvokerName);
		}
	}

	// We need to check against the user using just UIKit (and friends) in the FieldAttribute
	// so we need to reflect the libraries contained in our Constants class and do the mapping
	// we will return the system library path if found
	bool IsNotSystemLibrary (string library_name)
	{
		string library_path = null;
		return TryGetLibraryPath (library_name, ref library_path);
	}

	bool TryGetLibraryPath (string library_name, ref string library_path)
	{
		var libSuffixedName = $"{library_name}Library";
		var constType = TypeCache.Constants;
		var field = constType.GetFields (BindingFlags.Public | BindingFlags.Static).FirstOrDefault (f => f.Name == libSuffixedName);
		library_path = (string) (field?.GetRawConstantValue ());
		return library_path is null;
	}

	void GenerateLibraryHandles ()
	{
		sw = GetOutputStream ("ObjCRuntime", "Libraries");

		Header (sw);
		print ("namespace ObjCRuntime {"); indent++;
		print_generated_code ();
		print ("static partial class Libraries {"); indent++;
		foreach (var library_info in libraries.OrderBy (v => v.Key, StringComparer.Ordinal)) {
			var library_name = library_info.Key;
			var library_path = library_info.Value;
			print ("static public class {0} {{", library_name.Replace (".", string.Empty)); indent++;
			if (BindThirdPartyLibrary && library_name == "__Internal") {
				print ("static public readonly IntPtr Handle = Dlfcn.dlopen (null, 0);");
			} else if (BindThirdPartyLibrary && library_path is not null && IsNotSystemLibrary (library_name)) {
				print ($"static public readonly IntPtr Handle = Dlfcn.dlopen (\"{library_path}\", 0);");
			} else if (BindThirdPartyLibrary) {
				print ("static public readonly IntPtr Handle = Dlfcn.dlopen (Constants.{0}Library, 0);", library_name);
			} else {
				// Skip the path check that our managed `dlopen` method does
				// This is not required since the path is checked by `IsNotSystemLibrary`
				print ("static public readonly IntPtr Handle = Dlfcn._dlopen (Constants.{0}Library, 0);", library_name);
			}
			indent--; print ("}");
		}
		indent--; print ("}");
		indent--; print ("}");
		sw.Close ();
	}

	//
	// Processes the various StrongDictionaryAttribute interfaces
	//
	void GenerateStrongDictionaryTypes ()
	{
		foreach (var dictType in strong_dictionaries) {
			if (dictType.IsUnavailable (this))
				continue;
			var sa = AttributeManager.GetCustomAttribute<StrongDictionaryAttribute> (dictType);
			var keyContainerType = sa.TypeWithKeys;
			string suffix = sa.Suffix;

			using (var sw = GetOutputStreamForType (dictType)) {
				string typeName = dictType.Name;
				this.sw = sw;
				Header (sw);

				print ("namespace {0} {{", dictType.Namespace);
				indent++;
				PrintPlatformAttributes (dictType);
				print ("public partial class {0} : DictionaryContainer {{", typeName);
				indent++;
				sw.WriteLine ("#if !COREBUILD");
				print ("[Preserve (Conditional = true)]");
				print ("public {0} () : base (new NSMutableDictionary ()) {{}}\n", typeName);
				print ("[Preserve (Conditional = true)]");
				print ("public {0} (NSDictionary? dictionary) : base (dictionary) {{}}\n", typeName);

				foreach (var pi in dictType.GatherProperties (this)) {
					if (pi.IsUnavailable (this))
						continue;

					string keyname;
					var attrs = AttributeManager.GetCustomAttributes<ExportAttribute> (pi);
					if (attrs.Length == 0)
						keyname = keyContainerType + "." + pi.Name + suffix + "!";
					else {
						keyname = attrs [0].Selector;
						if (keyname.IndexOf ('.') == -1)
							keyname = keyContainerType + "." + keyname + "!";
					}

					PrintPlatformAttributes (pi);
					string modifier = pi.IsInternal (this) ? "internal" : "public";

					print ("{0} {1}? {2} {{",
						modifier,
						TypeManager.FormatType (dictType, pi.PropertyType),
						pi.Name);

					string getter = null, setter = null;
					Type fetchType = pi.PropertyType;
					string castToUnderlying = "";
					string castToEnum = "";
					bool isNativeEnum = false;

					if (pi.PropertyType.IsEnum) {
						fetchType = pi.PropertyType.GetEnumUnderlyingType ();
						castToUnderlying = "(" + fetchType + "?)";
						castToEnum = "(" + TypeManager.FormatType (dictType, pi.PropertyType) + "?)";
						isNativeEnum = IsNativeEnum (pi.PropertyType);
					}
					if (pi.PropertyType.IsValueType) {
						if (pi.PropertyType == TypeCache.System_Boolean) {
							getter = "{1} GetBoolValue ({0})";
							setter = "SetBooleanValue ({0}, {1}value)";
						} else if (fetchType == TypeCache.System_Int32) {
							getter = "{1} GetInt32Value ({0})";
							setter = "SetNumberValue ({0}, {1}value)";
						} else if (fetchType == TypeCache.System_nint) {
							getter = "{1} GetNIntValue ({0})";
							setter = "SetNumberValue ({0}, {1}value)";
						} else if (fetchType == TypeCache.System_Int64) {
							getter = "{1} GetLongValue ({0})";
							setter = "SetNumberValue ({0}, {1}value)";
						} else if (pi.PropertyType == TypeCache.System_Float) {
							getter = "{1} GetFloatValue ({0})";
							setter = "SetNumberValue ({0}, {1}value)";
						} else if (pi.PropertyType == TypeCache.System_Double) {
							getter = "{1} GetDoubleValue ({0})";
							setter = "SetNumberValue ({0}, {1}value)";
						} else if (fetchType == TypeCache.System_UInt32) {
							getter = "{1} GetUInt32Value ({0})";
							setter = "SetNumberValue ({0}, {1}value)";
						} else if (fetchType == TypeCache.System_nuint) {
							getter = "{1} GetNUIntValue ({0})";
							setter = "SetNumberValue ({0}, {1}value)";
						} else if (fetchType == TypeCache.CoreGraphics_CGRect) {
							getter = "{1} GetCGRectValue ({0})";
							setter = "SetCGRectValue ({0}, {1}value)";
						} else if (fetchType == TypeCache.CoreGraphics_CGSize) {
							getter = "{1} GetCGSizeValue ({0})";
							setter = "SetCGSizeValue ({0}, {1}value)";
						} else if (fetchType == TypeCache.CoreGraphics_CGPoint) {
							getter = "{1} GetCGPointValue ({0})";
							setter = "SetCGPointValue ({0}, {1}value)";
						} else if (Frameworks.HaveCoreMedia && fetchType == TypeCache.CMTime) {
							getter = "{1} GetCMTimeValue ({0})";
							setter = "SetCMTimeValue ({0}, {1}value)";
						} else if (isNativeEnum && fetchType == TypeCache.System_Int64) {
							getter = "{1} (long?) GetNIntValue ({0})";
							setter = "SetNumberValue ({0}, {1}value)";
						} else if (isNativeEnum && fetchType == TypeCache.System_UInt64) {
							getter = "{1} (ulong?) GetNUIntValue ({0})";
							setter = "SetNumberValue ({0}, {1}value)";
						} else {
							throw new BindingException (1033, true, pi.PropertyType, dictType, pi.Name);
						}
					} else {
						if (pi.PropertyType.IsArray) {
							var elementType = pi.PropertyType.GetElementType ();
							if (TypeManager.IsWrappedType (elementType)) {
								getter = "GetArray<" + TypeManager.FormatType (dictType, elementType) + "> ({0})";
								setter = "SetArrayValue ({0}, value)";
							} else if (elementType.IsEnum) {
								// Handle arrays of enums as arrays of NSNumbers, casted to the enum
								var enumTypeStr = TypeManager.FormatType (null, elementType);
								getter = "GetArray<" + enumTypeStr +
									"> ({0}, (ptr)=> {{\n\tusing (var num = Runtime.GetNSObject<NSNumber> (ptr)!){{\n\t\treturn (" +
									enumTypeStr + ") num.Int32Value;\n\t}}\n}})";
								setter = "SetArrayValue<" + enumTypeStr + "> ({0}, value)";
							} else if (elementType == TypeCache.System_String) {
								getter = "GetArray<string> ({0}, (ptr) => CFString.FromHandle (ptr)!)";
								setter = "SetArrayValue ({0}, value)";
							} else if (elementType.Name == "CTFontDescriptor") {
								getter = "GetArray<CTFontDescriptor> ({0}, (ptr) => new CTFontDescriptor (ptr, false))";
								setter = "SetArrayValue ({0}, value)";
							} else {
								throw new BindingException (1033, true, pi.PropertyType, dictType, pi.Name);
							}
						} else if (pi.PropertyType == TypeCache.NSString) {
							getter = "GetNSStringValue ({0})";
							setter = "SetStringValue ({0}, value)";
						} else if (pi.PropertyType == TypeCache.System_String) {
							getter = "GetStringValue ({0})";
							setter = "SetStringValue ({0}, value)";
						} else if (pi.PropertyType.Name.StartsWith ("NSDictionary", StringComparison.Ordinal)) {
							if (pi.PropertyType.IsGenericType) {
								var genericParameters = pi.PropertyType.GetGenericArguments ();
								// we want to keep {0} for later yet add the params for the template.
								getter = $"GetNSDictionary <{TypeManager.FormatType (dictType, genericParameters [0])}, {TypeManager.FormatType (dictType, genericParameters [1])}> " + "({0})";
							} else {
								getter = "GetNSDictionary ({0})";
							}
							setter = "SetNativeValue ({0}, value)";
						} else if (TypeManager.IsDictionaryContainerType (pi.PropertyType) || AttributeManager.HasAttribute<StrongDictionaryAttribute> (pi)) {
							var strType = pi.PropertyType.Name;
							getter = $"GetStrongDictionary<{strType}>({{0}}, (dict) => new {strType} (dict))";
							setter = "SetNativeValue ({0}, value.GetDictionary ())";
						} else if (TypeManager.IsWrappedType (pi.PropertyType)) {
							getter = "Dictionary [{0}] as " + pi.PropertyType;
							setter = "SetNativeValue ({0}, value)";
						} else if (pi.PropertyType.Name == "CGColorSpace") {
							getter = "GetNativeValue<" + pi.PropertyType + "> ({0})";
							setter = "SetNativeValue ({0}, value)";
						} else if (pi.PropertyType.Name == "CGImageSource") {
							getter = "GetNativeValue<" + pi.PropertyType + "> ({0})";
							setter = "SetNativeValue ({0}, value)";
						} else if (pi.PropertyType.Name == "CTFontDescriptor") {
							getter = "GetNativeValue<" + pi.PropertyType + "> ({0})";
							setter = "SetNativeValue ({0}, value)";
						} else {
							throw new BindingException (1033, true, pi.PropertyType, dictType, pi.Name);
						}
					}

					var og = getter;
					try {
						getter = String.Format (getter, keyname, castToEnum);
					} catch {
						Console.WriteLine ("OOPS: g={0} k={1} c={2}", og, keyname, castToEnum);
						throw;
					}
					setter = String.Format (setter, keyname, castToUnderlying);
					if (pi.CanRead) {
						indent++;
						print ("get {"); indent++;
						print ("return {0};", getter);
						indent--; print ("}");
						indent--;
					}
					if (pi.CanWrite) {
						if (setter is null)
							throw new BindingException (1032, true, pi.PropertyType, dictType, pi.Name);
						indent++;
						print ("set {"); indent++;
						print ("{0};", setter);
						indent--; print ("}");
						indent--;
					}
					print ("}\n");

				}
				sw.WriteLine ("#endif");
				indent--;
				print ("}");
				indent--;
				print ("}");
			}
		}
	}

	void GenerateEventArgsFile ()
	{
		sw = GetOutputStream ("ObjCRuntime", "EventArgs");

		Header (sw);
		foreach (Type eventType in notification_event_arg_types.Keys) {
			// Do not generate events for stuff with no arguments
			if (eventType is null)
				continue;

			if (eventType.Namespace is not null) {
				print ("namespace {0} {{", eventType.Namespace);
				indent++;
			}

			print ("public partial class {0} : NSNotificationEventArgs {{", eventType.Name); indent++;
			print ("public {0} (NSNotification notification) : base (notification) \n{{\n}}\n", eventType.Name);
			int i = 0;
			foreach (var prop in eventType.GetProperties (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)) {
				if (prop.IsUnavailable (this))
					continue;
				var attrs = AttributeManager.GetCustomAttributes<ExportAttribute> (prop);
				if (attrs.Length == 0)
					throw new BindingException (1010, true, eventType, prop.Name);

				var is_internal = prop.IsInternal (this);
				var export = attrs [0];
				var use_export_as_string_constant = export.ArgumentSemantic != ArgumentSemantic.None;
				var null_allowed = AttributeManager.HasAttribute<NullAllowedAttribute> (prop);
				var nullable_type = prop.PropertyType.IsValueType && null_allowed;
				var propertyType = prop.PropertyType;
				var propNamespace = prop.DeclaringType.Namespace;
				var probe_presence = AttributeManager.HasAttribute<ProbePresenceAttribute> (prop);

				string kn = "k" + (i++);
				if (use_export_as_string_constant) {
					print ("{0} {1}{2} {3} {{\n\tget {{\n",
						   is_internal ? "internal" : "public",
						   propertyType,
						   nullable_type ? "?" : "",
						   prop.Name);
					indent += 2;
					print ("{0} value;", NativeHandleType);
					print ("using (var str = new NSString (\"{0}\")){{", export.Selector);
					kn = "str.Handle";
					indent++;
				} else {
					var lib = propNamespace.Substring (propNamespace.IndexOf ('.') + 1);
					print ("[Field (\"{0}\", \"{1}\")]", export.Selector, lib);
					print ("static IntPtr {0};", kn);
					print ("");
					// linker will remove the attributes (but it's useful for testing)
					print_generated_code ();
					print ("{0} {1}{2} {3} {{",
						   is_internal ? "internal" : "public",
						   propertyType,
						   nullable_type ? "?" : "",
						   prop.Name);
					indent++;
					print ("get {");
					indent++;
					print ("{0} value;", NativeHandleType);
					print ("if ({0} == IntPtr.Zero)", kn);
					indent++;
					var libname = BindThirdPartyLibrary ? "__Internal" : lib;
					print ("{0} = ObjCRuntime.Dlfcn.GetIntPtr (Libraries.{1}.Handle, \"{2}\");", kn, libname, export.Selector);
					indent--;
				}
				// [NullAllowed] on `UserInfo` requires a check for every case
				print ("var userinfo = Notification.UserInfo;");
				print ("if (userinfo is null)");
				indent++;
				if (probe_presence)
					print ("return false;");
				else if (null_allowed)
					print ("return null!;");
				else
					print ("value = IntPtr.Zero;");
				indent--;
				print ("else");
				indent++;
				print ("value = userinfo.LowlevelObjectForKey ({0});", kn);
				indent--;
				if (use_export_as_string_constant) {
					indent--;
					print ("}");
				} else
					print ("");

				if (probe_presence)
					print ("return value != IntPtr.Zero;");
				else {
					var et = propertyType.GetElementType ();
					bool is_property_array_wrapped_type = propertyType.IsArray && TypeManager.IsWrappedType (et);
					bool is_property_wrapped_type = TypeManager.IsWrappedType (propertyType);
					bool is_system_string = (propertyType == TypeCache.System_String);
					bool skip_null_check = is_property_wrapped_type || is_property_array_wrapped_type || is_system_string;

					if (null_allowed && !skip_null_check)
						print ("if (value == IntPtr.Zero)\n\treturn null;");
					else if (propertyType.IsArray)
						print ("if (value == IntPtr.Zero)\n\treturn Array.Empty<{0}> ();", TypeManager.RenderType (et));
					else if (!skip_null_check)
						print ("if (value == IntPtr.Zero)\n\treturn default({0});", TypeManager.RenderType (propertyType));

					var fullname = propertyType.FullName;

					if (is_property_array_wrapped_type) {
						print ("return CFArray.ArrayFromHandle<{0}> (value)!;", TypeManager.RenderType (et));
					} else if (is_property_wrapped_type) {
						print ("return Runtime.GetNSObject<{0}> (value)!;", TypeManager.RenderType (propertyType));
					} else if (propertyType == TypeCache.System_Double)
						print (GenerateNSNumber ("", "DoubleValue"));
					else if (propertyType == TypeCache.System_Float)
						print (GenerateNSNumber ("", "FloatValue"));
					else if (fullname == "System.Drawing.PointF")
						print (GenerateNSValue ("PointFValue"));
					else if (fullname == "System.Drawing.SizeF")
						print (GenerateNSValue ("SizeFValue"));
					else if (fullname == "System.Drawing.RectangleF")
						print (GenerateNSValue ("RectangleFValue"));
					else if (fullname == "CoreGraphics.CGPoint")
						print (GenerateNSValue ("CGPointValue"));
					else if (fullname == "CoreGraphics.CGSize")
						print (GenerateNSValue ("CGSizeValue"));
					else if (fullname == "CoreGraphics.CGRect")
						print (GenerateNSValue ("CGRectValue"));
					else if (is_system_string)
						print ("return CFString.FromHandle (value)!;");
					else if (propertyType == TypeCache.NSString)
						print ("return new NSString (value);");
					else if (propertyType == TypeCache.System_String_Array) {
						print ("return CFArray.StringArrayFromHandle (value)!;");
					} else {
						Type underlying = propertyType.IsEnum ? propertyType.GetEnumUnderlyingType () : propertyType;
						string cast = propertyType.IsEnum ? "(" + propertyType.FullName + ") " : "";

						if (underlying == TypeCache.System_Int32)
							print (GenerateNSNumber (cast, "Int32Value"));
						else if (underlying == TypeCache.System_UInt32)
							print (GenerateNSNumber (cast, "UInt32Value"));
						else if (underlying == TypeCache.System_Int64)
							print (GenerateNSNumber (cast, "Int64Value"));
						else if (underlying == TypeCache.System_UInt64)
							print (GenerateNSNumber (cast, "UInt64Value"));
						else if (underlying == TypeCache.System_Int16)
							print (GenerateNSNumber (cast, "Int16Value"));
						else if (underlying == TypeCache.System_UInt16)
							print (GenerateNSNumber (cast, "UInt16Value"));
						else if (underlying == TypeCache.System_SByte)
							print (GenerateNSNumber (cast, "SByteValue"));
						else if (underlying == TypeCache.System_Byte)
							print (GenerateNSNumber (cast, "ByteValue"));
						else if (underlying == TypeCache.System_Boolean)
							print (GenerateNSNumber (cast, "BoolValue"));
						else if (underlying == TypeCache.System_nint)
							print (GenerateNSNumber (cast, "NIntValue"));
						else if (underlying == TypeCache.System_nuint)
							print (GenerateNSNumber (cast, "NUIntValue"));
						else
							throw new BindingException (1011, true, propertyType, underlying);
					}
				}
				indent -= 2;
				print ("\t}\n}\n");
			}

			indent--; print ("}");

			if (eventType.Namespace is not null) {
				indent--;
				print ("}");
			}
		}
		sw.Close ();
	}


	public void DumpChildren (int level, GeneratedType gt)
	{
		string prefix = new string ('\t', level);
		Console.WriteLine ("{2} {0} - {1}", gt.Type.Name, gt.ImplementsAppearance ? "APPEARANCE" : "", prefix);
		foreach (var c in (from s in gt.Children.OrderBy (s => s.Type.FullName, StringComparer.Ordinal) select s))
			DumpChildren (level + 1, c);
	}

	// this attribute allows the linker to be more clever in removing unused code in bindings - without risking breaking user code
	// only generate those for monotouch now since we can ensure they will be linked away before reaching the devices
	public void GeneratedCode (StreamWriter sw, int tabs, bool optimizable = true)
	{
		for (int i = 0; i < tabs; i++)
			sw.Write ('\t');
		sw.Write ("[BindingImpl (BindingImplOptions.GeneratedCode");
		if (optimizable)
			sw.Write (" | BindingImplOptions.Optimizable");
		sw.WriteLine (")]");
	}

	static void WriteIsDirectBindingCondition (StreamWriter sw, ref int tabs, bool? is_direct_binding, string is_direct_binding_value, Func<string> trueCode, Func<string> falseCode)
	{
		if (is_direct_binding_value is not null)
			sw.Write ('\t', tabs).WriteLine ("IsDirectBinding = {0};", is_direct_binding_value);

		// If we don't know the IsDirectBinding value, we need the condition
		if (!is_direct_binding.HasValue) {
			sw.Write ('\t', tabs).WriteLine ("if (IsDirectBinding) {");
			tabs++;
		}

		// We need the true part if we don't know, or if we know it's true
		if (is_direct_binding != false) {
			var code = trueCode ();
			if (!string.IsNullOrEmpty (code))
				sw.Write ('\t', tabs).WriteLine (code);
		}

		if (!is_direct_binding.HasValue)
			sw.Write ('\t', tabs - 1).WriteLine ("} else {");

		// We need the false part if we don't know, or if we know it's false
		if (is_direct_binding != true) {
			var code = falseCode ();
			if (!string.IsNullOrEmpty (code))
				sw.Write ('\t', tabs).WriteLine (code);
		}

		if (!is_direct_binding.HasValue) {
			tabs--;
			sw.Write ('\t', tabs).WriteLine ("}");
		}
	}

	public void print_generated_code (bool optimizable = true)
	{
		GeneratedCode (sw, indent, optimizable);
	}

	public void print (string format)
	{
		print (sw, format);
	}

	public void print (string format, params object [] args)
	{
		print (sw, format, args);
	}

	static char [] newlineCharacters = new char [] { '\n' };

	public void print (StreamWriter w, string format)
	{
		if (indent < 0)
			throw new InvalidOperationException ("Indent is a negative value.");

		var lines = format.Split (newlineCharacters);

		for (int i = 0; i < lines.Length; i++) {
			if (lines [i].Length == 0)
				continue;
			w.Write ('\t', indent);
			w.WriteLine (lines [i]);
		}
	}

	public void print (StreamWriter w, string format, params object [] args)
	{
		print (w, string.Format (format, args));
	}

	public void print (StreamWriter w, IEnumerable e)
	{
		foreach (var a in e)
			w.WriteLine (a);
	}

	bool Duplicated (AvailabilityBaseAttribute candidate, AvailabilityBaseAttribute [] attributes)
	{
		foreach (var a in attributes) {
			if (candidate.AvailabilityKind != a.AvailabilityKind)
				continue;
			if (candidate.Platform != a.Platform)
				continue;
			if (candidate.Version != a.Version)
				continue;
			// the actual message (when present) is not really important
			return true;
		}
		return false;
	}

	// do not generate introduced/supported attributes for versions earlier than the minimum supported
	// more important since dotnet and legacy have different minimums (so this can't be done in binding files)
	bool FilterMinimumVersion (AvailabilityBaseAttribute aa)
	{
#if NET
		// dotnet can never filter minimum versions, as they are semantically important in some cases
		// See for details: https://github.com/xamarin/xamarin-macios/issues/10170
		return true;
#else
		if (aa.AvailabilityKind != AvailabilityKind.Introduced)
			return true;

		Version min;
		switch (aa.Platform) {
		case PlatformName.iOS:
			min = Xamarin.SdkVersions.MiniOSVersion;
			break;
		case PlatformName.TvOS:
			min = Xamarin.SdkVersions.MinTVOSVersion;
			break;
		case PlatformName.WatchOS:
			min = Xamarin.SdkVersions.MinWatchOSVersion;
			break;
		case PlatformName.MacOSX:
			min = Xamarin.SdkVersions.MinOSXVersion;
			break;
		case PlatformName.MacCatalyst:
			min = Xamarin.SdkVersions.MinMacCatalystVersion;
			break;
		default:
			throw new BindingException (1047, aa.Platform.ToString ());
		}
		return aa.Version > min;
#endif
	}

	HashSet<string> GetFrameworkListForPlatform (PlatformName platform)
	{
		HashSet<string> frameworkList = null;
		switch (platform) {
		case PlatformName.iOS:
			frameworkList = Frameworks.iosframeworks;
			break;
		case PlatformName.TvOS:
			frameworkList = Frameworks.tvosframeworks;
			break;
		case PlatformName.MacOSX:
			frameworkList = Frameworks.macosframeworks;
			break;
		case PlatformName.MacCatalyst:
			frameworkList = Frameworks.maccatalystframeworks;
			break;
		default:
			frameworkList = new HashSet<string> ();
			break;
		}
		return new HashSet<string> (frameworkList.Select (x => x.ToLower (CultureInfo.InvariantCulture)));
	}

	static string FindNamespace (MemberInfo item)
	{
		switch (item) {
		case TypeInfo type:
			return type.Namespace;
		case PropertyInfo prop:
			return prop.DeclaringType.Namespace;
		case MethodInfo meth:
			return meth.DeclaringType.Namespace;
		default:
			throw new NotImplementedException ($"FindNamespace on {item} of type {item.GetType ()}");
		}
	}

	bool IsInSupportedFramework (MemberInfo klass, PlatformName platform)
	{
		string ns = FindNamespace (klass);
		if (string.IsNullOrEmpty (ns))
			return false;
		var list = GetFrameworkListForPlatform (platform);
		return list.Contains (ns.ToLower (CultureInfo.InvariantCulture));
	}

	void AddUnlistedAvailability (MemberInfo containingClass, List<AvailabilityBaseAttribute> availability)
	{
		// If there are no unavailable attributes for a platform on a type
		// add a supported introduced (without version) since it was "unlisted"
		foreach (var platform in BindingTouch.AllPlatformNames) {
			if (!PlatformMarkedUnavailable (platform, availability) && IsInSupportedFramework (containingClass, platform)) {
				availability.Add (AttributeFactory.CreateNoVersionSupportedAttribute (platform));
			}
		}
	}

	static bool PlatformHasIntroduced (PlatformName platform, List<AvailabilityBaseAttribute> memberAvailability)
	{
		return memberAvailability.Any (v => (v.Platform == platform && v is IntroducedAttribute));
	}

	static bool PlatformMarkedUnavailable (PlatformName platform, List<AvailabilityBaseAttribute> memberAvailability)
	{
		return memberAvailability.Any (v => (v.Platform == platform && v is UnavailableAttribute));
	}

	// Especially for TV and Catalyst some entire namespaces are removed via framework_sources.
	// As a final step, if we are on a namespace that flatly doesn't exist, drop it. Then if we don't have a not supported, add it
	void StripIntroducedOnNamespaceNotIncluded (List<AvailabilityBaseAttribute> memberAvailability, MemberInfo context)
	{
		if (context is TypeInfo containingClass) {
			var droppedPlatforms = new HashSet<PlatformName> ();

			// Walk all members and look for introduced that are nonsense for our containing class's platform
			foreach (var introduced in memberAvailability.Where (a => a.AvailabilityKind == AvailabilityKind.Introduced || a.AvailabilityKind == AvailabilityKind.Deprecated).ToList ()) {
				// Hack - WebKit namespace has two distinct implementations with different types
				// It can not be hacked in IsInSupportedFramework as AddUnlistedAvailability 
				// will add iOS implied to the mac version and so on. So hard code it here...
				if (FindNamespace (containingClass) == "WebKit" && introduced.Platform != PlatformName.TvOS) {
					continue;
				}
				if (!IsInSupportedFramework (containingClass, introduced.Platform)) {
					memberAvailability.Remove (introduced);
					droppedPlatforms.Add (introduced.Platform);
				}
			}

			// For each attribute we dropped, if we don't have an existing non-introduced, create one
			foreach (var platform in droppedPlatforms) {
				if (!memberAvailability.Any (a => platform == a.Platform && a.AvailabilityKind != AvailabilityKind.Introduced)) {
					memberAvailability.Add (AttributeFactory.CreateUnsupportedAttribute (platform));
				}
			}
		}
	}

	// This assumes the compiler implements property methods as get_ or set_ prefixes
	static PropertyInfo GetProperyFromGetSetMethod (MethodInfo method)
	{
		string name = method.Name;
		if (name.StartsWith ("get_", StringComparison.Ordinal) || name.StartsWith ("set_", StringComparison.Ordinal)) {
			return method.DeclaringType.GetProperty (name.Substring (4));
		}
		return null;
	}

	static MemberInfo FindContainingContext (MemberInfo mi)
	{
		if (mi is null) {
			throw new InvalidOperationException ("FindContainingContext could not find parent class?");
		}
		if (mi is MethodInfo method) {
			var containingProperty = GetProperyFromGetSetMethod (method);
			if (containingProperty is not null) {
				return containingProperty;
			}
		}
		if (mi is TypeInfo) {
			return mi;
		}
		return FindContainingContext (mi.DeclaringType);
	}

	// We need to collect all of the availability attributes walking up the chain of context.
	// Example: A get_Foo inside of a property Foo which is inside of a class Klass.
	//          The Foo property and the Klass both could have unique or duplicate attributes
	// We collect them all, starting with the inner most first in the list.
	// Later on CopyValidAttributes will handle only copying the first valid one down
	List<AvailabilityBaseAttribute> GetAllParentAttributes (MemberInfo context)
	{
		var parentAvailability = new List<AvailabilityBaseAttribute> ();
		while (true) {
			parentAvailability.AddRange (AttributeManager.GetCustomAttributes<AvailabilityBaseAttribute> (context));
			var parentContext = FindContainingContext (context);
			if (context == parentContext) {
				return parentAvailability;
			}
			context = parentContext;
		}
	}

	public bool PrintPlatformAttributes (MemberInfo mi, Type inlinedType = null)
	{
		bool printed = false;
		if (mi is null)
			return printed;

		AvailabilityBaseAttribute [] type_ca = null;

		foreach (var availability in GetPlatformAttributesToPrint (mi, null, inlinedType)) {
			var t = inlinedType ?? (mi as TypeInfo) ?? mi.DeclaringType;
			if (type_ca is null) {
				if (t is not null)
					type_ca = AttributeManager.GetCustomAttributes<AvailabilityBaseAttribute> (t);
				else
					type_ca = Array.Empty<AvailabilityBaseAttribute> ();
			}
#if !NET
			// if we're comparing to something else (than ourself) then don't generate duplicate attributes
			if ((mi != t) && Duplicated (availability, type_ca))
				continue;
#endif
			switch (availability.AvailabilityKind) {
			case AvailabilityKind.Unavailable:
				// an unavailable member can override type-level attribute
				print (availability.ToString ());
				printed = true;
				break;
			default:
#if !NET
				// can't introduce or deprecate/obsolete a member on a type that is not available
				if (IsUnavailable (type_ca, availability.Platform))
					continue;
#endif
				if (FilterMinimumVersion (availability))
					print (availability.ToString ());
				printed = true;
				break;
			}
		}
		return printed;
	}

	static bool IsUnavailable (IEnumerable<AvailabilityBaseAttribute> customAttributes, PlatformName platform)
	{
		if (customAttributes is null)
			return false;
		foreach (var ca in customAttributes) {
			if ((platform == ca.Platform) && (ca.AvailabilityKind == AvailabilityKind.Unavailable))
				return true;
		}
		return false;
	}

	static bool HasAvailability (IEnumerable<AvailabilityBaseAttribute> customAttributes, PlatformName platform)
	{
		if (customAttributes is null)
			return false;
		foreach (var ca in customAttributes) {
			if (platform == ca.Platform)
				return true;
		}
		return false;
	}

	public void PrintPlatformAttributesNoDuplicates (MemberInfo generatedType, MemberInfo inlinedMethod)
	{
		if ((generatedType is null) || (inlinedMethod is null))
			return;

		var inlined_ca = new List<AvailabilityBaseAttribute> ();
		inlined_ca.AddRange (GetPlatformAttributesToPrint (inlinedMethod, generatedType.DeclaringType, generatedType));
		if (inlinedMethod.DeclaringType is not null) {
			// if not conflictual add the custom attributes from the type
			foreach (var availability in GetPlatformAttributesToPrint (inlinedMethod.DeclaringType, null, generatedType)) {
				// already decorated, skip
				if (HasAvailability (inlined_ca, availability.Platform))
					continue;
				// not available, skip
				if (IsUnavailable (inlined_ca, availability.Platform))
					continue;
				// this type-level custom attribute has meaning and not covered by the method
				inlined_ca.Add (availability);
			}
		}

		var generated_type_ca = new HashSet<string> ();

#if !NET
		foreach (var availability in AttributeManager.GetCustomAttributes<AvailabilityBaseAttribute> (generatedType)) {
			var s = availability.ToString ();
			generated_type_ca.Add (s);
		}
#endif

		// the type, in which we are inlining the current method, might already have the same availability attribute
		// which we would duplicate if generated
		foreach (var availability in inlined_ca) {
			if (!FilterMinimumVersion (availability))
				continue;
			var s = availability.ToString ();
			if (!generated_type_ca.Contains (s))
				print (s);
		}
	}

	public string SelectorField (string s, bool ignore_inline_directive = false)
	{
		string name;

		if (InlineSelectors && !ignore_inline_directive)
			return "Selector.GetHandle (\"" + s + "\")";

		if (selector_names.TryGetValue (s, out name))
			return name;

		StringBuilder sb = new StringBuilder ();
		bool up = true;
		sb.Append ("sel");

		foreach (char c in s) {
			if (up && c != ':') {
				sb.Append (Char.ToUpper (c));
				up = false;
			} else if (c == ':') {
				// Selectors can differ only by colons.
				// Example 'mountDeveloperDiskImageWithError:' and 'mountDeveloperDiskImage:WithError:' (from Xamarin.Hosting)
				// This also showed up in a bug report: http://bugzilla.xamarin.com/show_bug.cgi?id=2626
				// So make sure we create a different name for those in C# as well, otherwise the generated code won't build.
				up = true;
				sb.Append ('_');
			} else
				sb.Append (c);
		}
		if (!InlineSelectors)
			sb.Append ("XHandle");
		name = sb.ToString ();
		selector_names [s] = name;
		return name;
	}

	// This will return the needed ObjC name for class_ptr lookup
	public string FormatCategoryClassName (BaseTypeAttribute bta)
	{
		object [] attribs;

		// If we are binding a third party library we need to check for the RegisterAttribute
		// its Name property will contain the propper name we are looking for
		if (BindThirdPartyLibrary) {
			attribs = AttributeManager.GetCustomAttributes<RegisterAttribute> (bta.BaseType);
			if (attribs.Length > 0) {
				var register = (RegisterAttribute) attribs [0];
				return register.Name;
			} else {
				// this will do for categories of third party classes defined in ApiDefinition.cs
				attribs = AttributeManager.GetCustomAttributes<BaseTypeAttribute> (bta.BaseType);
				if (attribs.Length > 0) {
					var baseT = (BaseTypeAttribute) attribs [0];
					if (baseT.Name is not null)
						return baseT.Name;
				}
			}
		} else {
			// If we are binding a category inside Xamarin.iOS the selector name will come in
			// the Name property of the BaseTypeAttribute of the base type if changed from the ObjC name
			attribs = AttributeManager.GetCustomAttributes<BaseTypeAttribute> (bta.BaseType);
			if (attribs.Length > 0) {
				var baseT = (BaseTypeAttribute) attribs [0];
				if (baseT.Name is not null)
					return baseT.Name;
			}
		}

		var objcClassName = TypeManager.FormatType (null, bta.BaseType);

		if (objcClassName.Contains ("global::"))
			objcClassName = objcClassName.Substring (objcClassName.LastIndexOf ('.') + 1);

		return objcClassName;
	}



	//
	// Makes the public signature for an exposed method
	//
	public string MakeSignature (MemberInformation minfo)
	{
		return MakeSignature (minfo, false, minfo.Method.GetParameters ());
	}

	//
	// Makes the public signature for an exposed method, taking into account if PreserveAttribute is needed
	//
	public string MakeSignature (MemberInformation minfo, bool alreadyPreserved)
	{
		return MakeSignature (minfo, false, minfo.Method.GetParameters (), "", alreadyPreserved);
	}

	public string GetAsyncName (MethodInfo mi)
	{
		var attr = AttributeManager.GetCustomAttribute<AsyncAttribute> (mi);
		if (attr.MethodName is not null)
			return attr.MethodName;
		return mi.Name + "Async";
	}

	public bool IsModel (Type type)
	{
		return AttributeManager.HasAttribute<ModelAttribute> (type) && !AttributeManager.HasAttribute<SyntheticAttribute> (type);
	}

	public bool IsProtocol (Type type)
	{
		return AttributeManager.HasAttribute<ProtocolAttribute> (type);
	}

	public bool Protocolize (ICustomAttributeProvider provider)
	{
		var attribs = AttributeManager.GetCustomAttributes<ProtocolizeAttribute> (provider);
		if (attribs is null || attribs.Length == 0)
			return false;

		var attrib = attribs [0];
		return XamcoreVersion >= attrib.Version;
	}

	public string MakeSignature (MemberInformation minfo, bool is_async, ParameterInfo [] parameters, string extra = "", bool alreadyPreserved = false)
	{
		var mi = minfo.Method;
		var category_class = minfo.category_extension_type;
		StringBuilder sb = new StringBuilder ();
		string name = minfo.is_ctor ? GetGeneratedTypeName (mi.DeclaringType) : is_async ? GetAsyncName (mi) : mi.Name;

		// Some codepaths already write preservation info
		PrintAttributes (minfo.mi, preserve: !alreadyPreserved, advice: true, bindAs: true, requiresSuper: true);

		if (!minfo.is_ctor && !is_async) {
			var prefix = "";
			if (!BindThirdPartyLibrary) {
				var hasReturnTypeProtocolize = Protocolize (minfo.Method.ReturnParameter);
				if (hasReturnTypeProtocolize) {
					if (!IsProtocol (minfo.Method.ReturnType)) {
						ErrorHelper.Warning (1108, minfo.Method.DeclaringType, minfo.Method, minfo.Method.ReturnType.FullName);
					} else {
						prefix = "I";
					}
				}
				if (minfo.Method.ReturnType.IsArray) {
					var et = minfo.Method.ReturnType.GetElementType ();
					if (IsModel (et))
						ErrorHelper.Warning (1109, minfo.Method.DeclaringType, minfo.Method.Name, et, et.Namespace, et.Name);
				}
				if (IsModel (minfo.Method.ReturnType) && !hasReturnTypeProtocolize)
					ErrorHelper.Warning (1107, minfo.Method.DeclaringType, minfo.Method.Name, minfo.Method.ReturnType, minfo.Method.ReturnType.Namespace, minfo.Method.ReturnType.Name);
			}

			if (minfo.is_bindAs) {
				if (IsMemberInsideProtocol (minfo.mi.DeclaringType))
					throw new BindingException (1050, true, minfo.mi.DeclaringType.Name);

				var bindAsAttrib = GetBindAsAttribute (minfo.mi);
				sb.Append (prefix + TypeManager.FormatType (bindAsAttrib.Type.DeclaringType, bindAsAttrib.Type));
				if (!bindAsAttrib.Type.IsValueType && AttributeManager.HasAttribute<NullAllowedAttribute> (mi.ReturnParameter))
					sb.Append ('?');
			} else {
				sb.Append (prefix);
				sb.Append (TypeManager.FormatType (mi.DeclaringType, mi.ReturnType));
				if (!mi.ReturnType.IsValueType && AttributeManager.HasAttribute<NullAllowedAttribute> (mi.ReturnParameter))
					sb.Append ('?');
			}

			sb.Append (" ");
		}
		// Unified internal methods automatically get a _ appended
		if (minfo.is_extension_method && minfo.Method.IsSpecialName) {
			if (name.StartsWith ("get_", StringComparison.Ordinal))
				name = "Get" + name.Substring (4);
			else if (name.StartsWith ("set_", StringComparison.Ordinal))
				name = "Set" + name.Substring (4);
		}
		sb.Append (name);
		if (minfo.is_unified_internal)
			sb.Append ("_");
		sb.Append (" (");

		bool comma = false;
		if (minfo.is_extension_method) {
			sb.Append ("this ");
			sb.Append ("I" + mi.DeclaringType.Name);
			sb.Append (" This");
			comma = true;
		} else if (category_class is not null) {
			sb.Append ("this ");
			//			Console.WriteLine ("Gto {0} and {1}", mi.DeclaringType, category_class);
			sb.Append (TypeManager.FormatType (mi.DeclaringType, category_class));
			sb.Append (" This");
			comma = true;
		}
		MakeSignatureFromParameterInfo (comma, sb, mi, minfo.type, parameters);
		sb.Append (extra);
		sb.Append (")");
		return sb.ToString ();
	}

	//
	// Renders the parameters in @parameters in a format suitable for a method declaration.
	// The result is place into the provided string builder 
	// 
	public void MakeSignatureFromParameterInfo (bool comma, StringBuilder sb, MemberInfo mi, Type declaringType, ParameterInfo [] parameters)
	{
		int parCount = parameters.Length;
		for (int pari = 0; pari < parCount; pari++) {
			var pi = parameters [pari];

			if (comma)
				sb.Append (", ");
			comma = true;

			// Format nicely the type, as succinctly as possible
			Type parType = pi.ParameterType;
			if (parType.IsSubclassOf (TypeCache.System_Delegate)) {
				var ti = MakeTrampoline (parType);
				sb.AppendFormat ("[BlockProxy (typeof (ObjCRuntime.Trampolines.{0}))]", ti.NativeInvokerName);
			}

			if (AttributeManager.HasAttribute<TransientAttribute> (pi))
				sb.Append ("[Transient] ");

			if (parType.IsByRef) {
				string reftype = pi.IsOut ? "out " : "ref ";
				sb.Append (reftype);
				parType = parType.GetElementType ();
			}
			// note: check for .NET `params` on the bindings, which generates a `ParamArrayAttribute` or the old (not really required) custom `ParamsAttribute`
			if (pari == parCount - 1 && parType.IsArray && (AttributeManager.HasAttribute<ParamsAttribute> (pi) || AttributeManager.HasAttribute<ParamArrayAttribute> (pi))) {
				sb.Append ("params ");
			}
			var protocolized = false;
			if (!BindThirdPartyLibrary && Protocolize (pi)) {
				if (!AttributeManager.HasAttribute<ProtocolAttribute> (parType)) {
					Console.WriteLine ("Protocolized attribute for type that does not have a [Protocol] for {0}'s parameter {1}", mi, pi);
				}
				protocolized = true;
			}

			var bindAsAtt = GetBindAsAttribute (pi);
			if (bindAsAtt is not null) {
				PrintBindAsAttribute (pi, sb);
				var bt = bindAsAtt.Type;
				sb.Append (TypeManager.FormatType (bt.DeclaringType, bt, protocolized));
				if (!bt.IsValueType && AttributeManager.HasAttribute<NullAllowedAttribute> (pi))
					sb.Append ('?');
			} else {
				sb.Append (TypeManager.FormatType (declaringType, parType, protocolized));
				// some `IntPtr` are decorated with `[NullAttribute]`
				if (!parType.IsValueType && AttributeManager.HasAttribute<NullAllowedAttribute> (pi))
					sb.Append ('?');
			}

			sb.Append (" ");
			sb.Append (pi.Name.GetSafeParamName ());
		}
	}

	void Header (StreamWriter w)
	{
		print (w, "//\n// Auto-generated from generator.cs, do not edit\n//");
		print (w, "// We keep references to objects, so warning 414 is expected\n");
		print (w, "#pragma warning disable 414\n");
		print (w, NamespaceCache.ImplicitNamespaces.OrderByDescending (n => n.StartsWith ("System", StringComparison.Ordinal)).ThenBy (n => n.Length).Select (n => "using " + n + ";"));
		print (w, "");
		print (w, "#nullable enable");
		print (w, "");
		print (w, "#if !NET");
		print (w, "using NativeHandle = System.IntPtr;");
		print (w, "#endif");
	}

	//
	// Given a method info that has a return value, produce the text necessary on
	// both sides of a call to wrap the result into user-facing MonoTouch types.
	//
	// @mi: the method info, should not have a returntype of void
	// @cast_a: left side to generate
	// @cast_b: right side to generate
	//
	void GetReturnsWrappers (MethodInfo mi, MemberInformation minfo, Type declaringType, out string cast_a, out string cast_b, StringBuilder postproc = null)
	{
		cast_a = cast_b = "";
		if (mi.ReturnType == TypeCache.System_Void) {
			throw new ArgumentException ("the provided Method has a void return type, it should never call this method");
		}

		MarshalInfo mai = new MarshalInfo (this, mi);
		MarshalType mt;

		if (GetNativeEnumToManagedExpression (mi.ReturnType, out cast_a, out cast_b, out var _, postproc)) {
			// we're done here
		} else if (mi.ReturnType.IsEnum) {
			cast_a = "(" + TypeManager.FormatType (mi.DeclaringType, mi.ReturnType) + ") ";
			cast_b = "";
		} else if (marshalTypes.TryGetMarshalType (mai.Type, out mt)) {
			cast_a = mt.CreateFromRet;
			cast_b = mt.ClosingCreate;
		} else if (TypeManager.IsWrappedType (mi.ReturnType)) {
			// protocol support means we can return interfaces and, as far as .NET knows, they might not be NSObject
			if (IsProtocolInterface (mi.ReturnType)) {
				cast_a = " Runtime.GetINativeObject<" + TypeManager.FormatType (mi.DeclaringType, mi.ReturnType) + "> (";
				cast_b = ", false)!";
			} else if (minfo is not null && minfo.protocolize) {
				cast_a = " Runtime.GetINativeObject<" + TypeManager.FormatType (mi.DeclaringType, mi.ReturnType.Namespace, FindProtocolInterface (mi.ReturnType, mi)) + "> (";
				cast_b = ", false)!";
			} else if (minfo is not null && minfo.is_forced) {
				cast_a = " Runtime.GetINativeObject<" + TypeManager.FormatType (declaringType, mi.ReturnType) + "> (";
				cast_b = $", true, {minfo.is_forced_owns})!";
			} else if (minfo is not null && minfo.is_bindAs) {
				var bindAs = GetBindAsAttribute (minfo.mi);
				var nullableBindAsType = TypeManager.GetUnderlyingNullableType (bindAs.Type);
				var isNullable = nullableBindAsType is not null;
				var bindAsType = TypeManager.GetUnderlyingNullableType (bindAs.Type) ?? bindAs.Type;
				var formattedBindAsType = TypeManager.FormatType (declaringType, bindAs.Type);
				string suffix;
				var wrapper = GetFromBindAsWrapper (minfo, out suffix);
				var formattedReturnType = TypeManager.FormatType (declaringType, mi.ReturnType);
				if (mi.ReturnType == TypeCache.NSString) {
					if (isNullable) {
						print ("{0} retvaltmp;", NativeHandleType);
						cast_a = "((retvaltmp = ";
						cast_b = $") == IntPtr.Zero ? default ({formattedBindAsType}) : ({wrapper}Runtime.GetNSObject<{formattedReturnType}> (retvaltmp)!){suffix})";
					} else {
						cast_a = $"{wrapper}Runtime.GetNSObject<{formattedReturnType}> (";
						cast_b = $")!{suffix}";
					}
				} else {
					var enumCast = (bindAsType.IsEnum && !minfo.type.IsArray) ? $"({formattedBindAsType}) " : string.Empty;
					print ("{0} retvaltmp;", NativeHandleType);
					cast_a = "((retvaltmp = ";
					cast_b = $") == IntPtr.Zero ? default ({formattedBindAsType}) : ({enumCast}Runtime.GetNSObject<{formattedReturnType}> (retvaltmp)!{wrapper})){suffix}";
				}
			} else {
				cast_a = " Runtime.GetNSObject<" + TypeManager.FormatType (declaringType, mi.ReturnType) + "> (";
				cast_b = ")!";
			}
		} else if (mi.ReturnType.IsGenericParameter) {
			cast_a = " Runtime.GetINativeObject<" + mi.ReturnType.Name + "> (";
			cast_b = ", false)!";
		} else if (mai.Type == TypeCache.System_String && !mai.PlainString) {
			cast_a = "CFString.FromHandle (";
			cast_b = ")!";
		} else if (mi.ReturnType.IsSubclassOf (TypeCache.System_Delegate)) {
			cast_a = "";
			cast_b = "";
		} else if (mai.Type.IsArray) {
			Type etype = mai.Type.GetElementType ();
			if (minfo is not null && minfo.is_bindAs) {
				var bindAttrType = GetBindAsAttribute (minfo.mi).Type;
				if (!bindAttrType.IsArray) {
					throw new BindingException (1071, true, minfo.mi.DeclaringType.FullName, minfo.mi.Name);
				}
				var bindAsT = bindAttrType.GetElementType ();
				var suffix = string.Empty;
				print ("{0} retvalarrtmp;", NativeHandleType);
				cast_a = "((retvalarrtmp = ";
				cast_b = ") == IntPtr.Zero ? null! : (";
				cast_b += $"NSArray.ArrayFromHandleFunc <{TypeManager.FormatType (bindAsT.DeclaringType, bindAsT)}> (retvalarrtmp, {GetFromBindAsWrapper (minfo, out suffix)})" + suffix;
				cast_b += "))";
			} else if (etype == TypeCache.System_String) {
				cast_a = "CFArray.StringArrayFromHandle (";
				cast_b = ")!";
			} else if (minfo is not null && minfo.protocolize) {
				cast_a = "CFArray.ArrayFromHandle<global::" + etype.Namespace + ".I" + etype.Name + ">(";
				cast_b = ")!";
			} else if (etype == TypeCache.Selector) {
				exceptions.Add (ErrorHelper.CreateError (1066, mai.Type.FullName, mi.DeclaringType.FullName, mi.Name));
			} else {
				if (NamespaceCache.NamespacesThatConflictWithTypes.Contains (etype.Namespace))
					cast_a = "CFArray.ArrayFromHandle<global::" + etype + ">(";
				else
					cast_a = "CFArray.ArrayFromHandle<" + TypeManager.FormatType (mi.DeclaringType, etype) + ">(";
				cast_b = ")!";
			}
		} else if (mi.ReturnType.Namespace == "System" && mi.ReturnType.Name == "nint") {
			cast_a = "(nint) ";
		} else if (mi.ReturnType.Namespace == "System" && mi.ReturnType.Name == "nuint") {
			cast_a = "(nuint) ";
		}
	}

	void GenerateInvoke (bool stret, bool supercall, MethodInfo mi, MemberInformation minfo, string selector, string args, bool assign_to_temp, Type category_type, bool aligned)
	{
		string target_name = (category_type is null && !minfo.is_extension_method) ? "this" : "This";
		string handle = supercall ? ".SuperHandle" : ".Handle";

		// If we have supercall == false, we can be a Bind method that has a [Target]
		if (supercall == false && !minfo.is_static) {
			foreach (var pi in mi.GetParameters ()) {
				if (IsTarget (pi)) {
					if (pi.ParameterType == TypeCache.System_String) {
						var mai = new MarshalInfo (this, mi, pi);

						if (mai.PlainString)
							ErrorHelper.Warning (1101);

						if (mai.ZeroCopyStringMarshal) {
							target_name = "(IntPtr)(&_s" + pi.Name + ")";
							handle = "";
						} else {
							target_name = "ns" + pi.Name;
							handle = "";
						}
					} else
						target_name = pi.Name.GetSafeParamName ();
					break;
				}
			}
		}

		string sig = supercall ? MakeSuperSig (mi, stret, aligned) : MakeSig (mi, stret, aligned);

		sig = "global::" + NamespaceCache.Messaging + "." + sig;

		string selector_field;
		if (minfo.is_interface_impl || minfo.is_extension_method) {
			var tmp = InlineSelectors;
			InlineSelectors = true;
			selector_field = SelectorField (selector);
			InlineSelectors = tmp;
		} else {
			selector_field = SelectorField (selector);
		}

		if (ShouldMarshalNativeExceptions (mi))
			args += ", &exception_gchandle";

		if (stret) {
			string ret_val = aligned ? "(IntPtr*) aligned_ret" : "&ret";
			if (minfo.is_static)
				print ("{0} ({5}, class_ptr, {3}{4});", sig, "/*unusued*/", "/*unusued*/", selector_field, args, ret_val);
			else
				print ("{0} ({5}, {1}{2}, {3}{4});", sig, target_name, handle, selector_field, args, ret_val);

			if (aligned)
				print ("aligned_assigned = true;");
		} else {
			bool returns = mi.ReturnType != TypeCache.System_Void && mi.Name != "Constructor";
			string cast_a = "", cast_b = "";
			StringBuilder postproc = new StringBuilder ();

			if (returns)
				GetReturnsWrappers (mi, minfo, mi.DeclaringType, out cast_a, out cast_b, postproc);
			else if (mi.Name == "Constructor") {
				cast_a = "InitializeHandle (";
				cast_b = ", \"" + selector + "\")";
			}

			if (minfo.is_static)
				print ("{0}{1}{2} (class_ptr, {5}{6}){7};",
					   returns ? (assign_to_temp ? "ret = " : "return ") : "",
					   cast_a, sig, target_name,
					   "/*unusued3*/", //supercall ? "Super" : "",
					   selector_field, args, cast_b);
			else
				print ("{0}{1}{2} ({3}{4}, {5}{6}){7};",
					   returns ? (assign_to_temp ? "ret = " : "return ") : "",
					   cast_a, sig, target_name,
					   handle,
					   selector_field, args, cast_b);

			if (postproc.Length > 0)
				print (postproc.ToString ());
		}
	}

	void GenerateNewStyleInvoke (bool supercall, MethodInfo mi, MemberInformation minfo, string selector, string args, bool assign_to_temp, Type category_type)
	{
		var returnType = mi.ReturnType;
		bool x64_stret = Stret.X86_64NeedStret (returnType, this);
		bool aligned = AttributeManager.HasAttribute<AlignAttribute> (mi);

		if (CurrentPlatform == PlatformName.MacOSX || CurrentPlatform == PlatformName.MacCatalyst) {
			if (x64_stret) {
				print ("if (global::ObjCRuntime.Runtime.IsARM64CallingConvention) {");
				indent++;
				GenerateInvoke (false, supercall, mi, minfo, selector, args, assign_to_temp, category_type, false);
				indent--;
				print ("} else {");
				indent++;
				GenerateInvoke (x64_stret, supercall, mi, minfo, selector, args, assign_to_temp, category_type, aligned && x64_stret);
				indent--;
				print ("}");
			} else {
				GenerateInvoke (false, supercall, mi, minfo, selector, args, assign_to_temp, category_type, false);
			}
			return;
		}

		bool arm_stret = Stret.ArmNeedStret (returnType, this);
		bool x86_stret = Stret.X86NeedStret (returnType, this);
		bool is_stret_multi = arm_stret || x86_stret || x64_stret;
		bool need_multi_path = is_stret_multi;

		if (need_multi_path) {
			if (is_stret_multi) {
				// First check for arm64
				print ("if (global::ObjCRuntime.Runtime.IsARM64CallingConvention) {");
				indent++;
				GenerateInvoke (false, supercall, mi, minfo, selector, args, assign_to_temp, category_type, false);
				indent--;
				// If we're not arm64, but we're 64-bit, then we're x86_64
				print ("} else if (IntPtr.Size == 8) {");
				indent++;
				GenerateInvoke (x64_stret, supercall, mi, minfo, selector, args, assign_to_temp, category_type, aligned && x64_stret);
				indent--;
				// if we're not 64-bit, but we're on device, then we're 32-bit arm
				print ("} else if (Runtime.Arch == Arch.DEVICE) {");
				indent++;
				GenerateInvoke (arm_stret, supercall, mi, minfo, selector, args, assign_to_temp, category_type, aligned && arm_stret);
				indent--;
				// if we're none of the above, we're x86
				print ("} else {");
				indent++;
				GenerateInvoke (x86_stret, supercall, mi, minfo, selector, args, assign_to_temp, category_type, aligned && x86_stret);
				indent--;
				print ("}");
			} else {
				print ("if (IntPtr.Size == 8) {");
				indent++;
				GenerateInvoke (x64_stret, supercall, mi, minfo, selector, args, assign_to_temp, category_type, aligned && x64_stret);
				indent--;
				print ("} else {");
				indent++;
				GenerateInvoke (x86_stret, supercall, mi, minfo, selector, args, assign_to_temp, category_type, aligned && x86_stret);
				indent--;
				print ("}");
			}
		} else {
			GenerateInvoke (false, supercall, mi, minfo, selector, args, assign_to_temp, category_type, false);
		}
	}

	static char [] newlineTab = new char [] { '\n', '\t' };

	void Inject<T> (MethodInfo method) where T : SnippetAttribute
	{
		var snippets = AttributeManager.GetCustomAttributes<T> (method);
		if (snippets.Length == 0)
			return;
		foreach (SnippetAttribute snippet in snippets)
			Inject (snippet);
	}

	void Inject (SnippetAttribute snippet)
	{
		if (snippet.Code is null)
			return;
		var lines = snippet.Code.Split (newlineTab);
		foreach (var l in lines) {
			if (l.Length == 0)
				continue;
			print (l);
		}
	}

	bool IsOptimizable (MemberInfo method)
	{
		var optimizable = true;
		var snippets = AttributeManager.GetCustomAttributes<SnippetAttribute> (method);
		if (snippets.Length > 0) {
			foreach (SnippetAttribute snippet in snippets)
				optimizable &= snippet.Optimizable;
		}
		return optimizable;
	}

	//
	// generates the code to marshal a string from C# to Objective-C:
	//
	// @probe_null: determines whether null is allowed, and
	// whether we need to generate code to handle this
	//
	// @must_copy: determines whether to create a new NSString, necessary
	// for NSString properties that are flagged with "retain" instead of "copy"
	//
	// @prefix: prefix to prepend on each line
	//
	// @property: the name of the property
	//
	public string GenerateMarshalString (bool probe_null, bool must_copy)
	{
		if (must_copy) {
			return "var ns{0} = CFString.CreateNative ({1});\n";
		}
		return
			"ObjCRuntime.NSStringStruct _s{0}; Console.WriteLine (\"" + CurrentMethod + ": Marshalling: {{1}}\", {1}); \n" +
			"_s{0}.ClassPtr = ObjCRuntime.NSStringStruct.ReferencePtr;\n" +
			"_s{0}.Flags = 0x010007d1; // RefCount=1, Unicode, InlineContents = 0, DontFreeContents\n" +
			"_s{0}.UnicodePtr = _p{0};\n" +
			"_s{0}.Length = " + (probe_null ? "{1} is null ? 0 : {1}.Length;" : "{1}.Length;\n");
	}

	public string GenerateDisposeString (bool probe_null, bool must_copy)
	{
		if (must_copy) {
			return "CFString.ReleaseNative (ns{0});\n";
		} else
			return "if (_s{0}.Flags != 0x010007d1) throw new Exception (\"String was retained, not copied\");";
	}

	List<string> CollectFastStringMarshalParameters (MethodInfo mi)
	{
		List<string> stringParameters = null;

		foreach (var pi in mi.GetParameters ()) {
			var mai = new MarshalInfo (this, mi, pi);

			if (mai.ZeroCopyStringMarshal) {
				if (stringParameters is null)
					stringParameters = new List<string> ();
				stringParameters.Add (pi.Name.GetSafeParamName ());
			}
		}
		return stringParameters;
	}

	AvailabilityBaseAttribute GetIntroduced (Type type, string methodName)
	{
		if (type is null)
			return null;

		var prop = type.GetProperties ()
			.Where (pi => pi.Name == methodName)
			.FirstOrDefault ();

		if (prop is not null)
			return prop.GetAvailability (AvailabilityKind.Introduced, this);

		return GetIntroduced (ReflectionExtensions.GetBaseType (type, this), methodName);
	}

	AvailabilityBaseAttribute GetIntroduced (MethodInfo mi, PropertyInfo pi)
	{
		return mi.GetAvailability (AvailabilityKind.Introduced, this) ?? pi.GetAvailability (AvailabilityKind.Introduced, this);
	}

	bool Is64BitiOSOnly (ICustomAttributeProvider provider)
	{
		if (BindThirdPartyLibrary)
			return false;
		if (BindingTouch.CurrentPlatform != PlatformName.iOS)
			return false;
		var attrib = provider.GetAvailability (AvailabilityKind.Introduced, this);
		if (attrib is null) {
			var minfo = provider as MemberInfo;
			if (minfo is not null && minfo.DeclaringType is not null)
				return Is64BitiOSOnly (minfo.DeclaringType);
			return false;
		}
		return attrib.Version?.Major >= 11;
	}

	//
	// Generates the code necessary to lower the MonoTouch-APIs to something suitable
	// to be passed to Objective-C.
	//
	// This turns things like strings into NSStrings, NSObjects into the Handle object,
	// INativeObjects into calling the handle and so on.
	//
	// The result is delivered as a StringBuilder that is used to prepare the marshaling
	// and then one that must be executed after the method has been invoked to cleaup the
	// results
	//
	// @mi: input parameter, contains the method info with the C# signature to generate lowering for
	// @null_allowed_override: this is suitable for applying [NullAllowed] at the property level,
	// and is a convenient override to pass down to this method.   
	//
	// The following are written into, these are expected to be fresh StringBuilders:
	// @args: arguments that should be passed to native
	// @convs: conversions to perform before the invocation
	// @disposes: dispose operations to perform after the invocation
	// @by_ref_processing
	void GenerateTypeLowering (MethodInfo mi, bool null_allowed_override, out StringBuilder args, out StringBuilder convs, out StringBuilder disposes, out StringBuilder by_ref_processing, out StringBuilder by_ref_init, PropertyInfo propInfo = null, bool castEnum = true)
	{
		args = new StringBuilder ();
		convs = new StringBuilder ();
		disposes = new StringBuilder ();
		by_ref_processing = new StringBuilder ();
		by_ref_init = new StringBuilder ();

		foreach (var pi in mi.GetParameters ()) {
			var safe_name = pi.Name.GetSafeParamName ();
			MarshalInfo mai = new MarshalInfo (this, mi, pi);

			if (!IsTarget (pi)) {
				// Construct invocation
				args.Append (", ");
				args.Append (MarshalParameter (mi, pi, null_allowed_override, propInfo, castEnum, convs));

				if (pi.ParameterType.IsByRef) {
					var et = pi.ParameterType.GetElementType ();
					var nullable = TypeManager.GetUnderlyingNullableType (et);
					if (nullable is not null) {
						var nt = TypeManager.FormatType (mi.DeclaringType, nullable);
						convs.Append ($"{nt}* converted_{safe_name} = null;\n");
						convs.Append ($"{nt} converted_{safe_name}_v = default ({nt});\n");
						convs.Append ($"if ({safe_name}.HasValue) {{\n");
						convs.Append ($"\tconverted_{safe_name}_v = {safe_name}.Value;\n");
						convs.Append ($"\tconverted_{safe_name} = &converted_{safe_name}_v;\n");
						convs.Append ("}\n");
						by_ref_processing.AppendLine ($"if ({safe_name}.HasValue)");
						by_ref_processing.AppendLine ($"\t{safe_name} = *converted_{safe_name};");
					}
				}
			}

			// Construct conversions
			if (mai.Type == TypeCache.System_String && !mai.PlainString) {
				bool probe_null = null_allowed_override || AttributeManager.HasAttribute<NullAllowedAttribute> (pi);

				convs.AppendFormat (GenerateMarshalString (probe_null, !mai.ZeroCopyStringMarshal), pi.Name, pi.Name.GetSafeParamName ());
				disposes.AppendFormat (GenerateDisposeString (probe_null, !mai.ZeroCopyStringMarshal), pi.Name);
			} else if (mai.Type.IsArray) {
				Type etype = mai.Type.GetElementType ();
				if (HasBindAsAttribute (pi)) {
					convs.AppendFormat ("var nsb_{0} = {1}\n", pi.Name, GetToBindAsWrapper (mi, null, pi));
					disposes.AppendFormat ("\nnsb_{0}?.Dispose ();", pi.Name);
				} else if (HasBindAsAttribute (propInfo)) {
					disposes.AppendFormat ("\nnsb_{0}?.Dispose ();", propInfo.Name);
				} else if (etype == TypeCache.System_String) {
					if (null_allowed_override || AttributeManager.HasAttribute<NullAllowedAttribute> (pi)) {
						convs.AppendFormat ("var nsa_{0} = {1} is null ? null : NSArray.FromStrings ({1});\n", pi.Name, pi.Name.GetSafeParamName ());
						disposes.AppendFormat ("if (nsa_{0} is not null)\n\tnsa_{0}.Dispose ();\n", pi.Name);
					} else {
						convs.AppendFormat ("var nsa_{0} = NSArray.FromStrings ({1});\n", pi.Name, pi.Name.GetSafeParamName ());
						disposes.AppendFormat ("nsa_{0}.Dispose ();\n", pi.Name);
					}
				} else if (etype == TypeCache.Selector) {
					exceptions.Add (ErrorHelper.CreateError (1065, mai.Type.FullName, string.IsNullOrEmpty (pi.Name) ? $"#{pi.Position}" : pi.Name, mi.DeclaringType.FullName, mi.Name));
				} else {
					if (null_allowed_override || AttributeManager.HasAttribute<NullAllowedAttribute> (pi)) {
						convs.AppendFormat ("var nsa_{0} = {1} is null ? null : NSArray.FromNSObjects ({1});\n", pi.Name, pi.Name.GetSafeParamName ());
						disposes.AppendFormat ("if (nsa_{0} is not null)\n\tnsa_{0}.Dispose ();\n", pi.Name);
					} else {
						convs.AppendFormat ("var nsa_{0} = NSArray.FromNSObjects ({1});\n", pi.Name, pi.Name.GetSafeParamName ());
						disposes.AppendFormat ("nsa_{0}.Dispose ();\n", pi.Name);
					}
				}
			} else if (mai.Type.IsSubclassOf (TypeCache.System_Delegate)) {
				string trampoline_name = MakeTrampoline (pi.ParameterType).StaticName;
				bool null_allowed = AttributeManager.HasAttribute<NullAllowedAttribute> (pi);
				if (!null_allowed) {
					var property = GetProperty (mi);
					if (property is not null)
						null_allowed = AttributeManager.HasAttribute<NullAllowedAttribute> (property);
				}
				var createBlockMethod = null_allowed ? "CreateNullableBlock" : "CreateBlock";
				convs.AppendFormat ("using var block_{0} = Trampolines.{1}.{3} ({2});\n", pi.Name, trampoline_name, safe_name, createBlockMethod);
				if (null_allowed) {
					convs.AppendFormat ("BlockLiteral *block_ptr_{0} = null;\n", pi.Name);
					convs.AppendFormat ("if ({0} is not null)\n", safe_name);
					convs.AppendFormat ("\tblock_ptr_{0} = &block_{0};\n", pi.Name);
				} else {
					convs.AppendFormat ("BlockLiteral *block_ptr_{0} = &block_{0};\n", pi.Name);
				}
			} else if (pi.ParameterType.IsGenericParameter) {
				//				convs.AppendFormat ("{0}.Handle", pi.Name.GetSafeParamName ());
			} else if (HasBindAsAttribute (pi)) {
				convs.AppendFormat ("var nsb_{0} = {1}\n", pi.Name, GetToBindAsWrapper (mi, null, pi));
			} else if (mai.Type.IsPointer && mai.Type.GetElementType ().IsValueType) {
				// nothing to do
			} else {
				if (mai.Type.IsClass && !mai.Type.IsByRef &&
					(mai.Type != TypeCache.Selector && mai.Type != TypeCache.Class && mai.Type != TypeCache.System_String && !TypeCache.INativeObject.IsAssignableFrom (mai.Type)))
					throw new BindingException (1020, true, mai.Type, mi.DeclaringType, mi.Name, mai.Type.IsByRef);
			}

			// Handle ByRef
			if (mai.Type.IsByRef && mai.Type.GetElementType ().IsValueType == false) {
				// For out/ref parameters we support:
				// * string and string[]
				// * NSObject (and subclasses), and array of NSObject (and subclasses)
				// * INativeObject subclasses (but not INativeObject itself), and array of INativeObject subclasses (but not arrays of INativeObject itself)
				// 	* Except that we do not support arrays of Selector
				// Modifications to an array (i.e. changing an element) are not marshalled back.
				// If the array is modified, then a new array instance must be created and assigned to the ref/out parameter for us to marshal back any modifications.
				var elementType = mai.Type.GetElementType ();
				var isString = elementType == TypeCache.System_String;
				var isINativeObject = elementType == TypeCache.INativeObject;
				var isINativeObjectSubclass = !isINativeObject && TypeCache.INativeObject.IsAssignableFrom (elementType);
				var isNSObject = IsNSObject (elementType);
				var isForcedType = HasForcedAttribute (pi, out var isForcedOwns);
				var isArray = elementType.IsArray;
				var isArrayOfString = isArray && elementType.GetElementType () == TypeCache.System_String;
				var isArrayOfNSObject = isArray && IsNSObject (elementType.GetElementType ());
				var isArrayOfINativeObject = isArray && elementType.GetElementType () == TypeCache.INativeObject;
				var isArrayOfINativeObjectSubclass = isArray && TypeCache.INativeObject.IsAssignableFrom (elementType.GetElementType ());
				var isArrayOfSelector = isArray && elementType.GetElementType () == TypeCache.Selector;

				if (!isString && !isArrayOfNSObject && !isNSObject && !isArrayOfString && !isINativeObjectSubclass && !isArrayOfINativeObjectSubclass || isINativeObject || isArrayOfSelector || isArrayOfINativeObject) {
					exceptions.Add (ErrorHelper.CreateError (1064, elementType.FullName, string.IsNullOrEmpty (pi.Name) ? $"#{pi.Position}" : pi.Name, mi.DeclaringType.FullName, mi.Name));
					continue;
				}

				if (pi.IsOut) {
					by_ref_init.AppendFormat ("{1} {0}Value = IntPtr.Zero;\n", pi.Name.GetSafeParamName (), NativeHandleType);
				} else {
					by_ref_init.AppendFormat ("{1} {0}Value = ", pi.Name.GetSafeParamName (), NativeHandleType);
					if (isString) {
						by_ref_init.AppendFormat ("NSString.CreateNative ({0}, true);\n", pi.Name.GetSafeParamName ());
						by_ref_init.AppendFormat ("{1} {0}OriginalValue = {0}Value;\n", pi.Name.GetSafeParamName (), NativeHandleType);
					} else if (isArrayOfNSObject || isArrayOfINativeObjectSubclass) {
						by_ref_init.Insert (0, string.Format ("NSArray {0}ArrayValue = NSArray.FromNSObjects ({0});\n", pi.Name.GetSafeParamName ()));
						by_ref_init.AppendFormat ("{0}ArrayValue is null ? NativeHandle.Zero : {0}ArrayValue.Handle;\n", pi.Name.GetSafeParamName ());
					} else if (isArrayOfString) {
						by_ref_init.Insert (0, string.Format ("NSArray {0}ArrayValue = {0} is null ? null : NSArray.FromStrings ({0});\n", pi.Name.GetSafeParamName ()));
						by_ref_init.AppendFormat ("{0}ArrayValue is null ? NativeHandle.Zero : {0}ArrayValue.Handle;\n", pi.Name.GetSafeParamName ());
					} else if (isNSObject || isINativeObjectSubclass) {
						by_ref_init.AppendFormat ("{0} is null ? NativeHandle.Zero : {0}.Handle;\n", pi.Name.GetSafeParamName ());
					} else {
						throw ErrorHelper.CreateError (88, mai.Type, mi);
					}
				}

				if (isString) {
					if (!pi.IsOut)
						by_ref_processing.AppendFormat ("if ({0}Value != {0}OriginalValue)\n\t", pi.Name.GetSafeParamName ());
					by_ref_processing.AppendFormat ("{0} = CFString.FromHandle ({0}Value)!;\n", pi.Name.GetSafeParamName ());
				} else if (isArray) {
					if (!pi.IsOut)
						by_ref_processing.AppendFormat ("if ({0}Value != ({0}ArrayValue is null ? NativeHandle.Zero : {0}ArrayValue.Handle))\n\t", pi.Name.GetSafeParamName ());

					if (isArrayOfNSObject || isArrayOfINativeObjectSubclass) {
						by_ref_processing.AppendFormat ("{0} = CFArray.ArrayFromHandle<{1}> ({0}Value)!;\n", pi.Name.GetSafeParamName (), TypeManager.RenderType (elementType.GetElementType ()));
					} else if (isArrayOfString) {
						by_ref_processing.AppendFormat ("{0} = CFArray.StringArrayFromHandle ({0}Value)!;\n", pi.Name.GetSafeParamName ());
					} else {
						throw ErrorHelper.CreateError (88, mai.Type, mi);
					}

					if (!pi.IsOut)
						by_ref_processing.AppendFormat ("{0}ArrayValue?.Dispose ();\n", pi.Name.GetSafeParamName ());
				} else if (isNSObject && !isForcedType) {
					by_ref_processing.AppendFormat ("{0} = Runtime.GetNSObject<{1}> ({0}Value)!;\n", pi.Name.GetSafeParamName (), TypeManager.RenderType (elementType));
				} else if (isINativeObjectSubclass) {
					if (!pi.IsOut)
						by_ref_processing.AppendFormat ("if ({0}Value != ({0} is null ? NativeHandle.Zero : {0}.Handle))\n\t", pi.Name.GetSafeParamName ());
					by_ref_processing.AppendFormat ("{0} = Runtime.GetINativeObject<{1}> ({0}Value, {2}, {3})!;\n", pi.Name.GetSafeParamName (), TypeManager.RenderType (elementType), isForcedType ? "true" : "false", isForcedType ? isForcedOwns : "false");
				} else {
					throw ErrorHelper.CreateError (88, mai.Type, mi);
				}
			}
		}
	}

	void GenerateArgumentChecks (MethodInfo mi, bool null_allowed_override, PropertyInfo propInfo = null)
	{
		if (AttributeManager.HasAttribute<NullAllowedAttribute> (mi))
			ErrorHelper.Show (new BindingException (1118, false, mi));

		foreach (var pi in mi.GetParameters ()) {
			var safe_name = pi.Name.GetSafeParamName ();
			bool protocolize = Protocolize (pi);
			if (!BindThirdPartyLibrary) {
				if (!mi.IsSpecialName && IsModel (pi.ParameterType) && !protocolize) {
					// don't warn on obsoleted API, there's likely a new version that fix this
					// any no good reason for using the obsolete API anyway
					if (!AttributeManager.HasAttribute<ObsoleteAttribute> (mi) && !AttributeManager.HasAttribute<ObsoleteAttribute> (mi.DeclaringType))
						ErrorHelper.Warning (1106, mi.DeclaringType, mi.Name, pi.Name, pi.ParameterType, pi.ParameterType.Namespace, pi.ParameterType.Name);
				}
			}

			var needs_null_check = ParameterNeedsNullCheck (pi, mi, propInfo);
			if (protocolize) {
				print ("if ({0} is not null) {{", safe_name);
				print ("\tif (!({0} is NSObject))\n", safe_name);
				print ("\t\tthrow new ArgumentException (\"The object passed of type \" + {0}.GetType () + \" does not derive from NSObject\");", safe_name);
				print ("}");
			}

			var cap = propInfo?.SetMethod == mi ? (ICustomAttributeProvider) propInfo : (ICustomAttributeProvider) pi;
			var bind_as = GetBindAsAttribute (cap);
			var pit = bind_as is null ? pi.ParameterType : bind_as.Type;
			if (TypeManager.IsWrappedType (pit) || TypeCache.INativeObject.IsAssignableFrom (pit)) {
				if (needs_null_check && !null_allowed_override) {
					print ($"var {safe_name}__handle__ = {safe_name}!.GetNonNullHandle (nameof ({safe_name}));");
				} else {
					print ($"var {safe_name}__handle__ = {safe_name}.GetHandle ();");
				}
			} else if (needs_null_check) {
				print ("if ({0} is null)", safe_name);
				print ("\tObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof ({0}));", safe_name);
			}
		}
	}

	// undecorated code is assumed to be iOS 2.0
	static AvailabilityBaseAttribute iOSIntroducedDefault = new IntroducedAttribute (PlatformName.iOS, 2, 0);

	string CurrentMethod;

	void GenerateThreadCheck (StreamWriter sw = null)
	{
		string s;
		switch (CurrentPlatform) {
		case PlatformName.iOS:
		case PlatformName.WatchOS:
		case PlatformName.TvOS:
		case PlatformName.MacCatalyst:
			s = $"global::UIKit.UIApplication.EnsureUIThread ();";
			break;
		case PlatformName.MacOSX:
			s = $"global::AppKit.NSApplication.EnsureUIThread ();";
			break;
		default:
			throw new BindingException (1047, CurrentPlatform);
		}
		if (sw is null)
			print (s);
		else
			sw.WriteLine (s);
	}

	// Stret.NeedStret is shared between generator and X.I dll so in order to wrap the exception
	// into a BindingException we need to set the try/catch here so we can provide a better message. Bugzilla ref 51212.
	bool CheckNeedStret (MethodInfo mi)
	{
		try {
			return Stret.NeedStret (mi.ReturnType, this);
		} catch (TypeLoadException ex) {
			throw new BindingException (0001, true, mi.ReturnType.Name, ex.Message);
		}
	}

	//
	// The NullAllowed can be applied on a property, to avoid the ugly syntax, we allow it on the property
	// So we need to pass this as `null_allowed_override',   This should only be used by setters.
	//
	public void GenerateMethodBody (MemberInformation minfo, MethodInfo mi, string sel, bool null_allowed_override, string var_name, BodyOption body_options, PropertyInfo propInfo = null)
	{
		var type = minfo.type;
		var category_type = minfo.category_extension_type;
		var is_appearance = minfo.is_appearance;
		TrampolineInfo trampoline_info = null;

		CurrentMethod = String.Format ("{0}.{1}", type.Name, mi.Name);

		// Warn about [Static] used in a member of [Category]
		var hasStaticAtt = AttributeManager.HasAttribute<StaticAttribute> (mi);
		if (category_type is not null && hasStaticAtt && !minfo.ignore_category_static_warnings) {
			var baseTypeAtt = AttributeManager.GetCustomAttribute<BaseTypeAttribute> (minfo.type);
			ErrorHelper.Warning (1117, mi.Name, type.FullName, baseTypeAtt?.BaseType.FullName);
		}

		indent++;
		// if the namespace/type needs it and if the member is NOT marked as safe (don't check)
		// if the namespace/type does NOT need it and if the member is marked as NOT safe (do check)
		if (type_needs_thread_checks ? (minfo.threadCheck != ThreadCheck.Off) : (minfo.threadCheck == ThreadCheck.On))
			GenerateThreadCheck ();

		Inject<PrologueSnippetAttribute> (mi);

		GenerateArgumentChecks (mi, false, propInfo);

		// Collect all strings that can be fast-marshalled
		List<string> stringParameters = CollectFastStringMarshalParameters (mi);

		GenerateTypeLowering (mi, null_allowed_override, out var args, out var convs, out var disposes, out var by_ref_processing, out var by_ref_init, propInfo);

		var argsArray = args.ToString ();

		if (by_ref_init.Length > 0)
			print (by_ref_init.ToString ());

		if (stringParameters is not null) {
			print ("fixed (char * {0}){{",
				  stringParameters.Select (name => "_p" + name + " = " + name).Aggregate ((first, second) => first + ", " + second));
			indent++;
		}

		if (propInfo is not null && IsSetter (mi) && HasBindAsAttribute (propInfo)) {
			convs.AppendFormat ("var nsb_{0} = {1}\n", propInfo.Name, GetToBindAsWrapper (mi, minfo, null));
		}

		if (convs.Length > 0)
			print (sw, convs.ToString ());

		Inject<PreSnippetAttribute> (mi);
		var align = AttributeManager.GetCustomAttribute<AlignAttribute> (mi);

		PostGetAttribute [] postget = null;
		// [PostGet] are not needed (and might not be available) when generating methods inside Appearance types
		// However we want them even if ImplementsAppearance is true (i.e. the original type needs them)
		if (!is_appearance) {
			postget = AttributeManager.GetCustomAttributes<PostGetAttribute> (mi);
			if (postget.Length == 0 && propInfo is not null)
				postget = AttributeManager.GetCustomAttributes<PostGetAttribute> (propInfo);

			if (postget is not null && postget.Length == 0)
				postget = null;
		}

		var shouldMarshalNativeExceptions = ShouldMarshalNativeExceptions (mi);
		if (shouldMarshalNativeExceptions)
			print ("IntPtr exception_gchandle = IntPtr.Zero;");

		bool use_temp_return =
			minfo.is_return_release ||
			(mi.Name != "Constructor" && shouldMarshalNativeExceptions && mi.ReturnType != TypeCache.System_Void) ||
			(mi.Name != "Constructor" && (CheckNeedStret (mi) || disposes.Length > 0 || postget is not null) && mi.ReturnType != TypeCache.System_Void) ||
			(AttributeManager.HasAttribute<FactoryAttribute> (mi)) ||
			((body_options & BodyOption.NeedsTempReturn) == BodyOption.NeedsTempReturn) ||
			(mi.ReturnType.IsSubclassOf (TypeCache.System_Delegate)) ||
			(AttributeManager.HasAttribute<ProxyAttribute> (mi.ReturnParameter)) ||
			(IsNativeEnum (mi.ReturnType)) ||
			(mi.ReturnType == TypeCache.System_Boolean) ||
			(mi.ReturnType == TypeCache.System_Char) ||
			(mi.Name != "Constructor" && by_ref_processing.Length > 0 && mi.ReturnType != TypeCache.System_Void);

		if (use_temp_return) {
			// for properties we (most often) put the attribute on the property itself, not the getter/setter methods
			if (mi.ReturnType.IsSubclassOf (TypeCache.System_Delegate)) {
				print ("{0} ret;", NativeHandleType);
				trampoline_info = MakeTrampoline (mi.ReturnType);
			} else if (align is not null) {
				print ("{0} ret = default({0});", TypeManager.FormatType (mi.DeclaringType, mi.ReturnType));
				print ("IntPtr ret_alloced = Marshal.AllocHGlobal (Marshal.SizeOf<{0}> () + {1});", TypeManager.FormatType (mi.DeclaringType, mi.ReturnType), align.Align);
				print ("IntPtr aligned_ret = new IntPtr (((nint) (ret_alloced + {0}) >> {1}) << {1});", align.Align - 1, align.Bits);
				print ("bool aligned_assigned = false;");
			} else if (minfo.protocolize) {
				print ("{0} ret;", TypeManager.FormatType (mi.DeclaringType, mi.ReturnType.Namespace, FindProtocolInterface (mi.ReturnType, mi)));
			} else if (minfo.is_bindAs) {
				var bindAsAttrib = GetBindAsAttribute (minfo.mi);
				// tricky, e.g. when an nullable `NSNumber[]` is bound as a `float[]`, since FormatType and bindAsAttrib have not clue about the original nullability 
				print ("{0} ret;", TypeManager.FormatType (bindAsAttrib.Type.DeclaringType, bindAsAttrib.Type));
			} else if (mi.ReturnType == TypeCache.System_Boolean) {
				print ("byte ret;");
			} else if (mi.ReturnType == TypeCache.System_Char) {
				print ("ushort ret;");
			} else {
				var isClassType = mi.ReturnType.IsClass || mi.ReturnType.IsInterface;
				var nullableReturn = isClassType ? "?" : string.Empty;
				print ("{0}{1} ret;", TypeManager.FormatType (mi.DeclaringType, mi.ReturnType), nullableReturn);
			}
		}

		bool needs_temp = use_temp_return || disposes.Length > 0;
		if (minfo.is_virtual_method || mi.Name == "Constructor") {
			//print ("if (this.GetType () == TypeManager.{0}) {{", type.Name);
			if (external || minfo.is_interface_impl || minfo.is_extension_method) {
				GenerateNewStyleInvoke (false, mi, minfo, sel, argsArray, needs_temp, category_type);
			} else {
				var may_throw = shouldMarshalNativeExceptions;
				var null_handle = may_throw && mi.Name == "Constructor";
				if (null_handle) {
					print ("try {");
					indent++;
				}

				WriteIsDirectBindingCondition (sw, ref indent, is_direct_binding,
								   mi.Name == "Constructor" ? is_direct_binding_value : null, // We only need to print the is_direct_binding value in constructors
											   () => { GenerateNewStyleInvoke (false, mi, minfo, sel, argsArray, needs_temp, category_type); return null; },
											   () => { GenerateNewStyleInvoke (true, mi, minfo, sel, argsArray, needs_temp, category_type); return null; }
								  );

				if (null_handle) {
					indent--;
					print ("} catch {");
					indent++;
					print ("Handle = IntPtr.Zero;");
					print ("throw;");
					indent--;
					print ("}");
				}
			}
		} else {
			GenerateNewStyleInvoke (false, mi, minfo, sel, argsArray, needs_temp, category_type);
		}

		if (shouldMarshalNativeExceptions)
			print ("Runtime.ThrowException (exception_gchandle);");

		if (minfo.is_return_release) {

			// Make sure we generate the required signature in Messaging only if needed 
			// bool_objc_msgSendSuper_IntPtr: for respondsToSelector:
			if (!send_methods.ContainsKey ("void_objc_msgSend")) {
				print (m, "[DllImport (LIBOBJC_DYLIB, EntryPoint=\"objc_msgSendSuper\")]");
				print (m, "public extern static void void_objc_msgSend (IntPtr receiever, IntPtr selector);");
				RegisterMethodName ("void_objc_msgSend");
			}

			print ("if (ret is not null)");
			indent++;
			print ("global::{0}.void_objc_msgSend (ret.Handle, Selector.GetHandle (\"release\"));", NamespaceCache.Messaging);
			indent--;
		}

		Inject<PostSnippetAttribute> (mi);

		if (disposes.Length > 0)
			print (sw, disposes.ToString ());
		if ((body_options & BodyOption.StoreRet) == BodyOption.StoreRet) {
			// nothing to do
		} else if ((body_options & BodyOption.CondStoreRet) == BodyOption.CondStoreRet) {
			// nothing to do
		} else if ((body_options & BodyOption.MarkRetDirty) == BodyOption.MarkRetDirty) {
			print ("MarkDirty ();");
			print ("{0} = ret;", var_name);
		}

		if ((postget is not null) && (postget.Length > 0)) {
			print ("#pragma warning disable 168");
			for (int i = 0; i < postget.Length; i++) {
				if (IsDisableForNewRefCount (postget [i], type))
					continue;


				bool version_check = false;
				if (CurrentPlatform != PlatformName.MacOSX) {
					// bug #7742: if this code, e.g. existing in iOS 2.0, 
					// tries to call a property available since iOS 5.0, 
					// then it will fail when executing in iOS 4.3
					var postget_avail = GetIntroduced (type, postget [i].MethodName);
					if (postget_avail is not null) {
						var caller_avail = GetIntroduced (mi, propInfo) ?? iOSIntroducedDefault;
						if (caller_avail.Version < postget_avail.Version) {
							version_check = true;
							print ("var postget{0} = {4}.UIDevice.CurrentDevice.CheckSystemVersion ({1},{2}) ? {3} : null;",
								i,
								postget_avail.Version.Major,
								postget_avail.Version.Minor,
								postget [i].MethodName,
								"UIKit");
						}
					}
				}
				if (!version_check)
					print ("var postget{0} = {1};", i, postget [i].MethodName);
			}
			print ("#pragma warning restore 168");
		}

		if (AttributeManager.HasAttribute<FactoryAttribute> (mi))
			print ("ret.Release (); // Release implicit ref taken by GetNSObject");
		if (by_ref_processing.Length > 0)
			print (sw, by_ref_processing.ToString ());
		if (use_temp_return) {
			if (AttributeManager.HasAttribute<ProxyAttribute> (mi.ReturnParameter))
				print ("ret.IsDirectBinding = true;");

			if (mi.ReturnType.IsSubclassOf (TypeCache.System_Delegate)) {
				print ("return global::ObjCRuntime.Trampolines.{0}.Create (ret)!;", trampoline_info.NativeInvokerName);
			} else if (align is not null) {
				print ("if (aligned_assigned)");
				indent++;
				print ("unsafe {{ ret = *({0} *) aligned_ret; }}", TypeManager.FormatType (mi.DeclaringType, mi.ReturnType));
				indent--;
				print ("Marshal.FreeHGlobal (ret_alloced);");
				print ("return ret;");
			} else if (mi.ReturnType == TypeCache.System_Boolean) {
				print ("return ret != 0;");
			} else if (mi.ReturnType == TypeCache.System_Char) {
				print ("return (char) ret;");
			} else {
				// we can't be 100% confident that the ObjC API annotations are correct so we always null check inside generated code
				print ("return ret!;");
			}
		}
		if (minfo.is_ctor)
			WriteMarkDirtyIfDerived (sw, mi.DeclaringType);
		if (stringParameters is not null) {
			indent--;
			print ("}");
		}
		indent--;
	}


	PropertyInfo GetProperty (PostGetAttribute @this, Type type)
	{
		if (type is null || type == TypeCache.System_Object)
			return null;

		var props = type.GetProperties ();
		foreach (var pi in props) {
			if (pi.Name != @this.MethodName)
				continue;
			return pi;
		}
		return GetProperty (@this, ReflectionExtensions.GetBaseType (type, this));
	}

	bool IsDisableForNewRefCount (PostGetAttribute @this, Type type)
	{
		PropertyInfo p = GetProperty (@this, type);
		var ea = AttributeManager.GetCustomAttributes<ExportAttribute> (p);
		var sem = ea [0].ArgumentSemantic;
		return (sem != ArgumentSemantic.Assign && sem != ArgumentSemantic.Weak); // also cover UnsafeUnretained
	}

	public IEnumerable<MethodInfo> GetTypeContractMethods (Type source)
	{
		if (source.IsEnum)
			yield break;
		foreach (var method in source.GatherMethods (BindingFlags.Public | BindingFlags.Instance, this))
			yield return method;
		foreach (var parent in source.GetInterfaces ()) {
			// skip interfaces that aren't available on the current platform
			if (parent.IsUnavailable (this))
				continue;
			// skip case where the interface implemented comes from an already built assembly (e.g. monotouch.dll)
			// e.g. Dispose won't have an [Export] since it's present to satisfy System.IDisposable
			if (parent.FullName != "System.IDisposable") {
				foreach (var method in parent.GatherMethods (BindingFlags.Public | BindingFlags.Instance, this)) {
					yield return method;
				}
			}
		}
	}

	public IEnumerable<PropertyInfo> GetTypeContractProperties (Type source)
	{
		foreach (var prop in source.GatherProperties (this))
			yield return prop;
		foreach (var parent in source.GetInterfaces ()) {
			// skip interfaces that aren't available on the current platform
			if (parent.IsUnavailable (this))
				continue;
			// skip case where the interface implemented comes from an already built assembly (e.g. monotouch.dll)
			// e.g. the Handle property won't have an [Export] since it's present to satisfyINativeObject
			if (parent.Name != "INativeObject") {
				foreach (var prop in parent.GatherProperties (this))
					yield return prop;
			}
		}
	}

	//
	// This is used to determine if the memberType is in the declaring type or in any of the
	// inherited versions of the type.   We use this now, since we support inlining protocols
	//
	public bool MemberBelongsToType (Type memberType, Type hostType)
	{
		if (memberType == hostType)
			return true;
		// we also need to inline the base type, e.g. UITableViewDelegate must bring UIScrollViewDelegate
		if (MemberBelongToInterface (memberType, hostType)) {
			return true;
		}
		return false;
	}

	bool MemberBelongToInterface (Type memberType, Type intf)
	{
		if (memberType == intf)
			return true;
		foreach (var p in intf.GetInterfaces ()) {
			if (memberType == p)
				return true;
			if (MemberBelongToInterface (memberType, ReflectionExtensions.GetBaseType (p, this)))
				return true;
		}
		return false;
	}

	Dictionary<string, object> generatedEvents = new Dictionary<string, object> ();
	Dictionary<string, object> generatedDelegates = new Dictionary<string, object> ();

	bool DoesTypeNeedBackingField (Type type)
	{
		return TypeManager.IsWrappedType (type) || (type.IsArray && TypeManager.IsWrappedType (type.GetElementType ()));
	}

	bool DoesPropertyNeedBackingField (PropertyInfo pi)
	{
		return DoesTypeNeedBackingField (pi.PropertyType) && !AttributeManager.HasAttribute<TransientAttribute> (pi);
	}

	bool DoesPropertyNeedDirtyCheck (PropertyInfo pi, ExportAttribute ea)
	{
		switch (ea?.ArgumentSemantic) {
		case ArgumentSemantic.Copy:
		case ArgumentSemantic.Retain: // same as Strong
		case ArgumentSemantic.None:
			return DoesPropertyNeedBackingField (pi);
		default: // Assign (same as UnsafeUnretained) or Weak
			return false;
		}
	}

	void PrintObsoleteAttributes (ICustomAttributeProvider provider, bool already_has_editor_browsable_attribute = false)
	{
		var obsoleteAttributes = AttributeManager.GetCustomAttributes<ObsoleteAttribute> (provider);

		foreach (var oa in obsoleteAttributes) {
			print ("[Obsolete (\"{0}\", {1})]", oa.Message, oa.IsError ? "true" : "false");
		}

		if (!already_has_editor_browsable_attribute && obsoleteAttributes.Any ())
			print ("[EditorBrowsable (EditorBrowsableState.Never)]");
	}

	void PrintPropertyAttributes (PropertyInfo pi, Type type, bool skipTypeInjection = false)
	{
		PrintObsoleteAttributes (pi);

		foreach (var ba in AttributeManager.GetCustomAttributes<DebuggerBrowsableAttribute> (pi))
			print ("[DebuggerBrowsable (DebuggerBrowsableState.{0})]", ba.State);

		foreach (var da in AttributeManager.GetCustomAttributes<DebuggerDisplayAttribute> (pi)) {
			var narg = da.Name is not null ? string.Format (", Name = \"{0}\"", da.Name) : string.Empty;
			var targ = da.Type is not null ? string.Format (", Type = \"{0}\"", da.Type) : string.Empty;
			print ("[DebuggerDisplay (\"{0}\"{1}{2})]", da.Value, narg, targ);
		}
		foreach (var oa in AttributeManager.GetCustomAttributes<OptionalImplementationAttribute> (pi)) {
			print ("[DebuggerBrowsable (DebuggerBrowsableState.Never)]");
		}

		// if we inline properties (e.g. from a protocol)
		// we must look if the type has an [Availability] attribute
		if (type != pi.DeclaringType) {
			// print, if not duplicated from the type (being inlined into), the property availability
			if (!PrintPlatformAttributes (pi, type) && !skipTypeInjection) {
				// print, if not duplicated from the type (being inlined into), the property declaring type (protocol) availability
				PrintPlatformAttributes (pi.DeclaringType, type);
			}
		} else {
			PrintPlatformAttributes (pi);
		}

		foreach (var sa in AttributeManager.GetCustomAttributes<ThreadSafeAttribute> (pi))
			print (sa.Safe ? "[ThreadSafe]" : "[ThreadSafe (false)]");
	}

	void GenerateProperty (Type type, PropertyInfo pi, List<string> instance_fields_to_clear_on_dispose, bool is_model, bool is_interface_impl = false)
	{
		string wrap;
		var export = GetExportAttribute (pi, out wrap);
		var minfo = new MemberInformation (this, this, pi, type, is_interface_impl);
		bool use_underscore = minfo.is_unified_internal;
		var mod = minfo.GetVisibility ();
		Type inlinedType = pi.DeclaringType == type ? null : type;
		minfo.protocolize = Protocolize (pi);
		GetAccessorInfo (pi, out var getter, out var setter, out var generate_getter, out var generate_setter);

		var nullable = !pi.PropertyType.IsValueType && AttributeManager.HasAttribute<NullAllowedAttribute> (pi);

		// So we don't hide the get or set of a parent property with the same name, we need to see if we have a parent declaring the same property
		PropertyInfo parentBaseType = TypeManager.GetParentTypeWithSameNamedProperty (ReflectionExtensions.GetBaseTypeAttribute (type, this), pi.Name);

		// If so, we're not static, and we can't both read and write, but they can
		if (!minfo.is_static && !(pi.CanRead && pi.CanWrite) && (parentBaseType is not null && parentBaseType.CanRead && parentBaseType.CanWrite)) {
			// Make sure the selector matches, sanity check that we aren't hiding something of a different type
			// We skip this for wrap'ed properties, as those get complicated to resolve the correct export
			if (wrap is null &&
				((pi.CanRead && (GetGetterExportAttribute (pi).Selector != GetGetterExportAttribute (parentBaseType).Selector)) ||
					pi.CanWrite && (GetSetterExportAttribute (pi).Selector != GetSetterExportAttribute (parentBaseType).Selector))) {
				throw new BindingException (1035, true, pi.Name, type, parentBaseType.DeclaringType);
			}
			// Then let's not write out our copy, since we'll reduce visibility
			return;
		}

		if (!BindThirdPartyLibrary) {
			var elType = pi.PropertyType.IsArray ? pi.PropertyType.GetElementType () : pi.PropertyType;

			if (IsModel (elType) && !minfo.protocolize) {
				ErrorHelper.Warning (1110, pi.DeclaringType, pi.Name, pi.PropertyType, pi.PropertyType.Namespace, pi.PropertyType.Name);
			}
		}

		if (wrap is not null) {
			print_generated_code ();
			PrintPropertyAttributes (pi, minfo.type);
			PrintAttributes (pi, preserve: true, advice: true);
			print ("{0} {1}{2}{3} {4}{5} {{",
				   mod,
				   minfo.GetModifiers (),
				   (minfo.protocolize ? "I" : "") + TypeManager.FormatType (pi.DeclaringType, pi.PropertyType),
				   nullable ? "?" : String.Empty,
					pi.Name.GetSafeParamName (),
				   use_underscore ? "_" : "");
			indent++;
			if (generate_getter) {
#if !NET
				PrintAttributes (pi, platform: true);
#endif
				PrintAttributes (pi.GetGetMethod (), platform: true, preserve: true, advice: true);
				print ("get {");
				indent++;

				if (TypeManager.IsDictionaryContainerType (pi.PropertyType)) {
					print ("var src = {0} is not null ? new NSMutableDictionary ({0}) : null;", wrap);
					print ("return src is null ? null! : new {0}(src);", TypeManager.FormatType (pi.DeclaringType, pi.PropertyType));
				} else {
					if (TypeManager.IsArrayOfWrappedType (pi.PropertyType))
						print ("return NSArray.FromArray<{0}>({1} as NSArray);", TypeManager.FormatType (pi.DeclaringType, pi.PropertyType.GetElementType ()), wrap);
					else if (pi.PropertyType.IsValueType)
						print ("return ({0}) ({1});", TypeManager.FormatType (pi.DeclaringType, pi.PropertyType), wrap);
					else
						print ("return ({0} as {1}{2})!;", wrap, minfo.protocolize ? "I" : String.Empty, TypeManager.FormatType (pi.DeclaringType, pi.PropertyType));
				}
				indent--;
				print ("}");
			}
			if (generate_setter) {
#if !NET
				PrintAttributes (pi, platform: true);
#endif
				PrintAttributes (pi.GetSetMethod (), platform: true, preserve: true, advice: true);
				print ("set {");
				indent++;

				var is_protocol_wrapper = IsProtocolInterface (pi.PropertyType, false);

				if (minfo.protocolize || is_protocol_wrapper) {
					print ("var rvalue = value as NSObject;");
					print ("if (!(value is null) && rvalue is null)");
					print ("\tthrow new ArgumentException (\"The object passed of type \" + value.GetType () + \" does not derive from NSObject\");");
				}

				if (TypeManager.IsDictionaryContainerType (pi.PropertyType))
					print ("{0} = value.GetDictionary ()!;", wrap);
				else {
					if (TypeManager.IsArrayOfWrappedType (pi.PropertyType))
						print ("{0} = NSArray.FromNSObjects (value);", wrap);
					else
						print ("{0} = {1}value;", wrap, minfo.protocolize || is_protocol_wrapper ? "r" : "");
				}
				indent--;
				print ("}");
			}

			indent--;
			print ("}\n");
			return;
		}

		string var_name = null;

		// [Model] has properties that only throws, so there's no point in adding unused backing fields
		if (!is_model && DoesPropertyNeedBackingField (pi) && !is_interface_impl && !minfo.is_static && !DoesPropertyNeedDirtyCheck (pi, export)) {
			var_name = string.Format ("__mt_{0}_var{1}", pi.Name, minfo.is_static ? "_static" : "");

			print_generated_code ();

			if (minfo.is_thread_static)
				print ("[ThreadStatic]");
			print ("{1}object? {0};", var_name, minfo.is_static ? "static " : "");

			if (!minfo.is_static && !is_interface_impl) {
				instance_fields_to_clear_on_dispose.Add (var_name);
			}
		}

		print_generated_code (optimizable: IsOptimizable (pi));
		PrintPropertyAttributes (pi, minfo.type);

		PrintAttributes (pi, preserve: true, advice: true, bindAs: true);

		string propertyTypeName;
		if (minfo.protocolize) {
			propertyTypeName = FindProtocolInterface (pi.PropertyType, pi);
		} else if (minfo.is_bindAs) {
			var bindAsAttrib = GetBindAsAttribute (minfo.mi);
			propertyTypeName = TypeManager.FormatType (bindAsAttrib.Type.DeclaringType, bindAsAttrib.Type);
			// it remains nullable only if the BindAs type can be null (i.e. a reference type)
			nullable = !bindAsAttrib.Type.IsValueType && AttributeManager.HasAttribute<NullAllowedAttribute> (pi);
		} else {
			propertyTypeName = TypeManager.FormatType (pi.DeclaringType, pi.PropertyType);
		}

		print ("{0} {1}{2}{3} {4}{5} {{",
			   mod,
			   minfo.GetModifiers (),
			   propertyTypeName,
				nullable ? "?" : "",
				pi.Name.GetSafeParamName (),
			   use_underscore ? "_" : "");
		indent++;

		if (minfo.has_inner_wrap_attribute) {
			// If property getter or setter has its own WrapAttribute we let the user do whatever their heart desires
			if (generate_getter) {
				PrintAttributes (pi, platform: true);
				PrintAttributes (pi.GetGetMethod (), platform: true, preserve: true, advice: true);
				print ("get {");
				indent++;

				print ($"return {minfo.wpmi.WrapGetter};");

				indent--;
				print ("}");
			}
			if (generate_setter) {
				var not_implemented_attr = AttributeManager.GetCustomAttribute<NotImplementedAttribute> (setter);

				PrintAttributes (pi, platform: true);
				PrintAttributes (setter, platform: true, preserve: true, advice: true, notImplemented: true);
				print ("set {");
				indent++;

				if (not_implemented_attr is not null)
					print ("throw new NotImplementedException ({0});", not_implemented_attr.Message is null ? "" : $@"""{not_implemented_attr.Message}""");
				else
					print ($"{minfo.wpmi.WrapSetter};");

				indent--;
				print ("}");
			}
			indent--;
			print ("}\n");
			return;
		}

		if (generate_getter) {
			var ba = GetBindAttribute (getter);
			string sel = ba is not null ? ba.Selector : export.Selector;

			// print availability separately since we could be inlining
#if !NET
			PrintPlatformAttributes (pi, type);
#endif

			if (!minfo.is_sealed || !minfo.is_wrapper) {
				PrintDelegateProxy (pi.GetGetMethod ());
				PrintExport (minfo, sel, export.ArgumentSemantic);
			}

			PrintAttributes (pi.GetGetMethod (), platform: true, preserve: true, advice: true, notImplemented: true, inlinedType: inlinedType);
#if NET
			if (false) {
#else
			if (minfo.is_abstract) {
				print ("get; ");
#endif
			} else {
				print ("get {");
				var is32BitNotSupported = Is64BitiOSOnly (pi);
				if (is32BitNotSupported) {
					print ("#if ARCH_32");
					print ("\tthrow new PlatformNotSupportedException (\"This API is not supported on this version of iOS\");");
					print ("#else");
				}
				if (debug)
					print ("Console.WriteLine (\"In {0}\");", pi.GetGetMethod ());
				if (is_model)
					print ("\tthrow new ModelNotImplementedException ();");
				else if (minfo.is_abstract)
					print ("throw new You_Should_Not_Call_base_In_This_Method ();");
				else {
					if (minfo.is_autorelease) {
						indent++;
						print ("using (var autorelease_pool = new NSAutoreleasePool ()) {");
					}
					if (is_interface_impl || !DoesPropertyNeedBackingField (pi)) {
						GenerateMethodBody (minfo, getter, sel, false, null, BodyOption.None, pi);
					} else if (minfo.is_static) {
						GenerateMethodBody (minfo, getter, sel, false, var_name, BodyOption.StoreRet, pi);
					} else {
						if (DoesPropertyNeedDirtyCheck (pi, export))
							GenerateMethodBody (minfo, getter, sel, false, var_name, BodyOption.CondStoreRet, pi);
						else
							GenerateMethodBody (minfo, getter, sel, false, var_name, BodyOption.MarkRetDirty, pi);
					}
					if (minfo.is_autorelease) {
						print ("}");
						indent--;
					}
				}
				if (is32BitNotSupported)
					print ("#endif");
				print ("}\n");
			}
		}
		if (generate_setter) {
			var ba = GetBindAttribute (setter);
			bool null_allowed = AttributeManager.HasAttribute<NullAllowedAttribute> (setter);
			if (null_allowed)
				ErrorHelper.Show (new BindingException (1118, false, setter));
			null_allowed |= AttributeManager.HasAttribute<NullAllowedAttribute> (pi);
			var not_implemented_attr = AttributeManager.GetCustomAttribute<NotImplementedAttribute> (setter);
			string sel;

			if (ba is null) {
				sel = GetSetterExportAttribute (pi)?.Selector;
			} else {
				sel = ba.Selector;
			}

			PrintBlockProxy (pi.PropertyType);

			// print availability separately since we could be inlining
#if !NET
			PrintPlatformAttributes (pi, type);
#endif

			if (not_implemented_attr is null && (!minfo.is_sealed || !minfo.is_wrapper))
				PrintExport (minfo, sel, export.ArgumentSemantic);

			PrintAttributes (pi.GetSetMethod (), platform: true, preserve: true, advice: true, notImplemented: true, inlinedType: inlinedType);
#if NET
			if (false) {
#else
			if (minfo.is_abstract) {
				print ("set; ");
#endif
			} else {
				print ("set {");
				var is32BitNotSupported = Is64BitiOSOnly (pi);
				if (is32BitNotSupported) {
					print ("#if ARCH_32");
					print ("\tthrow new PlatformNotSupportedException (\"This API is not supported on this version of iOS\");");
					print ("#else");
				}
				if (debug)
					print ("Console.WriteLine (\"In {0}\");", pi.GetSetMethod ());

				// If we're doing a setter for a weak property that is protocolized event back
				// we need to put in a check to verify you aren't stomping the "internal underscore"
				// generated delegate. We check CheckForEventAndDelegateMismatches global to disable the checks
				if (!BindThirdPartyLibrary && pi.Name.StartsWith ("Weak", StringComparison.Ordinal)) {
					string delName = pi.Name.Substring (4);
					if (SafeIsProtocolizedEventBacked (delName, type))
						print ("\t{0}.EnsureDelegateAssignIsNotOverwritingInternalDelegate ({1}, value, {2});", ApplicationClassName, string.IsNullOrEmpty (var_name) ? "null" : var_name, GetDelegateTypePropertyName (delName));
				}

				if (not_implemented_attr is not null) {
					print ("\tthrow new NotImplementedException ({0});", not_implemented_attr.Message is null ? "" : "\"" + not_implemented_attr.Message + "\"");
				} else if (is_model)
					print ("\tthrow new ModelNotImplementedException ();");
				else if (minfo.is_abstract)
					print ("throw new You_Should_Not_Call_base_In_This_Method ();");
				else {
					GenerateMethodBody (minfo, setter, sel, null_allowed, null, BodyOption.None, pi);
					if (!minfo.is_static && !is_interface_impl && DoesPropertyNeedBackingField (pi)) {
						if (!DoesPropertyNeedDirtyCheck (pi, export)) {
							print ("\tMarkDirty ();");
							print ("\t{0} = value;", var_name);
						}
					}
				}
				if (is32BitNotSupported)
					print ("#endif");
				print ("}");
			}
		}
		indent--;
		print ("}}\n", pi.Name.GetSafeParamName ());
	}

	string GetReturnType (AsyncMethodInfo minfo)
	{
		if (minfo.IsVoidAsync)
			return "Task";
		var ttype = GetAsyncTaskType (minfo);
		if (minfo.HasNSError && (ttype == "bool"))
			ttype = "Tuple<bool,NSError>";
		return "Task<" + ttype + ">";
	}

	string GetAsyncTaskType (AsyncMethodInfo minfo)
	{
		if (minfo.IsSingleArgAsync)
			return TypeManager.FormatType (minfo.type, minfo.AsyncCompletionParams [0].ParameterType);

		var attr = AttributeManager.GetCustomAttribute<AsyncAttribute> (minfo.mi);
		if (attr.ResultTypeName is not null)
			return attr.ResultTypeName;
		if (attr.ResultType is not null)
			return TypeManager.FormatType (minfo.type, attr.ResultType);

		//Console.WriteLine ("{0}", minfo.MethodInfo.GetParameters ().Last ().ParameterType);
		throw new BindingException (1077, true, minfo.mi);
	}

	string GetInvokeParamList (ParameterInfo [] parameters, bool suffix = true, bool force = false)
	{
		StringBuilder sb = new StringBuilder ();
		bool comma = false;
		foreach (var pi in parameters) {
			if (comma) {
				sb.Append (", ");
			}
			comma = true;
			sb.Append (pi.Name.GetSafeParamName ());
			if (suffix)
				sb.Append ('_');
			if (force)
				sb.Append ('!');
		}
		return sb.ToString ();
	}

	void PrintAsyncHeader (AsyncMethodInfo minfo, AsyncMethodKind asyncKind)
	{
		print_generated_code ();
		string extra = "";

		if (asyncKind == AsyncMethodKind.WithResultOutParameter) {
			if (minfo.Method.GetParameters ().Count () > 1)
				extra = ", ";
			extra += "out " + TypeManager.FormatType (minfo.MethodInfo.DeclaringType, minfo.MethodInfo.ReturnType) + " " + minfo.GetUniqueParamName ("result");
		}

		// async wrapper don't have to be abstract (it's counter productive)
		var modifier = minfo.GetModifiers ().Replace ("abstract ", String.Empty);

		print ("{0} {1}{2} {3}",
			   minfo.GetVisibility (),
			   modifier,
			   GetReturnType (minfo),
			   MakeSignature (minfo, true, minfo.AsyncInitialParams, extra),
			   minfo.is_abstract ? ";" : "");
	}

	void GenerateAsyncMethod (MemberInformation original_minfo, AsyncMethodKind asyncKind)
	{
		var mi = original_minfo.Method;
		var minfo = new AsyncMethodInfo (this, this, original_minfo.type, mi, original_minfo.category_extension_type, original_minfo.is_extension_method);
		var is_void = mi.ReturnType == TypeCache.System_Void;

		// Print a error if any of the method parameters or handler parameters is ref/out, it should not be asyncified.
		if (minfo.AsyncInitialParams is not null) {
			foreach (var param in minfo.AsyncInitialParams) {
				if (param.ParameterType.IsByRef) {
					throw new BindingException (1062, true, original_minfo.type.Name, mi.Name);
				}
			}
		}
		foreach (var param in minfo.AsyncCompletionParams) {
			if (param.ParameterType.IsByRef) {
				throw new BindingException (1062, true, original_minfo.type.Name, mi.Name);
			}
		}

		PrintMethodAttributes (minfo);

		PrintAsyncHeader (minfo, asyncKind);

		print ("{");
		indent++;

		var ttype = "bool";
		var tuple = false;
		if (!minfo.IsVoidAsync) {
			ttype = GetAsyncTaskType (minfo);
			tuple = (minfo.HasNSError && (ttype == "bool"));
			if (tuple)
				ttype = "Tuple<bool,NSError>";
		}
		print ("var tcs = new TaskCompletionSource<{0}> ();", ttype);
		bool ignoreResult = !is_void &&
			asyncKind == AsyncMethodKind.Plain &&
			AttributeManager.GetCustomAttribute<AsyncAttribute> (mi).PostNonResultSnippet is null;
		print ("{6}{5}{4}{0}({1}{2}({3}) => {{",
			mi.Name,
			GetInvokeParamList (minfo.AsyncInitialParams, false),
			minfo.AsyncInitialParams.Length > 0 ? ", " : "",
			GetInvokeParamList (minfo.AsyncCompletionParams),
			minfo.is_extension_method || minfo.is_category_extension ? "This." : string.Empty,
			is_void || ignoreResult ? string.Empty : minfo.GetUniqueParamName ("result") + " = ",
			is_void || ignoreResult ? string.Empty : (asyncKind == AsyncMethodKind.WithResultOutParameter ? string.Empty : "var ")
		);

		indent++;

		int nesting_level = 1;
		if (minfo.HasNSError && !tuple) {
			var var_name = minfo.AsyncCompletionParams.Last ().Name.GetSafeParamName (); ;
			print ("if ({0}_ is not null)", var_name);
			print ("\ttcs.SetException (new NSErrorException({0}_));", var_name);
			print ("else");
			++nesting_level; ++indent;
		}

		if (minfo.IsVoidAsync)
			print ("tcs.SetResult (true);");
		else if (tuple) {
			var cond_name = minfo.AsyncCompletionParams [0].Name;
			var var_name = minfo.AsyncCompletionParams.Last ().Name;
			print ("tcs.SetResult (new Tuple<bool,NSError> ({0}_, {1}_));", cond_name, var_name);
		} else if (minfo.IsSingleArgAsync)
			print ("tcs.SetResult ({0}_!);", minfo.AsyncCompletionParams [0].Name);
		else
			print ("tcs.SetResult (new {0} ({1}));",
				GetAsyncTaskType (minfo),
				GetInvokeParamList (minfo.HasNSError ? minfo.AsyncCompletionParams.DropLast () : minfo.AsyncCompletionParams, true, true));
		indent -= nesting_level;
		if (is_void || ignoreResult)
			print ("});");
		else
			print ("})!;");
		var attr = AttributeManager.GetCustomAttribute<AsyncAttribute> (mi);
		if (asyncKind == AsyncMethodKind.Plain && !string.IsNullOrEmpty (attr.PostNonResultSnippet))
			print (attr.PostNonResultSnippet);
		print ("return tcs.Task;");
		indent--;
		print ("}\n");


		if (attr.ResultTypeName is not null) {
			if (minfo.HasNSError)
				async_result_types.Add (new Tuple<string, ParameterInfo []> (attr.ResultTypeName, minfo.AsyncCompletionParams.DropLast ()));
			else
				async_result_types.Add (new Tuple<string, ParameterInfo []> (attr.ResultTypeName, minfo.AsyncCompletionParams));
		}
	}

	void PrintMethodAttributes (MemberInformation minfo)
	{
		MethodInfo mi = minfo.Method;
		var editor_browsable_attribute = false;

		foreach (var sa in AttributeManager.GetCustomAttributes<ThreadSafeAttribute> (mi))
			print (sa.Safe ? "[ThreadSafe]" : "[ThreadSafe (false)]");

		foreach (var ea in AttributeManager.GetCustomAttributes<EditorBrowsableAttribute> (mi)) {
			if (ea.State == EditorBrowsableState.Always) {
				print ("[EditorBrowsable]");
			} else {
				print ("[EditorBrowsable (EditorBrowsableState.{0})]", ea.State);
			}
			editor_browsable_attribute = true;
		}

		PrintObsoleteAttributes (mi, editor_browsable_attribute);

		if (minfo.is_return_release)
			print ("[return: ReleaseAttribute ()]");

		// when we inline methods (e.g. from a protocol) 
		if (minfo.type != minfo.Method.DeclaringType) {
			// we must look if the type has an [Availability] attribute
			// but we must not duplicate existing attributes for a platform, see https://github.com/xamarin/xamarin-macios/issues/7194
			PrintPlatformAttributesNoDuplicates (minfo.type, minfo.Method);
		} else {
			PrintPlatformAttributes (minfo.Method);
		}

		// in theory we could check for `minfo.is_ctor` but some manual bindings are using methods for `init*`
		if (AttributeManager.HasAttribute<DesignatedInitializerAttribute> (mi))
			print ("[DesignatedInitializer]");
	}


	void GenerateMethod (Type type, MethodInfo mi, bool is_model, Type category_extension_type, bool is_appearance, bool is_interface_impl = false, bool is_extension_method = false, string selector = null, bool isBaseWrapperProtocolMethod = false)
	{
		var minfo = new MemberInformation (this, this, mi, type, category_extension_type, is_interface_impl, is_extension_method, is_appearance, is_model, selector, isBaseWrapperProtocolMethod);
		GenerateMethod (minfo);
	}

	void PrintDelegateProxy (MemberInformation minfo)
	{
		PrintDelegateProxy (minfo.Method);
	}

	void PrintDelegateProxy (MethodInfo mi)
	{
		if (mi.ReturnType.IsSubclassOf (TypeCache.System_Delegate)) {
			var ti = MakeTrampoline (mi.ReturnType);
			print ("[return: DelegateProxy (typeof (ObjCRuntime.Trampolines.{0}))]", ti.StaticName);
		}
	}

	void PrintBlockProxy (Type type)
	{
		if (type.IsSubclassOf (TypeCache.System_Delegate)) {
			var ti = MakeTrampoline (type);
			print ("[param: BlockProxy (typeof (ObjCRuntime.Trampolines.{0}))]", ti.NativeInvokerName);
		}
	}

	void PrintExport (MemberInformation minfo)
	{
		if (minfo.is_export)
			print ("[Export (\"{0}\"{1})]", minfo.selector, minfo.is_variadic ? ", IsVariadic = true" : string.Empty);
	}

	void PrintExport (MemberInformation minfo, ExportAttribute ea)
	{
		PrintExport (minfo, ea.Selector, ea.ArgumentSemantic);
	}

	void PrintExport (MemberInformation minfo, string sel, ArgumentSemantic semantic)
	{
		bool output_semantics = semantic != ArgumentSemantic.None;
		// it does not make sense on every properties, depending on the their types
		if (output_semantics && (minfo.mi is PropertyInfo)) {
			var t = minfo.Property.PropertyType;
			output_semantics = !t.IsPrimitive || t == TypeCache.System_IntPtr;
		}

		if (output_semantics)
			print ("[Export (\"{0}\", ArgumentSemantic.{1})]", sel, semantic);
		else
			print ("[Export (\"{0}\")]", sel);
	}

	void GenerateMethod (MemberInformation minfo)
	{
		var mi = minfo.Method;

		// skip if we provide a manual implementation that would conflict with the generated code
		if (AttributeManager.HasAttribute<ManualAttribute> (mi))
			return;

		foreach (ParameterInfo pi in mi.GetParameters ())
			if (AttributeManager.HasAttribute<RetainAttribute> (pi)) {
				print ("#pragma warning disable 168");
				print ("{0}? __mt_{1}_{2};", pi.ParameterType, mi.Name, pi.Name);
				print ("#pragma warning restore 168");
			}

		int argCount = 0;
		if (minfo.is_export && !minfo.is_variadic) {
			foreach (char c in minfo.selector) {
				if (c == ':')
					argCount++;
			}
			if (minfo.Method.GetParameters ().Length != argCount) {
				ErrorHelper.Warning (1105,
					minfo.selector, argCount, minfo.Method, minfo.Method.GetParameters ().Length);
			}
		}

		PrintDelegateProxy (minfo);

		if (AttributeManager.HasAttribute<NoMethodAttribute> (minfo.mi)) {
			// Call for side effect
			MakeSignature (minfo);
			return;
		}

		PrintExport (minfo);

		if (!minfo.is_interface_impl) {
			PrintMethodAttributes (minfo);
		} else if (minfo.is_return_release) {
			print ("[return: ReleaseAttribute ()]");
		}

		var mod = minfo.GetVisibility ();

#if NET
		var is_abstract = false;
		var do_not_call_base = minfo.is_abstract || minfo.is_model;
#else
		var is_abstract = minfo.is_abstract;
		var do_not_call_base = minfo.is_model;
#endif
		print_generated_code (optimizable: IsOptimizable (minfo.mi));
		print ("{0} {1}{2}{3}",
			   mod,
			   minfo.GetModifiers (),
			   MakeSignature (minfo),
			   is_abstract ? ";" : "");


		if (!is_abstract) {
			if (minfo.is_ctor) {
				indent++;
				print (": {0}", minfo.wrap_method is null ? "base (NSObjectFlag.Empty)" : minfo.wrap_method);
				indent--;
			}

			print ("{");

			var is32BitNotSupported = Is64BitiOSOnly ((ICustomAttributeProvider) minfo.Method ?? minfo.Property);
			if (is32BitNotSupported) {
				print ("#if ARCH_32");
				print ("\tthrow new PlatformNotSupportedException (\"This API is not supported on this version of iOS\");");
				print ("#else");
			}
			if (debug)
				print ("\tConsole.WriteLine (\"In {0}\");", mi);

			if (do_not_call_base)
				print ("\tthrow new You_Should_Not_Call_base_In_This_Method ();");
			else if (minfo.wrap_method is not null) {
				if (!minfo.is_ctor) {
					indent++;

					string ret = mi.ReturnType == TypeCache.System_Void ? null : "return ";
					print ("{0}{1}{2};", ret, minfo.is_extension_method ? "This." : "", minfo.wrap_method);
					indent--;
				}
			} else {
				if (minfo.is_autorelease) {
					indent++;
					print ("using (var autorelease_pool = new NSAutoreleasePool ()) {");
				}
				PropertyInfo pinfo = null;
				MethodInfo method = minfo.Method;
				bool null_allowed = false;
				if (method.IsSpecialName) {
					// could be a property setter where [NullAllowed] is _allowed_
					// we do not need the information if it's a getter (it won't change generated code)
					pinfo = GetProperty (method, getter: false, setter: true);
					if (pinfo is not null)
						null_allowed = AttributeManager.HasAttribute<NullAllowedAttribute> (pinfo);
				}
				GenerateMethodBody (minfo, minfo.Method, minfo.selector, null_allowed, null, BodyOption.None, pinfo);
				if (minfo.is_autorelease) {
					print ("}");
					indent--;
				}
			}
			if (is32BitNotSupported)
				print ("#endif");
			print ("}\n");
		}

		if (AttributeManager.HasAttribute<AsyncAttribute> (mi)) {
			// We do not want Async methods inside internal wrapper classes, they are useless
			// internal sealed class FooWrapper : BaseWrapper, IMyFooDelegate
			// Also we do not want Async members inside [Model] classes
			if (minfo.is_basewrapper_protocol_method || minfo.is_model)
				return;

			GenerateAsyncMethod (minfo, AsyncMethodKind.Plain);

			// Generate the overload with the out parameter
			if (minfo.Method.ReturnType != TypeCache.System_Void) {
				GenerateAsyncMethod (minfo, AsyncMethodKind.WithResultOutParameter);
			}
		}
	}

	static PropertyInfo GetProperty (MethodInfo method, bool getter = true, bool setter = true)
	{
		var props = method.DeclaringType.GetProperties ();
		if (method.GetParameters ().Length == 0)
			return !getter ? null : props.FirstOrDefault (prop => prop.GetGetMethod () == method);
		else
			return !setter ? null : props.FirstOrDefault (prop => prop.GetSetMethod () == method);
	}

	public string GetGeneratedTypeName (Type type)
	{
		var bindOnType = AttributeManager.GetCustomAttributes<BindAttribute> (type);
		if (bindOnType.Length > 0)
			return bindOnType [0].Selector;
		else if (type.IsGenericTypeDefinition)
			return type.Name.Substring (0, type.Name.IndexOf ('`'));
		else
			return type.Name;
	}

	void RenderDelegates (Dictionary<string, MethodInfo> delegateTypes)
	{
		// Group the delegates by namespace
		var groupedTypes = from fullname in delegateTypes.Keys
						   where fullname != "System.Action"
						   let p = fullname.LastIndexOf ('.')
						   let ns = p == -1 ? String.Empty : fullname.Substring (0, p)
						   group fullname by ns into g
						   select new { Namespace = g.Key, Fullname = g };

		foreach (var group in groupedTypes.OrderBy (v => v.Namespace, StringComparer.Ordinal)) {
			if (group.Namespace is not null) {
				print ("namespace {0} {{", group.Namespace);
				indent++;
			}
			print ("#nullable enable");

			foreach (var deltype in group.Fullname.OrderBy (v => v, StringComparer.Ordinal)) {
				int p = deltype.LastIndexOf ('.');
				var shortName = deltype.Substring (p + 1);
				var mi = delegateTypes [deltype];

				if (shortName.StartsWith ("Func<", StringComparison.Ordinal))
					continue;

				var del = mi.DeclaringType;

				if (AttributeManager.HasAttribute (mi.DeclaringType, "MonoNativeFunctionWrapper"))
					print ("[MonoNativeFunctionWrapper]\n");

				var accessibility = mi.DeclaringType.IsInternal (this) ? "internal" : "public";
				print ("{3} delegate {0} {1} ({2});",
					   TypeManager.RenderType (mi.ReturnType, mi.ReturnTypeCustomAttributes),
					   shortName,
					   RenderParameterDecl (mi.GetParameters ()),
					   accessibility);
			}

			if (group.Namespace is not null) {
				indent--;
				print ("}\n");
			}
		}
	}

	IEnumerable<MethodInfo> SelectProtocolMethods (Type type, bool? @static = null, bool? required = null)
	{
		var list = type.GetMethods (BindingFlags.Public | BindingFlags.Instance);

		foreach (var m in list) {
			if (m.IsSpecialName)
				continue;

			if (m.Name == "Constructor")
				continue;

			var attrs = AttributeManager.GetCustomAttributes<Attribute> (m);
			AvailabilityBaseAttribute availabilityAttribute = null;
			bool hasExportAttribute = false;
			bool hasStaticAttribute = false;

			foreach (var a in attrs) {
				if (a is AvailabilityBaseAttribute) {
					availabilityAttribute = a as AvailabilityBaseAttribute;
				} else if (a is ExportAttribute)
					hasExportAttribute = true;
				else if (a is StaticAttribute)
					hasStaticAttribute = true;
			}
			if (availabilityAttribute is not null && m.IsUnavailable (this))
				continue;

			if (!hasExportAttribute)
				continue;

			if (@static.HasValue && @static.Value != hasStaticAttribute)
				continue;

			if (required.HasValue && required.Value != IsRequired (m, attrs))
				continue;

			yield return m;
		}
	}

	IEnumerable<PropertyInfo> SelectProtocolProperties (Type type, bool? @static = null, bool? required = null)
	{
		var list = type.GetProperties (BindingFlags.Public | BindingFlags.Instance);

		foreach (var p in list) {
			var attrs = AttributeManager.GetCustomAttributes<Attribute> (p);
			AvailabilityBaseAttribute availabilityAttribute = null;
			bool hasExportAttribute = false;
			bool hasStaticAttribute = false;

			foreach (var a in attrs) {
				if (a is AvailabilityBaseAttribute) {
					availabilityAttribute = a as AvailabilityBaseAttribute;
				} else if (a is ExportAttribute)
					hasExportAttribute = true;
				else if (a is StaticAttribute)
					hasStaticAttribute = true;
			}
			if (availabilityAttribute is not null && p.IsUnavailable (this))
				continue;

			if (!hasExportAttribute) {
				var getter = p.GetGetMethod ();
				if (p.CanRead && !AttributeManager.HasAttribute<ExportAttribute> (getter)) {
					if (!AttributeManager.HasAttribute<NotImplementedAttribute> (getter))
						continue;
				}
				var setter = p.GetSetMethod ();
				if (p.CanWrite && !AttributeManager.HasAttribute<ExportAttribute> (setter)) {
					if (!AttributeManager.HasAttribute<NotImplementedAttribute> (setter))
						continue;
				}
			}

			if (@static.HasValue && @static.Value != hasStaticAttribute)
				continue;

			if (required.HasValue && required.Value != IsRequired (p, attrs))
				continue;

			yield return p;
		}
	}

	// If a type comes from the assembly with the api definition we're processing.
	bool IsApiType (Type type)
	{
		return type.Assembly == types [0].Assembly;
	}

	bool IsRequired (MemberInfo provider, Attribute [] attributes = null)
	{
		var type = provider.DeclaringType;
		if (IsApiType (type)) {
			return AttributeManager.HasAttribute<AbstractAttribute> (provider, attributes);
		}
		if (type.IsInterface)
			return true;

		return false;
	}

	void GenerateProtocolTypes (Type type, string class_visibility, string TypeName, string protocol_name, ProtocolAttribute protocolAttribute)
	{
		var allProtocolMethods = new List<MethodInfo> ();
		var allProtocolProperties = new List<PropertyInfo> ();
		var ifaces = (IEnumerable<Type>) type.GetInterfaces ().Concat (new Type [] { ReflectionExtensions.GetBaseType (type, this) }).OrderBy (v => v.FullName, StringComparer.Ordinal);

		if (type.Namespace is not null) {
			print ("namespace {0} {{", type.Namespace);
			indent++;
		}

		ifaces = ifaces.Where ((v) => IsProtocolInterface (v, false));

		allProtocolMethods.AddRange (SelectProtocolMethods (type));
		allProtocolProperties.AddRange (SelectProtocolProperties (type));

		var requiredInstanceMethods = allProtocolMethods.Where ((v) => IsRequired (v) && !AttributeManager.HasAttribute<StaticAttribute> (v)).ToList ();
		var optionalInstanceMethods = allProtocolMethods.Where ((v) => !IsRequired (v) && !AttributeManager.HasAttribute<StaticAttribute> (v));
		var requiredInstanceProperties = allProtocolProperties.Where ((v) => IsRequired (v) && !AttributeManager.HasAttribute<StaticAttribute> (v)).ToList ();
		var optionalInstanceProperties = allProtocolProperties.Where ((v) => !IsRequired (v) && !AttributeManager.HasAttribute<StaticAttribute> (v));
		var requiredInstanceAsyncMethods = requiredInstanceMethods.Where (m => AttributeManager.HasAttribute<AsyncAttribute> (m)).ToList ();

		PrintAttributes (type, platform: true, preserve: true, advice: true);
		print ("[Protocol (Name = \"{1}\", WrapperType = typeof ({0}Wrapper){2}{3})]",
			   TypeName,
			   protocol_name,
			   protocolAttribute.IsInformal ? ", IsInformal = true" : string.Empty,
			   protocolAttribute.FormalSince is not null ? $", FormalSince = \"{protocolAttribute.FormalSince}\"" : string.Empty);

		var sb = new StringBuilder ();

		foreach (var mi in allProtocolMethods) {
			var attrib = GetExportAttribute (mi);
			sb.Clear ();
			sb.Append ("[ProtocolMember (");
			sb.Append ("IsRequired = ").Append (IsRequired (mi) ? "true" : "false");
			sb.Append (", IsProperty = false");
			sb.Append (", IsStatic = ").Append (AttributeManager.HasAttribute<StaticAttribute> (mi) ? "true" : "false");
			sb.Append (", Name = \"").Append (mi.Name).Append ("\"");
			sb.Append (", Selector = \"").Append (attrib.Selector).Append ("\"");
			if (mi.ReturnType != TypeCache.System_Void) {
				var retType = mi.ReturnType;
				sb.Append (", ReturnType = typeof (").Append (TypeManager.RenderType (retType)).Append (")");
				if (retType.IsSubclassOf (TypeCache.System_Delegate)) {
					var ti = MakeTrampoline (retType);
					sb.Append ($", ReturnTypeDelegateProxy = typeof (ObjCRuntime.Trampolines.{ti.StaticName})");
				}
			}
			var parameters = mi.GetParameters ();
			if (parameters is not null && parameters.Length > 0) {
				sb.Append (", ParameterType = new Type [] { ");
				for (int i = 0; i < parameters.Length; i++) {
					if (i > 0)
						sb.Append (", ");
					sb.Append ("typeof (");
					var pt = parameters [i].ParameterType;
					if (pt.IsByRef)
						pt = pt.GetElementType ();
					sb.Append (TypeManager.RenderType (pt));
					sb.Append (")");
				}
				sb.Append (" }");
				sb.Append (", ParameterByRef = new bool [] { ");
				for (int i = 0; i < parameters.Length; i++) {
					if (i > 0)
						sb.Append (", ");
					sb.Append (parameters [i].ParameterType.IsByRef ? "true" : "false");
				}
				sb.Append (" }");

				var blockProxies = new string [parameters.Length];
				var anyblockProxy = false;
				for (int i = 0; i < parameters.Length; i++) {
					var parType = parameters [i].ParameterType;
					if (parType.IsSubclassOf (TypeCache.System_Delegate)) {
						var ti = MakeTrampoline (parType);
						blockProxies [i] = $"typeof (ObjCRuntime.Trampolines.{ti.NativeInvokerName})";
						anyblockProxy = true;
					}
				}
				if (anyblockProxy) {
					sb.Append (", ParameterBlockProxy = new Type? [] { ");
					for (int i = 0; i < blockProxies.Length; i++) {
						if (i > 0)
							sb.Append (", ");
						sb.Append (blockProxies [i] is null ? "null" : blockProxies [i]);
					}
					sb.Append (" }");
				}

				if (attrib.IsVariadic)
					sb.Append (", IsVariadic = true");
			}
			sb.Append (")]");
			print (sb.ToString ());
		}
		foreach (var pi in allProtocolProperties) {
			var attrib = GetExportAttribute (pi);
			sb.Clear ();
			sb.Append ("[ProtocolMember (");
			sb.Append ("IsRequired = ").Append (IsRequired (pi) ? "true" : "false");
			sb.Append (", IsProperty = true");
			sb.Append (", IsStatic = ").Append (AttributeManager.HasAttribute<StaticAttribute> (pi) ? "true" : "false");
			sb.Append (", Name = \"").Append (pi.Name).Append ("\"");
			sb.Append (", Selector = \"").Append (attrib.Selector).Append ("\"");
			sb.Append (", PropertyType = typeof (").Append (TypeManager.RenderType (pi.PropertyType)).Append (")");
			if (pi.CanRead && !AttributeManager.HasAttribute<NotImplementedAttribute> (pi.GetGetMethod ())) {
				var ea = GetGetterExportAttribute (pi);
				var ba = GetBindAttribute (pi.GetGetMethod ());
				sb.Append (", GetterSelector = \"").Append (ba is not null ? ba.Selector : ea.Selector).Append ("\"");
			}
			if (pi.CanWrite && !AttributeManager.HasAttribute<NotImplementedAttribute> (pi.GetSetMethod ())) {
				var ea = GetSetterExportAttribute (pi);
				var ba = GetBindAttribute (pi.GetSetMethod ());
				sb.Append (", SetterSelector = \"").Append (ba is not null ? ba.Selector : ea.Selector).Append ("\"");
			}
			sb.Append (", ArgumentSemantic = ArgumentSemantic.").Append (attrib.ArgumentSemantic);
			// Check for block/delegate proxies
			var propType = pi.PropertyType;
			if (propType.IsSubclassOf (TypeCache.System_Delegate)) {
				var ti = MakeTrampoline (propType);
				if (pi.SetMethod is not null)
					sb.Append ($", ParameterBlockProxy = new Type [] {{ typeof (ObjCRuntime.Trampolines.{ti.NativeInvokerName}) }}");
				if (pi.GetMethod is not null)
					sb.Append ($", ReturnTypeDelegateProxy = typeof (ObjCRuntime.Trampolines.{ti.StaticName})");
			}
			sb.Append (")]");
			print (sb.ToString ());
		}

		PrintXpcInterfaceAttribute (type);
		print ("{0} partial interface I{1} : INativeObject, IDisposable{2}", class_visibility, TypeName, ifaces.Count () > 0 ? ", " : string.Empty);
		indent++;
		sb.Clear ();
		foreach (var iface in ifaces) {
			string iname = iface.Name;
			// if the [Protocol] interface is defined in the binding itself it won't start with an 'I'
			// but if it's a reference to something inside an already built assembly, 
			// e.g. monotouch.dll, then the 'I' will be present
			if (sb.Length > 0)
				sb.Append (", ");
			sb.Append (iface.Namespace).Append ('.');
			if (iface.Assembly.GetName ().Name == "temp")
				sb.Append ('I');
			else if (iface.IsClass)
				sb.Append ('I');
			sb.AppendLine (iname);
		}
		if (sb.Length > 0)
			print (sb.ToString ());
		indent--;

		print ("{");
		indent++;
		foreach (var mi in requiredInstanceMethods) {
			if (AttributeManager.HasAttribute<StaticAttribute> (mi))
				continue;

			var minfo = new MemberInformation (this, this, mi, type, null);
			var mod = string.Empty;

			PrintMethodAttributes (minfo);
			print_generated_code ();
			PrintDelegateProxy (minfo);
			PrintExport (minfo);
			print ("[Preserve (Conditional = true)]");
			if (minfo.is_unsafe)
				mod = "unsafe ";
			print ("{0}{1};", mod, MakeSignature (minfo, true));
			print ("");
		}

		foreach (var pi in requiredInstanceProperties) {
			var minfo = new MemberInformation (this, this, pi, type);
			var mod = string.Empty;
			minfo.is_export = true;

			print ("[Preserve (Conditional = true)]");
			PrintAttributes (pi, platform: true);

			if (minfo.is_unsafe)
				mod = "unsafe ";
			// IsValueType check needed for `IntPtr` signatures (which can't become `IntPtr?`)
			var nullable = !pi.PropertyType.IsValueType && AttributeManager.HasAttribute<NullAllowedAttribute> (pi) ? "?" : String.Empty;
			GetAccessorInfo (pi, out var getMethod, out var setMethod, out var generate_getter, out var generate_setter);
			print ("{0}{1}{2} {3} {{", mod, TypeManager.FormatType (type, pi.PropertyType), nullable, pi.Name, generate_getter ? "get;" : string.Empty, generate_setter ? "set;" : string.Empty);
			indent++;
			if (generate_getter) {
				var ea = GetGetterExportAttribute (pi);
				// there can be a [Bind] there that override the selector name to be used
				// e.g. IMTLTexture.FramebufferOnly
				var ba = GetBindAttribute (getMethod);
				PrintDelegateProxy (getMethod);
				if (!AttributeManager.HasAttribute<NotImplementedAttribute> (getMethod)) {
					if (ba is not null)
						PrintExport (minfo, ba.Selector, ea.ArgumentSemantic);
					else
						PrintExport (minfo, ea);
				}
				PrintAttributes (getMethod, notImplemented: true, platform: true);
				print ("get;");
			}
			if (generate_setter) {
				PrintBlockProxy (pi.PropertyType);
				PrintAttributes (setMethod, notImplemented: true, platform: true);
				if (!AttributeManager.HasAttribute<NotImplementedAttribute> (setMethod))
					PrintExport (minfo, GetSetterExportAttribute (pi));
				print ("set;");
			}
			indent--;
			print ("}");
			print ("");
		}
		indent--;
		print ("}");
		print ("");

		// avoid (for unified) all the metadata for empty static classes, we can introduce them later when required
		bool include_extensions = false;
		include_extensions = optionalInstanceMethods.Any () || optionalInstanceProperties.Any () || requiredInstanceAsyncMethods.Any ();
		if (include_extensions) {
			// extension methods
			PrintAttributes (type, preserve: true, advice: true);
			print ("{1} unsafe static partial class {0}_Extensions {{", TypeName, class_visibility);
			indent++;
			foreach (var mi in optionalInstanceMethods)
				GenerateMethod (type, mi, false, null, false, false, true);

			// Generate Extension Methods of required [Async] decorated methods (we already do optional) 
			foreach (var ami in requiredInstanceAsyncMethods) {
				var minfo = new MemberInformation (this, this, ami, type, null, isExtensionMethod: true);
				GenerateAsyncMethod (minfo, AsyncMethodKind.Plain);

				// Generate the overload with the out parameter
				if (minfo.Method.ReturnType != TypeCache.System_Void)
					GenerateAsyncMethod (minfo, AsyncMethodKind.WithResultOutParameter);
			}

			// C# does not support extension properties, so create Get* and Set* accessors instead.
			foreach (var pi in optionalInstanceProperties) {
				GetAccessorInfo (pi, out var getter, out var setter, out var generate_getter, out var generate_setter);
				var attrib = GetExportAttribute (pi);
				if (generate_getter) {
					PrintAttributes (pi, preserve: true, advice: true);
					var ba = GetBindAttribute (getter);
					GenerateMethod (type, getter, false, null, false, false, true, ba?.Selector ?? attrib.ToGetter (pi).Selector);
				}
				if (generate_setter) {
					PrintAttributes (pi, preserve: true, advice: true);
					var ba = GetBindAttribute (setter);
					GenerateMethod (type, setter, false, null, false, false, true, ba?.Selector ?? attrib.ToSetter (pi).Selector);
				}
			}
			indent--;
			print ("}");
			print ("");
		}

		// Add API from base interfaces we also need to implement.
		foreach (var iface in ifaces) {
			requiredInstanceMethods.AddRange (SelectProtocolMethods (iface, @static: false, required: true));
			requiredInstanceProperties.AddRange (SelectProtocolProperties (iface, @static: false, required: true));
		}

		PrintPreserveAttribute (type);
		print ("internal unsafe sealed class {0}Wrapper : BaseWrapper, I{0} {{", TypeName);
		indent++;
		// ctor (IntPtr, bool)
		print ("[Preserve (Conditional = true)]");
		print ("public {0}Wrapper ({1} handle, bool owns)", TypeName, NativeHandleType);
		print ("\t: base (handle, owns)");
		print ("{");
		print ("}");
		print ("");
		// Methods
		// First find duplicates and select the best one. We use the selector to determine what's a duplicate.
		var methodData = requiredInstanceMethods.Select ((v) => {
			return new MethodData {
				Method = v,
				ExportAttribute = GetExportAttribute (v),
			};
		}).ToArray ();
		var methodsGroupedBySelector = methodData.GroupBy ((v) => v.ExportAttribute.Selector);
		var duplicateMethodsGroupedBySelector = methodsGroupedBySelector.Where ((v) => v.Count () > 1);
		if (duplicateMethodsGroupedBySelector.Any ()) {
			// Yes, there are multiple methods with the same selector. Now we must compute the signature for all of them.
			var computeSignature = new Func<MethodInfo, string> ((MethodInfo minfo) => {
				var sig = new StringBuilder ();
				sig.Append (minfo.Name).Append (" (");
				foreach (var param in minfo.GetParameters ())
					sig.Append (param.ParameterType.FullName).Append (' ');
				sig.Append (')');
				return sig.ToString ();
			});
			foreach (var gr in duplicateMethodsGroupedBySelector)
				foreach (var m in gr)
					m.Signature = computeSignature (m.Method);

			// Verify that all the duplicate methods have the same signature.
			// If not, show an error, and if they all have the same signature, remove all but one from the list of methods to generate.
			foreach (var gr in duplicateMethodsGroupedBySelector) {
				var distinctMethodsBySignature = gr.GroupBy ((v) => v.Signature).Select ((v) => v.First ()).ToArray ();
				if (distinctMethodsBySignature.Length > 1) {
					exceptions.Add (ErrorHelper.CreateError (1069, type.FullName, gr.Key, distinctMethodsBySignature [0].Method.DeclaringType.FullName, distinctMethodsBySignature [1].Method.DeclaringType.FullName,
						distinctMethodsBySignature [0].Method.ToString (), distinctMethodsBySignature [1].Method.ToString ()));
					continue;
				}

				// Remove all but the first method from the list required instance methods
				var exclude = gr.Skip (1).Select ((v) => v.Method).ToArray ();
				requiredInstanceMethods.RemoveAll ((v) => exclude.Contains (v));
			}

		}
		// Go generate code for the methods
		foreach (var mi in requiredInstanceMethods) {
			GenerateMethod (type, mi, false, null, false, true, isBaseWrapperProtocolMethod: true);
		}

		// Properties
		// First find duplicates and select the best one. Here we use the property name to find duplicates.
		var propertyDuplicates = requiredInstanceProperties.GroupBy ((v) => v.Name).Where ((v) => v.Count () > 1).ToArray ();
		if (propertyDuplicates.Any ()) {
			foreach (var gr in propertyDuplicates) {
				// Check that all the selectors in the group are identical
				var properties = gr.ToArray ();
				var exportAttributes = new ExportAttribute [properties.Length];
				PropertyInfo readwrite = null;
				PropertyInfo @readonly = null;
				PropertyInfo @writeonly = null;
				for (var i = 0; i < properties.Length; i++) {
					// Check that all the duplicates use the same selector, and if not show a warning.
					exportAttributes [i] = GetExportAttribute (properties [i]);
					if (i > 0 && exportAttributes [i].Selector != exportAttributes [0].Selector) {
						ErrorHelper.Warning (1068, type.FullName, gr.Key, properties [0].DeclaringType.FullName, properties [i].DeclaringType.FullName,
							properties [0].DeclaringType.Name, properties [0].Name, exportAttributes [0].Selector, properties [i].DeclaringType.Name, properties [i].Name, exportAttributes [i].Selector);
					}
					if (properties [i].CanRead && properties [i].CanWrite) {
						readwrite = properties [i];
					} else if (!properties [i].CanRead) {
						writeonly = properties [i];
					} else if (!properties [i].CanWrite) {
						@readonly = properties [i];
					}
				}

				// Check that all the duplicates have the same property type
				var propertyTypes = properties.GroupBy ((v) => v.PropertyType.FullName).Select ((v) => v.First ()).ToArray ();
				if (propertyTypes.Length > 1) {
					exceptions.Add (ErrorHelper.CreateError (1070, type.FullName, gr.Key, propertyTypes [0].DeclaringType.FullName, propertyTypes [1].DeclaringType.FullName,
						FormatPropertyInfo (propertyTypes [0]), TypeManager.FormatType (type, propertyTypes [0].PropertyType), FormatPropertyInfo (propertyTypes [1]), TypeManager.FormatType (type, propertyTypes [0].PropertyType)));
				}

				// Select the best match of the properties: prefer a read/write property if it exists, otherwise a readonly or writeonly property.
				PropertyInfo bestMatch;
				if (readwrite is not null) {
					bestMatch = readwrite;
				} else if (@readonly is not null && writeonly is not null) {
					exceptions.Add (ErrorHelper.CreateError (1067, type.FullName, gr.Key, @readonly.DeclaringType.FullName, writeonly.DeclaringType.FullName,
						FormatPropertyInfo (@readonly), FormatPropertyInfo (writeonly)));
					continue;
				} else if (@readonly is not null) {
					bestMatch = @readonly;
				} else if (writeonly is not null) {
					bestMatch = writeonly;
				} else {
					exceptions.Add (ErrorHelper.CreateError (89, properties [0]));
					continue;
				}
				// Finally remove the properties we don't want to generate.
				var exclude = properties.Where ((v) => v != bestMatch);
				requiredInstanceProperties.RemoveAll ((v) => exclude.Contains (v));
			}
		}
		foreach (var pi in requiredInstanceProperties) {
			GenerateProperty (type, pi, null, false, true);
		}
		indent--;
		print ("}");

		if (type.Namespace is not null) {
			indent--;
			print ("}");
		}
	}

	void GetAccessorInfo (PropertyInfo pi, out MethodInfo getter, out MethodInfo setter, out bool generate_getter, out bool generate_setter)
	{
		getter = null;
		setter = null;
		generate_getter = false;
		generate_setter = false;
		if (pi.CanRead) {
			getter = pi.GetGetMethod ();
			generate_getter = !getter.IsUnavailable (this);
		}
		if (pi.CanWrite) {
			setter = pi.GetSetMethod ();
			generate_setter = !setter.IsUnavailable (this);
		}
	}

	class MethodData {
		public MethodInfo Method;
		public ExportAttribute ExportAttribute;
		public string Signature;
	}

	bool ConformToNSCoding (Type type)
	{
		foreach (var intf in type.GetInterfaces ()) {
			if (intf.Name == "NSCoding" || intf.Name == "INSCoding")
				return true;
		}
		// [BaseType (x)] might implement NSCoding... and this means we need the .ctor(NSCoder)
		var attrs = AttributeManager.GetCustomAttributes<BaseTypeAttribute> (type);
		if (attrs is null || attrs.Length == 0)
			return false;
		return ConformToNSCoding (attrs [0].BaseType);
	}

	StreamWriter GetOutputStream (string @namespace, string name)
	{
		var dir = basedir;

		if (!string.IsNullOrEmpty (@namespace))
			dir = Path.Combine (dir, @namespace);

		var filename = Path.Combine (dir, name + ".g.cs");
		var counter = 2;
		while (generated_files.Contains (filename)) {
			filename = Path.Combine (dir, name + counter.ToString () + ".g.cs");
			counter++;
		}
		generated_files.Add (filename);
		if (!Directory.Exists (dir))
			Directory.CreateDirectory (dir);

		return new StreamWriter (filename);
	}


	StreamWriter GetOutputStreamForType (Type type)
	{
		if (type.Namespace is null)
			ErrorHelper.Warning (1103, type.FullName);

		var tn = GetGeneratedTypeName (type);
		if (type.IsGenericType)
			tn = tn + "_" + type.GetGenericArguments ().Length.ToString ();
		return GetOutputStream (type.Namespace, tn);
	}

	void WriteMarkDirtyIfDerived (StreamWriter sw, Type type)
	{
		if (type.Name == "CALayer" && "CoreAnimation" == type.Namespace) {
			sw.WriteLine ("\t\t\tMarkDirtyIfDerived ();");
		}
	}

	public void PrintXpcInterfaceAttribute (ICustomAttributeProvider mi)
	{
		if (!AttributeManager.HasAttribute<XpcInterfaceAttribute> (mi))
			return;

		print ("[XpcInterface]");
	}

	// Function to check if PreserveAttribute is present and
	// generate/print the same attribute as in bindings
	public void PrintPreserveAttribute (ICustomAttributeProvider mi)
	{
		var p = AttributeManager.GetCustomAttribute<PreserveAttribute> (mi);
		if (p is null)
			return;

		if (p.AllMembers)
			print ("[Preserve (AllMembers = true)]");
		else if (p.Conditional)
			print ("[Preserve (Conditional = true)]");
		else
			print ("[Preserve]");
	}

	// Function to check if PrintAdviceAttribute is present and
	// generate/print the same attribute as in bindings
	public void PrintAdviceAttribute (ICustomAttributeProvider mi)
	{
		var p = AttributeManager.GetCustomAttribute<AdviceAttribute> (mi);
		if (p is null)
			return;

		print ($"[Advice ({p.Message.Quote ()})]");
	}

	public void PrintRequiresSuperAttribute (ICustomAttributeProvider mi)
	{
		var p = AttributeManager.GetCustomAttribute<RequiresSuperAttribute> (mi);
		if (p is null)
			return;

		print ("[RequiresSuper]");
	}

	public void PrintNotImplementedAttribute (ICustomAttributeProvider mi)
	{
		var p = AttributeManager.GetCustomAttribute<NotImplementedAttribute> (mi);
		if (p is null)
			return;

		print ($"[NotImplemented ({p.Message.Quote ()})]");
	}

	public void PrintBindAsAttribute (ICustomAttributeProvider mi, StringBuilder sb = null)
	{
		var p = GetBindAsAttribute (mi);
		if (p is null)
			return;

		var property = mi as PropertyInfo;
		var method = mi as MethodInfo;
		var param = mi as ParameterInfo;
		var originalType = method?.ReturnType ?? property?.PropertyType;
		originalType = originalType ?? param?.ParameterType;

		var declaringType = (mi as MemberInfo)?.DeclaringType ?? param.Member.DeclaringType;
		var pReturn = method is not null ? "return: " : string.Empty;

		var attribstr = $"[{pReturn}BindAs (typeof ({TypeManager.FormatType (declaringType, p.Type)}), OriginalType = typeof ({TypeManager.FormatType (declaringType, originalType)}))]";

		if (sb is not null)
			sb.Append ($"{attribstr} ");
		else
			print (attribstr);
	}

	public void PrintAttributes (ICustomAttributeProvider mi, bool platform = false, bool preserve = false, bool advice = false, bool notImplemented = false, bool bindAs = false, bool requiresSuper = false, Type inlinedType = null)
	{
		if (platform)
			PrintPlatformAttributes (mi as MemberInfo, inlinedType);
		if (preserve)
			PrintPreserveAttribute (mi);
		if (advice)
			PrintAdviceAttribute (mi);
		if (notImplemented)
			PrintNotImplementedAttribute (mi);
		if (bindAs)
			PrintBindAsAttribute (mi);
		if (requiresSuper)
			PrintRequiresSuperAttribute (mi);
	}

	public void ComputeLibraryName (FieldAttribute fieldAttr, Type type, string propertyName, out string library_name, out string library_path)
	{
		library_path = null;

		if (fieldAttr is not null && fieldAttr.LibraryName is not null) {
			// Remapped
			library_name = fieldAttr.LibraryName;
			if (library_name [0] == '+') {
				switch (library_name) {
				case "+CoreImage":
					library_name = CoreImageMap;
					break;
				case "+CoreServices":
					library_name = CoreServicesMap;
					break;
				case "+PDFKit":
					library_name = "PdfKit";
					library_path = PDFKitMap;
					break;
				}
			} else {
				// we get something in LibraryName from FieldAttribute so we asume
				// it is a path to a library, so we save the path and change library name
				// to a valid identifier if needed
				library_path = library_name;
				if (BindThirdPartyLibrary /* without extension makes more sense, but we can't change it since it breaks compat */) {
					library_name = Path.GetFileName (library_name);
				} else {
					library_name = Path.GetFileNameWithoutExtension (library_name);
				}
				if (library_name.Contains ('.'))
					library_name = library_name.Replace (".", string.Empty);
			}
		} else if (BindThirdPartyLibrary) {
			// User should provide a LibraryName
			throw ErrorHelper.CreateError (1042, /* Missing '[Field (LibraryName=value)]' for {0} (e.g."__Internal") */ type.FullName + "." + propertyName);
		} else {
			library_name = type.Namespace;
		}

		if (!libraries.ContainsKey (library_name))
			libraries.Add (library_name, library_path);
	}

	public string GetSelector (MemberInfo mi)
	{
		object [] attr = AttributeManager.GetCustomAttributes<ExportAttribute> (mi);
		if (attr.Length != 1) {
			attr = AttributeManager.GetCustomAttributes<BindAttribute> (mi);
			if (attr.Length != 1) {
				return null;
			} else {
				BindAttribute ba = (BindAttribute) attr [0];
				return ba.Selector;
			}
		} else {
			ExportAttribute ea = (ExportAttribute) attr [0];
			return ea.Selector;
		}
	}

	string GetAssemblyName ()
	{
		if (BindThirdPartyLibrary)
			return Path.GetFileNameWithoutExtension (BindingTouch.outfile);

		switch (BindingTouch.TargetFramework.Platform) {
		case ApplePlatform.iOS:
			return BindingTouch.IsDotNet ? "Microsoft.iOS" : "Xamarin.iOS";
		case ApplePlatform.MacOSX:
			return BindingTouch.IsDotNet ? "Microsoft.macOS" : "Xamarin.Mac";
		case ApplePlatform.TVOS:
			return BindingTouch.IsDotNet ? "Microsoft.tvOS" : "Xamarin.TVOS";
		case ApplePlatform.WatchOS:
			return BindingTouch.IsDotNet ? "Microsoft.watchOS" : "Xamarin.WatchOS";
		case ApplePlatform.MacCatalyst:
			return "Microsoft.MacCatalyst";
		default:
			throw ErrorHelper.CreateError (1053, /* Internal error: unknown target framework '{0}'. */ BindingTouch.TargetFramework);
		}
	}

	public void Generate (Type type)
	{
		if (ZeroCopyStrings) {
			ErrorHelper.Warning (1027);
			ZeroCopyStrings = false;
		}

		type_wants_zero_copy = AttributeManager.HasAttribute<ZeroCopyStringsAttribute> (type) || ZeroCopyStrings;
		var tsa = AttributeManager.GetCustomAttribute<ThreadSafeAttribute> (type);
		// if we're inside a special namespace then default is non-thread safe, otherwise default is thread safe
		if (NamespaceCache.UINamespaces.Contains (type.Namespace)) {
			// Any type inside these namespaces requires, by default, a thread check
			// unless it has a [ThreadSafe] or [ThreadSafe (true)] attribute
			type_needs_thread_checks = tsa is null || !tsa.Safe;
		} else {
			// Any type outside these namespaces do NOT require a thread check
			// unless it has a [ThreadSafe (false)] attribute
			type_needs_thread_checks = tsa is not null && !tsa.Safe;
		}

		string TypeName = GetGeneratedTypeName (type);
		indent = 0;
		var instance_fields_to_clear_on_dispose = new List<string> ();
		var gtype = GeneratedTypes.Lookup (type);
		var appearance_selectors = gtype.ImplementsAppearance ? gtype.AppearanceSelectors : null;

		using (var sw = GetOutputStreamForType (type)) {
			this.sw = sw;
			bool is_category_class = AttributeManager.HasAttribute<CategoryAttribute> (type);
			bool is_static_class = AttributeManager.HasAttribute<StaticAttribute> (type) || is_category_class;
			bool is_partial = AttributeManager.HasAttribute<PartialAttribute> (type);
			var model = AttributeManager.GetCustomAttribute<ModelAttribute> (type);
			bool is_model = model is not null;
			var protocol = AttributeManager.GetCustomAttribute<ProtocolAttribute> (type);
			bool is_protocol = protocol is not null;
			bool is_abstract = AttributeManager.HasAttribute<AbstractAttribute> (type);
			bool is_sealed = AttributeManager.HasAttribute<SealedAttribute> (type);
			string class_visibility = type.IsInternal (this) ? "internal" : "public";

			var default_ctor_visibility = AttributeManager.GetCustomAttribute<DefaultCtorVisibilityAttribute> (type);
			BaseTypeAttribute bta = ReflectionExtensions.GetBaseTypeAttribute (type, this);
			Type base_type = bta is not null ? bta.BaseType : TypeCache.System_Object;
			string objc_type_name = bta is not null ? (bta.Name is not null ? bta.Name : TypeName) : TypeName;
			string register_name = objc_type_name;

			if (is_model) {
				if (!string.IsNullOrEmpty (model.Name)) {
					register_name = model.Name;
#if NET
				} else {
					// For .NET, we'll always generate the Objective-C name. If a user wants to use a different name,
					// they can set the model name (so we'll enter the previous condition)
#else
				} else if (model.AutoGeneratedName) {
#endif
					register_name = Registrar.Registrar.SanitizeObjectiveCName (GetAssemblyName () + "__" + type.FullName);
				}
			}
			Header (sw);

			if (is_protocol) {
				if (is_static_class)
					throw new BindingException (1025, true, type.FullName);
				if (is_model && base_type == TypeCache.System_Object)
					ErrorHelper.Warning (1060, type.FullName);

				GenerateProtocolTypes (type, class_visibility, TypeName, protocol.Name ?? objc_type_name, protocol);
			}

			if (!is_static_class && bta is null && is_protocol)
				return;

			if (type.Namespace is not null) {
				print ("namespace {0} {{", type.Namespace);
				indent++;
			}

			bool core_image_filter = false;
			string class_mod = null;

			is_direct_binding_value = null;
			is_direct_binding = null;
			if (BindThirdPartyLibrary)
				is_direct_binding_value = string.Format ("GetType ().Assembly == global::{0}.this_assembly", NamespaceCache.Messaging);
			if (is_static_class || is_category_class || is_partial) {
				base_type = TypeCache.System_Object;
				if (!is_partial)
					class_mod = "static ";
			} else {
				if (is_protocol) {
					var pName = !string.IsNullOrEmpty (protocol.Name) ? $"Name = \"{protocol.Name}\"" : string.Empty;
					print ("[Protocol({0}{1}{2})]", pName, (!string.IsNullOrEmpty (pName) && protocol.IsInformal) ? ", " : string.Empty, protocol.IsInformal ? "IsInformal = true" : string.Empty);
				}
				core_image_filter = AttributeManager.HasAttribute<CoreImageFilterAttribute> (type);
				if (!type.IsEnum && !core_image_filter) {
					if (is_model || AttributeManager.HasAttribute<SyntheticAttribute> (type)) {
						is_direct_binding = false;
						is_direct_binding_value = "false";
					}
					print ("[Register(\"{0}\", {1})]", register_name, is_direct_binding == false ? "false" : "true");
				}
				if (is_abstract || need_abstract.ContainsKey (type)) {
					// Don't mark [Abstract] classes as abstract in .NET, we might need to create instances of them at some point.
					// This can happen if the OS gives an instance of a subclass, but that subclass is private (so we haven't bound it).
					// In this case, the best managed type is this type, but it can't be abstract if we want to create an instance of it.
					// Except that we declare models as abstract, because they're meant to be subclassed (and they're not wrapping a native type anyway).
#if NET
					if (is_model)
						class_mod = "abstract ";
#else
					class_mod = "abstract ";
#endif
				} else if (is_sealed)
					class_mod = "sealed ";
			}

			if (is_sealed && is_direct_binding is null) {
				is_direct_binding = true;
				is_direct_binding_value = "true";
			}

			if (is_model) {
				if (is_category_class)
					ErrorHelper.Show (new BindingException (1022, true));
				print ("[Model]");
			}

			PrintAttributes (type, platform: true, preserve: true, advice: true);

			if (type.IsEnum) {
				GenerateEnum (type);
				return;
			}

			if (core_image_filter) {
				GenerateFilter (type);
				return;
			}

			// Order of this list is such that the base type will be first,
			// followed by the protocol interface, followed by any other
			// interfaces in ascending order
			var implements_list = new List<string> ();

			foreach (var protocolType in type.GetInterfaces ()) {
				if (!AttributeManager.HasAttribute<ProtocolAttribute> (protocolType)) {
					if (protocolType.Name [0] != 'I')
						continue;

					string nonInterfaceName = protocolType.Name.Substring (1);
					if (!types.Any (x => x.Name.Contains (nonInterfaceName)))
						continue;

					if (is_category_class)
						continue;
					// 1. MKUserLocation implements the MKAnnotation protocol
					// 2. The MKAnnotation protocol has a required 'coordinate' getter, and an optional 'coordinate' setter.
					//    We've bound the former using an [Abstract] readonly Coordinate property, and the latter using a SetCoordinate method.
					// 3. MKUserLocation has a readwrite 'coordinate' property, which we've bound as a readwrite Coordinate property.
					// 4. If we make MKUserLocation implement the MKAnnotation protocol in the api bindings, then we'll
					//    inline all the MKAnnotation protocol members in MKUserLocation. This includes the SetCoordinate method,
					//    which causes problems, because it implements the 'setCoordinate:' selector, which the Coordinate property
					//    does as well, resulting in:
					//        error MM4119: Could not register the selector 'setCoordinate:' of the member 'MapKit.MKUserLocation.set_Coordinate' because the selector is already registered on the member 'SetCoordinate'.
					//
					// So we can't make the MKUserLocation implement the MKAnnotation protocol in the api definition (for now at least).
					if (type.Name == "MKUserLocation" && protocolType.Name == "IMKAnnotation")
						continue;
#if !NET
					if (type.Name == "NSFontAssetRequest" || protocolType.Name == "INSProgressReporting")
						continue;
#endif

					ErrorHelper.Warning (1111, protocolType, type, nonInterfaceName);
					continue;
				}

				// skip protocols that aren't available on the current platform
				if (protocolType.IsUnavailable (this))
					continue;

				// A protocol this class implements. We need to implement the corresponding interface for the protocol.
				string pname = protocolType.Name;
				// the extra 'I' is only required for the bindings being built, if it comes from something already
				// built (e.g. monotouch.dll) then the interface will alreadybe prefixed
				if (protocolType.Assembly.GetName ().Name == "temp")
					pname = "I" + pname;
				var iface = TypeManager.FormatType (type, protocolType.Namespace, pname);
				if (!implements_list.Contains (iface))
					implements_list.Add (iface);
			}

			implements_list.Sort (StringComparer.Ordinal);

			if (is_protocol)
				implements_list.Insert (0, "I" + type.Name);

			if (base_type != TypeCache.System_Object && TypeName != "NSObject" && !is_category_class)
				implements_list.Insert (0, TypeManager.FormatType (type, base_type));

			if (type.IsNested) {
				var nestedName = type.FullName.Substring (type.Namespace.Length + 1);
				var container = nestedName.Substring (0, nestedName.IndexOf ('+'));

				print ("partial class {0} {{", container);
				indent++;
			}

			var class_name = TypeName;
			var where_list = string.Empty;
			if (type.IsGenericType) {
				class_name += "<";
				var gargs = type.GetGenericArguments ();
				for (int i = 0; i < gargs.Length; i++) {
					if (i > 0)
						class_name += ", ";
					class_name += TypeManager.FormatType (type, gargs [i]);

					where_list += "\n\t\twhere " + gargs [i].Name + " : ";
					var constraints = gargs [i].GetGenericParameterConstraints ();
					if (constraints.Length > 0) {
						var comma = string.Empty;
						if (IsProtocol (constraints [0])) {
							where_list += "NSObject";
							comma = ", ";
						}

						for (int c = 0; c < constraints.Length; c++) {
							where_list += comma + TypeManager.FormatType (type, constraints [c]);
							comma = ", ";
						}
					} else {
						where_list += "NSObject";
					}
				}
				class_name += ">";
				if (where_list.Length > 0)
					where_list += " ";
			}

			print ("{0} unsafe {1}partial class {2} {3} {4}{{",
				   class_visibility,
				   class_mod,
				   class_name,
				   implements_list.Count == 0 ? string.Empty : ": " + string.Join (", ", implements_list),
				   where_list);

			indent++;

			if (!is_model && !is_partial) {
				foreach (var ea in selectors [type].OrderBy (s => s, StringComparer.Ordinal)) {
					var selectorField = SelectorField (ea, true);
					if (!InlineSelectors) {
						selectorField = selectorField.Substring (0, selectorField.Length - 6 /* Handle */);
						print_generated_code ();
						print ("const string {0} = \"{1}\";", selectorField, ea);
						print ("static readonly {2} {0} = Selector.GetHandle (\"{1}\");", SelectorField (ea), ea, NativeHandleType);
					}
				}
			}
			print ("");

			// Regular bindings (those that are not-static) or categories need this
			if (!(is_static_class || is_partial) || is_category_class) {
				if (is_category_class)
					objc_type_name = FormatCategoryClassName (bta);

				if (!is_model) {
					print_generated_code ();
					var is32BitNotSupported = Is64BitiOSOnly (type);
					if (is32BitNotSupported) {
						// potentially avoid a .cctor and extra, unusable code
						print ("#if ARCH_32");
						print ("#pragma warning disable {0}", is_static_class ? "169" : "649");
						print ("static readonly {0} class_ptr;", NativeHandleType);
						print ("#pragma warning restore {0}", is_static_class ? "169" : "649");
						print ("#else");
					}
					print ("static readonly {1} class_ptr = Class.GetHandle (\"{0}\");", objc_type_name, NativeHandleType);
					if (is32BitNotSupported)
						print ("#endif");
					print ("");
				}
			}

			if (!is_static_class && !is_partial) {
				if (!is_model && !external) {
					print ("public {1} {2} ClassHandle {{ get {{ return class_ptr; }} }}\n", objc_type_name, TypeName == "NSObject" ? "virtual" : "override", NativeHandleType);
				}

				var ctor_visibility = is_abstract ? "protected" : "public";
				var disable_default_ctor = false;
				if (default_ctor_visibility is not null) {
					switch (default_ctor_visibility.Visibility) {
					case Visibility.Public:
						ctor_visibility = "public";
						break;
					case Visibility.Internal:
						ctor_visibility = "internal";
						break;
					case Visibility.Protected:
						ctor_visibility = "protected";
						break;
					case Visibility.ProtectedInternal:
						ctor_visibility = "protected internal";
						break;
					case Visibility.Private:
						ctor_visibility = string.Empty;
						break;
					case Visibility.Disabled:
						disable_default_ctor = true;
						break;
					}
				}

				if (TypeName != "NSObject") {
					var initSelector = (InlineSelectors || BindThirdPartyLibrary) ? "Selector.GetHandle (\"init\")" : "Selector.Init";
					var initWithCoderSelector = (InlineSelectors || BindThirdPartyLibrary) ? "Selector.GetHandle (\"initWithCoder:\")" : "Selector.InitWithCoder";
					string v = (class_mod == "abstract " && default_ctor_visibility is null) ? "protected" : ctor_visibility;
					var is32BitNotSupported = Is64BitiOSOnly (type);
					if (external) {
						if (!disable_default_ctor) {
							GeneratedCode (sw, 2);
							if (AttributeManager.HasAttribute<DesignatedDefaultCtorAttribute> (type))
								sw.WriteLine ("\n\n[DesignatedInitializer]");
							sw.WriteLine ("\t\t[EditorBrowsable (EditorBrowsableState.Advanced)]");
							sw.WriteLine ("\t\t[Export (\"init\")]");
							sw.WriteLine ("\t\t{0} {1} () : base (NSObjectFlag.Empty)", v, TypeName);
							sw.WriteLine ("\t\t{");
							if (is32BitNotSupported) {
								sw.WriteLine ("\t\t#if ARCH_32");
								sw.WriteLine ("\tthrow new PlatformNotSupportedException (\"This API is not supported on this version of iOS\");");
								sw.WriteLine ("\t\t#else");
							}
							if (is_direct_binding_value is not null)
								sw.WriteLine ("\t\t\tIsDirectBinding = {0};", is_direct_binding_value);
							if (debug)
								sw.WriteLine ("\t\t\tConsole.WriteLine (\"{0}.ctor ()\");", TypeName);
							sw.WriteLine ("\t\t\tInitializeHandle (global::{1}.IntPtr_objc_msgSend (this.Handle, global::ObjCRuntime.{0}), \"init\");", initSelector, NamespaceCache.Messaging);
							sw.WriteLine ("\t\t\t");
							if (is32BitNotSupported)
								sw.WriteLine ("\t\t#endif");
							sw.WriteLine ("\t\t}");
						}
					} else {
						if (!disable_default_ctor) {
							GeneratedCode (sw, 2);
							if (AttributeManager.HasAttribute<DesignatedDefaultCtorAttribute> (type))
								sw.WriteLine ("\t\t[DesignatedInitializer]");
							sw.WriteLine ("\t\t[EditorBrowsable (EditorBrowsableState.Advanced)]");
							sw.WriteLine ("\t\t[Export (\"init\")]");
							sw.WriteLine ("\t\t{0} {1} () : base (NSObjectFlag.Empty)", v, TypeName);
							sw.WriteLine ("\t\t{");
							if (is32BitNotSupported) {
								sw.WriteLine ("\t\t#if ARCH_32");
								sw.WriteLine ("\tthrow new PlatformNotSupportedException (\"This API is not supported on this version of iOS\");");
								sw.WriteLine ("\t\t#else");
							}
							if (type_needs_thread_checks) {
								sw.Write ("\t\t\t");
								GenerateThreadCheck (sw);
							}
							var indentation = 3;
							WriteIsDirectBindingCondition (sw, ref indentation, is_direct_binding, is_direct_binding_value,
														   () => string.Format ("InitializeHandle (global::{1}.IntPtr_objc_msgSend (this.Handle, global::ObjCRuntime.{0}), \"init\");", initSelector, NamespaceCache.Messaging),
														   () => string.Format ("InitializeHandle (global::{1}.IntPtr_objc_msgSendSuper (this.SuperHandle, global::ObjCRuntime.{0}), \"init\");", initSelector, NamespaceCache.Messaging));

							WriteMarkDirtyIfDerived (sw, type);
							if (is32BitNotSupported)
								sw.WriteLine ("\t\t#endif");
							sw.WriteLine ("\t\t}");
							sw.WriteLine ();
						}
						var nscoding = ConformToNSCoding (type);
						if (nscoding) {
							GeneratedCode (sw, 2);
							sw.WriteLine ("\t\t[DesignatedInitializer]");
							sw.WriteLine ("\t\t[EditorBrowsable (EditorBrowsableState.Advanced)]");
							sw.WriteLine ("\t\t[Export (\"initWithCoder:\")]");
							sw.WriteLine ("\t\t{0} {1} (NSCoder coder) : base (NSObjectFlag.Empty)", v, TypeName);
							sw.WriteLine ("\t\t{");
							if (is32BitNotSupported) {
								sw.WriteLine ("\t\t#if ARCH_32");
								sw.WriteLine ("\tthrow new PlatformNotSupportedException (\"This API is not supported on this version of iOS\");");
								sw.WriteLine ("\t\t#else");
							}
							if (nscoding) {
								if (debug)
									sw.WriteLine ("\t\t\tConsole.WriteLine (\"{0}.ctor (NSCoder)\");", TypeName);
								if (type_needs_thread_checks) {
									sw.Write ("\t\t\t");
									GenerateThreadCheck (sw);
								}
								var indentation = 3;
								WriteIsDirectBindingCondition (sw, ref indentation, is_direct_binding, is_direct_binding_value,
															   () => string.Format ("InitializeHandle (global::{1}.IntPtr_objc_msgSend_IntPtr (this.Handle, {0}, coder.Handle), \"initWithCoder:\");", initWithCoderSelector, NamespaceCache.Messaging),
															   () => string.Format ("InitializeHandle (global::{1}.IntPtr_objc_msgSendSuper_IntPtr (this.SuperHandle, {0}, coder.Handle), \"initWithCoder:\");", initWithCoderSelector, NamespaceCache.Messaging));
								WriteMarkDirtyIfDerived (sw, type);
							} else {
								sw.WriteLine ("\t\t\tthrow new InvalidOperationException (\"Type does not conform to NSCoding\");");
							}
							if (is32BitNotSupported)
								sw.WriteLine ("\t\t#endif");
							sw.WriteLine ("\t\t}");
							sw.WriteLine ();
						}
					}
					if (!is_sealed) {
						GeneratedCode (sw, 2);
						sw.WriteLine ("\t\t[EditorBrowsable (EditorBrowsableState.Advanced)]");
						sw.WriteLine ("\t\tprotected {0} (NSObjectFlag t) : base (t)", TypeName);
						sw.WriteLine ("\t\t{");
						if (is_direct_binding_value is not null)
							sw.WriteLine ("\t\t\tIsDirectBinding = {0};", is_direct_binding_value);
						WriteMarkDirtyIfDerived (sw, type);
						sw.WriteLine ("\t\t}");
						sw.WriteLine ();
					}
					GeneratedCode (sw, 2);
					sw.WriteLine ("\t\t[EditorBrowsable (EditorBrowsableState.Advanced)]");
					sw.Write ($"\t\t");
					if (!is_sealed)
						sw.Write ("protected ");
					sw.WriteLine ($"internal {TypeName} ({NativeHandleType} handle) : base (handle)");
					sw.WriteLine ("\t\t{");
					if (is_direct_binding_value is not null)
						sw.WriteLine ("\t\t\tIsDirectBinding = {0};", is_direct_binding_value);
					WriteMarkDirtyIfDerived (sw, type);
					sw.WriteLine ("\t\t}");
					sw.WriteLine ();
				}
			}

			var bound_methods = new HashSet<MemberInformation> (); // List of methods bound on the class itself (not via protocols)
			var generated_methods = new List<MemberInformation> (); // All method that have been generated
			foreach (var mi in GetTypeContractMethods (type).OrderByDescending (m => m.Name == "Constructor").ThenBy (m => m.Name, StringComparer.Ordinal)) {
				if (mi.IsSpecialName || (mi.Name == "Constructor" && type != mi.DeclaringType))
					continue;

				if (mi.IsUnavailable (this))
					continue;

				if (appearance_selectors is not null && AttributeManager.HasAttribute<AppearanceAttribute> (mi))
					appearance_selectors.Add (mi);

				var minfo = new MemberInformation (this, this, mi, type, is_category_class ? bta.BaseType : null, isModel: is_model);
				// the type above is not the the we're generating, e.g. it would be `NSCopying` for `Copy(NSZone?)`
				minfo.is_type_sealed = is_sealed;

				if (type == mi.DeclaringType || type.IsSubclassOf (mi.DeclaringType)) {
					// not an injected protocol method.
					bound_methods.Add (minfo);
				} else {
					// don't inject a protocol method if the class already
					// implements the same method.
					if (bound_methods.Contains (minfo))
						continue;

					var protocolsThatHaveThisMethod = GetTypeContractMethods (type).Where (x => { var sel = GetSelector (x); return sel is not null && sel == minfo.selector; });
					if (protocolsThatHaveThisMethod.Count () > 1) {
						// If multiple protocols have this method and we haven't generated a copy yet
						if (generated_methods.Any (x => x.selector == minfo.selector))
							continue;

						// Verify all of the versions have the same arguments / return value
						// And just generate the first one (us)
						var methodParams = mi.GetParameters ();
						foreach (var duplicateMethod in protocolsThatHaveThisMethod) {
							if (mi.ReturnType != duplicateMethod.ReturnType)
								throw new BindingException (1038, true, mi.Name, type.Name);

							if (methodParams.Length != duplicateMethod.GetParameters ().Length)
								throw new BindingException (1039, true, minfo.selector, type.Name, mi.GetParameters ().Length, duplicateMethod.GetParameters ().Length);
						}

						int i = 0;
						foreach (var param in methodParams) {
							foreach (var duplicateMethod in protocolsThatHaveThisMethod) {
								var duplicateParam = duplicateMethod.GetParameters () [i];
								if (param.IsOut != duplicateParam.IsOut)
									throw new BindingException (1040, true, minfo.selector, type.Name, i);
								if (param.ParameterType != duplicateParam.ParameterType)
									throw new BindingException (1041, true, minfo.selector, type.Name, i, param.ParameterType, duplicateParam.ParameterType);
							}
							i++;
						}
					}
				}

				generated_methods.Add (minfo);
				GenerateMethod (minfo);
			}

			var field_exports = new List<PropertyInfo> ();
			var notifications = new List<PropertyInfo> ();
			var bound_properties = new List<string> (); // List of properties bound on the class itself (not via protocols)
			var generated_properties = new List<string> (); // All properties that have been generated

			foreach (var pi in GetTypeContractProperties (type).OrderBy (p => p.Name, StringComparer.Ordinal)) {

				if (pi.IsUnavailable (this))
					continue;

				if (AttributeManager.HasAttribute<FieldAttribute> (pi)) {
					field_exports.Add (pi);

					if (AttributeManager.HasAttribute<NotificationAttribute> (pi))
						notifications.Add (pi);
					continue;
				}

				if (appearance_selectors is not null && AttributeManager.HasAttribute<AppearanceAttribute> (pi))
					appearance_selectors.Add (pi);

				if (type == pi.DeclaringType || type.IsSubclassOf (pi.DeclaringType)) {
					// not an injected protocol property.
					bound_properties.Add (pi.Name);
				} else {
					// don't inject a protocol property if the class already
					// implements the same property.
					if (bound_properties.Contains (pi.Name))
						continue;

					var protocolsThatHaveThisProp = GetTypeContractProperties (type).Where (x => x.Name == pi.Name);
					if (protocolsThatHaveThisProp.Count () > 1) {
						// If multiple protocols have this property and we haven't generated a copy yet
						if (generated_properties.Contains (pi.Name))
							continue;

						// If there is a version that has get and set
						if (protocolsThatHaveThisProp.Any (x => x.CanRead && x.CanWrite)) {
							// Skip if we are not it
							if (!(pi.CanRead && pi.CanWrite))
								continue;
						} else {
							// Verify all of the versions have the same get/set abilities since there is no universal get/set version
							// And just generate the first one (us)
							if (!protocolsThatHaveThisProp.All (x => x.CanRead == pi.CanRead && x.CanWrite == pi.CanWrite))
								throw new BindingException (1037, true, pi.Name, type.Name);
						}
					}
				}

				generated_properties.Add (pi.Name);
				GenerateProperty (type, pi, instance_fields_to_clear_on_dispose, is_model);
			}

			if (field_exports.Count != 0) {
				foreach (var field_pi in field_exports.OrderBy (f => f.Name, StringComparer.Ordinal)) {
					var fieldAttr = AttributeManager.GetCustomAttribute<FieldAttribute> (field_pi);
					ComputeLibraryName (fieldAttr, type, field_pi.Name, out string library_name, out string library_path);

					bool is_unified_internal = field_pi.IsUnifiedInternal (this);

					string fieldTypeName;
					string smartEnumTypeName = null;
					if (IsSmartEnum (field_pi.PropertyType)) {
						fieldTypeName = TypeManager.FormatType (TypeCache.NSString.DeclaringType, TypeCache.NSString);
						smartEnumTypeName = TypeManager.FormatType (field_pi.DeclaringType, field_pi.PropertyType);
					} else
						fieldTypeName = TypeManager.FormatType (field_pi.DeclaringType, field_pi.PropertyType);

					bool nullable = false;
					// Value types we dont cache for now, to avoid Nullable<T>
					if (!field_pi.PropertyType.IsValueType || smartEnumTypeName is not null) {
						print_generated_code ();
						PrintPreserveAttribute (field_pi);
						print ("static {0}? _{1};", fieldTypeName, field_pi.Name);
					}

					PrintAttributes (field_pi, preserve: true, advice: true);
					PrintObsoleteAttributes (field_pi);
					print ("[Field (\"{0}\",  \"{1}\")]", fieldAttr.SymbolName, library_path ?? library_name);
					PrintPlatformAttributes (field_pi);
					if (AttributeManager.HasAttribute<AdvancedAttribute> (field_pi)) {
						print ("[EditorBrowsable (EditorBrowsableState.Advanced)]");
					}
					// Check if this is a notification and print Advice to use our Notification API
					if (AttributeManager.HasAttribute<NotificationAttribute> (field_pi))
						print ($"[Advice (\"Use {type.Name}.Notifications.Observe{GetNotificationName (field_pi)} helper method instead.\")]");

					print ("{0} static {1}{2} {3}{4} {{", field_pi.IsInternal (this) ? "internal" : "public",
						smartEnumTypeName ?? fieldTypeName,
						nullable ? "?" : "",
						field_pi.Name,
						is_unified_internal ? "_" : "");
					indent++;

					PrintAttributes (field_pi, platform: true);
					PrintAttributes (field_pi.GetGetMethod (), preserve: true, advice: true);
					print ("get {");
					indent++;
					if (field_pi.PropertyType == TypeCache.NSString) {
						print ("if (_{0} is null)", field_pi.Name);
						indent++;
						print ("_{0} = Dlfcn.GetStringConstant (Libraries.{2}.Handle, \"{1}\")!;", field_pi.Name, fieldAttr.SymbolName, library_name);
						indent--;
						print ("return _{0};", field_pi.Name);
					} else if (field_pi.PropertyType.Name == "NSArray") {
						print ("if (_{0} is null)", field_pi.Name);
						indent++;
						print ("_{0} = Runtime.GetNSObject<NSArray> (Dlfcn.GetIndirect (Libraries.{2}.Handle, \"{1}\"))!;", field_pi.Name, fieldAttr.SymbolName, library_name);
						indent--;
						print ("return _{0};", field_pi.Name);
					} else if (field_pi.PropertyType.Name == "UTType") {
						print ("if (_{0} is null)", field_pi.Name);
						indent++;
						print ("_{0} = Runtime.GetNSObject<UTType> (Dlfcn.GetIntPtr (Libraries.{2}.Handle, \"{1}\"))!;", field_pi.Name, fieldAttr.SymbolName, library_name);
						indent--;
						print ("return _{0};", field_pi.Name);
					} else if (field_pi.PropertyType == TypeCache.System_Int32) {
						print ("return Dlfcn.GetInt32 (Libraries.{2}.Handle, \"{1}\");", field_pi.Name, fieldAttr.SymbolName, library_name);
					} else if (field_pi.PropertyType == TypeCache.System_UInt32) {
						print ("return Dlfcn.GetUInt32 (Libraries.{2}.Handle, \"{1}\");", field_pi.Name, fieldAttr.SymbolName, library_name);
					} else if (field_pi.PropertyType == TypeCache.System_Double) {
						print ("return Dlfcn.GetDouble (Libraries.{2}.Handle, \"{1}\");", field_pi.Name, fieldAttr.SymbolName, library_name);
					} else if (field_pi.PropertyType == TypeCache.System_Float) {
						print ("return Dlfcn.GetFloat (Libraries.{2}.Handle, \"{1}\");", field_pi.Name, fieldAttr.SymbolName, library_name);
					} else if (field_pi.PropertyType == TypeCache.System_IntPtr) {
						print ("return Dlfcn.GetIntPtr (Libraries.{2}.Handle, \"{1}\");", field_pi.Name, fieldAttr.SymbolName, library_name);
					} else if (field_pi.PropertyType.FullName == "System.Drawing.SizeF") {
						print ("return Dlfcn.GetSizeF (Libraries.{2}.Handle, \"{1}\");", field_pi.Name, fieldAttr.SymbolName, library_name);
					} else if (field_pi.PropertyType == TypeCache.System_Int64) {
						print ("return Dlfcn.GetInt64 (Libraries.{2}.Handle, \"{1}\");", field_pi.Name, fieldAttr.SymbolName, library_name);
					} else if (field_pi.PropertyType == TypeCache.System_UInt64) {
						print ("return Dlfcn.GetUInt64 (Libraries.{2}.Handle, \"{1}\");", field_pi.Name, fieldAttr.SymbolName, library_name);
					} else
						//
						// Handle various blittable value types here
						//
						if (Frameworks.HaveCoreMedia && Frameworks.HaveAVFoundation && (field_pi.PropertyType == TypeCache.CMTime ||
						   field_pi.PropertyType == TypeCache.AVCaptureWhiteBalanceGains)) {
						print ("return *(({3} *) Dlfcn.dlsym (Libraries.{2}.Handle, \"{1}\"));", field_pi.Name, fieldAttr.SymbolName, library_name,
							TypeManager.FormatType (type, field_pi.PropertyType.Namespace, field_pi.PropertyType.Name));
					} else if (field_pi.PropertyType == TypeCache.System_nint) {
						print ("return Dlfcn.GetNInt (Libraries.{2}.Handle, \"{1}\");", field_pi.Name, fieldAttr.SymbolName, library_name);
					} else if (field_pi.PropertyType == TypeCache.System_nuint) {
						print ("return Dlfcn.GetNUInt (Libraries.{2}.Handle, \"{1}\");", field_pi.Name, fieldAttr.SymbolName, library_name);
					} else if (field_pi.PropertyType == TypeCache.System_nfloat) {
						print ("return Dlfcn.GetNFloat (Libraries.{2}.Handle, \"{1}\");", field_pi.Name, fieldAttr.SymbolName, library_name);
					} else if (field_pi.PropertyType == TypeCache.CoreGraphics_CGSize) {
						print ("return Dlfcn.GetCGSize (Libraries.{2}.Handle, \"{1}\");", field_pi.Name, fieldAttr.SymbolName, library_name);
					} else if (field_pi.PropertyType.IsEnum) {
						var btype = field_pi.PropertyType.GetEnumUnderlyingType ();
						if (smartEnumTypeName is not null) {
							print ("if (_{0} is null)", field_pi.Name);
							indent++;
							print ("_{0} = Dlfcn.GetStringConstant (Libraries.{2}.Handle, \"{1}\")!;", field_pi.Name, fieldAttr.SymbolName, library_name);
							indent--;
							print ($"return {smartEnumTypeName}Extensions.GetValue (_{field_pi.Name});");
						} else if (GetNativeEnumToManagedExpression (field_pi.PropertyType, out var preExpression, out var postExpression, out var _)) {
							if (btype == TypeCache.System_nint || btype == TypeCache.System_Int64)
								print ($"return {preExpression}Dlfcn.GetNInt (Libraries.{library_name}.Handle, \"{fieldAttr.SymbolName}\"){postExpression};");
							else if (btype == TypeCache.System_nuint || btype == TypeCache.System_UInt64)
								print ($"return {preExpression}Dlfcn.GetNUInt (Libraries.{library_name}.Handle, \"{fieldAttr.SymbolName}\"){postExpression};");
							else
								throw new BindingException (1014, true, fieldTypeName, FormatPropertyInfo (field_pi));
						} else {
							if (btype == TypeCache.System_Int32)
								print ($"return ({fieldTypeName}) Dlfcn.GetInt32 (Libraries.{library_name}.Handle, \"{fieldAttr.SymbolName}\");");
							else if (btype == TypeCache.System_UInt32)
								print ($"return ({fieldTypeName}) Dlfcn.GetUInt32 (Libraries.{library_name}.Handle, \"{fieldAttr.SymbolName}\");");
							else if (btype == TypeCache.System_Int64)
								print ($"return ({fieldTypeName}) Dlfcn.GetInt64 (Libraries.{library_name}.Handle, \"{fieldAttr.SymbolName}\");");
							else if (btype == TypeCache.System_UInt64)
								print ($"return ({fieldTypeName}) Dlfcn.GetUInt64 (Libraries.{library_name}.Handle, \"{fieldAttr.SymbolName}\");");
							else
								throw new BindingException (1014, true, fieldTypeName, FormatPropertyInfo (field_pi));
						}
					} else {
						if (field_pi.PropertyType == TypeCache.System_String)
							throw new BindingException (1013, true);
						else
							throw new BindingException (1014, true, fieldTypeName, FormatPropertyInfo (field_pi));
					}

					indent--;
					print ("}");

					if (field_pi.CanWrite) {
						PrintAttributes (field_pi, platform: true);
						PrintAttributes (field_pi.GetSetMethod (), preserve: true, advice: true);
						print ("set {");
						indent++;
						if (field_pi.PropertyType == TypeCache.System_Int32) {
							print ("Dlfcn.SetInt32 (Libraries.{2}.Handle, \"{1}\", value);", field_pi.Name, fieldAttr.SymbolName, library_name);
						} else if (field_pi.PropertyType == TypeCache.System_UInt32) {
							print ("Dlfcn.SetUInt32 (Libraries.{2}.Handle, \"{1}\", value);", field_pi.Name, fieldAttr.SymbolName, library_name);
						} else if (field_pi.PropertyType == TypeCache.System_Double) {
							print ("Dlfcn.SetDouble (Libraries.{2}.Handle, \"{1}\", value);", field_pi.Name, fieldAttr.SymbolName, library_name);
						} else if (field_pi.PropertyType == TypeCache.System_Float) {
							print ("Dlfcn.SetFloat (Libraries.{2}.Handle, \"{1}\", value);", field_pi.Name, fieldAttr.SymbolName, library_name);
						} else if (field_pi.PropertyType == TypeCache.System_IntPtr) {
							print ("Dlfcn.SetIntPtr (Libraries.{2}.Handle, \"{1}\", value);", field_pi.Name, fieldAttr.SymbolName, library_name);
						} else if (field_pi.PropertyType.FullName == "System.Drawing.SizeF") {
							print ("Dlfcn.SetSizeF (Libraries.{2}.Handle, \"{1}\", value);", field_pi.Name, fieldAttr.SymbolName, library_name);
						} else if (field_pi.PropertyType == TypeCache.System_Int64) {
							print ("Dlfcn.SetInt64 (Libraries.{2}.Handle, \"{1}\", value);", field_pi.Name, fieldAttr.SymbolName, library_name);
						} else if (field_pi.PropertyType == TypeCache.System_UInt64) {
							print ("Dlfcn.SetUInt64 (Libraries.{2}.Handle, \"{1}\", value);", field_pi.Name, fieldAttr.SymbolName, library_name);
						} else if (field_pi.PropertyType == TypeCache.NSString) {
							print ("Dlfcn.SetString (Libraries.{2}.Handle, \"{1}\", value);", field_pi.Name, fieldAttr.SymbolName, library_name);
						} else if (field_pi.PropertyType.Name == "NSArray") {
							print ("Dlfcn.SetArray (Libraries.{2}.Handle, \"{1}\", value);", field_pi.Name, fieldAttr.SymbolName, library_name);
						} else if (field_pi.PropertyType == TypeCache.System_nint) {
							print ("Dlfcn.SetNInt (Libraries.{2}.Handle, \"{1}\", value);", field_pi.Name, fieldAttr.SymbolName, library_name);
						} else if (field_pi.PropertyType == TypeCache.System_nuint) {
							print ("Dlfcn.SetNUInt (Libraries.{2}.Handle, \"{1}\", value);", field_pi.Name, fieldAttr.SymbolName, library_name);
						} else if (field_pi.PropertyType == TypeCache.System_nfloat) {
							print ("Dlfcn.SetNFloat (Libraries.{2}.Handle, \"{1}\", value);", field_pi.Name, fieldAttr.SymbolName, library_name);
						} else if (field_pi.PropertyType == TypeCache.CoreGraphics_CGSize) {
							print ("Dlfcn.SetCGSize (Libraries.{2}.Handle, \"{1}\", value);", field_pi.Name, fieldAttr.SymbolName, library_name);
						} else if (field_pi.PropertyType.IsEnum) {
							var btype = field_pi.PropertyType.GetEnumUnderlyingType ();
							if (smartEnumTypeName is not null)
								print ($"Dlfcn.SetString (Libraries.{library_name}.Handle, \"{fieldAttr.SymbolName}\", value.GetConstant ());");
							else if (GetNativeEnumToNativeExpression (field_pi.PropertyType, out var preExpression, out var postExpression, out var _)) {
								if (btype == TypeCache.System_nint || (BindThirdPartyLibrary && btype == TypeCache.System_Int64))
									print ($"Dlfcn.SetNInt (Libraries.{library_name}.Handle, \"{fieldAttr.SymbolName}\", (nint) {preExpression}value{postExpression});");
								else if (btype == TypeCache.System_nuint || (BindThirdPartyLibrary && btype == TypeCache.System_UInt64))
									print ($"Dlfcn.SetNUInt (Libraries.{library_name}.Handle, \"{fieldAttr.SymbolName}\", (nuint) {preExpression}value{postExpression});");
								else
									throw new BindingException (1021, true, fieldTypeName, field_pi.DeclaringType.FullName, field_pi.Name);
							} else {
								if (btype == TypeCache.System_Int32)
									print ($"Dlfcn.SetInt32 (Libraries.{library_name}.Handle, \"{fieldAttr.SymbolName}\", (int) value);");
								else if (btype == TypeCache.System_UInt32)
									print ($"Dlfcn.SetUInt32 (Libraries.{library_name}.Handle, \"{fieldAttr.SymbolName}\", (uint) value);");
								else if (btype == TypeCache.System_Int64)
									print ($"Dlfcn.SetInt64 (Libraries.{library_name}.Handle, \"{fieldAttr.SymbolName}\", (long) value);");
								else if (btype == TypeCache.System_UInt64)
									print ($"Dlfcn.SetUInt64 (Libraries.{library_name}.Handle, \"{fieldAttr.SymbolName}\", (ulong) value);");
								else
									throw new BindingException (1021, true, fieldTypeName, field_pi.DeclaringType.FullName, field_pi.Name);
							}
						} else
							throw new BindingException (1021, true, fieldTypeName, field_pi.DeclaringType.FullName, field_pi.Name);
						indent--;
						print ("}");
					}
					indent--;
					print ("}");
				}
			}

			var eventArgTypes = new Dictionary<string, ParameterInfo []> ();

			if (bta is not null && bta.Events is not null) {
				if (bta.Delegates is null)
					throw new BindingException (1015, true, type.FullName);

				print ("//");
				print ("// Events and properties from the delegate");
				print ("//\n");

				if (bta.Events.Length != bta.Delegates.Length)
					throw new BindingException (1023, true, type.FullName);

				int delidx = 0;
				foreach (var dtype in bta.Events) {
					string delName = bta.Delegates [delidx++];
					delName = delName.StartsWith ("Weak", StringComparison.Ordinal) ? delName.Substring (4) : delName;

					// Here's the problem:
					//    If you have two or more types in an inheritence structure in the binding that expose events
					//    they can fight over who's generated delegate gets used if you use the events at both level.
					//    e.g. NSComboxBox.SelectionChanged (on NSComboxBox) and NSComboBox.EditingEnded (on NSTextField)
					// We solve this under Unified when the delegate is protocalized (and leave Classic how it always has been)
					// To handle this case, we do two things:
					//    1) We have to ensure that the same internal delegate is uses for base and derived events, so they
					//     aren't stepping on each other toes. This means we always instance up the leaf class's internal delegate
					//    2) We have to have an some for of "inheritance" in the generated delegates, so the
					//     base class can use this derived type (and have the events it expects on it) We do this via inhertiance and
					//     use of the protocal interfaces.
					//     See xamcore/test/apitest/DerivedEventTest.cs for specific tests

					// Does this delegate qualify for this treatment. We fall back on the old "wrong" codegen if not.
					bool isProtocolizedEventBacked = IsProtocolizedEventBacked (delName, type);

					// The name of the the generated virtual property that specifies the type of the leaf most internal delegate
					string delegateTypePropertyName = GetDelegateTypePropertyName (delName);

					// The name of the method to instance up said internal delegate
					string delegateCreationMethodName = "CreateInternalEvent" + delName + "Type";

					// If we're not the base class in our inheritance tree to define this delegate, we need to override instead
					bool shouldOverride = HasParentWithSameNamedDelegate (bta, delName);

					// The name of the protocol in question
					string interfaceName = GenerateInterfaceTypeName (bta, delName, dtype.Name);

					bool hasKeepRefUntil = bta.KeepRefUntil is not null;

					if (isProtocolizedEventBacked) {
						// The generated virtual type property and creation virtual method
						string generatedTypeOverrideType = shouldOverride ? "override" : "virtual";
						print ("internal {0} Type {1}", generatedTypeOverrideType, delegateTypePropertyName);
						print ("{");
						print ("	get {{ return typeof (_{0}); }}", dtype.Name);
						print ("}\n");

						print ("internal {0} _{1} {2} ({3})", generatedTypeOverrideType, interfaceName, delegateCreationMethodName, hasKeepRefUntil ? "object oref" : "");
						print ("{");
						print ("	return (_{0})(new _{1}({2}));", interfaceName, dtype.Name, hasKeepRefUntil ? "oref" : "");
						print ("}\n");
					}

					if (!hasKeepRefUntil)
						print ("{0}_{1} Ensure{1} ()", isProtocolizedEventBacked ? "internal " : "", dtype.Name);
					else {
						print ("static System.Collections.ArrayList? instances;");
						print ("{0}_{1} Ensure{1} (object oref)", isProtocolizedEventBacked ? "internal " : "", dtype.Name);
					}

					print ("{"); indent++;

					if (isProtocolizedEventBacked) {
						// If our delegate not null and it isn't the same type as our property
						//   - We're in one of two cases: The user += an Event and then assigned their own delegate or the inverse
						//   - One of them isn't being called anymore no matter what. Throw an exception.
						if (!BindThirdPartyLibrary) {
							print ("if (Weak{0} is not null)", delName);
							print ("\t{0}.EnsureEventAndDelegateAreNotMismatched (Weak{1}, {2});", ApplicationClassName, delName, delegateTypePropertyName);
						}

						print ("var del = {1} as _{0};", dtype.Name, delName);

						print ("if (del is null){");
						indent++;
						if (!hasKeepRefUntil)
							print ("del = (_{0}){1} ();", dtype.Name, delegateCreationMethodName);
						else {
							string oref = "new object[] {\"oref\"}";
							print ("del = (_{0}){1} ({2});", dtype.Name, delegateCreationMethodName, oref);
							print ("if (instances is null) instances = new System.Collections.ArrayList ();");
							print ("if (!instances.Contains (this)) instances.Add (this);");
						}
						print ("{0} = (I{1})del;", delName, dtype.Name);
						indent--;
						print ("}");
						print ("return del;");
					} else {
						print ("var del = {0};", delName);
						print ("if (del is null || (!(del is _{0}))){{", dtype.Name);
						print ("\tdel = new _{0} ({1});", dtype.Name, bta.KeepRefUntil is null ? "" : "oref");
						if (hasKeepRefUntil) {
							print ("\tif (instances is null) instances = new System.Collections.ArrayList ();");
							print ("\tif (!instances.Contains (this)) instances.Add (this);");
						}
						print ("\t{0} = del;", delName);
						print ("}");
						print ("return (_{0}) del;", dtype.Name);
					}
					indent--; print ("}\n");

					var noDefaultValue = new List<MethodInfo> ();

					print ("#pragma warning disable 672");
					print ("[Register]");
					if (isProtocolizedEventBacked)
						print ("internal class _{0} : {1}I{2} {{ ", dtype.Name, shouldOverride ? "_" + interfaceName + ", " : "NSObject, ", dtype.Name);
					else
						print ("sealed class _{0} : {1} {{ ", dtype.Name, TypeManager.RenderType (dtype));


					indent++;
					if (hasKeepRefUntil) {
						print ("object reference;");
						print ("public _{0} (object reference) {{ this.reference = reference; IsDirectBinding = false; }}\n", dtype.Name);
					} else
						print ("public _{0} () {{ IsDirectBinding = false; }}\n", dtype.Name);


					string shouldOverrideDelegateString = isProtocolizedEventBacked ? "" : "override ";

					string previous_miname = null;
					int miname_count = 0;
					foreach (var mi in dtype.GatherMethods (this).OrderBy (m => m.Name, StringComparer.Ordinal)) {
						if (ShouldSkipEventGeneration (mi))
							continue;

						var pars = mi.GetParameters ();
						int minPars = bta.Singleton ? 0 : 1;

						if (AttributeManager.HasAttribute<NoDefaultValueAttribute> (mi))
							noDefaultValue.Add (mi);

						if (pars.Length < minPars)
							throw new BindingException (1003, true, dtype.FullName, mi.Name);

						var sender = pars.Length == 0 ? "this" : pars [0].Name;

						var miname = mi.Name.PascalCase ();
						if (miname == previous_miname) {
							// overloads, add a numbered suffix (it's internal)
							previous_miname = miname;
							miname += (++miname_count).ToString ();
						} else
							previous_miname = miname;

						if (mi.ReturnType == TypeCache.System_Void) {
							if (bta.Singleton || mi.GetParameters ().Length == 1)
								print ("internal EventHandler? {0};", miname);
							else
								print ("internal EventHandler<{0}>? {1};", Nomenclator.GetEventArgName (mi), miname);
						} else
							print ("internal {0}? {1};", Nomenclator.GetDelegateName (mi), miname);

						print ("[Preserve (Conditional = true)]");
						if (isProtocolizedEventBacked)
							print ("[Export (\"{0}\")]", FindSelector (dtype, mi));

						print ("public {0}{1} {2} ({3})", shouldOverrideDelegateString, TypeManager.RenderType (mi.ReturnType), mi.Name, RenderParameterDecl (pars));
						print ("{"); indent++;

						if (mi.Name == bta.KeepRefUntil)
							print ("instances?.Remove (reference);");

						if (mi.ReturnType == TypeCache.System_Void) {
							string eaname;

							if (debug)
								print ("Console.WriteLine (\"Method {0}.{1} invoked\");", dtype.Name, mi.Name);
							if (pars.Length != minPars) {
								eaname = Nomenclator.GetEventArgName (mi);
								if (!generatedEvents.ContainsKey (eaname) && !eventArgTypes.ContainsKey (eaname)) {
									eventArgTypes.Add (eaname, pars);
									generatedEvents.Add (eaname, pars);
								}
							} else
								eaname = "<NOTREACHED>";

							print ("var handler = {0};", miname.PascalCase ());
							print ("if (handler is not null){");
							indent++;
							string eventArgs;
							if (pars.Length == minPars)
								eventArgs = "EventArgs.Empty";
							else {
								print ("var args = new {0} ({1});", eaname, RenderArgs (pars.Skip (minPars), true));
								eventArgs = "args";
							}

							print ("handler ({0}, {1});", sender, eventArgs);
							if (pars.Length != minPars && MustPullValuesBack (pars.Skip (minPars))) {
								foreach (var par in pars.Skip (minPars)) {
									if (!par.ParameterType.IsByRef)
										continue;

									print ("{0} = args.{1};", par.Name, GetPublicParameterName (par));
								}
							}
							if (AttributeManager.HasAttribute<CheckDisposedAttribute> (mi)) {
								var arg = RenderArgs (pars.Take (1));
								print ("if ({0}.Handle == IntPtr.Zero)", arg);
								print ("\tthrow new ObjectDisposedException (\"{0}\", \"The object was disposed on the event, you should not call Dispose() inside the handler\");", arg);
							}
							indent--;
							print ("}");
						} else {
							var delname = Nomenclator.GetDelegateName (mi);

							if (!generatedDelegates.ContainsKey (delname) && !delegate_types.ContainsKey (delname)) {
								generatedDelegates.Add (delname, null);
								delegate_types.Add (type.Namespace + "." + delname, mi);
							}
							if (debug)
								print ("Console.WriteLine (\"Method {0}.{1} invoked\");", dtype.Name, mi.Name);

							print ("var handler = {0};", miname.PascalCase ());
							print ("if (handler is not null)");
							print ("	return handler ({0}{1});",
								   sender,
								   pars.Length == minPars ? "" : String.Format (", {0}", RenderArgs (pars.Skip (1))));

							if (AttributeManager.HasAttribute<NoDefaultValueAttribute> (mi))
								print ("throw new You_Should_Not_Call_base_In_This_Method ();");
							else {
								var def = GetDefaultValue (mi);
								if ((def is string) && ((def as string) == "null") && mi.ReturnType.IsValueType)
									print ("throw new Exception (\"No event handler has been added to the {0} event.\");", mi.Name);
								else {
									foreach (var j in pars) {
										if (j.ParameterType.IsByRef && j.IsOut) {
											print ("{0} = null;", j.Name.GetSafeParamName ());
										}
									}

									if (mi.ReturnType == TypeCache.System_nint) {
										print ("return ((nint) ({0}));", def);
									} else if (mi.ReturnType == TypeCache.System_nuint) {
										print ("return ((nuint) ({0}));", def);
									} else {
										print ("return {0}!;", def);
									}
								}
							}
						}

						indent--;
						print ("}\n");
					}

					if (noDefaultValue.Count > 0) {
						string selRespondsToSelector = "Selector.GetHandle (\"respondsToSelector:\")";

						if (!InlineSelectors) {
							foreach (var mi in noDefaultValue) {
								var export = AttributeManager.GetCustomAttribute<ExportAttribute> (mi);
								print ("static {2} sel{0}Handle = Selector.GetHandle (\"{1}\");", mi.Name, export.Selector, NativeHandleType);
							}
							print ("static {0} selRespondsToSelector = " + selRespondsToSelector + ";", NativeHandleType);
							selRespondsToSelector = "selRespondsToSelector";
						}

						print ("[Preserve (Conditional = true)]");
						print ("public override bool RespondsToSelector (Selector? sel)");
						print ("{");
						++indent;
						print ("if (sel is null)");
						++indent;
						print ("return false;");
						--indent;
						print ("{0} selHandle = sel.Handle;", NativeHandleType);
						foreach (var mi in noDefaultValue.OrderBy (m => m.Name, StringComparer.Ordinal)) {
							if (InlineSelectors) {
								var export = AttributeManager.GetCustomAttribute<ExportAttribute> (mi);
								print ("if (selHandle.Equals (Selector.GetHandle (\"{0}\")))", export.Selector);
							} else {
								print ("if (selHandle.Equals (sel{0}Handle))", mi.Name);
							}
							++indent;
							print ("return {0} is not null;", mi.Name.PascalCase ());
							--indent;
						}
						print ("return global::" + NamespaceCache.Messaging + ".bool_objc_msgSendSuper_IntPtr (SuperHandle, " + selRespondsToSelector + ", selHandle) != 0;");
						--indent;
						print ("}");

						// Make sure we generate the required signature in Messaging only if needed 
						// bool_objc_msgSendSuper_IntPtr: for respondsToSelector:
						if (!send_methods.ContainsKey ("bool_objc_msgSendSuper_IntPtr")) {
							print (m, "[DllImport (LIBOBJC_DYLIB, EntryPoint=\"objc_msgSendSuper\")]");
							print (m, "public extern static byte bool_objc_msgSendSuper_IntPtr (IntPtr receiever, IntPtr selector, IntPtr arg1);");
							RegisterMethodName ("bool_objc_msgSendSuper_IntPtr");
						}
					}

					indent--;
					print ("}");

					print ("#pragma warning restore 672");
				}
				print ("");

				string prev_miname = null;
				int minameCount = 0;
				Nomenclator.ForgetDelegateApiNames ();
				// Now add the instance vars and event handlers
				foreach (var dtype in bta.Events.OrderBy (d => d.Name, StringComparer.Ordinal)) {
					foreach (var mi in dtype.GatherMethods (this).OrderBy (m => m.Name, StringComparer.Ordinal)) {
						if (ShouldSkipEventGeneration (mi))
							continue;

						string ensureArg = bta.KeepRefUntil is null ? "" : "this";

						var miname = mi.Name.PascalCase ();
						if (miname == prev_miname) {
							// overloads, add a numbered suffix (it's internal)
							prev_miname = miname;
							miname += (++minameCount).ToString ();
						} else
							prev_miname = miname;

						if (mi.ReturnType == TypeCache.System_Void) {
							PrintObsoleteAttributes (mi);

							if (bta.Singleton && mi.GetParameters ().Length == 0 || mi.GetParameters ().Length == 1)
								print ("public event EventHandler {0} {{", Nomenclator.GetEventName (mi).CamelCase ());
							else
								print ("public event EventHandler<{0}> {1} {{", Nomenclator.GetEventArgName (mi), Nomenclator.GetEventName (mi).CamelCase ());
							print ("\tadd {{ Ensure{0} ({1})!.{2} += value; }}", dtype.Name, ensureArg, miname);
							print ("\tremove {{ Ensure{0} ({1})!.{2} -= value; }}", dtype.Name, ensureArg, miname);
							print ("}\n");
						} else {
							print ("public {0}? {1} {{", Nomenclator.GetDelegateName (mi), Nomenclator.GetDelegateApiName (mi).CamelCase ());
							print ("\tget {{ return Ensure{0} ({1})!.{2}; }}", dtype.Name, ensureArg, miname);
							print ("\tset {{ Ensure{0} ({1})!.{2} = value; }}", dtype.Name, ensureArg, miname);
							print ("}\n");
						}
					}
				}
			}

			//
			// Do we need a dispose method?
			//
			if (!is_static_class) {
				var attrs = AttributeManager.GetCustomAttributes<DisposeAttribute> (type);
				// historical note: unlike many attributes our `DisposeAttribute` has `AllowMultiple=true`
				var has_dispose_attributes = attrs.Length > 0;
				if (has_dispose_attributes || (instance_fields_to_clear_on_dispose.Count > 0)) {
					// if there'a any [Dispose] attribute then they all must opt-in in order for the generated Dispose method to be optimizable
					bool optimizable = !has_dispose_attributes || IsOptimizable (type);
					print_generated_code (optimizable: optimizable);
					print ("protected override void Dispose (bool disposing)");
					print ("{");
					indent++;
					if (has_dispose_attributes) {
						foreach (var da in attrs)
							Inject (da);
					}

					print ("base.Dispose (disposing);");

					if (instance_fields_to_clear_on_dispose.Count > 0) {
						print ("if (Handle == IntPtr.Zero) {");
						indent++;
						foreach (var field in instance_fields_to_clear_on_dispose.OrderBy (f => f, StringComparer.Ordinal))
							print ("{0} = null;", field);
						indent--;
						print ("}");
					}

					indent--;
					print ("}");
				}
			}

			//
			// Appearance class
			//
			var gt = GeneratedTypes.Lookup (type);
			if (gt.ImplementsAppearance) {
				var parent_implements_appearance = gt.Parent is not null && gt.ParentGenerated.ImplementsAppearance;
				string base_class;

				if (parent_implements_appearance) {
					var parent = GetGeneratedTypeName (gt.Parent);
					base_class = "global::" + gt.Parent.FullName + "." + parent + "Appearance";
				} else
					base_class = "UIAppearance";

				string appearance_type_name = TypeName + "Appearance";
				print ("public partial class {0} : {1} {{", appearance_type_name, base_class);
				indent++;
				print ("protected internal {0} (IntPtr handle) : base (handle) {{}}", appearance_type_name);

				if (appearance_selectors is not null) {
					var currently_ignored_fields = new List<string> ();

					foreach (MemberInfo mi in appearance_selectors.OrderBy (m => m.Name, StringComparer.Ordinal)) {
						if (mi is MethodInfo)
							GenerateMethod (type, mi as MethodInfo,
									is_model: false,
									category_extension_type: is_category_class ? base_type : null,
									is_appearance: true);
						else
							GenerateProperty (type, mi as PropertyInfo, currently_ignored_fields, false);
					}
				}

				indent--;
				print ("}\n");
				print ("public static {0}{1} Appearance {{", parent_implements_appearance ? "new " : "", appearance_type_name);
				indent++;
				print ("get {{ return new {0} (global::{1}.IntPtr_objc_msgSend (class_ptr, {2})); }}", appearance_type_name, NamespaceCache.Messaging, InlineSelectors ? "ObjCRuntime.Selector.GetHandle (\"appearance\")" : "UIAppearance.SelectorAppearance");
				indent--;
				print ("}\n");
				print ("public static {0}{1} GetAppearance<T> () where T: {2} {{", parent_implements_appearance ? "new " : "", appearance_type_name, TypeName);
				indent++;
				print ("return new {0} (global::{1}.IntPtr_objc_msgSend (Class.GetHandle (typeof (T)), {2}));", appearance_type_name, NamespaceCache.Messaging, InlineSelectors ? "ObjCRuntime.Selector.GetHandle (\"appearance\")" : "UIAppearance.SelectorAppearance");
				indent--;
				print ("}\n");
				print ("public static {0}{1} AppearanceWhenContainedIn (params Type [] containers)", parent_implements_appearance ? "new " : "", appearance_type_name);
				print ("{");
				indent++;
				print ("return new {0} (UIAppearance.GetAppearance (class_ptr, containers));", appearance_type_name);
				indent--;
				print ("}\n");

				print ("public static {0}{1} GetAppearance (UITraitCollection traits) {{", parent_implements_appearance ? "new " : "", appearance_type_name);
				indent++;
				print ("return new {0} (UIAppearance.GetAppearance (class_ptr, traits));", appearance_type_name);
				indent--;
				print ("}\n");

				print ("public static {0}{1} GetAppearance (UITraitCollection traits, params Type [] containers) {{", parent_implements_appearance ? "new " : "", appearance_type_name);
				indent++;
				print ("return new {0} (UIAppearance.GetAppearance (class_ptr, traits, containers));", appearance_type_name);
				indent--;
				print ("}\n");

				print ("public static {0}{1} GetAppearance<T> (UITraitCollection traits) where T: {2} {{", parent_implements_appearance ? "new " : "", appearance_type_name, TypeName);
				indent++;
				print ("return new {0} (UIAppearance.GetAppearance (Class.GetHandle (typeof (T)), traits));", appearance_type_name);
				indent--;
				print ("}\n");

				print ("public static {0}{1} GetAppearance<T> (UITraitCollection traits, params Type [] containers) where T: {2}{{", parent_implements_appearance ? "new " : "", appearance_type_name, TypeName);
				indent++;
				print ("return new {0} (UIAppearance.GetAppearance (Class.GetHandle (typeof (T)), containers));", appearance_type_name);
				indent--;
				print ("}\n");

				print ("");
			}
			//
			// Notification extensions
			//
			if (notifications.Count > 0) {
				print ("\n");
				print ("//");
				print ("// Notifications");
				print ("//");

				print ("public static partial class Notifications {\n");
				foreach (var property in notifications.OrderBy (p => p.Name, StringComparer.Ordinal)) {
					string notification_name = GetNotificationName (property);
					string notification_center = GetNotificationCenter (property);

					foreach (var notification_attribute in AttributeManager.GetCustomAttributes<NotificationAttribute> (property)) {
						Type event_args_type = notification_attribute.Type;
						string event_name = event_args_type is null ? "NSNotificationEventArgs" : event_args_type.FullName;

						if (event_args_type is not null)
							notification_event_arg_types [event_args_type] = event_args_type;
						print ("\tpublic static NSObject Observe{0} (EventHandler<{1}> handler)", notification_name, event_name);
						print ("\t{");
						print ("\t\treturn {0}.AddObserver ({1}, notification => handler (null, new {2} (notification)));", notification_center, property.Name, event_name);
						print ("\t}");
						print ("\tpublic static NSObject Observe{0} (NSObject objectToObserve, EventHandler<{1}> handler)", notification_name, event_name);
						print ("\t{");
						print ("\t\treturn {0}.AddObserver ({1}, notification => handler (null, new {2} (notification)), objectToObserve);", notification_center, property.Name, event_name);
						print ("\t}");
					}
				}
				print ("\n}");
			}

			indent--;
			print ("}} /* class {0} */", TypeName);

			//
			// Copy delegates from the API files into the output if they were declared there
			//
			var rootAssembly = types [0].Assembly;
			foreach (var deltype in trampolines.Keys.OrderBy (d => d.Name, StringComparer.Ordinal)) {
				if (deltype.Assembly != rootAssembly)
					continue;

				// This formats the delegate 
				delegate_types [deltype.FullName] = deltype.GetMethod ("Invoke");
			}

			if (eventArgTypes.Count > 0) {
				print ("\n");
				print ("//");
				print ("// EventArgs classes");
				print ("//");
			}
			// Now add the EventArgs classes
			foreach (var eaclass in eventArgTypes.Keys.OrderBy (e => e, StringComparer.Ordinal)) {
				if (Nomenclator.WasEventArgGenerated (eaclass)) {
					continue;
				}
				int minPars = bta.Singleton ? 0 : 1;

				var pars = eventArgTypes [eaclass];

				print ("public partial class {0} : EventArgs {{", eaclass); indent++;
				print ("public {0} ({1})", eaclass, RenderParameterDecl (pars.Skip (1), true));
				print ("{");
				indent++;
				foreach (var p in pars.Skip (minPars).OrderBy (p => p.Name, StringComparer.Ordinal)) {
					print ("this.{0} = {1};", GetPublicParameterName (p), p.Name.GetSafeParamName ());
				}
				indent--;
				print ("}");

				// Now print the properties
				foreach (var p in pars.Skip (minPars).OrderBy (p => p.Name, StringComparer.Ordinal)) {
					var pt = p.ParameterType;
					var bareType = pt.IsByRef ? pt.GetElementType () : pt;
					var nullable = !pt.IsValueType && AttributeManager.HasAttribute<NullAllowedAttribute> (p);

					print ("public {0}{1} {2} {{ get; set; }}", TypeManager.RenderType (bareType), nullable ? "?" : "", GetPublicParameterName (p));
				}
				indent--; print ("}\n");
			}

			if (async_result_types.Count > 0) {
				print ("\n");
				print ("//");
				print ("// Async result classes");
				print ("//");
			}

			foreach (var async_type in async_result_types.OrderBy (t => t.Item1, StringComparer.Ordinal)) {
				if (async_result_types_emitted.Contains (async_type.Item1))
					continue;
				async_result_types_emitted.Add (async_type.Item1);

				print ("public partial class {0} {{", async_type.Item1); indent++;

				StringBuilder ctor = new StringBuilder ();

				bool comma = false;
				foreach (var pi in async_type.Item2) {
					var safe_name = pi.Name.GetSafeParamName ();
					print ("public {0} {1} {{ get; set; }}",
						TypeManager.FormatType (type, pi.ParameterType),
						safe_name.Capitalize ());

					if (comma)
						ctor.Append (", ");
					comma = true;
					ctor.Append (TypeManager.FormatType (type, pi.ParameterType)).Append (" ").Append (safe_name);
				}

				print ("\npartial void Initialize ();");

				print ("\npublic {0} ({1}) {{", async_type.Item1, ctor); indent++;
				foreach (var pi in async_type.Item2) {
					var safe_name = pi.Name.GetSafeParamName ();
					print ("this.{0} = {1};", safe_name.Capitalize (), safe_name);
				}
				print ("Initialize ();");
				indent--; print ("}");

				indent--; print ("}\n");
			}
			async_result_types.Clear ();

			if (type.IsNested) {
				indent--;
				print ("}");
			}
			if (type.Namespace is not null) {
				indent--;
				print ("}");
			}
		}
	}

	static string GetDelegateTypePropertyName (string delName)
	{
		return "GetInternalEvent" + delName + "Type";
	}

	bool SafeIsProtocolizedEventBacked (string propertyName, Type type)
	{
		return CoreIsProtocolizedEventBacked (propertyName, type, false);
	}

	bool IsProtocolizedEventBacked (string propertyName, Type type)
	{
		return CoreIsProtocolizedEventBacked (propertyName, type, true);
	}

	bool CoreIsProtocolizedEventBacked (string propertyName, Type type, bool shouldThrowOnNotFound)
	{
		PropertyInfo pi = type.GetProperty (propertyName);
		BaseTypeAttribute bta = ReflectionExtensions.GetBaseTypeAttribute (type, this);
		if (pi is null || bta is null) {
			if (shouldThrowOnNotFound) {
				if (propertyName == "Delegate" && bta.Delegates.Count () > 0) {
					var delegates = new List<string> (bta.Delegates);
					// grab all the properties that have the Wrap attr
					var propsAttrs = from p in type.GetProperties ()
									 let attrs = AttributeManager.GetCustomAttributes<WrapAttribute> (p)
									 where p.Name != "Delegate" && attrs.Length > 0
									 select new { Name = p.Name, Attributes = Array.ConvertAll (attrs, item => item.MethodName) };

					var props = new List<string> ();
					foreach (var p in propsAttrs) {
						if (delegates.Intersect (p.Attributes).Any ()) {
							// add quoates since we are only using this info for the exception message
							props.Add (String.Format ("'{0}'", p.Name));
						}
					}
					if (props.Count == 1)
						throw new BindingException (1112, true,
							 props [0], false);
					else if (props.Count > 1)
						throw new BindingException (1112, true,
							String.Join (", ", props.ToArray ()), false);
					else
						throw new BindingException (1113, true, false);
				} else
					throw new BindingException (1114, propertyName, type, false);
			} else
				return false;
		}

		return Protocolize (pi) && bta.Events is not null && bta.Events.Any (x => x.Name == pi.PropertyType.Name);
	}

	string FindSelector (Type type, MethodInfo mi)
	{
		Type currentType = type;
		do {
			// avoid AmbiguousMatchException when GetMethod is used.
			var parameters = mi.GetParameters ().Select ((arg) => arg.ParameterType).ToArray ();
			MethodInfo method = currentType.GetMethod (mi.Name, parameters);
			if (method is not null) {
				string wrap;
				ExportAttribute export = GetExportAttribute (method, out wrap);
				if (export is not null)
					return export.Selector;
			}
			BaseTypeAttribute bta = ReflectionExtensions.GetBaseTypeAttribute (currentType, this);

			if (bta is null)
				break;
			currentType = bta.BaseType;
		}
		while (currentType is not null);
		throw new BindingException (1076, true, mi, type);
	}

	string GenerateInterfaceTypeName (BaseTypeAttribute bta, string delName, string currentTypeName)
	{
		bool shouldOverride = HasParentWithSameNamedDelegate (bta, delName);
		if (shouldOverride) {
			Type parentType = GetParentTypeWithSameNamedDelegate (bta, delName);
			PropertyInfo parentProperty = parentType.GetProperty (delName);
			return parentProperty.PropertyType.Name;
		}
		return currentTypeName;
	}

	bool ShouldSkipEventGeneration (MethodInfo mi)
	{
		// Skip property getter/setters
		if (mi.IsSpecialName && (mi.Name.StartsWith ("get_", StringComparison.Ordinal) || mi.Name.StartsWith ("set_", StringComparison.Ordinal)))
			return true;

		if (mi.IsUnavailable (this))
			return true;

		// Skip those methods marked to be ignored by the developer
		return AttributeManager.HasAttribute<IgnoredInDelegateAttribute> (mi);
	}

	// Safely strips away any Weak from the beginning of either delegate and returns if they match
	static bool CompareTwoDelegateNames (string lhsDel, string rhsDel)
	{
		lhsDel = lhsDel.StartsWith ("Weak", StringComparison.Ordinal) ? lhsDel.Substring (4) : lhsDel;
		rhsDel = rhsDel.StartsWith ("Weak", StringComparison.Ordinal) ? rhsDel.Substring (4) : rhsDel;
		return lhsDel == rhsDel;
	}

	Type GetParentTypeWithSameNamedDelegate (BaseTypeAttribute bta, string delegateName)
	{
		Type currentType = bta.BaseType;
		while (currentType is not null && currentType != TypeCache.NSObject) {
			BaseTypeAttribute currentBta = ReflectionExtensions.GetBaseTypeAttribute (currentType, this);
			if (currentBta is not null && currentBta.Events is not null) {
				int delidx = 0;
				foreach (var v in currentBta.Events) {
					string currentDelName = currentBta.Delegates [delidx++];
					if (CompareTwoDelegateNames (currentDelName, delegateName))
						return currentType;
				}
			}
			currentType = currentType.BaseType;
		}
		return null;
	}

	bool HasParentWithSameNamedDelegate (BaseTypeAttribute bta, string delegateName)
	{
		return GetParentTypeWithSameNamedDelegate (bta, delegateName) is not null;
	}

	string GetNotificationCenter (PropertyInfo pi)
	{
		var a = AttributeManager.GetCustomAttributes<NotificationAttribute> (pi);
		return a [0].NotificationCenter ?? "NSNotificationCenter.DefaultCenter";
	}

	string GetNotificationName (PropertyInfo pi)
	{
		// TODO: fetch the NotificationAttribute, see if there is an override there.
		var name = pi.Name;
		if (name.EndsWith ("Notification", StringComparison.Ordinal))
			return name.Substring (0, name.Length - "Notification".Length);
		return name;
	}

	Type GetNotificationArgType (PropertyInfo pi)
	{
		return AttributeManager.GetCustomAttributes<NotificationAttribute> (pi) [0].Type;
	}

	//
	// Support for the automatic delegate/event generation
	//
	string RenderParameterDecl (IEnumerable<ParameterInfo> pi)
	{
		return RenderParameterDecl (pi, false);
	}

	string RenderParameterDecl (IEnumerable<ParameterInfo> pi, bool removeRefTypes)
	{
		return String.Join (", ", pi.Select (p => RenderSingleParameter (p, removeRefTypes) + " " + p.Name.GetSafeParamName ()).ToArray ());
	}

	string RenderSingleParameter (ParameterInfo p, bool removeRefTypes)
	{
		var protocolized = Protocolize (p);
		var prefix = protocolized ? "I" : "";
		var pt = p.ParameterType;

		string name;
		if (pt.IsByRef) {
			pt = pt.GetElementType ();
			name = (removeRefTypes ? "" : (p.IsOut ? "out " : "ref ")) + prefix + TypeManager.RenderType (pt, p);
		} else
			name = prefix + TypeManager.RenderType (pt, p);
		if (!pt.IsValueType && AttributeManager.HasAttribute<NullAllowedAttribute> (p))
			name += "?";
		return name;
	}

	string GetPublicParameterName (ParameterInfo pi)
	{
		var attrs = AttributeManager.GetCustomAttributes<EventNameAttribute> (pi);
		if (attrs.Length == 0)
			return pi.Name.CamelCase ().GetSafeParamName ();

		var a = attrs [0];
		return a.EvtName.CamelCase ().GetSafeParamName ();
	}

	string RenderArgs (IEnumerable<ParameterInfo> pi)
	{
		return RenderArgs (pi, false);
	}

	string RenderArgs (IEnumerable<ParameterInfo> pi, bool removeRefTypes)
	{
		return String.Join (", ", pi.Select (p => (p.ParameterType.IsByRef ? (removeRefTypes ? "" : (p.IsOut ? "out " : "ref ")) : "") + p.Name.GetSafeParamName ()).ToArray ());
	}

	bool MustPullValuesBack (IEnumerable<ParameterInfo> parameters)
	{
		return parameters.Any (pi => pi.ParameterType.IsByRef);
	}

	object GetDefaultValue (MethodInfo mi)
	{
		Attribute a = AttributeManager.GetCustomAttribute<DefaultValueAttribute> (mi);
		if (a is null) {
			a = AttributeManager.GetCustomAttribute<DefaultValueFromArgumentAttribute> (mi);
			if (a is not null) {
				var fvfa = (DefaultValueFromArgumentAttribute) a;
				return fvfa.Argument;
			}

			throw new BindingException (1016, true, mi.DeclaringType.FullName, mi.Name);
		}
		var def = ((DefaultValueAttribute) a).Default;
		if (def is null)
			return "null";

		var type = def as Type;
		if (type is not null && (
			type.FullName == "System.Drawing.PointF" ||
			type.FullName == "System.Drawing.SizeF" ||
			type.FullName == "System.Drawing.RectangleF" ||
			type.FullName == "CoreGraphics.CGPoint" ||
			type.FullName == "CoreGraphics.CGSize" ||
			type.FullName == "CoreGraphics.CGRect"))
			return type.FullName + ".Empty";

		if (def is bool)
			return (bool) def ? "true" : "false";

		if (mi.ReturnType.IsEnum) {
			if (def is string)
				return def;
			var name = Enum.GetName (mi.ReturnType, def);
			if (string.IsNullOrEmpty (name)) {
				// The value could be negative so it need to be enclosed in parenthesis
				return "(" + mi.ReturnType.FullName + ") (" + def + ")";
			} else {
				return mi.ReturnType.FullName + "." + name;
			}
		}

		return def;
	}

	private static string FormatPropertyInfo (PropertyInfo pi)
	{
		return pi.DeclaringType.FullName + " " + pi.Name;
	}

	// Format a provider for error messages
	public static string FormatProvider (ICustomAttributeProvider provider)
	{
		if (provider is Type type) {
			return type.FullName;
		} else if (provider is MemberInfo mi) {
			return mi.DeclaringType.FullName + "." + mi.Name;
		} else {
			return provider?.ToString ();
		}
	}
}
