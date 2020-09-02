using System;

using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;

namespace Xamarin.MacDev.Tasks {
	public abstract class ParseBundlerArgumentsTaskBase : XamarinTask {
		public string ExtraArgs { get; set; }

		[Output]
		public string MarshalManagedExceptionMode { get; set; }

		[Output]
		public string MarshalObjectiveCExceptionMode { get; set; }

		[Output]
		public string NoSymbolStrip { get; set; }

		[Output]
		public string NoDSymUtil { get; set; }

		[Output]
		public string Optimize { get; set; }

		[Output]
		public int Verbosity { get; set; }

		public override bool Execute ()
		{
			if (string.IsNullOrEmpty (NoSymbolStrip))
				NoSymbolStrip = "false";

			if (string.IsNullOrEmpty (NoDSymUtil))
				NoDSymUtil = "false";

			if (!string.IsNullOrEmpty (ExtraArgs)) {
				var args = CommandLineArgumentBuilder.Parse (ExtraArgs);

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

					var eq = arg.IndexOfAny (new char [] { ':', '=' });
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
					case "optimize":
						if (!string.IsNullOrEmpty (Optimize))
							Optimize += ",";
						Optimize += value;
						break;
					default:
						break;
					}
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

