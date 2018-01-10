using System;
using Foundation;

namespace QTKit
{
	public partial class QTCaptureConnection
	{
		public NSObject GetAttribute (string attributeKey)
		{
			return GetAttribute ((NSString)attributeKey);
		}

		public void SetAttribute (NSObject attribute, string key)
		{
			SetAttribute (attribute, (NSString)key);
		}

		public QTMediaType MediaTypeValue
		{
			get {
				return QTMedia.QTMediaTypeFromNSString ((NSString)MediaType);
			}
		}
	}
}

