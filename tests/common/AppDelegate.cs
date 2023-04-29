#if !__WATCHOS__
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

using ObjCRuntime;
using Foundation;
#if !__MACOS__
using UIKit;
#endif

using MonoTouch.NUnit.UI;
using NUnit.Framework;
using NUnit.Framework.Internal;

[Register ("AppDelegate")]
public partial class AppDelegate : UIApplicationDelegate {
	public static TouchRunner Runner { get; set; }

#if !__MACOS__
	public override UIWindow Window { get; set; }
#endif

	partial void PostFinishedLaunching ();

	public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
	{
#if __MACCATALYST__ || __MACOS__
		TestRuntime.NotifyLaunchCompleted ();
#endif
		var window = new UIWindow (UIScreen.MainScreen.Bounds);

		var runner = new TouchRunner (window);
		foreach (var assembly in TestLoader.GetTestAssemblies ())
			runner.Add (assembly);

		Window = window;
		Runner = runner;

		window.RootViewController = new UINavigationController (runner.GetViewController ());
		window.MakeKeyAndVisible ();

		PostFinishedLaunching ();

		return true;
	}
}

public static class MainClass {
	static void Main (string [] args)
	{
#if __MACCATALYST__
		NativeLibrary.SetDllImportResolver (typeof (NSObject).Assembly, DllImportResolver);
		NativeLibrary.SetDllImportResolver (typeof (MainClass).Assembly, DllImportResolver);
#endif
#if !__MACOS__
		UIApplication.Main (args, null, typeof (AppDelegate));
#endif
	}

#if __MACCATALYST__
	// This is a workaround for a temporary issue in the .NET runtime
	// See https://github.com/xamarin/maccore/issues/2668
	// The issue is present in .NET 7.0.5, and will likely be fixed in .NET 7.0.6.
	static IntPtr DllImportResolver (string libraryName, global::System.Reflection.Assembly assembly, DllImportSearchPath? searchPath)
	{
		switch (libraryName) {
		case "/System/Library/Frameworks/SceneKit.framework/SceneKit":
		case "/System/Library/Frameworks/SceneKit.framework/Versions/A/SceneKit":
			var rv = NativeLibrary.Load (libraryName);
			Console.WriteLine ($"DllImportResolver callback loaded library \"{libraryName}\" from a P/Invoke in \"{assembly}\" => 0x{rv.ToString ("x")}");
			return rv;
		default:
			return IntPtr.Zero;
		}
	}
#endif
}
#endif // !__WATCHOS__
