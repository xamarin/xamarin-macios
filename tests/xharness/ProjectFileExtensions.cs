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
		public static void SetProperty (this XmlDocument csproj, string key, string value, bool last = true)
		{
			// Set all existing properties
			var xmlNodeList = csproj.SelectNodes ("/*[local-name() = 'Project']/*[local-name() = 'PropertyGroup']/*[local-name() = '" + key + "']")!.Cast<XmlNode> ();
			foreach (var item in xmlNodeList)
				item.InnerText = value;

			// Create a new one as well, in case any of the other ones are for a different configuration.
			var propertyGroup = GetSpecificPropertyGroup (csproj, last);
			var mea = csproj.CreateElement (key, csproj.GetNamespace ());
			mea.InnerText = value;
			propertyGroup.AppendChild (mea);
			propertyGroup.InsertBefore (csproj.CreateComment ($" This property was created by xharness "), mea);
		}

		public static void AppendToProperty (this XmlDocument csproj, string node, string value, string separator)
		{
			var propertyGroup = GetLastPropertyGroup (csproj);
			var newNode = csproj.CreateElement (node, csproj.GetNamespace ());
			newNode.InnerText = $"$({node}){separator}{value}";
			propertyGroup.AppendChild (newNode);
			propertyGroup.InsertBefore (csproj.CreateComment ($" This property was created by xharness "), newNode);
		}

		public static void AppendExtraMtouchArgs (this XmlDocument csproj, string value)
		{
			csproj.AppendToProperty ("MtouchExtraArgs", value, " ");
		}

		public static void AppendMonoBundlingExtraArgs (this XmlDocument csproj, string value)
		{
			csproj.AppendToProperty ("MonoBundlingExtraArgs", value, " ");
		}

		static int IndexOf (this XmlNodeList @this, XmlNode node)
		{
			for (var i = 0; i < @this.Count; i++) {
				if ((object?) node == (object?) @this [i])
					return i;
			}
			return -1;
		}

		static XmlElement GetLastPropertyGroup (this XmlDocument csproj)
		{
			return GetSpecificPropertyGroup (csproj, true);
		}

		static XmlElement GetSpecificPropertyGroup (this XmlDocument csproj, bool last /* or first */)
		{
			// If last:
			// 		Is the last property group Condition-less? If so, return it.
			// 		Definition of last: the last PropertyGroup before an Import node (or last in file if there are no Import nodes)
			var propertyGroups = csproj.SelectNodes ("/*[local-name() = 'Project']/*[local-name() = 'PropertyGroup']")!.Cast<XmlElement> ();
			var imports = csproj.SelectNodes ("/*[local-name() = 'Project']/*[local-name() = 'Import']")!.Cast<XmlElement> ();
			if (propertyGroups.Any ()) {
				XmlElement? specific = null;

				if (last && imports.Any ()) {
					var firstImport = imports.First ();
					var firstImportIndex = firstImport.ParentNode!.ChildNodes.IndexOf (firstImport);
					foreach (var pg in propertyGroups) {
						var pgIndex = pg.ParentNode!.ChildNodes.IndexOf (pg);
						if (pgIndex < firstImportIndex) {
							specific = pg;
						} else {
							break;
						}
					}
				} else {
					specific = last ? propertyGroups.Last () : propertyGroups.First ();
				}

				if (specific?.HasAttribute ("Condition") == false)
					return specific;
			}

			// Create a new PropertyGroup, and add it either:
			// If last:
			//     * Just before the first Import node
			//     * If no Import node, then after the last PropertyGroup.
			// If first:
			//     * At the very top, before the first PropertyGroup
			var projectNode = csproj.SelectSingleNode ("//*[local-name() = 'Project']")!;
			var newPropertyGroup = csproj.CreateElement ("PropertyGroup", csproj.GetNamespace ());
			if (last && imports.Any ()) {
				projectNode.InsertBefore (newPropertyGroup, imports.First ());
			} else {
				var allPropertyGroups = csproj.SelectNodes ("/*[local-name() = 'Project']/*[local-name() = 'PropertyGroup']")!.Cast<XmlNode> ();
				if (last) {
					projectNode.InsertAfter (newPropertyGroup, allPropertyGroups.Last ());
				} else {
					projectNode.InsertBefore (newPropertyGroup, allPropertyGroups.First ());
				}
			}
			projectNode.InsertBefore (csproj.CreateComment ($" This property group was created by xharness "), newPropertyGroup);
			return newPropertyGroup;
		}

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
			var properties = doc.SelectNodes ("/Project/*[local-name() = 'PropertyGroup' and not(@Condition)]//*")!;
			foreach (XmlNode? prop in properties) {
				if (prop is null)
					continue;
				if (!forcedProperties.ContainsKey (prop.Name))
					collectedProperties [prop.Name] = prop.InnerText.EvaluateAsMSBuildText (collectedProperties);
			}
			return collectedProperties;
		}

		static IEnumerable<XmlNode> SelectElementNodes (this XmlNode node, string name)
		{
			foreach (XmlNode? child in node.ChildNodes) {
				if (child is null)
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

				if (variableSubstitution is not null)
					input = input.EvaluateAsMSBuildText (variableSubstitution);

				var makeFullPath = input [0] != '$';
				if (makeFullPath) {
					input = input.Replace ('\\', '/'); // make unix-style
					input = Path.GetFullPath (Path.Combine (dir, input));
					var root = HarnessConfiguration.RootDirectory;
					if (input.Contains (root)) {
						input = input.Replace (root, "$(RootTestsDirectory)");
					} else if (input.Contains (Path.GetDirectoryName (root)!)) {
						input = input.Replace (Path.GetDirectoryName (root)!, "$(RootTestsDirectory)/..");
					}

					if (input == "")
						input = "./";
					input = input.Replace ('/', '\\'); // make windows-style again
				}

				return input;
			};

			var rootTestsDirectoryNodes = csproj.SelectElementNodes ("RootTestsDirectory");
			foreach (var node in rootTestsDirectoryNodes)
				node.InnerText = HarnessConfiguration.RootDirectory;

			foreach (var key in nodes_with_paths) {
				var nodes = csproj.SelectElementNodes (key);
				foreach (var node in nodes)
					node.InnerText = convert (node.InnerText);
			}

			foreach (var key in nodes_with_variables) {
				var nodes = csproj.SelectElementNodes (key);
				foreach (var node in nodes) {
					node.InnerText = node.InnerText.Replace ("${ProjectDir}", StringUtils.Quote (HarnessConfiguration.InjectRootTestsDirectory (Path.GetFullPath (Path.GetDirectoryName (project_path)!))));
				}
			}

			foreach (var kvp in attributes_with_paths) {
				var element = kvp.Element;
				var attrib = kvp.Attribute;
				var skipLogicalName = kvp.SkipLogicalName;
				var nodes = csproj.SelectElementNodes (element);
				foreach (XmlNode? node in nodes) {
					if (node is null)
						continue;
					var a = node.Attributes? [attrib];
					if (a is null)
						continue;

					// Fix any default LogicalName values (but don't change existing ones).
					if (!skipLogicalName) {
						var ln = node.SelectElementNodes ("LogicalName")?.SingleOrDefault ();
						var links = node.SelectElementNodes ("Link");
						if (ln is null && !links.Any ()) {
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
