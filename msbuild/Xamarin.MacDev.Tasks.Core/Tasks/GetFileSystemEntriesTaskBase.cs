using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Xamarin.MacDev.Tasks
{
	public abstract class GetFileSystemEntriesTaskBase : XamarinTask
	{
		#region Inputs

		[Required]
		public string DirectoryPath { get; set; }

		[Required]
		public string Pattern { get; set; }

		[Required]
		public bool Recursive { get; set; }

		[Required]
		public bool IncludeDirectories { get; set; }
		#endregion

		#region Outputs

		[Output]
		public ITaskItem[] Entries { get; set; }

		#endregion

		public override bool Execute ()
		{
			var searchOption = Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
			var entriesFullPath = IncludeDirectories ?
				Directory.GetFileSystemEntries (DirectoryPath, Pattern, searchOption) :
				Directory.GetFiles (DirectoryPath, Pattern, searchOption);

			Entries = entriesFullPath.Select (v => new TaskItem (v)).ToArray ();

			return !Log.HasLoggedErrors;
		}
	}
}
