//
// Unit tests for FSEventStream
//

#if __MACOS__

using System;
using System.IO;

using CoreFoundation;
using CoreServices;
using Foundation;

using NUnit.Framework;

namespace MonoTouchFixtures.CoreServices {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class FSEventStreamTest {

		[Test]
		public void Create ()
		{
			using var eventStream = new FSEventStream (new [] { Path.Combine (Environment.GetEnvironmentVariable ("HOME"), "Desktop") }, TimeSpan.FromSeconds (5), FSEventStreamCreateFlags.FileEvents);
		}
	}
}

#endif // __MACOS__
