using System;
using System.Drawing;

using Foundation;
using UIKit;

namespace MyExtensionWithPackageReference
{
	public partial class ActionViewController : UIViewController
	{
		public ActionViewController (IntPtr handle) : base (handle)
		{
			Console.WriteLine (typeof (Newtonsoft.Json.JsonReader));
		}

		partial void DoneClicked (NSObject sender)
		{
		}
	}
}

