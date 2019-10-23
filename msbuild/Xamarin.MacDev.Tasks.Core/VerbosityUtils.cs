// Copyright (c) Microsoft Corp

using System;
using Microsoft.Build.Framework;

namespace Xamarin.MacDev.Tasks {

	// Helper code for Task delegating work to external tools but that still
	// needs to be verbosity-aware
	public static class VerbosityUtils {

		// verbosity can be set in multiple ways
		// this makes it a consistent interpretation of them
		static public string Merge (string extraArguments, LoggerVerbosity taskVerbosity)
		{
			string result = String.Empty;
			var empty_extra = String.IsNullOrEmpty (extraArguments);
			// We give the priority to the extra arguments given to the tools
  			if (empty_extra || (!empty_extra && !extraArguments.Contains ("-q") && !extraArguments.Contains ("-v"))) {
				// if nothing is specified fall back to msbuild settings
				// first check if some were supplied on the command-line
				result = GetVerbosityLevel (Environment.CommandLine);
				// if not then use the default from the msbuild config files, which Visual Studio for Mac can override (to match the IDE setting for msbuild)
				if (result.Length == 0)
					result = GetVerbosityLevel (taskVerbosity);
			}
			return result;
		}

		//
		// This is an hack, since there can be multiple loggers.
		// However it's the most common use case and `mtouch` logs
		// are often the most important to gather and developers expect
		// a single change (in verbosity) to do the job and be consistent in CI.
		//
		// msbuild argument format
		//	-verbosity:<level> Display this amount of information in the event log.
		//		The available verbosity levels are: q[uiet], m[inimal],
		//		n[ormal], d[etailed], and diag[nostic]. (Short form: -v)
		//
		static public string GetVerbosityLevel (string commandLine)
		{
			var verbosity = String.Empty;
			char sep = ' ';
			// first chance: short form, case sensitive
			// note: `-v` (without arguments) is not a valid option
			var p = commandLine.IndexOf ("v:", StringComparison.Ordinal);
			if (p > 0) {
				sep = commandLine[p - 1];
				p += 2; // skip `v:`
			} else {
				// second change: long form, case sensitive
				p = commandLine.IndexOf ("verbosity:", StringComparison.Ordinal);
				if (p > 0) {
					sep = commandLine[p - 1];
					p += 10;
				}
			}
			// check separator, e.g. `-approv:`
			if ((p > 0) && (sep == '-' || sep == '/')) {
				// might (or not) be the last argument provided
				var end = commandLine.IndexOf (' ', p);
				verbosity = end == -1 ? commandLine.Substring (p) : commandLine.Substring (p, end - p);
			}

			// case sensitive
			switch (verbosity) {
			case "q":
			case "quiet":
				return GetVerbosityLevel (LoggerVerbosity.Quiet);
			case "m":
			case "minimal":
				return GetVerbosityLevel (LoggerVerbosity.Minimal);
			case "n":
			case "normal":
			default:
				return GetVerbosityLevel (LoggerVerbosity.Normal);
			case "d":
			case "detailed":
				return GetVerbosityLevel (LoggerVerbosity.Detailed);
			case "diag":
			case "diagnostic":
				return GetVerbosityLevel (LoggerVerbosity.Diagnostic);
			}
		}

		// The values here come from: https://github.com/mono/monodevelop/blob/143f9b6617123a0841a5cc5a2a4e13b309535792/main/src/core/MonoDevelop.Projects.Formats.MSBuild/MonoDevelop.Projects.MSBuild.Shared/RemoteBuildEngineMessages.cs#L186
		// Assume 'Normal (2)' is the default verbosity (no change), and the other values follow from there.
		static public string GetVerbosityLevel (LoggerVerbosity v)
		{
			switch ((LoggerVerbosity) v) {
			case LoggerVerbosity.Quiet:
				return "-q -q -q -q";
			case LoggerVerbosity.Minimal:
				return "-q -q";
			case LoggerVerbosity.Normal:
			default:
				return String.Empty;
			case LoggerVerbosity.Detailed:
				return "-v -v";
			case LoggerVerbosity.Diagnostic:
				return "-v -v -v -v";
			}
		}
	}
}
