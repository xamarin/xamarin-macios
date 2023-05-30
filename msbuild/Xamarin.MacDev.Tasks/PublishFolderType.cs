using System;

using Microsoft.Build.Framework;

#nullable enable

namespace Xamarin.MacDev.Tasks {

	public enum PublishFolderType {
		Unset,
		None,
		RootDirectory,
		Assembly,
		Resource,
		AppleBindingResourcePackage,
		CompressedAppleBindingResourcePackage,
		AppleFramework,
		CompressedAppleFramework,
		PlugIns,
		CompressedPlugIns,
		DynamicLibrary, // link with + copy to app bundle
		PluginLibrary, // copy to app bundle (but not link with main executable)
		StaticLibrary, // link with (but not copy to app bundle)
		XpcServices,
		CompressedXpcServices,
		Unknown,
	}

	public static class PublishFolderType_Extensions {
		public static PublishFolderType GetPublishFolderType (this ITaskItem item)
		{
			return ParsePublishFolderType (item.GetMetadata ("PublishFolderType"));
		}

		public static PublishFolderType ParsePublishFolderType (string value)
		{
			if (string.IsNullOrEmpty (value))
				return PublishFolderType.Unset;

			if (!Enum.TryParse<PublishFolderType> (value, true, out var result))
				result = PublishFolderType.Unknown;

			return result;
		}
	}
}
