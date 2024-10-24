using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.Utils;
using Xamarin.MacDev.Tasks;

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

		public static IList<string> SplitResourcePrefixes (string prefix)
		{
			return prefix.Split (new [] { ';' }, StringSplitOptions.RemoveEmptyEntries)
				.Select (s => s.Replace ('\\', Path.DirectorySeparatorChar).Trim () + Path.DirectorySeparatorChar)
				.Where (s => s.Length > 1)
				.ToList ();
		}

		static void Log (Task? task, string msg)
		{
			if (task is not null) {
				task.Log.LogMessage (MessageImportance.Low, msg);
			} else {
				Console.WriteLine (msg);
			}
		}

		public static string GetVirtualProjectPath (XamarinTask task, string projectDir, ITaskItem item)
		{
			return GetVirtualProjectPath (task, projectDir, item, !string.IsNullOrEmpty (task.SessionId));
		}

		public static string GetVirtualProjectPath (XamarinToolTask task, string projectDir, ITaskItem item)
		{
			return GetVirtualProjectPath (task, projectDir, item, !string.IsNullOrEmpty (task.SessionId));
		}

		public static string GetVirtualProjectPath (Task task, string projectDir, ITaskItem item, bool isVSBuild)
		{
			var link = item.GetMetadata ("Link");

			// Note: if the Link metadata exists, then it will be the equivalent of the ProjectVirtualPath
			if (!string.IsNullOrEmpty (link)) {
				if (Path.DirectorySeparatorChar != '\\') {
					Log (task, $"GetVirtualProjectPath ({projectDir}, {item.ItemSpec}, {isVSBuild}) => Link={link.Replace ('\\', '/')} (original {link})");
					return link.Replace ('\\', '/');
				}

				Log (task, $"GetVirtualProjectPath ({projectDir}, {item.ItemSpec}, {isVSBuild}) => Link={link.Replace ('\\', '/')}");
				return link;
			}

			var isDefaultItem = item.GetMetadata ("IsDefaultItem") == "true";
			var localMSBuildProjectFullPath = item.GetMetadata ("LocalMSBuildProjectFullPath");
			var localDefiningProjectFullPath = item.GetMetadata ("LocalDefiningProjectFullPath").Replace ('\\', '/');
			string path;
			string baseDir;

			string rv;

			if (string.IsNullOrEmpty (localDefiningProjectFullPath)) {
				task.Log.LogError ($"The item {item.ItemSpec} does not have a 'LocalDefiningProjectFullPath' value set.");
				return "placeholder";
			}

			if (string.IsNullOrEmpty (localMSBuildProjectFullPath)) {
				task.Log.LogError ($"The item {item.ItemSpec} does not have a 'LocalMSBuildProjectFullPath' value set.");
				return "placeholder";
			}

			if (isVSBuild) {
				// 'path' is full path on Windows
				path = PathUtils.PathCombineWindows (projectDir, item.ItemSpec);

				// 'baseDir' is the base directory in Windows
				if (isDefaultItem) {
					baseDir = Path.GetDirectoryName (localMSBuildProjectFullPath);
				} else {
					baseDir = Path.GetDirectoryName (localDefiningProjectFullPath);
				}

				rv = PathUtils.AbsoluteToRelativeWindows (baseDir, path);
				// Make it a mac-style path
				rv = rv.Replace ('\\', '/');

				Log (task, $"GetVirtualProjectPath\n" +
						$"\t\t\t{projectDir}\n" +
						$"\t\t\t{item.ItemSpec}\n" +
						$"\t\t\t{isVSBuild}\n" +
						$"\t\t\t\tisDefaultItem={isDefaultItem}\n" +
						$"\t\t\t\tLocalMSBuildProjectFullPath={localMSBuildProjectFullPath}\n" +
						$"\t\t\t\tLocalDefiningProjectFullPath={localDefiningProjectFullPath}\n" +
						$"\t\t\t\tpath={path}\n" +
						$"\t\t\t\tbaseDir={baseDir}\n" +
						$"\t\t\t\t ==> {rv}");
			} else {
				path = Path.Combine (projectDir, item.ItemSpec);

				if (isDefaultItem) {
					baseDir = Path.GetDirectoryName (localMSBuildProjectFullPath);
				} else if (!string.IsNullOrEmpty (localDefiningProjectFullPath)) {
					baseDir = Path.GetDirectoryName (localDefiningProjectFullPath);
				} else {
					baseDir = projectDir;
				}

				var originalBaseDir = baseDir;
				var originalPath = path;

				baseDir = PathUtils.ResolveSymbolicLinks (baseDir);
				path = PathUtils.ResolveSymbolicLinks (path);

				rv = PathUtils.AbsoluteToRelative (baseDir, path);
				Log (task, $"GetVirtualProjectPath\n" +
						$"\t\t\t{projectDir}\n" +
						$"\t\t\t{item.ItemSpec}\n" +
						$"\t\t\t{isVSBuild}\n" +
						$"\t\t\t\tisDefaultItem={isDefaultItem}\n" +
						$"\t\t\t\tLocalMSBuildProjectFullPath={localMSBuildProjectFullPath}\n" +
						$"\t\t\t\tLocalDefiningProjectFullPath={localDefiningProjectFullPath}\n" +
						$"\t\t\t\tpath={path} ({originalPath})\n" +
						$"\t\t\t\tbaseDir={baseDir} ({originalBaseDir})\n" +
						$"\t\t\t\t ==> {rv}");
			}

			return rv;
		}

		static bool GetISVSBuild (Task task)
		{
			if (task is XamarinTask xt)
				return !string.IsNullOrEmpty (xt.SessionId);
			if (task is XamarinToolTask xtt)
				return !string.IsNullOrEmpty (xtt.SessionId);
			return false;
		}

		public static string GetLogicalName (Task task, string projectDir, IList<string> prefixes, ITaskItem item)
		{
			var logicalName = item.GetMetadata ("LogicalName");

			if (!string.IsNullOrEmpty (logicalName)) {
				if (Path.DirectorySeparatorChar != '\\') {
					task?.Log.LogMessage (MessageImportance.Low, $"GetLogicalName ({projectDir}, {string.Join (";", prefixes)}, {item.ItemSpec}) => has LogicalName={logicalName.Replace ('\\', '/')} (original {logicalName})");
					return logicalName.Replace ('\\', '/');
				}
				task?.Log.LogMessage (MessageImportance.Low, $"GetLogicalName ({projectDir}, {string.Join (";", prefixes)}, {item.ItemSpec}) => has LogicalName={logicalName}");
				return logicalName;
			}

			var vpath = GetVirtualProjectPath (task, projectDir, item, GetISVSBuild (task));
			int matchlen = 0;

			foreach (var prefix in prefixes) {
				if (vpath.StartsWith (prefix, StringComparison.OrdinalIgnoreCase) && prefix.Length > matchlen)
					matchlen = prefix.Length;
			}

			if (matchlen > 0) {
				task?.Log.LogMessage (MessageImportance.Low, $"GetLogicalName ({projectDir}, {string.Join (";", prefixes)}, {item.ItemSpec}) => has LogicalName={vpath.Substring (matchlen)} with vpath {vpath} substring {matchlen}");
				return vpath.Substring (matchlen);
			}

			task?.Log.LogMessage (MessageImportance.Low, $"GetLogicalName ({projectDir}, {string.Join (";", prefixes)}, {item.ItemSpec}) => has LogicalName={vpath.Substring (matchlen)} with vpath {vpath}");
			return vpath;
		}
	}
}
