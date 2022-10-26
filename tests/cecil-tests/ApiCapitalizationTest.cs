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

		bool IsMethodObsolete (MethodDefinition method)
		{
			if (method == null)
				return false;
			
			//method.Name == "get_path_radio_type" || method.Name== "dlclose"
			//if (method.Name == "eUbiquitousContainerIdentifierKey") {
			//	var r = from f in method.CustomAttributes
			//			select f.AttributeType.Name;
			//	Console.WriteLine ("attrs: ");
			//	foreach(var i in r) {
			//		Console.WriteLine (i);
			//	}
			//}

			if (method.HasCustomAttributes && method.CustomAttributes.Where ((m) => m.AttributeType.Name == "ObsoleteAttribute").Any ()) {
				return true;
			}


			return false;
		}

		bool IsTypeObsolete (TypeDefinition type)
		{
			if (type == null)
				return false;

			//method.Name == "get_path_radio_type" || method.Name== "dlclose"
			if (type.Name == "UTType") {
				var r = from f in type.CustomAttributes
						select f.AttributeType.Name;
				Console.WriteLine ("attrs: ");
				foreach(var i in r) {
					Console.WriteLine (i);
				}
			}

			if (type.HasCustomAttributes && type.CustomAttributes.Where ((m) => m.AttributeType.Name == "ObsoleteAttribute" || m.AttributeType.Name == "DepracatedAttribute").Any ()) {
				return true;
			}


			return false;
		}

		bool IsPropertyObsolete (PropertyDefinition property)
		{
			if (property == null)
				return false;

			//if (property.Name == "eUbiquitousContainerIdentifierKey" || property.Name == "k3dObject") {
			//	var r = from f in property.CustomAttributes
			//			select f.AttributeType.Name;
			//	Console.WriteLine ("attrs: ");
			//	foreach (var i in r) {
			//		Console.WriteLine (i);
			//	}
			//}

			if (property.HasCustomAttributes && property.CustomAttributes.Where ((p) => p.AttributeType.Name == "ObsoleteAttribute").Any ()) {
				return true;
			}

			return false;
		}


		bool IsException (MethodDefinition m)
		{ 

			if (m == null)
				return false;


			if (m.IsRemoveOn || m.IsAddOn || m.IsConstructor || m.IsSpecialName || IsMethodObsolete (m) || m.IsFamilyOrAssembly || m.IsPInvokeImpl)
				return true;

			return false;
		}


		Dictionary<string, HashSet<string>> allowedProperties = new Dictionary<string, HashSet<string>> () {
			["MPMusicPlayerController"] = new HashSet<string> () { "iPodMusicPlayer" },
			["ABPersonPhoneLabel"] = new HashSet<string> () { "iPhone" },
			["CNLabelPhoneNumberKey"] = new HashSet<string> () { "iPhone"},
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
						.Where (p => !Skip (type.Name, p.Name, allowedProperties) && !IsPropertyObsolete (p) && !p.IsSpecialName)
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
						where m.IsPublic && !IsException (m) && !Skip (type.Name, m.Name, allowedMethods)
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
						where f.IsFamilyOrAssembly
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

			var publicTypes = assembly.MainModule.Types.Where ((t) => t.IsPublic && !IsTypeObsolete(t));

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
