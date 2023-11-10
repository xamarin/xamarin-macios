using System.Collections.Generic;
using System.IO;
using System.Linq;

#nullable enable

static class ReferenceFixer {
	public static void FixSDKReferences (string sdkRoot, string sdk_offset, List<string> references) => FixSDKReferences (Path.Combine (sdkRoot, sdk_offset), references);

	public static void FixSDKReferences (string sdk_path, List<string> references, bool forceSystemDrawing = false)
	{
		FixRelativeReferences (sdk_path, references);
		AddMissingRequiredReferences (sdk_path, references, forceSystemDrawing);
	}

	static bool ContainsReference (List<string> references, string name) => references.Any (v => Path.GetFileNameWithoutExtension (v) == name);
	static void AddSDKReference (List<string> references, string sdk_path, string name) => references.Add (Path.Combine (sdk_path, name));

	static void AddMissingRequiredReferences (string sdk_path, List<string> references, bool forceSystemDrawing = false)
	{
		foreach (var requiredLibrary in new string [] { "System", "mscorlib", "System.Core" }) {
			if (!ContainsReference (references, requiredLibrary))
				AddSDKReference (references, sdk_path, requiredLibrary + ".dll");
		}
		if (forceSystemDrawing && !ContainsReference (references, "System.Drawing"))
			AddSDKReference (references, sdk_path, "System.Drawing.dll");
	}

	static bool ExistsInSDK (string sdk_path, string name) => File.Exists (Path.Combine (sdk_path, name));

	static void FixRelativeReferences (string sdk_path, List<string> references)
	{
		foreach (var r in references.Where (x => ExistsInSDK (sdk_path, x + ".dll")).ToList ()) {
			references.Remove (r);
			AddSDKReference (references, sdk_path, r + ".dll");
		}
	}
}

