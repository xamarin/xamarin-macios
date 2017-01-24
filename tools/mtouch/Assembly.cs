// Copyright 2013 Xamarin Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.IO;

using Mono.Cecil;

using Xamarin.Utils;

namespace Xamarin.Bundler {
	public enum AssemblyBuildTarget
	{
		StaticObject,
		DynamicLibrary,
		Framework,
	}

	public partial class Assembly
	{
		List<string> dylibs;
		public string Dylib;
		public AssemblyBuildTarget BuildTarget;
		public string BuildTargetName;

		public List<string> AotDataFiles = new List<string> ();

		HashSet<string> dependencies;

		public IEnumerable<string> Dylibs {
			get { return dylibs; }
		}

		// Recursively list all the assemblies the specified assembly depends on.
		HashSet<string> ComputeDependencies (List<Exception> warnings)
		{
			if (dependencies != null)
				return dependencies;

			dependencies = new HashSet<string> ();

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
						if (!dependencies.Contains (a.FullPath)) {
							dependencies.Add (a.FullPath);
							dependencies.UnionWith (a.ComputeDependencies (warnings));
						}
						found = true;
						break;
					}
				}

				if (!found)
					warnings.Add (new MonoTouchException (3005, false, "The dependency '{0}' of the assembly '{1}' was not found. Please review the project's references.",
					                                      ar.FullName, AssemblyDefinition.FullName));
			}

			return dependencies;
		}

		// returns false if the assembly was not copied (because it was already up-to-date).
		public bool CopyAssembly (string source, string target, bool copy_mdb = true)
		{
			var copied = false;

			try {
				if (!Application.IsUptodate (source, target) && !Cache.CompareAssemblies (source, target)) {
					copied = true;
					Application.CopyFile (source, target);
				}

				// Update the mdb even if the assembly didn't change.
				if (copy_mdb && File.Exists (source + ".mdb"))
					Application.UpdateFile (source + ".mdb", target + ".mdb", true);

				CopyConfigToDirectory (Path.GetDirectoryName (target));
			} catch (Exception e) {
				throw new MonoTouchException (1009, true, e, "Could not copy the assembly '{0}' to '{1}': {2}", source, target, e.Message);
			}

			return copied;
		}

		public void CopyMdbToDirectory (string directory)
		{
			string mdb_src = FullPath + ".mdb";
			if (File.Exists (mdb_src)) {
				string mdb_target = Path.Combine (directory, FileName + ".mdb");
				Application.UpdateFile (mdb_src, mdb_target);
			}
		}
		
		public void CopyMSymToDirectory (string directory)
		{
			string msym_src = FullPath + ".aotid.msym";
			var dirInfo = new DirectoryInfo (msym_src);
			if (!dirInfo.Exists) // got no aot data
				return;
			var subdirs = dirInfo.GetDirectories();
			foreach (var subdir in subdirs) {
				var destPath = Path.Combine (directory, subdir.Name.ToUpperInvariant ());
				var destInfo = new DirectoryInfo (destPath);
				if (!destInfo.Exists)
					Directory.CreateDirectory (destPath);
				var files = subdir.GetFiles ();
				foreach (FileInfo file in files) {
					string temppath = Path.Combine (destPath, file.Name);
					file.CopyTo(temppath, true);
				}
			}
		}

		public void CopyConfigToDirectory (string directory)
		{
			string config_src = FullPath + ".config";
			if (File.Exists (config_src)) {
				string config_target = Path.Combine (directory, FileName + ".config");
				Application.UpdateFile (config_src, config_target);
			}
		}

		// returns false if the assembly was not copied (because it was already up-to-date).
		public bool CopyToDirectory (string directory, bool reload = true, bool check_case = false, bool only_copy = false, bool copy_mdb = true)
		{
			var target = Path.Combine (directory, FileName);

			var fileNameNoExtension = Path.GetFileNameWithoutExtension (FileName);
			var assemblyName = AssemblyDefinition.Name.Name;
			if (check_case && fileNameNoExtension != assemblyName && string.Equals (fileNameNoExtension, assemblyName, StringComparison.OrdinalIgnoreCase)) {
				// Fix up the name of the target file to match the assembly name.
				target = Path.Combine (directory, assemblyName + Path.GetExtension (FileName));
			}

			var copied = false;

			// our Copy code deletes the target (so copy'ing over itself is a bad idea)
			if (directory != Path.GetDirectoryName (FullPath))
				copied = CopyAssembly (FullPath, target, copy_mdb: copy_mdb);

			CopySatellitesToDirectory (directory);

			if (!only_copy) {
				if (reload) {
					LoadAssembly (target);
				} else {
					FullPath = target;
				}
			}

			return copied;
		}

		IEnumerable<BuildTask> CreateCompileTasks (string s, string asm_infile, string llvm_infile, Abi abi)
		{
			var compile_tasks = new BuildTasks ();
			if (asm_infile != null) {
				var task = CreateCompileTask (s, asm_infile, abi);
				if (task != null)
					compile_tasks.Add (task);
			}

			if (llvm_infile != null) {
				var taskllvm = CreateCompileTask (s, llvm_infile, abi);
				if (taskllvm != null)
					compile_tasks.Add (taskllvm);
			}
			return compile_tasks.Count > 0 ? compile_tasks : null;
		}

		IEnumerable<BuildTask> CreateManagedToAssemblyTasks (string s, Abi abi, string build_dir)
		{
			var arch = abi.AsArchString ();
			var asm_dir = App.Cache.Location;
			var asm = Path.Combine (asm_dir, Path.GetFileName (s)) + "." + arch + ".s";
			var llvm_asm = Path.Combine (asm_dir, Path.GetFileName (s)) + "." + arch + "-llvm.s";
			var data = Path.Combine (asm_dir, Path.GetFileNameWithoutExtension (s)) + ".aotdata" + "." + arch;
			string llvm_ofile, llvm_aot_ofile = "";
			var is_llvm = (abi & Abi.LLVM) == Abi.LLVM;
			bool assemble_llvm = is_llvm && Driver.GetLLVMAsmWriter (App);

			if (!File.Exists (s))
				throw new MonoTouchException (3004, true, "Could not AOT the assembly '{0}' because it doesn't exist.", s);

			HashSet<string> dependencies = null;
			List<string> deps = null;
			List<string> outputs = new List<string> ();
			var warnings = new List<Exception> ();

			dependencies = ComputeDependencies (warnings);

			if (warnings.Count > 0) {
				ErrorHelper.Show (warnings);
				ErrorHelper.Warning (3006, "Could not compute a complete dependency map for the project. This will result in slower build times because Xamarin.iOS can't properly detect what needs to be rebuilt (and what does not need to be rebuilt). Please review previous warnings for more details.");
			} else {
				deps = new List<string> (dependencies.ToArray ());
				deps.Add (s);
				deps.Add (Driver.GetAotCompiler (App, Target.Is64Build));
			}

			if (App.EnableLLVMOnlyBitCode) {
				//
				// In llvm-only mode, the AOT compiler emits a .bc file and no .s file for JITted code
				//
				llvm_ofile = Path.Combine (asm_dir, Path.GetFileName (s)) + "." + arch + ".bc";
				outputs.Add (llvm_ofile);
				llvm_aot_ofile = llvm_ofile;
			} else {
				llvm_ofile = Path.Combine (asm_dir, Path.GetFileName (s)) + "." + arch + "-llvm.o";
				outputs.Add (asm);

				if (is_llvm) {
					if (assemble_llvm) {
						llvm_aot_ofile = llvm_asm;
					} else {
						llvm_aot_ofile = llvm_ofile;
						Target.LinkWith (llvm_ofile);
					}
					outputs.Add (llvm_aot_ofile);
				}
			}

			if (deps != null && Application.IsUptodate (deps, outputs)) {
				Driver.Log (3, "Target {0} is up-to-date.", asm);
				if (App.EnableLLVMOnlyBitCode)
					return CreateCompileTasks (s, null, llvm_ofile, abi);
				else
					return CreateCompileTasks (s, asm, assemble_llvm ? llvm_asm : null, abi);
			} else {
				Application.TryDelete (asm); // otherwise the next task might not detect that it will have to rebuild.
				Application.TryDelete (llvm_asm);
				Application.TryDelete (llvm_ofile);
				Driver.Log (3, "Target {0} needs to be rebuilt.", asm);
			}

			var aotCompiler = Driver.GetAotCompiler (App, Target.Is64Build);
			var aotArgs = Driver.GetAotArguments (App, s, abi, build_dir, asm, llvm_aot_ofile, data);
			Driver.Log (3, "Aot compiler: {0} {1}", aotCompiler, aotArgs);

			AotDataFiles.Add (data);

			IEnumerable<BuildTask> nextTasks;
			if (App.EnableLLVMOnlyBitCode)
				nextTasks = CreateCompileTasks (s, null, llvm_ofile, abi);
			else
				nextTasks = CreateCompileTasks (s, asm, assemble_llvm ? llvm_asm : null, abi);

			return new BuildTask [] { new AOTTask ()
				{
					AssemblyName = s,
					AddBitcodeMarkerSection = App.FastDev && App.EnableMarkerOnlyBitCode,
					AssemblyPath = asm,
					ProcessStartInfo = Driver.CreateStartInfo (App, aotCompiler, aotArgs, Path.GetDirectoryName (s)),
					NextTasks = nextTasks
				}
			};
		}

		// The input file is either a .s or a .bc file
		BuildTask CreateCompileTask (string assembly_name, string infile_path, Abi abi)
		{
			var ext = App.FastDev ? "dylib" : "o";
			var ofile = Path.ChangeExtension (infile_path, ext);
			var install_name = string.Empty;

			if (App.FastDev) {
				if (dylibs == null)
					dylibs = new List<string> ();
				dylibs.Add (ofile);
				install_name = "lib" + Path.GetFileName (assembly_name) + ".dylib";
			} else {
				Target.LinkWith (ofile);
			}

			if (Application.IsUptodate (new string [] { infile_path, App.CompilerPath }, new string [] { ofile })) {
				Driver.Log (3, "Target {0} is up-to-date.", ofile);
				return null;
			} else {
				Application.TryDelete (ofile); // otherwise the next task might not detect that it will have to rebuild.
				Driver.Log (3, "Target {0} needs to be rebuilt.", ofile);
			}

			var compiler_flags = new CompilerFlags () { Target = Target };

			BuildTask bitcode_task = null;
			BuildTask link_task = null;
			string link_task_input, link_language = "";

			if (App.EnableAsmOnlyBitCode) {
				link_task_input = infile_path + ".ll";
				link_language = "";
				// linker_flags.Add (" -fembed-bitcode");

				bitcode_task = new BitCodeifyTask () {
					Input = infile_path,
					OutputFile = link_task_input,
					Platform = App.Platform,
					Abi = abi,
					DeploymentTarget = App.DeploymentTarget,
				};
			} else {
				link_task_input = infile_path;
				if (infile_path.EndsWith (".s", StringComparison.Ordinal))
					link_language = "assembler";
			}

			if (App.FastDev) {
				compiler_flags.AddFrameworks (Frameworks, WeakFrameworks);
				compiler_flags.AddLinkWith (LinkWith, ForceLoad);
				compiler_flags.LinkWithMono ();
				compiler_flags.LinkWithXamarin ();
				compiler_flags.AddOtherFlags (LinkerFlags);
				if (Target.GetEntryPoints ().ContainsKey ("UIApplicationMain"))
					compiler_flags.AddFramework ("UIKit");
				compiler_flags.LinkWithPInvokes (abi);

				if (HasLinkWithAttributes && !App.EnableBitCode)
					compiler_flags.ReferenceSymbols (Target.GetRequiredSymbols (this, true));
			}

			if (App.EnableLLVMOnlyBitCode) {
				// The AOT compiler doesn't optimize the bitcode so clang will do it
				compiler_flags.AddOtherFlag ("-fexceptions");
				var optimizations = App.GetLLVMOptimizations (this);
				if (optimizations == null) {
					compiler_flags.AddOtherFlag ("-O2");
				} else if (optimizations.Length > 0) {
					compiler_flags.AddOtherFlag (optimizations);
				}
			}

			link_task = new LinkTask ()
			{
				Target = Target,
				AssemblyName = assembly_name,
				Abi = abi,
				InputFile = link_task_input,
				OutputFile = ofile,
				InstallName = install_name,
				CompilerFlags = compiler_flags,
				SharedLibrary = App.FastDev,
				Language = link_language,
			};

			if (bitcode_task != null) {
				bitcode_task.NextTasks = new BuildTask[] { link_task };
				return bitcode_task;
			}
			return link_task;
		}

		public bool CanSymLinkForApplication ()
		{
			if (EnableCxx || NeedsGccExceptionHandling || ForceLoad)
				return false;

			if (LinkerFlags != null && LinkerFlags.Count > 0)
				return false;

			if (LinkWith != null && LinkWith.Count > 0)
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

			if (Satellites != null) {
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

		public void CreateCompilationTasks (BuildTasks tasks, string build_dir, IEnumerable<Abi> abis)
		{
			var assembly = Path.Combine (build_dir, FileName);

			if (App.FastDev)
				Dylib = Path.Combine (App.AppDirectory, "lib" + Path.GetFileName (FullPath) + ".dylib");

			foreach (var abi in abis) {
				var task = CreateManagedToAssemblyTasks (assembly, abi, build_dir);
				if (task != null)
					tasks.AddRange (task);
			}
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
				throw new MonoTouchException (1010, true, e, "Could not load the assembly '{0}': {1}", FullPath, e.Message);
			}
		}
	}
}
