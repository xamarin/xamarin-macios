using System;
using System.Drawing;

using Foundation;
using PhotosUI;
using Photos;
using UIKit;

namespace MyPhotoEditingExtension {
	public partial class PhotoEditingViewController : UIViewController, IPHContentEditingController {
		PHContentEditingInput input;

		public PhotoEditingViewController (IntPtr handle) : base (handle)
		{
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();

			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Do any additional setup after loading the view.
		}

		public bool ShouldShowCancelConfirmation {
			get { return true; }
		}

		public bool CanHandleAdjustmentData (PHAdjustmentData adjustmentData)
		{
			// Inspect the adjustmentData to determine whether your extension can work with past edits.
			// (Typically, you use its formatIdentifier and formatVersion properties to do this.)
			return false;
		}

		public void StartContentEditing (PHContentEditingInput contentEditingInput, UIImage placeholderImage)
		{
			// Present content for editing and keep the contentEditingInput for use when closing the edit session.
			// If you returned true from CanHandleAdjustmentData(), contentEditingInput has the original image and adjustment data.
			// If you returned false, the contentEditingInput has past edits "baked in".
			input = contentEditingInput;
		}

		public void FinishContentEditing (Action<PHContentEditingOutput> completionHandler)
		{
			// Update UI to reflect that editing has finished and output is being rendered.

			// Render and provide output on a background queue.
			NSObject.InvokeInBackground (() => {
				// Create editing output from the editing input.
				var output = new PHContentEditingOutput (input);

				// Provide new adjustments and render output to given location.
				// output.AdjustmentData = <#new adjustment data#>;
				// NSData renderedJPEGData = <#output JPEG#>;
				// renderedJPEGData.Save (output.RenderedContentURL, true);

				// Call completion handler to commit edit to Photos.
				completionHandler (output);

				// Clean up temporary files, etc.
			});
		}

		public void CancelContentEditing ()
		{
			// Clean up temporary files, etc.
			// May be called after finishContentEditingWithCompletionHandler: while you prepare output.
		}
	}
}
