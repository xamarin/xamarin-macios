using System;
using System.Text;
using System.Collections.Generic;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Xamarin.Localization.MSBuild;

// Disable until we get around to enable + fix any issues.
#nullable disable

namespace Xamarin.MacDev.Tasks {
	public abstract class PackLibraryResourcesTaskBase : XamarinTask {
		#region Inputs

		[Required]
		public string Prefix { get; set; }

		public ITaskItem [] BundleResourcesWithLogicalNames { get; set; }

		#endregion

		#region Outputs

		[Output]
		public ITaskItem [] EmbeddedResources { get; set; }

		#endregion

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
			var results = new List<ITaskItem> ();

			if (BundleResourcesWithLogicalNames is not null) {
				foreach (var item in BundleResourcesWithLogicalNames) {
					var logicalName = item.GetMetadata ("LogicalName");

					if (string.IsNullOrEmpty (logicalName)) {
						Log.LogError (MSBStrings.E0161);
						return false;
					}

					var embedded = new TaskItem (item);

					embedded.SetMetadata ("LogicalName", "__" + Prefix + "_content_" + EscapeMangledResource (logicalName));

					results.Add (embedded);
				}
			}

			EmbeddedResources = results.ToArray ();

			return !Log.HasLoggedErrors;
		}
	}
}
