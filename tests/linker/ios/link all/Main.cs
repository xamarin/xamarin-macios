using System;

using UIKit;

namespace LinkAll
{
	public class Application
	{
#if !__WATCHOS__
		// This is the main entry point of the application.
		static void Main (string[] args)
		{
			// if you want to use a different Application Delegate class from "AppDelegate"
			// you can specify it here.
			UIApplication.Main (args, null, "AppDelegate");
		}
#endif // !__WATCHOS__
	}

#if NET
	// https://github.com/mono/linker/issues/1913
	[Foundation.Preserve (AllMembers = true)]
	class Preserver 
	{
		public Preserver ()
		{
			GC.KeepAlive (new System.Runtime.CompilerServices.InternalsVisibleToAttribute ("preserve this constructor"));
		}
	}
#endif
}
