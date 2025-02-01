// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Macios.Transformer.Attributes;
using Xamarin.Tests;
using Xamarin.Utils;

namespace Microsoft.Macios.Transformer.Tests.Attributes;

public class NotificationDataTests : AttributeParsingTestClass {

	class TestDataTryCreate : IEnumerable<object []> {
		public IEnumerator<object []> GetEnumerator ()
		{
			const string path = "/some/random/path.cs";

			// simple notification
			const string simpleNotification = @"
using System;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace Test;

[NoMacCatalyst]
[BaseType (typeof (NSView))]
partial interface NSSplitView {
		[Notification]
		[Field (""NSSplitViewWillResizeSubviewsNotification"")]
		NSString NSSplitViewWillResizeSubviewsNotification { get; }
}
";
			yield return [(Source: simpleNotification, Path: path), new NotificationData (null, null)];

			// notification type
			const string notificationWithType = @"
using System;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace Test;

class MyNotification {}

[NoMacCatalyst]
[BaseType (typeof (NSView))]
partial interface NSSplitView {
		[Notification (typeof (MyNotification))]
		[Field (""NSSplitViewWillResizeSubviewsNotification"")]
		NSString NSSplitViewWillResizeSubviewsNotification { get; }
}
";

			yield return [(Source: notificationWithType, Path: path), new NotificationData ("Test.MyNotification", null)];

			// notification center
			const string notificationWithCenter = @"
using System;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace Test;

class MyNotification {}

[NoMacCatalyst]
[BaseType (typeof (NSView))]
partial interface NSSplitView {
		[Notification (""SharedWorkspace.NotificationCenter"")]
		[Field (""NSSplitViewWillResizeSubviewsNotification"")]
		NSString NSSplitViewWillResizeSubviewsNotification { get; }
}
";
			yield return [(Source: notificationWithCenter, Path: path), new NotificationData (null, "SharedWorkspace.NotificationCenter")];

			// both
			const string notificationWithBoth = @"
using System;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace Test;

class MyNotification {}

[NoMacCatalyst]
[BaseType (typeof (NSView))]
partial interface NSSplitView {
		[Notification (typeof (MyNotification), ""SharedWorkspace.NotificationCenter"")]
		[Field (""NSSplitViewWillResizeSubviewsNotification"")]
		NSString NSSplitViewWillResizeSubviewsNotification { get; }
}
";
			yield return [(Source: notificationWithBoth, Path: path), new NotificationData ("Test.MyNotification", "SharedWorkspace.NotificationCenter")];
		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
	}

	[Theory]
	[AllSupportedPlatformsClassData<TestDataTryCreate>]
	void TryCreateTests (ApplePlatform platform, (string Source, string Path) source,
		NotificationData expectedData)
		=> AssertTryCreate<NotificationData, PropertyDeclarationSyntax> (platform, source, AttributesNames.NotificationAttribute,
			expectedData, NotificationData.TryParse, lastOrDefault: true);
}
