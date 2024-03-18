// Copyright 2013 Xamarin Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.IO;

using Mono.Cecil;

using Xamarin.Utils;

namespace Xamarin.Bundler {
	public class AotInfo {
		public AOTTask Task;
		public LinkTask LinkTask;
		public List<string> BitcodeFiles = new List<string> (); // .bc files produced by the AOT compiler
		public List<string> AsmFiles = new List<string> (); // .s files produced by the AOT compiler.
		public List<string> AotDataFiles = new List<string> (); // .aotdata files produced by the AOT compiler
		public List<string> ObjectFiles = new List<string> (); // .o files produced by the AOT compiler
	}

	public partial class Assembly {
		public bool BundleInContainerApp;

		public Dictionary<Abi, AotInfo> AotInfos = new Dictionary<Abi, AotInfo> ();

		HashSet<string> dependency_map;
		bool has_dependency_map;

		public bool HasDependencyMap {
			get {
				return has_dependency_map;
			}
		}

		public HashSet<string> DependencyMap {
			get {
				return dependency_map;
			}
		}

		// Recursively list all the assemblies the specified assembly depends on.
		HashSet<string> ComputeDependencies (List<Exception> warnings)
		{
			if (dependency_map is not null)
				return dependency_map;

			dependency_map = new HashSet<string> ();
			has_dependency_map = true;

			foreach (var ar in AssemblyDefinition.MainModule.AssemblyReferences) {
				var found = false;

				if (ar.FullName == AssemblyDefinition.FullName)
					continue;

				// find the dependent assembly
				foreach (var a in Target.Assemblies) {
					if (a == this)
						continue;

					if (a.AssemblyDefinition.Name.Name == ar.Name) {
						// gotcha
						if (!dependency_map.Contains (a.FullPath)) {
							dependency_map.Add (a.FullPath);
							dependency_map.UnionWith (a.ComputeDependencies (warnings));
						}
						found = true;
						break;
					}
				}

				if (!found) {
					warnings.Add (new ProductException (3005, false, Errors.MT3005, ar.FullName, AssemblyDefinition.FullName));
					has_dependency_map = false;
				}
			}

			return dependency_map;
		}

		public void ComputeDependencyMap (List<Exception> exceptions)
		{
			ComputeDependencies (exceptions);
		}

		// this will copy (and optionally strip) the assembly and almost all the related files:
		// * debug file (.mdb)
		// * config file (.config)
		// * satellite assemblies (<language id>/.dll)
		//
		// Aot data is copied separately, because we might want to copy aot data 
		// even if we don't want to copy the assembly (if 32/64-bit assemblies are identical, 
		// only one is copied, but we still want the aotdata for both).
		public void CopyToDirectory (string directory, bool reload = true, bool check_case = false, bool only_copy = false, bool copy_debug_symbols = true, StripAssembly strip = null)
		{
			var target = Path.Combine (directory, FileName);

			var fileNameNoExtension = Path.GetFileNameWithoutExtension (FileName);
			var assemblyName = AssemblyDefinition.Name.Name;
			if (check_case && fileNameNoExtension != assemblyName && string.Equals (fileNameNoExtension, assemblyName, StringComparison.OrdinalIgnoreCase)) {
				// Fix up the name of the target file to match the assembly name.
				target = Path.Combine (directory, assemblyName + Path.GetExtension (FileName));
			}

			// our Copy code deletes the target (so copy'ing over itself is a bad idea)
			if (directory != Path.GetDirectoryName (FullPath))
				CopyAssembly (FullPath, target, copy_debug_symbols: copy_debug_symbols, strip: strip);

			CopySatellitesToDirectory (directory);

			if (!only_copy) {
				if (reload) {
					LoadAssembly (target);
				} else {
					FullPath = target;
				}
			}
		}

		public void CopyAotDataFilesToDirectory (string directory)
		{
			foreach (var aotdata in AotInfos.Values.SelectMany ((info) => info.AotDataFiles))
				Application.UpdateFile (aotdata, Path.Combine (directory, Path.GetFileName (aotdata)));
		}

		/*
		 * Runs the AOT compiler, creating one of the following:
		 *     [not llvm]     => .s           + .aotdata
		 *     [is llvm-only] => .bc          + .aotdata
		 *     [is llvm]      => 
		 *          [is llvm creating assembly code] => .s + -llvm.s + .aotdata
		 *          [is llvm creating object code]   => .s + -llvm.o + .aotdata
		 */
		public void CreateAOTTask (Abi abi)
		{
			// Check if we've already created the AOT tasks.
			if (AotInfos.ContainsKey (abi))
				return;

			var assembly_path = FullPath;
			var build_dir = Path.GetDirectoryName (assembly_path);
			var arch = abi.AsArchString ();
			var asm_dir = Path.Combine (App.Cache.Location, arch);
			var asm = Path.Combine (asm_dir, Path.GetFileName (assembly_path)) + ".s";
			var data = Path.Combine (asm_dir, Path.GetFileNameWithoutExtension (assembly_path)) + ".aotdata" + "." + arch;
			var llvm_aot_ofile = "";
			var asm_output = (string) null;
			var other_output = string.Empty;
			var is_llvm = (abi & Abi.LLVM) == Abi.LLVM;

			Directory.CreateDirectory (asm_dir);

			if (!File.Exists (assembly_path))
				throw new ProductException (3004, true, Errors.MT3004, assembly_path);

			var aotInfo = new AotInfo ();
			AotInfos.Add (abi, aotInfo);

			if (App.EnableLLVMOnlyBitCode) {
				// In llvm-only mode, the AOT compiler emits a .bc file and no .s file for JITted code
				llvm_aot_ofile = Path.Combine (asm_dir, Path.GetFileName (assembly_path)) + ".bc";
				aotInfo.BitcodeFiles.Add (llvm_aot_ofile);
				other_output = Path.Combine (asm_dir, Path.GetFileName (assembly_path)) + "-output";
			} else if (is_llvm) {
				if (Driver.GetLLVMAsmWriter (App)) {
					llvm_aot_ofile = Path.Combine (asm_dir, Path.GetFileName (assembly_path)) + "-llvm.s";
					aotInfo.AsmFiles.Add (llvm_aot_ofile);
				} else {
					llvm_aot_ofile = Path.Combine (asm_dir, Path.GetFileName (assembly_path)) + "-llvm.o";
					aotInfo.ObjectFiles.Add (llvm_aot_ofile);
				}
				asm_output = asm;
			} else {
				asm_output = asm;
			}

			if (!string.IsNullOrEmpty (asm_output))
				aotInfo.AsmFiles.Add (asm_output);
			aotInfo.AotDataFiles.Add (data);

			var aotCompiler = Driver.GetAotCompiler (App, abi, Target.Is64Build);
			var aotArgs = App.GetAotArguments (assembly_path, abi, build_dir, asm_output ?? other_output, llvm_aot_ofile, data);
			var task = new AOTTask {
				Assembly = this,
				AssemblyName = assembly_path,
				AddBitcodeMarkerSection = BuildTarget != AssemblyBuildTarget.StaticObject && App.EnableMarkerOnlyBitCode,
				AssemblyPath = asm,
				FileName = aotCompiler,
				Arguments = aotArgs,
				Environment = new Dictionary<string, string> { { "MONO_PATH", Path.GetDirectoryName (assembly_path) } },
				AotInfo = aotInfo,
			};
			if (App.Platform == ApplePlatform.WatchOS) {
				// Visual Studio for Mac sets this environment variable, and it confuses the AOT compiler.
				// So unset it.
				// See https://github.com/mono/mono/issues/11765
				task.Environment ["MONO_THREADS_SUSPEND"] = null;
			}

			aotInfo.Task = task;
		}

		public bool CanSymLinkForApplication ()
		{
			if (EnableCxx || NeedsGccExceptionHandling || ForceLoad)
				return false;

			if (LinkerFlags is not null && LinkerFlags.Count > 0)
				return false;

			if (LinkWith is not null && LinkWith.Count > 0)
				return false;

			return true;
		}

		public bool Symlink ()
		{
			bool symlink_failed = false;

			string target = Path.Combine (Target.TargetDirectory, Path.GetFileName (FullPath));
			string source = FullPath;

			if (!Driver.SymlinkAssembly (App, source, target, Path.GetDirectoryName (target))) {
				symlink_failed = true;
				CopyAssembly (source, target);
			}

			if (Satellites is not null) {
				foreach (var a in Satellites) {
					string s_target_dir = Path.Combine (Target.TargetDirectory, Path.GetFileName (Path.GetDirectoryName (a)));
					string s_target = Path.Combine (s_target_dir, Path.GetFileName (a));

					if (!Driver.SymlinkAssembly (App, a, s_target, s_target_dir)) {
						CopyAssembly (a, s_target);
					}
				}
			}

			return symlink_failed;
		}

		public void LoadAssembly (string filename)
		{
			try {
				AssemblyDefinition = Target.Resolver.Load (filename);
				FullPath = AssemblyDefinition.MainModule.FileName;
				if (symbols_loaded.HasValue && symbols_loaded.Value) {
					symbols_loaded = null;
					LoadSymbols ();
				}
				Driver.Log (3, "Loaded '{0}'", FullPath);
			} catch (Exception e) {
				// cecil might not be able to load the assembly, e.g. bug #758
				throw new ProductException (1010, true, e, Errors.MT1010, FullPath, e.Message);
			}
		}
	}
}
