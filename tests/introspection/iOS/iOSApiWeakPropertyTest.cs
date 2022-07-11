using System;
using System.Reflection;

using Foundation;

using NUnit.Framework;

namespace Introspection {

	[TestFixture]
	// we want the tests to be available because we use the linker
	[Preserve (AllMembers = true)]
	public class iOSApiWeakPropertyTest : ApiWeakPropertyTest {

		public iOSApiWeakPropertyTest ()
		{
			ContinueOnFailure = true;
		}

		protected override bool Skip (PropertyInfo property)
		{
			switch (property.DeclaringType.Name) {
			// WeakVideoGravity is an NSString that we could/should provide a better binding (e.g. enum)
			case "AVPlayerViewController":
				return property.Name == "WeakVideoGravity";
			// CATextLayer.WeakFont is done correctly by hand
			case "CATextLayer":
				return property.Name == "WeakFont";
			// NSAttributedStringDocumentAttributes is a DictionaryContainer that expose some Weak* NSDictionary
			case "NSAttributedStringDocumentAttributes":
				return property.Name == "WeakDocumentType" || property.Name == "WeakDefaultAttributes";
			// UIFontAttributes is a DictionaryContainer that expose a Weak* NSDictionary
			case "UIFontAttributes":
				return property.Name == "WeakFeatureSettings";
			// UIStringAttributes is a DictionaryContainer that expose a Weak* NSString
			case "UIStringAttributes":
				return property.Name == "WeakTextEffect";
			// VNImageOptions is a DictionaryContainer that exposes a Weak* NSDictionary
			case "VNImageOptions":
				return property.Name == "WeakProperties";
			// NSString manually bound as smart enum CLSContextTopic
			case "CLSContext":
				return property.Name == "WeakTopic";
#if !XAMCORE_3_0
			// #37451 - setter does not exists but we have to keep it for binary compatibility
			// OTOH we can't give it a selector (private API) even if we suspect Apple is mostly running `strings` on executable
			case "IUIViewControllerPreviewing":
				return property.Name == "WeakDelegate";
			case "UIViewControllerPreviewingWrapper":
				return property.Name == "WeakDelegate";
#endif
			case "CHHapticPatternDefinition":
				return property.Name == "WeakParameterCurve" || property.Name == "WeakParameterCurveControlPoints";
			}
			return base.Skip (property);
		}
	}
}
