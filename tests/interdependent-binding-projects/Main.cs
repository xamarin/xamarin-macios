using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using NUnit.Framework;

#nullable enable

public static partial class TestLoader {
	static partial void AddTestAssembliesImpl (HashSet<Assembly> assemblies)
	{
		assemblies.Add (typeof (Xamarin.BindingTests2.BindingTest).Assembly);
		assemblies.Add (typeof (Xamarin.BindingTests.ProtocolTest).Assembly);
	}
}

[TestFixture]
public class LoaderTest {
	public void TestAssemblyCount ()
	{
		Assert.AreEqual (3, TestLoader.GetTestAssemblies ().Count (), "Test assembly count");
	}
}
