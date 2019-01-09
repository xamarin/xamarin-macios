using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using NUnit.Framework;
using Xamarin.Tests;
using System.Linq;

namespace Xamarin.iOS.Tasks
{
	[TestFixture]
	public class FrameworkListTests
	{
		[TestCase ("Xamarin.iOS-FrameworkList.xml.in", false)]
		[TestCase ("Xamarin.TVOS-FrameworkList.xml.in", false)]
		[TestCase ("Xamarin.WatchOS-FrameworkList.xml.in", false)]
		[TestCase ("FrameworkList.xml.in", true)] // Xamarin.Mac
		public void CheckFrameworkListFile (string frameworkListFile, bool isMac)
		{
			var errorCount = 0;
			var frameworkListAssemblies = ScanFrameworkListXml (frameworkListFile, isMac);
			var frameworkName = isMac ? "Xamarin.Mac" : frameworkListFile.Split ('-')[0];
			var installedAssemblies = ScanAssemblyDirectory (frameworkName, isMac);
			using (var sw = new StringWriter ()) {
				using (var writer = XmlWriter.Create (sw, new XmlWriterSettings { Encoding = Encoding.UTF8, ConformanceLevel = ConformanceLevel.Fragment, Indent = true })) {
					foreach (var assembly in installedAssemblies) {
						if (!frameworkListAssemblies.Any (a => a.Name == assembly.Name)) {
							writer.WriteStartElement ("File");
							writer.WriteAttributeString ("AssemblyName", assembly.Name);
							WriteNonEmptyAttribute ("Version", assembly.Version);
							WriteNonEmptyAttribute ("PublicKeyToken", assembly.PublicKeyToken);
							WriteNonEmptyAttribute ("Culture", assembly.Culture);
							if (assembly.ProcessorArchitecture != ProcessorArchitecture.None)
								writer.WriteAttributeString ("ProcessorArchitecture", assembly.ProcessorArchitecture.ToString ());
							if (assembly.InGac)
								writer.WriteAttributeString ("InGac", "true");
							writer.WriteEndElement ();
							errorCount++;
						}
					}

					void WriteNonEmptyAttribute (string name, string val)
					{
						if (!string.IsNullOrEmpty (val))
							writer.WriteAttributeString (name, val);
					}
				}
				Assert.AreEqual (0, errorCount, $"Missing assemblies in '{frameworkListFile}'\n{sw.ToString ()}");
			}
		}

		List<AssemblyInfo> ScanFrameworkListXml (string frameworkListFile, bool isMac)
		{
			var assemblies = new List<AssemblyInfo> ();
			var path = string.Empty;
			if (isMac)
				path = Path.GetFullPath (Path.Combine ("..", "..", "Xamarin.Mac.Tasks", frameworkListFile));
			else
				path = Path.GetFullPath (Path.Combine ("..", "..", "Xamarin.iOS.Tasks.Core", frameworkListFile));
			using (var reader = XmlReader.Create (path)) {
				while (reader.Read ()) {
					if (reader.IsStartElement ()) {
						switch (reader.LocalName) {
						case "File":
							assemblies.Add (ReadFileElement (reader));
							break;
						}
					}
				}
			}
			return assemblies;
		}

		List<AssemblyInfo> ScanAssemblyDirectory (string frameworkName, bool isMac)
		{
			var assemblies = new List<AssemblyInfo> ();
			var path = string.Empty;
			if (isMac)
				path = Path.GetFullPath (Path.Combine (Configuration.SdkRootXM, "lib", "mono", frameworkName));
			else
				path = Path.GetFullPath (Path.Combine (Configuration.MonoTouchRootDirectory, "lib", "mono", frameworkName));
			foreach (var f in Directory.EnumerateFiles (path, "*.dll")) {
				try {
					var an = AssemblyName.GetAssemblyName (f);
					var ainfo = new AssemblyInfo ();
					ainfo.Update (an);
					assemblies.Add (ainfo);
				} catch (Exception ex) {
					Console.WriteLine ("Error reading assembly '{0}' in framework '{1}':{2}{3}", f, frameworkName, Environment.NewLine, ex);
				}
			}
			return assemblies;
		}

		static AssemblyInfo ReadFileElement (XmlReader reader)
		{
			var ainfo = new AssemblyInfo ();
			if (reader.MoveToAttribute ("AssemblyName") && reader.ReadAttributeValue ())
				ainfo.Name = reader.ReadContentAsString ();
			if (string.IsNullOrEmpty (ainfo.Name))
				throw new Exception ("Missing AssemblyName attribute");
			if (reader.MoveToAttribute ("Version") && reader.ReadAttributeValue ())
				ainfo.Version = reader.ReadContentAsString ();
			if (reader.MoveToAttribute ("PublicKeyToken") && reader.ReadAttributeValue ())
				ainfo.PublicKeyToken = reader.ReadContentAsString ();
			if (reader.MoveToAttribute ("Culture") && reader.ReadAttributeValue ())
				ainfo.Culture = reader.ReadContentAsString ();
			if (reader.MoveToAttribute ("ProcessorArchitecture") && reader.ReadAttributeValue ())
			ainfo.ProcessorArchitecture = (ProcessorArchitecture)
			Enum.Parse (typeof (ProcessorArchitecture), reader.ReadContentAsString (), true);
			if (reader.MoveToAttribute ("InGac") && reader.ReadAttributeValue ())
				ainfo.InGac = reader.ReadContentAsBoolean ();
			return ainfo;
		}
	}

	class AssemblyInfo
	{
		public string Name;

		public string Version;

		public string PublicKeyToken;

		public string Culture;

		public ProcessorArchitecture ProcessorArchitecture = ProcessorArchitecture.MSIL;

		public bool InGac;

		public void Update (AssemblyName aname)
		{
			Name = aname.Name;
			Version = aname.Version.ToString ();
			ProcessorArchitecture = aname.ProcessorArchitecture;
			Culture = aname.CultureInfo.Name;
			string fn = aname.ToString ();
			string key = "publickeytoken=";
			int i = fn.IndexOf (key, StringComparison.OrdinalIgnoreCase) + key.Length;
			int j = fn.IndexOf (',', i);
			if (j == -1) j = fn.Length;
			PublicKeyToken = fn.Substring (i, j - i);
		}
	}
}
