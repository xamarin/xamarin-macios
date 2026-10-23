// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Threading;

using AudioUnit;
using Foundation;

namespace MonotouchTest.AudioUnitExtensionHost {
	public class Program {
		const string logPrefix = "[monotouch-test-audio-unit-container]";

		static int Main (string [] args)
		{
			GC.KeepAlive (typeof (NSObject)); // prevent linking away the platform assembly

			if (ShouldRunExtensionTests (args))
				return RunExtensionTests ();

			Console.WriteLine (Environment.GetEnvironmentVariable ("MAGIC_WORD"));

			return args.Length;
		}

		static bool ShouldRunExtensionTests (string [] args)
		{
			if (Environment.GetEnvironmentVariable ("RUN_EXTENSION_TESTS") == "1")
				return true;

			foreach (var arg in args) {
				if (arg == "--run-extension-tests")
					return true;
			}

			return false;
		}

		static int RunExtensionTests ()
		{
			var desc = new AudioComponentDescription {
				ComponentType = AudioComponentType.Effect,
#if __MACCATALYST__
				ComponentSubType = (AudioUnitSubType) FourCC ("mttc"),
#else
				ComponentSubType = (AudioUnitSubType) FourCC ("mtts"),
#endif
				ComponentManufacturer = (AudioComponentManufacturerType) (uint) FourCC ("Xmrn"),
			};

			Console.WriteLine ($"{logPrefix} Instantiating audio unit extension: {desc}");

			NSError? error = null;
			AUAudioUnit? audioUnit = null;
			using var instantiated = new ManualResetEventSlim ();
			AUAudioUnit.FromComponentDescription (desc, AudioComponentInstantiationOptions.OutOfProcess, (au, err) => {
				audioUnit = au;
				error = err;
				instantiated.Set ();
			});

			while (!instantiated.IsSet)
				NSRunLoop.Current.RunUntil (NSDate.FromTimeIntervalSinceNow (0.25));

			if (error is not null || audioUnit is null || audioUnit.Handle == IntPtr.Zero) {
				Console.Error.WriteLine ($"{logPrefix} Failed to instantiate the audio unit extension: {error}");
				return 1;
			}

			using (audioUnit) {
#if __MACOS__ || __MACCATALYST__
			Console.WriteLine ($"{logPrefix} Loaded AudioUnit out-of-process: {!audioUnit.IsLoadedInProcess}");
#endif
				Console.WriteLine ($"{logPrefix} Holding the host open while the extension runs tests.");

				while (true)
					NSRunLoop.Current.RunUntil (NSDate.FromTimeIntervalSinceNow (0.25));
			}
		}

		static int FourCC (string value)
		{
			if (value.Length != 4)
				throw new ArgumentException ("A FourCC must be exactly four characters long.", nameof (value));

			return (value [0] << 24) | (value [1] << 16) | (value [2] << 8) | value [3];
		}
	}
}
