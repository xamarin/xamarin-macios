#nullable enable

using CoreFoundation;

namespace NaturalLanguage {
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
	// nicer replacement for `NSDictionary<NSString, NSArray<NSString>>`
	public class NLStrongDictionary : DictionaryContainer {

#if !COREBUILD
		public NLStrongDictionary ()
		{
		}

		public NLStrongDictionary (NSDictionary dictionary) : base (dictionary)
		{
		}

		public string? []? this [NSString key] {
			get {
				if (key is null)
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (key));

				var value = Dictionary.LowlevelObjectForKey (key);
				return CFArray.StringArrayFromHandle (value);
			}

			set {
				SetArrayValue (key, value!);
			}
		}

		public string? []? this [string key] {
			get {
				return this [(NSString) key];
			}
			set {
				SetArrayValue ((NSString) key, value!);
			}
		}
#endif
	}
}
