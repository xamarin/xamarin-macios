// Copyright (C) 2012 Xamarin, Inc. All rights reserved.

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Xamarin.ObjcBinding.Tasks {
	public class CreateEmbeddedResources : Task
	{
		[Required]
		public ITaskItem[] BundleResourcesWithNames { get; set; }
		
		[Output]
		public ITaskItem[] EmbeddedResources { get; set; }
		
		static string MangleResourceName (string resName)
		{
			var sb = new StringBuilder (resName);
			sb.Replace ("_", "__");
			sb.Replace ("/", "_f");
			sb.Replace ("\\", "_b");
			return sb.ToString ();
		}
		
		public override bool Execute ()
		{
			EmbeddedResources = new ITaskItem[BundleResourcesWithNames.Length];
			
			for (int i = 0; i < BundleResourcesWithNames.Length; i++) {
				var item = BundleResourcesWithNames[i];
				
				// clone the item
				var newItem = new TaskItem (item.ItemSpec);
				item.CopyMetadataTo (newItem);
				
				// mangle the resource name
				var logicalName = "__monotouch_content_" + MangleResourceName (item.GetMetadata ("BundleResourceName"));
				newItem.SetMetadata ("LogicalName", logicalName);
				
				// add it to the output connection
				EmbeddedResources[i] = newItem;
			}
			
			return true;
		}
	}
}
