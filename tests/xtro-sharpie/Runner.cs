using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Mono.Cecil;

using Clang.Ast;

namespace Extrospection {

	public class Runner {

		public Runner ()
		{
		}

		public void Execute (string pchFile, IEnumerable<string> assemblyNames, string outputDirectory, IEnumerable<string> searchDirectories)
		{
			var managed_reader = new AssemblyReader () {
				new MapNamesVisitor (), // must come first to map managed and native names.
				new ReleaseAttributeCheck (),
				new DesignatedInitializerCheck (),
				new DllImportCheck (),
				new EnumCheck (),
				new FieldCheck (),
				new ObjCInterfaceCheck (),
				new ObjCProtocolCheck (),
				new SelectorCheck (),
				new SimdCheck (),
				new RequiresSuperCheck (),
				new DeprecatedCheck (),
				new NullabilityCheck (),
				new UIAppearanceCheck (),
//				new ListNative (), // for debug
			};
			foreach (var assemblyName in assemblyNames) {
				var name = Path.GetFileNameWithoutExtension (assemblyName);
				if (name.EndsWith (".iOS", StringComparison.Ordinal))
					Helpers.Platform = Platforms.iOS;
				else if (name.EndsWith (".Mac", StringComparison.Ordinal) || name.EndsWith (".macOS", StringComparison.Ordinal))
					Helpers.Platform = Platforms.macOS;
				else if (name.EndsWith (".WatchOS", StringComparison.Ordinal))
					Helpers.Platform = Platforms.watchOS;
				else if (name.EndsWith (".TVOS", StringComparison.Ordinal) || name.EndsWith (".tvOS", StringComparison.Ordinal))
					Helpers.Platform = Platforms.tvOS;
				else if (name.EndsWith (".MacCatalyst", StringComparison.Ordinal))
					Helpers.Platform = Platforms.MacCatalyst;
				Helpers.IsDotNet = assemblyName.Contains ("/runtimes/");
				managed_reader.Load (assemblyName, searchDirectories);
			}
			managed_reader.Process ();

			var reader = new AstReader ();
			foreach (var v in managed_reader) {
				reader.TranslationUnitParsed += tu => {
					tu.Accept (v);
				};
			}

			reader.Load (pchFile);

			managed_reader.End ();

			Log.Save (outputDirectory);
		}
	}

	class AssemblyResolver : IAssemblyResolver, IDisposable {
		Dictionary<string, AssemblyDefinition> cache = new Dictionary<string, AssemblyDefinition> (StringComparer.Ordinal);
		HashSet<string> directories = new HashSet<string> ();

		public AssemblyDefinition Resolve (AssemblyNameReference name)
		{
			return Resolve (name, new ReaderParameters ());
		}

		AssemblyDefinition SearchDirectories (AssemblyNameReference name, ReaderParameters parameters)
		{
			var extensions = new string [] { ".dll", ".exe" };
			var paths = directories
				.SelectMany (dir => extensions.Select (ext => Path.Combine (dir, name.Name + ext)))
				.Where (File.Exists)
				.ToArray ();

			if (paths.Length == 0)
				throw new Exception ($"Unable to resolve the assembly {name.FullName} because it wasn't found in any of the search directories:\n\t{string.Join ("\n\t", directories)}");

			if (paths.Length > 1)
				throw new Exception ($"Unable to resolve the assembly {name.FullName} because multiple candidates were found:\n\t{string.Join ("\n\t", paths)}\nIn the search directories:\n\t{string.Join ("\n\t", directories)}");

			var path = paths [0];
			if (parameters.AssemblyResolver is null)
				parameters.AssemblyResolver = this;
			return AssemblyDefinition.ReadAssembly (path, parameters);
		}

		public AssemblyDefinition Load (string path)
		{
			var parameters = new ReaderParameters () {
				AssemblyResolver = this,
			};
			var rv = AssemblyDefinition.ReadAssembly (path, parameters);
			cache [rv.Name.FullName] = rv;
			return rv;
		}

		public AssemblyDefinition Resolve (AssemblyNameReference name, ReaderParameters parameters)
		{
			var key = name.FullName;
			if (cache.TryGetValue (key, out var assembly))
				return assembly;

			assembly = SearchDirectories (name, parameters);
			cache [key] = assembly;
			return assembly;
		}

		public void AddSearchDirectory (params string [] values)
		{
			foreach (var value in values)
				directories.Add (value.TrimEnd ('/'));
		}

		public void Dispose ()
		{
			// Nothing to do.
		}
	}

	class AssemblyReader : IEnumerable<BaseVisitor> {

		HashSet<AssemblyDefinition> assemblies = new HashSet<AssemblyDefinition> ();
		AssemblyResolver resolver = new AssemblyResolver ();

		public void Load (string filename, IEnumerable<string> searchDirectories)
		{
			resolver.AddSearchDirectory (searchDirectories.ToArray ());
			resolver.AddSearchDirectory (Path.GetDirectoryName (filename));
			assemblies.Add (resolver.Load (filename));
		}

		public void Process ()
		{
			foreach (var ad in assemblies) {
				foreach (var v in Visitors) {
					v.VisitManagedAssembly (ad);
					foreach (var module in ad.Modules) {
						v.VisitManagedModule (module);
						if (!module.HasTypes)
							continue;
						foreach (var td in module.Types)
							ProcessType (v, td);
					}
				}
			}
		}

		void ProcessType (BaseVisitor v, TypeDefinition type)
		{
			v.VisitManagedType (type);
			if (type.HasMethods) {
				foreach (var md in type.Methods)
					v.VisitManagedMethod (md);
			}

			if (type.HasNestedTypes) {
				foreach (var nested in type.NestedTypes)
					ProcessType (v, nested);
			}
		}

		List<BaseVisitor> Visitors { get; } = new List<BaseVisitor> ();

		public void Add (BaseVisitor visitor)
		{
			Visitors.Add (visitor);
		}

		public void End ()
		{
			foreach (var v in Visitors)
				v.End ();
		}

		public IEnumerator<BaseVisitor> GetEnumerator ()
		{
			return Visitors.GetEnumerator ();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
		{
			return Visitors.GetEnumerator ();
		}
	}

	public class BaseVisitor : AstVisitor {

		public virtual void VisitManagedAssembly (AssemblyDefinition assembly)
		{
		}

		public virtual void VisitManagedModule (ModuleDefinition module)
		{
		}

		public virtual void VisitManagedType (TypeDefinition type)
		{
		}

		public virtual void VisitManagedMethod (MethodDefinition method)
		{
		}

		// last chance to report errors
		public virtual void End ()
		{
		}
	}


	// debug
	class ListNative : BaseVisitor {

		public override void VisitDecl (Decl decl)
		{
			if (decl is FunctionDecl) {
				;
			} else if (decl is VarDecl) {
				;
			} else if (decl is ObjCProtocolDecl) {
				;
			} else if (decl is ObjCInterfaceDecl) {
				;
			} else if (decl is EnumDecl) {
				;
			} else {
				Console.WriteLine ("{0}\t{1}", decl, decl.GetType ().Name);
			}
		}
	}
}
