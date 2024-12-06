using System;
using System.IO;
using UIKit;
using MonoTouch.Dialog;
using MonoTouch.Dialog.Utilities;
using System.Threading;
using System.Drawing;

namespace Sample
{
	public partial class AppDelegate
	{
		public void DemoStyled () 
		{
			var imageBackground = new Uri ("file://" + Path.GetFullPath ("background.png"));
			var image = ImageLoader.DefaultRequestImage (imageBackground, null);
			var small = image.Scale (new SizeF (32, 32));
			
			var imageIcon = new StyledStringElement ("Local image icon") {
				Image = small
			};
			var backgroundImage = new StyledStringElement ("Image downloaded") {
				BackgroundUri = new Uri ("http://www.google.com/images/logos/ps_logo2.png")
			};
			var localImage = new StyledStringElement ("Local image"){
				BackgroundUri = imageBackground
			};
			
			var backgroundSolid = new StyledStringElement ("Solid background") {
				BackgroundColor = UIColor.Green
			};
			var colored = new StyledStringElement ("Colored", "Detail in Green") {
				TextColor = UIColor.Yellow,
				BackgroundColor = UIColor.Red,
				DetailColor = UIColor.Green,
			};
			var sse = new StyledStringElement ("DetailDisclosureIndicator") { Accessory = UITableViewCellAccessory.DetailDisclosureButton };
			sse.AccessoryTapped += delegate {
				var alertController = UIAlertController.Create ("Accessory", "Accessory clicked", UIAlertControllerStyle.Alert);
				alertController.AddAction (UIAlertAction.Create ("Ok", UIAlertActionStyle.Default, (obj) => { }));
				window.RootViewController.PresentViewController (alertController, true, () => { });
			};
			var root = new RootElement("Styled Elements") {
				new Section ("Image icon"){
					imageIcon
				},
				new Section ("Background") { 
					backgroundImage, backgroundSolid, localImage
				},
				new Section ("Text Color"){
					colored
				},
				new Section ("Cell Styles"){
					new StyledStringElement ("Default", "Invisible value", UITableViewCellStyle.Default),
					new StyledStringElement ("Value1", "Aligned on each side", UITableViewCellStyle.Value1),
					new StyledStringElement ("Value2", "Like the Addressbook", UITableViewCellStyle.Value2),
					new StyledStringElement ("Subtitle", "Makes it sound more important", UITableViewCellStyle.Subtitle),
					new StyledStringElement ("Subtitle", "Brown subtitle", UITableViewCellStyle.Subtitle) {
						 DetailColor = UIColor.Brown
					}
				},
				new Section ("Accessories"){
					new StyledStringElement ("DisclosureIndicator") { Accessory = UITableViewCellAccessory.DisclosureIndicator },
					new StyledStringElement ("Checkmark") { Accessory = UITableViewCellAccessory.Checkmark },
					sse
				}
			};
			
			var dvc = new DialogViewController (root, true);
			navigation.PushViewController (dvc, true);
		}
	}
}

