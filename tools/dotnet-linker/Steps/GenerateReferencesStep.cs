using System.Collections.Generic;
using System.IO;
using System.Text;

using Mono.Cecil;

using Xamarin.Bundler;
using Xamarin.Linker;

#nullable enable

namespace Xamarin {

	public class GenerateReferencesStep : ConfigurationAwareStep {
		protected override string Name { get; } = "Generate References";
		protected override int ErrorCode { get; } = 2320;

		protected override void TryEndProcess ()
		{
			base.TryEndProcess ();

			var app = Configuration.Application;
			var required_symbols = Configuration.DerivedLinkContext.RequiredSymbols;
			var items = new List<MSBuildItem> ();

			switch (app.SymbolMode) {
			case SymbolMode.Ignore:
				break;
			case SymbolMode.Code:
				var reference_m = Path.Combine (Configuration.CacheDirectory, "reference.m");
				reference_m = Configuration.Target.GenerateReferencingSource (reference_m, required_symbols);
				if (!string.IsNullOrEmpty (reference_m)) {
					var item = new MSBuildItem (reference_m);
					items.Add (item);
				}
				Configuration.WriteOutputForMSBuild ("_ReferencesFile", items);
				break;
			case SymbolMode.Linker:
				foreach (var symbol in required_symbols) {
					var item = new MSBuildItem ("-u" + symbol.Prefix + symbol.Name);
					items.Add (item);
				}
				Configuration.WriteOutputForMSBuild ("_ReferencesLinkerFlags", items);
				break;
			default:
				throw ErrorHelper.CreateError (99, Errors.MX0099, $"invalid symbol mode: {app.SymbolMode}");
			}
		}
	}
}
