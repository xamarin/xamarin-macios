
#if !WATCH // no header
using System;
using System.Runtime.InteropServices;
using CoreFoundation;
using Foundation;
using ObjCRuntime;
using CoreAnimation;
using CoreLocation;
#if IOS
using MapKit;
#endif
using UIKit;
using CoreGraphics;

namespace UIKit {
	public partial class UITableView {
		public UITableViewSource Source {
			get {
				var d = WeakDelegate as UITableViewSource;
				if (d is not null)
					return d;
				d = WeakDataSource as UITableViewSource;
				if (d is not null)
					return d;
				return null;
			}

			set {
				WeakDelegate = value;
				WeakDataSource = value;
			}
		}

		public void RegisterClassForCellReuse (Type cellType, NSString reuseIdentifier)
		{
			RegisterClassForCellReuse (cellType is null ? IntPtr.Zero : Class.GetHandle (cellType), reuseIdentifier);
		}

		public void RegisterClassForCellReuse (Type cellType, string reuseIdentifier)
		{
			using (var str = (NSString) reuseIdentifier)
				RegisterClassForCellReuse (cellType is null ? IntPtr.Zero : Class.GetHandle (cellType), str);
		}

		public void RegisterClassForHeaderFooterViewReuse (Type cellType, string reuseIdentifier)
		{
			using (var str = (NSString) reuseIdentifier)
				RegisterClassForHeaderFooterViewReuse (cellType is null ? IntPtr.Zero : Class.GetHandle (cellType), str);
		}

		public void RegisterClassForHeaderFooterViewReuse (Type cellType, NSString reuseIdentifier)
		{
			RegisterClassForHeaderFooterViewReuse (cellType is null ? IntPtr.Zero : Class.GetHandle (cellType), reuseIdentifier);
		}

		// This is not obsolete, we provide both a (UINib,string) overload and a (UINib,NSString) overload.
		// The difference is that in Unified the overridable method is the (UINib,NSString) overload to
		// be consistent with other API taking a reuseIdentifier.
		public void RegisterNibForCellReuse (UINib nib, string reuseIdentifier)
		{
			using (var str = (NSString) reuseIdentifier)
				RegisterNibForCellReuse (nib, str);
		}

		public UITableViewCell DequeueReusableCell (string reuseIdentifier, NSIndexPath indexPath)
		{
			using (var str = (NSString) reuseIdentifier)
				return DequeueReusableCell (str, indexPath);
		}

		public UITableViewHeaderFooterView DequeueReusableHeaderFooterView (string reuseIdentifier)
		{
			using (var str = (NSString) reuseIdentifier)
				return DequeueReusableHeaderFooterView (str);
		}

		// This is not obsolete, we provide both a (UINib,string) overload and a (UINib,NSString) overload.
		// The difference is that in Unified the overridable method is the (UINib,NSString) overload to
		// be consistent with other API taking a reuseIdentifier.
		public void RegisterNibForHeaderFooterViewReuse (UINib nib, string reuseIdentifier)
		{
			using (var str = (NSString) reuseIdentifier)
				RegisterNibForHeaderFooterViewReuse (nib, str);
		}
	}
}
#endif // !WATCH
