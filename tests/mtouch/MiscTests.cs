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
			MTouch.AssertDeviceAvailable ();

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
			MTouch.AssertDeviceAvailable ();

			var prohibited_symbols = new string [] { "_NSGetEnviron", "PKService", "SPPluginDelegate" };

			foreach (var symbol in prohibited_symbols) {
				var contents = ASCIIEncoding.ASCII.GetBytes (symbol);
				var sdk = "iphoneos"; // we don't care about private symbols for simulator builds
				foreach (var static_lib in Directory.EnumerateFiles (Path.Combine (Configuration.MonoTouchRootDirectory, "SDKs", "MonoTouch." + sdk + ".sdk", "usr", "lib"), "*.a")) {
					Assert.IsFalse (Contains (static_lib, contents), "Found \"{0}\" in {1}", symbol, static_lib);
				}
			}
		}

		[Test]
		[TestCase (Profile.iOS)]
		[TestCase (Profile.tvOS)]
		[TestCase (Profile.watchOS)]
		[TestCase (Profile.macOSMobile)]
		public void PublicSymbols (Profile profile)
		{
			var paths = new HashSet<string> ();
			paths.UnionWith (Directory.GetFileSystemEntries (Configuration.GetSdkPath (profile, true), "*.a", SearchOption.AllDirectories));
			paths.UnionWith (Directory.GetFileSystemEntries (Configuration.GetSdkPath (profile, false), "*.a", SearchOption.AllDirectories));
			var failed = new StringBuilder ();

			var prefixes = new string [] {
				// xamarin-macios
				"_xamarin_",
				"_monotouch_",
				"_monomac_",
				"_OBJC_METACLASS_$_Xamarin",
				"_OBJC_CLASS_$_Xamarin",
				"_OBJC_IVAR_$_Xamarin",
				"__ZN13XamarinObject",
				"_main",
				 // I think these are inline functions from a header
				"__Z7isasciii",
				"__Z7isblanki",
				"__Z7isdigiti",
				"__Z8__istypeim",
				"__Z9__isctypeim",
				// mono
				"_mono_",
				"_monoeg_",
				"_eg_",
				"_mini_",
				"_proflog_",
				"_ves_icall_",
				"___mono_jit_",
				"_sdb_options",
				"_SystemNative_",
				"_gateway_from_rtm",
				"_sgen_",
				"_arm_patch",
				"_g_printv",
				// These two aren't public in a way we care about
				"l_OBJC_LABEL_PROTOCOL_$_",
				"l_OBJC_PROTOCOL_$_",
			};

			paths.RemoveWhere ((v) => {
				var file = Path.GetFileName (v);
				switch (file) {
				case "libxammac-classic.a":
				case "libxammac-classic-debug.a":
				case "libxammac-system-classic.a":
				case "libxammac-system-classic-debug.a":
					return true;
				}
				return false;
			});


			foreach (var path in paths) {
				var symbols = MTouchTool.GetNativeSymbolsInExecutable (path);

				// Remove known public symbols
				symbols = symbols.Where ((v) => {
					foreach (var prefix in prefixes) {
						if (v.StartsWith (prefix, StringComparison.Ordinal))
							return false;
					}

					// zlib-helper symbols
					switch (v) {
					case "_CloseZStream":
					case "_CreateZStream":
					case "_Flush":
					case "_ReadZStream":
					case "_WriteZStream":
						return false;
					}

					// Be a bit more lenient with symbols from the static registrar
					if (path.Contains (".registrar.")) {
						if (v.StartsWith ("_OBJC_CLASS_$", StringComparison.Ordinal))
							return false;
						if (v.StartsWith ("_OBJC_IVAR_$", StringComparison.Ordinal))
							return false;
						if (v.StartsWith ("_OBJC_METACLASS_$", StringComparison.Ordinal))
							return false;
					}

					return true;
				});

				// If there are any public symbols left, that's a problem so fail the test.
				if (symbols.Any ())
					failed.AppendLine ($"{path}:\n\t{string.Join ("\n\t", symbols.ToArray ())}");
			}

			Assert.IsEmpty (failed.ToString (), "Failed libraries");
		}
	}
}

