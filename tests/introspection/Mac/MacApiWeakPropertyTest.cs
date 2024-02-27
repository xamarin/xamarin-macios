using System;

using Foundation;

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
			// VNImageOptions is a DictionaryContainer that exposes a Weak* NSDictionary
			case "VNImageOptions":
				return true;
			}

			switch (type.Namespace) {
			case "MonoMac.QTKit": // QTKit has been removed from macOS
			case "QTKit":
				return true;
			}

			return base.Skip (type);
		}
	}
}
