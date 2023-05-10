/*
 * Copyright 2014 Xamarin Inc. All rights reserved.
 * Copyright 2019 Microsoft Corp. All rights reserved.
 *
 * Authors:
 *   Rolf Bjarne Kvinge <rolf@xamarin.com>
 *
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Utils;

namespace Xamarin.Bundler {
	public partial class Driver {
		public static int RunCommand (string path, IList<string> args, Dictionary<string, string> env, out StringBuilder output, bool suppressPrintOnErrors, int verbose)
		{
			output = new StringBuilder ();
			return RunCommand (path, args, env, output, suppressPrintOnErrors, verbose);
		}

		public static int RunCommand (string path, IList<string> args, Dictionary<string, string> env, out StringBuilder output, bool suppressPrintOnErrors)
		{
			output = new StringBuilder ();
			return RunCommand (path, args, env, output, suppressPrintOnErrors, Verbosity);
		}

		public static int RunCommand (string path, IList<string> args, Dictionary<string, string> env, StringBuilder output, bool suppressPrintOnErrors, int verbosity)
		{
			return RunCommand (path, args, env, output, output, suppressPrintOnErrors, verbosity);
		}

		public static int RunCommand (string path, IList<string> args, Dictionary<string, string> env, StringBuilder output, bool suppressPrintOnErrors = false)
		{
			return RunCommand (path, args, env, output, output, suppressPrintOnErrors, Verbosity);
		}

		public static int RunCommand (string path, params string [] args)
		{
			return RunCommand (path, args, null, (Action<string>) null, (Action<string>) null, false, Verbosity);
		}

		public static int RunCommand (string path, IList<string> args)
		{
			return RunCommand (path, args, null, (Action<string>) null, (Action<string>) null, false, Verbosity);
		}

		public static int RunCommand (string path, IList<string> args, StringBuilder output)
		{
			return RunCommand (path, args, null, output, output, false, Verbosity);
		}

		public static int RunCommand (string path, IList<string> args, StringBuilder output, bool suppressPrintOnErrors)
		{
			return RunCommand (path, args, null, output, output, suppressPrintOnErrors, Verbosity);
		}

		public static int RunCommand (string path, IList<string> args, StringBuilder output, StringBuilder error)
		{
			return RunCommand (path, args, null, output, error, false, Verbosity);
		}

		public static int RunCommand (string path, IList<string> args, StringBuilder output, StringBuilder error, bool suppressPrintOnErrors)
		{
			return RunCommand (path, args, null, output, error, suppressPrintOnErrors, Verbosity);
		}

		public static int RunCommand (string path, IList<string> args, Dictionary<string, string> env, StringBuilder output, StringBuilder error, bool suppressPrintOnErrors, int verbosity)
		{
			var output_received = output is null ? null : new Action<string> ((v) => { if (v is not null) output.AppendLine (v); });
			var error_received = error is null ? null : new Action<string> ((v) => { if (v is not null) error.AppendLine (v); });
			return RunCommand (path, args, env, output_received, error_received, suppressPrintOnErrors, verbosity);
		}

		static int RunCommand (string path, IList<string> args, Dictionary<string, string> env, Action<string> output_received, bool suppressPrintOnErrors)
		{
			return RunCommand (path, args, env, output_received, output_received, suppressPrintOnErrors, Verbosity);
		}

		static int RunCommand (string path, IList<string> args, Dictionary<string, string> env, Action<string> output_received, Action<string> error_received)
		{
			return RunCommand (path, args, env, output_received, error_received, false, Verbosity);
		}

		static int RunCommand (string path, IList<string> args, Dictionary<string, string> env, Action<string> output_received, Action<string> error_received, bool suppressPrintOnErrors, int verbosity)
		{
			var output = new StringBuilder ();
			var outputCallback = new Action<string> ((string line) => {
				if (output_received is not null)
					output_received (line);
				lock (output)
					output.AppendLine (line);
			});
			var errorCallback = new Action<string> ((string line) => {
				if (error_received is not null)
					error_received (line);
				lock (output)
					output.AppendLine (line);
			});

			if (verbosity > 0)
				Console.WriteLine ($"{path} {StringUtils.FormatArguments (args)}");

			var p = Execution.RunWithCallbacksAsync (path, args, env, outputCallback, errorCallback).Result;

			if (output_received is not null)
				output_received (null);
			if (error_received is not null)
				error_received (null);

			if (p.ExitCode != 0) {
				// note: this repeats the failing command line. However we can't avoid this since we're often
				// running commands in parallel (so the last one printed might not be the one failing)
				if (!suppressPrintOnErrors) {
					// We re-use the stringbuilder so that we avoid duplicating the amount of required memory,
					// while only calling Console.WriteLine once to make it less probable that other threads
					// also write to the Console, confusing the output.
					output.Insert (0, $"Process exited with code {p.ExitCode}, command:\n{path} {StringUtils.FormatArguments (args)}\n");
					Console.Error.WriteLine (output);
				}
				return p.ExitCode;
			} else if (verbosity > 0 && output?.Length > 0 && !suppressPrintOnErrors) {
				Console.WriteLine (output.ToString ());
			}

			return p.ExitCode;
		}

		public static Task<int> RunCommandAsync (string path, IList<string> args, Dictionary<string, string> env = null, Action<string> output_received = null, bool suppressPrintOnErrors = false, int? verbosity = null)
		{
			return Task.Run (() => RunCommand (path, args, env, output_received, output_received, suppressPrintOnErrors, verbosity ?? Verbosity));
		}

		public static Task<int> RunCommandAsync (string path, IList<string> args, Dictionary<string, string> env = null, StringBuilder output = null, bool suppressPrintOnErrors = false, int? verbosity = null)
		{
			return Task.Run (() => RunCommand (path, args, env, output, output, suppressPrintOnErrors, verbosity ?? Verbosity));
		}

#if BGENERATOR
		internal static int Verbosity => ErrorHelper.Verbosity;
#elif !MTOUCH && !MMP && !BUNDLER
		internal static int Verbosity;
#endif
	}
}
