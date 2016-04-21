using System;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.MacDev;

namespace Xamarin.MacDev.Tasks
{
	public abstract class GetPropertyListValueTaskBase : Task
	{
		#region Inputs

		[Required]
		public string PropertyListFile { get; set; }

		[Required]
		public string Property { get; set; }

		#endregion Inputs

		#region Outputs

		[Output]
		public string Value { get; set; }

		#endregion Outputs

		public override bool Execute ()
		{
			PArray array = null;
			PDictionary dict;
			PObject value;
			string[] path;
			int i = 0;

			Log.LogTaskName ("GetPropertyListValue");
			Log.LogTaskProperty ("PropertyListFile", PropertyListFile);
			Log.LogTaskProperty ("Property", Property);

			path = Property.TrimStart (':').Split (new [] { ':' });

			if (path.Length == 0) {
				Log.LogError ("No property specified.");
				return false;
			}

			try {
				value = dict = PDictionary.FromFile (PropertyListFile);
			} catch (Exception ex) {
				Log.LogError ("Error loading '{0}': {1}", PropertyListFile, ex.Message);
				return false;
			}

			do {
				if (dict != null) {
					if (!dict.TryGetValue (path[i], out value)) {
						var item = i > 0 ? string.Join ("/", path, 0, i - 1) : PropertyListFile;
						Log.LogError ("The dictionary at '{0}' did not contain the key: {1}", item, path[i]);
						return false;
					}
				} else if (array != null) {
					int arrayIndex;

					if (!int.TryParse (path[i], out arrayIndex)) {
						Log.LogError ("Could not parse array index: {0}", path[i]);
						return false;
					}

					if (arrayIndex < 0 || arrayIndex >= array.Count) {
						var item = i > 0 ? string.Join ("/", path, 0, i - 1) : PropertyListFile;
						Log.LogError ("Array index out of range for item '{0}'", item);
						return false;
					}

					value = array[arrayIndex];
				} else {
					Log.LogError ("{0} values do not support child properties.", value.Type);
					return false;
				}

				dict = value as PDictionary;
				array = value as PArray;
				i++;
			} while (i < path.Length);

			if (array != null || dict != null) {
				Log.LogError ("Getting {0} values is not supported.", value.Type.ToString ().ToLowerInvariant ());
				return false;
			}

			Value = value.ToString ();

			return !Log.HasLoggedErrors;
		}
	}
}
