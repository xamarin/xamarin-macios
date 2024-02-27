using System;
using System.IO;
using Newtonsoft.Json.Linq;

namespace MyLibrary {
	public class MyClass {
		public static void DoIt ()
		{
			JArray array = new JArray ();
			array.Add ("Manual text");
			array.Add (new DateTime (2000, 5, 23));

			JObject o = new JObject ();
			o ["MyArray"] = array;

			var fileName = "../../../../../TestResult.txt";
			if (File.Exists (fileName))
				File.Delete (fileName);

			using (TextWriter writer = File.CreateText (fileName)) {
				writer.WriteLine (o);
			}
			Environment.Exit (0);
		}

	}
}
