using System;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.MacDev;
using Xamarin.Localization.MSBuild;

// Disable until we get around to enable + fix any issues.
#nullable disable

namespace Xamarin.MacDev.Tasks {
	public abstract class GetPropertyListValueTaskBase : Task {
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
			string [] path;
			int i = 0;

			path = Property.TrimStart (':').Split (new [] { ':' });

			if (path.Length == 0) {
				Log.LogError (MSBStrings.E0152);
				return false;
			}

			try {
				value = dict = PDictionary.FromFile (PropertyListFile);
			} catch (Exception ex) {
				Log.LogError (MSBStrings.E0010, PropertyListFile, ex.Message);
				return false;
			}

			do {
				if (dict is not null) {
					if (!dict.TryGetValue (path [i], out value)) {
						var item = i > 0 ? string.Join ("/", path, 0, i - 1) : PropertyListFile;
						Log.LogError (MSBStrings.E0153, item, path [i]);
						return false;
					}
				} else if (array is not null) {
					int arrayIndex;

					if (!int.TryParse (path [i], out arrayIndex)) {
						Log.LogError (MSBStrings.E0145, path [i]);
						return false;
					}

					if (arrayIndex < 0 || arrayIndex >= array.Count) {
						var item = i > 0 ? string.Join ("/", path, 0, i - 1) : PropertyListFile;
						Log.LogError (MSBStrings.E0155, item);
						return false;
					}

					value = array [arrayIndex];
				} else {
					Log.LogError (MSBStrings.E0156, value.Type);
					return false;
				}

				dict = value as PDictionary;
				array = value as PArray;
				i++;
			} while (i < path.Length);

			if (array is not null || dict is not null) {
				Log.LogError (MSBStrings.E0157, value.Type.ToString ().ToLowerInvariant ());
				return false;
			}

			Value = value.ToString ();

			return !Log.HasLoggedErrors;
		}
	}
}
