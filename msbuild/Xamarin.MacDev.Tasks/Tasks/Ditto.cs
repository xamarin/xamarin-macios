using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Build.Framework;
using Xamarin.Messaging.Build.Client;

namespace Xamarin.MacDev.Tasks
{
	public class Ditto : DittoTaskBase, ITaskCallback
	{
		public override bool Execute ()
		{
			if (ShouldExecuteRemotely ()) {
				var taskRunner = new TaskRunner (SessionId, BuildEngine4);

				taskRunner.FixReferencedItems (new ITaskItem [] { Source });

				return taskRunner.RunAsync (this).Result;
			}

			return base.Execute ();
		}

		public override void Cancel ()
		{
			base.Cancel ();

			if (ShouldExecuteRemotely ())
				BuildConnection.CancelAsync (SessionId, BuildEngine4).Wait ();
		}

		public IEnumerable<ITaskItem> GetAdditionalItemsToBeCopied ()
		{	
			if (!Directory.Exists(Source.ItemSpec))
				return Enumerable.Empty<ITaskItem> ();

			return Directory.GetFiles (Source.ItemSpec, "*", SearchOption.AllDirectories)
				.Select(f => new TaskItem(f));
		} 

		public bool ShouldCopyToBuildServer (ITaskItem item) => true;

		public bool ShouldCreateOutputFile (ITaskItem item)
		{
			var fileExtension = Path.GetExtension (item.ItemSpec);

			return fileExtension != ".app" && fileExtension != ".appex";
		}
	}
}
