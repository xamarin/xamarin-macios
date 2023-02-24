#if !NET
using System;
using ObjCRuntime;
using System.Runtime.InteropServices;

[assembly: LinkWith ("libtest.a", LinkTarget.Simulator | LinkTarget.ArmV6 | LinkTarget.ArmV7 | LinkTarget.ArmV7s | LinkTarget.Arm64 | LinkTarget.Simulator64, SmartLink = true, Frameworks = LinkWithConstants.Frameworks, LinkerFlags = "-lz")]

static class LinkWithConstants {
#if __WATCHOS__
	public const string Frameworks = "Foundation CoreLocation";
#else
	public const string Frameworks = "Foundation ModelIO CoreLocation";
#endif
}
#endif
