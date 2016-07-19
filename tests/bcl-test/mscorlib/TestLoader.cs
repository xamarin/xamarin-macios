using System;
using System.Collections.Generic;
using System.Reflection;

using MonoTouch.NUnit.UI;
using NUnit.Framework.Internal.Filters;

public static partial class TestLoader
{
	static partial void AddTestAssembliesImpl (BaseTouchRunner runner)
	{
		var assemblies = new HashSet<Assembly> ();
		// Test files are sorted by namespace, so since we have two assemblies,
		// pick something at the top (Mono) and at the bottom (System.Text) to
		// make sure we get both assemblies.
		assemblies.Add (typeof (MonoTests.Mono.DataConverterTest).Assembly);
		assemblies.Add (typeof (MonoTests.System.Text.UTF8EncodingTest).Assembly);
		if (assemblies.Count != 2)
			throw new Exception ("Should have found two test assemblies");
		foreach (var asm in assemblies)
			runner.Add (asm);
	}
}
