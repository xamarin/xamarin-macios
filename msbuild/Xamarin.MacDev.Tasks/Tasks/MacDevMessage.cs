using System;
using System.Resources;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.Localization.MSBuild;

namespace Xamarin.MacDev.Tasks {

	/// <summary>
	/// Provides a localizable way to log a message from an MSBuild target.
	/// </summary>
	public class MacDevMessage : Task {

		/// <summary>
		/// The name of the resource from Properties\Resources.resx that contains the message
		/// </summary>
		[Required]
		public string ResourceName { get; set; } = string.Empty;

		/// <summary>
		/// The string format arguments to use for any numbered format items in the resource provided by ResourceName
		/// </summary>
		public string [] FormatArguments { get; set; } = Array.Empty<string> ();

		public override bool Execute ()
		{
			Log.LogMessage (
				MSBStrings.ResourceManager.GetString (ResourceName, MSBStrings.Culture),
				FormatArguments
			);
			return true;
		}
	}
}
