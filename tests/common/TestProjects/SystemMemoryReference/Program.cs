using System;

namespace app {
	class Program {
		static void Main (string [] args)
		{
			Console.WriteLine (typeof (ReadOnlySpan<char>).Assembly.FullName);
			System.Console.WriteLine (typeof (UIKit.UIWindow));
		}
	}
}
