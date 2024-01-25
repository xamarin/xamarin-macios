using System;

using CoreFoundation;

using Foundation;

using ObjCRuntime;

namespace BI1064 {
	[BaseType (typeof (NSObject))]
	interface RefOutParameters {
		[Export ("testCFBundle:a:b:")]
		void TestCFBundle (int action, ref CFBundle refValue, out CFBundle outValue);

		[Export ("testINSCoding:a:b:")]
		void TestINSCoding (int action, ref INSCoding refValue, out INSCoding outValue);

		[Export ("testNSObject:a:b:")]
		void TestNSObject (int action, ref NSObject refValue, out NSObject outValue);

		[Export ("testNSValue:a:b:")]
		void TestValue (int action, ref NSValue refValue, out NSValue outValue);

		[Export ("testString:a:b:")]
		void TestString (int action, ref string refValue, out string outValue);

		[Export ("testInt:a:b:c:")]
		unsafe void TestInt (int action, ref int refValue, out int outValue, int* ptrValue);

		[Export ("testSelector:a:b:")]
		void TestSelector (int action, ref Selector refValue, out Selector outValue);

		[Export ("testClass:a:b:")]
		void TestClass (int action, ref Class refValue, out Class outValue);

		[Export ("testINSCodingArray:a:b:")]
		void TestINSCodingArray (int action, ref INSCoding [] refValues, out INSCoding [] outValues);

		[Export ("testNSObjectArray:a:b:")]
		void TestNSObjectArray (int action, ref NSObject [] refValues, out NSObject [] outValues);

		[Export ("testNSValueArray:a:b:")]
		void TestNSValueArray (int action, ref NSValue [] refValues, out NSValue [] outValues);

		[Export ("testStringArray:a:b:")]
		void TestStringArray (int action, ref string [] refStrings, out string [] outStrings);

		[Export ("testClassArray:a:b:")]
		void TestClassArray (int action, ref Class [] refStrings, out Class [] outStrings);
	}
}
