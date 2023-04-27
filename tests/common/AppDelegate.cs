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
#if NET
		NativeLibrary.SetDllImportResolver (typeof (NSObject).Assembly, DllImportResolver);
		NativeLibrary.SetDllImportResolver (typeof (MainClass).Assembly, DllImportResolver);
#endif
#if !__MACOS__
		UIApplication.Main (args, null, typeof (AppDelegate));
#endif
	}

#if NET
	static IntPtr DllImportResolver (string libraryName, global::System.Reflection.Assembly assembly, DllImportSearchPath? searchPath)
	{
		switch (libraryName) {
		case "/System/Library/Frameworks/SceneKit.framework/SceneKit":
		case "/System/Library/Frameworks/SceneKit.framework/Versions/A/SceneKit":
			var rv = NativeLibrary.Load (libraryName);
			NSLog ($"DllImportResolver ({libraryName}, {assembly}, {searchPath}) => 0x{rv.ToString ("x")}");
			return rv;
		default:
			return IntPtr.Zero;
		}
	}

	[DllImport ("__Internal")]
	extern static void xamarin_log (IntPtr s);

	[DllImport (Constants.libcLibrary)]
	extern static nint write (int filedes, byte [] buf, nint nbyte);

	internal static void NSLog (string value)
	{
		try {
			xamarin_log (Marshal.StringToHGlobalUni (value));
		} catch {
			// Append a newline like NSLog does
			if (!value.EndsWith ('\n'))
				value += "\n";
			// Don't use Console.WriteLine, since that brings in a lot of supporting code and may bloat apps.
			var utf8 = Encoding.UTF8.GetBytes (value);
			write (2 /* STDERR */, utf8, utf8.Length);
			// Ignore any errors writing to stderr (might happen on devices if the developer tools haven't been mounted, but xamarin_log should always work on devices).
		}
	}
#endif
}
#endif // !__WATCHOS__
