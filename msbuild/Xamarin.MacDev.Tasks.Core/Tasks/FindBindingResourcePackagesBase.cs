using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Xamarin.MacDev.Tasks {
	public abstract class FindBindingResourcePackagesBase: Task {
		public string SessionId { get; set; }
		
		[Required]		
		public ITaskItem[] References { get; set; }
		
		[Required]		
		public ITaskItem[] ReferencesFoundInSolution { get; set; }
		
		[Output]
		public ITaskItem[] AdditionalNativeReferences { get; set; }

		[Output]
		public ITaskItem[] ManifestFound { get; set; }
		
		public override bool Execute ()
		{
			var ReferencesAlreadyResolved = new HashSet <string> (ReferencesFoundInSolution.Select (x => Path.GetFileName (x.ItemSpec)));

			var additionalNativeReferences = new List<ITaskItem> ();
			foreach (var reference in References) {
				string referenceFile = Path.GetFileName (reference.ItemSpec);
				if (ReferencesAlreadyResolved.Contains (referenceFile)) {
					Log.LogMessage (MessageImportance.Low, $"Using in-solution reference for {referenceFile} instead of potential bundle.");
					continue;
				}

				string referencePath = Path.GetDirectoryName (reference.ItemSpec);
				string resourceBundlePath = Path.Combine (referencePath, Path.GetFileNameWithoutExtension (reference.ItemSpec) + ".resources");
				if (Directory.Exists (resourceBundlePath)) {
					string manifestPath = Path.Combine (resourceBundlePath, "manifest");
					if (File.Exists (manifestPath)) {
						Log.LogMessage ($"Found resource bundle at {resourceBundlePath}");

						ManifestFound = new ITaskItem [] { new TaskItem ("ManifestFile") { ItemSpec = manifestPath } };
						additionalNativeReferences.AddRange (ReadManifestAndCreateReferences (reference.ItemSpec, resourceBundlePath, manifestPath));
					}
				}
			}
			AdditionalNativeReferences = additionalNativeReferences.ToArray ();
			return true;
		}

		IEnumerable<ITaskItem> ReadManifestAndCreateReferences (string referencePath, string resourceBundlePath, string manifestPath)
		{
			XmlDocument document = new XmlDocument ();
			document.Load (manifestPath);

			string expectedMVID = document.DocumentElement.GetAttribute ("MVID");
			string actualMVID = MVIDHelpers.FindAssemblyMVID (referencePath);
			if (expectedMVID != actualMVID)
				Log.LogError ($"Assembly MVID mismatch in binding resource package. Expected {expectedMVID} but found {actualMVID}. File an issue at https://github.com/xamarin/xamarin-macios/issues/new and rebuild your binding project.");

			foreach (XmlNode referenceNode in document.GetElementsByTagName ("NativeReference")) {
				var attributes = new Dictionary<string, string> ();
				string nativeLibraryPath = Path.Combine (resourceBundlePath, referenceNode.Attributes ["Name"].Value);
				attributes["Assembly"] = Path.Combine (resourceBundlePath, referencePath);
				foreach (XmlNode attribute in referenceNode.ChildNodes) {
					if (!string.IsNullOrEmpty (attribute.InnerText))
						attributes[attribute.Name] = attribute.InnerText;
				}
				yield return new TaskItem ("NativeReference", attributes) { ItemSpec = nativeLibraryPath };
			}
		}
	}
}
