using System.IO;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.MacDev.Tasks;

// Disable until we get around to enable + fix any issues.
#nullable disable

namespace Xamarin.MacDev.Tasks {
	public abstract class GetFullPathTaskBase : XamarinTask {
		[Required]
		public string RelativePath { get; set; }

		[Output]
		public string FullPath { get; set; }

		public override bool Execute ()
		{
			FullPath = Path.GetFullPath (RelativePath);

			return !Log.HasLoggedErrors;
		}
	}
}
