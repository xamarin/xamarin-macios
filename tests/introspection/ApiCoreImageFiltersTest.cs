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
using System.Text;

using NUnit.Framework;

using CoreImage;
using Foundation;
using ObjCRuntime;
#if !MONOMAC
using UIKit;
#endif

namespace Introspection {

	[TestFixture]
	// we want the tests to be available because we use the linker
	[Preserve (AllMembers = true)]
	public abstract class ApiCoreImageFiltersTest : ApiBaseTest {

		static Type CIFilterType = typeof (CIFilter);

#if false
		static TextWriter BindingOutput;
#else
		static TextWriter BindingOutput = Console.Out;
#endif

		protected virtual bool Skip (Type type)
		{
			return Skip (type.Name) || SkipDueToAttribute (type);
		}

		protected virtual bool Skip (string nativeName)
		{
			switch (nativeName) {
			case "CIRawFilter":
				return true;
			// Both reported in radar #21548819
			//  NSUnknownKeyException [<CIDepthOfField 0x158586970> valueForUndefinedKey:]: this class is not key value coding-compliant for the key inputPoint2.
			case "CIDepthOfField":
				return true;
			// Apple does **not** document filters as API (like we do)
			// uncomment calls to `GenerateBinding` to use introspection code to generate the skeleton binding code and complete it
			// e.g. picking better types like `bool` instead of `NSNumber'
			default:
				return false;
			case "CIConvertLabToRGB":
			case "CIConvertRGBtoLab":
				return true;
			}
		}

		[Test]
		// this test checks that all native filters have a managed peer, i.e. against missing filters
		public void CheckNativeFilters ()
		{
			Errors = 0;
			List<string> filters = new List<string> ();
			int n = 0;
			string qname = CIFilterType.AssemblyQualifiedName;
			// that will give us only the list of filters supported by the executing version of iOS
			foreach (var filter_name in CIFilter.FilterNamesInCategories (null)) {
				if (Skip (filter_name))
					continue;
				string type_name = qname.Replace ("CIFilter", filter_name);
				if (Type.GetType (type_name, false, true) is null) {
					filters.Add (filter_name);
					if (BindingOutput is not null)
						GenerateBinding (CIFilter.FromName (filter_name), BindingOutput);
				}
				n++;
			}
			Assert.That (filters.Count, Is.EqualTo (0), "{0} native filters missing: {1}", filters.Count, String.Join (", ", filters));
		}

		[Test]
		// this test checks that all managed filters have a native peer, i.e. against extra filters
		public void CheckManagedFilters ()
		{
			Errors = 0;
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
				if ((ctor is null) || ctor.IsAbstract)
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
						GenerateBinding (CIFilter.FromName (super), BindingOutput);
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

			if (!attributes.TryGetValue ((NSString) "CIAttributeFilterAvailable_iOS", out value)) {
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

			if (!attributes.TryGetValue ((NSString) "CIAttributeFilterAvailable_Mac", out value)) {
				writer.WriteLine ("[NoMac]");
			} else {
				try {
					var mac = Version.Parse (value.ToString ());
					// we only document availability for 10.7+
					if (mac.Minor > 6)
						writer.WriteLine ("[Mac ({0},{1})]", mac.Major, mac.Minor);
				} catch (FormatException) {
					// 10.? is not a valid version - we'll assume it was added a long time ago (in a galaxy far away)
					writer.WriteLine ("// incorrect version string for OSX: '{0}' Double-check documentation", value);
				}
			}
			writer.WriteLine ("[BaseType (typeof (CIFilter))]");
			var fname = attributes [(NSString) "CIAttributeFilterName"].ToString ();
			writer.WriteLine ("interface {0} {{", fname);
			foreach (var k in attributes.Keys) {
				var key = k.ToString ();
				if (key.StartsWith ("CIAttribute", StringComparison.Ordinal))
					continue;
#if !NET
				// CIFilter defines it for all filters
				if (key == "inputImage")
					continue;
#endif

				writer.WriteLine ();
				var dict = attributes [k] as NSDictionary;
				var type = dict [(NSString) "CIAttributeClass"];
				writer.WriteLine ($"\t[CoreImageFilterProperty (\"{key}\")]");

#if NET
				// by default we drop the "input" prefix, but keep the "output" prefix to avoid confusion, except for 'inputImage'
				if (key.StartsWith ("input", StringComparison.Ordinal) && key != "inputImage")
#else
				// by default we drop the "input" prefix, but keep the "output" prefix to avoid confusion
				if (key.StartsWith ("input", StringComparison.Ordinal))
#endif
					key = Char.ToUpperInvariant (key [5]) + key.Substring (6);

				writer.WriteLine ("\t/* REMOVE-ME");
				writer.WriteLine (dict);
				writer.WriteLine ("\t*/");
				writer.WriteLine ($"\t{type} {key} {{ get; set; }}");
			}
			writer.WriteLine ("}");
			writer.WriteLine ();
			writer.Flush ();
		}

		[Test]
		public void Protocols ()
		{
			Errors = 0;
			var to_confirm_manually = new StringBuilder ();
			ContinueOnFailure = true;
			var nspace = CIFilterType.Namespace;
			var types = CIFilterType.Assembly.GetTypes ();
			foreach (Type t in types) {
				if (t.Namespace != nspace)
					continue;

				// e.g. FooProtocolWrapper
				if (!t.IsPublic)
					continue;

				switch (t.Name) {
				// we are interested in subclasses (real) filters
				case "CIFilter":
					continue;
				// no protocol has been added (yet?) you can confirm with `grep` that it does not report anything in the terminal
				case "CIAdditionCompositing":
				case "CIAreaAverage":
				case "CIAreaHistogram":
				case "CIAreaMaximum":
				case "CIAreaMaximumAlpha":
				case "CIAreaMinimum":
				case "CIAreaMinimumAlpha":
				case "CIAreaMinMax":
				case "CIAreaMinMaxRed":
				case "CIBlendFilter":
				case "CIBumpDistortion":
				case "CIBumpDistortionLinear":
				case "CICameraCalibrationLensCorrection":
				case "CICircleSplashDistortion":
				case "CICircularWrap":
				case "CIClamp":
				case "CICodeGenerator":
				case "CIColorBlendMode":
				case "CIColorBurnBlendMode":
				case "CIColorDodgeBlendMode":
				case "CIColumnAverage":
				case "CICompositingFilter":
				case "CIConstantColorGenerator":
				case "CIConvolution3X3":
				case "CIConvolution5X5":
				case "CIConvolution7X7":
				case "CIConvolution9Horizontal":
				case "CIConvolution9Vertical":
				case "CIConvolutionCore":
				case "CICoreMLModelFilter":
				case "CICrop":
				case "CIDarkenBlendMode":
				case "CIDepthBlurEffect":
				case "CIDepthDisparityConverter":
				case "CIDifferenceBlendMode":
				case "CIDisplacementDistortion":
				case "CIDistortionFilter":
				case "CIDivideBlendMode":
				case "CIDroste":
				case "CIExclusionBlendMode":
				case "CIFaceBalance":
				case "CIGlassDistortion":
				case "CIGlassLozenge":
				case "CIGuidedFilter":
				case "CIHardLightBlendMode":
				case "CIHistogramDisplayFilter":
				case "CIHoleDistortion":
				case "CIHueBlendMode":
				case "CIImageGenerator":
				case "CIKeystoneCorrection":
				case "CIKMeans":
				case "CILightenBlendMode":
				case "CILightTunnel":
				case "CILinearBlur":
				case "CILinearBurnBlendMode":
				case "CILinearDodgeBlendMode":
				case "CILuminosityBlendMode":
				case "CIMaximumCompositing":
				case "CIMinimumCompositing":
				case "CIMorphology":
				case "CIMorphologyRectangle":
				case "CIMultiplyBlendMode":
				case "CIMultiplyCompositing":
				case "CINinePartStretched":
				case "CINinePartTiled":
				case "CIOverlayBlendMode":
				case "CIPinchDistortion":
				case "CIPinLightBlendMode":
				case "CIReductionFilter":
				case "CIRowAverage":
				case "CISampleNearest":
				case "CISaturationBlendMode":
				case "CIScreenBlendMode":
				case "CIScreenFilter":
				case "CISoftLightBlendMode":
				case "CISourceAtopCompositing":
				case "CISourceInCompositing":
				case "CISourceOutCompositing":
				case "CISourceOverCompositing":
				case "CIStretchCrop":
				case "CISubtractBlendMode":
				case "CITileFilter":
				case "CITorusLensDistortion":
				case "CITwirlDistortion":
				case "CIVortexDistortion":
					// this list is likely to change with newer Xcode - uncomment if you want to the script to check the list
					//to_confirm_manually.AppendLine ($"grep {t.Name} `xcode-select -p`/Platforms/iPhoneOS.platform/Developer/SDKs/iPhoneOS.sdk/System/Library/Frameworks/CoreImage.framework/Headers/*.h");
					// since xtro will report the missing protocols this is a 2nd layer of safety :)
					continue;
				}

				bool assign = typeof (ICIFilterProtocol).IsAssignableFrom (t);
				bool suffix = t.Name.EndsWith ("Protocol", StringComparison.Ordinal);

				if (t.IsInterface) {
					if (assign) {
						// check that `IFooProtocol` has a [Protocol (Name = "Foo")] attribute
						var ca = t.GetCustomAttribute<ProtocolAttribute> (false);
						if (ca is null) {
							ReportError ($"Managed {t.Name} should have a '[Protocol (Name=\"{t.Name.Replace ("Protocol", "")}\")]' attribute");
						}
						// check that the managed name ends with Protocol, so we can have the _normal_ name to be a concrete type (like our historic, strongly typed filters)
						if (!suffix) {
							ReportError ($"Managed {t.Name} should have a 'Protocol' suffix");
						}
					} else if (suffix) {
						ReportError ($"Managed {t.Name} should implement 'ICIFilterProtocol' interface.");
					}
				} else if (suffix) {
					ReportError ($"Managed {t.Name} should be an interface since it represent a protocol");
				} else if (assign) {
					// all CIFilter should map to a `ICI*Protocol` interface / protocol
					bool found = false;
					foreach (var inft in t.GetInterfaces ()) {
						if (inft.Namespace == nspace && inft.Name.EndsWith ("Protocol", StringComparison.Ordinal)) {
							found = true;
							break;
						}
					}
					if (!found)
						ReportError ($"Managed CIFilter '{t.Name}' does not conform to any CoreImage filter protocol.");
				} else if (CIFilterType.IsAssignableFrom (t)) {
					// missing ICIFilterProtocol
					to_confirm_manually.AppendLine ($"grep \"protocol {t.Name} \" `xcode-select -p`/Platforms/iPhoneOS.platform/Developer/SDKs/iPhoneOS.sdk/System/Library/Frameworks/CoreImage.framework/Headers/*.h");
					ReportError ($"Managed CIFilter '{t.Name}' does not conform to 'ICIFilterProtocol' protocol. Confirm with generated `grep` script on console.");
				}
			}
			if (to_confirm_manually.Length > 0) {
				Console.WriteLine (to_confirm_manually);
			}
			Assert.AreEqual (0, Errors, "{0} potential errors found{1}", Errors, Errors == 0 ? string.Empty : ":\n" + ErrorData.ToString () + "\n");
		}

		[Test]
		public void Keys ()
		{
			Errors = 0;
			ContinueOnFailure = true;
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
				if ((ctor is null) || ctor.IsAbstract)
					continue;

				CIFilter f = ctor.Invoke (null) as CIFilter;

				// first check that every property can be mapped to an input key - except if it starts with "Output"
				foreach (var p in t.GetProperties (BindingFlags.Public | BindingFlags.Instance)) {
					var pt = p.DeclaringType;
					if (!CIFilterType.IsAssignableFrom (pt) || (pt == CIFilterType))
						continue;

					if (SkipDueToAttribute (p))
						continue;

					var getter = p.GetGetMethod ();
					var ea = getter.GetCustomAttribute<ExportAttribute> (false);
					// only properties coming (inlined) from protocols have an [Export] attribute
					if (ea is null)
						continue;
					var key = ea.Selector;
					// 'output' is always explicit
					if (key.StartsWith ("output", StringComparison.Ordinal)) {
						if (Array.IndexOf (f.OutputKeys, key) < 0) {
							ReportError ($"{t.Name}: Property `{p.Name}` mapped to key `{key}` is not part of `OutputKeys`.");
							//GenerateBinding (f, Console.Out);
						}
					} else {
						// special cases (protocol names are better)
						switch (t.Name) {
						case "CIBicubicScaleTransform":
							switch (key) {
							case "parameterB":
								key = "inputB";
								break;
							case "parameterC":
								key = "inputC";
								break;
							}
							break;
						case "CICmykHalftone":
							switch (key) {
							case "grayComponentReplacement":
								key = "inputGCR";
								break;
							case "underColorRemoval":
								key = "inputUCR";
								break;
							}
							break;
						case "CIGlassDistortion":
							switch (key) {
							case "textureImage":
								key = "texture";
								break;
							}
							break;
						}
						// 'input' is implied (generally) and explicit (in a few cases)
						if (!key.StartsWith ("input", StringComparison.Ordinal))
							key = "input" + Char.ToUpperInvariant (key [0]) + key.Substring (1);

						if (Array.IndexOf (f.InputKeys, key) < 0) {
							ReportError ($"{t.Name}: Property `{p.Name}` mapped to key `{key}` is not part of `InputKeys`.");
							//GenerateBinding (f, Console.Out);
						}
					}
				}

				// second check that every input key is mapped to an property
				foreach (var key in f.InputKeys) {
					string cap = Char.ToUpperInvariant (key [0]) + key.Substring (1);
					// special cases (protocol names are better)
					switch (t.Name) {
					case "CICmykHalftone":
						switch (key) {
						case "inputGCR":
							cap = "GrayComponentReplacement";
							break;
						case "inputUCR":
							cap = "UnderColorRemoval";
							break;
						}
						break;
					case "CIAccordionFoldTransition":
						switch (key) {
						case "inputNumberOfFolds":
							cap = "FoldCount";
							break;
						}
						break;
					case "CIBicubicScaleTransform":
						switch (key) {
						case "inputB":
							cap = "ParameterB";
							break;
						case "inputC":
							cap = "ParameterC";
							break;
						}
						break;
					}
					// IgnoreCase because there are acronyms (more than 2 letters) that naming convention force us to change
					var pi = t.GetProperty (cap, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
					if (pi is null) {
						// 2nd chance: some, but not all, property are prefixed by `Input`
						if (key.StartsWith ("input", StringComparison.Ordinal)) {
							cap = Char.ToUpperInvariant (key [5]) + key.Substring (6);
							pi = t.GetProperty (cap, BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
						}
					}
					if (pi is null) {
						ReportError ($"{t.Name}: Input Key `{key}` is NOT mapped to a `{cap}` property.");
						//GenerateBinding (f, Console.Out);
					} else if (pi.GetSetMethod () is null)
						ReportError ($"{t.Name}: Property `{pi.Name}` MUST have a setter.");
				}

				// third check that every output key is mapped to an property
				foreach (var key in f.OutputKeys) {
					// special cases
					switch (t.Name) {
					case "CIKeystoneCorrectionCombined":
					case "CIKeystoneCorrectionHorizontal":
					case "CIKeystoneCorrectionVertical":
						switch (key) {
						case "outputRotationFilter":
							continue; // lack of documentation about the returned type
						}
						break;
					case "CILanczosScaleTransform":
						switch (key) {
						// ref: https://github.com/xamarin/xamarin-macios/issues/7209
						case "outputImageNewScaleX:scaleY:":
						case "outputImageOldScaleX:scaleY:":
							continue;
						}
						break;
					case "CIDiscBlur":
						switch (key) {
						// existed in iOS 10.3 but not in iOS 13 - we're not adding them
						case "outputImageOriginal":
						case "outputImageEnhanced":
							continue;
						}
						break;
					case "CIGaussianBlur":
						switch (key) {
						case "outputImageV1":
							// existed briefly in macOS 10.11, but neither before nor after.
							continue;
						}
						break;
					case "CIAreaAverage":
					case "CIAreaHistogram":
					case "CIAreaMinMax":
						switch (key) {
						case "outputImageMPS":
						case "outputImageMPS:":
						case "outputImageNonMPS:":
							// no doc for argument
							continue;
						}
						break;
					case "CIAreaLogarithmicHistogram":
						switch (key) {
						case "outputImageNonMPS":
						case "outputData":
						case "outputImageMPS":
							// no doc for argument
							continue;
						}
						break;
					}

					var cap = Char.ToUpperInvariant (key [0]) + key.Substring (1);
					// IgnoreCase because there are acronyms (more than 2 letters) that naming convention force us to change
					var po = t.GetProperty (cap, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
					if (po is null) {
						ReportError ($"{t.Name}: Output Key `{key}` is NOT mapped to a `{cap}` property.");
						//GenerateBinding (f, Console.Out);
					} else if (po.GetSetMethod () is not null)
						ReportError ($"{t.Name}: Property `{po.Name}` should NOT have a setter.");
				}
			}
			Assert.AreEqual (0, Errors, "{0} potential errors found{1}", Errors, Errors == 0 ? string.Empty : ":\n" + ErrorData.ToString () + "\n");
		}
	}
}

#endif // !__WATCHOS__
