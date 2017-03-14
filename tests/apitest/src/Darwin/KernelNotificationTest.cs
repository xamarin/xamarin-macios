using System;
using System.Diagnostics;

#if XAMCORE_2_0
using Darwin;
#else
using MonoMac.Darwin;
#endif

using NUnit.Framework;

namespace apitest
{
	[TestFixture]
	public class KernelNotificationTest
	{
		KernelEvent [] CreateEvents (Process process)
		{
			return new KernelEvent [] {
				new KernelEvent {
					Ident = (IntPtr) process.Id,
					Filter = EventFilter.Proc,
					Flags = EventFlags.Add,
					FilterFlags = (uint) (FilterFlags.ProcExit),
				}
			};
		}

		[Test]
		public void KEvent ()
		{
			// (KernelEvent[], KernelEvent[])
			using (var sleep = Process.Start ("/bin/sleep", "0.5")) {
				using (var kqueue = new KernelQueue ()) {
					var events = CreateEvents (sleep);
					Assert.IsTrue (kqueue.KEvent (events, events), "kevent");
				}
			}

			// (KernelEvent[], KernelEvent[], TimeSpan?)
			using (var sleep = Process.Start ("/bin/sleep", "0.5")) {
				using (var kqueue = new KernelQueue ()) {
					var events = CreateEvents (sleep);
					Assert.AreEqual (1, kqueue.KEvent (events, events, TimeSpan.FromSeconds (5)), "kevent");
				}
			}

			// (KernelEvent[], KernelEvent[], TimeSpan?)
			using (var sleep = Process.Start ("/bin/sleep", "0.5")) {
				using (var kqueue = new KernelQueue ()) {
					var events = CreateEvents (sleep);
					Assert.AreEqual (1, kqueue.KEvent (events, events, null), "kevent");
				}
			}

			// (KernelEvent[], int, KernelEvent[], int, TimeSpec?)
			using (var sleep = Process.Start ("/bin/sleep", "0.5")) {
				using (var kqueue = new KernelQueue ()) {
					var events = CreateEvents (sleep);
					TimeSpec ts = new TimeSpec
					{
						Seconds = 5,
					};
					Assert.AreEqual (1, kqueue.KEvent (events, events.Length, events, events.Length, ts), "kevent");
				}
			}

			// (KernelEvent[], int, KernelEvent[], int, TimeSpec?)
			using (var sleep = Process.Start ("/bin/sleep", "0.5")) {
				using (var kqueue = new KernelQueue ()) {
					var events = CreateEvents (sleep);
					Assert.AreEqual (1, kqueue.KEvent (events, events.Length, events, events.Length, null), "kevent");
				}
			}

			// (KernelEvent[], int, KernelEvent[], int, ref TimeSpec)
			using (var sleep = Process.Start ("/bin/sleep", "0.5")) {
				using (var kqueue = new KernelQueue ()) {
					var events = CreateEvents (sleep);
					TimeSpec ts = new TimeSpec
					{
						Seconds = 5,
					};
					Assert.IsTrue (kqueue.KEvent (events, events.Length, events, events.Length, ref ts), "kevent");
				}
			}

			// (KernelEvent[], KernelEvent[], ref TimeSpec)
			using (var sleep = Process.Start ("/bin/sleep", "0.5")) {
				using (var kqueue = new KernelQueue ()) {
					var events = CreateEvents (sleep);
					TimeSpec ts = new TimeSpec
					{
						Seconds = 5,
					};
					Assert.IsTrue (kqueue.KEvent (events, events, ref ts), "kevent");
				}
			}

			// (KernelEvent[], int, KernelEvent[], int)
			using (var sleep = Process.Start ("/bin/sleep", "0.5")) {
				using (var kqueue = new KernelQueue ()) {
					var events = CreateEvents (sleep);
					Assert.IsTrue (kqueue.KEvent (events, events.Length, events, events.Length), "kevent");
				}
			}
		}
	}
}
