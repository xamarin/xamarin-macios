using System;
using Foundation;

namespace QTKit
{
	public partial class QTFormatDescription
	{
		public QTMediaType MediaTypeValue
		{
			get {
				return QTMedia.QTMediaTypeFromNSString ((NSString)MediaType);
			}
		}
	}
}

