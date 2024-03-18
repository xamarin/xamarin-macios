using System;
using NUnit.Framework;
using Xharness.Jenkins;
using Xharness.TestImporter;

#nullable enable

namespace Xharness.Tests.Jenkins {

	[TestFixture]
	public class TestSelectionTests {

		TestSelection selection = new TestSelection ();

		[SetUp]
		public void Setup ()
		{
			selection = new TestSelection ();
		}

		[Test]
		public void DefaultSelectionTest ()
		{
			// Assert tests
			Assert.IsTrue (selection.SelectedTests.HasFlag (TestLabel.Msbuild), "msbuild");
			Assert.IsTrue (selection.IsEnabled (TestLabel.Msbuild), "IsEnabled (msbuild)");

			Assert.IsTrue (selection.SelectedTests.HasFlag (TestLabel.Monotouch), "monotouch");
			Assert.IsTrue (selection.IsEnabled (TestLabel.Monotouch), "IsEnabled (monotouch)");

			Assert.IsTrue (selection.SelectedTests.HasFlag (TestLabel.DotnetTest), "dotnet");
			Assert.IsTrue (selection.IsEnabled (TestLabel.DotnetTest), "IsEnabled (dotnet)");

			// Assert platforms
			Assert.IsTrue (selection.SelectedPlatforms.HasFlag (PlatformLabel.tvOS), "tvos");
			Assert.IsTrue (selection.IsEnabled (PlatformLabel.tvOS), "IsEnabled (tvOS)");

			Assert.IsTrue (selection.SelectedPlatforms.HasFlag (PlatformLabel.watchOS), "watchOS");
			Assert.IsTrue (selection.IsEnabled (PlatformLabel.watchOS), "IsEnabled (watchOS)");

			Assert.IsTrue (selection.SelectedPlatforms.HasFlag (PlatformLabel.iOS), "iOS");
			Assert.IsTrue (selection.IsEnabled (PlatformLabel.iOS), "IsEnabled (iOS)");

			Assert.IsTrue (selection.SelectedPlatforms.HasFlag (PlatformLabel.iOSSimulator), "iOSSimulator");
			Assert.IsTrue (selection.IsEnabled (PlatformLabel.iOSSimulator), "IsEnabled (iOSSimulator)");

			Assert.IsTrue (selection.SelectedPlatforms.HasFlag (PlatformLabel.MacCatalyst), "maccatalyst");
			Assert.IsTrue (selection.IsEnabled (PlatformLabel.MacCatalyst), "IsEnabled (maccatalyst)");
		}

		[Test]
		public void DisableAllTests ()
		{
			selection.SetEnabled (TestLabel.All, false);
			// loop over all tests labels and except None and All, assert they are not enabled
			foreach (var obj in Enum.GetValues (typeof (TestLabel))) {
				if (obj is TestLabel label) {
					switch (label) {
					case TestLabel.All:
					case TestLabel.None:
						continue;
					default:
						Assert.IsFalse (selection.IsEnabled (label), label.ToString ());
						break;
					}
				}
			}
		}

		[Test]
		public void EnableAllTests ()
		{
			selection.SetEnabled (TestLabel.All, true);
			// loop over all tests labels and except None and All, assert they are not enabled
			foreach (var obj in Enum.GetValues (typeof (TestLabel))) {
				if (obj is TestLabel label) {
					switch (label) {
					case TestLabel.All:
					case TestLabel.None:
						continue;
					default:
						Assert.IsTrue (selection.IsEnabled (label), label.ToString ());
						break;
					}
				}
			}
		}

		[Test]
		public void EnableSingleTest ()
		{
			// disable all, then enable a single tests
			selection.SetEnabled (TestLabel.All, false);
			selection.SetEnabled (TestLabel.Bcl, true);
			foreach (var obj in Enum.GetValues (typeof (TestLabel))) {
				if (obj is TestLabel label) {
					switch (label) {
					case TestLabel.All:
					case TestLabel.None:
						continue;
					case TestLabel.Bcl:
						Assert.IsTrue (selection.IsEnabled (label), label.ToString ());
						break;
					default:
						Assert.IsFalse (selection.IsEnabled (label), label.ToString ());
						break;
					}
				}
			}
		}

		[Test]
		public void EnableSeveralTests ()
		{
			selection.SetEnabled (TestLabel.All, false);
			selection.SetEnabled (TestLabel.Bcl, true);
			selection.SetEnabled (TestLabel.Monotouch, true);
#if NET
			foreach (var obj in Enum.GetValues<TestLabel> ()) {
#else
			foreach (var obj in Enum.GetValues (typeof (TestLabel))) {
#endif
				if (obj is TestLabel label) {
					switch (label) {
					case TestLabel.All:
					case TestLabel.None:
						continue;
					case TestLabel.Bcl:
					case TestLabel.Monotouch:
						Assert.IsTrue (selection.IsEnabled (label), label.ToString ());
						break;
					default:
						Assert.IsFalse (selection.IsEnabled (label), label.ToString ());
						break;
					}
				}
			}
		}
	}
}
