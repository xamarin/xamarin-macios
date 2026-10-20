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
	Dictionary<string, XmlNode>? memberLookup;

	public DocumentationManager (string assembly)
	{
		this.xml = Path.ChangeExtension (assembly, ".xml");
		if (File.Exists (xml)) {
			var doc = new XmlDocument ();
			doc.LoadWithoutNetworkAccess (xml);
			// Pre-build a dictionary for O(1) member lookups instead of
			// O(n) XPath queries on every TryGetDocumentation call.
			var members = doc.SelectNodes ("/doc/members/member[@name]");
			if (members is not null) {
				memberLookup = new Dictionary<string, XmlNode> (members.Count);
				foreach (XmlNode member in members) {
					var name = member.Attributes? ["name"]?.Value;
					if (name is not null)
						memberLookup [name] = member;
				}
			}
		}
	}

	public bool WriteDocumentation (StreamWriter sw, int indent, MemberInfo member, Func<XmlNode, XmlNode>? transformNode = null)
	{
		if (!TryGetDocumentation (member, out var docs, transformNode))
			return false;

		foreach (var line in docs) {
			sw.Write ('\t', indent);
			sw.WriteLine (line);
		}

		return true;
	}

	public bool TryGetDocumentation (MemberInfo member, [NotNullWhen (true)] out string []? documentation, Func<XmlNode, XmlNode>? transformNode = null)
	{
		documentation = null;

		if (memberLookup is null)
			return false;

		if (!TryGetId (member, out var id))
			return false;

		if (!memberLookup.TryGetValue (id, out var node))
			return false;

		if (transformNode is not null)
			node = transformNode (node);

		// Remove indentation, make triple-slash comments
		var lines = node.InnerXml.Split ('\n', '\r');
		for (var i = 0; i < lines.Length; i++) {
			var line = lines [i].TrimStart (' ');

			// The build compiles code twice:
			// 1. The API bindings
			// 2. The final (generated) binding code
			// Any xml documentation from the API definitions are copied to the generated binding code,
			// but since the xml docs in the API definitions may refer to APIs that don't exist yet
			// (they will be generated), we ignore any warnings from the C# compiler about failures to
			// resolve crefs in API definitions. However, the C# compiler will modify any such cref
			// attribute, and prefix them with '?:' - which presumably means "unresolved". The problem
			// is that any such cref are not validated by the C# compiler when we compile the final
			// bindings, effectively producing crefs that point to supposedly inexistent APIs.
			// So what we do here is to undo the '?:' prefixing from the compiled API definitions, so
			// that when we compile the generated bindings, the C# compiler validates all the crefs.
			var needle = "cref=\"!:";
			var idx = line.IndexOf (needle);
			while (idx >= 0) {
				var idx2 = line.IndexOf ('"', idx + needle.Length);
				if (idx2 < idx)
					break;
				var cref = line [(idx + needle.Length)..idx2];

				// Fixup this:
				// error CS1584: XML comment has syntactically incorrect cref attribute
				// UIKit.UIApplication.Notifications.ObserveContentSizeCategoryChanged(EventHandler&amp;lt;UIContentSizeCategoryChangedEventArgs&amp;gt;)
				cref = cref.Replace ("&amp;lt;", "{");
				cref = cref.Replace ("&amp;gt;", "}");

				// replace the existing cref with the fixed version
				line = line [..idx] + "cref=\"" + cref + "\"" + line [(idx2 + 1)..];

				// there can be more than one cref per line, so keep looking
				idx = line.IndexOf (needle, idx + needle.Length);
			}

			lines [i] = "/// " + line;
		}

		documentation = lines;

		return true;
	}

	// https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/xmldoc/#id-strings
	// See tests/cecil-tests/Documentation.cs for an implementation that works with Cecil.
	// There's already an implementation in Roslyn, but that's a rather heavy dependency,
	// so we're implementing this in our own code instead.

	public static string GetDocId (MethodInfo md, bool includeDeclaringType = true, bool alwaysIncludeParenthesis = false, Func<ParameterInfo, Type>? parameterTypeProvider = null)
	{
		var methodName = md.Name.Replace ('.', '#');
		var name = methodName;
		if (includeDeclaringType)
			name = GetDocId (md.DeclaringType!) + "." + name;
		if (md.IsGenericMethodDefinition)
			name += $"``{md.GetGenericArguments ().Length}";
		var parameters = md.GetParameters ();
		if (parameters.Length > 0) {
			name += "(" + string.Join (",", parameters.Select (p => GetDocId (parameterTypeProvider?.Invoke (p) ?? p.ParameterType))) + ")";
		} else if (alwaysIncludeParenthesis) {
			name += "()";
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
			if (tr.DeclaringMethod is not null) {
				name.Append ("``");
			} else {
				name.Append ('`');
			}
			name.Append (tr.GenericParameterPosition);
		} else if (tr.IsSZArray) {
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
		} else if (tr.IsByRef) {
			name.Append (GetDocId (tr.GetElementType ()!));
			name.Append ('@');
		} else {
			if (tr.IsNested) {
				var decl = tr.DeclaringType!;
				while (true) {
					name.Insert (0, '.');
					name.Insert (0, decl.Name);
					if (!decl.IsNested)
						break;
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
			} else {
				name.Append (tr.Name);
			}
		}

		return name.ToString ();
	}
}
