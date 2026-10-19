// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

//
// Verify that [UnsupportedSimulator] / [SupportedSimulator] attributes
// are consistent with actual symbol availability in the simulator.
//

using System.IO;
using System.Linq;
using System.Reflection;
using Xamarin.Tests;

#nullable enable

namespace Introspection {
	[Preserve (AllMembers = true)]
	public abstract class ApiSimulatorAvailabilityTest : ApiBaseTest {

		protected ApiSimulatorAvailabilityTest ()
		{
			ContinueOnFailure = true;
			LogProgress = false;
		}

		protected virtual bool Skip (Type type)
		{
			return false;
		}

		protected virtual bool SkipPInvoke (MethodInfo mi)
		{
			return false;
		}

		protected virtual bool SkipField (PropertyInfo pi)
		{
			return SkipMissingSimulatorField (pi);
		}

		protected virtual bool SkipLibrary (string? libraryName)
		{
			return false;
		}

		/// <summary>
		/// Verifies that P/Invokes marked as unsupported in the simulator are indeed unavailable.
		/// Also detects P/Invokes not marked as unsupported that are missing from the simulator.
		/// </summary>
		[Test]
		public void PInvokeSimulatorAvailability ()
		{
			if (!TestRuntime.IsSimulator) {
				Assert.Ignore ("This test only runs in the simulator.");
				return;
			}

			var failed_api = new List<string> ();
			Errors = 0;
			int n = 0;

			foreach (var type in Assembly.GetTypes ()) {
				if (Skip (type))
					continue;

				if (!type.IsAvailableOnHostPlatform ())
					continue;

				foreach (var mi in type.GetMethods (BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)) {
					var dllimport = mi.GetCustomAttribute<DllImportAttribute> ();
					if (dllimport is null)
						continue;

					if (!mi.IsAvailableOnHostPlatform ())
						continue;

					if (MemberHasObsolete (mi))
						continue;

					if (SkipPInvoke (mi))
						continue;

					var libname = dllimport.Value;
					switch (libname) {
					case "__Internal":
					case "System.Native":
					case "System.Security.Cryptography.Native.Apple":
					case "System.Net.Security.Native":
						continue;
					}

					if (SkipLibrary (libname))
						continue;

					string name = dllimport.EntryPoint ?? mi.Name;
					// skip known missing symbols
					switch (name) {
					case "objc_msgSend_stret":
					case "objc_msgSendSuper_stret":
						continue;
					}

					string path = FindLibrary (libname!, requiresFullPath: true);
					IntPtr lib = Dlfcn.dlopen (path, 0);
					bool symbolExists = lib != IntPtr.Zero && Dlfcn.GetIndirect (lib, name) != IntPtr.Zero;
					if (lib != IntPtr.Zero)
						Dlfcn.dlclose (lib);

					bool markedUnavailable = !mi.IsAvailableInSimulator () || !type.IsAvailableInSimulator ();

					if (markedUnavailable && symbolExists) {
						var msg = $"P/Invoke '{type.FullName}.{mi.Name}' (symbol '{name}') is marked as unavailable in the simulator, but the symbol exists in '{path}'.";
						ReportError (msg);
						failed_api.Add ($"{type.FullName}.{mi.Name}");
					} else if (!markedUnavailable && !symbolExists && lib != IntPtr.Zero) {
						// The library exists but the symbol doesn't - might need [UnsupportedSimulator]
						var msg = $"P/Invoke '{type.FullName}.{mi.Name}' (symbol '{name}') is not marked as unavailable in the simulator, but the symbol was not found in '{path}'. Consider adding [UnsupportedSimulator].";
						ReportError (msg);
						failed_api.Add ($"{type.FullName}.{mi.Name}");
					}
					n++;
				}
			}

			AssertIfErrors ("{0} errors found in {1} P/Invoke simulator availability checks: {2}",
				Errors, n, string.Join (", ", failed_api));
		}

		/// <summary>
		/// Verifies that [Field] properties marked as unsupported in the simulator are indeed unavailable.
		/// Also detects [Field] properties not marked as unsupported that are missing from the simulator.
		/// </summary>
		[Test]
		public void FieldSimulatorAvailability ()
		{
			if (!TestRuntime.IsSimulator) {
				Assert.Ignore ("This test only runs in the simulator.");
				return;
			}

			var failed_fields = new List<string> ();
			Errors = 0;
			int n = 0;

			foreach (var type in Assembly.GetTypes ()) {
				if (Skip (type))
					continue;

				if (!type.IsAvailableOnHostPlatform ())
					continue;

				foreach (var p in type.GetProperties (BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)) {
					if (p.CanWrite || !p.CanRead)
						continue;

					var fieldAttr = p.GetCustomAttribute<FieldAttribute> ();
					if (fieldAttr is null)
						continue;

					if (!p.IsAvailableOnHostPlatform ())
						continue;

					if (MemberHasObsolete (p))
						continue;

					if (SkipField (p))
						continue;

					if (fieldAttr.SymbolName is not string symbolName)
						continue;
					string? libName = fieldAttr.LibraryName;

					if (SkipLibrary (libName))
						continue;

					string path = FindLibrary (libName!);
					IntPtr lib = Dlfcn.dlopen (path, 0);
					bool symbolExists = lib != IntPtr.Zero && Dlfcn.GetIndirect (lib, symbolName) != IntPtr.Zero;
					if (lib != IntPtr.Zero)
						Dlfcn.dlclose (lib);

					bool markedUnavailable = !p.IsAvailableInSimulator () || !type.IsAvailableInSimulator ();

					if (markedUnavailable && symbolExists) {
						var msg = $"Field '{type.FullName}.{p.Name}' (symbol '{symbolName}') is marked as unavailable in the simulator, but the symbol exists in '{path}'.";
						ReportError (msg);
						failed_fields.Add ($"{type.FullName}.{p.Name}");
					} else if (!markedUnavailable && !symbolExists && lib != IntPtr.Zero) {
						// The library exists but the symbol doesn't - might need [UnsupportedSimulator]
						var msg = $"Field '{type.FullName}.{p.Name}' (symbol '{symbolName}') is not marked as unavailable in the simulator, but the symbol was not found in '{path}'. Consider adding [UnsupportedSimulator].";
						ReportError (msg);
						failed_fields.Add ($"{type.FullName}.{p.Name}");
					}
					n++;
				}
			}

			AssertIfErrors ("{0} errors found in {1} field simulator availability checks: {2}",
				Errors, n, string.Join (", ", failed_fields));
		}
	}
}
