using System;
using System.Reflection;

using Foundation;

using NUnit.Framework;

// Disable until we get around to enable + fix any issues.
#nullable disable

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
			case "CHHapticPatternDefinition":
				return property.Name == "WeakParameterCurve" || property.Name == "WeakParameterCurveControlPoints";
			}
			return base.Skip (property);
		}
	}
}
