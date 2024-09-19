//
// Unit tests for PKJapanIndividualNumberCardMetadata
//

#if HAS_PASSKIT
using System;

using Foundation;
using CoreGraphics;
using ObjCRuntime;
using PassKit;

using NUnit.Framework;

using Xamarin.Utils;

namespace MonoTouchFixtures.PassKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class PKJapanIndividualNumberCardMetadataTest {
		[Test]
		public void Ctor_CardTemplateIdentifier ()
		{
			TestRuntime.AssertXcodeVersion (16, 0);
			var frame = new CGSize (32, 32);
			using var provider = new CGDataProvider (new byte [(int) frame.Width * (int) frame.Height * 4]);
			using var colorSpace = CGColorSpace.CreateWithName (CGColorSpaceNames.Itur_2100_PQ);
			using var img = new CGImage (0.0f, (int) frame.Width, (int) frame.Height, 8, 32, 4 * (int) frame.Width, colorSpace, CGBitmapFlags.ByteOrderDefault | CGBitmapFlags.Last, provider, null, false, CGColorRenderingIntent.Default);
			using var preview = new PKAddPassMetadataPreview (img, "description");
			using var obj = new PKJapanIndividualNumberCardMetadata ("credentialIdentifier", "sharingInstanceIdentifier", "cardTemplateIdentifier", preview, PKJapanIndividualNumberCardMetadataConstructorOption.CardTemplateIdentifier);
			Assert.IsNull (obj.AuthenticationPassword, "AuthenticationPassword");
			Assert.IsNull (obj.SigningPassword, "SigningPassword");
			// There doesn't seem to be an easy way to verify that the ctor actually worked
		}

		[Test]
		public void Ctor_CardConfigurationIdentifier ()
		{
			TestRuntime.AssertXcodeVersion (16, 0);
			var frame = new CGSize (32, 32);
			using var provider = new CGDataProvider (new byte [(int) frame.Width * (int) frame.Height * 4]);
			using var colorSpace = CGColorSpace.CreateWithName (CGColorSpaceNames.Itur_2100_PQ);
			using var img = new CGImage (0.0f, (int) frame.Width, (int) frame.Height, 8, 32, 4 * (int) frame.Width, colorSpace, CGBitmapFlags.ByteOrderDefault | CGBitmapFlags.Last, provider, null, false, CGColorRenderingIntent.Default);
			using var preview = new PKAddPassMetadataPreview (img, "description");
			using var obj = new PKJapanIndividualNumberCardMetadata ("credentialIdentifier", "sharingInstanceIdentifier", "cardConfigurationIdentifier", preview, PKJapanIndividualNumberCardMetadataConstructorOption.CardConfigurationIdentifier);
			Assert.IsNull (obj.AuthenticationPassword, "AuthenticationPassword");
			Assert.IsNull (obj.SigningPassword, "SigningPassword");
			// There doesn't seem to be an easy way to verify that the ctor actually worked
		}
	}
}
#endif // HAS_PASSKIT
