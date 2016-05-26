using System;
using Finder;
#if XAMCORE_2_0
using AppKit;
using ScriptingBridge;
#else
using nuint = System.Int32;
using MonoMac.AppKit;
using MonoMac.ScriptingBridge;
#endif

namespace ScriptingBridge.Tests
{
	public static class MainClass
	{
		static int Main (string[] args)
		{
			FinderApplication app = SBApplication.FromBundleIdentifier<FinderApplication> ("com.apple.finder");

			if ((int)app.Trash.Items.Count > 0) {
				for (int i = 0 ; i < (int)app.Trash.Items.Count ; ++i) {
					FinderItem item = app.Trash.Items.GetItem <FinderItem> ((nuint)i);
					if (item.Name == null)
						return 0;
				}
			}

			Console.WriteLine ("Passed");

			return 0;
		}
	}
}
