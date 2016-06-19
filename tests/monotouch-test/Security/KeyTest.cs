//
// SecKey Unit Tests
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2013-2015 Xamarin Inc.
//

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
#if XAMCORE_2_0
using Foundation;
using Security;
using UIKit;
#else
using MonoTouch.Foundation;
using MonoTouch.Security;
using MonoTouch.UIKit;
#endif
using NUnit.Framework;

namespace MonoTouchFixtures.Security {

	[TestFixture]
	// we want the test to be availble if we use the linker
	[Preserve (AllMembers = true)]
	public class KeyTest {

		static X509Certificate2 _c;
		static X509Certificate2 c {
			get {
				if (_c == null)
					_c = new X509Certificate2 (ImportExportTest.farscape_pfx, "farscape");
				return _c;
			}
		}

		[Test]
		public void Encrypt_Old ()
		{
			// the old API was not working but the crash was fixed, still you need to provide an adequatly sized buffer
			using (SecPolicy p = SecPolicy.CreateBasicX509Policy ())
				using (SecTrust t = new SecTrust (c, p)) {
				// getting the public key won't (always) work if evaluate was not called
				t.Evaluate ();
				using (SecKey pubkey = t.GetPublicKey ()) {
					byte[] plain = new byte [20];
					byte[] cipher = new byte [pubkey.BlockSize];
					Assert.That (pubkey.Encrypt (SecPadding.PKCS1, plain, cipher), Is.EqualTo (SecStatusCode.Success), "Encrypt");
				}
			}
		}

		[Test]
		public void Encrypt_Empty ()
		{
			using (SecPolicy p = SecPolicy.CreateBasicX509Policy ())
			using (SecTrust t = new SecTrust (c, p)) {
				// getting the public key won't (always) work if evaluate was not called
				t.Evaluate ();
				using (SecKey pubkey = t.GetPublicKey ()) {
					byte[] plain = new byte [0];
					byte[] secret;
					Assert.That (pubkey.Encrypt (SecPadding.PKCS1, plain, out secret), Is.EqualTo (SecStatusCode.Success), "Encrypt");
					Assert.That (secret.Length, Is.EqualTo (128), "secret.Length");
				}
			}
		}

		[Test]
		public void Encrypt_New ()
		{
			using (SecPolicy p = SecPolicy.CreateBasicX509Policy ())
			using (SecTrust t = new SecTrust (c, p)) {
				// getting the public key won't (always) work if evaluate was not called
				t.Evaluate ();
				using (SecKey pubkey = t.GetPublicKey ()) {
					byte[] plain = new byte [20];
					byte[] secret;
					Assert.That (pubkey.Encrypt (SecPadding.PKCS1, plain, out secret), Is.EqualTo (SecStatusCode.Success), "Encrypt");
					Assert.That (secret.Length, Is.EqualTo (128), "secret.Length");
				}
			}
		}

		[Test]
		public void RoundtripRSA512PKCS1 ()
		{
			SecKey private_key;
			SecKey public_key;
			using (var record = new SecRecord (SecKind.Key)) {
				record.KeyType = SecKeyType.RSA;
				record.KeySizeInBits = 512; // it's not a performance test :)

				Assert.That (SecKey.GenerateKeyPair (record.ToDictionary (), out public_key, out private_key), Is.EqualTo (SecStatusCode.Success), "GenerateKeyPair");

				byte [] plain = new byte [20] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 };
				byte [] cipher;
				Assert.That (public_key.Encrypt (SecPadding.PKCS1, plain, out cipher), Is.EqualTo (SecStatusCode.Success), "Encrypt");
				public_key.Dispose ();

				byte[] result;
				Assert.That (private_key.Decrypt (SecPadding.PKCS1, cipher, out result), Is.EqualTo (SecStatusCode.Success), "Decrypt");
				Assert.That (plain, Is.EqualTo (result), "match");
				private_key.Dispose ();
			}
		}

		[Test]
		public void EncryptTooLarge ()
		{
			SecKey private_key;
			SecKey public_key;
			using (var record = new SecRecord (SecKind.Key)) {
				record.KeyType = SecKeyType.RSA;
				record.KeySizeInBits = 512; // it's not a performance test :)

				Assert.That (SecKey.GenerateKeyPair (record.ToDictionary (), out public_key, out private_key), Is.EqualTo (SecStatusCode.Success), "GenerateKeyPair");

				byte [] plain = new byte [64]; // 64 * 8 == 512 - but there's the padding to consider
				byte [] cipher;
				Assert.That (public_key.Encrypt (SecPadding.PKCS1, plain, out cipher), Is.EqualTo (SecStatusCode.Param), "Encrypt");

				public_key.Dispose ();
				private_key.Dispose ();
			}
		}

		[Test]
		public void RoundtripRSA1024OAEP ()
		{
			SecKey private_key;
			SecKey public_key;
			using (var record = new SecRecord (SecKind.Key)) {
				record.KeyType = SecKeyType.RSA;
				record.KeySizeInBits = 1024; // it's not a performance test :)

				Assert.That (SecKey.GenerateKeyPair (record.ToDictionary (), out public_key, out private_key), Is.EqualTo (SecStatusCode.Success), "GenerateKeyPair");

				byte [] plain = new byte [0];
				byte [] cipher;
				Assert.That (public_key.Encrypt (SecPadding.OAEP, plain, out cipher), Is.EqualTo (SecStatusCode.Success), "Encrypt");
				public_key.Dispose ();

				byte[] result;
				Assert.That (private_key.Decrypt (SecPadding.OAEP, cipher, out result), Is.EqualTo (SecStatusCode.Success), "Decrypt");
				Assert.That (plain, Is.EqualTo (result), "match");
				private_key.Dispose ();
			}
		}

		[Test]
		public void SignVerifyRSA512PKCS1SHA1 ()
		{
			SecKey private_key;
			SecKey public_key;
			using (var record = new SecRecord (SecKind.Key)) {
				record.KeyType = SecKeyType.RSA;
				record.KeySizeInBits = 512; // it's not a performance test :)

				Assert.That (SecKey.GenerateKeyPair (record.ToDictionary (), out public_key, out private_key), Is.EqualTo (SecStatusCode.Success), "GenerateKeyPair");

				byte [] hash = new byte [20] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 };
				byte [] sign;
				Assert.That (private_key.RawSign (SecPadding.PKCS1SHA1, hash, out sign), Is.EqualTo (SecStatusCode.Success), "RawSign");
				Assert.That (public_key.RawVerify (SecPadding.PKCS1SHA1, hash, sign), Is.EqualTo (SecStatusCode.Success), "RawVerify");

				var empty = new byte [0];
				if (UIDevice.CurrentDevice.CheckSystemVersion (10, 0)) {
					Assert.That (private_key.RawSign (SecPadding.PKCS1SHA1, empty, out sign), Is.EqualTo (SecStatusCode.Success), "RawSign-empty");
					Assert.That (public_key.RawVerify (SecPadding.PKCS1SHA1, empty, empty), Is.EqualTo (SecStatusCode.Success), "RawVerify-empty");
				} else {
					Assert.That (private_key.RawSign (SecPadding.PKCS1SHA1, empty, out sign), Is.EqualTo (SecStatusCode.Param), "RawSign-empty");
					Assert.That (public_key.RawVerify (SecPadding.PKCS1SHA1, empty, empty), Is.EqualTo (SecStatusCode.Param), "RawVerify-empty");
				}

				private_key.Dispose ();
				public_key.Dispose ();
			}
		}

		[Test]
		public void SignVerifyECSHA1 ()
		{
			SecKey private_key;
			SecKey public_key;
			using (var record = new SecRecord (SecKind.Key)) {
				record.KeyType = SecKeyType.EC;
				record.KeySizeInBits = 256; // it's not a performance test :)

				Assert.That (SecKey.GenerateKeyPair (record.ToDictionary (), out public_key, out private_key), Is.EqualTo (SecStatusCode.Success), "GenerateKeyPair");

				byte [] hash = new byte [20] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 };
				byte [] sign;
				// PKCS1SHA1 implies RSA (oid)
				Assert.That (private_key.RawSign (SecPadding.PKCS1, hash, out sign), Is.EqualTo (SecStatusCode.Success), "RawSign");
				Assert.That (public_key.RawVerify (SecPadding.PKCS1, hash, sign), Is.EqualTo (SecStatusCode.Success), "RawVerify");

				var empty = new byte [0];
				// there does not seem to be a length-check on PKCS1, likely because not knowning the hash algorithm makes it harder
				Assert.That (private_key.RawSign (SecPadding.PKCS1, empty, out sign), Is.EqualTo (SecStatusCode.Success), "RawSign-empty");
				if (UIDevice.CurrentDevice.CheckSystemVersion (10, 0)) {
					Assert.That (public_key.RawVerify (SecPadding.PKCS1, empty, empty), Is.EqualTo (SecStatusCode.Success), "RawVerify-empty");
				} else {
					// but that does not work at verification time
					Assert.That (public_key.RawVerify (SecPadding.PKCS1, empty, empty), Is.EqualTo ((SecStatusCode)(-9809)), "RawVerify-empty");
				}

				private_key.Dispose ();
				public_key.Dispose ();
			}
		}

		[Test]
		public void GenerateKeyPairTooLargeRSA ()
		{
			SecKey private_key;
			SecKey public_key;
			using (var record = new SecRecord (SecKind.Key)) {
				record.KeyType = SecKeyType.RSA;
				// maximum documented as 2048, .NET maximum is 16384
				record.KeySizeInBits = 16384; 
				Assert.That (SecKey.GenerateKeyPair (record.ToDictionary (), out public_key, out private_key), Is.EqualTo (SecStatusCode.Param), "16384");
				record.KeySizeInBits = 8192; 
				if (TestRuntime.CheckiOSSystemVersion (9, 0)) {
					// It seems iOS 9 supports 8192, but it takes a long time to generate (~40 seconds on my iPad Air 2), so skip it.
//					Assert.That (SecKey.GenerateKeyPair (record.ToDictionary (), out public_key, out private_key), Is.EqualTo (SecStatusCode.Success), "8192");
				} else {
					Assert.That (SecKey.GenerateKeyPair (record.ToDictionary (), out public_key, out private_key), Is.EqualTo (SecStatusCode.Param), "8192");
				}
			}
			/* On iOS 7.1 the device console logs will show:
			 * 
			 * Mar 18 08:27:30 Mercure monotouchtest[1397] <Warning>:  SecRSAPrivateKeyInit Invalid or missing key size in: {
			 * 	    bsiz = 16384;
			 * 	    class = keys;
			 * 	    type = 42;
			 * 	}
			 * Mar 18 08:27:30 Mercure monotouchtest[1397] <Warning>:  SecKeyCreate init RSAPrivateKey key: -50
			*/
		}

		[Test]
		[Ignore ("Just to compare performance with BenchmarkNative4096")]
		public void BenchmarkManaged4096 ()
		{
			var chrono = new Stopwatch ();
			chrono.Start ();
			var rsa = new RSACryptoServiceProvider (4096);
			Assert.IsNotNull (rsa.ExportParameters (true), "ExportParameters"); // that will provoke the key generation
			Console.WriteLine ("Key generation {0} ms", chrono.ElapsedMilliseconds);
			chrono.Restart ();

			byte [] hash = new byte [20] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 };
			var result = rsa.Encrypt (hash, true);
			Console.WriteLine ("Encrypt {0} ms", chrono.ElapsedMilliseconds);
			chrono.Restart ();

			rsa.Decrypt (result, true);
			Console.WriteLine ("Decrypt {0} ms", chrono.ElapsedMilliseconds);
			chrono.Restart ();

			result = rsa.SignHash (hash, null /* SHA1 */);
			Console.WriteLine ("Sign {0} ms", chrono.ElapsedMilliseconds);
			chrono.Restart ();

			rsa.VerifyHash (hash, null, result);
			Console.WriteLine ("Verify {0} ms", chrono.ElapsedMilliseconds);
		}

		[Test]
		[Ignore ("Just to compare performance with BenchmarkManaged4096")]
		public void BenchmarkNative4096 ()
		{
			SecKey private_key;
			SecKey public_key;
			using (var record = new SecRecord (SecKind.Key)) {
				record.KeyType = SecKeyType.RSA;
				record.KeySizeInBits = 4096;

				var chrono = new Stopwatch ();
				chrono.Start ();
				Assert.That (SecKey.GenerateKeyPair (record.ToDictionary (), out public_key, out private_key), Is.EqualTo (SecStatusCode.Success), "GenerateKeyPair");
				Console.WriteLine ("Key generation {0} ms", chrono.ElapsedMilliseconds);
				chrono.Restart ();

				byte [] hash = new byte [20] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 };
				byte[] result;
				public_key.Encrypt (SecPadding.OAEP, hash, out result);
				Console.WriteLine ("Encrypt {0} ms", chrono.ElapsedMilliseconds);
				chrono.Restart ();

				private_key.Decrypt (SecPadding.OAEP, result, out result);
				Console.WriteLine ("Decrypt {0} ms", chrono.ElapsedMilliseconds);
				chrono.Restart ();

				private_key.RawSign (SecPadding.PKCS1SHA1, hash, out result);
				Console.WriteLine ("Sign {0} ms", chrono.ElapsedMilliseconds);
				chrono.Restart ();

				public_key.RawVerify (SecPadding.PKCS1SHA1, hash, result);
				Console.WriteLine ("Verify {0} ms", chrono.ElapsedMilliseconds);

				private_key.Dispose ();
				public_key.Dispose ();
			}
		}
	}
}
