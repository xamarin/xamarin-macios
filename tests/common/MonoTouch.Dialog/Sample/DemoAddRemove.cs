//
// Shows how to add/remove cells dynamically.
//

using System;
using UIKit;
using MonoTouch.Dialog;

namespace Sample
{
	public partial class AppDelegate
	{
		RootElement demoRoot;
		Section region;
		Random rnd;
		int count;		
		
		public void DemoAddRemove ()
		{
			rnd = new Random ();
			var section = new Section (null, "Elements are added randomly") {
				new StringElement ("Add elements", AddElements),
				new StringElement ("Add, with no animation", AddNoAnimation),
				new StringElement ("Remove top element", RemoveElements),
				new StringElement ("Add Section", AddSection),
				new StringElement ("Remove Section", RemoveSection)
			};
			region = new Section ();
			
			demoRoot = new RootElement ("Add/Remove Demo") { section, region };
			var dvc = new DialogViewController (demoRoot, true);
			navigation.PushViewController (dvc, true);
		}
		
		void AddElements ()
		{
			region.Insert (rnd.Next (0, region.Elements.Count),
			               UITableViewRowAnimation.Fade,
			               new StringElement ("Ding " + count++),
			               new StringElement ("Dong " + count++));	               
		}
		
		void AddNoAnimation ()
		{
			region.Add (new StringElement ("Insertion not animated"));
		}
		
		void RemoveElements ()
		{
			region.RemoveRange (0, 1);
		}	
		
		void AddSection ()
		{
			var section = new Section () {
				new StringElement ("Demo Section Added")
			};
			
			demoRoot.Insert (demoRoot.Count, section);
		}
		
		void RemoveSection ()
		{
			// Do not delete the top (our buttons) or the second (where the buttons add stuff)
			if (demoRoot.Count == 2)
				return;
		
			demoRoot.RemoveAt (demoRoot.Count-1);
		}
	}
}
