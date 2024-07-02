using System;
using System.Runtime.InteropServices;

using Foundation;

namespace MySimpleApp {
	public class Program {
		static int Main (string [] args)
		{
			GC.KeepAlive (typeof (NSObject)); // prevent linking away the platform assembly

			Console.WriteLine (Environment.GetEnvironmentVariable ("MAGIC_WORD"));

			var myObject = new MyClass (typeof (MyClass));
			Console.WriteLine (myObject.X);

			return args.Length;
		}
	}

	internal class MyClass {
		public object X { get; set; }

		public MyClass (object x)
		{
			X = x.GetType ().GetProperties ();
		}
	}
}
