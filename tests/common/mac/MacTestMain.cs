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
			TestRunner.Main(new[] {
				typeof(MainClass).Assembly.Location,
				"-labels",
				"-noheader"
			});
		}

		class NSRunLoopIntegration : NSObject, IMainLoopIntegration
		{
			public void InitializeToolkit ()
			{
			}

			public void RunMainLoop ()
			{
			}
			 
			public void InvokeOnMainLoop (InvokerHelper helper)
			{
				NSApplication.SharedApplication.InvokeOnMainThread (helper.Invoke);
			}

			public void Shutdown ()
			{
				Environment.Exit (TestRunner.ExitCode);
			}
		}
	}
}
