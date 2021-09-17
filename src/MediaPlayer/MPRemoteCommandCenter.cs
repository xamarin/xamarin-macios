using System;

#nullable enable

namespace MediaPlayer {
#if !MONOMAC
	public partial class MPRemoteCommandCenter {
		[Obsolete ("Use MPRemoteCommandCenter.Shared")]
		public MPRemoteCommandCenter ()
		{
		}
	}
#endif
}
