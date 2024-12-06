using System;
using System.Drawing;

using UIKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;

namespace MonoTouch.Dialog {

	public partial class MessageSummaryView : UIView {
		static UIFont SenderFont = UIFont.BoldSystemFontOfSize (19);
		static UIFont SubjectFont = UIFont.SystemFontOfSize (14);
		static UIFont TextFont = UIFont.SystemFontOfSize (13);
		static UIFont CountFont = UIFont.BoldSystemFontOfSize (13);
		public string Sender { get; private set; }
		public string Body { get; private set; }
		public string Subject { get; private set; }
		public DateTime Date { get; private set; }
		public bool NewFlag { get; private set; }
		public int MessageCount { get; private set; }

		static CGGradient gradient;

		static MessageSummaryView ()
		{
			using (var colorspace = CGColorSpace.CreateDeviceRGB ()) {
				gradient = new CGGradient (colorspace, new nfloat [] { /* first */ .52f, .69f, .96f, 1, /* second */ .12f, .31f, .67f, 1 }, null); //new float [] { 0, 1 });
			}
		}

		public MessageSummaryView ()
		{
			BackgroundColor = UIColor.White;
		}

		public void Update (string sender, string body, string subject, DateTime date, bool newFlag, int messageCount)
		{
			Sender = sender;
			Body = body;
			Subject = subject;
			Date = date;
			NewFlag = newFlag;
			MessageCount = messageCount;
			SetNeedsDisplay ();
		}

		public override void Draw (CGRect rect)
		{
			const int padright = 21;
			var ctx = UIGraphics.GetCurrentContext ();
			nfloat boxWidth;
			CGSize ssize;

			if (MessageCount > 0) {
				var ms = MessageCount.ToString ();
				ssize = ms.StringSize (CountFont);
				boxWidth = (nfloat) Math.Min (22 + ssize.Width, 18);
				var crect = new CGRect (Bounds.Width - 20 - boxWidth, 32, boxWidth, 16);

				UIColor.Gray.SetFill ();
				GraphicsUtil.FillRoundedRect (ctx, crect, 3);
				UIColor.White.SetColor ();
				crect.X += 5;
				ms.DrawString (crect, CountFont);

				boxWidth += padright;
			} else
				boxWidth = 0;

			UIColor.FromRGB (36, 112, 216).SetColor ();
			var diff = DateTime.Now - Date;
			var now = DateTime.Now;
			string label;
			if (now.Day == Date.Day && now.Month == Date.Month && now.Year == Date.Year)
				label = Date.ToShortTimeString ();
			else if (diff <= TimeSpan.FromHours (24))
				label = "Yesterday".GetText ();
			else if (diff < TimeSpan.FromDays (6))
				label = Date.ToString ("dddd");
			else
				label = Date.ToShortDateString ();
			ssize = label.StringSize (SubjectFont);
			nfloat dateSize = ssize.Width + padright + 5;
			label.DrawString (new CGRect (Bounds.Width - dateSize, 6, dateSize, 14), SubjectFont, UILineBreakMode.Clip, UITextAlignment.Left);

			const int offset = 33;
			nfloat bw = Bounds.Width - offset;

			UIColor.Black.SetColor ();
			Sender.DrawString (new CGPoint (offset, 2), (float) (bw - dateSize), SenderFont, UILineBreakMode.TailTruncation);
			Subject.DrawString (new CGPoint (offset, 23), (float) (bw - offset - boxWidth), SubjectFont, UILineBreakMode.TailTruncation);

			//UIColor.Black.SetFill ();
			//ctx.FillRect (new CGRect (offset, 40, bw-boxWidth, 34));
			UIColor.Gray.SetColor ();
			Body.DrawString (new CGRect (offset, 40, bw - boxWidth, 34), TextFont, UILineBreakMode.TailTruncation, UITextAlignment.Left);

			if (NewFlag) {
				ctx.SaveState ();
				ctx.AddEllipseInRect (new CGRect (10, 32, 12, 12));
				ctx.Clip ();
				ctx.DrawLinearGradient (gradient, new CGPoint (10, 32), new CGPoint (22, 44), CGGradientDrawingOptions.DrawsAfterEndLocation);
				ctx.RestoreState ();
			}

#if WANT_SHADOWS
			ctx.SaveState ();
			UIColor.FromRGB (78, 122, 198).SetStroke ();
			ctx.SetShadow (new CGSize (1, 1), 3);
			ctx.StrokeEllipseInRect (new CGRect (10, 32, 12, 12));
			ctx.RestoreState ();
#endif
		}
	}

	public class MessageElement : Element, IElementSizing {
		static NSString mKey = new NSString ("MessageElement");

		public string Sender, Body, Subject;
		public DateTime Date;
		public bool NewFlag;
		public int MessageCount;

		class MessageCell : UITableViewCell {
			MessageSummaryView view;

			public MessageCell () : base (UITableViewCellStyle.Default, mKey)
			{
				view = new MessageSummaryView ();
				ContentView.Add (view);
				Accessory = UITableViewCellAccessory.DisclosureIndicator;
			}

			public void Update (MessageElement me)
			{
				view.Update (me.Sender, me.Body, me.Subject, me.Date, me.NewFlag, me.MessageCount);
			}

			public override void LayoutSubviews ()
			{
				base.LayoutSubviews ();
				view.Frame = ContentView.Bounds;
				view.SetNeedsDisplay ();
			}
		}

		public MessageElement () : base ("")
		{
		}

		public MessageElement (Action<DialogViewController, UITableView, NSIndexPath> tapped) : base ("")
		{
			Tapped += tapped;
		}

		public override UITableViewCell GetCell (UITableView tv)
		{
			var cell = tv.DequeueReusableCell (mKey) as MessageCell;
			if (cell is null)
				cell = new MessageCell ();
			cell.Update (this);
			return cell;
		}

		public nfloat GetHeight (UITableView tableView, NSIndexPath indexPath)
		{
			return 78;
		}

		public event Action<DialogViewController, UITableView, NSIndexPath> Tapped;

		public override void Selected (DialogViewController dvc, UITableView tableView, NSIndexPath path)
		{
			if (Tapped is not null)
				Tapped (dvc, tableView, path);
		}

		public override bool Matches (string text)
		{
			if (Sender is not null && Sender.IndexOf (text, StringComparison.CurrentCultureIgnoreCase) != -1)
				return true;
			if (Body is not null && Body.IndexOf (text, StringComparison.CurrentCultureIgnoreCase) != -1)
				return true;
			if (Subject is not null && Subject.IndexOf (text, StringComparison.CurrentCultureIgnoreCase) != -1)
				return true;

			return false;
		}
	}
}

