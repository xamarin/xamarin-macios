using System.Collections.Generic;
using System.Xml;

namespace Xamarin.Tests {
	public class MSBuildProject {
		const string MSBuild_Namespace = "http://schemas.microsoft.com/developer/msbuild/2003";

		TestBase testBase;
		public ProjectPaths ProjectPaths { get; private set; }

		public MSBuildProject (ProjectPaths paths, TestBase testBase)
		{
			ProjectPaths = paths;
			this.testBase = testBase;
		}

		public MSBuildItem [] GetItems (string name)
		{
			return testBase.Engine.GetItems (ProjectPaths, name);
		}

		public void SetProperty (string name, string value)
		{
			var doc = new XmlDocument ();
			doc.Load (ProjectPaths.ProjectCSProjPath);
			var project = doc.SelectSingleNode ($"//*[local-name() = 'Project']");
			var propertyGroup = doc.CreateElement ("PropertyGroup", MSBuild_Namespace);
			var property = doc.CreateElement (name, MSBuild_Namespace);
			property.InnerText = value;
			propertyGroup.AppendChild (property);
			project.AppendChild (propertyGroup);
			doc.Save (ProjectPaths.ProjectCSProjPath);
		}

		public string GetPropertyValue (string name)
		{
			return testBase.Engine.GetPropertyValue (name).Replace ('\\', '/');
		}

		public void AddItem (string name, string value, Dictionary<string, string> metadata = null)
		{
			var doc = new XmlDocument ();
			doc.Load (ProjectPaths.ProjectCSProjPath);
			var project = doc.SelectSingleNode ($"//*[local-name() = 'Project']");
			var itemGroup = doc.CreateElement ("ItemGroup", MSBuild_Namespace);
			var item = doc.CreateElement (name, MSBuild_Namespace);
			item.SetAttribute ("Include", value);
			if (metadata is not null) {
				foreach (var kvp in metadata) {
					var m = doc.CreateElement (kvp.Key, MSBuild_Namespace);
					m.InnerText = kvp.Value;
					item.AppendChild (m);
				}
			}
			itemGroup.AppendChild (item);
			project.AppendChild (itemGroup);
			doc.Save (ProjectPaths.ProjectCSProjPath);
		}

		public void RemoveItems (string name)
		{
			var doc = new XmlDocument ();
			doc.Load (ProjectPaths.ProjectCSProjPath);
			var nodes = doc.SelectNodes ($"//*[local-name() = 'ItemGroup']/*[local-name() = '{name}']");
			foreach (XmlNode node in nodes) {
				node.ParentNode.RemoveChild (node);
			}
			doc.Save (ProjectPaths.ProjectCSProjPath);
		}
	}

	public class MSBuildItem {
		public string EvaluatedInclude;
		public Dictionary<string, string> Metadata = new Dictionary<string, string> ();

		public string GetMetadataValue (string name)
		{
			if (Metadata.TryGetValue (name, out var value))
				return value;
			return null;
		}
	}
}
