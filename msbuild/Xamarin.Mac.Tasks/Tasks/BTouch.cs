using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;
﻿
using Xamarin.MacDev.Tasks;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Xamarin.Mac.Tasks
{
	public class BTouch : BTouchTaskBase
	{
		public string FrameworkRoot { get; set; }

		protected override string GenerateCommandLineCommands ()
		{
			bool isMobile;
			if (TargetFrameworkIdentifier != null) {
				if (TargetFrameworkIdentifier == "Xamarin.Mac") {
					isMobile = true; // Expected case for Mobile targetting
				}
				else if (TargetFrameworkIdentifier == ".NETFramework") {
					isMobile = false; // Expected case for XM 4.5
				}
				else { // If it is something else, don't guess
					Log.LogError ("BTouch doesn't know how to deal with TargetFrameworkIdentifier: " + TargetFrameworkIdentifier);
					return string.Empty;
				}
			}
			else {
				isMobile = true; // Some older binding don't have either tag, assume mobile since it is the default
			}

			EnvironmentVariables = new string[] {
				"MONO_PATH=" + string.Format ("{0}/lib/mono/{1}", FrameworkRoot, isMobile ? "Xamarin.Mac" : "4.5")
			};

			var sb = new StringBuilder ();
			if (isMobile) {
				sb.Append (Path.Combine (FrameworkRoot, "lib", "bmac", "bmac-mobile.exe"));
			} else {
				sb.Append (Path.Combine (FrameworkRoot, "lib", "bmac", "bmac-full.exe"));
			}
			sb.Append (" -nostdlib ");
			sb.Append (base.GenerateCommandLineCommands ());
			return sb.ToString ();
		}

		protected override string GetTargetFrameworkArgument ()
		{
			switch (TargetFrameworkIdentifier) {
				case null:
				case "":
				case "Xamarin.Mac":
					return "/target-framework=Xamarin.Mac,Version=v2.0,Profile=Mobile";
				case ".NETFramework":
					return "/target-framework=Xamarin.Mac,Version=v4.5,Profile=Full";
				default:
					Log.LogError ($"Unknown target framework identifier: {TargetFrameworkIdentifier}.");
					return string.Empty;
			}
		}

	}
}
