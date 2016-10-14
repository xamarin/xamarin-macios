#if !XAMCORE_2_0
using MonoMac.Foundation;
#else
using AppKit;
using Foundation;
#endif
using NUnit.Framework;

namespace Xamarin.Mac.Tests {

	[TestFixture]
	public class NSMetadataItemTest {
		
		[Test]
		public void CtorUrl ()
		{
			var url = NSRunningApplication.CurrentApplication.BundleUrl;
			using (var mi = new NSMetadataItem (url)) {
				Assert.That (mi.DisplayName.ToString (), Is.EqualTo ("apitest"), "DisplayName");
				Assert.NotNull (mi.FileSystemContentChangeDate, "FileSystemContentChangeDate");
				Assert.NotNull (mi.FileSystemCreationDate, "FileSystemCreationDate");
				Assert.That (mi.FileSystemName.ToString (), Is.EqualTo ("apitest.app"), "FileSystemName");
				Assert.That (mi.FileSystemSize.UInt64Value, Is.GreaterThan (0), "FileSystemSize");
				Assert.False (mi.IsUbiquitous, "IsUbiquitous");
				Assert.That (mi.Path.ToString (), Is.StringEnding ("/apitest.app"), "Path");
				Assert.False (mi.UbiquitousItemHasUnresolvedConflicts, "UbiquitousItemHasUnresolvedConflicts");
				Assert.False (mi.UbiquitousItemIsDownloading, "UbiquitousItemIsDownloading");
				Assert.False (mi.UbiquitousItemIsUploaded, "UbiquitousItemIsUploaded");
				Assert.False (mi.UbiquitousItemIsUploading, "UbiquitousItemIsUploading");
				Assert.That (mi.UbiquitousItemPercentDownloaded, Is.EqualTo (0), "UbiquitousItemPercentDownloaded");
				Assert.That (mi.UbiquitousItemPercentUploaded, Is.EqualTo (0), "UbiquitousItemPercentUploaded");
				Assert.Null (mi.Url, "Url");

				// 10.9
				if (TestRuntime.CheckXcodeVersion (5,1)) {
					Assert.That (mi.ContentType.ToString (), Is.EqualTo ("com.apple.application-bundle"), "ContentType");
					Assert.That (mi.ContentTypeTree.Length, Is.GreaterThan (1), "ContentTypeTree");
					Assert.That (mi.DownloadingStatus, Is.EqualTo (NSItemDownloadingStatus.Unknown), "DownloadingStatus");
					Assert.Null (mi.UbiquitousItemDownloadingError, "UbiquitousItemDownloadingError");
					Assert.Null (mi.UbiquitousItemUploadingError, "UbiquitousItemUploadingError");
					Assert.Null (mi.UbiquitousItemContainerDisplayName, "UbiquitousItemContainerDisplayName");
					Assert.Null (mi.UbiquitousItemUrlInLocalContainer, "UbiquitousItemUrlInLocalContainer");
				}
				// 10.10
				if (TestRuntime.CheckXcodeVersion (6,0)) {
					Assert.False (mi.UbiquitousItemDownloadRequested, "UbiquitousItemDownloadRequested");
					Assert.False (mi.UbiquitousItemIsExternalDocument, "UbiquitousItemIsExternalDocument");
				}
			}
		}
	}
}
