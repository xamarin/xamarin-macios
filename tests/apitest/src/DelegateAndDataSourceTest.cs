using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using System.Runtime.CompilerServices;

using Foundation;
using AppKit;
using ObjCRuntime;

namespace Xamarin.Mac.Tests
{
	static class TypeExtension {
		public static PropertyInfo GetMostDerivedProperty (this Type t, string name)
		{
			while (t != null && t != t.BaseType) {
				var rv = t.GetProperty (name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
				if (rv != null)
					return rv;
				t = t.BaseType;
			}
			return null;
		}
	}

	[TestFixture]
	public class DelegateAndDataSourceTest
	{
		[Test]
		public void DelegateAndDataSourceAllowsNull ()
		{
			var failingTypes = new Dictionary<Type, string> ();

			// Get our binding assembly
			var xamMac = typeof (NSObject).Assembly;

			// Walk all non abstract types, looking for things with zero param constructors
			foreach (Type t in xamMac.GetTypes ().Where (t => !t.IsAbstract)) {
				// Check availability attributes.
				if (Asserts.SkipDueToAvailabilityAttribute (t) || Skip (t))
					continue;

				var ctor = t.GetConstructor (BindingFlags.Instance | BindingFlags.Public, null, new Type[0], null);
				if (Asserts.SkipDueToAvailabilityAttribute (ctor))
					continue;

				// If they have one of the properites we are testing
				if (ctor != null) {
					PropertyInfo weakDelegate = t.GetMostDerivedProperty("WeakDelegate");
					PropertyInfo del = t.GetMostDerivedProperty("Delegate");
					PropertyInfo weakDataSource = t.GetMostDerivedProperty("WeakDataSource");
					PropertyInfo dataSource = t.GetMostDerivedProperty("DataSource");
					if (isValidToTest (weakDelegate) || isValidToTest (del) ||
					isValidToTest (weakDataSource)  || isValidToTest (dataSource) ) {
						try {
							// Create an instance and try to set null
							using (var instance = (IDisposable) ctor.Invoke (null)) {
								if (isValidToTest (weakDelegate)) {
									weakDelegate.SetValue (instance, null, null);
								}
								if (isValidToTest (del)) {
									del.SetValue (instance, null, null);
								}
								if (isValidToTest (weakDataSource)) {
									weakDataSource.SetValue (instance, null, null);
								}
								if (isValidToTest (dataSource)) {
									dataSource.SetValue (instance, null, null);
								}
							}
						}
						catch (TargetInvocationException e) {
							failingTypes.Add (t, e.InnerException.Message);
						}
						catch (Exception e) {
							Assert.Fail ("Unexpected exception {0} while testing {1}", e, t);
						}
					}
				}
			}

			GC.Collect (2); // Flush out random failures. Some classes only act badly when disposed
			if (failingTypes.Count > 0) {
				Console.WriteLine ("{0} failing types:", failingTypes.Count);
				foreach (var kvp in failingTypes)
					Console.WriteLine ("{0}: {1}", kvp.Key, kvp.Value);
				Assert.Fail ("{0} failing types", failingTypes.Count);
			}
		}

		bool Skip (Type t)
		{
			switch (t.Name) {
			case "IKPictureTaker": // radar://29311598
			case "AVAssetResourceLoader":
			case "AVAssetResourceLoadingRequest":
			case "AVAssetResourceLoadingContentInformationRequest":
			case "SCNRenderer":
			case "NSStream":
			case "NSSharingServicePicker":
			case "NSCache":
			case "NSToolbar":
			case "NSComboBox":
			case "NSComboBoxCell":
			case "IKScannerDeviceView":
			case "NSUserActivity":
			case "NSFontPanel":
			case "AVAudioRecorder":
			case "MKMapView":
			case "SKScene":
			case "NSSpeechRecognizer":
			case "NSClickGestureRecognizer":
			case "NSPopover":
				// These classes don't do well when you instance them without support
				return true;
			case "SCNLayer":
			case "SCNProgram":
				if (Asserts.IsAtLeastElCapitan && IntPtr.Size == 4)
					return true;
				break;
			case "CBCentralManager":
				if (IntPtr.Size == 4 && PlatformHelper.CheckSystemVersion(10, 13)) // Removed from 32-bit in macOS 10.13
					return true;
				break;
			case "QTMovie":
				return TestRuntime.CheckSystemVersion (PlatformName.MacOSX, 10, 14, 4); // Broke in macOS 10.14.4.
			case "AVCaptureView":
				// Deallocating a AVCaptureView makes it trigger a permission dialog, which we don't want, so just skip this type.
				return true;
			}

			switch (t.Namespace) {
			case "QTKit":
				return TestRuntime.CheckSystemVersion (PlatformName.MacOSX, 10, 15); // QTKit is gone in 10.15.
			}

			return false;
		}

//		Based on bug 28505 - NSTabView wasn't holding a reference to the Delegate property under new ref count.
//		An ArgumentSemantic (Strong, Retain, etc) is required to keep the reference around so the app doesn't crash after a GC.
// 		This test scans all bindings looking for instances where bindings don't have the correct ArgumentSemantic
		[Test]
		public void DelegateAndDataSourceHaveArgumentSemanticAttribute ()
		{
			var failingTypes = new Dictionary<Type, string> ();

			// Get our binding assembly
			var xamMac = typeof (NSObject).Assembly;

			foreach (Type t in xamMac.GetTypes ().Where (t => !t.IsAbstract)) {
				// Check availability attributes.
				if (Asserts.SkipDueToAvailabilityAttribute (t))
					continue;

				PropertyInfo weakDelegate = t.GetMostDerivedProperty ("WeakDelegate");
				PropertyInfo del = t.GetMostDerivedProperty ("Delegate");

				MethodInfo[] accessors = null;

				if (del != null) {
					if (weakDelegate != null) {
						if (!weakDelegate.CanWrite)
							continue;
						
						accessors = weakDelegate.GetAccessors ();
					} else {
						if (!del.CanWrite)
							continue;
						
						accessors = del.GetAccessors ();
					}

					foreach (var accessor in accessors) {
						var attr = accessor.GetCustomAttributes <ExportAttribute> ().FirstOrDefault (a => a.Selector == "delegate");
						if (attr == null)
							continue;

						if (attr.ArgumentSemantic == ArgumentSemantic.None) {
							failingTypes.Add (t, "Delegate has no ArgumentSemantic set");
							break;
						}
					}
				}

				PropertyInfo weakDataSource = t.GetMostDerivedProperty ("WeakDataSource");
				PropertyInfo dataSource = t.GetMostDerivedProperty ("DataSource");

				if (dataSource != null) {
					accessors = null;
					if (weakDataSource != null) {
						if (!weakDataSource.CanWrite)
							continue;
						
						accessors = weakDataSource.GetAccessors ();
					} else {
						if (!dataSource.CanWrite)
							continue;
						
						accessors = dataSource.GetAccessors ();
					}
					
					foreach (var accessor in accessors) {
						var attr = accessor.GetCustomAttributes <ExportAttribute> ().FirstOrDefault (a => a.Selector == "dataSource");
						if (attr == null)
							continue;
						
						if (attr.ArgumentSemantic == ArgumentSemantic.None) {
							failingTypes.Add (t, "Data Source has no ArgumentSemantic set");
							break;
						}
					}
				}
			}

			if (failingTypes.Count > 0) {
				Console.WriteLine ("{0} failing types:", failingTypes.Count);
				foreach (var kvp in failingTypes)
					Console.WriteLine ("{0}: {1}", kvp.Key, kvp.Value);
				Assert.Fail ("{0} failing types", failingTypes.Count);
			}
		}

		[Test]
		public void TargetArgumentSemanticAttribute ()
		{
			var failingTypes = new Dictionary<Type, string> ();

			// Get our binding assembly
			var xamMac = typeof (NSObject).Assembly;

			foreach (Type t in xamMac.GetTypes ().Where (t => !t.IsAbstract)) {
				// Check availability attributes.
				if (Asserts.SkipDueToAvailabilityAttribute (t))
					continue;

				PropertyInfo target = t.GetMostDerivedProperty ("Target");
				if (target != null && target.PropertyType == typeof (NSObject)) {
					MethodInfo[] accessors = target.GetAccessors ();
					foreach (var accessor in accessors) {
						var attr = accessor.GetCustomAttributes <ExportAttribute> ().FirstOrDefault (a => a.Selector == "target");
						if (attr == null)
							continue;

						if (attr.ArgumentSemantic == ArgumentSemantic.None) {
							failingTypes.Add (t, "Target has no ArgumentSemantic set");
							break;
						}
					}
				}
			}

			if (failingTypes.Count > 0) {
				Console.WriteLine ("{0} failing types:", failingTypes.Count);
				foreach (var kvp in failingTypes)
					Console.WriteLine ("{0}: {1}", kvp.Key, kvp.Value);
				Assert.Fail ("{0} failing types", failingTypes.Count);
			}
		}
			
		bool isValidToTest (PropertyInfo p)
		{
			return p != null && p.CanWrite && !Asserts.SkipDueToAvailabilityAttribute (p);
		}
	}
}

