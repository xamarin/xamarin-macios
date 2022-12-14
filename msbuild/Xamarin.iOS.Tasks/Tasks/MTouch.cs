using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Xamarin.Messaging.Build.Client;

namespace Xamarin.iOS.Tasks {
	public class MTouch : MTouchTaskBase, ITaskCallback {
		public ITaskItem [] ConfigFiles { get; set; }

		public override bool Execute ()
		{
			if (!ShouldExecuteRemotely ())
				return base.Execute ();

			var frameworkAssemblies = References.Where (x => x.IsFrameworkItem ());

			//Avoid having duplicated entries, which can happen with netstandard libraries that uses
			//some Reference Assemblies from NuGet packages
			var otherAssemblies = References
				.Where (x => !x.IsFrameworkItem ())
				.Where (x => !frameworkAssemblies.Any (f => f.GetMetadata ("Filename") == x.GetMetadata ("Filename")));

			TaskItemFixer.FixItemSpecs (Log, item => OutputPath, MainAssembly);
			TaskItemFixer.FixFrameworkItemSpecs (Log, item => OutputPath, TargetFramework.Identifier, frameworkAssemblies.ToArray ());
			TaskItemFixer.FixItemSpecs (Log, item => OutputPath, otherAssemblies.ToArray ());
			TaskItemFixer.ReplaceItemSpecsWithBuildServerPath (AppExtensionReferences, SessionId);

			var references = new List<ITaskItem> ();

			references.AddRange (frameworkAssemblies);
			references.AddRange (otherAssemblies);

			ConfigFiles = GetConfigFiles (references).ToArray ();

			TaskItemFixer.FixItemSpecs (Log, item => OutputPath, ConfigFiles);

			References = references.OrderBy (x => x.ItemSpec).ToArray ();

			return new TaskRunner (SessionId, BuildEngine4).RunAsync (this).Result;
		}

		public bool ShouldCopyToBuildServer (ITaskItem item) => !item.IsFrameworkItem ();

		public bool ShouldCreateOutputFile (ITaskItem item) => true;

		public IEnumerable<ITaskItem> GetAdditionalItemsToBeCopied ()
		{
			if (NativeReferences == null)
				yield break;

			foreach (var nativeRef in NativeReferences) {
				var path = nativeRef.ItemSpec;
				// look for frameworks, if the library is part of one then bring all related files
				var dir = Path.GetDirectoryName (path);
				if ((Path.GetExtension (dir) == ".framework") && Directory.Exists (dir)) {
					foreach (var item in GetItemsFromNativeReference (dir)) {
						// don't return the native library itself (it's the original input, not something additional)
						if (item.ItemSpec != path)
							yield return item;
					}
				}
			}
		}

		public override void Cancel ()
		{
			base.Cancel ();

			if (!string.IsNullOrEmpty (SessionId))
				BuildConnection.CancelAsync (BuildEngine4).Wait ();
		}

		IEnumerable<ITaskItem> GetConfigFiles (IEnumerable<ITaskItem> references)
		{
			var assemblies = new List<ITaskItem> { MainAssembly };

			assemblies.AddRange (references);

			foreach (var item in assemblies) {
				var configFile = item.ItemSpec + ".config";

				if (File.Exists (configFile))
					yield return new TaskItem (configFile);
			}
		}

		IEnumerable<TaskItem> GetItemsFromNativeReference (string folderPath)
		{
			foreach (var file in Directory
				.EnumerateFiles (folderPath, "*", SearchOption.AllDirectories)
				.Select (x => new TaskItem (x)))
				yield return file;
		}
	}
}
