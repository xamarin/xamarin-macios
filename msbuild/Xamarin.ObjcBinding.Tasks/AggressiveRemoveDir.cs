// Temporary HACK: RemoveDir in Mono <= 2.10.8.x does not recursively delete contents

using System;
using System.IO;
using System.Collections.Generic;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Xamarin.ObjcBinding.Tasks {
	public class AggressiveRemoveDir : Task {
		[Required]
		public ITaskItem[] Directories { get; set; }
		
		[Output]
		public ITaskItem[] RemovedDirectories { get; set; }
		
		public AggressiveRemoveDir ()
		{
		}
		
		public override bool Execute ()
		{
			if (Directories.Length == 0)
				return true;
			
			List<ITaskItem> removed = new List <ITaskItem> ();
			
			foreach (ITaskItem directory in Directories) {
				try {
					string fullpath = directory.GetMetadata ("FullPath");
					if (Directory.Exists (fullpath)) {
						Directory.Delete (fullpath, true);
						removed.Add (directory);
					}
				}
				catch (Exception ex) {
					Log.LogErrorFromException (ex);
				}
			}
			
			RemovedDirectories = removed.ToArray ();
			
			return true;
		}
	}
}
