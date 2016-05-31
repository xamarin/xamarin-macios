using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using Xamarin.Tests;

using NUnit.Framework;

namespace Xamarin.Tests
{
	[TestFixture]
	public class Misc
	{
		[Test]
		public void InvalidStructOffset ()
		{
			var str = "invalid struct offset";
			var contents = ASCIIEncoding.ASCII.GetBytes (str);

			foreach (var sdk in new string [] { "iphoneos", "iphonesimulator"}) {
				foreach (var ext in new string [] { "dylib", "a" }) {
					var fn = Path.Combine (Configuration.MonoTouchRootDirectory, "SDKs", "MonoTouch." + sdk + ".sdk", "usr", "lib", "libmonosgen-2.0." + ext);
					Assert.IsFalse (Contains (fn, contents), "Found \"{0}\" in {1}", str, fn);
				}
			}
		}

		bool Contains (string file, byte[] contents)
		{
			var pagesize = 4096;
			var buffer = new byte [pagesize * 1024];
			int read;

			using (var fs = new FileStream (file, FileMode.Open, FileAccess.Read)) {
				while ((read = fs.Read (buffer, 0, buffer.Length)) > 0) {
					for (int i = 0; i < read - contents.Length; i++) {
						if (buffer [i] == contents [0]) {
							var found = true;
							for (int c = 1; c < contents.Length; c++) {
								if (buffer [i + c] != contents [c]) {
									found = false;
									break;
								}
							}
							if (found)
								return true;
						}
					}
					if (fs.Position == fs.Length)
						break;
					fs.Position -= pagesize; // return a bit so that we don't miss contents spanning multiple reads.
				}
			}

			return false;
		}

		[Test]
		public void VerifySymbols ()
		{
			var prohibited_symbols = new string [] { "_NSGetEnviron", "PKService", "SPPluginDelegate" };

			foreach (var symbol in prohibited_symbols) {
				var contents = ASCIIEncoding.ASCII.GetBytes (symbol);
				var sdk = "iphoneos"; // we don't care about private symbols for simulator builds
				foreach (var static_lib in Directory.EnumerateFiles (Path.Combine (Configuration.MonoTouchRootDirectory, "SDKs", "MonoTouch." + sdk + ".sdk", "usr", "lib"), "*.a")) {
					Assert.IsFalse (Contains (static_lib, contents), "Found \"{0}\" in {1}", symbol, static_lib);
				}
			}
		}
	}
}

