//
// javascriptcore.cs: binding for iOS (7+) and Mac (10.9+) JavaScriptCore framework
//
// Authors:
//   Aaron Bockover (abock@xamarin.com)
//
// Copyright 2013-2014 Xamarin, Inc.

using System;
using ObjCRuntime;
using Foundation;
using CoreGraphics;

namespace JavaScriptCore {

	delegate void JSContextExceptionHandler (JSContext context, JSValue exception);

	[Mac (10,9, onlyOn64: true), iOS (7,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	partial interface JSContext {

		[Export ("init")]
		IntPtr Constructor ();

		[Export ("initWithVirtualMachine:")]
		IntPtr Constructor (JSVirtualMachine virtualMachine);

		[Mac (10,10), iOS (8,0)]
		[NullAllowed] // by default this property is null
		[Export ("name")]
		string Name { get; set; }

		[Mac (10,10), iOS (8,0)]
		[Export ("evaluateScript:withSourceURL:")]
		JSValue EvaluateScript (string script, NSUrl sourceUrl);

		[Export ("evaluateScript:")]
		JSValue EvaluateScript (string script);

		[Export ("globalObject")]
		JSValue GlobalObject { get; }

		[Static, Export ("currentContext")]
		JSContext CurrentContext { get; }

		[Static, Export ("currentThis")]
		JSValue CurrentThis { get; }

		[Static, Export ("currentArguments")]
		JSValue [] CurrentArguments { get; }

		[Mac (10,10), iOS (8,0)]
		[Static, Export ("currentCallee")]
		JSValue CurrentCallee { get; }

		[NullAllowed] // by default this property is null
		[Export ("exception", ArgumentSemantic.Retain)]
		JSValue Exception { get; set; }

		[Export ("exceptionHandler", ArgumentSemantic.Copy)]
		JSContextExceptionHandler ExceptionHandler { get; set; }

		[Export ("virtualMachine", ArgumentSemantic.Retain)]
		JSVirtualMachine VirtualMachine { get; }

		#region Subscript Support Category

		[Internal, Export ("objectForKeyedSubscript:")]
		JSValue _GetObject (NSObject key);

		// Since the getter returns JSValue (documented as such) we'll provide a setter that do so (not NSObject)

		[Internal, Export ("setObject:forKeyedSubscript:")]
		void _SetObject (JSValue obj, NSObject key);

		#endregion

		/* C API Bridging functions */
		[Mac (10,9), iOS (7,0)]
		[Static, Export ("contextWithJSGlobalContextRef:")]
		JSContext FromJSGlobalContextRef (IntPtr nativeJsGlobalContextRef);

		[Mac (10,9), iOS (7,0)]
		[Export ("JSGlobalContextRef")]
		IntPtr JSGlobalContextRefPtr { get; }
	}

	[Mac (10,9, onlyOn64: true), iOS (7,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor] // invalid (null) Handle is returned
	partial interface JSValue {
		[Static, Export ("valueWithObject:inContext:")]
		JSValue From (NSObject value, JSContext context);

		[Static, Export ("valueWithBool:inContext:")]
		JSValue From (bool value, JSContext context);

		[Static, Export ("valueWithDouble:inContext:")]
		JSValue From (double value, JSContext context);

		[Static, Export ("valueWithInt32:inContext:")]
		JSValue From (int /* int32_t */ ivalue, JSContext context);

		[Static, Export ("valueWithUInt32:inContext:")]
		JSValue From (uint /* uint32_t */ value, JSContext context);

		[Static, Export ("valueWithNewObjectInContext:")]
		JSValue CreateObject (JSContext context);

		[Static, Export ("valueWithNewArrayInContext:")]
		JSValue CreateArray (JSContext context);

		[Static, Export ("valueWithNewRegularExpressionFromPattern:flags:inContext:")]
		JSValue CreateRegularExpression (string pattern, string flags, JSContext context);

		[Static, Export ("valueWithNewErrorFromMessage:inContext:")]
		JSValue CreateError (string message, JSContext context);

		[Static, Export ("valueWithNullInContext:")]
		JSValue Null (JSContext context);

		[Static, Export ("valueWithUndefinedInContext:")]
		JSValue Undefined (JSContext context);

		[Export ("toObject")]
		NSObject ToObject ();

		[Export ("toObjectOfClass:")]
		NSObject ToObject (Class ofExpectedClass);

		[Export ("toBool")]
		bool ToBool ();

		[Export ("toDouble")]
		double ToDouble ();

		[Export ("toInt32")]
		int ToInt32 ();

		[Export ("toUInt32")]
		uint ToUInt32 ();

		[Export ("toNumber")]
		NSNumber ToNumber ();

		[Internal, Export ("toString")]
		string _ToString ();

		[Export ("toDate")]
		NSDate ToDate ();

		[Export ("toArray")]
		JSValue [] ToArray ();

		[Export ("toDictionary")]
		NSDictionary ToDictionary ();

		[Export ("valueForProperty:")]
		JSValue GetProperty (string property);

		[Export ("setValue:forProperty:")]
		void SetProperty (NSObject value, string property);

		[Export ("deleteProperty:")]
		bool DeleteProperty (string property);

		[Export ("hasProperty:")]
		bool HasProperty (string property);

		[Export ("defineProperty:descriptor:")]
		void DefineProperty (string property, NSObject descriptor);

		[Export ("valueAtIndex:")]
		JSValue GetValueAt (nuint index);

		[Export ("setValue:atIndex:")]
		void SetValue (JSValue value, nuint index);

		[Export ("isUndefined")]
		bool IsUndefined { get; }

		[Export ("isNull")]
		bool IsNull { get; }

		[Export ("isBoolean")]
		bool IsBoolean { get; }

		[Export ("isNumber")]
		bool IsNumber { get; }

		[Export ("isString")]
		bool IsString { get; }

		[Export ("isObject")]
		bool IsObject { get; }

		[iOS(9,0), Mac(10,11)]
		[Export ("isArray")]
		bool IsArray { get; }

		[iOS(9,0), Mac(10,11)]
		[Export ("isDate")]
		bool IsDate { get; }

		[Export ("isEqualToObject:")]
		// quick test shows that an NSNumber compares fine with a JSValue, so we'll keep NSObject
		bool IsEqualTo (NSObject value);

		[Export ("isEqualWithTypeCoercionToObject:")]
		// quick test shows that an NSNumber compares fine with a JSValue, so we'll keep NSObject
		bool IsEqualWithTypeCoercionTo (NSObject value);

		[Export ("isInstanceOf:")]
		bool IsInstanceOf (NSObject value);

		[Export ("callWithArguments:")]
		JSValue Call ([Params] JSValue [] arguments);

		[Export ("constructWithArguments:")]
		JSValue Construct ([Params] JSValue [] arguments);

		[Export ("invokeMethod:withArguments:")]
		JSValue Invoke (string method, [Params] JSValue [] arguments);

		[Export ("context", ArgumentSemantic.Retain)]
		JSContext Context { get; }

		#region Struct Support Category

		[Static, Export ("valueWithPoint:inContext:")]
		JSValue From (CGPoint point, JSContext context);

		[Static, Export ("valueWithRange:inContext:")]
		JSValue From (NSRange range, JSContext context);

		[Static, Export ("valueWithRect:inContext:")]
		JSValue From (CGRect rect, JSContext context);

		[Static, Export ("valueWithSize:inContext:")]
		JSValue From (CGSize size, JSContext context);

		[Export ("toPoint")]
		CGPoint ToPoint ();

		[Export ("toRange")]
		NSRange ToRange ();

		[Export ("toRect")]
		CGRect ToRect ();

		[Export ("toSize")]
		CGSize ToSize ();

		#endregion

		#region Subscript Support Category

		[Internal, Export ("objectForKeyedSubscript:")]
		JSValue _ObjectForKeyedSubscript (NSObject key);

		[Internal, Export ("objectAtIndexedSubscript:")]
		JSValue _ObjectAtIndexedSubscript (nuint index);

		// Since the getters returns JSValue (documented as such) we'll provide setters that do so (not NSObject)

		[Internal, Export ("setObject:forKeyedSubscript:")]
		void _SetObject (JSValue obj, NSObject key);

		[Internal, Export ("setObject:atIndexedSubscript:")]
		void _SetObject (JSValue obj, nuint index);

		#endregion

		[Mac (10,9), iOS (7,0)]
		[Static, Export ("valueWithJSValueRef:inContext:")]
		JSValue FromJSJSValueRef (IntPtr nativeJsValueRefvalue, JSContext context);

		[Export ("JSValueRef")]
		IntPtr JSValueRefPtr { get; }
	}

	[Mac (10,9, onlyOn64: true), iOS (7,0)]
	[BaseType (typeof (NSObject))]
#if XAMCORE_3_0
	[DisableDefaultCtor]
#endif
	partial interface JSManagedValue {
		[Static, Export ("managedValueWithValue:")]
		JSManagedValue Get (JSValue value);

		[Mac (10,10), iOS (8,0)]
		[Static, Export ("managedValueWithValue:andOwner:")]
		JSManagedValue Get (JSValue value, NSObject owner);

		[Export ("initWithValue:")]
		IntPtr Constructor (JSValue value);

		[Export ("value")]
		JSValue Value { get; }
	}

	[Mac (10,9, onlyOn64: true), iOS (7,0)]
	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	partial interface JSVirtualMachine {

		[Export ("init")]
		IntPtr Constructor ();

		[Export ("addManagedReference:withOwner:")]
		void AddManagedReference (NSObject obj, NSObject owner);

		[Export ("removeManagedReference:withOwner:")]
		void RemoveManagedReference (NSObject obj, NSObject owner);
	}

	[Mac (10,9, onlyOn64: true), iOS (7,0)]
	[Static]
	interface JSPropertyDescriptorKeys {

		[Field ("JSPropertyDescriptorWritableKey")]
		NSString Writable { get; }

		[Field ("JSPropertyDescriptorEnumerableKey")]
		NSString Enumerable { get; }

		[Field ("JSPropertyDescriptorConfigurableKey")]
		NSString Configurable { get; }

		[Field ("JSPropertyDescriptorValueKey")]
		NSString Value { get; }

		[Field ("JSPropertyDescriptorGetKey")]
		NSString Get { get; }

		[Field ("JSPropertyDescriptorSetKey")]
		NSString Set { get; }
	}

	[Mac (10,9, onlyOn64: true), iOS (7,0)]
	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface JSExport {

	}
}
