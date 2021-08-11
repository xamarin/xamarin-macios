using System;

using NUnit.Framework;

using Xamarin.Bundler;

namespace Xamarin.Test.Bundler {

	[TestFixture]
	public class ErrorTest {

		[Test]
		public void Ctor2 ()
		{
			var e = new ProductException (0, "valid");
			Assert.That (e.ToString (), Is.EqualTo ($"warning {ProductException.Prefix}0000: valid"), "ToString-1");

			e = new ProductException (9999, "valid") {
				FileName = "here.cs",
				LineNumber = 42
			};
			Assert.That (e.ToString (), Is.EqualTo ($"here.cs(42): warning {ProductException.Prefix}9999: valid"), "ToString-2");
		}

		[Test]
		public void Ctor3 ()
		{
			var e = new ProductException (0, "valid", new string [] { });
			Assert.That (e.ToString (), Is.EqualTo ($"warning {ProductException.Prefix}0000: valid"), "ToString-1");

			e = new ProductException (10, "valid {0}", "output");
			Assert.That (e.ToString (), Is.EqualTo ($"warning {ProductException.Prefix}0010: valid output"), "ToString-2");

			e = new ProductException (9999, "valid", new string [] { }) {
				FileName = "here.cs",
				LineNumber = 42
			};
			Assert.That (e.ToString (), Is.EqualTo ($"here.cs(42): warning {ProductException.Prefix}9999: valid"), "ToString-3");

			e = new ProductException (9999, "valid {0}", 1) {
				FileName = "there.cs",
				LineNumber = 911
			};
			Assert.That (e.ToString (), Is.EqualTo ($"there.cs(911): warning {ProductException.Prefix}9999: valid 1"), "ToString-4");
		}

		[Test]
		public void Ctor4 ()
		{
			var e = new ProductException (0, true, "valid", new string [] { });
			Assert.That (e.ToString (), Is.EqualTo ($"error {ProductException.Prefix}0000: valid"), "ToString-1");

			e = new ProductException (10, true, "valid {0}", "output");
			Assert.That (e.ToString (), Is.EqualTo ($"error {ProductException.Prefix}0010: valid output"), "ToString-2");

			e = new ProductException (9999, true, "valid", new string [] { }) {
				FileName = "here.cs",
				LineNumber = 42
			};
			Assert.That (e.ToString (), Is.EqualTo ($"here.cs(42): error {ProductException.Prefix}9999: valid"), "ToString-32");

			e = new ProductException (9999, true, "valid {0}", 1) {
				FileName = "there.cs",
				LineNumber = 911
			};
			Assert.That (e.ToString (), Is.EqualTo ($"there.cs(911): error {ProductException.Prefix}9999: valid 1"), "ToString-4");
		}

		[Test]
		public void Ctor5 ()
		{
			var e = new ProductException (0, true, null, "valid", new string [] { });
			Assert.That (e.ToString (), Is.EqualTo ($"error {ProductException.Prefix}0000: valid"), "ToString-1");

			e = new ProductException (10, true, new Exception ("uho"), "valid {0}", "output");
			Assert.That (e.ToString (), Is.EqualTo ($"error {ProductException.Prefix}0010: valid output"), "ToString-2");

			e = new ProductException (9999, true, new NotFiniteNumberException (), "valid", new string [] { }) {
				FileName = "here.cs",
				LineNumber = 42
			};
			Assert.That (e.ToString (), Is.EqualTo ($"here.cs(42): error {ProductException.Prefix}9999: valid"), "ToString-32");

			e = new ProductException (9999, true, new ObjectDisposedException ("uho"), "valid {0}", 1) {
				FileName = "there.cs",
				LineNumber = 911
			};
			Assert.That (e.ToString (), Is.EqualTo ($"there.cs(911): error {ProductException.Prefix}9999: valid 1"), "ToString-4");
		}

		[Test]
		public void BadFormats ()
		{
			// {0} without argument - not using the `args` overload -> no exception
			var e = new ProductException (0, true, null, "valid {0}");
			Assert.That (e.ToString (), Is.EqualTo ($"error {ProductException.Prefix}0000: valid {{0}}"), "ToString-0");

			// {0} without argument
			e = new ProductException (0, true, null, "invalid {0}", null);
			Assert.That (e.ToString (), Is.EqualTo ($"error {ProductException.Prefix}0000: invalid {{0}}. String.Format failed! Arguments were:. Please file an issue to report this incorrect error handling."), "ToString-1");

			// {0} with 2 arguments -> no exception
			e = new ProductException (10, "valid {0}", 1, 2);
			Assert.That (e.ToString (), Is.EqualTo ($"warning {ProductException.Prefix}0010: valid 1"), "ToString-2");

			// {0} {1} with 1 argument
			e = new ProductException (10, true, new Exception ("uho"), "invalid {0} {1}", 1);
			Assert.That (e.ToString (), Is.EqualTo ($"error {ProductException.Prefix}0010: invalid {{0}} {{1}}. String.Format failed! Arguments were: \"1\". Please file an issue to report this incorrect error handling."), "ToString-3");
		}
	}
}
