using System;

namespace XamCore.MediaPlayer {
#if !MONOMAC
	public partial class MPRemoteCommandCenter {
		[Obsolete ("Use MPRemoteCommandCenter.Shared")]
		public MPRemoteCommandCenter ()
		{
		}
	}
#endif
}
