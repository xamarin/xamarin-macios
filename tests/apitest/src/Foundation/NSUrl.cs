using System;
using System.Threading.Tasks;
using NUnit.Framework;

#if !XAMCORE_2_0
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;
using MonoMac.Foundation;
using nint = System.Int32;
#else
using AppKit;
using ObjCRuntime;
using Foundation;
using CoreGraphics;
#endif

namespace Xamarin.Mac.Tests
{
	[TestFixture]
	public class NSUrlTests
	{
		const string UrlBase = "https://www.xamarin.com/";
		const string MalformedUrlBase = "https://www.xamarin%$#!)(*&%(*&#$.com/";

		const string RelativeBit = "platform";
		const string MalformedRelativeBit = "platf%$#!)(*&%(*&#$.orm";


		[Test]
		public void NSUrl_SmokeTest ()
		{
			var a = new NSUrl (UrlBase);
			Assert.AreNotEqual (IntPtr.Zero, a.Handle);

			var b = new NSUrl (RelativeBit, a);
			Assert.AreNotEqual (IntPtr.Zero, b.Handle);

			var c = NSUrl.FromString (UrlBase);
			Assert.AreNotEqual (IntPtr.Zero, c.Handle);

			var d = NSUrl.FromStringRelative (RelativeBit, c);
			Assert.AreNotEqual (IntPtr.Zero, d.Handle);

			var e = a.MakeRelative (RelativeBit);
			Assert.AreNotEqual (IntPtr.Zero, e.Handle);
		}

		[Test]
		public void NSUrl_MalformedUrlsThrow ()
		{
			var b = new NSUrl (UrlBase);

			Assert.Throws<ArgumentException> (() => new NSUrl (MalformedUrlBase));
			Assert.Throws<ArgumentException> (() => new NSUrl (MalformedRelativeBit, b));
			Assert.Throws<ArgumentException> (() => NSUrl.FromString (MalformedUrlBase));
			Assert.Throws<ArgumentException> (() => NSUrl.FromStringRelative (MalformedRelativeBit, b));
			Assert.Throws<ArgumentException> (() => b.MakeRelative (MalformedRelativeBit));
		}
	}
}
