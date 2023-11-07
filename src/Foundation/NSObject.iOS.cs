
#if !MONOMAC

using System;
using System.Reflection;

namespace Foundation {
	public partial class NSObject {
#if !COREBUILD

#if !NET && !WATCH
		[Obsolete ("Use 'PlatformAssembly' for easier code sharing across platforms.")]
		public readonly static Assembly MonoTouchAssembly = typeof (NSObject).Assembly;
#endif
#endif // !COREBUILD
	}
}

#endif // !MONOMAC
