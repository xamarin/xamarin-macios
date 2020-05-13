using System.IO;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.MacDev.Tasks;

namespace Xamarin.iOS.Tasks
{
	public abstract class GetFullPathTaskBase : XamarinTask
	{
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
