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
