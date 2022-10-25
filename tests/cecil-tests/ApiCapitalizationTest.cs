using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;

using Mono.Cecil;
using Xamarin.Tests;
using Xamarin.Utils;
#nullable enable

namespace Cecil.Tests {
	[TestFixture]
	public class ApiCapitalizationTest {

		bool IsObsolete (MethodDefinition method)
		{
			if (method == null)
				return false;

			if (method.HasCustomAttributes && method.CustomAttributes.Where ((v) => v.AttributeType.Name == "IsObsoleteAttribute").Any ())
				return true;

			return false;
		}

		bool IsException (MethodDefinition m)
		{
			if (m == null)
				return false;

			if (m.IsRemoveOn || m.IsAddOn || m.IsConstructor || m.IsSpecialName || IsObsolete(m))
				return true;

			return false;
		}


		Dictionary<string, HashSet<string>> allowedProperties = new Dictionary<string, HashSet<string>> () {
			["MPMusicPlayerController"] = new HashSet<string> () { "iPodMusicPlayer" },
			["ABPersonPhoneLabel"] = new HashSet<string> () { "iPhone" },
			["AVMetadata"] = new HashSet<string> () { "iTunesMetadataKeyAccountKind",
			"iTunesMetadataKeyAcknowledgement",
			"iTunesMetadataKeyAlbum",
			"iTunesMetadataKeyAlbumArtist",
			"iTunesMetadataKeyAppleID",
			"iTunesMetadataKeyArranger",
			"iTunesMetadataKeyArtDirector",
			"iTunesMetadataKeyArtist",
			"iTunesMetadataKeyArtistID",
			"iTunesMetadataKeyAuthor",
			"iTunesMetadataKeyBeatsPerMin",
			"iTunesMetadataKeyComposer",
			"iTunesMetadataKeyConductor",
			"iTunesMetadataKeyContentRating",
			"iTunesMetadataKeyCopyright",
			"iTunesMetadataKeyCoverArt",
			"iTunesMetadataKeyCredits",
			"iTunesMetadataKeyDescription",
			"iTunesMetadataKeyDirector",
			"iTunesMetadataKeyDiscCompilation",
			"iTunesMetadataKeyDiscNumber",
			"iTunesMetadataKeyEQ",
			"iTunesMetadataKeyEncodedBy",
			"iTunesMetadataKeyEncodingTool",
			"iTunesMetadataKeyExecProducer",
			"iTunesMetadataKeyGenreID",
			"iTunesMetadataKeyGrouping",
			"iTunesMetadataKeyLinerNotes",
			"iTunesMetadataKeyLyrics",
			"iTunesMetadataKeyOnlineExtras",
			"iTunesMetadataKeyOriginalArtist",
			"iTunesMetadataKeyPerformer",
			"iTunesMetadataKeyPhonogramRights",
			"iTunesMetadataKeyPlaylistID",
			"iTunesMetadataKeyPredefinedGenre",
			"iTunesMetadataKeyProducer",
			"iTunesMetadataKeyPublisher",
			"iTunesMetadataKeyRecordCompany",
			"iTunesMetadataKeyReleaseDate",
			"iTunesMetadataKeySoloist",
			"iTunesMetadataKeySongID",
			"iTunesMetadataKeySongName",
			"iTunesMetadataKeySoundEngineer",
			"iTunesMetadataKeyThanks",
			"iTunesMetadataKeyTrackNumber",
			"iTunesMetadataKeyTrackSubTitle",
			"iTunesMetadataKeyUserComment",
			"iTunesMetadataKeyUserGenre" }
		};

		Dictionary<string, HashSet<string>> allowedFields = new Dictionary<string, HashSet<string>> () {
			["MPMusicPlayerController"] = new HashSet<string> () { "iPodMusicPlayer" },
			["ABPersonPhoneLabel"] = new HashSet<string> () { "iPhone" },
			["CMRotationMatrix"] = new HashSet<string> () { "m11", "m12", "m13", "m21", "m22", "m23", "m31", "m32", "m33" },
			["CTRunDelegateCallbacks"] = new HashSet<string> () { "version", "dealloc", "getAscent", "getDescent", "getWidth" },
			["CTParagraphStyleSettingValue"] = new HashSet<string> () { "int8", "single", "native_uint", "pointer" },
			["CTParagraphStyleSetting"] = new HashSet<string> () { "spec", "valueSize", "value" },
			["CMQuaternion"] = new HashSet<string> () { "x", "y", "z", "w" },
			["CMRotationRate"] = new HashSet<string> () { "x", "y", "z" },
			["CIFormat"] = new HashSet<string> () { "kRGBAf", "kBGRA8", "kRGBA8" },
			["MusicSequenceFileTypeID"] = new HashSet<string> () { "iMelody" },
			["UIImageStatusDispatcher"] = new HashSet<string> () { "callbackSelector" },
			["UIVideoStatusDispatcher"] = new HashSet<string> () { "callbackSelector" },
			["vImageAffineTransformFloat"] = new HashSet<string> () { "a", "b", "c", "d", "tx", "ty" },
			["vImageAffineTransformDouble"] = new HashSet<string> () { "a", "b", "c", "d", "tx", "ty" },
			["vImageGamma"] = new HashSet<string> () { "kUseGammaValue", "kUseGammaValueHalfPrecision",
				"k5over9_", "k9over5_", "k5over11_", "k11over5_",
				"ksRGB_", "ksRGB_", "k11over9_", "k9over11_",
				"kBT709_", "kBT709_" },
			["vImageMDTableUsageHint"] = new HashSet<string> () { "k16Q12", "kFloat" },
			["BlockDescriptor"] = new HashSet<string> () { "reserved", "size", "copy_helper", "dispose", "signature" },
			["XamarinBlockDescriptor"] = new HashSet<string> () { "descriptor", "ref_count" },
			["Constants"] = new HashSet<string> () { "libSystemLibrary", "libcLibrary", "libdispatchLibrary", "libcompressionLibrary" },
			["LinkTarget"] = new HashSet<string> () { "i386", "x86_64" },
			["MTLFeatureSet"] = new HashSet<string> () { "iOS_", "macOS_", "tvOS_" },
			["MTLGpuFamily"] = new HashSet<string> () { "iOSMac1", "iOSMac2" },
			["MTLLanguageVersion"] = new HashSet<string> () { "v1_0", "v1_1", "v1_2", "v2_0", "v2_1", "v2_2", "v2_3", "v2_4" },
			["MTLTextureType"] = new HashSet<string> () { "k1D", "k1DArray", "k2D", "k2DArray", "k2DMultisample", "kCube",
				"kCubeArray", "k3D", "k2DMultisampleArray", "kTextureBuffer" },
			["INCarAudioSource"] = new HashSet<string> () { "iPod" },
			["INPersonHandleLabel"] = new HashSet<string> () { "iPhone" },
			["NSDecimal"] = new HashSet<string> () { "fields", "m1", "m2", "m3", "m4", "m5", "m6", "m7", "m8" },
			["CGVector"] = new HashSet<string> () { "dx", "dy" },
			["AudioFormatType"] = new HashSet<string> () { "iLBC" },
			["NSTextScalingType"] = new HashSet<string> () { "iOS" },
			["ITLibDistinguishedPlaylistKind"] = new HashSet<string> () { "iTunesU" },
			["ITLibMediaItemMediaKind"] = new HashSet<string> () { "iOSApplication", "iTunesU" },
			["PHAssetSourceType"] = new HashSet<string> () { "iTunesSynced" }
		};


		Dictionary<string, HashSet<string>> allowedMethods = new Dictionary<string, HashSet<string>> () {

		};

		public bool Skip (string type, string m, Dictionary<string, HashSet<string>> allowed)
		{
			if (allowed.ContainsKey (type)) {
				if (allowed [type].Contains (m)) {
					return true;
				}
			}

			return Char.IsUpper (m [0]);
		}

		[TestCaseSource (typeof (Helper), nameof (Helper.PlatformAssemblies))]
		[TestCaseSource (typeof (Helper), nameof (Helper.NetPlatformAssemblies))]
		[Test]
		public void PropertiesCapitalizationTest (string assemblyPath)
		{
			Func<TypeDefinition, IEnumerable<string>> selectLambda = (type) => {
				var typeName = type.Name;
				var c = type.Properties
						.Where (p => p.GetMethod?.IsPublic == true || p.SetMethod?.IsPublic == true)
						.Where (p => !Skip (type.Name, p.Name, allowedProperties))
						.Select (p => p.Name);
				return c;
			};
			CapitalizationTest (assemblyPath, selectLambda);
		}

		[TestCaseSource (typeof (Helper), nameof (Helper.PlatformAssemblies))]
		[TestCaseSource (typeof (Helper), nameof (Helper.NetPlatformAssemblies))]
		[Test]
		public void MethodsCapitalizationTest (string assemblyPath)
		{
			Func<TypeDefinition, IEnumerable<string>> selectLambda = (type) => {
				var typeName = type.Name;
				var c = from m in type.Methods
						where !IsException (m) && m.IsPublic && !Skip (type.Name, m.Name, allowedMethods)
						select m.Name;
				return c;
			};
			CapitalizationTest (assemblyPath, selectLambda);
		}

		[TestCaseSource (typeof (Helper), nameof (Helper.PlatformAssemblies))]
		[TestCaseSource (typeof (Helper), nameof (Helper.NetPlatformAssemblies))]
		[Test]
		public void EventsCapitalizationTest (string assemblyPath)
		{
			Func<TypeDefinition, IEnumerable<string>> selectLambda = (type) => {
				var typeName = type.Name;
				var c = from e in type.Events
						where !(Char.IsUpper (e.Name [0]))
						select e.Name;
				return c;
			};
			CapitalizationTest (assemblyPath, selectLambda);
		}

		[TestCaseSource (typeof (Helper), nameof (Helper.PlatformAssemblies))]
		[TestCaseSource (typeof (Helper), nameof (Helper.NetPlatformAssemblies))]
		[Test]
		public void FieldsCapitalizationTest (string assemblyPath)
		{
			Func<TypeDefinition, IEnumerable<string>> selectLambda = (type) => {

				var typeName = type.Name;
				var c = from f in type.Fields
						where f.IsPublic && !Skip (type.Name, f.Name, allowedFields)
						where f.Name != "value__"
						select f.Name;
				return c;
			};
			CapitalizationTest (assemblyPath, selectLambda);
		}

		public void CapitalizationTest (string assemblyPath, Func<TypeDefinition, IEnumerable<string>> selectLambda)
		{
			var assembly = Helper.GetAssembly (assemblyPath);
			if (assembly is null) {
				Assert.Ignore ($"{assemblyPath} could not be found (might be disabled in build)");
				return;
			}

			var typeDict = new Dictionary<string, string> ();

			var publicTypes = assembly.MainModule.Types.Where ((t) => t.IsPublic);

			foreach (var type in publicTypes) {
				var err = selectLambda (type);
				if (err is not null && err.Any ()) {
					if (typeDict.ContainsKey ($"Type: {type.Name}")) {
						typeDict [$"Type: {type.Name}"] += string.Join ("; ", err);
					} else {
						typeDict.Add ($"Type: {type.Name}", string.Join ("; ", err));
					}
				}
			}

			if (typeDict is not null) {
				Assert.AreEqual (0, typeDict.Count (), $"Capitalization Issues Found: {string.Join (Environment.NewLine, typeDict)}");
			}
		}

	}
}
