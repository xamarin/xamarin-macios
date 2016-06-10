using System;
using System.Reflection;
using System.IO;

using AppKit;
using Simple;

namespace MobileTestApp
{
	static class MainClass
	{
		// http://stackoverflow.com/questions/52797/how-do-i-get-the-path-of-the-assembly-the-code-is-in
		public static string GetCurrentExecutingDirectory()
		{
			string filePath = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
			return Path.GetDirectoryName(filePath);
		}

		static void Main (string[] args)
		{
#pragma warning disable 0219		
			var v = ObjCRuntime.Dlfcn.dlopen (GetCurrentExecutingDirectory () + "/SimpleClassDylib.dylib", 0);
#pragma warning restore 0219

			NSApplication.Init ();
			SimpleClass c = new SimpleClass ();
			Console.WriteLine (c.DoIt());
		}
	}
}
