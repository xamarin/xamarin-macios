using System;
using System.IO;
using System.Reflection;

namespace ConsoleXMApp {
	class MainClass {
		public static void Main (string [] args)
		{
			var xm = Assembly.LoadFile (Path.Combine (Path.GetDirectoryName (typeof (MainClass).Assembly.Location), "Stuff", "Xamarin.Mac.dll"));
			xm.GetType ("AppKit.NSApplication").GetMethod ("Init").Invoke (null, new object [] { });
			var o = xm.GetType ("Foundation.NSObject").GetConstructor (new Type [] { }).Invoke (new object [] { });
			Console.WriteLine (o is not null);
		}
	}
}
