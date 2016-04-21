using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
﻿
using Xamarin.MacDev.Tasks;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Xamarin.Mac.Tasks
{
	public class BTouch : BTouchTaskBase
	{
		public string TargetFrameworkIdentifier { get; set; }

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

			return string.Format ("{0}/lib/bmac/bmac-{1}.exe ", FrameworkRoot, isMobile ? "mobile" : "full") + "-nostdlib " + base.GenerateCommandLineCommands ();
		}
	}
}
