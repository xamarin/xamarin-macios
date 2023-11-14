using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xamarin.MacDev.Tasks;
using Xamarin.Messaging.Build.Client;

namespace Xamarin.iOS.Tasks {
	public class PrepareObjCBindingNativeFrameworks : XamarinTask, ITaskCallback, ICancelableTask {
		public ITaskItem [] ObjCBindingNativeFrameworks { get; set; }

		public override bool Execute ()
		{
			try {
				//This task runs locally, and its purpose is just to copy the ObjCBindingNativeFrameworks to the build server
				new TaskRunner (SessionId, BuildEngine4).CopyInputsAsync (this).Wait ();
			} catch (Exception ex) {
				Log.LogErrorFromException (ex);

				return false;
			}

			return true;
		}

		public bool ShouldCopyToBuildServer (ITaskItem item) => true;

		public bool ShouldCreateOutputFile (ITaskItem item) => false;

		public IEnumerable<ITaskItem> GetAdditionalItemsToBeCopied ()
		{
			return CreateItemsForAllFilesRecursively (ObjCBindingNativeFrameworks);
		}

		public void Cancel () => BuildConnection.CancelAsync (BuildEngine4).Wait ();
	}
}
