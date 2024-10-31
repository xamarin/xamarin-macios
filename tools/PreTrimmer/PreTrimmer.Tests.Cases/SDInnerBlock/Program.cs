using System;

Console.WriteLine ("Hello, World!");
_ = typeof (ObjCRuntime.Trampolines.SDInnerBlock);

namespace ObjCRuntime {

	class Trampolines {
		static internal class SDInnerBlock {
			// this field is not preserved by other means, but it must not be linked away
			static internal readonly DInnerBlock Handler = Invoke;

			[MonoPInvokeCallback (typeof (DInnerBlock))]
			static internal void Invoke (IntPtr block, int magic_number)
			{
			}
			internal delegate void DInnerBlock (IntPtr block, int magic_number);
		}
	}
	class MonoPInvokeCallbackAttribute : Attribute {
		public MonoPInvokeCallbackAttribute (Type type)
		{
		}
	}

}
