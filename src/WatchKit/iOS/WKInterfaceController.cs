#nullable enable

#if __IOS__ && !NET
using System;
using System.ComponentModel;
using System.Runtime.Versioning;
using System.Threading.Tasks;

using CoreGraphics;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace WatchKit {
	[Register ("WKInterfaceController", SkipRegistration = true)]
	[Unavailable (PlatformName.iOS, PlatformArchitecture.All)]
	[Obsolete (Constants.WatchKitRemoved)]
	[EditorBrowsable (EditorBrowsableState.Never)]
	public abstract class WKInterfaceController : NSObject {
		public override IntPtr ClassHandle { get { throw new PlatformNotSupportedException (Constants.WatchKitRemoved); } }

		protected WKInterfaceController (NSObjectFlag t) : base (t)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		protected internal WKInterfaceController (IntPtr handle) : base (handle)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		public WKInterfaceController ()
			: base (NSObjectFlag.Empty)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		public virtual void AddMenuItem (global::UIKit.UIImage image, string title, Selector action)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		public virtual void AddMenuItem (string imageName, string title, Selector action)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		public virtual void AddMenuItem (WKMenuItemIcon itemIcon, string title, Selector action)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		public virtual void Awake (NSObject context)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		public virtual void BecomeCurrentPage ()
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		public virtual void ClearAllMenuItems ()
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		public virtual void DidDeactivate ()
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		public virtual void DidSelectRow (WKInterfaceTable table, nint rowIndex)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		public virtual void DismissController ()
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		public virtual void DismissTextInputController ()
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		public virtual NSObject GetContextForSegue (string segueIdentifier)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		public virtual NSObject GetContextForSegue (string segueIdentifier, WKInterfaceTable table, nint rowIndex)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		public virtual NSObject[] GetContextsForSegue (string segueIdentifier)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		public virtual NSObject[] GetContextsForSegue (string segueIdentifier, WKInterfaceTable table, nint rowIndex)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		public virtual void HandleAction (string identifier, global::UserNotifications.UNNotification notification)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		public virtual void HandleLocalNotificationAction (string identifier, global::UIKit.UILocalNotification localNotification)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		public virtual void HandleRemoteNotificationAction (string identifier, NSDictionary remoteNotification)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		public virtual void HandleUserActivity (NSDictionary userActivity)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		public virtual void InvalidateUserActivity ()
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		public static bool OpenParentApplication (NSDictionary userInfo, [BlockProxy (typeof (ObjCRuntime.Trampolines.NIDActionArity2V69))]global::System.Action<NSDictionary, NSError> reply)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		public virtual void PopController ()
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		public virtual void PopToRootController ()
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		public virtual void PresentController (string name, NSObject context)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		public virtual void PresentController (string[] names, NSObject[] contexts)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		public virtual void PresentTextInputController (string[] suggestions, WKTextInputMode inputMode, global::System.Action<NSArray> completion)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		public virtual Task<NSArray> PresentTextInputControllerAsync (string[] suggestions, WKTextInputMode inputMode)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		public virtual void PresentTextInputController (global::System.Func<NSString, NSArray> suggestionsHandler, WKTextInputMode inputMode, [BlockProxy (typeof (ObjCRuntime.Trampolines.NIDActionArity1V208))]global::System.Action<NSArray> completion)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		public virtual Task<NSArray> PresentTextInputControllerAsync (global::System.Func<NSString, NSArray> suggestionsHandler, WKTextInputMode inputMode)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		public virtual void PushController (string name, NSObject context)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		public static void ReloadRootControllers (string[] names, NSObject[] contexts)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		public virtual void SetTitle (string title)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		public virtual void UpdateUserActivity (string type, NSDictionary userInfo, NSUrl webpageURL)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		public virtual void WillActivate ()
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		public virtual CGRect ContentFrame {
			get {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
		}

		public static NSString ErrorDomain {
			get {
				throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
			}
		}


		public void PushController (string name, string context)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		public void PresentController (string name, string context)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		public void PresentController (string [] names, string [] contexts)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		public void AddMenuItem (UIImage image, string title, Action action)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		public void AddMenuItem (string imageName, string title, Action action)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		public void AddMenuItem (WKMenuItemIcon itemIcon, string title, Action action)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}
	} /* class WKInterfaceController */

	[Obsolete (Constants.WatchKitRemoved)]
	[EditorBrowsable (EditorBrowsableState.Never)]
	public class WKPresentMediaPlayerResult {
		public WKPresentMediaPlayerResult (bool didPlayToEnd, double /* NSTimeInterval */ endTime)
		{
			throw new PlatformNotSupportedException (Constants.WatchKitRemoved);
		}

		public bool DidPlayToEnd { get; set; }

		public double EndTime { get; set; }
	}
}
#endif // __IOS__ && !NET
