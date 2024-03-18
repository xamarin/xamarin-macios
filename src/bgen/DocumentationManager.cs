using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;

#nullable enable

public class DocumentationManager {
	string xml;
	XmlDocument? doc;

	public DocumentationManager (string assembly)
	{
		this.xml = Path.ChangeExtension (assembly, ".xml");
		if (File.Exists (xml)) {
			doc = new XmlDocument ();
			doc.LoadWithoutNetworkAccess (xml);
		}
	}

	public void WriteDocumentation (StreamWriter sw, int indent, MemberInfo member)
	{
		if (!TryGetDocumentation (member, out var docs))
			return;

		foreach (var line in docs) {
			sw.Write ('\t', indent);
			sw.WriteLine (line);
		}
	}

	public bool TryGetDocumentation (MemberInfo member, [NotNullWhen (true)] out string []? documentation)
	{
		documentation = null;

		if (doc is null)
			return false;

		if (!TryGetId (member, out var id))
			return false;

		var node = doc.SelectSingleNode ($"/doc/members/member[@name='{id}']");
		if (node is null)
			return false;

		// Remove indentation, make triple-slash comments
		var lines = node.InnerXml.Split ('\n', '\r');
		for (var i = 0; i < lines.Length; i++) {
			lines [i] = "/// " + lines [i].TrimStart (' ');
		}

		documentation = lines;

		return true;
	}

	// https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/xmldoc/#id-strings
	// See tests/cecil-tests/Documentation.cs for an implementation that works with Cecil.
	// There's already an implementation in Roslyn, but that's a rather heavy dependency,
	// so we're implementing this in our own code instead.

	static string GetDocId (MethodInfo md)
	{
		var methodName = md.Name.Replace ('.', '#');
		var name = GetDocId (md.DeclaringType!) + "." + methodName;
		if (md.IsGenericMethodDefinition)
			name += $"``{md.GetGenericArguments ().Length}";
		var parameters = md.GetParameters ();
		if (parameters.Length > 0) {
			name += "(" + string.Join (",", parameters.Select (p => GetDocId (p.ParameterType))) + ")";
		}

		if (md.Name == "op_Explicit" || md.Name == "op_Implicit") {
			name += "~" + GetDocId (md.ReturnType);
		}

		return name;
	}

	static string GetDocId (EventInfo ed) => GetDocId (ed.DeclaringType!) + "." + ed.Name;

	static string GetDocId (PropertyInfo pd)
	{
		var name = GetDocId (pd.DeclaringType!) + "." + pd.Name;
		var parameters = pd.GetIndexParameters ();
		if (parameters.Length > 0) {
			name += "(" + string.Join (",", parameters.Select (p => GetDocId (p.ParameterType))) + ")";
		}
		return name;
	}


	static bool TryGetId (MemberInfo member, [NotNullWhen (true)] out string? name)
	{
		name = null;

		if (member is MethodInfo md) {
			name = "M:" + GetDocId (md);
		} else if (member is PropertyInfo pd) {
			name = "P:" + GetDocId (pd);
		} else if (member is FieldInfo fd) {
			name = "F:" + GetDocId (fd.DeclaringType!) + "." + fd.Name;
		} else if (member is EventInfo ed) {
			name = "E:" + GetDocId (ed);
		} else if (member is Type td) {
			name = "T:" + GetDocId (td);
		} else {
			return false;
		}
		return true;
	}

	static string GetDocId (Type tr)
	{
		var name = new StringBuilder ();

		if (tr.IsGenericParameter) {
			name.Append ('`');
			name.Append (tr.GenericParameterPosition);
#if NET
		} else if (tr.IsSZArray) {
#else
		} else if (tr.IsArray && tr.GetArrayRank () == 1) { // Not quite the same as IsSZArray (see https://github.com/dotnet/runtime/issues/20376), but good enough for legacy.
#endif
			name.Append (GetDocId (tr.GetElementType ()!));
			name.Append ("[]");
		} else if (tr.IsArray) {
			// As far as I can tell, System.Reflection doesn't provide a way to get the dimensions (lower/upper bounds) of the array type.
			// That said, C# doesn't provide a way to set them either, so this should work for xml documentation produced by C# at least.
			name.Append (GetDocId (tr.GetElementType ()!));
			name.Append ('[');
			for (var i = 0; i < tr.GetArrayRank (); i++) {
				if (i > 0)
					name.Append (',');
				name.Append ("0:"); // C# always produces multidimensional arrays with lower bound = 0 and no upper bound.
			}
			name.Append (']');
		} else {
			if (tr.IsNested) {
				var decl = tr.DeclaringType!;
				while (decl.IsNested) {
					name.Append (decl.Name);
					name.Append ('.');
					name.Append (name);
					decl = decl.DeclaringType!;
				}
				name.Insert (0, '.');
				name.Insert (0, decl.Namespace);
			} else {
				name.Append (tr.Namespace);
				name.Append ('.');
			}

			if (tr.IsGenericTypeDefinition) {
				name.Append (tr.Name);
			} else if (tr.IsGenericType) {
				name.Append (tr.Name, 0, tr.Name.IndexOf ('`'));
				name.Append ('{');
				var genericArguments = tr.GetGenericArguments ();
				for (var i = 0; i < genericArguments.Length; i++) {
					if (i > 0)
						name.Append (',');
					name.Append (GetDocId (genericArguments [i]));
				}
				name.Append ('}');
			} else if (tr.IsByRef) {
				name.Append (tr.GetElementType ()!.Name);
				name.Append ('@');
			} else {
				name.Append (tr.Name);
			}
		}

		return name.ToString ();
	}
}
