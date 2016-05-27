using System;
using System.Security.Cryptography;
using MonoMac.AppKit;

// Test
// * application calls Aes.Create where Aes now (4.0+) resides in mscorlib.dll
// * AesCryptoServiceProvider however still resides in System.Core.dll
//
// Requirement
// * Link SDK or Link All must be enabled

namespace Xamarin.Mac.Linker.Test {
	
	class Crypto {

		static void Main (string[] args)
		{
			NSApplication.Init ();
			
			Test.EnsureLinker (true);

			Aes aes = null;
			string msg = "is null";

			try {
				aes = Aes.Create ();
				if (aes != null)
					msg = "usable as " + aes.GetType ().FullName;
			}
			catch (Exception e) {
				msg = "throwed " + e.ToString ();
			}

			Test.Log.WriteLine ("[{0}]\tAES {1}", aes == null ? "FAIL" : "PASS", msg);

			Test.Terminate ();
		}
	}
}