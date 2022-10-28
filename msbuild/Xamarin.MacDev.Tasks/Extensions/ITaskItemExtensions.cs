using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Build.Framework {
	public static class ITaskItemExtensions {
		const string FrameworkFileMetadataKey = "FrameworkFile";

		public static bool IsFrameworkItem (this ITaskItem item)
		{
			return (bool.TryParse (item.GetMetadata (FrameworkFileMetadataKey), out var isFrameworkFile) && isFrameworkFile) ||
				item.GetMetadata ("ResolvedFrom") == "{TargetFrameworkDirectory}" ||
				item.GetMetadata ("ResolvedFrom") == "ImplicitlyExpandDesignTimeFacades";
		}

		public static void SetMetadataIfNotSet (this ITaskItem self, string metadata, string value)
		{
			if (!string.IsNullOrEmpty (self.GetMetadata (metadata)))
				return;
			self.SetMetadata (metadata, value);
		}

		public static Dictionary<string, string> CloneCustomMetadataToDictionary (this ITaskItem item)
		{
			var custom = item.CloneCustomMetadata ();
			var rv = new Dictionary<string, string> (custom.Count, StringComparer.OrdinalIgnoreCase);
			foreach (DictionaryEntry entry in custom)
				rv [(string) entry.Key] = (string) entry.Value;
			return rv;
		}
	}
}
