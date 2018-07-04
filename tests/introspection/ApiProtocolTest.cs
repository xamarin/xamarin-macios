//
// Test the generated API for common protocol support
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2013, 2015 Xamarin Inc.
//

using System;
using System.Reflection;
using System.Runtime.InteropServices;
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

	public abstract class ApiProtocolTest : ApiBaseTest {

		static IntPtr conform_to = Selector.GetHandle ("conformsToProtocol:");

		public ApiProtocolTest ()
		{
			ContinueOnFailure = true;
		}

		static bool ConformTo (IntPtr klass, IntPtr protocol)
		{
			return bool_objc_msgSend_IntPtr (klass, conform_to, protocol);
		}

		protected virtual bool Skip (Type type)
		{
			switch (type.Name) {
			// *** NSForwarding: warning: object 0x5cbd078 of class 'JSExport' does not implement methodSignatureForSelector: -- trouble ahead
			// *** NSForwarding: warning: object 0x5cbd078 of class 'JSExport' does not implement doesNotRecognizeSelector: -- abort
			case "JSExport":
				return true;
			default:
#if !XAMCORE_2_0
				// in Classic our internal delegates _inherits_ the type with the [Protocol] attribute
				// in Unified our internal delegates _implements_ the interface that has the [Protocol] attribute
				if (type.GetCustomAttribute<ProtocolAttribute> (true) != null)
					return true;
#endif
				return SkipDueToAttribute (type);
			}
		}

		IntPtr GetClass (Type type)
		{
			return Class.GetHandle (type);
		}

		protected virtual bool Skip (Type type, string protocolName)
		{
			// The following protocols are skipped in classic since they were added in 
			// later versions
			if (IntPtr.Size == 4) {
				switch (protocolName) {
				case "UIUserActivityRestoring":
					return true;
				}
			}

			switch (protocolName) {
			case "NSCopying":
				switch (type.Name) {
				// undocumented conformance (up to 7.0) and conformity varies between iOS versions
				case "CAEmitterCell":
				case "GKAchievement":
				case "GKScore":
				case "MPMediaItem":
				// new in iOS8 and 10.0
				case "NSExtensionContext":
				case "NSLayoutAnchor`1":
				case "NSLayoutDimension":
				case "NSLayoutXAxisAnchor":
				case "NSLayoutYAxisAnchor":
				// iOS 10 beta 3
				case "GKCloudPlayer":
				// iOS 10 : test throw because of generic usage
				case "NSMeasurement`1":
				// Xcode 9 - Conformance not in headers
				case "MLDictionaryConstraint":
				case "MLImageConstraint":
				case "MLMultiArrayConstraint":
				case "VSSubscription":
					return true; // skip
				// xcode 10
				case "VSAccountMetadata":
				case "VSAccountMetadataRequest":
				case "VSAccountProviderResponse":
				case "PHEditingExtensionContext":
				case "HKCumulativeQuantitySeriesSample":
					return true;
				}
				break;
			case "NSMutableCopying":
				switch (type.Name) {
				// iOS 10 : test throw because of generic usage
				case "NSMeasurement`1":
					return true; // skip
				// Xcode 10
				case "UNNotificationCategory":
				case "UNNotificationSound":
					return true;
				}
				break;
			case "NSCoding":
				switch (type.Name) {
				// only documented to support NSCopying - not NSCoding (fails on iOS 7.1 but not 8.0)
				case "NSUrlSessionTask":
				case "NSUrlSessionDataTask":
				case "NSUrlSessionUploadTask":
				case "NSUrlSessionDownloadTask":
				// 
				case "NSUrlSessionConfiguration":
				case "NSMergeConflict":
				// new in iOS8 and 10.0
				case "NSExtensionContext":
				case "NSItemProvider":
				// iOS9 / 10.11
				case "CNSaveRequest":
				case "NSLayoutAnchor`1":
				case "NSLayoutDimension":
				case "NSLayoutXAxisAnchor":
				case "NSLayoutYAxisAnchor":
				case "GKCloudPlayer":
				case "GKGameSession":
				// iOS 10 : test throw because of generic usage
				case "NSMeasurement`1":
				// iOS 11 / tvOS 11
				case "VSSubscription":
				// iOS 11.3 / macOS 10.13.4
				case "NSEntityMapping":
				case "NSMappingModel":
				case "NSPropertyMapping":
					return true;
				// Xcode 10
				case "NSManagedObjectID":
				case "VSAccountMetadata":
				case "VSAccountMetadataRequest":
				case "VSAccountProviderResponse":
				case "PHEditingExtensionContext":
					return true;
				}
				break;
			case "NSSecureCoding":
				switch (type.Name) {
				case "NSMergeConflict": // undocumented
				// only documented to support NSCopying (and OSX side only does that)
				case "NSUrlSessionTask":
				case "NSUrlSessionDataTask":
				case "NSUrlSessionUploadTask":
				case "NSUrlSessionDownloadTask":
				case "NSUrlSessionConfiguration":
				// new in iOS8 and 10.0
				case "NSExtensionContext":
				case "NSItemProvider":
				case "NSParagraphStyle": //17770106
				case "NSMutableParagraphStyle": //17770106
					return true; // skip
				// iOS9 / 10.11
				case "CNSaveRequest":
				case "NSPersonNameComponentsFormatter":
				case "GKCloudPlayer":
				case "GKGameSession":
				// iOS 10 : test throw because of generic usage
				case "NSMeasurement`1":
					return true; // skip
				// xcode 9
				case "NSConstraintConflict": // Conformance not in headers
				case "VSSubscription":
				// iOS 11.3 / macOS 10.13.4
				case "NSEntityMapping":
				case "NSMappingModel":
				case "NSPropertyMapping":
					return true;
				case "MPSImageAllocator": // Header shows NSSecureCoding, but intro gives: MPSImageAllocator conforms to NSSecureCoding but SupportsSecureCoding returned false
					return true;
				// Xcode 10
				case "ARDirectionalLightEstimate":
				case "ARFrame":
				case "ARLightEstimate":
				case "NSManagedObjectID":
				// beta 2
				case "NSShadow":
				case "NSTextAttachment":
				case "VSAccountMetadata":
				case "VSAccountMetadataRequest":
				case "VSAccountProviderResponse":
				case "PHEditingExtensionContext":
					return true;
				}
				break;
			// conformance added in Xcode 8 (iOS 10 / macOS 10.12)
			case "MDLNamed":
				switch (type.Name) {
				case "MTKMeshBuffer":
				case "MDLVoxelArray": // base class changed to MDLObject (was NSObject before)
					return true;
				}
				break;
			case "CALayerDelegate":
				switch (type.Name) {
				case "MTKView":
					return true;
				}
				break;
			case "NSProgressReporting":
				if (!TestRuntime.CheckXcodeVersion (9, 0))
					return true;
				break;
			case "GKSceneRootNodeType":
				// it's an empty protocol, defined by a category and does not reply as expected
				switch (type.Name) {
				// GameplayKit.framework/Headers/SceneKit+Additions.h
				case "SCNScene":
				// GameplayKit.framework/Headers/SpriteKit+Additions.h
				case "SKScene":
					return true;
				}
				break;
			case "UIUserActivityRestoring":
				switch (type.Name) {
				// UIKit.framework/Headers/UIDocument.h
				case "UIDocument":
				// inherits it from UIDocument
				case "UIManagedDocument":
					return true;
				}
				break;
			}
			return false;
		}

		void CheckProtocol (string protocolName, Action<Type,IntPtr,bool> action)
		{
			IntPtr protocol = Runtime.GetProtocol (protocolName);
			Assert.AreNotEqual (protocol, IntPtr.Zero, protocolName);

			int n = 0;
			foreach (Type t in Assembly.GetTypes ()) {
				if (!NSObjectType.IsAssignableFrom (t))
					continue;

				if (Skip (t) || Skip (t, protocolName))
					continue;

				if (LogProgress)
					Console.WriteLine ("{0}. {1} conforms to {2}", ++n, t.FullName, protocolName);

				IntPtr klass = GetClass (t);
				action (t, klass, ConformTo (klass, protocol));
			}
		}

		[Test]
		public void Coding ()
		{
			Errors = 0;
			CheckProtocol ("NSCoding", delegate (Type type, IntPtr klass, bool result) {
				if (result) {
					// `type` conforms to (native) NSCoding so...
					if (result) {
						// the type should implements INSCoding
						if (!typeof (INSCoding).IsAssignableFrom (type)) {
							ReportError ("{0} conforms to NSCoding but does not implement INSCoding", type.Name);
						}
						// FIXME: and implement the .ctor(NSCoder)
					}
				}
			});
			Assert.AreEqual (Errors, 0, "{0} types conforms to NSCoding but does not implement INSCoding", Errors);
		}

		// [Test] -> iOS 6.0+ and Mountain Lion (10.8) +
		public virtual void SecureCoding ()
		{
			Errors = 0;
			CheckProtocol ("NSSecureCoding", delegate (Type type, IntPtr klass, bool result) {
				if (result) {
					// the type should implements INSSecureCoding
					if (!typeof (INSSecureCoding).IsAssignableFrom (type)) {
						ReportError ("{0} conforms to NSSecureCoding but does not implement INSSecureCoding", type.Name);
					}
				}
			});
			Assert.AreEqual (Errors, 0, "{0} types conforms to NSSecureCoding but does not implement INSSecureCoding", Errors);
		}

		bool SupportsSecureCoding (Type type)
		{
			Class cls = new Class (type);
			if (!bool_objc_msgSend_IntPtr (cls.Handle, Selector.GetHandle ("respondsToSelector:"), Selector.GetHandle ("supportsSecureCoding")))
				return false;

			return NSSecureCoding.SupportsSecureCoding (type);
		}


		// [Test] -> iOS 6.0+ and Mountain Lion (10.8) +
		public virtual void SupportsSecureCoding ()
		{
			Errors = 0;
			CheckProtocol ("NSSecureCoding", delegate (Type type, IntPtr klass, bool result) {
				bool supports = SupportsSecureCoding (type);
				if (result) {
					// check that +supportsSecureCoding returns YES
					if (!supports) {
						ReportError ("{0} conforms to NSSecureCoding but SupportsSecureCoding returned false", type.Name);
					}
				} else if (type.IsPublic && supports) {
					// there are internal types, e.g. DataWrapper : NSData, that subclass NSSecureCoding-types without
					// [re-]declaring their allegiance - but we can live with those small betrayals
					Assert.IsFalse (NSSecureCoding.SupportsSecureCoding (type), "{0} !SupportsSecureCoding", type.Name);
					ReportError ("SupportsSecureCoding returns true but {0} does not conforms to NSSecureCoding", type.Name);
				}
			});
			Assert.AreEqual (Errors, 0, "{0} types conforms to NSCoding but does not implement INSSecureCoding", Errors);
		}

		[Test]
		public void Copying ()
		{
			Errors = 0;
			CheckProtocol ("NSCopying", delegate (Type type, IntPtr klass, bool result) {
				// `type` conforms to (native) NSCopying so...
				if (result) {
					// the type should implements INSCopying
					if (!typeof (INSCopying).IsAssignableFrom (type)) {
						ReportError ("{0} conforms to NSCopying but does not implement INSCopying", type.Name);
					}
				}
			});
			Assert.AreEqual (Errors, 0, "{0} types conforms to NSCopying but does not implement INSCopying", Errors);
		}

		[Test]
		public void MutableCopying ()
		{
			Errors = 0;
			CheckProtocol ("NSMutableCopying", delegate (Type type, IntPtr klass, bool result) {
				// `type` conforms to (native) NSMutableCopying so...
				if (result) {
					// the type should implements INSMutableCopying
					if (!typeof (INSMutableCopying).IsAssignableFrom (type)) {
						ReportError ("{0} conforms to NSMutableCopying but does not implement INSMutableCopying", type.Name);
					}
				}
			});
			Assert.AreEqual (Errors, 0, "{0} types conforms to NSMutableCopying but does not implement INSMutableCopying", Errors);
		}

		[Test]
		public void GeneralCase ()
		{
			Errors = 0;
			foreach (Type t in Assembly.GetTypes ()) {
				if (!NSObjectType.IsAssignableFrom (t))
					continue;

				if (Skip (t))
					continue;

				foreach (var intf in t.GetInterfaces ()) {
					if (SkipDueToAttribute (intf))
						continue;

					string protocolName = intf.Name.Substring (1);
					switch (protocolName) {
					case "NSCoding":
					case "NSSecureCoding":
					case "NSCopying":
					case "NSMutableCopying":
						// we have special test cases for them
						continue;
					default:
#if !XAMCORE_2_0
						// in Classic our internal delegates _inherits_ the type with the [Protocol] attribute
						// in Unified our internal delegates _implements_ the interface that has the [Protocol] attribute
						var pt = Type.GetType (intf.Namespace + "." + protocolName + ", " + intf.Assembly.FullName);
						if (SkipDueToAttribute (pt))
							continue;
#endif
						if (Skip (t, protocolName))
							continue;
						break;
					}

					var a = intf.GetCustomAttribute<ProtocolAttribute> (true);
					if (a == null || a.IsInformal)
						continue;

					IntPtr protocol = Runtime.GetProtocol (protocolName);
					if (protocol == IntPtr.Zero)
						continue; // not a protocol

					if (LogProgress)
						Console.WriteLine ("{0} conforms to {1}", t.FullName, protocolName);

					var klass = new Class (t);
					if (klass.Handle == IntPtr.Zero) {
						// This can often by caused by [Protocol] classes with no [Model] but having a [BaseType].
						// Either have both a Model and BaseType or neither
						AddErrorLine ("[FAIL] Could not load {0}", t.FullName);
					} else if (t.IsPublic && !ConformTo (klass.Handle, protocol)) {
						// note: some internal types, e.g. like UIAppearance subclasses, return false (and there's not much value in changing this)
						ReportError ("Type {0} (native: {1}) does not conform {2}", t.FullName, klass.Name, protocolName);
					}
				}
			}
			AssertIfErrors ("{0} types do not really conform to the protocol interfaces", Errors);
		}
	}
}
