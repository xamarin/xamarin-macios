using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.iOS.Tasks.Windows.Properties;
using Xamarin.MacDev.Tasks;

#nullable enable

namespace Xamarin.iOS.HotRestart.Tasks {
	public class ComputeHotRestartBundleContents : Task {
		#region Inputs

		[Required]
		public string HotRestartAppContentDir { get; set; } = string.Empty;

		[Required]
		public string HotRestartContentDir { get; set; } = string.Empty;

		[Required]
		public string HotRestartContentStampDir { get; set; } = string.Empty;

		[Required]
		public string HotRestartAppBundlePath { get; set; } = string.Empty;

		[Required]
		public string RelativeAppBundlePath { get; set; } = string.Empty;

		[Required]
		public string TargetFrameworkMoniker { get; set; } = string.Empty;

		[Required]
		public ITaskItem [] ResolvedFileToPublish { get; set; } = Array.Empty<ITaskItem> ();

		#endregion

		#region Outputs

		[Output]
		public ITaskItem [] HotRestartAppContentDirContents { get; set; } = Array.Empty<ITaskItem> ();

		[Output]
		public ITaskItem [] HotRestartContentDirContents { get; set; } = Array.Empty<ITaskItem> ();

		[Output]
		public ITaskItem [] HotRestartAppBundleContents { get; set; } = Array.Empty<ITaskItem> ();

		#endregion

		ITaskItem CopyWithDestinationAndStamp (ITaskItem item, string destinationDirectory, string? stampDirectory = null)
		{
			var rv = new TaskItem (item);
			// The RelativePath metadata specifies the path of the item inside the app bundle relative to the output directory.
			// We need to convert this to a path relative to the root of the app bundle, since we copy files to a different
			// directory than the normal app bundle output path.
			var relativePath = item.GetMetadata ("RelativePath");
			if (relativePath.StartsWith (RelativeAppBundlePath, StringComparison.OrdinalIgnoreCase))
				relativePath = relativePath.Substring (RelativeAppBundlePath.Length).TrimStart ('\\', '/');
			relativePath = relativePath.Replace ('/', Path.DirectorySeparatorChar);
			// And here we compute the final absolute path of the item, into our own output directory.
			rv.SetMetadata ("DestinationFile", Path.Combine (destinationDirectory, relativePath));
			// Also set a stamp file metadata if we're supposed to write a stamp file.
			if (!string.IsNullOrEmpty (stampDirectory))
				rv.SetMetadata ("StampFile", Path.Combine (stampDirectory, relativePath));
			return rv;
		}

		// The Copy task can't copy directories, so expand directories to their individual files
		List<ITaskItem> ExpandDirectories (List<ITaskItem> items)
		{
			var rv = new List<ITaskItem> ();

			foreach (var item in items) {
				if (File.Exists (item.ItemSpec)) {
					rv.Add (item);
				} else if (Directory.Exists (item.ItemSpec)) {
					var entries = Directory.GetFileSystemEntries (item.ItemSpec, "*", SearchOption.AllDirectories).ToArray ();
					Log.LogMessage (MessageImportance.Low, $"Expanding {item.ItemSpec} with {entries.Length} items:");
					foreach (var entry in entries) {
						if (Directory.Exists (entry)) {
							Log.LogMessage (MessageImportance.Low, $"    Skipped directory: {entry}");
							continue;
						}
						var relativePathSuffix = entry.Substring (item.ItemSpec.Length).TrimStart ('\\', '/');
						var relativePath = Path.Combine (item.GetMetadata ("RelativePath"), relativePathSuffix);
						var destinationFile = Path.Combine (item.GetMetadata ("DestinationFile"), relativePathSuffix);
						var file = new TaskItem (item);
						file.ItemSpec = entry;
						file.SetMetadata ("RelativePath", relativePath);
						file.SetMetadata ("DestinationFile", destinationFile);
						rv.Add (file);
						Log.LogMessage (MessageImportance.Low, $"    Added {file.ItemSpec} with relative path: {relativePath} and destination file: {destinationFile}");
					}
				} else {
					// Trust that this will just somehow work.
					rv.Add (item);
				}
			}

			return rv;
		}

		public override bool Execute ()
		{
			var appContentDirContents = new List<ITaskItem> ();
			var contentDirContents = new List<ITaskItem> ();
			var appBundleContents = new List<ITaskItem> ();

			foreach (var item in ResolvedFileToPublish) {
				var publishFolderType = item.GetPublishFolderType ();
				switch (publishFolderType) {
				case PublishFolderType.RootDirectory:
				case PublishFolderType.Assembly:
				case PublishFolderType.Resource:
					appContentDirContents.Add (CopyWithDestinationAndStamp (item, HotRestartAppContentDir));
					contentDirContents.Add (CopyWithDestinationAndStamp (item, HotRestartContentDir, HotRestartContentStampDir));
					break;

				case PublishFolderType.AppleFramework:
					var filename = Path.GetFileName (item.ItemSpec);
					var dirname = Path.GetFileName (Path.GetDirectoryName (item.ItemSpec));
					if (string.Equals (filename + ".framework", dirname, StringComparison.OrdinalIgnoreCase))
						item.ItemSpec = Path.GetDirectoryName (item.ItemSpec);
					// These have to be signed
					appBundleContents.Add (CopyWithDestinationAndStamp (item, HotRestartAppBundlePath));
					break;
				case PublishFolderType.PlugIns:
				case PublishFolderType.DynamicLibrary:
				case PublishFolderType.PluginLibrary:
				case PublishFolderType.XpcServices:
					// These have to be signed
					appBundleContents.Add (CopyWithDestinationAndStamp (item, HotRestartAppBundlePath));
					break;

				case PublishFolderType.Unset: // Don't copy unknown stuff anywhere
				case PublishFolderType.None: // Don't copy unknown stuff anywhere
				case PublishFolderType.Unknown: // Don't copy unknown stuff anywhere
				case PublishFolderType.AppleBindingResourcePackage: // These aren't copied to the bundle
				case PublishFolderType.CompressedAppleBindingResourcePackage: // These aren't copied to the bundle
				case PublishFolderType.CompressedAppleFramework: // Shouldn't really happen? Should be uncompresed by the time we get here.
				case PublishFolderType.CompressedPlugIns: // Shouldn't really happen? Should be uncompresed by the time we get here.
				case PublishFolderType.CompressedXpcServices: // Shouldn't really happen? Should be uncompresed by the time we get here.
					Log.LogMessage (MessageImportance.Low, $"    Skipped {item.ItemSpec} because PublishFolderType={publishFolderType} items aren't copied to the app bundle.");
					continue;
				case PublishFolderType.StaticLibrary: // These aren't copied to the bundle
					Log.LogWarning (null, null, null, item.ItemSpec, 0, 0, 0, 0, Resources.HotRestartStaticLibraryNotSupported);
					continue;
				default:
					Log.LogMessage (MessageImportance.Low, $"    Skipped {item.ItemSpec} because of unknown PublishFolderType={publishFolderType}.");
					continue;
				}
			}

			appContentDirContents = ExpandDirectories (appContentDirContents);
			contentDirContents = ExpandDirectories (contentDirContents);
			appBundleContents = ExpandDirectories (appBundleContents);

			HotRestartAppContentDirContents = appContentDirContents.ToArray ();
			HotRestartContentDirContents = contentDirContents.ToArray ();
			HotRestartAppBundleContents = appBundleContents.ToArray ();

			return !Log.HasLoggedErrors;
		}
	}
}
