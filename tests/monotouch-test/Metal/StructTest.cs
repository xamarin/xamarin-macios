#if !__WATCHOS__

using System;
using System.Runtime.InteropServices;

#if XAMCORE_2_0
using Metal;
#else
using MonoTouch.Metal;
#endif

using NUnit.Framework;

namespace MonoTouchFixtures.Metal {
	
	[TestFixture]
	public class StructTest {
		
		[Test]
		public void MTLQuadTessellationFactorsHalfStructSize ()
		{
			// tested with a native iOS app
			Assert.AreEqual (12, Marshal.SizeOf<MTLQuadTessellationFactorsHalf> (), $"Reported size was {Marshal.SizeOf<MTLQuadTessellationFactorsHalf> ()}");
		}

		[Test]
		public void MTLTriangleTessellationFactorsHalfStructSize ()
		{
			// tested with a native iOS app
			Assert.AreEqual (8, Marshal.SizeOf<MTLTriangleTessellationFactorsHalf> (), $"Reported size was {Marshal.SizeOf<MTLTriangleTessellationFactorsHalf> ()}");
		}
	}
}

#endif // !__WATCHOS__

