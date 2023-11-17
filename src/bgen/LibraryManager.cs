using System.Collections.Generic;
using System.Linq;
using ObjCRuntime;
using Xamarin.Utils;

#nullable enable

public class LibraryManager {
	List<string> libs = new List<string> ();
	public bool skipSystemDrawing = false;
	TargetFramework? targetFramework; // TODO fix casing on everything in this class
	public TargetFramework TargetFramework {
		get { return targetFramework!.Value; }
	}
	internal bool IsDotNet {
		get { return TargetFramework.IsDotNet; }
	}

	public string GetAttributeLibraryPath (string? attributedll, PlatformName currentPlatform)
	{
		if (!string.IsNullOrEmpty (attributedll))
			return attributedll!;

		if (IsDotNet)
			return currentPlatform.GetPath ("lib", "Xamarin.Apple.BindingAttributes.dll");

		switch (currentPlatform) {
		case PlatformName.iOS:
			return currentPlatform.GetPath ("lib", "bgen", "Xamarin.iOS.BindingAttributes.dll");
		case PlatformName.WatchOS:
			return currentPlatform.GetPath ("lib", "bgen", "Xamarin.WatchOS.BindingAttributes.dll");
		case PlatformName.TvOS:
			return currentPlatform.GetPath ("lib", "bgen", "Xamarin.TVOS.BindingAttributes.dll");
		case PlatformName.MacCatalyst:
			return currentPlatform.GetPath ("lib", "bgen", "Xamarin.MacCatalyst.BindingAttributes.dll");
		case PlatformName.MacOSX:
			if (targetFramework == TargetFramework.Xamarin_Mac_4_5_Full) {
				return currentPlatform.GetPath ("lib", "bgen", "Xamarin.Mac-full.BindingAttributes.dll");
			} else if (targetFramework == TargetFramework.Xamarin_Mac_4_5_System) {
				return currentPlatform.GetPath ("lib", "bgen", "Xamarin.Mac-full.BindingAttributes.dll");
			} else if (targetFramework == TargetFramework.Xamarin_Mac_2_0_Mobile) {
				return currentPlatform.GetPath ("lib", "bgen", "Xamarin.Mac-mobile.BindingAttributes.dll");
			} else {
				throw ErrorHelper.CreateError (1053, targetFramework);
			}
		default:
			throw new BindingException (1047, currentPlatform);
		}
	}

	public IEnumerable<string> GetLibraryDirectories (PlatformName currentPlatform)
	{
		if (!IsDotNet) {
			switch (currentPlatform) {
			case PlatformName.iOS:
				yield return currentPlatform.GetPath ("lib", "mono", "Xamarin.iOS");
				break;
			case PlatformName.WatchOS:
				yield return currentPlatform.GetPath ("lib", "mono", "Xamarin.WatchOS");
				break;
			case PlatformName.TvOS:
				yield return currentPlatform.GetPath ("lib", "mono", "Xamarin.TVOS");
				break;
			case PlatformName.MacCatalyst:
				yield return currentPlatform.GetPath ("lib", "mono", "Xamarin.MacCatalyst");
				break;
			case PlatformName.MacOSX:
				if (targetFramework == TargetFramework.Xamarin_Mac_4_5_Full) {
					yield return currentPlatform.GetPath ("lib", "reference", "full");
					yield return currentPlatform.GetPath ("lib", "mono", "4.5");
				} else if (targetFramework == TargetFramework.Xamarin_Mac_4_5_System) {
					yield return "/Library/Frameworks/Mono.framework/Versions/Current/lib/mono/4.5";
					yield return currentPlatform.GetPath ("lib", "mono", "4.5");
				} else if (targetFramework == TargetFramework.Xamarin_Mac_2_0_Mobile) {
					yield return currentPlatform.GetPath ("lib", "mono", "Xamarin.Mac");
				} else {
					throw ErrorHelper.CreateError (1053, targetFramework);
				}
				break;
			default:
				throw new BindingException (1047, currentPlatform);
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
		targetFramework = tf;

		if (!TargetFramework.IsValidFramework (targetFramework.Value))
			throw ErrorHelper.CreateError (70, targetFramework.Value,
				string.Join (" ", TargetFramework.ValidFrameworks.Select ((v) => v.ToString ()).ToArray ()));
	}

	public bool SetBaseLibDllAndReferences(ref string baselibdll, out PlatformName currentPlatform, List<string> references) // TODO is ref good idea?
	{
		bool nostdlib = false; // TODO make sure default to false is recommended
		if (!targetFramework.HasValue)
			throw ErrorHelper.CreateError(86);

		switch (targetFramework.Value.Platform)
		{
		case ApplePlatform.iOS:
			currentPlatform = PlatformName.iOS;
			nostdlib = true;
			if (string.IsNullOrEmpty(baselibdll))
				baselibdll = currentPlatform.GetPath( "lib/mono/Xamarin.iOS/Xamarin.iOS.dll");
			if (!IsDotNet)
			{
				references.Add("Facades/System.Drawing.Common");
				ReferenceFixer.FixSDKReferences(currentPlatform, "lib/mono/Xamarin.iOS", references);
			}

			break;
		case ApplePlatform.TVOS:
			currentPlatform = PlatformName.TvOS;
			nostdlib = true;
			if (string.IsNullOrEmpty(baselibdll))
				baselibdll = currentPlatform.GetPath( "lib/mono/Xamarin.TVOS/Xamarin.TVOS.dll");
			if (!IsDotNet)
			{
				references.Add("Facades/System.Drawing.Common");
				ReferenceFixer.FixSDKReferences(currentPlatform, "lib/mono/Xamarin.TVOS", references);
			}

			break;
		case ApplePlatform.WatchOS:
			currentPlatform = PlatformName.WatchOS;
			nostdlib = true;
			if (string.IsNullOrEmpty(baselibdll))
				baselibdll = currentPlatform.GetPath( "lib/mono/Xamarin.WatchOS/Xamarin.WatchOS.dll");
			if (!IsDotNet)
			{
				references.Add("Facades/System.Drawing.Common");
				ReferenceFixer.FixSDKReferences(currentPlatform, "lib/mono/Xamarin.WatchOS", references);
			}

			break;
		case ApplePlatform.MacCatalyst:
			currentPlatform = PlatformName.MacCatalyst;
			nostdlib = true;
			if (string.IsNullOrEmpty(baselibdll))
				baselibdll = currentPlatform.GetPath( "lib/mono/Xamarin.MacCatalyst/Xamarin.MacCatalyst.dll");
			if (!IsDotNet)
			{
				// references.Add ("Facades/System.Drawing.Common");
				ReferenceFixer.FixSDKReferences(currentPlatform, "lib/mono/Xamarin.MacCatalyst", references);
			}

			break;
		case ApplePlatform.MacOSX:
			currentPlatform = PlatformName.MacOSX;
			nostdlib = true;
			if (string.IsNullOrEmpty(baselibdll))
			{
				if (targetFramework == TargetFramework.Xamarin_Mac_2_0_Mobile)
					baselibdll = currentPlatform.GetPath( "lib", "reference", "mobile", "Xamarin.Mac.dll");
				else if (targetFramework == TargetFramework.Xamarin_Mac_4_5_Full ||
				         targetFramework == TargetFramework.Xamarin_Mac_4_5_System)
					baselibdll = currentPlatform.GetPath( "lib", "reference", "full", "Xamarin.Mac.dll");
				else if (targetFramework == TargetFramework.DotNet_macOS)
					baselibdll = currentPlatform.GetPath( "lib", "mono", "Xamarin.Mac", "Xamarin.Mac.dll");
				else
					throw ErrorHelper.CreateError(1053, targetFramework);
			}

			if (targetFramework == TargetFramework.Xamarin_Mac_2_0_Mobile)
			{
				skipSystemDrawing = true;
				references.Add("Facades/System.Drawing.Common");
				ReferenceFixer.FixSDKReferences(currentPlatform, "lib/mono/Xamarin.Mac", references);
			}
			else if (targetFramework == TargetFramework.Xamarin_Mac_4_5_Full)
			{
				skipSystemDrawing = true;
				references.Add("Facades/System.Drawing.Common");
				ReferenceFixer.FixSDKReferences(currentPlatform, "lib/mono/4.5", references);
			}
			else if (targetFramework == TargetFramework.Xamarin_Mac_4_5_System)
			{
				skipSystemDrawing = false;
				ReferenceFixer.FixSDKReferences("/Library/Frameworks/Mono.framework/Versions/Current/lib/mono/4.5",
					references, forceSystemDrawing: true);
			}
			else if (targetFramework == TargetFramework.DotNet_macOS)
			{
				skipSystemDrawing = false;
			}
			else
			{
				throw ErrorHelper.CreateError(1053, targetFramework);
			}

			break;
		default:
			throw ErrorHelper.CreateError(1053, targetFramework);
		}

		return nostdlib;
	}
}

