// Copyright 2015 Xamarin Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Mono.Cecil;
using NUnit.Framework;

using Xamarin.Tests;

namespace Xamarin.Linker {

	public abstract class BaseProfile {

		protected abstract bool IsSdk (string assemblyName);
	}

	public class ProfilePoker : MobileProfile {

		static ProfilePoker p = new ProfilePoker ();

		public static bool IsWellKnownSdk (string assemblyName)
		{
			return p.IsSdk (assemblyName);
		}
	}

	[TestFixture]
	public partial class SdkTest {
		
		static string UnifiedPath { get { return Path.Combine (Configuration.MonoTouchRootDirectory, "lib/mono/Xamarin.iOS/"); } }
		static string tvOSPath { get { return Path.Combine (Configuration.MonoTouchRootDirectory, "lib/mono/Xamarin.TVOS/"); } }
		static string watchOSPath { get { return Path.Combine (Configuration.MonoTouchRootDirectory, "lib/mono/Xamarin.WatchOS/"); } }

		void BCL (string path)
		{
			var failed_bcl = new List<string> ();
			foreach (var file in Directory.GetFiles (path, "*.dll")) {
				var aname = Path.GetFileNameWithoutExtension (file);
				switch (aname) {
				case "FSharp.Core":
				case "I18N":
				case "I18N.CJK":
				case "I18N.MidEast":
				case "I18N.Other":
				case "I18N.Rare":
				case "I18N.West":
					// need to be sure they are link-capable to include them into MobileProfile
					break;
				case "MonoTouch.Dialog-1":
				case "MonoTouch.NUnitLite":
				case "Xamarin.iOS":
				case "Xamarin.TVOS":
				case "Xamarin.WatchOS":
					// product assembly (use a different check than SDK/BCL)
					break;
				case "Newtonsoft.Json":
				case "Xamarin.iOS.Tasks":
				case "Xamarin.iOS.Tasks.Core":
				case "Xamarin.ObjcBinding.Tasks":
				case "Xamarin.MacDev":
				case "Xamarin.MacDev.Tasks":
				case "Xamarin.MacDev.Tasks.Core":
				case "Xamarin.Analysis.Tasks":
					// other stuff that is not part of the SDK but shipped in the same 2.1 directory
					failed_bcl.Add (aname);
					break;
				default:
					if (!ProfilePoker.IsWellKnownSdk (aname))
						failed_bcl.Add (aname);
					break;
				}
			}
			CollectionAssert.IsEmpty (failed_bcl, "BCL");
		}

		void REPL (string path)
		{
			var repl = Path.Combine (path, "repl");
			var failed_repl = new List<string> ();
			foreach (var file in Directory.GetFiles (repl, "*.dll")) {
				var aname = Path.GetFileNameWithoutExtension (file);
				switch (aname) {
				// sub-list that are SDK assemblies
				case "mscorlib":
				case "System":
				case "System.Core":
				case "System.Xml":
				case "Mono.CSharp":
					if (!ProfilePoker.IsWellKnownSdk (aname))
						failed_repl.Add (aname);
					break;
				default:
					failed_repl.Add (aname);
					break;
				}
			}
			CollectionAssert.IsEmpty (failed_repl, "Repl");
		}

		void Facades (string path)
		{
			var facades = Path.Combine (path, "Facades");
			var failed_facades = new List<string> ();
			foreach (var file in Directory.GetFiles (facades, "*.dll")) {
				var aname = Path.GetFileNameWithoutExtension (file);
				if (!ProfilePoker.IsWellKnownSdk (aname))
					failed_facades.Add (aname);
			}
			CollectionAssert.IsEmpty (failed_facades, "Facades");
		}

		[Test]
		public void iOS_Unified ()
		{
			BCL (UnifiedPath);
			REPL (UnifiedPath);
			Facades (UnifiedPath);
		}

		[Test]
		public void tvOS ()
		{
			BCL (tvOSPath);
			REPL (tvOSPath);
			Facades (tvOSPath);
		}

		[Test]
		public void watchOS ()
		{
			BCL (watchOSPath);
			REPL (watchOSPath);
			Facades (watchOSPath);
		}

		[Test]
		public void NoAssemblyReferenceInAttributes ()
		{
			var dir = Path.Combine (Configuration.MonoTouchRootDirectory, "lib/mono");
			var failed = new List<string> ();
			var curdir = Environment.CurrentDirectory;
			foreach (var filename in Directory.GetFiles (dir, "*.dll", SearchOption.AllDirectories)) {
				// This tests verifies that there aren't any attributes in any assembly we ship
				// that references an assembly that's not in the normal assembly references.
				// It takes a significant amount of time to look in all the attributes for assembly references,
				// and knowing that no such attributes exist in any assembly we ship, allows us
				// to complete skip this step in mtouch
				try {
					Environment.CurrentDirectory = Path.GetDirectoryName (filename);
					VerifyNoAdditionalAssemblyReferenceInAttributes (filename);
				} catch (Exception e) {
					Console.WriteLine ($"Failed to process {filename}: {e.ToString ()}");
					failed.Add ($"Failed to process {filename}: {e.Message}");
				} finally {
					Environment.CurrentDirectory = curdir;
				}
			}
			Assert.IsEmpty (string.Join ("\n", failed), "Failed files");
		}

		void VerifyNoAdditionalAssemblyReferenceInAttributes (string filename)
		{
			var references = new HashSet<AssemblyNameReference> ();
			using (var assembly = AssemblyDefinition.ReadAssembly (filename)) {
				var main = assembly.MainModule;
				references.UnionWith (main.AssemblyReferences);

				var pre_attributes = new HashSet<AssemblyNameReference> (references);

				GetCustomAttributeReferences (assembly, references);
				GetCustomAttributeReferences (main, references);
				if (main.HasTypes) {
					foreach (var ca in main.GetCustomAttributes ())
						GetCustomAttributeReferences (ca, references);
				}

				var post_attributes = pre_attributes.Except (references).ToArray ();
				Assert.IsEmpty (post_attributes, assembly.Name.Name);
			}
		}

		void GetCustomAttributeReferences (ICustomAttributeProvider cap, HashSet<AssemblyNameReference> references)
		{
			if (!cap.HasCustomAttributes)
				return;
			foreach (var ca in cap.CustomAttributes)
				GetCustomAttributeReferences (ca, references);
		}

		static void GetCustomAttributeReferences (CustomAttribute ca, HashSet<AssemblyNameReference> references)
		{
			if (ca.HasConstructorArguments) {
				foreach (var arg in ca.ConstructorArguments)
					GetCustomAttributeArgumentReference (arg, references);
			}
			if (ca.HasFields) {
				foreach (var arg in ca.Fields)
					GetCustomAttributeArgumentReference (arg.Argument, references);
			}
			if (ca.HasProperties) {
				foreach (var arg in ca.Properties)
					GetCustomAttributeArgumentReference (arg.Argument, references);
			}
		}

		static void GetCustomAttributeArgumentReference (CustomAttributeArgument arg, HashSet<AssemblyNameReference> references)
		{
			if (!arg.Type.Is ("System", "Type"))
				return;
			var ar = (arg.Value as TypeReference)?.Scope as AssemblyNameReference;
			if (ar == null)
				return;
			references.Add (ar);
		}
	}
}
