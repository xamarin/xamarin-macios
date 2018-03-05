using System;
using System.Collections.Generic;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.MacDev;

namespace Xamarin.MacDev.Tasks
{
	public abstract class CompileAppManifestTaskBase : Task
	{
		#region Inputs

		public string SessionId { get; set; }

		[Required]
		public string AppBundleName { get; set; }

		[Required]
		public bool IsAppExtension { get; set; }

		[Required]
		public string AppBundleDir { get; set; }

		[Required]
		public string AppManifest { get; set; }

		[Required]
		public string AssemblyName { get; set; }

		[Required]
		public string BundleIdentifier { get; set; }

		public ITaskItem[] PartialAppManifests { get; set; }

		#endregion

		#region Outputs

		[Output]
		public ITaskItem CompiledAppManifest { get; set; }

		#endregion

		protected void LogAppManifestError (string format, params object[] args)
		{
			// Log an error linking to the Info.plist file
			Log.LogError (null, null, null, AppManifest, 0, 0, 0, 0, format, args);
		}

		protected void LogAppManifestWarning (string format, params object[] args)
		{
			// Log a warning linking to the Info.plist file
			Log.LogWarning (null, null, null, AppManifest, 0, 0, 0, 0, format, args);
		}

		protected void SetValue (PDictionary dict, string key, string value)
		{
			if (dict.ContainsKey (key))
				return;

			if (string.IsNullOrEmpty (value))
				LogAppManifestWarning ("Could not determine value for manifest key '{0}'", key);
			else
				dict[key] = value;
		}

		protected void MergePartialPlistDictionary (PDictionary plist, PDictionary partial)
		{
			foreach (var property in partial) {
				if (plist.ContainsKey (property.Key)) {
					var value = plist[property.Key];

					if (value is PDictionary && property.Value is PDictionary) {
						MergePartialPlistDictionary ((PDictionary) value, (PDictionary) property.Value);
					} else {
						plist[property.Key] = property.Value.Clone ();
					}
				} else {
					plist[property.Key] = property.Value.Clone ();
				}
			}
		}

		protected void MergePartialPlistTemplates (PDictionary plist)
		{
			if (PartialAppManifests == null)
				return;

			foreach (var template in PartialAppManifests) {
				PDictionary partial;

				try {
					partial = PDictionary.FromFile (template.ItemSpec);
				} catch (Exception ex) {
					Log.LogError ("Error loading partial Info.plist template file '{0}': {1}", template.ItemSpec, ex.Message);
					continue;
				}

				MergePartialPlistDictionary (plist, partial);
			}
		}
	}
}
