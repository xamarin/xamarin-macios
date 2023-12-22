using System;
using System.Collections.Generic;

using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;

using Xamarin.Localization.MSBuild;
using Xamarin.Utils;

namespace Xamarin.MacDev.Tasks {
	public abstract class ParseBundlerArgumentsTaskBase : XamarinTask {
		public string ExtraArgs { get; set; }

		[Output]
		public ITaskItem [] Aot { get; set; }

		[Output]
		public ITaskItem [] DlSym { get; set; }

		[Output]
		public ITaskItem [] EnvironmentVariables { get; set; }

		[Output]
		public string MarshalManagedExceptionMode { get; set; }

		[Output]
		public string MarshalObjectiveCExceptionMode { get; set; }

		[Output]
		public string CustomBundleName { get; set; }

		[Output]
		public ITaskItem [] CustomLinkFlags { get; set; }

		[Output]
		public string NoSymbolStrip { get; set; }

		[Output]
		public string NoDSymUtil { get; set; }

		[Output]
		public string Optimize { get; set; }

		[Output]
		public string PackageDebugSymbols { get; set; }

		[Output]
		public string Registrar { get; set; }

		[Output]
		public string RequirePInvokeWrappers { get; set; }

		// This is input too
		[Output]
		public string NoStrip { get; set; }

		[Output]
		public string SkipMarkingNSObjectsInUserAssemblies { get; set; }

		[Output]
		public int Verbosity { get; set; }

		[Output]
		public ITaskItem [] XmlDefinitions { get; set; }

		public override bool Execute ()
		{
			if (string.IsNullOrEmpty (NoSymbolStrip))
				NoSymbolStrip = "false";

			if (string.IsNullOrEmpty (NoDSymUtil))
				NoDSymUtil = "false";

			if (!string.IsNullOrEmpty (ExtraArgs)) {
				var extraArgs = ExtraArgs;
				if (Environment.OSVersion.Platform == PlatformID.Win32NT) {
					// The backslash character is pretty common on Windows (since it's a path separator),
					// but the argument parser will treat it as an escape character, and just skip it.
					// This is obviously wrong, so just replace backslashes with forward slashes, which
					// should work just as well on Windows, and will also be parsed correctly.
					// The downside is that now there's no way to escape characters, but that should be a
					// very rare problem (much rarer than backslash characters), and there are other
					// ways around the problem (set the actual target property instead of going through
					// the MtouchExtraArgs property for instance):
					extraArgs = extraArgs.Replace ('\\', '/');
				}
				var args = CommandLineArgumentBuilder.Parse (extraArgs);
				List<string> xml = null;
				List<string> customLinkFlags = null;
				var aot = new List<ITaskItem> ();
				var envVariables = new List<ITaskItem> ();
				var dlsyms = new List<ITaskItem> ();

				for (int i = 0; i < args.Length; i++) {
					var arg = args [i];

					if (string.IsNullOrEmpty (arg))
						continue;

					var index = 1;
					if (arg [0] == '/') {
						// nothing to do
					} else if (arg [0] == '-') {
						if (arg.Length >= 2 && arg [1] == '-')
							index++;
					} else {
						continue;
					}
					arg = arg.Substring (index);

					var separators = new char [] { ':', '=' };
					var eq = arg.IndexOfAny (separators);
					var value = string.Empty;
					var name = arg;
					var nextValue = string.Empty;
					var hasValue = false;
					if (eq >= 0) {
						name = arg.Substring (0, eq);
						value = arg.Substring (eq + 1);
						hasValue = true;
					} else if (i < args.Length - 1) {
						nextValue = args [i + 1];
					}

					switch (name) {
					case "aot":
						aot.Add (new TaskItem (value));
						break;
					case "nosymbolstrip":
						// There's also a version that takes symbols as arguments:
						// --nosymbolstrip:symbol1,symbol2
						// in that case we do want to run the SymbolStrip target, so we
						// do not set the MtouchNoSymbolStrip property to 'true' in that case.
						NoSymbolStrip = string.IsNullOrEmpty (value) ? "true" : "false";
						break;
					case "dlsym":
						dlsyms.Add (new TaskItem (string.IsNullOrEmpty (value) ? "true" : value));
						break;
					case "dsym":
						NoDSymUtil = ParseBool (value) ? "false" : "true";
						break;
					case "verbose":
					case "v":
						Verbosity++;
						break;
					case "quiet":
					case "q":
						Verbosity--;
						break;
					case "marshal-managed-exceptions":
						value = hasValue ? value : nextValue; // requires a value, which might be the next option
						MarshalManagedExceptionMode = value;
						break;
					case "marshal-objectivec-exceptions":
						value = hasValue ? value : nextValue; // requires a value, which might be the next option
						MarshalObjectiveCExceptionMode = value;
						break;
					case "custom_bundle_name":
						value = hasValue ? value : nextValue; // requires a value, which might be the next option
						CustomBundleName = value;
						break;
					case "optimize":
						if (!string.IsNullOrEmpty (Optimize))
							Optimize += ",";
						Optimize += value;
						break;
					case "package-debug-symbols":
						PackageDebugSymbols = string.IsNullOrEmpty (value) ? "true" : value;
						break;
					case "require-pinvoke-wrappers":
						RequirePInvokeWrappers = string.IsNullOrEmpty (value) ? "true" : value;
						break;
					case "registrar":
						value = hasValue ? value : nextValue; // requires a value, which might be the next option
						Registrar = value;
						break;
					case "setenv":
						value = hasValue ? value : nextValue; // requires a value, which might be the next option
						var colon = value.IndexOfAny (separators);
						var item = new TaskItem (value.Substring (0, colon));
						item.SetMetadata ("Value", value.Substring (colon + 1));
						envVariables.Add (item);
						break;
					case "skip-marking-nsobjects-in-user-assemblies":
						SkipMarkingNSObjectsInUserAssemblies = ParseBool (value) ? "true" : "false";
						break;
					case "xml":
						if (xml is null)
							xml = new List<string> ();
						value = hasValue ? value : nextValue; // requires a value, which might be the next option
						xml.Add (value);
						break;
					case "nostrip":
						// Output is EnableAssemblyILStripping so we enable if --nostrip=false and disable if true
						NoStrip = ParseBool (value) ? "false" : "true";
						break;
					case "gcc_flags": // mtouch uses gcc_flags
					case "link_flags": // mmp uses link_flags
						value = hasValue ? value : nextValue; // requires a value, which might be the next option
						if (!StringUtils.TryParseArguments (value, out var lf, out var ex)) {
							Log.LogError (MSBStrings.E0189 /* Could not parse the custom linker argument(s) '-{0}': {1} */, $"-{name}={value}", ex.Message);
							continue;
						}
						if (customLinkFlags is null)
							customLinkFlags = new List<string> ();
						customLinkFlags.AddRange (lf);
						break;
					default:
						Log.LogMessage (MessageImportance.Low, "Skipping unknown argument '{0}' with value '{1}'", name, value);
						break;
					}
				}

				if (xml is not null) {
					var defs = new List<ITaskItem> ();
					if (XmlDefinitions is not null)
						defs.AddRange (XmlDefinitions);
					foreach (var x in xml)
						defs.Add (new TaskItem (x));
					XmlDefinitions = defs.ToArray ();
				}

				if (customLinkFlags is not null) {
					var defs = new List<ITaskItem> ();
					if (CustomLinkFlags is not null)
						defs.AddRange (CustomLinkFlags);
					foreach (var lf in customLinkFlags)
						defs.Add (new TaskItem (lf));
					CustomLinkFlags = defs.ToArray ();
				}

				if (envVariables.Count > 0) {
					if (EnvironmentVariables is not null)
						envVariables.AddRange (EnvironmentVariables);
					EnvironmentVariables = envVariables.ToArray ();
				}

				if (dlsyms.Count > 0) {
					if (DlSym is not null)
						dlsyms.AddRange (DlSym);
					DlSym = dlsyms.ToArray ();
				}

				if (aot.Count > 0) {
					if (Aot is not null)
						aot.AddRange (Aot);
					Aot = aot.ToArray ();
				}
			}

			return !Log.HasLoggedErrors;
		}

		static bool ParseBool (string value)
		{
			if (string.IsNullOrEmpty (value))
				return true;

			switch (value.ToLowerInvariant ()) {
			case "1":
			case "yes":
			case "true":
			case "enable":
				return true;
			case "0":
			case "no":
			case "false":
			case "disable":
				return false;
			default:
				try {
					return bool.Parse (value);
				} catch {
					return false;
				}
			}
		}
	}
}
