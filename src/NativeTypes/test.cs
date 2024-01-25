#nullable enable

using System;
using System.Drawing;

using CoreGraphics;
namespace Foundation {
	public class NSDictionary { }
}

unsafe static class Test {
	static void Main ()
	{
		nint a = 55;
		a *= 3;
		Console.WriteLine (a + 32);
		Console.WriteLine (a.GetType ());

		short s = 100;
		nint b = s;
		Console.WriteLine (b);

		Console.WriteLine (sizeof (nint));


		var fr = new RectangleF (1.5f, 2.5f, 3.5f, 4.5f);
		var dr = fr;
		Console.WriteLine (fr);
		Console.WriteLine (dr);
		fr = (RectangleF) dr;
		Console.WriteLine (fr);

	}
}
