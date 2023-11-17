using System;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;
using Xamarin.MacDev;
using System.Linq;
using Xamarin.MacDev.Tasks;

// Disable until we get around to enable + fix any issues.
#nullable disable

namespace Xamarin.iOS.Tasks {
	public abstract class WriteAssetPackManifestTaskBase : XamarinTask {
		#region Inputs

		[Required]
		public ITaskItem OutputFile { get; set; }

		[Required]
		public ITaskItem TemplatePath { get; set; }

		[Required]
		public string OnDemandResourceUrl { get; set; }

		[Required]
		public bool IsStreamable { get; set; }

		#endregion

		public override bool Execute ()
		{
			var template = PDictionary.FromFile (TemplatePath.ItemSpec);
			var resources = template.GetArray ("resources");

			foreach (var resource in resources.OfType<PDictionary> ()) {
				PString url;
				string name;
				int slash;

				if (!resource.TryGetValue ("URL", out url))
					continue;

				if ((slash = url.Value.LastIndexOf ('/')) == -1)
					continue;

				name = url.Value.Substring (slash);

				url.Value = OnDemandResourceUrl + name;

				resource ["isStreamable"] = new PNumber (IsStreamable ? 1 : 0);
			}

			template.Save (OutputFile.ItemSpec, true, true);

			return !Log.HasLoggedErrors;
		}
	}
}
