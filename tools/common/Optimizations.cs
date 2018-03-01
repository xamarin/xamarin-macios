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
			"register-protocols",
			"inline-dynamic-registration-supported",
#if MONOTOUCH
			"remove-dynamic-registrar",
#else
			"", // dummy value to make indices match up between XM and XI
#endif
#if MONOTOUCH
			"", // dummy value to make indices match up between XM and XI
#else
			"trim-architectures",
#endif
		};

		enum Opt
		{
			RemoveUIThreadChecks,
			DeadCodeElimination,
			InlineIsDirectBinding,
			InlineIntPtrSize,
			InlineRuntimeArch,
			BlockLiteralSetupBlock,
			RegisterProtocols,
			InlineDynamicRegistrationSupported,
			RemoveDynamicRegistrar,
			TrimArchitectures,
		}

		bool? [] values;

		public bool? RemoveUIThreadChecks {
			get { return values [(int) Opt.RemoveUIThreadChecks]; }
			set { values [(int) Opt.RemoveUIThreadChecks] = value; }
		}
		public bool? DeadCodeElimination {
			get { return values [(int) Opt.DeadCodeElimination]; }
			set { values [(int) Opt.DeadCodeElimination] = value; }
		}
		public bool? InlineIsDirectBinding {
			get { return values [(int) Opt.InlineIsDirectBinding]; }
			set { values [(int) Opt.InlineIsDirectBinding] = value; }
		}
		public bool? InlineIntPtrSize {
			get { return values [(int) Opt.InlineIntPtrSize]; }
			set { values [(int) Opt.InlineIntPtrSize] = value; }
		}
#if MONOTOUCH
		public bool? InlineRuntimeArch {
			get { return values [(int) Opt.InlineRuntimeArch]; }
			set { values [(int) Opt.InlineRuntimeArch] = value; }
		}
#endif
		public bool? OptimizeBlockLiteralSetupBlock {
			get { return values [(int) Opt.BlockLiteralSetupBlock]; }
			set { values [(int) Opt.BlockLiteralSetupBlock] = value; }
		}
		public bool? RegisterProtocols {
			get { return values [(int) Opt.RegisterProtocols]; }
			set { values [(int) Opt.RegisterProtocols] = value; }
		}
		public bool? InlineDynamicRegistrationSupported {
			get { return values [(int) Opt.InlineDynamicRegistrationSupported]; }
			set { values [(int) Opt.InlineDynamicRegistrationSupported] = value; }
		}

		public bool? RemoveDynamicRegistrar {
			get { return values [(int) Opt.RemoveDynamicRegistrar]; }
			set { values [(int) Opt.RemoveDynamicRegistrar] = value; }
		}
		
		public bool? TrimArchitectures {
			get { return values [(int) Opt.TrimArchitectures]; }
			set { values [(int) Opt.TrimArchitectures] = value; }
		}

		public Optimizations ()
		{
			values = new bool? [opt_names.Length];
		}

		public void Initialize (Application app)
		{
			// warn if the user asked to optimize something when the optimization can't be applied
			for (int i = 0; i < values.Length; i++) {
				if (!values [i].HasValue)
					continue;
				switch ((Opt) i) {
				case Opt.TrimArchitectures:
					break; // Does not require linker
				case Opt.RegisterProtocols:
				case Opt.RemoveDynamicRegistrar:
					if (app.Registrar != RegistrarMode.Static) {
						ErrorHelper.Warning (2003, $"Option '--optimize={(values [i].Value ? "" : "-")}{opt_names [i]}' will be ignored since the static registrar is not enabled");
						values [i] = false;
						continue;
					}
					goto default; // also requires the linker
				default:
					if (app.LinkMode == LinkMode.None) {
						ErrorHelper.Warning (2003, $"Option '--optimize={(values [i].Value ? "" : "-")}{opt_names [i]}' will be ignored since linking is disabled");
						values [i] = false;
					}
					break;
				}
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
			if (!OptimizeBlockLiteralSetupBlock.HasValue) {
#if MONOMAC
				// Restrict to Unified, since XamMac.dll doesn't have the new managed block API (SetupBlockImpl) to make the block optimization work.
				OptimizeBlockLiteralSetupBlock = app.Registrar == RegistrarMode.Static && Driver.IsUnified;
#else
				OptimizeBlockLiteralSetupBlock = app.Registrar == RegistrarMode.Static;
#endif
			}

			// We will register protocols if the static registrar is enabled
			if (!RegisterProtocols.HasValue) {
#if MONOTOUCH
				RegisterProtocols = app.Registrar == RegistrarMode.Static;
#else
				RegisterProtocols = false;
#endif
			} else if (app.Registrar != RegistrarMode.Static && RegisterProtocols == true) {
				RegisterProtocols = false; // we've already shown a warning for this.
			}

			// By default we always inline calls to Runtime.DynamicRegistrationSupported
			if (!InlineDynamicRegistrationSupported.HasValue)
				InlineDynamicRegistrationSupported = true;

			if (!RemoveDynamicRegistrar.HasValue) {
				if (InlineDynamicRegistrationSupported != true) {
					// Can't remove the dynamic registrar unless also inlining Runtime.DynamicRegistrationSupported
					RemoveDynamicRegistrar = false;
				} else if (app.Registrar != RegistrarMode.Static || app.LinkMode == LinkMode.None) {
					// Both the linker and the static registrar are also required
					RemoveDynamicRegistrar = false;
				} else {
#if MONOTOUCH
					// We don't have enough information yet to determine if we can remove the dynamic
					// registrar or not, so let the value stay unset until we do know (when running the linker).
#else
					// By default disabled for XM apps
					RemoveDynamicRegistrar = false;
#endif
				}
			}

#if !MONOTOUCH
			// By default on macOS trim-architectures for Release and not for debug 
			if (!TrimArchitectures.HasValue)
				TrimArchitectures = !app.EnableDebug;
#endif

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
				for (int i = 0; i < values.Length; i++) {
					if (!string.IsNullOrEmpty (opt_names [i]))
						values [i] = enabled;
				}
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