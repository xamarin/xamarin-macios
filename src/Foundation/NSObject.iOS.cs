
#if !MONOMAC

using System;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

using UIKit;
using ObjCRuntime;

namespace Foundation {
	public partial class NSObject : INativeObject
#if !COREBUILD
	, IDisposable
#endif
	{
#if !COREBUILD

#if !NET && !WATCH
		[Obsolete ("Use 'PlatformAssembly' for easier code sharing across platforms.")]
		public readonly static Assembly MonoTouchAssembly = typeof (NSObject).Assembly;
#endif

		public static NSObject Alloc (Class kls) {
			var h = Messaging.IntPtr_objc_msgSend (kls.Handle, Selector.GetHandle (Selector.Alloc));
			return new NSObject (h, true);
		}

		public void Init () {
			if (handle == IntPtr.Zero)
				throw new Exception ("you have not allocated the native object");

			handle = Messaging.IntPtr_objc_msgSend (handle, Selector.GetHandle ("init"));
		}

		public static void InvokeInBackground (Action action)
		{
			// using the parameterized Thread.Start to avoid capturing
			// the 'action' parameter (it'll needlessly create an extra
			// object).
			new System.Threading.Thread ((v) =>
			{
				((Action) v) ();
			})
			{
				IsBackground = true,
			}.Start (action);
		}
#endif // !COREBUILD
	}
}

#endif // !MONOMAC
