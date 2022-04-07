using Xamarin.MacDev.Tasks;
using Xamarin.MacDev;

#nullable enable

namespace Xamarin.Mac.Tasks {
	public abstract class CompileAppManifestTaskCore : CompileAppManifestTaskBase {

		protected override bool Compile (PDictionary plist)
		{
			if (!IsAppExtension || (IsAppExtension && IsXPCService))
				plist.SetIfNotPresent ("MonoBundleExecutable", AssemblyName + ".exe");

			return !Log.HasLoggedErrors;
		}
	}
}
