//
// Copyright 2014 Xamarin Inc.
//

#if !__WATCHOS__

using System;
using Foundation;
using JavaScriptCore;
using ObjCRuntime;
using NUnit.Framework;

using XamarinTests.ObjCRuntime;
using MonoTouchFixtures.ObjCRuntime;

namespace MonoTouchFixtures.JavascriptCore {

	// we want the test to be availble if we use the linker
	[Preserve (AllMembers = true)]
	[TestFixture]
	public class JSExportTest {
		[Test]
		public void ExportTest ()
		{
			TestRuntime.AssertXcodeVersion (5, 0, 1);

			if (!global::XamarinTests.ObjCRuntime.Registrar.IsStaticRegistrar)
				Assert.Ignore ("Exporting protocols to JavaScriptCore requires the static registrar.");

			var context = new JSContext ();
			JSValue exc = null;
			context.ExceptionHandler = (JSContext context2, JSValue exception) => {
				exc = exception;
			};
			var obj = new MyJavaExporter ();
			context [(NSString) "obj"] = JSValue.From (obj, context);
			context.EvaluateScript ("obj.myFunc ();");
			Assert.IsNull (exc, "JS exception");
			Assert.IsTrue (obj.MyFuncCalled, "Called");

			context.EvaluateScript ("obj.hello (42);");
			context.EvaluateScript ("obj.callMeBack (function() { return 314; });");
		}
	}

	// This interface will show a warning with the dynamic registrar:
	// MonoTouch.RuntimeException: Detected a protocol (MonoTouchFixtures.JavascriptCore.IMyJavaExporter) inheriting from the JSExport protocol while using the dynamic registrar. It is not possible to export protocols to JavaScriptCore dynamically; the static registrar must be used (add '--registrar:static to the additional mtouch arguments in the project's iOS Build options to select the static registrar).
	// This warning is expected.
	[Protocol ()]
	interface IMyJavaExporter : IJSExport {
		[Export ("myFunc")]
		void MyFunc ();

		[Export ("hello:")]
		void Hello (JSValue val);

		[Export ("callMeBack:")]
		void CallMeBack (JSValue callbackFunc);
	}

	class MyJavaExporter : NSObject, IMyJavaExporter {
		public bool MyFuncCalled;
		public void MyFunc ()
		{
			MyFuncCalled = true;
		}

		public void Hello (JSValue val)
		{
			Assert.IsTrue (val.IsNumber, "Hello - IsNumber");
			Assert.AreEqual (42, val.ToNumber ().Int32Value, "Hello - Number");
		}

		public void CallMeBack (JSValue callbackFunc)
		{
			var rv = callbackFunc.Call ();
			Assert.IsTrue (rv.IsNumber, "CallMeBack - IsNumber");
			Assert.AreEqual (314, rv.ToNumber ().Int32Value, "CallMeBack - Number");
		}
	}
}

#endif // !__WATCHOS__
