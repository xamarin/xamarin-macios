//
// Timer.cs
//
// Author:
//   Aaron Bockover <abock@xamarin.com>
//
// Copyright 2015 Xamarin Inc. All rights reserved.

using System;
using System.Collections.Generic;

namespace Xamarin.Pmcs
{
	public class Timer
	{
		public static readonly IReadOnlyList<Timer> All = new List<Timer> ();

		readonly MonitorImpl monitor;
		DateTime operationStartTime;
		int operations;
		TimeSpan totalTime;

		public string Name { get; private set; }

		public int Operations {
			get { return operations; }
		}

		public TimeSpan TotalTime {
			get { return totalTime; }
		}

		public Timer (string name)
		{
			Name = name;
			monitor = new MonitorImpl (this);
			((List<Timer>)All).Add (this);
		}

		class MonitorImpl : IDisposable
		{
			readonly Timer timer;

			public MonitorImpl (Timer timer)
			{
				this.timer = timer;
			}

			public void Dispose ()
			{
				timer.operations++;
				timer.totalTime += DateTime.UtcNow - timer.operationStartTime;
			}
		}

		public IDisposable Monitor ()
		{
			operationStartTime = DateTime.UtcNow;
			return monitor;
		}

		public override string ToString ()
		{
			return string.Format ("{0}: {1} operations in {2} seconds",
				Name, Operations, TotalTime.TotalSeconds);
		}
	}
}