//
// Unit tests for CGDataProvider
//
// Authors:
//	Sebastien Pouliot <sebastien@xamarin.com>
//
// Copyright 2014-2015 Xamarin Inc. All rights reserved.
//

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using NUnit.Framework;

namespace MonoTouchFixtures.CoreGraphics {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class DataProviderTest {

		[Test]
		public void FromPNG ()
		{
			string file = Path.Combine (NSBundle.MainBundle.ResourcePath, "basn3p08.png");
			using (var dp = CGDataProvider.FromFile (file))
			using (var img = CGImage.FromPNG (dp, null, false, CGColorRenderingIntent.Default)) {
				Assert.That ((int) img.Height, Is.EqualTo (32), "Height");
				Assert.That ((int) img.Width, Is.EqualTo (32), "Width");
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGDataProviderRef */ IntPtr CGDataProviderCreateWithFilename (/* const char* */ string filename);

		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern /* CGDataProviderRef */ IntPtr CGDataProviderCreateWithURL (/* CFURLRef __nullable */ IntPtr url);

		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern /* CGDataProviderRef */ IntPtr CGDataProviderCreateWithCFData (/* CFDataRef __nullable */ IntPtr data);

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static IntPtr CGDataProviderCreateWithData (/* void* */ IntPtr info, /* const void* */ IntPtr data, /* size_t */ IntPtr size, /* CGDataProviderReleaseDataCallback */ IntPtr releaseData);

		[Test]
		public void Create_Nullable ()
		{
			// the native API accept a nil argument but it returns nil, so we must keep our ArgumentNullException as
			// a .NET constructor can't return null (and we don't want invalid managed instances)
			Assert.That (CGDataProviderCreateWithFilename (null), Is.EqualTo (IntPtr.Zero), "CGDataProviderCreateWithFilename");
			Assert.That (CGDataProviderCreateWithURL (IntPtr.Zero), Is.EqualTo (IntPtr.Zero), "CGDataConsumerCreateWithURL");
			Assert.That (CGDataProviderCreateWithCFData (IntPtr.Zero), Is.EqualTo (IntPtr.Zero), "CGDataProviderCreateWithCFData");
			// crash with iOS 13 beta 3
			// Assert.That (CGDataProviderCreateWithData (IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero), Is.EqualTo (IntPtr.Zero), "CGDataProviderCreateWithData");
		}

		[Test]
		public void Create_ReleaseCallback ()
		{
			IntPtr memory = Marshal.AllocHGlobal (20);
			using (var provider = new CGDataProvider (memory, 20, ((IntPtr mem) => {
				Assert.AreEqual (memory, mem, "mem");
				Marshal.FreeHGlobal (mem);
				memory = IntPtr.Zero;
			}))) {
			}

			Assert.AreEqual (IntPtr.Zero, memory, "mem freed");
		}

		[Test]
		public void CreateWithUnownedMemory ()
		{
			const string canary = "canary";
			var source = Encoding.UTF8.GetBytes (canary);
			IntPtr memory = Marshal.AllocHGlobal (20);
			Marshal.Copy (source, 0, memory, source.Length);
			using (var provider = new CGDataProvider (memory, 20, false)) {
				// `memory` is copied, but not freed (but the copy is freed)
				Assert.That (provider.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle");
			}
			// so `memory` still accessible afterward and must be freed
			// using `canary.Length` since the allocated memory might not be zero'ed
			// so reading back the string would not hit a null after what we copied
			Assert.That (Marshal.PtrToStringAuto (memory, canary.Length), Is.EqualTo (canary), "canary check");
			Marshal.FreeHGlobal (memory);
		}
	}
}
