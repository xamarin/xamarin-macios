using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.IO;

using NUnit.Framework;

using Mono.Cecil;

using Xamarin.Utils;
using Xamarin.Tests;

#nullable enable

namespace Cecil.Tests {

	[TestFixture]
	public class AttributeTest {
		// https://github.com/xamarin/xamarin-macios/issues/10170
		// Every binding class that has net6 availability attributes on a method/property
		// must have one matching every platform listed on the availabilities of the class
		//
		// Example:
		// [SupportedOSPlatform("ios1.0")]
		// [SupportedOSPlatform("maccatalyst1.0")]
		// class TestType
		// {
		//     public static void Original () { }
		//
		//     [SupportedOSPlatform(""ios2.0"")]
		//     public static void Extension () { }
		// }
		//
		// In this example, Extension is _not_ considered part of maccatalyst1.0
		// Because having _any_ SupportedOSPlatform implies only the set explicitly set on that member
		//
		// This test should find Extension, note that it has an ios attribute,
		// and insist that some maccatalyst must also be set.
		[TestCaseSource (typeof (Helper), nameof (Helper.NetPlatformAssemblies))]
		public void ChildElementsListAvailabilityForAllPlatformsOnParent (string assemblyPath)
		{
			var assembly = Helper.GetAssembly (assemblyPath);
			if (assembly is null) {
				Assert.Ignore ("{assemblyPath} could not be found (might be disabled in build)");
				return;
			}

			HashSet<string> found = new HashSet<string> ();
			foreach (var prop in Helper.FilterProperties (assembly, a => HasAnyAvailabilityAttribute (a, includeUnsupported: true))) {
				CheckAllPlatformsOnParent (prop, prop.FullName, prop.DeclaringType, found);
			}
			foreach (var meth in Helper.FilterMethods (assembly, a => HasAnyAvailabilityAttribute (a, includeUnsupported: true))) {
				CheckAllPlatformsOnParent (meth, meth.FullName, meth.DeclaringType, found);
			}
			// XXX - This is triggering due to lack of detection of source code generated types
			// foreach (var field in Helper.FilterFields (assembly, a => HasAnyAvailabilityAttribute (a, includeUnsupported: true))) {
			// 	CheckAllPlatformsOnParent (field, field.FullName, field.DeclaringType, found);
			// }

			Assert.That (found, Is.Empty);
		}

		// Look for classes/struct that have double attributes, either because of partial classes or binding errors
		// Example:
		// TestType.cs
		// [SupportedOSPlatform("maccatalyst1.0")]
		// public partial class TestType { }
		//
		// TestType.g.cs
		// [SupportedOSPlatform("maccatalyst1.0")]
		// public partial class TestType { }
		// 
		// Example #2:
		// [Watch (5,0), NoTV, NoMac, iOS (12,0), NoTV]
		// interface Type { }
		[TestCaseSource (typeof (Helper), nameof (Helper.NetPlatformAssemblies))]
		public void DoubleAttributedElements (string assemblyPath)
		{
			var assembly = Helper.GetAssembly (assemblyPath);
			if (assembly is null) {
				Assert.Ignore ("{assemblyPath} could not be found (might be disabled in build)");
				return;
			}

			var doubleAttributed = new List<string>();
			foreach (var type in Helper.FilterTypes (assembly, a => HasAnyAvailabilityAttribute (a, includeUnsupported: true))) {
				var platformCount = new Dictionary<string, int> ();
				foreach (var attribute in type.CustomAttributes.Where (a => IsAvailabilityAttribute (a, includeUnsupported: true))) {
					var kind = FindAvailabilityKind (attribute);
					if (kind is not null) {
						string key = $"{attribute.AttributeType.Name}-{kind}";						
						if (platformCount.ContainsKey (key)) {
							platformCount[key] += 1;
						}
						else {
							platformCount[key] = 1;
						}
					}
				}
				foreach (var (kind, count) in platformCount) {
					if (count != 1) {
						doubleAttributed.Add ($"{kind} on {type} had a count of {count}");
#if DEBUG
						Console.Error.WriteLine ($"{kind} on {type} had a count of {count}");
#endif
					}
				}
			}
			Assert.That (doubleAttributed, Is.Empty);
		}

		void CheckAllPlatformsOnParent (ICustomAttributeProvider item, string fullName, TypeDefinition parent, HashSet<string> found)
		{
			// XXX - For now skip generated code until associated generator.cs changes are in
			if (Ignore (fullName) || HasCodegenAttribute (item)) {
				return;
			}
// #if DEBUG
// 			const string Filter = "AppKit";
// 			if (!fullName.Contains (" " + Filter)) {
// 				return;
// 			}
// #endif

			var parentAvailability = GetAvailabilityAttributes (parent, includeUnsupported: false).ToList ();
// 			// This is true in theory, but our code should be explicit and list every platform individually
// 			// This can be re-enabled if that decision is reverted.

// 			// iOS implies maccatalyst, but only for parent scope
// 			if (parentAvailability.Contains("ios") && !parentAvailability.Contains("maccatalyst")) {
// 				parentAvailability.Append("maccatalyst");
// 			}

			var myAvailability = GetAvailabilityAttributes (item, includeUnsupported: true);
			if (!FirstContainsAllOfSecond (myAvailability, parentAvailability)) {
				DebugPrint (fullName, parentAvailability, myAvailability);
				found.Add (fullName);
			}
		}

		// https://github.com/xamarin/xamarin-macios/issues/10170
		// Every binding class that has net6 any availability attributes on a method/property
		// must have an introduced for the current platform.
		//
		// Example:
		// class TestType
		// {
		//     public static void Original () { }
		//
		//     [SupportedOSPlatform(""ios2.0"")]
		//     public static void Extension () { }
		// }
		//
		// When run against mac, this fails as Extension does not include a mac supported of any kind attribute
		// XXX - This test is disabled until generator work is complete, as there are too many false positives
		// [TestCaseSource (typeof (Helper), "NetPlatformAssemblies")]
		public void AllAttributedItemsMustIncludeCurrentPlatform (string assemblyPath)
		{
			var assembly = Helper.GetAssembly (assemblyPath);
			if (assembly is null) {
				Assert.Ignore ("{assemblyPath} could not be found (might be disabled in build)");
				return;
			}

#if DEBUG
			Console.WriteLine(assemblyPath);
#endif

			string platformName = AssemblyToAttributeName (assemblyPath);

			HashSet<string> found = new HashSet<string> ();
			foreach (var type in Helper.FilterTypes (assembly, a => HasAnyAvailabilityAttribute (a, includeUnsupported: true))) {
				CheckCurrentPlatformIncludedIfAny (type, platformName, type.FullName, type.DeclaringType, found);
			}
			foreach (var prop in Helper.FilterProperties (assembly, a => HasAnyAvailabilityAttribute (a, includeUnsupported: true))) {
				CheckCurrentPlatformIncludedIfAny (prop, platformName, prop.FullName, prop.DeclaringType, found);
			}
			foreach (var meth in Helper.FilterMethods (assembly, a => HasAnyAvailabilityAttribute (a, includeUnsupported: true))) {
				CheckCurrentPlatformIncludedIfAny (meth, platformName, meth.FullName, meth.DeclaringType, found);
			}
			foreach (var field in Helper.FilterFields (assembly, a => HasAnyAvailabilityAttribute (a, includeUnsupported: true))) {
				CheckCurrentPlatformIncludedIfAny (field, platformName, field.FullName, field.DeclaringType, found);
			}

			Assert.That (found, Is.Empty);
		}

		void CheckCurrentPlatformIncludedIfAny (ICustomAttributeProvider item, string platformName, string fullName, TypeDefinition parent, HashSet<string> found)
		{
			if (Ignore (fullName)) {
				return;
			}

			if (HasAnyAvailabilityAttribute (item, true)) {
				var supportedAttributes = item.CustomAttributes.Where (a => IsSupportedAttribute (a));
				if (!supportedAttributes.Any (a => FindAvailabilityKind (a) == platformName)) {
#if DEBUG
					Console.WriteLine (fullName);
#endif
					found.Add (fullName);
				}
			}
		}

		string AssemblyToAttributeName (string assemblyPath)
		{
			var baseName = Path.GetFileName (assemblyPath);
			if (Configuration.GetBaseLibraryName (TargetFramework.DotNet_iOS.Platform, true) == baseName)
				return "ios";
			if (Configuration.GetBaseLibraryName (TargetFramework.DotNet_tvOS.Platform, true) == baseName)
				return "tvos";
			if (Configuration.GetBaseLibraryName (TargetFramework.DotNet_macOS.Platform, true) == baseName)
				return "macos";
			if (Configuration.GetBaseLibraryName (TargetFramework.DotNet_MacCatalyst.Platform, true) == baseName)
				return "maccatalyst";
			throw new NotImplementedException ();
		}

		[Conditional ("DEBUG")]
		void DebugPrint (string fullName, IEnumerable<string> parentAvailability, IEnumerable<string> myAvailability)
		{
			Console.WriteLine (fullName);
			Console.WriteLine ("Parent: " + string.Join (" ", parentAvailability));
			Console.WriteLine ("Mine: " + string.Join (" ", myAvailability));
			Console.WriteLine ();
		}

		bool Ignore (string fullName)
		{
			string ns;
			if (fullName.Split (' ').Length < 2) {
				ns = fullName.Split ('.').First ();
			}
			else {
				// Skip the return type, then split namespace off
				ns = fullName.Split (' ') [1].Split ('.').First ();
			}
			switch (ns) {
			case "Security":
			case "CoreMedia":
			case "CoreText":
				return true;
			}
			switch (fullName) {
			// Generated code DictionaryContainer
			case "System.String StoreKit.StoreProductParameters::ProductIdentifier()":
			case "System.Nullable`1<System.Boolean> AVFoundation.AVAssetDownloadOptions::MediaSelectionPrefersMultichannel()":
			case "System.Nullable`1<System.Boolean> AVFoundation.AVAssetDownloadOptions::PrefersHdr()":
			case "System.Nullable`1<CoreGraphics.CGSize> AVFoundation.AVAssetDownloadOptions::MinimumRequiredPresentationSize()":
			case "Foundation.NSData VideoToolbox.VTCompressionProperties::ContentLightLevelInfo()":
			case "Foundation.NSData VideoToolbox.VTCompressionProperties::MasteringDisplayColorVolume()":
			case "Foundation.NSUuid StoreKit.StoreProductParameters::AdNetworkNonce()":
			case "System.Nullable`1<System.Boolean> CoreImage.CIImageInitializationOptions::ApplyOrientationProperty()":
			case "System.Nullable`1<System.Boolean> CoreImage.CIImageInitializationOptions::AuxiliaryDepth()":
			case "System.Nullable`1<System.Boolean> CoreImage.CIImageInitializationOptions::AuxiliaryDisparity()":
			case "System.Nullable`1<System.Boolean> CoreImage.CIImageInitializationOptions::AuxiliaryPortraitEffectsMatte()":
			case "System.Nullable`1<System.Boolean> CoreImage.CIImageInitializationOptions::AuxiliarySemanticSegmentationGlassesMatte()":
			case "System.Nullable`1<System.Boolean> CoreImage.CIImageInitializationOptions::AuxiliarySemanticSegmentationHairMatte()":
			case "System.Nullable`1<System.Boolean> CoreImage.CIImageInitializationOptions::AuxiliarySemanticSegmentationSkinMatte()":
			case "System.Nullable`1<System.Boolean> CoreImage.CIImageInitializationOptions::AuxiliarySemanticSegmentationTeethMatte()":
			case "System.Nullable`1<System.Boolean> CoreImage.CIImageInitializationOptions::NearestSampling()":
			case "System.Nullable`1<System.Boolean> CoreImage.CIImageInitializationOptions::ToneMapHdrToSdr()":
			case "System.Nullable`1<System.Boolean> SceneKit.SCNSceneLoadingOptions::ConvertToYUp()":
			case "System.Nullable`1<System.Boolean> VideoToolbox.VTCompressionProperties::AllowOpenGop()":
			case "System.Nullable`1<System.Boolean> VideoToolbox.VTCompressionProperties::MaximizePowerEfficiency()":
			case "System.Nullable`1<System.Boolean> VideoToolbox.VTDecompressionProperties::MaximizePowerEfficiency()":
			case "System.Nullable`1<System.Boolean> VideoToolbox.VTPixelTransferProperties::RealTime()":
			case "System.Nullable`1<System.Double> VideoToolbox.VTCompressionProperties::BaseLayerFrameRate()":
			case "System.Nullable`1<System.Double> VideoToolbox.VTCompressionProperties::GammaLevel()":
			case "System.Nullable`1<System.Int32> VideoToolbox.VTDecompressionProperties::TemporalLevelLimit()":
			case "System.Nullable`1<System.Single> SceneKit.SCNSceneLoadingOptions::ConvertUnitsToMeters()":
			case "System.Nullable`1<System.Single> VideoToolbox.VTCompressionProperties::TargetQualityForAlpha()":
			case "System.Nullable`1<System.UInt32> StoreKit.StoreProductParameters::AdNetworkCampaignIdentifier()":
			case "System.Nullable`1<System.UInt32> StoreKit.StoreProductParameters::AdNetworkTimestamp()":
			case "System.Nullable`1<System.UInt32> VideoToolbox.VTCompressionProperties::UsingGpuRegistryId()":
			case "System.Nullable`1<System.UInt32> VideoToolbox.VTDecompressionProperties::UsingGpuRegistryId()":
			case "System.String StoreKit.StoreProductParameters::AdNetworkAttributionSignature()":
			case "System.String StoreKit.StoreProductParameters::AdNetworkIdentifier()":
			case "System.String StoreKit.StoreProductParameters::AdNetworkSourceAppStoreIdentifier()":
			case "System.String StoreKit.StoreProductParameters::AdNetworkVersion()":
			case "System.String VideoToolbox.VTCompressionProperties::EncoderId()":
				return true;
			default:
				return false;
			}
		}

		bool FirstContainsAllOfSecond<T> (IEnumerable<T> first, IEnumerable<T> second)
		{
			var firstSet = new HashSet<T> (first);
			return second.All (s => firstSet.Contains (s));
		}

		IEnumerable<string> GetAvailabilityAttributes (ICustomAttributeProvider provider, bool includeUnsupported) => GetAvailabilityAttributes (provider.CustomAttributes, includeUnsupported);

		IEnumerable<string> GetAvailabilityAttributes (IEnumerable<CustomAttribute> attributes, bool includeUnsupported)
		{
			var availability = new List<string> ();
			foreach (var attribute in attributes.Where (a => IsAvailabilityAttribute (a, includeUnsupported))) {
				var kind = FindAvailabilityKind (attribute);
				if (kind is not null) {
					availability.Add (kind);
				}
			}
			return availability;
		}

		bool HasCodegenAttribute (ICustomAttributeProvider provider)
		{
			// get/set don't have BindingImpl directly, it is on the parent context
			if (provider is MethodDefinition method) {
				var property = method.DeclaringType.Properties.FirstOrDefault (v => method.Name.Length > 4 && v.Name == method.Name.Substring (4));
				if (property != null && property.CustomAttributes.Any (HasGeneratedCodeAttribute)) {
					return true;
				}
			}
			return provider.CustomAttributes.Any (HasGeneratedCodeAttribute);
		}

		string? FindAvailabilityKind (CustomAttribute attribute)
		{
			if (attribute.ConstructorArguments.Count == 1 && attribute.ConstructorArguments [0].Type.Name == "String") {
				string full = (string) attribute.ConstructorArguments [0].Value;
				switch (full) {
				case string s when full.StartsWith ("ios", StringComparison.Ordinal):
					return "ios";
				case string s when full.StartsWith ("tvos"):
					return "tvos";
				case string s when full.StartsWith ("macos"):
					return "macos";
				case string s when full.StartsWith ("maccatalyst"):
					return "maccatalyst";
				case string s when full.StartsWith ("watchos"):
					return null; // WatchOS is ignored for comparision
				default:
					throw new System.NotImplementedException ($"Unknown platform kind: {full}");
				}
			}
			return null;
		}

		bool HasAnyAvailabilityAttribute (ICustomAttributeProvider provider, bool includeUnsupported)
		{
			return provider.CustomAttributes.Any (a => IsAvailabilityAttribute (a, includeUnsupported));
		}

		bool IsAvailabilityAttribute (CustomAttribute attribute, bool includeUnsupported)
		{
			return IsSupportedAttribute (attribute) ||
				(includeUnsupported && attribute.AttributeType.Name == "UnsupportedOSPlatformAttribute");
		}

		bool IsSupportedAttribute (CustomAttribute attribute) => attribute.AttributeType.Name == "SupportedOSPlatformAttribute";
		bool HasGeneratedCodeAttribute (CustomAttribute attribute)
		{
			// There is no one attribute that marks code as generate touched
			// But these are a good coverage, and rarely if ever used 
			// in hand-written code. This is all temporary anyway.
			return attribute.AttributeType.Name == "BindingImplAttribute" ||
				attribute.AttributeType.Name == "FieldAttribute";
		}
	}
}
