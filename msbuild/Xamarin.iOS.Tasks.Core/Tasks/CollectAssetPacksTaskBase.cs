using System;
using System.IO;
using System.Collections.Generic;

using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;

using Xamarin.MacDev.Tasks;

namespace Xamarin.iOS.Tasks
{
	public abstract class CollectAssetPacksTaskBase : XamarinTask
	{
		#region Inputs

		[Required]
		public string OnDemandResourcesPath { get; set; }

		#endregion

		#region Outputs

		[Output]
		public ITaskItem[] AssetPacks { get; set; }

		#endregion

		public override bool Execute ()
		{
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
	}
}

