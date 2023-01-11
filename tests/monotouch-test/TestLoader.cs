using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using NUnit.Framework;

using Foundation;

#nullable enable

public static partial class TestLoader {
	static partial void AddTestAssembliesImpl (HashSet<Assembly> assemblies)
	{
		assemblies.Add (typeof (EmbeddedResources.ResourcesTest).Assembly);
		assemblies.Add (typeof (Xamarin.BindingTests.ProtocolTest).Assembly);
	}
}

[TestFixture]
[Preserve (AllMembers = true)]
public class LoaderTest {
	public void TestAssemblyCount ()
	{
		Assert.AreEqual (3, TestLoader.GetTestAssemblies ().Count (), "Test assembly count");
	}
}
