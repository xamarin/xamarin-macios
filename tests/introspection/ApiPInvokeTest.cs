//
// ApiPInvokeTest.cs: enforce P/Invoke signatures
//
// Authors:
//   Aaron Bockover <abock@xamarin.com>
//   Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2013-2014 Xamarin, Inc.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Xamarin.Tests;

using NUnit.Framework;

#if XAMCORE_2_0
using ObjCRuntime;
using Foundation;
#elif MONOMAC
using MonoMac.ObjCRuntime;
using MonoMac.Foundation;
#else
using MonoTouch.ObjCRuntime;
using MonoTouch.Foundation;
#endif

namespace Introspection
{
	[Preserve (AllMembers = true)]
	public abstract class ApiPInvokeTest : ApiBaseTest {
		IEnumerable pinvokeQuery;

		public ApiPInvokeTest ()
		{
			ContinueOnFailure = true;
			LogProgress = false;

			pinvokeQuery = from type in Assembly.GetTypes ()
					where !Skip (type)
				from mi in type.GetMethods (
					BindingFlags.NonPublic |
					BindingFlags.Public |
					BindingFlags.Static)
				let attr = mi.GetCustomAttribute<DllImportAttribute> ()
					where attr != null && !Skip (mi)
				select mi;
		}

		protected virtual bool Skip (Type type)
		{
			return SkipDueToAttribute (type);
		}

		protected virtual bool Skip (MethodInfo methodInfo)
		{
			return SkipDueToAttribute (methodInfo);
		}

		[Test]
		public void Signatures ()
		{
			int totalPInvokes = 0;
			Errors = 0;

			foreach (MethodInfo mi in pinvokeQuery) {
				totalPInvokes++;
				if (!CheckSignature (mi)) {

					if (!ContinueOnFailure)
						break;
				}
			}

			AssertIfErrors (
				"{0} errors found in {1} P/Invoke signatures validated",
				Errors, totalPInvokes);
		}

		protected virtual bool CheckSignature (MethodInfo mi)
		{
			var success = true;

			if (!CheckReturnParameter (mi, mi.ReturnParameter))
				success = false;

			foreach (var pi in mi.GetParameters ()) {
				if (!CheckParameter (mi, pi))
					success = false;
			}

			return success;
		}

		protected virtual bool CheckReturnParameter (MethodInfo mi, ParameterInfo pi)
		{
			return CheckParameter (mi, pi);
		}

		protected virtual bool CheckParameter (MethodInfo mi, ParameterInfo pi)
		{
			bool result = true;
			// `ref` is fine but it can hide the droids we're looking for
			var pt = pi.ParameterType;
			if (pt.IsByRef)
				pt = pt.GetElementType ();
			// we don't want generics in p/invokes except for delegates like Func<> and Action<> which we know how to deal with
			// ref: https://bugzilla.xamarin.com/show_bug.cgi?id=42699
			if (pt.IsGenericType && !pt.IsSubclassOf (typeof (Delegate))) {
				AddErrorLine ("[FAIL] {0}.{1} has a generic parameter in its signature: {2} {3}",
					mi.DeclaringType.FullName, mi.Name, pt, pi.Name);
				result = false;
			}
			result &= CheckForEnumParameter (mi, pi);
			return result;
		}

		protected virtual bool CheckForEnumParameter (MethodInfo mi, ParameterInfo pi)
		{
			if (pi.ParameterType.IsEnum && pi.ParameterType.GetCustomAttribute<NativeAttribute> () != null) {
				AddErrorLine ("[FAIL] {0}.{1} has a [Native] enum parameter in its signature: {2} {3}",
					mi.DeclaringType.FullName, mi.Name, pi.ParameterType, pi.Name);
				return false;
			}

			return true;
		}

		protected virtual bool Skip (string symbolName)
		{
			return false;
		}

		protected virtual bool SkipLibrary (string libraryName)
		{
			return false;
		}

		[Test]
		public void SymbolExists ()
		{
			var failed_api = new List<string> ();
			Errors = 0;
			int c = 0, n = 0;
			foreach (MethodInfo mi in pinvokeQuery) {

				if (LogProgress)
					Console.WriteLine ("{0}. {1}", c++, mi);

				var dllimport = mi.GetCustomAttribute<DllImportAttribute> ();

				string libname = dllimport.Value;
				switch (libname) {
				case "__Internal":
					continue;
				case "System.Native":
				case "System.Security.Cryptography.Native.Apple":
					switch (MonoNativeConfig.LinkMode) {
					case MonoNativeLinkMode.Static:
						libname = null;
						break;
					case MonoNativeLinkMode.Dynamic:
						libname = MonoNativeConfig.DynamicLibraryName;
						break;
					case MonoNativeLinkMode.None:
						continue;
					default:
						AddErrorLine ($"[FAIL] Invalid link mode: {MonoNativeConfig.LinkMode}");
						continue;
					}
					break;
				}

				if (SkipLibrary (libname))
					continue;

				string path = FindLibrary (libname, requiresFullPath: true);

				string name = dllimport.EntryPoint ?? mi.Name;
				if (Skip (name))
					continue;

				IntPtr lib = Dlfcn.dlopen (path, 0);
				if (Dlfcn.GetIndirect (lib, name) == IntPtr.Zero) {
					ReportError ("Could not find the field '{0}' in {1}", name, path);
					failed_api.Add (name);
				}
				Dlfcn.dlclose (lib);
				n++;
			}
			Assert.AreEqual (0, Errors, "{0} errors found in {1} functions validated: {2}", Errors, n, string.Join (", ", failed_api));
		}

		// we just want to confirm the symbol exists so `dlsym` can be disabled
		protected void Check (Assembly a)
		{
			Errors = 0;
			ErrorData.Clear ();
			int n = 0;
			foreach (var t in a.GetTypes ()) {
				foreach (var m in t.GetMethods (BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)) {
					if ((m.Attributes & MethodAttributes.PinvokeImpl) == 0)
						continue;

					var dllimport = m.GetCustomAttribute<DllImportAttribute> ();

					string name = dllimport.EntryPoint ?? m.Name;
					switch (name) {
					// known not to be present in ARM64
					case "objc_msgSend_stret":
					case "objc_msgSendSuper_stret":
						// the linker normally removes them (IntPtr.Size optimization)
						continue;
					}

					string path = dllimport.Value;
					switch (path) {
					case "__Internal":
						// load from executable
						path = null;
						break;
					case "libc":
						// we still have some rogue/not-fully-qualified DllImport
						path = "/usr/lib/libSystem.dylib";
						break;
					case "System.Native":
					case "System.Security.Cryptography.Native.Apple":
						switch (MonoNativeConfig.LinkMode) {
						case MonoNativeLinkMode.Static:
							path = null;
							break;
						case MonoNativeLinkMode.Dynamic:
							path = MonoNativeConfig.DynamicLibraryName;
							break;
						case MonoNativeLinkMode.None:
							continue;
						default:
							AddErrorLine ($"[FAIL] Invalid link mode: {MonoNativeConfig.LinkMode}");
							continue;
						}
						break;
					}

					var lib = Dlfcn.dlopen (path, 0);
					var h = Dlfcn.dlsym (lib, name);
					if (h == IntPtr.Zero)
						ReportError ("Could not find the symbol '{0}' in {1}", name, path);
					Dlfcn.dlclose (lib);
					n++;
				}
			}
			Assert.AreEqual (0, Errors, "{0} errors found in {1} symbol lookups{2}", Errors, n, Errors == 0 ? string.Empty : ":\n" + ErrorData.ToString () + "\n");
		}

		protected abstract bool SkipAssembly (Assembly a);

		// Note: this looks very similar to the "SymbolExists" test above (and it is)
		// except that we never skip based on availability attributes or __Internal...
		// since this is a test to ensure thigns will work at native link time (e.g. 
		// for devices) when dlsym is disabled

		[Test]
		public void Product ()
		{
			var a = typeof (NSObject).Assembly;
			if (!SkipAssembly (a))
				Check (a);
		}

		// since we already have non-linked version of the most common assemblies available here
		// we can use them to check for missing symbols (from DllImport)
		// it's not complete (there's many more SDK assemblies) but we cannot add all of them into a single project anyway

		[Test]
		public void Corlib ()
		{
			var a = typeof (int).Assembly;
			if (!SkipAssembly (a))
				Check (a);
		}

		[Test]
		public void System ()
		{
			var a = typeof (System.Net.WebClient).Assembly;
			if (!SkipAssembly (a))
				Check (a);
		}

		[Test]
		public void SystemCore ()
		{
			var a = typeof (Enumerable).Assembly;
			if (!SkipAssembly (a))
				Check (a);
		}
	}
}
