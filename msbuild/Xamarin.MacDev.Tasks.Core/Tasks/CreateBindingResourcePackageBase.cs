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
			if (NativeReferences.Length == 0) {
				Log.LogError (7068, null, $"Can't create a binding resource package unless there are native references in the binding project.");
				return false;
			}

			string bindingResourcePath = Path.Combine (ProjectDir, OutputPath, Path.ChangeExtension (Path.GetFileName (BindingAssembly), ".resources"));
			Log.LogMessage ($"Creating binding resource package: {bindingResourcePath}");

			Directory.CreateDirectory (bindingResourcePath);
			foreach (var nativeRef in NativeReferences)
				Xamarin.Bundler.FileCopier.UpdateDirectory (nativeRef.ItemSpec, bindingResourcePath);

			CreateManifest (bindingResourcePath);

			return true;
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
}
