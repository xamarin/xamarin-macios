using System;
using System.Runtime.InteropServices;
using Foundation;
using ObjCRuntime;

namespace MySimpleApp {
	public class Program {
		static int Main (string [] args)
		{
			var someObj = new SomeObj ();
			var handle = someObj.ClassHandle;
			return handle == NativeHandle.Zero ? 1 : 0;
		}
	}

	public class SomeObj : NSObject {
		static NativeHandle class_ptr = Class.GetHandle (typeof (SomeObj));
		[Export ("whatever")]
		public IntPtr Whatever ()
		{
			return new IntPtr (0xdeadf00d);
		}
		public override NativeHandle ClassHandle => class_ptr;
	}
}
