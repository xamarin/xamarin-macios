// Copyright 2019 Microsoft Corp. All rights reserved.

using System;

namespace Xamarin.Bundler
{
	partial class Target
	{
		public void SelectMonoNative ()
		{
			if (App.DeploymentTarget >= new Version (10, 12))
				MonoNativeMode = MonoNativeMode.Unified;
			else
				MonoNativeMode = MonoNativeMode.Compat;
		}
	}
}