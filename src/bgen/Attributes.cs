using System;
using System.Reflection;
using System.Text;

// Disable until we get around to enable + fix any issues.
#nullable disable

//
// All the attributes in this file are compiled into two binaries:
// * Xamarin.*.Attributes.dll: this assembly references the platform assemblies (mscorlib, etc), and is used when compiling the API definition.
// * bgen-ikvm.exe: when compiled into the generator itself, the attributes are used as mock objects to load the corresponding attributes from Xamarin.*.Attributes.dll.
//

//
// ForcedTypeAttribute
//
// The ForcedTypeAttribute is used to enforce the creation of a managed type even
// if the returned unmanaged object does not match the type described in the binding definition.
//
// This is useful when the type described in a header does not match the returned type
// of the native method for example take the following Objective-C definition from NSURLSession:
//
//	- (NSURLSessionDownloadTask *)downloadTaskWithRequest:(NSURLRequest *)request
//
// It clearly states that it will return an NSURLSessionDownloadTask instance, but yet
// it returns a NSURLSessionTask, which is a superclass and thus not convertible to 
// NSURLSessionDownloadTask. Since we are in a type-safe context an InvalidCastException will happen.
//
// In order to comply with the header description and avoid the InvalidCastException, 
// the ForcedTypeAttribute is used.
//
//	[BaseType (typeof (NSObject), Name="NSURLSession")]
//	interface NSUrlSession {
//		[Export ("downloadTaskWithRequest:")]
//		[return: ForcedType]
//		NSUrlSessionDownloadTask CreateDownloadTask (NSUrlRequest request);
//	}
//
// The `ForcedTypeAttribute` also accepts a boolean value named `Owns` that is `false`
// by default `[ForcedType (owns: true)]`. The owns parameter could be used to follow
// the Ownership Policy[1] for Core Foundation objects.
//
// [1]: https://developer.apple.com/library/content/documentation/CoreFoundation/Conceptual/CFMemoryMgmt/Concepts/Ownership.html
//

[AttributeUsage (AttributeTargets.ReturnValue | AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false)]
public class ForcedTypeAttribute : Attribute {
	public ForcedTypeAttribute (bool owns = false)
	{
		Owns = owns;
	}
	public bool Owns;
}

#if !XAMCORE_5_0
// Used to flag a type as needing to be turned into a protocol on output for Unified
// For example:
//   [Protocolize, Wrap ("WeakDelegate")]
//   MyDelegate Delegate { get; set; }
//
// becomes:
//   IMyDelegate Delegate { get; set; }
//
// on the Unified API.
//
// Valid on return values and parameters
//
// To protocolize newer versions, use [Protocolize (3)] for XAMCORE_3_0, [Protocolize (4)] for NET, etc
//
[Obsolete ("This attribute no longer has any effect; do not use")]
public class ProtocolizeAttribute : Attribute {
	public ProtocolizeAttribute ()
	{
		Version = 2;
	}

	public ProtocolizeAttribute (int version)
	{
		Version = version;
	}

	public int Version { get; set; }
}
#endif

// Used to mark if a type is not a wrapper type.
public class SyntheticAttribute : Attribute {
	public SyntheticAttribute () { }
}

public class NeedsAuditAttribute : Attribute {
	public NeedsAuditAttribute (string reason)
	{
		Reason = reason;
	}

	public string Reason { get; set; }
}

public class MarshalNativeExceptionsAttribute : Attribute {
}

public class RetainAttribute : Attribute {
	public RetainAttribute ()
	{
	}

	public RetainAttribute (string wrap)
	{
		WrapName = wrap;
	}
	public string WrapName { get; set; }
}

public class ReleaseAttribute : Attribute {
}

[AttributeUsage (AttributeTargets.All, AllowMultiple = true)]
public class PostGetAttribute : Attribute {
	public PostGetAttribute (string name)
	{
		MethodName = name;
	}

	public string MethodName { get; set; }
}

public class BaseTypeAttribute : Attribute {
	public BaseTypeAttribute (Type t)
	{
		BaseType = t;
	}
	public Type BaseType { get; set; }
	public string Name { get; set; }
	public Type [] Events { get; set; }
	public string [] Delegates { get; set; }
	public bool Singleton { get; set; }

	// If set, the code will keep a reference in the EnsureXXX method for
	// delegates and will clear the reference to the object in the method
	// referenced by KeepUntilRef.   Currently uses an ArrayList, so this
	// is not really designed as a workaround for systems that create
	// too many objects, but two cases in particular that users keep
	// trampling on: UIAlertView and UIActionSheet
	public string KeepRefUntil { get; set; }
}

//
// Used for methods that invoke other targets, not this.Handle
//
public class BindAttribute : Attribute {
	public BindAttribute (string sel)
	{
		Selector = sel;
	}
	public string Selector { get; set; }

	// By default [Bind] makes non-virtual methods
	public bool Virtual { get; set; }
}

public class WrapAttribute : Attribute {
	public WrapAttribute (string methodname, bool isVirtual = false)
	{
		MethodName = methodname;
		IsVirtual = isVirtual;
	}

	public string MethodName { get; set; }
	public bool IsVirtual { get; set; }
}

//
// This attribute is a convenience shorthand for settings the
// [EditorBrowsable (EditorBrowsableState.Advanced)] flags
//
public class AdvancedAttribute : Attribute {
	public AdvancedAttribute () { }
}

// When applied instructs the generator to call Release on the returned objects
// this happens when factory methods in Objective-C return objects with refcount=1
public class FactoryAttribute : Attribute {
	public FactoryAttribute () { }
}

// When applied, it instructs the generator to not use NSStrings for marshalling.
public class PlainStringAttribute : Attribute {
	public PlainStringAttribute () { }
}

public class AutoreleaseAttribute : Attribute {
	public AutoreleaseAttribute () { }
}

// When applied, the generator generates a check for the Handle being valid on the main object, to
// ensure that the user did not Dispose() the object.
//
// This is typically used in scenarios where the user might be tempted to dispose
// the object in a callback:
//
//     foo.FinishedDownloading += delegate { foo.Dispose (); }
//
// This would invalidate "foo" and force the code to return to a destroyed/freed
// object
public class CheckDisposedAttribute : Attribute {
	public CheckDisposedAttribute () { }
}

//
// When applied, instructs the generator to use this object as the
// target, instead of the implicit Handle Can only be used in methods
// that are [Bind] instead of [Export].
// Not supported for Unified API; use [Category] support instead for
// Objective-C categories (which will create extension methods).
//
public class TargetAttribute : Attribute {
	public TargetAttribute () { }
}

public class ProxyAttribute : Attribute {
	public ProxyAttribute () { }
}

// When applied to a member, generates the member as static
public class StaticAttribute : Attribute {
	public StaticAttribute () { }
}

// When applied to a type generate a partial class even if the type does not subclass NSObject
// useful for Core* types that declare Fields
public class PartialAttribute : Attribute {
	public PartialAttribute () { }
}

// flags the backing field for the property to with .NET's [ThreadStatic] property
public class IsThreadStaticAttribute : Attribute {
	public IsThreadStaticAttribute () { }
}

// When applied to a member, generates the member as static
// and passes IntPtr.Zero or null if the parameter is null
public class NullAllowedAttribute : Attribute {
	public NullAllowedAttribute () { }
}

// When applied to a method or property, flags the resulting generated code as internal
public class InternalAttribute : Attribute {
	public InternalAttribute () { }
}

#if !XAMCORE_5_0
// This is a conditional "Internal" method, that flags methods as internal only when
// compiling with Unified, otherwise, this is ignored.
//
// In addition, UnifiedInternal members automatically get an underscore after their name
// so [UnifiedInternal] void Foo(); becomes "Foo_()"
[Obsolete ("This attribute no longer has any effect; do no use")]
public class UnifiedInternalAttribute : Attribute {
	public UnifiedInternalAttribute () { }
}
#endif

// When applied to a method or property, flags the resulting generated code as internal
public sealed class ProtectedAttribute : Attribute {
}

// When this attribute is applied to the interface definition it will
// flag the default constructor as private.  This means that you can
// still instantiate object of this class internally from your
// extension file, but it just wont be accessible to users of your
// class.
public class PrivateDefaultCtorAttribute : DefaultCtorVisibilityAttribute {
	public PrivateDefaultCtorAttribute () : base (Visibility.Private) { }
}

public enum Visibility {
	Public,
	Protected,
	Internal,
	ProtectedInternal,
	Private,
	Disabled
}

// When this attribute is applied to the interface definition it will
// flag the default ctor with the corresponding visibility (or disabled
// altogether if Visibility.Disabled is used).
public class DefaultCtorVisibilityAttribute : Attribute {
	public DefaultCtorVisibilityAttribute (Visibility visibility)
	{
		this.Visibility = visibility;
	}

	public Visibility Visibility { get; set; }
}

// When this attribute is applied to the interface definition it will
// prevent the generator from producing the default constructor.
public class DisableDefaultCtorAttribute : DefaultCtorVisibilityAttribute {
	public DisableDefaultCtorAttribute () : base (Visibility.Disabled) { }
}

//
// If this attribute is applied to a property, we do not generate a
// backing field.   See bugzilla #3359 and Assistly 7032 for some
// background information
//
public class TransientAttribute : Attribute {
	public TransientAttribute () { }
}

// Used for mandatory methods that must be implemented in a [Model].
[AttributeUsage (AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Interface, AllowMultiple = true)]
public class AbstractAttribute : Attribute {
	public AbstractAttribute () { }
}

// Used for mandatory methods that must be implemented in a [Model].
public class OverrideAttribute : Attribute {
	public OverrideAttribute () { }
}

// Makes the result use the `new' attribtue
public class NewAttribute : Attribute {
	public NewAttribute () { }
}

// Makes the result sealed
public class SealedAttribute : Attribute {
	public SealedAttribute () { }
}

// Flags the object as being thread safe
public class ThreadSafeAttribute : Attribute {
	public ThreadSafeAttribute (bool safe = true)
	{
		Safe = safe;
	}

	public bool Safe { get; private set; }
}

// Marks a struct parameter/return value as requiring a certain alignment.
public class AlignAttribute : Attribute {
	public int Align { get; set; }
	public AlignAttribute (int align)
	{
		Align = align;
	}
	public int Bits {
		get {
			int bits = 0;
			int tmp = Align;
			while (tmp > 1) {
				bits++;
				tmp /= 2;
			}
			return bits;
		}
	}
}

//
// Indicates that this array should be turned into a params
//
[AttributeUsage (AttributeTargets.Parameter, AllowMultiple = false)]
public class ParamsAttribute : Attribute {
}

//
// When applied, flags the [Flags] as a notification and generates the
// code to strongly type the notification.
//
// This attribute can be applied multiple types, once of each kind of event
// arguments that you would want to consume.
//
// The type has information about the strong type notification, while the
// NotificationCenter if not null, indicates how to get the notification center.
//
// If you do not specify it, it will use NSNotificationCenter.DefaultCenter,
// you would typically use this to specify the code needed to get to it.
//
[AttributeUsage (AttributeTargets.Property, AllowMultiple = true)]
public class NotificationAttribute : Attribute {
	public NotificationAttribute (Type t) { Type = t; }
	public NotificationAttribute (Type t, string notificationCenter) { Type = t; NotificationCenter = notificationCenter; }
	public NotificationAttribute (string notificationCenter) { NotificationCenter = notificationCenter; }
	public NotificationAttribute () { }

	public Type Type { get; set; }
	public string NotificationCenter { get; set; }
}

//
// Applied to attributes in the notification EventArgs
// to generate code that merely probes for the existence of
// the key, instead of extracting a value out of the
// userInfo dictionary
//
[AttributeUsage (AttributeTargets.Property, AllowMultiple = true)]
public class ProbePresenceAttribute : Attribute {
	public ProbePresenceAttribute () { }
}

public class EventArgsAttribute : Attribute {
	public EventArgsAttribute (string s)
	{
		ArgName = s;
	}
	public EventArgsAttribute (string s, bool skip)
	{
		ArgName = s;
		SkipGeneration = skip;
	}
	public EventArgsAttribute (string s, bool skip, bool fullname)
	{
		ArgName = s;
		SkipGeneration = skip;
		FullName = fullname;
	}

	public string ArgName { get; set; }
	public bool SkipGeneration { get; set; }
	public bool FullName { get; set; }
}

//
// Used to specify the delegate type that will be created when
// the generator creates the delegate properties on the host
// class that holds events
//
// example:
// interface SomeDelegate {
//     [Export ("foo"), DelegateName ("GetBoolean"), DefaultValue (false)]
//     bool Confirm (Some source);
//
public class DelegateNameAttribute : Attribute {
	public DelegateNameAttribute (string s)
	{
		Name = s;
	}

	public string Name { get; set; }
}

//
// Used to specify the delegate property name that will be created when
// the generator creates the delegate property on the host
// class that holds events.
//
// This is really useful when you have two overload methods that makes
// sense to keep them named as is but you want to expose them in the host class
// with a better given name.
//
// example:
// interface SomeDelegate {
//     [Export ("foo"), DelegateApiName ("Confirmation"), DelegateName ("Func<bool>"), DefaultValue (false)]
//     bool Confirm (Some source);
// }
//
// Generates property in the host class:
//	Func<bool> Confirmation { get; set; }
//
//
public class DelegateApiNameAttribute : Attribute {
	public DelegateApiNameAttribute (string apiName)
	{
		Name = apiName;
	}

	public string Name { get; set; }
}

public class EventNameAttribute : Attribute {
	public EventNameAttribute (string s)
	{
		EvtName = s;
	}
	public string EvtName { get; set; }
}

public class DefaultValueAttribute : Attribute {
	public DefaultValueAttribute (object o)
	{
		Default = o;
	}
	public object Default { get; set; }
}

public class DefaultValueFromArgumentAttribute : Attribute {
	public DefaultValueFromArgumentAttribute (string s)
	{
		Argument = s;
	}
	public string Argument { get; set; }
}

public class NoDefaultValueAttribute : Attribute {
}

// Attribute used to mark those methods that will be ignored when
// generating C# events, there are several situations in which using
// this attribute makes sense:
// 1. when there are overloaded methods. This means that we can mark
//    the default overload to be used in the events.
// 2. whe some of the methods should not be exposed as events.
public class IgnoredInDelegateAttribute : Attribute {
}

// Apply to strings parameters that are merely retained or assigned,
// not copied this is an exception as it is advised in the coding
// standard for Objective-C to avoid this, but a few properties do use
// this.  Use this attribtue for properties flagged with `retain' or
// `assign', which look like this:
//
// @property (retain) NSString foo;
// @property (assign) NSString assigned;
//
// This forced the generator to create an NSString before calling the
// API instead of using the fast string marshalling code.
public class DisableZeroCopyAttribute : Attribute {
	public DisableZeroCopyAttribute () { }
}

// Apply this attribute to methods that need a custom binding method.
//
// This is usually required for methods that take SIMD types
// (vector_floatX, vector_intX, etc).
//
// Workflow:
// * Add the attribute to the method or property accessor in question:
//   [MarshalDirective (NativePrefix = "xamarin_simd__", Library = "__Internal")]
// * Rebuild the class libraries, and build the dontlink tests for device.
// * You'll most likely get a list of unresolved externals, each mentioning
//   a different objc_msgSend* signature (if not, you're done).
// * Add the signature to runtime/bindings-generator.cs:GetFunctionData,
//   and rebuild runtime/.
//   * It is not necessary to add overloads for the super and stret 
//     variations of objc_msgSend, those are created automatically.
// * Rebuild dontlink for device again, making sure the new signature is
//   detected.
// * Make sure to build all variants of dontlink (classic, 32bit, 64bit),
//   since the set of signatures may differ.
//
// This is only for internal use (for now at least).
//
[AttributeUsage (AttributeTargets.Method)]
public class MarshalDirectiveAttribute : Attribute {
	public string NativePrefix { get; set; }
	public string NativeSuffix { get; set; }
	public string Library { get; set; }
}

//
// By default, the generator will not do Zero Copying of strings, as most
// third party libraries do not follow Apple's design guidelines of making
// string properties and parameters copy parameters, instead many libraries
// "retain" as a broken optimization [1].
//
// The consumer of the generator can force this by passing
// --use-zero-copy or setting the [assembly:ZeroCopyStrings] attribute.
// When these are set, the generator assumes the library perform
// copies over any NSStrings it keeps instead of retains/assigns and
// that any property that happens to be a retain/assign has the
// [DisableZeroCopyAttribute] attribute applied.
//
// [1] It is broken because consumer code can pass an NSMutableString, the
// library retains the value, but does not have a way of noticing changes
// that might happen to the mutable string behind its back.
//
// In the ZeroCopy case it is a problem because we pass handles to stack-allocated
// strings that stop existing after the invocation is over.
//
[AttributeUsage (AttributeTargets.Assembly | AttributeTargets.Method | AttributeTargets.Interface, AllowMultiple = true)]
public class ZeroCopyStringsAttribute : Attribute {
}

[AttributeUsage (AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true)]
public class SnippetAttribute : Attribute {
	public SnippetAttribute (string s)
	{
		Code = s;
	}
	public string Code { get; set; }

	public bool Optimizable { get; set; }
}

//
// PreSnippet code is inserted after the parameters have been validated/marshalled
// Adding this attribute will, by default, make the method non-optimizable by the SDK tools
// 
public class PreSnippetAttribute : SnippetAttribute {
	public PreSnippetAttribute (string s) : base (s) { }
}

//
// PrologueSnippet code is inserted before any code is generated
// Adding this attribute will, by default, make the method non-optimizable by the SDK tools
// 
public class PrologueSnippetAttribute : SnippetAttribute {
	public PrologueSnippetAttribute (string s) : base (s) { }
}

//
// PostSnippet code is inserted before returning, before paramters are disposed/released
// Adding this attribute will, by default, make the method non-optimizable by the SDK tools
// 
public class PostSnippetAttribute : SnippetAttribute {
	public PostSnippetAttribute (string s) : base (s) { }
}

//
// Code to run from a generated Dispose method, before any generated code is executed
// Adding this attribute will, by default, make the method non-optimizable by the SDK tools
//
[AttributeUsage (AttributeTargets.Interface, AllowMultiple = true)]
public class DisposeAttribute : SnippetAttribute {
	public DisposeAttribute (string s) : base (s) { }
}

//
// This attribute is used to flag properties that should be exposed on the strongly typed
// nested Appearance class.   It is usually a superset of what Apple has labeled with
// UI_APPEARANCE_SELECTOR because they do support more selectors than those flagged in
// the UIAppearance proxies, so we must label all the options.   This will be a list that
// is organically grown as we find them
//
[AttributeUsage (AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false)]
public class AppearanceAttribute : Attribute {
	public AppearanceAttribute () { }
}

//
// Apply this attribute to a class to add methods that in Objective-C
// are added as categories
//
// Use the BaseType attribute to reference which class this is extending
//
// Like this:
//   [Category]
//   [BaseType (typeof (UIView))]
//   interface UIViewExtensions {
//     [Export ("method_in_the_objective_c_category")]
//     void ThisWillBecome_a_c_sharp_extension_method_in_class_UIViewExtensions ();
// }
[AttributeUsage (AttributeTargets.Interface, AllowMultiple = false)]
public class CategoryAttribute : Attribute {
#if !NET
	public bool AllowStaticMembers;
#endif
	public CategoryAttribute () { }
#if !NET
	[Obsolete ("Inline the static members in this category in the category's class (and remove this obsolete once fixed)")]
	public CategoryAttribute (bool allowStaticMembers)
	{
		AllowStaticMembers = allowStaticMembers;
	}
#endif
}

//
// Apply this attribute when an `init*` selector is decorated with NS_DESIGNATED_INITIALIZER
//
[AttributeUsage (AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = false)]
public class DesignatedInitializerAttribute : Attribute {
	public DesignatedInitializerAttribute ()
	{
	}
}

//
// Apple this attribute to ObjC types where the default `init` selector 
// is decorated with `NS_DESIGNATED_INITIALIZER`
//
// The generator will produce a `[DesignatedInitializer]` when generating the
// default constructor when `[DesignatedDefaultCtor]` is present on the type
//
[AttributeUsage (AttributeTargets.Interface, AllowMultiple = false)]
public class DesignatedDefaultCtorAttribute : Attribute {
	public DesignatedDefaultCtorAttribute ()
	{
	}
}

//
// Apply this attribute to a method that you want an async version of a callback method.
//
// Use the ResultType or ResultTypeName attribute to describe any composite value to be by the Task object.
// Use MethodName to customize the name of the generated method
//
// Note that this only supports the case where the callback is the last parameter of the method.
//
// Like this:
//[Export ("saveAccount:withCompletionHandler:")] [Async]
//void SaveAccount (ACAccount account, ACAccountStoreSaveCompletionHandler completionHandler);
// }
[AttributeUsage (AttributeTargets.Method, AllowMultiple = false)]
public class AsyncAttribute : Attribute {

	//This will automagically generate the async method.
	//This works with 4 kinds of callbacks: (), (NSError), (result), (result, NSError)
	public AsyncAttribute () { }

	//This works with 2 kinds of callbacks: (...) and (..., NSError).
	//Parameters are passed in order to a constructor in resultType
	public AsyncAttribute (Type resultType)
	{
		ResultType = resultType;
	}

	//This works with 2 kinds of callbacks: (...) and (..., NSError).
	//Parameters are passed in order to a result type that is automatically created if size > 1
	//The generated method is named after the @methodName
	public AsyncAttribute (string methodName)
	{
		MethodName = methodName;
	}

	public Type ResultType { get; set; }
	public string MethodName { get; set; }
	public string ResultTypeName { get; set; }
	public string PostNonResultSnippet { get; set; }
}

//
// When this attribute is applied to an interface, it directs the generator to
// create a strongly typed DictionaryContainer for the specified fields.
//
// The constructor argument is the name of the type that contains the keys to lookup
//
// If an export attribute is present, if the value contains a dot,
// then the the value of the export is used to lookup the keyname.  If
// there is no dot present, then this prefixes the value with the
// typeWithKeys value.  If it is not, then the value is inferred as
// being the result of typeWithKeys.\(propertyName\)Key
//
// For example:
//
//  [StrongDictionary ("foo")] interface X { [Export ("bar")] string Bar;
//  This looks up in foo.bar
//
//  [StrongDictionary ("foo")] interface X { [Export ("bar.baz")] string Bar;
//  This looks up in bar.baz
//
//  [StrongDictionary ("foo")] interface X { string Bar; }
//  This looks up in foo.BarKey
//
// The parameterless ctor can be applied to individual property members of
// a DictionaryContainer to instruct the generator that the property is another
// DictionaryContainer and generate the necessary code to handle it.
//
// For Example
//  [StrongDictionary ("FooOptionsKeys")]
//  interface FooOptions {
//
//      [StrongDictionary]
//	    BarOptions BarDictionary { get; set; }
//  }
//
[AttributeUsage (AttributeTargets.Interface | AttributeTargets.Property, AllowMultiple = false)]
public class StrongDictionaryAttribute : Attribute {
	public StrongDictionaryAttribute ()
	{
	}
	public StrongDictionaryAttribute (string typeWithKeys)
	{
		TypeWithKeys = typeWithKeys;
		Suffix = "Key";
	}
	public string TypeWithKeys;
	public string Suffix;
}

//
// When this attribtue is applied to a property, currently it merely adds
// a DebuggerBrowsable(Never) to the property, to prevent a family of crashes
//
[AttributeUsage (AttributeTargets.Property, AllowMultiple = false)]
public class OptionalImplementationAttribute : Attribute {
	public OptionalImplementationAttribute () { }
}

//
// Use this attribute if some definitions are required at definition-compile
// time but when you need the final binding assembly to include your own
// custom implementation
//
[AttributeUsage (AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false)]
public class ManualAttribute : Attribute {
	public ManualAttribute () { }
}

[AttributeUsage (AttributeTargets.Interface)]
public class CoreImageFilterAttribute : Attribute {

	public CoreImageFilterAttribute ()
	{
		// default is public - will be skipped for abstract types
		DefaultCtorVisibility = MethodAttributes.Public;

		IntPtrCtorVisibility = MethodAttributes.PrivateScope;

		// not needed by default, automatically `protected` if the type is abstract
		StringCtorVisibility = MethodAttributes.PrivateScope;
	}

	public MethodAttributes DefaultCtorVisibility { get; set; }

	public MethodAttributes IntPtrCtorVisibility { get; set; }

	public MethodAttributes StringCtorVisibility { get; set; }
}

[AttributeUsage (AttributeTargets.Property)]
public class CoreImageFilterPropertyAttribute : Attribute {

	public CoreImageFilterPropertyAttribute (string name)
	{
		Name = name;
	}

	public string Name { get; private set; }
}

// If the enum is used to represent error code then this attribute can be used to
// generate an extension type that will return the associated error domain based
// on the field name (given as a parameter)
[AttributeUsage (AttributeTargets.Enum)]
public class ErrorDomainAttribute : Attribute {

	public ErrorDomainAttribute (string domain)
	{
		ErrorDomain = domain;
	}

	public string ErrorDomain { get; set; }
}

[AttributeUsage (AttributeTargets.Field)]
public class DefaultEnumValueAttribute : Attribute {

	public DefaultEnumValueAttribute ()
	{
	}
}

//
// This prevents the generator from generating the managed proxy to
// the method being called, this is done when we are interested in
// the side effects of the binding of the method, rather than the
// method itself -- for security for example, we generate the
// proxy code for invoking blocks and callbacks to blocks
// see security.cs for an example
[AttributeUsage (AttributeTargets.Method)]
public class NoMethodAttribute : Attribute {
}

#if NET
public enum PlatformName : byte {
	None,
	MacOSX,
	iOS,
	WatchOS,
	TvOS,
	MacCatalyst,
}

public enum AvailabilityKind {
	Introduced,
	Deprecated,
	Obsoleted,
	Unavailable,
}

[AttributeUsage (
	AttributeTargets.Assembly |
	AttributeTargets.Class |
	AttributeTargets.Constructor |
	AttributeTargets.Delegate |
	AttributeTargets.Enum |
	AttributeTargets.Event |
	AttributeTargets.Field |
	AttributeTargets.Interface |
	AttributeTargets.Method |
	AttributeTargets.Property |
	AttributeTargets.Struct,
	AllowMultiple = true
)]
public abstract class AvailabilityBaseAttribute : Attribute {
	public AvailabilityKind AvailabilityKind { get; private set; }
	public PlatformName Platform { get; private set; }
	public Version Version { get; private set; }
	public string Message { get; private set; }

	internal AvailabilityBaseAttribute ()
	{
	}

	internal AvailabilityBaseAttribute (
		AvailabilityKind availabilityKind,
		PlatformName platform,
		Version version,
		string message)
	{
		AvailabilityKind = availabilityKind;
		Platform = platform;
		Version = version;
		Message = message;
	}

	void GeneratePlatformDefine (StringBuilder builder)
	{
		switch (Platform) {
		case PlatformName.iOS:
			builder.AppendLine ("#if __IOS__");
			break;
		case PlatformName.TvOS:
			builder.AppendLine ("#if __TVOS__");
			break;
		case PlatformName.WatchOS:
			builder.AppendLine ("#if __WATCHOS__");
			break;
		case PlatformName.MacOSX:
			builder.AppendLine ("#if __MACOS__");
			break;
		case PlatformName.MacCatalyst:
			builder.AppendLine ("#if __MACCATALYST__ && !__IOS__");
			break;
		default:
			throw new NotSupportedException ($"Unknown platform: {Platform}");
		}
	}

	void GenerateUnsupported (StringBuilder builder)
	{
		builder.Append ("[UnsupportedOSPlatform (\"");
		GeneratePlatformNameAndVersion (builder);
		builder.Append ("\"");
		if (!String.IsNullOrEmpty (Message))
			builder.Append (", \"").Append (Message).Append ('"');

		builder.AppendLine (")]");
	}

	void GenerateDeprecated (StringBuilder builder)
	{
		builder.Append ("[ObsoletedOSPlatform (\"");
		GeneratePlatformNameAndVersion (builder);
		builder.Append ("\"");
		if (!String.IsNullOrEmpty (Message))
			builder.Append (", \"").Append (Message).Append ('"');
		builder.AppendLine (")]");
	}

	void GenerateSupported (StringBuilder builder)
	{
#if BGENERATOR
		// If the version is less than or equal to the min version for the platform in question,
		// the version is redundant, so just skip it.
		if (Version is not null && Version <= Xamarin.SdkVersions.GetMinVersion (Platform.AsApplePlatform ()))
			Version = null;
#endif

		builder.Append ("[SupportedOSPlatform (\"");
		GeneratePlatformNameAndVersion (builder);
		builder.AppendLine ("\")]");
	}

	void GeneratePlatformNameAndVersion (StringBuilder builder)
	{
		switch (Platform) {
		case PlatformName.iOS:
			builder.Append ("ios");
			break;
		case PlatformName.TvOS:
			builder.Append ("tvos");
			break;
		case PlatformName.WatchOS:
			builder.Append ("watchos");
			break;
		case PlatformName.MacOSX:
			builder.Append ("macos"); // no 'x'
			break;
		case PlatformName.MacCatalyst:
			builder.Append ("maccatalyst");
			break;
		default:
			throw new NotSupportedException ($"Unknown platform: {Platform}");
		}

		if (Version is not null)
			builder.Append (Version.ToString (Version.Build >= 0 ? 3 : 2));
	}

	public override string ToString ()
	{
		var builder = new StringBuilder ();
		switch (AvailabilityKind) {
		case AvailabilityKind.Introduced:
			GenerateSupported (builder);
			break;
		case AvailabilityKind.Deprecated:
			GenerateDeprecated (builder);
			break;
		case AvailabilityKind.Obsoleted:
			GenerateUnsupported (builder);
			break;
		case AvailabilityKind.Unavailable:
			GenerateUnsupported (builder);
			break;
		}
		return builder.ToString ();
	}
}

public class IntroducedAttribute : AvailabilityBaseAttribute {
	public IntroducedAttribute (PlatformName platform, string message = null)
		: base (AvailabilityKind.Introduced, platform, null, message)
	{
	}

	public IntroducedAttribute (PlatformName platform, int majorVersion, int minorVersion, string message = null)
		: base (AvailabilityKind.Introduced, platform, new Version (majorVersion, minorVersion), message)
	{
	}

	public IntroducedAttribute (PlatformName platform, int majorVersion, int minorVersion, int subminorVersion, string message = null)
		: base (AvailabilityKind.Introduced, platform, new Version (majorVersion, minorVersion, subminorVersion), message)
	{
	}
}

public sealed class DeprecatedAttribute : AvailabilityBaseAttribute {
	public DeprecatedAttribute (PlatformName platform, string message = null)
		: base (AvailabilityKind.Deprecated, platform, null, message)
	{
	}

	public DeprecatedAttribute (PlatformName platform, int majorVersion, int minorVersion, string message = null)
		: base (AvailabilityKind.Deprecated, platform, new Version (majorVersion, minorVersion), message)
	{
	}

	public DeprecatedAttribute (PlatformName platform, int majorVersion, int minorVersion, int subminorVersion, string message = null)
		: base (AvailabilityKind.Deprecated, platform, new Version (majorVersion, minorVersion, subminorVersion), message)
	{
	}
}

public sealed class ObsoletedAttribute : AvailabilityBaseAttribute {
	public ObsoletedAttribute (PlatformName platform, string message = null)
		: base (AvailabilityKind.Obsoleted, platform, null, message)
	{
	}

	public ObsoletedAttribute (PlatformName platform, int majorVersion, int minorVersion, string message = null)
		: base (AvailabilityKind.Obsoleted, platform, new Version (majorVersion, minorVersion), message)
	{
	}

	public ObsoletedAttribute (PlatformName platform, int majorVersion, int minorVersion, int subminorVersion, string message = null)
		: base (AvailabilityKind.Obsoleted, platform, new Version (majorVersion, minorVersion, subminorVersion), message)
	{
	}
}

public class UnavailableAttribute : AvailabilityBaseAttribute {
	public UnavailableAttribute (PlatformName platform, string message = null)
		: base (AvailabilityKind.Unavailable, platform, null, message)
	{
	}
}

public sealed class TVAttribute : IntroducedAttribute {
	public TVAttribute (byte major, byte minor)
		: base (PlatformName.TvOS, (int) major, (int) minor)
	{
	}

	public TVAttribute (byte major, byte minor, byte subminor)
		: base (PlatformName.TvOS, (int) major, (int) minor, subminor)
	{
	}
}

public sealed class WatchAttribute : IntroducedAttribute {
	public WatchAttribute (byte major, byte minor)
		: base (PlatformName.WatchOS, (int) major, (int) minor)
	{
	}

	public WatchAttribute (byte major, byte minor, byte subminor)
		: base (PlatformName.WatchOS, (int) major, (int) minor, subminor)
	{
	}
}

public sealed class MacCatalystAttribute : IntroducedAttribute {
	public MacCatalystAttribute (byte major, byte minor)
		: base (PlatformName.MacCatalyst, (int) major, (int) minor)
	{
	}

	public MacCatalystAttribute (byte major, byte minor, byte subminor)
		: base (PlatformName.MacCatalyst, (int) major, (int) minor, subminor)
	{
	}
}

public sealed class NoMacAttribute : UnavailableAttribute {
	public NoMacAttribute ()
		: base (PlatformName.MacOSX)
	{
	}
}

public sealed class NoiOSAttribute : UnavailableAttribute {
	public NoiOSAttribute ()
		: base (PlatformName.iOS)
	{
	}
}

public sealed class NoWatchAttribute : UnavailableAttribute {
	public NoWatchAttribute ()
		: base (PlatformName.WatchOS)
	{
	}
}

public sealed class NoTVAttribute : UnavailableAttribute {
	public NoTVAttribute ()
		: base (PlatformName.TvOS)
	{
	}
}

public sealed class NoMacCatalystAttribute : UnavailableAttribute {
	public NoMacCatalystAttribute ()
		: base (PlatformName.MacCatalyst)
	{
	}
}

enum Platform : ulong {
	None = 0x0,
	// Processed in generator-attribute-manager.cs
	//            0xT000000000MMmmss
	iOS_Version = 0x0000000000ffffff,
	iOS_2_0 = 0x0000000000020000,
	iOS_2_2 = 0x0000000000020200,
	iOS_3_0 = 0x0000000000030000,
	iOS_3_1 = 0x0000000000030100,
	iOS_3_2 = 0x0000000000030200,
	iOS_4_0 = 0x0000000000040000,
	iOS_4_1 = 0x0000000000040100,
	iOS_4_2 = 0x0000000000040200,
	iOS_4_3 = 0x0000000000040300,
	iOS_5_0 = 0x0000000000050000,
	iOS_5_1 = 0x0000000000050100,
	iOS_6_0 = 0x0000000000060000,
	iOS_6_1 = 0x0000000000060100,
	iOS_7_0 = 0x0000000000070000,
	iOS_7_1 = 0x0000000000070100,
	iOS_8_0 = 0x0000000000080000,
	iOS_8_1 = 0x0000000000080100,
	iOS_8_2 = 0x0000000000080200,
	iOS_8_3 = 0x0000000000080300,
	iOS_8_4 = 0x0000000000080400,
	iOS_9_0 = 0x0000000000090000,
	iOS_9_1 = 0x0000000000090100,
	iOS_9_2 = 0x0000000000090200,
	iOS_9_3 = 0x0000000000090300,
	iOS_10_0 = 0x00000000000a0000,
	iOS_11_0 = 0x00000000000b0000,

	//            0xT000000000MMmmss
	Mac_Version = 0x1000000000ffffff,
	Mac_10_0 = 0x1000000000000000,
	Mac_10_1 = 0x1000000000010000,
	Mac_10_2 = 0x1000000000020000,
	Mac_10_3 = 0x1000000000030000,
	Mac_10_4 = 0x1000000000040000,
	Mac_10_5 = 0x1000000000050000,
	Mac_10_6 = 0x1000000000060000,
	Mac_10_7 = 0x1000000000070000,
	Mac_10_8 = 0x1000000000080000,
	Mac_10_9 = 0x1000000000090000,
	Mac_10_10 = 0x10000000000a0000,
	Mac_10_10_3 = 0x10000000000a0300,
	Mac_10_11 = 0x10000000000b0000,
	Mac_10_11_3 = 0x10000000000b0300,
	Mac_10_12 = 0x10000000000c0000,
	Mac_10_13 = 0x10000000000d0000,
	Mac_10_14 = 0x10000000000e0000,

	//              0xT000000000MMmmss
	Watch_Version = 0x2000000000ffffff,
	Watch_1_0 = 0x2000000000010000,
	Watch_2_0 = 0x2000000000020000,
	Watch_3_0 = 0x2000000000030000,
	Watch_4_0 = 0x2000000000040000,

	//             0xT000000000MMmmss
	TV_Version = 0x3000000000ffffff,
	TV_9_0 = 0x3000000000090000,
	TV_10_0 = 0x30000000000a0000,
	TV_11_0 = 0x30000000000b0000,
}
#endif // NET
