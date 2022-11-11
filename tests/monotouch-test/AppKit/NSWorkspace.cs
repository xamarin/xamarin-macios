#if __MACOS__
using NUnit.Framework;
using System;

using AppKit;
using Foundation;
using ObjCRuntime;

namespace Xamarin.Mac.Tests {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public class NSWorkspaceTests {
		[Test]
		public void NSWorkspaceConstantTests ()
		{
			Assert.IsNotNull (NSWorkspace.LaunchConfigurationAppleEvent);
			Assert.IsNotNull (NSWorkspace.LaunchConfigurationArguments);
			Assert.IsNotNull (NSWorkspace.LaunchConfigurationEnvironment);
			Assert.IsNotNull (NSWorkspace.LaunchConfigurationArchitecture);
		}

		[Test]
		public void HfsTypeCode4CCTest ()
		{
			/* Generic Finder icons */
			Assert.That (FourCC ((int) HfsTypeCode.ClipboardIcon), Is.EqualTo ("CLIP"), "CLIP");
			Assert.That (FourCC ((int) HfsTypeCode.ClippingUnknownTypeIcon), Is.EqualTo ("clpu"), "clpu");
			Assert.That (FourCC ((int) HfsTypeCode.ClippingPictureTypeIcon), Is.EqualTo ("clpp"), "clpp");
			Assert.That (FourCC ((int) HfsTypeCode.ClippingTextTypeIcon), Is.EqualTo ("clpt"), "clpt");
			Assert.That (FourCC ((int) HfsTypeCode.ClippingSoundTypeIcon), Is.EqualTo ("clps"), "clps");
			Assert.That (FourCC ((int) HfsTypeCode.DesktopIcon), Is.EqualTo ("desk"), "desk");
			Assert.That (FourCC ((int) HfsTypeCode.FinderIcon), Is.EqualTo ("FNDR"), "FNDR");
			Assert.That (FourCC ((int) HfsTypeCode.ComputerIcon), Is.EqualTo ("root"), "root");
			Assert.That (FourCC ((int) HfsTypeCode.FontSuitcaseIcon), Is.EqualTo ("FFIL"), "FFIL");
			Assert.That (FourCC ((int) HfsTypeCode.FullTrashIcon), Is.EqualTo ("ftrh"), "ftrh");
			Assert.That (FourCC ((int) HfsTypeCode.GenericApplicationIcon), Is.EqualTo ("APPL"), "APPL");
			Assert.That (FourCC ((int) HfsTypeCode.GenericCdromIcon), Is.EqualTo ("cddr"), "cddr");
			Assert.That (FourCC ((int) HfsTypeCode.GenericControlPanelIcon), Is.EqualTo ("APPC"), "APPC");
			Assert.That (FourCC ((int) HfsTypeCode.GenericControlStripModuleIcon), Is.EqualTo ("sdev"), "sdev");
			Assert.That (FourCC ((int) HfsTypeCode.GenericComponentIcon), Is.EqualTo ("thng"), "thng");
			Assert.That (FourCC ((int) HfsTypeCode.GenericDeskAccessoryIcon), Is.EqualTo ("APPD"), "APPD");
			Assert.That (FourCC ((int) HfsTypeCode.GenericDocumentIcon), Is.EqualTo ("docu"), "docu");
			Assert.That (FourCC ((int) HfsTypeCode.GenericEditionFileIcon), Is.EqualTo ("edtf"), "edtf");
			Assert.That (FourCC ((int) HfsTypeCode.GenericExtensionIcon), Is.EqualTo ("INIT"), "INIT");
			Assert.That (FourCC ((int) HfsTypeCode.GenericFileServerIcon), Is.EqualTo ("srvr"), "srvr");
			Assert.That (FourCC ((int) HfsTypeCode.GenericFontIcon), Is.EqualTo ("ffil"), "ffil");
			Assert.That (FourCC ((int) HfsTypeCode.GenericFontScalerIcon), Is.EqualTo ("sclr"), "sclr");
			Assert.That (FourCC ((int) HfsTypeCode.GenericFloppyIcon), Is.EqualTo ("flpy"), "flpy");
			Assert.That (FourCC ((int) HfsTypeCode.GenericHardDiskIcon), Is.EqualTo ("hdsk"), "hdsk");
			Assert.That (FourCC ((int) HfsTypeCode.GenericIDiskIcon), Is.EqualTo ("idsk"), "idsk");
			Assert.That (FourCC ((int) HfsTypeCode.GenericRemovableMediaIcon), Is.EqualTo ("rmov"), "rmov");
			Assert.That (FourCC ((int) HfsTypeCode.GenericMoverObjectIcon), Is.EqualTo ("movr"), "movr");
			Assert.That (FourCC ((int) HfsTypeCode.GenericPCCardIcon), Is.EqualTo ("pcmc"), "pcmc");
			Assert.That (FourCC ((int) HfsTypeCode.GenericPreferencesIcon), Is.EqualTo ("pref"), "pref");
			Assert.That (FourCC ((int) HfsTypeCode.GenericQueryDocumentIcon), Is.EqualTo ("qery"), "qery");
			Assert.That (FourCC ((int) HfsTypeCode.GenericRamDiskIcon), Is.EqualTo ("ramd"), "ramd");
#if NET
			Assert.That (FourCC ((int) HfsTypeCode.GenericSharedLibraryIcon), Is.EqualTo ("shlb"), "shlb");
#else
			Assert.That (FourCC ((int) HfsTypeCode.GenericSharedLibaryIcon), Is.EqualTo ("shlb"), "shlb");
#endif
			Assert.That (FourCC ((int) HfsTypeCode.GenericStationeryIcon), Is.EqualTo ("sdoc"), "sdoc");
			Assert.That (FourCC ((int) HfsTypeCode.GenericSuitcaseIcon), Is.EqualTo ("suit"), "suit");
			Assert.That (FourCC ((int) HfsTypeCode.GenericUrlIcon), Is.EqualTo ("gurl"), "gurl");
			Assert.That (FourCC ((int) HfsTypeCode.GenericWormIcon), Is.EqualTo ("worm"), "worm");
			Assert.That (FourCC ((int) HfsTypeCode.InternationalResourcesIcon), Is.EqualTo ("ifil"), "ifil");
			Assert.That (FourCC ((int) HfsTypeCode.KeyboardLayoutIcon), Is.EqualTo ("kfil"), "kfil");
			Assert.That (FourCC ((int) HfsTypeCode.SoundFileIcon), Is.EqualTo ("sfil"), "sfil");
			Assert.That (FourCC ((int) HfsTypeCode.SystemSuitcaseIcon), Is.EqualTo ("zsys"), "zsys");
			Assert.That (FourCC ((int) HfsTypeCode.TrashIcon), Is.EqualTo ("trsh"), "trsh");
			Assert.That (FourCC ((int) HfsTypeCode.TrueTypeFontIcon), Is.EqualTo ("tfil"), "tfil");
			Assert.That (FourCC ((int) HfsTypeCode.TrueTypeFlatFontIcon), Is.EqualTo ("sfnt"), "sfnt");
			Assert.That (FourCC ((int) HfsTypeCode.TrueTypeMultiFlatFontIcon), Is.EqualTo ("ttcf"), "ttcf");
			Assert.That (FourCC ((int) HfsTypeCode.UserIDiskIcon), Is.EqualTo ("udsk"), "udsk");
			Assert.That (FourCC ((int) HfsTypeCode.UnknownFSObjectIcon), Is.EqualTo ("unfs"), "unfs");

			/* Internet locations */
			Assert.That (FourCC ((int) HfsTypeCode.InternetLocationHttpIcon), Is.EqualTo ("ilht"), "ilht");
			Assert.That (FourCC ((int) HfsTypeCode.InternetLocationFtpIcon), Is.EqualTo ("ilft"), "ilft");
			Assert.That (FourCC ((int) HfsTypeCode.InternetLocationAppleShareIcon), Is.EqualTo ("ilaf"), "ilaf");
			Assert.That (FourCC ((int) HfsTypeCode.InternetLocationAppleTalkZoneIcon), Is.EqualTo ("ilat"), "ilat");
			Assert.That (FourCC ((int) HfsTypeCode.InternetLocationFileIcon), Is.EqualTo ("ilfi"), "ilfi");
			Assert.That (FourCC ((int) HfsTypeCode.InternetLocationMailIcon), Is.EqualTo ("ilma"), "ilma");
			Assert.That (FourCC ((int) HfsTypeCode.InternetLocationNewsIcon), Is.EqualTo ("ilnw"), "ilnw");
			Assert.That (FourCC ((int) HfsTypeCode.InternetLocationNslNeighborhoodIcon), Is.EqualTo ("ilns"), "ilns");
			Assert.That (FourCC ((int) HfsTypeCode.InternetLocationGenericIcon), Is.EqualTo ("ilge"), "ilge");

			/* Folders */
			Assert.That (FourCC ((int) HfsTypeCode.GenericFolderIcon), Is.EqualTo ("fldr"), "fldr");
			Assert.That (FourCC ((int) HfsTypeCode.DropFolderIcon), Is.EqualTo ("dbox"), "dbox");
			Assert.That (FourCC ((int) HfsTypeCode.MountedFolderIcon), Is.EqualTo ("mntd"), "mntd");
			Assert.That (FourCC ((int) HfsTypeCode.OpenFolderIcon), Is.EqualTo ("ofld"), "ofld");
			Assert.That (FourCC ((int) HfsTypeCode.OwnedFolderIcon), Is.EqualTo ("ownd"), "ownd");
			Assert.That (FourCC ((int) HfsTypeCode.PrivateFolderIcon), Is.EqualTo ("prvf"), "prvf");
			Assert.That (FourCC ((int) HfsTypeCode.SharedFolderIcon), Is.EqualTo ("shfl"), "shfl");

			/* Sharingprivileges icons */
			Assert.That (FourCC ((int) HfsTypeCode.SharingPrivsNotApplicableIcon), Is.EqualTo ("shna"), "shna");
			Assert.That (FourCC ((int) HfsTypeCode.SharingPrivsReadOnlyIcon), Is.EqualTo ("shro"), "shro");
			Assert.That (FourCC ((int) HfsTypeCode.SharingPrivsReadWriteIcon), Is.EqualTo ("shrw"), "shrw");
			Assert.That (FourCC ((int) HfsTypeCode.SharingPrivsUnknownIcon), Is.EqualTo ("shuk"), "shuk");
			Assert.That (FourCC ((int) HfsTypeCode.SharingPrivsWritableIcon), Is.EqualTo ("writ"), "writ");

			/* Users and Groups icons */
			Assert.That (FourCC ((int) HfsTypeCode.UserFolderIcon), Is.EqualTo ("ufld"), "ufld");
			Assert.That (FourCC ((int) HfsTypeCode.WorkgroupFolderIcon), Is.EqualTo ("wfld"), "wfld");
			Assert.That (FourCC ((int) HfsTypeCode.GuestUserIcon), Is.EqualTo ("gusr"), "gusr");
			Assert.That (FourCC ((int) HfsTypeCode.UserIcon), Is.EqualTo ("user"), "user");
			Assert.That (FourCC ((int) HfsTypeCode.OwnerIcon), Is.EqualTo ("susr"), "susr");
			Assert.That (FourCC ((int) HfsTypeCode.GroupIcon), Is.EqualTo ("grup"), "grup");

			/* Special folders */
			Assert.That (FourCC ((int) HfsTypeCode.AppearanceFolderIcon), Is.EqualTo ("appr"), "appr");
			Assert.That (FourCC ((int) HfsTypeCode.AppleMenuFolderIcon), Is.EqualTo ("amnu"), "amnu");
			Assert.That (FourCC ((int) HfsTypeCode.ApplicationsFolderIcon), Is.EqualTo ("apps"), "apps");
			Assert.That (FourCC ((int) HfsTypeCode.ApplicationSupportFolderIcon), Is.EqualTo ("asup"), "asup");
			Assert.That (FourCC ((int) HfsTypeCode.ColorSyncFolderIcon), Is.EqualTo ("prof"), "prof");
			Assert.That (FourCC ((int) HfsTypeCode.ContextualMenuItemsFolderIcon), Is.EqualTo ("cmnu"), "cmnu");
			Assert.That (FourCC ((int) HfsTypeCode.ControlPanelDisabledFolderIcon), Is.EqualTo ("ctrD"), "ctrD");
			Assert.That (FourCC ((int) HfsTypeCode.ControlPanelFolderIcon), Is.EqualTo ("ctrl"), "ctrl");
			Assert.That (FourCC ((int) HfsTypeCode.DocumentsFolderIcon), Is.EqualTo ("docs"), "docs");
			Assert.That (FourCC ((int) HfsTypeCode.ExtensionsDisabledFolderIcon), Is.EqualTo ("extD"), "extD");
			Assert.That (FourCC ((int) HfsTypeCode.ExtensionsFolderIcon), Is.EqualTo ("extn"), "extn");
			Assert.That (FourCC ((int) HfsTypeCode.FavoritesFolderIcon), Is.EqualTo ("favs"), "favs");
			Assert.That (FourCC ((int) HfsTypeCode.FontsFolderIcon), Is.EqualTo ("font"), "font");
			Assert.That (FourCC ((int) HfsTypeCode.InternetSearchSitesFolderIcon), Is.EqualTo ("issf"), "issf");
			Assert.That (FourCC ((int) HfsTypeCode.PublicFolderIcon), Is.EqualTo ("pubf"), "pubf");
			Assert.That (FourCC ((int) HfsTypeCode.PrinterDescriptionFolderIcon), Is.EqualTo ("ppdf"), "ppdf");
			Assert.That (FourCC ((int) HfsTypeCode.PrintMonitorFolderIcon), Is.EqualTo ("prnt"), "prnt");
			Assert.That (FourCC ((int) HfsTypeCode.RecentApplicationsFolderIcon), Is.EqualTo ("rapp"), "rapp");
			Assert.That (FourCC ((int) HfsTypeCode.RecentDocumentsFolderIcon), Is.EqualTo ("rdoc"), "rdoc");
			Assert.That (FourCC ((int) HfsTypeCode.RecentServersFolderIcon), Is.EqualTo ("rsrv"), "rsrv");
			Assert.That (FourCC ((int) HfsTypeCode.ShutdownItemsDisabledFolderIcon), Is.EqualTo ("shdD"), "shdD");
			Assert.That (FourCC ((int) HfsTypeCode.ShutdownItemsFolderIcon), Is.EqualTo ("shdf"), "shdf");
			Assert.That (FourCC ((int) HfsTypeCode.SpeakableItemsFolder), Is.EqualTo ("spki"), "spki");
			Assert.That (FourCC ((int) HfsTypeCode.StartupItemsDisabledFolderIcon), Is.EqualTo ("strD"), "strD");
			Assert.That (FourCC ((int) HfsTypeCode.StartupItemsFolderIcon), Is.EqualTo ("strt"), "strt");
			Assert.That (FourCC ((int) HfsTypeCode.SystemExtensionDisabledFolderIcon), Is.EqualTo ("macD"), "macD");
			Assert.That (FourCC ((int) HfsTypeCode.SystemFolderIcon), Is.EqualTo ("macs"), "macs");
			Assert.That (FourCC ((int) HfsTypeCode.VoicesFolderIcon), Is.EqualTo ("fvoc"), "fvoc");

			/* Badges */
			Assert.That (FourCC ((int) HfsTypeCode.AppleScriptBadgeIcon), Is.EqualTo ("scrp"), "scrp");
			Assert.That (FourCC ((int) HfsTypeCode.LockedBadgeIcon), Is.EqualTo ("lbdg"), "lbdg");
			Assert.That (FourCC ((int) HfsTypeCode.MountedBadgeIcon), Is.EqualTo ("mbdg"), "mbdg");
			Assert.That (FourCC ((int) HfsTypeCode.SharedBadgeIcon), Is.EqualTo ("sbdg"), "sbdg");
			Assert.That (FourCC ((int) HfsTypeCode.AliasBadgeIcon), Is.EqualTo ("abdg"), "abdg");
			Assert.That (FourCC ((int) HfsTypeCode.AlertCautionBadgeIcon), Is.EqualTo ("cbdg"), "cbdg");

			/* Alert icons */
			Assert.That (FourCC ((int) HfsTypeCode.AlertNoteIcon), Is.EqualTo ("note"), "note");
			Assert.That (FourCC ((int) HfsTypeCode.AlertCautionIcon), Is.EqualTo ("caut"), "caut");
			Assert.That (FourCC ((int) HfsTypeCode.AlertStopIcon), Is.EqualTo ("stop"), "stop");

			/* Networking icons */
			Assert.That (FourCC ((int) HfsTypeCode.AppleTalkIcon), Is.EqualTo ("atlk"), "atlk");
			Assert.That (FourCC ((int) HfsTypeCode.AppleTalkZoneIcon), Is.EqualTo ("atzn"), "atzn");
			Assert.That (FourCC ((int) HfsTypeCode.AfpServerIcon), Is.EqualTo ("afps"), "afps");
			Assert.That (FourCC ((int) HfsTypeCode.FtpServerIcon), Is.EqualTo ("ftps"), "ftps");
			Assert.That (FourCC ((int) HfsTypeCode.HttpServerIcon), Is.EqualTo ("htps"), "htps");
			Assert.That (FourCC ((int) HfsTypeCode.GenericNetworkIcon), Is.EqualTo ("gnet"), "gnet");
			Assert.That (FourCC ((int) HfsTypeCode.IPFileServerIcon), Is.EqualTo ("isrv"), "isrv");

			/* Toolbar icons */
			Assert.That (FourCC ((int) HfsTypeCode.ToolbarCustomizeIcon), Is.EqualTo ("tcus"), "tcus");
			Assert.That (FourCC ((int) HfsTypeCode.ToolbarDeleteIcon), Is.EqualTo ("tdel"), "tdel");
			Assert.That (FourCC ((int) HfsTypeCode.ToolbarFavoritesIcon), Is.EqualTo ("tfav"), "tfav");
			Assert.That (FourCC ((int) HfsTypeCode.ToolbarHomeIcon), Is.EqualTo ("thom"), "thom");
			Assert.That (FourCC ((int) HfsTypeCode.ToolbarAdvancedIcon), Is.EqualTo ("tbav"), "tbav");
			Assert.That (FourCC ((int) HfsTypeCode.ToolbarInfoIcon), Is.EqualTo ("tbin"), "tbin");
			Assert.That (FourCC ((int) HfsTypeCode.ToolbarLabelsIcon), Is.EqualTo ("tblb"), "tblb");
			Assert.That (FourCC ((int) HfsTypeCode.ToolbarApplicationsFolderIcon), Is.EqualTo ("tAps"), "tAps");
			Assert.That (FourCC ((int) HfsTypeCode.ToolbarDocumentsFolderIcon), Is.EqualTo ("tDoc"), "tDoc");
			Assert.That (FourCC ((int) HfsTypeCode.ToolbarMovieFolderIcon), Is.EqualTo ("tMov"), "tMov");
			Assert.That (FourCC ((int) HfsTypeCode.ToolbarMusicFolderIcon), Is.EqualTo ("tMus"), "tMus");
			Assert.That (FourCC ((int) HfsTypeCode.ToolbarPicturesFolderIcon), Is.EqualTo ("tPic"), "tPic");
			Assert.That (FourCC ((int) HfsTypeCode.ToolbarPublicFolderIcon), Is.EqualTo ("tPub"), "tPub");
			Assert.That (FourCC ((int) HfsTypeCode.ToolbarDesktopFolderIcon), Is.EqualTo ("tDsk"), "tDsk");
			Assert.That (FourCC ((int) HfsTypeCode.ToolbarDownloadsFolderIcon), Is.EqualTo ("tDwn"), "tDwn");
			Assert.That (FourCC ((int) HfsTypeCode.ToolbarLibraryFolderIcon), Is.EqualTo ("tLib"), "tLib");
			Assert.That (FourCC ((int) HfsTypeCode.ToolbarUtilitiesFolderIcon), Is.EqualTo ("tUtl"), "tUtl");
			Assert.That (FourCC ((int) HfsTypeCode.ToolbarSitesFolderIcon), Is.EqualTo ("tSts"), "tSts");

			/* Other icons */
			Assert.That (FourCC ((int) HfsTypeCode.AppleLogoIcon), Is.EqualTo ("capl"), "capl");
			Assert.That (FourCC ((int) HfsTypeCode.AppleMenuIcon), Is.EqualTo ("sapl"), "sapl");
			Assert.That (FourCC ((int) HfsTypeCode.BackwardArrowIcon), Is.EqualTo ("baro"), "baro");
			Assert.That (FourCC ((int) HfsTypeCode.FavoriteItemsIcon), Is.EqualTo ("favr"), "favr");
			Assert.That (FourCC ((int) HfsTypeCode.ForwardArrowIcon), Is.EqualTo ("faro"), "faro");
			Assert.That (FourCC ((int) HfsTypeCode.GridIcon), Is.EqualTo ("grid"), "grid");
			Assert.That (FourCC ((int) HfsTypeCode.HelpIcon), Is.EqualTo ("help"), "help");
			Assert.That (FourCC ((int) HfsTypeCode.KeepArrangedIcon), Is.EqualTo ("arng"), "arng");
			Assert.That (FourCC ((int) HfsTypeCode.LockedIcon), Is.EqualTo ("lock"), "lock");
			Assert.That (FourCC ((int) HfsTypeCode.NoFilesIcon), Is.EqualTo ("nfil"), "nfil");
			Assert.That (FourCC ((int) HfsTypeCode.NoFolderIcon), Is.EqualTo ("nfld"), "nfld");
			Assert.That (FourCC ((int) HfsTypeCode.NoWriteIcon), Is.EqualTo ("nwrt"), "nwrt");
			Assert.That (FourCC ((int) HfsTypeCode.ProtectedApplicationFolderIcon), Is.EqualTo ("papp"), "papp");
			Assert.That (FourCC ((int) HfsTypeCode.ProtectedSystemFolderIcon), Is.EqualTo ("psys"), "psys");
			Assert.That (FourCC ((int) HfsTypeCode.RecentItemsIcon), Is.EqualTo ("rcnt"), "rcnt");
			Assert.That (FourCC ((int) HfsTypeCode.ShortcutIcon), Is.EqualTo ("shrt"), "shrt");
			Assert.That (FourCC ((int) HfsTypeCode.SortAscendingIcon), Is.EqualTo ("asnd"), "asnd");
			Assert.That (FourCC ((int) HfsTypeCode.SortDescendingIcon), Is.EqualTo ("dsnd"), "dsnd");
			Assert.That (FourCC ((int) HfsTypeCode.UnlockedIcon), Is.EqualTo ("ulck"), "ulck");
			Assert.That (FourCC ((int) HfsTypeCode.ConnectToIcon), Is.EqualTo ("cnct"), "cnct");
			Assert.That (FourCC ((int) HfsTypeCode.GenericWindowIcon), Is.EqualTo ("gwin"), "gwin");
			Assert.That (FourCC ((int) HfsTypeCode.QuestionMarkIcon), Is.EqualTo ("ques"), "ques");
			Assert.That (FourCC ((int) HfsTypeCode.DeleteAliasIcon), Is.EqualTo ("dali"), "dali");
			Assert.That (FourCC ((int) HfsTypeCode.EjectMediaIcon), Is.EqualTo ("ejec"), "ejec");
			Assert.That (FourCC ((int) HfsTypeCode.BurningIcon), Is.EqualTo ("burn"), "burn");
			Assert.That (FourCC ((int) HfsTypeCode.RightContainerArrowIcon), Is.EqualTo ("rcar"), "rcar");
		}

		string FourCC (int value)
		{
			return new string (new char [] {
			(char) (byte) (value >> 24),
			(char) (byte) (value >> 16),
			(char) (byte) (value >> 8),
			(char) (byte) value });
		}
	}
}
#endif // __MACOS__
