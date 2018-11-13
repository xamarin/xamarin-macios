//
// MonoNativeTests.cs
//
// Author:
//       Martin Baulig <mabaul@microsoft.com>
//
// Copyright (c) 2018 Xamarin Inc. (http://www.xamarin.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.IO;
using System.Linq;
using NUnit.Framework;

// using MTouchLinker = Xamarin.Tests.LinkerOption;
// using ExecutionHelper = Xamarin.Tests.ExecutionHelper;
// using MTouchRegistrar = Xamarin.Tests.RegistrarOption;

namespace Xamarin
{
	using Tests;
	using Utils;

	[TestFixture]
	public class MonoNativeTests
	{
		[Test]
		public void TestDebugSymlink ()
		{
			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryApp ();
				mtouch.Debug = true;
				mtouch.FastDev = true;
				mtouch.Linker = LinkerOption.DontLink;
				mtouch.AssertExecute (MTouchAction.BuildSim, "build");

				AssertSymlinked (mtouch.AppPath);
			}
		}

		[Test]
		public void TestDebugLinkOut ()
		{
			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryApp ();
				mtouch.Debug = true;
				mtouch.FastDev = true;
				mtouch.Linker = LinkerOption.LinkAll;
				mtouch.AssertExecute (MTouchAction.BuildSim, "build");

				AssertStaticLinked (mtouch);
				Assert.That (mtouch.NativeSymbolsInExecutable, Does.Not.Contain ("_mono_native_initialize"));
				Assert.That (mtouch.NativeSymbolsInExecutable, Does.Not.Contain ("_NetSecurityNative_ImportUserName"));
			}
		}

		[Test]
		public void TestDeviceLinkOut ()
		{
			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryApp ();
				mtouch.Linker = LinkerOption.LinkSdk;
				mtouch.AssertExecute (MTouchAction.BuildDev, "build");

				AssertStaticLinked (mtouch);
				Assert.That (mtouch.NativeSymbolsInExecutable, Does.Not.Contain ("_mono_native_initialize"));
				Assert.That (mtouch.NativeSymbolsInExecutable, Does.Not.Contain ("_NetSecurityNative_ImportUserName"));
			}
		}

		[Test]
		public void TestDebugLinkAll ()
		{
			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryApp (code: MonoNativeInitialize);
				mtouch.Debug = true;
				mtouch.FastDev = true;
				mtouch.Linker = LinkerOption.LinkAll;
				mtouch.AssertExecute (MTouchAction.BuildSim, "build");

				AssertStaticLinked (mtouch);
				Assert.That (mtouch.NativeSymbolsInExecutable, Does.Contain ("_mono_native_initialize"));
				Assert.That (mtouch.NativeSymbolsInExecutable, Does.Not.Contain ("_NetSecurityNative_ImportUserName"));
			}
		}

		[Test]
		public void TestDeviceLinkAll ()
		{
			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryApp (code: MonoNativeInitialize);
				mtouch.Linker = LinkerOption.LinkAll;
				mtouch.AssertExecute (MTouchAction.BuildDev, "build");

				AssertStaticLinked (mtouch);
				Assert.That (mtouch.NativeSymbolsInExecutable, Does.Contain ("_mono_native_initialize"));
				Assert.That (mtouch.NativeSymbolsInExecutable, Does.Not.Contain ("_NetSecurityNative_ImportUserName"));
			}
		}

		[Test]
		public void TestDeviceFrameworkCompat ()
		{
			TestDeviceFramework ("9.3", "libmono-native-compat.dylib");
		}

		[Test]
		public void TestDeviceFrameworkUnified ()
		{
			TestDeviceFramework ("10.0", "libmono-native-unified.dylib");
		}

		void TestDeviceFramework (string version, string mono_native_dylib)
		{
			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryApp (code: MonoNativeInitialize);
				mtouch.Linker = LinkerOption.LinkAll;
				mtouch.AssemblyBuildTargets.Add ("@all=framework");
				mtouch.TargetVer = version;

				mtouch.AssertExecute (MTouchAction.BuildDev, "build");

				var files = Directory.EnumerateFiles (mtouch.AppPath, "libmono-native*", SearchOption.AllDirectories).Select (Path.GetFileName);
				Assert.That (files.Count, Is.EqualTo (1), "One single libmono-native* library");
				Assert.That (files.First (), Is.EqualTo (mono_native_dylib));

				var mono_native_path = Path.Combine (mtouch.AppPath, mono_native_dylib);

				var symbols = MTouch.GetNativeSymbols (mono_native_path);
				Assert.That (symbols, Does.Contain ("_mono_native_initialize"));
				Assert.That (symbols, Does.Contain ("_NetSecurityNative_ImportUserName"));

				var otool_dylib = ExecutionHelper.Execute ("otool", $"-L {StringUtils.Quote (mono_native_path)}", hide_output: true);
				Assert.That (otool_dylib, Does.Contain ("/System/Library/Frameworks/GSS.framework/GSS"));

				var otool_exe = ExecutionHelper.Execute ("otool", $"-L {StringUtils.Quote (mtouch.NativeExecutablePath)}", hide_output: true);
				Assert.That (otool_exe, Does.Not.Contain ("GSS"));
				Assert.That (otool_exe, Does.Contain ($"@rpath/{mono_native_dylib}")); 
			}
		}

		[Test]
		public void TestDeviceFrameworkLinkOut ()
		{
			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryApp ();
				mtouch.Linker = LinkerOption.LinkAll;
				mtouch.AssemblyBuildTargets.Add ("@all=framework");
				mtouch.TargetVer = "10.0";
				mtouch.Verbosity = 3;

				mtouch.AssertExecute (MTouchAction.BuildDev, "build");

				var files = Directory.EnumerateFiles (mtouch.AppPath, "libmono-native*", SearchOption.AllDirectories).Select (Path.GetFileName);
				Assert.That (files.Count, Is.EqualTo (0), "No libmono-native* library");
			}
		}

		[Test]
		public void TestGss ()
		{
			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryApp (code: MonoNativeGss);
				mtouch.Linker = LinkerOption.LinkAll;
				mtouch.AssertExecute (MTouchAction.BuildSim, "build");

				AssertStaticLinked (mtouch);
				var symbols = mtouch.NativeSymbolsInExecutable;
				Assert.That (symbols, Does.Contain ("_mono_native_initialize"));
				Assert.That (symbols, Does.Contain ("_NetSecurityNative_ImportUserName"));

				var otool_exe = ExecutionHelper.Execute ("otool", $"-L {StringUtils.Quote (mtouch.NativeExecutablePath)}", hide_output: true);
				Assert.That (otool_exe, Does.Contain ("/System/Library/Frameworks/GSS.framework/GSS"));
			}
		}

		[Test]
		public void TestGssTv ()
		{
			using (var mtouch = new MTouchTool ()) {
				mtouch.Profile = Profile.tvOS;
				mtouch.CreateTemporaryApp (code: MonoNativeGss);
				mtouch.Linker = LinkerOption.LinkAll;
				mtouch.Verbosity = 3;
				
				mtouch.AssertExecuteFailure (MTouchAction.BuildSim, "build");
				mtouch.AssertError (5214, "Native linking failed, undefined symbol: _NetSecurityNative_ImportUserName. This symbol was referenced by the managed member X.NetSecurityNative_ImportUserName. Please verify that all the necessary frameworks have been referenced and native libraries linked.");
			}
		}

		void AssertSymlinked (string path)
		{
			var files = Directory.EnumerateFiles (path, "libmono-native*", SearchOption.AllDirectories).Select (Path.GetFileName);
			Assert.That (files.Count, Is.EqualTo (1), "One single libmono-native* library");
			Assert.That (files.First (), Is.EqualTo ("libmono-native.dylib"), "Found libmono-native.dylib");
		}

		void AssertStaticLinked (MTouchTool app)
		{
			var files = Directory.EnumerateFiles (app.AppPath, "libmono-native*", SearchOption.AllDirectories).Select (Path.GetFileName);
			Assert.That (files.Count, Is.EqualTo (0), "No libmono-native* libraries");
		}

		string MonoNativeInitialize => @"
class X {
	[System.Runtime.InteropServices.DllImport (""System.Native"")]
	extern static void mono_native_initialize ();

	static void Main ()
	{
		System.Console.WriteLine (typeof (ObjCRuntime.Runtime).ToString ());
		mono_native_initialize ();
	}
}
";

		string MonoNativeGss => @"
using System;

class X {
	[System.Runtime.InteropServices.DllImport (""System.Native"")]
	extern static void mono_native_initialize ();

	[System.Runtime.InteropServices.DllImport (""System.Net.Security.Native"")]
	extern static void NetSecurityNative_ImportUserName (IntPtr a, IntPtr b, int c, IntPtr d);

	static void Main ()
	{
		// Reference Xamarin.iOS
		var runtime = typeof (ObjCRuntime.Runtime).ToString ();
		// Always false, but the linker does not know that, so the following code won't be linked out.
		if (runtime.Equals (""XXX"")) {
			mono_native_initialize ();
			NetSecurityNative_ImportUserName (IntPtr.Zero, IntPtr.Zero, 0, IntPtr.Zero);
		}
	}
}
";

	}
}
