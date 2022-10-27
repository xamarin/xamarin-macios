// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

using Microsoft.DotNet.XHarness.iOS.Shared.Utilities;
using Xamarin.Utils;

#nullable enable

namespace Xharness {
	public static class EvolvedProjectFileExtensions {
		// Evaluates a text and replaces '$(Variable)' with the property value specified in the 'properties' dictionary.
		// Contrary to what MSBuild does, if the variable can't be found in the dictionary, it's not replaced with
		// an empty string, instead the variable reference stays as-is.
		public static string EvaluateAsMSBuildText (this string text, Dictionary<string, string> properties)
		{
			if (text.Length < 4)
				return text;

			if (text.IndexOf ('$') == -1)
				return text;

			var sb = new StringBuilder ();

			for (var i = 0; i < text.Length; i++) {
				if (text [i] != '$' || i + 2 >= text.Length) {
					sb.Append (text [i]);
					continue;
				}

				if (text [i + 1] != '(') {
					sb.Append ('$');
					sb.Append (text [i + 1]);
					continue;
				}

				var endParenthesis = text.IndexOf (')', i + 2);
				if (endParenthesis == -1)
					continue;

				var variable = text.Substring (i + 2, endParenthesis - i - 2);
				if (properties.TryGetValue (variable, out var value)) {
					sb.Append (value);
				} else {
					// Put back the variable reference
					sb.Append ('$');
					sb.Append ('(');
					sb.Append (variable);
					sb.Append (')');
				}

				i = endParenthesis;
			}

			return sb.ToString ();
		}

		// Collect all top-level properties in a csproj. They're evaluated as they're found, so that multiple properties concatenating
		// strings work correctly. The 'forcedProperties' dictionary contains properties that are hardcoded and should be returned as
		// the dictionary says.
		public static Dictionary<string, string> CollectAndEvaluateTopLevelProperties (this XmlDocument doc, Dictionary<string, string> forcedProperties)
		{
			var collectedProperties = new Dictionary<string, string> (forcedProperties);
			var properties = doc.SelectNodes ("/Project/*[local-name() = 'PropertyGroup' and not(@Condition)]//*");
			foreach (XmlNode prop in properties) {
				if (!forcedProperties.ContainsKey (prop.Name))
					collectedProperties [prop.Name] = prop.InnerText.EvaluateAsMSBuildText (collectedProperties);
			}
			return collectedProperties;
		}

		static IEnumerable<XmlNode> SelectElementNodes (this XmlNode node, string name)
		{
			foreach (XmlNode? child in node.ChildNodes) {
				if (child == null)
					continue;

				if (child.NodeType == XmlNodeType.Element && string.Equals (child.Name, name, StringComparison.OrdinalIgnoreCase))
					yield return child;

				if (!child.HasChildNodes)
					continue;

				foreach (XmlNode descendent in child.SelectElementNodes (name))
					yield return descendent;
			}
		}

		// This is an evolved version of https://github.com/dotnet/xharness/blob/b2297d610df1ae15fc7ba8bd8c9bc0a7192aaefa/src/Microsoft.DotNet.XHarness.iOS.Shared/Utilities/ProjectFileExtensions.cs#L1168
		public static void ResolveAllPaths (this XmlDocument csproj, string project_path, Dictionary<string, string>? variableSubstitution = null)
		{
			var dir = Path.GetDirectoryName (project_path)!;
			var nodes_with_paths = new string []
			{
				"AssemblyOriginatorKeyFile",
				"CodesignEntitlements",
				"TestLibrariesDirectory",
				"HintPath",
				"RootTestsDirectory",
			};
			var attributes_with_paths = new []
			{
				new { Element = "None", Attribute = "Include", SkipLogicalName = false, },
				new { Element = "Compile", Attribute = "Include", SkipLogicalName = false, },
				new { Element = "Compile", Attribute = "Exclude", SkipLogicalName = false, },
				new { Element = "ProjectReference", Attribute = "Include", SkipLogicalName = true, },
				new { Element = "InterfaceDefinition", Attribute = "Include", SkipLogicalName = false, },
				new { Element = "BundleResource", Attribute = "Include", SkipLogicalName = false, },
				new { Element = "EmbeddedResource", Attribute = "Include", SkipLogicalName = false, },
				new { Element = "ImageAsset", Attribute = "Include", SkipLogicalName = false, },
				new { Element = "GeneratedTestInput", Attribute = "Include", SkipLogicalName = false, },
				new { Element = "GeneratedTestOutput", Attribute = "Include", SkipLogicalName = false, },
				new { Element = "TestLibrariesInput", Attribute = "Include", SkipLogicalName = false, },
				new { Element = "TestLibrariesOutput", Attribute = "Include", SkipLogicalName = false, },
				new { Element = "Content", Attribute = "Include", SkipLogicalName = false, },
				new { Element = "ObjcBindingApiDefinition", Attribute = "Include", SkipLogicalName = false, },
				new { Element = "ObjcBindingCoreSource", Attribute = "Include", SkipLogicalName = false, },
				new { Element = "ObjcBindingNativeLibrary", Attribute = "Include", SkipLogicalName = false, },
				new { Element = "ObjcBindingNativeFramework", Attribute = "Include", SkipLogicalName = false, },
				new { Element = "Import", Attribute = "Project", SkipLogicalName = true, },
				new { Element = "FilesToCopy", Attribute = "Include", SkipLogicalName = false, },
				new { Element = "FilesToCopyFoo", Attribute = "Include", SkipLogicalName = false, },
				new { Element = "FilesToCopyFooBar", Attribute = "Include", SkipLogicalName = false, },
				new { Element = "FilesToCopyEncryptedXml", Attribute = "Include", SkipLogicalName = false, },
				new { Element = "FilesToCopyCryptographyPkcs", Attribute = "Include", SkipLogicalName = false, },
				new { Element = "FilesToCopyResources", Attribute = "Include", SkipLogicalName = false, },
				new { Element = "FilesToCopyXMLFiles", Attribute = "Include", SkipLogicalName = false, },
				new { Element = "FilesToCopyChannels", Attribute = "Include", SkipLogicalName = false, },
				new { Element = "CustomMetalSmeltingInput", Attribute = "Include", SkipLogicalName = false, },
				new { Element = "Metal", Attribute = "Include", SkipLogicalName = false, },
				new { Element = "NativeReference", Attribute = "Include", SkipLogicalName = false, },
			};
			var nodes_with_variables = new string []
			{
				"MtouchExtraArgs",
			};
			Func<string, string>? convert = null;
			convert = (input) => {
				if (input.Contains (';')) {
					var split = input.Split (new char [] { ';' }, StringSplitOptions.RemoveEmptyEntries);
					for (var i = 0; i < split.Length; i++)
						split [i] = convert!.Invoke (split [i]);

					return string.Join (";", split);
				}

				if (variableSubstitution != null)
					input = input.EvaluateAsMSBuildText (variableSubstitution);

				var makeFullPath = input [0] != '$';
				if (makeFullPath) {
					input = input.Replace ('\\', '/'); // make unix-style
					input = Path.GetFullPath (Path.Combine (dir, input));
					input = input.Replace ('/', '\\'); // make windows-style again
				}

				return input;
			};

			foreach (var key in nodes_with_paths) {
				var nodes = csproj.SelectElementNodes (key);
				foreach (var node in nodes)
					node.InnerText = convert (node.InnerText);
			}

			foreach (var key in nodes_with_variables) {
				var nodes = csproj.SelectElementNodes (key);
				foreach (var node in nodes)
					node.InnerText = node.InnerText.Replace ("${ProjectDir}", StringUtils.Quote (Path.GetDirectoryName (project_path)));
			}

			foreach (var kvp in attributes_with_paths) {
				var element = kvp.Element;
				var attrib = kvp.Attribute;
				var skipLogicalName = kvp.SkipLogicalName;
				var nodes = csproj.SelectElementNodes (element);
				foreach (XmlNode node in nodes) {
					var a = node.Attributes [attrib];
					if (a == null)
						continue;

					// Fix any default LogicalName values (but don't change existing ones).
					if (!skipLogicalName) {
						var ln = node.SelectElementNodes ("LogicalName")?.SingleOrDefault ();
						var links = node.SelectElementNodes ("Link");
						if (ln == null && !links.Any ()) {
							ln = csproj.CreateElement ("LogicalName", csproj.GetNamespace ());
							node.AppendChild (ln);

							string logicalName = a.Value;
							switch (element) {
							case "BundleResource":
								if (logicalName.StartsWith ("Resources\\", StringComparison.Ordinal))
									logicalName = logicalName.Substring ("Resources\\".Length);

								break;
							default:
								break;
							}
							ln.InnerText = logicalName;
						}
					}

					a.Value = convert (a.Value);
				}
			}
		}
	}
}
