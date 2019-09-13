using NUnit.Framework;
using System;

#if !XAMCORE_2_0
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;
#else
using AppKit;
using ObjCRuntime;
#endif

namespace Xamarin.Mac.Tests
{
	[TestFixture]
	public class NSWorkspaceTests
	{
		[Test]
		public void NSWorkspaceConstantTests ()
		{
			Assert.IsNotNull (NSWorkspace.LaunchConfigurationAppleEvent);
			Assert.IsNotNull (NSWorkspace.LaunchConfigurationArguments);
			Assert.IsNotNull (NSWorkspace.LaunchConfigurationEnvironment);
			Assert.IsNotNull (NSWorkspace.LaunchConfigurationArchitecture);
		}

        [Test]
        public void HFSTypeCode4CCTest()
        {
            Assert.That(FourCC((int)HFSTypeCode.ClipboardIcon), Is.EqualTo("CLIP"), "dev#");
            Assert.That(FourCC((int)HFSTypeCode.ClippingUnknownTypeIcon), Is.EqualTo("clpu"), "clpu");
        }

        string FourCC(int value)
        {
            return new string(new char[] {
                (char) (byte) (value >> 24),
                (char) (byte) (value >> 16),
                (char) (byte) (value >> 8),
                (char) (byte) value });
        }
    }
}
