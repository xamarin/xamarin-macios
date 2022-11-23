// Copyright 2016 Xamarin Inc. All rights reserved.

#nullable enable

#if !XAMCORE_3_0

using System;

namespace Twitter {

	public partial class TWTweetComposeViewController {

		[Obsolete ("Use the 'CompletionHandler' property.")]
		public virtual void SetCompletionHandler (Action<TWTweetComposeViewControllerResult> handler)
		{
			CompletionHandler = handler;
		}
	}
}

#endif
