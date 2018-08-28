using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Mono.Cecil;
using Mono.Cecil.Cil;

#if MTOUCH
using ProductException=Xamarin.Bundler.MonoTouchException;
#else
using ProductException=Xamarin.Bundler.MonoMacException;
#endif

namespace Xamarin.Bundler {

	public abstract class CoreResolver : IAssemblyResolver {

		internal Dictionary<string, AssemblyDefinition> cache;
		Dictionary<string,ReaderParameters> params_cache;

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
				try {
					assembly = ModuleDefinition.ReadModule (fileName, parameters).Assembly;
					params_cache [assembly.Name.ToString ()] = parameters;
				}
				catch (SymbolsNotMatchingException) {
					parameters.ReadSymbols = false;
					parameters.SymbolReaderProvider = null;
					assembly = ModuleDefinition.ReadModule (fileName, parameters).Assembly;
					// only report the warning (on symbols) if we can actually load the assembly itself (otherwise it's more confusing than helpful)
					ErrorHelper.Show (ErrorHelper.CreateWarning (129, $"Debugging symbol file for '{fileName}' does not match the assembly and is ignored."));
				}
			}
			catch (Exception e) {
				throw new ProductException (9, true, e, "Error while loading assemblies: {0}", fileName);
			}
			cache.Add (name, assembly);
			return assembly;
		}

		protected AssemblyDefinition SearchDirectory (string name, string directory, string extension = ".dll")
		{
			var file = DirectoryGetFile (directory, name + extension);
			if (file.Length > 0)
				return Load (file);
			return null;
		}

		static string DirectoryGetFile (string directory, string file)
		{
			var files = Directory.GetFiles (directory, file);
			if (files != null && files.Length > 0) {
				if (files.Length > 1) {
					ErrorHelper.Warning (133, "Found more than 1 assembly matching '{0}', choosing first:{1}{2}", file, Environment.NewLine, string.Join ("\n", files));
				}
				return files [0];
			}

			return String.Empty;
		}
	}
}
