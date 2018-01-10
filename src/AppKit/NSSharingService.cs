using System;
using Foundation;
using ObjCRuntime;

namespace AppKit
{
	public partial class NSSharingService
	{
		public static NSSharingService GetSharingService (NSSharingServiceName service)
		{
			switch (service) {
			
			case NSSharingServiceName.PostOnFacebook:
				return NSSharingService.GetSharingService(NSSharingServiceNamePostOnFacebook);
			
			case NSSharingServiceName.PostOnTwitter:
				return NSSharingService.GetSharingService(NSSharingServiceNamePostOnTwitter);

			case NSSharingServiceName.PostOnSinaWeibo:
				return NSSharingService.GetSharingService(NSSharingServiceNamePostOnSinaWeibo);

			case NSSharingServiceName.ComposeEmail:
				return NSSharingService.GetSharingService(NSSharingServiceNameComposeEmail);

			case NSSharingServiceName.ComposeMessage:
				return NSSharingService.GetSharingService(NSSharingServiceNameComposeMessage);

			case NSSharingServiceName.SendViaAirDrop:
				return NSSharingService.GetSharingService(NSSharingServiceNameSendViaAirDrop);

			case NSSharingServiceName.AddToSafariReadingList:
				return NSSharingService.GetSharingService(NSSharingServiceNameAddToSafariReadingList);

			case NSSharingServiceName.AddToIPhoto:
				return NSSharingService.GetSharingService(NSSharingServiceNameAddToIPhoto);

			case NSSharingServiceName.AddToAperture:
				return NSSharingService.GetSharingService(NSSharingServiceNameAddToAperture);

			case NSSharingServiceName.UseAsTwitterProfileImage:
				return NSSharingService.GetSharingService(NSSharingServiceNameUseAsTwitterProfileImage);

			case NSSharingServiceName.UseAsDesktopPicture:
				return NSSharingService.GetSharingService(NSSharingServiceNameUseAsDesktopPicture);

			case NSSharingServiceName.PostImageOnFlickr:
				return NSSharingService.GetSharingService(NSSharingServiceNamePostImageOnFlickr);

			case NSSharingServiceName.PostVideoOnVimeo:
				return NSSharingService.GetSharingService(NSSharingServiceNamePostVideoOnVimeo);

			case NSSharingServiceName.PostVideoOnYouku:
				return NSSharingService.GetSharingService(NSSharingServiceNamePostVideoOnYouku);

			case NSSharingServiceName.PostVideoOnTudou:
				return NSSharingService.GetSharingService(NSSharingServiceNamePostVideoOnTudou);
			
			case NSSharingServiceName.CloudSharing:
				return NSSharingService.GetSharingService (NSSharingServiceNameCloudSharing);
			default:
				return null;
			}
		}
	}
}

