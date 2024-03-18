// Copyright 2014-2015 Xamarin Inc. All rights reserved

using System;
using System.Diagnostics;

using CoreFoundation;
using Foundation;
#if HAS_LOCALAUTHENTICATION
using LocalAuthentication;
#endif
using Security;
#if MONOMAC
using AppKit;
#else
using UIKit;
#endif
using NUnit.Framework;
using NUnit.Framework.Internal;
using System.Security.Cryptography.X509Certificates;

namespace MonoTouchFixtures.Security {

	[TestFixture]
	[Preserve (AllMembers = true)]
	public class RecordTest {

		static void InitSecRecord (SecRecord rec)
		{
#if __MACOS__
			// ignore on macOS 11.* (but not 12+)
			if (TestRuntime.CheckXcodeVersion (12, 2) && !TestRuntime.CheckXcodeVersion (13, 0))
				TestRuntime.IgnoreInCI ("Skip on macOS 11.* because it hangs");
#endif

#if HAS_LOCALAUTHENTICATION
			var context = new LAContext ();
			context.InteractionNotAllowed = true;
			rec.AuthenticationContext = context;
#else
			// This is deprecated, but there's no alternative on tvOS
			rec.AuthenticationUI = SecAuthenticationUI.Fail;
#endif
		}

		public static SecRecord CreateSecRecord (SecKind kind, string? account = null, NSData? valueData = null, SecAccessible? accessible = null, SecProtocol? protocol = null, string? server = null, SecAuthenticationType? authenticationType = null, string? service = null, string? label = null)
		{
			var rec = new SecRecord (kind);
			InitSecRecord (rec);

			if (account is not null)
				rec.Account = account;

			if (valueData is not null)
				rec.ValueData = valueData;

			if (accessible is not null)
				rec.Accessible = accessible.Value;

			if (protocol is not null)
				rec.Protocol = protocol.Value;

			if (server is not null)
				rec.Server = server;

			if (authenticationType is not null)
				rec.AuthenticationType = authenticationType.Value;

			if (service is not null)
				rec.Service = service;

			if (label is not null)
				rec.Label = label;

			return rec;
		}

		public static SecRecord CreateSecRecord (SecIdentity identity)
		{
			var rec = new SecRecord (identity);
			InitSecRecord (rec);
			return rec;
		}

		public static SecRecord CreateSecRecord (SecKey key)
		{
			var rec = new SecRecord (key);
			InitSecRecord (rec);
			return rec;
		}

		public static SecRecord CreateSecRecord (SecCertificate certificate, string? label = null)
		{
			var rec = new SecRecord (certificate);
			InitSecRecord (rec);

			if (label is not null)
				rec.Label = label;

			return rec;
		}

		[Test]
		public void Identity ()
		{
			var rec = CreateSecRecord (SecKind.Identity,
				account: "Username",
				valueData: NSData.FromString ("Password")
			);

			// prior to iOS7 you had to deal without the class
			using (var dict = rec.ToDictionary ()) {
				var hasIdnt = true;
#if !__MACOS__
				if (!TestRuntime.CheckXcodeVersion (5, 0))
					hasIdnt = false;
#endif
				if (hasIdnt)
					Assert.That (dict ["class"].ToString (), Is.EqualTo ("idnt"), "idnt");
				else
					Assert.Null (dict ["class"], "idnt");
			}
		}

		void Accessible (SecAccessible access)
		{
			var rec = CreateSecRecord (SecKind.GenericPassword,
				account: "Username"
			);
			SecKeyChain.Remove (rec); // it might already exists (or not)

			rec = CreateSecRecord (SecKind.GenericPassword,
				account: "Username",
				valueData: NSData.FromString ("Password"),
				accessible: access
			);

			Assert.That (SecKeyChain.Add (rec), Is.EqualTo (SecStatusCode.Success), "Add");

			SecStatusCode code;
			var match = SecKeyChain.QueryAsRecord (rec, out code);
			Assert.That (code, Is.EqualTo (SecStatusCode.Success), "QueryAsRecord");

			Assert.That (match.Accessible, Is.EqualTo (access), "Accessible");
		}

		[Test]
		public void Match ()
		{
			var rec = CreateSecRecord (SecKind.GenericPassword,
				account: "Username"
			);
			Assert.Null (rec.MatchIssuers, "MatchIssuers");
			// we do not have a way (except the getter) to craete SecKeyChain instances
			Assert.Null (rec.MatchItemList, "MatchItemList");

			using (var data = new NSData ()) {
				rec.MatchIssuers = new NSData [] { data };
				Assert.AreSame (rec.MatchIssuers [0], data, "MatchIssuers [0]");
			}

			if (!TestRuntime.CheckXcodeVersion (7, 0))
				return;
#if __TVOS__ || __WATCHOS__
			Assert.That (rec.AuthenticationUI, Is.EqualTo (SecAuthenticationUI.Fail), "AuthenticationUI-get");
#else
			Assert.That (rec.AuthenticationUI, Is.EqualTo (SecAuthenticationUI.NotSet), "AuthenticationUI-get");
#endif
			rec.AuthenticationUI = SecAuthenticationUI.Allow;
			Assert.That (rec.AuthenticationUI, Is.EqualTo (SecAuthenticationUI.Allow), "AuthenticationUI-set");
		}

		[Test]
#if MONOMAC
		[Ignore ("Returns SecAccessible.Invalid")]
#endif
#if __MACCATALYST__
		[Ignore ("This test requires an app signed with the keychain-access-groups entitlement, and for Mac Catalyst that requires a custom provisioning profile.")]
#endif
		public void Accessible_17579 ()
		{
			Accessible (SecAccessible.AfterFirstUnlock);
			Accessible (SecAccessible.AfterFirstUnlockThisDeviceOnly);
			Accessible (SecAccessible.Always);
			Accessible (SecAccessible.AlwaysThisDeviceOnly);
			Accessible (SecAccessible.WhenUnlocked);
			Accessible (SecAccessible.WhenUnlockedThisDeviceOnly);
		}

		void Protocol (SecProtocol protocol)
		{
			var account = $"Protocol-{protocol}-{CFBundle.GetMain ().Identifier}-{GetType ().FullName}-{Process.GetCurrentProcess ().Id}";
			var rec = CreateSecRecord (SecKind.InternetPassword,
				account: account
			);
			try {
				SecKeyChain.Remove (rec); // it might already exists (or not)

				rec = CreateSecRecord (SecKind.InternetPassword,
					account: account,
					valueData: NSData.FromString ("Password"),
					protocol: protocol,
					server: "www.xamarin.com"
				);

				Assert.That (SecKeyChain.Add (rec), Is.EqualTo (SecStatusCode.Success), $"Add: {protocol}");

				SecStatusCode code;
				var match = SecKeyChain.QueryAsRecord (rec, out code);
				Assert.That (code, Is.EqualTo (SecStatusCode.Success), $"QueryAsRecord: {protocol}");

				Assert.That (match.Protocol, Is.EqualTo (protocol), "Protocol");
			} finally {
				// Clean up after us
				SecKeyChain.Remove (rec);
			}
		}

		[Test]
#if __MACCATALYST__
		[Ignore ("This test requires an app signed with the keychain-access-groups entitlement, and for Mac Catalyst that requires a custom provisioning profile.")]
#endif
		public void Protocol_17579 ()
		{
			Protocol (SecProtocol.Afp);
			Protocol (SecProtocol.AppleTalk);
			Protocol (SecProtocol.Daap);
			Protocol (SecProtocol.Eppc);
			Protocol (SecProtocol.Ftp);
			Protocol (SecProtocol.FtpAccount);
			Protocol (SecProtocol.FtpProxy);
			Protocol (SecProtocol.Ftps);
			Protocol (SecProtocol.Http);
			Protocol (SecProtocol.HttpProxy);
			Protocol (SecProtocol.Https);
			Protocol (SecProtocol.HttpsProxy);
			Protocol (SecProtocol.Imap);
			Protocol (SecProtocol.Imaps);
			Protocol (SecProtocol.Ipp);
			Protocol (SecProtocol.Irc);
			Protocol (SecProtocol.Ircs);
			Protocol (SecProtocol.Ldap);
			Protocol (SecProtocol.Ldaps);
			Protocol (SecProtocol.Nntp);
			Protocol (SecProtocol.Nntps);
			Protocol (SecProtocol.Pop3);
			Protocol (SecProtocol.Pop3s);
			Protocol (SecProtocol.Rtsp);
			Protocol (SecProtocol.RtspProxy);
			Protocol (SecProtocol.Smb);
			Protocol (SecProtocol.Smtp);
			Protocol (SecProtocol.Socks);
			Protocol (SecProtocol.Ssh);
			Protocol (SecProtocol.Telnet);
			Protocol (SecProtocol.Telnets);
		}

		void AuthenticationType (SecAuthenticationType type)
		{
			var rec = CreateSecRecord (SecKind.InternetPassword,
				account: "AuthenticationType"
			);
			SecKeyChain.Remove (rec); // it might already exists (or not)

			rec = CreateSecRecord (SecKind.InternetPassword,
				account: $"{CFBundle.GetMain ().Identifier}-{GetType ().FullName}-{Process.GetCurrentProcess ().Id}",
				valueData: NSData.FromString ("Password"),
				authenticationType: type,
				server: "www.xamarin.com"
			);

			try {
				Assert.That (SecKeyChain.Add (rec), Is.EqualTo (SecStatusCode.Success), "Add");

				var query = CreateSecRecord (SecKind.InternetPassword,
					account: rec.Account,
					authenticationType: rec.AuthenticationType,
					server: rec.Server
				);

				SecStatusCode code;
				var match = SecKeyChain.QueryAsRecord (query, out code);
				Assert.That (code, Is.EqualTo (SecStatusCode.Success), "QueryAsRecord");

				Assert.That (match.AuthenticationType, Is.EqualTo (type), "AuthenticationType");
			} finally {
				// Clean up after us
				SecKeyChain.Remove (rec);
			}
		}

#if __MACCATALYST__
		[Ignore ("This test requires an app signed with the keychain-access-groups entitlement, and for Mac Catalyst that requires a custom provisioning profile.")]
#endif
		[Test]
		public void AuthenticationType_17579 ()
		{
			AuthenticationType (SecAuthenticationType.Default);
			AuthenticationType (SecAuthenticationType.Dpa);
			AuthenticationType (SecAuthenticationType.HtmlForm);
			AuthenticationType (SecAuthenticationType.HttpBasic);
			AuthenticationType (SecAuthenticationType.HttpDigest);
			AuthenticationType (SecAuthenticationType.Msn);
			AuthenticationType (SecAuthenticationType.Ntlm);
			AuthenticationType (SecAuthenticationType.Rpa);
		}

		// Test Case provided by user
		// This test case scenario used to fail under iOS 6 or lower
		[Test]
#if __MACCATALYST__
		[Ignore ("This test requires an app signed with the keychain-access-groups entitlement, and for Mac Catalyst that requires a custom provisioning profile.")]
#endif
		public void DeskCase_83099_InmutableDictionary ()
		{
			var testUsername = "testusername";

			//TEST 1: Save a keychain value
			var test1 = SaveUserPassword (testUsername, "testValue1");
			Assert.IsTrue (test1, "Password could not be saved to keychain");

			//TEST 2: Get the saved keychain value
			var test2 = GetUserPassword (testUsername);
			Assert.IsTrue (StringUtil.StringsEqual (test2, "testValue1", false));

			//TEST 3: Update the keychain value
			var test3 = SaveUserPassword (testUsername, "testValue2");
			Assert.IsTrue (test3, "Password could not be saved to keychain");

			//TEST 4: Get the updated keychain value
			var test4 = GetUserPassword (testUsername);
			Assert.IsTrue (StringUtil.StringsEqual (test4, "testValue2", false));

			//TEST 5: Clear the keychain values
			var test5 = ClearUserPassword (testUsername);
			Assert.IsTrue (test5, "Password could not be cleared from keychain");

			//TEST 6: Verify no keychain value
			var test6 = GetUserPassword (testUsername);
			Assert.IsNull (test6, "No password should exist here");
		}

		public static string GetUserPassword (string username)
		{
			string password = null;
			var searchRecord = CreateSecRecord (SecKind.InternetPassword,
				server: "Test1",
				account: username.ToLower ()
			);
			SecStatusCode code;
			var record = SecKeyChain.QueryAsRecord (searchRecord, out code);
			if (code == SecStatusCode.Success && record is not null)
				password = NSString.FromData (record.ValueData, NSStringEncoding.UTF8);
			return password;
		}

		public static bool SaveUserPassword (string username, string password)
		{
			var success = false;
			var searchRecord = CreateSecRecord (SecKind.InternetPassword,
				server: "Test1",
				account: username.ToLower ()
			);
			SecStatusCode queryCode;
			var record = SecKeyChain.QueryAsRecord (searchRecord, out queryCode);
			if (queryCode == SecStatusCode.ItemNotFound) {
				record = CreateSecRecord (SecKind.InternetPassword,
					server: "Test1",
					account: username.ToLower (),
					valueData: NSData.FromString (password)
				);
				var addCode = SecKeyChain.Add (record);
				success = (addCode == SecStatusCode.Success);
			}
			if (queryCode == SecStatusCode.Success && record is not null) {
				record.ValueData = NSData.FromString (password);
				var updateCode = SecKeyChain.Update (searchRecord, record);
				success = (updateCode == SecStatusCode.Success);
			}
			return success;
		}

		public static bool ClearUserPassword (string username)
		{
			var success = false;
			var searchRecord = CreateSecRecord (SecKind.InternetPassword,
				server: "Test1",
				account: username.ToLower ()
			);
			SecStatusCode queryCode;
			var record = SecKeyChain.QueryAsRecord (searchRecord, out queryCode);

			if (queryCode == SecStatusCode.Success && record is not null) {
				var removeCode = SecKeyChain.Remove (searchRecord);
				success = (removeCode == SecStatusCode.Success);
			}
			return success;
		}

		[Test]
#if MONOMAC
		[Ignore ("SecStatusCode code = SecKeyChain.Add (rec); returns SecStatusCode.Param")]
#elif __MACCATALYST__
		[Ignore ("This test requires an app signed with the keychain-access-groups entitlement, and for Mac Catalyst that requires a custom provisioning profile.")]
#endif
		public void IdentityRecordTest ()
		{
			if (TestRuntime.CheckXcodeVersion (13, 0))
				Assert.Ignore ("code == errSecInternal (-26276)");

			using (var identity = IdentityTest.GetIdentity ())
			using (var rec = CreateSecRecord (identity)) {
				SecStatusCode code = SecKeyChain.Add (rec);
				Assert.True (code == SecStatusCode.DuplicateItem || code == SecStatusCode.Success, "Identity added");

				var ret = rec.GetIdentity ();
				Assert.NotNull (ret, "ret is null");
				Assert.That (identity.Handle, Is.EqualTo (ret.Handle), "Same Handle");

				Assert.Throws<InvalidOperationException> (() => rec.GetKey (), "GetKey should throw");
				Assert.Throws<InvalidOperationException> (() => rec.GetCertificate (), "GetCertificate should throw");
			}
		}

#if !MONOMAC // Works different on Mac
		[Test]
		public void SecRecordRecordTest ()
		{
			using (var cert = new X509Certificate (CertificateTest.mail_google_com))
			using (var sc = new SecCertificate (cert))
			using (var rec = CreateSecRecord (sc)) {
				Assert.NotNull (rec, "rec is null");

				var ret = rec.GetCertificate ();
				Assert.That (ret.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle");
#if !NET
				// dotnet PAL layer does not return the same instance
				Assert.That (ret.Handle, Is.EqualTo (cert.Handle), "Same Handle");
#endif
				Assert.That (cert.ToString (true), Is.EqualTo (ret.ToX509Certificate ().ToString (true)), "X509Certificate");

				Assert.Throws<InvalidOperationException> (() => rec.GetKey (), "GetKey should throw");
				Assert.Throws<InvalidOperationException> (() => rec.GetIdentity (), "GetIdentity should throw");
			}
		}

		[Test]
		public void KeyRecordTest ()
		{
			using (var cert = new X509Certificate2 (ImportExportTest.farscape_pfx, "farscape"))
			using (var policy = SecPolicy.CreateBasicX509Policy ())
			using (var trust = new SecTrust (cert, policy)) {
				trust.Evaluate ();
				using (SecKey pubkey = trust.GetPublicKey ())
				using (var rec = CreateSecRecord (pubkey)) {
					Assert.NotNull (rec, "rec is null");

					var ret = rec.GetKey ();
					Assert.That (ret.Handle, Is.Not.EqualTo (IntPtr.Zero), "Handle");
					Assert.That (ret.Handle, Is.EqualTo (pubkey.Handle), "Same Handle");

					Assert.Throws<InvalidOperationException> (() => rec.GetCertificate (), "GetCertificate should throw");
					Assert.Throws<InvalidOperationException> (() => rec.GetIdentity (), "GetIdentity should throw");
				}
			}
		}
#endif
	}
}
