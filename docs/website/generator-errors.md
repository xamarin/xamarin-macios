---
id: 505F978C-9553-49A8-9632-0D6D8A0A0287
title: "Xamarin.iOS/Xamarin.Mac binding errors"
dateupdated: 2017-06-26
---

[//]: # (The original file resides under https://github.com/xamarin/xamarin-macios/tree/master/docs/website/)
[//]: # (This allows all contributors (including external) to submit, using a PR, updates to the documentation that match the tools changes)
[//]: # (Modifications outside of xamarin-macios/master will be lost on future updates)

# BI0xxx: binding error messages

E.g. parameters, environment

<!-- 0xxx: the generator itself, e.g. parameters, environment -->
### <a name="BI0000"/>BI0000: Unexpected error - Please fill a bug report at https://bugzilla.xamarin.com

An unexpected error condition occurred. Please [file a bug report](https://bugzilla.xamarin.com/enter_bug.cgi?product=iOS&component=Tools) with as much information as possible, including:

* Full build logs, with maximum verbosity
* A minimal test case that reproduce the error
* All version informations

The easiest way to get exact version information is to use the **Xamarin Studio** menu, **About Xamarin Studio** item, **Show Details** button and copy/paste the version informations (you can use the **Copy Information** button).

### <a name='BI0001'/>BI0001: The .NET runtime could not load the {mi.ReturnType.Name} type. Message: {ex.Message}

### <a name='BI0026'/>BI0026: Could not parse the command line argument '{argument}': {message}

### <a name='BI0068'/>BI0068: Invalid value for target framework: *.

An invalid target framework was passed using the --target-framework argument. Please specify a valid target framework.

### <a name='BI0070'/>BI0070: Invalid target framework: *. Valid target frameworks are: *.

An invalid target framework was passed using the --target-framework argument. Please specify a valid target framework.

### <a name='BI0086'/>BI0086: A target framework (--target-framework) must be specified when building for *.

This usually indicates a bug in Xamarin.iOS/Xamarin.Mac; please [file a bug report](https://bugzilla.xamarin.com/enter_bug.cgi?product=iOS) with a test case.

# BI1xxx: code generation

<!-- 1xxx: code generation -->
<!-- 10xx: errors -->

### <a name='BI1001'/>BI1001: Do not know how to make a trampoline for *

### <a name='BI1002'/>BI1002: Unknown kind * in method '*'

### <a name='BI1003'/>BI1003: The delegate method *.* needs to take at least one parameter

### <a name='BI1004'/>BI1004: The delegate method *.* is missing the [EventArgs] attribute (has * parameters)

### <a name='BI1005'/>BI1005: EventArgs in *.* attribute should not include the text `EventArgs' at the end

### <a name='BI1006'/>BI1006: The delegate method *.* is missing the [DelegateName] attribute (or EventArgs)

### <a name='BI1007'/>BI1007: Unknown attribute * on *

### <a name='BI1008'/>BI1008: [IsThreadStatic] is only valid on properties that are also [Static]

### <a name='BI1009'/>BI1009: No selector specified for method `*.*'

### <a name='BI1010'/>BI1010: No Export attribute on *.* property

### <a name='BI1011'/>BI1011: Do not know how to extract type */* from an NSDictionary

### <a name='BI1012'/>BI1012: No Export or Bind attribute defined on *.*

### <a name='BI1013'/>BI1013: Unsupported type for Fields (string), you probably meant NSString

### <a name='BI1014'/>BI1014: Unsupported type for Fields: * for '*'.

Please go to [[FieldAttribute]](https://developer.xamarin.com/guides/cross-platform/macios/binding/binding-types-reference/#FieldAttribute) documentation to see supported types.

### <a name='BI1015'/>BI1015: In class * You specified the Events property, but did not bind those to names with Delegates

### <a name='BI1016'/>BI1016: The delegate method *.* is missing the [DefaultValue] attribute

### <a name='BI1017'/>BI1017: Do not know how to make a signature for *

### <a name='BI1018'/>BI1018: No [Export] attribute on property *.*

### <a name='BI1019'/>BI1019: Invalid [NoDefaultValue] attribute on method `*.*'

### <a name='BI1020'/>BI1020: Unsupported type * used on exported method *.*

### <a name='BI1021'/>BI1021: Unsupported type for read/write Fields: *

### <a name='BI1022'/>BI1022: Model classes can not be categories

### <a name='BI1023'/>BI1023: The number of Events (Type) and Delegates (string) must match for `*`

### <a name='BI1024'/>BI1024: No selector specified for property '*.*'

### <a name='BI1025'/>BI1025: [Static] and [Protocol] are mutually exclusive (*)

### <a name='BI1026'/>BI1026: `*`: Enums attributed with [*] must have an underlying type of `long` or `ulong`

### <a name='BI1027'/>BI1027: Support for ZeroCopy strings is not implemented. Strings will be marshalled as NSStrings.

### <a name='BI1028'/>BI1028: Internal sanity check failed, please file a bug report (http://bugzilla.xamarin.com) with a test case.

### <a name='BI1029'/>BI1029: Internal error: invalid enum mode for type '*'

### <a name='BI1030'/>BI1030: * cannot have [BaseType(typeof(*))] as it creates a circular dependency

### <a name='BI1031'/>BI1031: The [Target] attribute is not supported for the Unified API (found on the member '*.*'). For Objective-C categories, create an api definition interface with the [Category] attribute instead.

### <a name='BI1034'/>BI1034: The [Protocolize] attribute is set on the property *.*, but the property's type (*) is not a protocol.

### <a name='BI1035'/>BI1035: The property * on class * is hiding a property from a parent class * but the selectors do not match.

### <a name='BI1036'/>BI1036: The last parameter in the method '*.*' must be a delegate (it's '*').

### <a name='BI1037'/>BI1037: The selector * on type * is found multiple times with both read only and write only versions, with no read/write version.

### <a name='BI1038'/>BI1038: The selector * on type * is found multiple times with different return types.

### <a name='BI1039'/>BI1039: The selector * on type * is found multiple times with different argument length * : *.

### <a name='BI1040'/>BI1040: The selector * on type * is found multiple times with different argument out states on argument *.

### <a name='BI1041'/>BI1041: The selector * on type * is found multiple times with different argument types on argument * - * : *.

### <a name='BI1042'/>BI1042: Missing '[Field (LibraryName=value)]' for {field_pi.Name} (e.g."__Internal")

### <a name='BI1043'/>BI1043: Repeated overload {mi.Name} and no [DelegateApiNameAttribute] provided to generate property name on host class.

### <a name='BI1044'/>BI1044: Repeated name '{apiName.Name}' provided in [DelegateApiNameAttribute].

### <a name='BI1045'/>BI1045: Only a single [DefaultEnumValue] attribute can be used inside enum {type.Name}.

### <a name='BI1046'/>BI1046: The [Field] constant {fa.SymbolName} cannot only be used once inside enum {type.Name}.

### <a name='BI1047'/>BI1047: Unsupported platform: *. Please file a bug report (http://bugzilla.xamarin.com) with a test case.

### <a name='BI1048'/>BI1048: Unsupported type * decorated with [BindAs].

### <a name='BI1049'/>BI1049: Could not unbox type * from * container used on * member decorated with [BindAs].

### <a name='BI1050'/>BI1050: [BindAs] cannot be used inside Protocol or Model types. Type: *

### <a name='BI1051'/>BI1051: Internal error: Don't know how to get attributes for *. Please file a bug report (http://bugzilla.xamarin.com) with a test case.

### <a name='BI1052'/>BI1052: Internal error: Could not find the type * in the assembly *. Please file a bug report (http://bugzilla.xamarin.com) with a test case.

### <a name='BI1053'/>BI1053: Internal error: unknown target framework '*'.

### <a name='BI1054'/>BI1054: Internal error: can't convert type '*' (unknown assembly). Please file a bug report (https://bugzilla.xamarin.com) with a test case.

This usually indicates a bug in Xamarin.iOS/Xamarin.Mac; please [file a bug report](https://bugzilla.xamarin.com/enter_bug.cgi?product=iOS&component=Tools) with a test case.

### <a name='BI1055'/>BI1055: Internal error: failed to convert type '*'. Please file a bug report (https://bugzilla.xamarin.com) with a test case.

This usually indicates a bug in Xamarin.iOS/Xamarin.Mac; please [file a bug report](https://bugzilla.xamarin.com/enter_bug.cgi?product=iOS&component=Tools) with a test case.

### <a name='BI1056'/>BI1056: Internal error: failed to instantiate mock attribute '*' (could not convert type constructor argument #*). Please file a bug report (https://bugzilla.xamarin.com) with a test case.

This usually indicates a bug in Xamarin.iOS/Xamarin.Mac; please [file a bug report](https://bugzilla.xamarin.com/enter_bug.cgi?product=iOS&component=Tools) with a test case.

### <a name='BI1057'/>BI1057: Internal error: failed to instantiate mock attribute '*' (could not convert constructor type #* (*)). Please file a bug report (https://bugzilla.xamarin.com) with a test case.

This usually indicates a bug in Xamarin.iOS/Xamarin.Mac; please [file a bug report](https://bugzilla.xamarin.com/enter_bug.cgi?product=iOS&component=Tools) with a test case.

### <a name='BI1058'/>BI1058: Internal error: could not find a constructor for the mock attribute '*'. Please file a bug report (https://bugzilla.xamarin.com) with a test case.

This usually indicates a bug in Xamarin.iOS/Xamarin.Mac; please [file a bug report](https://bugzilla.xamarin.com/enter_bug.cgi?product=iOS&component=Tools) with a test case.

### <a name='BI1059'/>BI1059: Found * * attributes on the member *. At most one was expected.

### <a name='BI1060'/>BI1060: The * protocol is decorated with [Model], but not [BaseType]. Please verify that [Model] is relevant for this protocol; if so, add [BaseType] as well, otherwise remove [Model].

### <a name='BI1061'/>BI1061: The attribute '{attribute}' found on '{member}' is not a valid binding attribute. Please remove this attribute.

### <a name='BI1062'/>BI1062: The member '*' contains ref/out parameters and must not be decorated with [Async].

### <a name='BI1063'/>BI1063: The 'WrapAttribute' can only be used at the property or at getter/setter level at a given time. Property: '*'.

# BI11xx: warnings

<!-- 11xx: warnings -->

### <a name='BI1101'/>BI1101: Trying to use a string as a [Target]

### <a name='BI1102'/>BI1102: Using the deprecated EventArgs for a delegate signature in *.*, please use DelegateName instead

### <a name='BI1103'/>BI1103: '*' does not live under a namespace; namespaces are a highly recommended .NET best practice

### <a name='BI1104'/>BI1104: Could not load the referenced library '*': *.

### <a name='BI1105'/>BI1105: Potential selector/argument mismatch [Export ("*")] has * arguments and *.* has * arguments

### <a name='BI1106'/>BI1106: The parameter * in the method *.* exposes a model (*). Please expose the corresponding protocol type instead (*.I*)

### <a name='BI1107'/>BI1107: The return type of the method *.* exposes a model (*). Please expose the corresponding protocol type instead (*.I*).

### <a name='BI1108'/>BI1108: The [Protocolize] attribute is applied to the return type of the method *.*, but the return type (*) isn't a model and can thus not be protocolized. Please remove the [Protocolize] attribute.

### <a name='BI1109'/>BI1109: The return type of the method *.* exposes a model (*). Please expose the corresponding protocol type instead (*.I*).

### <a name='BI1110'/>BI1110: The property *.* exposes a model (*). Please expose the corresponding protocol type instead (*.I*).

### <a name='BI1111'/>BI1111: Interface '*' on '*' is being ignored as it is not a protocol. Did you mean '*' instead?

### <a name='BI1112'/>BI1112: Property * should be renamed to 'Delegate' for BaseType.Events and BaseType.Delegates to work.

### <a name='BI1113'/>BI1113: BaseType.Delegates were set but no properties could be found. Do ensure that the WrapAttribute is used on the right properties.

### <a name='BI1114'/>BI1114: Binding error: test unable to find property: * on *.

### <a name='BI1115'/>BI1115: The parameter '*' in the delegate '*' does not have a [CCallback] or [BlockCallback] attribute. Defaulting to [CCallback].

### <a name='BI1116'/>BI1116: The parameter '*' in the delegate '*' does not have a [CCallback] or [BlockCallback] attribute. Defaulting to [CCallback]. Declare a custom delegate instead of using System.Action / System.Func and add the attribute on the corresponding parameter.

### <a name='BI1117'/>BI1117: The member '*' is decorated with [Static] and its container class * is decorated with [Category] this leads to hard to use code. Please inline * into * class.

<!-- 2xxx: reserved -->
<!-- 3xxx: reserved -->
<!-- 4xxx: reserved -->
<!-- 5xxx: reserved -->
<!-- 6xxx: reserved -->
<!-- 7xxx: reserved -->
<!-- 8xxx: reserved -->
<!-- 9xxx: reserved -->
