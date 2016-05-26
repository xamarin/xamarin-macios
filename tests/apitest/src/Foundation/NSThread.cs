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
	public class NSThreadTests
	{
		[Test]
		public void NSThread_CallStack_Test()
		{
			string [] stack = NSThread.NativeCallStack;
			Assert.IsNotNull (stack);
			Assert.IsTrue (stack.Length > 0);
		}
	}
}
