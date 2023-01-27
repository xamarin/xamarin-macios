// Tests are common to both mtouch and mmp
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using Mono.Cecil;

using Xamarin.Tests;

#if MONOTOUCH
using BundlerTool = Xamarin.MTouchTool;
#else
using BundlerTool = Xamarin.MmpTool;
#endif

namespace Xamarin {
	[TestFixture]
	public class BundlerTests {
#if __MACOS__
		// The cache doesn't work properly in mmp yet.
		// [TestCase (Profile.macOSMobile)]
#else
		[Test]
		[TestCase (Profile.iOS)]
#endif
		public void ModifiedResponseFile (Profile profile)
		{
			using (var bundler = new BundlerTool ()) {
				bundler.Profile = profile;
				bundler.CreateTemporaryCacheDirectory ();
				bundler.CreateTemporaryApp (profile);
				var tmpDir = bundler.CreateTemporaryDirectory ();
				var responseFile = Path.Combine (tmpDir, "test-arguments.txt");
				File.WriteAllText (responseFile, "");
				bundler.ResponseFile = responseFile;
				bundler.Linker = LinkerOption.DontLink; // faster test
				bundler.Debug = true; // faster test
				bundler.Verbosity = 4;
				bundler.AssertExecute ();
				bundler.AssertWarningCount (0);
				bundler.AssertOutputPattern ("A full rebuild will be performed because the cache is either incomplete or entirely missing.");

				File.WriteAllText (responseFile, "/linksdkonly");
				bundler.AssertExecute ();
				bundler.AssertWarningCount (0);
				bundler.AssertOutputPattern ("A full rebuild has been forced because the cache for .* is not valid.");
			}
		}

		[Test]
#if __MACOS__
		[TestCase (Profile.macOSMobile)]
#else
		[TestCase (Profile.iOS)]
#endif
		public void RegisterProtocolOptimization (Profile profile)
		{
			using (var bundler = new BundlerTool ()) {
				bundler.Profile = profile;
				bundler.CreateTemporaryCacheDirectory ();
				bundler.CreateTemporaryApp (profile);
				bundler.Linker = LinkerOption.LinkAll;
				bundler.Registrar = RegistrarOption.Static;
				bundler.Optimize = new string [] { "register-protocols" };
				bundler.AssertExecute ();
				bundler.AssertWarningCount (0);

				AssemblyDefinition ad = AssemblyDefinition.ReadAssembly (bundler.GetPlatformAssemblyInApp ());
				var failures = new List<string> ();
				foreach (var attrib in ad.MainModule.GetCustomAttributes ()) {
					switch (attrib.AttributeType.Name) {
					case "ProtocolAttribute":
					case "ProtocolMemberAttribute":
					case "AdoptsAttribute":
						// Unfortunately the CustomAttribute doesn't know its owner, so we can't show that in the test failure message :(
						failures.Add ($"Found an unexpected attribute: {attrib.AttributeType.FullName}");
						break;
					}
				}
				Assert.That (failures, Is.Empty, "all these attributes should have been linked away");
			}
		}

		[TestCase ("4.6")]
		[TestCase ("4.7")]
#if __MACOS__
		[TestCase ("Xamarin.iOS,v1.0")]
		[TestCase ("Xamarin.WatchOS,v1.0")]
		[TestCase ("Xamarin.TVOS,v1.0")]
#else
		[TestCase ("Xamarin.Mac,Version=v2.0,Profile=Mobile")]
		[TestCase ("Xamarin.Mac,Version=v4.5,Profile=Full")]
		[TestCase ("Xamarin.Mac,Version=v4.5,Profile=System")]
#endif
		public void MX0070 (string target_framework)
		{
			using (var bundler = new BundlerTool ()) {
				bundler.Sdk = BundlerTool.None;
				bundler.TargetFramework = target_framework;
#if MONOTOUCH
				bundler.Action = MTouchAction.None;
#endif
				bundler.AssertExecuteFailure ();
				bundler.AssertErrorPattern (70, $"Invalid target framework: {target_framework}.*");
				bundler.AssertWarningCount (0);
				bundler.AssertErrorCount (1);
			}
		}

#if __MACOS__
		[Test]
		[TestCase ("2.0", "Xamarin.Mac,Version=v4.5,Profile=Full")]
		[TestCase ("3.0", "Xamarin.Mac,Version=v4.5,Profile=Full")]
		[TestCase ("3.5", "Xamarin.Mac,Version=v4.5,Profile=Full")]
		[TestCase ("4.0", "Xamarin.Mac,Version=v4.5,Profile=Full")]
		[TestCase ("4.5", "Xamarin.Mac,Version=v4.5,Profile=Full")]
		[TestCase ("Xamarin.Mac,v2.0", "Xamarin.Mac,Version=v2.0,Profile=Mobile")]
		[TestCase ("Xamarin.Mac,Version=v2.0,Profile=Whatever", "Xamarin.Mac,Version=v2.0,Profile=Mobile")]
		[TestCase ("Xamarin.Mac,Version=v2.0", "Xamarin.Mac,Version=v2.0,Profile=Mobile")]
		[TestCase ("Xamarin.Mac,Version=v2.0,Profile=Full", "Xamarin.Mac,Version=v2.0,Profile=Mobile")]
		[TestCase ("Xamarin.Mac,Version=v4.0,Profile=Full", "Xamarin.Mac,Version=v4.5,Profile=Full")]
		[TestCase ("Xamarin.Mac,Version=v4.6,Profile=Full", "Xamarin.Mac,Version=v4.5,Profile=Full")]
		[TestCase ("Xamarin.Mac,Version=v2.0,Profile=System", "Xamarin.Mac,Version=v2.0,Profile=Mobile")]
		[TestCase ("Xamarin.Mac,Version=v4.0,Profile=System", "Xamarin.Mac,Version=v4.5,Profile=System")]
		[TestCase ("Xamarin.Mac,Version=v4.6,Profile=System", "Xamarin.Mac,Version=v4.5,Profile=System")]
		public void MX0090 (string target_framework, string expected_target_framework)
		{
			using (var bundler = new BundlerTool ()) {
				bundler.Sdk = BundlerTool.None;
				bundler.TargetFramework = target_framework;
				bundler.Linker = LinkerOption.DontLink; // This makes it possible to trigger a reliable error to shortcut mmp execution soon after showing the warning we're looking for.
				bundler.AssertExecuteFailure ();
				bundler.AssertWarning (90, $"The target framework '{target_framework}' is deprecated. Use '{expected_target_framework}' instead.");
				bundler.AssertError (17, "You should provide a root assembly."); // We only want to test the MX0090 warning, and then stop doing stuff asap, which this error does.
				bundler.AssertWarningCount (1);
				bundler.AssertErrorCount (1);
			}
		}

#endif

		[Test]
#if __MACOS__
		[TestCase (Profile.macOSMobile)]
#else
		[TestCase (Profile.iOS)]
#endif
		public void MX2106 (Profile profile)
		{
			using (var bundler = new BundlerTool ()) {
				var code = @"
using System;
using Foundation;
using ObjCRuntime;
class T {
	[BindingImpl (BindingImplOptions.Optimizable)]
	void SetupBlockOptimized_Delegate (Action callback, Delegate block_callback)
	{
		BlockLiteral block = new BlockLiteral ();
		block.SetupBlock (block_callback, callback);
		// don't need anything here, since this won't be executed
		block.CleanupBlock ();
	}

	[BindingImpl (BindingImplOptions.Optimizable)]
	void SetupBlockOptimized_MulticastDelegate (Action callback, MulticastDelegate block_callback)
	{
		BlockLiteral block = new BlockLiteral ();
		block.SetupBlock (block_callback, callback);
		// don't need anything here, since this won't be executed
		block.CleanupBlock ();
	}

	static void Main ()
	{
		Console.WriteLine (typeof (NSObject));
	}
}
";
				bundler.Profile = profile;
				bundler.CreateTemporaryCacheDirectory ();
				bundler.CreateTemporaryApp (profile, code: code, extraArgs: new [] { "/debug:full" });
				bundler.Linker = LinkerOption.LinkAll;
				bundler.Optimize = new string [] { "blockliteral-setupblock" };
				bundler.AssertExecute ();
				bundler.AssertWarning (2106, "Could not optimize the call to BlockLiteral.SetupBlock in System.Void T::SetupBlockOptimized_Delegate(System.Action,System.Delegate) because the type of the value passed as the first argument (the trampoline) is System.Delegate, which makes it impossible to compute the block signature.", "testApp.cs", 10);
				bundler.AssertWarning (2106, "Could not optimize the call to BlockLiteral.SetupBlock in System.Void T::SetupBlockOptimized_MulticastDelegate(System.Action,System.MulticastDelegate) because the type of the value passed as the first argument (the trampoline) is System.MulticastDelegate, which makes it impossible to compute the block signature.", "testApp.cs", 19);
				bundler.AssertWarningCount (2);
			}
		}

#if __MACOS__
		// XM doesn't support removing the dynamic registrar yet.
		//[TestCase (Profile.macOSMobile)]
#else
		[Test]
		[TestCase (Profile.iOS)]
#endif
		public void MX2107 (Profile profile)
		{
			using (var bundler = new BundlerTool ()) {
				var code = @"
using System;
using Foundation;
using ObjCRuntime;
class T {
	static void Main ()
	{
		TypeConverter.ToManaged (""@"");
		Runtime.ConnectMethod (typeof (NSObject), typeof (T).GetMethod (""Main""), new Selector (""sel""));
		Runtime.ConnectMethod (typeof (NSObject), typeof (T).GetMethod (""Main""), new ExportAttribute (""sel""));
		Runtime.ConnectMethod (typeof (T).GetMethod (""Main""), new Selector (""sel""));
		Runtime.RegisterAssembly (null);
		BlockLiteral bl = default (BlockLiteral);
		Action action = null;
		bl.SetupBlock (action, action);
		bl.SetupBlockUnsafe (action, action);
	}
}
";
				bundler.Profile = profile;
				bundler.CreateTemporaryCacheDirectory ();
				bundler.CreateTemporaryApp (profile, code: code, extraArgs: new [] { "/debug:full" });
				bundler.Linker = LinkerOption.LinkSdk;
				bundler.Registrar = RegistrarOption.Static;
				bundler.Optimize = new string [] { "remove-dynamic-registrar" };
				bundler.AssertExecute ();
				bundler.AssertWarning (2107, "It's not safe to remove the dynamic registrar, because testApp references 'ObjCRuntime.TypeConverter.ToManaged (System.String)'.");
				bundler.AssertWarning (2107, "It's not safe to remove the dynamic registrar, because testApp references 'ObjCRuntime.Runtime.ConnectMethod (System.Type, System.Reflection.MethodInfo, ObjCRuntime.Selector)'.");
				bundler.AssertWarning (2107, "It's not safe to remove the dynamic registrar, because testApp references 'ObjCRuntime.Runtime.ConnectMethod (System.Type, System.Reflection.MethodInfo, Foundation.ExportAttribute)'.");
				bundler.AssertWarning (2107, "It's not safe to remove the dynamic registrar, because testApp references 'ObjCRuntime.Runtime.ConnectMethod (System.Reflection.MethodInfo, ObjCRuntime.Selector)'.");
				bundler.AssertWarning (2107, "It's not safe to remove the dynamic registrar, because testApp references 'ObjCRuntime.Runtime.RegisterAssembly (System.Reflection.Assembly)'.");
				bundler.AssertWarning (2107, "It's not safe to remove the dynamic registrar, because testApp references 'ObjCRuntime.BlockLiteral.SetupBlock (System.Delegate, System.Delegate)'.");
				bundler.AssertWarning (2107, "It's not safe to remove the dynamic registrar, because testApp references 'ObjCRuntime.BlockLiteral.SetupBlockUnsafe (System.Delegate, System.Delegate)'.");
				bundler.AssertWarningCount (7);

				// try again with link all, now the warnings about SetupBlock[Unsafe] should be gone
				bundler.Linker = LinkerOption.LinkAll;
				bundler.AssertExecute ();
				bundler.AssertWarning (2107, "It's not safe to remove the dynamic registrar, because testApp references 'ObjCRuntime.TypeConverter.ToManaged (System.String)'.");
				bundler.AssertWarning (2107, "It's not safe to remove the dynamic registrar, because testApp references 'ObjCRuntime.Runtime.ConnectMethod (System.Type, System.Reflection.MethodInfo, ObjCRuntime.Selector)'.");
				bundler.AssertWarning (2107, "It's not safe to remove the dynamic registrar, because testApp references 'ObjCRuntime.Runtime.ConnectMethod (System.Type, System.Reflection.MethodInfo, Foundation.ExportAttribute)'.");
				bundler.AssertWarning (2107, "It's not safe to remove the dynamic registrar, because testApp references 'ObjCRuntime.Runtime.ConnectMethod (System.Reflection.MethodInfo, ObjCRuntime.Selector)'.");
				bundler.AssertWarning (2107, "It's not safe to remove the dynamic registrar, because testApp references 'ObjCRuntime.Runtime.RegisterAssembly (System.Reflection.Assembly)'.");
				bundler.AssertWarningCount (5);
			}
		}

		[Test]
#if __MACOS__
		[TestCase (Profile.macOSMobile)]
#else
		[TestCase (Profile.iOS)]
#endif
		public void MX4105 (Profile profile)
		{
			using (var bundler = new BundlerTool ()) {
				var code = @"
using System;
using Foundation;
using ObjCRuntime;
class D : NSObject {
	[Export (""d1:"")]
	public void D1 (Delegate d)
	{
	}

	[Export (""d2:"")]
	public void D2 (MulticastDelegate d)
	{
	}

	[Export (""d3:"")]
	public void D3 (Action d)
	{
		// This should not show errors
	}

	[Export (""d4"")]
	public Delegate D4 ()
	{
		return null;
	}

	[Export (""d5"")]
	public MulticastDelegate D5 ()
	{
		return null;
	}

	[Export (""d6"")]
	public Action D6 ()
	{
		return null;
	}

	static void Main ()
	{
		Console.WriteLine (typeof (NSObject));
	}
}
";
				bundler.Profile = profile;
				bundler.CreateTemporaryCacheDirectory ();
				bundler.CreateTemporaryApp (profile, code: code, extraArgs: new [] { "/debug:full" });
				bundler.Optimize = new string [] { "blockliteral-setupblock" };
				bundler.Registrar = RegistrarOption.Static;
				bundler.AssertExecuteFailure ();
				bundler.AssertError (4105, "The registrar cannot marshal the parameter of type `System.Delegate` in signature for method `D.D1`.");
				bundler.AssertError (4105, "The registrar cannot marshal the parameter of type `System.MulticastDelegate` in signature for method `D.D2`.");
				bundler.AssertWarning (4173, "The registrar can't compute the block signature for the delegate of type System.Delegate in the method D.D4 because System.Delegate doesn't have a specific signature.", "testApp.cs", 24);
				bundler.AssertWarning (4173, "The registrar can't compute the block signature for the delegate of type System.MulticastDelegate in the method D.D5 because System.MulticastDelegate doesn't have a specific signature.", "testApp.cs", 30);
				bundler.AssertWarning (4174, "Unable to locate the block to delegate conversion method for the method D.D3's parameter #1.", "testApp.cs", 18);
				bundler.AssertWarning (4176, "Unable to locate the delegate to block conversion type for the return value of the method D.D4.", "testApp.cs", 24);
				bundler.AssertWarning (4176, "Unable to locate the delegate to block conversion type for the return value of the method D.D5.", "testApp.cs", 30);
				bundler.AssertWarning (4176, "Unable to locate the delegate to block conversion type for the return value of the method D.D6.", "testApp.cs", 36);
				bundler.AssertErrorCount (2);
				bundler.AssertWarningCount (6);
			}
		}

		[Test]
#if __MACOS__
		[TestCase (Profile.macOSMobile)]
#else
		[TestCase (Profile.iOS)]
#endif
		public void MX4175 (Profile profile)
		{
			var code = @"
using System;
using Foundation;
using ObjCRuntime;
class Issue4072Session : NSUrlSession {
	public Issue4072Session ()
		: base (IntPtr.Zero)
	{
	}
	public override NSUrlSessionDataTask CreateDataTask (NSUrl url, [BlockProxy (typeof (Delegate))] NSUrlSessionResponse completionHandler)
	{
		return base.CreateDataTask (url, completionHandler);
	}

	static void Main ()
	{
		Console.WriteLine (typeof (NSObject));
	}
}
";
			using (var bundler = new BundlerTool ()) {
				bundler.Profile = profile;
				bundler.CreateTemporaryCacheDirectory ();
				bundler.CreateTemporaryApp (profile, code: code, extraArgs: new [] { "/debug:full" });
				bundler.Registrar = RegistrarOption.Static;
				bundler.Linker = LinkerOption.DontLink;
				bundler.AssertExecute ();
				bundler.AssertWarning (4175, "The parameter 'completionHandler' in the method 'Issue4072Session.CreateDataTask(Foundation.NSUrl,Foundation.NSUrlSessionResponse)' has an invalid BlockProxy attribute (the type passed to the attribute does not have a 'Create' method).", "testApp.cs", 11);
#if __MACOS__
				bundler.AssertWarning (5220, "Skipping framework 'QTKit'. It is prohibited (rejected) by the Mac App Store");
				bundler.AssertWarningCount (2);
#else
				bundler.AssertWarningCount (1);
#endif
			}

			using (var bundler = new BundlerTool ()) {
				bundler.Profile = profile;
				bundler.CreateTemporaryCacheDirectory ();
				bundler.CreateTemporaryApp (profile, code: code, extraArgs: new [] { "/debug-" }); // Build without debug info so that the source code location isn't available.
				bundler.Registrar = RegistrarOption.Static;
#if !__MACOS__
				bundler.Linker = LinkerOption.LinkAll; // This will remove the parameter name in Xamarin.iOS (the parameter name removal optimization (MetadataReducerSubStep) isn't implemented for Xamarin.Mac).
#endif
				bundler.AssertExecute ();
#if __MACOS__
				bundler.AssertWarning (4175, "The parameter 'completionHandler' in the method 'Issue4072Session.CreateDataTask(Foundation.NSUrl,Foundation.NSUrlSessionResponse)' has an invalid BlockProxy attribute (the type passed to the attribute does not have a 'Create' method).");
#else
				bundler.AssertWarning (4175, "Parameter #2 in the method 'Issue4072Session.CreateDataTask(Foundation.NSUrl,Foundation.NSUrlSessionResponse)' has an invalid BlockProxy attribute (the type passed to the attribute does not have a 'Create' method).");
#endif
				bundler.AssertWarningCount (1);
			}
		}

		[Test]
#if __MACOS__
		[TestCase (Profile.macOSMobile)]
#else
		[TestCase (Profile.iOS)]
#endif
		public void XmlUnexpandedVariable_7904 (Profile profile)
		{
			using (var bundler = new BundlerTool ()) {
				var code = @"
using System;
using Foundation;
using ObjCRuntime;
class T {
 	static void Main ()
	{
		Console.WriteLine (typeof (NSObject));
	}
}
";
				bundler.Profile = profile;
				bundler.CreateTemporaryCacheDirectory ();
				bundler.CreateTemporaryApp (profile, code: code);
				// typo is intentional since `{ProjectDir}` should be expanded (different issue)
				bundler.CustomArguments = new [] { "--xml=${xProjectDir}/linker.xml", "-v", "-v", "-v", "-v" };
				bundler.AssertExecuteFailure ();
				// {} messing with a good pattern substitution - we want to avoid an MT0000 when verbosity is applied
				bundler.AssertErrorPattern (2004, "");
				bundler.AssertErrorCount (1);
			}
		}

		[Test]
#if __MACOS__
		[TestCase (Profile.macOSMobile)]
#else
		[TestCase (Profile.iOS)]
#endif
		public void MX1502_3 (Profile profile)
		{
			using (var bundler = new BundlerTool ()) {
				var code = @"
using System;
using Foundation;
using ObjCRuntime;
class T {
	static void Main ()
	{
		Console.WriteLine (typeof (NSObject));
	}
}
";
				bundler.Profile = profile;
				bundler.CreateTemporaryCacheDirectory ();
				bundler.CreateTemporaryApp (profile, code: code);
				bundler.Linker = LinkerOption.LinkAll;
				bundler.CustomArguments = new [] { "--warn-on-type-ref=Foundation.NSObject" };
				bundler.AssertExecute ();
				bundler.AssertWarning (1502, "One or more reference(s) to type 'Foundation.NSObject' already exists inside 'testApp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null' before linking");
				bundler.AssertWarning (1503, "One or more reference(s) to type 'Foundation.NSObject' still exists inside 'testApp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null' after linking");
				bundler.AssertWarningCount (2);
			}
		}
	}
}
