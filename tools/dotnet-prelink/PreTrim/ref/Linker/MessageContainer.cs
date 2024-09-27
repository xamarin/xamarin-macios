// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Mono.Linker
{
	public readonly partial struct MessageContainer
	{
		public static partial MessageContainer CreateCustomErrorMessage (string text, int code, string subcategory = "", MessageOrigin? origin = null);
		public static partial MessageContainer CreateCustomWarningMessage (LinkContext context, string text, int code, MessageOrigin origin, WarnVersion version, string subcategory = "");
		public static partial MessageContainer CreateInfoMessage (string text);
		public static partial MessageContainer CreateDiagnosticMessage (string text);
	}
}
