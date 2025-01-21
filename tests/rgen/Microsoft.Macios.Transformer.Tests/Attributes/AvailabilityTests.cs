// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Generator.Attributes;
using Microsoft.Macios.Generator.Availability;
using Microsoft.Macios.Generator.Extensions;
using Xamarin.Tests;
using Xamarin.Utils;

namespace Microsoft.Macios.Transformer.Tests.Attributes;

public class AvailabilityTests : BaseTransformerTestClass {
	class TestDataTryCreate : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			const string path = "/some/random/path.cs";
			var builder = SymbolAvailability.CreateBuilder ();

			const string allPlatformsIncluded = @"
using System;
using ObjCRuntime;
using Foundation;

namespace Test;

[DisableDefaultCtor]
[Abstract] // abstract class that should not be used directly
[BaseType (typeof (NSObject))]
interface UIFeedbackGenerator : UIInteraction {

	[Export (""prepare"")]
	void Prepare ();
}
";
			builder.Add (new SupportedOSPlatformData ("ios"));
			builder.Add (new SupportedOSPlatformData ("tvos"));
			builder.Add (new SupportedOSPlatformData ("macos"));
			builder.Add (new SupportedOSPlatformData ("maccatalyst"));
			yield return [(Source: allPlatformsIncluded, Path: path), builder.ToImmutable ()];

			builder.Clear ();
			const string singlePlatformRemoved = @"
using System;
using ObjCRuntime;
using Foundation;

namespace Test;

[NoTV]
[DisableDefaultCtor]
[Abstract] // abstract class that should not be used directly
[BaseType (typeof (NSObject))]
interface UIFeedbackGenerator : UIInteraction {

	[Export (""prepare"")]
	void Prepare ();
}
";

			builder.Add (new SupportedOSPlatformData ("ios"));
			builder.Add (new UnsupportedOSPlatformData ("tvos"));
			builder.Add (new SupportedOSPlatformData ("macos"));
			builder.Add (new SupportedOSPlatformData ("maccatalyst"));
			yield return [(Source: singlePlatformRemoved, Path: path), builder.ToImmutable ()];


			builder.Clear ();
			const string onePlatformSpecificVersion = @"
using System;
using ObjCRuntime;
using Foundation;

namespace Test;

[MacCatalyst (13, 1)]
[DisableDefaultCtor]
[Abstract] // abstract class that should not be used directly
[BaseType (typeof (NSObject))]
interface UIFeedbackGenerator : UIInteraction {

	[Export (""prepare"")]
	void Prepare ();
}
";
			
			builder.Add (new SupportedOSPlatformData ("ios"));
			builder.Add (new SupportedOSPlatformData("tvos"));
			builder.Add (new SupportedOSPlatformData ("macos"));
			builder.Add (new SupportedOSPlatformData ("maccatalyst13.1"));
			yield return [(Source: onePlatformSpecificVersion, Path: path), builder.ToImmutable ()];

			builder.Clear ();
			const string allPlatformsRemovedButOne = @"
using System;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace Test;

[NoTV, NoMacCatalyst, NoiOS]
[BaseType (typeof (NSObject))]
interface UIFeedbackGenerator : UIInteraction {

	[Export (""prepare"")]
	void Prepare ();
}
";

			builder.Add (new UnsupportedOSPlatformData("ios"));
			builder.Add (new UnsupportedOSPlatformData("tvos"));
			builder.Add (new SupportedOSPlatformData ("macos"));
			builder.Add (new UnsupportedOSPlatformData("maccatalyst"));
			yield return [(Source: allPlatformsRemovedButOne, Path: path), builder.ToImmutable ()];
		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	[Theory]
	[AllSupportedPlatformsClassData<TestDataTryCreate>]
	void TryCreateTests (ApplePlatform platform, (string Source, string Path) source, SymbolAvailability expectedData)
	{
		var compilation = CreateCompilation (platform, sources: source);
		var x = compilation.GetDiagnostics ();
		var syntaxTree = compilation.SyntaxTrees.FirstOrDefault(t => t.FilePath == source.Path);
		Assert.NotNull (syntaxTree);
		var declaration = syntaxTree.GetRoot ()
			.DescendantNodes ().OfType<BaseTypeDeclarationSyntax> ()
			.FirstOrDefault ();
		Assert.NotNull (declaration);
		
		var semanticModel = compilation.GetSemanticModel (syntaxTree);
		Assert.NotNull (semanticModel);

		var symbol = semanticModel.GetDeclaredSymbol (declaration);
		Assert.NotNull (symbol);

		// the transformation does not care about the parents, we want the exact same as was added by 
		// the developer.
		var availability = symbol.GetAvailabilityForSymbol ();
		Assert.Equal (expectedData, availability);
	}
}
