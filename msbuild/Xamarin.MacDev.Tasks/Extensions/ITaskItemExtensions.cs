namespace Microsoft.Build.Framework {
	public static class ITaskItemExtensions {
		const string FrameworkFileMetadataKey = "FrameworkFile";

		public static bool IsFrameworkItem (this ITaskItem item)
		{
			return (bool.TryParse (item.GetMetadata (FrameworkFileMetadataKey), out var isFrameworkFile) && isFrameworkFile) ||
				item.GetMetadata ("ResolvedFrom") == "{TargetFrameworkDirectory}" ||
				item.GetMetadata ("ResolvedFrom") == "ImplicitlyExpandDesignTimeFacades";
		}
	}
}
