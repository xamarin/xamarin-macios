// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Transformer.Attributes;
using Xamarin.Tests;
using Xamarin.Utils;

namespace Microsoft.Macios.Transformer.Tests.Attributes;

public class BaseTypeDataTests : AttributeParsingTestClass {

	class TestDataTryCreate : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			var path = "/some/random/path.cs";

			const string simpleBaseType = @"
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

	[Export (""prepare"")]
	void Prepare ();
}
";
			yield return [(Source: simpleBaseType, Path: path), new BaseTypeData ("Foundation.NSObject")];

			const string baseTypeWithName = @"
using System;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace Test;

[NoTV]
[MacCatalyst (13, 1)]
[DisableDefaultCtor]
[Abstract]
[BaseType (typeof (NSObject), Name =""MyObjcName"")]
interface UIFeedbackGenerator : UIInteraction {

	[Export (""prepare"")]
	void Prepare ();
}
";
			yield return [
				(Source: baseTypeWithName, Path: path),
				new BaseTypeData ("Foundation.NSObject") {
					Name = "MyObjcName",
				}];

			const string baseTypeWithEvents = @"
using System;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace Test;

[NoiOS]
[NoMacCatalyst]
[BaseType (typeof (NSObject))]
[Model]
[Protocol]
interface NSAnimationDelegate {}

[BaseType (typeof (NSObject), Delegates = new string [] { ""WeakDelegate"" }, Events = new Type [] { typeof (NSAnimationDelegate) })]
interface NSAnimation : NSCoding, NSCopying {
	[Export (""startAnimation"")]
	void StartAnimation ();
}
";
			yield return [
				(Source: baseTypeWithEvents, Path: path),
				new BaseTypeData ("Foundation.NSObject") {
					Delegates = ["WeakDelegate"],
					Events = ["Test.NSAnimationDelegate"]
				}
			];

			const string singleton = @"
using System;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace Test;

[NoTV]
[MacCatalyst (13, 1)]
[DisableDefaultCtor]
[Abstract]
[BaseType (typeof (NSObject), Singleton = true)]
interface UIFeedbackGenerator : UIInteraction {

	[Export (""prepare"")]
	void Prepare ();
}
";
			yield return [
				(Source: singleton, Path: path),
				new BaseTypeData ("Foundation.NSObject") {
					Singleton = true
				}];

			const string keepRefUntil = @"
using System;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace Test;

[NoTV]
[BaseType (typeof (NSObject), KeepRefUntil = ""Dismissed"")] 
[Deprecated (PlatformName.iOS, 8, 3, message: ""Use 'UIAlertController' with 'UIAlertControllerStyle.ActionSheet' instead."")]
[MacCatalyst (13, 1)]
[Deprecated (PlatformName.MacCatalyst, 13, 1, message: ""Use 'UIAlertController' with 'UIAlertControllerStyle.ActionSheet' instead."")]
interface UIActionSheet {
}
";
			yield return [
				(Source: keepRefUntil, Path: path),
				new BaseTypeData ("Foundation.NSObject") {
					KeepRefUntil = "Dismissed",
				}];

			const string isStubClass = @"
using System;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace Test;

[NoTV]
[MacCatalyst (13, 1)]
[DisableDefaultCtor]
[Abstract]
[BaseType (typeof (NSObject), IsStubClass = true)]
interface UIFeedbackGenerator : UIInteraction {

	[Export (""prepare"")]
	void Prepare ();
}
";
			yield return [
				(Source: isStubClass, Path: path),
				new BaseTypeData ("Foundation.NSObject") {
					IsStubClass = true
				}];
		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	[Theory]
	[AllSupportedPlatformsClassData<TestDataTryCreate>]
	void TryCreateTests (ApplePlatform platform, (string Source, string Path) source, BaseTypeData expectedData)
		=> AssertTryCreate<BaseTypeData, BaseTypeDeclarationSyntax> (platform, source, AttributesNames.BaseTypeAttribute,
			expectedData, BaseTypeData.TryParse, lastOrDefault: true);
}
