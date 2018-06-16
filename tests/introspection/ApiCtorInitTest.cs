//
// Test the generated API `init` selectors are usable by the binding consumers
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2012-2015 Xamarin Inc.
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

using System;
using System.Reflection;
using System.Linq;
using System.Text;

using NUnit.Framework;

#if XAMCORE_2_0
using Foundation;
using ObjCRuntime;
#elif MONOMAC
using MonoMac.Foundation;
using MonoMac.ObjCRuntime;
#else
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
#endif

namespace Introspection {

	public abstract class ApiCtorInitTest : ApiBaseTest {

		string instance_type_name;

		/// <summary>
		/// Gets or sets a value indicating whether this test fixture will log untested types.
		/// </summary>
		/// <value><c>true</c> if log untested types; otherwise, <c>false</c>.</value>
		public bool LogUntestedTypes { get; set; }

		/// <summary>
		/// Override this method if you want the test to skip some specific types.
		/// By default types decorated with [Model] will be skipped.
		/// </summary>
		/// <param name="type">The Type to be tested</param>
		protected virtual bool Skip (Type type)
		{
			if (type.ContainsGenericParameters)
				return true;

#if !XAMCORE_2_0
			// skip delegate (and other protocol references)
			foreach (object ca in type.GetCustomAttributes (false)) {
				if (ca is ProtocolAttribute)
					return true;
				if (ca is ModelAttribute)
					return true;
			}
#endif

			switch (type.Name) {
			case "JSExport":
				// This is interesting: Apple defines a private JSExport class - if you try to define your own in an Objective-C project you get this warning at startup:
				//     objc[334]: Class JSExport is implemented in both /Applications/Xcode91.app/Contents/Developer/Platforms/iPhoneOS.platform/Developer/Library/CoreSimulator/Profiles/Runtimes/iOS.simruntime/Contents/Resources/RuntimeRoot/System/Library/Frameworks/JavaScriptCore.framework/JavaScriptCore (0x112c1e430) and /Users/rolf/Library/Developer/CoreSimulator/Devices/AC5323CF-225F-44D9-AA18-A37B7C28CA68/data/Containers/Bundle/Application/DEF9EAFC-CB5C-454F-97F5-669BBD00A609/jsexporttest.app/jsexporttest (0x105b49df0). One of the two will be used. Which one is undefined.
				// Due to how we treat models, we'll always look the Objective-C type up at runtime (even with the static registrar),
				// see that there's an existing JSExport type, and use that one instead of creating a new type.
				// This is problematic, because Apple's JSExport is completely unusable, and will crash if you try to do anything.
				return true;
#if !XAMCORE_2_0
			case "AVAssetResourceLoader": // We have DisableDefaultCtor in XAMCORE_2_0 but can't change in compat because of backwards compat
			case "AVAssetResourceLoadingRequest":
			case "AVAssetResourceLoadingContentInformationRequest":
#endif
			// on iOS 8.2 (beta 1) we get:  NSInvalidArgumentException Caller did not provide an activityType, and this process does not have a NSUserActivityTypes in its Info.plist.
			// even if we specify an NSUserActivityTypes in the Info.plist - might be a bug or there's a new (undocumented) requirement
			case "NSUserActivity":
				return true;
			case "NEPacketTunnelProvider":
				return true;
			case "NSUnitDispersion": // -init should never be called on NSUnit!
			case "NSUnitVolume": // -init should never be called on NSUnit!
			case "NSUnitDuration": // -init should never be called on NSUnit!
			case "NSUnitElectricCharge": // -init should never be called on NSUnit!
			case "NSUnitElectricCurrent": // -init should never be called on NSUnit!
			case "NSUnitElectricPotentialDifference": // -init should never be called on NSUnit!
			case "NSUnitElectricResistance": // -init should never be called on NSUnit!
			case "NSUnit": // -init should never be called on NSUnit!
			case "NSUnitEnergy": // -init should never be called on NSUnit!
			case "NSUnitAcceleration": // -init should never be called on NSUnit!
			case "NSUnitFrequency": // -init should never be called on NSUnit!
			case "NSUnitAngle": // -init should never be called on NSUnit!
			case "NSUnitFuelEfficiency": // -init should never be called on NSUnit!
			case "NSUnitArea": // -init should never be called on NSUnit!
			case "NSUnitIlluminance": // -init should never be called on NSUnit!
			case "NSUnitConcentrationMass": // -init should never be called on NSUnit!
			case "NSUnitLength": // -init should never be called on NSUnit!
			case "NSUnitMass": // -init should never be called on NSUnit!
			case "NSUnitPower": // -init should never be called on NSUnit!
			case "NSUnitPressure": // -init should never be called on NSUnit!
			case "NSUnitSpeed": // -init should never be called on NSUnit!
				return true;
			case "MPSCnnNeuron": // Cannot directly initialize MPSCNNNeuron. Use one of the sub-classes of MPSCNNNeuron
			case "MPSCnnNeuronPReLU":
			case "MPSCnnNeuronHardSigmoid":
			case "MPSCnnNeuronSoftPlus":
				return true;
			case "MPSCnnBinaryConvolution": // [MPSCNNBinaryConvolution initWithDevice:] is not allowed. Please use initializers that are not marked NS_UNAVAILABLE.
			case "MPSCnnDilatedPoolingMax": // [MPSCNNDilatedPoolingMax initWithDevice:] is not allowed. Please use initializers that are not marked NS_UNAVAILABLE.
			case "MPSCnnPoolingL2Norm": // [MPSCNNPoolingL2Norm initWithDevice:] is not allowed. Please use initializers that are not marked NS_UNAVAILABLE.
				return true;
			case "MPSCnnBinaryFullyConnected": // Please initialize the MPSCNNBinaryFullyConnected class with initWithDevice:convolutionDescriptor:kernelWeights:biasTerms
				return true;
			case "MPSCnnUpsampling": // Cannot directly initialize MPSCNNUpsampling. Use one of the sub-classes of MPSCNNUpsampling
			case "MPSCnnUpsamplingBilinear":
			case "MPSCnnUpsamplingNearest":
				return true;
			case "MPSImageArithmetic": // Cannot directly initialize MPSImageArithmetic. Use one of the sub-classes of MPSImageArithmetic.
				return true;
			}

			return SkipDueToAttribute (type);
		}

		/// <summary>
		/// Checks that the Handle property of the specified NSObject instance is not null (not IntPtr.Zero).
		/// </summary>
		/// <param name="obj">NSObject instance to validate</param>
		protected virtual void CheckHandle (NSObject obj)
		{
			if (obj.Handle == IntPtr.Zero)
				ReportError ("{0} : Handle", instance_type_name);
		}

		/// <summary>
		/// Checks that ToString does not return null (not helpful for debugging) and that it does not crash.
		/// </summary>
		/// <param name="obj">NSObject instance to validate</param>
		protected virtual void CheckToString (NSObject obj)
		{
			if (obj.ToString () == null)
				ReportError ("{0} : ToString", instance_type_name);
		}
		
		bool GetIsDirectBinding (NSObject obj)
		{
#if XAMCORE_2_0
			int flags = (byte) typeof (NSObject).GetField ("flags", BindingFlags.Instance | BindingFlags.GetField | BindingFlags.NonPublic).GetValue (obj);
			return (flags & 4) == 4;
#else
			return (bool) typeof (NSObject).GetField ("IsDirectBinding", BindingFlags.Instance | BindingFlags.GetField | BindingFlags.NonPublic).GetValue (obj);
#endif
		}

		/// <summary>
		/// Checks that the IsDirectBinding property is identical to the IsWrapper property of the Register attribute.
		/// </summary>
		/// <param name="obj">Object.</param>
		protected virtual void CheckIsDirectBinding (NSObject obj)
		{
			var attrib = obj.GetType ().GetCustomAttribute<RegisterAttribute> (false);
			// only check types that we register - that way we avoid the 118 MonoTouch.CoreImagge.CI* "special" types
			if (attrib == null)
				return;
			var is_wrapper = attrib != null && attrib.IsWrapper;
			var is_direct_binding = GetIsDirectBinding (obj);
			if (is_direct_binding != is_wrapper)
				ReportError ("{0} : IsDirectBinding (expected {1}, got {2})", instance_type_name, is_wrapper, is_direct_binding);
		}

		/// <summary>
		/// Skip, or not, the specified pproperty from being verified.
		/// </summary>
		/// <param name="pi">PropertyInfo candidate</param>
		protected virtual bool Skip (PropertyInfo pi)
		{
			// manually bound API can have the attributes only on the property (and not on the getter/setter)
			return SkipDueToAttribute (pi);
		}

		/// <summary>
		/// Dispose the specified NSObject instance. In some cases objects cannot be disposed safely.
		/// Override this method to keep them alive while the remaining tests execute.
		/// </summary>
		/// <param name="obj">NSObject instance to dispose</param>
		/// <param name="type">Type of the object, to be used if special logic is required.</param>
		protected virtual void Dispose (NSObject obj, Type type)
		{
			//***** ApiCtorInitTest.DefaultCtorAllowed
			//2017-01-23 15:52:09.762 introspection[4084:16658258] *** -[NSKeyedArchiver dealloc]: warning: NSKeyedArchiver deallocated without having had -finishEncoding called on it.
			(obj as NSKeyedArchiver)?.FinishEncoding ();

			obj.Dispose ();
		}

		protected virtual void CheckNSObjectProtocol (NSObject obj)
		{
			// not documented to allow null, but commonly used this way. OTOH it's not clear what code answer this
			// (it might be different implementations) but we can make sure that Apple allows null with this test
			// ref: https://bugzilla.xamarin.com/show_bug.cgi?id=35924
			var kind_of_null = obj.IsKindOfClass (null);
			if (kind_of_null)
				ReportError ("{0} : IsKindOfClass(null) failed", instance_type_name);
			var is_member_of_null = obj.IsMemberOfClass (null);
			if (is_member_of_null)
				ReportError ("{0} : IsMemberOfClass(null) failed", instance_type_name);
			var respond_to_null = obj.RespondsToSelector (null);
			if (respond_to_null)
				ReportError ("{0} : RespondToSelector(null) failed", instance_type_name);
			var conforms_to_null = obj.ConformsToProtocol (IntPtr.Zero);
			if (conforms_to_null)
				ReportError ("{0} : ConformsToProtocol(null) failed", instance_type_name);
		}

		// if a .ctor is obsolete then it's because it was not usable (nor testable)
		protected override bool SkipDueToAttribute (MemberInfo member)
		{
			if (member == null)
				return false;
			var ca = member.GetCustomAttribute<ObsoleteAttribute> ();
			return ca != null || base.SkipDueToAttribute (member);
		}

		[Test]
		public void DefaultCtorAllowed ()
		{
			Errors = 0;
			ErrorData.Clear ();
			int n = 0;
			
			foreach (Type t in Assembly.GetTypes ()) {
				if (t.IsAbstract || !NSObjectType.IsAssignableFrom (t))
					continue;
				
				if (Skip (t))
					continue;

				var ctor = t.GetConstructor (Type.EmptyTypes);
				if (SkipDueToAttribute (ctor))
					continue;

				if ((ctor == null) || ctor.IsAbstract) {
					if (LogUntestedTypes)
						Console.WriteLine ("[WARNING] {0} was skipped because it had no default constructor", t);
					continue;
				}
				
				instance_type_name = t.FullName;
				if (LogProgress)
						Console.WriteLine ("{0}. {1}", n, instance_type_name);

				NSObject obj = null;
				try {
					obj = ctor.Invoke (null) as NSObject;
					CheckHandle (obj);
					CheckToString (obj);
					CheckIsDirectBinding (obj);
					CheckNSObjectProtocol (obj);
					Dispose (obj, t);
				}
				catch (Exception e) {
					// Objective-C exception thrown
					if (!ContinueOnFailure)
						throw;

					TargetInvocationException tie = (e as TargetInvocationException);
					if (tie != null)
						e = tie.InnerException;
					ReportError ("Default constructor not allowed for {0} : {1}", instance_type_name, e.Message);
				}
				n++;
			}
			Assert.AreEqual (0, Errors, "{0} potential errors found in {1} default ctor validated{2}", Errors, n, Errors == 0 ? string.Empty : ":\n" + ErrorData.ToString () + "\n");
		}

		// .NET constructors are not virtual, so we need to re-expose the base class .ctor when a subclass is created.
		// That's very important for designated initializer since we can end up with no correct/safe way to create
		// subclasses of an existing type
		[Test]
		public void DesignatedInitializer ()
		{
			Errors = 0;
			int n = 0;

			foreach (Type t in Assembly.GetTypes ()) {
				// we only care for NSObject subclasses that we expose publicly
				if (!t.IsPublic || !NSObjectType.IsAssignableFrom (t))
					continue;

				int designated = 0;
				foreach (var ctor in t.GetConstructors ()) {
					if (ctor.GetCustomAttribute<DesignatedInitializerAttribute> () == null)
						continue;
					designated++;
				}
				// that does not mean that inlining is not required, i.e. it might be useful, even needed
				// but it's not a showstopper for subclassing so we'll start with those cases
				if (designated > 1)
					continue;

				var base_class = t.BaseType;
				// NSObject ctor requirements are handled by the generator
				if (base_class == NSObjectType)
					continue;
				foreach (var ctor in base_class.GetConstructors ()) {
					// if the base ctor is a designated (not a convenience) initializer then we should re-expose it
					if (ctor.GetCustomAttribute<DesignatedInitializerAttribute> () == null)
						continue;

					// check if this ctor (from base type) is exposed in the current (subclass) type
					if (!Match (ctor, t))
						ReportError ("{0} should re-expose {1}::{2}", t, base_class.Name, ctor.ToString ().Replace ("Void ", String.Empty));
					n++;
				}
			}
			Assert.AreEqual (0, Errors, "{0} potential errors found in {1} designated initializer validated", Errors, n);
		}

		protected virtual bool Match (ConstructorInfo ctor, Type type)
		{
			switch (type.Name) {
			case "MKTileOverlayRenderer":
				// NSInvalidArgumentEception Expected a MKTileOverlay
				// looks like Apple has not yet added a DI for this type, but it should be `initWithTileOverlay:`
				if (ctor.ToString () == "Void .ctor(IMKOverlay)")
					return true;
				break;
			case "MPSMatrixMultiplication":
				// marked as NS_UNAVAILABLE - Use the above initialization method instead.
			case "MPSImageHistogram":
				// Could not initialize an instance of the type 'MetalPerformanceShaders.MPSImageHistogram': the native 'initWithDevice:' method returned nil.
				// make sense: there's a `initWithDevice:histogramInfo:` DI
				if (ctor.ToString () == "Void .ctor(IMTLDevice)")
					return true;
				break;
			case "NSDataDetector":
				// -[NSDataDetector initWithPattern:options:error:]: Not valid for NSDataDetector
				if (ctor.ToString () == "Void .ctor(NSString, NSRegularExpressionOptions, NSError&)")
					return true;
				break;
			case "SKStoreProductViewController":
			case "SKCloudServiceSetupViewController":
				// SKStoreProductViewController and SKCloudServiceSetupViewController are OS View Controllers which can't be customized. Therefore they shouldn't re-expose initWithNibName:bundle:
				if (ctor.ToString () == "Void .ctor(String, NSBundle)")
					return true;
				break;
			case "MKCompassButton":
			case "MKScaleView":
			case "MKUserTrackingButton":
				// Xcode9 added types that are created only from static methods (no init)
				return true;
#if __TVOS__
			case "UISearchBar":
				// - (nullable instancetype)initWithCoder:(NSCoder *)aDecoder NS_DESIGNATED_INITIALIZER __TVOS_PROHIBITED;
				return true;
#endif
			case "PdfAnnotationButtonWidget":
			case "PdfAnnotationChoiceWidget":
			case "PdfAnnotationCircle":
			case "PdfAnnotationFreeText":
			case "PdfAnnotationInk":
			case "PdfAnnotationLine":
			case "PdfAnnotationLink":
			case "PdfAnnotationMarkup":
			case "PdfAnnotationPopup":
			case "PdfAnnotationSquare":
			case "PdfAnnotationStamp":
			case "PdfAnnotationText":
			case "PdfAnnotationTextWidget":
				// This ctor was introduced in 10,13 but all of the above objects are deprecated in 10,12
				// so it does not make much sense to expose this ctor in all the deprecated subclasses
#if XAMCORE_2_0
				if (ctor.ToString () == "Void .ctor(CGRect, NSString, NSDictionary)")
#else
				if (ctor.ToString () == "Void .ctor(RectangleF, NSString, NSDictionary)")
#endif
					return true;
				break;
			case "VNTargetedImageRequest": // Explicitly disabled
				if (ctor.ToString () == "Void .ctor(VNRequestCompletionHandler)")
					return true;
				break;
			case "PKPaymentRequestShippingContactUpdate":
				// a more precise designated initializer is provided
				if (ctor.ToString () == "Void .ctor(PKPaymentSummaryItem[])")
					return true;
				break;
			case "NSApplication": // Does not make sense, also it crashes
			case "NSBitmapImageRep": // exception raised
			case "NSCachedImageRep": // exception raised
			case "NSCIImageRep": // exception raised
			case "NSCustomImageRep": // exception raised
			case "NSEPSImageRep": // exception raised
			case "NSPdfImageRep": // exception raised
				if (ctor.ToString () == "Void .ctor()")
					return true;
				break;
			case "AUPannerView": // Do not make sense without the AudioUnit
			case "AUGenericView": // Do not make sense without the AudioUnit
				if (ctor.ToString () == "Void .ctor(CGRect)")
					return true;
				break;
			case "MDLNoiseTexture":
			case "MDLSkyCubeTexture":
			case "MDLNormalMapTexture":
			case "MDLUrlTexture":
			case "MDLCheckerboardTexture":
			case "MDLColorSwatchTexture":
				// they don't make sense without extra arguments
				return true;
			case "ASCredentialProviderViewController": // goal is to "provides a standard interface for creating a credential provider extension", not a custom one
			case "INUIAddVoiceShortcutViewController": // Doesn't make sense without INVoiceShortcut and there is no other way to set this unless you use the other only .ctor
			case "INUIEditVoiceShortcutViewController": // Doesn't make sense without INVoiceShortcut and there is no other way to set this unless you use the other only .ctor
			case "ILClassificationUIExtensionViewController": // Meant to be an extension
				if (ctor.ToString () == "Void .ctor(String, NSBundle)")
					return true;
				break;
			}

			var ep = ctor.GetParameters ();
			// NonPublic to get `protected` which can be autogenerated for abstract types (subclassing a non-abstract type)
			foreach (var candidate in type.GetConstructors (BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)) {
				var cp = candidate.GetParameters ();
				if (ep.Length != cp.Length)
					continue;
				var result = true;
				for (int i = 0; i < ep.Length; i++) {
					var cpt = cp [i].ParameterType;
					var ept = ep [i].ParameterType;
					if (cpt == ept)
						continue;
					if (!cp [i].ParameterType.IsSubclassOf (ep [i].ParameterType))
						result = false;
				}
				if (result)
					return true;
			}
			return false;
		}

		[Test]
		public void ShouldNotExposeDefaultCtorTest ()
		{
			Errors = 0;
			int n = 0;

			// Set to 'true' to generate alloc/init ObjC code of types that fail this test.
			bool genObjCTestCode = false;
			var objCCode = genObjCTestCode ? new StringBuilder () : null;

			var types = Assembly.GetTypes ();
			var cifiltertype = types.FirstOrDefault (c => c.Name == "CIFilter");
			foreach (Type t in types) {
				// TODO: Remove this MPS check in the future, at the time of writing this we currently only care about MPS.
				if (!t.Name.StartsWith ("MPS", StringComparison.OrdinalIgnoreCase))
					continue;

				if (!t.IsPublic || !NSObjectType.IsAssignableFrom (t))
					continue;

				// ignore CIFilter derived subclasses since they are specially generated
				if (cifiltertype != null && t.IsSubclassOf (cifiltertype))
					continue;

				if (SkipCheckShouldNotExposeDefaultCtor (t))
					continue;

				var ctor = t.GetConstructor (Type.EmptyTypes);
				if (SkipDueToAttribute (ctor))
					continue;

				if (ctor == null || ctor.IsAbstract) {
					if (LogUntestedTypes)
						Console.WriteLine ("[WARNING] {0} was skipped because it had no default constructor", t);
					continue;
				}

				if (LogProgress)
					Console.WriteLine ($"{n}: {t.FullName}");

				var parentType = t.BaseType;
				var parentCtor = parentType.GetConstructor (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null);

				if (parentCtor == null) {
					ReportError ($"Type '{t.FullName}' is a possible candidate for [DisableDefaultCtor] because its BaseType '{parentType.FullName}' does not have one.");

					// Useful to test in Xcode
					if (genObjCTestCode) {
						var export = t.GetCustomAttribute<RegisterAttribute> ();
						var typeName = export?.Name ?? t.Name;
						objCCode.AppendLine ($"{typeName}* test{n} = [[{typeName} alloc] init];");
					}
				}
				n++;
			}
			Assert.AreEqual (0, Errors, $"{Errors} potential errors found in {n} BaseType empty ctor validated: \n{ErrorData}\n{(genObjCTestCode ? $"\n\n{objCCode}\n" : string.Empty)}");
		}

		protected virtual bool SkipCheckShouldNotExposeDefaultCtor (Type type)
		{
			if (type.ContainsGenericParameters)
				return true;

			foreach (object ca in type.GetCustomAttributes (false)) {
				if (ca is ProtocolAttribute || ca is ModelAttribute)
					return true;
			}

			// Add Skipped types here
			//switch (type.Namespace) {
			//case "":
			//	return true;
			//}

			return SkipDueToAttribute (type);
		}
	}
}
