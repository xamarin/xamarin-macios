#if __MACOS__
using System;
using NUnit.Framework;
using OpenGL;

namespace Xamarin.Mac.Tests {
	[TestFixture]
	public class CGLContextTests {
		[Test]
		public void CurrentContextAllowsNull ()
		{
			Assert.DoesNotThrow (() => {
				CGLContext.CurrentContext = null;
			});
		}
	}
}
#endif // __MACOS__
