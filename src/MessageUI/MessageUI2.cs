//
// MessageUI.cs: This file describes the API that the generator will produce for MessageUI
//
// Authors:
//   Miguel de Icaza
//
// Copyright 2009, Novell, Inc.
//

using System;
using ObjCRuntime;
using Foundation;
using CoreFoundation;

namespace MessageUI {

	public class MFComposeResultEventArgs : EventArgs {
		public MFComposeResultEventArgs (MFMailComposeViewController controller, MFMailComposeResult result, NSError error)
		{
			Result = result;
			Error = error;
			Controller = controller;
		}
		public MFMailComposeResult Result { get; private set; }
		public NSError Error { get; private set; }
		public MFMailComposeViewController Controller { get; private set; }
	}
	
	public partial class MFMailComposeViewController {
		Mono_MFMailComposeViewControllerDelegate EnsureDelegate ()
		{
			NSObject del = WeakMailComposeDelegate;
			if (del == null || (!(del is Mono_MFMailComposeViewControllerDelegate))){
				del = new Mono_MFMailComposeViewControllerDelegate ();
				WeakMailComposeDelegate = del;
			}
			return (Mono_MFMailComposeViewControllerDelegate) del;
		}

		public event EventHandler<MFComposeResultEventArgs> Finished {
			add {
				EnsureDelegate ().cbFinished += value;
			}

			remove {
				EnsureDelegate ().cbFinished -= value;
			}
		}
	}

	class Mono_MFMailComposeViewControllerDelegate : MFMailComposeViewControllerDelegate {
		internal EventHandler<MFComposeResultEventArgs> cbFinished;

		public Mono_MFMailComposeViewControllerDelegate ()
		{
			IsDirectBinding = false;
		}

		[Preserve (Conditional = true)]
		public override void Finished (MFMailComposeViewController controller, MFMailComposeResult result, NSError error)
		{
			if (cbFinished != null)
				cbFinished (controller, new MFComposeResultEventArgs (controller, result, error));
		}
	}


	public class MFMessageComposeResultEventArgs : EventArgs {
		public MFMessageComposeResultEventArgs (MFMessageComposeViewController controller, MessageComposeResult result)
		{
			Result = result;
			Controller = controller;
		}
		public MessageComposeResult Result { get; private set; }
		public MFMessageComposeViewController Controller { get; private set; }
	}


	public partial class MFMessageComposeViewController {
		Mono_MFMessageComposeViewControllerDelegate EnsureDelegate ()
		{
			NSObject del = WeakMessageComposeDelegate;
			if (del == null || (!(del is Mono_MFMessageComposeViewControllerDelegate))){
				del = new Mono_MFMessageComposeViewControllerDelegate ();
				WeakMessageComposeDelegate = del;
			}
			return (Mono_MFMessageComposeViewControllerDelegate) del;
		}

		public event EventHandler<MFMessageComposeResultEventArgs> Finished {
			add {
				EnsureDelegate ().cbFinished += value;
			}

			remove {
				EnsureDelegate ().cbFinished -= value;
			}
		}
	}

	class Mono_MFMessageComposeViewControllerDelegate : MFMessageComposeViewControllerDelegate {
		internal EventHandler<MFMessageComposeResultEventArgs> cbFinished;

		public Mono_MFMessageComposeViewControllerDelegate ()
		{
			IsDirectBinding = false;
		}

		[Preserve (Conditional = true)]
		public override void Finished (MFMessageComposeViewController controller, MessageComposeResult result)
		{
			if (cbFinished != null)
				cbFinished (controller, new MFMessageComposeResultEventArgs (controller, result));
		}
	}


}
