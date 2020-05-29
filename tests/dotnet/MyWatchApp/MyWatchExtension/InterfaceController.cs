using System;

using WatchKit;
using Foundation;

namespace MyWatchExtension
{
	public partial class InterfaceController : WKInterfaceController
	{
		public InterfaceController (IntPtr handle) : base (handle)
		{
		}

		public override void Awake (NSObject context)
		{
			base.Awake (context);

			Console.WriteLine ("{0} awake with context", this);
		}

		public override void WillActivate ()
		{
			Console.WriteLine ("{0} will activate", this);
		}

		public override void DidDeactivate ()
		{
			Console.WriteLine ("{0} did deactivate", this);
		}
	}
}
