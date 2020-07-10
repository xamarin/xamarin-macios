using System;
using System.Runtime.InteropServices;
using Foundation;
using CoreFoundation;
using ObjCRuntime;

namespace ClockKit {
	public partial class CLKComplication {
			[Watch (7,0)]
			[DllImport (Constants.ClockKitLibrary)]
			static extern IntPtr CLKAllComplicationFamilies ();

			public static NSNumber[] GetAllComplicationFamilies ()
			{
				using (var nsArray = new NSArray (CLKAllComplicationFamilies ())) {
					var families = new NSNumber [(int)nsArray.Count];
					for (nuint i = 0; i < nsArray.Count; i++)
					{
						families[i] = nsArray.GetItem <NSNumber> (i);
					}
					return families;
				}
			}
	}
}