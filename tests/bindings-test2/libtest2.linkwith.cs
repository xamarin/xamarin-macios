using System;
using ObjCRuntime;
using System.Runtime.InteropServices;

[assembly: LinkWith ("libtest2.a", LinkTarget.Simulator | LinkTarget.ArmV6 | LinkTarget.ArmV7 | LinkTarget.ArmV7s | LinkTarget.Arm64 | LinkTarget.Simulator64, SmartLink = true, Frameworks = "Foundation", LinkerFlags = "-lz")]

public static class LibTest {
	[DllImport ("__Internal")]
	public static extern int theUltimateAnswer ();
}
