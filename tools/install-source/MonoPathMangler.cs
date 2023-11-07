using System;
using System.IO;

namespace InstallSources {
	public class MonoPathMangler : IPathMangler {
		public static readonly string iOSFramework = "/Library/Frameworks/Xamarin.iOS.framework/Versions/Current/src/Xamarin.iOS/";
		public static readonly string MacFramework = "/Library/Frameworks/Xamarin.Mac.framework/Versions/Current/src/Xamarin.Mac/";
		string monoSubmodulePath;
		string xamarinSourcePath;

		/// <summary>
		/// Gets and sets the location of the mono sources.
		/// </summary>
		/// <value>The mono source path.</value>
		public string MonoSourcePath { get; set; }

		/// <summary>
		/// Gets or sets the frame work dir.
		/// </summary>
		/// <value>The frame work dir.</value>
		public string DestinationDir { get; set; }

		/// <summary>
		/// Gets and sets the path to the xamarin source.
		/// </summary>
		/// <value>The xamarin source path.</value>
		public string XamarinSourcePath {
			get {
				return xamarinSourcePath;
			}
			set {
				xamarinSourcePath = value;
				monoSubmodulePath = Path.Combine (value.Replace ("src/", ""), "external", "mono") + "/";
			}
		}

		/// <summary>
		/// Gets or sets the install dir.
		/// </summary>
		/// <value>The install dir.</value>
		public string InstallDir { get; set; }

		bool IsCheckout (string path)
			=> path.StartsWith (monoSubmodulePath, StringComparison.Ordinal);

		public string GetSourcePath (string path)
		{
			// we are dealing with a Mono archive assembly
			if (path.StartsWith (iOSFramework, StringComparison.Ordinal)) {
				return Path.Combine (MonoSourcePath, path.Substring (iOSFramework.Length));
			} else if (path.StartsWith (MacFramework, StringComparison.Ordinal)) {
				return Path.Combine (MonoSourcePath, path.Substring (MacFramework.Length));
			}

			// we are dealing with a local build
			return path;
		}

		public string GetTargetPath (string path)
		{
			var relativePath = path.Substring (IsCheckout (path) ? monoSubmodulePath.Length : MonoSourcePath.Length);
			if (relativePath.StartsWith ("/", StringComparison.Ordinal))
				relativePath = relativePath.Remove (0, 1);
			var target = Path.Combine (DestinationDir, "src", (InstallDir.Contains ("Xamarin.iOS") ? "Xamarin.iOS" : "Xamarin.Mac"), relativePath);
			return target;
		}
	}
}
