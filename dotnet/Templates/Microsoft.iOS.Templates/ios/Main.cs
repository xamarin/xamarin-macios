using iOSApp1;

#if DEBUG
// Enable runtime checks to ensure some types are only used from the main (UI) thread
UIApplication.CheckForIllegalCrossThreadCalls = true;
// Enable runtime checks to ensure ObjC delegates and events (using their own delegates) are not mixed
UIApplication.CheckForEventAndDelegateMismatches = true;
#endif

// This is the main entry point of the application.
// If you want to use a different Application Delegate class from "AppDelegate"
// you can specify it here.
UIApplication.Main (args, null, typeof (AppDelegate));
