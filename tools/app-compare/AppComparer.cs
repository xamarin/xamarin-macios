using System;
using System.IO;

namespace Tools {
	
	public class AppComparer : DirectoryComparer
	{
		public AppComparer (string dir1, string dir2)
		{
			Directory1 = new DirectoryInfo (dir1);
			Directory2 = new DirectoryInfo (dir2);

			app1 = dir1;
			app2 = dir2;
		}

		string dir;
		string app1, app2;
		// stats
		long native1, native2;
		long aotdata1, aotdata2;
		long managed1, managed2;
		long total1, total2;

		public override string GetKey (string rootedName)
		{
			var sep = rootedName.EndsWith (".aotdata") ? "!!!!!!!!!!!" : "!!!!!!!!!!!!!!!!!" ;
			return Path.GetDirectoryName (rootedName) + sep + Path.GetFileName (rootedName);
		}

		public override void Start ()
		{
			base.Start ();
			Output.WriteLine ("# Application Comparer");
			Output.WriteLine ();
			Output.WriteLine ($"* **A** `{app1}`");
			Output.WriteLine ($"* **B** `{app2}`");
			Output.WriteLine ();
			Output.WriteLine ("| Directories / Files |  A  |  B  | diff |  %  |");
			Output.WriteLine ("| ------------------- | --: | --: | ---: | --: |");
		}

		string GetSize (FileInfo info, out long length)
		{
			length = 0;
			if (info == null)
				return "0";
			if ((info.Attributes & FileAttributes.ReparsePoint) != 0)
				return "-";
			length = info.Length;
			return length.ToString ("N0");
		}

		public override void Process (Item item)
		{
			if (item.Directory != dir) {
				dir = item.Directory;
				Output.Write ("| ./");
				Output.Write (dir);
				Output.Write (" | | | | |");
			}

			long s1 = 0;
			long s2 = 0;

			var d1 = GetSize (item.Info1, out s1);
			var d2 = GetSize (item.Info2, out s2);
			WriteStats ("    " + item.Name, s1, s2, d1, d2);

			switch (Path.GetExtension (item.Name)) {
			case ".aotdata":
				aotdata1 += s1;
				aotdata2 += s2;
				break;
			case ".dll":
			case ".exe":
				managed1 += s1;
				managed2 += s2;
				break;
			case "":
				// we assume the largest file without extension is the native executable
				if (s1 > native1)
					native1 = s1;
				if (s2 > native2)
					native2 = s2;
				break;
			default:
				break;
			}
			total1 += s1;
			total2 += s2;
		}

		void WriteStats (string name, long value1, long value2, string display1 = null, string display2 = null)
		{
			if (display1 == null)
				display1 = value1.ToString ("N0");
			if (display2 == null)
				display2 = value2.ToString ("N0");
			var percent = (value1 == 0) ? "-  " : ((value2 - value1) / (double) value1).ToString ("P1");
			Output.WriteLine ($"| {name} | {display1} | {display2} | {(value2 - value1):N0} | {percent} |");
		}

		public override void End()
		{
			Output.WriteLine ("| | | | | |");
			Output.WriteLine ("| **Statistics** | | | | |");
			Output.WriteLine ("| | | | | |");
			WriteStats ("Native subtotal", native1 + aotdata1, native2 + aotdata2);
			WriteStats ("    Executable", native1, native2);
			WriteStats ("    AOT data *.aotdata", aotdata1, aotdata2);
			Output.WriteLine ("| | | | | |");
			WriteStats ("Managed *.dll/exe", managed1, managed2);
			Output.WriteLine ("| | | | | |");
			WriteStats ("**TOTAL**", total1, total2);
			base.End ();
		}
	}
}
