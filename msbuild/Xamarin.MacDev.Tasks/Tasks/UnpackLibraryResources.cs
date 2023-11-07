using System.Linq;
using System.Collections.Generic;

using Mono.Cecil;
using Xamarin.Messaging.Build.Client;
using Microsoft.Build.Framework;

#nullable enable

namespace Xamarin.MacDev.Tasks {
	public class UnpackLibraryResources : UnpackLibraryResourcesTaskBase, ITaskCallback, ICancelableTask {
		public override bool Execute ()
		{
			bool result;

			if (ShouldExecuteRemotely ()) {
				result = new TaskRunner (SessionId, BuildEngine4).RunAsync (this).Result;

				if (result && BundleResourcesWithLogicalNames is not null) {
					// Fix LogicalName path for Windows
					foreach (var resource in BundleResourcesWithLogicalNames) {
						var logicalName = resource.GetMetadata ("LogicalName");

						if (!string.IsNullOrEmpty (logicalName)) {
							resource.SetMetadata ("LogicalName", logicalName.Replace ("/", "\\"));
						}
					}
				}
			} else {
				result = base.Execute ();
			}

			return result;
		}

		public void Cancel ()
		{
			if (ShouldExecuteRemotely ())
				BuildConnection.CancelAsync (BuildEngine4).Wait ();
		}

		public bool ShouldCopyToBuildServer (ITaskItem item)
		{
			if (item.IsFrameworkItem ())
				return false;

			if (NoOverwrite is not null && NoOverwrite.Contains (item))
				return false;

			return true;
		}

		public bool ShouldCreateOutputFile (ITaskItem item) => UnpackedResources?.Contains (item) == true;

		public IEnumerable<ITaskItem> GetAdditionalItemsToBeCopied () => Enumerable.Empty<ITaskItem> ();

		protected override IEnumerable<ManifestResource> GetAssemblyManifestResources (string fileName)
		{
			AssemblyDefinition? assembly = null;
			try {
				try {
					assembly = AssemblyDefinition.ReadAssembly (fileName);
				} catch {
					yield break;
				}

				foreach (var _r in assembly.MainModule.Resources.OfType<EmbeddedResource> ()) {
					var r = _r;
					yield return new ManifestResource (r.Name, r.GetResourceStream);
				}
			} finally {
				assembly?.Dispose ();
			}
		}
	}
}
