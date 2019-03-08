using System;

namespace xharness
{
	// iOS here means Xamarin.iOS, not iOS as opposed to tvOS/watchOS.
	public class iOSTarget : Target
	{
		public iOSTestProject TestProject;

		public MonoNativeInfo MonoNativeInfo => TestProject.MonoNativeInfo;
	}
}
