using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using Microsoft.Build.Framework;

using Xamarin.Utils;

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

		public static bool IsIllegalName (string? name, [NotNullWhen (true)] out string? illegal)
		{
			if (name is null) {
				illegal = null;
				return false;
			}

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

		public static IList<string> SplitResourcePrefixes (string prefix)
		{
			return prefix.Split (new [] { ';' }, StringSplitOptions.RemoveEmptyEntries)
				.Select (s => s.Replace ('\\', Path.DirectorySeparatorChar).Trim () + Path.DirectorySeparatorChar)
				.Where (s => s.Length > 1)
				.ToList ();
		}

		public static string GetVirtualProjectPath (string projectDir, ITaskItem item, bool isVSBuild)
		{
			var link = item.GetMetadata ("Link");

			// Note: if the Link metadata exists, then it will be the equivalent of the ProjectVirtualPath
			if (!string.IsNullOrEmpty (link)) {
				if (Path.DirectorySeparatorChar != '\\')
					return link.Replace ('\\', '/');

				return link;
			}

			// HACK: This is for Visual Studio iOS projects
			if (isVSBuild) {
				if (item.GetMetadata ("DefiningProjectFullPath") != item.GetMetadata ("MSBuildProjectFullPath")) {
					return item.GetMetadata ("FullPath").Replace (item.GetMetadata ("DefiningProjectDirectory"), string.Empty);
				} else {
					return item.ItemSpec;
				}
			}

			var isDefaultItem = item.GetMetadata ("IsDefaultItem") == "true";
			var definingProjectFullPath = item.GetMetadata (isDefaultItem ? "MSBuildProjectFullPath" : "DefiningProjectFullPath");
			var path = item.GetMetadata ("FullPath");
			string baseDir;

			if (!string.IsNullOrEmpty (definingProjectFullPath)) {
				baseDir = Path.GetDirectoryName (definingProjectFullPath);
			} else {
				baseDir = projectDir;
			}

			baseDir = PathUtils.ResolveSymbolicLinks (baseDir);
			path = PathUtils.ResolveSymbolicLinks (path);

			return PathUtils.AbsoluteToRelative (baseDir, path);
		}

		public static string GetLogicalName (string projectDir, IList<string> prefixes, ITaskItem item, bool isVSBuild)
		{
			var logicalName = item.GetMetadata ("LogicalName");

			if (!string.IsNullOrEmpty (logicalName)) {
				if (Path.DirectorySeparatorChar != '\\')
					return logicalName.Replace ('\\', '/');

				return logicalName;
			}

			var vpath = GetVirtualProjectPath (projectDir, item, isVSBuild);
			int matchlen = 0;

			foreach (var prefix in prefixes) {
				if (vpath.StartsWith (prefix, StringComparison.OrdinalIgnoreCase) && prefix.Length > matchlen)
					matchlen = prefix.Length;
			}

			if (matchlen > 0)
				return vpath.Substring (matchlen);

			return vpath;
		}
	}
}
