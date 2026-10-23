// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

using Foundation;
using TVServices;

namespace MonotouchTest.AudioUnitExtensionHost {
	public class Program {
		const string logPrefix = "[monotouch-test-top-shelf-container]";

		static int Main (string [] args)
		{
			GC.KeepAlive (typeof (NSObject)); // prevent linking away the platform assembly

			if (Environment.GetEnvironmentVariable ("RUN_EXTENSION_TESTS") == "1") {
				Console.WriteLine ($"{logPrefix} Requesting updated top-shelf content from the TV Services extension.");
				TVTopShelfContentProvider.DidChange ();
				return 0;
			}

			Console.WriteLine (Environment.GetEnvironmentVariable ("MAGIC_WORD"));
			return args.Length;
		}
	}
}
