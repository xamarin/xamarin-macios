using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Mono.Cecil;

namespace Xamarin.Bundler {
	public enum SymbolType {
		Function,
		ObjectiveCClass,
		Field,
	}

	public enum SymbolMode {
		Default,
		Linker, // pass "-u symbol" to the native linker
		Code, // generate code
		Ignore, // do nothing and hope for the best
	}

	public class Symbol {
		public SymbolType Type;
		public bool Ignore;
		public Abi? ValidAbis;

		static string ObjectiveCPrefix {
			get {
				return "OBJC_CLASS_$_";
			}
		}

		string name;
		public string Name {
			get {
				if (name is not null)
					return name;
				if (ObjectiveCName is not null)
					return ObjectiveCPrefix + ObjectiveCName;
				throw ErrorHelper.CreateError (99, Errors.MX0099, $"symbol without a name (type: {Type})");
			}
			set {
				name = value;
				if (name.StartsWith (ObjectiveCPrefix, StringComparison.Ordinal)) {
					ObjectiveCName = name.Substring (ObjectiveCPrefix.Length);
					name = null;
				}
			}
		}
		public string ObjectiveCName;

		public string Prefix {
			get {
				return "_";
			}
		}

		List<MemberReference> members = new List<MemberReference> ();
		public IEnumerable<MemberReference> Members { get { return members; } }

		public HashSet<AssemblyDefinition> Assemblies { get; private set; } = new HashSet<AssemblyDefinition> ();

		public void AddMember (MemberReference member)
		{
			members.Add (member);
			Assemblies.Add (member.Module.Assembly);
		}

		public void AddAssembly (AssemblyDefinition assembly)
		{
			Assemblies.Add (assembly);
		}
	}

	public class Symbols : IEnumerable<Symbol> {
		Dictionary<string, Symbol> store = new Dictionary<string, Symbol> (StringComparer.Ordinal);

		public int Count {
			get {
				return store.Count;
			}
		}

		public void Add (Symbol symbol)
		{
			store.Add (symbol.Name, symbol);
		}

		public Symbol AddObjectiveCClass (string class_name)
		{
			var symbol = new Symbol {
				Type = SymbolType.ObjectiveCClass,
				ObjectiveCName = class_name,
			};
			var existing = Find (symbol.Name);
			if (existing is not null)
				return existing;
			Add (symbol);
			return symbol;
		}

		public Symbol AddField (string name)
		{
			Symbol rv = Find (name);
			if (rv is null) {
				rv = new Symbol { Name = name, Type = SymbolType.Field };
				Add (rv);
			}
			return rv;
		}

		public Symbol AddFunction (string name)
		{
			Symbol rv = Find (name);
			if (rv is null) {
				rv = new Symbol { Name = name, Type = SymbolType.Function };
				Add (rv);
			}
			return rv;
		}

		public void Remove (Func<Symbol, bool> condition)
		{
			foreach (var symbol in this) {
				if (condition (symbol))
					store.Remove (symbol.Name);
			}
		}

		public IEnumerator<Symbol> GetEnumerator ()
		{
			return store.Values.GetEnumerator ();
		}

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return store.Values.GetEnumerator ();
		}

		public Symbol Find (string name)
		{
			Symbol rv;
			store.TryGetValue (name, out rv);
			return rv;
		}

		public bool Contains (string name)
		{
			return store.ContainsKey (name);
		}

		public Symbol this [string name] {
			get {
				return store [name];
			}
		}

		public void Load (string filename, Target target)
		{
			using (var reader = new StreamReader (filename)) {
				string line;
				Symbol current = null;
				while ((line = reader.ReadLine ()) is not null) {
					if (line.Length == 0)
						continue;
					if (line [0] == '\t') {
						var asm = line.Substring (1);
						Assembly assembly;
						if (!target.Assemblies.TryGetValue (Assembly.GetIdentity (asm), out assembly))
							throw ErrorHelper.CreateError (99, Errors.MX0099, $"serialized assembly {asm} for symbol {current.Name}, but no such assembly loaded");
						current.AddAssembly (assembly.AssemblyDefinition);
					} else {
						var eq = line.IndexOf ('=');
						var typestr = line.Substring (0, eq);
						var name = line.Substring (eq + 1);
						current = new Symbol { Name = name, Type = (SymbolType) Enum.Parse (typeof (SymbolType), typestr) };
						Add (current);
					}
				}
			}
		}

		public void Save (string filename)
		{
			using (var writer = new StreamWriter (filename)) {
				foreach (var symbol in store.Values) {
					writer.WriteLine ("{0}={1}", symbol.Type, symbol.Name);
					foreach (var asm in symbol.Assemblies)
						writer.WriteLine ($"\t{asm.MainModule.FileName}");
				}
			}
		}
	}
}
