// Copyright 2015 Xamarin Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Mono.Cecil;

using NUnit.Framework;

using Xamarin.Tests;

namespace Xamarin.Linker {
	public class ProfilePoker : MobileProfile {

		static ProfilePoker p = new ProfilePoker ();

		public override string ProductAssembly => throw new NotImplementedException ();

		public static bool IsWellKnownSdk (string assemblyName)
		{
			return p.IsSdk (assemblyName);
		}

		protected override bool IsProduct (string assemblyName)
		{
			throw new NotImplementedException ();
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
				case "Xamarin.ObjcBinding.Tasks":
				case "Xamarin.MacDev":
				case "Xamarin.MacDev.Tasks":
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
			if (ar is null)
				return;
			references.Add (ar);
		}

		static string [] GetWatchOSAssemblies ()
		{
			var rv = new List<string> ();

			rv.AddRange (Directory.GetFiles (watchOSPath, "*.dll", SearchOption.TopDirectoryOnly).
				Select ((v) => v.Substring (watchOSPath.Length)).ToArray ());
			rv.Remove ("Xamarin.WatchOS.dll");
			rv.Add (Path.Combine ("..", "..", "32bits", "watchOS", "Xamarin.WatchOS.dll"));
			return rv.ToArray ();
		}

		static Dictionary<string, Tuple<int /* expected exit code */, string [] /* expected 'LLVM failed' lines */>> known_llvm_failures = new Dictionary<string, Tuple<int, string []>> {
			{ "System.Data.dll", new Tuple<int, string[]> (0, new string [] {
				"LLVM failed for 'XmlDataDocument.HasPointers': non-finally/catch/fault clause.",
				"LLVM failed for 'XmlDataDocument.OnFoliated': non-finally/catch/fault clause.",
				"LLVM failed for 'XmlDataDocument.SetRowValueFromXmlText': non-finally/catch/fault clause.",
				"LLVM failed for 'Constraint.CheckStateForProperty': non-finally/catch/fault clause.",
				"LLVM failed for 'ConstraintCollection.Clear': non-finally/catch/fault clause.",
				"LLVM failed for 'DataColumn.set_Expression': non-finally/catch/fault clause.",
				"LLVM failed for 'DataColumnCollection.BaseAdd': non-finally/catch/fault clause.",
				"LLVM failed for 'DataColumnCollection.Clear': non-finally/catch/fault clause.",
				"LLVM failed for 'DataRelation.CheckStateForProperty': non-finally/catch/fault clause.",
				"LLVM failed for 'DataRow.set_Item': non-finally/catch/fault clause.",
				"LLVM failed for 'DataRow.set_ItemArray': non-finally/catch/fault clause.",
				"LLVM failed for 'DataSet.SetLocaleValue': non-finally/catch/fault clause.",
				"LLVM failed for 'DataTable.RestoreIndexEvents': non-finally/catch/fault clause.",
				"LLVM failed for 'DataTable.set_Locale': non-finally/catch/fault clause.",
				"LLVM failed for 'DataTable.NewRecordFromArray': non-finally/catch/fault clause.",
				"LLVM failed for 'DataTable.RaiseRowChanged': non-finally/catch/fault clause.",
				"LLVM failed for 'DataTable.SetNewRecordWorker': non-finally/catch/fault clause.",
				"LLVM failed for 'DataView.OnListChanged': non-finally/catch/fault clause.",
				"LLVM failed for 'DataView.SetDataViewManager': non-finally/catch/fault clause.",
				"LLVM failed for 'DataViewManager.OnListChanged': non-finally/catch/fault clause.",
				"LLVM failed for 'DataExpression.Evaluate': non-finally/catch/fault clause.",
				"LLVM failed for 'DataExpression.ToBoolean': non-finally/catch/fault clause.",
				"LLVM failed for 'ExpressionParser.Parse': non-finally/catch/fault clause.",
				"LLVM failed for 'ExpressionParser.ParseAggregateArgument': non-finally/catch/fault clause.",
				"LLVM failed for 'Merger.MergeRelation': non-finally/catch/fault clause.",
				"LLVM failed for 'RecordManager.CopyRecord': non-finally/catch/fault clause.",
				"LLVM failed for 'Select.AcceptRecord': non-finally/catch/fault clause.",
				"LLVM failed for 'XDRSchema.GetMinMax': non-finally/catch/fault clause.",
				"LLVM failed for 'XmlTreeGen.SetMSDataAttribute': non-finally/catch/fault clause.",
				"LLVM failed for 'SNITCPHandle.ReceiveAsync': non-finally/catch/fault clause.",
				"LLVM failed for 'SchemaMapping.SetupSchemaWithoutKeyInfo': non-finally/catch/fault clause.",
				"LLVM failed for 'SchemaMapping.SetupSchemaWithKeyInfo': non-finally/catch/fault clause.",
				"LLVM failed for 'DbCommandBuilder.RowUpdatingHandler': non-finally/catch/fault clause.",
				"LLVM failed for 'DataAdapter.FillLoadDataRowChunk': non-finally/catch/fault clause.",
				"LLVM failed for 'DataAdapter.FillLoadDataRow': non-finally/catch/fault clause.",
				"LLVM failed for 'DataAdapter.FillMapping': non-finally/catch/fault clause.",
				"LLVM failed for 'DataAdapter.FillNextResult': non-finally/catch/fault clause.",
				"LLVM failed for 'DataRecordInternal.GetBytes': non-finally/catch/fault clause.",
				"LLVM failed for 'DataRecordInternal.GetChars': non-finally/catch/fault clause.",
				"LLVM failed for 'DbDataAdapter.Update': non-finally/catch/fault clause.",
				"LLVM failed for 'DataSetRelationCollection.AddCore': non-finally/catch/fault clause.",
			}) },
			{ "System.dll", new Tuple<int, string[]> (0, new string [] {
				"LLVM failed for 'WebClient.DownloadDataInternal': non-finally/catch/fault clause.",
				"LLVM failed for 'WebClient.DownloadFile': non-finally/catch/fault clause.",
				"LLVM failed for 'WebClient.OpenRead': non-finally/catch/fault clause.",
				"LLVM failed for 'WebClient.OpenWrite': non-finally/catch/fault clause.",
				"LLVM failed for 'WebClient.UploadDataInternal': non-finally/catch/fault clause.",
				"LLVM failed for 'WebClient.UploadValues': non-finally/catch/fault clause.",
				"LLVM failed for 'WebClient.AbortRequest': non-finally/catch/fault clause.",
				"LLVM failed for 'WebClient.DownloadBits': non-finally/catch/fault clause.",
				"LLVM failed for 'WebClient.UploadBits': non-finally/catch/fault clause.",
				"LLVM failed for 'WebClient.GetStringUsingEncoding': non-finally/catch/fault clause.",
				"LLVM failed for 'WebClient.OpenReadAsync': non-finally/catch/fault clause.",
				"LLVM failed for 'WebClient.OpenWriteAsync': non-finally/catch/fault clause.",
				"LLVM failed for 'WebClient.DownloadStringAsyncCallback': non-finally/catch/fault clause.",
				"LLVM failed for 'WebClient.DownloadStringAsync': non-finally/catch/fault clause.",
				"LLVM failed for 'WebClient.DownloadDataAsync': non-finally/catch/fault clause.",
				"LLVM failed for 'WebClient.DownloadFileAsync': non-finally/catch/fault clause.",
				"LLVM failed for 'WebClient.UploadStringAsync': non-finally/catch/fault clause.",
				"LLVM failed for 'WebClient.UploadDataAsync': non-finally/catch/fault clause.",
				"LLVM failed for 'WebClient.UploadFileAsync': non-finally/catch/fault clause.",
				"LLVM failed for 'WebClient.UploadValuesAsync': non-finally/catch/fault clause.",
				"LLVM failed for 'WebClient.<UploadStringAsync>b__179_0': non-finally/catch/fault clause.",
				"LLVM failed for '<DownloadBitsAsync>d__150.MoveNext': non-finally/catch/fault clause.",
				"LLVM failed for '<UploadBitsAsync>d__152.MoveNext': non-finally/catch/fault clause.",
				"LLVM failed for '<>c__DisplayClass164_0.<OpenReadAsync>b__0': non-finally/catch/fault clause.",
				"LLVM failed for '<>c__DisplayClass167_0.<OpenWriteAsync>b__0': non-finally/catch/fault clause.",
				"LLVM failed for '<WaitForWriteTaskAsync>d__55.MoveNext': non-finally/catch/fault clause.",
				"LLVM failed for '<SendFrameFallbackAsync>d__56.MoveNext': non-finally/catch/fault clause.",
				"LLVM failed for '<ReceiveAsyncPrivate>d__61`2.MoveNext': non-finally/catch/fault clause.",
				"LLVM failed for '<ReceiveAsyncPrivate>d__61`2.MoveNext': non-finally/catch/fault clause.",
				"LLVM failed for 'NetworkStream.Read': non-finally/catch/fault clause.",
				"LLVM failed for 'NetworkStream.Write': non-finally/catch/fault clause.",
				"LLVM failed for 'NetworkStream.BeginRead': non-finally/catch/fault clause.",
				"LLVM failed for 'NetworkStream.EndRead': non-finally/catch/fault clause.",
				"LLVM failed for 'NetworkStream.BeginWrite': non-finally/catch/fault clause.",
				"LLVM failed for 'NetworkStream.EndWrite': non-finally/catch/fault clause.",
				"LLVM failed for 'NetworkStream.ReadAsync': non-finally/catch/fault clause.",
				"LLVM failed for 'NetworkStream.WriteAsync': non-finally/catch/fault clause.",
			}) },
			{ "System.Core.dll", new Tuple<int, string[]> (0, new string [] {
				"LLVM failed for 'EnterTryCatchFinallyInstruction.Run': non-finally/catch/fault clause.",
			}) },
			{ "System.Net.Http.dll", new Tuple<int, string[]> (0, new string [] {
			}) },
			{ "mscorlib.dll", new Tuple<int, string[]> (0, new string [] {
				"LLVM failed for 'Console.Write': opcode arglist",
				"LLVM failed for 'Console.WriteLine': opcode arglist",
			}) },
		};

		[Test]
		[TestCaseSource ("GetWatchOSAssemblies")]
		public void NoLLVMFailuresInWatchOS (string asm)
		{
			MTouch.AssertDeviceAvailable ();

			// Run LLVM on every assembly we ship in watchOS, using the arguments we usually use when done from mtouch.
			var aot_compiler = Path.Combine (Configuration.BinDirXI, "armv7k-unknown-darwin-mono-sgen");
			var tmpdir = Cache.CreateTemporaryDirectory ();
			var llvm_path = Path.Combine (Configuration.SdkRootXI, "LLVM", "bin");
			var env = new Dictionary<string, string> {
				{ "MONO_PATH", watchOSPath }
			};
			var arch = "armv7k";
			var arch_dir = Path.Combine (tmpdir, arch);
			Directory.CreateDirectory (arch_dir);
			var args = new List<string> ();
			args.Add ("--debug");
			args.Add ("--llvm");
			args.Add ("-O=float32");
			args.Add ($"--aot=mtriple={arch}-ios" +
				$",data-outfile={Path.Combine (arch_dir, Path.GetFileNameWithoutExtension (asm) + ".aotdata." + arch)}" +
				$",static,asmonly,direct-icalls,llvmonly,nodebug,dwarfdebug,direct-pinvoke" +
				$",msym-dir={Path.Combine (arch_dir, Path.GetFileNameWithoutExtension (asm) + ".mSYM")}" +
				$",llvm-path={llvm_path}" +
				$",llvm-outfile={Path.Combine (arch_dir, Path.GetFileName (asm) + ".bc")}");
			args.Add (Path.Combine (watchOSPath, asm));

			StringBuilder output = new StringBuilder ();
			var rv = ExecutionHelper.Execute (aot_compiler, args, stdout: output, stderr: output, environmentVariables: env, timeout: TimeSpan.FromMinutes (5));
			var llvm_failed = output.ToString ().Split ('\n').Where ((v) => v.Contains ("LLVM failed"));
			Console.WriteLine (output);

			int expected_exit_code = 0;
			if (known_llvm_failures.TryGetValue (asm, out var known_failures)) {
				expected_exit_code = known_failures.Item1;
				Assert.AreEqual (expected_exit_code, rv, "AOT compilation");
				if (known_failures.Item2 is not null) {
					// Check if there are known failures for failures we've fixed
					var known_inexistent_failures = known_failures.Item2.Where ((v) => !llvm_failed.Contains (v));
					Assert.IsEmpty (string.Join ("\n", known_inexistent_failures), $"Redundant known failures: should be removed from dictionary for {asm}");
					// Filter the known failures from the failed llvm lines.
					llvm_failed = llvm_failed.Where ((v) => !known_failures.Item2.Contains (v));
				}
			}

			Assert.AreEqual (expected_exit_code, rv, "AOT compilation");
			Assert.IsEmpty (string.Join ("\n", llvm_failed), "LLVM failed");
		}
	}
}
