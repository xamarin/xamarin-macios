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
			bool isModern;
			if (TargetFrameworkIdentifier != null) {
				if (TargetFrameworkIdentifier == "Xamarin.Mac") {
					isModern = true; // Expected case for Modern targetting
				}
				else if (TargetFrameworkIdentifier == "FullFramework") {
					isModern = false; // Expected case for Full framework
				}
				else { // If it is something else, don't guess
					Log.LogError ("BTouch doesn't know how to deal with TargetFrameworkIdentifier: " + TargetFrameworkIdentifier);
					return string.Empty;
				}
			}
			else {
				isModern = true; // Some older binding don't have either tag, assume modern since it is the default
			}

			EnvironmentVariables = new string[] {
				"MONO_PATH=" + string.Format ("{0}/lib/mono/{1}", FrameworkRoot, isModern ? "Xamarin.Mac" : "4.5")
			};

			var sb = new StringBuilder ();
			var bgen = Path.Combine (FrameworkRoot, "lib", "bgen", "bgen.exe");
			if (File.Exists (bgen)) {
				sb.Append (bgen);
			} else {
				if (isModern) {
					sb.Append (Path.Combine (FrameworkRoot, "lib", "bmac", "bmac-mobile.exe"));
				} else {
					sb.Append (Path.Combine (FrameworkRoot, "lib", "bmac", "bmac-full.exe"));
				}
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
					return "/target-framework=Xamarin.Mac,Version=v2.0,Profile=Modern";
				case "FullFramework":
					return "/target-framework=Xamarin.Mac,Version=v4.5,Profile=Full";
				default:
					Log.LogError ($"Unknown target framework identifier: {TargetFrameworkIdentifier}.");
					return string.Empty;
			}
		}

	}
}
