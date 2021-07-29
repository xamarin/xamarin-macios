#if MONOMAC
using System;
using System.Runtime.InteropServices;
using ObjCRuntime;

namespace FileProvider {

	[NoiOS, NoMacCatalyst, Mac (12,0)]
	[StructLayout (LayoutKind.Sequential)]
	public struct NSFileProviderTypeAndCreator
	{
		public uint Type;

		public uint Creator;
	}

}
#endif
