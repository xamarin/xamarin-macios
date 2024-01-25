#nullable enable

using System;
using System.Runtime.Versioning;

using CoreFoundation;

using Foundation;

namespace NaturalLanguage {


#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
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

				var value = CFDictionary.GetValue (Dictionary.Handle, key.Handle);
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
