using System.Collections.Generic;

using ObjCRuntime;

#nullable enable

public partial class Frameworks {
	HashSet<string>? frameworks;

	public PlatformName CurrentPlatform { get; private set; }

	public Frameworks (PlatformName currentPlatform)
	{
		CurrentPlatform = currentPlatform;
	}

	bool GetValue (string framework)
	{
		if (frameworks is not null)
			return frameworks.Contains (framework);

		switch (CurrentPlatform) {
		case PlatformName.iOS:
			frameworks = iosframeworks;
			break;
		case PlatformName.WatchOS:
			frameworks = watchosframeworks;
			break;
		case PlatformName.TvOS:
			frameworks = tvosframeworks;
			break;
		case PlatformName.MacOSX:
			frameworks = macosframeworks;
			break;
		case PlatformName.MacCatalyst:
			frameworks = maccatalystframeworks;
			break;
		default:
			throw new BindingException (1047, CurrentPlatform);
		}

		return frameworks.Contains (framework);
	}
}
