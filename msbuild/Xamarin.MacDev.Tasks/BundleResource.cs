#define TRACE

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.Localization.MSBuild;
using Xamarin.Utils;
using Xamarin.MacDev.Tasks;

#nullable enable

namespace Xamarin.MacDev {
	public static class BundleResource {
		static readonly HashSet<string> illegalDirectoryNames = new HashSet<string> (new [] {
			"Resources",
			"_CodeSignature",
		}, StringComparer.OrdinalIgnoreCase);

		static readonly HashSet<string> illegalFileNames = new HashSet<string> (new [] {
			"Info.plist",
			"embedded.mobileprovision",
			"ResourceRules.plist",
			"PkgInfo",
			"CodeResources",
			"_CodeSignature",
		}, StringComparer.OrdinalIgnoreCase);

		public static bool IsIllegalName (string name, [NotNullWhen (true)] out string? illegal)
		{
			if (illegalFileNames.Contains (name)) {
				illegal = name;
				return true;
			}

			int delim = name.IndexOf (Path.DirectorySeparatorChar);

			if (delim == -1 && illegalDirectoryNames.Contains (name)) {
				illegal = name;
				return true;
			}

			if (delim != -1 && illegalDirectoryNames.Contains (name.Substring (0, delim))) {
				illegal = name.Substring (0, delim);
				return true;
			}

			illegal = null;

			return false;
		}

		public static IList<string> SplitResourcePrefixes (string? prefix)
		{
			if (prefix is null)
				return Array.Empty<string> ();

			return prefix.Split (new [] { ';' }, StringSplitOptions.RemoveEmptyEntries)
				.Select (s => s.Replace ('\\', Path.DirectorySeparatorChar).Trim () + Path.DirectorySeparatorChar)
				.Where (s => s.Length > 1)
				.ToList ();
		}

		[Conditional ("TRACE")]
		static void Trace (Task task, string msg)
		{
			task.Log.LogMessage (MessageImportance.Low, msg);
		}

		// Compute the path of 'item' relative to the project.
		public static string GetVirtualProjectPath<T> (T task, ITaskItem item) where T : Task, IHasProjectDir, IHasSessionId
		{
			// If the Link metadata exists, use that, it takes precedence over anything else.
			var link = item.GetMetadata ("Link");
			if (!string.IsNullOrEmpty (link)) {
				// Canonicalize to use macOS-style directory separators.
				link = link.Replace ('\\', '/');
				Trace (task, $"BundleResource.GetVirtualProjectPath ({item.ItemSpec}) => Link={link}");
				return link;
			}

			// Note that '/' is a valid path separator on Windows (in addition to '\'), so canonicalize the paths to use '/' as the path separator.

			var isDefaultItem = item.GetMetadata ("IsDefaultItem") == "true";
			var localMSBuildProjectFullPath = item.GetMetadata ("LocalMSBuildProjectFullPath").Replace ('\\', '/');
			var localDefiningProjectFullPath = item.GetMetadata ("LocalDefiningProjectFullPath").Replace ('\\', '/');
			if (string.IsNullOrEmpty (localDefiningProjectFullPath)) {
				task.Log.LogError (null, null, null, item.ItemSpec, 0, 0, 0, 0, MSBStrings.E7133 /* The item '{0}'' does not have a '{1}' value set. */, item.ItemSpec, "LocalDefiningProjectFullPath");
				return "placeholder";
			}

			if (string.IsNullOrEmpty (localMSBuildProjectFullPath)) {
				task.Log.LogError (null, null, null, item.ItemSpec, 0, 0, 0, 0, MSBStrings.E7133 /* The item '{0}'' does not have a '{1}' value set. */, item.ItemSpec, "LocalMSBuildProjectFullPath");
				return "placeholder";
			}

			// * If we're not a default item, compute the path relative to the
			//   file that declared the item in question.
			// * If we're a default item (IsDefaultItem=true), compute
			//   relative to the user's project file (because the file that
			//   declared the item is our Microsoft.Sdk.DefaultItems.template.props file,
			//   and the path relative to that file is certainly not what we want).
			//
			// We use the 'LocalMSBuildProjectFullPath' and
			// 'LocalDefiningProjectFullPath' metadata because the
			// 'MSBuildProjectFullPath' and 'DefiningProjectFullPath' are not
			// necessarily correct when building remotely (the relative path
			// between files might not be the same on macOS once XVS has
			// copied them there, in particular for files outside the project
			// directory).
			//
			// The 'LocalMSBuildProjectFullPath' and 'LocalDefiningProjectFullPath'
			// values are set to the Windows version of 'MSBuildProjectFullPath'
			// and 'DefiningProjectFullPath' when building remotely, and the macOS
			// version when building on macOS.

			// First find the absolute path to the item
			var projectAbsoluteDir = task.ProjectDir;
			var isRemoteBuild = !string.IsNullOrEmpty (task.SessionId);
			string itemAbsolutePath;
			if (isRemoteBuild) {
				itemAbsolutePath = PathUtils.PathCombineWindows (projectAbsoluteDir, item.ItemSpec);
			} else {
				itemAbsolutePath = Path.Combine (projectAbsoluteDir, item.ItemSpec);
			}
			var originalItemAbsolutePath = itemAbsolutePath;

			// Then find the directory we should use to compute the result relative to.
			string relativeToDirectory; // this is an absolute path.
			if (isDefaultItem) {
				relativeToDirectory = Path.GetDirectoryName (localMSBuildProjectFullPath);
			} else {
				relativeToDirectory = Path.GetDirectoryName (localDefiningProjectFullPath);
			}
			var originalRelativeToDirectory = relativeToDirectory;

			// On macOS we need to resolve symlinks before computing the relative path.
			if (!isRemoteBuild) {
				relativeToDirectory = PathUtils.ResolveSymbolicLinks (relativeToDirectory);
				itemAbsolutePath = PathUtils.ResolveSymbolicLinks (itemAbsolutePath);
			}

			// Compute the relative path we want to return.
			string rv;
			if (isRemoteBuild) {
				rv = PathUtils.AbsoluteToRelativeWindows (relativeToDirectory, itemAbsolutePath);
			} else {
				rv = PathUtils.AbsoluteToRelative (relativeToDirectory, itemAbsolutePath);
			}
			// Make it a mac-style path
			rv = rv.Replace ('\\', '/');

			Trace (task, $"BundleResource.GetVirtualProjectPath ({item.ItemSpec}) => {rv}\n" +
					$"\t\t\tprojectAbsoluteDir={projectAbsoluteDir}\n" +
					$"\t\t\tIsRemoteBuild={isRemoteBuild}\n" +
					$"\t\t\tisDefaultItem={isDefaultItem}\n" +
					$"\t\t\tLocalMSBuildProjectFullPath={localMSBuildProjectFullPath}\n" +
					$"\t\t\tLocalDefiningProjectFullPath={localDefiningProjectFullPath}\n" +
					$"\t\t\toriginalItemAbsolutePath={originalItemAbsolutePath}\n" +
					$"\t\t\titemAbsolutePath={itemAbsolutePath}\n" +
					$"\t\t\toriginalRelativeToDirectory={originalRelativeToDirectory}\n" +
					$"\t\t\trelativeToDirectory={relativeToDirectory}\n");

			return rv;
		}

		public static string GetLogicalName<T> (T task, ITaskItem item) where T : Task, IHasProjectDir, IHasResourcePrefix, IHasSessionId
		{
			var logicalName = item.GetMetadata ("LogicalName");

			// If an item has the LogicalName metadata set, return that.
			if (!string.IsNullOrEmpty (logicalName)) {
				Trace (task, $"BundleResource.GetLogicalName ({item.ItemSpec}) => has LogicalName={logicalName.Replace ('\\', '/')} (original {logicalName})");
				// Canonicalize to use macOS-style directory separators.
				return logicalName.Replace ('\\', '/');
			}

			// Check if the start of the item matches any of the resource prefixes, in which case choose
			// the longest resource prefix, and subtract it from the start of the item.
			var vpath = GetVirtualProjectPath (task, item);
			int matchlen = 0;
			var prefixes = SplitResourcePrefixes (task.ResourcePrefix);
			foreach (var prefix in prefixes) {
				if (vpath.StartsWith (prefix, StringComparison.OrdinalIgnoreCase) && prefix.Length > matchlen)
					matchlen = prefix.Length;
			}
			if (matchlen > 0) {
				Trace (task, $"BundleResource.GetLogicalName ({item.ItemSpec}) => LogicalName={vpath.Substring (matchlen)} (vpath={vpath} matchlen={matchlen} prefixes={string.Join (",", prefixes)})");
				return vpath.Substring (matchlen);
			}

			// Otherwise return the item as-is.
			Trace (task, $"BundleResource.GetLogicalName ({item.ItemSpec}) => LogicalName={vpath} (prefixes={string.Join (",", prefixes)})");
			return vpath;
		}
	}
}
