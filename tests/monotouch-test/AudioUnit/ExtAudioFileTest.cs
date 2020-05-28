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
using Foundation;
using AudioUnit;
using CoreFoundation;
using System.IO;

namespace MonoTouchFixtures.AudioUnit {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class ExtAudioFileTest
	{
		[Test]
		public void WrapAudioFileID ()
		{
#if MONOMAC
			var path = NSBundle.MainBundle.PathForResource ("1", "caf", "AudioToolbox");
#else
			var path = Path.GetFullPath (Path.Combine ("AudioToolbox", "1.caf"));
#endif
			using (var file = ExtAudioFile.OpenUrl (CFUrl.FromFile (path))) {
				Assert.IsNotNull (file.AudioFile, "#1");

				ExtAudioFile f2;
				Assert.AreEqual (ExtAudioFileError.OK, ExtAudioFile.WrapAudioFileID (file.AudioFile.Value, true, out f2));
			}
		}

		[Test]
		public void ClientDataFormat ()
		{
#if MONOMAC
			var path = NSBundle.MainBundle.PathForResource ("1", "caf", "AudioToolbox");
#else
			var path = Path.GetFullPath (Path.Combine ("AudioToolbox", "1.caf"));
#endif
			using (var file = ExtAudioFile.OpenUrl (CFUrl.FromFile (path))) {
				var fmt = file.ClientDataFormat;
			}
		}

		[Test]
		public void OpenNSUrlTest ()
		{
#if MONOMAC
			var path = NSBundle.MainBundle.PathForResource ("1", "caf", "AudioToolbox");
#else
			var path = Path.GetFullPath (Path.Combine ("AudioToolbox", "1.caf"));
#endif
			ExtAudioFileError err;
			using (var file = ExtAudioFile.OpenUrl (NSUrl.FromFilename (path), out err)) {
				Assert.IsTrue (err == ExtAudioFileError.OK, "OpenNSUrlTest");
				Assert.IsNotNull (file.AudioFile, "OpenNSUrlTest");
			}
		}

		[Test]
		public void OpenCFUrlTest ()
		{
#if MONOMAC
			var path = NSBundle.MainBundle.PathForResource ("1", "caf", "AudioToolbox");
#else
			var path = Path.GetFullPath (Path.Combine ("AudioToolbox", "1.caf"));
#endif
			ExtAudioFileError err;
			using (var file = ExtAudioFile.OpenUrl (CFUrl.FromFile (path), out err)) {
				Assert.IsTrue (err == ExtAudioFileError.OK, "OpenCFUrlTest");
				Assert.IsNotNull (file.AudioFile, "OpenCFUrlTest");
			}
		}
	}
}

#endif // !__WATCHOS__
