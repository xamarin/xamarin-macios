using System;

using CoreFoundation;
using Foundation;
using ObjCRuntime;

using BenchmarkDotNet.Attributes;

using Bindings.Test;

namespace PerfTest {

	[Register ("StringClass")]
	class StringClass : NSObject {
			public override string Description => "constant";
	}

	public class TollFreeString {
		IntPtr StringClassClassHandle = Class.GetHandle (typeof (StringClass));

		string s;

		StringClass sc;
		IntPtr description;

		[GlobalSetup]
		public void ReturnString_Setup ()
		{
			sc = new StringClass ();
			description = Messaging.IntPtr_objc_msgSend (sc.Handle, Selector.GetHandle ("description"));
			Messaging.void_objc_msgSend (description, Selector.GetHandle ("retain"));
		}

		[GlobalCleanup]
		public void ReturnString_Cleanup ()
		{
			Messaging.void_objc_msgSend (description, Selector.GetHandle ("release"));
			sc.Dispose ();
		}

		/*
		 * Measure time required to get a string - using NSString (ObjC) selector-based API
		 */

		[Benchmark]
		public string ReturnString_NSString ()
		{
			var d = NSString.FromHandle (description);
			return s = d;
		}

		/*
		 * Measure time required to get a string - using CFString (C) p/invoke-based API
		 */

		[Benchmark]
		public string ReturnString_CFString ()
		{
			var d = CFString.FromHandle (description);
			return s = d;
		}
    }
}
