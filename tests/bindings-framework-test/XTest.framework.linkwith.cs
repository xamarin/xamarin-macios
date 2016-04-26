using System;
#if __UNIFIED__
using ObjCRuntime;
#else
using MonoTouch.ObjCRuntime;
#endif
using System.Runtime.InteropServices;

#if __UNIFIED__
[assembly: LinkWith ("XTest.framework")]
[assembly: LinkWith ("XStaticObjectTest.framework", LinkerFlags = "-lz")]
[assembly: LinkWith ("XStaticArTest.framework")]
#endif
