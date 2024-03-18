using System;
using System.Collections.Generic;
using System.Reflection;

#nullable enable

public static partial class TestLoader {
	public static IEnumerable<Assembly> GetTestAssemblies ()
	{
		var assemblies = new HashSet<Assembly> ();
		assemblies.Add (Assembly.GetExecutingAssembly ());
		AddTestAssembliesImpl (assemblies);
		return assemblies;
	}

	static partial void AddTestAssembliesImpl (HashSet<Assembly> assemblies);
}
