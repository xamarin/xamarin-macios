// Copyright 2011-2012 Xamarin Inc. All rights reserved

#if !MONOMAC
using System;
using System.IO;
#if !__WATCHOS__
using System.Drawing;
#endif
using Foundation;
using UIKit;
using ObjCRuntime;
using NUnit.Framework;

namespace MonoTouchFixtures.UIKit {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class ColorTest {

		[Test]
		public void ToString_ ()
		{
			Assert.That ("UIColor [A=255, R=0, G=0, B=0]",
				Is.EqualTo (UIColor.Black.ToString ()), "Black");
			Assert.That ("UIColor [A=255, R=0, G=0, B=255]",
				Is.EqualTo (UIColor.Blue.ToString ()), "Blue");
			Assert.That ("UIColor [A=255, R=153, G=102, B=51]",
				Is.EqualTo (UIColor.Brown.ToString ()), "Brown");
			Assert.That ("UIColor [A=0, R=0, G=0, B=0]",
				Is.EqualTo (UIColor.Clear.ToString ()), "Clear");
			Assert.That ("UIColor [A=255, R=0, G=255, B=255]",
				Is.EqualTo (UIColor.Cyan.ToString ()), "Cyan");
			Assert.That ("UIColor [A=255, R=85, G=85, B=85]",
				Is.EqualTo (UIColor.DarkGray.ToString ()), "DarkGray");
			Assert.That ("UIColor [A=255, R=127, G=127, B=127]",
				Is.EqualTo (UIColor.Gray.ToString ()), "Gray");
			Assert.That ("UIColor [A=255, R=0, G=255, B=0]",
				Is.EqualTo (UIColor.Green.ToString ()), "Green");
			Assert.That ("UIColor [A=255, R=170, G=170, B=170]",
				Is.EqualTo (UIColor.LightGray.ToString ()), "LightGray");
			Assert.That ("UIColor [A=255, R=255, G=0, B=255]",
				Is.EqualTo (UIColor.Magenta.ToString ()), "Magenta");
			Assert.That ("UIColor [A=255, R=255, G=127, B=0]",
				Is.EqualTo (UIColor.Orange.ToString ()), "Orange");
			Assert.That ("UIColor [A=255, R=127, G=0, B=127]",
				Is.EqualTo (UIColor.Purple.ToString ()), "Purple");
			Assert.That ("UIColor [A=255, R=255, G=0, B=0]",
				Is.EqualTo (UIColor.Red.ToString ()), "Red");
			Assert.That ("UIColor [A=255, R=255, G=255, B=255]",
				Is.EqualTo (UIColor.White.ToString ()), "White");
			Assert.That ("UIColor [A=255, R=255, G=255, B=0]",
				Is.EqualTo (UIColor.Yellow.ToString ()), "Yellow");
		}

		void RoundtripHSBA (UIColor c, bool supported = true)
		{
			try {
				nfloat h, s, b, a;
				/*bool result =*/
				c.GetHSBA (out h, out s, out b, out a);
				UIColor r = UIColor.FromHSBA (h, s, b, a);
#if true
				Assert.That (r.ToString (), Is.EqualTo (c.ToString ()), c.ToString ());
#else
				if (result) {
					string cs = c.ToString ();
					float h2, s2, b2, a2;
					c.GetHSBA2 (out h2, out s2, out b2, out a2);
					Assert.That (h, Is.EqualTo (h2), cs);
					Assert.That (s, Is.EqualTo (s2), cs);
					Assert.That (b, Is.EqualTo (b2), cs);
					Assert.That (a, Is.EqualTo (a2), cs);
				}
#endif
			} catch (Exception) {
				if (supported)
					Assert.Fail (c.ToString ());
			}
		}

		[Test]
		[DefaultFloatingPointTolerance (0.00001)]
		public void HSBA ()
		{
			RoundtripHSBA (UIColor.Black);
			RoundtripHSBA (UIColor.Blue);
			RoundtripHSBA (UIColor.Brown);
			RoundtripHSBA (UIColor.Clear);
			RoundtripHSBA (UIColor.Cyan);
			RoundtripHSBA (UIColor.DarkGray);
			RoundtripHSBA (UIColor.Gray);
			RoundtripHSBA (UIColor.Green);
			RoundtripHSBA (UIColor.LightGray);
			RoundtripHSBA (UIColor.Magenta);
			RoundtripHSBA (UIColor.Orange);
			RoundtripHSBA (UIColor.Purple);
			RoundtripHSBA (UIColor.Red);
			RoundtripHSBA (UIColor.White);
			RoundtripHSBA (UIColor.Yellow);
#if !__TVOS__ && !__WATCHOS__
			RoundtripHSBA (UIColor.DarkText);
			RoundtripHSBA (UIColor.GroupTableViewBackground, false);           // unsupported color space
			RoundtripHSBA (UIColor.LightText);
			RoundtripHSBA (UIColor.ScrollViewTexturedBackground, false);       // unsupported color space
			RoundtripHSBA (UIColor.UnderPageBackground, false);                        // unsupported color space
			RoundtripHSBA (UIColor.ViewFlipsideBackground, false);                     // unsupported color space
#endif
#if false
			for (int r = 0; r < 256; r++) {
				for (int g = 0; g < 256; g++) {
					for (int b = 0; b < 256; b++) {
						RoundtripHSBA (UIColor.FromRGBA (r, g, b, b));
					}
					Console.WriteLine ("\tg {0}", g);
				}
				Console.WriteLine ("\t\tr {0}", r);
			}
#endif
		}

		[Test]
		public void HSBA_No_Saturation ()
		{
			nfloat h = 0.0f;
			nfloat s = 0.0f;
			nfloat b = 0.0f;
			nfloat a = 0.0f;
			UIColor c = UIColor.FromHSBA (h, s, b, a);
			c.GetHSBA (out h, out s, out b, out a);
			Assert.That (h, Is.EqualTo ((nfloat) 0f), "h");
			Assert.That (s, Is.EqualTo ((nfloat) 0f), "s");
			Assert.That (b, Is.EqualTo ((nfloat) 0f), "b");
			Assert.That (a, Is.EqualTo ((nfloat) 0f), "a");
		}

		// note: MonoTouch addition - not fully compatible with "getHue:saturation:brightness:alpha:" wrt alpha
		void RoundtripHSB (UIColor c)
		{
			nfloat h, s, b, a;
			c.GetHSBA (out h, out s, out b, out a);
			UIColor r = UIColor.FromHSB (h, s, b);
			Assert.That (r.ToString (), Is.EqualTo (c.ToString ()), c.ToString ());
		}

		[Test]
		public void HSB ()
		{
			RoundtripHSB (UIColor.Black);
			RoundtripHSB (UIColor.Blue);
			RoundtripHSB (UIColor.Brown);
			//RoundtripHSB (UIColor.Clear); 							// alpha is 0
			RoundtripHSB (UIColor.Cyan);
			RoundtripHSB (UIColor.DarkGray);
#if !__TVOS__ && !__WATCHOS__
			RoundtripHSB (UIColor.DarkText);
#endif
			RoundtripHSB (UIColor.Gray);
			RoundtripHSB (UIColor.Green);
			//RoundtripHSB (UIColor.GroupTableViewBackgroundColor);		// unsupported color space
			RoundtripHSB (UIColor.LightGray);
			//RoundtripHSB (UIColor.LightTextColor); 					// alpha is 153
			RoundtripHSB (UIColor.Magenta);
			RoundtripHSB (UIColor.Orange);
			RoundtripHSB (UIColor.Purple);
			RoundtripHSB (UIColor.Red);
			//RoundtripHSB (UIColor.ScrollViewTexturedBackgroundColor);	// unsupported color space
			//RoundtripHSB (UIColor.UnderPageBackgroundColor);			// unsupported color space
			//RoundtripHSB (UIColor.ViewFlipsideBackgroundColor);		// unsupported color space
			RoundtripHSB (UIColor.White);
			RoundtripHSB (UIColor.Yellow);
		}

		void RoundtripRGBA (UIColor c)
		{
			nfloat r, g, b, a;
			c.GetRGBA (out r, out g, out b, out a);
			UIColor k = UIColor.FromRGBA (r, g, b, a);
			Assert.That (k.ToString (), Is.EqualTo (c.ToString ()), c.ToString ());
		}

		[Test]
		public void RGBA ()
		{
			RoundtripRGBA (UIColor.Black);
			RoundtripRGBA (UIColor.Blue);
			RoundtripRGBA (UIColor.Brown);
			RoundtripRGBA (UIColor.Clear);
			RoundtripRGBA (UIColor.Cyan);
			RoundtripRGBA (UIColor.DarkGray);
#if !__TVOS__ && !__WATCHOS__
			RoundtripRGBA (UIColor.DarkText);
#endif
			RoundtripRGBA (UIColor.Gray);
			RoundtripRGBA (UIColor.Green);
			//RoundtripRGBA (UIColor.GroupTableViewBackgroundColor);
			RoundtripRGBA (UIColor.LightGray);
			//RoundtripRGBA (UIColor.LightTextColor);
			RoundtripRGBA (UIColor.Magenta);
			RoundtripRGBA (UIColor.Orange);
			RoundtripRGBA (UIColor.Purple);
			RoundtripRGBA (UIColor.Red);
			//RoundtripRGBA (UIColor.ScrollViewTexturedBackgroundColor);	// unsupported color space
			//RoundtripRGBA (UIColor.UnderPageBackgroundColor);				// unsupported color space
			//RoundtripRGBA (UIColor.ViewFlipsideBackgroundColor); 			// unsupported color space
			RoundtripRGBA (UIColor.White);
			RoundtripRGBA (UIColor.Yellow);
		}

		// note: MonoTouch addition - not fully compatible with "getRed:green:blue:alpha:" wrt alpha
		void RoundtripRGB (UIColor c)
		{
			nfloat r, g, b, a;
			c.GetRGBA (out r, out g, out b, out a);
			UIColor k = UIColor.FromRGB (r, g, b);
			Assert.That (k.ToString (), Is.EqualTo (c.ToString ()), c.ToString ());
		}

		[Test]
		public void RGB ()
		{
			RoundtripRGB (UIColor.Black);
			RoundtripRGB (UIColor.Blue);
			RoundtripRGB (UIColor.Brown);
			// RoundtripRGB (UIColor.Clear);							// alpha is 0
			RoundtripRGB (UIColor.Cyan);
			RoundtripRGB (UIColor.DarkGray);
#if !__TVOS__ && !__WATCHOS__
			RoundtripRGB (UIColor.DarkText);
#endif
			RoundtripRGB (UIColor.Gray);
			RoundtripRGB (UIColor.Green);
			//RoundtripRGB (UIColor.GroupTableViewBackgroundColor);		// unsupported color space
			RoundtripRGB (UIColor.LightGray);
			//RoundtripRGB (UIColor.LightTextColor);					// alpha is 153
			RoundtripRGB (UIColor.Magenta);
			RoundtripRGB (UIColor.Orange);
			RoundtripRGB (UIColor.Purple);
			RoundtripRGB (UIColor.Red);
			//RoundtripRGB (UIColor.ScrollViewTexturedBackgroundColor);	// unsupported color space
			//RoundtripRGB (UIColor.UnderPageBackgroundColor);			// unsupported color space
			//RoundtripRGB (UIColor.ViewFlipsideBackgroundColor);		// unsupported color space
			RoundtripRGB (UIColor.White);
			RoundtripRGB (UIColor.Yellow);
		}

		[Test]
		public void Pattern_7362 ()
		{
			using (var img = UIImage.FromFile (Path.Combine (NSBundle.MainBundle.ResourcePath, "basn3p08.png")))
			using (var color = UIColor.FromPatternImage (img)) {
				Assert.That (color.ToString (), Is.EqualTo (color.Description), "not an RGBA color");
			}
		}

		void RoundtripConstructorRGB (UIColor c)
		{
			nfloat r, g, b, a;
			c.GetRGBA (out r, out g, out b, out a);
			var k = new UIColor (r, g, b, a);
			Assert.That (k.ToString (), Is.EqualTo (c.ToString ()), c.ToString ());
		}

		[Test]
		public void RGBAConstructor ()
		{
			RoundtripConstructorRGB (UIColor.Black);
			RoundtripConstructorRGB (UIColor.Blue);
			RoundtripConstructorRGB (UIColor.Brown);
			RoundtripConstructorRGB (UIColor.Cyan);
			RoundtripConstructorRGB (UIColor.DarkGray);
#if !__TVOS__ && !__WATCHOS__
			RoundtripConstructorRGB (UIColor.DarkText);
#endif
			RoundtripConstructorRGB (UIColor.Gray);
			RoundtripConstructorRGB (UIColor.Green);
			RoundtripConstructorRGB (UIColor.LightGray);
			RoundtripConstructorRGB (UIColor.Magenta);
			RoundtripConstructorRGB (UIColor.Orange);
			RoundtripConstructorRGB (UIColor.Purple);
			RoundtripConstructorRGB (UIColor.Red);
			RoundtripConstructorRGB (UIColor.White);
			RoundtripConstructorRGB (UIColor.Yellow);
		}

		[TestCase (0.2, 0.4)]
		[TestCase (0.3, 0.5)]
		[TestCase (0.4, 0.6)]
		[TestCase (0.5, 0.7)]
		public void WAConstructor (double w, double a)
		{
			var nw = (nfloat) w;
			var na = (nfloat) a;
			var c = UIColor.FromWhiteAlpha (nw, na);
			var r = new UIColor (nw, na);
			Assert.That (r.ToString (), Is.EqualTo (c.ToString ()), c.ToString ());
		}

#if !__WATCHOS__
		[Test]
		public void UIConfigurationColorTransformerTest ()
		{
			TestRuntime.AssertXcodeVersion (12, TestRuntime.MinorXcode12APIMismatch);
			var redColor = UIColor.Red;
			var transformer = UIConfigurationColorTransformer.Grayscale;
			var grayColor = transformer (redColor);
			Assert.NotNull (grayColor, "Not null");
		}
#endif
	}
}
#endif
