using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Reflection;

namespace Xamarin.MMP.Tests
{
	[TestFixture]
	public partial class MMPTests
	{
		enum AotType {
			None,
			All,
			Platform,
			PlatformWithIgnore,
			Explicit
		}

		void ValidateAOTState (AotType type, string path) {
			var dirInfo = new DirectoryInfo (path);

			foreach (var file in dirInfo.EnumerateFiles ()) {
				string extension = file.Extension.ToLowerInvariant ();
				if (extension == ".exe" || extension == ".dll") {
					bool shouldBeAOT;
					switch (type) {
						case AotType.All:
							shouldBeAOT = true;
							break;
						case AotType.Explicit:
							shouldBeAOT = file.Name == "Xamarin.Mac.dll"; // Arbitrary file chosen for test
							break;
						case AotType.Platform:
							shouldBeAOT = (file.Name == "Xamarin.Mac.dll" || file.Name == "System.dll" || file.Name == "mscorlib.dll");
							break;
						case AotType.PlatformWithIgnore:
							shouldBeAOT = (file.Name == "System.dll" || file.Name == "mscorlib.dll");
							break;
						case AotType.None:
						default:
							shouldBeAOT = false;
							break;
					}

					Assert.AreEqual (shouldBeAOT, File.Exists (file.FullName + ".dylib"), "{0} should {1} be AOT", file.FullName, shouldBeAOT ? "" : "not");
				}
			}
		}

		string GetMonoBundlePath (string tmpDir, bool xm45)
		{
			return Path.Combine (tmpDir, string.Format ("bin/Debug/{0}.app/Contents/MonoBundle/", xm45 ? "XM45Example" : "UnifiedExample"));
		}

		[Test]
		public void Unified_AOTAll_RunsAndCompilesAll () {
			RunMMPTest (tmpDir => {
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) {
					CSProjConfig = "<MonoBundlingExtraArgs>--aot=all</MonoBundlingExtraArgs>"
				};
				foreach (bool xm45 in new bool[] { false, true }) {
					test.XM45 = xm45;
					TI.TestUnifiedExecutable (test);
					ValidateAOTState (AotType.All, GetMonoBundlePath (tmpDir, xm45));
				}
			});
		}

		[Test]
		public void Unified_AOTPlatform_RunsAndCompilesPlatform () {
			RunMMPTest (tmpDir => {
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) {
					CSProjConfig = "<MonoBundlingExtraArgs>--aot=platform</MonoBundlingExtraArgs>"
				};
				foreach (bool xm45 in new bool[] { false, true }) {
					test.XM45 = xm45;
					TI.TestUnifiedExecutable (test);
					ValidateAOTState (AotType.Platform, GetMonoBundlePath (tmpDir, xm45));
				}
			});
		}

		[Test]
		public void Unified_AOTIgnore_IgnoresSpecificAssembly () {
			RunMMPTest (tmpDir => {
				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) {
					CSProjConfig = "<MonoBundlingExtraArgs>--aot=platform --aot-ignore=Xamarin.Mac.dll</MonoBundlingExtraArgs>"
				};
				foreach (bool xm45 in new bool[] { false, true }) {
					test.XM45 = xm45;
					TI.TestUnifiedExecutable (test);
					ValidateAOTState (AotType.PlatformWithIgnore, GetMonoBundlePath (tmpDir, xm45));
				}
			});
		}

		[Test]
		public void Unified_AOTExplicit_JustListed () {
			RunMMPTest (tmpDir => {
				foreach (bool xm45 in new bool[] { false, true }) {
					// Without --aot-reference we should AOT none
					TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) {
						XM45 = xm45,
						CSProjConfig = "<MonoBundlingExtraArgs>--aot=explicit</MonoBundlingExtraArgs>"
					};

					TI.TestUnifiedExecutable (test);
					ValidateAOTState (AotType.None, GetMonoBundlePath (tmpDir, xm45));

					test.CSProjConfig = "<MonoBundlingExtraArgs>--aot=explicit --aot-reference=Xamarin.Mac.dll</MonoBundlingExtraArgs>";

					TI.TestUnifiedExecutable (test);
					ValidateAOTState (AotType.Explicit, GetMonoBundlePath (tmpDir, xm45));
				}
			});
		}

	}
}
