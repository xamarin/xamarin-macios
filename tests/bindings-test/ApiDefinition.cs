using System.Numerics;

using CoreGraphics;
using CoreMedia;

#if __MACOS__
using AppKit;
#else
using UIKit;
#endif

namespace Bindings.Test {
#if FRAMEWORK_TEST
	[BaseType (typeof (NSObject))]
	public interface FrameworkTest
	{
		[Export ("func")]
		int Func ();
	}
#endif

	[Static]
	interface Globals {
		[Field ("x_GlobalString", "__Internal")]
		NSString GlobalString { get; }
	}

	[BaseType (typeof (NSObject))]
	public interface UltimateMachine {
		[Export ("getAnswer")]
		int GetAnswer ();

		[Export ("sharedInstance")]
		[Static]
		UltimateMachine SharedInstance { get; }
	}

	delegate uint RegistrarTestBlock (uint magic);

	/*
	 * ObjC test class used for registrar
	 */
	[BaseType (typeof (NSObject))]
	partial interface ObjCRegistrarTest {
		[Export ("Pi1")]
		int Pi1 { get; set; }

		[Export ("Pi2")]
		int Pi2 { get; set; }

		[Export ("Pi3")]
		int Pi3 { get; set; }

		[Export ("Pi4")]
		int Pi4 { get; set; }

		[Export ("Pi5")]
		int Pi5 { get; set; }

		[Export ("Pi6")]
		int Pi6 { get; set; }

		[Export ("Pi7")]
		int Pi7 { get; set; }

		[Export ("Pi8")]
		int Pi8 { get; set; }

		[Export ("Pi9")]
		int Pi9 { get; set; }

		[Export ("Pf1")]
		float Pf1 { get; set; }

		[Export ("Pf2")]
		float Pf2 { get; set; }

		[Export ("Pf3")]
		float Pf3 { get; set; }

		[Export ("Pf4")]
		float Pf4 { get; set; }

		[Export ("Pf5")]
		float Pf5 { get; set; }

		[Export ("Pf6")]
		float Pf6 { get; set; }

		[Export ("Pf7")]
		float Pf7 { get; set; }

		[Export ("Pf8")]
		float Pf8 { get; set; }

		[Export ("Pf9")]
		float Pf9 { get; set; }

		[Export ("Pd1")]
		double Pd1 { get; set; }

		[Export ("Pd2")]
		double Pd2 { get; set; }

		[Export ("Pd3")]
		double Pd3 { get; set; }

		[Export ("Pd4")]
		double Pd4 { get; set; }

		[Export ("Pd5")]
		double Pd5 { get; set; }

		[Export ("Pd6")]
		double Pd6 { get; set; }

		[Export ("Pd7")]
		double Pd7 { get; set; }

		[Export ("Pd8")]
		double Pd8 { get; set; }

		[Export ("Pd9")]
		double Pd9 { get; set; }

		[Export ("Pc1")]
		sbyte Pc1 { get; set; }

		[Export ("Pc2")]
		sbyte Pc2 { get; set; }

		[Export ("Pc3")]
		sbyte Pc3 { get; set; }

		[Export ("Pc4")]
		sbyte Pc4 { get; set; }

		[Export ("Pc5")]
		sbyte Pc5 { get; set; }

		[Export ("V")]
		void V ();

		[Static]
		[Export ("staticV")]
		void StaticV ();

		[Export ("getEmptyString")]
		string GetEmptyString ();

		[Export ("getShortString")]
		string GetShortString ();

		[Export ("getLongString")]
		string GetLongString ();

		[Export ("someObject")]
		NSObject SomeObject { get; set; }

		[Export ("someArray")]
		NSObject [] SomeArray { get; set; }

		[Export ("F")]
		float F ();

		[Export ("D")]
		double D ();

		[Export ("Sd")]
		Sd Sd ();

		[Export ("Sf")]
		Sf Sf ();

		[Export ("V:i:i:i:i:i:i:")]
		void V (int i1, int i2, int i3, int i4, int i5, int i6, int i7);

		[Export ("V:f:f:f:f:f:f:f:f:")]
		void V (float f1, float f2, float f3, float f4, float f5, float f6, float f7, float f8, float f9);

		[Export ("V:i:i:i:i:i:i:f:f:f:f:f:f:f:f:f:")]
		void V (int i1, int i2, int i3, int i4, int i5, int i6, int i7, float f1, float f2, float f3, float f4, float f5, float f6, float f7, float f8, float f9);

		[Export ("V:d:d:d:d:d:d:d:d:")]
		void V (double d1, double d2, double d3, double d4, double d5, double d6, double d7, double d8, double d9);

		[Export ("V:i:Siid:i:i:d:d:d:i:i:i:")]
		void V (int i1, int i2, Siid Siid, int i3, int i4, double d1, double d2, double d3, int i5, int i6, int i7);

		[Export ("V:i:f:Siid:i:i:d:d:d:i:i:i:")]
		void V (int i1, int i2, float f1, Siid Siid, int i3, int i4, double d1, double d2, double d3, int i5, int i6, int i7);

		[Export ("V:c:c:c:c:i:d:")]
		void V (sbyte c1, sbyte c2, sbyte c3, sbyte c4, sbyte c5, int i1, double d1);

		[Export ("V:n:")]
		void V (out NSObject n1, out NSString n2);

		[Export ("invoke_V")]
		void Invoke_V ();

		[Export ("invoke_F")]
		float Invoke_F ();

		[Export ("invoke_D")]
		double Invoke_D ();

		[Export ("Sf_invoke")]
		Sf Sf_invoke ();

		[Export ("invoke_V_null_out")]
		void Invoke_V_null_out ();

		[Export ("methodReturningBlock")]
		RegistrarTestBlock MethodReturningBlock ();

		[Export ("propertyReturningBlock")]
		RegistrarTestBlock PropertyReturningBlock { get; }

		[Export ("testBlocks")]
		bool TestBlocks ();

		[Export ("idAsIntPtr:")]
		void IdAsIntPtr (IntPtr id);

		[Export ("outNSErrorOnStack:i:i:i:i:i:err:")]
		void OutNSErrorOnStack (int i1, int i2, int i3, int i4, int i5, int i6, out NSError error);

		[Export ("outNSErrorOnStack:obj:obj:int64:i:err:")]
		void OutNSErrorOnStack (NSObject i1, NSObject i2, NSObject i3, long i4, int i5, out NSError error);

		[NullAllowed]
		[Export ("stringArrayProperty")]
		string [] StringArrayProperty { get; set; }

		[Export ("setStringArrayMethod:")]
		void SetStringArrayMethod ([NullAllowed] string [] array);

		[return: NullAllowed]
		[Export ("getStringArrayMethod")]
		string [] GetStringArrayMethod ();

		[NullAllowed]
		[Export ("nsobjectArrayProperty")]
		NSObject [] NSObjectArrayProperty { get; set; }

		[Export ("setNSObjectArrayMethod:")]
		void SetNSObjectArrayMethod ([NullAllowed] NSObject [] array);

		[return: NullAllowed]
		[Export ("getNSObjectArrayMethod")]
		NSObject [] GetNSObjectArrayMethod ();

		[NullAllowed]
		[Export ("INSCodingArrayProperty")]
		INSCoding [] INSCodingArrayProperty { get; set; }

		[Export ("setINSCodingArrayMethod:")]
		void SetINSCodingArrayMethod ([NullAllowed] INSCoding [] array);

		[return: NullAllowed]
		[Export ("getINSCodingArrayMethod")]
		INSCoding [] GetINSCodingArrayMethod ();
	}

	[Protocol]
	interface ProtocolAssignerProtocol {
	}

	interface IProtocolAssignerProtocol { }

	[BaseType (typeof (NSObject))]
	interface ProtocolAssigner {
		[Export ("setProtocol")]
		void SetProtocol ();

		[Export ("completedSetProtocol:")]
		void CompletedSetProtocol (IProtocolAssignerProtocol value);
	}

	[BaseType (typeof (NSObject))]
	interface ObjCExceptionTest {
		[Export ("throwObjCException")]
		void ThrowObjCException ();

		[Export ("throwManagedException")]
		void ThrowManagedException ();

		[Export ("invokeManagedExceptionThrower")]
		void InvokeManagedExceptionThrower ();

		[Export ("invokeManagedExceptionThrowerAndRethrow")]
		void InvokeManagedExceptionThrowerAndRethrow ();

		[Export ("invokeManagedExceptionThrowerAndCatch")]
		void InvokeManagedExceptionThrowerAndCatch ();
	}

	[BaseType (typeof (NSObject))]
	interface CtorChaining1 {
		[Export ("initCalled")]
		bool InitCalled { get; set; }

		[Export ("initCallsInitCalled")]
		bool InitCallsInitCalled { get; set; }

		[Export ("initCallsInit:")]
		IntPtr Constructor (int value);
	}

	[BaseType (typeof (NSObject))]
	[Model]
	[Protocol]
	interface ObjCProtocolTest {
		[Export ("idAsIntPtr:")]
		void IdAsIntPtr (IntPtr p1);

		[Export ("methodEncodings:obj2:obj3:obj4:obj5:obj6:obj7:")]
		void GetMethodEncodings (ref NSObject obj1, ref NSObject obj2, ref NSObject obj3, ref NSObject obj4, ref NSObject obj5, ref NSObject obj6, ref NSObject obj7);

		[Export ("setPtrPropertyCGRect:p2:p3:p4:p5:p6:")]
		void SetPtrPropertyCGRect (nint p1, nint p2, nint p3, nint p4, ref global::CoreGraphics.CGRect p5, nint p6);
	}

	[Protocol]
	interface ObjCProtocolBlockTest {
		[Abstract]
		[Export ("requiredCallback:")]
		void RequiredCallback (Action<int> completionHandler);

		[Abstract]
		[Static]
		[Export ("requiredStaticCallback:")]
		void RequiredStaticCallback (Action<int> completionHandler);

		[Export ("optionalCallback:")]
		void OptionalCallback (Action<int> completionHandler);

		[Static]
		[Export ("optionalStaticCallback:")]
		void OptionalStaticCallback (Action<int> completionHandler);

		[Abstract]
		[Export ("requiredReturnValue")]
		Action<int> RequiredReturnValue ();

		[Abstract]
		[Static]
		[Export ("requiredStaticReturnValue")]
		Action<int> RequiredStaticReturnValue ();

		[Export ("optionalReturnValue")]
		Action<int> OptionalReturnValue ();

		[Static]
		[Export ("optionalStaticReturnValue")]
		Action<int> OptionalStaticReturnValue ();
	}

	interface IObjCProtocolBlockTest { }

	[BaseType (typeof (NSObject))]
	interface ObjCBlockTester {
		[Export ("TestObject", ArgumentSemantic.Retain)]
		IObjCProtocolBlockTest TestObject { get; set; }

		[Static]
		[Export ("TestClass")]
		Class TestClass { get; set; }

		[Export ("classCallback:")]
		void ClassCallback (Action<int> completionHandler);

		[Export ("callClassCallback")]
		void CallClassCallback ();

		[Export ("callRequiredCallback")]
		void CallRequiredCallback ();

		[Static]
		[Export ("callRequiredStaticCallback")]
		void CallRequiredStaticCallback ();

		[Export ("callOptionalCallback")]
		void CallOptionalCallback ();

		[Static]
		[Export ("callOptionalStaticCallback")]
		void CallOptionalStaticCallback ();

		[Static]
		[Export ("callAssertMainThreadBlockRelease:")]
		void CallAssertMainThreadBlockRelease (OuterBlock completionHandler);

		[Static]
		[Export ("callAssertMainThreadBlockReleaseQOS:")]
		void CallAssertMainThreadBlockReleaseQOS (OuterBlock completionHandler);

		[Export ("assertMainThreadBlockReleaseCallback:")]
		void AssertMainThreadBlockReleaseCallback (InnerBlock completionHandler);

		[Export ("callAssertMainThreadBlockReleaseCallback")]
		void CallAssertMainThreadBlockReleaseCallback ();

		[Export ("callAssertMainThreadBlockReleaseCallbackQOS")]
		void CallAssertMainThreadBlockReleaseCallbackQOS ();

		[Export ("testFreedBlocks")]
		void TestFreedBlocks ();

		[Static]
		[Export ("freedBlockCount")]
		int FreedBlockCount { get; }

		[Static]
		[Export ("calledBlockCount")]
		int CalledBlockCount { get; }

		[Static]
		[Export ("callProtocolWithBlockProperties:required:instance:")]
		void CallProtocolWithBlockProperties (IProtocolWithBlockProperties obj, bool required, bool instance);

		[Static]
		[Export ("callProtocolWithBlockReturnValue:required:instance:")]
		void CallProtocolWithBlockReturnValue (IObjCProtocolBlockTest obj, bool required, bool instance);

		[Static]
		[Export ("setProtocolWithBlockProperties:required:instance:")]
		void SetProtocolWithBlockProperties (IProtocolWithBlockProperties obj, bool required, bool instance);

		[Export ("nullableCallback:")]
		bool NullableCallback ([NullAllowed] Action<int> completionHandler);
	}

	delegate void InnerBlock (int magic_number);
	delegate void OuterBlock ([BlockCallback] InnerBlock callback);

	[Protocol]
	interface ConstructorProtocol {
		[Abstract]
		[Export ("initRequired:")]
		IntPtr Constructor (string p0);

		[Export ("initOptional:")]
		IntPtr Constructor (NSDate p0);
	}

	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface TypeProvidingProtocolConstructors : ConstructorProtocol {
		[Export ("initRequired:")]
		new IntPtr Constructor (string p0);

		[Export ("initOptional:")]
		new IntPtr Constructor (NSDate p0);

		[Export ("stringValue")]
		string StringValue { get; set; }

		[Export ("dateValue")]
		NSDate DateValue { get; set; }
	}

	[BaseType (typeof (NSObject))]
	interface EvilDeallocator {
		[Export ("evilCallback")]
		Action<int> EvilCallback { get; set; }
	}

	delegate void SimpleCallback ();
	[BaseType (typeof (NSObject))]
	[Protocol]
	interface ProtocolWithBlockProperties {
		[Abstract]
		[Export ("myRequiredProperty")]
		SimpleCallback MyRequiredProperty { get; set; }

		[Export ("myOptionalProperty")]
		SimpleCallback MyOptionalProperty { get; set; }

		[Static]
		[Abstract]
		[Export ("myRequiredStaticProperty")]
		SimpleCallback MyRequiredStaticProperty { get; set; }

		[Static]
		[Export ("myOptionalStaticProperty")]
		SimpleCallback MyOptionalStaticProperty { get; set; }
	}
	interface IProtocolWithBlockProperties { }

	[BaseType (typeof (NSObject))]
	interface SwiftTestClass {
		[Export ("SayHello")]
		string SayHello ();

		[Export ("DoSomethingWithMessage:")]
		string DoSomething (string message);

		[Export ("DoSomethingAsyncWithMessage:completionHandler:")]
		void DoSomethingAsync (string message, Action<NSString> completionHandler);

		[Export ("DoSomethingComplexAsyncWithMessage:complexParameter:completionHandler:")]
		// The type for 'complexParameter' is something like: Func<Func<Int16, Int64>, NSString>
		// But the generator can't handle that, it generates code that doesn't compile.
		// So just bind it as IntPtr.
		// This is not a problem for this test, because the point of this test is to verify that
		// we're able to skip the corresponding objc type encoding, and for that we don't need to
		// provide an actual argument when calling the method.
		void DoSomethingComplexAsync (string message, IntPtr complexParameter, Action<NSString> completionHandler);
	}

	[BaseType (typeof (SwiftTestClass), IsStubClass = true)]
	interface SwiftTestClass2 {
		[Export ("SayHello2")]
		string SayHello2 ();
	}

	[Protocol]
	interface VeryGenericElementProtocol {
		[Export ("when", ArgumentSemantic.Retain)]
		NSDate When { get; }
	}

	interface IVeryGenericElementProtocol : INativeObject { }

	[Protocol]
	interface VeryGenericElementProtocol1 : VeryGenericElementProtocol {
		[Export ("number")]
		nint Number { get; }
	}

	interface IVeryGenericElementProtocol1 : IVeryGenericElementProtocol { }

	[Protocol]
	interface VeryGenericElementProtocol2 : VeryGenericElementProtocol {
		[Export ("animal", ArgumentSemantic.Retain)]
		string Animal { get; }
	}

	interface IVeryGenericElementProtocol2 : IVeryGenericElementProtocol { }

	[BaseType (typeof (NSObject))]
	interface VeryGenericCollection<Key, Element>
		where Key : NSString
		where Element : IVeryGenericElementProtocol {
		[Export ("count")]
		nuint Count { get; }

		[Export ("getElement:"), NullAllowed]
		Element GetElement (Key key);

		[Export ("elementEnumerator"), NullAllowed]
		NSEnumerator<Element> GetEnumerator ();

		[Export ("add:")]
		void Add (Element element);
	}

	[Protocol]
	interface VeryGenericConsumerProtocol {
		[Export ("first", ArgumentSemantic.Retain)]
		VeryGenericCollection<NSString, IVeryGenericElementProtocol1> First { get; }

		[Export ("second", ArgumentSemantic.Retain)]
		VeryGenericCollection<NSString, IVeryGenericElementProtocol2> Second { get; }
	}

	interface IVeryGenericConsumerProtocol { }

	[BaseType (typeof (NSObject))]
	interface VeryGenericFactory {
		[Export ("getConsumer")]
		[Static]
		IVeryGenericConsumerProtocol GetConsumer ();
	}

	[BaseType (typeof (NSObject))]
	interface WeakReferencedObject {
		[Export ("doSomething")]
		int DoSomething ();
	}

	[BaseType (typeof (NSObject))]
	interface WeakReferenceHolder {
		[Export ("addObject:")]
		void AddObject (WeakReferencedObject obj);

		[Export ("callDoSomething:nonNilObjectCount:gotExpectedResponse:gotUnexpectedResponse:gotFinalizedResponse:")]
		void CallDoSomething (ref int nilObjectCount, ref int nonNilObjectCount, ref int gotExpectedResponse, ref int gotUnexpectedResponse, ref int gotFinalizedResponse);
	}

	enum StrongEnum {
		[Field ("StrongEnumA", LibraryName = "__Internal")]
		A,

		[Field ("StrongEnumB", LibraryName = "__Internal")]
		B,

		[Field ("StrongEnumC", LibraryName = "__Internal")]
		C,
	}

	interface IBindAsProtocol { }

	[BaseType (typeof (NSObject)), Model]
	[Protocol]
	interface BindAsProtocol {
		[Export ("setStrongEnum:")]
		void SetStrongEnum ([BindAs (typeof (StrongEnum))] NSString value);

		[Export ("setStrongEnum:other:")]
		void SetStrongEnums ([BindAs (typeof (StrongEnum))] NSString value, [BindAs (typeof (StrongEnum))] NSString other);

		[Export ("getValues:strongEnum:handler:")]
		void GetValues (string [] identifiers, [BindAs (typeof (StrongEnum))] NSString value, Action<string []> handler);
	}

	enum NormalEnum {
		X,
		Y,
		Z,
	}

	[Partial]
	interface StrongDictionaryKeys {
		// simple types
		[Field ("StrongDictionarySByteField", LibraryName = "__Internal")]
		NSString SByteField { get; }

		[Field ("StrongDictionaryInt16Field", LibraryName = "__Internal")]
		NSString Int16Field { get; }

		[Field ("StrongDictionaryInt32Field", LibraryName = "__Internal")]
		NSString Int32Field { get; }

		[Field ("StrongDictionaryInt64Field", LibraryName = "__Internal")]
		NSString Int64Field { get; }

		[Field ("StrongDictionaryByteField", LibraryName = "__Internal")]
		NSString ByteField { get; }

		[Field ("StrongDictionaryUInt16Field", LibraryName = "__Internal")]
		NSString UInt16Field { get; }

		[Field ("StrongDictionaryUInt32Field", LibraryName = "__Internal")]
		NSString UInt32Field { get; }

		[Field ("StrongDictionaryUInt64Field", LibraryName = "__Internal")]
		NSString UInt64Field { get; }

		[Field ("StrongDictionaryNIntField", LibraryName = "__Internal")]
		NSString NIntField { get; }

		[Field ("StrongDictionaryNUIntField", LibraryName = "__Internal")]
		NSString NUIntField { get; }

		[Field ("StrongDictionarySingleField", LibraryName = "__Internal")]
		NSString SingleField { get; }

		[Field ("StrongDictionaryDoubleField", LibraryName = "__Internal")]
		NSString DoubleField { get; }

		[Field ("StrongDictionaryNFloatField", LibraryName = "__Internal")]
		NSString NFloatField { get; }

		[Field ("StrongDictionaryNSObjectField", LibraryName = "__Internal")]
		NSString NSObjectField { get; }

		[Field ("StrongDictionaryBooleanField", LibraryName = "__Internal")]
		NSString BooleanField { get; }

		[Field ("StrongDictionaryNSStringField", LibraryName = "__Internal")]
		NSString NSStringField { get; }

		[Field ("StrongDictionaryNSDateField", LibraryName = "__Internal")]
		NSString NSDateField { get; }

		[Field ("StrongDictionaryNSDictionaryField", LibraryName = "__Internal")]
		NSString NSDictionaryField { get; }

		[Field ("StrongDictionaryStrongDictionaryField", LibraryName = "__Internal")]
		NSString NSStrongDictionaryField { get; }

		[Field ("StrongDictionaryStrongEnumField", LibraryName = "__Internal")]
		NSString StrongEnumField { get; }

		[Field ("StrongDictionaryNormalEnumField", LibraryName = "__Internal")]
		NSString NormalEnumField { get; }


		// arrays of the above

		[Field ("StrongDictionaryArrayOfSByteField", LibraryName = "__Internal")]
		NSString ArrayOfSByteField { get; }

		[Field ("StrongDictionaryArrayOfInt16Field", LibraryName = "__Internal")]
		NSString ArrayOfInt16Field { get; }

		[Field ("StrongDictionaryArrayOfInt32Field", LibraryName = "__Internal")]
		NSString ArrayOfInt32Field { get; }

		[Field ("StrongDictionaryArrayOfInt64Field", LibraryName = "__Internal")]
		NSString ArrayOfInt64Field { get; }

		[Field ("StrongDictionaryArrayOfByteField", LibraryName = "__Internal")]
		NSString ArrayOfByteField { get; }

		[Field ("StrongDictionaryArrayOfUInt16Field", LibraryName = "__Internal")]
		NSString ArrayOfUInt16Field { get; }

		[Field ("StrongDictionaryArrayOfUInt32Field", LibraryName = "__Internal")]
		NSString ArrayOfUInt32Field { get; }

		[Field ("StrongDictionaryArrayOfUInt64Field", LibraryName = "__Internal")]
		NSString ArrayOfUInt64Field { get; }

		[Field ("StrongDictionaryArrayOfNIntField", LibraryName = "__Internal")]
		NSString ArrayOfNIntField { get; }

		[Field ("StrongDictionaryArrayOfNUIntField", LibraryName = "__Internal")]
		NSString ArrayOfNUIntField { get; }

		[Field ("StrongDictionaryArrayOfSingleField", LibraryName = "__Internal")]
		NSString ArrayOfSingleField { get; }

		[Field ("StrongDictionaryArrayOfDoubleField", LibraryName = "__Internal")]
		NSString ArrayOfDoubleField { get; }

		[Field ("StrongDictionaryArrayOfNFloatField", LibraryName = "__Internal")]
		NSString ArrayOfNFloatField { get; }

		[Field ("StrongDictionaryArrayOfNSObjectField", LibraryName = "__Internal")]
		NSString ArrayOfNSObjectField { get; }

		[Field ("StrongDictionaryArrayOfBooleanField", LibraryName = "__Internal")]
		NSString ArrayOfBooleanField { get; }

		[Field ("StrongDictionaryArrayOfNSStringField", LibraryName = "__Internal")]
		NSString ArrayOfNSStringField { get; }

		[Field ("StrongDictionaryArrayOfNSDateField", LibraryName = "__Internal")]
		NSString ArrayOfNSDateField { get; }

		[Field ("StrongDictionaryArrayOfNSDictionaryField", LibraryName = "__Internal")]
		NSString ArrayOfNSDictionaryField { get; }

		[Field ("StrongDictionaryArrayOfStrongDictionaryField", LibraryName = "__Internal")]
		NSString ArrayOfStrongDictionaryField { get; }

		[Field ("StrongDictionaryArrayOfStrongEnumField", LibraryName = "__Internal")]
		NSString ArrayOfStrongEnumField { get; }

		[Field ("StrongDictionaryArrayOfNormalEnumField", LibraryName = "__Internal")]
		NSString ArrayOfNormalEnumField { get; }

		// other fields

		[Field ("StrongDictionaryStringField", LibraryName = "__Internal")]
		NSString StringField { get; }

		[Field ("StrongDictionaryDateTimeField", LibraryName = "__Internal")]
		NSString DateTimeField { get; }

		[Field ("StrongDictionaryGenericNSDictionaryField", LibraryName = "__Internal")]
		NSString GenericNSDictionaryField { get; }

		[Field ("StrongDictionaryArrayOfStringField", LibraryName = "__Internal")]
		NSString ArrayOfStringField { get; }

		[Field ("StrongDictionaryNSDataField", LibraryName = "__Internal")]
		NSString NSDataField { get; }

		[Field ("StrongDictionaryNSDataAsMatrix3Field", LibraryName = "__Internal")]
		NSString NSDataAsMatrix3Field { get; }

		[Field ("StrongDictionaryCGRectField", LibraryName = "__Internal")]
		NSString CGRectField { get; }

		[Field ("StrongDictionaryCGSizeField", LibraryName = "__Internal")]
		NSString CGSizeField { get; }

		[Field ("StrongDictionaryCGPointField", LibraryName = "__Internal")]
		NSString CGPointField { get; }

		[Field ("StrongDictionaryCMTimeField", LibraryName = "__Internal")]
		NSString CMTimeField { get; }

#if !__MACOS__
		[Field ("StrongDictionaryUIEdgeInsetsField", LibraryName = "__Internal")]
		NSString UIEdgeInsetsField { get; }
#endif // !__MACOS__
	}

	[StrongDictionary ("StrongDictionaryKeys", Suffix = "")]
	interface WrappedNSDictionary {
		// simple types
		sbyte SByteField { get; set; }
		short Int16Field { get; set; }
		int Int32Field { get; set; }
		long Int64Field { get; set; }
		byte ByteField { get; set; }
		ushort UInt16Field { get; set; }
		uint UInt32Field { get; set; }
		ulong UInt64Field { get; set; }
		nint NIntField { get; set; }
		nuint NUIntField { get; set; }
		float SingleField { get; set; }
		double DoubleField { get; set; }
		nfloat NFloatField { get; set; }
		NSObject NSObjectField { get; set; }
		bool BooleanField { get; set; }
		NSString NSStringField { get; set; }
		NSDate NSDateField { get; set; }
		NSDictionary NSDictionaryField { get; set; }
		WrappedNSDictionary NSStrongDictionaryField { get; set; }
		StrongEnum StrongEnumField { get; set; }
		NormalEnum NormalEnumField { get; set; }

		// arrays of the above
		sbyte [] ArrayOfSByteField { get; set; }
		short [] ArrayOfInt16Field { get; set; }
		int [] ArrayOfInt32Field { get; set; }
		long [] ArrayOfInt64Field { get; set; }
		byte [] ArrayOfByteField { get; set; }
		ushort [] ArrayOfUInt16Field { get; set; }
		uint [] ArrayOfUInt32Field { get; set; }
		ulong [] ArrayOfUInt64Field { get; set; }
		nint [] ArrayOfNIntField { get; set; }
		nuint [] ArrayOfNUIntField { get; set; }
		float [] ArrayOfSingleField { get; set; }
		double [] ArrayOfDoubleField { get; set; }
		nfloat [] ArrayOfNFloatField { get; set; }
		NSObject [] ArrayOfNSObjectField { get; set; }
		bool [] ArrayOfBooleanField { get; set; }
		NSString [] ArrayOfNSStringField { get; set; }
		NSDate [] ArrayOfNSDateField { get; set; }
		NSDictionary [] ArrayOfNSDictionaryField { get; set; }
		WrappedNSDictionary [] ArrayOfStrongDictionaryField { get; set; }
		StrongEnum [] ArrayOfStrongEnumField { get; set; }
		NormalEnum [] ArrayOfNormalEnumField { get; set; }

		// other fields
		string StringField { get; set; }
		DateTime DateTimeField { get; set; }
		NSDictionary<NSString, NSObject> GenericNSDictionaryField { get; set; }
		string [] ArrayOfStringField { get; set; }
		NSData NSDataField { get; set; }
		NMatrix3 NSDataAsMatrix3Field { get; set; }
		CGRect CGRectField { get; set; }
		CGSize CGSizeField { get; set; }
		CGPoint CGPointField { get; set; }
		CMTime CMTimeField { get; set; }
#if !__MACOS__
		UIEdgeInsets UIEdgeInsetsField { get; set; }
#endif // !__MACOS____MACOS__
	}

	[BaseType (typeof (NSObject))]
	[DisableDefaultCtor]
	interface ClassWithNoDefaultCtor { }

	interface IHitchhikerDelegate { }

	[BaseType (typeof (NSObject)), Model]
	[Protocol]
	interface HitchhikerDelegate {
		[EventArgs ("")]
		[Export ("buildIntergalacticHighway:")]
		void BuildIntergalacticHighway (Hitchhiker sender);

		[EventArgs ("")]
		[Export ("byeByeDolphins:")]
		void ByeByeDolphins (Hitchhiker sender);

		[EventArgs ("")]
		[Export ("hitchhikeWithVogons:")]
		void HitchhikeWithVogons (Hitchhiker sender);
	}

	[BaseType (typeof (NSObject), Delegates = new string [] { "Delegate" }, Events = new Type [] { typeof (HitchhikerDelegate) })]
	interface Hitchhiker {
		[Export ("delegate", ArgumentSemantic.Retain)]
		IHitchhikerDelegate Delegate { get; set; }

		[Export ("destroyEarth")]
		void DestroyEarth ();

		[Export ("buildHighway")]
		void BuildHighway ();
	}

	// Helper for the issue #25861 design work: its native 'init' calls a method that
	// can be overridden in managed code, to verify the overridden managed method is
	// invoked (on the correct instance) while 'init' is still executing.
	[BaseType (typeof (NSObject))]
	interface InitCallsVirtualMethod {
		[Export ("virtualMethodCalledDuringInit:")]
		void VirtualMethodCalledDuringInit (int value);
	}

	// Helper for the issue #25861 design work: a directly-bound native class whose
	// native 'init' synchronously surfaces 'self' to managed code via a C callback.
	[BaseType (typeof (NSObject))]
	interface InitSurfacesSelfToManaged {
	}

	// Helpers for a deterministic reproduction of the alloc/init handle-reuse race in
	// issue #25861 (and #9478): ReuseSlotClassA's 'init' frees its own instance and forces
	// the next allocation to reuse that exact address, so a ReuseSlotClassB allocated from
	// managed code during that 'init' deterministically lands on the address freed by the
	// ReuseSlotClassA instance.
	[BaseType (typeof (NSObject))]
	interface ReuseSlotClassA {
	}

	[BaseType (typeof (NSObject))]
	interface ReuseSlotClassB {
	}

	// Helper for issue #23679: a native class whose 'init' fails (releases self and
	// returns nil), so constructing the managed wrapper throws.
	[BaseType (typeof (NSObject))]
	interface InitReturnsNilClass {
	}

	[BaseType (typeof (NSObject))]
	interface ThreadExitObject {
		[Static]
		[Export ("releaseOnThreadExit:")]
		bool ReleaseOnThreadExit (NSObject obj);

		[Static]
		[Export ("releasedObjectCount")]
		int ReleasedObjectCount { get; }
	}
}
