#nullable enable

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.MacDev;
using Xamarin.Utils;
using Xamarin.Localization.MSBuild;

namespace Xamarin.MacDev.Tasks {
	public class ComputeBundleLocation : XamarinTask {
		// not required because this can be the root directory (so an empty string)
		public string AssemblyDirectory { get; set; } = string.Empty;

		public ITaskItem []? BundleResource { get; set; }
		public ITaskItem []? Content { get; set; }
		public ITaskItem []? EmbeddedResource { get; set; }

		[Required]
		public string FrameworksDirectory { get; set; } = string.Empty;

		[Required]
		public bool BundlerDebug { get; set; }

		public string PackageDebugSymbols { get; set; } = string.Empty;

		[Required]
		public string PlugInsDirectory { get; set; } = string.Empty;

		[Required]
		public string ProjectDir { get; set; } = string.Empty;

		// not required because this can be the root directory (so an empty string)
		public string ResourceDirectory { get; set; } = string.Empty;

		[Required]
		public ITaskItem []? ResolvedFileToPublish { get; set; }

		[Output]
		public ITaskItem []? UpdatedResolvedFileToPublish { get; set; }

		HashSet<string> resourceFilesSet = new HashSet<string> ();

		// We package the symbols if the PackageDebugSymbols is set to 'true', we don't if set to anything else, and if set to
		// nothing, then we package symbols unless we're doing a release build.
		bool PackageSymbols {
			get {
				if (!string.IsNullOrEmpty (PackageDebugSymbols))
					return string.Equals ("true", PackageDebugSymbols, StringComparison.OrdinalIgnoreCase);
				return BundlerDebug;
			}
		}

		void AddResourceFiles (ITaskItem []? items)
		{
			if (items is null || items.Length == 0)
				return;

			var resources = items.
				// Remove any items with PublishFolderType set
				Where (v => string.IsNullOrEmpty (v.GetMetadata ("PublishFolderType"))).
				// Get the full path
				Select (v => Path.GetFullPath (v.ItemSpec));

			resourceFilesSet.UnionWith (resources);
		}

		public override bool Execute ()
		{
			if (ResolvedFileToPublish is null || ResolvedFileToPublish.Length == 0)
				return !Log.HasLoggedErrors;

			// Make sure we use the correct path separator, these are relative paths, so it doesn't look
			// like MSBuild does the conversion automatically.
			FrameworksDirectory = FrameworksDirectory.Replace ('\\', Path.DirectorySeparatorChar);
			PlugInsDirectory = PlugInsDirectory.Replace ('\\', Path.DirectorySeparatorChar);
			ResourceDirectory = ResourceDirectory.Replace ('\\', Path.DirectorySeparatorChar);

			// Collect all our BundleResource, Content and EmbeddedResource paths into one big dictionary for later lookup.
			AddResourceFiles (BundleResource);
			AddResourceFiles (Content);
			AddResourceFiles (EmbeddedResource);

			var appleFrameworks = new Dictionary<string, List<ITaskItem>> ();
			var list = ResolvedFileToPublish.ToList ();
			foreach (var item in list.ToArray ()) { // iterate over a copy of the list, because we might modify the original list
													// Compute the publish folder type if it's not specified
				var publishFolderType = item.GetPublishFolderType ();
				if (publishFolderType == PublishFolderType.Unset) {
					publishFolderType = ComputePublishFolderType (list, item);
					item.SetMetadata ("PublishFolderType", publishFolderType.ToString ());
				}

				// Figure out the relative directory inside the app bundle where the item is supposed to be placed.
				var relativePath = string.Empty;
				var virtualProjectPath = GetVirtualAppBundlePath (item);
				switch (publishFolderType) {
				case PublishFolderType.Assembly:
					relativePath = AssemblyDirectory;
					break;
				case PublishFolderType.Resource:
					relativePath = ResourceDirectory;
					break;
				case PublishFolderType.AppleFramework:
					if (TryGetFrameworkDirectory (item.ItemSpec, out var frameworkDirectory)) {
						if (!appleFrameworks.TryGetValue (frameworkDirectory!, out var items))
							appleFrameworks [frameworkDirectory!] = items = new List<ITaskItem> ();
						items.Add (item);
						// Remove AppleFramework entries, we'll add back one entry per framework at the end
						list.Remove (item);
						continue;
					}
					Log.LogError (7094, item.ItemSpec, MSBStrings.E7094 /* The file or directory '{0}' is not a framework nor a file within a framework. */, item.ItemSpec);
					continue;
				case PublishFolderType.CompressedAppleFramework:
					relativePath = FrameworksDirectory;
					virtualProjectPath = Path.GetFileNameWithoutExtension (item.ItemSpec);
					if (virtualProjectPath.EndsWith (".xcframework", StringComparison.OrdinalIgnoreCase))
						virtualProjectPath = Path.ChangeExtension (virtualProjectPath, ".framework");
					break;
				case PublishFolderType.AppleBindingResourcePackage:
					// Nothing to do here, this is handled fully in the targets file
					break;
				case PublishFolderType.CompressedAppleBindingResourcePackage:
					// Nothing to do here, this is handled fully in the targets file
					virtualProjectPath = RemoveExtension (virtualProjectPath, ".zip");
					break;
				case PublishFolderType.PlugIns:
					relativePath = PlugInsDirectory;
					break;
				case PublishFolderType.CompressedPlugIns:
					relativePath = PlugInsDirectory;
					virtualProjectPath = string.Empty;
					break;
				case PublishFolderType.RootDirectory:
					break;
				case PublishFolderType.DynamicLibrary:
					relativePath = AssemblyDirectory;
					break;
				case PublishFolderType.PluginLibrary:
					relativePath = AssemblyDirectory;
					break;
				case PublishFolderType.StaticLibrary:
					// Nothing to do here.
					continue;
				case PublishFolderType.None:
					continue;
				case PublishFolderType.Unknown:
				default:
					ReportUnknownPublishFolderType (item);
					item.SetMetadata ("PublishFolderType", "None");
					continue;
				}

				// Compute the relative path of the item relative to the root of the app bundle
				relativePath = Path.Combine (relativePath, virtualProjectPath);
				item.SetMetadata ("RelativePath", relativePath);
			}

			// We may have multiple input items for each framework, but we only want to return a single
			// entry per framework. In the loop above we removed all input items corresponding with a 
			// framework, so add back a single item here.
			foreach (var entry in appleFrameworks) {
				var items = entry.Value;
				var item = new TaskItem (entry.Key);
				item.SetMetadata ("PublishFolderType", "AppleFramework");
				item.SetMetadata ("RelativePath", Path.Combine (FrameworksDirectory, Path.ChangeExtension (Path.GetFileName (entry.Key), "framework")));
				list.Add (item);
			}

			UpdatedResolvedFileToPublish = list.ToArray ();

			return !Log.HasLoggedErrors;
		}

		static string RemoveExtension (string path, string extension)
		{
			if (path.EndsWith (extension, StringComparison.OrdinalIgnoreCase))
				return path.Substring (0, path.Length - extension.Length);
			return path;
		}

		// Check if the input, or any of it's parent directories is either an *.xcframework, or a *.framework
		static bool TryGetFrameworkDirectory (string path, out string? frameworkDirectory)
		{
			if (string.IsNullOrEmpty (path)) {
				frameworkDirectory = null;
				return false;
			}

			if (path.EndsWith (".xcframework", StringComparison.OrdinalIgnoreCase)) {
				frameworkDirectory = path;
				return true;
			}

			if (path.EndsWith (".framework", StringComparison.OrdinalIgnoreCase)) {
				// We might be inside a .xcframework, so check for that first
				if (TryGetFrameworkDirectory (Path.GetDirectoryName (path), out var xcframeworkDirectory) && xcframeworkDirectory!.EndsWith (".xcframework", StringComparison.OrdinalIgnoreCase)) {
					frameworkDirectory = xcframeworkDirectory;
					return true;
				}

				frameworkDirectory = path;
				return true;
			}

			return TryGetFrameworkDirectory (Path.GetDirectoryName (path), out frameworkDirectory);
		}

		// Check if the input, or any of it's parent directories is a *.resources directory or a *.resources.zip file next to a *.dll.
		static bool IsBindingResourcePackage (string path, out PublishFolderType type)
		{
			type = PublishFolderType.None;
			if (string.IsNullOrEmpty (path))
				return false;

			if (path.EndsWith (".resources", StringComparison.OrdinalIgnoreCase) && File.Exists (Path.ChangeExtension (path, "dll"))) {
				type = PublishFolderType.AppleBindingResourcePackage;
				return true;
			}

			if (path.EndsWith (".resources.zip", StringComparison.OrdinalIgnoreCase) && File.Exists (Path.ChangeExtension (Path.GetFileNameWithoutExtension (path), "dll"))) {
				type = PublishFolderType.CompressedAppleBindingResourcePackage;
				return true;
			}

			return IsBindingResourcePackage (Path.GetDirectoryName (path), out type);
		}

		static string GetVirtualAppBundlePath (ITaskItem item)
		{
			// We need to take "TargetPath" into account - this is path of the file relative to the output directory, and may also change the filename itself (it's for instance used to rename 'app.config' to the 'mainassembly.exe.config').
			// If "TargetPath" is specified, we rename the item to have "TargetPath" as the file name (the rest of the path is kept).
			// This value takes precedence over the "Link" metadata (https://github.com/dotnet/msbuild/issues/2795)
			var targetPath = item.GetMetadata ("TargetPath");
			if (!string.IsNullOrEmpty (targetPath))
				return targetPath;

			// If there's no "TargetPath" metadata, then we check the "Link" metadata, which works the same way as "TargetPath" otherwise.
			var link = item.GetMetadata ("Link");
			if (!string.IsNullOrEmpty (link))
				return link;

			var virtualPath = Path.GetFileName (item.ItemSpec);

			// If neither "TargetPath" nor "Link" is set, we need to take "DestinationSubDirectory" into account - this is used to specify the subdirectory for resource assemblies for instance.
			// Ref: https://github.com/dotnet/sdk/blob/0fc72ddb758dd136182972c2aea1d504ea046cfd/src/Tasks/Common/ItemUtilities.cs#L126-L128
			// Contrary to the "TargetPath" and "Link" metadata, this value doesn't specify the filename itself, only the containing directory name.
			var destinationSubDirectory = item.GetMetadata ("DestinationSubDirectory");
			if (!string.IsNullOrEmpty (destinationSubDirectory))
				virtualPath = Path.Combine (destinationSubDirectory, virtualPath);

			return virtualPath;
		}

		void ReportUnknownPublishFolderType (ITaskItem item)
		{
			var publishFolderType = item.GetMetadata ("PublishFolderType");

			var metadata = item.GetMetadata ("CopyToOutputDirectory");
			if (!string.IsNullOrEmpty (metadata)) {
				Log.LogWarning (MSBStrings.E7090 /* The 'PublishFolderType' metadata value '{0}' on the item '{1}' is not recognized. The file will not be copied to the app bundle. If the file is not supposed to be copied to the app bundle, remove the '{2}' metadata on the item. */, publishFolderType, item.ItemSpec, "CopyToOutputDirectory");
				return;
			}

			metadata = item.GetMetadata ("CopyToPublishDirectory");
			if (!string.IsNullOrEmpty (metadata)) {
				Log.LogWarning (MSBStrings.E7090 /* The 'PublishFolderType' metadata value '{0}' on the item '{1}' is not recognized. The file will not be copied to the app bundle. If the file is not supposed to be copied to the app bundle, remove the '{2}' metadata on the item. */, publishFolderType, item.ItemSpec, "CopyToPublishDirectory");
				return;
			}

			Log.LogWarning (MSBStrings.E7088 /* The 'PublishFolderType' metadata value '{0}' on the item '{1}' is not recognized. The file will not be copied to the app bundle. */, publishFolderType, item.ItemSpec);
		}

		// 'item' is not supposed to have a PublishFolderType set
		PublishFolderType ComputePublishFolderType (IList<ITaskItem> items, ITaskItem item)
		{
			var filename = item.ItemSpec;
			var targetPath = item.GetMetadata ("TargetPath");
			if (!string.IsNullOrEmpty (targetPath))
				filename = Path.Combine (Path.GetDirectoryName (filename), Path.GetFileName (targetPath));

			// Check if the item came from @(BundleResource), @(Content) or @(EmbeddedResource)
			if (resourceFilesSet.Contains (Path.GetFullPath (item.ItemSpec)))
				return PublishFolderType.Resource;

			// Assemblies and their related files
			var assemblyExtensions = new string [] {
				".dll", ".exe", ".config",
			};
			foreach (var extension in assemblyExtensions) {
				if (filename.EndsWith (extension, StringComparison.OrdinalIgnoreCase))
					return PublishFolderType.Assembly;
			}

			// Assemblies and their related files
			var assemblyDebugExtensions = new string [] {
				".pdb", ".dll.mdb", ".exe.mdb",
			};
			foreach (var extension in assemblyDebugExtensions) {
				if (filename.EndsWith (extension, StringComparison.OrdinalIgnoreCase))
					return PackageSymbols ? PublishFolderType.Assembly : PublishFolderType.None;
			}

			// If an xml file matches the filename of any assembly, then treat that xml file as PublishFolderType=None
			if (filename.EndsWith (".xml", StringComparison.OrdinalIgnoreCase)) {
				var baseName = Path.GetFileNameWithoutExtension (filename);
				if (items.Any (v => {
					var fn = Path.GetFileName (v.ItemSpec);
					if (fn.Length != baseName.Length + 4)
						return false;

					if (!(fn.EndsWith (".exe", StringComparison.OrdinalIgnoreCase) || fn.EndsWith (".dll", StringComparison.OrdinalIgnoreCase)))
						return false;

					return fn.StartsWith (baseName, StringComparison.OrdinalIgnoreCase);
				})) {
					return PublishFolderType.None;
				}
			}

			// Binding resource package (*.resources / *.resources.zip)
			if (IsBindingResourcePackage (filename, out var type))
				return type;

			// Native (xc)frameworks.
			// We do this after checking for binding resource packages, because those might contain frameworks.
			if (TryGetFrameworkDirectory (filename, out _))
				return PublishFolderType.AppleFramework;

			// resources (png, jpg, ...?)
			var resourceExtensions = new string [] {
				".jpg",
				".png",
			};
			foreach (var extension in resourceExtensions) {
				if (filename.EndsWith (extension, StringComparison.OrdinalIgnoreCase))
					return PublishFolderType.Resource;
			}

			// *.framework.zip, *.xcframework.zip
			var compressedAppleFrameworksExtensions = new string [] {
				".framework.zip",
				".xcframework.zip",
			};
			foreach (var extension in compressedAppleFrameworksExtensions) {
				if (filename.EndsWith (extension, StringComparison.OrdinalIgnoreCase))
					return PublishFolderType.CompressedAppleFramework;
			}

			// *.a, *.dylib and *.so
			if (filename.EndsWith (".a", StringComparison.OrdinalIgnoreCase)) {
				return PublishFolderType.StaticLibrary;
			} else if (filename.EndsWith (".dylib", StringComparison.OrdinalIgnoreCase)) {
				return PublishFolderType.DynamicLibrary;
			} else if (filename.EndsWith (".so", StringComparison.OrdinalIgnoreCase)) {
				return PublishFolderType.PluginLibrary;
			}

			// no other files are copied

			Log.LogWarning (MSBStrings.E7089 /* The file '{0}' does not specify a 'PublishFolderType' metadata, and a default value could not be calculated. The file will not be copied to the app bundle. */, item.ItemSpec);

			return PublishFolderType.None;
		}
	}
}
