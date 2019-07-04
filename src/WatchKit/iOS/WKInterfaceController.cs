#if __IOS__
using System;
using System.ComponentModel;
using System.Threading.Tasks;

using CoreGraphics;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace WatchKit {
	[Register ("WKInterfaceController", SkipRegistration = true)]
	[Introduced (PlatformName.iOS, 8,2, PlatformArchitecture.All)]
	[Unavailable (PlatformName.iOS, PlatformArchitecture.All)]
	[Obsolete ("The WatchKit framework has been removed from iOS")]
	[EditorBrowsable (EditorBrowsableState.Never)]
	public abstract class WKInterfaceController : NSObject {
		public override IntPtr ClassHandle { get { throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS"); } }

		protected WKInterfaceController (NSObjectFlag t) : base (t)
		{
			throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
		}

		protected internal WKInterfaceController (IntPtr handle) : base (handle)
		{
			throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
		}

		public WKInterfaceController ()
			: base (NSObjectFlag.Empty)
		{
			throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
		}

		public virtual void AddMenuItem (global::UIKit.UIImage image, string title, Selector action)
		{
			throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
		}

		public virtual void AddMenuItem (string imageName, string title, Selector action)
		{
			throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
		}

		public virtual void AddMenuItem (WKMenuItemIcon itemIcon, string title, Selector action)
		{
			throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
		}

		public virtual void Awake (NSObject context)
		{
			throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
		}

		public virtual void BecomeCurrentPage ()
		{
			throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
		}

		public virtual void ClearAllMenuItems ()
		{
			throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
		}

		public virtual void DidDeactivate ()
		{
			throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
		}

		public virtual void DidSelectRow (WKInterfaceTable table, nint rowIndex)
		{
			throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
		}

		public virtual void DismissController ()
		{
			throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
		}

		public virtual void DismissTextInputController ()
		{
			throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
		}

		public virtual NSObject GetContextForSegue (string segueIdentifier)
		{
			throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
		}

		public virtual NSObject GetContextForSegue (string segueIdentifier, WKInterfaceTable table, nint rowIndex)
		{
			throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
		}

		public virtual NSObject[] GetContextsForSegue (string segueIdentifier)
		{
			throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
		}

		public virtual NSObject[] GetContextsForSegue (string segueIdentifier, WKInterfaceTable table, nint rowIndex)
		{
			throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
		}

		public virtual void HandleAction (string identifier, global::UserNotifications.UNNotification notification)
		{
			throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
		}

		public virtual void HandleLocalNotificationAction (string identifier, global::UIKit.UILocalNotification localNotification)
		{
			throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
		}

		public virtual void HandleRemoteNotificationAction (string identifier, NSDictionary remoteNotification)
		{
			throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
		}

		public virtual void HandleUserActivity (NSDictionary userActivity)
		{
			throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
		}

		public virtual void InvalidateUserActivity ()
		{
			throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
		}

		public static bool OpenParentApplication (NSDictionary userInfo, [BlockProxy (typeof (ObjCRuntime.Trampolines.NIDActionArity2V69))]global::System.Action<NSDictionary, NSError> reply)
		{
			throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
		}

		public virtual void PopController ()
		{
			throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
		}

		public virtual void PopToRootController ()
		{
			throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
		}

		public virtual void PresentController (string name, NSObject context)
		{
			throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
		}

		public virtual void PresentController (string[] names, NSObject[] contexts)
		{
			throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
		}

		public virtual void PresentTextInputController (string[] suggestions, WKTextInputMode inputMode, global::System.Action<NSArray> completion)
		{
			throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
		}

		public virtual Task<NSArray> PresentTextInputControllerAsync (string[] suggestions, WKTextInputMode inputMode)
		{
			throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
		}

		public virtual void PresentTextInputController (global::System.Func<NSString, NSArray> suggestionsHandler, WKTextInputMode inputMode, [BlockProxy (typeof (ObjCRuntime.Trampolines.NIDActionArity1V208))]global::System.Action<NSArray> completion)
		{
			throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
		}

		public virtual Task<NSArray> PresentTextInputControllerAsync (global::System.Func<NSString, NSArray> suggestionsHandler, WKTextInputMode inputMode)
		{
			throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
		}

		public virtual void PushController (string name, NSObject context)
		{
			throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
		}

		public static void ReloadRootControllers (string[] names, NSObject[] contexts)
		{
			throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
		}

		public virtual void SetTitle (string title)
		{
			throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
		}

		public virtual void UpdateUserActivity (string type, NSDictionary userInfo, NSUrl webpageURL)
		{
			throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
		}

		public virtual void WillActivate ()
		{
			throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
		}

		public virtual CGRect ContentFrame {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}

		public static NSString ErrorDomain {
			get {
				throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
			}
		}


		public void PushController (string name, string context)
		{
			throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
		}

		public void PresentController (string name, string context)
		{
			throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
		}

		public void PresentController (string [] names, string [] contexts)
		{
			throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
		}

		public void AddMenuItem (UIImage image, string title, Action action)
		{
			throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
		}

		public void AddMenuItem (string imageName, string title, Action action)
		{
			throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
		}

		public void AddMenuItem (WKMenuItemIcon itemIcon, string title, Action action)
		{
			throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
		}
	} /* class WKInterfaceController */

	public class WKPresentMediaPlayerResult {
		public WKPresentMediaPlayerResult (bool didPlayToEnd, double /* NSTimeInterval */ endTime)
		{
			throw new PlatformNotSupportedException ("The WatchKit framework has been removed from iOS");
		}

		public bool DidPlayToEnd { get; set; }

		public double EndTime { get; set; }
	}
}
#endif // __IOS__
