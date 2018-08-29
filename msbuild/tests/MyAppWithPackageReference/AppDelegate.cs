using System;
using System.Linq;
using System.Collections.Generic;

using Foundation;
using UIKit;

namespace MyTabbedApplication
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		public override UIWindow Window {
			get;
			set;
		}

		// This is the main entry point of the application.
		static void Main (string[] args)
		{
			Console.WriteLine (typeof (Newtonsoft.Json.JsonReader));
			UIApplication.Main (args, null, "AppDelegate");
		}
	}
}

