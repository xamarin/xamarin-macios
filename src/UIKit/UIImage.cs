//
// UIImage.cs: Extra helper routines for images
//
// Authors:
//   Miguel de Icaza
//
// Copyright 2009, Novell, Inc.
// Copyrigh 2014 Xamarin Inc.
//
using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Collections;

using Foundation;
using ObjCRuntime;
using CoreGraphics;

namespace UIKit {

	partial class UIImage {
#if IOS
		public delegate void SaveStatus (UIImage image, NSError error);

		[DllImport (Constants.UIKitLibrary)]
		extern static void UIImageWriteToSavedPhotosAlbum (/* UIImage */ IntPtr image, /* id */ IntPtr obj, /* SEL */ IntPtr selector, /*vcoid* */ IntPtr ctx);

		public void SaveToPhotosAlbum (SaveStatus status)
		{
			UIImageStatusDispatcher dis = null;
			UIApplication.EnsureUIThread ();			

			if (status != null)
				dis = new UIImageStatusDispatcher (status);
			
			UIImageWriteToSavedPhotosAlbum (Handle, dis != null ? dis.Handle : IntPtr.Zero, dis != null ? Selector.GetHandle (UIImageStatusDispatcher.callbackSelector) : IntPtr.Zero, IntPtr.Zero);
		}
#endif

		[DllImport (Constants.UIKitLibrary)]
		extern static /* NSData */ IntPtr UIImagePNGRepresentation (/* UIImage */ IntPtr image);

		public NSData AsPNG ()
		{
			using (var pool = new NSAutoreleasePool ())
				return (NSData) Runtime.GetNSObject (UIImagePNGRepresentation (Handle));
		}
		
		[DllImport (Constants.UIKitLibrary)]
		extern static /* NSData */ IntPtr UIImageJPEGRepresentation (/* UIImage */ IntPtr image, /* CGFloat */ nfloat compressionQuality);

		public NSData AsJPEG ()
		{
			using (var pool = new NSAutoreleasePool ())
				return (NSData) Runtime.GetNSObject (UIImageJPEGRepresentation (Handle, 1.0f));
		}

		public NSData AsJPEG (nfloat compressionQuality)
		{
			using (var pool = new NSAutoreleasePool ())
				return (NSData) Runtime.GetNSObject (UIImageJPEGRepresentation (Handle, compressionQuality));
		}

		public UIImage Scale (CGSize newSize, nfloat scaleFactor)
		{
			UIGraphics.BeginImageContextWithOptions (newSize, false, scaleFactor);

			Draw (new CGRect (0, 0, newSize.Width, newSize.Height));

			var scaledImage = UIGraphics.GetImageFromCurrentImageContext();
			UIGraphics.EndImageContext();

			return scaledImage;
		}

		public UIImage Scale (CGSize newSize)
		{
			UIGraphics.BeginImageContext (newSize);

			Draw (new CGRect (0, 0, newSize.Width, newSize.Height));
	
			var scaledImage = UIGraphics.GetImageFromCurrentImageContext();
			UIGraphics.EndImageContext();

			return scaledImage;			
		}

#if !XAMCORE_2_0
		[Obsolete ("This is identical to FromFile. Caching is done when using FromBundle")]
		public static UIImage FromFileUncached (string filename)
		{
			return FromFile (filename);
		}

		[Obsolete ("This always returns null. Use the overload that accept a System.String 'name' instead.")]
		public static UIImage CreateAnimatedImage (UIImage [] images, UIEdgeInsets capInsets, double duration)
		{
			return null;
		}
#endif

		// required because of GetCallingAssembly (if we ever inline across assemblies)
		[MethodImpl (MethodImplOptions.NoInlining)]
		public static UIImage FromResource (Assembly assembly, string name)
		{
			if (name == null)
				throw new ArgumentNullException ("name");
			if (assembly == null)
				assembly = Assembly.GetCallingAssembly ();
			var stream = assembly.GetManifestResourceStream (name);
			if (stream == null)
				throw new ArgumentException (String.Format ("No resource named `{0}' found", name));

			byte [] buffer = new byte [stream.Length];
			stream.Read (buffer, 0, buffer.Length);
			unsafe {
				fixed (byte *p = &buffer [0]){
					var data = NSData.FromBytes ((IntPtr) p, (uint) stream.Length);
					return LoadFromData (data);
				}
			}
		}

// that was used (03be3e0d43085dfef2e732494216d9b2bf8fc079) to implement FromResource but that code 
// was changed later (d485b61793b0d986f416c8d6154fb92c7a57d79d) making it unused AFAICS
#if false
		internal class DataWrapper : NSData {
			IntPtr buffer;
			uint len;
			
			public DataWrapper (IntPtr buffer, uint len)
			{
				this.buffer = buffer;
				this.len = len;
			}

			public override nuint Length { get { return len; } }
			public override IntPtr Bytes { get { return buffer; } }
		}
#endif
	}
	
#if IOS
	[Register ("__MonoTouch_UIImageStatusDispatcher")]
	internal class UIImageStatusDispatcher : NSObject {
		public const string callbackSelector = "Xamarin_Internal__image:didFinishSavingWithError:contextInfo:";
		UIImage.SaveStatus status;
		
		public UIImageStatusDispatcher (UIImage.SaveStatus status)
		{
			IsDirectBinding = false;
			this.status = status;
			DangerousRetain ();
		}

		[Export (callbackSelector)]
		[Preserve (Conditional = true)]
		public void Callback (UIImage image, NSError err, IntPtr ctx)
		{
			status (image, err);
			DangerousRelease ();
		}
	}
#endif
}
