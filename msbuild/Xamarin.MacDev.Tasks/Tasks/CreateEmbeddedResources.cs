using System;
using System.Text;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.MacDev.Tasks;

// Disable until we get around to enable + fix any issues.
#nullable disable

namespace Xamarin.MacDev.Tasks {
	public class CreateEmbeddedResources : XamarinTask {
		[Required]
		public ITaskItem [] BundleResources { get; set; }

		[Required]
		public string Prefix { get; set; }

		[Output]
		public ITaskItem [] EmbeddedResources { get; set; }

		static string EscapeMangledResource (string name)
		{
			var mangled = new StringBuilder ();

			for (int i = 0; i < name.Length; i++) {
				switch (name [i]) {
				case '\\': mangled.Append ("_b"); break;
				case '/': mangled.Append ("_f"); break;
				case '_': mangled.Append ("__"); break;
				default: mangled.Append (name [i]); break;
				}
			}

			return mangled.ToString ();
		}

		public override bool Execute ()
		{
			if (ShouldExecuteRemotely ()) {
				foreach (var bundleResource in this.BundleResources) {
					var logicalName = bundleResource.GetMetadata ("LogicalName");

					if (!string.IsNullOrEmpty (logicalName)) {
						logicalName = logicalName.Replace ("\\", "/");
						bundleResource.SetMetadata ("LogicalName", logicalName);
					}
				}
			}

			EmbeddedResources = new ITaskItem [BundleResources.Length];

			for (int i = 0; i < BundleResources.Length; i++) {
				var bundleResource = BundleResources [i];

				// clone the item
				var embeddedResource = new TaskItem (bundleResource.ItemSpec);
				bundleResource.CopyMetadataTo (embeddedResource);

				// mangle the resource name
				var logicalName = "__" + Prefix + "_content_" + EscapeMangledResource (bundleResource.GetMetadata ("LogicalName"));
				embeddedResource.SetMetadata ("LogicalName", logicalName);

				// add it to the output connection
				EmbeddedResources [i] = embeddedResource;
			}

			return true;
		}
	}
}
