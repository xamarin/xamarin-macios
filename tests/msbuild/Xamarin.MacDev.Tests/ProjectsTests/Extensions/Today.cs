using System;
using NUnit.Framework;

namespace Xamarin.MacDev.Tasks {
	[TestFixture ("iPhone")]
	[TestFixture ("iPhoneSimulator")]
	public class TodayTests : ExtensionTestBase {
		public TodayTests (string platform) : base (platform)
		{
		}

		[Test]
		public void BasicTest ()
		{
			this.BuildExtension ("MyOpenGLApp", "MyTodayExtension");
		}
	}
}
