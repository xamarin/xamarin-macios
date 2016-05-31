//
// Unit tests for ExtAudioFile
//
// Authors:
//	Marek Safar (marek.safar@gmail.com)
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

#if !__WATCHOS__

using NUnit.Framework;
#if XAMCORE_2_0
using Foundation;
using AudioUnit;
using CoreFoundation;
#else
using MonoTouch.Foundation;
using MonoTouch.AudioUnit;
using MonoTouch.CoreFoundation;
#endif
using System.IO;

namespace MonoTouchFixtures.AudioUnit {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class ExtAudioFileTest
	{
		[Test]
		public void WrapAudioFileID ()
		{
			var path = Path.GetFullPath (Path.Combine ("AudioToolbox", "1.caf"));
			using (var file = ExtAudioFile.OpenUrl (CFUrl.FromFile (path))) {
				Assert.IsNotNull (file.AudioFile, "#1");

				ExtAudioFile f2;
				Assert.AreEqual (ExtAudioFileError.OK, ExtAudioFile.WrapAudioFileID (file.AudioFile.Value, true, out f2));
			}
		}

		[Test]
		public void ClientDataFormat ()
		{
			var path = Path.GetFullPath (Path.Combine ("AudioToolbox", "1.caf"));
			using (var file = ExtAudioFile.OpenUrl (CFUrl.FromFile (path))) {
				var fmt = file.ClientDataFormat;
			}
		}

		[Test]
		public void OpenNSUrlTest ()
		{
			var path = Path.GetFullPath (Path.Combine ("AudioToolbox", "1.caf"));
			ExtAudioFileError err;
			using (var file = ExtAudioFile.OpenUrl (NSUrl.FromFilename (path), out err)) {
				Assert.IsTrue (err == ExtAudioFileError.OK, "OpenNSUrlTest");
				Assert.IsNotNull (file.AudioFile, "OpenNSUrlTest");
			}
		}

		[Test]
		public void OpenCFUrlTest ()
		{
			var path = Path.GetFullPath (Path.Combine ("AudioToolbox", "1.caf"));
			ExtAudioFileError err;
			using (var file = ExtAudioFile.OpenUrl (CFUrl.FromFile (path), out err)) {
				Assert.IsTrue (err == ExtAudioFileError.OK, "OpenCFUrlTest");
				Assert.IsNotNull (file.AudioFile, "OpenCFUrlTest");
			}
		}
	}
}

#endif // !__WATCHOS__
