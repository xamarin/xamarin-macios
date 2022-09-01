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
		[TestCaseSource (typeof (Helper), nameof (Helper.NetPlatformAssemblies))]
		public void ChildElementsListAvailabilityForAllPlatformsOnParent (string assemblyPath)
		{
			var assembly = Helper.GetAssembly (assemblyPath);

			HashSet<string> found = new HashSet<string> ();
			foreach (var prop in Helper.FilterProperties (assembly, a => HasAnyAvailabilityAttribute (a))) {
				CheckAllPlatformsOnParent (prop, prop.FullName, prop.DeclaringType, found);
			}
			foreach (var meth in Helper.FilterMethods (assembly, a => HasAnyAvailabilityAttribute (a))) {
				CheckAllPlatformsOnParent (meth, meth.FullName, meth.DeclaringType, found);
			}
			foreach (var field in Helper.FilterFields (assembly, a => HasAnyAvailabilityAttribute (a))) {
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
		[TestCaseSource (typeof (Helper), nameof (Helper.NetPlatformAssemblies))]
		public void DoubleAttributedElements (string assemblyPath)
		{
			var assembly = Helper.GetAssembly (assemblyPath);
			if (assembly is null) {
				Assert.Ignore ("{assemblyPath} could not be found (might be disabled in build)");
				return;
			}

			var doubleAttributed = new List<string>();
			foreach (var type in Helper.FilterTypes (assembly, a => HasAnyAvailabilityAttribute (a))) {
				var platformCount = new Dictionary<string, int> ();
				foreach (var attribute in type.CustomAttributes.Where (a => IsAvailabilityAttribute (a))) {
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
					// AVFoundation.AVMetadataIdentifiers uses an old pattern of a parent
					// class and many child classes with constants.
					if (type.ToString() == "AVFoundation.AVMetadataIdentifiers") {
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

		public class PlatformClaimInfo
		{
			public HashSet<string> MentionedPlatforms { get; set; } // Mentioned in both Supported and Unsupported contexts
			public HashSet<string> ClaimedPlatforms { get; set; } // Mentioned only in Supported contexts

			public PlatformClaimInfo (List<string> mentionedPlatforms, List<string> claimedPlatforms)
			{
				MentionedPlatforms = new HashSet<string> (mentionedPlatforms);
				ClaimedPlatforms = new HashSet<string> (claimedPlatforms);
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
		// This test detects these by loading every NET6 platform assembly, and
		// comparing reality (what types/members actually exist) with the attributes
		// We can not do this against a single assembly at a time, as the attributes saying "this is supported on mac" aren't 
		// in the mac assembly, that's the bug. 
		[Test]
		public void FindSupportedOnElementsThatDoNotExistInThatAssembly ()
		{
			// Dictionary of (FullName of Member) -> (Dictionary of (Actual Platform) -> Platform Claim Info)
			var harvestedInfo = new Dictionary<string, Dictionary<string, PlatformClaimInfo>> ();

			// Load each platform assembly
			foreach (string assemblyPath in Helper.NetPlatformAssemblies) {
				var assembly = Helper.GetAssembly (assemblyPath);
				if (assembly is null) {
					Assert.Ignore ("{assemblyPath} could not be found (might be disabled in build)");
					return;
				}

				string currentPlatform = AssemblyToAttributeName (assemblyPath);

				// Walk every class/struct/enum/property/method/enum value/pinvoke/event
				foreach (var module in assembly.Modules) {
					foreach (var type in module.Types) {
						foreach (var member in GetAllTypeMembers (type)) {
							var mentionedPlatforms = GetAvailabilityAttributes (member).ToList();
							if (mentionedPlatforms.Any()) {
								var claimedPlatforms = GetSupportedAvailabilityAttributes (member).ToList();
								string key = GetMemberLookupKey (member);
								if (!harvestedInfo.ContainsKey (key)) {
									harvestedInfo[key] = new Dictionary<string, PlatformClaimInfo>();
								}
								var claimInfo = new PlatformClaimInfo (mentionedPlatforms, claimedPlatforms);
								if (harvestedInfo[key].ContainsKey(currentPlatform)) {
									harvestedInfo[key][currentPlatform].UnionWith (claimInfo);
								}
								else {
									harvestedInfo[key][currentPlatform] = claimInfo;
								}
							}
						}
					}
				}
			}
					
			// Now walk every item found above and check two things:
			var attributesWereCompiledOut = new List<string>();			
			var doesNotExistWhereClaimed = new List<string>();
			foreach (var (member, info) in harvestedInfo) {
				// 1. All platforms match in count of mentioned (we did not conditionally compile out attributes)
				int expectedPlatformCount = info.First().Value.MentionedPlatforms.Count();
				if (info.Any (i => i.Value.MentionedPlatforms.Count() != expectedPlatformCount)) {
						if (IgnoreElementsThatDoNotExistInThatAssembly (member)) {
							continue;
						}
						string detailedPlatformBreakdown = string.Join ("\n", info.Select(x => ($"Assembly {x.Key} => {x.Value}")));
						string errorMessage = $"{member} did not have the same number of SupportedOSPlatformAttribute in every assembly:\n{detailedPlatformBreakdown}";
						attributesWereCompiledOut.Add (errorMessage);
#if DEBUG
						Console.Error.WriteLine (errorMessage);
#endif
				}


				// 2. For each supported attribute claim exist, that it exists on that platform
				// Since we know each platform claims are now equal, just use the first one
				var claimedPlatforms = info.First().Value.ClaimedPlatforms;
				foreach (var platform in claimedPlatforms) {
					if (!info.ContainsKey (platform)) {
						if (IgnoreElementsThatDoNotExistInThatAssembly (member)) {
							continue;
						}
						string detailedPlatformBreakdown = string.Join ("\n", info.Select(x => ($"Assembly {x.Key} => Declares ({string.Join (" ", x.Value)})")));
						string errorMessage = $"{member} was not found on {platform} despite being declared supported there.";
						doesNotExistWhereClaimed.Add (errorMessage);
#if DEBUG
						Console.Error.WriteLine (errorMessage);
#endif		
					}
				}
			}

			Assert.That (attributesWereCompiledOut, Is.Empty, $"{attributesWereCompiledOut.Count} issues found");
			Assert.That (doesNotExistWhereClaimed, Is.Empty, $"{doesNotExistWhereClaimed.Count} issues found");
		}

		static bool IgnoreElementsThatDoNotExistInThatAssembly (string member)
		{
			// Xkit has many platform specific bits in AppKit/UIKit and is a mess to get right
			if (member.StartsWith ("Kit")) {
				return true;
			}
			// QuickLook is aliased with QuickLookUI on some platforms
			if (member.StartsWith("QuickLook")) {
				return true;
			}
			// These two types are defined with non-trivial define magic and one platform doesn't necessarily have
			// the same members
			if (member.StartsWith ("MediaPlayer.MPMediaItem") || member.StartsWith ("MediaPlayer.MPMediaEntity")) {
				return true;
			}
			// These are defined with a code behind due to API differences based on version
			switch (member) {
			case "MetricKit.MXMetaData.get_DictionaryRepresentation":
			case "MetricKit.MXMetaData.DictionaryRepresentation":
				return true;
			}
			// Generator Bug - Protocol inline with different attribute bug
			switch (member) {
			case string s when s.StartsWith("SceneKit.SCNLayer"):
				return true;
			case string s when s.StartsWith("AVFoundation.AVAudioSession"):
				return true;
			case string s when s.StartsWith("MediaPlayer.MPMoviePlayerController"):
				return true;
			}
			switch (member) {
			case "GameplayKit.GKHybridStrategist.get_GameModel":
			case "GameplayKit.GKHybridStrategist.get_RandomSource":
			case "GameplayKit.GKHybridStrategist.set_GameModel":
			case "GameplayKit.GKHybridStrategist.set_RandomSource":
			case "GLKit.GLKMeshBuffer.get_Allocator":
			case "GLKit.GLKMeshBuffer.get_Length":
			case "GLKit.GLKMeshBuffer.get_Map":
			case "GLKit.GLKMeshBuffer.get_Type":
			case "GLKit.GLKMeshBuffer.get_Zone":
			case "Intents.INObject.get_AlternativeSpeakableMatches":
			case "Intents.INObject.get_Identifier":
			case "Intents.INObject.get_PronunciationHint":
			case "Intents.INObject.get_SpokenPhrase":
			case "Intents.INObject.get_VocabularyIdentifier":
			case "TVKit.TVMediaItemContentView.get_Configuration":
			case "TVKit.TVMediaItemContentView.set_Configuration":
			case "TVKit.TVMonogramContentView.get_Configuration":
			case "TVKit.TVMonogramContentView.set_Configuration":
			case "CarPlay.CPApplicationDelegate.get_Window":
			case "CarPlay.CPApplicationDelegate.set_Window":
			case "AVFoundation.AVAssetDownloadDelegate.DidFinishCollectingMetrics":
			case "AVFoundation.AVAssetDownloadDelegate.TaskIsWaitingForConnectivity":
			case "AVFoundation.AVAssetDownloadDelegate.WillBeginDelayedRequest":
			case "AVFoundation.AVAssetDownloadDelegate.DidCreateTask":
			case "ARKit.ARQuickLookPreviewItem.get_PreviewItemTitle":
			case "ARKit.ARQuickLookPreviewItem.get_PreviewItemUrl":
			case "Intents.INPerson.get_AlternativeSpeakableMatches":
			case "Intents.INPerson.get_Identifier":
			case "Intents.INPerson.get_PronunciationHint":
			case "Intents.INPerson.get_SpokenPhrase":
			case "Intents.INPerson.get_VocabularyIdentifier":
			case "MetricKit.MXUnitSignalBars.get_Symbol":
			case "MetricKit.MXUnitAveragePixelLuminance.get_Symbol":
			case "WebKit.DomNode.Copy":
			case "WebKit.DomEventTarget.Copy":
			case "WebKit.DomObject.Copy":
			case "WebKit.WebArchive.Copy":
			case "WebKit.WebHistoryItem.Copy":
			case "CoreWlan.CWConfiguration.Copy":
			case "CoreWlan.CWConfiguration.MutableCopy":
			case "CoreWlan.CWChannel.Copy":
			case "CoreWlan.CWMutableNetworkProfile.Copy":
			case "CoreWlan.CWMutableNetworkProfile.MutableCopy":
			case "CoreWlan.CWNetwork.Copy":
			case "CoreWlan.CWNetworkProfile.Copy":
			case "CoreWlan.CWNetworkProfile.MutableCopy":
				return true;
			}
			// Generator Bug/Limitation - Related to ^, Wrapper protocol get/set with attributes
			switch (member) {
			case "AuthenticationServices.ASExtensionErrorCodeExtensions.get_LocalizedFailureReasonErrorKey":
			case "AuthenticationServices.ASExtensionErrorCodeExtensions.LocalizedFailureReasonErrorKey":
			case "Intents.INSearchForMessagesIntentHandling_Extensions.ResolveSpeakableGroupNames:":
			case "Intents.INSearchIntents.INSearchCallHistoryIntentHandling_Extensions":
			case "Intents.INStartAudioCallIntentHandling_Extensions.ResolveDestinationType":
			case "Metal.IMTLResourceStateCommandEncoder.Wait":
			case "Metal.IMTLBlitCommandEncoder.ResetTextureAccessCounters":
			case "Metal.IMTLRenderCommandEncoder_Extensions.SetScissorRects":
			case "Metal.IMTLRenderCommandEncoder_Extensions.SetViewports":
			case "Metal.MTLAccelerationStructureCommandEncoderWrapper.get_Device":
			case "Metal.MTLAccelerationStructureCommandEncoderWrapper.get_Label":
			case "Metal.MTLAccelerationStructureCommandEncoderWrapper.set_Label":
			case "Metal.MTLAccelerationStructureWrapper.AllocatedSize":
			case "Metal.MTLAccelerationStructureWrapper.get_AllocatedSize":
			case "Metal.MTLAccelerationStructureWrapper.get_CpuCacheMode":
			case "Metal.MTLAccelerationStructureWrapper.get_Device":
			case "Metal.MTLAccelerationStructureWrapper.get_HazardTrackingMode":
			case "Metal.MTLAccelerationStructureWrapper.get_Heap":
			case "Metal.MTLAccelerationStructureWrapper.get_HeapOffset":
			case "Metal.MTLAccelerationStructureWrapper.get_IsAliasable":
			case "Metal.MTLAccelerationStructureWrapper.get_Label":
			case "Metal.MTLAccelerationStructureWrapper.get_ResourceOptions":
			case "Metal.MTLAccelerationStructureWrapper.get_StorageMode":
			case "Metal.MTLAccelerationStructureWrapper.HazardTrackingMode":
			case "Metal.MTLAccelerationStructureWrapper.Heap":
			case "Metal.MTLAccelerationStructureWrapper.HeapOffset":
			case "Metal.MTLAccelerationStructureWrapper.IsAliasable":
			case "Metal.MTLAccelerationStructureWrapper.ResourceOptions":
			case "Metal.MTLAccelerationStructureWrapper.set_Label":
			case "Metal.MTLBlitCommandEncoder_Extensions.GetTextureAccessCounters":
			case "Metal.MTLBlitCommandEncoder_Extensions.ResetTextureAccessCounters":
			case "Metal.IMTLBlitCommandEncoder.GetTextureAccessCounters":
			case "Metal.MTLIntersectionFunctionTableWrapper.AllocatedSize":
			case "Metal.MTLIntersectionFunctionTableWrapper.get_AllocatedSize":
			case "Metal.MTLIntersectionFunctionTableWrapper.get_CpuCacheMode":
			case "Metal.MTLIntersectionFunctionTableWrapper.get_Device":
			case "Metal.MTLIntersectionFunctionTableWrapper.get_HazardTrackingMode":
			case "Metal.MTLIntersectionFunctionTableWrapper.get_Heap":
			case "Metal.MTLIntersectionFunctionTableWrapper.get_HeapOffset":
			case "Metal.MTLIntersectionFunctionTableWrapper.get_IsAliasable":
			case "Metal.MTLIntersectionFunctionTableWrapper.get_Label":
			case "Metal.MTLIntersectionFunctionTableWrapper.get_ResourceOptions":
			case "Metal.MTLIntersectionFunctionTableWrapper.get_StorageMode":
			case "Metal.MTLIntersectionFunctionTableWrapper.HazardTrackingMode":
			case "Metal.MTLIntersectionFunctionTableWrapper.Heap":
			case "Metal.MTLIntersectionFunctionTableWrapper.HeapOffset":
			case "Metal.MTLIntersectionFunctionTableWrapper.IsAliasable":
			case "Metal.MTLIntersectionFunctionTableWrapper.ResourceOptions":
			case "Metal.MTLIntersectionFunctionTableWrapper.set_Label":
			case "Metal.MTLResourceStateCommandEncoder_Extensions.Wait":
			case "Metal.IMTLResourceStateCommandEncoder_Extensions.Wait":
			case "Metal.MTLResourceStateCommandEncoderWrapper.get_Device":
			case "Metal.MTLResourceStateCommandEncoderWrapper.get_Label":
			case "Metal.MTLResourceStateCommandEncoderWrapper.set_Label":
			case "Metal.IMTLResourceStateCommandEncoder.Update":
			case "Metal.MTLTexture_Extensions.GetFirstMipmapInTail":
			case "Metal.MTLTexture_Extensions.GetIsSparse":
			case "Metal.MTLTexture_Extensions.GetTailSizeInBytes":
			case "Metal.MTLTextureWrapper.FirstMipmapInTail":
			case "Metal.MTLTextureWrapper.get_FirstMipmapInTail":
			case "Metal.MTLTextureWrapper.get_IsSparse":
			case "Metal.MTLTextureWrapper.get_TailSizeInBytes":
			case "Metal.MTLTextureWrapper.IsSparse":
			case "Metal.MTLTextureWrapper.TailSizeInBytes":
			case "Metal.MTLVisibleFunctionTableWrapper.AllocatedSize":
			case "Metal.MTLVisibleFunctionTableWrapper.get_AllocatedSize":
			case "Metal.MTLVisibleFunctionTableWrapper.get_CpuCacheMode":
			case "Metal.MTLVisibleFunctionTableWrapper.get_Device":
			case "Metal.MTLVisibleFunctionTableWrapper.get_HazardTrackingMode":
			case "Metal.MTLVisibleFunctionTableWrapper.get_Heap":
			case "Metal.MTLVisibleFunctionTableWrapper.get_HeapOffset":
			case "Metal.MTLVisibleFunctionTableWrapper.get_IsAliasable":
			case "Metal.MTLVisibleFunctionTableWrapper.get_Label":
			case "Metal.MTLVisibleFunctionTableWrapper.get_ResourceOptions":
			case "Metal.MTLVisibleFunctionTableWrapper.get_StorageMode":
			case "Metal.MTLVisibleFunctionTableWrapper.HazardTrackingMode":
			case "Metal.MTLVisibleFunctionTableWrapper.Heap":
			case "Metal.MTLVisibleFunctionTableWrapper.HeapOffset":
			case "Metal.MTLVisibleFunctionTableWrapper.IsAliasable":
			case "Metal.MTLVisibleFunctionTableWrapper.ResourceOptions":
			case "Metal.MTLVisibleFunctionTableWrapper.set_Label":
			case "WebKit.WKPreviewActionItemWrapper.get_Title":
				return true;
			}
			// Generator Bug/Limitation - Also related to 2 ^, this time with catagories
			switch (member) {
			case "AVFoundation.AVMovie_AVMovieTrackInspection.LoadTrack":
			case "AVFoundation.AVMovie_AVMovieTrackInspection.LoadTrackAsync":
			case "AVFoundation.AVMovie_AVMovieTrackInspection.LoadTracksWithMediaCharacteristic":
			case "AVFoundation.AVMovie_AVMovieTrackInspection.LoadTracksWithMediaCharacteristicAsync":
			case "AVFoundation.AVMovie_AVMovieTrackInspection.LoadTracksWithMediaType":
			case "AVFoundation.AVMovie_AVMovieTrackInspection.LoadTracksWithMediaTypeAsync":
			case "AVFoundation.AVMutableMovie.LoadTrack":
			case "AVFoundation.AVMutableMovie.LoadTrackAsync":
			case "AVFoundation.AVMutableMovie.LoadTracksWithMediaCharacteristic":
			case "AVFoundation.AVMutableMovie.LoadTracksWithMediaCharacteristicAsync":
			case "AVFoundation.AVMutableMovie.LoadTracksWithMediaType":
			case "AVFoundation.AVMutableMovie.LoadTracksWithMediaTypeAsync":
				return true;
			}

			// Generator Bug/Limitation - Conditional protocol inclusion (NSItemProviderReading, NSItemProviderWriting, NSCoding, NSProgressReporting)
			if (member.EndsWith ("GetItemProviderVisibilityForTypeIdentifier") ||
				member.EndsWith ("GetObject") ||
				member.EndsWith ("LoadData") ||
				member.EndsWith ("LoadDataAsync") ||
				member.EndsWith ("get_ReadableTypeIdentifiers") ||
				member.EndsWith ("get_WritableTypeIdentifiers") ||
				member.EndsWith ("get_WritableTypeIdentifiersForItemProvider") ||
				member.EndsWith ("ReadableTypeIdentifiers") ||
				member.EndsWith ("WritableTypeIdentifiers") ||
				member.EndsWith ("WritableTypeIdentifiersForItemProvider") ||
				member.EndsWith ("get_Progress") ||
				member.EndsWith ("EncodeTo")) {
				return true;
			}
			// Generator Limitation - Conditional Abstract
			switch (member) {
			case "PassKit.PKPaymentAuthorizationControllerDelegate_Extensions.GetPresentationWindow":
			case "PassKit.IPKPaymentAuthorizationControllerDelegate.GetPresentationWindow":
				return true;
			}
			return false;
		}

		static string GetMemberLookupKey (IMemberDefinition member)
		{
			// Members of xkit and other places conditionally inline and include members in one of two namespaces
			// based upon platform assembly. Cludge them to the same key, so we don't mistakenly think members are missing
			// from some platforms
			return $"{member.DeclaringType.FullName}.{member.Name}".Replace("AppKit", "Kit").Replace("UIKit", "Kit");
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
		[TestCaseSource (typeof (Helper), nameof (Helper.NetPlatformAssemblies))]
		public void AllAttributedItemsMustIncludeCurrentPlatform (string assemblyPath)
		{
			var assembly = Helper.GetAssembly (assemblyPath);

			string platformName = AssemblyToAttributeName (assemblyPath);

			HashSet<string> found = new HashSet<string> ();
			foreach (var type in Helper.FilterTypes (assembly, a => HasAnyAvailabilityAttribute (a))) {
				CheckCurrentPlatformIncludedIfAny (type, platformName, type.FullName, type.DeclaringType, found);
			}
			foreach (var prop in Helper.FilterProperties (assembly, a => HasAnyAvailabilityAttribute (a))) {
				CheckCurrentPlatformIncludedIfAny (prop, platformName, prop.FullName, prop.DeclaringType, found);
			}
			foreach (var meth in Helper.FilterMethods (assembly, a => HasAnyAvailabilityAttribute (a))) {
				CheckCurrentPlatformIncludedIfAny (meth, platformName, meth.FullName, meth.DeclaringType, found);
			}
			foreach (var field in Helper.FilterFields (assembly, a => HasAnyAvailabilityAttribute (a))) {
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
					Console.WriteLine (String.Join(" ", supportedAttributes.Select (x => FindAvailabilityKind(x))));
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
	}
}
