using System;
using System.IO;
using System.Collections.Generic;

using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;

using Xamarin.MacDev.Tasks;
using Xamarin.Messaging.Build.Client;

namespace Xamarin.iOS.Tasks {
	public class CollectAssetPacks : XamarinTask, ICancelableTask {
		#region Inputs

		[Required]
		public string OnDemandResourcesPath { get; set; }

		#endregion

		#region Outputs

		[Output]
		public ITaskItem [] AssetPacks { get; set; }

		#endregion

		public override bool Execute ()
		{
			if (ShouldExecuteRemotely ())
				return new TaskRunner (SessionId, BuildEngine4).RunAsync (this).Result;

			var assetpacks = new List<ITaskItem> ();

			foreach (var dir in Directory.EnumerateDirectories (OnDemandResourcesPath, "*.assetpack")) {
				//We need to add the directory separator at the end of the path 
				//so the TaskRunner don't get confused, copying the asset pack as a file
				assetpacks.Add (new TaskItem (dir + Path.DirectorySeparatorChar));
			}

			AssetPacks = assetpacks.ToArray ();

			Log.LogTaskProperty ("AssetPacks", AssetPacks);

			return !Log.HasLoggedErrors;
		}

		public void Cancel ()
		{
			if (ShouldExecuteRemotely ())
				BuildConnection.CancelAsync (BuildEngine4).Wait ();
		}
	}
}
