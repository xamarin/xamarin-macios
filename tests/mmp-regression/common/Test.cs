// Copyright 2012 Xamarin Inc. All rights reserved.

using System;
using System.IO;
using System.Reflection;

using AppKit;

using Foundation;

namespace Xamarin.Mac.Linker.Test {

	public static class Test {

		static TextWriter log;

		public static TextWriter Log {
			get {
				// if not defined the results will be inside Console.app
				if (log is null) {
					string logfile = Environment.GetEnvironmentVariable ("TEST_LOG_FILE");
					if (String.IsNullOrEmpty (logfile))
						log = Console.Out;
					else
						log = new StreamWriter (logfile, true);
				}
				return log;
			}
		}

		static string linker_removed_type = "CoreImage.CIColor, Xamarin.Mac";

		public static void EnsureLinker (bool enabled)
		{

			if ((Type.GetType (linker_removed_type) is not null) == enabled) {
				Log.WriteLine ("[FAIL]\tThe linker was {0}enabled on this build", enabled ? "not " : String.Empty);
			}
		}

		public static void Terminate ()
		{
			Log.Flush ();
			NSApplication.SharedApplication.Terminate (new NSObject ());
		}
	}
}
