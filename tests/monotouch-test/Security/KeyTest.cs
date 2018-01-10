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
#if MONOMAC
using AppKit;
#else
using UIKit;
#endif
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

#if MONOMAC
		// kSecRSAMin          = 1024 - see https://bugzilla.xamarin.com/show_bug.cgi?id=51277
		const int MinRsaKeySize = 1024;
#else
		const int MinRsaKeySize = 512;
#endif

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
		public void RoundtripRSAMinPKCS1 ()
		{
			NSError error;
			SecKey private_key;
			SecKey public_key;
			using (var record = new SecRecord (SecKind.Key)) {
				record.KeyType = SecKeyType.RSA;
				record.KeySizeInBits = MinRsaKeySize; // it's not a performance test :)

				Assert.That (SecKey.GenerateKeyPair (record.ToDictionary (), out public_key, out private_key), Is.EqualTo (SecStatusCode.Success), "GenerateKeyPair");

				byte [] plain = new byte [20] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 };
				byte [] cipher;
				if (TestRuntime.CheckXcodeVersion (8,0)) {
					Assert.True (public_key.IsAlgorithmSupported (SecKeyOperationType.Encrypt, SecKeyAlgorithm.RsaEncryptionPkcs1), "public/IsAlgorithmSupported/Encrypt");

#if MONOMAC
					Assert.That (public_key.IsAlgorithmSupported (SecKeyOperationType.Decrypt, SecKeyAlgorithm.RsaEncryptionPkcs1), Is.EqualTo (TestRuntime.CheckMacSystemVersion (10, 13)), "public/IsAlgorithmSupported/Decrypt");

					using (var pub = public_key.GetPublicKey ()) {
						// a new native instance of the key is returned (so having a new managed SecKey is fine)
						Assert.True (pub.Handle == public_key.Handle, "public/GetPublicKey");
					}
#else
					Assert.True (public_key.IsAlgorithmSupported (SecKeyOperationType.Decrypt, SecKeyAlgorithm.RsaEncryptionPkcs1), "public/IsAlgorithmSupported/Decrypt");

					using (var pub = public_key.GetPublicKey ())
					{
						// a new native instance of the key is returned (so having a new managed SecKey is fine)
						Assert.False (pub.Handle == public_key.Handle, "public/GetPublicKey");
					}
#endif

					using (var attrs = public_key.GetAttributes ()) {
						Assert.That (attrs.Count, Is.GreaterThan (0), "public/GetAttributes");
					}
					using (var data = public_key.GetExternalRepresentation (out error)) {
						Assert.Null (error, "public/error-1");
						Assert.NotNull (data, "public/GetExternalRepresentation");

						using (var key = SecKey.Create (data, SecKeyType.RSA, SecKeyClass.Public, MinRsaKeySize, null, out error)) {
							Assert.Null (error, "public/Create/error-1");
						}
					}
				}
				Assert.That (public_key.Encrypt (SecPadding.PKCS1, plain, out cipher), Is.EqualTo (SecStatusCode.Success), "Encrypt");

				byte[] result;
				if (TestRuntime.CheckXcodeVersion (8,0)) {
					Assert.False (private_key.IsAlgorithmSupported (SecKeyOperationType.Encrypt, SecKeyAlgorithm.RsaEncryptionPkcs1), "private/IsAlgorithmSupported/Encrypt");
					Assert.True (private_key.IsAlgorithmSupported (SecKeyOperationType.Decrypt, SecKeyAlgorithm.RsaEncryptionPkcs1), "private/IsAlgorithmSupported/Decrypt");

#if MONOMAC
					using (var pub2 = private_key.GetPublicKey ()) {
						Assert.That (pub2.Handle, Is.EqualTo (public_key.Handle), "private/GetPublicKey");
					}
#else
					using (var pub2 = private_key.GetPublicKey ()) {
						// a new native instance of the key is returned (so having a new managed SecKey is fine)
						Assert.That (pub2.Handle, Is.Not.EqualTo (public_key.Handle), "private/GetPublicKey");
					}
#endif
					using (var attrs = private_key.GetAttributes ()) {
						Assert.That (attrs.Count, Is.GreaterThan (0), "private/GetAttributes");
					}
					using (var data2 = private_key.GetExternalRepresentation (out error)) {
						Assert.Null (error, "private/error-1");
						Assert.NotNull (data2, "private/GetExternalRepresentation");

						using (var key = SecKey.Create (data2, SecKeyType.RSA, SecKeyClass.Private, MinRsaKeySize, null, out error)) {
							Assert.Null (error, "private/Create/error-1");
						}
					}
				}
				public_key.Dispose ();
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
				record.KeySizeInBits = MinRsaKeySize; // it's not a performance test :)

				Assert.That (SecKey.GenerateKeyPair (record.ToDictionary (), out public_key, out private_key), Is.EqualTo (SecStatusCode.Success), "GenerateKeyPair");

				byte [] plain = new byte [MinRsaKeySize / 8];
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
				if (TestRuntime.CheckXcodeVersion (8,0)) {
					Assert.True (public_key.IsAlgorithmSupported (SecKeyOperationType.Encrypt, SecKeyAlgorithm.RsaEncryptionOaepSha1), "public/IsAlgorithmSupported/Encrypt");
					// I would have expect false
#if MONOMAC
					Assert.That (public_key.IsAlgorithmSupported (SecKeyOperationType.Decrypt, SecKeyAlgorithm.RsaEncryptionOaepSha1), Is.EqualTo (TestRuntime.CheckMacSystemVersion (10, 13)), "public/IsAlgorithmSupported/Decrypt");
#else
 					Assert.True (public_key.IsAlgorithmSupported (SecKeyOperationType.Decrypt, SecKeyAlgorithm.RsaEncryptionOaepSha1), "public/IsAlgorithmSupported/Decrypt");
#endif
				}
				Assert.That (public_key.Encrypt (SecPadding.OAEP, plain, out cipher), Is.EqualTo (SecStatusCode.Success), "Encrypt");
				public_key.Dispose ();

				byte[] result;
				if (TestRuntime.CheckXcodeVersion (8,0)) {
					Assert.False (private_key.IsAlgorithmSupported (SecKeyOperationType.Encrypt, SecKeyAlgorithm.RsaEncryptionOaepSha1), "private/IsAlgorithmSupported/Encrypt");
					Assert.True (private_key.IsAlgorithmSupported (SecKeyOperationType.Decrypt, SecKeyAlgorithm.RsaEncryptionOaepSha1), "private/IsAlgorithmSupported/Decrypt");
				}
				Assert.That (private_key.Decrypt (SecPadding.OAEP, cipher, out result), Is.EqualTo (SecStatusCode.Success), "Decrypt");
				Assert.That (plain, Is.EqualTo (result), "match");
				private_key.Dispose ();
			}
		}

		[Test]
		public void SignVerifyRSAMinPKCS1SHA1 ()
		{
			SecKey private_key;
			SecKey public_key;
			using (var record = new SecRecord (SecKind.Key)) {
				record.KeyType = SecKeyType.RSA;
				record.KeySizeInBits = MinRsaKeySize; // it's not a performance test :)

				Assert.That (SecKey.GenerateKeyPair (record.ToDictionary (), out public_key, out private_key), Is.EqualTo (SecStatusCode.Success), "GenerateKeyPair");

				byte [] hash = new byte [20] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 };
				byte [] sign;
				Assert.That (private_key.RawSign (SecPadding.PKCS1SHA1, hash, out sign), Is.EqualTo (SecStatusCode.Success), "RawSign");
				Assert.That (public_key.RawVerify (SecPadding.PKCS1SHA1, hash, sign), Is.EqualTo (SecStatusCode.Success), "RawVerify");

				var empty = new byte [0];
				Assert.That (private_key.RawSign (SecPadding.PKCS1, empty, out sign), Is.EqualTo (SecStatusCode.Success), "RawSign-empty");
				// results vary per iOS version - but that's out of our control and we only care that it does not crash
				public_key.RawVerify (SecPadding.PKCS1SHA1, empty, empty);

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
				// results vary per iOS version - but that's out of our control and we only care that it does not crash
				public_key.RawVerify (SecPadding.PKCS1, empty, empty);

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
				if (TestRuntime.CheckXcodeVersion (7, 0)) {
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

		[Test]
		public void RSA ()
		{
			TestRuntime.AssertXcodeVersion (8, 0);
			NSError error;
			using (var key = SecKey.CreateRandomKey (SecKeyType.RSA, MinRsaKeySize, null, out error)) {
				Assert.Null (error, "RSA/error");

				using (var data = NSData.FromArray (new byte [] { 1, 2, 3 })) {
					using (var sig = key.CreateSignature (SecKeyAlgorithm.RsaSignatureRaw, data, out error)) {
						Assert.Null (error, "Sign/error");

						using (var pub = key.GetPublicKey ()) {
							var result = pub.VerifySignature (SecKeyAlgorithm.RsaSignatureRaw, data, sig, out error);
							Assert.Null (error, "Verify/no-error");
							Assert.True (result, "Verify/true");

							result = pub.VerifySignature (SecKeyAlgorithm.RsaSignatureRaw, data, data, out error);
							Assert.NotNull (error, "Verify/error");
							Assert.False (result, "Verify/false");

							using (var cipher = pub.CreateEncryptedData (SecKeyAlgorithm.RsaEncryptionPkcs1, data, out error)) {
								Assert.Null (error, "Encrypt/error");

								using (var plain = key.CreateDecryptedData (SecKeyAlgorithm.RsaEncryptionPkcs1, cipher, out error)) {
									Assert.Null (error, "Decrypt/error");
									Assert.That (data.ToArray (), Is.EqualTo (plain.ToArray ()), "roundtrip");
								}

								Assert.Null (key.CreateDecryptedData (SecKeyAlgorithm.RsaEncryptionPkcs1, data, out error), "bad data");
								Assert.NotNull (error, "bad decrypt");
							}
						}
					}

					using (var sig = key.CreateSignature (SecKeyAlgorithm.EcdsaSignatureRfc4754, data, out error)) {
						Assert.NotNull (error, "wrong key type");
					}
				}
			}
		}

		[Test]
		public void EC ()
		{
			TestRuntime.AssertXcodeVersion (8, 0);
			NSError error;
			using (var key = SecKey.CreateRandomKey (SecKeyType.EC, 384, null, out error)) {
				Assert.Null (error, "EC/error");
				using (var data = NSData.FromArray (new byte [] { 1, 2, 3 })) {
					using (var sig = key.CreateSignature (SecKeyAlgorithm.EcdsaSignatureRfc4754, data, out error)) {
						Assert.Null (error, "Sign/error");

						using (var pub = key.GetPublicKey ()) {
							var result = pub.VerifySignature (SecKeyAlgorithm.EcdsaSignatureRfc4754, data, sig, out error);
							Assert.Null (error, "Verify/no-error");
							Assert.True (result, "Verify/true");

							result = pub.VerifySignature (SecKeyAlgorithm.EcdsaSignatureRfc4754, data, data, out error);
							Assert.NotNull (error, "Verify/error");
							Assert.False (result, "Verify/false");

							using (var cipher = pub.CreateEncryptedData (SecKeyAlgorithm.EciesEncryptionCofactorX963Sha1AesGcm, data, out error)) {
								Assert.Null (error, "Encrypt/error");

								using (var plain = key.CreateDecryptedData (SecKeyAlgorithm.EciesEncryptionCofactorX963Sha1AesGcm, cipher, out error)) {
									Assert.Null (error, "Decrypt/error");
									Assert.That (data.ToArray (), Is.EqualTo (plain.ToArray ()), "roundtrip");
								}

								Assert.Null (key.CreateDecryptedData (SecKeyAlgorithm.EciesEncryptionCofactorX963Sha1AesGcm, data, out error), "bad data");
								Assert.NotNull (error, "bad decrypt");
							}
						}
					}

					using (var sig = key.CreateSignature (SecKeyAlgorithm.RsaSignatureRaw, data, out error)) {
						Assert.NotNull (error, "wrong key type");
					}
				}
			}
		}

		[Test]
		public void ECSecPrimeRandom ()
		{
			TestRuntime.AssertXcodeVersion (8,0);
			NSError error;
			using (var key = SecKey.CreateRandomKey (SecKeyType.ECSecPrimeRandom, 384, null, out error)) {
				Assert.Null (error, "ECSecPrimeRandom/error");

				SecKeyKeyExchangeParameter p = new SecKeyKeyExchangeParameter () {
					RequestedSize = 16,
					SharedInfo = NSData.FromArray (new byte [] { 4, 5, 6 })
				};

				using (var pub = key.GetPublicKey ())
				using (var ex = key.GetKeyExchangeResult (SecKeyAlgorithm.EcdhKeyExchangeStandardX963Sha512, pub, p.Dictionary, out error)) {
					Assert.Null (error, "GetKeyExchangeResult/error");
					Assert.That (ex.Length, Is.EqualTo (p.RequestedSize), "GetKeyExchangeResult/result");
				}
			}
		}
	}
}
