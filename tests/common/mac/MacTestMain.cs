using System;
using System.Collections.Generic;
#if XAMCORE_2_0
using AppKit;
using Foundation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
#endif
using GuiUnit;
using NUnit.Framework;

// This is a bit of a hack. A number of BCL types have using statements with this namespace but no uses that we care about
namespace NUnit.Framework.SyntaxHelpers
{
}

namespace Xamarin.Mac.Tests
{
	static class MainClass
	{
		static void Main (string[] args)
		{
#if !NO_GUI_TESTING
			NSApplication.Init();
#endif
			RunTests ();
		}

		static void RunTests ()
		{
			TestRunner.MainLoop = new NSRunLoopIntegration ();
			List<string> args = new List<string> () { typeof (MainClass).Assembly.Location, "-labels", "-noheader" };

			string testName = System.Environment.GetEnvironmentVariable ("XM_TEST_NAME");
			if (testName != null)
				args.Add ($"-test={testName}");

			if (System.Environment.GetEnvironmentVariable ("XM_BCL_TEST") != null)
				args.Add ("-exclude=MobileNotWorking,NotOnMac,NotWorking,ValueAdd,CAS,InetAccess,NotWorkingInterpreter");
			TestRunner.Main (args.ToArray ());
#if NO_GUI_TESTING
			// HACK - TestRunner.Main assumes you have a message pump spinning, but when I hack it out via NO_GUI_TESTING it returns right away
			// We will exit via Environment.Exit in NSRunLoopIntegration
			while (true) {
				System.Threading.Thread.Sleep (1000);
			}
#endif
		}

#if !NO_GUI_TESTING
		class NSRunLoopIntegration : NSObject, IMainLoopIntegration
		{
			public void InitializeToolkit ()
			{
			}

			public void RunMainLoop ()
			{
				NSApplication.SharedApplication.Run ();
			}

			public void InvokeOnMainLoop (InvokerHelper helper)
			{
				NSApplication.SharedApplication.InvokeOnMainThread(helper.Invoke);
			}

			public void Shutdown ()
			{
				Environment.Exit (TestRunner.ExitCode);
			}
		}
#else
		class NSRunLoopIntegration : IMainLoopIntegration
		{
			public void InitializeToolkit ()
			{
			}

			public void RunMainLoop ()
			{
			}

			public void InvokeOnMainLoop (InvokerHelper helper)
			{
				helper.Invoke ();
			}

			public void Shutdown ()
			{
				Environment.Exit (TestRunner.ExitCode);
			}
		}
#endif
	}
}
