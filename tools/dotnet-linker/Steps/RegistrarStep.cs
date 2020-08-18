using System;

using Xamarin.Bundler;
using Xamarin.Utils;

namespace Xamarin.Linker {
	public class RegistrarStep : ConfigurationAwareStep {
		protected override void EndProcess ()
		{
			base.EndProcess ();

			var app = Configuration.Application;

			app.SelectRegistrar ();

			switch (app.Registrar) {
			case RegistrarMode.Dynamic:
				// Nothing to do here
				break;
			case RegistrarMode.PartialStatic:
				string method = null;
				switch (app.Platform) {
				case ApplePlatform.iOS:
					method = "xamarin_create_classes_Xamarin_iOS";
					break;
				case ApplePlatform.WatchOS:
					method = "xamarin_create_classes_Xamarin_WatchOS";
					break;
				case ApplePlatform.TVOS:
					method = "xamarin_create_classes_Xamarin_TVOS";
					break;
				case ApplePlatform.MacOSX:
					method = "xamarin_create_classes_Xamarin_Mac";
					break;
				default:
					Report (ErrorHelper.CreateError (71, Errors.MX0071, app.Platform, app.ProductName));
					break;
				}
				Configuration.RegistrationMethods.Add (method);
				Configuration.CompilerFlags.AddLinkWith (Configuration.PartialStaticRegistrarLibrary);
				break;
			case RegistrarMode.Static:
				Report (ErrorHelper.CreateError (99, Errors.MX0099, $"The static registrar hasn't been implemented yet."));
				break;
			case RegistrarMode.Default: // We should have resolved 'Default' to an actual mode by now.
			default:
				Report (ErrorHelper.CreateError (99, Errors.MX0099, $"Invalid registrar mode: {app.Registrar}"));
				break;
			}
		}
	}
}
