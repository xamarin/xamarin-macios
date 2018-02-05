using System;

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
