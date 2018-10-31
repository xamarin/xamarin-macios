using System;
using NUnit.Framework;
using Mono;

namespace Mono.Native.Tests
{
	[TestFixture]
	public class NativePlatformConfig
	{
		static bool ShouldUseCompat {
			get {
#if MONO_NATIVE_COMPAT
				return true;
#elif MONO_NATIVE_UNIFIED
				return false;
#else
				Assert.Fail ("Missing `MONO_NATIVE_COMPAT` or `MONO_NATIVE_UNIFIED`");
				throw new NotImplementedException ();
#endif
			}
		}
		[Test]
		public void PlatformType ()
		{
			var type = MonoNativePlatform.GetPlatformType ();
			Assert.That ((int)type, Is.GreaterThan (0), "platform type");

			Console.Error.WriteLine ($"NATIVE PLATFORM TYPE: {type}");

			var usingCompat = (type & MonoNativePlatformType.MONO_NATIVE_PLATFORM_TYPE_COMPAT) != 0;
			Assert.AreEqual (ShouldUseCompat, usingCompat, "using compatibility layer");
		}
	}
}
