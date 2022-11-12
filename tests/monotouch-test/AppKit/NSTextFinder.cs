#if __MACOS__
using System;
using System.Threading.Tasks;
using NUnit.Framework;

using AppKit;
using AudioUnit;
using AudioToolbox;
using Foundation;

namespace Xamarin.Mac.Tests {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NSTextFinderTests {
		[Test]
		public void NSTextFinderConstructor ()
		{
			NSTextFinder f = new NSTextFinder ();
			Assert.IsNotNull (f);

			FinderClient client = new FinderClient ();
			f.Client = client;
		}

#if NET
		class FinderClient : NSObject, INSTextFinderClient
#else
		class FinderClient : NSTextFinderClient
#endif
		{
#if NET
			public bool AllowsMultipleSelection { get { return true; } }

			public bool Editable { get { return true; } }

			public string String { get { return "Testing One Two Three"; } }

			public NSRange FirstSelectedRange { get { return new NSRange (); } }

			public NSArray SelectedRanges { get; set; }

			public NSArray VisibleCharacterRanges { get { return new NSArray (); } }

			public bool Selectable { get { return true; } }
#else
			public override bool AllowsMultipleSelection { get { return true; } }

			public override bool Editable { get { return true; } }

			public override string String { get { return "Testing One Two Three"; } }

			public override NSRange FirstSelectedRange { get { return new NSRange(); } }

			public override NSArray SelectedRanges { get; set; }

			public override NSArray VisibleCharacterRanges { get { return new NSArray(); } }

			public override bool Selectable { get { return true; } }
#endif

#if NET
			public string GetString (nuint characterIndex, out NSRange outRange, bool outFlag)
#else
			public override string StringAtIndexeffectiveRangeendsWithSearchBoundary (nuint characterIndex, ref NSRange outRange, bool outFlag)
#endif
			{
				outRange = default (NSRange);
				return String;
			}

#if NET
			public nuint StringLength {
				get {
					return (nuint) String.Length;
				}
			}
#else
			public override nuint StringLength ()
			{
				return (nuint)String.Length;
			}
#endif

#if NET
			public void ScrollRangeToVisible (NSRange range)
#else
			public override void ScrollRangeToVisible (NSRange range)
#endif
			{
			}

#if NET
			public bool ShouldReplaceCharacters (NSArray ranges, NSArray strings)
#else
			public override bool ShouldReplaceCharactersInRangeswithStrings (NSArray ranges, NSArray strings)
#endif
			{
				return false;
			}

#if NET
			public void ReplaceCharacters (NSRange range, string str)
#else
			public override void ReplaceCharactersInRangewithString (NSRange range, string str)
#endif
			{
			}

#if NET
			public void DidReplaceCharacters ()
#else
			public override void DidReplaceCharacters ()
#endif
			{
			}

#if NET
			public NSView GetContentView (nuint index, out NSRange outRange)
#else
			public override NSView ContentViewAtIndexeffectiveCharacterRange (nuint index, ref NSRange outRange)
#endif
			{
				outRange = default (NSRange);
				return null;
			}

#if NET
			public NSArray GetRects (NSRange characterRange)
#else
			public override NSArray RectsForCharacterRange (NSRange range)
#endif
			{
				return null;
			}

#if NET
			public void DrawCharacters (NSRange range, NSView view)
#else
			public override void DrawCharactersInRangeforContentView (NSRange range, NSView view)
#endif
			{
			}
		}
	}
}

#endif // __MACOS__
