using System;
using MonoTouch.Dialog;
using UIKit;
using Foundation;
namespace Sample {
	// 
	// To support editing, you need to subclass the DialogViewController and provide
	// your own "Source" classes (which you also have to subclass).
	//
	// There are two source classes, a sizing one (for elements that have different
	// sizes, and one for fixed element sizes.   Since we are just using strings,
	// we are going to take a shortcut and just implement one of the sources.
	//

	public class AdvancedEditingDialog : DialogViewController {

		// This is our subclass of the fixed-size Source that allows editing
		public class EditingSource : DialogViewController.Source {
			public EditingSource (DialogViewController dvc) : base (dvc) { }

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

			public override bool CanMoveRow (UITableView tableView, NSIndexPath indexPath)
			{
				return true;
			}

			public override void MoveRow (UITableView tableView, NSIndexPath sourceIndexPath, NSIndexPath destinationIndexPath)
			{
				var section = Container.Root [sourceIndexPath.Section];
				var source = section [sourceIndexPath.Row];
				section.Remove (source);
				section.Insert (destinationIndexPath.Row, source);
			}
		}

		public override Source CreateSizingSource (bool unevenRows)
		{
			if (unevenRows)
				throw new NotImplementedException ("You need to create a new SourceSizing subclass, this sample does not have it");
			return new EditingSource (this);
		}

		public AdvancedEditingDialog (RootElement root, bool pushing) : base (root, pushing)
		{
		}
	}

	public partial class AppDelegate {
		public void DemoAdvancedEditing ()
		{
			var root = new RootElement ("Todo list") {
				new Section() {
					new StringElement ("1", "Todo item 1"),
					new StringElement ("2","Todo item 2"),
					new StringElement ("3","Todo item 3"),
					new StringElement ("4","Todo item 4"),
					new StringElement ("5","Todo item 5")
				}
			};
			var dvc = new AdvancedEditingDialog (root, true);
			AdvancedConfigEdit (dvc);
			navigation.PushViewController (dvc, true);
		}

		void AdvancedConfigEdit (DialogViewController dvc)
		{
			dvc.NavigationItem.RightBarButtonItem = new UIBarButtonItem (UIBarButtonSystemItem.Edit, delegate
			{
				// Activate editing
				// Switch the root to editable elements		
				dvc.Root = CreateEditableRoot (dvc.Root, true);
				dvc.ReloadData ();
				// Activate row editing & deleting
				dvc.TableView.SetEditing (true, true);
				AdvancedConfigDone (dvc);
			});
		}

		void AdvancedConfigDone (DialogViewController dvc)
		{
			dvc.NavigationItem.RightBarButtonItem = new UIBarButtonItem (UIBarButtonSystemItem.Done, delegate
			{
				// Deactivate editing
				dvc.ReloadData ();
				// Switch updated entry elements to StringElements
				dvc.Root = CreateEditableRoot (dvc.Root, false);
				dvc.TableView.SetEditing (false, true);
				AdvancedConfigEdit (dvc);
			});
		}

		RootElement CreateEditableRoot (RootElement root, bool editable)
		{
			var rootElement = new RootElement ("Todo list") {
				new Section()
			};

			foreach (var element in root [0].Elements) {
				if (element is StringElement) {
					rootElement [0].Add (CreateEditableElement (element.Caption, (element as StringElement).Value, editable));
				} else {
					rootElement [0].Add (CreateEditableElement (element.Caption, (element as EntryElement).Value, editable));
				}
			}

			return rootElement;
		}

		Element CreateEditableElement (string caption, string content, bool editable)
		{
			if (editable) {
				return new EntryElement (caption, "todo", content);
			} else {
				return new StringElement (caption, content);
			}
		}
	}
}
