using System;

namespace iTunesLibrary
{
#if !XAMCORE_2_0
    public partial class ITLibrary {

        [Obsolete ("Does not return a valid instance on macOS 14.")]
        public ITLibrary ()
        {
        }
    }
#endif
}