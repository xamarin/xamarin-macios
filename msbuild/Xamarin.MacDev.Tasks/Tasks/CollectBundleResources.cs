using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Xamarin.Localization.MSBuild;
using Xamarin.Messaging.Build.Client;
using Xamarin.Utils;

namespace Xamarin.MacDev.Tasks {
	public class CollectBundleResources : XamarinTask, ICancelableTask {
		#region Inputs

		public ITaskItem [] BundleResources { get; set; } = Array.Empty<ITaskItem> ();

		public bool OptimizePropertyLists { get; set; }

		public bool OptimizePNGs { get; set; }

		[Required]
		public string ProjectDir { get; set; } = string.Empty;

		[Required]
		public string ResourcePrefix { get; set; } = string.Empty;

		#endregion

		#region Outputs

		[Output]
		public ITaskItem [] BundleResourcesWithLogicalNames { get; set; } = Array.Empty<ITaskItem> ();

		public ITaskItem [] UnpackedResources { get; set; } = Array.Empty<ITaskItem> ();

		#endregion

		static bool CanOptimize (string path)
		{
			switch (Path.GetExtension (path).ToLowerInvariant ()) {
			case ".png": case ".plist": case ".strings": return true;
			default: return false;
			}
		}

		public override bool Execute ()
		{
			try {
				if (ShouldExecuteRemotely ()) {
					// Copy the bundle files to the build server
					new TaskRunner (SessionId, BuildEngine4).CopyInputsAsync (this).Wait ();
				}

				// But execute locally
				return ExecuteImpl ();
			} catch (PathTooLongException ptle) when (Environment.OSVersion.Platform == PlatformID.Win32NT && !PathUtils.OSSupportsLongPaths) {
				Log.LogError (MSBStrings.E7122 /* A path exceeding max path was detected. Enabling long paths in Windows may help. For more information see https://learn.microsoft.com/en-us/windows/win32/fileio/maximum-file-path-limitation. */);
				Log.LogErrorFromException (ptle); // report the original exception too.
				return false;
			} catch (Exception ex) {
				Log.LogErrorFromException (ex);

				return false;
			}
		}

		bool ExecuteImpl ()
		{
			var prefixes = BundleResource.SplitResourcePrefixes (ResourcePrefix);
			var bundleResources = new List<ITaskItem> ();

			foreach (var item in BundleResources) {
				if (!TryCreateItemWithLogicalName (this, item, ProjectDir, prefixes, SessionId, out var bundleResource))
					continue;

				bool optimize = false;

				if (CanOptimize (item.ItemSpec)) {
					var metadata = item.GetMetadata ("Optimize");

					// fall back to old metadata name
					if (string.IsNullOrEmpty (metadata))
						metadata = item.GetMetadata ("OptimizeImage");

					if (string.IsNullOrEmpty (metadata) || !bool.TryParse (metadata, out optimize)) {
						switch (Path.GetExtension (item.ItemSpec).ToLowerInvariant ()) {
						case ".plist": case ".strings": optimize = OptimizePropertyLists; break;
						case ".png": optimize = OptimizePNGs; break;
						}
					}
				}

				bundleResource.SetMetadata ("Optimize", optimize.ToString ());

				bundleResources.Add (bundleResource);
			}

			bundleResources.AddRange (UnpackedResources);

			BundleResourcesWithLogicalNames = bundleResources.ToArray ();

			return !Log.HasLoggedErrors;
		}

		public static bool TryCreateItemWithLogicalName (Task task, ITaskItem item, string projectDir, IList<string> prefixes, string sessionId, [NotNullWhen (true)] out TaskItem? itemWithLogicalName)
		{
			itemWithLogicalName = null;

			// Skip anything with the PublishFolderType metadata, these are copied directly to the ResolvedFileToPublish item group instead.
			var publishFolderType = item.GetMetadata ("PublishFolderType");
			if (!string.IsNullOrEmpty (publishFolderType))
				return false;

			var logicalName = BundleResource.GetLogicalName (task, projectDir, prefixes, item);
			// We need a physical path here, ignore the Link element
			var path = item.GetMetadata ("FullPath");

			if (!File.Exists (path)) {
				task.Log.LogError (MSBStrings.E0099, logicalName, path);
				return false;
			}

			if (logicalName.StartsWith (".." + Path.DirectorySeparatorChar, StringComparison.Ordinal)) {
				task.Log.LogError (null, null, null, item.ItemSpec, 0, 0, 0, 0, MSBStrings.E0100, logicalName);
				return false;
			}

			if (logicalName == "Info.plist") {
				task.Log.LogWarning (null, null, null, item.ItemSpec, 0, 0, 0, 0, MSBStrings.E0101);
				return false;
			}

			if (BundleResource.IsIllegalName (logicalName, out var illegal)) {
				task.Log.LogError (null, null, null, item.ItemSpec, 0, 0, 0, 0, MSBStrings.E0102, illegal);
				return false;
			}

			itemWithLogicalName = new TaskItem (item);
			itemWithLogicalName.SetMetadata ("LogicalName", logicalName);
			return true;
		}

		public void Cancel ()
		{
			if (ShouldExecuteRemotely ())
				BuildConnection.CancelAsync (BuildEngine4).Wait ();
		}
	}
}
