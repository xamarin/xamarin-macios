using System;
using NUnit.Framework;

using AppKit;
using ObjCRuntime;
using Foundation;

namespace Xamarin.Mac.Tests
{
	public class NSSearchFieldTests
	{
		[Test]
		public void NSSearchFieldShouldSetSearchMenuTemplate ()
		{
			if (PlatformHelper.ToMacVersion (PlatformHelper.GetHostApiPlatform ()) < Platform.Mac_10_10)
				return;

			var searchField = new NSSearchField ();
			var searchMenuTemplate = searchField.SearchMenuTemplate;
			searchField.SearchMenuTemplate = new NSMenu ("Test");

			Assert.IsTrue (searchField.SearchMenuTemplate != searchMenuTemplate, "NSSearchFieldShouldSetSearchMenuTemplate - Failed to set the SearchMenuTemplate property.");
		}

		[Test]
		public void NSSearchFieldShouldSetSendsWholeSearchString ()
		{
			if (PlatformHelper.ToMacVersion (PlatformHelper.GetHostApiPlatform ()) < Platform.Mac_10_10)
				return;

			var searchField = new NSSearchField ();
			var sendsWholeSearchString = searchField.SendsWholeSearchString;
			searchField.SendsWholeSearchString = !sendsWholeSearchString;

			Assert.IsTrue (searchField.SendsWholeSearchString != sendsWholeSearchString, "NSSearchFieldShouldSetSendsWholeSearchString - Failed to set the SendsWholeSearchString property.");
		}

		[Test]
		public void NSSearchFieldShouldSetMaximumRecents ()
		{
			if (PlatformHelper.ToMacVersion (PlatformHelper.GetHostApiPlatform ()) < Platform.Mac_10_10)
				return;

			var searchField = new NSSearchField ();
			var maximumRecents = searchField.MaximumRecents;
			searchField.MaximumRecents = maximumRecents + 3;

			Assert.IsTrue (searchField.MaximumRecents != maximumRecents, "NSSearchFieldShouldSetMaximumRecents - Failed to set the MaximumRecents property.");
		}

		[Test]
		public void NSSearchFieldShouldSetSendsSearchStringImmediately ()
		{
			if (PlatformHelper.ToMacVersion (PlatformHelper.GetHostApiPlatform ()) < Platform.Mac_10_10)
				return;

			var searchField = new NSSearchField ();
			var sendsSearchStringImmediately = searchField.SendsSearchStringImmediately;
			searchField.SendsSearchStringImmediately = !sendsSearchStringImmediately;

			Assert.IsTrue (searchField.SendsSearchStringImmediately != sendsSearchStringImmediately, "NSSearchFieldShouldSetSendsSearchStringImmediately - Failed to set the SendsSearchStringImmediately property.");
		}
	}
}

