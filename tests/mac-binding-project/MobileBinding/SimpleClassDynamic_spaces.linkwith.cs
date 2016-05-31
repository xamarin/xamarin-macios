using ObjCRuntime;

[assembly: LinkWith ("SimpleClass Dylib.dylib", SmartLink = true, ForceLoad = true)]
