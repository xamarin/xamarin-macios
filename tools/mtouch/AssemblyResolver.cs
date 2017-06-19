//
// AssemblyResolver.cs
//
// Authors:
//   Jb Evain (jbevain@novell.com)
//   Sebastien Pouliot  <sebastien@xamarin.com>
//
// (C) 2010 Novell, Inc.
// Copyright 2012-2014 Xamarin Inc. All rights reserved.
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Mono.Cecil;
using Mono.Tuner;

using Xamarin.Bundler;

namespace MonoTouch.Tuner {

	public partial class MonoTouchManifestResolver : MonoTouchResolver {

		internal List<Exception> list = new List<Exception> ();
		
		public override AssemblyDefinition Load (string file)
		{
			if (EnableRepl && Profile.IsSdkAssembly (Path.GetFileNameWithoutExtension (file))) {
				var fn = Path.Combine (Path.GetDirectoryName (file), "repl", Path.GetFileName (file));
				if (File.Exists (fn))
					file = fn;
			}
			return base.Load (file);
		}
	}

	// recent cecil removed some overloads - https://github.com/mono/cecil/commit/42db79cc16f1cbe8dbab558904e188352dba2b41
	public static class AssemblyResolverRocks {

		static ReaderParameters defaults = new ReaderParameters ();

		public static AssemblyDefinition Resolve (this IAssemblyResolver self, string fullName)
		{
			if (fullName == null)
				throw new ArgumentNullException (nameof (fullName));
			
			return self.Resolve (AssemblyNameReference.Parse (fullName), defaults);
		}
	}

	public class MonoTouchResolver : IAssemblyResolver {

		public string FrameworkDirectory { get; set; }
		public string RootDirectory { get; set; }
		public string ArchDirectory { get; set; }
		public bool EnableRepl { get; set; }

		Dictionary<string, AssemblyDefinition> cache;

		public MonoTouchResolver ()
		{
			cache = new Dictionary<string, AssemblyDefinition> (NormalizedStringComparer.OrdinalIgnoreCase);
		}

		ReaderParameters CreateParameters (string path)
		{
			var parameters = new ReaderParameters ();
			parameters.AssemblyResolver = this;
			parameters.InMemory = new FileInfo (path).Length < 1024 * 1024 * 100; // 100 MB.
			return parameters;
		}

		public IDictionary ToResolverCache ()
		{
			var resolver_cache = new Dictionary<string, AssemblyDefinition> (NormalizedStringComparer.OrdinalIgnoreCase);
			foreach (var pair in cache)
				resolver_cache.Add (pair.Key, pair.Value);

			return resolver_cache;
		}

		public IEnumerable<AssemblyDefinition> GetAssemblies ()
		{
			return cache.Values.Cast<AssemblyDefinition> ();
		}

		public virtual AssemblyDefinition Load (string fileName)
		{
			if (!File.Exists (fileName))
				return null;

			AssemblyDefinition assembly;
			var name = Path.GetFileNameWithoutExtension (fileName);
			if (cache.TryGetValue (name, out assembly))
				return assembly;

			try {
				fileName = Target.GetRealPath (fileName);

				// Check the architecture-specific directory
				if (Path.GetDirectoryName (fileName) == FrameworkDirectory && !string.IsNullOrEmpty (ArchDirectory)) {
					var archName = Path.Combine (ArchDirectory, Path.GetFileName (fileName));
					if (File.Exists (archName))
						fileName = archName;
				}

				assembly = ModuleDefinition.ReadModule (fileName, CreateParameters (fileName)).Assembly;
			}
			catch (Exception e) {
				throw new MonoTouchException (9, true, e, "Error while loading assemblies: {0}", fileName);
			}
			cache.Add (name, assembly);
			return assembly;
		}

		public void Add (AssemblyDefinition assembly)
		{
			cache [Path.GetFileNameWithoutExtension (assembly.MainModule.FileName)] = assembly;
		}

		public AssemblyDefinition Resolve (string fullName)
		{
			return Resolve (AssemblyNameReference.Parse (fullName), null);
		}

		public AssemblyDefinition Resolve (string fullName, ReaderParameters parameters)
		{
			return Resolve (AssemblyNameReference.Parse (fullName), null);
		}

		public AssemblyDefinition Resolve (AssemblyNameReference reference)
		{
			return Resolve (reference, null);
		}

		public AssemblyDefinition Resolve (AssemblyNameReference name, ReaderParameters parameters)
		{
			var aname = name.Name;

			AssemblyDefinition assembly;
			if (cache.TryGetValue (aname, out assembly))
				return assembly;

			if (EnableRepl) {
				var replDir = Path.Combine (FrameworkDirectory, "repl");
				if (Directory.Exists (replDir)) {
					assembly = SearchDirectory (aname, replDir);
					if (assembly != null)
						return assembly;
				}
			}

			var facadeDir = Path.Combine (FrameworkDirectory, "Facades");
			if (Directory.Exists (facadeDir)) {
				assembly = SearchDirectory (aname, facadeDir);
				if (assembly != null)
					return assembly;
			}

			if (ArchDirectory != null) {
				assembly = SearchDirectory (aname, ArchDirectory);
				if (assembly != null)
					return assembly;
			}

			assembly = SearchDirectory (aname, FrameworkDirectory);
			if (assembly != null)
				return assembly;

			assembly = SearchDirectory (aname, RootDirectory);
			if (assembly != null)
				return assembly;

			assembly = SearchDirectory (aname, RootDirectory, ".exe");
			if (assembly != null)
				return assembly;

			return null;
		}

		AssemblyDefinition SearchDirectory (string name, string directory, string extension = ".dll")
		{
			var file = DirectoryGetFile (directory, name + extension);
			if (file.Length > 0)
				return Load (file);
			return null;
		}

		static string DirectoryGetFile (string directory, string file)
		{
			var files = Directory.GetFiles (directory, file);
			if (files != null && files.Length > 0)
				return files [0];

			return String.Empty;
		}

		public void Dispose ()
		{
		}
	}
}
