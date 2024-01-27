//
// Unit tests for AudioFileGlobalInfo
//
// Authors:
//	Marek Safar (marek.safar@gmail.com)
//
// Copyright 2012 Xamarin Inc. All rights reserved.
//

#nullable enable

#if !__WATCHOS__
using System;
using System.Linq;

using Foundation;
using AudioToolbox;

using NUnit.Framework;

namespace MonoTouchFixtures.AudioToolbox {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class AudioFileGlobalInfoTest {
		[Test]
		public void Properties ()
		{
			Assert.Multiple (() => {
				Assert.NotNull (AudioFileGlobalInfo.ReadableTypes, "ReadableTypes");
				Assert.That (AudioFileGlobalInfo.ReadableTypes?.Length, Is.GreaterThan (0), "ReadableTypes #");

				Assert.NotNull (AudioFileGlobalInfo.WritableTypes, "WritableTypes");
				Assert.That (AudioFileGlobalInfo.WritableTypes?.Length, Is.GreaterThan (0), "WritableTypes #");

				var validFileTypeAndAudioFormatTypeCombinations = 0;
#if NET
				var validAudioFileTypes = Enum.GetValues<AudioFileType> ().ToList ();
#else
				var validAudioFileTypes = Enum.GetValues (typeof (AudioFileType)).Cast<AudioFileType> ().ToList ();
#endif
				validAudioFileTypes.Remove (AudioFileType.SoundDesigner2); // returns null in most APIs below
				validAudioFileTypes.Remove (AudioFileType.AAC_ADTS); // doesn't work on macOS 11
				validAudioFileTypes.Remove (AudioFileType.AMR); // doesn't work on macOS 11
				validAudioFileTypes.Remove (AudioFileType.MP1); // doesn't work on macOS 11
				validAudioFileTypes.Remove (AudioFileType.MP2); // doesn't work on macOS 11

				foreach (var fileType in validAudioFileTypes) {
					Assert.NotNull (AudioFileGlobalInfo.GetFileTypeName (fileType), $"GetFileTypeName: {fileType}");

					Assert.NotNull (AudioFileGlobalInfo.GetAvailableFormats (fileType), $"GetAvailableFormats: {fileType}");
					Assert.That (AudioFileGlobalInfo.GetAvailableFormats (fileType)?.Length ?? -1, Is.GreaterThan (0), $"GetAvailableFormats #: {fileType}");

					Assert.NotNull (AudioFileGlobalInfo.GetExtensions (fileType), $"GetExtensions: {fileType}");
					Assert.That (AudioFileGlobalInfo.GetExtensions (fileType)?.Length ?? -1, Is.GreaterThan (0), $"GetExtensions #: {fileType}");

					Assert.NotNull (AudioFileGlobalInfo.GetMIMETypes (fileType), $"GetMIMETypes: {fileType}");
					Assert.That (AudioFileGlobalInfo.GetMIMETypes (fileType)?.Length ?? -1, Is.GreaterThan (0), $"GetMIMETypes #: {fileType}");

					Assert.NotNull (AudioFileGlobalInfo.GetUTIs (fileType), $"GetUTIs: {fileType}");
					Assert.That (AudioFileGlobalInfo.GetUTIs (fileType)?.Length ?? -1, Is.GreaterThan (0), $"GetUTIs #: {fileType}");

#if NET
					foreach (var audioFormatType in Enum.GetValues<AudioFormatType> ()) {
#else
					foreach (AudioFormatType audioFormatType in Enum.GetValues (typeof (AudioFormatType))) {
#endif
						var descs = AudioFileGlobalInfo.GetAvailableStreamDescriptions (fileType, audioFormatType);
						if (descs is not null) {
							validFileTypeAndAudioFormatTypeCombinations++;
							Assert.That (descs?.Length ?? -1, Is.GreaterThan (0), $"GetAvailableStreamDescriptions ({fileType}, {audioFormatType}) #");
						}
					}
				}
				Assert.That (validFileTypeAndAudioFormatTypeCombinations, Is.GreaterThan (50), "Valid FileType And AudioFormatType Combinations");

				Assert.NotNull (AudioFileGlobalInfo.AllExtensions, "AllExtensions");
				Assert.That (AudioFileGlobalInfo.AllExtensions.Length, Is.GreaterThan (0), $"AllExtensions #");
			});
		}
	}
}

#endif // !__WATCHOS__
