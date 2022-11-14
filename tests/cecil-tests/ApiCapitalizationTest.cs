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
using System.Configuration.Assemblies;
using MethodAttributes = Mono.Cecil.MethodAttributes;

#nullable enable

namespace Cecil.Tests {
	[TestFixture]
	public class ApiCapitalizationTest {

		Dictionary<string, (AssemblyDefinition Assembly, HashSet<String> MethodCache)> dllCache = new Dictionary<string, (AssemblyDefinition Assembly, HashSet<String> MethodCache)> ();


		[OneTimeSetUp]
		public void SetUp ()
		{ 
			foreach (string assemblyPath in Helper.NetPlatformAssemblies) {
				HashSet<String> methodCache = new HashSet<String> ();
				var assembly = Helper.GetAssembly (assemblyPath);
				var publicTypes = assembly.MainModule.Types.Where ((t) => t.IsPublic && !IsTypeObsolete (t));
				var pinvokes = AllPInvokes (assembly);
				var res = from item in pinvokes
						  where item.Name.Contains ("dl")
						  select item.PInvokeInfo.EntryPoint;
						  //select item.Name;
				foreach (var go in res) {
					methodCache.Add (go);
				}

				dllCache.Add (assemblyPath, (Assembly: assembly, MethodCache: methodCache));
			}
		}

		[OneTimeTearDown]
		public void TearDown ()
		{
			dllCache.Clear ();
		}

		bool IsTypeObsolete (TypeDefinition type)
		{
			if (type == null)
				return false;

			if (type.HasCustomAttributes && type.CustomAttributes.Where ((m) => m.AttributeType.Name == "ObsoleteAttribute" || m.AttributeType.Name == "AdviceAttribute").Any ()) {
				return true;
			}


			return false;
		}


		bool IsMethodObsolete (MethodDefinition method)
		{
			if (method == null)
				return false;

			//if (method.Name == "dlsym") {
			//	foreach (var i in method.CustomAttributes) {
			//		Console.WriteLine (i.AttributeType);
			//	}
			//	//Console.WriteLine(method.CustomAttributes.A)
			//}

			if (method.HasCustomAttributes && method.CustomAttributes.Where ((m) => m.AttributeType.Name == "ObsoleteAttribute").Any ()) {
				//Console.WriteLine ("method: " + method.Name);
				return true;
			}


			return false;
		}

		bool IsPropertyObsolete (PropertyDefinition property, TypeDefinition t)
		{
			if (property == null)
				return false;

			if (property.HasCustomAttributes && property.CustomAttributes.Where ((p) => p.AttributeType.Name == "ObsoleteAttribute").Any()) {
				return true;
			}

			return false;
		}


		bool IsException (string assemblyPath, TypeDefinition t, MethodDefinition m)
		{ 

			if (m == null)
				return false;

			if (m.Name == "inputs") {
				Console.WriteLine ("dlsym attrs: ");
				foreach (var attr in m.CustomAttributes) {
					Console.WriteLine (attr.AttributeType);
				}
			}

			if (m.IsRemoveOn || m.IsAddOn || m.IsConstructor || m.IsSpecialName || IsMethodObsolete (m) || m.IsFamilyOrAssembly || m.IsPInvokeImpl || m.HasPInvokeInfo || m.IsCompilerControlled)
				return true;



			var set = dllCache [assemblyPath].MethodCache;
			//if (set.Contains ($"{t.Name}.{m.Name}")) {
			//	return true;
			//}
			if (set.Contains($"{m.Name}")) {
				return true;
			}


			return false;
		}


		Dictionary<string, HashSet<string>> allowedProperties = new Dictionary<string, HashSet<string>> () {
			["MPMusicPlayerController"] = new HashSet<string> () { "iPodMusicPlayer" },
			["ABPersonPhoneLabel"] = new HashSet<string> () { "iPhone" },
			["CNLabelPhoneNumberKey"] = new HashSet<string> () { "iPhone"},
			["NSPersistentStoreCoordinator"] = new HashSet<string> () { "eUbiquitousContainerIdentifierKey" },
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

		[TestCaseSource (typeof (Helper), nameof (Helper.NetPlatformAssemblies))]
		[Test]
		public void PropertiesCapitalizationTest (string assemblyPath)
		{
			Func<TypeDefinition, IEnumerable<string>> selectLambda = (type) => {
				var typeName = type.Name;
				var c = type.Properties
						.Where (p => p.GetMethod?.IsPublic == true || p.SetMethod?.IsPublic == true)
						.Where (p => !Skip (type.Name, p.Name, allowedProperties) && !IsPropertyObsolete (p, type) && !p.IsSpecialName)
						.Select (p => p.Name);
				return c;
			};
			CapitalizationTest (assemblyPath, selectLambda);
		}

		[TestCaseSource (typeof (Helper), nameof (Helper.NetPlatformAssemblies))]
		[Test]
		public void MethodsCapitalizationTest (string assemblyPath)
		{
			Func<TypeDefinition, IEnumerable<string>> selectLambda = (type) => {
				var typeName = type.Name;
				var c = from m in type.Methods
						where m.IsPublic && !IsException (assemblyPath, type, m) && !Skip (type.Name, m.Name, allowedMethods)
						select m.Name;
				return c;
			};
			CapitalizationTest (assemblyPath, selectLambda);
		}

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


			//var assemblyPathName = dllCache [assemblyPath];
			//var assembly = assemblyPathName.Assembly;
		
			//if (assembly is null) {
			//	Assert.Ignore ($"{assemblyPath} could not be found (might be disabled in build)");
			//	return;
			//}

			if (dllCache.TryGetValue (assemblyPath, out var cache)) {
				var typeDict = new Dictionary<string, string> ();

				var publicTypes = cache.Assembly.MainModule.Types.Where ((t) => t.IsPublic && !IsTypeObsolete (t));

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
			} else {
				Assert.Ignore ($"{assemblyPath} could not be found (might be disabled in build)");
			}

		}

		IEnumerable<MethodDefinition> AllPInvokes (AssemblyDefinition assembly)
		{
			return Helper.FilterMethods (assembly, method =>
				(method.Attributes & MethodAttributes.PInvokeImpl) != 0);
		}

	}
}
