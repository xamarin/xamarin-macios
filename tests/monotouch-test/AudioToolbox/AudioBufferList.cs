
#if !__WATCHOS__
#if NET

using System;
using System.Runtime.InteropServices;

using AudioToolbox;
using Foundation;

using ObjCRuntime;

using NUnit.Framework;

namespace MonoTouchFixtures.AudioToolbox {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class AudioBufferListTest {
		[Test]
		public unsafe void Usage ()
		{
			var buffer = new byte [1024];
			fixed (byte* ptr = buffer) {
				var list = (AudioBufferList*) ptr;
				Assert.AreEqual (0, list->Count, "Count");
				Assert.Throws<ArgumentOutOfRangeException> (() => list->GetBuffer (0), "Item 0");
				Assert.Throws<ArgumentOutOfRangeException> (() => list->GetBuffer (-1), "Item -1");
				Assert.Throws<ArgumentOutOfRangeException> (() => list->GetBuffer (1), "Item 1");

				*(int*) ptr = 3;
				Assert.AreEqual (3, list->Count, "Count B");
				for (var i = 0; i < 3; i++) {
					Assert.AreEqual (0, list->GetBuffer (i)->NumberChannels, $"NumberChannels B#{i}");
					Assert.AreEqual (0, list->GetBuffer (i)->DataByteSize, $"DataByteSize B#{i}");
					Assert.AreEqual ((nint) 0, list->GetBuffer (i)->Data, $"Data B#{i}");

					list->GetBuffer (i)->NumberChannels = (i + 1) * 10;
					list->GetBuffer (i)->DataByteSize = (i + 1) * 100;
					list->GetBuffer (i)->Data = new IntPtr ((i + 1) * 1000);
				}
				Assert.Throws<ArgumentOutOfRangeException> (() => list->GetBuffer (-1), "Item -1 B");
				Assert.Throws<ArgumentOutOfRangeException> (() => list->GetBuffer (3), "Item 3 B");

				int* iptr = (int*) ptr;
				Assert.AreEqual (10, iptr [2 + 0 * 4], "10"); // NumberChannels
				Assert.AreEqual (100, iptr [2 + 0 * 4 + 1], "20"); // DataByteSize
				Assert.AreEqual (20, iptr [2 + 1 * 4], "20"); // NumberChannels
				Assert.AreEqual (200, iptr [2 + 1 * 4 + 1], "40"); // DataByteSize
				Assert.AreEqual (30, iptr [2 + 2 * 4], "30"); // NumberChannels
				Assert.AreEqual (300, iptr [2 + 2 * 4 + 1], "60"); // DataByteSize

				nint* nptr = (nint*) ptr;
				Assert.AreEqual ((nint) 1000, nptr [1 + 0 * 2 + 1], "1000"); // Data
				Assert.AreEqual ((nint) 2000, nptr [1 + 1 * 2 + 1], "2000"); // Data
				Assert.AreEqual ((nint) 3000, nptr [1 + 2 * 2 + 1], "3000"); // Data
			}
		}
	}
}

#endif // NET
#endif // !__WATCHOS__
