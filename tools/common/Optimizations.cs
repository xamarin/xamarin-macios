using System.Linq;

namespace Xamarin.Bundler
{
	public class Optimizations
	{
		static string [] opt_names =
		{
			"remove-uithread-checks",
			"dead-code-elimination",
			"inline-isdirectbinding",
			"inline-intptr-size",
#if MONOTOUCH
			"inline-runtime-arch",
#else
			"", // dummy value to make indices match up between XM and XI
#endif
			"blockliteral-setupblock",
		};

		bool? [] values;

		public bool? RemoveUIThreadChecks {
			get { return values [0]; }
			set { values [0] = value; }
		}
		public bool? DeadCodeElimination {
			get { return values [1]; }
			set { values [1] = value; }
		}
		public bool? InlineIsDirectBinding {
			get { return values [2]; }
			set { values [2] = value; }
		}
		public bool? InlineIntPtrSize {
			get { return values [3]; }
			set { values [3] = value; }
		}
#if MONOTOUCH
		public bool? InlineRuntimeArch {
			get { return values [4]; }
			set { values [4] = value; }
		}
#endif
		public bool? OptimizeBlockLiteralSetupBlock {
			get { return values [5]; }
			set { values [5] = value; }
		}

		public Optimizations ()
		{
			values = new bool? [opt_names.Length];
		}

		public void Initialize (Application app)
		{
			// warn if the user asked to optimize something when the linker is not enabled
			if (app.LinkMode == LinkMode.None) {
				for (int i = 0; i < values.Length; i++) {
					if (!values [i].HasValue)
						continue;
					ErrorHelper.Warning (2003, $"Option '--optimize={(values [i].Value ? "" : "-")}{opt_names [i]}' will be ignored since linking is disabled");
				}
				return;
			}

			// by default we keep the code to ensure we're executing on the UI thread (for UI code) for debug builds
			// but this can be overridden to either (a) remove it from debug builds or (b) keep it in release builds
			if (!RemoveUIThreadChecks.HasValue)
				RemoveUIThreadChecks = !app.EnableDebug;

			// By default we always eliminate dead code.
			if (!DeadCodeElimination.HasValue)
				DeadCodeElimination = true;

			if (!InlineIsDirectBinding.HasValue) {
#if MONOTOUCH
				// By default we always inline calls to NSObject.IsDirectBinding
				InlineIsDirectBinding = true;
#else
				// NSObject.IsDirectBinding is not a safe optimization to apply to XM apps,
				// because there may be additional code/assemblies we don't know about at build time.
				InlineIsDirectBinding = false;
#endif
			}

			// The default behavior for InlineIntPtrSize depends on the assembly being linked,
			// which means we can't set it to a global constant. It's handled in the OptimizeGeneratedCodeSubStep directly.

#if MONOTOUCH
			// By default we always inline calls to Runtime.Arch
			if (!InlineRuntimeArch.HasValue)
				InlineRuntimeArch = true;
#endif

			// We try to optimize calls to BlockLiteral.SetupBlock if the static registrar is enabled
			if (!OptimizeBlockLiteralSetupBlock.HasValue)
				OptimizeBlockLiteralSetupBlock = app.Registrar == RegistrarMode.Static;

			if (Driver.Verbosity > 3)
				Driver.Log (4, "Enabled optimizations: {0}", string.Join (", ", values.Select ((v, idx) => v == true ? opt_names [idx] : string.Empty).Where ((v) => !string.IsNullOrEmpty (v))));
		}

		public void Parse (string options)
		{
			foreach (var option in options.Split (',')) {
				if (option == null || option.Length < 2)
					throw ErrorHelper.CreateError (10, $"Could not parse the command line argument '--optimize={options}'");

				ParseOption (option);
			}
		}

		void ParseOption (string option)
		{
			bool enabled;
			string opt;
			switch (option [0]) {
			case '-':
				enabled = false;
				opt = option.Substring (1);
				break;
			case '+':
				enabled = true;
				opt = option.Substring (1);
				break;
			default:
				opt = option;
				enabled = true;
				break;
			}

			if (opt == "all") {
				for (int i = 0; i < values.Length; i++)
					values [i] = enabled;
			} else {
				var found = false;
				for (int i = 0; i < values.Length; i++) {
					if (opt_names [i] != opt)
						continue;
					found = true;
					values [i] = enabled;
				}
				if (!found)
					ErrorHelper.Warning (132, $"Unknown optimization: '{opt}'. Valid optimizations are: {string.Join (", ", opt_names.Where ((v) => !string.IsNullOrEmpty (v)))}.");
			}
		}
	}
}