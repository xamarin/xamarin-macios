using System.Collections.Generic;
using System.Linq;
using ObjCRuntime;
using Xamarin.Utils;

#nullable enable

public class LibraryConfig {
	List<string> libs = new List<string> ();
	public PlatformName CurrentPlatform;
	TargetFramework? target_framework;
	public TargetFramework TargetFramework {
		get { return target_framework!.Value; }
	}
	internal bool IsDotNet {
		get { return TargetFramework.IsDotNet; }
	}

	public string GetAttributeLibraryPath (string attributedll)
	{
		if (!string.IsNullOrEmpty (attributedll))
			return attributedll!;

		if (IsDotNet)
			return CurrentPlatform.GetPath ("lib", "Xamarin.Apple.BindingAttributes.dll");

		switch (CurrentPlatform) {
		case PlatformName.iOS:
			return CurrentPlatform.GetPath ("lib", "bgen", "Xamarin.iOS.BindingAttributes.dll");
		case PlatformName.WatchOS:
			return CurrentPlatform.GetPath ("lib", "bgen", "Xamarin.WatchOS.BindingAttributes.dll");
		case PlatformName.TvOS:
			return CurrentPlatform.GetPath ("lib", "bgen", "Xamarin.TVOS.BindingAttributes.dll");
		case PlatformName.MacCatalyst:
			return CurrentPlatform.GetPath ("lib", "bgen", "Xamarin.MacCatalyst.BindingAttributes.dll");
		case PlatformName.MacOSX:
			if (target_framework == TargetFramework.Xamarin_Mac_4_5_Full) {
				return CurrentPlatform.GetPath ("lib", "bgen", "Xamarin.Mac-full.BindingAttributes.dll");
			} else if (target_framework == TargetFramework.Xamarin_Mac_4_5_System) {
				return CurrentPlatform.GetPath ("lib", "bgen", "Xamarin.Mac-full.BindingAttributes.dll");
			} else if (target_framework == TargetFramework.Xamarin_Mac_2_0_Mobile) {
				return CurrentPlatform.GetPath ("lib", "bgen", "Xamarin.Mac-mobile.BindingAttributes.dll");
			} else {
				throw ErrorHelper.CreateError (1053, target_framework);
			}
		default:
			throw new BindingException (1047, CurrentPlatform);
		}
	}

	public IEnumerable<string> GetLibraryDirectories ()
	{
		if (!IsDotNet) {
			switch (CurrentPlatform) {
			case PlatformName.iOS:
				yield return CurrentPlatform.GetPath ("lib", "mono", "Xamarin.iOS");
				break;
			case PlatformName.WatchOS:
				yield return CurrentPlatform.GetPath ("lib", "mono", "Xamarin.WatchOS");
				break;
			case PlatformName.TvOS:
				yield return CurrentPlatform.GetPath ("lib", "mono", "Xamarin.TVOS");
				break;
			case PlatformName.MacCatalyst:
				yield return CurrentPlatform.GetPath ("lib", "mono", "Xamarin.MacCatalyst");
				break;
			case PlatformName.MacOSX:
				if (target_framework == TargetFramework.Xamarin_Mac_4_5_Full) {
					yield return CurrentPlatform.GetPath ("lib", "reference", "full");
					yield return CurrentPlatform.GetPath ("lib", "mono", "4.5");
				} else if (target_framework == TargetFramework.Xamarin_Mac_4_5_System) {
					yield return "/Library/Frameworks/Mono.framework/Versions/Current/lib/mono/4.5";
					yield return CurrentPlatform.GetPath ("lib", "mono", "4.5");
				} else if (target_framework == TargetFramework.Xamarin_Mac_2_0_Mobile) {
					yield return CurrentPlatform.GetPath ("lib", "mono", "Xamarin.Mac");
				} else {
					throw ErrorHelper.CreateError (1053, target_framework);
				}
				break;
			default:
				throw new BindingException (1047, CurrentPlatform);
			}
		}
		foreach (var lib in libs)
			yield return lib;
	}

	public void SetTargetFramework (string fx)
	{
		TargetFramework tf;
		if (!TargetFramework.TryParse (fx, out tf))
			throw ErrorHelper.CreateError (68, fx);
		target_framework = tf;

		if (!TargetFramework.IsValidFramework (target_framework.Value))
			throw ErrorHelper.CreateError (70, target_framework.Value,
				string.Join (" ", TargetFramework.ValidFrameworks.Select ((v) => v.ToString ()).ToArray ()));
	}
}

