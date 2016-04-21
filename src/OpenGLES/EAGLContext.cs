using System;
using System.Runtime.InteropServices;
using XamCore.ObjCRuntime;

namespace XamCore.OpenGLES
{
	public partial class EAGLContext
	{
		[DllImport (Constants.OpenGLESLibrary)]
		public extern static void EAGLGetVersion (out nuint major, out nuint minor);

#if !XAMCORE_3_0
		[Obsolete ("iOS9 does not allow creating an empty instance")]
		public EAGLContext ()
		{
		}
#endif
	}
}

