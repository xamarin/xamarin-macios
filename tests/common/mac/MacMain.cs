using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Xamarin.Mac.Tests {
	static class MainClass {
		static async Task<int> Main (string [] args)
		{
			// Skip arguments added by VSfM/macOS when running from the IDE
			var arguments = new List<string> (args);
			arguments.RemoveAll ((arg) => arg.StartsWith ("-psn_", StringComparison.Ordinal));

			var assemblies = new List<Assembly> ();
			assemblies.Add (typeof (MainClass).Assembly);
			TestLoader.AddTestAssemblies (assemblies);

			var exit_code = await MonoTouch.NUnit.UI.MacRunner.MainAsync (arguments, true, _exit, assemblies.ToArray ());

#if NET
			var exit_monitor = new Thread (() =>
			{
				// Wait for 3 seconds
				Thread.Sleep (3000);
				// If we're still here, then something went wrong. Let's exit.
				Console.WriteLine ($"The process didn't exit within 3s of returning from Main. Assuming something is deadlocked, and will now exit immediately and forcefully (with exit code {exit_code}).");
				Console.Out.Flush ();
				Console.Err.Flush ();
				_exit (exit_code);
			}) {
				Name = "Exit monitor",
				IsBackground = true,
			};
			exit_monitor.Start ();
#else
			Console.Out.Flush ();
			Console.Err.Flush ();
			_exit (exit_code);
#endif

			return exit_code;
		}

		[DllImport ("/usr/lib/libSystem.dylib")]
		static extern void _exit (int exit_code);
	}

	public static partial class TestLoader
	{
		static partial void AddTestAssembliesImpl (List<Assembly> assemblies);

		public static void AddTestAssemblies (List<Assembly> assemblies)
		{
			AddTestAssembliesImpl (assemblies);
		}
	}
}
