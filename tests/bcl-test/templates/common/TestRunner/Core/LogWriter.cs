using System;
using System.IO;
using Foundation;
#if !MONOMAC
using UIKit;
#endif
using ObjCRuntime;
using Constants = global::ObjCRuntime.Constants;

namespace Xamarin.iOS.UnitTests {
	public class LogWriter {
		TextWriter writer;

		public MinimumLogLevel MinimumLogLevel { get; set; } = MinimumLogLevel.Info;

		public LogWriter () : this (Console.Out) { }

		public LogWriter (TextWriter w)
		{
			writer = w ?? Console.Out;
			InitLogging ();
		}

		[System.Runtime.InteropServices.DllImport ("/usr/lib/libobjc.dylib")]
		static extern IntPtr objc_msgSend (IntPtr receiver, IntPtr selector);

		static string UniqueIdentifier {
			get {
#if !__WATCHOS__ && !MONOMAC
				IntPtr handle = UIDevice.CurrentDevice.Handle;
				if (UIDevice.CurrentDevice.RespondsToSelector (new Selector ("uniqueIdentifier")))
					return NSString.FromHandle (objc_msgSend (handle, Selector.GetHandle ("uniqueIdentifier")));
#endif
				return "unknown";
			}
		}

		public void InitLogging ()
		{
#if !__WATCHOS__ && !MONOMAC
			UIDevice device = UIDevice.CurrentDevice;
#endif

			// print some useful info
			writer.WriteLine ("[Runner executing:\t{0}]", "Run everything");
			writer.WriteLine ("[MonoTouch Version:\t{0}]", Constants.Version);
			writer.WriteLine ("[Assembly:\t{0}.dll ({1} bits)]", typeof (NSObject).Assembly.GetName ().Name, IntPtr.Size * 8);
#if !__WATCHOS__ && !MONOMAC
			writer.WriteLine ("[{0}:\t{1} v{2}]", device.Model, device.SystemName, device.SystemVersion);
			writer.WriteLine ("[Device Name:\t{0}]", device.Name);
#endif
			writer.WriteLine ("[Device UDID:\t{0}]", UniqueIdentifier);
			writer.WriteLine ("[Device Locale:\t{0}]", NSLocale.CurrentLocale.Identifier);
			writer.WriteLine ("[Device Date/Time:\t{0}]", DateTime.Now); // to match earlier C.WL output
			writer.WriteLine ("[Bundle:\t{0}]", NSBundle.MainBundle.BundleIdentifier);
		}
		public void OnError (string message)
		{
			if (MinimumLogLevel < MinimumLogLevel.Error)
				return;
			writer.WriteLine (message);
			writer.Flush ();
		}

		public void OnWarning (string message)
		{
			if (MinimumLogLevel < MinimumLogLevel.Warning)
				return;
			writer.WriteLine (message);
			writer.Flush ();
		}

		public void OnDebug (string message)
		{
			if (MinimumLogLevel < MinimumLogLevel.Debug)
				return;
			writer.WriteLine (message);
			writer.Flush ();
		}

		public void OnDiagnostic (string message)
		{
			if (MinimumLogLevel < MinimumLogLevel.Verbose)
				return;
			writer.WriteLine (message);
			writer.Flush ();
		}

		public void OnInfo (string message)
		{
			if (MinimumLogLevel < MinimumLogLevel.Info)
				return;
			writer.WriteLine (message);
			writer.Flush ();
		}

		public void Info (string message)
		{
			writer.WriteLine (message);
			writer.Flush ();
		}

	}
}
