using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Xamarin.Bundler {

	public abstract class CoreResolver : IAssemblyResolver {

		internal Dictionary<string, AssemblyDefinition> cache;
		Dictionary<string, ReaderParameters> params_cache;

		public string FrameworkDirectory { get; set; }
		public string RootDirectory { get; set; }
		public string ArchDirectory { get; set; }


		public CoreResolver ()
		{
			cache = new Dictionary<string, AssemblyDefinition> (NormalizedStringComparer.OrdinalIgnoreCase);
			params_cache = new Dictionary<string, ReaderParameters> (StringComparer.Ordinal);
		}

		public IDictionary<string, AssemblyDefinition> ResolverCache { get { return cache; } }

		public IDictionary ToResolverCache ()
		{
			var resolver_cache = new Dictionary<string, AssemblyDefinition> (NormalizedStringComparer.OrdinalIgnoreCase);
			foreach (var pair in cache)
				resolver_cache.Add (pair.Key, pair.Value);

			return resolver_cache;
		}

		public void Dispose ()
		{
		}

		public AssemblyDefinition Resolve (AssemblyNameReference name)
		{
			var key = name.ToString ();
			if (!params_cache.TryGetValue (key, out ReaderParameters parameters)) {
				parameters = new ReaderParameters { AssemblyResolver = this };
				params_cache [key] = parameters;
			}
			return Resolve (name, parameters);
		}

		public abstract AssemblyDefinition Resolve (AssemblyNameReference name, ReaderParameters parameters);

		protected ReaderParameters CreateDefaultReaderParameters (string path)
		{
			var parameters = new ReaderParameters ();
			parameters.AssemblyResolver = this;
			parameters.InMemory = new FileInfo (path).Length < 1024 * 1024 * 100; // 100 MB.
			parameters.ReadSymbols = true;
			parameters.SymbolReaderProvider = new DefaultSymbolReaderProvider (throwIfNoSymbol: false);
			return parameters;
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

				var parameters = CreateDefaultReaderParameters (fileName);
				var symbolLoadFailure = false;
				try {
					assembly = ModuleDefinition.ReadModule (fileName, parameters).Assembly;
					params_cache [assembly.Name.ToString ()] = parameters;
					if (!assembly.MainModule.HasSymbols) {
						// We Cecil didn't load symbols, but there's a pdb, then something went wrong loading it (maybe an old-style pdb?).
						// Warn about this.
						var pdb = Path.ChangeExtension (fileName, "pdb");
						if (File.Exists (pdb))
							ErrorHelper.Show (ErrorHelper.CreateWarning (178, Errors.MX0178, fileName));
					}
					// Don't load native .pdb symbols, because we won't be able to write them back out again (so just drop them)
					if (assembly.MainModule?.SymbolReader?.GetType ()?.FullName == "Mono.Cecil.Pdb.NativePdbReader") {
						parameters.ReadSymbols = false;
						parameters.SymbolReaderProvider = null;
						assembly = ModuleDefinition.ReadModule (fileName, parameters).Assembly;
						ErrorHelper.Show (ErrorHelper.CreateWarning (178, Errors.MX0178, fileName));
					}
				} catch (IOException ex) when (ex.GetType ().FullName == "Microsoft.Cci.Pdb.PdbException") { // Microsoft.Cci.Pdb.PdbException is not public, so we have to check the runtime type :/
					symbolLoadFailure = true;
				} catch (SymbolsNotMatchingException) {
					symbolLoadFailure = true;
				}
				if (symbolLoadFailure) {
					parameters.ReadSymbols = false;
					parameters.SymbolReaderProvider = null;
					assembly = ModuleDefinition.ReadModule (fileName, parameters).Assembly;
					// only report the warning (on symbols) if we can actually load the assembly itself (otherwise it's more confusing than helpful)
					ErrorHelper.Show (ErrorHelper.CreateWarning (129, Errors.MX0129, fileName));
				}
			} catch (Exception e) {
				throw new ProductException (9, true, e, Errors.MX0009, fileName);
			}
			return CacheAssembly (assembly);
		}

		public AssemblyDefinition CacheAssembly (AssemblyDefinition assembly)
		{
			cache [assembly.Name.Name] = assembly;
			return assembly;
		}

		protected AssemblyDefinition SearchDirectory (string name, string directory, string extension = ".dll")
		{
			if (!Directory.Exists (directory))
				return null;

			var file = DirectoryGetFile (directory, name + extension);
			if (file.Length > 0)
				return Load (file);
			return null;
		}

		static string DirectoryGetFile (string directory, string file)
		{
			var files = Directory.GetFiles (directory, file);
			if (files is not null && files.Length > 0) {
				if (files.Length > 1) {
					ErrorHelper.Warning (133, Errors.MX0133, file, Environment.NewLine, string.Join ("\n", files));
				}
				return files [0];
			}

			return String.Empty;
		}

		public virtual void Configure ()
		{
		}
	}
}
