// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#pragma warning disable format
using Microsoft.Macios.Transformer.Attributes;
using Microsoft.Macios.Transformer.Generator;

namespace Microsoft.Macios.Transformer;

static class AttributesNames {

	/// <summary>
	/// The [Abstract] attribute can be applied to either methods or properties and causes the generator to flag the
	/// generated member as abstract and the class to be an abstract class.
	/// </summary>
	[BindingFlag (AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Interface)]
	public const string AbstractAttribute = "AbstractAttribute";

	[BindingFlag] 
	public const string AdvancedAttribute = "AdvancedAttribute";
	public const string AdviceAttribute = "Foundation.AdviceAttribute";

	/// <summary>
	/// The [Appearance] attribute can be applied to any method or property that participate in the
	/// UIAppearance framework
	/// </summary>
	[BindingFlag (AttributeTargets.Property | AttributeTargets.Method)]
	public const string AppearanceAttribute = "AppearanceAttribute";

	/// <summary>
	/// Use the [AutoReleaseAttribute] on methods and properties to wrap the method
	/// invocation to the method in an NSAutoReleasePool.
	/// </summary>
	[BindingFlag (AttributeTargets.Method | AttributeTargets.Property)]
	public const string AutoreleaseAttribute = "AutoreleaseAttribute";
	
	[BindingAttribute(typeof(AsyncData), AttributeTargets.Method)]
	public const string AsyncAttribute = "AsyncAttribute";

	[BindingAttribute(typeof(BackingFieldTypeData), AttributeTargets.Enum)]
	public const string BackingFieldTypeAttribute = "BackingFieldTypeAttribute";
	public const string BaseTypeAttribute = "BaseTypeAttribute";
	[BindingAttribute(typeof(BindData), AttributeTargets.Method )]
	public const string BindAttribute = "BindAttribute";

	/// <summary>
	/// Use this attribute on a type definition to bind Objective-C categories and to expose those as C# extension
	/// methods to mirror the way Objective-C exposes the functionality.
	/// </summary>
	[BindingFlag (AttributeTargets.Interface)]
	public const string CategoryAttribute = "CategoryAttribute";

	[BindingFlag] 
	public const string CheckDisposedAttribute = "CheckDisposedAttribute";
	
	[BindingAttribute(typeof(CoreImageFilterData), AttributeTargets.Interface)]
	public const string CoreImageFilterAttribute = "CoreImageFilterAttribute";
	
	[BindingAttribute(typeof(CoreImageFilterPropertyData), AttributeTargets.Property)]
	public const string CoreImageFilterPropertyAttribute = "CoreImageFilterPropertyAttribute";
	
	[BindingFlag (AttributeTargets.Interface)]
	public const string DefaultCtorVisibilityAttribute = "DefaultCtorVisibilityAttribute";

	[BindingFlag (AttributeTargets.Field)]
	public const string DefaultEnumValueAttribute = "DefaultEnumValueAttribute";
	public const string DeprecatedAttribute = "DeprecatedAttribute";

	[BindingFlag (AttributeTargets.Interface)]
	public const string DesignatedDefaultCtorAttribute = "DesignatedDefaultCtorAttribute";

	[BindingFlag (AttributeTargets.Constructor | AttributeTargets.Method)]
	public const string DesignatedInitializerAttribute = "DesignatedInitializerAttribute";

	/// <summary>
	/// When this attribute is applied to the interface definition it will prevent the generator from producing
	/// the default constructor.
	/// </summary>
	[BindingFlag (AttributeTargets.Interface)]
	public const string DisableDefaultCtorAttribute = "DisableDefaultCtorAttribute";

	/// <summary>
	/// This attribute is applied to string parameters or string properties and instructs the code generator to not
	/// use the zero-copy string marshaling for this parameter, and instead create a new NSString instance from the C# string
	/// </summary>
	[BindingFlag (AttributeTargets.Parameter | AttributeTargets.Property)]
	public const string DisableZeroCopyAttribute = "DisableZeroCopyAttribute";

	/// <summary>
	/// Code to run from a generated Dispose method, before any generated code is executed
	/// Adding this attribute will, by default, make the method non-optimizable by the SDK tools
	/// </summary>
	[BindingAttribute(typeof(ErrorDomainData), AttributeTargets.Interface | AttributeTargets.Class)]
	public const string DisposeAttribute = "DisposeAttribute";
	
	public const string EditorBrowsableAttribute = "System.ComponentModel.EditorBrowsableAttribute";
	
	[BindingAttribute(typeof(ErrorDomainData), AttributeTargets.Enum)]
	public const string ErrorDomainAttribute = "ErrorDomainAttribute";
	
	public const string ExperimentalAttribute = "System.Diagnostics.CodeAnalysis.ExperimentalAttribute";
	
	[BindingAttribute(typeof(ExportData), AttributeTargets.Method | AttributeTargets.Property)]
	public const string ExportAttribute = "Foundation.ExportAttribute";

	[BindingFlag] 
	public const string FactoryAttribute = "FactoryAttribute";
	
	[BindingAttribute(typeof(FieldData), AttributeTargets.Field | AttributeTargets.Property)]
	public const string FieldAttribute = "Foundation.FieldAttribute";
	
	[BindingFlag (AttributeTargets.Enum)]
	public const string FlagsAttribute = "System.FlagsAttribute";

	/// <summary>
	/// Sometimes it makes sense not to expose an event or delegate property from a Model class into the host class so
	/// adding this attribute will instruct the generator to avoid the generation of any method decorated with it.
	/// </summary>
	[BindingFlag (AttributeTargets.Method | AttributeTargets.Property)]
	public const string IgnoredInDelegateAttribute = "IgnoredInDelegateAttribute";

	/// <summary>
	/// The [Internal] attribute can be applied to methods or properties and it has the effect of flagging the
	/// generated code with the internal C# keyword making the code only accessible to code in the generated assembly.
	/// </summary>
	[BindingFlag (AttributeTargets.Method | AttributeTargets.Property)]
	public const string InternalAttribute = "InternalAttribute";

	public const string IntroducedAttribute = "IntroducedAttribute";

	/// <summary>
	/// This attribute flags the backing field for a property to be annotated with the .NET [ThreadStatic] attribute.
	/// This is useful if the field is a thread static variable.
	/// </summary>
	[BindingFlag (AttributeTargets.Property)]
	public const string IsThreadStaticAttribute = "IsThreadStaticAttribute";

	public const string MacAttribute = "ObjCRuntime.MacAttribute";
	public const string MacCatalystAttribute = "MacCatalystAttribute";

	/// <summary>
	///  Use this attribute if some definitions are required at definition-compile
	/// time but when you need the final binding assembly to include your own
	/// custom implementation
	/// </summary>
	[BindingFlag (AttributeTargets.Method | AttributeTargets.Property)]
	public const string ManualAttribute = "ManualAttribute";

	[BindingFlag]
	public const string MarshalNativeExceptionsAttribute = "MarshalNativeExceptionsAttribute";

	/// <summary>
	/// Mark a class or interface to be a model.
	/// </summary>
	[BindingFlag (AttributeTargets.Class | AttributeTargets.Interface)]
	public const string ModelAttribute = "Foundation.ModelAttribute";
	[BindingAttribute(typeof(NativeData), AttributeTargets.Enum)]
	public const string NativeAttribute = "ObjCRuntime.NativeAttribute";
	[BindingAttribute(typeof(NativeData), AttributeTargets.Enum | AttributeTargets.Struct)]
	public const string NativeNameAttribute = "ObjCRuntime.NativeNameAttribute";

	/// <summary>
	/// This attribute is applied to methods and properties to have the generator generate the new keyword in front
	/// of the declaration.
	/// </summary>
	[BindingFlag (AttributeTargets.Method | AttributeTargets.Property)]
	public const string NewAttribute = "NewAttribute";

	/// <summary>
	/// Specifies that the method on the model does not provide a default return value.
	/// </summary>
	[BindingFlag (AttributeTargets.Method)]
	public const string NoDefaultValueAttribute = "NoDefaultValueAttribute";

	public const string NoMacAttribute = "NoMacAttribute";
	public const string NoMacCatalystAttribute = "NoMacCatalystAttribute";

	/// <summary>
	/// This prevents the generator from generating the managed proxy to
	/// the method being called, this is done when we are interested in
	/// the side effects of the binding of the method, rather than the
	/// method itself 
	/// </summary>
	[BindingFlag (AttributeTargets.Method)]
	public const string NoMethodAttribute = "NoMethodAttribute";

	public const string NoTVAttribute = "NoTVAttribute";
	public const string NoiOSAttribute = "NoiOSAttribute";

	/// <summary>
	/// When this is applied to a property it flags the property as allowing the value null to be assigned to it.
	/// This is only valid for reference types.
	/// When this is applied to a parameter in a method signature it indicates that the specified parameter can
	/// be null and that no check should be performed for passing null values.
	/// </summary>
	[BindingFlag (AttributeTargets.Property | AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
	public const string NullAllowedAttribute = "NullAllowedAttribute";

	public const string ObsoleteAttribute = "System.ObsoleteAttribute";
	public const string ObsoletedAttribute = "ObsoletedAttribute";

	/// <summary>
	///  When this attribtue is applied to a property, currently it merely adds
	/// a DebuggerBrowsable(Never) to the property, to prevent a family of crashes
	/// </summary>
	[BindingFlag (AttributeTargets.Property)]
	public const string OptionalImplementationAttribute = "OptionalImplementationAttribute";

	/// <summary>
	/// Use this attribute to instruct the binding generator that the binding for this particular method should be
	/// flagged with an override keyword.
	/// </summary>
	[BindingFlag (AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event)]
	public const string OverrideAttribute = "OverrideAttribute";

	/// <summary>
	///  Indicates that this array should be turned into a params
	/// </summary>
	[BindingFlag (AttributeTargets.Parameter)]
	public const string ParamsAttribute = "ParamsAttribute";

	/// <summary>
	///  When applied to a type generate a partial class even if the type does not subclass NSObject
	/// useful for Core* types that declare Fields
	/// </summary>
	[BindingFlag (AttributeTargets.Interface)]
	public const string PartialAttribute = "PartialAttribute";

	[BindingFlag]
	public const string PlainStringAttribute = "PlainStringAttribute";

	/// <summary>
	/// PostSnippet code is inserted before returning, before paramters are disposed/released
	/// Adding this attribute will, by default, make the method non-optimizable by the SDK tools
	/// </summary>
	[BindingAttribute (typeof(SnippetData), AttributeTargets.Method | AttributeTargets.Property)]
	public const string PostSnippetAttribute = "PostSnippetAttribute";
	
	/// <summary>
	/// PreSnippet code is inserted after the parameters have been validated/marshalled
	/// Adding this attribute will, by default, make the method non-optimizable by the SDK tools
	/// </summary>
	[BindingAttribute (typeof(SnippetData), AttributeTargets.Method | AttributeTargets.Property)]
	public const string PreSnippetAttribute = "PreSnippetAttribute";

	/// <summary>
	/// When this attribute is applied to the interface definition it will flag the default constructor as private.
	/// </summary>
	[BindingFlag (AttributeTargets.Interface)]
	public const string PrivateDefaultCtorAttribute = "PrivateDefaultCtorAttribute";

	[BindingFlag (AttributeTargets.Property)]
	public const string ProbePresenceAttribute = "ProbePresenceAttribute";
	
	/// <summary>
	/// PrologueSnippet code is inserted before any code is generated
	/// Adding this attribute will, by default, make the method non-optimizable by the SDK tools
	/// </summary>
	[BindingAttribute (typeof(SnippetData), AttributeTargets.Method | AttributeTargets.Property)]
	public const string PrologueSnippetAttribute = "PrologueSnippetAttribute";

	/// <summary>
	/// Use this attribute to instruct the binding generator that the binding for this particular method should be
	/// flagged with an protected keyword.
	/// </summary>
	[BindingFlag (AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event)]
	public const string ProtectedAttribute = "ProtectedAttribute";

	[BindingFlag (AttributeTargets.Interface | AttributeTargets.Class)]
	public const string ProtocolAttribute = "Foundation.ProtocolAttribute";

	/// <summary>
	/// This attribute is applied to return values to flag them as being proxy objects
	/// </summary>
	[BindingFlag (AttributeTargets.ReturnValue)]
	public const string ProxyAttribute = "ProxyAttribute";

	/// <summary>
	/// Use the [AutoReleaseAttribute] on methods and properties to wrap the method invocation to the method
	/// in an NSAutoReleasePool
	/// </summary>
	[BindingFlag (AttributeTargets.ReturnValue)]
	public const string ReleaseAttribute = "ReleaseAttribute";

	/// <summary>
	/// Instructs the generator to flag the generated method as sealed
	/// </summary>
	[BindingFlag (AttributeTargets.Method | AttributeTargets.Property)]
	public const string SealedAttribute = "SealedAttribute";

	/// <summary>
	/// When this attribute is applied to a class it will just generate a static class, one that does not derive
	/// from NSObject.
	/// </summary>
	[BindingFlag (AttributeTargets.Class)]
	public const string StaticAttribute = "StaticAttribute";
	
	/// <summary>
	/// When this attribute is applied to an interface, it directs the generator to
	/// create a strongly typed DictionaryContainer for the specified fields. 
	/// </summary>
	[BindingAttribute(typeof(StrongDictionaryData), AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Property)]
	public const string StrongDictionaryAttribute = "StrongDictionaryAttribute";

	/// <summary>
	/// Used to mark if a type is not a wrapper type.
	/// </summary>
	[BindingFlag (AttributeTargets.Class | AttributeTargets.Interface)]
	public const string SyntheticAttribute = "SyntheticAttribute";
	public const string TVAttribute = "TVAttribute";

	/// <summary>
	/// hen applied, instructs the generator to use this object as the
	/// target, instead of the implicit Handle 
	/// </summary>
	[BindingFlag (AttributeTargets.Class | AttributeTargets.Interface)]
	public const string TargetAttribute = "TargetAttribute";
	
	/// <summary>
	/// Flags the object as being thread safe.
	/// </summary>
	[BindingAttribute(typeof(BackingFieldTypeData), AttributeTargets.All)]
	public const string ThreadSafeAttribute = "ThreadSafeAttribute";

	/// <summary>
	/// If this attribute is applied to a property, we do not generate a
	/// backing field.
	/// </summary>
	[BindingFlag (AttributeTargets.Property)]
	public const string TransientAttribute = "TransientAttribute";
	public const string UnavailableAttribute = "UnavailableAttribute";

	[BindingFlag (AttributeTargets.Assembly | AttributeTargets.Method | AttributeTargets.Interface)]
	public const string ZeroCopyStringsAttribute = "ZeroCopyStringsAttribute";
	public const string iOSAttribute = "ObjCRuntime.iOSAttribute";
}
