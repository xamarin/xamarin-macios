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
			assemblies.Add (typeof (EmbeddedResources.ResourcesTest).Assembly);
			assemblies.Add (typeof (Xamarin.BindingTests.ProtocolTest).Assembly);
		}
	}
}
#elif !__WATCHOS__
public partial class AppDelegate {
	public partial IEnumerable<Assembly> GetTestAssemblies ()
	{
		return new Assembly [] {
			Assembly.GetExecutingAssembly (),
			typeof (EmbeddedResources.ResourcesTest).Assembly,
			typeof (Xamarin.BindingTests.ProtocolTest).Assembly,
		};
	}
}
#else
public static partial class TestLoader {
	static partial void AddTestAssembliesImpl (BaseTouchRunner runner)
	{
		runner.Add (typeof (EmbeddedResources.ResourcesTest).Assembly);
		runner.Add (typeof (Xamarin.BindingTests.ProtocolTest).Assembly);
	}
}

#endif // !__WATCHOS__
