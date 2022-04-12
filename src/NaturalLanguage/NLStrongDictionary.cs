#nullable enable

using System;

using CoreFoundation;
using Foundation;

namespace NaturalLanguage {

	// nicer replacement for `NSDictionary<NSString, NSArray<NSString>>`
	public class NLStrongDictionary : DictionaryContainer {

#if !COREBUILD
		public NLStrongDictionary ()
		{
		}

		public NLStrongDictionary (NSDictionary dictionary) : base (dictionary)
		{
		}

		public string[] this [NSString key] {
			get {
				if (key == null)
					throw new ArgumentNullException (nameof (key));

				var value = CFDictionary.GetValue (Dictionary.Handle, key.Handle);
				var array = CFArray.StringArrayFromHandle (value) ?? throw new ArgumentOutOfRangeException (nameof (key));
				foreach (var str in array) {
					if (str is null)
						ObjCRuntime.ThrowHelper.ThrowArgumentNullException ($"Key value {nameof (key)} contains a null string.");
				}
				return array!;
			}
			set {
				SetArrayValue (key, value);
			}
		}

		public string[] this [string key] {
			get {
				return this [(NSString) key];
			}
			set {
				SetArrayValue ((NSString) key, value);
			}
		}
#endif
	}
}
