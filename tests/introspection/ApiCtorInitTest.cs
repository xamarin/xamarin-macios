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
			
			// skip delegate (and other protocol references)
			foreach (object ca in type.GetCustomAttributes (false)) {
				if (ca is ProtocolAttribute)
					return true;
				if (ca is ModelAttribute)
					return true;
			}

			switch (type.Name) {
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
#if __TVOS__
			case "UISearchBar":
				// - (nullable instancetype)initWithCoder:(NSCoder *)aDecoder NS_DESIGNATED_INITIALIZER __TVOS_PROHIBITED;
				return true;
#endif
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
	}
}
