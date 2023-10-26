using System.IO;
using System.Linq;
using Microsoft.Build.Framework;
using Xamarin.Localization.MSBuild;

#nullable enable

namespace Xamarin.MacDev.Tasks {
	public class FindILLink : XamarinBuildTask {
		[Output]
		public string ILLinkPath { get; set; } = string.Empty;

		protected override bool ExecuteLocally ()
		{
			if (!TryGetProperty ("ILLinkTasksAssembly", out var illinkTaskPath))
				return false;

			// Don't do anything else if something already went wrong (in particular don't check if illink.dll exists).
			if (Log.HasLoggedErrors)
				return false;

			if (!string.IsNullOrEmpty (illinkTaskPath))
				ILLinkPath = Path.Combine (Path.GetDirectoryName (illinkTaskPath), "illink.dll");

			if (!File.Exists (ILLinkPath))
				Log.LogError (MSBStrings.E7115 /*"The illink assembly doesn't exist: '{0}'" */, ILLinkPath);

			return !Log.HasLoggedErrors;
		}
	}
}
