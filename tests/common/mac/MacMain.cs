using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Xamarin.Mac.Tests {
	static class MainClass {
		static async Task<int> Main (string [] args)
		{
			// Skip arguments added by VSfM/macOS when running from the IDE
			var arguments = new List<string> (args);
			arguments.RemoveAll ((arg) => arg.StartsWith ("-psn_", StringComparison.Ordinal));

			var exit_code = await MonoTouch.NUnit.UI.MacRunner.MainAsync (arguments, true, _exit, typeof (MainClass).Assembly);
			_exit (exit_code);
			return exit_code;
		}

		[DllImport ("/usr/lib/libSystem.dylib")]
		static extern void _exit (int exit_code);
	}
}
