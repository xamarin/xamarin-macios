// Copyright 2018, Microsoft, Corp.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
using System;
using System.Linq;
using System.Collections.Generic;

using Foundation;

namespace NaturalLanguage {

	public partial class NLLanguageRecognizer {

		public static NLLanguage GetDominantLanguage (string @string)
		{
			var nsstring = NSString.CreateNative (@string);
			var nslang = _GetDominantLanguage (nsstring);
			var lang =  NLLanguage.Undetermined;
			if (nslang != null)
				lang = NLLanguageExtensions.GetValue (nslang);
			NSString.ReleaseNative (nsstring);
			nslang.Dispose ();
			return lang;
		}
	
		public Dictionary<NLLanguage, double> GetLanguageHypotheses (nuint maxHypotheses)
		{
			using (var hypo = GetNativeLanguageHypotheses (maxHypotheses)) {
				var result = new Dictionary<NLLanguage, double> (hypo.Keys.Length);
				foreach (var k in hypo.Keys) {
					result[NLLanguageExtensions.GetValue (k)] = hypo[k].DoubleValue;
				}
				return result;
			}
		}

		public Dictionary<NLLanguage, double> LanguageHints
		{
			get {
				var result = new Dictionary<NLLanguage, double> (NativeLanguageHints.Keys.Length);
				foreach (var k in NativeLanguageHints.Keys) {
					result[NLLanguageExtensions.GetValue (k)] = NativeLanguageHints[k].DoubleValue;
				}
				return result;
			}
			set {
				var i = 0;
				var nsKeys = new NSString[value.Keys.Count];
				var nsValues = new NSNumber[value.Keys.Count];
				foreach (var item in value) {
					nsKeys[i] = NLLanguageExtensions.GetConstant (item.Key);
					nsValues[i] = new NSNumber (item.Value);
					i++;
				}
				NativeLanguageHints = NSDictionary<NSString, NSNumber>.FromObjectsAndKeys (nsValues, nsKeys, nsKeys.Length);
			}
		}
	}
}
