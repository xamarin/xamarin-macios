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

		Dictionary<string, AssemblyDefinition> assemblyCache = new Dictionary<string, AssemblyDefinition> ();


		[OneTimeSetUp]
		public void SetUp ()
		{
			foreach (string assemblyPath in Helper.NetPlatformAssemblies) {

				var assembly = Helper.GetAssembly (assemblyPath);
				assemblyCache.Add (assemblyPath, assembly);
			}
		}

		[OneTimeTearDown]
		public void TearDown ()
		{
			assemblyCache.Clear ();
		}

		bool IsTypeObsolete (TypeDefinition type)
		{
			if (type == null)
				return false;

			if (type.HasCustomAttributes && type.CustomAttributes.Where ((t) => t.AttributeType.Name == "ObsoleteAttribute" || t.AttributeType.Name == "AdviceAttribute").Any ()) {
				return true;
			}

			return false;
		}


		bool IsMethodObsolete (MethodDefinition method)
		{
			if (method == null)
				return false;

			if (method.HasCustomAttributes && method.CustomAttributes.Where ((m) => m.AttributeType.Name == "ObsoleteAttribute").Any ()) {
				return true;
			}


			return false;
		}

		bool IsPropertyObsolete (PropertyDefinition property, TypeDefinition t)
		{
			if (property == null)
				return false;

			if (property.HasCustomAttributes && property.CustomAttributes.Where ((p) => p.AttributeType.Name == "ObsoleteAttribute" || p.AttributeType.Name == "ObsoletedOSPlatformAttribute").Any ()) {
				return true;
			}

			return false;
		}


		bool IsException (string assemblyPath, MethodDefinition m)
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
			["CNLabelPhoneNumberKey"] = new HashSet<string> () { "iPhone" },
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
			["Dlfcn"] = new HashSet<string> () { "dlopen", "dlerror", "dlsym" }
		};

		public bool IsSkip (string type, string memberName, Dictionary<string, HashSet<string>> allowed)
		{
			if (allowed.TryGetValue (type, out var result)) {
				if (allowed [type].TryGetValue (memberName, out var skipped)) {
					return true;
				}
			}

			return Char.IsUpper (memberName [0]);
		}

		[TestCaseSource (typeof (Helper), nameof (Helper.NetPlatformAssemblies))]
		[Test]
		public void PropertiesCapitalizationTest (string assemblyPath)
		{
			Func<TypeDefinition, IEnumerable<string>> selectLambda = (type) => {
				var typeName = type.Name;
				var c = type.Properties
						.Where (p => p.GetMethod?.IsPublic == true || p.SetMethod?.IsPublic == true)
						.Where (p => !IsSkip (type.Name, p.Name, allowedProperties) && !IsPropertyObsolete (p, type) && !p.IsSpecialName)
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
				var c = from m in type.Methods
						where m.IsPublic && !IsSkip (type.Name, m.Name, allowedMethods) && !IsException (assemblyPath, m)
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
				var c = from f in type.Fields
						where f.IsPublic && f.IsFamilyOrAssembly && !IsSkip (type.Name, f.Name, allowedFields)
						select f.Name;
				return c;
			};
			CapitalizationTest (assemblyPath, selectLambda);
		}

		public void CapitalizationTest (string assemblyPath, Func<TypeDefinition, IEnumerable<string>> selectLambda)
		{

			if (assemblyCache.TryGetValue (assemblyPath, out var cache)) {
				var typeDict = new Dictionary<string, string> ();

				var publicTypes = cache.MainModule.Types.Where ((t) => t.IsPublic && !IsTypeObsolete (t));

				foreach (var type in publicTypes) {
					var err = selectLambda (type);
					if (err is not null && err.Any ()) {
						if (typeDict.TryGetValue ($"Type: {type.Name}", out var errMembers)) {
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
