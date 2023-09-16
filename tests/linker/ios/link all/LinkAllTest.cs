//
// Link All [Regression] Tests
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2012-2016 Xamarin Inc. All rights reserved.
//

using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;

using MonoTouch;
using CoreGraphics;
using Foundation;
using ObjCRuntime;
#if !__WATCHOS__
using StoreKit;
#endif
#if __MACOS__ || __IOS__
using PdfKit;
#endif
#if !__MACOS__
using UIKit;
#endif
using NUnit.Framework;
using MonoTests.System.Net.Http;

namespace LinkAll {

	// we DO NOT want the code to be "fully" available
	public class NotPreserved {

		public byte One {
			get; set;
		}

		[DefaultValue (2)]
		public int Two {
			get; set;
		}
	}

	// nothing directly uses Parent
	public class Parent {
		// but the nested type is a subclass of NSObject and gets preserved (as it's not part of monotouch.dll)
		public class Derived : NSObject {
			[Export ("foo")]
			public void Foo () { }
		}

		public void UnusedMethod () { }
	}

	[TestFixture]
	// we want the tests to be available because we use the linker
	[Preserve (AllMembers = true)]
	public class LinkAllRegressionTest {
#if __MACCATALYST__
		public const string NamespacePrefix = "";
#if NET
		public const string AssemblyName = "Microsoft.MacCatalyst";
#else
		public const string AssemblyName = "Xamarin.MacCatalyst";
#endif
#elif __IOS__
		public const string NamespacePrefix = "";
#if NET
		public const string AssemblyName = "Microsoft.iOS";
#else
		public const string AssemblyName = "Xamarin.iOS";
#endif
#elif __TVOS__
		public const string NamespacePrefix = "";
#if NET
		public const string AssemblyName = "Microsoft.tvOS";
#else
		public const string AssemblyName = "Xamarin.TVOS";
#endif
#elif __WATCHOS__
		public const string NamespacePrefix = "";
#if NET
		public const string AssemblyName = "Microsoft.watchOS";
#else
		public const string AssemblyName = "Xamarin.WatchOS";
#endif
#elif __MACOS__
		public const string NamespacePrefix = "";
#if NET
		public const string AssemblyName = "Microsoft.macOS";
#else
		public const string AssemblyName = "Xamarin.Mac";
#endif
#else
#error Unknown platform
#endif

		Type not_preserved_type = typeof (NotPreserved);


		class TypeAttribute : Attribute {
			public TypeAttribute (Type type) { }
		}

		[Type (null)]
		public void NullTypeInAttribute ()
		{
			// there's no need to execute this test.
			// desk #68380.
		}

		[Test]
		public void GetterOnly ()
		{
			// that ensure the getter is not linked away, 
			// which means the property will be available for MEF_3862
			NotPreserved np = new NotPreserved ();
			Assert.That (np.Two, Is.EqualTo (0), "Two==0");

			PropertyInfo pi = not_preserved_type.GetProperty ("Two");
			// check the *unused* setter absence from the application
			Assert.NotNull (pi.GetGetMethod (), "getter");
			Assert.Null (pi.GetSetMethod (), "setter");
		}

		[Test]
		public void SetterOnly ()
		{
			// that ensure the setter is not linked away, 
			NotPreserved np = new NotPreserved ();
			np.One = 1;

			PropertyInfo pi = not_preserved_type.GetProperty ("One");
			// check the *unused* setter absence from the application
			Assert.Null (pi.GetGetMethod (), "getter");
			Assert.NotNull (pi.GetSetMethod (), "setter");
		}

		[Test]
		public void MEF_3862 ()
		{
			// note: avoiding using "typeof(DefaultValueAttribute)" in the code
			// so the linker does not keep it just because of it
			PropertyInfo pi = not_preserved_type.GetProperty ("Two");
			object [] attrs = pi.GetCustomAttributes (false);
			bool default_value = false;
			foreach (var ca in attrs) {
				if (ca.GetType ().Name == "DefaultValueAttribute") {
					default_value = true;
					break;
				}
			}
			Assert.True (default_value, "DefaultValue");
		}

#if NET
		static void Check (string calendarName, bool present)
		{
			var type = Type.GetType ("System.Globalization." + calendarName);
			bool success = present == (type is not null);
			Assert.AreEqual (present, type is not null, calendarName);
		}

		[Test]
		public void Calendars ()
		{
			Check ("GregorianCalendar", true);
			Check ("UmAlQuraCalendar", true);
			Check ("HijriCalendar", true);
			Check ("ThaiBuddhistCalendar", true);
		}
#endif // NET

		public enum CertificateProblem : long {
			CertEXPIRED = 0x800B0101,
			CertVALIDITYPERIODNESTING = 0x800B0102,
			CertROLE = 0x800B0103,
			CertPATHLENCONST = 0x800B0104,
			CertCRITICAL = 0x800B0105,
			CertPURPOSE = 0x800B0106,
			CertISSUERCHAINING = 0x800B0107,
			CertMALFORMED = 0x800B0108,
			CertUNTRUSTEDROOT = 0x800B0109,
			CertCHAINING = 0x800B010A,
			CertREVOKED = 0x800B010C,
			CertUNTRUSTEDTESTROOT = 0x800B010D,
			CertREVOCATION_FAILURE = 0x800B010E,
			CertCN_NO_MATCH = 0x800B010F,
			CertWRONG_USAGE = 0x800B0110,
			CertUNTRUSTEDCA = 0x800B0112,
			CertTRUSTEFAIL = 0x800B010B,
		}

#if !NET
		// ICertificatePolicy has been removed from .NET 5+
		class TestPolicy : ICertificatePolicy {

			const int RecoverableTrustFailure = 5; // SecTrustResult

			public TestPolicy ()
			{
				CheckCount = 0;
			}

			public int CheckCount { get; private set; }

			public bool CheckValidationResult (ServicePoint srvPoint, X509Certificate certificate, WebRequest request, int certificateProblem)
			{
				Assert.That (certificateProblem, Is.EqualTo (0), GetProblemMessage ((CertificateProblem) certificateProblem));
				CheckCount++;
				return true;
			}

			string GetProblemMessage (CertificateProblem problem)
			{
				var problemMessage = "";
				CertificateProblem problemList = new CertificateProblem ();
				var problemCodeName = Enum.GetName (problemList.GetType (), problem);
				problemMessage = problemCodeName is not null ? problemMessage + "-Certificateproblem:" + problemCodeName : "Unknown Certificate Problem";
				return problemMessage;
			}
		}

		static TestPolicy test_policy = new TestPolicy ();

		[Test]
		public void TrustUsingOldPolicy ()
		{
#if __WATCHOS__
			Assert.Ignore ("WatchOS doesn't support BSD sockets, which our network stack currently requires.");
#endif
			// Three similar tests exists in dontlink, linkall and linksdk to test 3 different cases
			// untrusted, custom ICertificatePolicy and ServerCertificateValidationCallback without
			// having caching issues (in S.Net or the SSL handshake cache)
			ICertificatePolicy old = ServicePointManager.CertificatePolicy;
			try {
				ServicePointManager.CertificatePolicy = test_policy;
				WebClient wc = new WebClient ();
				Assert.IsNotNull (wc.DownloadString (NetworkResources.XamarinUrl));
				// caching means it will be called at least for the first run, but it might not
				// be called again in subsequent requests (unless it expires)
				Assert.That (test_policy.CheckCount, Is.GreaterThan (0), "policy checked");
			} catch (WebException we) {
				TestRuntime.IgnoreInCIIfBadNetwork (we);
				throw;
			} finally {
				ServicePointManager.CertificatePolicy = old;
			}
		}
#endif

#if !__MACOS__
		[Test]
		public void DetectPlatform ()
		{
#if !__WATCHOS__
			// for (future) nunit[lite] platform detection - if this test fails then platform detection won't work
			var typename = NamespacePrefix + "UIKit.UIApplicationDelegate, " + AssemblyName;
			Assert.NotNull (Helper.GetType (typename), typename);
#endif
#if NET
			Assert.Null (Helper.GetType ("Mono.Runtime"), "Mono.Runtime");
#else
			// and you can trust the old trick with the linker
			Assert.NotNull (Helper.GetType ("Mono.Runtime"), "Mono.Runtime");
#endif
		}
#endif // !__MACOS__

		[Test]
#if !XAMCORE_3_0 && !NET
		[Availability ()]
#endif
#if NET
		[SupportedOSPlatform ("none")]
		[UnsupportedOSPlatform ("none)")]
#else
		[Introduced (PlatformName.None)]
		[Deprecated (PlatformName.None)]
		[Obsoleted (PlatformName.None)]
		[Unavailable (PlatformName.None)]
#endif
		[ThreadSafe]
		public void RemovedAttributes ()
		{
			// Don't use constants here, because the linker can see what we're trying to do and keeps what we're verifying has been removed.
			string prefix = NamespacePrefix;
			string suffix = AssemblyName;

			// since we're linking the attributes will NOT be available - even if they are used
#if !XAMCORE_3_0
			Assert.Null (Helper.GetType (prefix + "ObjCRuntime.AvailabilityAttribute, " + suffix), "AvailabilityAttribute");
#endif
			Assert.Null (Helper.GetType (prefix + "ObjCRuntime.IntroducedAttribute, " + suffix), "IntroducedAttribute");
			Assert.Null (Helper.GetType (prefix + "ObjCRuntime.DeprecatedAttribute, " + suffix), "DeprecatedAttribute");
			Assert.Null (Helper.GetType (prefix + "ObjCRuntime.ObsoletedAttribute, " + suffix), "ObsoletedAttribute");
			Assert.Null (Helper.GetType (prefix + "ObjCRuntime.UnavailableAttribute, " + suffix), "UnavailableAttribute");
			Assert.Null (Helper.GetType (prefix + "ObjCRuntime.ThreadSafeAttribute, " + suffix), "ThreadSafeAttribute");
#if NET
			Assert.Null (Helper.GetType ("System.Runtime.Versioning.SupportedOSPlatformAttribute, " + suffix), "SupportedOSPlatformAttribute");
			Assert.Null (Helper.GetType ("System.Runtime.Versioning.UnsupportedOSPlatformAttribute, " + suffix), "UnsupportedOSPlatformAttribute");
#endif
		}

		[Test]
		public void Assembly_Load ()
		{
#if NET
			Assembly mscorlib = Assembly.Load ("System.Private.CoreLib.dll");
			Assert.NotNull (mscorlib, "System.Private.CoreLib.dll");
#else
			Assembly mscorlib = Assembly.Load ("mscorlib.dll");
			Assert.NotNull (mscorlib, "mscorlib");

			Assembly system = Assembly.Load ("System.dll");
			Assert.NotNull (system, "System");
#endif
		}

		string FindAssemblyPath ()
		{
			var filename = Path.GetFileName (GetType ().Assembly.Location);
			var bundlePath = NSBundle.MainBundle.BundlePath;
			var isExtension = bundlePath.EndsWith (".appex", StringComparison.Ordinal);
			var mainBundlePath = bundlePath;
			if (isExtension)
				mainBundlePath = Path.GetDirectoryName (Path.GetDirectoryName (bundlePath));
			foreach (var filepath in Directory.EnumerateFiles (mainBundlePath, filename, SearchOption.AllDirectories)) {
				var fname = Path.GetFileName (filepath);
				if (filepath.EndsWith ($"{fname}.framework/{fname}", StringComparison.Ordinal)) {
					// This isn't the assembly, but the native AOT'ed executable for the assembly.
					continue;
				}

				if (isExtension) {
					return "../../" + filepath.Substring (mainBundlePath.Length + 1);
				} else {
					return filepath.Substring (mainBundlePath.Length + 1);
				}
			}
			throw new FileNotFoundException ($"Could not find the assembly ${filename} in the bundle {bundlePath}.");
		}

		[Test]
		public void Assembly_LoadFile ()
		{
			string filename = FindAssemblyPath ();
			Assert.NotNull (Assembly.LoadFile (Path.GetFullPath (filename)), "1");
		}

		[Test]
		public void Assembly_LoadFrom ()
		{
			string filename = FindAssemblyPath ();
			Assert.NotNull (Assembly.LoadFrom (filename), "1");
		}

		[Test]
		public void Assembly_ReflectionOnlyLoadFrom ()
		{
			string filename = FindAssemblyPath ();
#if NET
			// new behavior across all platforms, see https://github.com/dotnet/runtime/issues/50529
			Assert.Throws<PlatformNotSupportedException> (() => Assembly.ReflectionOnlyLoadFrom (filename));
#else
			Assert.NotNull (Assembly.ReflectionOnlyLoadFrom (filename), "1");
#endif
		}

#if !NET
		[Test]
		public void SystemDataSqlClient ()
		{
#if __WATCHOS__
			Assert.Throws<PlatformNotSupportedException> (() => new System.Data.SqlClient.SqlConnection ());
#else
			// notes:
			// * this test is mean to fail when building the application using a Community or Indie licenses
			// * linksdk.app references System.Data (assembly) but not types in SqlClient namespace
			using (var sc = new System.Data.SqlClient.SqlConnection ()) {
				Assert.NotNull (sc);
			}
#endif
		}
#endif

#if !__TVOS__ && !__WATCHOS__ && !__MACOS__
		[Test]
		public void Pasteboard_ImagesTest ()
		{
			string file = Path.Combine (NSBundle.MainBundle.ResourcePath, "basn3p08.png");
			using (var dp = new CGDataProvider (file)) {
				using (var cgimg = CGImage.FromPNG (dp, null, false, CGColorRenderingIntent.Default)) {
					using (var img = new UIImage (cgimg)) {
						UIPasteboard.General.Images = new UIImage [] { img };
						if (TestRuntime.CheckXcodeVersion (8, 0))
							Assert.True (UIPasteboard.General.HasImages, "HasImages");

						Assert.AreEqual (1, UIPasteboard.General.Images.Length, "a - length");

						UIPasteboard.General.Images = new UIImage [] { img, img };
						Assert.AreEqual (2, UIPasteboard.General.Images.Length, "b - length");
						Assert.IsNotNull (UIPasteboard.General.Images [0], "b - nonnull[0]");
						Assert.IsNotNull (UIPasteboard.General.Images [1], "b - nonnull[0]");
					}
				}
			}
		}
#endif // !__TVOS__

		[Test]
		public void UltimateBindings ()
		{
			Assert.IsNotNull (Bindings.Test.UltimateMachine.SharedInstance, "SharedInstance");
		}

		#region bug 14456

		[TypeConverter (typeof (TestEnumTypeConverter))]
		public enum TestEnum {
			One, Two
		}

		[Preserve (AllMembers = true)]
		public class TestEnumTypeConverter : System.ComponentModel.TypeConverter {

			public override object ConvertTo (ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
			{
				return "hello";
			}
		}

		[Test]
		public void Bug14456 ()
		{
			var tc = TypeDescriptor.GetConverter (typeof (TestEnum));
			// not using Is.TypeOf since it would give additional clue to the linker
			Assert.That (tc.GetType ().Name, Is.EqualTo ("TestEnumTypeConverter"), "TestEnumTypeConverter");
			// notes:
			// * without [Preserve (AllMembers = true)] -> MissingMethodException : Default constructor 
			// * without [TypeConverter (typeof (TestEnumTypeConverter))] -> EnumConverter
		}

		#endregion

		[Test]
		public void SingleEpsilon_Compare ()
		{
			TestRuntime.AssertNotDevice ("Known to fail on devices, see bug #15802");
			// works on some ARM CPU (e.g. iPhone5S) but not others (iPad4 or iPodTouch5)
			Assert.That (Single.Epsilon, Is.Not.EqualTo (0f), "Epsilon");
			Assert.That (-Single.Epsilon, Is.Not.EqualTo (0f), "-Epsilon");
		}

		[Test]
		public void SingleEpsilon_ToString ()
		{
			TestRuntime.AssertNotDevice ("Known to fail on devices, see bug #15802");
			var ci = CultureInfo.InvariantCulture;
#if NET
			Assert.That (Single.Epsilon.ToString (ci), Is.EqualTo ("1E-45"), "Epsilon.ToString()");
			Assert.That ((-Single.Epsilon).ToString (ci), Is.EqualTo ("-1E-45"), "-Epsilon.ToString()");
#else
			Assert.That (Single.Epsilon.ToString (ci), Is.EqualTo ("1.401298E-45"), "Epsilon.ToString()");
			Assert.That ((-Single.Epsilon).ToString (ci), Is.EqualTo ("-1.401298E-45"), "-Epsilon.ToString()");
#endif
		}

		[Test]
		public void DoubleEpsilon_Compare ()
		{
			TestRuntime.AssertNotDevice ("Known to fail on devices, see bug #15802");
			// works on some ARM CPU (e.g. iPhone5S) but not others (iPad4 or iPodTouch5)
			Assert.That (Double.Epsilon, Is.Not.EqualTo (0f), "Epsilon");
			Assert.That (-Double.Epsilon, Is.Not.EqualTo (0f), "-Epsilon");
		}

		[Test]
		public void DoubleEpsilon_ToString ()
		{
			TestRuntime.AssertNotDevice ("Known to fail on devices, see bug #15802");
			var ci = CultureInfo.InvariantCulture;
			// note: unlike Single this works on both my iPhone5S and iPodTouch5
#if NET
			Assert.That (Double.Epsilon.ToString (ci), Is.EqualTo ("5E-324"), "Epsilon.ToString()");
			Assert.That ((-Double.Epsilon).ToString (ci), Is.EqualTo ("-5E-324"), "-Epsilon.ToString()");
#else
			Assert.That (Double.Epsilon.ToString (ci), Is.EqualTo ("4.94065645841247E-324"), "Epsilon.ToString()");
			Assert.That ((-Double.Epsilon).ToString (ci), Is.EqualTo ("-4.94065645841247E-324"), "-Epsilon.ToString()");
#endif
		}

		[Test]
		[Ignore ("Assumption broken with mono 2017-12")]
		public void AssemblyReferences_16213 ()
		{
			foreach (var assembly in typeof (System.Data.AcceptRejectRule).Assembly.GetReferencedAssemblies ()) {
				// Unlike the original bug report the unit tests uses Mono.Data.Tds so it will be part of the .app
				// so we check for System.Transactions which is not used (but referenced by the original System.Data)
				if (assembly.Name == "System.Transactions")
					Assert.Fail ("System.Transactions reference should have removed by the linker");
			}
		}

#if !__WATCHOS__ && !__MACCATALYST__
#if !NET // OpenTK-1.0.dll isn't supported in .NET yet
		[Test]
		public void OpenTk10_Preserved ()
		{
			// that will bring OpenTK-1.0 into the .app
			OpenTK.WindowState state = OpenTK.WindowState.Normal;
			// Compiler optimization (roslyn release) can remove the variable, which removes OpenTK-1.dll from the app and fail the test
			Assert.That (state, Is.EqualTo (OpenTK.WindowState.Normal), "normal");

			var gl = Helper.GetType ("OpenTK.Graphics.ES11.GL, OpenTK-1.0", false);
			Assert.NotNull (gl, "ES11/GL");
			var core = Helper.GetType ("OpenTK.Graphics.ES11.GL/Core, OpenTK-1.0", false);
			Assert.NotNull (core, "ES11/Core");

			gl = Helper.GetType ("OpenTK.Graphics.ES20.GL, OpenTK-1.0", false);
			Assert.NotNull (gl, "ES20/GL");
			core = Helper.GetType ("OpenTK.Graphics.ES20.GL/Core, OpenTK-1.0", false);
			Assert.NotNull (core, "ES20/Core");
		}
#endif // !NET
#endif // !__WATCHOS__ && !__MACCATALYST__

		[Test]
		public void NestedNSObject ()
		{
			// Parent type is not used - but it's not linked out
			var p = Helper.GetType ("LinkAll.Parent");
			Assert.NotNull (p, "Parent");
			// because a nested type is a subclass of NSObject (and not part of monotouch.dll)
			var n = p.GetNestedType ("Derived");
			Assert.NotNull (n, "Derived");
			// however other stuff in Parent, like unused methods, will be removed
			Assert.Null (p.GetMethod ("UnusedMethod"), "unused method");
			// while exported stuff will be present
			Assert.NotNull (n.GetMethod ("Foo"), "unused Export method");
		}

		[Test]
		public void Bug20363 ()
		{
			// testing compile time error
			CancelEventArgs cea = new CancelEventArgs ();
			Assert.NotNull (cea, "CancelEventArgs");
		}

		string GetField (object o, string s)
		{
			var type = o.GetType ();
			var f1 = type.GetField (s, BindingFlags.Instance | BindingFlags.NonPublic);
			var f2 = type.GetField (s + "i__Field", BindingFlags.Instance | BindingFlags.NonPublic);
			if (f1 is null && f2 is null)
				return s;

			//Console.WriteLine (f.GetValue (o));
			return null;
		}

		string FromPattern (string pattern, object o)
		{
			var s = GetField (o, "<action>");
			if (s is not null)
				return s;
			s = GetField (o, "<id>");
			if (s is not null)
				return s;
			return GetField (o, "<contentType>");
		}

		[Test]
		public void AnonymousType ()
		{
			var result = FromPattern ("/{action}/{id}.{contentType}", new {
				action = "foo",
				id = 1234,
				contentType = "xml"
			});
			Assert.Null (result, result);
		}

#if !__WATCHOS__
		[Test]
		public void Events ()
		{
			using (var pr = new SKProductsRequest ()) {
				Assert.Null (pr.WeakDelegate, "none");
				// event on SKProductsRequest itself
				pr.ReceivedResponse += (object sender, SKProductsRequestResponseEventArgs e) => { };

				var t = pr.WeakDelegate.GetType ();
				Assert.That (t.Name, Is.EqualTo ("_SKProductsRequestDelegate"), "delegate");

				var fi = t.GetField ("receivedResponse", BindingFlags.NonPublic | BindingFlags.Instance);
				Assert.NotNull (fi, "receivedResponse");
				var value = fi.GetValue (pr.WeakDelegate);
				Assert.NotNull (value, "value");

				// and on the SKRequest defined one
				pr.RequestFailed += (object sender, SKRequestErrorEventArgs e) => { };
				// and the existing (initial field) is still set
				fi = t.GetField ("receivedResponse", BindingFlags.NonPublic | BindingFlags.Instance);
				Assert.NotNull (fi, "receivedResponse/SKRequest");
			}
		}
#endif // !__WATCHOS__

		[Test]
		public void Aot_27116 ()
		{
			var nix = (from nic in System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces ()
					   where nic.Id.StartsWith ("en") || nic.Id.StartsWith ("pdp_ip")
					   select nic);
			Assert.NotNull (nix);
		}

		[Test]
		public void AppleTls ()
		{
			// make test work for classic (monotouch) and unified (iOS, tvOS and watchOS)
			var fqn = typeof (NSObject).AssemblyQualifiedName.Replace ("Foundation.NSObject", "Security.Tls.AppleTlsProvider");
			Assert.Null (Helper.GetType (fqn), "Should NOT be included (no SslStream or Socket support)");
		}

		[Test]
		// https://bugzilla.xamarin.com/show_bug.cgi?id=59247
		public void WebKit_NSProxy ()
		{
			// this test works only because "Link all" does not use WebKit
			var fqn = typeof (NSObject).AssemblyQualifiedName.Replace ("Foundation.NSObject", "Foundation.NSProxy");
			Assert.Null (Helper.GetType (fqn), fqn);
		}

		static Type type_Task = typeof (Task);

		[Test]
		public void Bug59015 ()
		{
			CheckAsyncTaskMethodBuilder (typeof (AsyncTaskMethodBuilder));
			CheckAsyncTaskMethodBuilder (typeof (AsyncTaskMethodBuilder<int>));
			var snfwc = type_Task.GetMethod ("NotifyDebuggerOfWaitCompletion", BindingFlags.Instance | BindingFlags.NonPublic);
#if DEBUG
			Assert.NotNull (snfwc, "Task.NotifyDebuggerOfWaitCompletion");
#else
			// something keeps it from being removed
			// Assert.Null (snfwc, "Task.NotifyDebuggerOfWaitCompletion");
#endif
		}

		void CheckAsyncTaskMethodBuilder (Type atmb)
		{
			Assert.NotNull (atmb, "AsyncTaskMethodBuilder");
			var snfwc = atmb.GetMethod ("SetNotificationForWaitCompletion", BindingFlags.Instance | BindingFlags.NonPublic);
			var oifd = atmb.GetProperty ("ObjectIdForDebugger", BindingFlags.Instance | BindingFlags.NonPublic);
#if DEBUG
			Assert.NotNull (snfwc, atmb.FullName + ".SetNotificationForWaitCompletion");
			Assert.NotNull (oifd,  atmb.FullName + ".ObjectIdForDebugger");
#else
			Assert.Null (snfwc, atmb.FullName + ".SetNotificationForWaitCompletion");
			Assert.Null (oifd, atmb.FullName + ".ObjectIdForDebugger");
#endif
		}

		[Test]
#if NET
		[Ignore ("BUG https://github.com/xamarin/xamarin-macios/issues/11280")]
#endif
		public void LinkedAwayGenericTypeAsOptionalMemberInProtocol ()
		{
			// https://github.com/xamarin/xamarin-macios/issues/3523
			// This test will fail at build time if it regresses (usually these types of build tests go into monotouch-test, but monotouch-test uses NSSet<T> elsewhere, which this test requires to be linked away).
			Assert.IsNull (typeof (NSObject).Assembly.GetType (NamespacePrefix + "Foundation.NSSet`1"), "NSSet<T> must be linked away, otherwise this test is useless");
		}

		[Protocol (Name = "ProtocolWithGenericsInOptionalMember", WrapperType = typeof (ProtocolWithGenericsInOptionalMemberWrapper))]
		[ProtocolMember (IsRequired = false, IsProperty = false, IsStatic = false, Name = "ConfigureView", Selector = "configureViewForParameters:", ParameterType = new Type [] { typeof (global::Foundation.NSSet<global::Foundation.NSString>) }, ParameterByRef = new bool [] { false })]
		public interface IProtocolWithGenericsInOptionalMember : INativeObject, IDisposable { }

		internal sealed class ProtocolWithGenericsInOptionalMemberWrapper : BaseWrapper, IProtocolWithGenericsInOptionalMember {
			public ProtocolWithGenericsInOptionalMemberWrapper (IntPtr handle, bool owns) : base (handle, owns) { }
		}

		[Test]
		public void NoFatCorlib ()
		{
			var corlib = typeof (int).Assembly.Location;
			// special location when we build a shared (app and extensions) framework for mono
			if (corlib.EndsWith ("/Frameworks/Xamarin.Sdk.framework/MonoBundle/mscorlib.dll", StringComparison.Ordinal))
				Assert.Pass (corlib);

#if __MACCATALYST__ || __MACOS__
			var bundleLocation = Path.Combine ("Contents", "MonoBundle");
#else
			var bundleLocation = string.Empty;
#endif
			var bundlePath = Path.Combine (NSBundle.MainBundle.BundlePath, bundleLocation);
			var isExtension = bundlePath.EndsWith (".appex", StringComparison.Ordinal);
			var bundleName = isExtension ? "link all.appex" : "link all.app";
#if NET
			const string corelib = "System.Private.CoreLib.dll";
#else
			const string corelib = "mscorlib.dll";
#endif
			var suffix = Path.Combine (bundleName, bundleLocation, corelib);
			Assert.That (corlib, Does.EndWith (suffix), corlib);
		}

#if __MACOS__ || __IOS__
		[Test]
		public void CGPdfPage ()
		{
			TestRuntime.AssertXcodeVersion (9, 0);
			var pdfPath = NSBundle.MainBundle.PathForResource ("Tamarin", "pdf");
			using var view = new PdfView ();
			view.Document = new PdfDocument (NSUrl.FromFilename (pdfPath));
			using var page = view.CurrentPage;
			Assert.IsNotNull (page.Page, "Page");
		}
#endif
	}

#if NET
	[SupportedOSPlatform ("macos1.0")]
	[SupportedOSPlatform ("ios1.0")]
	[SupportedOSPlatform ("tvos1.0")]
	[SupportedOSPlatform ("maccatalyst1.0")]
#else
	[Introduced (PlatformName.MacOSX, 1, 0, PlatformArchitecture.Arch64)]
	[Introduced (PlatformName.iOS, 1, 0)]
	[Introduced (PlatformName.TvOS, 1, 0)]
	[Introduced (PlatformName.WatchOS, 1, 0)]
#endif
	[Preserve]
	public class ClassFromThePast : NSObject {
		[Export ("foo:")]
		public void Foo (ClassFromThePast obj)
		{
		}
	}
}
