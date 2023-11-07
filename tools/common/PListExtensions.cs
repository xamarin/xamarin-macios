using System;
using System.IO;
using System.Xml;

namespace Xamarin {
	static class PListExtensions {
		public static void LoadWithoutNetworkAccess (this XmlDocument doc, string filename)
		{
			using (var fs = new FileStream (filename, FileMode.Open, FileAccess.Read)) {
				var settings = new XmlReaderSettings () {
					XmlResolver = null,
					DtdProcessing = DtdProcessing.Parse,
				};
				using (var reader = XmlReader.Create (fs, settings)) {
					doc.Load (reader);
				}
			}
		}

		public static void LoadXmlWithoutNetworkAccess (this XmlDocument doc, string xml)
		{
			using (var fs = new StringReader (xml)) {
				var settings = new XmlReaderSettings () {
					XmlResolver = null,
					DtdProcessing = DtdProcessing.Parse,
				};
				using (var reader = XmlReader.Create (fs, settings)) {
					doc.Load (reader);
				}
			}
		}

		public static void SetMinimumOSVersion (this XmlDocument plist, string value)
		{
			SetPListStringValue (plist, "MinimumOSVersion", value);
		}

		public static void SetMinimummacOSVersion (this XmlDocument plist, string value)
		{
			SetPListStringValue (plist, "LSMinimumSystemVersion", value);
		}

		public static void SetCFBundleDisplayName (this XmlDocument plist, string value)
		{
			SetPListStringValue (plist, "CFBundleDisplayName", value);
		}

		public static string GetMinimumOSVersion (this XmlDocument plist)
		{
			return GetPListStringValue (plist, "MinimumOSVersion");
		}

		public static string GetMinimummacOSVersion (this XmlDocument plist)
		{
			return GetPListStringValue (plist, "LSMinimumSystemVersion");
		}
		public static void SetCFBundleIdentifier (this XmlDocument plist, string value)
		{
			SetPListStringValue (plist, "CFBundleIdentifier", value);
		}

		public static void SetCFBundleName (this XmlDocument plist, string value)
		{
			SetPListStringValue (plist, "CFBundleName", value);
		}

		public static void SetUIDeviceFamily (this XmlDocument plist, params int [] families)
		{
			SetPListArrayOfIntegerValues (plist, "UIDeviceFamily", families);
		}

		public static string GetCFBundleIdentifier (this XmlDocument plist)
		{
			return GetPListStringValue (plist, "CFBundleIdentifier");
		}

		public static string GetNSExtensionPointIdentifier (this XmlDocument plist)
		{
			return plist.SelectSingleNode ("//dict/key[text()='NSExtensionPointIdentifier']")?.NextSibling?.InnerText;
		}

		public static string GetPListStringValue (this XmlDocument plist, string node)
		{
			return plist.SelectSingleNode ("//dict/key[text()='" + node + "']").NextSibling.InnerText;
		}

		public static void SetPListStringValue (this XmlDocument plist, string node, string value)
		{
			var element = plist.SelectSingleNode ("//dict/key[text()='" + node + "']");
			if (element is null) {
				AddPListStringValue (plist, node, value);
			} else {
				element.NextSibling.InnerText = value;
			}
		}

		public static void AddPListStringValue (this XmlDocument plist, string node, string value)
		{
			var keyElement = plist.CreateElement ("key");
			keyElement.InnerText = node;
			var valueElement = plist.CreateElement ("string");
			valueElement.InnerText = value;
			var root = plist.SelectSingleNode ("//dict");
			root.AppendChild (keyElement);
			root.AppendChild (valueElement);
		}

		public static void AddPListKeyValuePair (this XmlDocument plist, string node, string valueType, string value)
		{
			var keyElement = plist.CreateElement ("key");
			keyElement.InnerText = node;
			var valueElement = plist.CreateElement (valueType);
			valueElement.InnerXml = value;
			var root = plist.SelectSingleNode ("//dict");
			root.AppendChild (keyElement);
			root.AppendChild (valueElement);
		}

		public static void SetPListArrayOfIntegerValues (this XmlDocument plist, string node, params int [] values)
		{
			var key = plist.SelectSingleNode ("//dict/key[text()='" + node + "']");
			key.ParentNode.RemoveChild (key.NextSibling);
			var array = plist.CreateElement ("array");
			foreach (var value in values) {
				var element = plist.CreateElement ("integer");
				element.InnerText = value.ToString ();
				array.AppendChild (element);
			}
			key.ParentNode.InsertAfter (array, key);
		}

		public static void RemoveUIRequiredDeviceCapabilities (this XmlDocument plist)
		{
			var key = plist.SelectSingleNode ("//dict/key[text()='UIRequiredDeviceCapabilities']");
			key.ParentNode.RemoveChild (key.NextSibling);
			key.ParentNode.RemoveChild (key);
		}

		public static bool ContainsKey (this XmlDocument plist, string key)
		{
			return plist.SelectSingleNode ("//dict/key[text()='" + key + "']") is not null;
		}
	}
}
