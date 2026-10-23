//
// MAImageCaptioning Unit Tests
//
// Authors:
//	Sebastien Pouliot  <sebastien.pouliot@microsoft.com>
//
// Copyright 2019 Microsoft Corporation
//

using System.Diagnostics;
using System.IO;
using MediaAccessibility;
using MonoTests.System.Net.Http;

namespace MonoTouchFixtures.MediaAccessibility {

	[TestFixture]
	// we want the test to be availble if we use the linker
	[Preserve (AllMembers = true)]
	public class ImageCaptioningTest {

		[Test]
		public void GetCaption ()
		{
			TestRuntime.AssertXcodeVersion (11, 0);
			Assert.Throws<ArgumentNullException> (() => MAImageCaptioning.GetCaption (null, out _));
			using (NSUrl url = new NSUrl (NetworkResources.MicrosoftUrl)) {
				var s = MAImageCaptioning.GetCaption (url, out var e);
				Assert.That (s, Is.Null, "remote / return value");
				Assert.That (e, Is.Null.Or.Not.Null, "remote / error"); // sometimes we get an error, and sometimes we don't 🤷‍♂️
			}
			string file = Path.Combine (NSBundle.MainBundle.ResourcePath, "basn3p08.png");
			file = file.Replace (" ", "%20");
			using (NSUrl url = new NSUrl (file)) {
				var s = MAImageCaptioning.GetCaption (url, out var e);
				Assert.That (s, Is.Null, "local / return value");
				Assert.That (e, Is.Not.Null, "local / error"); // does not like the URL (invalid)
			}
			file = NSBundle.MainBundle.ResourceUrl.AbsoluteString + "basn3p08.png";
			file = file.Replace (" ", "%20");
			using (NSUrl url = new NSUrl (file)) {
				var s = MAImageCaptioning.GetCaption (url, out var e);
				Assert.That (s, Is.Null, "local / return value");
				Assert.That (e, Is.Null, "local / no error");
			}
		}

		[Test]
		public void GetMetadataTagPath ()
		{
			TestRuntime.AssertXcodeVersion (11, 0);
			// https://iptc.org/standards/photo-metadata/iptc-standard/
			// but headers mention `Iptc4xmpExt:AOContentDescription` for the Get/Set API
			Assert.That (MAImageCaptioning.GetMetadataTagPath (), Is.EqualTo ("Iptc4xmpExt:ArtworkContentDescription"));
		}

		[Test]
#if APP_EXTENSION
		[Ignore ("App extension bundle resources are read-only.")]
#endif
		public void SetCaption ()
		{
			TestRuntime.AssertXcodeVersion (11, 0);
			Assert.Throws<ArgumentNullException> (() => MAImageCaptioning.SetCaption (null, "xamarin", out _));
			// note: calling on a remote URL crash the process - not that it should work but...

			var temp = String.Empty;
			try {
				using (NSUrl url = new NSUrl (NSBundle.MainBundle.ResourceUrl.AbsoluteString + "basn3p08.png")) {
#if __MACOS__ || __MACCATALYST__
					var read_only = false;
#else
					var read_only = Runtime.Arch == Arch.DEVICE;
#endif
					if (read_only) {
						Assert.That (MAImageCaptioning.SetCaption (url, "xamarin", out var e), Is.False, "Set");
						Assert.That (e, Is.Not.Null, "ro / set / no error"); // weird, it can't be saved back to the file metadata

						var s = MAImageCaptioning.GetCaption (url, out e);
						Assert.That (s, Is.Null, "ro / roundtrip 1"); // not very surprising since Set can't save it
						Assert.That (e, Is.Null, "ro / get / no error");

						Assert.That (MAImageCaptioning.SetCaption (url, "xamarin", out e), Is.False, "Set 2");
						s = MAImageCaptioning.GetCaption (url, out e);
						Assert.That (s, Is.Null, "ro / back to original");
						Assert.That (e, Is.Null, "ro / get back / no error");
					} else {
						Assert.That (MAImageCaptioning.SetCaption (url, "xamarin", out var e), Is.True, "Set");
						Assert.That (e, Is.Null, "ro / set / no error"); // weird, it can't be saved back to the file metadata

						var s = MAImageCaptioning.GetCaption (url, out e);
						if (TestRuntime.CheckXcodeVersion (12, TestRuntime.MinorXcode12APIMismatch)) {
							Assert.That (s, Is.EqualTo ("xamarin"), "ro / roundtrip 2");
						} else {
							Assert.That (s, Is.Null, "ro / roundtrip 3"); // not very surprising since Set can't save it
						}
						Assert.That (e, Is.Null, "ro / get / no error");

						Assert.That (MAImageCaptioning.SetCaption (url, "xamarin", out e), Is.True, "Set 2");
						s = MAImageCaptioning.GetCaption (url, out e);
						if (TestRuntime.CheckXcodeVersion (12, TestRuntime.MinorXcode12APIMismatch)) {
							Assert.That (s, Is.EqualTo ("xamarin"), "ro / back to original");
						} else {
							Assert.That (s, Is.Null, "ro / back to original");
						}
						Assert.That (e, Is.Null, "ro / get back / no error");

						// Restore original value
						Assert.That (MAImageCaptioning.SetCaption (url, null, out e), Is.True, "Set 2");
						s = MAImageCaptioning.GetCaption (url, out e);
						Assert.That (s, Is.Null, "ro / back to null");
						Assert.That (e, Is.Null, "ro / get back null / no error");
					}

					// 2nd try with a read/write copy
					temp = Path.Combine (Path.GetTempPath (), $"basn3p08-{Process.GetCurrentProcess ().Id}.png");
					File.Copy (url.Path, temp, overwrite: true);
				}
				using (var rw_url = NSUrl.FromFilename (temp)) {
					Assert.That (MAImageCaptioning.SetCaption (rw_url, "xamarin", out var e), Is.True, "Set");
					Assert.That (e, Is.Null, "rw / set / no error"); // weird, it can't be saved back to the file metadata

					var s = MAImageCaptioning.GetCaption (rw_url, out e);
					if (TestRuntime.CheckXcodeVersion (12, TestRuntime.MinorXcode12APIMismatch)) {
						Assert.That (s, Is.EqualTo ("xamarin"), "rw / roundtrip"); // :)
					} else {
						Assert.That (s, Is.Null, "rw / roundtrip"); // :(
					}
					Assert.That (e, Is.Null, "rw / get / no error");

					Assert.That (MAImageCaptioning.SetCaption (rw_url, "xamarin", out e), Is.True, "Set 2");
					s = MAImageCaptioning.GetCaption (rw_url, out e);
					if (TestRuntime.CheckXcodeVersion (12, TestRuntime.MinorXcode12APIMismatch)) {
						Assert.That (s, Is.EqualTo ("xamarin"), "rw / back to original");
					} else {
						Assert.That (s, Is.Null, "rw / back to original");
					}
					Assert.That (e, Is.Null, "rw / get back / no error");
				}

			} finally {
				if (File.Exists (temp))
					File.Delete (temp);
			}
		}
	}
}
