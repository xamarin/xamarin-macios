using System;
using XamCore.Foundation;

namespace XamCore.QTKit
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

