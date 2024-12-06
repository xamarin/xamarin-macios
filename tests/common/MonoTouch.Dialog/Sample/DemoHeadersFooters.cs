using System;
using System.Drawing;
using System.Linq;
using UIKit;
using MonoTouch.Dialog;

namespace Sample {
	public partial class AppDelegate {
		public void DemoHeadersFooters ()
		{
			var section = new Section () {
				HeaderView = new UIImageView (UIImage.FromFile ("caltemplate.png")),
#if !__TVOS__
				FooterView = new UISwitch (new RectangleF (0, 0, 80, 30)),
#endif // !__TVOS__
			};

			// Fill in some data 
			var linqRoot = new RootElement ("LINQ source"){
				from x in new string [] { "one", "two", "three" }
					select new Section (x) {
						from y in "Hello:World".Split (':')
							select (Element) new StringElement (y)
				}
			};

			section.Add (new RootElement ("Desert", new RadioGroup ("desert", 0)){
				new Section () {
					new RadioElement ("Ice Cream", "desert"),
					new RadioElement ("Milkshake", "desert"),
					new RadioElement ("Chocolate Cake", "desert")
				},
			});

			var root = new RootElement ("Headers and Footers") {
				section,
				new Section () { linqRoot }
			};
			var dvc = new DialogViewController (root, true);
			navigation.PushViewController (dvc, true);
		}

	}
}
