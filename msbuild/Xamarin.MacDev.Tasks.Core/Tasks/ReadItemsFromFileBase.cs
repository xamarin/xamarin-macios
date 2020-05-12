using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.Build.Tasks;
using System.Xml.Linq;

namespace Xamarin.MacDev.Tasks
{
	public abstract class ReadItemsFromFileBase : XamarinTask
	{
		static readonly XNamespace XmlNs = XNamespace.Get("http://schemas.microsoft.com/developer/msbuild/2003");

		static readonly XName ItemGroupElementName = XmlNs + "ItemGroup";
		const string IncludeAttributeName = "Include";

		#region Inputs

		[Output]
		[Required]
		public ITaskItem File { get; set; }

		#endregion

		#region Outputs

		[Output]
		public ITaskItem[] Items { get; set; }

		#endregion

		public override bool Execute ()
		{
			var document = XDocument.Load (this.File.ItemSpec);

			this.Items = document.Root
				.Elements (ItemGroupElementName)
				.SelectMany (element => element.Elements ())
				.Select (element => this.CreateItemFromElement (element))
				.ToArray ();

			return true;
		}

		private ITaskItem CreateItemFromElement (XElement element)
		{
			var item = new TaskItem (element.Attribute (IncludeAttributeName).Value);

			foreach (var metadata in element.Elements ()) {
				item.SetMetadata (metadata.Name.LocalName, metadata.Value);
			}

			return item;
		}
	}
}