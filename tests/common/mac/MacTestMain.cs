using System;
#if XAMCORE_2_0
using AppKit;
using Foundation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
#endif
using GuiUnit;
using NUnit.Framework;

namespace Xamarin.Mac.Tests
{	
	static class MainClass
	{
		static void Main(string[] args)
		{
			NSApplication.Init();
			NSRunLoop.Main.InvokeOnMainThread(RunTests);
			NSApplication.Main(args);
		}

		static void RunTests()
		{
			TestRunner.MainLoop = new NSRunLoopIntegration();
			string testName = System.Environment.GetEnvironmentVariable ("XM_TEST_NAME");
			string [] args = testName != null ?
				new [] { typeof(MainClass).Assembly.Location, "-labels", "-noheader", string.Format ("-test={0}", testName) } :
				new [] { typeof(MainClass).Assembly.Location, "-labels", "-noheader" };

			TestRunner.Main (args);
		}

		class NSRunLoopIntegration : NSObject, IMainLoopIntegration
		{
			public void InitializeToolkit()
			{
			}

			public void RunMainLoop()
			{
			}

			public void InvokeOnMainLoop(InvokerHelper helper)
			{
				NSApplication.SharedApplication.InvokeOnMainThread(helper.Invoke);
			}

			public void Shutdown()
			{
				Environment.Exit(TestRunner.ExitCode);
			}
		}
	}
}

partial class TestRuntime {
	public static bool RunAsync (DateTime timeout, Action action, Func<bool> check_completed)
	{
		NSTimer.CreateScheduledTimer (0.01, (v) => action ());
		do {
			if (timeout < DateTime.Now)
				return false;
			NSRunLoop.Main.RunUntil (NSDate.Now.AddSeconds (0.1));
		} while (!check_completed ());

		return true;
	}
}
