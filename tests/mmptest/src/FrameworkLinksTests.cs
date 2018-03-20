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
	public class FrameworkLinkTests
	{
		const string LinkerEnabledConfig = "<LinkMode>Full</LinkMode>";
		const string StaticRegistrarConfig = "<MonoBundlingExtraArgs>--registrar=static</MonoBundlingExtraArgs>";

		enum LinkStatus { Strong, Weak }

		Dictionary <string, LinkStatus> CalculateFrameworkLinkStatus (string[] clangParts)
		{
			var status = new Dictionary<string, LinkStatus> ();
			for (int i = 0; i < clangParts.Length; ++i) {
				string currentPart = clangParts[i];
				if (currentPart == "-weak_framework" || currentPart == "-framework") {
					string name = clangParts[i + 1];
					status[name] = currentPart == "-framework" ? LinkStatus.Strong : LinkStatus.Weak;
				}
			}
			return status;
		}

		void AssertAppKitLinkage (Dictionary<string, LinkStatus> status)
		{
			Assert.IsTrue (status.ContainsKey ("AppKit"), "AppKit must have framework reference in clang invocation");
			Assert.AreEqual (LinkStatus.Strong, status["AppKit"], "AppKit must be strong linked");
		}

		void AssertFrameworkMinOSRespected (Dictionary <string, LinkStatus> status)
		{
			// Walk rest of frameworks and verify they are weak_framework if newer than 10.7, which is defined in tests/common/mac/Info-Unified.plist
			foreach (var entry in status) {
				LinkStatus linkStatus;
				Framework currentFramework = Frameworks.MacFrameworks.Find (entry.Key);
				if (currentFramework == null) {
					// There are a few entries not in Framesworks.cs that we know about
					switch (entry.Key) {
						case "Carbon":
						case "CoreGraphics":
						case "CoreFoundation":
						case "ApplicationServices":
							linkStatus = LinkStatus.Strong;
							break;
						default:
							Assert.Fail ("Unknown entry in AssertFrameworkMinOSRespected - " + entry.Key);
							return;
					}
				}
				else {
					linkStatus = currentFramework.Version > new Version (10, 7) ? LinkStatus.Weak : LinkStatus.Strong;
				}
				Assert.AreEqual (linkStatus, entry.Value, $"Framework link status of {entry.Key} was {entry.Value} but expected to be {linkStatus}");
			}
		}

		[Test]
		public void UnifiedWithoutLinking_ShouldHaveManyFrameworkClangLines ()
		{
			MMPTests.RunMMPTest (tmpDir => {
				// By default we -framework in all frameworks we bind.
				string [] clangParts = MMPTests.GetUnifiedProjectClangInvocation (tmpDir);
				AssertUnlinkedFrameworkStatus (clangParts);

				// Even with static registrar
				clangParts = MMPTests.GetUnifiedProjectClangInvocation (tmpDir, StaticRegistrarConfig);
				AssertUnlinkedFrameworkStatus (clangParts);
			});
		}

		void AssertUnlinkedFrameworkStatus (string[] clangParts)
		{
			Dictionary<string, LinkStatus> status = CalculateFrameworkLinkStatus (clangParts);

			AssertAppKitLinkage (status);	

			// We expect a large number of entires, which will grow as we add more bindings
			Assert.Greater (status.Count, 20, "Did not found as many framework entries in clang invocation as expected - {0}\n{1}", status.Count, string.Join (" ", clangParts));

			AssertFrameworkMinOSRespected (status);
		}

		[Test]
		public void UnifiedWithLinking_ShouldHaveFewFrameworkClangLines ()
		{
			MMPTests.RunMMPTest (tmpDir => {
				// When we link, we should throw away pretty much everything that isn't AppKit. 
				string[] clangParts = MMPTests.GetUnifiedProjectClangInvocation (tmpDir, LinkerEnabledConfig);
				AssertLinkedFrameworkStatus (clangParts);

				// Even with static registrar
				clangParts = MMPTests.GetUnifiedProjectClangInvocation (tmpDir, LinkerEnabledConfig + StaticRegistrarConfig);
				AssertLinkedFrameworkStatus (clangParts);
			});
		}

		void AssertLinkedFrameworkStatus (string[] clangParts)
		{
			Dictionary<string, LinkStatus> status = CalculateFrameworkLinkStatus (clangParts);

			AssertAppKitLinkage (status);

			// We expect a few number of entires, which should not grow much over time
			// Today - Foundation, AppKit, Security, QuartzCore, CoreFoundation, CFNetwork, Carbon, CoreServices, CoreData, Quartz, CloudKit
			Assert.Less (status.Count, 11, "Found more framework entries in clang invocation then expected - {0}\n{1}", string.Join (" ", status.Select ((v) => v.Key)), string.Join (" ", clangParts));

			AssertFrameworkMinOSRespected (status);
		}


		[Test]
		public void ProjectWithLinkToPrivateFramework_ShouldBuild ()
		{
			MMPTests.RunMMPTest (tmpDir => {

				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) { TestDecl = @"
	const string MobileDeviceLibrary = ""/System/Library/PrivateFrameworks/MobileDevice.framework/MobileDevice"";
	[System.Runtime.InteropServices.DllImport (MobileDeviceLibrary)]
	private static extern void TestMethod ();
				" };

				TI.TestUnifiedExecutable (test);
			});
		}

		[Test]
		public void ProjectWithSubFramework_ShouldBuild ()
		{
			MMPTests.RunMMPTest (tmpDir => {

				TI.UnifiedTestConfig test = new TI.UnifiedTestConfig (tmpDir) { TestDecl = @"
	[System.Runtime.InteropServices.DllImport (""/System/Library/Frameworks/CoreServices.framework/Frameworks/LaunchServices.framework/LaunchServices"")]
	static extern int GetIconRef (short vRefNum, int creator, int iconType, out System.IntPtr iconRef);
				" };

				TI.TestUnifiedExecutable (test);
			});
		}
	}
}
