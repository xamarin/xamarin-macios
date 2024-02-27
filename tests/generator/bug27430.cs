using System;
using System.Drawing;

using ObjCRuntime;
using Foundation;
using UIKit;

namespace Bug27430 {
	delegate void SomeHandler (int @int, string @string, NSError @namespace);
	delegate void SomeHandler2 (NSDictionary [] @switch, string [] @case, NSError @namespace);

	[BaseType (typeof (NSObject))]
	interface Widget {

		[Export ("initWithTest:")]
		IntPtr Constructor (IFooTestDelegate @delegate);

		[Async (ResultTypeName = "FooResultTask")]
		[Export ("doSomething:atIndex::::")]
		void DoSomething (NSObject obj, string str, int i, uint @uint, SomeHandler handler);

		[Async]
		[Export ("doSomething1:atIndex::::")]
		void DoSomething1 (NSObject @object, string @string, int @int, uint @uint, Action @delegate);

		[Export ("outTest:atIndex::::")]
		void OutTest (out NSObject @object, out string @string, out int @int, out uint @uint, Action @delegate);

		[Export ("refTest:atIndex::::")]
		void RefTest (ref NSObject @object, ref string @string, ref int @int, ref uint @uint, Action @delegate);

		[Export ("RefHandlderTest:atIndex::::")]
		void RefHandlderTest (ref NSObject @object, ref string @string, ref int @int, ref uint @uint, SomeHandler @delegate);

		[Export ("PlainStringTest:atIndex::::")]
		void PlainStringTest (NSObject @object, [PlainString] string @string, NSString @for, NSNumber @foreach, NSNumber @override);

		[Export ("NullAllowedTest:atIndex::::")]
		void NullAllowedTest ([NullAllowed] NSObject @object, [NullAllowed] string @string, [NullAllowed] NSString @for, [NullAllowed] NSNumber @foreach, [NullAllowed] NSNumber @override);

		[Async]
		[Export ("arrayTest:atIndex::::")]
		void ArrayTest (NSObject [] @object, string [] @string, int @int, uint @uint, Action @delegate);

		[Async (ResultTypeName = "ArrayResultTask")]
		[Export ("arrayTest2:atIndex::::")]
		void ArrayTest2 (NSObject [] @object, string [] @string, int @int, uint @uint, SomeHandler2 @delegate);

		[Async]
		[Export ("doSomethingErr:atIndex::::")]
		void DoSomethingErrTest (NSObject @object, string @string, int @int, uint @uint, Action<NSError> @delegate);

		[Async (ResultTypeName = "FooResultTask2")]
		[Export ("doSomethingErr2:atIndex::::")]
		void DoSomethingErr2Test (NSObject @object, string @string, int @int, uint @uint, Action<int, double, NSError> @delegate);

		[Export ("putDelegatesSomewhere:::")]
		void PutDelegatesTest (IFooTestDelegate @delegate, IUIPageViewControllerDelegate @namespace, int @int);

		[Export ("delegate", ArgumentSemantic.Weak), NullAllowed]
		IFooTestDelegate @delegate { get; set; }

		[Export ("dont")]
		string @string { get; set; }

		[Export ("dont2")]
		int @int { [Bind ("isDont2")] get; set; }

		[Export ("dont3")]
		UIView [] @foreach { get; [Bind ("foo3:")] set; }

		[Export ("dont4")]
		UIView [] @case { [Bind ("bar4")] get; [Bind ("foo4:")] set; }

		[Export ("dont")]
		NSArray @void { get; }
	}

	interface IFooTestDelegate { }

	[Protocol, Model]
	[BaseType (typeof (NSObject))]
	interface FooTestDelegate {

		[Export ("someFakeDelegateMethod:withDelegate:")]
		void SomeFakeDelegateMethod (UIViewController @switch, IFooTestDelegate @delegate);
	}
}
