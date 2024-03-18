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
			var items = this.Items;
			if (items is null)
				items = new ITaskItem [0];

			var document = new XDocument (
				new XElement (ProjectElementName,
					new XElement (ItemGroupElementName,
						items.Select (item => this.CreateElementFromItem (item)))));

			var file = this.File?.ItemSpec;
			if (this.Overwrite && System.IO.File.Exists (file))
				System.IO.File.Delete (file);

			if (!Directory.Exists (Path.GetDirectoryName (file)))
				Directory.CreateDirectory (Path.GetDirectoryName (file));

			document.Save (file);

			return true;
		}

		private XElement CreateElementFromItem (ITaskItem item)
		{
			return new XElement (XmlNs + ItemName,
				new XAttribute (IncludeAttributeName, item.ItemSpec),
					this.CreateMetadataFromItem (item));
		}

		private IEnumerable<XElement> CreateMetadataFromItem (ITaskItem item)
		{
			if (this.IncludeMetadata) {
				var metadata = item.CloneCustomMetadata ();

				return metadata.Keys
					.OfType<object> ()
					.Select (key => new XElement (XmlNs + key.ToString (), metadata [key].ToString ()));
			}

			return Enumerable.Empty<XElement> ();
		}
	}
}
