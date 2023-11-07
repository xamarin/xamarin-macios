using System.Collections.Generic;
using Microsoft.Build.Framework;
using Xamarin.MacDev.Tasks;
using Xamarin.Messaging.Build.Client;
using System.Linq;
using System;
using System.IO;
using Microsoft.Build.Utilities;
using Xamarin.Messaging;

using Xamarin.iOS.Tasks;

#nullable enable

namespace Xamarin.MacDev.Tasks {
	public class BTouch : BTouchTaskBase, ITaskCallback {
		public override bool Execute ()
		{
			if (!ShouldExecuteRemotely ())
				return base.Execute ();

			try {
				BTouchToolPath = PlatformPath.GetPathForCurrentPlatform (BTouchToolPath);
				BaseLibDll = PlatformPath.GetPathForCurrentPlatform (BaseLibDll);

				TaskItemFixer.FixFrameworkItemSpecs (Log, item => OutputPath, TargetFramework.Identifier, References.Where (x => x.IsFrameworkItem ()).ToArray ());
				TaskItemFixer.FixItemSpecs (Log, item => OutputPath, References.Where (x => !x.IsFrameworkItem ()).ToArray ());

				var taskRunner = new TaskRunner (SessionId, BuildEngine4);
				var success = taskRunner.RunAsync (this).Result;

				if (success)
					GetGeneratedSourcesAsync (taskRunner).Wait ();

				return success;
			} catch (Exception ex) {
				Log.LogErrorFromException (ex);

				return false;
			}
		}

		public bool ShouldCopyToBuildServer (ITaskItem item) => !item.IsFrameworkItem ();

		public bool ShouldCreateOutputFile (ITaskItem item) => false;

		public IEnumerable<ITaskItem> GetAdditionalItemsToBeCopied ()
		{
			if (ObjectiveCLibraries is null)
				return new ITaskItem [0];

			return ObjectiveCLibraries.Select (item => {
				var linkWithFileName = String.Concat (Path.GetFileNameWithoutExtension (item.ItemSpec), ".linkwith.cs");
				return new TaskItem (linkWithFileName);
			}).ToArray ();
		}

		public override void Cancel ()
		{
			base.Cancel ();

			if (!string.IsNullOrEmpty (SessionId))
				BuildConnection.CancelAsync (BuildEngine4).Wait ();
		}

		async System.Threading.Tasks.Task GetGeneratedSourcesAsync (TaskRunner taskRunner)
		{
			await taskRunner.GetFileAsync (this, GeneratedSourcesFileList).ConfigureAwait (continueOnCapturedContext: false);

			var localGeneratedSourcesFileNames = new List<string> ();
			var generatedSourcesFileNames = File.ReadAllLines (GeneratedSourcesFileList);

			foreach (var generatedSourcesFileName in generatedSourcesFileNames) {
				var localRelativePath = GetLocalRelativePath (generatedSourcesFileName);

				await taskRunner.GetFileAsync (this, localRelativePath).ConfigureAwait (continueOnCapturedContext: false);

				var localGeneratedSourcesFileName = PlatformPath.GetPathForCurrentPlatform (localRelativePath);

				localGeneratedSourcesFileNames.Add (localGeneratedSourcesFileName);
			}

			File.WriteAllLines (GeneratedSourcesFileList, localGeneratedSourcesFileNames);
		}

		string GetLocalRelativePath (string path)
		{
			// convert mac full path in windows relative path
			// must remove \users\{user}\Library\Caches\Xamarin\mtbs\builds\{appname}\{sessionid}\
			if (path.Contains (SessionId)) {
				var start = path.IndexOf (SessionId) + SessionId.Length + 1;

				return path.Substring (start);
			} else {
				return path;
			}
		}
	}
}
