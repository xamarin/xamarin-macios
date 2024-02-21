using System;

#nullable enable

[Flags]
public enum BodyOption {
	None = 0x0,
	NeedsTempReturn = 0x1,
	CondStoreRet = 0x3,
	MarkRetDirty = 0x5,
	StoreRet = 0x7,
}
