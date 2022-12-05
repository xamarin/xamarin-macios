using System;
using System.Collections.Generic;
using System.Reflection;

using Foundation;
#if !__MACOS__
using UIKit;
#endif

using MonoTouch.NUnit.UI;
using NUnit.Framework;
using NUnit.Framework.Internal;

#if __MACOS__
namespace Xamarin.Mac.Tests {
	public static partial class TestLoader {
		static partial void AddTestAssembliesImpl (List<Assembly> assemblies)
		{
			assemblies.Add (typeof (Xamarin.BindingTests2.BindingTest).Assembly);
			assemblies.Add (typeof (Xamarin.BindingTests.ProtocolTest).Assembly);
		}
	}
}
#elif !__WATCHOS__
public partial class AppDelegate
{
	public partial IEnumerable<Assembly> GetTestAssemblies ()
	{
		return new Assembly [] {
			Assembly.GetExecutingAssembly (),
			typeof (Xamarin.BindingTests2.BindingTest).Assembly,
			typeof (Xamarin.BindingTests.ProtocolTest).Assembly,
		};
	}
}
#else
public static partial class TestLoader {
	static partial void AddTestAssembliesImpl (BaseTouchRunner runner)
	{
		runner.Add (typeof (Xamarin.BindingTests2.BindingTest).Assembly);
		runner.Add (typeof (Xamarin.BindingTests.ProtocolTest).Assembly);
	}
}

#endif // !__WATCHOS__

// In some cases NUnit fails if asked to run tests from an assembly that doesn't have any tests. So add a dummy test here to not fail in that scenario.
[TestFixture]
public class DummyTest
{
	public void TestMe ()
	{
		Assert.True (true, "YAY!");
	}
}
