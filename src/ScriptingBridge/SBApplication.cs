using System;
using System.Reflection;
using XamCore.AppKit;
using XamCore.Foundation;
using XamCore.ObjCRuntime;
#if !COREBUILD
using MacAttribute = XamCore.ObjCRuntime.Extensions.MacAttribute;
using iOSAttribute = XamCore.ObjCRuntime.Extensions.iOSAttribute;
#endif
using AvailabilityAttribute = XamCore.ObjCRuntime.Extensions.AvailabilityAttribute;
using Platform = XamCore.ObjCRuntime.Extensions.Platform;


namespace XamCore.ScriptingBridge {
	public partial class SBApplication
	{
		// We want to instance up a version of your derived class, not SBApplication
		public static T FromBundleIdentifier<T> (string ident) where T : SBApplication, new()
		{
			using (var u = FromBundleIdentifier (ident))
				return (T)System.Activator.CreateInstance (typeof(T), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new object [] { u.Handle }, null);
		}

		public static T FromURL<T> (NSUrl url) where T : SBApplication
		{
			using (var u = FromURL (url))
				return (T)System.Activator.CreateInstance (typeof(T), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new object [] { u.Handle }, null);
		}

		public static T FromProcessIdentifier<T> (int /* pid_t = int */ pid) where T : SBApplication
		{
			using (var u = FromProcessIdentifier (pid))
				return (T)System.Activator.CreateInstance (typeof(T), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new object [] { u.Handle }, null);
		}
	}
}