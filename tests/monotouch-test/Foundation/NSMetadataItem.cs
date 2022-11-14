#if __MACOS__
using AppKit;
using Foundation;
using NUnit.Framework;

namespace Xamarin.Mac.Tests {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NSMetadataItemTest {

		[Test]
		[Ignore ("Fails on bots, disabled until investigated")]
		public void CtorUrl ()
		{
			// 10.9 for NSMetadataItem initWithURL:
			TestRuntime.AssertXcodeVersion (5, 1);

			var url = NSBundle.MainBundle.BundleUrl;
			using (var mi = new NSMetadataItem (url)) {
				Assert.That (mi.DisplayName.ToString (), Is.EqualTo ("apitest"), "DisplayName");
				Assert.NotNull (mi.FileSystemContentChangeDate, "FileSystemContentChangeDate");
				Assert.NotNull (mi.FileSystemCreationDate, "FileSystemCreationDate");
				Assert.That (mi.FileSystemName.ToString (), Is.EqualTo ("apitest.app"), "FileSystemName");
				Assert.That (mi.FileSystemSize.UInt64Value, Is.GreaterThan (0), "FileSystemSize");
				Assert.False (mi.IsUbiquitous, "IsUbiquitous");
				Assert.That (mi.Path.ToString (), Does.EndWith ("/apitest.app"), "Path");
				Assert.False (mi.UbiquitousItemHasUnresolvedConflicts, "UbiquitousItemHasUnresolvedConflicts");
				Assert.False (mi.UbiquitousItemIsDownloading, "UbiquitousItemIsDownloading");
				Assert.False (mi.UbiquitousItemIsUploaded, "UbiquitousItemIsUploaded");
				Assert.False (mi.UbiquitousItemIsUploading, "UbiquitousItemIsUploading");
				Assert.That (mi.UbiquitousItemPercentDownloaded, Is.EqualTo (0), "UbiquitousItemPercentDownloaded");
				Assert.That (mi.UbiquitousItemPercentUploaded, Is.EqualTo (0), "UbiquitousItemPercentUploaded");
				Assert.Null (mi.Url, "Url");

				Assert.That (mi.ContentType.ToString (), Is.EqualTo ("com.apple.application-bundle"), "ContentType");
				Assert.That (mi.ContentTypeTree.Length, Is.GreaterThan (1), "ContentTypeTree");
#if NET
				Assert.That (mi.UbiquitousItemDownloadingStatus, Is.EqualTo (NSItemDownloadingStatus.Unknown), "UbiquitousItemDownloadingStatus");
#else
				Assert.That (mi.DownloadingStatus, Is.EqualTo (NSItemDownloadingStatus.Unknown), "DownloadingStatus");
#endif
				Assert.Null (mi.UbiquitousItemDownloadingError, "UbiquitousItemDownloadingError");
				Assert.Null (mi.UbiquitousItemUploadingError, "UbiquitousItemUploadingError");
				Assert.Null (mi.UbiquitousItemContainerDisplayName, "UbiquitousItemContainerDisplayName");
				Assert.Null (mi.UbiquitousItemUrlInLocalContainer, "UbiquitousItemUrlInLocalContainer");

				// 10.10
				if (TestRuntime.CheckXcodeVersion (6, 0)) {
					Assert.False (mi.UbiquitousItemDownloadRequested, "UbiquitousItemDownloadRequested");
					Assert.False (mi.UbiquitousItemIsExternalDocument, "UbiquitousItemIsExternalDocument");
				}
			}
		}
	}
}
#endif // __MACOS__
