namespace Xharness.TestImporter {
	/// <summary>
	/// Represents the supported platforms to which we can create projects.
	/// </summary>
	public enum Platform {
		iOS,
		WatchOS,
		TvOS,
		MacOSFull,
		MacOSModern,
	}

	/// <summary>
	/// Represents the different types of wathcOS apps.
	/// </summary>
	public enum WatchAppType {
		App,
		Extension
	}
}