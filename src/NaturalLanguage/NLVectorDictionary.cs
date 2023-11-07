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
	// nicer replacement for `NSDictionary<NSString, NSArray<NSNumber>>`
	public class NLVectorDictionary : DictionaryContainer {

#if !COREBUILD
		public NLVectorDictionary ()
		{
		}

		public NLVectorDictionary (NSDictionary dictionary) : base (dictionary)
		{
		}

		public float [] this [NSString key] {
			get {
				if (key is null)
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (key));

				var a = CFDictionary.GetValue (Dictionary.Handle, key.Handle);
				return NSArray.ArrayFromHandle<float> (a, input => {
					return new NSNumber (input).FloatValue;
				});
			}
			set {
				if (key is null)
					ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (key));

				if (value is null)
					RemoveValue (key);
				else
					Dictionary [key] = NSArray.From (value);
			}
		}

		public float [] this [string key] {
			get {
				return this [(NSString) key];
			}
			set {
				this [(NSString) key] = value;
			}
		}
#endif
	}
}
