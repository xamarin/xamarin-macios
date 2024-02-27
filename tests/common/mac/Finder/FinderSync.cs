using AppKit;
using FinderSync;
using Foundation;
using Social;

namespace FinderExtensionTest {
	[Register ("FinderSync")]
	public partial class FinderSync : FIFinderSync {
		public override void BeginRequestWithExtensionContext (NSExtensionContext context)
		{
		}

		public override NSMenu GetMenu (FIMenuKind menuKind)
		{
			NSMenu menu = new NSMenu ("");
			menu.AddItem ("Menu Item from C#", new ObjCRuntime.Selector ("sampleAction:"), "");
			return menu;
		}

		public override string ToolbarItemName {
			get {
				return "FinderExtension";
			}
		}

		public override string ToolbarItemToolTip {
			get {
				return "FinderExtension: Click the toolbar item for a menu.";
			}
		}

		public override NSImage ToolbarItemImage {
			get {
				return NSImage.ImageNamed (NSImageName.Caution);
			}
		}

		[Export ("sampleAction:")]
		public void Action (NSObject sender)
		{
		}
	}
}
