#if NET

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

#nullable enable

// Let's hope that by .NET 11 we've ironed out all the bugs in the API.
// This can of course be adjusted as needed (until we've released as stable).
#if NET110_0_OR_GREATER
#define STABLE_FSKIT
#endif

namespace FSKit {
#if !STABLE_FSKIT
	[Experimental ("APL0002")]
#endif
	[StructLayout (LayoutKind.Sequential)]
	public struct FSMetadataReadahead
	{
		public long Offset;
		public nuint Length;
	}
}
#endif
