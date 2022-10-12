using Microsoft.Build.Tasks;
using Xamarin.MacDev.Tasks;

namespace Xamarin.MacDev.Tasks {
	public class CreateEmbeddedResources : CreateEmbeddedResourcesTaskBase {
		public override bool Execute ()
		{
			if (this.ShouldExecuteRemotely (SessionId)) {
				foreach (var bundleResource in this.BundleResources) {
					var logicalName = bundleResource.GetMetadata ("LogicalName");

					if (!string.IsNullOrEmpty (logicalName)) {
						logicalName = logicalName.Replace ("\\", "/");
						bundleResource.SetMetadata ("LogicalName", logicalName);
					}
				}
			}

			return base.Execute ();
		}
	}
}
