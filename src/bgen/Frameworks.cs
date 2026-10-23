using System.Collections.Generic;

#nullable enable

public partial class BGenFrameworks {
	HashSet<string>? frameworks;

	public PlatformName CurrentPlatform { get; private set; }

	public BGenFrameworks (PlatformName currentPlatform)
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
