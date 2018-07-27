//
// Test the generated API for all CoreImage filters
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2013, 2015 Xamarin Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

#if !__WATCHOS__

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using NUnit.Framework;

#if XAMCORE_2_0
using CoreImage;
using Foundation;
using ObjCRuntime;
#if !MONOMAC
using UIKit;
#endif
#elif MONOMAC
using MonoMac.CoreImage;
using MonoMac.Foundation;
#else
using MonoTouch.CoreImage;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.UIKit;
#endif

namespace Introspection {

	[TestFixture]
	// we want the tests to be available because we use the linker
	[Preserve (AllMembers = true)]
	public abstract class ApiCoreImageFiltersTest : ApiBaseTest {

		static Type CIFilterType = typeof (CIFilter);

		protected virtual bool Skip (Type type)
		{
			return Skip (type.Name) || SkipDueToAttribute (type);
		}

		protected virtual bool Skip (string nativeName)
		{
			switch (nativeName) {
			// Both reported in radar #21548819
			//  NSUnknownKeyException [<CIDepthOfField 0x158586970> valueForUndefinedKey:]: this class is not key value coding-compliant for the key inputPoint2.
			case "CIDepthOfField":
				return true;
			// Apple does **not** document filters as API (like we do)
			// uncomment calls to `GenerateBinding` to use introspection code to generate the skeleton binding code and complete it
			// e.g. picking better types like `bool` instead of `NSNumber'
			default:
 				return false;
			}
		}

		[Test]
		// this test checks that all native filters have a managed peer, i.e. against missing filters
		public void CheckNativeFilters ()
		{
			List<string> filters = new List<string> ();
			int n = 0;
			string qname = CIFilterType.AssemblyQualifiedName;
			// that will give us only the list of filters supported by the executing version of iOS
			foreach (var filter_name in CIFilter.FilterNamesInCategories (null)) {
				if (Skip (filter_name))
					continue;
				string type_name = qname.Replace ("CIFilter", filter_name);
				if (Type.GetType (type_name, false, true) == null) {
					filters.Add (filter_name);
					// uncomment to generate bindings for any new native filter
//					GenerateBinding (CIFilter.FromName (filter_name), Console.Out);
				}
				n++;
			}
			Assert.That (filters.Count, Is.EqualTo (0), "{0} native filters missing: {1}", filters.Count, String.Join (", ", filters));
		}

		[Test]
		// this test checks that all managed filters have a native peer, i.e. against extra filters
		public void CheckManagedFilters ()
		{
			ContinueOnFailure = true;
			List<string> filters = new List<string> (CIFilter.FilterNamesInCategories (null));
			var superFilters = new List<string> ();
			var nspace = CIFilterType.Namespace;
			var types = CIFilterType.Assembly.GetTypes ();
			foreach (Type t in types) {
				if (t.Namespace != nspace)
					continue;

				if (t.IsAbstract || !CIFilterType.IsAssignableFrom (t))
					continue;

				// we need to skip the filters that are not supported by the executing version of iOS
				if (Skip (t))
					continue; 

				var ctor = t.GetConstructor (Type.EmptyTypes);
				if ((ctor == null) || ctor.IsAbstract)
					continue;

				NSObject obj = ctor.Invoke (null) as NSObject;
#if false
				// check base type - we might have our own base type or different names, so it's debug only (not failure)
				var super = new Class (obj.Class.SuperClass).Name;
				var bt = t.BaseType.Name;
				if ((super != bt) && (bt == "CIFilter")) { // check if we should (like Apple) use a non-default base type for filters
					Console.WriteLine ("[WARN] {0}.SuperClass == {1} (native) and {2} managed", t.Name, super, bt);
					if (!superFilters.Contains (super)) {
						superFilters.Add (super);
						Console.WriteLine ("[GENERATED] {0}", super);
						GenerateBinding (CIFilter.FromName (super), Console.Out);
					}
				}
#endif
				int result = filters.RemoveAll (s => StringComparer.OrdinalIgnoreCase.Compare (t.Name, s) == 0);
				if ((result == 0) && !Skip (t))
					ReportError ($"Managed {t.Name} was not part of the native filter list");
			}
			// in case it's a buggy filter we need to try to remove it from the list too
			for (int i = filters.Count - 1; i >= 0; i--) {
				if (Skip (filters [i]))
					filters.RemoveAt (i);
			}
			Assert.That (filters.Count, Is.EqualTo (0), "Managed filters not found for {0}", String.Join (", ", filters));
		}

		static void GenerateBinding (NSObject filter, TextWriter writer)
		{
			NSObject value;
			var attributes = (filter as CIFilter).Attributes;

			writer.WriteLine ("[CoreImageFilter]");

			if (!attributes.TryGetValue ((NSString)"CIAttributeFilterAvailable_iOS", out value)) {
				writer.WriteLine ("[NoiOS]");
			} else {
				var v = value.ToString ();
				// in the (quite common) case we get "5" for iOS 5.0
				if (v.IndexOf ('.') == -1)
					v += ".0";
				var ios = Version.Parse (v);
				// we only document availability for iOS 6+
				if (ios.Major > 5)
					writer.WriteLine ("[iOS ({0},{1})]", ios.Major, ios.Minor);
			}

			if (!attributes.TryGetValue ((NSString)"CIAttributeFilterAvailable_Mac", out value)) {
				writer.WriteLine ("[NoMac]");
			} else {
				try {
					var mac = Version.Parse (value.ToString ());
					// we only document availability for 10.7+
					if (mac.Minor > 6)
						writer.WriteLine ("[Mac ({0},{1})]", mac.Major, mac.Minor);
				}
				catch (FormatException) {
					// 10.? is not a valid version - we'll assume it was added a long time ago (in a galaxy far away)
					writer.WriteLine ("// incorrect version string for OSX: '{0}' Double-check documentation", value);
				}
			}
			writer.WriteLine ("[BaseType (typeof (CIFilter))]");
			var fname = attributes [(NSString)"CIAttributeFilterName"].ToString ();
			writer.WriteLine ("interface {0} {{", fname);
			foreach (var k in attributes.Keys) {
				var key = k.ToString ();
				if (key.StartsWith ("CIAttribute", StringComparison.Ordinal))
					continue;
				// CIFilter defines it for all filters
				if (key == "inputImage")
					continue;
				
				writer.WriteLine ();
				var dict = attributes [k] as NSDictionary;
				var type = dict [(NSString) "CIAttributeClass"];
				writer.WriteLine ("\t[CoreImageFilterProperty (\"{0}\")]", key);

				// by default we drop the "input" prefix, but keep the "output" prefix to avoid confusion
				if (key.StartsWith ("input", StringComparison.Ordinal))
					key = Char.ToUpperInvariant (key [5]) + key.Substring (6);
				
				var ptype = type.ToString ();
				// Too many things ends up in NSNumber but we do a better job in our bindings
				if (ptype == "NSNumber") {
					ptype = "float";
					writer.WriteLine ("\t// TODO: this was an NSNumber transformed to float, but maybe an int or bool is more appropriate");
				}
				writer.WriteLine ("\t{0} {1} {{ get; set; }}", ptype, key);
			}
			writer.WriteLine ("}");
			writer.WriteLine ();
		}
	}
}

#endif // !__WATCHOS__
