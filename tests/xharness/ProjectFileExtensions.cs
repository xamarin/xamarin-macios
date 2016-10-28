using System;
using System.Xml;

namespace xharness
{
	static class ProjectFileExtensions {
		const string MSBuild_Namespace = "http://schemas.microsoft.com/developer/msbuild/2003";

		public static void SetProjectTypeGuids (this XmlDocument csproj, string value)
		{
			SetNode (csproj, "ProjectTypeGuids", value);
		}

		public static string GetProjectGuid (this XmlDocument csproj)
		{
			return csproj.SelectSingleNode ("/*/*/*[local-name() = 'ProjectGuid']").InnerText;
		}

		public static void SetProjectGuid (this XmlDocument csproj, string value)
		{
			csproj.SelectSingleNode ("/*/*/*[local-name() = 'ProjectGuid']").InnerText = value;
		}

		public static string GetOutputType (this XmlDocument csproj)
		{
			return csproj.SelectSingleNode ("/*/*/*[local-name() = 'OutputType']").InnerText;
		}

		public static void SetOutputType (this XmlDocument csproj, string value)
		{
			csproj.SelectSingleNode ("/*/*/*[local-name() = 'OutputType']").InnerText = value;
		}

		static string[] eqsplitter = new string[] { "==" };
		static string[] orsplitter = new string[] { " Or " };
		static char[] pipesplitter = new char[] { '|' };
		static char[] trimchars = new char[] { '\'', ' ' };

		static void ParseConditions (this XmlNode node, out string platform, out string configuration)
		{
			// This parses the platform/configuration out of conditions like this:
			//
			// Condition=" '$(Configuration)|$(Platform)' == 'Debug|iPhoneSimulator' "
			//
			platform = "Any CPU";
			configuration = "Debug";
			while (node != null) {
				if (node.Attributes != null) {
					var conditionAttribute = node.Attributes ["Condition"];
					if (conditionAttribute != null) {
						var condition = conditionAttribute.Value;
						var eqsplit = condition.Split (eqsplitter, StringSplitOptions.None);
						if (eqsplit.Length == 2) {
							var left = eqsplit [0].Trim (trimchars).Split (pipesplitter);
							var right = eqsplit [1].Trim (trimchars).Split (pipesplitter);
							if (left.Length == right.Length) {
								for (int i = 0; i < left.Length; i++) {
									switch (left [i]) {
									case "$(Configuration)":
										configuration = right [i];
										break;
									case "$(Platform)":
										platform = right [i];
										break;
									default:
										throw new Exception (string.Format ("Unknown condition logic: {0}", left [i]));
									}
								}
							}
						}

						if (string.IsNullOrEmpty (platform) || string.IsNullOrEmpty (configuration))
							throw new Exception (string.Format ("Could not parse the condition: {0}", conditionAttribute.Value));
					}
				}
				node = node.ParentNode;
			}

			if (string.IsNullOrEmpty (platform) || string.IsNullOrEmpty (configuration))
				throw new Exception ("Could not find a condition attribute.");
		}

		public static void SetOutputPath (this XmlDocument csproj, string value)
		{
			var nodes = csproj.SelectNodes ("/*/*/*[local-name() = 'OutputPath']");
			if (nodes.Count == 0)
				throw new Exception ("Could not find node OutputPath");
			foreach (XmlNode n in nodes) {
				// OutputPath needs to be expanded, otherwise Xamarin Studio isn't able to launch the project.
				string platform, configuration;
				ParseConditions (n, out platform, out configuration);
				n.InnerText = value.Replace ("$(Platform)", platform).Replace ("$(Configuration)", configuration);
			}
		}

		static bool IsNodeApplicable (XmlNode node, string platform, string configuration)
		{
			while (node != null) {
				if (!EvaluateCondition (node, platform, configuration))
					return false;
				node = node.ParentNode;
			}
			return true;
		}

		static bool EvaluateCondition (XmlNode node, string platform, string configuration)
		{
			if (node.Attributes == null)
				return true;
			
			var condition = node.Attributes ["Condition"];
			if (condition == null)
				return true;
				
			var conditionValue = condition.Value;
			conditionValue = conditionValue.Replace ("$(Configuration)", configuration).Replace ("$(Platform)", platform);

			var orsplits = conditionValue.Split (orsplitter, StringSplitOptions.None);
			foreach (var orsplit in orsplits) {
				var eqsplit = orsplit.Split (eqsplitter, StringSplitOptions.None);
				if (eqsplit.Length != 2) {
					Console.WriteLine ("Could not parse condition; {0}", conditionValue);
					return false;
				}

				var left = eqsplit [0].Trim (trimchars);
				var right = eqsplit [1].Trim (trimchars);
				if (left == right)
					return true;
			}

			return false;
		}

		public static string GetOutputPath (this XmlDocument csproj, string platform, string configuration)
		{
			var nodes = csproj.SelectNodes ("/*/*/*[local-name() = 'OutputPath']");
			if (nodes.Count == 0)
				throw new Exception ("Could not find node OutputPath");
			
			foreach (XmlNode n in nodes) {
				if (IsNodeApplicable (n, platform, configuration))
					return n.InnerText.Replace ("$(Platform)", platform).Replace ("$(Configuration)", configuration);
			}
			throw new Exception ("Could not find OutputPath");
		}

		public static void SetIntermediateOutputPath (this XmlDocument csproj, string value)
		{
			var project = csproj.ChildNodes [1];
			var property_group = project.ChildNodes [0];

			var intermediateOutputPath = csproj.CreateElement ("IntermediateOutputPath", MSBuild_Namespace);
			intermediateOutputPath.InnerText = value;
			property_group.AppendChild (intermediateOutputPath);
		}

		public static void SetTargetFrameworkIdentifier (this XmlDocument csproj, string value)
		{
			SetTopLevelPropertyGroupValue (csproj, "TargetFrameworkIdentifier", value);
		}

		public static void SetTopLevelPropertyGroupValue (this XmlDocument csproj, string key, string value)
		{
			var firstPropertyGroups = csproj.SelectNodes ("//*[local-name() = 'PropertyGroup']")[0];
			var targetFrameworkIdentifierNode = firstPropertyGroups.SelectSingleNode (string.Format ("//*[local-name() = '{0}']", key));
			if (targetFrameworkIdentifierNode != null)
			{
				SetNode (csproj, key, value);
			}
			else
			{
				var mea = csproj.CreateElement (key, MSBuild_Namespace);
				mea.InnerText = value;
				firstPropertyGroups.AppendChild (mea);
			}
 		}

		public static void RemoveTargetFrameworkIdentifier (this XmlDocument csproj)
		{
			RemoveNode (csproj, "TargetFrameworkIdentifier");
		}

		public static void SetAssemblyName (this XmlDocument csproj, string value)
		{
			SetNode (csproj, "AssemblyName", value);
		}

		public static string GetAssemblyName (this XmlDocument csproj)
		{
			return csproj.SelectSingleNode ("/*/*/*[local-name() = 'AssemblyName']").InnerText;
		}

		public static void SetPlatformAssembly (this XmlDocument csproj, string value)
		{
			SetAssemblyReference (csproj, "Xamarin.iOS", value);
		}

		public static void SetAssemblyReference (this XmlDocument csproj, string current, string value)
		{
			var project = csproj.ChildNodes [1];
			var reference = csproj.SelectSingleNode ("/*/*/*[local-name() = 'Reference' and @Include = '" + current + "']");
			if (reference != null)
				reference.Attributes ["Include"].Value = value;
		}

		public static void RemoveReferences (this XmlDocument csproj, string projectName)
		{
			var reference = csproj.SelectSingleNode ("/*/*/*[local-name() = 'Reference' and @Include = '" + projectName + "']");
			if (reference != null)
				reference.ParentNode.RemoveChild (reference);
		}

		public static void SetHintPath (this XmlDocument csproj, string current, string value)
		{
			var project = csproj.ChildNodes [1];
			var reference = csproj.SelectSingleNode ("/*/*/*[local-name() = 'Reference' and @Include = '" + current + "']");
			if (reference != null) {
				var hintPath = csproj.CreateElement ("HintPath", MSBuild_Namespace);
				hintPath.InnerText = value;
				reference.AppendChild (hintPath);
			}
		}

		public static void AddCompileInclude (this XmlDocument csproj, string link, string include)
		{
			var compile_node = csproj.SelectSingleNode ("//*[local-name() = 'Compile']");
			var item_group = compile_node.ParentNode;

			var node = csproj.CreateElement ("Compile", MSBuild_Namespace);
			var include_attribute = csproj.CreateAttribute ("Include");
			include_attribute.Value = include;
			node.Attributes.Append (include_attribute);
			var linkElement = csproj.CreateElement ("Link", MSBuild_Namespace);
			linkElement.InnerText = link;
			node.AppendChild (linkElement);
			item_group.AppendChild (node);
		}

		public static void SetImport (this XmlDocument csproj, string value)
		{
			var imports = csproj.SelectNodes ("/*/*[local-name() = 'Import']");
			if (imports.Count != 1)
				throw new Exception ("More than one import");
			imports [0].Attributes ["Project"].Value = value;
		}

		public static void SetExtraLinkerDefs (this XmlDocument csproj, string value)
		{
			var mtouchExtraArgs = csproj.SelectNodes ("//*[local-name() = 'MtouchExtraArgs']");
			foreach (XmlNode mea in mtouchExtraArgs)
				mea.InnerText = mea.InnerText.Replace ("extra-linker-defs.xml", value);
			var nones = csproj.SelectNodes ("//*[local-name() = 'None' and @Include = 'extra-linker-defs.xml']");
			foreach (XmlNode none in nones)
				none.Attributes ["Include"].Value = value;
		}

		public static void AddExtraMtouchArgs (this XmlDocument csproj, string value, string platform, string configuration)
		{
			var mtouchExtraArgs = csproj.SelectNodes ("//*[local-name() = 'MtouchExtraArgs']");
			var found = false;
			foreach (XmlNode mea in mtouchExtraArgs) {
				if (!IsNodeApplicable (mea, platform, configuration))
					continue;

				mea.InnerText += " " + value;
				found = true;
			}

			if (found)
				return;

			// Not all projects have a MtouchExtraArgs node, so create one of none was found.
			var propertyGroups = csproj.SelectNodes ("//*[local-name() = 'PropertyGroup' and @Condition]");
			foreach (XmlNode pg in propertyGroups) {
				if (!EvaluateCondition (pg, platform, configuration))
					continue;

				var mea = csproj.CreateElement ("MtouchExtraArgs", MSBuild_Namespace);
				mea.InnerText = value;
				pg.AppendChild (mea);
			}
		}

		public static void SetMtouchUseLlvm (this XmlDocument csproj, bool value, string platform, string configuration)
		{
			SetNode (csproj, "MtouchUseLlvm", true ? "true" : "false", platform, configuration);
		}

		public static void SetMtouchUseBitcode (this XmlDocument csproj, bool value, string platform, string configuration)
		{
			SetNode (csproj, "MtouchEnableBitcode", true ? "true" : "false", platform, configuration);
		}

		public static void SetNode (this XmlDocument csproj, string node, string value, string platform, string configuration)
		{
			var projnode = csproj.SelectNodes ("//*[local-name() = '" + node + "']");
			var found = false;
			foreach (XmlNode xmlnode in projnode) {
				if (!IsNodeApplicable (xmlnode, platform, configuration))
					continue;

				xmlnode.InnerText = value;
				found = true;
			}

			if (found)
				return;

			// Not all projects have a MtouchExtraArgs node, so create one of none was found.
			var propertyGroups = csproj.SelectNodes ("//*[local-name() = 'PropertyGroup' and @Condition]");
			foreach (XmlNode pg in propertyGroups) {
				if (!EvaluateCondition (pg, platform, configuration))
					continue;

				var mea = csproj.CreateElement (node, MSBuild_Namespace);
				mea.InnerText = value;
				pg.AppendChild (mea);
			}
		}


		public static string GetImport (this XmlDocument csproj)
		{
			var imports = csproj.SelectNodes ("/*/*[local-name() = 'Import']");
			if (imports.Count != 1)
				throw new Exception ("More than one import");
			return imports [0].Attributes ["Project"].Value;
		}

		public static void FixProjectReferences (this XmlDocument csproj, string suffix, Func<string, bool> fixCallback = null)
		{
			var nodes = csproj.SelectNodes ("/*/*/*[local-name() = 'ProjectReference']");
			if (nodes.Count == 0)
				return;
			foreach (XmlNode n in nodes) {
				var name = n ["Name"].InnerText;
				if (fixCallback != null && !fixCallback (name))
					continue;
				var include = n.Attributes ["Include"];
				include.Value = include.Value.Replace (".csproj", suffix + ".csproj");
				include.Value = include.Value.Replace (".fsproj", suffix + ".fsproj");
				var nameElement = n ["Name"];
				nameElement.InnerText = System.IO.Path.GetFileNameWithoutExtension (include.Value);
			}
		}

		public static void FixTestLibrariesReferences (this XmlDocument csproj, string platform)
		{
			var nodes = csproj.SelectNodes ("//*[local-name() = 'ObjcBindingNativeLibrary']");
			foreach (XmlNode node in nodes) {
				var includeAttribute = node.Attributes ["Include"];
				if (includeAttribute != null)
					includeAttribute.Value = includeAttribute.Value.Replace ("test-libraries\\.libs\\ios\\libtest.a", "test-libraries\\.libs\\" + platform + "\\libtest.a");
			}
			nodes = csproj.SelectNodes ("//*[local-name() = 'Target' and @Name = 'BeforeBuild']");
			foreach (XmlNode node in nodes) {
				var outputsAttribute = node.Attributes ["Outputs"];
				if (outputsAttribute != null)
					outputsAttribute.Value = outputsAttribute.Value.Replace ("test-libraries\\.libs\\ios\\libtest.a", "test-libraries\\.libs\\" + platform + "\\libtest.a");
			}
		}

		public static void FixArchitectures (this XmlDocument csproj, string simulator_arch, string device_arch)
		{
			var nodes = csproj.SelectNodes ("/*/*/*[local-name() = 'MtouchArch']");
			if (nodes.Count == 0)
				throw new Exception (string.Format ("Could not find MtouchArch at all"));
			foreach (XmlNode n in nodes) {
				switch (n.InnerText.ToLower ()) {
				case "i386":
				case "x86_64":
				case "i386, x86_64":
					n.InnerText = simulator_arch;
					break;
				case "armv7":
				case "armv7s":
				case "arm64":
				case "armv7, arm64":
					n.InnerText = device_arch;
					break;
				default:
					throw new NotImplementedException (string.Format ("Unhandled architecture: {0}", n.Value));

				}
			}
		}

		public static void FindAndReplace (this XmlDocument csproj, string find, string replace)
		{
			FindAndReplace (csproj.ChildNodes, find, replace);
		}

		static void FindAndReplace (XmlNode node, string find, string replace)
		{
			if (node.HasChildNodes) {
				FindAndReplace (node.ChildNodes, find, replace);
			} else {
				if (node.NodeType == XmlNodeType.Text)
					node.InnerText = node.InnerText.Replace (find, replace);
			}
			if (node.Attributes != null) {
				foreach (XmlAttribute attrib in node.Attributes)
					attrib.Value = attrib.Value.Replace (find, replace);
			}
		}

		static void FindAndReplace (XmlNodeList nodes, string find, string replace)
		{
			foreach (XmlNode node in nodes)
				FindAndReplace (node, find, replace);
		}

		public static void FixInfoPListInclude (this XmlDocument csproj, string suffix)
		{
			var import = csproj.SelectSingleNode ("/*/*/*[local-name() = 'None' and @Include = 'Info.plist']");
			import.Attributes ["Include"].Value = "Info" + suffix + ".plist";
			var logicalName = csproj.CreateElement ("LogicalName", MSBuild_Namespace);
			logicalName.InnerText = "Info.plist";
			import.AppendChild (logicalName);
		}

		public static string GetInfoPListInclude (this XmlDocument csproj)
		{
			var logicalNames = csproj.SelectNodes ("//*[local-name() = 'LogicalName']");
			foreach (XmlNode ln in logicalNames) {
				if (ln.InnerText != "Info.plist")
					continue;
				return ln.ParentNode.Attributes ["Include"].Value;
			}
			var nones = csproj.SelectNodes ("//*[local-name() = 'None' and @Include = 'Info.plist']");
			if (nones.Count > 0)
				return "Info.plist";
			throw new Exception ("Could not find Info.plist include");
		}

		public static void SetProjectReferenceValue (this XmlDocument csproj, string projectInclude, string node, string value)
		{
			var nameNode = csproj.SelectSingleNode ("//*[local-name() = 'ProjectReference' and @Include = '" + projectInclude + "']/*[local-name() = '" + node + "']");
			nameNode.InnerText = value;
		}

		public static void CreateProjectReferenceValue (this XmlDocument csproj, string existingInclude, string path, string guid, string name)
		{
			var referenceNode = csproj.SelectSingleNode ("//*[local-name() = 'Reference' and @Include = '" + existingInclude + "']");
			var projectReferenceNode = csproj.CreateElement ("ProjectReference", MSBuild_Namespace);
			var includeAttribute = csproj.CreateAttribute ("Include");
			includeAttribute.Value = path.Replace ('/', '\\');
			projectReferenceNode.Attributes.Append (includeAttribute);
			var projectNode = csproj.CreateElement ("Project", MSBuild_Namespace);
			projectNode.InnerText = guid;
			projectReferenceNode.AppendChild (projectNode);
			var nameNode = csproj.CreateElement ("Name", MSBuild_Namespace);
			nameNode.InnerText = name;
			projectReferenceNode.AppendChild (nameNode);

			XmlNode itemGroup;
			if (referenceNode != null) {
				itemGroup = referenceNode.ParentNode;
				referenceNode.ParentNode.RemoveChild (referenceNode);
			} else {
				itemGroup = csproj.CreateElement ("ItemGroup", MSBuild_Namespace);
				csproj.SelectSingleNode ("//*[local-name() = 'Project']").AppendChild (itemGroup);
			}
			itemGroup.AppendChild (projectReferenceNode);
		}

		public static void AddAdditionalDefines (this XmlDocument csproj, string value)
		{
			var mainPropertyGroup = csproj.SelectSingleNode ("//*[local-name() = 'PropertyGroup' and not(@Condition)]");
			var mainDefine = mainPropertyGroup.SelectSingleNode ("/*[local-name() = 'DefineConstants']");
			if (mainDefine == null) {
				mainDefine = csproj.CreateElement ("DefineConstants", MSBuild_Namespace);
				mainDefine.InnerText = value;
				mainPropertyGroup.AppendChild (mainDefine);
			} else {
				mainDefine.InnerText = mainDefine.InnerText + ";" + value;
			}

			// make sure all other DefineConstants include the main one
			var otherDefines = csproj.SelectNodes ("//*[local-name() = 'PropertyGroup' and @Condition]/*[local-name() = 'DefineConstants']");
			foreach (XmlNode def in otherDefines) {
				if (!def.InnerText.Contains ("$(DefineConstants"))
					def.InnerText = def.InnerText + ";$(DefineConstants)";
			}
		}

		public static void SetNode (this XmlDocument csproj, string node, string value)
		{
			var nodes = csproj.SelectNodes ("/*/*/*[local-name() = '" + node + "']");
			if (nodes.Count == 0)
				throw new Exception (string.Format ("Could not find node {0}", node));
			foreach (XmlNode n in nodes) {
				n.InnerText = value;
			}
		}

		public static void RemoveNode (this XmlDocument csproj, string node)
		{
			var nodes = csproj.SelectNodes ("/*/*/*[local-name() = '" + node + "']");
			if (nodes.Count == 0)
				throw new Exception (string.Format ("Could not find node {0}", node));
			foreach (XmlNode n in nodes) {
				n.ParentNode.RemoveChild (n);
			}
		}

		public static void CloneConfiguration (this XmlDocument csproj, string platform, string configuration, string new_configuration)
		{
			var projnode = csproj.SelectNodes ("//*[local-name() = 'PropertyGroup']");
			foreach (XmlNode xmlnode in projnode) {
				if (xmlnode.Attributes ["Condition"] == null)
					continue;
				if (!IsNodeApplicable (xmlnode, platform, configuration))
					continue;
				
				var clone = xmlnode.Clone ();
				var condition = clone.Attributes ["Condition"];
				condition.InnerText = condition.InnerText.Replace (configuration, new_configuration);
				xmlnode.ParentNode.InsertAfter (clone, xmlnode);
				return;
			}

			throw new Exception ("Configuration not found.");
		}

		public static void DeleteConfiguration (this XmlDocument csproj, string platform, string configuration)
		{
			var projnode = csproj.SelectNodes ("//*[local-name() = 'PropertyGroup']");
			foreach (XmlNode xmlnode in projnode) {
				if (xmlnode.Attributes ["Condition"] == null)
					continue;
				if (!IsNodeApplicable (xmlnode, platform, configuration))
					continue;
				xmlnode.ParentNode.RemoveChild (xmlnode);

				return;
			}

			throw new Exception ("Configuration not found.");
		}
	}
}

