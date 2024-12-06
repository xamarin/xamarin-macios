//
// Shows how to add/remove cells dynamically.
//

using System;
using UIKit;
using Foundation;
using MonoTouch.Dialog;

namespace Sample {
	public partial class AppDelegate {
		const string longString =
			"Today a major announcement was done in my kitchen, when " +
			"I managed to not burn the onions while on the microwave";

		UIImage badgeImage;

		public void DemoDate ()
		{
			if (badgeImage is null)
				badgeImage = UIImage.FromFile ("jakub-calendar.png");

			var badgeSection = new Section ("Basic Badge Properties"){
				new BadgeElement (badgeImage, "New Movie Day") {
					Font = UIFont.FromName ("Helvetica", 36f)
				},
				new BadgeElement (badgeImage, "Valentine's Day"),

				new BadgeElement (badgeImage, longString) {
					Lines = 3,
					Font = UIFont.FromName ("Helvetica", 12f)
				}
			};

			//
			// Use the MakeCalendarBadge API
			//
			var font = UIFont.FromName ("Helvetica", 14f);
			var dates = new string [] [] {
				new string [] { "January", "1", "Hangover day" },
				new string [] { "February", "14", "Valentine's Day" },
 				new string [] { "March", "3", "Third day of March" },
				new string [] { "March", "31", "Prank Preparation day" },
				new string [] { "April", "1", "Pranks" },
			};
			var calendarSection = new Section ("Date sample");
			foreach (string [] date in dates) {
				calendarSection.Add (new BadgeElement (BadgeElement.MakeCalendarBadge (badgeImage, date [0], date [1]), date [2]) {
					Font = font
				});
			}

			UIImage favorite = UIImage.FromFile ("favorite.png");
			UIImage favorited = UIImage.FromFile ("favorited.png");

			var imageSection = new Section ("Image Booleans"){
				new BooleanImageElement ("Gone with the Wind", true, favorited, favorite),
				new BooleanImageElement ("Policy Academy 38", false, favorited, favorite),
			};

			var messageSection = new Section ("Message Elements"){
				new MessageElement (msgSelected) {
					Sender = "Miguel de Icaza (mdeicaza.home@emailserver.com)",
					Subject = "Re: [Gtk-sharp-list] Glib Timeout and other ways to handle idle",
					Body = "Please bring friends, but make sure that you also bring eggs and bacon as we are running short of those for the coctails tonight",
					Date = DateTime.Now - TimeSpan.FromHours (23),
					NewFlag = true,
					MessageCount = 0
				},
				new MessageElement (msgSelected) {
					Sender = "Nat Friedman (nfriedman.home@emailserver.com)",
					Subject = "Pictures from Vietnam",
					Body = "Hey dude, here are the pictures that I promised from Vietnam",
					Date = new DateTime (2010, 10, 20),
					NewFlag = false,
					MessageCount = 2
				}
			};

			var entrySection = new Section ("Keyboard styles for entry"){
				new EntryElement ("Number ", "Some cute number", "1.2") { KeyboardType = UIKeyboardType.NumberPad },
				new EntryElement ("Email ", "", null) { KeyboardType = UIKeyboardType.EmailAddress },
				new EntryElement ("Url ", "", null) { KeyboardType = UIKeyboardType.Url },
				new EntryElement ("Phone ", "", "1.2") { KeyboardType = UIKeyboardType.PhonePad },
			};

			var root = new RootElement ("Assorted Elements") {
				imageSection,
				messageSection,
				entrySection,
				calendarSection,
				badgeSection,
			};
			var dvc = new DialogViewController (root, true);
			dvc.Style = UITableViewStyle.Plain;

			navigation.PushViewController (dvc, true);
		}

		void msgSelected (DialogViewController dvc, UITableView tv, NSIndexPath path)
		{
			var np = new DialogViewController (new RootElement ("Message Display") {
				new Section () {
					new StyledMultilineElement (
						"From: foo\n" +
						"To: bar\n" +
						"Subject: Hey there\n\n" +
						"This is very simple!")
				}
			}, true);
			dvc.ActivateController (np);
		}
	}

}
