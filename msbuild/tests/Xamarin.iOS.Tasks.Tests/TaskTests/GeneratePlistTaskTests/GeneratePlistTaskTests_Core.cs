using System.IO;

using NUnit.Framework;

using Xamarin.MacDev;
using System.Linq;

namespace Xamarin.iOS.Tasks
{
	[TestFixture]
	public abstract class GeneratePlistTaskTests_Core : TestBase
	{
		protected const string appBundleName = "BundleName";
		protected const string assemblyName = "AssemblyName";
		protected const string bundleIdentifier = "DefaultIdentifier";
		protected const string displayName = "DisplayName";
		protected const string identifier = "Identifier";

		protected PDictionary Plist {
			get; set;
		}

		protected PDictionary CompiledPlist {
			get; set;
		}

		protected CompileAppManifestTaskBase Task {
			get; set;
		}

		public virtual void ConfigureTask ()
		{
			Task = CreateTask<CompileAppManifest> ();

			Task.AppBundleName = appBundleName;
			Task.AppBundleDir = "AppBundlePath";
			Task.AssemblyName = assemblyName;
			Task.AppManifest = CreateTempFile ("foo.plist");
			Task.BundleIdentifier = bundleIdentifier;
			Task.SdkPlatform = "iPhoneSimulator";

			Plist = new PDictionary ();
			Plist ["CFBundleDisplayName"] = displayName;
			Plist ["CFBundleIdentifier"] = identifier;
			Plist.Save (Task.AppManifest);
		}

		public override void Setup ()
		{
			base.Setup ();

			ConfigureTask ();

			Task.Execute ();
			CompiledPlist = PDictionary.FromFile (Task.CompiledAppManifest.ItemSpec);
		}

		#region General tests
		[Test]
		public void PlistMissing ()
		{
			File.Delete (Task.AppManifest);
			Assert.IsFalse (Task.Execute (), "#1");
		}

		[Test]
		public void NormalPlist ()
		{
			Assert.IsTrue (Task.Execute (), "#1");
			Assert.IsNotNull (Task.CompiledAppManifest, "#2");
			Assert.IsTrue (File.Exists (Task.CompiledAppManifest.ItemSpec), "#3");
		}

		[Test]
		public void MissingBundleIdentifier ()
		{
			Plist.Remove ("CFBundleIdentifier");
			Plist.Save (Task.AppManifest);
			Assert.IsTrue (Task.Execute (), "#1");
		}

		[Test]
		public void MissingDisplayName ()
		{
			Plist.Remove ("CFBundleDisplayName");
			Plist.Save (Task.AppManifest);
			Assert.IsTrue (Task.Execute (), "#1");
		}

		[Test]
		public virtual void XamarinVersion ()
		{
			Assert.That (CompiledPlist.ContainsKey ("com.xamarin.ios"), "#1");
			Assert.IsNotNullOrEmpty (CompiledPlist.Get<PDictionary> ("com.xamarin.ios").GetString ("Version").Value, "#2");
		}
		#endregion

		#region Keys tests
		[Test]
		public void BuildMachineOSBuild ()
		{
			Assert.That (CompiledPlist.ContainsKey (ManifestKeys.BuildMachineOSBuild), "#1");
			Assert.IsNotNullOrEmpty (CompiledPlist.Get<PString> (ManifestKeys.BuildMachineOSBuild).Value, "#2");
		}

		[Test]
		public void BundleDevelopmentRegion ()
		{
			Assert.IsFalse (CompiledPlist.ContainsKey (ManifestKeys.CFBundleDevelopmentRegion), "#1");
		}

		[Test]
		public virtual void BundleExecutable ()
		{
			Assert.That (CompiledPlist.ContainsKey (ManifestKeys.CFBundleExecutable), "#1");
			Assert.AreEqual (CompiledPlist.Get<PString> (ManifestKeys.CFBundleExecutable).Value, assemblyName, "#2");
		}

		[Test]
		public virtual void BundleName ()
		{
			Assert.That (CompiledPlist.ContainsKey (ManifestKeys.CFBundleName), "#1");
			Assert.AreEqual (CompiledPlist.Get<PString> (ManifestKeys.CFBundleName).Value, appBundleName, "#2");
		}

		[Test]
		public virtual void BundleIdentifier ()
		{
			Assert.That (CompiledPlist.ContainsKey (ManifestKeys.CFBundleIdentifier), "#1");
			Assert.AreEqual (CompiledPlist.Get<PString> (ManifestKeys.CFBundleIdentifier).Value, identifier, "#2");
		}

		[Test]
		public virtual void BundleInfoDictionaryVersion ()
		{
			Assert.That (CompiledPlist.ContainsKey (ManifestKeys.CFBundleInfoDictionaryVersion), "#1");
			Assert.IsNotNullOrEmpty (CompiledPlist.Get<PString> (ManifestKeys.CFBundleInfoDictionaryVersion).Value, "#2");
		}

		[Test]
		public virtual void BundlePackageType ()
		{
			Assert.That (CompiledPlist.ContainsKey (ManifestKeys.CFBundlePackageType), "#1");
			Assert.AreEqual (CompiledPlist.Get<PString> (ManifestKeys.CFBundlePackageType).Value, "APPL", "#2");
		}

		[Test]
		public virtual void BundleSignature ()
		{
			Assert.That (CompiledPlist.ContainsKey (ManifestKeys.CFBundleSignature), "#1");
			Assert.AreEqual (CompiledPlist.Get<PString> (ManifestKeys.CFBundleSignature).Value, "????", "#2");
		}

		[Test]
		public virtual void BundleSupportedPlatforms ()
		{
			Assert.That (CompiledPlist.ContainsKey (ManifestKeys.CFBundleSupportedPlatforms), "#1");
			Assert.That (CompiledPlist.Get<PArray> (ManifestKeys.CFBundleSupportedPlatforms).Any (), "#2");
		}

		[Test]
		public virtual void BundleVersion ()
		{
			Assert.That (CompiledPlist.ContainsKey (ManifestKeys.CFBundleVersion), "#1");
			Assert.IsNotNullOrEmpty (CompiledPlist.Get<PString> (ManifestKeys.CFBundleVersion).Value, "#2");
		}

		[Test]
		public virtual void VerifyAllDT ()
		{
			const string dtCompiler = "DTCompiler";
			const string dtPlatformBuild = "DTPlatformBuild";
			const string dtSDKBuild = "DTSDKBuild";
			const string dtPlatformName = "DTPlatformName";
			const string dtPlatformVersion = "DTPlatformVersion";
			const string dtSDKName = "DTSDKName";
			const string dtXcode = "DTXcode";
			const string dtXcodeBuild = "DTXcodeBuild";
			Assert.That (CompiledPlist.ContainsKey (dtCompiler), "#1");
			Assert.That (CompiledPlist.ContainsKey (dtPlatformBuild), "#2");
			Assert.That (CompiledPlist.ContainsKey (dtSDKBuild), "#2");
			Assert.That (CompiledPlist.ContainsKey (dtPlatformName), "#3");
			Assert.That (CompiledPlist.ContainsKey (dtPlatformVersion), "#4");
			Assert.That (CompiledPlist.ContainsKey (dtSDKName), "#5");
			Assert.That (CompiledPlist.ContainsKey (dtXcode), "#6");
			Assert.That (CompiledPlist.ContainsKey (dtXcodeBuild), "#7");
			Assert.IsNotNullOrEmpty (CompiledPlist.Get<PString> (dtCompiler).Value, "#8");
			Assert.IsNotNullOrEmpty (CompiledPlist.Get<PString> (dtPlatformBuild).Value, "#9");
			Assert.IsNotNullOrEmpty (CompiledPlist.Get<PString> (dtSDKBuild).Value, "#10");
			Assert.IsNotNullOrEmpty (CompiledPlist.Get<PString> (dtPlatformName).Value, "#11");
			Assert.IsNotNullOrEmpty (CompiledPlist.Get<PString> (dtPlatformVersion).Value, "#12");
			Assert.IsNotNullOrEmpty (CompiledPlist.Get<PString> (dtSDKName).Value, "#13");
			Assert.IsNotNullOrEmpty (CompiledPlist.Get<PString> (dtXcode).Value, "#14");
			Assert.IsNotNullOrEmpty (CompiledPlist.Get<PString> (dtXcodeBuild).Value, "#15");
		}

		[Test]
		public virtual void DeviceFamily ()
		{
			Assert.That (CompiledPlist.ContainsKey (ManifestKeys.UIDeviceFamily), "#1");
			Assert.That (CompiledPlist.Get<PArray> (ManifestKeys.UIDeviceFamily).Any (), "#2");
		}

		[Test]
		public virtual void MinimumOSVersion ()
		{
			Assert.That (CompiledPlist.ContainsKey (ManifestKeys.MinimumOSVersion), "#1");
			Assert.IsNotNullOrEmpty (CompiledPlist.Get<PString> (ManifestKeys.MinimumOSVersion).Value, "#2");
		}
		#endregion
	}
}

