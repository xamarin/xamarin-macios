using System;
using System.Threading.Tasks;
using NUnit.Framework;

using AppKit;
using AudioUnit;
using AudioToolbox;
using Foundation;

namespace Xamarin.Mac.Tests
{
	[TestFixture]
	public class NSTextFinderTests
	{
		[Test]
		public void NSTextFinderConstructor ()
		{
			NSTextFinder f = new NSTextFinder ();
			Assert.IsNotNull (f);

			FinderClient client = new FinderClient ();
			f.Client = client;
		}

		class FinderClient : NSTextFinderClient
		{
			public override bool AllowsMultipleSelection { get { return true; } }

			public override bool Editable { get { return true; } }

			public override string String { get { return "Testing One Two Three"; } }

			public override NSRange FirstSelectedRange { get { return new NSRange(); } }

			public override NSArray SelectedRanges { get; set; }

			public override NSArray VisibleCharacterRanges { get { return new NSArray(); } }

			public override bool Selectable { get { return true; } }

			public override string StringAtIndexeffectiveRangeendsWithSearchBoundary (nuint characterIndex, ref NSRange outRange, bool outFlag)
			{
				return String;
			}

			public override nuint StringLength ()
			{
				return (nuint)String.Length;
			}

			public override void ScrollRangeToVisible (NSRange range)
			{
			}

			public override bool ShouldReplaceCharactersInRangeswithStrings (NSArray ranges, NSArray strings)
			{
				return false;
			}

			public override void ReplaceCharactersInRangewithString (NSRange range, string str)
			{
			}

			public override void DidReplaceCharacters ()
			{
			}

			public override NSView ContentViewAtIndexeffectiveCharacterRange (nuint index, ref NSRange outRange)
			{
				return null;
			}

			public override NSArray RectsForCharacterRange (NSRange range)
			{
				return null;
			}

			public override void DrawCharactersInRangeforContentView (NSRange range, NSView view)
			{
			}
		}
	}
}

