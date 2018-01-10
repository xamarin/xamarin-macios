using System;
using Foundation;

namespace QTKit
{
	public partial class QTMedia
	{
		internal static QTMediaType QTMediaTypeFromNSString (NSString str)
		{
			if (str == QTMedia.TypeVideo)
				return QTMediaType.Video;
			else if (str == QTMedia.TypeSound)
				return QTMediaType.Sound;
			else if (str == QTMedia.TypeText)
				return QTMediaType.Text;
			else if (str == QTMedia.TypeBase)
				return QTMediaType.Base;
			else if (str == QTMedia.TypeMusic)
				return QTMediaType.Music;
			else if (str == QTMedia.TypeTimeCode)
				return QTMediaType.TimeCode;
			else if (str == QTMedia.TypeSprite)
				return QTMediaType.Sprite;
			else if (str == QTMedia.TypeFlash)
				return QTMediaType.Flash;
			else if (str == QTMedia.TypeMovie)
				return QTMediaType.Movie;
			else if (str == QTMedia.TypeTween)
				return QTMediaType.Tween;
			else if (str == QTMedia.Type3D)
				return QTMediaType.Type3D;
			else if (str == QTMedia.TypeSkin)
				return QTMediaType.Skin;
			else if (str == QTMedia.TypeQTVR)
				return QTMediaType.Qtvr;
			else if (str == QTMedia.TypeHint)
				return QTMediaType.Hint;
			else if (str == QTMedia.TypeStream)
				return QTMediaType.Stream;
			else if (str == QTMedia.TypeMuxed)
				return QTMediaType.Muxed;
			else if (str == QTMedia.TypeQuartzComposer)
				return QTMediaType.QuartzComposer;

			throw new ArgumentException ("No enum found matching the supplied NSString");
		}

		internal static NSString NSStringFromQTMediaType (QTMediaType mediaType)
		{
			switch (mediaType) {
			case QTMediaType.Video:
				return QTMedia.TypeVideo;
			case QTMediaType.Sound:
				return QTMedia.TypeSound;
			case QTMediaType.Text:
				return QTMedia.TypeText;
			case QTMediaType.Base:
				return QTMedia.TypeBase;
			case QTMediaType.Mpeg:
				return QTMedia.TypeMpeg;
			case QTMediaType.Music:
				return QTMedia.TypeMusic;
			case QTMediaType.TimeCode:
				return QTMedia.TypeTimeCode;
			case QTMediaType.Sprite:
				return QTMedia.TypeSprite;
			case QTMediaType.Flash:
				return QTMedia.TypeFlash;
			case QTMediaType.Movie:
				return QTMedia.TypeMovie;
			case QTMediaType.Tween:
				return QTMedia.TypeTween;
			case QTMediaType.Type3D:
				return QTMedia.Type3D;
			case QTMediaType.Skin:
				return QTMedia.TypeSkin;
			case QTMediaType.Qtvr:
				return QTMedia.TypeQTVR;
			case QTMediaType.Hint:
				return QTMedia.TypeHint;
			case QTMediaType.Stream:
				return QTMedia.TypeStream;
			case QTMediaType.Muxed:
				return QTMedia.TypeMuxed;
			case QTMediaType.QuartzComposer:
				return QTMedia.TypeQuartzComposer;
			default:
				throw new ArgumentException ("No enum found matching the supplied NSString");
			}
		}
	}
}

