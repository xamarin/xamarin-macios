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

using MTouchLinker = Xamarin.Tests.LinkerOption;
using MTouchRegistrar = Xamarin.Tests.RegistrarOption;

namespace Xamarin
{
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
				mtouch.Linker = MTouchLinker.DontLink;
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
				mtouch.Linker = MTouchLinker.LinkAll;
				mtouch.AssertExecute (MTouchAction.BuildSim, "build");

				AssertStaticLinked (mtouch);
				Assert.That (mtouch.NativeSymbolsInExecutable, Does.Not.Contain ("mono_native_initialize"));
			}
		}

		[Test]
		public void TestDeviceLinkOut ()
		{
			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryApp ();
				mtouch.Linker = MTouchLinker.LinkSdk;
				mtouch.AssertExecute (MTouchAction.BuildDev, "build");

				AssertStaticLinked (mtouch);
				Assert.That (mtouch.NativeSymbolsInExecutable, Does.Not.Contain ("mono_native_initialize"));
			}
		}

		[Test]
		public void TestDebugLinkAll ()
		{
			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryApp (code: MonoNativeInitialize);
				mtouch.Debug = true;
				mtouch.FastDev = true;
				mtouch.Linker = MTouchLinker.LinkAll;
				mtouch.AssertExecute (MTouchAction.BuildSim, "build");

				AssertStaticLinked (mtouch);
				Assert.That (mtouch.NativeSymbolsInExecutable, Does.Contain ("_mono_native_initialize"));
			}
		}

		[Test]
		public void TestDeviceLinkAll ()
		{
			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryApp (code: MonoNativeInitialize);
				mtouch.Linker = MTouchLinker.LinkAll;
				mtouch.AssertExecute (MTouchAction.BuildDev, "build");

				AssertStaticLinked (mtouch);
				Assert.That (mtouch.NativeSymbolsInExecutable, Does.Contain ("_mono_native_initialize"));
			}
		}

		[Test]
		public void TestDeviceFrameworkCompat ()
		{
			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryApp (code: MonoNativeInitialize);
				mtouch.Linker = MTouchLinker.LinkAll;
				mtouch.AssemblyBuildTargets.Add ("@all=framework");
				mtouch.TargetVer = "9.3";

				mtouch.AssertExecute (MTouchAction.BuildDev, "build");

				var files = Directory.EnumerateFiles (mtouch.AppPath, "libmono-native*", SearchOption.AllDirectories).Select (Path.GetFileName);
				Assert.That (files.Count, Is.EqualTo (1), "One single libmono-native* library");
				Assert.That (files.First (), Is.EqualTo ("libmono-native-compat.dylib"), "Found libmono-native-compat.dylib");

				var libpath = Path.Combine (mtouch.AppPath, "libmono-native-compat.dylib");
				Assert.That (MTouch.GetNativeSymbols (libpath), Does.Contain ("_mono_native_initialize"));
			}
		}

		[Test]
		public void TestDeviceFrameworkUnified ()
		{
			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryApp (code: MonoNativeInitialize);
				mtouch.Linker = MTouchLinker.LinkAll;
				mtouch.AssemblyBuildTargets.Add ("@all=framework");
				mtouch.TargetVer = "10.0";

				mtouch.AssertExecute (MTouchAction.BuildDev, "build");

				var files = Directory.EnumerateFiles (mtouch.AppPath, "libmono-native*", SearchOption.AllDirectories).Select (Path.GetFileName);
				Assert.That (files.Count, Is.EqualTo (1), "One single libmono-native* library");
				Assert.That (files.First (), Is.EqualTo ("libmono-native-unified.dylib"), "Found libmono-native-compat.dylib");

				var libpath = Path.Combine (mtouch.AppPath, "libmono-native-unified.dylib");
				Assert.That (MTouch.GetNativeSymbols (libpath), Does.Contain ("_mono_native_initialize"));
			}
		}

		// [Test]
		public void MartinTest ()
		{
			using (var mtouch = new MTouchTool ()) {
				mtouch.CreateTemporaryApp ();
				mtouch.Debug = true;
				mtouch.FastDev = true;
				mtouch.Verbosity = 3;
				mtouch.Linker = MTouchLinker.DontLink;
				mtouch.AssertExecute (MTouchAction.BuildSim, "build");

				Console.WriteLine ($"BUILD: {mtouch.AppPath}");

				foreach (var file in Directory.EnumerateFiles (mtouch.AppPath))
					Console.WriteLine ($"  FILE: {file}");

				var files2 = Directory.EnumerateFiles (mtouch.AppPath, "libmono-native*", SearchOption.AllDirectories).Select (Path.GetFileName);
				foreach (var file in files2)
					Console.WriteLine ($"  FILE #1: {file}");

				Console.WriteLine ($"DONE!");

				AssertSymlinked (mtouch.AppPath);

				// CollectionAssert.Contains (Directory.EnumerateFiles (extension.AppPath, "*", SearchOption.AllDirectories).Select ((v) => Path.GetFileName (v)), "testServiceExtension.dll.config", "extension config added");


				throw new NotImplementedException ();
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
	[System.Runtime.InteropServices.DllImport (""__Internal"")]
	extern static void mono_native_initialize ();

	static void Main ()
	{
		System.Console.WriteLine (typeof (ObjCRuntime.Runtime).ToString ());
		mono_native_initialize ();
	}
}
";
	}
}
