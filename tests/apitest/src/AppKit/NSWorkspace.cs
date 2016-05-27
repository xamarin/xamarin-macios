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
	}
}
