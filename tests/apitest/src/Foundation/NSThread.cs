using System;
using System.Threading.Tasks;
using NUnit.Framework;

using AppKit;
using ObjCRuntime;
using Foundation;
using CoreGraphics;

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
