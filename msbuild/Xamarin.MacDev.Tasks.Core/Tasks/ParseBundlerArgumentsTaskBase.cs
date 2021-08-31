using System.Collections.Generic;

using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;

namespace Xamarin.MacDev.Tasks {
	public abstract class ParseBundlerArgumentsTaskBase : XamarinTask {
		public string ExtraArgs { get; set; }

		[Output]
		public ITaskItem [] DlSym { get; set; }

		[Output]
		public ITaskItem[] EnvironmentVariables { get; set; }

		[Output]
		public string MarshalManagedExceptionMode { get; set; }

		[Output]
		public string MarshalObjectiveCExceptionMode { get; set; }

		[Output]
		public string CustomBundleName { get; set; }

		[Output]
		public string NoSymbolStrip { get; set; }

		[Output]
		public string NoDSymUtil { get; set; }

		[Output]
		public string Optimize { get; set; }

		[Output]
		public string Registrar { get; set; }

		[Output]
		public int Verbosity { get; set; }

		[Output]
		public ITaskItem[] XmlDefinitions { get; set; }

		public override bool Execute ()
		{
			if (string.IsNullOrEmpty (NoSymbolStrip))
				NoSymbolStrip = "false";

			if (string.IsNullOrEmpty (NoDSymUtil))
				NoDSymUtil = "false";

			if (!string.IsNullOrEmpty (ExtraArgs)) {
				var args = CommandLineArgumentBuilder.Parse (ExtraArgs);
				List<string> xml = null;
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
					if (eq >= 0) {
						name = arg.Substring (0, eq);
						value = arg.Substring (eq + 1);
					}

					switch (name) {
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
						MarshalManagedExceptionMode = value;
						break;
					case "marshal-objectivec-exceptions":
						MarshalObjectiveCExceptionMode = value;
						break;
					case "custom_bundle_name":
						CustomBundleName = value;
						break;
					case "optimize":
						if (!string.IsNullOrEmpty (Optimize))
							Optimize += ",";
						Optimize += value;
						break;
					case "registrar":
						Registrar = value;
						break;
					case "setenv":
						var colon = value.IndexOfAny (separators);
						var item = new TaskItem (value.Substring (0, colon));
						item.SetMetadata ("Value", value.Substring (colon + 1));
						envVariables.Add (item);
						break;
					case "xml":
						if (xml == null)
							xml = new List<string> ();
						xml.Add (value);
						break;
					default:
						Log.LogMessage (MessageImportance.Low, "Skipping unknown argument '{0}' with value '{1}'", name, value);
						break;
					}
				}

				if (xml != null) {
					var defs = new List<ITaskItem> ();
					if (XmlDefinitions != null)
						defs.AddRange (XmlDefinitions);
					foreach (var x in xml)
						defs.Add (new TaskItem (x));
					XmlDefinitions = defs.ToArray ();
				}

				if (envVariables.Count > 0) {
					if (EnvironmentVariables != null)
						envVariables.AddRange (EnvironmentVariables);
					EnvironmentVariables = envVariables.ToArray ();
				}

				if (dlsyms.Count > 0) {
					if (DlSym != null)
						dlsyms.AddRange (DlSym);
					DlSym = dlsyms.ToArray ();
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
