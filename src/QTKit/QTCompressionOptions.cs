using System;
using XamCore.Foundation;

namespace XamCore.QTKit
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

