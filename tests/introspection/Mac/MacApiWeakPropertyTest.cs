using System;

#if XAMCORE_2_0
using Foundation;
#else
using MonoMac.Foundation;
#endif

using NUnit.Framework;
using Xamarin.Tests;

namespace Introspection {

	[TestFixture]
	public class MacApiWeakPropertyTest : ApiWeakPropertyTest {
		protected override bool Skip (Type type)
		{
			switch (type.Name) {
			case "CATextLayer": // CATextLayer.WeakFont is done correctly by hand
				return true;
			case "NSAttributedStringDocumentAttributes": // NSAttributedStringDocumentAttributes.WeakDocumentType is done by hand, not a binding
				return true;
			}
			return base.Skip (type);
		}
	}
}
