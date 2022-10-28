using System;

#nullable enable

namespace MediaPlayer {
#if !MONOMAC && !NET
	public partial class MPRemoteCommandCenter {
		[Obsolete ("Use MPRemoteCommandCenter.Shared")]
		public MPRemoteCommandCenter ()
		{
		}
	}
#endif
}
