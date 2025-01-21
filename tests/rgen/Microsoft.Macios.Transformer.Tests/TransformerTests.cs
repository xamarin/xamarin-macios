// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Transformer.Workers;
using Xamarin.Tests;
using Xamarin.Utils;

namespace Microsoft.Macios.Transformer.Tests;

public class TransformerTests : BaseTransformerTestClass, IDisposable {
	string targetDirectory;

	public TransformerTests ()
	{
		targetDirectory = Path.Combine (Path.GetTempPath (), Path.GetRandomFileName ());
		Directory.CreateDirectory (targetDirectory);
	}


	class TestDataSkipTests : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			const string sampleCode = @"
using System;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace Test;

[NoTV]
[MacCatalyst (13, 1)]
[DisableDefaultCtor]
[Abstract]
[BaseType (typeof (NSObject))]
interface UIFeedbackGenerator : UIInteraction {

	[iOS (17, 5), MacCatalyst (17, 5)]
	[Static]
	[Export (""feedbackGeneratorForView:"")]
	UIFeedbackGenerator GetFeedbackGenerator (UIView forView);

	[Export (""prepare"")]
	void Prepare ();
}
";
			// correct path, not namespaces, do not skip and destination
			yield return [(sampleCode, "/some/random/path.cs"), null!, false, "Test"];

			// correct path, namespaces, do not skip and destination
			yield return [(sampleCode, "/some/random/path.cs"), new [] { "Test" }, false, "Test"];

			// correct path, namespaces, do not skip and destination
			yield return [(sampleCode, "/some/random/path.cs"), new [] { "UIKit" }, true, null!];
		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	[Theory]
	[AllSupportedPlatformsClassData<TestDataSkipTests>]
	public void SkipTests (ApplePlatform platform, (string Source, string Path) source, string []? targetNamespaces,
		bool expectedResult, string? expectedDestination)
	{
		// create a compilation used to create the transformer
		var compilation = CreateCompilation (platform, sources: source);
		var syntaxTree = compilation.SyntaxTrees.FirstOrDefault ();
		Assert.NotNull (syntaxTree);

		var semanticModel = compilation.GetSemanticModel (syntaxTree);
		Assert.NotNull (semanticModel);

		var declaration = syntaxTree.GetRoot ()
			.DescendantNodes ().OfType<BaseTypeDeclarationSyntax> ()
			.FirstOrDefault ();
		Assert.NotNull (declaration);

		var symbol = semanticModel.GetDeclaredSymbol (declaration);
		Assert.NotNull (symbol);

		var transformer = new Transformer (targetDirectory, [(platform, compilation)], targetNamespaces);
		if (expectedDestination is not null)
			expectedDestination = Path.Combine (targetDirectory, expectedDestination);
		Assert.Equal (expectedResult, transformer.Skip (syntaxTree, symbol, out var destination));
		Assert.Equal (expectedDestination, destination);
	}

	class TestDataSelectTopicTests : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			const string path = "/path/to/source.cs";
			const string errorDomain = @"
using System;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace Test;

[NoTV, NoMacCatalyst, iOS (18, 2)]
[ErrorDomain (""UIApplicationCategoryDefaultErrorDomain"")]
[Native]
public enum UIApplicationCategoryDefaultErrorCode : long {
	RateLimited = 1,
}
";

			yield return [(errorDomain, path), nameof (ErrorDomainTransformer)];

			const string smartEnum = @"
using System;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace Test;

public enum UIAccessibilityTraits : long {
	[Field (""UIAccessibilityTraitNone"")]
	None,

	[Field (""UIAccessibilityTraitButton"")]
	Button,

	[Field (""UIAccessibilityTraitLink"")]
	Link,

	[Field (""UIAccessibilityTraitHeader"")]
	Header,

	[Field (""UIAccessibilityTraitSearchField"")]
	SearchField,
}
";

			yield return [(smartEnum, path), nameof (SmartEnumTransformer)];

			const string normalEnum = @"
using System;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace Test;

public enum UIAccessibilityTraits : long {
	None,
	Button,
	Link,
	Header,
	SearchField,
}
";

			yield return [(normalEnum, path), nameof (CopyTransformer)];

			const string category = @"
using System;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace Test;

[TV (18, 0), iOS (18, 0), MacCatalyst (18, 0)]
[Category]
[BaseType (typeof (NSObject))]
interface NSObject_UIAccessibilityHitTest {
	[Export (""accessibilityHitTest:withEvent:"")]
	[return: NullAllowed]
	NSObject AccessibilityHitTest (CGPoint point, [NullAllowed] UIEvent withEvent);
}
";

			yield return [(category, path), nameof (CategoryTransformer)];

			const string baseType = @"
using System;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace Test;

[TV (18, 0), iOS (18, 0), MacCatalyst (18, 0)]
[BaseType (typeof (NSObject))]
interface UIZoomTransitionInteractionContext {
	[Export (""location"")]
	CGPoint Location { get; }

	[Export (""velocity"")]
	CGVector Velocity { get; }

	[Export (""willBegin"")]
	bool WillBegin { get; }
}
";

			yield return [(baseType, path), nameof (ClassTransformer)];

			const string protocol = @"
using System;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace Test;

[NoTV, NoMacCatalyst, iOS (18, 0)]
[Protocol (BackwardsCompatibleCodeGeneration = false), Model]
[BaseType (typeof (NSObject))]
interface UITextFormattingViewControllerDelegate {
	[Abstract]
	[Export (""textFormattingViewController:didChangeValue:"")]
	void DidChangeValue (UITextFormattingViewController viewController, UITextFormattingViewControllerChangeValue changeValue);

	[Export (""textFormattingViewController:shouldPresentFontPicker:"")]
	bool ShouldPresentFontPicker (UITextFormattingViewController viewController, UIFontPickerViewController fontPicker);

	[Export (""textFormattingViewController:shouldPresentColorPicker:"")]
	bool ShouldPresentColorPicker (UITextFormattingViewController viewController, UIColorPickerViewController colorPicker);

	[Export (""textFormattingDidFinish:"")]
	void TextFormattingDidFinish (UITextFormattingViewController viewController);
}
";

			yield return [(protocol, path), nameof (ProtocolTransformer)];
		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	[Theory]
	[AllSupportedPlatformsClassData<TestDataSelectTopicTests>]
	public void SelectTopicTests (ApplePlatform platform, (string Source, string Path) source, string expectedTopic)
	{
		// create a compilation used to create the transformer
		var compilation = CreateCompilation (platform, sources: source);
		var syntaxTree = compilation.SyntaxTrees.FirstOrDefault ();
		Assert.NotNull (syntaxTree);

		var semanticModel = compilation.GetSemanticModel (syntaxTree);
		Assert.NotNull (semanticModel);

		var declaration = syntaxTree.GetRoot ()
			.DescendantNodes ().OfType<BaseTypeDeclarationSyntax> ()
			.FirstOrDefault ();
		Assert.NotNull (declaration);

		var symbol = semanticModel.GetDeclaredSymbol (declaration);
		Assert.NotNull (symbol);

		// there is not need for a transformer, we are just testing the topic selection
		Assert.Equal (expectedTopic, Transformer.SelectTopic (symbol));
	}

	public void Dispose ()
	{
		// remove the temporary directory
		Directory.Delete (targetDirectory, recursive: true);
	}
}
