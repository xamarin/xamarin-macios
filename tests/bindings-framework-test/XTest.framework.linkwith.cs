using System;
#if __UNIFIED__
using ObjCRuntime;
#else
using MonoTouch.ObjCRuntime;
#endif
using System.Runtime.InteropServices;

#if __UNIFIED__
[assembly: LinkWith ("XTest.framework", Frameworks = LinkWithConstants.Frameworks)]
[assembly: LinkWith ("XStaticObjectTest.framework", LinkerFlags = "-lz", Frameworks = LinkWithConstants.Frameworks)]
[assembly: LinkWith ("XStaticArTest.framework", Frameworks = LinkWithConstants.Frameworks)]
#endif

static class LinkWithConstants
{
#if __WATCHOS__
	public const string Frameworks = "";
#else
	public const string Frameworks = "ModelIO";
#endif
}
