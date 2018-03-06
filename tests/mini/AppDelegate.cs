using System;
using System.Collections.Generic;
using System.Linq;
#if XAMCORE_2_0
using Foundation;
using UIKit;
#else
using MonoTouch.Foundation;
using MonoTouch.UIKit;
#endif

using MonoTouch.NUnit.UI;
using NUnit.Framework;
using NUnit.Framework.Internal.Filters;

namespace mini
{
	[TestFixture]
	class JitTests {
#if BITCODE
		static string[] args = new string[] { "--exclude", "!FULLAOT", "--exclude", "!BITCODE", "--verbose" };
#else
		static string[] args = new string[] { "--exclude", "!FULLAOT", "--verbose" };
#endif

		[Test]
		public static void Basic () {
			int res = TestDriver.RunTests (typeof (BasicTests), args);
			Assert.AreEqual (0, res);
		}

		[Test]
		public static void Arrays () {
			int res = TestDriver.RunTests (typeof (ArrayTests), args);
			Assert.AreEqual (0, res);
		}

		[Test]
		public static void Calls () {
			int res = TestDriver.RunTests (typeof (CallsTests), args);
			Assert.AreEqual (0, res);
		}
		[Test]
		public static void Float () {
			int res = TestDriver.RunTests (typeof (FloatTests), args);
			Assert.AreEqual (0, res);
		}
		[Test]
		public static void Long () {
			int res = TestDriver.RunTests (typeof (LongTests), args);
			Assert.AreEqual (0, res);
		}

		[Test]
		public static void Math () {
			int res = TestDriver.RunTests (typeof (MathTests), args);
			Assert.AreEqual (0, res);
		}

		[Test]
		public static void Objects () {
			int res = TestDriver.RunTests (typeof (ObjectTests.Tests), args);
			Assert.AreEqual (0, res);
		}

		[Test]
		public static void Generics () {
			int res = TestDriver.RunTests (typeof (GenericsTests), args);
			Assert.AreEqual (0, res);
		}

		[Test]
		public static void GShared () {
			int res = TestDriver.RunTests (typeof (GSharedTests), args);
			Assert.AreEqual (0, res);
		}

		[Test]
		public static void Exceptions () {
			int res = TestDriver.RunTests (typeof (ExceptionTests), args);
			Assert.AreEqual (0, res);
		}

		[Test]
		public static void Aot () {
			int res = TestDriver.RunTests (typeof (AotTests), args);
			Assert.AreEqual (0, res);
		}
	}

#if !__WATCHOS__
	// The UIApplicationDelegate for the application. This class is responsible for launching the 
	// User Interface of the application, as well as listening (and optionally responding) to 
	// application events from iOS.
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		// class-level declarations
		UIWindow window;
		TouchRunner runner;

		//
		// This method is invoked when the application has loaded and is ready to run. In this 
		// method you should instantiate the window, load the UI into it and then make the window
		// visible.
		//
		// You have 17 seconds to return from this method, or iOS will terminate your application.
		//
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			window = new UIWindow (UIScreen.MainScreen.Bounds);
			runner = new TouchRunner (window);

			runner.Add (System.Reflection.Assembly.GetExecutingAssembly ());

			window.RootViewController = new UINavigationController (runner.GetViewController ());
			window.MakeKeyAndVisible ();

			return true;
		}
	}
#endif
}

