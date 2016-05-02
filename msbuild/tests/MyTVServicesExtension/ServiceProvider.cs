using System;
using Foundation;
using TVServices;

namespace MyTVServicesExtension
{
	public class ServiceProvider : NSObject, ITVTopShelfProvider
	{
		protected ServiceProvider (IntPtr handle) : base (handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

		public TVContentItem [] TopShelfItems {
			[Export ("topShelfItems")]
			get;
		}

		public TVTopShelfContentStyle TopShelfStyle {
			[Export ("topShelfStyle")]
			get;
		}
	}
}

