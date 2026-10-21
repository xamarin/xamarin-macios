// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.IO;
using System.Linq;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using NUnit.Framework;

namespace Xamarin.MacDev.Tasks {
	[TestFixture]
	public class GetCachedFileHashTaskTests : TestBase {
		[Test]
		public void HashesOnlyChangedFiles ()
		{
			var directory = Cache.CreateTemporaryDirectory ();
			var first = Path.Combine (directory, "first.dll");
			var second = Path.Combine (directory, "second.dll");
			var replacement = Path.Combine (directory, "replacement.dll");
			var cache = Path.Combine (directory, "r2r-input-hashes.json");
			File.WriteAllText (first, "first");
			File.WriteAllText (second, "second");

			var initial = Hash (cache, first, second);
			Assert.That (initial.FilesHashed, Is.EqualTo (2), "Initial build");
			var firstHash = initial.Items [0].GetMetadata ("FileHash");
			var secondHash = initial.Items [1].GetMetadata ("FileHash");
			Assert.That (firstHash, Does.Match ("^[0-9A-F]{64}$"), "SHA256 format");

			var unchanged = Hash (cache, second, first);
			Assert.That (unchanged.FilesHashed, Is.Zero, "Unchanged build");
			Assert.That (unchanged.Items.Select (item => item.GetMetadata ("FileHash")), Is.EqualTo (new [] { secondHash, firstHash }), "Input order");
			Assert.That (unchanged.Items [0].GetMetadata ("FileHashAlgorithm"), Is.EqualTo ("SHA256"), "Cached hash algorithm");

			var timestamp = File.GetLastWriteTimeUtc (first).AddSeconds (3);
			File.SetLastWriteTimeUtc (first, timestamp);
			var touched = Hash (cache, first, second);
			Assert.That (touched.FilesHashed, Is.EqualTo (1), "Timestamp-only change");
			Assert.That (touched.Items [0].GetMetadata ("FileHash"), Is.EqualTo (firstHash), "Unchanged contents");

			File.WriteAllText (first, "changed");
			File.SetLastWriteTimeUtc (first, timestamp.AddSeconds (3));
			var changed = Hash (cache, first, second);
			Assert.That (changed.FilesHashed, Is.EqualTo (1), "Content change");
			Assert.That (changed.Items [0].GetMetadata ("FileHash"), Is.Not.EqualTo (firstHash), "Changed contents");
			Assert.That (changed.Items [1].GetMetadata ("FileHash"), Is.EqualTo (secondHash), "Unchanged input");

			File.WriteAllText (replacement, "second");
			File.SetLastWriteTimeUtc (replacement, File.GetLastWriteTimeUtc (second));
			var replaced = Hash (cache, first, replacement);
			Assert.That (replaced.FilesHashed, Is.EqualTo (1), "Added input path");
			Assert.That (replaced.Items [1].GetMetadata ("FileHash"), Is.EqualTo (secondHash), "Same content at new path");
			Assert.That (Hash (cache, first, second).FilesHashed, Is.EqualTo (1), "Removed input not retained in cache");

			var secondTimestamp = File.GetLastWriteTimeUtc (second);
			File.WriteAllText (second, "longer contents");
			File.SetLastWriteTimeUtc (second, secondTimestamp);
			Assert.That (Hash (cache, first, second).FilesHashed, Is.EqualTo (1), "Size change without a timestamp change");
		}

		[Test]
		public void InvalidCacheIsRecomputed ()
		{
			var directory = Cache.CreateTemporaryDirectory ();
			var file = Path.Combine (directory, "input.dll");
			var cache = Path.Combine (directory, "r2r-input-hashes.json");
			File.WriteAllText (file, "contents");
			File.WriteAllText (cache, "invalid json");

			Assert.That (Hash (cache, file).FilesHashed, Is.EqualTo (1), "Invalid cache");
			Assert.That (Engine.Logger.WarningsEvents.Count, Is.EqualTo (1), "Cache warning");
			Assert.That (Hash (cache, file).FilesHashed, Is.Zero, "Rewritten cache");
		}

		[Test]
		public void InvalidCachedHashIsRecomputed ()
		{
			var directory = Cache.CreateTemporaryDirectory ();
			var file = Path.Combine (directory, "input.dll");
			var cache = Path.Combine (directory, "r2r-input-hashes.json");
			File.WriteAllText (file, "contents");
			var expectedHash = Hash (cache, file).Items [0].GetMetadata ("FileHash");
			File.WriteAllText (cache, File.ReadAllText (cache).Replace (expectedHash, "invalid"));

			var task = Hash (cache, file);
			Assert.That (task.FilesHashed, Is.EqualTo (1), "Invalid cached hash");
			Assert.That (Engine.Logger.WarningsEvents.Count, Is.EqualTo (1), "Cache warning");
			Assert.That (task.Items [0].GetMetadata ("FileHash"), Is.EqualTo (expectedHash), "Recomputed hash");
		}

		GetCachedFileHash Hash (string cache, params string [] paths)
		{
			var task = CreateTask<GetCachedFileHash> ();
			task.Files = paths.Select (path => (ITaskItem) new TaskItem (path)).ToArray ();
			task.CacheFile = cache;
			ExecuteTask (task);
			return task;
		}
	}
}
