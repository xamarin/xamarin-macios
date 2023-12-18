using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Utils;

namespace Xamarin.Bundler {
	public class Optimizations {
		static string [] opt_names =
		{
			"remove-uithread-checks",
			"dead-code-elimination",
			"inline-isdirectbinding",
			"inline-intptr-size",
			"inline-runtime-arch",
			"blockliteral-setupblock",
			"register-protocols",
			"inline-dynamic-registration-supported",
			"static-block-to-delegate-lookup",
			"remove-dynamic-registrar",
			"trim-architectures",
			"remove-unsupported-il-for-bitcode",
			"inline-is-arm64-calling-convention",
			"seal-and-devirtualize",
			"cctor-beforefieldinit",
			"custom-attributes-removal",
			"experimental-xforms-product-type",
			"force-rejected-types-removal",
			"redirect-class-handles",
		};

		static ApplePlatform [] [] valid_platforms = new ApplePlatform [] [] {
			/* Opt.RemoveUIThreadChecks               */ new ApplePlatform [] { ApplePlatform.iOS, ApplePlatform.MacOSX, ApplePlatform.WatchOS, ApplePlatform.TVOS, ApplePlatform.MacCatalyst },
			/* Opt.DeadCodeElimination                */ new ApplePlatform [] { ApplePlatform.iOS, ApplePlatform.MacOSX, ApplePlatform.WatchOS, ApplePlatform.TVOS, ApplePlatform.MacCatalyst },
			/* Opt.InlineIsDirectBinding              */ new ApplePlatform [] { ApplePlatform.iOS, ApplePlatform.MacOSX, ApplePlatform.WatchOS, ApplePlatform.TVOS, ApplePlatform.MacCatalyst },
			/* Opt.InlineIntPtrSize                   */ new ApplePlatform [] { ApplePlatform.iOS, ApplePlatform.MacOSX, ApplePlatform.WatchOS, ApplePlatform.TVOS, ApplePlatform.MacCatalyst },
			/* Opt.InlineRuntimeArch                  */ new ApplePlatform [] { ApplePlatform.iOS,                       ApplePlatform.WatchOS, ApplePlatform.TVOS                            },
			/* Opt.BlockLiteralSetupBlock             */ new ApplePlatform [] { ApplePlatform.iOS, ApplePlatform.MacOSX, ApplePlatform.WatchOS, ApplePlatform.TVOS, ApplePlatform.MacCatalyst },
			/* Opt.RegisterProtocols                  */ new ApplePlatform [] { ApplePlatform.iOS, ApplePlatform.MacOSX, ApplePlatform.WatchOS, ApplePlatform.TVOS, ApplePlatform.MacCatalyst },
			/* Opt.InlineDynamicRegistrationSupported */ new ApplePlatform [] { ApplePlatform.iOS, ApplePlatform.MacOSX, ApplePlatform.WatchOS, ApplePlatform.TVOS, ApplePlatform.MacCatalyst },
			/* Opt.StaticBlockToDelegateLookup        */ new ApplePlatform [] { ApplePlatform.iOS, ApplePlatform.MacOSX, ApplePlatform.WatchOS, ApplePlatform.TVOS, ApplePlatform.MacCatalyst },
			/* Opt.RemoveDynamicRegistrar             */ new ApplePlatform [] { ApplePlatform.iOS,                       ApplePlatform.WatchOS, ApplePlatform.TVOS, ApplePlatform.MacCatalyst },
			/* Opt.TrimArchitectures                  */ new ApplePlatform [] {                    ApplePlatform.MacOSX,                                                                      },
			/* Opt.RemoveUnsupportedILForBitcode      */ new ApplePlatform [] {                                          ApplePlatform.WatchOS,                                               },
			/* Opt.InlineIsARM64CallingConvention     */ new ApplePlatform [] { ApplePlatform.iOS, ApplePlatform.MacOSX, ApplePlatform.WatchOS, ApplePlatform.TVOS, ApplePlatform.MacCatalyst },
			/* Opt.SealAndDevirtualize                */ new ApplePlatform [] { ApplePlatform.iOS,                       ApplePlatform.WatchOS, ApplePlatform.TVOS, ApplePlatform.MacCatalyst },
			/* Opt.StaticConstructorBeforeFieldInit   */ new ApplePlatform [] { ApplePlatform.iOS, ApplePlatform.MacOSX, ApplePlatform.WatchOS, ApplePlatform.TVOS, ApplePlatform.MacCatalyst },
			/* Opt.CustomAttributesRemoval            */ new ApplePlatform [] { ApplePlatform.iOS, ApplePlatform.MacOSX, ApplePlatform.WatchOS, ApplePlatform.TVOS, ApplePlatform.MacCatalyst },
			/* Opt.ExperimentalFormsProductType       */ new ApplePlatform [] { ApplePlatform.iOS, ApplePlatform.MacOSX, ApplePlatform.WatchOS, ApplePlatform.TVOS, ApplePlatform.MacCatalyst },
			/* Opt.ForceRejectedTypesRemoval          */ new ApplePlatform [] { ApplePlatform.iOS,                       ApplePlatform.WatchOS, ApplePlatform.TVOS, ApplePlatform.MacCatalyst },
			/* Opt.RedirectClassHandles               */ new ApplePlatform [] { ApplePlatform.iOS, ApplePlatform.MacOSX, ApplePlatform.WatchOS, ApplePlatform.TVOS, ApplePlatform.MacCatalyst },
		};

		enum Opt {
			RemoveUIThreadChecks,
			DeadCodeElimination,
			InlineIsDirectBinding,
			InlineIntPtrSize,
			InlineRuntimeArch,
			BlockLiteralSetupBlock,
			RegisterProtocols,
			InlineDynamicRegistrationSupported,
			StaticBlockToDelegateLookup,
			RemoveDynamicRegistrar,
			TrimArchitectures,
			RemoveUnsupportedILForBitcode,
			InlineIsARM64CallingConvention,
			SealAndDevirtualize,
			StaticConstructorBeforeFieldInit,
			CustomAttributesRemoval,
			ExperimentalFormsProductType,
			ForceRejectedTypesRemoval,
			RedirectClassHandles,
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
		public bool? InlineRuntimeArch {
			get { return values [(int) Opt.InlineRuntimeArch]; }
			set { values [(int) Opt.InlineRuntimeArch] = value; }
		}
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

		public bool? StaticBlockToDelegateLookup {
			get { return values [(int) Opt.StaticBlockToDelegateLookup]; }
			set { values [(int) Opt.StaticBlockToDelegateLookup] = value; }
		}
		public bool? RemoveDynamicRegistrar {
			get { return values [(int) Opt.RemoveDynamicRegistrar]; }
			set { values [(int) Opt.RemoveDynamicRegistrar] = value; }
		}

		public bool? TrimArchitectures {
			get { return values [(int) Opt.TrimArchitectures]; }
			set { values [(int) Opt.TrimArchitectures] = value; }
		}

		public bool? RemoveUnsupportedILForBitcode {
			get { return values [(int) Opt.RemoveUnsupportedILForBitcode]; }
			set { values [(int) Opt.RemoveUnsupportedILForBitcode] = value; }
		}

		public bool? InlineIsARM64CallingConvention {
			get { return values [(int) Opt.InlineIsARM64CallingConvention]; }
			set { values [(int) Opt.InlineIsARM64CallingConvention] = value; }
		}

		public bool? SealAndDevirtualize {
			get { return values [(int) Opt.SealAndDevirtualize]; }
			set { values [(int) Opt.SealAndDevirtualize] = value; }
		}

		public bool? StaticConstructorBeforeFieldInit {
			get { return values [(int) Opt.StaticConstructorBeforeFieldInit]; }
			set { values [(int) Opt.StaticConstructorBeforeFieldInit] = value; }
		}

		public bool? CustomAttributesRemoval {
			get { return values [(int) Opt.CustomAttributesRemoval]; }
			set { values [(int) Opt.CustomAttributesRemoval] = value; }
		}

		public bool? ExperimentalFormsProductType {
			get { return true; }
			set { }
		}

		public bool? ForceRejectedTypesRemoval {
			get { return values [(int) Opt.ForceRejectedTypesRemoval]; }
			set { values [(int) Opt.ForceRejectedTypesRemoval] = value; }
		}

		public bool? RedirectClassHandles {
			get { return values [(int) Opt.RedirectClassHandles]; }
			set { values [(int) Opt.RedirectClassHandles] = value; }
		}

		public Optimizations ()
		{
			values = new bool? [opt_names.Length];
		}

		public void Initialize (Application app, out List<ProductException> messages)
		{
			messages = new List<ProductException> ();
			// warn if the user asked to optimize something when the optimization can't be applied
			for (int i = 0; i < values.Length; i++) {
				if (!values [i].HasValue)
					continue;

				// The remove-dynamic-registrar optimization is required when using NativeAOT
				if (app.XamarinRuntime == XamarinRuntime.NativeAOT && (Opt) i == Opt.RemoveDynamicRegistrar && values [i] == false) {
					messages.Add (ErrorHelper.CreateWarning (2016, Errors.MX2016 /* Keeping the dynamic registrar (by passing '--optimize=-remove-dynamic-registrar') is not possible, because the dynamic registrar is not supported when using NativeAOT. Support for dynamic registration will still be removed. */));
					values [i] = true;
					continue;
				}

				// The remove-dynamic-registrar optimization is a bit if a special case on macOS:
				// it only works in very specific circumstances, so we don't add it to valid_platforms.
				// This means it won't be listed in --help, and it won't be enabled if all optimizations
				// are enabled. Yet we still might want to enable it manually, and this condition
				// allows us to manually pass --optimize=remove-dynamic-registrar and enable it that way.
				if (app.Platform == ApplePlatform.MacOSX && app.XamarinRuntime != XamarinRuntime.NativeAOT && (Opt) i == Opt.RemoveDynamicRegistrar)
					continue;

				// check if the optimization is valid for the current platform
				var valid = valid_platforms [i];
				if (Array.IndexOf (valid, app.Platform) < 0) {
					messages.Add (ErrorHelper.CreateWarning (2003, Errors.MT2003_C, opt_names [i], string.Join (", ", valid.Select (v => v.AsString ()))));
					values [i] = false;
					continue;
				}

				switch ((Opt) i) {
				case Opt.StaticBlockToDelegateLookup:
					if (app.Registrar != RegistrarMode.Static && app.Registrar != RegistrarMode.ManagedStatic) {
						messages.Add (ErrorHelper.CreateWarning (2003, Errors.MT2003, (values [i].Value ? "" : "-"), opt_names [i]));
						values [i] = false;
						continue;
					}
					break; // does not require the linker
				case Opt.TrimArchitectures:
					break; // Does not require linker
				case Opt.RegisterProtocols:
				case Opt.RemoveDynamicRegistrar:
				case Opt.RedirectClassHandles:
					if (app.Registrar != RegistrarMode.Static && app.Registrar != RegistrarMode.ManagedStatic) {
						messages.Add (ErrorHelper.CreateWarning (2003, Errors.MT2003, (values [i].Value ? "" : "-"), opt_names [i]));
						values [i] = false;
						continue;
					}
					goto default; // also requires the linker
				default:
					if (!app.AreAnyAssembliesTrimmed) {
						messages.Add (ErrorHelper.CreateWarning (2003, Errors.MT2003_B, (values [i].Value ? "" : "-"), opt_names [i]));
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
				if (app.Platform != ApplePlatform.MacOSX) {
					// By default we always inline calls to NSObject.IsDirectBinding
					// unless the interpreter is enabled (we can't predict if code will be subclassed)
					InlineIsDirectBinding = !app.UseInterpreter;
				} else {
					// NSObject.IsDirectBinding is not a safe optimization to apply to XM apps,
					// because there may be additional code/assemblies we don't know about at build time.
					InlineIsDirectBinding = false;
				}
			}

			// The default behavior for InlineIntPtrSize depends on the assembly being linked,
			// which means we can't set it to a global constant. It's handled in the OptimizeGeneratedCodeSubStep directly.

			if (app.Platform != ApplePlatform.MacOSX) {
				// By default we always inline calls to Runtime.Arch
				if (!InlineRuntimeArch.HasValue)
					InlineRuntimeArch = true;
			}

			// We try to optimize calls to BlockLiteral.SetupBlock and certain BlockLiteral constructors if the static registrar is enabled
			if (!OptimizeBlockLiteralSetupBlock.HasValue) {
				OptimizeBlockLiteralSetupBlock = app.Registrar == RegistrarMode.Static || app.Registrar == RegistrarMode.ManagedStatic;
			}

			// We will register protocols if the static registrar is enabled and loading assemblies is not possible
			if (!RegisterProtocols.HasValue) {
				if (app.Platform != ApplePlatform.MacOSX || app.XamarinRuntime == XamarinRuntime.NativeAOT) {
					RegisterProtocols = (app.Registrar == RegistrarMode.Static || app.Registrar == RegistrarMode.ManagedStatic) && !app.UseInterpreter;
				} else {
					RegisterProtocols = false;
				}
			} else if (app.Registrar != RegistrarMode.Static && app.Registrar != RegistrarMode.ManagedStatic && RegisterProtocols == true) {
				RegisterProtocols = false; // we've already shown a warning for this.
			}

			// By default we always inline calls to Runtime.DynamicRegistrationSupported
			if (!InlineDynamicRegistrationSupported.HasValue)
				InlineDynamicRegistrationSupported = true;

			// By default always enable static block-to-delegate lookup (it won't make a difference unless the static registrar is used though)
			if (!StaticBlockToDelegateLookup.HasValue)
				StaticBlockToDelegateLookup = true;

			if (!RemoveDynamicRegistrar.HasValue) {
				if (app.XamarinRuntime == XamarinRuntime.NativeAOT) {
					RemoveDynamicRegistrar = true;
				} else if (InlineDynamicRegistrationSupported != true) {
					// Can't remove the dynamic registrar unless also inlining Runtime.DynamicRegistrationSupported
					RemoveDynamicRegistrar = false;
				} else if (StaticBlockToDelegateLookup != true) {
					// Can't remove the dynamic registrar unless also generating static lookup of block-to-delegates in the static registrar.
					RemoveDynamicRegistrar = false;
				} else if ((app.Registrar != RegistrarMode.Static && app.Registrar != RegistrarMode.ManagedStatic) || !app.AreAnyAssembliesTrimmed) {
					// Both the linker and the static registrar are also required
					RemoveDynamicRegistrar = false;
				} else {
					if (app.Platform != ApplePlatform.MacOSX) {
						// we can't predict is unknown (at build time) code will require registration (at runtime)
						if (app.UseInterpreter) {
							RemoveDynamicRegistrar = false;
						}
						// We don't have enough information yet to determine if we can remove the dynamic
						// registrar or not, so let the value stay unset until we do know (when running the linker).
					} else {
						// By default disabled for XM apps
						RemoveDynamicRegistrar = false;
					}
				}
			}

			if (app.Platform == ApplePlatform.MacOSX) {
				// By default on macOS trim-architectures for Release and not for debug 
				if (!TrimArchitectures.HasValue)
					TrimArchitectures = !app.EnableDebug;
			}

			if (app.Platform != ApplePlatform.MacOSX) {
				if (!RemoveUnsupportedILForBitcode.HasValue) {
					// By default enabled for watchOS device builds.
					RemoveUnsupportedILForBitcode = app.Platform == ApplePlatform.WatchOS && app.IsDeviceBuild;
				}

				if (!SealAndDevirtualize.HasValue) {
					// by default run the linker SealerSubStep unless the interpreter is enabled
					SealAndDevirtualize = !app.UseInterpreter;
				}
			}

			// By default Runtime.IsARM64CallingConvention inlining is always enabled.
			if (!InlineIsARM64CallingConvention.HasValue)
				InlineIsARM64CallingConvention = true;

			// by default we try to eliminate any .cctor we can
			if (!StaticConstructorBeforeFieldInit.HasValue)
				StaticConstructorBeforeFieldInit = true;

			// by default we remove rarely used custom attributes
			if (!CustomAttributesRemoval.HasValue)
				CustomAttributesRemoval = true;
		}

		public void Parse (ApplePlatform platform, string options, List<ProductException> messages)
		{
			foreach (var option in options.Split (',')) {
				if (option is null || option.Length < 2) {
					messages.Add (ErrorHelper.CreateError (10, Errors.MX0010, $"'--optimize={options}'"));
					return;
				}

				ParseOption (platform, option, messages);
			}
		}

		void ParseOption (ApplePlatform platform, string option, List<ProductException> messages)
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
					var valid = valid_platforms [i];
					if (Array.IndexOf (valid, platform) < 0)
						continue; // Don't apply 'all' to optimizations that aren't valid for the current platform
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
#if NET
					messages.Add (ErrorHelper.CreateWarning (132, Errors.MX0132, opt, string.Join (", ", Enum.GetValues<Opt> ().Where (o => Array.IndexOf (valid_platforms [(int) o], platform) >= 0).Select (o => opt_names [(int) o]))));
#else
					messages.Add (ErrorHelper.CreateWarning (132, Errors.MX0132, opt, string.Join (", ", Enum.GetValues (typeof (Opt)).Cast<Opt> ().Where (o => Array.IndexOf (valid_platforms [(int) o], platform) >= 0).Select (o => opt_names [(int) o]))));
#endif
			}
		}

		public override string ToString ()
		{
			var sb = new StringBuilder ();
			for (var i = 0; i < values.Length; i++) {
				if (values [i] is null)
					continue;
				if (sb.Length > 0)
					sb.Append (' ');
				sb.Append (values [i].Value ? "+" : "-");
				sb.Append (opt_names [i]);
			}
			if (sb.Length == 0)
				return "<default>";
			return sb.ToString ();
		}
	}
}
