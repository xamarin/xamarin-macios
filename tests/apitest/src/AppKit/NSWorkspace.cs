using NUnit.Framework;
using System;

#if !XAMCORE_2_0
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;
#else
using AppKit;
using ObjCRuntime;
#endif

namespace Xamarin.Mac.Tests
{
	[TestFixture]
	public class NSWorkspaceTests
	{
		[Test]
		public void NSWorkspaceConstantTests ()
		{
			Assert.IsNotNull (NSWorkspace.LaunchConfigurationAppleEvent);
			Assert.IsNotNull (NSWorkspace.LaunchConfigurationArguments);
			Assert.IsNotNull (NSWorkspace.LaunchConfigurationEnvironment);
			Assert.IsNotNull (NSWorkspace.LaunchConfigurationArchitecture);
		}

		[Test]
		public void HFSTypeCode4CCTest()
		{
		    	/* Generic Finder icons */
			Assert.That(FourCC((int)HFSTypeCode.ClipboardIcon), Is.EqualTo("CLIP"), "CLIP");
			Assert.That(FourCC((int)HFSTypeCode.ClippingUnknownTypeIcon), Is.EqualTo("clpu"), "clpu");
			Assert.That(FourCC((int)HFSTypeCode.ClippingPictureTypeIcon), Is.EqualTo("clpp"), "clpp");
			Assert.That(FourCC((int)HFSTypeCode.ClippingTextTypeIcon), Is.EqualTo("clpt"), "clpt");
			Assert.That(FourCC((int)HFSTypeCode.ClippingSoundTypeIcon), Is.EqualTo("clps"), "clps");
			Assert.That(FourCC((int)HFSTypeCode.DesktopIcon), Is.EqualTo("desk"), "desk");
			Assert.That(FourCC((int)HFSTypeCode.FinderIcon), Is.EqualTo("FNDR"), "FNDR");
			Assert.That(FourCC((int)HFSTypeCode.ComputerIcon), Is.EqualTo("root"), "root");
			Assert.That(FourCC((int)HFSTypeCode.FontSuitcaseIcon), Is.EqualTo("FFIL"), "FFIL");
			Assert.That(FourCC((int)HFSTypeCode.FullTrashIcon), Is.EqualTo("ftrh"), "ftrh");
			Assert.That(FourCC((int)HFSTypeCode.GenericApplicationIcon), Is.EqualTo("APPL"), "APPL");
			Assert.That(FourCC((int)HFSTypeCode.GenericCDROMIcon), Is.EqualTo("cddr"), "cddr");
			Assert.That(FourCC((int)HFSTypeCode.GenericControlPanelIcon), Is.EqualTo("APPC"), "APPC");
			Assert.That(FourCC((int)HFSTypeCode.GenericControlStripModuleIcon), Is.EqualTo("sdev"), "sdev");
			Assert.That(FourCC((int)HFSTypeCode.GenericComponentIcon), Is.EqualTo("thng"), "thng");
			Assert.That(FourCC((int)HFSTypeCode.GenericDeskAccessoryIcon), Is.EqualTo("APPD"), "APPD");
			Assert.That(FourCC((int)HFSTypeCode.GenericDocumentIcon), Is.EqualTo("docu"), "docu");
			Assert.That(FourCC((int)HFSTypeCode.GenericEditionFileIcon), Is.EqualTo("edtf"), "edtf");
			Assert.That(FourCC((int)HFSTypeCode.GenericExtensionIcon), Is.EqualTo("INIT"), "INIT");
			Assert.That(FourCC((int)HFSTypeCode.GenericFileServerIcon), Is.EqualTo("srvr"), "srvr");
			Assert.That(FourCC((int)HFSTypeCode.GenericFontIcon), Is.EqualTo("ffil"), "ffil");
			Assert.That(FourCC((int)HFSTypeCode.GenericFontScalerIcon), Is.EqualTo("sclr"), "sclr");
			Assert.That(FourCC((int)HFSTypeCode.GenericFloppyIcon), Is.EqualTo("flpy"), "flpy");
			Assert.That(FourCC((int)HFSTypeCode.GenericHardDiskIcon), Is.EqualTo("hdsk"), "hdsk");
			Assert.That(FourCC((int)HFSTypeCode.GenericIDiskIcon), Is.EqualTo("idsk"), "idsk");
			Assert.That(FourCC((int)HFSTypeCode.GenericRemovableMediaIcon), Is.EqualTo("rmov"), "rmov");
			Assert.That(FourCC((int)HFSTypeCode.GenericMoverObjectIcon), Is.EqualTo("movr"), "movr");
			Assert.That(FourCC((int)HFSTypeCode.GenericPCCardIcon), Is.EqualTo("pcmc"), "pcmc");
			Assert.That(FourCC((int)HFSTypeCode.GenericPreferencesIcon), Is.EqualTo("pref"), "pref");
			Assert.That(FourCC((int)HFSTypeCode.GenericQueryDocumentIcon), Is.EqualTo("qery"), "qery");
			Assert.That(FourCC((int)HFSTypeCode.GenericRAMDiskIcon), Is.EqualTo("ramd"), "ramd");
			Assert.That(FourCC((int)HFSTypeCode.GenericSharedLibaryIcon), Is.EqualTo("shlb"), "shlb");
			Assert.That(FourCC((int)HFSTypeCode.GenericStationeryIcon), Is.EqualTo("sdoc"), "sdoc");
			Assert.That(FourCC((int)HFSTypeCode.GenericSuitcaseIcon), Is.EqualTo("suit"), "suit");
			Assert.That(FourCC((int)HFSTypeCode.GenericURLIcon), Is.EqualTo("gurl"), "gurl");
			Assert.That(FourCC((int)HFSTypeCode.GenericWORMIcon), Is.EqualTo("worm"), "worm");
			Assert.That(FourCC((int)HFSTypeCode.InternationalResourcesIcon), Is.EqualTo("ifil"), "ifil");
			Assert.That(FourCC((int)HFSTypeCode.KeyboardLayoutIcon), Is.EqualTo("kfil"), "kfil");
			Assert.That(FourCC((int)HFSTypeCode.SoundFileIcon), Is.EqualTo("sfil"), "sfil");
			Assert.That(FourCC((int)HFSTypeCode.SystemSuitcaseIcon), Is.EqualTo("zsys"), "zsys");
			Assert.That(FourCC((int)HFSTypeCode.TrashIcon), Is.EqualTo("trsh"), "trsh");
			Assert.That(FourCC((int)HFSTypeCode.TrueTypeFontIcon), Is.EqualTo("tfil"), "tfil");
			Assert.That(FourCC((int)HFSTypeCode.TrueTypeFlatFontIcon), Is.EqualTo("sfnt"), "sfnt");
			Assert.That(FourCC((int)HFSTypeCode.TrueTypeMultiFlatFontIcon), Is.EqualTo("ttcf"), "ttcf");
			Assert.That(FourCC((int)HFSTypeCode.UserIDiskIcon), Is.EqualTo("udsk"), "udsk");
			Assert.That(FourCC((int)HFSTypeCode.UnknownFSObjectIcon), Is.EqualTo("unfs"), "unfs");

			/* Internet locations */
			Assert.That(FourCC((int)HFSTypeCode.InternetLocationHTTPIcon), Is.EqualTo("ilht"), "ilht");
			Assert.That(FourCC((int)HFSTypeCode.InternetLocationFTPIcon), Is.EqualTo("ilft"), "ilft");
			Assert.That(FourCC((int)HFSTypeCode.InternetLocationAppleShareIcon), Is.EqualTo("ilaf"), "ilaf");
			Assert.That(FourCC((int)HFSTypeCode.InternetLocationAppleTalkZoneIcon), Is.EqualTo("ilat"), "ilat");
			Assert.That(FourCC((int)HFSTypeCode.InternetLocationFileIcon), Is.EqualTo("ilfi"), "ilfi");
			Assert.That(FourCC((int)HFSTypeCode.InternetLocationMailIcon), Is.EqualTo("ilma"), "ilma");
			Assert.That(FourCC((int)HFSTypeCode.InternetLocationNewsIcon), Is.EqualTo("ilnw"), "ilnw");
			Assert.That(FourCC((int)HFSTypeCode.InternetLocationNSLNeighborhoodIcon), Is.EqualTo("ilns"), "ilns");
			Assert.That(FourCC((int)HFSTypeCode.InternetLocationGenericIcon), Is.EqualTo("ilge"), "ilge");

			/* Folders */
			Assert.That(FourCC((int)HFSTypeCode.GenericFolderIcon), Is.EqualTo("fldr"), "fldr");
			Assert.That(FourCC((int)HFSTypeCode.DropFolderIcon), Is.EqualTo("dbox"), "dbox");
			Assert.That(FourCC((int)HFSTypeCode.MountedFolderIcon), Is.EqualTo("mntd"), "mntd");
			Assert.That(FourCC((int)HFSTypeCode.OpenFolderIcon), Is.EqualTo("ofld"), "ofld");
			Assert.That(FourCC((int)HFSTypeCode.OwnedFolderIcon), Is.EqualTo("ownd"), "ownd");
			Assert.That(FourCC((int)HFSTypeCode.PrivateFolderIcon), Is.EqualTo("prvf"), "prvf");
			Assert.That(FourCC((int)HFSTypeCode.SharedFolderIcon), Is.EqualTo("shfl"), "shfl");

			/* Sharingprivileges icons */
			Assert.That(FourCC((int)HFSTypeCode.SharingPrivsNotApplicableIcon), Is.EqualTo("shna"), "shna");
			Assert.That(FourCC((int)HFSTypeCode.SharingPrivsReadOnlyIcon), Is.EqualTo("shro"), "shro");
			Assert.That(FourCC((int)HFSTypeCode.SharingPrivsReadWriteIcon), Is.EqualTo("shrw"), "shrw");
			Assert.That(FourCC((int)HFSTypeCode.SharingPrivsUnknownIcon), Is.EqualTo("shuk"), "shuk");
			Assert.That(FourCC((int)HFSTypeCode.SharingPrivsWritableIcon), Is.EqualTo("writ"), "writ");

			/* Users and Groups icons */
			Assert.That(FourCC((int)HFSTypeCode.UserFolderIcon), Is.EqualTo("ufld"), "ufld");
			Assert.That(FourCC((int)HFSTypeCode.WorkgroupFolderIcon), Is.EqualTo("wfld"), "wfld");
			Assert.That(FourCC((int)HFSTypeCode.GuestUserIcon), Is.EqualTo("gusr"), "gusr");
			Assert.That(FourCC((int)HFSTypeCode.UserIcon), Is.EqualTo("user"), "user");
			Assert.That(FourCC((int)HFSTypeCode.OwnerIcon), Is.EqualTo("susr"), "susr");
			Assert.That(FourCC((int)HFSTypeCode.GroupIcon), Is.EqualTo("grup"), "grup");

			/* Special folders */
			Assert.That(FourCC((int)HFSTypeCode.AppearanceFolderIcon), Is.EqualTo("appr"), "appr");
			Assert.That(FourCC((int)HFSTypeCode.AppleMenuFolderIcon), Is.EqualTo("amnu"), "amnu");
			Assert.That(FourCC((int)HFSTypeCode.ApplicationsFolderIcon), Is.EqualTo("apps"), "apps");
			Assert.That(FourCC((int)HFSTypeCode.ApplicationSupportFolderIcon), Is.EqualTo("asup"), "asup");
			Assert.That(FourCC((int)HFSTypeCode.ColorSyncFolderIcon), Is.EqualTo("prof"), "prof");
			Assert.That(FourCC((int)HFSTypeCode.ContextualMenuItemsFolderIcon), Is.EqualTo("cmnu"), "cmnu");
			Assert.That(FourCC((int)HFSTypeCode.ControlPanelDisabledFolderIcon), Is.EqualTo("ctrD"), "ctrD");
			Assert.That(FourCC((int)HFSTypeCode.ControlPanelFolderIcon), Is.EqualTo("ctrl"), "ctrl");
			Assert.That(FourCC((int)HFSTypeCode.DocumentsFolderIcon), Is.EqualTo("docs"), "docs");
			Assert.That(FourCC((int)HFSTypeCode.ExtensionsDisabledFolderIcon), Is.EqualTo("extD"), "extD");
			Assert.That(FourCC((int)HFSTypeCode.ExtensionsFolderIcon), Is.EqualTo("extn"), "extn");
			Assert.That(FourCC((int)HFSTypeCode.FavoritesFolderIcon), Is.EqualTo("favs"), "favs");
			Assert.That(FourCC((int)HFSTypeCode.FontsFolderIcon), Is.EqualTo("font"), "font");
			Assert.That(FourCC((int)HFSTypeCode.InternetSearchSitesFolderIcon), Is.EqualTo("issf"), "issf");
			Assert.That(FourCC((int)HFSTypeCode.PublicFolderIcon), Is.EqualTo("pubf"), "pubf");
			Assert.That(FourCC((int)HFSTypeCode.PrinterDescriptionFolderIcon), Is.EqualTo("ppdf"), "ppdf");
			Assert.That(FourCC((int)HFSTypeCode.PrintMonitorFolderIcon), Is.EqualTo("prnt"), "prnt");
			Assert.That(FourCC((int)HFSTypeCode.RecentApplicationsFolderIcon), Is.EqualTo("rapp"), "rapp");
			Assert.That(FourCC((int)HFSTypeCode.RecentDocumentsFolderIcon), Is.EqualTo("rdoc"), "rdoc");
			Assert.That(FourCC((int)HFSTypeCode.RecentServersFolderIcon), Is.EqualTo("rsrv"), "rsrv");
			Assert.That(FourCC((int)HFSTypeCode.ShutdownItemsDisabledFolderIcon), Is.EqualTo("shdD"), "shdD");
			Assert.That(FourCC((int)HFSTypeCode.ShutdownItemsFolderIcon), Is.EqualTo("shdf"), "shdf");
			Assert.That(FourCC((int)HFSTypeCode.SpeakableItemsFolder), Is.EqualTo("spki"), "spki");
			Assert.That(FourCC((int)HFSTypeCode.StartupItemsDisabledFolderIcon), Is.EqualTo("strD"), "strD");
			Assert.That(FourCC((int)HFSTypeCode.StartupItemsFolderIcon), Is.EqualTo("strt"), "strt");
			Assert.That(FourCC((int)HFSTypeCode.SystemExtensionDisabledFolderIcon), Is.EqualTo("macD"), "macD");
			Assert.That(FourCC((int)HFSTypeCode.SystemFolderIcon), Is.EqualTo("macs"), "macs");
			Assert.That(FourCC((int)HFSTypeCode.VoicesFolderIcon), Is.EqualTo("fvoc"), "fvoc");

			/* Badges */
			Assert.That(FourCC((int)HFSTypeCode.AppleScriptBadgeIcon), Is.EqualTo("scrp"), "scrp");
			Assert.That(FourCC((int)HFSTypeCode.LockedBadgeIcon), Is.EqualTo("lbdg"), "lbdg");
			Assert.That(FourCC((int)HFSTypeCode.MountedBadgeIcon), Is.EqualTo("mbdg"), "mbdg");
			Assert.That(FourCC((int)HFSTypeCode.SharedBadgeIcon), Is.EqualTo("sbdg"), "sbdg");
			Assert.That(FourCC((int)HFSTypeCode.AliasBadgeIcon), Is.EqualTo("abdg"), "abdg");
			Assert.That(FourCC((int)HFSTypeCode.AlertCautionBadgeIcon), Is.EqualTo("cbdg"), "cbdg");

			/* Alert icons */
			Assert.That(FourCC((int)HFSTypeCode.AlertNoteIcon), Is.EqualTo("note"), "note");
			Assert.That(FourCC((int)HFSTypeCode.AlertCautionIcon), Is.EqualTo("caut"), "caut");
			Assert.That(FourCC((int)HFSTypeCode.AlertStopIcon), Is.EqualTo("stop"), "stop");

			/* Networking icons */
			Assert.That(FourCC((int)HFSTypeCode.AppleTalkIcon), Is.EqualTo("atlk"), "atlk");
			Assert.That(FourCC((int)HFSTypeCode.AppleTalkZoneIcon), Is.EqualTo("atzn"), "atzn");
			Assert.That(FourCC((int)HFSTypeCode.AFPServerIcon), Is.EqualTo("afps"), "afps");
			Assert.That(FourCC((int)HFSTypeCode.FTPServerIcon), Is.EqualTo("ftps"), "ftps");
			Assert.That(FourCC((int)HFSTypeCode.HTTPServerIcon), Is.EqualTo("htps"), "htps");
			Assert.That(FourCC((int)HFSTypeCode.GenericNetworkIcon), Is.EqualTo("gnet"), "gnet");
			Assert.That(FourCC((int)HFSTypeCode.IPFileServerIcon), Is.EqualTo("isrv"), "isrv");

			/* Toolbar icons */
			Assert.That(FourCC((int)HFSTypeCode.ToolbarCustomizeIcon), Is.EqualTo("tcus"), "tcus");
			Assert.That(FourCC((int)HFSTypeCode.ToolbarDeleteIcon), Is.EqualTo("tdel"), "tdel");
			Assert.That(FourCC((int)HFSTypeCode.ToolbarFavoritesIcon), Is.EqualTo("tfav"), "tfav");
			Assert.That(FourCC((int)HFSTypeCode.ToolbarHomeIcon), Is.EqualTo("thom"), "thom");
			Assert.That(FourCC((int)HFSTypeCode.ToolbarAdvancedIcon), Is.EqualTo("tbav"), "tbav");
			Assert.That(FourCC((int)HFSTypeCode.ToolbarInfoIcon), Is.EqualTo("tbin"), "tbin");
			Assert.That(FourCC((int)HFSTypeCode.ToolbarLabelsIcon), Is.EqualTo("tblb"), "tblb");
			Assert.That(FourCC((int)HFSTypeCode.ToolbarApplicationsFolderIcon), Is.EqualTo("tAps"), "tAps");
			Assert.That(FourCC((int)HFSTypeCode.ToolbarDocumentsFolderIcon), Is.EqualTo("tDoc"), "tDoc");
			Assert.That(FourCC((int)HFSTypeCode.ToolbarMovieFolderIcon), Is.EqualTo("tMov"), "tMov");
			Assert.That(FourCC((int)HFSTypeCode.ToolbarMusicFolderIcon), Is.EqualTo("tMus"), "tMus");
			Assert.That(FourCC((int)HFSTypeCode.ToolbarPicturesFolderIcon), Is.EqualTo("tPic"), "tPic");
			Assert.That(FourCC((int)HFSTypeCode.ToolbarPublicFolderIcon), Is.EqualTo("tPub"), "tPub");
			Assert.That(FourCC((int)HFSTypeCode.ToolbarDesktopFolderIcon), Is.EqualTo("tDsk"), "tDsk");
			Assert.That(FourCC((int)HFSTypeCode.ToolbarDownloadsFolderIcon), Is.EqualTo("tDwn"), "tDwn");
			Assert.That(FourCC((int)HFSTypeCode.ToolbarLibraryFolderIcon), Is.EqualTo("tLib"), "tLib");
			Assert.That(FourCC((int)HFSTypeCode.ToolbarUtilitiesFolderIcon), Is.EqualTo("tUtl"), "tUtl");
			Assert.That(FourCC((int)HFSTypeCode.ToolbarSitesFolderIcon), Is.EqualTo("tSts"), "tSts");

			/* Other icons */
			Assert.That(FourCC((int)HFSTypeCode.AppleLogoIcon), Is.EqualTo("capl"), "capl");
			Assert.That(FourCC((int)HFSTypeCode.AppleMenuIcon), Is.EqualTo("sapl"), "sapl");
			Assert.That(FourCC((int)HFSTypeCode.BackwardArrowIcon), Is.EqualTo("baro"), "baro");
			Assert.That(FourCC((int)HFSTypeCode.FavoriteItemsIcon), Is.EqualTo("favr"), "favr");
			Assert.That(FourCC((int)HFSTypeCode.ForwardArrowIcon), Is.EqualTo("faro"), "faro");
			Assert.That(FourCC((int)HFSTypeCode.GridIcon), Is.EqualTo("grid"), "grid");
			Assert.That(FourCC((int)HFSTypeCode.HelpIcon), Is.EqualTo("help"), "help");
			Assert.That(FourCC((int)HFSTypeCode.KeepArrangedIcon), Is.EqualTo("arng"), "arng");
			Assert.That(FourCC((int)HFSTypeCode.LockedIcon), Is.EqualTo("lock"), "lock");
			Assert.That(FourCC((int)HFSTypeCode.NoFilesIcon), Is.EqualTo("nfil"), "nfil");
			Assert.That(FourCC((int)HFSTypeCode.NoFolderIcon), Is.EqualTo("nfld"), "nfld");
			Assert.That(FourCC((int)HFSTypeCode.NoWriteIcon), Is.EqualTo("nwrt"), "nwrt");
			Assert.That(FourCC((int)HFSTypeCode.ProtectedApplicationFolderIcon), Is.EqualTo("papp"), "papp");
			Assert.That(FourCC((int)HFSTypeCode.ProtectedSystemFolderIcon), Is.EqualTo("psys"), "psys");
			Assert.That(FourCC((int)HFSTypeCode.RecentItemsIcon), Is.EqualTo("rcnt"), "rcnt");
			Assert.That(FourCC((int)HFSTypeCode.ShortcutIcon), Is.EqualTo("shrt"), "shrt");
			Assert.That(FourCC((int)HFSTypeCode.SortAscendingIcon), Is.EqualTo("asnd"), "asnd");
			Assert.That(FourCC((int)HFSTypeCode.SortDescendingIcon), Is.EqualTo("dsnd"), "dsnd");
			Assert.That(FourCC((int)HFSTypeCode.UnlockedIcon), Is.EqualTo("ulck"), "ulck");
			Assert.That(FourCC((int)HFSTypeCode.ConnectToIcon), Is.EqualTo("cnct"), "cnct");
			Assert.That(FourCC((int)HFSTypeCode.GenericWindowIcon), Is.EqualTo("gwin"), "gwin");
			Assert.That(FourCC((int)HFSTypeCode.QuestionMarkIcon), Is.EqualTo("ques"), "ques");
			Assert.That(FourCC((int)HFSTypeCode.DeleteAliasIcon), Is.EqualTo("dali"), "dali");
			Assert.That(FourCC((int)HFSTypeCode.EjectMediaIcon), Is.EqualTo("ejec"), "ejec");
			Assert.That(FourCC((int)HFSTypeCode.BurningIcon), Is.EqualTo("burn"), "burn");
			Assert.That(FourCC((int)HFSTypeCode.RightContainerArrowIcon), Is.EqualTo("rcar"), "rcar");
        	}

		string FourCC(int value)
		{
		    return new string(new char[] {
			(char) (byte) (value >> 24),
			(char) (byte) (value >> 16),
			(char) (byte) (value >> 8),
			(char) (byte) value });
		}
    }
}
