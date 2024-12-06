// TestElement.cs: MonoTouch.Dialog element for ITest, i.e. TestSuite and TestCase
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2011-2012 Xamarin Inc.
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

using UIKit;

using MonoTouch.Dialog;

using NUnit.Framework.Internal;
#if NUNITLITE_NUGET
using NUnit.Framework.Interfaces;
#else
using NUnit.Framework.Api;
#endif

namespace MonoTouch.NUnit.UI {

	abstract class TestElement : StyledMultilineElement {
		
#if NET
		static internal UIColor DarkGreen = UIDevice.CurrentDevice.CheckSystemVersion (13, 0) ? UIColor.SystemGreen : UIColor.FromRGB (0x00, 0x77, 0x00);
		static internal UIColor Orange = UIDevice.CurrentDevice.CheckSystemVersion (13, 0) ? UIColor.SystemOrange : UIColor.Orange;
		static internal UIColor Red = UIDevice.CurrentDevice.CheckSystemVersion (13, 0) ? UIColor.SystemRed : UIColor.Red;
#else
#pragma warning disable CS0618
		static internal UIColor DarkGreen = UIDevice.CurrentDevice.CheckSystemVersion (13, 0) ? UIColor.SystemGreenColor : UIColor.FromRGB (0x00, 0x77, 0x00);
		static internal UIColor Orange = UIDevice.CurrentDevice.CheckSystemVersion (13, 0) ? UIColor.SystemOrangeColor : UIColor.Orange;
		static internal UIColor Red = UIDevice.CurrentDevice.CheckSystemVersion (13, 0) ? UIColor.SystemRedColor : UIColor.Red;
#pragma warning restore CS0618
#endif
	
		private TestResult result;
		
		public TestElement (ITest test, TouchRunner runner)
			: base ("?", "?", UITableViewCellStyle.Subtitle)
		{
			if (test == null)
				throw new ArgumentNullException ("test");
			if (runner == null)
				throw new ArgumentNullException ("runner");

			if (UIDevice.CurrentDevice.CheckSystemVersion (13, 0)) {
#if NET
				TextColor = UIColor.Label;
#else
#pragma warning disable CS0618
				TextColor = UIColor.LabelColor;
#pragma warning restore CS0618
#endif
			}
			Test = test;
			Runner = runner;
		}

		protected TouchRunner Runner { get; private set; }
		
		public TestResult Result {
			get { return result ?? new TestCaseResult (Test as TestMethod); }
			set { result = value; }
		}
		
		protected ITest Test { get; private set; }
		
		public void TestFinished (TestResult result)
		{
			Result = result;
			
			TestFinished ();
		}
		
		abstract public void TestFinished ();
	}
}
