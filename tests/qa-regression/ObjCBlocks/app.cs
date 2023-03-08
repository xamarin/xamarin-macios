using System;
using System.IO;
using System.Runtime.InteropServices;

using MonoMac.AppKit;

using Test;

static class App {
	[DllImport ("dl")]
	static extern IntPtr dlopen (string path, int mode);

	static void Main ()
	{
		Console.WriteLine ("[managed] enter Main");
		var selfPath = Path.GetDirectoryName (typeof (App).Assembly.Location);
		var libHandle = dlopen (Path.Combine (selfPath, "block.dylib"), 2 /* RTLD_NOW */);
		Console.WriteLine ("[managed] block.dylib handle: {0}", libHandle);

		Console.WriteLine ("[managed] about to call NSApplication.Init");
		NSApplication.Init ();
		Console.WriteLine ("[managed] NSApplication.Init has exited");

		Console.WriteLine ("[managed] about to instantiate ObjCBlocksTest");
		using (var test = new ObjCBlocksTest ()) {
			Console.WriteLine ("[managed] about to call DoInvoke");
			test.DoInvoke (() => Console.WriteLine ("[managed] DoInvoke blockHandler"));
			Console.WriteLine ("[managed] DoInvoke has exited");
		}
		Console.WriteLine ("[managed] ObjBlocksTest has been disposed");
		Console.WriteLine ("[managed] exit Main (0)");
	}
}
