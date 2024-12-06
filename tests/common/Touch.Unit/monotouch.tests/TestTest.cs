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
using System.Threading.Tasks;
using Foundation;
using ObjCRuntime;
using NUnit.Framework;

namespace MonoTouchFixtures.Test {
	
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class Test {

		bool fixture_setup = false;

		[TestFixtureSetUp]
		public void Setup ()
		{
			fixture_setup = true;
		}

		[TestFixtureTearDown]
		public void Teardown ()
		{
			fixture_setup = false;
		}

		[Test]
		public void TestFixtureSetUpCalled ()
		{
			Assert.True (fixture_setup);
		}
		
		[Test]
		public void Ok ()
		{
			Assert.Null (null);
			Assert.True (true);
			Assert.False (false);
		}

		[Test]
		[ExpectedException (typeof (NullReferenceException))]
		public void ExpectedExceptionException ()
		{
			string s = null;
			Assert.That (s.Length, Is.EqualTo (0), "Length");
		}
		
		[Test]
		[Ignore ("don't even execute this one")]
		public void IgnoredByAttribute ()
		{
			throw new NotImplementedException ();
		}

		[Test]
		public void IgnoreInCode ()
		{
			Assert.Ignore ("let's forget about this one");
			throw new NotImplementedException ();
		}

		[Test]
		public void InconclusiveInCode ()
		{
			Assert.Inconclusive ("it did not mean anything");
			throw new NotImplementedException ();
		}
		
		[Test]
		public void FailInCode ()
		{
			Assert.Fail ("that won't do it");
			throw new NotImplementedException ();
		}

		[Test]
		public void PassInCode ()
		{
			Assert.Pass ("good enough");
			throw new NotImplementedException ();
		}

		[Test]
		[ExpectedException (typeof (NullReferenceException))]
		public void IgnoredExpectedException ()
		{
			Assert.Ignore ("ignore [ExpectedException]");
		}

		[Test]
		[ExpectedException (typeof (NullReferenceException))]
		public void InconclusiveExpectedException ()
		{
			Assert.Inconclusive ("inconclusive [ExpectedException]");
		}

		[Test]
		[ExpectedException (typeof (NullReferenceException))]
		public void PassExpectedException ()
		{
			Assert.Pass ("pass [ExpectedException]");
		}

		Task GetException ()
		{
			throw new Exception ();
		}

		[Test]
		[ExpectedException (typeof (Exception))]
		public async void AsyncException ()
		{
			await GetException ();
		}

		[Test]
		[Timeout (Int32.MaxValue)]
		public async Task NestedAsync ()
		{
			await Task.Run (async () => {
				await Task.Delay (1000);
			});
			Assert.Pass ("Delayed");
		}
	}
}