using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using NUnit.Framework;
using Xamarin.Tests;
using System.Linq;

namespace Xamarin.MacDev.Tasks {
	[TestFixture]
	public class FrameworkListTests {
		[TestCase ("Xamarin.iOS-FrameworkList.xml.in")]
		[TestCase ("Xamarin.TVOS-FrameworkList.xml.in")]
		[TestCase ("Xamarin.WatchOS-FrameworkList.xml.in")]
		[TestCase ("Xamarin.Mac-Full-FrameworkList.xml.in")]
		[TestCase ("Xamarin.Mac-Mobile-FrameworkList.xml.in")]
		public void CheckFrameworkListFile (string frameworkListFile)
		{
			Configuration.AssertLegacyXamarinAvailable ();

			var fameworkListFileParts = frameworkListFile.Split ('-');
			string frameworkName = fameworkListFileParts [0];
			switch (frameworkName) {
			case "Xamarin.iOS":
				if (!Configuration.include_ios)
					Assert.Inconclusive ("include_ios is disabled");
				break;
			case "Xamarin.TVOS":
				if (!Configuration.include_tvos)
					Assert.Inconclusive ("include_tvos is disabled");
				break;
			case "Xamarin.WatchOS":
				if (!Configuration.include_watchos)
					Assert.Inconclusive ("include_watchos is disabled");
				break;
			case "Xamarin.Mac":
				if (!Configuration.include_mac)
					Assert.Inconclusive ("include_mac is disabled");
				break;
			}
			var isMac = frameworkName == "Xamarin.Mac";
			var isFull = fameworkListFileParts [1] == "Full";
			var frameworkListAssemblies = ScanFrameworkListXml (frameworkListFile);
			var installedAssemblies = ScanAssemblyDirectory (frameworkName, isMac, isFull);

			foreach (var assembly in frameworkListAssemblies) {
				if (!installedAssemblies.Any (a => a.Name == assembly.Name))
					ReportAssemblies (assembly, $"One or more assemblies listed in '{frameworkListFile}' were not found in the final SDK root folder. Update the list if an assembly was intentionally removed.");
			}

			foreach (var assembly in installedAssemblies) {
				if (!frameworkListAssemblies.Any (a => a.Name == assembly.Name))
					ReportAssemblies (assembly, $"One or more assemblies in the the SDK root folder are not listed in '{frameworkListFile}'. Update the list if an assembly was intentionally added.");
				else if (!frameworkListAssemblies.Single (a => a.Name == assembly.Name).Equals (assembly))
					ReportAssemblies (assembly, $"One or more assemblies in the the SDK root folder do not match the entry in '{frameworkListFile}'. Update the list if an assembly was intentionally modified.");
			}
		}

		void ReportAssemblies (AssemblyInfo assembly, string message)
		{
			var errorCount = 0;
			using (var sw = new StringWriter ()) {
				using (var writer = XmlWriter.Create (sw, new XmlWriterSettings { Encoding = Encoding.UTF8, ConformanceLevel = ConformanceLevel.Fragment, Indent = true })) {
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

					void WriteNonEmptyAttribute (string name, string val)
					{
						if (!string.IsNullOrEmpty (val))
							writer.WriteAttributeString (name, val);
					}
				}
				Assert.AreEqual (0, errorCount, $"{message}\n{sw.ToString ()}");
			}
		}

		List<AssemblyInfo> ScanFrameworkListXml (string frameworkListFile)
		{
			var assemblies = new List<AssemblyInfo> ();
			var path = Path.GetFullPath (Path.Combine (Configuration.SourceRoot, "msbuild", "Xamarin.Shared", frameworkListFile));
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

		List<AssemblyInfo> ScanAssemblyDirectory (string frameworkName, bool isMac, bool isFull)
		{
			var assemblies = new List<AssemblyInfo> ();
			var assembliesPath = Path.GetFullPath (Path.Combine (isMac ? Configuration.SdkRootXM : Configuration.MonoTouchRootDirectory, "lib", "mono", isFull ? "4.5" : frameworkName));
			AddAssemblies (assembliesPath);
			AddAssemblies (Path.Combine (assembliesPath, "Facades"));

			void AddAssemblies (string path)
			{
				foreach (var f in Directory.EnumerateFiles (path, "*.dll")) {
					try {
						var an = AssemblyName.GetAssemblyName (f);
						assemblies.Add (new AssemblyInfo (an));
					} catch (Exception ex) {
						Assert.Fail ("Error reading assembly '{0}' in framework '{1}':{2}{3}", f, frameworkName, Environment.NewLine, ex);
					}
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

	class AssemblyInfo {
		public string Name;

		public string Version;

		public string PublicKeyToken;

		public string Culture;

		public ProcessorArchitecture ProcessorArchitecture = ProcessorArchitecture.MSIL;

		public bool InGac;

		public AssemblyInfo ()
		{
		}

		public AssemblyInfo (AssemblyName aname)
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

		public bool Equals (AssemblyInfo other)
		{
			// ignore Culture and InGac for equality since those are not mentioned in the FrameworkList.xml
			return other.Name == this.Name &&
				other.Version == this.Version &&
				other.PublicKeyToken == this.PublicKeyToken &&
				other.ProcessorArchitecture == this.ProcessorArchitecture;
		}
	}
}
