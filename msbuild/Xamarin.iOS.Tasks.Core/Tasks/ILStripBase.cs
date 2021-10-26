using System.Collections.Generic;
using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

// Normally this would be ILStrip.Tasks but ILStrip is in the global namespace
// And having a type and the parent namespace have the same name really confuses the compiler
namespace ILStripTasks {
	public class ILStripBase : ILStrip
	{
		public string SessionId { get; set; }

		[Output]
		public ITaskItem [] StrippedAssemblies { get; set; }

		public override bool Execute ()
		{
			var result = base.Execute ();

			var stripedItems = new List<ITaskItem> ();

			if (result)
			{
				foreach (var item in Assemblies)
				{
					stripedItems.Add (new TaskItem (item.GetMetadata("OutputPath"), item.CloneCustomMetadata ()));
				}
			}

			StrippedAssemblies = stripedItems.ToArray();

			return result;
		}
	}
}
