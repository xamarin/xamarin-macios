using System;
using System.Threading.Tasks;
using NUnit.Framework;

using AppKit;

namespace Xamarin.Mac.Tests
{
	[TestFixture]
	public class NSColorTests
	{
		[Test]
		public void NSColor_ComponentTests ()
		{
			NSColor c = NSColor.Blue;
			nfloat [] components;
			c.GetComponents (out components);
			Assert.IsTrue (0f == components[0], "Red");
			Assert.IsTrue (0f == components[1], "Green");
			Assert.IsTrue (1f == components[2], "Blue");
		}

		[Test]
		public void SingleComponents ()
		{
			var c = NSColor.Red;
			nfloat[] components;
			c.GetComponents (out components);
			Assert.AreEqual (c.RedComponent, components [0], "Red");
			Assert.AreEqual (c.GreenComponent, components [1], "Green");
			Assert.AreEqual (c.BlueComponent, components [2], "Blue");
		}
	}
}

