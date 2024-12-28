using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Macios.Generator.Extensions;
using Xamarin.Tests;
using Xamarin.Utils;
using Xunit;

namespace Microsoft.Macios.Generator.Tests.Extensions;

public class SyntaxTreeExtensionsTests : BaseGeneratorTestClass {
	readonly CollectionComparer<string> comparer = new (StringComparer.InvariantCulture);

	class TestDataCollectUsingStatementsTest : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			const string noUsingPresent = @"
namespace NS {
	public class TestClass {
	}
}
";
			yield return [noUsingPresent, Array.Empty<string>()];

			const string singleUsing = @"
using System;

namespace NS {
	public class TestClass {
	}
}
";
			yield return [singleUsing, new [] {"System" }];

			const string unsortedUsingStatements = @"
using System;
using AVFoundation;
using ObjCRuntime;
using System.Collections.Generic;

namespace NS {
	public class TestClass {
	}
}
";
			yield return [unsortedUsingStatements, new [] {"System", "AVFoundation", "ObjCRuntime", "System.Collections.Generic"}];
		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	[Theory]
	[AllSupportedPlatformsClassData<TestDataCollectUsingStatementsTest>]
	public void CollectUsingStatementsTest (ApplePlatform platform, string inputText, string [] expectedUsingStatements)
	{
		var (_, sourceTrees) = CreateCompilation (platform, sources: inputText);
		Assert.Single (sourceTrees);
		var tree = sourceTrees [0].GetRoot ().SyntaxTree;
		Assert.NotNull (tree);
		Assert.Equal (expectedUsingStatements, tree.CollectUsingStatements (), comparer);
	}
}
