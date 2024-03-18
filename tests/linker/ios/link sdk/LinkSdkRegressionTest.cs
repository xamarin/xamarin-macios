using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
#if !NET // https://github.com/xamarin/xamarin-macios/issues/11710
using System.Json;
#endif
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
#if !NET
using System.Security.Permissions;
#endif
using System.Security.Principal;
using System.Threading;
using System.Xml;
#if !NET
using Mono.Data.Sqlite;
#endif
using MonoTouch;
#if HAS_ADDRESSBOOK
using AddressBook;
#endif
#if HAS_ADDRESSBOOKUI
using AddressBookUI;
#endif
#if !__WATCHOS__
using CoreAnimation;
#endif
using CoreData;
using CoreFoundation;
using Foundation;
using ObjCRuntime;
#if !__TVOS__
using MapKit;
#endif
#if __MACOS__
using AppKit;
#else
using UIKit;
#endif
#if !__WATCHOS__ && !__MACCATALYST__ && !__MACOS__
using OpenGLES;
#endif
#if !(__TVOS__ && NET)
using WebKit;
#endif
using NUnit.Framework;
using MonoTests.System.Net.Http;
using Xamarin.Utils;


namespace LinkSdk {

#if !NET
	[FileIOPermission (SecurityAction.LinkDemand, AllLocalFiles = FileIOPermissionAccess.AllAccess)]
	public class SecurityDeclarationDecoratedUserCode {

		[FileIOPermission (SecurityAction.Assert, AllLocalFiles = FileIOPermissionAccess.NoAccess)]
		static public bool Check ()
		{
			return true;
		}
	}
#endif

	[TestFixture]
	// we want the test to be availble if we use the linker
	[Preserve (AllMembers = true)]
	public class LinkSdkRegressionTest {

		[Test]
		// https://github.com/xamarin/monotouch/commit/cbefbeaea2eda820dfc7214e976edc83a55df38e
		public void MonoAssembly_LinkedOut ()
		{
			Assembly a = Assembly.GetExecutingAssembly ();
			Assert.That (a.GetType ().Name, Is.EqualTo ("RuntimeAssembly"), "RuntimeAssembly");
		}

		[Test]
		// http://bugzilla.xamarin.com/show_bug.cgi?id=205
		// https://bugzilla.novell.com/show_bug.cgi?id=688414
		public void Bug205_ExposingIEnumerable ()
		{
			var ds = new DataContractSerializer (typeof (IEnumerable<int>));
			using (var xw = XmlWriter.Create (System.IO.Stream.Null))
				ds.WriteObject (xw, new int [] { 1, 2, 3 });
			// the above should not throw System.Runtime.Serialization.SerializationException
		}

#if !NET // This test requires Mono.Data.SqliteConnection, which .NET 5+ doesn't have
		[Test]
		// http://bugzilla.xamarin.com/show_bug.cgi?id=233
		public void Bug233_MonoPInvokeCallback ()
		{
			var c = new SqliteConnection ("Data Source=:memory:");
			c.Open ();
			c.Update += (sender, e) => { };
			// the above should not crash
			c.Close ();
		}
#endif

		[Test]
		// http://bugzilla.xamarin.com/show_bug.cgi?id=234
		public void Bug234_Interlocked ()
		{
			string str = null;
			Assert.Null (Interlocked.Exchange (ref str, "one"), "Exchange");
			// the above should not crash with System.ExecutionEngineException
			Assert.That (str, Is.EqualTo ("one"), "one");
		}

		[Test]
		// http://bugzilla.xamarin.com/show_bug.cgi?id=300
		// http://stackoverflow.com/questions/6517736/monotouch-crash-dictionary-firstordefault-type-initializer-predicateof
		public void Bug300_Linker_PredicateOf ()
		{
			Dictionary<string, DateTime> queued = new Dictionary<string, DateTime> ();
			KeyValuePair<string, DateTime> valuePair = queued.FirstOrDefault ();
			// above should not crash with System.ExecutionEngineException
			Assert.NotNull (valuePair);
		}

#if !__WATCHOS__
		[Test]
		// http://bugzilla.xamarin.com/show_bug.cgi?id=328
		public void Bug328_CompletionBlock ()
		{
			CATransaction.Begin ();
			CATransaction.CompletionBlock = delegate { };
			// the above should not crash with a MonoTouchException
			CATransaction.Commit ();
		}
#endif // !__WATCHOS__

#if !__TVOS__ && !__WATCHOS__
		[Test]
		// http://bugzilla.xamarin.com/show_bug.cgi?id=769
		public void Bug769_UnregistredDelegate ()
		{
			var tmp = Class.ThrowOnInitFailure;
			Class.ThrowOnInitFailure = false;
			try {
				Assert.NotNull (new MKMapViewDelegate ());
				// the above should not throw an Exception
			} finally {
				Class.ThrowOnInitFailure = tmp;
			}
		}
#endif // !__TVOS__ && !__WATCHOS__

#if !__WATCHOS__ && !__MACOS__
		[Test]
		// http://bugzilla.xamarin.com/show_bug.cgi?id=865
		public void Bug865_CanOpenUrl ()
		{
			if (TestRuntime.CheckXcodeVersion (15, 0))
				Assert.Ignore ("NSUrl was fixed with Xcode 15.0");

			Assert.False (UIApplication.SharedApplication.CanOpenUrl (null), "null");
			// the above should not throw an ArgumentNullException
			// and that's important because NSUrl.FromString and NSUrl.ctor(string) differs
			const string bad_tel = "tel://1800 023 009";
			Assert.Null (NSUrl.FromString (bad_tel), "bad url");
			// we now throw if `init*` fails
			Assert.Throws<Exception> (() => new NSUrl (bad_tel), "ctor, bad url");
		}
#endif // !__WATCHOS__

		[Test]
		// issue indirectly found when trying:  http://bugzilla.xamarin.com/show_bug.cgi?id=928
		// similar to MonoAssembly_LinkedOut
		// https://github.com/xamarin/monotouch/commit/409316f87f23723a384cb072163abd03ae7e6045
		public void Bug928_MonoModule_LinkedOut ()
		{
			Module m = Assembly.GetExecutingAssembly ().ManifestModule;
			Assert.That (m.GetType ().Name, Is.EqualTo ("RuntimeModule"), "RuntimeModule");
		}

#if HAS_ADDRESSBOOK && HAS_ADDRESSBOOKUI
		[Test]
		// http://bugzilla.xamarin.com/show_bug.cgi?id=980
		public void Bug980_AddressBook_NRE ()
		{
			TestRuntime.AssertSystemVersion (ApplePlatform.MacCatalyst, 14, 0, throwIfOtherPlatform: false); // The AddressBook framework was introduced in Mac Catalyst 14.0
			using (ABPeoplePickerNavigationController picker = new ABPeoplePickerNavigationController ()) {
				// no NRE should occur
				if (UIDevice.CurrentDevice.CheckSystemVersion (8, 0))
					Assert.Null (picker.AddressBook);
				else
					Assert.NotNull (picker.AddressBook);
			}
		}

		[Test]
		public void AddressBook_Constants ()
		{
#if !__MACOS__
			// we want to ensure we can get the constants without authorization (on iOS 6.0+) so this application
			// needs to be unauthorized (in settings.app). Note: authorization checks only occurs on devices
			if (TestRuntime.IsDevice && UIDevice.CurrentDevice.CheckSystemVersion (6,0)) {
				Assert.That (ABAddressBook.GetAuthorizationStatus (), Is.Not.EqualTo (ABAuthorizationStatus.Authorized),
					"Please deny access to contacts for this this application (it's important for this test)");
			}
#endif // !__MACOS__
			TestRuntime.AssertSystemVersion (ApplePlatform.MacCatalyst, 14, 0, throwIfOtherPlatform: false); // The AddressBook framework was introduced in Mac Catalyst 14.0
			Assert.IsNotNull (ABPersonAddressKey.City, "ABPersonAddressKey");
		}
#endif // HAS_ADDRESSBOOKUI

#if !__MACOS__
		[Test]
		// http://bugzilla.xamarin.com/show_bug.cgi?id=1387
		public void Bug1387_UIEdgeInsets_ToString ()
		{
			var insets = new UIEdgeInsets (1, 2, 3, 4);
			Assert.False (insets.ToString ().Contains ("UIEdgeInsets"));
		}
#endif // !__MACOS__

		void CheckExceptionDetailProperty (PropertyInfo pi)
		{
			bool data_member = true;
			foreach (var ca in pi.GetCustomAttributes (false)) {
				if (ca is DataMemberAttribute) {
					data_member = true;
					break;
				}
			}
			// to be valid both getter and setter must be present if [DataMember]
			if (data_member) {
				Assert.NotNull (pi.GetGetMethod (true), "get_" + pi.Name);
				Assert.NotNull (pi.GetSetMethod (true), "set_" + pi.Name);
			} else {
				// check well-known [DataMember]
				switch (pi.Name) {
				case "HelpLink":
				case "InnerException":
				case "Message":
				case "StackTrace":
				case "Type":
					Assert.Fail ("{0} is missing it's [DataMember] attribute", pi.Name);
					break;
				}
			}
		}

#if !NET // This test requires System.ServiceModel.dll, which .NET 5+ doesn't have
		[Test]
		// http://bugzilla.xamarin.com/show_bug.cgi?id=1415
		public void Bug1415_Linker_DataMember ()
		{
			// the typeof ensure we're can't (totally) link away System.ServiceModel.dll
			Type ed = typeof (System.ServiceModel.AuditLevel).Assembly.GetType ("System.ServiceModel.ExceptionDetail", false);
			// which means it's [DataContract] / [DataMember] should not be linked out
			// even if we're not specifically using them (and without [Preserve] being added)
			// which is important since System.ServiceModel.dll is an SDK assembly
			Assert.NotNull (ed, "ExceptionDetail");
			bool help_link = false;
			bool inner_exception = false;
			bool message = false;
			bool stack_trace = false;
			bool type = false;
			foreach (var pi in ed.GetProperties ()) {
				CheckExceptionDetailProperty (pi);
				switch (pi.Name) {
				case "HelpLink":
					help_link = true;
					break;
				case "InnerException":
					inner_exception = true;
					break;
				case "Message":
					message = true;
					break;
				case "StackTrace":
					stack_trace = true;
					break;
				case "Type":
					type = true;
					break;
				}
			}
			// ensure all properties are still present
			Assert.True (help_link, "HelpLink");
			Assert.True (inner_exception, "InnerException");
			Assert.True (message, "Message");
			Assert.True (stack_trace, "StackTrace");
			Assert.True (type, "Type");
		}
#endif // !NET

#if !NET // This test requires System.ServiceModel.dll, which .NET 5+ doesn't have
		[Test]
		// http://bugzilla.xamarin.com/show_bug.cgi?id=1415
		// not really part of the bug - but part of the same fix
		public void Bug1415_Linker_XmlAttribute ()
		{
			// the typeof ensure we're can't (totally) link away System.ServiceModel.dll
			Type ed = typeof (System.ServiceModel.AuditLevel).Assembly.GetType ("System.ServiceModel.EndpointAddress10", false);
			// type is decorated with both [XmlSchemaProvider] and [XmlRoot]
			Assert.NotNull (ed, "EndpointAddress10");

#if !__WATCHOS__ // FIXME: this needs to use a different type than OpenTK.Quaternion, so that the test can run on WatchOS as wells
			var q = new OpenTK.Quaternion ();
			Assert.Null (q.GetType ().GetProperty ("XYZ"), "XmlIgnore");
			// should be null if application is linked (won't be if "Don't link" is used)
#endif // !__WATCHOS__
		}
#endif // !NET

		[Test]
		// http://bugzilla.xamarin.com/show_bug.cgi?id=1443
		public void Bug1443_Linq_Aot ()
		{
			List<string> list = new List<string> ();
			Dictionary<string, Type> dict = new Dictionary<string, Type> ();
			list.AddRange (from kv in dict orderby kv.Key select kv.Value.ToString ());
			// should not throw an ExecutionEngineException on devices
		}

#if !__TVOS__ && !__WATCHOS__ && !__MACOS__
		[Test]
		// http://bugzilla.xamarin.com/show_bug.cgi?id=1516
		public void Bug1516_Appearance_Linker ()
		{
			// iOS7, since DP3, validates ?some? appearance properties, e.g.
			// Objective-C exception thrown.  Name: NSInvalidArgumentException Reason: -setTintColor: is not allowed for use with the appearance proxy. Perhaps you want to use the barTintColor property.
			// UINavigationBar.Appearance.TintColor = UIColor.FromRGB (238,234,222);
			UINavigationBar.Appearance.SetTitleTextAttributes (new UITextAttributes () {
				TextColor = UIColor.FromRGB (85, 108, 17),
				TextShadowColor = UIColor.Clear
			});
			// should not throw if the application is linked
		}
#endif

		[Test]
		// http://bugzilla.xamarin.com/show_bug.cgi?id=1790
		public void Bug1790_TimeZoneInfo_Local ()
		{
			// the simulator has complete file access but the device won't have - i.e. we can't depend on it
			var hasFileAccess = TestRuntime.IsSimulatorOrDesktop;
			Assert.That (File.Exists ("/etc/localtime"), Is.EqualTo (hasFileAccess), "/etc/localtime");
			Assert.NotNull (TimeZoneInfo.Local, "Local");
			// should not throw a TimeZoneNotFoundException on devices
		}

		[Test]
		// http://bugzilla.xamarin.com/show_bug.cgi?id=1922
		public void Bug1922_PLinq_Aot ()
		{
			const int size = 100;
			Random random = new Random ();
			int [] values = new int [size];
			int currentNumber = 0;
			List<int> numbers = new List<int> ();
			while (currentNumber < 100) {
				values [currentNumber] = random.Next (1, 10);
				numbers.Add (currentNumber);
				currentNumber++;
			}
			numbers.AsParallel ().ForAll (number => {
				Thread.Sleep (values [number]);
				//Console.WriteLine (number);
			});
		}

		[Test]
		public void Bug2000_NSPersistentStoreCoordinator ()
		{
			// from http://www.sgmunn.com/?p=1#comments
			NSAttributeDescription description = new NSAttributeDescription ();
			Assert.That (description.Handle, Is.Not.EqualTo (IntPtr.Zero), "NSAttributeDescription");
			description.AttributeType = NSAttributeType.Integer32;
			description.Name = "SomeId";
			description.Optional = false;

			NSEntityDescription entity = new NSEntityDescription ();
			Assert.That (entity.Handle, Is.Not.EqualTo (IntPtr.Zero), "NSEntityDescription");
			entity.Name = "TestEntity";
			entity.Properties = new NSPropertyDescription [1] { description };

			NSManagedObjectModel model = new NSManagedObjectModel ();
			Assert.That (model.Handle, Is.Not.EqualTo (IntPtr.Zero), "NSManagedObjectModel");
			model.Entities = new NSEntityDescription [1] { entity };
			model.SetEntities (model.Entities, String.Empty);

			var sqlitePath = Path.Combine (NSFileManager.TemporaryDirectory, $"test-{System.Diagnostics.Process.GetCurrentProcess ().Id}.sqlite");
			NSUrl url = NSUrl.FromFilename (sqlitePath);

			try {
				// from http://bugzilla.xamarin.com/show_bug.cgi?id=2000
				NSError error;
				var c = new NSPersistentStoreCoordinator (model);
#if NET
				c.AddPersistentStore (NSPersistentStoreCoordinator.SQLiteStoreType, null, url, null, out error);
#else
				c.AddPersistentStoreWithType (NSPersistentStoreCoordinator.SQLiteStoreType, null, url, null, out error);
#endif
				Assert.IsNull (error, "error");
			} finally {
				File.Delete (sqlitePath);
			}
		}

		[Test]
		// http://lists.dot.net/pipermail/monotouch/2011-December/006976.html
		public void Linker_RuntimeWrappedException ()
		{
			try {
				// can't throw anything else but Exception-derived types from C#
				support.throw_object ();
			} catch (Exception e) {
				Assert.That (e.GetType ().Name, Is.EqualTo ("RuntimeWrappedException"), "RuntimeWrappedException");
			}
		}

#if !NET // This test requires Mono.Data.SqliteConnection, which .NET 5+ doesn't have
		[Test]
		// http://stackoverflow.com/questions/8602726/cant-open-sqlite-database-in-read-only-mode
		public void Sqlite_ReadOnly ()
		{
			var c = new SqliteConnection ("Data Source=:memory:;Read Only=true");
			c.Open ();
			// the above should not throw a 'misuse' exception
			c.Close ();
		}
#endif

		[Test]
		public void AsQueryable_3028 ()
		{
			string [] foos = new string [] { "hi", "bye" };
			string f = foos.AsQueryable ().First ();
			Assert.That (f, Is.EqualTo ("hi"), "f");
		}

#if !__WATCHOS__ && !__MACCATALYST__
#if !NET // OpenTK-1.0.dll is not supported yet
		[Test]
		public void OpenTk_3049 ()
		{
			using (var gc1 = OpenTK.Platform.Utilities.CreateGraphicsContext (EAGLRenderingAPI.OpenGLES1)) {
				Assert.NotNull (gc1);
			}
			using (var gc2 = OpenTK.Platform.Utilities.CreateGraphicsContext (EAGLRenderingAPI.OpenGLES2)) {
				Assert.NotNull (gc2);
			}
		}

		[Test]
		public void OpenTk_Preserved ()
		{
			const string OpenTKAssembly = "OpenTK-1.0";
			var gl = GetTypeHelper ("OpenTK.Graphics.ES11.GL, " + OpenTKAssembly, false);
			Assert.NotNull (gl, "ES11/GL");
			var core = GetTypeHelper ("OpenTK.Graphics.ES11.GL/Core, " + OpenTKAssembly, false);
			Assert.NotNull (core, "ES11/Core");

			gl = GetTypeHelper ("OpenTK.Graphics.ES20.GL, " + OpenTKAssembly, false);
			Assert.NotNull (gl, "ES20/GL");
			core = GetTypeHelper ("OpenTK.Graphics.ES20.GL/Core, " + OpenTKAssembly, false);
			Assert.NotNull (core, "ES20/Core");
		}
#endif // !NET
#endif // !__WATCHOS__ && !__MACCATALYST__

		[Test]
		public void XElement_3137 ()
		{
			CultureInfo current = Thread.CurrentThread.CurrentCulture;
			try {
				Thread.CurrentThread.CurrentCulture = new CultureInfo ("en-US");
				var element1 = new System.Xml.Linq.XElement ("Property1", new System.Xml.Linq.XAttribute ("type", "number"), 1.2343445);
				Assert.That (element1.ToString (), Is.EqualTo ("<Property1 type=\"number\">1.2343445</Property1>"), "en-US");

				Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo ("de-DE");
				// this was already working because the element was created with en-US
				Assert.That (element1.ToString (), Is.EqualTo ("<Property1 type=\"number\">1.2343445</Property1>"), "de-DE/1");
				// however creating a new, identical, element under de-DE return*ed* a different string
				var element2 = new System.Xml.Linq.XElement ("Property1", new System.Xml.Linq.XAttribute ("type", "number"), 1.2343445);
				Assert.That (element2.ToString (), Is.EqualTo ("<Property1 type=\"number\">1.2343445</Property1>"), "de-DE/2");
			} finally {
				Thread.CurrentThread.CurrentCulture = current;
			}
		}

#if !__TVOS__ && !__WATCHOS__ && !__MACOS__
		[Test]
		public void Modal_3489 ()
		{
			using (UIViewController a = new UIViewController ())
			using (UIViewController b = new UIViewController ())
			using (UIViewController c = new UIViewController ()) {
				a.PresentModalViewController (b, true);
				b.PresentModalViewController (c, true);

				b.DismissModalViewController (true);
				a.DismissModalViewController (true); //error
			}
		}
#endif // !__TVOS__ && !__WATCHOS__

		[Test]
		public void Parse_3677 ()
		{
			Assert.That (sbyte.Parse ("E3", NumberStyles.HexNumber), Is.EqualTo (-29), "SByte");
			Assert.That (short.Parse ("E3E3", NumberStyles.HexNumber), Is.EqualTo (-7197), "Int16");
		}

		public class DeviceHardware {
			public const string HardwareProperty = "hw.machine";

			public enum HardwareVersion {
				iPhone,
				iPhone3G,
				iPhone3GS,
				iPhone4,
				iPod1G,
				iPod2G,
				iPod3G,
				iPod4G,
				iPad,
				iPhoneSimulator,
				iPhone4Simulator,
				iPadSimulator,
				Unknown
			}

			// Changing the constant to "/usr/bin/libSystem.dylib" allows this P/Invoke to work on Mac OS X
			// Using "hw.model" as property gives Macintosh model, "hw.machine" kernel arch (ppc, ppc64, i386, x86_64)
			[DllImport (Constants.libSystemLibrary)]
			internal static extern int sysctlbyname ([MarshalAs (UnmanagedType.LPStr)] string property, // name of the property
				IntPtr output, // output
				IntPtr oldLen, // IntPtr.Zero
				IntPtr newp, // IntPtr.Zero
				uint newlen // 0
				);

			public static HardwareVersion Version {
				get {
					// get the length of the string that will be returned
					var pLen = Marshal.AllocHGlobal (sizeof (int));
					sysctlbyname (DeviceHardware.HardwareProperty, IntPtr.Zero, pLen, IntPtr.Zero, 0);

					var length = Marshal.ReadInt32 (pLen);

					// check to see if we got a length
					if (length == 0) {
						Marshal.FreeHGlobal (pLen);
						return HardwareVersion.Unknown;
					}

					// get the hardware string
					var pStr = Marshal.AllocHGlobal (length);
					sysctlbyname (DeviceHardware.HardwareProperty, pStr, pLen, IntPtr.Zero, 0);

					// convert the native string into a C# string
					var hardwareStr = Marshal.PtrToStringAnsi (pStr);
					var ret = HardwareVersion.Unknown;

					// determine which hardware we are running
					if (hardwareStr == "iPhone1,1")
						ret = HardwareVersion.iPhone;
					else if (hardwareStr == "iPhone1,2")
						ret = HardwareVersion.iPhone3G;
					else if (hardwareStr == "iPhone2,1")
						ret = HardwareVersion.iPhone3GS;
					else if (hardwareStr == "iPhone3,1")
						ret = HardwareVersion.iPhone4;
					else if (hardwareStr == "iPad1,1")
						ret = HardwareVersion.iPad;
					else if (hardwareStr == "iPod1,1")
						ret = HardwareVersion.iPod1G;
					else if (hardwareStr == "iPod2,1")
						ret = HardwareVersion.iPod2G;
					else if (hardwareStr == "iPod3,1")
						ret = HardwareVersion.iPod3G;
					else if (hardwareStr == "iPod4,1")
						ret = HardwareVersion.iPod3G;
					else if (hardwareStr == "i386" || hardwareStr == "x86_64") {
#if __WATCHOS__ || __MACOS__
						ret = HardwareVersion.Unknown;
#else
						if (UIDevice.CurrentDevice.Model.Contains ("iPhone"))
							ret = UIScreen.MainScreen.Bounds.Height * UIScreen.MainScreen.Scale == 960 || UIScreen.MainScreen.Bounds.Width * UIScreen.MainScreen.Scale == 960 ? HardwareVersion.iPhone4Simulator : HardwareVersion.iPhoneSimulator;
						else
							ret = HardwareVersion.iPadSimulator;
#endif
					} else ret = HardwareVersion.Unknown;

					// cleanup
					Marshal.FreeHGlobal (pLen);
					Marshal.FreeHGlobal (pStr);

					return ret;
				}
			}
		}

		[Test]
		// http://stackoverflow.com/questions/9685134/monotouch-fatal-crash-once-deployed
		// could not be duplicated on iPad2 (rolf), iPad1 (spouliot), iPodTouch4 (spouliot)
		public void Hardware_SO ()
		{
			Assert.NotNull (DeviceHardware.Version, "Hardware");
		}

		public class Location { }

		private static Location mInstance = null;

		[MethodImpl (MethodImplOptions.Synchronized)]
		public static Location getInstance ()
		{
			if (mInstance is null)
				mInstance = new Location ();
			return mInstance;
		}

		[Test]
		public void Synchronized_3904 ()
		{
			// crash with LLVM
			Assert.NotNull (getInstance (), "Location");
		}

#if !NET // https://github.com/xamarin/xamarin-macios/issues/11710
		[Test]
		[Culture ("en")]
		public void Json_Parse_4415 ()
		{
			var f = 4.25987E-06f;
			// note: always use '.' see http://www.json.org/fatfree.html
			var s = f.ToString ();
			var v = JsonValue.Parse (s);
			Assert.AreEqual (f, (float) v, "Parse Negative Exponent");
			f *= 10;
			Assert.AreNotEqual (f, (float) v, "non-equal");
		}
#endif // !NET

		[Test]
		[Culture ("en")]
		public void ConvertToDouble_4620 ()
		{
			// can't duplicate bug when using invarient culture
			Assert.That (Convert.ToDouble ("0.0"), Is.EqualTo (0.0d));
		}

		[Test]
		public void NetworkInterface_4631 ()
		{
			Assert.NotNull (NetworkInterface.GetAllNetworkInterfaces ());
		}

		[Test]
		public void WebClient_SSL_Leak ()
		{
#if __WATCHOS__
			Assert.Ignore ("WatchOS doesn't support BSD sockets, which our network stack currently requires.");
#endif
			var exceptions = new List<string> ();
			WebClient wc = new WebClient ();
			foreach (var url in NetworkResources.HttpsUrls) {
				try {
					// note: needs to be executed under Instrument to verify it does not leak
					string s = wc.DownloadString (url);
					Assert.NotNull (s);
					return; // one url succeeded, that's enough
				} catch (Exception e) {
					var msg = $"Url '{url}' failed: {e.ToString ()}";
					Console.WriteLine (msg); // If this keeps occurring locally for the same url, we might have to take it off the list of urls to test.
					exceptions.Add (msg);
				}
			}
			Assert.That (exceptions, Is.Empty, "At least one url should work");
		}

#if !__TVOS__ && !__WATCHOS__ && !__MACOS__
		[Test]
		public void WebProxy_Leak ()
		{
			// note: needs to be executed under Instrument to verify it does not leak
#if NET
			Assert.NotNull (global::CoreFoundation.CFNetwork.GetSystemProxySettings (), "should not leak");
#else
			Assert.NotNull (CFNetwork.GetSystemProxySettings (), "should not leak");
#endif
		}
#endif // !__TVOS__ && !__WATCHOS__

		[Test]
		// https://bugzilla.novell.com/show_bug.cgi?id=650402
#if __WATCHOS__
		// Fails with:
		//     System.ExecutionEngineException : Attempting to JIT compile method 'System.Data.DataColumn:set_Expression (string)' while running in aot-only mode.
		// because DataColumn.set_Expression uses filter clauses, which we don't support with bitcode:
		//     LLVM failed for 'DataColumn.set_Expression': non-finally/catch/fault clause.
		[Ignore ("https://bugzilla.xamarin.com/show_bug.cgi?id=59987")]
#endif
		public void ForeignKey_650402 ()
		{
			DataSet data = new DataSet ();
			DataTable parent = new DataTable ("parent");
			DataColumn pk = parent.Columns.Add ("PK");
			DataTable child = new DataTable ("child");
			DataColumn fk = child.Columns.Add ("FK");

			data.Tables.Add (parent);
			data.Tables.Add (child);
			data.Relations.Add (pk, fk);

			parent.Rows.Add ("value");
			child.Rows.Add ("value");
			data.AcceptChanges ();
			child.Rows [0].Delete ();
			parent.Rows [0] [0] = "value2";

			data.EnforceConstraints = false;
			data.EnforceConstraints = true;
		}

		[Test]
		public void Pointer_5200 ()
		{
			// ensure the linker did not remove the type, which is used by the runtime
			Assert.NotNull (GetTypeHelper ("System.Reflection.Pointer, " + typeof (int).Assembly.GetName ().Name));
		}

		[Test]
		public void LockRecursionException_5311 ()
		{
			Assert.Throws<LockRecursionException> (delegate { throw new LockRecursionException (); });
		}

		class AddedInSilverlight5 : INotifyPropertyChanging {
			public event PropertyChangingEventHandler PropertyChanging;
		}

		[Test]
		public void INotifyPropertyChanging_5337 ()
		{
			new AddedInSilverlight5 ().PropertyChanging += delegate { };
		}

		[Test]
		public void MonoIOStat_6118 ()
		{
			string file = NSBundle.MainBundle.ExecutablePath;
			DateTime c1 = File.GetCreationTime (file).ToUniversalTime ();
			DateTime c2 = (DateTime) NSFileManager.DefaultManager.GetAttributes (file).CreationDate;
			Assert.That ((c1 - c2).Seconds, Is.LessThan (30), "MonoIOStat");
		}

		[Test]
		public void ObjectHandleCtor ()
		{
			Type o = typeof (Object);
			// this returns a new System.Runtime.Remoting.ObjectHandle which (was) linked away previously 
			Assert.NotNull (Activator.CreateInstance (o.Assembly.GetName ().Name, o.FullName), "ObjectHandle");
		}

		[Test]
		public void AttributeUsageAttribute_Persistance ()
		{
			Assert.IsFalse (Attribute.IsDefined (GetType (), typeof (SerializableAttribute)));
		}

#if !NET // This test requires System.Runtime.Remoting.dll, which .NET 5+ doesn't have
		[Test]
		public void LinkedAway ()
		{
			Assert.Throws<NotSupportedException> (() => new System.Runtime.Remoting.RemotingException ());
		}
#endif // !NET

		[Test]
		public void ArrayClear_11184 ()
		{
			byte [] key = new byte [16];
			for (int i = 0; i < key.Length; i++)
				key [i] = (byte) (255 - i);
			Array.Clear (key, 5, 11);
			for (int i = 5; i < key.Length; i++)
				Assert.That (key [i], Is.EqualTo (0), i.ToString ());
		}

		public class Demo_14493 {
			public void Update (object o)
			{
			}
		}

		[Test]
		public void Action_14493 ()
		{
			var Demo = new Demo_14493 ();
			Action<object> a = null;
			a += Demo.Update;
			a -= Demo.Update; // Crash here
		}

		enum AnEnum {
			First = 1,
			Second = 2
		};

		[Test]
		public void AotGcMemmove_Crash_17284 ()
		{
			var arr = new AnEnum [16];
			var c = new ReadOnlyCollection<AnEnum> (arr);
			Assert.False (c.Contains (AnEnum.First));
		}

		enum MyEnum {
			AValue,
			AnotherValue
		}

		[Test]
		public void Aot_Gsharedvt_21893 ()
		{
			IList<MyEnum> _myValues = new List<MyEnum> { MyEnum.AValue };
			bool pleaseDontCrash = _myValues.Contains (MyEnum.AnotherValue);
			Assert.False (pleaseDontCrash);
		}

		[Test]
		public void PrivateMemorySize64 ()
		{
			// ref: https://bugzilla.xamarin.com/show_bug.cgi?id=21882
#if NET
#if __MACOS__ || __MACCATALYST__
			var mem = System.Diagnostics.Process.GetCurrentProcess ().PrivateMemorySize64;
			Assert.That (mem, Is.EqualTo (0), "PrivateMemorySize64");
#else
			// It's not entirely clear, but it appears this is not implemented, and won't be, for mobile platforms: https://github.com/dotnet/runtime/issues/28990
			Assert.Throws<PlatformNotSupportedException> (() => { var mem = System.Diagnostics.Process.GetCurrentProcess ().PrivateMemorySize64; }, "PrivateMemorySize64");
#endif // __MACOS__ || __MACCATALYST__
#else
			var mem = System.Diagnostics.Process.GetCurrentProcess ().PrivateMemorySize64;
			// the above used a mach call that iOS samdbox did *not* allow (sandbox) on device
			// but has been fixed (different call) for the same PID
			Assert.That (mem, Is.Not.EqualTo (0), "PrivateMemorySize64");
#endif
		}

		string TestFolder (Environment.SpecialFolder folder, bool supported = true, bool? exists = true, bool readOnly = false)
		{
			var path = Environment.GetFolderPath (folder);
			Assert.That (path.Length > 0, Is.EqualTo (supported), $"SpecialFolder: {folder.ToString ()} Path: {path} Supported: {supported}");
			if (!supported)
				return path;

			var dirExists = Directory.Exists (path);
			if (exists.HasValue)
				Assert.That (dirExists, Is.EqualTo (exists), path);
			if (!dirExists)
				return path;

			string file = Path.Combine (path, "temp.txt");
			try {
				File.WriteAllText (file, "mine");
				Assert.False (readOnly, "!readOnly " + folder);
			} catch {
				Assert.True (readOnly, "readOnly " + folder);
			} finally {
				File.Delete (file);
			}
			return path;
		}

		[Test]
		public void SpecialFolder ()
		{
			try {
				SpecialFolderImpl ();
			} catch (Exception e) {
#if NET
				Console.WriteLine ($"An exception occurred in this test: {e}");
				Console.WriteLine ($"Dumping info about various directories:");
				foreach (var value in Enum.GetValues<NSSearchPathDirectory> ().OrderBy (v => v.ToString ())) {
					var urls = NSFileManager.DefaultManager.GetUrls (value, NSSearchPathDomain.User);
					Console.WriteLine ($"NSFileManager.GetUrls ({value} = {(int) value}) returned {urls.Length} results:");
					foreach (var url in urls)
						Console.WriteLine ($"    {url.Path}");
				}

				foreach (var value in Enum.GetValues<Environment.SpecialFolder> ().OrderBy (v => v.ToString ()))
					Console.WriteLine ($"SpecialFolder '{value}' => {Environment.GetFolderPath (value)}");
#endif

				// Throw the original exception so that the test actually fails.
				throw;
			}
		}

		void SpecialFolderImpl ()
		{
			// iOS8 changes the rules of the game
			var fm = NSFileManager.DefaultManager;
			var docs = fm.GetUrls (NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.User) [0].Path;
			var libs = fm.GetUrls (NSSearchPathDirectory.LibraryDirectory, NSSearchPathDomain.User) [0].Path;
#if __MACOS__
			var home = Environment.GetEnvironmentVariable ("HOME");
#endif

			// note: this test is more interesting on devices because of the sandbox they have
			var device = TestRuntime.IsDevice;

			// some stuff we do not support (return String.Empty for the path)
			TestFolder (Environment.SpecialFolder.Programs, supported: false);
			TestFolder (Environment.SpecialFolder.Startup, supported: false);
			TestFolder (Environment.SpecialFolder.Recent, supported: false);
			TestFolder (Environment.SpecialFolder.SendTo, supported: false);
			TestFolder (Environment.SpecialFolder.StartMenu, supported: false);
			TestFolder (Environment.SpecialFolder.MyComputer, supported: false);
			TestFolder (Environment.SpecialFolder.NetworkShortcuts, supported: false);
			TestFolder (Environment.SpecialFolder.CommonStartMenu, supported: false);
			TestFolder (Environment.SpecialFolder.CommonPrograms, supported: false);
			TestFolder (Environment.SpecialFolder.CommonStartup, supported: false);
			TestFolder (Environment.SpecialFolder.CommonDesktopDirectory, supported: false);
			TestFolder (Environment.SpecialFolder.PrinterShortcuts, supported: false);
			TestFolder (Environment.SpecialFolder.Cookies, supported: false);
			TestFolder (Environment.SpecialFolder.History, supported: false);
			TestFolder (Environment.SpecialFolder.Windows, supported: false);
#if __MACOS__
			TestFolder (Environment.SpecialFolder.System, supported: true, readOnly: true);
#else
			TestFolder (Environment.SpecialFolder.System, supported: false);
#endif
			TestFolder (Environment.SpecialFolder.SystemX86, supported: false);
			TestFolder (Environment.SpecialFolder.ProgramFilesX86, supported: false);
			TestFolder (Environment.SpecialFolder.CommonProgramFiles, supported: false);
			TestFolder (Environment.SpecialFolder.CommonProgramFilesX86, supported: false);
			TestFolder (Environment.SpecialFolder.CommonDocuments, supported: false);
			TestFolder (Environment.SpecialFolder.CommonAdminTools, supported: false);
			TestFolder (Environment.SpecialFolder.AdminTools, supported: false);
			TestFolder (Environment.SpecialFolder.CommonMusic, supported: false);
			TestFolder (Environment.SpecialFolder.CommonPictures, supported: false);
			TestFolder (Environment.SpecialFolder.CommonVideos, supported: false);
			TestFolder (Environment.SpecialFolder.LocalizedResources, supported: false);
			TestFolder (Environment.SpecialFolder.CommonOemLinks, supported: false);
			TestFolder (Environment.SpecialFolder.CDBurning, supported: false);

			// some stuff we return a value - but the directory does not exists 

#if __MACOS__
			var path = TestFolder (Environment.SpecialFolder.Desktop, exists: true);
#else
			var path = TestFolder (Environment.SpecialFolder.Desktop, exists: false);
#endif

#if __MACOS__ || __MACCATALYST__
			// The behavior for the Favorites folder changes betwee macOS versions, and it's quite complicated
			// to get it right, so just skip any checks for this particular folder.
#else
			path = TestFolder (Environment.SpecialFolder.Favorites, exists: false);
#endif

#if __MACOS__
			var myExists = true;
#else
			var myExists = false;
#endif
			path = TestFolder (Environment.SpecialFolder.MyMusic, exists: myExists);

#if __MACOS__ && !NET8_0_OR_GREATER
			path = TestFolder (Environment.SpecialFolder.MyVideos, supported: false);
#else
			path = TestFolder (Environment.SpecialFolder.MyVideos, exists: myExists);
#endif

			path = TestFolder (Environment.SpecialFolder.DesktopDirectory, exists: myExists);

#if __TVOS__
			path = TestFolder (Environment.SpecialFolder.Fonts, exists: null, supported: true);
#elif __MACOS__ || __MACCATALYST__
			// See comment about the Favorites folder, it applies to the Fonts folder as well.
#else
			path = TestFolder (Environment.SpecialFolder.Fonts, exists: myExists);
#endif

#if __MACOS__
			path = TestFolder (Environment.SpecialFolder.Templates, supported: false);
#elif __TVOS__
			path = TestFolder (Environment.SpecialFolder.Templates, exists: null, supported: true);
#else
			path = TestFolder (Environment.SpecialFolder.Templates, exists: false);
#endif

			path = TestFolder (Environment.SpecialFolder.MyPictures, exists: myExists);

#if __MACOS__
			path = TestFolder (Environment.SpecialFolder.CommonTemplates, supported: false);
#else
			path = TestFolder (Environment.SpecialFolder.CommonTemplates, exists: false);
#endif

			// some stuff we return and are usable either as read-only
			path = TestFolder (Environment.SpecialFolder.CommonApplicationData, readOnly: true);
			Assert.That (path, Is.EqualTo ("/usr/share"), "path - CommonApplicationData");

			// and the simulator is more lax
#if NET
			path = TestFolder (Environment.SpecialFolder.ProgramFiles, readOnly: device, exists: null /* may or may not exist */);
#if __MACOS__
			var applicationsPath = "/Applications";
#else
			// ProgramFiles is different on .NET: https://github.com/dotnet/runtime/pull/41959#discussion_r485069017
			var applicationsPath = NSSearchPath.GetDirectories (NSSearchPathDirectory.ApplicationDirectory, NSSearchPathDomain.All, true).FirstOrDefault ();
#endif
			Assert.That (path, Is.EqualTo (applicationsPath), "path - ProgramFiles");
#else

			path = TestFolder (Environment.SpecialFolder.ProgramFiles, readOnly: device);
			Assert.That (path, Is.EqualTo ("/Applications"), "path - ProgramFiles");
#endif

			path = TestFolder (Environment.SpecialFolder.UserProfile, readOnly: device);
			var bundlePath = NSBundle.MainBundle.BundlePath;
			var isExtension = bundlePath.EndsWith (".appex", StringComparison.Ordinal);
#if __MACOS__
			Assert.That (path, Is.EqualTo (home), "UserProfile");
#else
			if (TestRuntime.IsDevice) {
				if (isExtension)
					Assert.That (path, Does.StartWith ("/private/var/mobile/Containers/Data/PluginKitPlugin/"), "Containers-ios8");
#if !__WATCHOS__
				else if (UIDevice.CurrentDevice.CheckSystemVersion (8, 0))
					Assert.That (path, Does.StartWith ("/private/var/mobile/Containers/Data/Application/"), "Containers-ios8");
#endif
				else
					Assert.That (path, Does.StartWith ("/private/var/mobile/Applications/"), "pre-Containers");
			}
#endif // __MACOS__

#if !__WATCHOS__ && !__MACOS__
			// tvOS (device sandbox) is more restrictive than iOS as it limit access to more
			// directories, mostly because they are not guaranteed to be preserved between executions
			bool tvos = UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.TV;
			if (tvos)
				docs = Path.Combine (libs, "Caches", "Documents");
#else
			bool tvos = false;
#endif

#if __MACOS__ && NET8_0_OR_GREATER
			path = Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments);
			if (string.IsNullOrEmpty (path) && TestRuntime.IsInCI) {
				// ignore this
			} else {
				path = TestFolder (Environment.SpecialFolder.MyDocuments);
				Assert.That (path, Is.EqualTo (docs), "path - MyDocuments");
			}
#else
			// and some stuff is read/write
			path = TestFolder (Environment.SpecialFolder.MyDocuments);
#if __MACOS__ && !NET8_0_OR_GREATER
			Assert.That (path, Is.EqualTo (home), "path - MyDocuments");
#else
			Assert.That (path, Is.EqualTo (docs), "path - MyDocuments");
#endif
#endif // __MACOS__ && NET8_0_OR_GREATER

#if NET
			path = TestFolder (Environment.SpecialFolder.ApplicationData, exists: null /* may or may not exist */);
#else
			path = TestFolder (Environment.SpecialFolder.ApplicationData);
#endif
#if __MACOS__
#if NET8_0_OR_GREATER
			Assert.That (path, Is.EqualTo (Path.Combine (home, "Library", "Application Support")), "path - ApplicationData");
#else
			Assert.That (path, Is.EqualTo (Path.Combine (home, ".config")), "path - ApplicationData");
#endif
#else
			Assert.That (path, Is.EqualTo (docs + "/.config"), "path - ApplicationData");
#endif

			path = TestFolder (Environment.SpecialFolder.LocalApplicationData);
#if __MACOS__
#if NET8_0_OR_GREATER
			Assert.That (path, Is.EqualTo (Path.Combine (home, "Library", "Application Support")), "path - ApplicationData");
#else
			Assert.That (path, Is.EqualTo (Path.Combine (home, ".local", "share")), "path - LocalApplicationData");
#endif
#else
			Assert.That (path, Is.EqualTo (docs), "path - LocalApplicationData");
#endif

			path = TestFolder (Environment.SpecialFolder.InternetCache);
			Assert.That (path, Is.EqualTo (libs + "/Caches"), "path - InternetCache");

			// new: expose NSLibraryDirectory from Environment.GetFolder
#if __MACOS__
			path = TestFolder (Environment.SpecialFolder.Resources, supported: false);
#else
			path = TestFolder (Environment.SpecialFolder.Resources, readOnly: tvos && device);
			Assert.True (path.EndsWith ("/Library", StringComparison.Ordinal), "Resources");
#endif
		}

#if !__WATCHOS__ && !__MACOS__
		[Test]
		public void Events ()
		{
			using (var tv = new UITextView ()) {
				Assert.Null (tv.WeakDelegate, "none");
				// event on UITextView itself
				tv.Ended += (object sender, EventArgs e) => { };

				var t = tv.WeakDelegate.GetType ();
				Assert.That (t.Name, Is.EqualTo ("_UITextViewDelegate"), "textview");

				var fi = t.GetField ("editingEnded", BindingFlags.NonPublic | BindingFlags.Instance);
				Assert.NotNull (fi, "editingEnded");
				var value = fi.GetValue (tv.WeakDelegate);
				Assert.NotNull (value, "value");

				// and on the UIScrollView defined one
				tv.Scrolled += (object sender, EventArgs e) => { };
				// and the existing (initial field) is still set
				fi = t.GetField ("editingEnded", BindingFlags.NonPublic | BindingFlags.Instance);
				Assert.NotNull (fi, "editingEnded/scrollview");
			}
		}
#endif // !__WATCHOS__ && !__MACOS__

#if !NET
		[Test]
		public void SecurityDeclaration ()
		{
			// note: security declarations != custom attributes
			// we ensure that we can create the type / call the code
			Assert.True (SecurityDeclarationDecoratedUserCode.Check (), "call");
			// we ensure that both the permission and the flag are NOT part of the final/linked binary (link removes security declarations)
			Assert.Null (GetTypeHelper ("System.Security.Permissions.FileIOPermissionAttribute, mscorlib"), "FileIOPermissionAttribute");
			Assert.Null (GetTypeHelper ("System.Security.Permissions.FileIOPermissionAccess, mscorlib"), "FileIOPermissionAccess");
		}
#endif

#if !__WATCHOS__ && !__MACOS__
		[Test]
		public void UIButtonSubclass ()
		{
			// ensure the linker keeps the .ctor(UIButtonType) around
			using (var b = new UIButton (UIButtonType.Custom)) {
				// https://trello.com/c/Nf2B8mIM/484-remove-debug-code-in-the-linker
				var m = b.GetType ().GetMethod ("VerifyIsUIButton", BindingFlags.Instance | BindingFlags.NonPublic);
#if NET
				CheckILLinkStubbedMethod (m);
#else // NET
#if DEBUG
				// kept in debug builds
				Assert.NotNull (m, "VerifyIsUIButton");
#else
				// removed from release builds
				Assert.Null (m, "VerifyIsUIButton");
#endif
#endif // NET
			}
		}

#endif // !__WATCHOS__ && !__MACOS__

#if NET
		static void CheckILLinkStubbedMethod (MethodInfo m)
		{
			// ILLink does not remove the method, but it can "stub" (empty) it
			Assert.NotNull (m, "Method not found (null)");
			var mb = m.GetMethodBody ();
			Assert.NotNull (m, "GetMethodBody");
			var il = mb.GetILAsByteArray ();
#if DEBUG
			// means some stuff in addition to the `ret` instruction
			Assert.That (il.Length, Is.GreaterThan (1), "il > 1");
#else
			// empty means a `ret` instruction (and that's true even if IL is stripped)
			Assert.That (il.Length, Is.EqualTo (1), "il == 1");
#endif
		}

#if __MACOS__
		static Type ApplicationType = typeof (NSApplication);
#else
		static Type ApplicationType = typeof (UIApplication);
#endif

		[Test]
		public void EnsureEventAndDelegateAreNotMismatched ()
		{
			var m = ApplicationType.GetMethod ("EnsureEventAndDelegateAreNotMismatched", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
			CheckILLinkStubbedMethod (m);
		}

#if __MACOS__
		// add some code to make sure these methods aren't completely linked away
		static void PreserveEnsureDelegateMethods ()
		{
			NSApplication.SharedApplication.ApplicationDockMenu = null;
			NSApplication.SharedApplication.WeakDelegate = null;
		}
#endif

		[Test]
		public void EnsureDelegateAssignIsNotOverwritingInternalDelegate ()
		{
			var m = ApplicationType.GetMethod ("EnsureDelegateAssignIsNotOverwritingInternalDelegate", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
			CheckILLinkStubbedMethod (m);
		}
#endif

		[Test]
		public void MonoRuntime34671 ()
		{
#if NET
			Assert.Null (GetTypeHelper ("Mono.Runtime"), "Mono.Runtime");
#else
			Assert.NotNull (GetTypeHelper ("Mono.Runtime"), "Mono.Runtime");
#endif
		}

		[Test]
		public void TraceListeners36255 ()
		{
			Trace.Close (); // here too
			Assert.NotNull (Trace.Listeners, "C6 had a SecurityPermission call");
		}

#if !__WATCHOS__
#if !NET // This test requires Mono.Security.dll, which .NET 5+ doesn't have
		[Test]
		public void TlsProvider_Apple ()
		{
			var provider = Mono.Security.Interface.MonoTlsProviderFactory.GetProvider ();
			Assert.NotNull (provider, "provider");
			Assert.That (provider.ID, Is.EqualTo (new Guid ("981af8af-a3a3-419a-9f01-a518e3a17c1c")), "correct provider");
		}
#endif

#if !__MACOS__
		[Test]
		public void Github5024 ()
		{
			TestRuntime.AssertXcodeVersion (6, 0);
			using (var controller = new UISplitViewController ()) {
				var sc = new UISearchController ((UIViewController) controller);
				sc.SetSearchResultsUpdater ((vc) => { });

				var a = typeof (UISearchController).AssemblyQualifiedName;
				var n = a.Replace ("UIKit.UISearchController", "UIKit.UISearchController+__Xamarin_UISearchResultsUpdating");
				var t = Type.GetType (n);
				Assert.NotNull (t, "private inner type");
				Assert.IsNotNull (t.GetMethod ("UpdateSearchResultsForSearchController"), "preserved");
			}
		}
#endif // !__MACOS__
#endif // !__WATCHOS__

		[Test]
		public void OldTlsProvider_LinkedOut ()
		{
			// make test work for classic (monotouch) and unified (iOS, tvOS and watchOS)
			var fqn = typeof (NSObject).AssemblyQualifiedName.Replace ("Foundation.NSObject", "Security.Tls.OldTlsProvider");
			Assert.Null (GetTypeHelper (fqn), "Should not be included");
		}

		[Test]
		public void AppleTls_Default ()
		{
			// make test work for classic (monotouch) and unified (iOS, tvOS and watchOS)
			var fqn = typeof (NSObject).AssemblyQualifiedName.Replace ("Foundation.NSObject", "Security.Tls.AppleTlsProvider");
			Assert.Null (GetTypeHelper (fqn), "Should be included");
		}

#if !__WATCHOS__ && !__TVOS__ // WebKit isn't available in tvOS or watchOS
		[Test]
		// https://bugzilla.xamarin.com/show_bug.cgi?id=59247
		public void WebKit_NSProxy ()
		{
			// a reference to WKWebView will bring the internal NSProxy type
			var t = typeof (WKWebView);
			Assert.NotNull (t, "avoid compiler optimization of unused variable");
			var fqn = typeof (NSObject).AssemblyQualifiedName.Replace ("Foundation.NSObject", "Foundation.NSProxy");
			Assert.NotNull (GetTypeHelper (fqn), fqn);
		}
#endif // !__WATCHOS__ && !__TVOS__

		// Fools linker not to keep the type by using it in test check
		static Type GetTypeHelper (string name)
		{
			return Type.GetType (name);
		}

		static Type GetTypeHelper (string name, bool throwOnError)
		{
			return Type.GetType (name, throwOnError);
		}

#if !__WATCHOS__
		[Test]
		// https://github.com/xamarin/xamarin-macios/issues/6711
		public void PreserveINativeObject ()
		{
			// linker will keep the MTAudioProcessingTap type
			var mta = typeof (MediaToolbox.MTAudioProcessingTap);
			// and we check that it still implement INativeObject
			Assert.IsNotNull (mta.GetInterface ("ObjCRuntime.INativeObject"), "INativeObject");
		}
#endif

		[Test]
		// https://github.com/xamarin/xamarin-macios/issues/6346
		public void AsQueryable_Enumerable ()
		{
			var list = new List<string> { "hello hello" };
			Assert.NotNull (list.AsQueryable ().GroupBy (x => x).FirstOrDefault ()?.FirstOrDefault (), "Enumerable");
		}

		public class CustomIdentity : IIdentity {
			public string AuthenticationType => "test";
			public bool IsAuthenticated => true;
			public string Name => "abc";
		}

		public class CustomPrincipal : IPrincipal {
			public IIdentity Identity => new CustomIdentity ();
			public bool IsInRole (string role) => true;
		}

		[Test]
		// https://github.com/xamarin/xamarin-macios/issues/7321
		public void Principal ()
		{
			Thread.CurrentPrincipal = new CustomPrincipal ();
			Assert.That (Thread.CurrentPrincipal.Identity.Name, Is.EqualTo ("abc"), "Name");
		}
	}
}
