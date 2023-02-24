#nullable enable

using System;

namespace iTunesLibrary {
#if !NET
	public partial class ITLibrary {

		[Obsolete ("This constructor does not create a valid instance of the type.")]
		public ITLibrary ()
		{
		}
	}
#endif
}
