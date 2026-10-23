#if __MACOS__

using AppKit;

namespace Xamarin.Mac.Tests {
	[Preserve (AllMembers = true)]
	public class NSTextViewTests {
		NSTextView view;

		[SetUp]
		public void SetUp ()
		{
			view = new NSTextView ();
		}

		[Test]
		public void StandardKeyBindingResponding ()
		{
			using var textView = new NSTextView ();
			INSStandardKeyBindingResponding responder = textView;

			textView.InsertNewlineIgnoringFieldEditor (null);
			Assert.That (textView.Value, Is.EqualTo ("\n"), "Newline");

			using var text = new NSString ("text");
			responder.InsertText (text);
			Assert.That (textView.Value, Is.EqualTo ("\ntext"), "InsertText");

			var commandCount = 0;
			NSTextViewSelectorCommand handler = (sender, selector) => {
				commandCount++;
				return true;
			};
			textView.DoCommandBySelector += handler;
			responder.DoCommandBySelector (new ObjCRuntime.Selector ("insertNewline:"));
			textView.DoCommandBySelector -= handler;
			Assert.That (commandCount, Is.EqualTo (1), "Command handler");
			Assert.That (textView.Value, Is.EqualTo ("\ntext"), "Handled command");
		}

		[Test]
		public void NSTextViewShouldChangeUsesRolloverButtonForSelection ()
		{
			var usesRollover = view.UsesRolloverButtonForSelection;
			view.UsesRolloverButtonForSelection = !usesRollover;

			Assert.That (view.UsesRolloverButtonForSelection == usesRollover, Is.False, "NSTextViewShouldChangeUsesRolloverButtonForSelection - Failed to set the UsesRolloverButtonForSelection property");
		}
	}
}
#endif // __MACOS__
