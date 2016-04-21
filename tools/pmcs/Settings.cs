// Settings.cs
//
// Author:
//   Aaron Bockover <abock@xamarin.com>
//
// Copyright 2013 Xamarin, Inc.

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using Mono.Options;

namespace Xamarin.Pmcs
{
	enum AssemblyTarget
	{
		Unspecified,
		Exe,
		Library
	}

	class Settings
	{
		OptionSet options;

		public Profile Profile { get; set; }

		public bool ShowHelp { get; set; }
		public bool Verbose { get; set; }
		public bool Time { get; set; }
		public bool CleanMode { get; set; }
		public bool KeepPreprocessedFiles { get; set; }
		public bool SkipCompilation { get; set; }
		public bool PreprocessInPlace { get; set; }
		public bool PreprocessToStdout { get; set; }
		public bool OmitLineDirective { get; set; }
		public bool SkipMdbRebase { get; set; }

		public AssemblyTarget AssemblyTarget { get; set; }
		public string AssemblyOutFile { get; set; }
		public bool HaveExplicitOutOption { get; set; }
		public List<string> CompilerPaths { get; private set; }

		public Settings ()
		{
			Profile = new Profile ();
			CompilerPaths = new List<string> ();
		}

		public void Usage (TextWriter writer, bool showLogo = true)
		{
			if (showLogo) {
				writer.WriteLine ("pmcs: preprocessing frontend to mcs");
				writer.WriteLine ();
			}

			writer.WriteLine ("Usage: pmcs [options] source-files");
			writer.WriteLine ("       pmcs clean");

			if (options == null)
				return;

			writer.WriteLine ();
			writer.WriteLine ("Available Options:");
			options.WriteOptionDescriptions (writer);
			writer.WriteLine ();
			writer.WriteLine ("PMCS_OPTIONS Environment Variable:");
			writer.WriteLine ("  Any options above can also be specified inside the PMCS_OPTIONS");
			writer.WriteLine ("  environment variable. Options should be separated by a semicolon:");
			writer.WriteLine ();
			writer.WriteLine ("    export PMCS_OPTIONS=\"-skip-mdb-rebase;-verbose\"");
			writer.WriteLine ();
			writer.WriteLine ("Clean Mode:");
			writer.WriteLine ("  When 'clean' is specified as the first non-option argument");
			writer.WriteLine ("  all options are ignored and any remaining preprocessed");
			writer.WriteLine ("  sources left on disk in the current directory are removed.");
			writer.WriteLine ("  This mode is useful when using the --keep option.");
			writer.WriteLine ();
		}

		static void ParseDefine (ReplacementCollection collection, string define, string optionName)
		{
			var split = define.Split (new [] { '=' }, 2);
			if (split.Length < 2)
				throw new OptionException ("invalid define: must be PATTERN=REPLACEMENT", optionName);

			collection.Add (split [0].Trim (), split [1].Trim ());
		}

		string[] ParseCommandLineArguments (string line)
		{
			var list = new List<string> ();
			var inquote = false;
			var builder = new StringBuilder ();
			for (int i = 0; i < line.Length; i++) {
				if (line [i] == '"') {
					inquote = !inquote;
				} else if (line [i] == '\\') {
					builder.Append (line [i + 1]);
					i++;
				} else if (!inquote && line [i] == ' ') {
					if (builder.Length > 0) {
						list.Add (builder.ToString ());
						builder.Length = 0;
					}
				} else {
					builder.Append (line [i]);
				}
			}
			if (builder.Length > 0)
				list.Add (builder.ToString ());
			return list.ToArray ();
		}

		void ParseExtraArgs (IEnumerable<string> args)
		{
			foreach (var arg in args)
				ParseExtraArg (arg);
		}

		void ParseExtraArg (string arg)
		{
			if (arg [0] == '-' || arg [0] == '/') {
				Profile.CompilerOptions.Add (arg);
			} else if (arg [0] == '@') {
				var responseFile = arg.Substring (1);
				if (!File.Exists (responseFile))
					throw new OptionException ("Unable to open response file:", responseFile);

				foreach (var argset in File.ReadLines (responseFile))
					ParseArguments (ParseCommandLineArguments (argset));
			} else {
				CompilerPaths.Add (arg);
			}
		}

		void ParseArguments (string [] args)
		{
			options = new OptionSet {
				{ "h|help", "show this help", v => ShowHelp = true },
				{ "rich-help", "open the rich help document in a web browser", v => RichHelp () },
				{ "v|verbose", "be verbose with output", v => Verbose = true },
				{ "time", "output timing data to stderr", v => Time = true },
				{ "ignore=", "do not preprocess VALUE", v => {
						if (!File.Exists (v))
							throw new OptionException ("file does not exist: " + v, "ignore");
						Profile.IgnorePaths.Add (v);
					} },
				{ "compiler=", "compiler binary to use after preprocessing", v => Profile.CompilerExecutable = v },
				{ "P|profile=", " VALUE", v => Profile.Load (v) },
				{ "global-replace=", "define a global replacement where VALUE=PATTERN=REPLACEMENT",
					v => ParseDefine (Profile.GlobalReplacements, v, "global-replace")
				},
				{ "enum-replace=", "define an enum backing type replacement where VALUE=PATTERN=REPLACEMENT",
					v => ParseDefine (Profile.EnumBackingTypeReplacements, v, "enum-replace")
				},
				{ "out=", "specifies output assembly name", v => {
						AssemblyOutFile = v;
						HaveExplicitOutOption = true;
						Profile.CompilerOptions.Add ("-out:" + v);
					} },
				{ "target=", "specifies the format of the output assembly", v => {
						switch (v.ToLower ()) {
						case "exe":
							AssemblyTarget = AssemblyTarget.Exe;
							break;
						case "library":
							AssemblyTarget = AssemblyTarget.Library;
							break;
						default:
							throw new OptionException ("Invalid target kind: " + v, "target");
						}
						Profile.CompilerOptions.Add ("-target:" + v);
					} },
				{ "omit-line-directive", "do not generate a #line directive at the start of files",
					v => OmitLineDirective = true
				},
				{ "skip-mdb-rebase", "do not rebase path names in .mdb symbol files (if -debug was specified)",
					v => SkipMdbRebase = true
				},
				{ "keep", "do not delete preprocessed intermediate files", v => KeepPreprocessedFiles = true },
				{ "skip-compile", "do not perform compilation (preprocess only)", v => SkipCompilation = true },
				{ "in-place", "preprocess in place instead of outputting intermediate " +
					"files (WARNING: this can be dangerous - only use this option if files " +
					"are under version control and do not have local changes)", v => PreprocessInPlace = true },
				{ "stdout", "write preprocessed output to stdout (implies -skip-compile " +
					"and no files on disk are modified)", v => PreprocessToStdout = true }
			};

			var extra = options.Parse (args);
			var option = Environment.GetEnvironmentVariable ("PMCS_OPTIONS");
			string [] pmcsEnvOptions = null;

			if (option != null)
				pmcsEnvOptions = option.Split(';');
			if (pmcsEnvOptions != null && pmcsEnvOptions.Length > 0)
				options.Parse (pmcsEnvOptions);

			if (extra.Count > 0 && extra [0] == "clean") {
				CleanMode = true;
				extra.RemoveAt (0);
				return;
			}

			ParseExtraArgs (extra);
		}

		public void Parse (string [] args)
		{
			Profile.CompilerExecutable = "mcs";

			ParseArguments (args);
			if (CleanMode)
				return;

			if (AssemblyOutFile == null && CompilerPaths.Count > 0) {
				AssemblyOutFile = Path.GetFileNameWithoutExtension (CompilerPaths [0]);
				if (AssemblyTarget == AssemblyTarget.Unspecified ||
					AssemblyTarget == AssemblyTarget.Exe) {
					AssemblyOutFile += ".exe";
				} else {
					AssemblyOutFile += ".dll";
				}
			}

			if (AssemblyOutFile != null) {
				switch (Path.GetExtension (AssemblyOutFile)) {
				case ".exe":
					AssemblyTarget = AssemblyTarget.Exe;
					break;
				case ".dll":
					AssemblyTarget = AssemblyTarget.Library;
					break;
				}
			}

			if (!HaveExplicitOutOption && !String.IsNullOrEmpty (AssemblyOutFile))
				Profile.CompilerOptions.Add ("-out:" + AssemblyOutFile);
		}

		void RichHelp ()
		{
			System.Diagnostics.Process.Start ("https://github.com/xamarin/maccore/blob/master/tools/pmcs/README.md");
			Environment.Exit (1);
		}

		public override string ToString ()
		{
			var builder = new StringBuilder ();
			builder.AppendFormat ("  Profile:\n");
			builder.Append (Profile);
			builder.AppendFormat ("  Show help:         {0}\n", ShowHelp);
			builder.AppendFormat ("  Verbose:           {0}\n", Verbose);
			builder.AppendFormat ("  Time:              {0}\n", Time);
			builder.AppendFormat ("  Clean mode:        {0}\n", CleanMode);
			builder.AppendFormat ("  Omit #line:        {0}\n", OmitLineDirective);
			builder.AppendFormat ("  Skip .mdb rebase:  {0}\n", SkipMdbRebase);
			builder.AppendFormat ("  Keep preprocessed: {0}\n", KeepPreprocessedFiles);
			builder.AppendFormat ("  Skip compilation:  {0}\n", SkipCompilation);
			builder.AppendFormat ("  In-place preproc:  {0}\n", PreprocessInPlace);
			builder.AppendFormat ("  Output to stdout:  {0}\n", PreprocessToStdout);
			builder.AppendFormat ("  Assembly out file: {0}\n", AssemblyOutFile);
			builder.AppendFormat ("  Assembly target:   {0}\n", AssemblyTarget.ToString ().ToLower ());
			builder.AppendFormat ("  Explicit -out:     {0}\n", HaveExplicitOutOption);
			builder.AppendFormat ("  Compiler paths:    {0}\n", String.Join (" ", CompilerPaths));
			return builder.ToString ();
		}
	}
}
