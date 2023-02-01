//
// ActionDispatcher.cs: Helper to map Target-Action to event
//
// Author:
//   Michael Hutchinson <mhutchinson@novell.com>
//
// Copyright 2010, Novell, Inc.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
using System;
using ObjCRuntime;
using Foundation;

#nullable enable

namespace AppKit {
	[Register ("__monomac_internal_ActionDispatcher")]
	internal class ActionDispatcher : NSObject
#if !__MACCATALYST__
		, INSMenuValidation // INSMenuValidation needed for using the Activated method of NSMenuItems if you want to be able to validate
#endif
	{
		const string skey = "__monomac_internal_ActionDispatcher_activated:";
		const string dkey = "__monomac_internal_ActionDispatcher_doubleActivated:";
		public static Selector Action = new Selector (skey);
		public static Selector DoubleAction = new Selector (dkey);
		public EventHandler? Activated;
		public EventHandler? DoubleActivated;
#if !__MACCATALYST__
		public Func<NSMenuItem, bool>? ValidateMenuItemFunc;
#endif // !__MACCATALYST__

		[Preserve, Export (skey)]
		public void OnActivated (NSObject sender)
		{
			var handler = Activated;
			if (handler is not null)
				handler (sender, EventArgs.Empty);
		}

		[Preserve, Export (dkey)]
		public void OnActivated2 (NSObject sender)
		{
			var handler = DoubleActivated;
			if (handler is not null)
				handler (sender, EventArgs.Empty);
		}

		public ActionDispatcher (EventHandler handler)
		{
			IsDirectBinding = false;
			Activated = handler;
		}

		public ActionDispatcher ()
		{
			IsDirectBinding = false;
		}

		public static NSObject SetupAction (NSObject? target, EventHandler handler)
		{
			var ctarget = target as ActionDispatcher;
			if (ctarget is null) {
				ctarget = new ActionDispatcher ();
			}
			ctarget.Activated += handler;
			return ctarget;
		}

		public static void RemoveAction (NSObject? target, EventHandler handler)
		{
			var ctarget = target as ActionDispatcher;
			if (ctarget is null)
				return;
			ctarget.Activated -= handler;
		}

		public static NSObject SetupDoubleAction (NSObject? target, EventHandler doubleHandler)
		{
			var ctarget = target as ActionDispatcher;
			if (ctarget is null) {
				ctarget = new ActionDispatcher ();
			}
			ctarget.DoubleActivated += doubleHandler;
			return ctarget;
		}

		public static void RemoveDoubleAction (NSObject? target, EventHandler doubleHandler)
		{
			var ctarget = target as ActionDispatcher;
			if (ctarget is null)
				return;
			ctarget.DoubleActivated -= doubleHandler;
		}

#if !__MACCATALYST__
		public bool ValidateMenuItem (NSMenuItem menuItem)
		{
			if (ValidateMenuItemFunc is not null)
				return ValidateMenuItemFunc (menuItem);

			return true;
		}
#endif // !__MACCATALYST__

		[Preserve]
		public bool WorksWhenModal {
			[Export ("worksWhenModal")]
			get { return true; }
		}
	}
}
