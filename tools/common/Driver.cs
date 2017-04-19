/*
 * Copyright 2014 Xamarin Inc. All rights reserved.
 *
 * Authors:
 *   Rolf Bjarne Kvinge <rolf@xamarin.com>
 *
 */

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Utils;
using XamCore.ObjCRuntime;

namespace Xamarin.Bundler {
	public partial class Driver {
		static void AddSharedOptions (Application app, Mono.Options.OptionSet options)
		{
			options.Add ("warnaserror:", "An optional comma-separated list of warning codes that should be reported as errors (if no warnings are specified all warnings are reported as errors).", v =>
			{
				try {
					if (!string.IsNullOrEmpty (v)) {
						foreach (var code in v.Split (new char [] { ',' }, StringSplitOptions.RemoveEmptyEntries))
							ErrorHelper.SetWarningLevel (ErrorHelper.WarningLevel.Error, int.Parse (code));
					} else {
						ErrorHelper.SetWarningLevel (ErrorHelper.WarningLevel.Error);
					}
				} catch (Exception ex) {
					ErrorHelper.Error (26, ex, "Could not parse the command line argument '{0}': {1}", "--warnaserror", ex.Message);
				}
			});
			options.Add ("nowarn:", "An optional comma-separated list of warning codes to ignore (if no warnings are specified all warnings are ignored).", v =>
			{
				try {
					if (!string.IsNullOrEmpty (v)) {
						foreach (var code in v.Split (new char [] { ',' }, StringSplitOptions.RemoveEmptyEntries))
							ErrorHelper.SetWarningLevel (ErrorHelper.WarningLevel.Disable, int.Parse (code));
					} else {
						ErrorHelper.SetWarningLevel (ErrorHelper.WarningLevel.Disable);
					}
				} catch (Exception ex) {
					ErrorHelper.Error (26, ex, "Could not parse the command line argument '{0}': {1}", "--nowarn", ex.Message);
				}
			});
			options.Add ("coop:", "If the GC should run in cooperative mode.", v => { app.EnableCoopGC = ParseBool (v, "coop"); }, hidden: true);
			options.Add ("sgen-conc", "Enable the *experimental* concurrent garbage collector.", v => { app.EnableSGenConc = true; });
			options.Add ("marshal-objectivec-exceptions:", "Specify how Objective-C exceptions should be marshalled. Valid values: default, unwindmanagedcode, throwmanagedexception, abort and disable. The default depends on the target platform (on watchOS the default is 'throwmanagedexception', while on all other platforms it's 'disable').", v => {
				switch (v) {
				case "default":
					app.MarshalObjectiveCExceptions = MarshalObjectiveCExceptionMode.Default;
					break;
				case "unwindmanaged":
				case "unwindmanagedcode":
					app.MarshalObjectiveCExceptions = MarshalObjectiveCExceptionMode.UnwindManagedCode;
					break;
				case "throwmanaged":
				case "throwmanagedexception":
					app.MarshalObjectiveCExceptions = MarshalObjectiveCExceptionMode.ThrowManagedException;
					break;
				case "abort":
					app.MarshalObjectiveCExceptions = MarshalObjectiveCExceptionMode.Abort;
					break;
				case "disable":
					app.MarshalObjectiveCExceptions = MarshalObjectiveCExceptionMode.Disable;
					break;
				default:
					throw ErrorHelper.CreateError (26, "Could not parse the command line argument '{0}': {1}", "--marshal-objective-exceptions", $"Invalid value: {v}. Valid values are: default, unwindmanagedcode, throwmanagedexception, abort and disable.");
				}
			});
			options.Add ("marshal-managed-exceptions:", "Specify how managed exceptions should be marshalled. Valid values: default, unwindnativecode, throwobjectivecexception, abort and disable. The default depends on the target platform (on watchOS the default is 'throwobjectivecexception', while on all other platform it's 'disable').", v => {
				switch (v) {
				case "default":
					app.MarshalManagedExceptions = MarshalManagedExceptionMode.Default;
					break;
				case "unwindnative":
				case "unwindnativecode":
					app.MarshalManagedExceptions = MarshalManagedExceptionMode.UnwindNativeCode;
					break;
				case "throwobjectivec":
				case "throwobjectivecexception":
					app.MarshalManagedExceptions = MarshalManagedExceptionMode.ThrowObjectiveCException;
					break;
				case "abort":
					app.MarshalManagedExceptions = MarshalManagedExceptionMode.Abort;
					break;
				case "disable":
					app.MarshalManagedExceptions = MarshalManagedExceptionMode.Disable;
					break;
				default:
					throw ErrorHelper.CreateError (26, "Could not parse the command line argument '{0}': {1}", "--marshal-managed-exceptions", $"Invalid value: {v}. Valid values are: default, unwindnativecode, throwobjectivecexception, abort and disable.");
				}
			});
			options.Add ("j|jobs=", "The level of concurrency. Default is the number of processors.", v => {
				Jobs = int.Parse (v);
			});
			options.Add ("embeddinator", "Enables Embeddinator targetting mode.", v => {
				app.Embeddinator = true;
			}, true);
		}

		static int Jobs;
		public static int Concurrency {
			get {
				return Jobs == 0 ? Environment.ProcessorCount : Jobs;
			}
		}

#if MONOMAC
#pragma warning disable 0414
		static string userTargetFramework = TargetFramework.Default.ToString ();
#pragma warning restore 0414
#endif

		static TargetFramework? targetFramework;
		public static bool HasTargetFramework {
			get { return targetFramework.HasValue; }
		}

		public static TargetFramework TargetFramework {
			get { return targetFramework.Value; }
			set { targetFramework = value; }
		}

		static void SetTargetFramework (string fx)
		{
#if MONOMAC
			userTargetFramework = fx;
#endif

			switch (fx.Trim ().ToLowerInvariant ()) {
#if MONOMAC
			case "xammac":
			case "mobile":
			case "xamarin.mac":
				targetFramework = TargetFramework.Xamarin_Mac_2_0;
				break;
#endif
			default:
				TargetFramework parsedFramework;
				if (!Xamarin.Utils.TargetFramework.TryParse (fx, out parsedFramework))
					throw ErrorHelper.CreateError (68, "Invalid value for target framework: {0}.", fx);
#if MONOMAC
				if (parsedFramework == TargetFramework.Net_3_0 || parsedFramework == TargetFramework.Net_3_5)
					parsedFramework = TargetFramework.Net_2_0;
#endif

				targetFramework = parsedFramework;

				break;
			}

#if MTOUCH
			if (Array.IndexOf (TargetFramework.ValidFrameworks, targetFramework.Value) == -1)
				throw ErrorHelper.CreateError (70, "Invalid target framework: {0}. Valid target frameworks are: {1}.", targetFramework.Value, string.Join (" ", TargetFramework.ValidFrameworks.Select ((v) => v.ToString ()).ToArray ()));
#endif
		}

		public static int RunCommand (string path, string args, string[] env = null, StringBuilder output = null, bool suppressPrintOnErrors = false)
		{
			Exception stdin_exc = null;
			var info = new ProcessStartInfo (path, args);
			info.UseShellExecute = false;
			info.RedirectStandardInput = false;
			info.RedirectStandardOutput = true;
			info.RedirectStandardError = true;
			System.Threading.ManualResetEvent stdout_completed = new System.Threading.ManualResetEvent (false);
			System.Threading.ManualResetEvent stderr_completed = new System.Threading.ManualResetEvent (false);

			if (output == null)
				output = new StringBuilder ();

			if (env != null){
				if (env.Length % 2 != 0)
					throw new Exception ("You passed an environment key without a value");

				for (int i = 0; i < env.Length; i+= 2)
					info.EnvironmentVariables [env[i]] = env[i+1];
			}

			if (verbose > 0)
				Console.WriteLine ("{0} {1}", path, args);

			using (var p = Process.Start (info)) {

				p.OutputDataReceived += (s, e) => {
					if (e.Data != null) {
						lock (output)
							output.AppendLine (e.Data);
					} else {
						stdout_completed.Set ();
					}
				};

				p.ErrorDataReceived += (s, e) => {
					if (e.Data != null) {
						lock (output)
							output.AppendLine (e.Data);
					} else {
						stderr_completed.Set ();
					}
				};

				p.BeginOutputReadLine ();
				p.BeginErrorReadLine ();

				p.WaitForExit ();

				stderr_completed.WaitOne (TimeSpan.FromSeconds (1));
				stdout_completed.WaitOne (TimeSpan.FromSeconds (1));

				if (p.ExitCode != 0) {
					// note: this repeat the failing command line. However we can't avoid this since we're often
					// running commands in parallel (so the last one printed might not be the one failing)
					if (!suppressPrintOnErrors)
						Console.Error.WriteLine ("Process exited with code {0}, command:\n{1} {2}{3}", p.ExitCode, path, args, output.Length > 0 ? "\n" + output.ToString () : string.Empty);
					return p.ExitCode;
				} else if (verbose > 0 && output.Length > 0 && !suppressPrintOnErrors) {
					Console.WriteLine (output.ToString ());
				}

				if (stdin_exc != null)
					throw stdin_exc;
			}

			return 0;
		}

		public static Task<int> RunCommandAsync (string path, string args, string [] env = null, StringBuilder output = null, bool suppressPrintOnErrors = false)
		{
			return Task.Run (() => RunCommand (path, args, env, output, suppressPrintOnErrors));
		}

#if !MMP_TEST
		static void FileMove (string source, string target)
		{
			Application.TryDelete (target);
			File.Move (source, target);
		}

		static void MoveIfDifferent (string path, string tmp)
		{
			// Don't read the entire file into memory, it can be quite big in certain cases.

			bool move = false;

			using (var fs1 = new FileStream (path, FileMode.Open, FileAccess.Read)) {
				using (var fs2 = new FileStream (tmp, FileMode.Open, FileAccess.Read)) {
					if (fs1.Length != fs2.Length) {
						Log (3, "New file '{0}' has different length, writing new file.", path);
						move = true;
					} else {
						move = !Cache.CompareStreams (fs1, fs2);
					}
				}
			}

			if (move) {
				FileMove (tmp, path);
			} else {
				Log (3, "Target {0} is up-to-date.", path);
			}
		}

		public static void WriteIfDifferent (string path, string contents)
		{
			var tmp = path + ".tmp";

			try {
				if (!File.Exists (path)) {
					Directory.CreateDirectory (Path.GetDirectoryName (path));
					File.WriteAllText (path, contents);
					Log (3, "File '{0}' does not exist, creating it.", path);
					return;
				}

				File.WriteAllText (tmp, contents);
				MoveIfDifferent (path, tmp);
			} catch (Exception e) {
				File.WriteAllText (path, contents);
				ErrorHelper.Warning (1014, e, "Failed to re-use cached version of '{0}': {1}.", path, e.Message);
			} finally {
				Application.TryDelete (tmp);
			}
		}

		public static void WriteIfDifferent (string path, byte[] contents)
		{
			var tmp = path + ".tmp";

			try {
				if (!File.Exists (path)) {
					File.WriteAllBytes (path, contents);
					Log (3, "File '{0}' does not exist, creating it.", path);
					return;
				}

				File.WriteAllBytes (tmp, contents);
				MoveIfDifferent (path, tmp);
			} catch (Exception e) {
				File.WriteAllBytes (path, contents);
				ErrorHelper.Warning (1014, e, "Failed to re-use cached version of '{0}': {1}.", path, e.Message);
			} finally {
				Application.TryDelete (tmp);
			}
		}
#endif


		internal static string GetFullPath ()
		{
			return System.Reflection.Assembly.GetExecutingAssembly ().Location;
		}

		static Version xcode_version;
		public static Version XcodeVersion {
			get {
				return xcode_version;
			}
		}

	}
}
