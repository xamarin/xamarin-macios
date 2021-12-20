using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Xamarin.iOS.Tasks {
	public static class TaskItemFixer {
		const string XIFrameworkRootDir = "/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/lib/mono/";

		public static void FixFrameworkItemSpecs (TaskLoggingHelper log, Func<ITaskItem, string> itemPathFactory, string targetFrameworkIdentifier, params ITaskItem [] items)
		{
			foreach (var item in items) {
				var macDir = XIFrameworkRootDir + targetFrameworkIdentifier + "/";

				if (item.GetMetadata ("ResolvedFrom") == "ImplicitlyExpandDesignTimeFacades" || Path.GetDirectoryName (item.ItemSpec).EndsWith ("\\Facades"))
					macDir = macDir + "Facades/";

				item.ItemSpec = macDir + item.GetMetadata ("Filename") + item.GetMetadata ("Extension");
			}
		}

		public static void FixItemSpecs (TaskLoggingHelper log, Func<ITaskItem, string> itemPathFactory, params ITaskItem [] items)
		{
			foreach (var item in items) {
				var targetPath = Path.Combine (itemPathFactory (item), Path.GetFileName (item.ItemSpec));

				if (!Directory.Exists (Path.GetDirectoryName (targetPath)))
					Directory.CreateDirectory (Path.GetDirectoryName (targetPath));

				// HACK: If the target path is a network directory, GetLastWriteTimeUtc returns some a difference of some milliseconds
				// for the same file. So we just use Year/Month/Day/Hour/Minute/Second to decide if we should copy the item to the target location.
				var sourceLastWrite = File.GetLastWriteTimeUtc (item.ItemSpec);
				var sourceLastWriteFixed = new DateTime (sourceLastWrite.Year, sourceLastWrite.Month, sourceLastWrite.Day, sourceLastWrite.Hour, sourceLastWrite.Minute, sourceLastWrite.Second);
				var targetLastWrite = File.GetLastWriteTimeUtc (targetPath);
				var targetLastWriteFixed = new DateTime (targetLastWrite.Year, targetLastWrite.Month, targetLastWrite.Day, targetLastWrite.Hour, targetLastWrite.Minute, targetLastWrite.Second);

				if (!File.Exists (targetPath) || sourceLastWriteFixed > targetLastWriteFixed) {
					try {
						File.Copy (item.ItemSpec, targetPath, true);
					} catch (Exception ex) {
						log.LogWarning (ex.Message);
					}
				}

				item.ItemSpec = targetPath;
			}
		}

		public static void ReplaceItemSpecsWithBuildServerPath (IEnumerable<ITaskItem> items, string sessionId)
		{
			foreach (var item in items) {
				var buildSessionId = item.GetMetadata ("BuildSessionId");
				var buildServerPath = item.GetMetadata ("BuildServerPath");

				if (!string.IsNullOrEmpty (buildSessionId) && buildSessionId != sessionId && !string.IsNullOrEmpty (buildServerPath)) {
					item.ItemSpec = buildServerPath;
				}
			}
		}
	}
}
