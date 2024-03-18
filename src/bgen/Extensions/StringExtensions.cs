using System;
using System.Collections;

#nullable enable

// Fixes bug 27430 - btouch doesn't escape identifiers with the same name as C# keywords
public static class StringExtensions {

	public static string Quote (this string? self)
	{
		return self switch {
			null => String.Empty,
			"" => @"""""",
			_ => $"@\"{self.Replace ("\"", "\"\"")}\""
		};
	}

	public static string? RemoveArity (this string? typeName)
	{
		if (typeName is null)
			return typeName;

		var arity = typeName.IndexOf ('`');
		return arity > 0 ? typeName.Substring (0, arity) : typeName;
	}

	public static string CamelCase (this string ins)
	{
		return Char.ToUpper (ins [0]) + ins.Substring (1);
	}

	public static string PascalCase (this string ins)
	{
		return Char.ToLower (ins [0]) + ins.Substring (1);
	}

	public static string Capitalize (this string str)
	{
		if (str.StartsWith ("@", StringComparison.Ordinal))
			return char.ToUpper (str [1]) + str.Substring (2);

		return char.ToUpper (str [0]) + str.Substring (1);
	}

	public static string? GetSafeParamName (this string? paramName)
	{
		if (paramName is null)
			return paramName;

		if (!IsValidIdentifier (paramName, out var hasIllegalChars)) {
			return hasIllegalChars ? null : "@" + paramName;
		}
		return paramName;
	}

	// Since we're building against the iOS assemblies and there's no code generation there,
	// I'm bringing the implementation from:
	// mono/mcs/class//System/Microsoft.CSharp/CSharpCodeGenerator.cs
	static bool IsValidIdentifier (string? identifier, out bool hasIllegalChars)
	{
		hasIllegalChars = false;
		if (String.IsNullOrEmpty (identifier))
			return false;

		if (keywordsTable is null)
			FillKeywordTable ();

		if (keywordsTable!.Contains (identifier))
			return false;

		if (!is_identifier_start_character (identifier! [0])) {
			// if we are dealing with a number, we are ok, we can prepend @, else we have a problem
			hasIllegalChars = !Char.IsNumber (identifier [0]);
			return false;
		}

		for (int i = 1; i < identifier.Length; i++)
			if (!is_identifier_part_character (identifier [i])) {
				hasIllegalChars = true;
				return false;
			}

		return true;
	}

	static bool is_identifier_start_character (char c)
	{
		return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_' || c == '@' || Char.IsLetter (c);
	}

	static bool is_identifier_part_character (char c)
	{
		return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_' || (c >= '0' && c <= '9') || Char.IsLetter (c);
	}

	static void FillKeywordTable ()
	{
		lock (keywords) {
			if (keywordsTable is null) {
				keywordsTable = new Hashtable ();
				foreach (string keyword in keywords) {
					keywordsTable.Add (keyword, keyword);
				}
			}
		}
	}

	static Hashtable? keywordsTable;

	static string [] keywords = new string [] {
		"abstract","event","new","struct","as","explicit","null","switch","base","extern",
		"this","false","operator","throw","break","finally","out","true",
		"fixed","override","try","case","params","typeof","catch","for",
		"private","foreach","protected","checked","goto","public",
		"unchecked","class","if","readonly","unsafe","const","implicit","ref",
		"continue","in","return","using","virtual","default",
		"interface","sealed","volatile","delegate","internal","do","is",
		"sizeof","while","lock","stackalloc","else","static","enum",
		"namespace",
		"object","bool","byte","float","uint","char","ulong","ushort",
		"decimal","int","sbyte","short","double","long","string","void",
		"partial", "yield", "where"
	};
}
