
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Xamarin.MacDev.Tasks;
using Xamarin.Localization.MSBuild;
using System.ComponentModel;
using System.Reflection;

namespace Xamarin.iOS.Tasks
{
	public abstract class LocalizationStrings : XamarinTask
	{
		public string MSBErrorCode { get; set; }

		public override bool Execute ()
		{
			if (MSBErrorCode == null){
				Log.LogError (MSBStrings.E0007);
			}
			else {
					var error = MSBErrorCode;
					PropertyInfo propertyInfo = typeof(MSBStrings).GetProperty(error);
					if (propertyInfo == null){
						Log.LogError ("Error code not found");
						return false;
					}
					string errorMessage = (string) propertyInfo.GetValue(null, null);
					Log.LogError (errorMessage);
			}
			return false;
		}
	}
}