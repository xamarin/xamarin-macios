// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#if __MACOS__ && !XAMCORE_5_0

using AppKit;

namespace Xamarin.Mac.Tests {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NSAccessibilityElementLoadingTest {
		[Preserve (AllMembers = true)]
		class LegacyElementLoader : NSObject, INSAccessibilityElementLoading {
			public NSAccessibilityElement Element { get; set; }

			public NSAccessibilityElement GetAccessibilityElement (INSSecureCoding token)
			{
				return Element;
			}
		}

		[Preserve (AllMembers = true)]
		class ElementLoader : NSObject, INSAccessibilityElementLoading {
			public INSAccessibilityElementProtocol Element { get; set; }

			public INSAccessibilityElementProtocol GetAccessibilityElement2 (INSSecureCoding token)
			{
				return Element;
			}
		}

		class CompatibilityAccessibilityElement : NSAccessibilityElement, INSAccessibilityElementProtocol {
		}

		[Test]
		public void LegacyImplementation ()
		{
			using var element = new CompatibilityAccessibilityElement ();
			using var loader = new LegacyElementLoader { Element = element };
			using var token = new NSString ("token");
			INSAccessibilityElementLoading protocol = loader;

			Assert.That (loader.RespondsToSelector (new Selector ("accessibilityElementWithToken:")), Is.True, "NativeCallback");
			var result = Messaging.IntPtr_objc_msgSend_IntPtr (loader.Handle, Selector.GetHandle ("accessibilityElementWithToken:"), token.Handle);
			Assert.That (result, Is.EqualTo ((IntPtr) element.Handle), "NativeResult");
			GC.KeepAlive (loader);
			GC.KeepAlive (token);
#pragma warning disable CS0618
			Assert.That (protocol.GetAccessibilityElement (token), Is.SameAs (element), "LegacyMethod");
#pragma warning restore CS0618
		}

		[Test]
		public void CorrectedImplementation ()
		{
			using var element = new CompatibilityAccessibilityElement ();
			using var view = new NSView ();
			using var loader = new ElementLoader { Element = element };
			using var token = new NSString ("token");
			INSAccessibilityElementLoading protocol = loader;

			Assert.That (protocol.GetAccessibilityElement2 (token), Is.SameAs (element), "CorrectedMethod");
#pragma warning disable CS0618
			Assert.That (protocol.GetAccessibilityElement (token), Is.SameAs (element), "LegacyMethod");

			loader.Element = view;
			Assert.That (protocol.GetAccessibilityElement2 (token), Is.SameAs (view), "View");
			Assert.Throws<InvalidCastException> (() => protocol.GetAccessibilityElement (token), "IncompatibleElement");

			loader.Element = null;
			Assert.That (protocol.GetAccessibilityElement2 (token), Is.Null, "Null");
			Assert.That (protocol.GetAccessibilityElement (token), Is.Null, "LegacyNull");
#pragma warning restore CS0618
		}
	}
}
#endif
