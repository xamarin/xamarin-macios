using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.Text;

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
		[TestCaseSource (typeof (Helper), nameof (Helper.NetPlatformAssemblyDefinitions))]
		public void ChildElementsListAvailabilityForAllPlatformsOnParent (AssemblyInfo info)
		{
			var assembly = info.Assembly;

			HashSet<string> found = new HashSet<string> ();
			foreach (var prop in assembly.EnumerateProperties (a => HasAnyAvailabilityAttribute (a))) {
				CheckAllPlatformsOnParent (prop, prop.FullName, prop.DeclaringType, found);
			}
			foreach (var meth in assembly.EnumerateMethods (a => HasAnyAvailabilityAttribute (a))) {
				CheckAllPlatformsOnParent (meth, meth.FullName, meth.DeclaringType, found);
			}
			foreach (var field in assembly.EnumerateFields (a => HasAnyAvailabilityAttribute (a))) {
				CheckAllPlatformsOnParent (field, field.FullName, field.DeclaringType, found);
			}
			Assert.That (found, Is.Empty, $"{found.Count} issues found");
		}

		bool IgnoreAllPlatformsOnParent (string fullName)
		{
			switch (fullName) {
			// Generator Bug - Obsolete does not imply introduced but prevents unlisted from created one
			case "AudioUnit.AudioUnitParameterType AudioUnit.AudioUnitParameterType::AUDCFilterDecayTime":
			case "AppKit.NSBoxType AppKit.NSBoxType::NSBoxSecondary":
			case "AppKit.NSBoxType AppKit.NSBoxType::NSBoxOldStyle":
				return true;
			default:
				return false;
			}
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
		[TestCaseSource (typeof (Helper), nameof (Helper.NetPlatformAssemblyDefinitions))]
		public void DoubleAttributedElements (AssemblyInfo info)
		{
			var assembly = info.Assembly;
			var doubleAttributed = new List<string> ();
			foreach (var type in assembly.EnumerateTypes (a => HasAnyAvailabilityAttribute (a))) {
				var platformCount = new Dictionary<string, int> ();
				foreach (var attribute in type.CustomAttributes.Where (a => IsAvailabilityAttribute (a))) {
					var kind = FindAvailabilityKind (attribute);
					if (kind is not null) {
						string key = $"{attribute.AttributeType.Name}-{kind}";
						if (platformCount.ContainsKey (key)) {
							platformCount [key] += 1;
						} else {
							platformCount [key] = 1;
						}
					}
				}
				foreach (var (kind, count) in platformCount) {
					// AVFoundation.AVMetadataIdentifiers uses an old pattern of a parent
					// class and many child classes with constants.
					if (type.ToString () == "AVFoundation.AVMetadataIdentifiers") {
						continue;
					}
					if (count != 1) {
						doubleAttributed.Add ($"{kind} on {type} had a count of {count}");
#if DEBUG
						Console.Error.WriteLine ($"{kind} on {type} had a count of {count}");
#endif
					}
				}
			}
			Assert.That (doubleAttributed, Is.Empty, $"{doubleAttributed.Count} issues found");
		}

		void CheckAllPlatformsOnParent (ICustomAttributeProvider item, string fullName, TypeDefinition parent, HashSet<string> found)
		{
			if (IgnoreAllPlatformsOnParent (fullName)) {
				return;
			}

			var parentAvailability = GetAvailabilityAttributes (parent).ToList ();

			var myAvailability = GetAvailabilityAttributes (item);
			if (!FirstContainsAllOfSecond (myAvailability, parentAvailability)) {
				DebugPrint (fullName, parentAvailability, myAvailability);
				found.Add (fullName);
			}
		}

		public class PlatformClaimInfo {
			public HashSet<string> MentionedPlatforms { get; set; } // Mentioned in both Supported and Unsupported contexts
			public HashSet<string> ClaimedPlatforms { get; set; } // Mentioned only in Supported contexts
			public IMemberDefinition Member { get; set; }

			public PlatformClaimInfo (List<string> mentionedPlatforms, List<string> claimedPlatforms, IMemberDefinition member)
			{
				MentionedPlatforms = new HashSet<string> (mentionedPlatforms);
				ClaimedPlatforms = new HashSet<string> (claimedPlatforms);
				Member = member;
			}

			public void UnionWith (PlatformClaimInfo other)
			{
				MentionedPlatforms.UnionWith (other.MentionedPlatforms);
				ClaimedPlatforms.UnionWith (other.ClaimedPlatforms);
			}

			public override string ToString ()
			{
				var str = new StringBuilder ();
				str.Append ($"Mentioned: ({String.Join (", ", MentionedPlatforms)})");
				str.Append ($" Claimed: ({String.Join (", ", ClaimedPlatforms)})");
				return str.ToString ();
			}
		}

		// Due to binding and conversion mistakes, sometimes attributes like `[SupportedOSPlatform ("macos")]`
		// might be documented on elements that are removed from the build with conditional compilation
		// This test detects these by loading every NET platform assembly, and
		// comparing reality (what types/members actually exist) with the attributes
		// We can not do this against a single assembly at a time, as the attributes saying "this is supported on mac" aren't 
		// in the mac assembly, that's the bug. 
		[Test]
		public void FindSupportedOnElementsThatDoNotExistInThatAssembly ()
		{
			Configuration.IgnoreIfAnyIgnoredPlatforms ();

			// Dictionary of (FullName of Member) -> (Dictionary of (Actual Platform) -> Platform Claim Info)
			var harvestedInfo = new Dictionary<string, Dictionary<string, PlatformClaimInfo>> ();

			// Load each platform assembly
			foreach (var info in Helper.NetPlatformAssemblyDefinitions) {
				var assembly = info.Assembly;
				string currentPlatform = AssemblyToAttributeName (assembly);

				// Walk every class/struct/enum/property/method/enum value/pinvoke/event
				foreach (var module in assembly.Modules) {
					foreach (var type in module.Types) {
						switch (type.Namespace) {
						case "AppKit":
						case "UIKit":
							// The availability attributes between AppKit and UIKit are quite inconsistent:
							// https://github.com/xamarin/xamarin-macios/issues/17292
							// So let's just skip these two namespaces for now.
							continue;
						}
						foreach (var member in GetAllTypeMembers (type)) {
							// If a member is hidden, it's probably because it's broken in some way, so don't consider it.
							if (ObsoleteTest.HasEditorBrowseableNeverAttribute (member))
								continue;

							var mentionedPlatforms = GetAvailabilityAttributes (member).ToList ();
							if (mentionedPlatforms.Any ()) {
								var claimedPlatforms = GetSupportedAvailabilityAttributes (member).ToList ();
								string key = GetMemberLookupKey (member);
								if (!harvestedInfo.ContainsKey (key)) {
									harvestedInfo [key] = new Dictionary<string, PlatformClaimInfo> ();
								}
								var claimInfo = new PlatformClaimInfo (mentionedPlatforms, claimedPlatforms, member);
								if (harvestedInfo [key].TryGetValue (currentPlatform, out var existingClaim))
									throw new InvalidOperationException ($"The key {key} was computed for two different members:\n\tMember 1: {existingClaim.Member.FullName}\n\tMember 2: {member.FullName}\n\tKey: {key}");
								harvestedInfo [key] [currentPlatform] = claimInfo;
							}
						}
					}
				}
			}

			// Now walk every item found above and check two things:
			var failures = new Dictionary<string, string> ();
			foreach (var (member, info) in harvestedInfo) {
				// 1. All platforms match in count of mentioned (we did not conditionally compile out attributes)
				int expectedPlatformCount = info.First ().Value.MentionedPlatforms.Count ();
				if (info.Any (i => i.Value.MentionedPlatforms.Count () != expectedPlatformCount)) {
					var detailedPlatformBreakdown = string.Join ("\n\t", info.Select (x => ($"Assembly {x.Key} => {x.Value}")));
					var errorMessage = $"{member} did not have the same number of SupportedOSPlatformAttribute in every assembly:\n\t{detailedPlatformBreakdown}";
					if (failures.TryGetValue (member, out var existingFailure)) {
						failures [member] = existingFailure + "\n" + errorMessage;
					} else {
						failures [member] = errorMessage;
					}
				}

				// 2. For each supported attribute claim exist, that it exists on that platform
				// Since we know each platform claims are now equal, just use the first one
				var claimedPlatforms = info.First ().Value.ClaimedPlatforms;
				foreach (var platform in claimedPlatforms) {
					if (!info.ContainsKey (platform)) {
						var detailedPlatformBreakdown = string.Join ("\n\t", info.Select (x => ($"Assembly {x.Key} => Declares ({string.Join (" ", x.Value)})")));
						var errorMessage = $"{member} was not found on {platform} despite being declared supported there:\n\t{detailedPlatformBreakdown}";
						if (failures.TryGetValue (member, out var existingFailure)) {
							failures [member] = existingFailure + "\n" + errorMessage;
						} else {
							failures [member] = errorMessage;
						}
					}
				}
			}

			Helper.AssertFailures (failures, IgnoreElementsThatDoNotExistInThatAssembly, nameof (IgnoreElementsThatDoNotExistInThatAssembly), "Supported inconsistencies");
		}

		static HashSet<string> IgnoreElementsThatDoNotExistInThatAssembly {
			get {
				return new HashSet<string> {
					// This is from the NSItemProviderWriting protocol: NSUserActivity does not implement NSItemProviderWriting on tvOS and macOS.
					"Foundation.NSUserActivity.GetItemProviderVisibilityForTypeIdentifier (System.String)",
					"Foundation.NSUserActivity.LoadData (System.String, System.Action`2<Foundation.NSData, Foundation.NSError>)",
					"Foundation.NSUserActivity.LoadDataAsync (System.String)",
					"Foundation.NSUserActivity.LoadDataAsync (System.String, Foundation.NSProgress&)",
					"Foundation.NSUserActivity.WritableTypeIdentifiers",
					"Foundation.NSUserActivity.WritableTypeIdentifiersForItemProvider",
					"Foundation.NSUserActivity.get_WritableTypeIdentifiers ()",
					"Foundation.NSUserActivity.get_WritableTypeIdentifiersForItemProvider ()",

					// This is from the NSItemProviderReading protocol: NSUserActivity does not implement NSItemProviderReading on tvOS and macOS.
					"Foundation.NSUserActivity.GetObject (Foundation.NSData, System.String, Foundation.NSError&)",
					"Foundation.NSUserActivity.ReadableTypeIdentifiers",
					"Foundation.NSUserActivity.get_ReadableTypeIdentifiers ()",

					// This is from the NSItemProviderWriting protocol: MKMapItem does not implement NSItemProviderWriting on tvOS and macOS.
					"MapKit.MKMapItem.GetItemProviderVisibilityForTypeIdentifier (System.String)",
					"MapKit.MKMapItem.LoadData (System.String, System.Action`2<Foundation.NSData, Foundation.NSError>)",
					"MapKit.MKMapItem.LoadDataAsync (System.String)",
					"MapKit.MKMapItem.LoadDataAsync (System.String, Foundation.NSProgress&)",
					"MapKit.MKMapItem.WritableTypeIdentifiers",
					"MapKit.MKMapItem.WritableTypeIdentifiersForItemProvider",
					"MapKit.MKMapItem.get_WritableTypeIdentifiers ()",
					"MapKit.MKMapItem.get_WritableTypeIdentifiersForItemProvider ()",

					// This is from the NSItemProviderReading protocol: MKMapItem does not implement NSItemProviderReading on tvOS and macOS.
					"MapKit.MKMapItem.GetObject (Foundation.NSData, System.String, Foundation.NSError&)",
					"MapKit.MKMapItem.ReadableTypeIdentifiers",
					"MapKit.MKMapItem.get_ReadableTypeIdentifiers ()",

					// This is from the NSItemProviderReading protocol: PHLivePhoto does not implement NSItemProviderReading on tvOS and macOS.
					"Photos.PHLivePhoto.GetObject (Foundation.NSData, System.String, Foundation.NSError&)",
					"Photos.PHLivePhoto.ReadableTypeIdentifiers",
					"Photos.PHLivePhoto.get_ReadableTypeIdentifiers ()",


					// This is from the NSSecureCoding protocol: SKView only implements NSSecureCoding on macOS.
					"SpriteKit.SKView.EncodeTo (Foundation.NSCoder)",

					// These methods have different optional/required semantics between platforms.
					"Metal.IMTLBlitCommandEncoder.GetTextureAccessCounters (Metal.IMTLTexture, Metal.MTLRegion, System.UIntPtr, System.UIntPtr, System.Boolean, Metal.IMTLBuffer, System.UIntPtr)",
					"Metal.IMTLBlitCommandEncoder.ResetTextureAccessCounters (Metal.IMTLTexture, Metal.MTLRegion, System.UIntPtr, System.UIntPtr)",
					"Metal.MTLBlitCommandEncoder_Extensions.GetTextureAccessCounters (Metal.IMTLBlitCommandEncoder, Metal.IMTLTexture, Metal.MTLRegion, System.UIntPtr, System.UIntPtr, System.Boolean, Metal.IMTLBuffer, System.UIntPtr)",
					"Metal.MTLBlitCommandEncoder_Extensions.ResetTextureAccessCounters (Metal.IMTLBlitCommandEncoder, Metal.IMTLTexture, Metal.MTLRegion, System.UIntPtr, System.UIntPtr)",
					"PassKit.IPKPaymentAuthorizationControllerDelegate.GetPresentationWindow (PassKit.PKPaymentAuthorizationController)",
					"PassKit.PKPaymentAuthorizationControllerDelegate_Extensions.GetPresentationWindow (PassKit.IPKPaymentAuthorizationControllerDelegate, PassKit.PKPaymentAuthorizationController)",
					"Metal.MTLTextureWrapper.FirstMipmapInTail",
					"Metal.MTLTextureWrapper.IsSparse",
					"Metal.MTLTextureWrapper.TailSizeInBytes",
					"Metal.IMTLTexture.FirstMipmapInTail",
					"Metal.IMTLTexture.IsSparse",
					"Metal.IMTLTexture.TailSizeInBytes",


					// HKSeriesBuilder doesn't implement the ISNCopying protocol on all platforms (and shouldn't on any according to the headers, so removed for XAMCORE_5_0).
					"HealthKit.HKSeriesBuilder.EncodeTo (Foundation.NSCoder)",

					// The signature is slightly different between platforms for this member, this is expected
					"SceneKit.SCNRenderer.FromContext (OpenGLES.EAGLContext, Foundation.NSDictionary)",
					"SceneKit.SCNRenderer.FromContext (OpenGL.CGLContext, Foundation.NSDictionary)",

					// For historical reasons, MPMediaItem and MPMediaEntity are wildly different between platforms (https://github.com/xamarin/xamarin-macios/issues/17291).
					"MediaPlayer.MPMediaEntity.EncodeTo (Foundation.NSCoder)",
					"MediaPlayer.MPMediaEntity.get_PropertyPersistentID ()",
					"MediaPlayer.MPMediaEntity.GetObject (Foundation.NSObject)",
					"MediaPlayer.MPMediaEntity.PropertyPersistentID",
					"MediaPlayer.MPMediaItem.DateAdded",
					"MediaPlayer.MPMediaItem.get_PropertyPersistentID ()",
					"MediaPlayer.MPMediaItem.GetObject (Foundation.NSObject)",
					"MediaPlayer.MPMediaItem.HasProtectedAsset",
					"MediaPlayer.MPMediaItem.IsExplicitItem",
					"MediaPlayer.MPMediaItem.IsPreorder",
					"MediaPlayer.MPMediaItem.PlaybackStoreID",
					"MediaPlayer.MPMediaItem.PropertyPersistentID",

					// Despite what headers say, NSAttributedString only implements NSItemProviderReading and NSItemProviderWriting on iOS (headers say tvOS and watchOS as well).
					// Ref: https://github.com/xamarin/xamarin-macios/pull/17306
					"Foundation.NSAttributedString.GetItemProviderVisibilityForTypeIdentifier (System.String)",
					"Foundation.NSAttributedString.GetObject (Foundation.NSData, System.String, Foundation.NSError&)",
					"Foundation.NSAttributedString.LoadData (System.String, System.Action`2<Foundation.NSData, Foundation.NSError>)",
					"Foundation.NSAttributedString.LoadDataAsync (System.String)",
					"Foundation.NSAttributedString.LoadDataAsync (System.String, Foundation.NSProgress&)",
					"Foundation.NSAttributedString.ReadableTypeIdentifiers",
					"Foundation.NSAttributedString.WritableTypeIdentifiers",
					"Foundation.NSAttributedString.WritableTypeIdentifiersForItemProvider",
				};
			}
		}

		static string GetTypeReferenceLookupKey (TypeReference tr)
		{
			if (tr is ArrayType at) {
				var sb = new StringBuilder (GetTypeReferenceLookupKey (at.ElementType));
				for (var i = 0; i < at.Rank; i++)
					sb.Append ("[]");
				return sb.ToString ();
			}

			if (tr is ByReferenceType rt)
				return GetTypeReferenceLookupKey (rt.ElementType) + "&";

			if (tr is GenericInstanceType git) {
				var sb = new StringBuilder (GetTypeReferenceLookupKey (git.ElementType));
				sb.Append ('<');
				for (var i = 0; i < git.GenericArguments.Count; i++) {
					if (i > 0)
						sb.Append (", ");
					sb.Append (GetTypeReferenceLookupKey (git.GenericArguments [i]));
				}
				sb.Append ('>');
				return sb.ToString ();
			}

			var td = tr.Resolve ();
			if (td is not null)
				return GetMemberLookupKeyInternal ((IMemberDefinition) td);
			return string.IsNullOrEmpty (tr.FullName) ? tr.Name : tr.FullName;
		}

		static string GetMemberLookupKey (IMemberDefinition member)
		{
			var key = GetMemberLookupKeyInternal (member);

			// The availability attributes between AppKit and UIKit are quite inconsistent:
			// https://github.com/xamarin/xamarin-macios/issues/17292
			key = key
				.Replace ("AppKit.NS", "XKit.X")
				.Replace ("UIKit.UI", "XKit.X")
				.Replace ("AppKit.INS", "XKit.IX")
				.Replace ("UIKit.IUI", "XKit.IX")
				.Replace ("AppKit", "XKit")
				.Replace ("UIKit", "XKit");

			return key;
		}

		static string GetMemberLookupKeyInternal (IMemberDefinition member)
		{
			if (member is FieldDefinition fd)
				return $"{GetMemberLookupKeyInternal (fd.DeclaringType)}.{fd.Name}";

			if (member is MethodDefinition md) {
				var sb = new StringBuilder ();
				if (md.IsSpecialName && md.Name.StartsWith ("op_", StringComparison.Ordinal)) {
					// operators are overloaded on return type, so the return type must be in the key
					sb.Append (GetTypeReferenceLookupKey (md.ReturnType));
					sb.Append (' ');
				}
				sb.Append (GetMemberLookupKeyInternal (md.DeclaringType));
				sb.Append ('.');
				sb.Append (md.Name);
				if (md.ContainsGenericParameter) {
					sb.Append ('`');
					sb.Append (md.GenericParameters.Count.ToString ());
				}
				sb.Append (" (");
				for (var i = 0; i < md.Parameters.Count; i++) {
					if (i > 0)
						sb.Append (", ");
					sb.Append (GetTypeReferenceLookupKey (md.Parameters [i].ParameterType));
				}
				sb.Append (")");
				return sb.ToString ();
			}

			if (member is PropertyDefinition pd)
				return $"{GetMemberLookupKeyInternal (pd.DeclaringType)}.{pd.Name}";

			if (member is TypeDefinition td)
				return string.IsNullOrEmpty (td.FullName) ? td.Name : td.FullName;

			if (member is EventDefinition ed)
				return "event: " + GetMemberLookupKeyInternal (ed.DeclaringType) + "." + ed.Name;

			throw new NotImplementedException (member.GetType ().FullName);
		}

		IEnumerable<IMemberDefinition> GetAllTypeMembers (TypeDefinition type)
		{
			foreach (var method in type.Methods.Where (m => m.IsPublic)) {
				yield return method;
			}
			foreach (var field in type.Fields.Where (f => f.IsPublic)) {
				yield return field;
			}
			foreach (var prop in type.Properties.Where (p => p.GetMethod?.IsPublic == true || p.SetMethod?.IsPublic == true)) {
				yield return prop;
			}
			foreach (var e in type.Events) {
				yield return e;
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
		[TestCaseSource (typeof (Helper), nameof (Helper.NetPlatformAssemblyDefinitions))]
		public void AllAttributedItemsMustIncludeCurrentPlatform (AssemblyInfo info)
		{
			var assembly = info.Assembly;

			string platformName = AssemblyToAttributeName (assembly);

			HashSet<string> found = new HashSet<string> ();
			foreach (var type in assembly.EnumerateTypes (a => HasAnyAvailabilityAttribute (a))) {
				CheckCurrentPlatformIncludedIfAny (type, platformName, type.FullName, type.DeclaringType, found);
			}
			foreach (var prop in assembly.EnumerateProperties (a => HasAnyAvailabilityAttribute (a))) {
				CheckCurrentPlatformIncludedIfAny (prop, platformName, prop.FullName, prop.DeclaringType, found);
			}
			foreach (var meth in assembly.EnumerateMethods (a => HasAnyAvailabilityAttribute (a))) {
				CheckCurrentPlatformIncludedIfAny (meth, platformName, meth.FullName, meth.DeclaringType, found);
			}
			foreach (var field in assembly.EnumerateFields (a => HasAnyAvailabilityAttribute (a))) {
				CheckCurrentPlatformIncludedIfAny (field, platformName, field.FullName, field.DeclaringType, found);
			}
			Assert.That (found, Is.Empty, $"{found.Count} issues found");
		}

		void CheckCurrentPlatformIncludedIfAny (ICustomAttributeProvider item, string platformName, string fullName, TypeDefinition parent, HashSet<string> found)
		{
			if (HasAnyAvailabilityAttribute (item)) {
				if (IgnoreCurrentPlatform (fullName)) {
					return;
				}
				var supportedAttributes = item.CustomAttributes.Where (a => IsAvailabilityAttribute (a));
				if (!supportedAttributes.Any (a => FindAvailabilityKind (a) == platformName)) {
#if DEBUG
					Console.WriteLine (fullName);
					Console.WriteLine (String.Join (" ", supportedAttributes.Select (x => FindAvailabilityKind (x))));
					Console.WriteLine (platformName);
#endif
					found.Add (fullName);
				}
			}
		}

		bool IgnoreCurrentPlatform (string fullName)
		{
			switch (fullName) {
			// Generator Bug - Obsolete does not imply introduced but prevents unlisted from created one
			case "AudioUnit.AudioUnitParameterType AudioUnit.AudioUnitParameterType::AUDCFilterDecayTime":
			case "AppKit.NSBoxType AppKit.NSBoxType::NSBoxSecondary":
			case "AppKit.NSBoxType AppKit.NSBoxType::NSBoxOldStyle":
				return true;
			default:
				return false;
			}
		}

		string AssemblyToAttributeName (AssemblyDefinition assembly)
		{
			var baseName = assembly.Name.Name + ".dll";
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

		bool FirstContainsAllOfSecond<T> (IEnumerable<T> first, IEnumerable<T> second)
		{
			var firstSet = new HashSet<T> (first);
			return second.All (s => firstSet.Contains (s));
		}

		IEnumerable<string> GetAvailabilityAttributes (ICustomAttributeProvider provider) => GetAvailabilityAttributes (provider.CustomAttributes);
		IEnumerable<string> GetAvailabilityAttributes (IEnumerable<CustomAttribute> attributes) => GetAvailabilityAttributesCore (attributes, IsAvailabilityAttribute);

		IEnumerable<string> GetSupportedAvailabilityAttributes (ICustomAttributeProvider provider) => GetSupportedAvailabilityAttributes (provider.CustomAttributes);
		IEnumerable<string> GetSupportedAvailabilityAttributes (IEnumerable<CustomAttribute> attributes) => GetAvailabilityAttributesCore (attributes, IsSupportedAttribute);

		IEnumerable<string> GetAvailabilityAttributesCore (IEnumerable<CustomAttribute> attributes, Func<CustomAttribute, bool> filter)
		{
			var availability = new HashSet<string> ();
			foreach (var attribute in attributes.Where (a => filter (a))) {
				var kind = FindAvailabilityKind (attribute);
				if (kind is not null) {
					availability.Add (kind);
				}
			}
			return availability;
		}

		string? FindAvailabilityKind (CustomAttribute attribute)
		{
			if (attribute.ConstructorArguments.Count == 1 && attribute.ConstructorArguments [0].Type.Name == "String") {
				string full = (string) attribute.ConstructorArguments [0].Value;
				switch (full) {
				case string s when full.StartsWith ("ios", StringComparison.Ordinal):
					return "ios";
				case string s when full.StartsWith ("tvos", StringComparison.Ordinal):
					return "tvos";
				case string s when full.StartsWith ("macos", StringComparison.Ordinal):
					return "macos";
				case string s when full.StartsWith ("maccatalyst", StringComparison.Ordinal):
					return "maccatalyst";
				case string s when full.StartsWith ("watchos", StringComparison.Ordinal):
					return null; // WatchOS is ignored for comparision
				default:
					throw new System.NotImplementedException ($"Unknown platform kind: {full}");
				}
			}
			return null;
		}

		bool HasAnyAvailabilityAttribute (ICustomAttributeProvider provider) => provider.CustomAttributes.Any (a => IsAvailabilityAttribute (a));
		bool HasAnySupportedAttribute (ICustomAttributeProvider provider) => provider.CustomAttributes.Any (a => IsSupportedAttribute (a));

		bool IsAvailabilityAttribute (CustomAttribute attribute) => IsSupportedAttribute (attribute) || attribute.AttributeType.Name == "UnsupportedOSPlatformAttribute";
		bool IsSupportedAttribute (CustomAttribute attribute) => attribute.AttributeType.Name == "SupportedOSPlatformAttribute";

		[TestCaseSource (typeof (Helper), nameof (Helper.NetPlatformAssemblyDefinitions))]
		public void ModelMustBeProtocol (AssemblyInfo info)
		{
			// Verify that all types with a [Model] attribute must also have a [Protocol] attribute.
			// Exception: If the type in question has a [Register] attribute with IsWrapper = false, then that's OK.
			var failures = new HashSet<string> ();
			var typesWithModelAttribute = 0;
			var typesWithProtocolAttribute = 0;
			var assembly = info.Assembly;

			foreach (var type in assembly.EnumerateTypes ()) {
				if (!type.HasCustomAttributes)
					continue;

				var attributes = type.CustomAttributes;

				if (!attributes.Any (v => v.AttributeType.Is ("Foundation", "ModelAttribute")))
					continue;
				typesWithModelAttribute++;

				if (attributes.Any (v => v.AttributeType.Is ("Foundation", "ProtocolAttribute"))) {
					typesWithProtocolAttribute++;
					continue;
				}

				var registerAttribute = attributes.SingleOrDefault (v => v.AttributeType.Is ("Foundation", "RegisterAttribute"));
				if (registerAttribute is not null && !GetIsWrapper (registerAttribute))
					continue;

				failures.Add ($"The type {type.FullName} has a [Model] attribute, but no [Protocol] attribute.");
			}

			Assert.That (failures, Is.Empty, "Failures");
			Assert.That (typesWithModelAttribute, Is.GreaterThan (0), "No types with the [Model] attribute?");
			Assert.That (typesWithProtocolAttribute, Is.GreaterThan (0), "No types with the [Protocol] attribute?");

			static bool GetIsWrapper (CustomAttribute attrib)
			{
				// .ctor (string name, bool isWrapper)
				if (attrib.ConstructorArguments.Count == 2)
					return (bool) attrib.ConstructorArguments [1].Value;

				// public bool IsWrapper { get; set; }
				foreach (var field in attrib.Fields) {
					if (field.Name == "IsWrapper")
						return (bool) field.Argument.Value;
				}
				return false;
			}
		}
	}
}
