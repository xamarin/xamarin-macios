using System;
using System.Runtime.InteropServices;
using Foundation;
using ObjCRuntime;

namespace ClockKit {
	public partial class CLKComplication {
			[Watch (7,0)]
			[DllImport (Constants.ClockKitLibrary, EntryPoint = "CLKAllComplicationFamilies")]
			public static extern NSNumber[] GetAllComplicationFamilies ();
	}
}