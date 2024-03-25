using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.Build.Tasks;
using System.Xml.Linq;

namespace Xamarin.MacDev.Tasks {
	public class WriteItemsToFile : XamarinTask {
		static readonly XNamespace XmlNs = XNamespace.Get ("http://schemas.microsoft.com/developer/msbuild/2003");

		static readonly XName ProjectElementName = XmlNs + "Project";
		static readonly XName ItemGroupElementName = XmlNs + "ItemGroup";
		const string IncludeAttributeName = "Include";

		#region Inputs

		public ITaskItem [] Items { get; set; } = Array.Empty<ITaskItem> ();

		public string ItemName { get; set; } = string.Empty;

		[Output]
		[Required]
		public ITaskItem? File { get; set; }

		public bool Overwrite { get; set; }

		public bool IncludeMetadata { get; set; }

		#endregion

		public override bool Execute ()
		{
			Write (this, File?.ItemSpec, Items, ItemName, Overwrite, IncludeMetadata);
			return true;
		}

		public static void Write (Task task, string? file, IEnumerable<ITaskItem> items, string itemName, bool overwrite, bool includeMetadata)
		{
			if (file is null) {
				task.Log.LogWarning ($"No output file to write to for item {itemName}");
				return;
			}

			var document = new XDocument (
				new XElement (ProjectElementName,
					new XElement (ItemGroupElementName,
						items.Select (item => CreateElementFromItem (item, itemName, includeMetadata)))));

			if (overwrite && System.IO.File.Exists (file))
				System.IO.File.Delete (file);

			if (!Directory.Exists (Path.GetDirectoryName (file)))
				Directory.CreateDirectory (Path.GetDirectoryName (file));

			document.Save (file);
		}

		static XElement CreateElementFromItem (ITaskItem item, string itemName, bool includeMetadata)
		{
			return new XElement (XmlNs + itemName,
				new XAttribute (IncludeAttributeName, item.ItemSpec),
					CreateMetadataFromItem (item, includeMetadata));
		}

		static IEnumerable<XElement> CreateMetadataFromItem (ITaskItem item, bool includeMetadata)
		{
			if (includeMetadata) {
				var metadata = item.CloneCustomMetadata ();

				return metadata.Keys
					.OfType<object> ()
					.Select (key => new XElement (XmlNs + key.ToString (), metadata [key].ToString ()));
			}

			return Enumerable.Empty<XElement> ();
		}
	}
}
