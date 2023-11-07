//
// UIVideo.cs: Support for saving videos
//
// Authors:
//   Miguel de Icaza
//
// Copyright 2009, Novell, Inc.
// Copyrigh 2014-2015 Xamarin Inc.
//

#if IOS

using System;
using System.Runtime.InteropServices;
using System.Collections;
using Foundation;
using ObjCRuntime;

namespace UIKit {

	[Register ("__MonoTouch_UIVideoStatusDispatcher")]
	internal class UIVideoStatusDispatcher : NSObject {
		public const string callbackSelector = "Xamarin_Internal__video:didFinishSavingWithError:contextInfo:";
		UIVideo.SaveStatus status;
		
		public UIVideoStatusDispatcher (UIVideo.SaveStatus status)
		{
			this.status = status;
			DangerousRetain ();
			IsDirectBinding = false;
		}

		[Export (callbackSelector)]
		[Preserve (Conditional = true)]
		public void Callback (string str, NSError err, IntPtr ctx)
		{
			status (str, err);
			DangerousRelease ();
		}
	}
	
	public static class UIVideo {
		public delegate void SaveStatus (string path, NSError error);
		
		[DllImport (Constants.UIKitLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		extern static /* BOOL */ bool UIVideoAtPathIsCompatibleWithSavedPhotosAlbum (/* NSString* */ IntPtr videoPath);
		
		public static bool IsCompatibleWithSavedPhotosAlbum (string path)
		{
			UIApplication.EnsureUIThread ();
			using (var ns = new NSString (path))
				return UIVideoAtPathIsCompatibleWithSavedPhotosAlbum (ns.Handle);
		}

		[DllImport (Constants.UIKitLibrary)]
		extern static void UISaveVideoAtPathToSavedPhotosAlbum (/* NSString* */ IntPtr videoPath, /* id */ IntPtr completionTarget, /* SEL */ IntPtr selector, /* void* */ IntPtr contextInfo);

		public static void SaveToPhotosAlbum (string path, SaveStatus status)
		{
			if (path is null)
				throw new ArgumentNullException ("path");
			if (status is null)
				throw new ArgumentNullException ("status");
			UIApplication.EnsureUIThread ();
			var dis = new UIVideoStatusDispatcher (status);
			
			using (var ns = new NSString (path))
				UISaveVideoAtPathToSavedPhotosAlbum (ns.Handle, dis.Handle, Selector.GetHandle (UIVideoStatusDispatcher.callbackSelector), IntPtr.Zero);
		}
		
	}
}

#endif // IOS
