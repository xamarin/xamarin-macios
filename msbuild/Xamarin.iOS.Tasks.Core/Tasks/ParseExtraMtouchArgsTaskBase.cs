using System;

using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;

using Xamarin.MacDev;
using Xamarin.MacDev.Tasks;

namespace Xamarin.iOS.Tasks
{
	public abstract class ParseExtraMtouchArgsTaskBase : Task
	{
		public string SessionId { get; set; }

		public string ExtraArgs { get; set; }

		[Output]
		public string NoSymbolStrip { get; set; }

		[Output]
		public string NoDSymUtil { get; set; }

		public override bool Execute ()
		{
			if (string.IsNullOrEmpty (NoSymbolStrip))
				NoSymbolStrip = "false";

			if (string.IsNullOrEmpty (NoDSymUtil))
				NoDSymUtil = "false";

			if (!string.IsNullOrEmpty (ExtraArgs)) {
				var args = CommandLineArgumentBuilder.Parse (ExtraArgs);

				for (int i = 0; i < args.Length; i++) {
					string arg;

					if (string.IsNullOrEmpty (args[i]))
						continue;

					if (args[i][0] == '/') {
						arg = args[i].Substring (1);
					} else if (args[i][0] == '-') {
						if (args[i].Length >= 2 && args[i][1] == '-')
							arg = args[i].Substring (2);
						else
							arg = args[i].Substring (1);
					} else {
						continue;
					}

					switch (arg) {
					case "nosymbolstrip":
						NoSymbolStrip = "true";
						// There's also a version that takes symbols as arguments:
						// --nosymbolstrip:symbol1,symbol2
						// in that case we do want to run the SymbolStrip target, so we
						// do not set the MtouchNoSymbolStrip property to 'true' in that case.
						break;
					case "dsym":
						NoDSymUtil = "false";
						break;
					default:
						if (arg.StartsWith ("dsym:", StringComparison.Ordinal) || arg.StartsWith ("dsym=", StringComparison.Ordinal)) {
							NoDSymUtil = ParseBool (arg.Substring (5)) ? "false" : "true";
						}
						break;
					}
				}
			}

			Log.LogTaskProperty ("NoSymbolStrip Output", NoSymbolStrip);
			Log.LogTaskProperty ("NoDSymUtil Output", NoDSymUtil);

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
