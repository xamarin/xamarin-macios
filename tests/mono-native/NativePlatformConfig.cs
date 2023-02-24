using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using Mono;

namespace Xamarin.Tests {
	[TestFixture]
	public class NativePlatformConfig {
		[Test]
		public void PlatformType ()
		{
			var type = MonoNativePlatform.GetPlatformType ();
			Assert.That ((int) type, Is.GreaterThan (0), "platform type");

			var usingCompat = (type & MonoNativePlatformType.MONO_NATIVE_PLATFORM_TYPE_COMPAT) != 0;
			Assert.AreEqual (MonoNativeConfig.UsingCompat, usingCompat, "using compatibility layer");
		}
	}
}
