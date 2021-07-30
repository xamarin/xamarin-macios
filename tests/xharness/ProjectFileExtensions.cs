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

		static IEnumerable<XmlNode> SelectElementNodes (this XmlNode node, string name)
		{
			foreach (XmlNode? child in node.ChildNodes) {
				if (child == null)
					continue;

				if (child.NodeType == XmlNodeType.Element && child.Name == name)
					yield return child;

				if (!child.HasChildNodes)
					continue;

				foreach (XmlNode descendent in child.SelectElementNodes (name))
					yield return descendent;
			}
		}

		// This is an evolved version of https://github.com/dotnet/xharness/blob/b2297d610df1ae15fc7ba8bd8c9bc0a7192aaefa/src/Microsoft.DotNet.XHarness.iOS.Shared/Utilities/ProjectFileExtensions.cs#L1168
		public static void ResolveAllPaths (this XmlDocument csproj, string project_path, string? rootDirectory = null)
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
			var attributes_with_paths = new string [] []
			{
				new string [] { "None", "Include" },
				new string [] { "Compile", "Include" },
				new string [] { "Compile", "Exclude" },
				new string [] { "ProjectReference", "Include" },
				new string [] { "InterfaceDefinition", "Include" },
				new string [] { "BundleResource", "Include" },
				new string [] { "EmbeddedResource", "Include" },
				new string [] { "ImageAsset", "Include" },
				new string [] { "GeneratedTestInput", "Include" },
				new string [] { "GeneratedTestOutput", "Include" },
				new string [] { "TestLibrariesInput", "Include" },
				new string [] { "TestLibrariesOutput", "Include" },
				new string [] { "Content", "Include" },
				new string [] { "ObjcBindingApiDefinition", "Include" },
				new string [] { "ObjcBindingCoreSource", "Include" },
				new string [] { "ObjcBindingNativeLibrary", "Include" },
				new string [] { "ObjcBindingNativeFramework", "Include" },
				new string [] { "Import", "Project", "CustomBuildActions.targets", "..\\shared.targets" },
				new string [] { "FilesToCopy", "Include" },
				new string [] { "FilesToCopyFoo", "Include" },
				new string [] { "FilesToCopyFooBar", "Include" },
				new string [] { "FilesToCopyEncryptedXml", "Include" },
				new string [] { "FilesToCopyCryptographyPkcs", "Include" },
				new string [] { "FilesToCopyResources", "Include" },
				new string [] { "FilesToCopyXMLFiles", "Include" },
				new string [] { "FilesToCopyChannels", "Include" },
				new string [] { "CustomMetalSmeltingInput", "Include" },
				new string [] { "Metal", "Include" },
				new string [] { "NativeReference", "Include" },
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

				if (input [0] == '/') {
					return input; // This is already a full path.
				}

				input = input.Replace ('\\', '/'); // make unix-style

				if (rootDirectory != null) {
					input = input.Replace ("$(RootTestsDirectory)", rootDirectory);
				}

				// Don't process anything that starts with a variable, it's either a full path already, or the variable will be updated according to the new location
				if (input.StartsWith ("$(", StringComparison.Ordinal))
					return input;

				input = Path.GetFullPath (Path.Combine (dir, input));
				input = input.Replace ('/', '\\'); // make windows-style again
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
				var element = kvp [0];
				var attrib = kvp [1];
				var nodes = csproj.SelectElementNodes (element);
				foreach (XmlNode node in nodes) {
					var a = node.Attributes [attrib];
					if (a == null)
						continue;

					// entries after index 2 is a list of values to filter the attribute value against.
					var found = kvp.Length == 2;
					var skipLogicalName = kvp.Length > 2;
					for (var i = 2; i < kvp.Length; i++)
						found |= a.Value == kvp [i];

					if (!found)
						continue;

					// Fix any default LogicalName values (but don't change existing ones).
					var ln = node.SelectElementNodes ("LogicalName")?.SingleOrDefault ();
					var links = node.SelectElementNodes ("Link");
					if (!skipLogicalName && ln == null && !links.Any ()) {
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

					a.Value = convert (a.Value);
				}
			}
		}
	}
}
