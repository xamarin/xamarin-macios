using System.Collections.Generic;
using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace ILLink.Tasks {
	public class ILLinkBase : ILLink
	{
		public string SessionId { get; set; }

		public ITaskItem [] DebugSymbols { get; set; }

		[Required]
		public string LinkerItemsDirectory { get; set; }

		[Output]
		public ITaskItem [] LinkerOutputItems { get; set; }

		[Output]
		public ITaskItem [] LinkedItems { get; set; }

		public override bool Execute ()
		{
			var result = base.Execute ();

			var linkerItems = new List<ITaskItem> ();
			var linkedItems = new List<ITaskItem> ();

			if (result)
			{
				// Adds all the files in the linker-items dir
				foreach (var item in Directory.EnumerateFiles (LinkerItemsDirectory))
				{
					linkerItems.Add (new TaskItem (item));
				}

				// Adds all the files in the linked output dir
				foreach (var item in Directory.EnumerateFiles (OutputDirectory.ItemSpec))
				{
					linkedItems.Add (new TaskItem (item));
				}
			}

			LinkerOutputItems = linkerItems.ToArray ();
			LinkedItems = linkedItems.ToArray();

			return result;
		}
	}
}
