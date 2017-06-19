using System;
#if XAMCORE_2_0
using CoreData;
using Foundation;
#else
using MonoTouch.CoreData;
using MonoTouch.Foundation;
#endif
using NUnit.Framework;
using Xamarin.Tests;

namespace MonoTouchFixtures.CoreData
{
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NSQueryGenerationTokenTest
	{
		[Test]
		public void EncodeWithCoderTest ()
		{
			// Added test to ensure we do support NSCoding even when introspection fails.
			if (IntPtr == 8  && TestRuntime.CheckExactXcodeVersion (11, 0, beta: 1)) {
				using (var data = new NSMutableData ())
				using (var archiver = new NSKeyedArchiver (data))
				using (var coder = new NSCoder ()) {
					NSQueryGenerationToken.CurrentToken.EncodeTo (archiver);
				}
			}
		}
	}
}
