/*
 * Copyright (c) 2010, Geoff Norton <gnorton@novell.com>
 * Copyright (c) 2010, JB Evain <jbevain@novell.com>
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

using Mono.Cecil;

namespace Xamarin.Bundler {
	public partial class MonoMacResolver : IAssemblyResolver {
		public static bool IsClassic { get { return Driver.IsClassic; } }
		public static bool IsUnified { get { return Driver.IsUnified; } }

		public string FrameworkDirectory { get; set; }
		public string RootDirectory { get; set; }
		public string ArchDirectory { get; set; }

		public List <string> CommandLineAssemblies { get; set; }
		public List<Exception> Exceptions = new List<Exception> ();

		Dictionary<string, AssemblyDefinition> cache;

		public MonoMacResolver ()
		{
			cache = new Dictionary<string, AssemblyDefinition> ();
		}

		public IDictionary<string, AssemblyDefinition> ResolverCache { get { return cache; } }

		public IDictionary ToResolverCache ()
		{
			var resolver_cache = new Hashtable ();
			foreach (var pair in cache)
				resolver_cache.Add (pair.Key, pair.Value);

			return resolver_cache;
		}

		public AssemblyDefinition AddAssembly (string fileName)
		{
			if (!File.Exists (fileName))
				return null;

			AssemblyDefinition assembly;
			var name = Path.GetFileNameWithoutExtension (fileName);
			if (cache.TryGetValue (name, out assembly))
				return assembly;

			assembly = AssemblyDefinition.ReadAssembly (fileName, new ReaderParameters 
				{
					AssemblyResolver = this,
					InMemory = new FileInfo (fileName).Length < 1024 * 1024 * 100 // 100 MB
				});
			cache.Add (name, assembly);
			return assembly;
		}

		public AssemblyDefinition GetAssembly (string fileName)
		{
			return Resolve (Path.GetFileNameWithoutExtension (fileName));
		}

		public AssemblyDefinition Resolve (string fullName, ReaderParameters parameters)
		{
			return Resolve (AssemblyNameReference.Parse (fullName), parameters);
		}

		public AssemblyDefinition Resolve (string fullName)
		{
			return Resolve (fullName, new ReaderParameters { AssemblyResolver = this });
		}

		public AssemblyDefinition Resolve (AssemblyNameReference reference)
		{
			return Resolve (reference, new ReaderParameters { AssemblyResolver = this });
		}

		public AssemblyDefinition Resolve (AssemblyNameReference reference, ReaderParameters parameters)
		{
			var name = reference.Name;

			AssemblyDefinition assembly;
			if (cache.TryGetValue (name, out assembly))
				return assembly;
			
			if (CommandLineAssemblies != null && CommandLineAssemblies.Count > 0) {
				string cmdasm = CommandLineAssemblies.Find (t => {
					if (String.IsNullOrEmpty (t))
						return false;
					return String.Compare (name, Path.GetFileNameWithoutExtension (t), StringComparison.Ordinal) == 0;
				});
				assembly = String.IsNullOrEmpty (cmdasm) ? null : AddAssembly (cmdasm);
				if (assembly != null)
					return assembly;
			}

			if (ArchDirectory != null) {
				assembly = SearchDirectory (name, ArchDirectory);
				if (assembly != null)
					return assembly;
			}
			
			assembly = SearchDirectory (name, FrameworkDirectory);
			if (assembly != null)
				return assembly;

			var pclPath = Path.Combine (FrameworkDirectory, "Facades");
			if (Directory.Exists (pclPath)) {
				assembly = SearchDirectory (name, pclPath);
				if (assembly != null)
					return assembly;
			}

			assembly = SearchDirectory (name, RootDirectory);
			if (assembly != null)
				return assembly;

			return null;
		}

		AssemblyDefinition SearchDirectory (string name, string directory)
		{
			var file = DirectoryGetFile (directory, name + ".dll");
			if (file.Length > 0)
				return AddAssembly (file);

			file = DirectoryGetFile (directory, name + ".exe");
			if (file.Length > 0)
				return AddAssembly (file);

			return null;
		}

		static string DirectoryGetFile (string directory, string file)
		{
			var files = Directory.GetFiles (directory, file);
			if (files != null && files.Length > 0)
				return files [0];

			return "";
		}

		public void Dispose ()
		{
		}
	}
}
