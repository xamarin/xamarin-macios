#if NET
using System.Runtime.InteropServices;

#nullable enable

namespace FSKit {
	[StructLayout (LayoutKind.Sequential)]
	public struct FSMetadataReadahead
	{
		public long Offset;
		public nuint Length;
	}
}
#endif
