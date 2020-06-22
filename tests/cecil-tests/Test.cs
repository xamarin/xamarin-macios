using System;
using System.Collections.Generic;
using System.IO;

using NUnit.Framework;

using Mono.Cecil;
using Mono.Cecil.Cil;

#nullable enable

namespace Cecil.Tests {

	[TestFixture]
	public class Test {

		[TestCaseSource (typeof(Helper), "PlatformAssemblies")]
		// ref: https://github.com/xamarin/xamarin-macios/pull/7760
		public void IdentifyBackingFieldAssignation (string assemblyPath)
		{
			var assembly = Helper.GetAssembly (assemblyPath);
			if (assembly == null)
				Assert.Ignore ("{assemblyPath} could not be found (might be disabled in build)");
			// look inside all .cctor (static constructor) insde `assemblyName`
			foreach (var m in Helper.FilterMethods (assembly!, (m) => m.IsStatic && m.IsConstructor)) {
				foreach (var ins in m.Body.Instructions) {
					if (ins.OpCode != OpCodes.Stsfld)
						continue;
					if (!(ins.Operand is FieldDefinition f))
						continue;
					var name = f.Name;
					if ((name [0] != '<') || !name.EndsWith (">k__BackingField"))
						continue;
					// filter valid usage
					// it's fine if the returned value is constant (won't ever change during execution)
					// there should be a comment in the source that confirm this behaviour
					switch (m.DeclaringType.FullName) {
					case "CoreFoundation.OSLog":
						if (name == "<Default>k__BackingField")
							break;
						goto default;
					case "Vision.VNUtils":
						if (name == "<NormalizedIdentityRect>k__BackingField")
							break;
						goto default;
					default:
						Assert.Fail ($"Unaudited {m.DeclaringType.FullName} -> {name}");
						break;
					}
				}
			}
		}

		[TestCaseSource (typeof (Helper), "PlatformAssemblies")]
		// ref: https://github.com/xamarin/xamarin-macios/issues/8249
		public void EnsureUIThreadOnInit (string assemblyPath)
		{
			var assembly = Helper.GetAssembly (assemblyPath);
			if (assembly == null) {
				Assert.Ignore ("{assemblyPath} could not be found (might be disabled in build)");
				return; // just to help nullability
			}

			// `CNContactsUserDefaults` is `[ThreadSafe (false)]` and part of iOS and macOS
			var t = assembly.MainModule.GetType ("Contacts.CNContactsUserDefaults");
			if (t == null) {
				// tvOS does not have the type so let's find an alternative
				t = assembly.MainModule.GetType ("PhotosUI.PHLivePhotoView");
			}
			if (t == null) {
				Assert.Fail ($"No type found for {assembly}");
				return; // just to help nullability
			}

			foreach (var c in t.Methods) {
				if (!c.IsConstructor || c.IsStatic || c.HasParameters)
					continue;
				// .ctor(IntPtr)
				var found = false;
				foreach (var ins in c.Body.Instructions) {
					if (ins.OpCode.Code != Code.Call)
						continue;
					found |= (ins.Operand as MethodReference)?.Name == "EnsureUIThread";
				}
				if (!found)
					Assert.Fail ("EnsureUIThread missing");
				else
					return; // single case, no point in iterating anymore
			}
		}

		[TestCaseSource (typeof (Helper), "PlatformAssemblies")]
		public void NoSystemConsoleReference (string assemblyPath)
		{
			if (Path.GetFileName (assemblyPath) == "Xamarin.Mac.dll")
				Assert.Ignore ("Xamarin.Mac has a workaround for Sierra bug w/NSLog");

			var assembly = Helper.GetAssembly (assemblyPath);
			if (assembly == null) {
				Assert.Ignore ("{assemblyPath} could not be found (might be disabled in build)");
				return; // just to help nullability
			}
			// this has a quite noticable impact on (small) app size
			if (assembly.MainModule.TryGetTypeReference ("System.Console", out var _))
				Assert.Fail ($"{assemblyPath} has a reference to `System.Console`. Please use `Runtime.NSLog` inside the platform assemblies");
		}

		// we should not p/invoke into API that are banned (by MS) from the C runtime
		// list is copied from binscope for mac (not all of them actually exists on macOS)
		static HashSet<string> BannedCApi = new HashSet<string> () {
			"_alloca", "_ftcscat", "_ftcscpy", "_getts", "_gettws", "_i64toa",
			"_i64tow", "_itoa", "_itow", "_makepath", "_mbccat", "_mbccpy",
			"_mbscat", "_mbscpy", "_mbslen", "_mbsnbcat", "_mbsnbcpy", "_mbsncat",
			"_mbsncpy", "_mbstok", "_mbstrlen", "_snprintf", "_sntprintf",
			"_sntscanf", "_snwprintf", "_splitpath", "_stprintf", "_stscanf",
			"_tccat", "_tccpy", "_tcscat", "_tcscpy", "_tcsncat", "_tcsncpy",
			"_tcstok", "_tmakepath", "_tscanf", "_tsplitpath", "_ui64toa",
			"_ui64tot", "_ui64tow", "_ultoa", "_ultot", "_ultow", "_vsnprintf",
			"_vsntprintf", "_vsnwprintf", "_vstprintf", "_wmakepath", "_wsplitpath",
			"alloca", "changewindowmessagefilter", "chartooem", "chartooema",
			"chartooembuffa", "chartooembuffw", "chartooemw", "copymemory", "gets",
			"isbadcodeptr", "isbadhugereadptr", "isbadhugewriteptr", "isbadreadptr",
			"isbadstringptr", "isbadwriteptr", "lstrcat", "lstrcata", "lstrcatn",
			"lstrcatna", "lstrcatnw", "lstrcatw", "lstrcpy", "lstrcpya", "lstrcpyn",
			"lstrcpyna", "lstrcpynw", "lstrcpyw", "lstrlen", "lstrncat", "makepath",
			"memcpy", "oemtochar", "oemtochara", "oemtocharw", "rtlcopymemory", "scanf",
			"snscanf", "snwscanf", "sprintf", "sprintfa", "sprintfw", "sscanf", "strcat",
			"strcata", "strcatbuff", "strcatbuffa", "strcatbuffw", "strcatchainw",
			"strcatn", "strcatna", "strcatnw", "strcatw", "strcpy", "strcpya", "strcpyn",
			"strcpyna", "strcpynw", "strcpyw", "strlen", "strncat", "strncata", "strncatw",
			"strncpy", "strncpya", "strncpyw", "strtok", "swprintf", "swscanf", "vsnprintf",
			"vsprintf", "vswprintf", "wcscat", "wcscpy", "wcslen", "wcsncat", "wcsncpy",
			"wcstok", "wmemcpy", "wnsprintf", "wnsprintfa", "wnsprintfw", "wscanf", "wsprintf",
			"wsprintfa", "wsprintfw", "wvnsprintf", "wvnsprintfa", "wvnsprintfw", "wvsprintf",
			"wvsprintfa", "wvsprintfw"
		};

		[TestCaseSource (typeof (Helper), "PlatformAssemblies")]
		public void NoBannedApi (string assemblyPath)
		{
			var assembly = Helper.GetAssembly (assemblyPath);
			if (assembly == null) {
				Assert.Ignore ("{assemblyPath} could not be found (might be disabled in build)");
				return; // just to help nullability
			}
			List<string> found = new List<string> ();
			foreach (var m in Helper.FilterMethods (assembly!, (m) => m.IsPInvokeImpl)) {
				var symbol = m.PInvokeInfo.EntryPoint;
				if (BannedCApi.Contains (symbol))
					found.Add (symbol);
			}
			// if multiple p/invoke are defined then the same symbol will show multiple times
			// it's a feature :)
			Assert.That (found, Is.Empty, string.Join (", ", found));
		}
	}
}