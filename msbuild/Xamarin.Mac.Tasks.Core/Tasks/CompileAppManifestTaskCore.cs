using Xamarin.MacDev.Tasks;
using Xamarin.MacDev;

namespace Xamarin.Mac.Tasks {
	public abstract class CompileAppManifestTaskCore : CompileAppManifestTaskBase {

		protected override bool Compile (PDictionary plist)
		{
			if (!IsAppExtension || (IsAppExtension && IsXPCService))
				plist.SetIfNotPresent ("MonoBundleExecutable", AssemblyName + ".exe");
			plist.SetIfNotPresent (ManifestKeys.CFBundleName, AppBundleName);

			return !Log.HasLoggedErrors;
		}
	}
}
