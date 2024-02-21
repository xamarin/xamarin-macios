using System;
using Finder;
using AppKit;
using ScriptingBridge;

namespace ScriptingBridge.Tests {
	public static class MainClass {
		static int Main (string [] args)
		{
			FinderApplication app = SBApplication.FromBundleIdentifier<FinderApplication> ("com.apple.finder");

			if ((int) app.Trash.Items.Count > 0) {
				for (int i = 0; i < (int) app.Trash.Items.Count; ++i) {
					FinderItem item = app.Trash.Items.GetItem<FinderItem> ((nuint) i);
					if (item.Name is null)
						return 0;
				}
			}

			Console.WriteLine ("Passed");

			return 0;
		}
	}
}
