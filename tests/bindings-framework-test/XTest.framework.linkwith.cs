using System;
using ObjCRuntime;
using System.Runtime.InteropServices;

[assembly: LinkWith ("XTest.framework", Frameworks = LinkWithConstants.Frameworks)]
[assembly: LinkWith ("XStaticObjectTest.framework", LinkerFlags = "-lz", Frameworks = LinkWithConstants.Frameworks)]
[assembly: LinkWith ("XStaticArTest.framework", Frameworks = LinkWithConstants.Frameworks)]

static class LinkWithConstants
{
#if __WATCHOS__
	public const string Frameworks = "CoreLocation";
#else
	public const string Frameworks = "CoreLocation ModelIO";
#endif
}
