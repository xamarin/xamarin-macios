using System.IO;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.MacDev.Tasks;

namespace Xamarin.iOS.Tasks
{
	public abstract class GetFullPathTaskBase : Task
	{
		public string SessionId { get; set; }

		[Required]
		public string RelativePath { get; set; }

		[Output]
		public string FullPath { get; set; }

		public override bool Execute ()
		{
			Log.LogTaskName ("GetFullPath");
			Log.LogTaskProperty ("RelativePath", RelativePath);

			FullPath = Path.GetFullPath (RelativePath);

			return !Log.HasLoggedErrors;
		}
	}
}
