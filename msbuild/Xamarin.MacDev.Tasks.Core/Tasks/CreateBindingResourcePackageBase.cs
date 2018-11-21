using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Xamarin.MacDev.Tasks {
	public abstract class CreateBindingResourcePackageBase : Task {
		public string SessionId { get; set; }
		
		[Required]
		public string OutputPath { get; set; }
		
		[Required]		
		public ITaskItem[] NativeReferences { get; set; }
		
		[Required]
		public string ProjectDir { get; set; }
		
		[Required]
		public string BindingAssembly { get; set; }
		
		public override bool Execute ()
		{
			// LinkWith must be migrated for NoBindingEmbedding styled binding projects
			if (NativeReferences.Length == 0)
				Log.LogError ($"Unable to create a Binding Resource Package with no Native References. Move from LinkWith.cs to NativeReferences or disable NoBindingEmbedding");

			string bindingResourcePath = Path.Combine (ProjectDir, OutputPath, $"{Path.GetFileNameWithoutExtension (BindingAssembly)}.resources");
			Log.LogMessage ($"Creating binding resource package: {bindingResourcePath}");

			Directory.CreateDirectory (bindingResourcePath);
			foreach (var nativeRef in NativeReferences)
				CopyNativeReference (nativeRef.ItemSpec, bindingResourcePath);

			CreateManifest (bindingResourcePath);

			return true;
		}

		// There is no built-in C# API for recursive copy of directories
		void CopyNativeReference (string nativeRef, string resourcePath)
		{
			// TODO - Should this be smarter if we didn't change? Can't use smart copy unless framework? rsync?
			// TODO - Is this safe for Windows builds?
			var copyTask = new Microsoft.Build.Tasks.Exec {
				Command = $"cp -r {nativeRef} {resourcePath}",
				BuildEngine = this.BuildEngine
			};
			copyTask.Execute ();
			if (copyTask.ExitCode != 0)
				Log.LogError ($"Binding resource packaging copy exited with code {copyTask.ExitCode}");
		}

		string [] NativeReferenceAttributeNames = new string [] { "Kind", "ForceLoad", "SmartLink", "Frameworks", "WeakFrameworks", "LinkerFlags", "NeedsGccExceptionHandling", "IsCxx"};

		void CreateManifest (string resourcePath)
		{
			XmlWriterSettings settings = new XmlWriterSettings() {
				OmitXmlDeclaration = true,
				Indent = true,
				IndentChars = "\t",
			};

			using (var writer = XmlWriter.Create (Path.Combine (resourcePath, "manifest"), settings)) {
				writer.WriteStartElement ("BindingAssembly");
				writer.WriteAttributeString ("MVID", MVIDHelpers.FindAssemblyMVID (Path.Combine (ProjectDir, BindingAssembly)));

				foreach (var nativeRef in NativeReferences) {
					writer.WriteStartElement ("NativeReference");
					writer.WriteAttributeString ("Name", Path.GetFileName (nativeRef.ItemSpec));

					foreach (string attribute in NativeReferenceAttributeNames) {
						writer.WriteStartElement (attribute);
						writer.WriteString (nativeRef.GetMetadata (attribute));
						writer.WriteEndElement ();
					}

					writer.WriteEndElement ();
				}
				writer.WriteEndElement ();
			}
		}
	}

	public static class MVIDHelpers {
		public static string FindAssemblyMVID (string path)
		{
			var assembly = Assembly.ReflectionOnlyLoadFrom (path);
			return assembly.Modules.First ().ModuleVersionId.ToString ();
		}
	}
}
