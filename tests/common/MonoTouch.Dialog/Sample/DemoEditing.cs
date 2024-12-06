using System;
using UIKit;
using Foundation;
using MonoTouch.Dialog;
using System.Threading;

namespace Sample
{
	// 
	// To support editing, you need to subclass the DialogViewController and provide
	// your own "Source" classes (which you also have to subclass).
	//
	// There are two source classes, a sizing one (for elements that have different
	// sizes, and one for fixed element sizes.   Since we are just using strings,
	// we are going to take a shortcut and just implement one of the sources.
	//
	public class EditingDialog : DialogViewController {
		
		// This is our subclass of the fixed-size Source that allows editing
		public class EditingSource : DialogViewController.Source {
			public EditingSource (DialogViewController dvc) : base (dvc) {}
			
			public override bool CanEditRow (UITableView tableView, NSIndexPath indexPath)
			{
				// Trivial implementation: we let all rows be editable, regardless of section or row
				return true;
			}
			
			public override UITableViewCellEditingStyle EditingStyleForRow (UITableView tableView, NSIndexPath indexPath)
			{
				// trivial implementation: show a delete button always
				return UITableViewCellEditingStyle.Delete;
			}
			
			public override void CommitEditingStyle (UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
			{
				//
				// In this method, we need to actually carry out the request
				//
				var section = Container.Root [indexPath.Section];
				var element = section [indexPath.Row];
				section.Remove (element);
			}
		}
		
		public override Source CreateSizingSource (bool unevenRows)
		{
			if (unevenRows)
				throw new NotImplementedException ("You need to create a new SourceSizing subclass, this sample does not have it");
			return new EditingSource (this);
		}
		
		public EditingDialog (RootElement root, bool pushing) : base (root, pushing)
		{
		}
	}
		
	public partial class AppDelegate
	{

		void ConfigEdit (DialogViewController dvc)
		{
			dvc.NavigationItem.RightBarButtonItem = new UIBarButtonItem (UIBarButtonSystemItem.Edit, delegate {
				// Activate editing
				dvc.TableView.SetEditing (true, true);
				ConfigDone (dvc);
			});
		}

		void ConfigDone (DialogViewController dvc)
		{
			dvc.NavigationItem.RightBarButtonItem = new UIBarButtonItem (UIBarButtonSystemItem.Done, delegate {
				// Deactivate editing
				dvc.TableView.SetEditing (false, true);
				ConfigEdit (dvc);
			});
		}
		
		public void DemoEditing () 
		{
			var editingSection = new Section ("To-do list") {
				new StringElement ("Donate to non-profit"),
				new StringElement ("Read new Chomsky book"),
				new StringElement ("Practice guitar"),
				new StringElement ("Watch Howard Zinn Documentary")
			};
				
			var root = new RootElement("Edit Support") {
				editingSection
			};
			var dvc = new EditingDialog (root, true);
			ConfigEdit (dvc);
			
			navigation.PushViewController (dvc, true);
		}
	}
}
