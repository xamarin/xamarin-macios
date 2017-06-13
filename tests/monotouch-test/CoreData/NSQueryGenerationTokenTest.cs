using System;
#if XAMCORE_2_0
using CoreData;
using Foundation;
#else
using MonoTouch.CoreData;
using MonoTouch.Foundation;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.CoreData
{
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NSQueryGenerationTokenTest
	{
		[Test]
		public void EncodeWithCoderTest ()
		{
			using (var data = new NSMutableData ())
			using (var archiver = new NSKeyedArchiver (data))
			using (var coder = new NSCoder ()){
				NSQueryGenerationToken.CurrentToken.EncodeTo (archiver);
			}
		}
	}
}
