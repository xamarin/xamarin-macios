// Based on AndroidUpdateResDir.cs
// Copyright (c) 2010 Novell, Inc. (http://www.novell.com)
// Copyright (C) 2012 Xamarin, Inc. All rights reserved.

using System;
using System.IO;
using System.Collections.Generic;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Xamarin.ObjcBinding.Tasks {
	public class AssignBundleResourceNames : Task
	{
		[Required]
		public ITaskItem[] BundleResources { get; set; }
		
		public string ResourceDirectoryPrefixes { get; set; }
		
		[Output]
		public ITaskItem[] BundleResourcesWithNames { get; set; }
		
		public override bool Execute ()
		{
			Log.LogMessage (MessageImportance.Low, "AssignBundleResourceNames Task");
			Log.LogMessage (MessageImportance.Low, "  ResourceDirectoryPrefixes: {0}", ResourceDirectoryPrefixes);
			
			BundleResourcesWithNames = new ITaskItem[BundleResources.Length];
			
			string[] prefixes = ResourceDirectoryPrefixes != null ? ResourceDirectoryPrefixes.Split (';') : null;
			if (prefixes != null) {
				for (int i = 0; i < prefixes.Length; i++) {
					string prefix = prefixes[i];
					char c = prefix[prefix.Length - 1];
					if (c != '\\' && c != '/')
						prefixes[i] = prefix + Path.DirectorySeparatorChar;
				}
			}
			
			for (int i = 0; i < BundleResources.Length; i++) {
				Log.LogMessage (MessageImportance.Low, "  BundleResource: {0}", BundleResources[i]);
				var item = BundleResources[i];
				
				// clone the item
				var newItem = new TaskItem (item.ItemSpec);
				item.CopyMetadataTo (newItem);
				
				// if the item doesn't have a LogicalName, compute one
				var logicalName = item.GetMetadata ("LogicalName");
				if (string.IsNullOrEmpty (logicalName)) {
					// get the link name or real name
					logicalName = item.GetMetadata ("Link");
					if (string.IsNullOrEmpty (logicalName))
						logicalName = item.GetMetadata ("Identity");
					
					// strip prefixes
					if (prefixes != null) {
						foreach (var prefix in prefixes) {
							if (logicalName.StartsWith (prefix)) {
								logicalName = logicalName.Substring (prefix.Length);
								break;
							}
						}
					}
				}
				
				newItem.SetMetadata ("BundleResourceName", logicalName);
				
				// add it to the output connection
				BundleResourcesWithNames[i] = newItem;
			}
			
			return true;
		}
	}
}
