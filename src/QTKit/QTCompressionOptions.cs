using System;
using Foundation;

namespace QTKit
{
	public partial class QTCompressionOptions
	{
		public QTMediaType MediaTypeValue
		{
			get {
				return QTMedia.QTMediaTypeFromNSString ((NSString)MediaType);
			}
		}

		public string [] GetCompressionOptionsIdentifiers (QTMediaType forMediaType)
		{
			return GetCompressionOptionsIdentifiers (QTMedia.NSStringFromQTMediaType (forMediaType));
		}
	}
}

