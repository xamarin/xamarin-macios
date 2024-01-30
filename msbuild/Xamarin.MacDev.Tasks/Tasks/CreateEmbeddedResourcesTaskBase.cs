using System;
using System.Text;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.MacDev.Tasks;

#nullable enable

namespace Xamarin.MacDev.Tasks {
	public class CreateEmbeddedResources : XamarinTask {
		[Required]
		public ITaskItem [] BundleResources { get; set; } = Array.Empty<ITaskItem> ();

		[Required]
		public string Prefix { get; set; } = string.Empty;

		[Output]
		public ITaskItem [] EmbeddedResources { get; set; } = Array.Empty<ITaskItem> ();

		public override bool Execute ()
		{
			EmbeddedResources = new ITaskItem [BundleResources.Length];

			for (int i = 0; i < BundleResources.Length; i++) {
				var bundleResource = BundleResources [i];

				// clone the item
				var embeddedResource = new TaskItem (bundleResource.ItemSpec);
				bundleResource.CopyMetadataTo (embeddedResource);

				// mangle the resource name
				var logicalName = "__" + Prefix + "_content_" + PackLibraryResources.EscapeMangledResource (bundleResource.GetMetadata ("LogicalName"));
				embeddedResource.SetMetadata ("LogicalName", logicalName);

				// add it to the output connection
				EmbeddedResources [i] = embeddedResource;
			}

			return true;
		}
	}
}
