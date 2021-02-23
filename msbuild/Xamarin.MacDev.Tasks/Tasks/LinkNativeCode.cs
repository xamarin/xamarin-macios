using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Build.Framework;
using Xamarin.Messaging.Build.Client;

namespace Xamarin.MacDev.Tasks
{
	public class LinkNativeCode : LinkNativeCodeTaskBase, ITaskCallback
	{
		string outputPath;

		public override bool Execute ()
		{
			if (!string.IsNullOrEmpty (SessionId)) {
				outputPath = Path.GetDirectoryName (OutputFile);

				return new TaskRunner (SessionId, BuildEngine4).RunAsync (this).Result;
			}

			return base.Execute ();
		}

		// We should avoid copying files from the output path because those already exist on the Mac
		// and the ones on Windows are empty, so we will break the build
		public bool ShouldCopyToBuildServer (ITaskItem item) => Path.GetDirectoryName (item.ItemSpec) != outputPath;

		public bool ShouldCreateOutputFile (ITaskItem item) => false;

		public IEnumerable<ITaskItem> GetAdditionalItemsToBeCopied () => Enumerable.Empty<ITaskItem> ();
	}
}

