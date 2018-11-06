using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;

namespace Mono.Native.Tests
{
	public enum MonoNativeLinkMode
	{
		None,
		Static,
		Dynamic,
		Framework
	}

	[TestFixture]
	public class NativePlatformConfig
	{
		public static MonoNativeLinkMode LinkMode {
			get {
#if MONO_NATIVE_STATIC
				return MonoNativeLinkMode.Static;
#elif MONO_NATIVE_DYLIB
				return MonoNativeLinkMode.Dynamic;
#else
				throw new NotImplementedException ();
#endif
			}
		}

		public static string RootDirectory => Path.GetDirectoryName (Assembly.GetEntryAssembly ().Location);

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
