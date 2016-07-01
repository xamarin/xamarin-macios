using System;
using System.IO;

using NUnit.Framework;

namespace Xamarin.iOS.Tasks
{
	[TestFixture ("iPhone")]
	[TestFixture ("iPhoneSimulator")]
	public class IBToolLinking : ProjectTest
	{
		public IBToolLinking (string platform) : base (platform)
		{
		}

		[Test]
		public void BuildTest ()
		{
			BuildProject ("MyIBToolLinkTest", Platform);
		}
	}
}
