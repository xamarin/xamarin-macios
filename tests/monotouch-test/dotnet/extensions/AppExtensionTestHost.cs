// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

using Foundation;
using MonoTouch.NUnit.UI;
using ObjCRuntime;

namespace MonotouchTest.AudioUnitExtensionHost {
	static class ExtensionTestHost {
		const string logPrefix = "[monotouch-test-app-extension]";
		static bool hasRun;
		static bool debugHooksInstalled;
		static readonly object guard = new object ();

		// xamarin_log is a non-variadic native function in libxamarin
		// that calls NSLog internally. We use this instead of P/Invoking
		// NSLog directly, because NSLog is variadic, and P/Invoke doesn't
		// handle variadic functions correctly on ARM64.
		[DllImport ("__Internal")]
		static extern void xamarin_log (IntPtr unicodeMessage);

		[DllImport ("/usr/lib/libSystem.B.dylib")]
		static extern unsafe int atexit (delegate* unmanaged<void> callback);

		static void Log (string message)
		{
			Console.WriteLine (message);
			var logMessage = message;
			unsafe {
				fixed (char* ptr = logMessage)
					xamarin_log ((IntPtr) ptr);
			}
		}

		static void SafeLog (string message)
		{
			try {
				Log (message);
			} catch (Exception ex) {
				Console.WriteLine ($"{message}{Environment.NewLine}{ex}");
			}
		}

		public static unsafe void InstallDebugHooks ()
		{
			lock (guard) {
				if (debugHooksInstalled)
					return;
				debugHooksInstalled = true;
			}

			Runtime.MarshalObjectiveCException += ObjectiveCExceptionMarshaled;
			Runtime.MarshalManagedException += ManagedExceptionMarshaled;

			var rv = atexit (&AtExitCallback);
			SafeLog ($"{logPrefix} Installed debug hooks (atexit registration result: {rv}).");
		}

		static void ObjectiveCExceptionMarshaled (object? sender, MarshalObjectiveCExceptionEventArgs args)
		{
			var stackTrace = args.Exception.CallStackSymbols is null
				? new StackTrace (1, true).ToString ()
				: string.Join (Environment.NewLine, args.Exception.CallStackSymbols);
			SafeLog ($"{logPrefix} Objective-C exception marshaled. Mode: {args.ExceptionMode}. Name: {args.Exception.Name}. Reason: {args.Exception.Reason}{Environment.NewLine}{stackTrace}");
		}

		static void ManagedExceptionMarshaled (object? sender, MarshalManagedExceptionEventArgs args)
		{
			SafeLog ($"{logPrefix} Managed exception marshaled. Mode: {args.ExceptionMode}.{Environment.NewLine}{args.Exception}");
		}

		[UnmanagedCallersOnly]
		static void AtExitCallback ()
		{
			SafeLog ($"{logPrefix} Process is exiting.{Environment.NewLine}{new StackTrace (1, true)}");
		}

		static string? GetTestName ()
		{
			var testName = NSUserDefaults.StandardUserDefaults.StringForKey ("test.name");
			if (string.IsNullOrWhiteSpace (testName))
				return null;
			SafeLog ($"{logPrefix} Using test filter from NSUserDefaults: {testName}");
			return testName;
		}

		public static async Task RunOnce ()
		{
			lock (guard) {
				if (hasRun)
					return;
				hasRun = true;
			}

			var testName = GetTestName ();
			var runner = ExtensionTestRunner.CreateHeadlessRunner (TestLoader.GetTestAssemblies (), testName, Log);
			runner.LogCallback = Log;

			var runDescription = string.IsNullOrEmpty (testName) ? "all monotouch-test tests" : testName;
			Log ($"{logPrefix} Starting monotouch-test app extension test run ({runDescription})");
			try {
				await ExtensionTestRunner.RunAsync (runner);
				Log ($"{logPrefix} Finished monotouch-test app extension test run. Passed: {runner.PassedCount} Failed: {runner.FailedCount} Ignored: {runner.IgnoredCount} Inconclusive: {runner.InconclusiveCount}");
			} catch (Exception ex) {
				Log ($"{logPrefix} Extension test run failed: {ex}");
			}
		}
	}
}
