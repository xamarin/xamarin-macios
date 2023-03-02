using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

public static partial class TestLoader {
	static partial void AddTestAssembliesImpl (HashSet<Assembly> assemblies)
	{
		assemblies.Add (typeof (BundledResources.ResourcesTest).Assembly);
	}
}

[TestFixture]
public class LoaderTest {
	public void TestAssemblyCount ()
	{
		Assert.AreEqual (2, TestLoader.GetTestAssemblies ().Count (), "Test assembly count");
	}
}
