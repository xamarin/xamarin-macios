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
			if (fullName is null)
				throw new ArgumentNullException (nameof (fullName));

			return self.Resolve (AssemblyNameReference.Parse (fullName), defaults);
		}
	}

	public class MonoTouchResolver : CoreResolver {

		public bool EnableRepl { get; set; }

		public IEnumerable<AssemblyDefinition> GetAssemblies ()
		{
			return cache.Values.Cast<AssemblyDefinition> ();
		}

		public void Add (AssemblyDefinition assembly)
		{
			cache [Path.GetFileNameWithoutExtension (assembly.MainModule.FileName)] = assembly;
		}

		public override AssemblyDefinition Resolve (AssemblyNameReference name, ReaderParameters parameters)
		{
			var aname = name.Name;

			AssemblyDefinition assembly;
			if (cache.TryGetValue (aname, out assembly))
				return assembly;

			if (EnableRepl && FrameworkDirectory is not null) {
				var replDir = Path.Combine (FrameworkDirectory, "repl");
				if (Directory.Exists (replDir)) {
					assembly = SearchDirectory (aname, replDir);
					if (assembly is not null)
						return assembly;
				}
			}

			if (FrameworkDirectory is not null) {
				var facadeDir = Path.Combine (FrameworkDirectory, "Facades");
				assembly = SearchDirectory (aname, facadeDir);
				if (assembly is not null)
					return assembly;
			}

			if (ArchDirectory is not null) {
				assembly = SearchDirectory (aname, ArchDirectory);
				if (assembly is not null)
					return assembly;
			}

			assembly = SearchDirectory (aname, FrameworkDirectory);
			if (assembly is not null)
				return assembly;

			assembly = SearchDirectory (aname, RootDirectory);
			if (assembly is not null)
				return assembly;

			assembly = SearchDirectory (aname, RootDirectory, ".exe");
			if (assembly is not null)
				return assembly;

			return null;
		}
	}
}
