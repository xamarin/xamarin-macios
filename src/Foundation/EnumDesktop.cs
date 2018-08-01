//
// Copyright 2010, Novell, Inc.
// Copyright 2012 Xamarin Inc.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

// FIXME: EnumDesktop.cs should be merged into Enums.cs
using System;

#if MONOMAC

using ObjCRuntime;

namespace Foundation {
	internal enum NSAttributedStringDataType {
		RTF,
		RTFD,
		HTML,
		DocFormat
	}

	// Utility enum, ObjC uses NSString.  Not passed to the API, so doesn't need nint.
	public enum NSDocumentType {
		Unknown = -1,
		PlainText,
		RTF,
		RTFD,
		HTML,
		MacSimpleText,
		DocFormat,
		WordML,
		OfficeOpenXml,
		WebArchive,
		OpenDocument
	}

	// NSTask.h:typedef NS_ENUM(NSInteger, NSTaskTerminationReason)
	[Native]
	public enum NSTaskTerminationReason : long {
		Exit = 1,
		UncaughtSignal = 2
	}

	// The following constants where taken from MonoDevelop Carbon.cs MacInterop file
	public enum AEEventID : uint {
		OpenApplication = 1868656752, // 'oapp'
		ReopenApplication = 1918988400, //'rapp'
		OpenDocuments = 1868853091, // 'odoc'
		PrintDocuments = 188563030, // 'pdoc'
		OpenContents = 1868787566, // 'ocon'
		QuitApplication =  1903520116, // 'quit'
		ShowPreferences = 1886545254, // 'pref'
		ApplicationDied = 1868720500, // 'obit'
		GetUrl = 1196773964, // 'GURL'
	}

	public enum AEEventClass : uint {
		Mouse = 1836021107, // 'mous'
		Keyboard = 1801812322, // 'keyb'
		TextInput = 1952807028, // 'text'
		Application = 1634758764, // 'appl'
		RemoteAppleEvent = 1701867619,  //'eppc' //remote apple event?
		Menu = 1835363957, // 'menu'
		Window = 2003398244, // 'wind'
		Control = 1668183148, // 'cntl'
		Command = 1668113523, // 'cmds'
		Tablet = 1952607348, // 'tblt'
		Volume = 1987013664, // 'vol '
		Appearance = 1634758765, // 'appm'
		Service = 1936028278, // 'serv'
		Toolbar = 1952604530, // 'tbar'
		ToolbarItem = 1952606580, // 'tbit'
		Accessibility = 1633903461, // 'acce'
		HIObject = 1751740258, // 'hiob'
		AppleEvent = 1634039412, // 'aevt'
		Internet = 1196773964, // 'GURL'
	}

	// Added from NSUserNotification.h
	[Native]
	[Advice ("'NSUserNotification' usages should be replaced with 'UserNotifications framework'.")]
	public enum NSUserNotificationActivationType : long {
		None = 0,
		ContentsClicked = 1,
		ActionButtonClicked = 2,
		Replied = 3,
		AdditionalActionClicked = 4
	}

	[Mac (10,11)]
	[Native][Flags]
	public enum NSAppleEventSendOptions : ulong
	{
		NoReply = 0x00000001, // kAENoReply,
		QueueReply = 0x00000002, // kAEQueueReply,
		WaitForReply = 0x00000003, // kAEWaitReply,
		NeverInteract = 0x00000010, // kAENeverInteract,
		CanInteract = 0x00000020, // kAECanInteract,
		AlwaysInteract = 0x00000030, // kAEAlwaysInteract,
		CanSwitchLayer = 0x00000040, // kAECanSwitchLayer,
		DontRecord = 0x00001000, // kAEDontRecord,
		DontExecute = 0x00002000, // kAEDontExecute,
		DontAnnotate = 0x00010000, // kAEDoNotAutomaticallyAddAnnotationsToEvent,
		DefaultOptions = WaitForReply | CanInteract
	}
}

#endif // MONOMAC
