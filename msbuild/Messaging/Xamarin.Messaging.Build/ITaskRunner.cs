using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Messaging.Build.Contracts;

namespace Xamarin.Messaging.Build
{
	public interface ITaskRunner
	{
		ExecuteTaskResult Execute(string taskName, string inputs);
	}
}