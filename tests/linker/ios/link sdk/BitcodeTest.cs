using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using ObjCRuntime;

using NUnit.Framework;

namespace LinkSdk {
	[TestFixture]
	public class BitcodeTest {
		[Test]
		public void FilterClauseTest ()
		{
			var supported = true;
#if __WATCHOS__
			if (Runtime.Arch == Arch.DEVICE)
				supported = false;
#endif
			if (supported) {
				Assert.AreEqual (0, FilterClause (), "Filter me");
				Assert.AreEqual (10, FilterClauseProperty, "Filter me getter");
				Assert.DoesNotThrow (() => FilterClauseProperty = 20, "Filter me setter");
			} else {
				Assert.Throws<NotSupportedException> (() => FilterClause (), "Filter me not supported");
				Assert.Throws<NotSupportedException> (() => { var x = FilterClauseProperty; }, "Filter me getter not supported");
				Assert.Throws<NotSupportedException> (() => FilterClauseProperty = 30, "Filter me setter not supported");
			}
		}

		// A method with a filter clause
		// mtouch will show a warning for this method (MT2105) when building for watchOS device. This is expected.
		static int FilterClause ()
		{
			try {
				throw new Exception ("FilterMe");
			} catch (Exception e) when (e.Message == "FilterMe") {
				return 0;
			} catch {
				return 1;
			}
		}

		// A property with a filter clause
		// mtouch will show a warning for this property (MT2105) when building for watchOS device. This is expected.
		static int FilterClauseProperty {
			get {
				try {
					throw new Exception ("FilterMe");
				} catch (Exception e) when (e.Message == "FilterMe") {
					return 10;
				} catch {
					return 11;
				}
			}
			set {
				try {
					throw new Exception ("FilterMe");
				} catch (Exception e) when (e.Message == "FilterMe") {
				} catch {
					Assert.Fail ("Filter failure: {0}", value);
				}
			}
		}

		[Test]
		public void FaultClauseTest ()
		{
#if !DEBUG
			Assert.Ignore ("Only works if IL isn't stripped (i.e. only Debug)");
#endif
			// First assert that the method that is supposed to have a fault clause actually has a fault clause.
			// This is somewhat complicated because it's not the method we call that has the fault clause, but a generated method in a generated class.
			// This is because we only have an indirect way of making csc produce a fault clause
			var enumeratorType = GetType ().GetNestedTypes (BindingFlags.NonPublic).First ((v) => v.Name.Contains ($"<{nameof (FaultClause)}>"));
			var method = enumeratorType.GetMethod ("MoveNext", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
			var body = method.GetMethodBody ();
			Assert.IsTrue (body.ExceptionHandlingClauses.Any ((v) => v.Flags == ExceptionHandlingClauseOptions.Fault), "Any fault clauses");

			// Then assert that the method can be called successfully.
			var rv = FaultClause ().ToArray ();
			Assert.AreEqual (1, rv.Count (), "Count");
			Assert.AreEqual (1, rv [0], "Item 1");

		}

		// The C# compiler uses fault clauses for 'using' blocks inside iterators.
		static IEnumerable<int> FaultClause ()
		{
			using (var d = new Disposable ()) {
				yield return 1;
			}
		}
		class Disposable : IDisposable {
			void IDisposable.Dispose () { }
		}
	}
}
