using System;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using AppKit;
using Foundation;

namespace MacIssue3199 {
	public partial class ViewController : NSViewController {
		public ViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			var dt = new KeyValuePair<DateTime, decimal> [2];
			ref byte asRefByte = ref Unsafe.As<KeyValuePair<DateTime, decimal>, byte> (ref dt [0]);
		}

		public override NSObject RepresentedObject {
			get {
				return base.RepresentedObject;
			}
			set {
				base.RepresentedObject = value;
				// Update the view, if already loaded.
			}
		}
	}
}
