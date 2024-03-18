using System;

using ObjCRuntime;
using Foundation;
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

	}

	delegate void InnerBlock (int magic_number);
	delegate void OuterBlock ([BlockCallback] InnerBlock callback);

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
}
