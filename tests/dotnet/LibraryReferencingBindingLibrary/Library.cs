using System;

namespace LibraryReferencingBindingLibrary {
	public class Library {
		public static void DoSomething ()
		{
			Console.WriteLine (typeof (MyClassLibrary.MyClass));
		}
	}
}
