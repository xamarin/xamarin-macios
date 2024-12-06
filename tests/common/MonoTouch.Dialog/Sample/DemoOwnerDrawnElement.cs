using System;
using System.Drawing;
using CoreFoundation;
using CoreGraphics;
using UIKit;
using Foundation;
using MonoTouch.Dialog;

namespace Sample
{
	public partial class AppDelegate
	{
		private const string SmallText="Lorem ipsum dolor sit amet";
		private const string MediumText = "Integer molestie rhoncus bibendum. Cras ullamcorper magna a enim laoreet";
		private const string LargeText = "Phasellus laoreet, massa non cursus porttitor, sapien tellus placerat metus, vitae ornare urna mi sit amet dui.";
		private const string WellINeverWhatAWhopperString="Nulla mattis tempus placerat. Curabitur euismod ullamcorper lorem. Praesent massa magna, pulvinar nec condimentum ac, blandit blandit mi. Donec vulputate sapien a felis aliquam consequat. Sed sit amet libero non sem rhoncus semper sed at tortor.";		

		
		public void DemoOwnerDrawnElement () 
		{
			var root = new RootElement("Owner Drawn") {
				new Section() {
					new SampleOwnerDrawnElement("000 - "+SmallText, DateTime.Now, "David Black"),
					new SampleOwnerDrawnElement("001 - "+MediumText, DateTime.Now - TimeSpan.FromDays(1), "Peter Brian Telescope"),
					new SampleOwnerDrawnElement("002 - "+LargeText, DateTime.Now - TimeSpan.FromDays(3), "John Raw Vegitable"),
					new SampleOwnerDrawnElement("003 - "+SmallText, DateTime.Now - TimeSpan.FromDays(5), "Tarquin Fintimlinbinwhinbimlim Bus Stop F'tang  F'tang Ole  Biscuit-Barrel"),
					new SampleOwnerDrawnElement("004 - "+WellINeverWhatAWhopperString, DateTime.Now - TimeSpan.FromDays(9), "Kevin Phillips Bong"),
					new SampleOwnerDrawnElement("005 - "+LargeText, DateTime.Now - TimeSpan.FromDays(11), "Alan Jones"),
					new SampleOwnerDrawnElement("006 - "+MediumText, DateTime.Now - TimeSpan.FromDays(32), "Mrs Elsie Zzzzzzzzzzzzzzz"),
					new SampleOwnerDrawnElement("007 - "+SmallText, DateTime.Now - TimeSpan.FromDays(45), "Jeanette Walker"),
					new SampleOwnerDrawnElement("008 - "+MediumText, DateTime.Now - TimeSpan.FromDays(99), "Adrian  Blackpool Rock  Stoatgobblerk"),
					new SampleOwnerDrawnElement("009 - "+SmallText, DateTime.Now - TimeSpan.FromDays(123), "Thomas Moo"),
				}
			};
			root.UnevenRows = true;
			var dvc = new DialogViewController (root, true);
			
			navigation.PushViewController (dvc, true);
		}
	}
	
	
	/// <summary>
	/// This is an example of implementing the OwnerDrawnElement abstract class.
	/// It makes it very simple to create an element that you draw using CoreGraphics
	/// </summary>
	public class SampleOwnerDrawnElement : OwnerDrawnElement
	{
		CGGradient gradient;
		private UIFont subjectFont = UIFont.SystemFontOfSize(10.0f);
		private UIFont fromFont = UIFont.BoldSystemFontOfSize(14.0f);
		private UIFont dateFont = UIFont.BoldSystemFontOfSize(14.0f);
		

		public SampleOwnerDrawnElement (string text, DateTime sent, string from) : base(UITableViewCellStyle.Default, "sampleOwnerDrawnElement")
		{
			this.Subject = text;
			this.From = from;
			this.Sent = FormatDate(sent);
			CGColorSpace colorSpace = CGColorSpace.CreateDeviceRGB();
			gradient = new CGGradient(
			    colorSpace,
			    new nfloat[] { 0.95f, 0.95f, 0.95f, 1, 
							  0.85f, 0.85f, 0.85f, 1},
				new nfloat[] { 0, 1 } );
		}
		
		public string Subject
		{
			get; set; 
		}
		
		public string From
		{
			get; set; 
		}

		public string Sent
		{
			get; set; 
		}
		
		
		public string FormatDate (DateTime date)
		{
	
			if (DateTime.Today == date.Date) {
				return date.ToString ("hh:mm");
			} else if ((DateTime.Today - date.Date).TotalDays < 7) {
				return date.ToString ("ddd hh:mm");
			} else
			{
				return date.ToShortDateString ();			
			}
		}

		const int cellPadding = 10;
		public override void Draw (CGRect bounds, CGContext context, UIView view)
		{
			UIColor.White.SetFill ();
			context.FillRect (bounds);
			
			context.DrawLinearGradient (gradient, new CGPoint (bounds.Left, bounds.Top), new CGPoint (bounds.Left, bounds.Bottom), CGGradientDrawingOptions.DrawsAfterEndLocation);
			
			UIColor.Red.SetColor ();
			nfloat captionHeight = TextHeight (bounds, From, fromFont);
			((NSString) From).DrawString (new CGRect (10, 5, bounds.Width / 2, captionHeight), NSStringDrawingOptions.TruncatesLastVisibleLine | NSStringDrawingOptions.UsesLineFragmentOrigin, new UIStringAttributes ()
			{
				Font = fromFont,
			}, null)
			;
			UIColor.Yellow.SetColor ();
			((NSString) Sent).DrawString (new CGRect (bounds.Width / 2, 5, (bounds.Width / 2) - 10, 10), NSStringDrawingOptions.TruncatesLastVisibleLine | NSStringDrawingOptions.UsesLineFragmentOrigin, new UIStringAttributes ()
			{
				Font = dateFont,
				ParagraphStyle = new NSMutableParagraphStyle ()
				{
					Alignment = UITextAlignment.Right,
				},
			}, null);
			
			UIColor.Green.SetColor();
			((NSString) Subject).DrawString (new CGRect (10, cellPadding + captionHeight, bounds.Width - 20, TextHeight (bounds, Subject, subjectFont)), NSStringDrawingOptions.TruncatesLastVisibleLine | NSStringDrawingOptions.UsesLineFragmentOrigin, new UIStringAttributes ()
			{
				Font = subjectFont,
				ParagraphStyle = new NSMutableParagraphStyle ()
				{
					LineBreakMode = UILineBreakMode.WordWrap,
				},
			}, null);
		}
		
		public override nfloat Height (CGRect bounds)
		{
			var height = TextHeight (bounds, From, fromFont) + TextHeight (bounds, Subject, subjectFont) + cellPadding * 2;
			return height;
		}
		
		private nfloat TextHeight (CGRect bounds, string text, UIFont font)
		{
			using (NSString str = new NSString (text))
			{
				return str.GetBoundingRect (new CGSize (bounds.Width - 20, 1000), NSStringDrawingOptions.UsesLineFragmentOrigin, new UIStringAttributes ()
				{
					Font = font,
					ParagraphStyle = new NSMutableParagraphStyle ()
					{
						LineBreakMode = UILineBreakMode.WordWrap,	
					},
				}, null).Height;
			}
		}
		
		public override string ToString ()
		{
			return string.Format (Subject);
		}
	}
}

