using System;
using System.Collections.Generic;
using System.Reflection;

using Foundation;
#if !__MACOS__
using UIKit;
#endif

using MonoTouch.NUnit.UI;
using NUnit.Framework;
using NUnit.Framework.Internal;

#if __MACOS__
namespace Xamarin.Mac.Tests {
	public static partial class TestLoader {
		static partial void AddTestAssembliesImpl (List<Assembly> assemblies)
		{
			assemblies.Add (typeof (Xamarin.BindingTests2.BindingTest).Assembly);
			assemblies.Add (typeof (Xamarin.BindingTests.ProtocolTest).Assembly);
		}
	}
}
#elif !__WATCHOS__
[Register ("AppDelegate")]
public partial class AppDelegate : UIApplicationDelegate
{
	UIWindow window;
	TouchRunner runner;

	public override bool FinishedLaunching (UIApplication app, NSDictionary options)
	{
#if __MACCATALYST__
		// Debug spew to track down https://github.com/xamarin/maccore/issues/2414
		Console.WriteLine ("AppDelegate.FinishedLaunching");
#endif
		window = new UIWindow (UIScreen.MainScreen.Bounds);

		runner = new TouchRunner (window);
		runner.Add (System.Reflection.Assembly.GetExecutingAssembly ());
		runner.Add (typeof (Xamarin.BindingTests2.BindingTest).Assembly);
		runner.Add (typeof (Xamarin.BindingTests.ProtocolTest).Assembly);

		window.RootViewController = new UINavigationController (runner.GetViewController ());
		window.MakeKeyAndVisible ();

		return true;
	}

	static void Main (string[] args)
	{
		UIApplication.Main (args, null, typeof (AppDelegate));
	}
}
#else
public static partial class TestLoader {
	static partial void AddTestAssembliesImpl (BaseTouchRunner runner)
	{
		runner.Add (typeof (Xamarin.BindingTests2.BindingTest).Assembly);
		runner.Add (typeof (Xamarin.BindingTests.ProtocolTest).Assembly);
	}
}

#endif // !__WATCHOS__

// In some cases NUnit fails if asked to run tests from an assembly that doesn't have any tests. So add a dummy test here to not fail in that scenario.
[TestFixture]
public class DummyTest
{
	public void TestMe ()
	{
		Assert.True (true, "YAY!");
	}
}
