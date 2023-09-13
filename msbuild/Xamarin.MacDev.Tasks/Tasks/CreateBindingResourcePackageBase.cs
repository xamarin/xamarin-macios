using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Xamarin.Localization.MSBuild;
using Xamarin.Utils;

// Disable until we get around to enable + fix any issues.
#nullable disable

namespace Xamarin.MacDev.Tasks {
	public abstract class CreateBindingResourcePackageBase : XamarinTask {
		[Required]
		public string Compress { get; set; }

		[Required]
		public string BindingResourcePath { get; set; }

		[Required]
		public string IntermediateOutputPath { get; set; }

		[Required]
		public ITaskItem [] NativeReferences { get; set; }

		// This is a list of files to copy back to Windows
		[Output]
		public ITaskItem [] PackagedFiles { get; set; }

		public override bool Execute ()
		{
			if (NativeReferences.Length == 0) {
				// Nothing to do here
				return true;
			}

			var nonexistent = NativeReferences.Where (v => !(Directory.Exists (v.ItemSpec) || File.Exists (v.ItemSpec)));
			if (nonexistent.Any ()) {
				foreach (var nonex in nonexistent)
					Log.LogError (MSBStrings.E0190 /* The NativeResource item '{0}' does not exist. */, nonex.ItemSpec);
				return false;
			}

			var compress = false;
			if (string.Equals (Compress, "true", StringComparison.OrdinalIgnoreCase)) {
				compress = true;
			} else if (string.Equals (Compress, "false", StringComparison.OrdinalIgnoreCase)) {
				compress = false;
			} else if (string.Equals (Compress, "auto", StringComparison.OrdinalIgnoreCase)) {
				compress = ContainsSymlinks (NativeReferences);
				if (compress)
					Log.LogMessage (MessageImportance.Low, MSBStrings.W7085 /* "Creating a compressed binding resource package because there are symlinks in the input." */);
			} else {
				Log.LogError (MSBStrings.E7086 /* "The value '{0}' is invalid for the Compress property. Valid values: 'true', 'false' or 'auto'." */, Compress);
			}

			Directory.CreateDirectory (compress ? IntermediateOutputPath : BindingResourcePath);

			var manifestDirectory = compress ? IntermediateOutputPath : BindingResourcePath;
			var manifestPath = CreateManifest (manifestDirectory);
			var packagedFiles = new List<string> ();

			if (compress) {
				var zipFile = Path.GetFullPath (BindingResourcePath + ".zip");
				Log.LogMessage (MSBStrings.M0121, zipFile);
				if (File.Exists (zipFile))
					File.Delete (zipFile);
				Directory.CreateDirectory (Path.GetDirectoryName (zipFile));

				var filesToZip = NativeReferences.Select (v => v.ItemSpec).ToList ();
				filesToZip.Add (manifestPath);

				foreach (var nativeRef in filesToZip) {
					CompressionHelper.TryCompress (Log, zipFile, nativeRef, false);
				}
				packagedFiles.Add (zipFile);
			} else {
				var bindingResourcePath = BindingResourcePath;
				Log.LogMessage (MSBStrings.M0121, bindingResourcePath);
				Directory.CreateDirectory (bindingResourcePath);
				foreach (var nativeRef in NativeReferences) {
					Xamarin.Bundler.FileCopier.UpdateDirectory (nativeRef.ItemSpec, bindingResourcePath, FileCopierReportErrorCallback, FileCopierLogCallback);

					var bindingOutputPath = Path.Combine (bindingResourcePath, Path.GetFileName (nativeRef.ItemSpec));
					if (Directory.Exists (bindingOutputPath)) {
						packagedFiles.AddRange (Directory.GetFiles (bindingOutputPath, "*", SearchOption.AllDirectories));
					} else if (File.Exists (bindingOutputPath)) {
						packagedFiles.Add (bindingOutputPath);
					} else {
						Log.LogWarning (MSBStrings.W7100, bindingOutputPath);
					}
				}
				packagedFiles.Add (manifestPath);
			}

			PackagedFiles = packagedFiles.Select (v => new TaskItem (v)).ToArray ();

			return !Log.HasLoggedErrors;
		}

		bool ContainsSymlinks (ITaskItem [] items)
		{
			foreach (var item in items) {
				if (PathUtils.IsSymlinkOrContainsSymlinks (item.ItemSpec)) {
					if (Environment.OSVersion.Platform == PlatformID.Win32NT)
						Log.LogError (MSBStrings.E7120 /* Can't process the native reference '{0}' on this platform because it is or contains a symlink. */, item.ItemSpec);
					return true;
				}
			}

			return false;
		}

		string [] NativeReferenceAttributeNames = new string [] { "Kind", "ForceLoad", "SmartLink", "Frameworks", "WeakFrameworks", "LinkerFlags", "NeedsGccExceptionHandling", "IsCxx" };

		string CreateManifest (string resourcePath)
		{
			XmlWriterSettings settings = new XmlWriterSettings () {
				OmitXmlDeclaration = true,
				Indent = true,
				IndentChars = "\t",
			};

			string manifestPath = Path.Combine (resourcePath, "manifest");
			using (var writer = XmlWriter.Create (manifestPath, settings)) {
				writer.WriteStartElement ("BindingAssembly");

				foreach (var nativeRef in NativeReferences) {
					writer.WriteStartElement ("NativeReference");
					writer.WriteAttributeString ("Name", Path.GetFileName (nativeRef.ItemSpec));

					var customMetadata = nativeRef.CloneCustomMetadataToDictionary ();
					var allKeys = customMetadata.Keys.Union (NativeReferenceAttributeNames, StringComparer.OrdinalIgnoreCase);
					foreach (var key in allKeys.OrderBy (v => v)) {
						writer.WriteStartElement (key);
						if (customMetadata.TryGetValue (key, out var value)) {
							writer.WriteString (value);
						} else {
							writer.WriteString (string.Empty);
						}
						writer.WriteEndElement ();
					}

					writer.WriteEndElement ();
				}
				writer.WriteEndElement ();
			}
			return manifestPath;
		}
	}
}
