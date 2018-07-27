//
// Base test fixture for introspection tests
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2012-2016 Xamarin Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using NUnit.Framework;
using Xamarin.Utils;
using System.Linq;
using Xamarin.Tests;

#if XAMCORE_2_0
using Foundation;
using ObjCRuntime;
#if MONOTOUCH
using UIKit;
#endif
#else
#if MONOMAC
using MonoMac.Foundation;
using MonoMac.ObjCRuntime;
#else
using MonoTouch.Foundation;
using MonoTouch.UIKit;
#endif
#endif

namespace Introspection {

	public abstract class ApiBaseTest {
		[DllImport ("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		protected static extern bool bool_objc_msgSend_IntPtr (IntPtr receiver, IntPtr selector, IntPtr arg1);
		[DllImport ("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
		protected static extern IntPtr IntPtr_objc_msgSend (IntPtr receiver, IntPtr selector);

		private LatchedEnvironmentVariable continueOnFailure = new LatchedEnvironmentVariable ("API_TEST_CONTINUE_ON_FAILURE");

		StringBuilder error_output = new StringBuilder ();

		protected ApiBaseTest ()
		{
			//LogProgress = true;
			//ContinueOnFailure = true;
		}

		protected void AddErrorLine (string line)
		{
			error_output.AppendLine (line);
			if (!line.StartsWith ("[FAIL] ", StringComparison.Ordinal))
				Console.Error.Write ("[FAIL] ");
			Console.Error.WriteLine (line);
			Errors++;
		}

		protected void AddErrorLine (string format, params object[] parameters)
		{
			AddErrorLine (string.Format (format, parameters));
		}

		/// <summary>
		/// Gets or sets a value indicating whether this test fixture will continue after failures.
		/// </summary>
		/// <value>
		/// <c>true</c> if continue on failure; otherwise, <c>false</c>.
		/// </value>
		public bool ContinueOnFailure { 
			get { return continueOnFailure.Value; }
			set { continueOnFailure.Value = value; }
		}


		private LatchedEnvironmentVariable logProgress = new LatchedEnvironmentVariable ("API_TEST_LOG_PROGRESS");

		/// <summary>
		/// Gets or sets a value indicating whether this test fixture will log it's progress.
		/// </summary>
		/// <value>
		/// <c>true</c> if log progress; otherwise, <c>false</c>.
		/// </value>
		public bool LogProgress {
			get { return logProgress.Value; }
			set { logProgress.Value = value; }
		}

		StringBuilder error_data;
		protected StringBuilder ErrorData {
			get {
				return error_data ?? (error_data = new StringBuilder ());
			}
		}

		protected TextWriter Writer {
#if MONOMAC
			get { return Console.Out; }
#elif __WATCHOS__
			get { return Console.Out; }
#else
			get { return AppDelegate.Runner.Writer; }
#endif
		}

		protected int Errors;
		protected void ReportError (string s, params object [] parameters)
		{
			if (!ContinueOnFailure)
				Assert.Fail (s, parameters);
			else {
				Writer.Write ("[FAIL] ");
				Writer.WriteLine (s, parameters);
				ErrorData.AppendFormat (s, parameters).AppendLine ();
				Errors++;
			}
		}

		protected void AssertIfErrors (string s, params object[] parameters)
		{
			if (Errors == 0)
				return;

			var msg = string.Format (s, parameters);
			if (error_output.Length > 0) {
				msg += "\n" + error_output.ToString () + "\n";
				error_output.Clear ();
			}
			Assert.Fail (msg);
		}
			
		static protected Type NSObjectType = typeof (NSObject);

		protected virtual bool Skip (Attribute attribute)
		{
			return false;
		}

		protected virtual bool SkipDueToAttribute (MemberInfo member)
		{
			if (member == null)
				return false;

			return !member.IsAvailableOnHostPlatform () ||
				          SkipDueToAttribute (member.DeclaringType) ||
				          SkipDueToAttributeInProperty (member);
		}

		// We need to check Availability info on PropertyInfo attributes too
		// due to sometimes the Availability info only exist on the property
		// and not on the property Getter or Setter, this complements the fix for bug:
		// https://bugzilla.xamarin.com/show_bug.cgi?id=35176
		protected bool SkipDueToAttributeInProperty (MemberInfo member)
		{
			if (member == null)
				return false;

			var m = member as MethodInfo;

			if (m == null || // Skip anything that is not a method
			    !m.Attributes.HasFlag (MethodAttributes.SpecialName)) // We want properties with SpecialName Attribute
				return false;

			// FIXME: In the future we could cache this to reduce memory requirements
			var property = m.DeclaringType
			                .GetProperties ()
			                .SingleOrDefault (p => p.GetGetMethod () == m || p.GetSetMethod () == m);
			return property != null && SkipDueToAttribute (property);
		}

		/// <summary>
		/// Gets the assembly on which the test fixture will reflect the NSObject-derived types.
		/// The default implementation returns the assembly where NSObject is defined, e.g.
		/// monotouch.dll or xammac.dll. 
		/// You need to override this method to return the binding assembly you wish to test.
		/// </summary>
		/// <value>
		/// The assembly on which the fixture will execute it's tests.
		/// </value>
		protected virtual Assembly Assembly {
			get { return NSObjectType.Assembly; }
		}

		const string libprefix = "/System/Library/Frameworks";
		static readonly string simprefix = Environment.GetEnvironmentVariable ("IPHONE_SIMULATOR_ROOT");

		protected virtual string FindLibrary (string libname, bool requiresFullPath = false)
		{
			string prefix;
			if (!String.IsNullOrEmpty (simprefix) && libname.StartsWith (libprefix, StringComparison.Ordinal)) {
				libname = simprefix + libname;
				prefix = String.Empty;
			} else {
				prefix = libprefix; // re-root libname
			}

			switch (libname) {
#if !MONOMAC
			case "AudioUnit":
				libname = "AudioToolbox";
				break;
			case "IOSurface":
				if (!TestRuntime.CheckXcodeVersion (9, 0))
					prefix = Path.Combine (Path.GetDirectoryName (prefix), "PrivateFrameworks");
				break;
			case "PdfKit":
				libname = "PDFKit";
				break;
#endif
			case "CoreAnimation":
				// generated code uses QuartzCore correctly - even if the [Field] property is wrong
				libname = "QuartzCore";
				break;
			case "CoreMidi":
				// generated code uses CoreMIDI correctly
				libname = "CoreMIDI";
				break;
			default:
				if (requiresFullPath && (Path.GetDirectoryName (libname).Length == 0))
					ReportError ("[FAIL] Library '{0}' is specified without a path", libname);
				break;
			}

			return Path.Combine (prefix, libname + ".framework", libname); 
		}
	}
}
