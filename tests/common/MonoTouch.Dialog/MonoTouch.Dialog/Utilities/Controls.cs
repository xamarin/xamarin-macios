using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

using UIKit;
using CoreGraphics;
using Foundation;
using CoreAnimation;

using MonoTouch.Dialog.Utilities;

namespace MonoTouch.Dialog {
	public enum RefreshViewStatus {
		ReleaseToReload,
		PullToReload,
		Loading
	}

	// This cute method will be added to UIImage.FromResource, but for old installs 
	// make a copy here
	internal static class Util {
		public static UIImage FromResource (Assembly assembly, string name)
		{
			if (name is null)
				throw new ArgumentNullException ("name");
			assembly = Assembly.GetCallingAssembly ();
			var stream = assembly.GetManifestResourceStream (name);
			if (stream is null)
				return null;

			try {
				using (var data = NSData.FromStream (stream))
					return UIImage.LoadFromData (data);
			} finally {
				stream.Dispose ();
			}
		}

	}

	public class SearchChangedEventArgs : EventArgs {
		public SearchChangedEventArgs (string text)
		{
			Text = text;
		}
		public string Text { get; set; }
	}
}
