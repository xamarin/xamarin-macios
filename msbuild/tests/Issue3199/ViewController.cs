using System;

using UIKit;
using System.Runtime.CompilerServices;
using System.Collections.Generic;

namespace Issue3199 {
	public partial class ViewController : UIViewController {
		protected ViewController (IntPtr handle) : base (handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			// Perform any additional setup after loading the view, typically from a nib.

			var dt = new KeyValuePair<DateTime, decimal> [2];
			ref byte asRefByte = ref Unsafe.As<KeyValuePair<DateTime, decimal>, byte> (ref dt [0]);
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}
	}
}
