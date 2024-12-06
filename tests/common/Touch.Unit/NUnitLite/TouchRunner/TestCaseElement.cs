// TestCaseElement.cs: MonoTouch.Dialog element for TestCase
//
// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2011-2013 Xamarin Inc.
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
using System.Reflection;

using UIKit;

using MonoTouch.Dialog;

using NUnit.Framework;
using NUnit.Framework.Internal;
#if NUNITLITE_NUGET
using NUnit.Framework.Interfaces;
#else
using NUnit.Framework.Api;
#endif

namespace MonoTouch.NUnit.UI {
	
	class TestCaseElement : TestElement {
		bool tapped;
		
		public TestCaseElement (TestMethod testCase, TouchRunner runner)
			: base (testCase, runner)
		{
			Caption = testCase.Name;
			Value = "NotExecuted";
			this.Tapped += delegate {
				if (!Runner.OpenWriter (Test.FullName))
					return;

#if NUNITLITE_NUGET
				Run ();
#else
				var suite = (testCase.Parent as TestSuite);
				var context = TestExecutionContext.CurrentContext;
				context.TestObject = Reflect.Construct (testCase.Method.ReflectedType, null);

				suite.GetOneTimeSetUpCommand ().Execute (context);
				Run ();
				suite.GetOneTimeTearDownCommand ().Execute (context);
#endif

				Runner.CloseWriter ();
				tapped = true;
			};
		}
		
		public TestMethod TestCase {
			get { return Test as TestMethod; }
		}
		
		public void Run ()
		{
			Runner.Run (TestCase);
		}
		
		public override void TestFinished ()
		{
			if (Result.IsIgnored ()) {
				Value = Result.GetMessage ();
				DetailColor = Orange;
			} else if (Result.IsSuccess () || Result.IsInconclusive ()) {
				int counter = Result.AssertCount;
				Value = String.Format ("{0} {1} ms for {2} assertion{3}",
					Result.IsInconclusive () ? "Inconclusive." : "Success!",
					Result.GetDuration ().TotalMilliseconds, counter,
					counter == 1 ? String.Empty : "s");
				DetailColor = DarkGreen;
			} else if (Result.IsFailure ()) {
				Value = Result.GetMessage ();
				DetailColor = Red;
			} else {
				// Assert.Ignore falls into this
				Value = Result.GetMessage ();
			}

			if (tapped) {
				// display more details on (any) failure (but not when ignored)
				if ((TestCase.RunState == RunState.Runnable) && !Result.IsSuccess ()) {
					var root = new RootElement ("Results") {
						new Section () {
							new TestResultElement (Result)
						}
					};
					var dvc = new DialogViewController (root, true) { Autorotate = true };
					Runner.NavigationController.PushViewController (dvc, true);
				}
				// we still need to update our current element
				if (GetContainerTableView () != null) {
					var root = GetImmediateRootElement ();
					root.Reload (this, UITableViewRowAnimation.Fade);
				}
				tapped = false;
			}
		}
	}
}
