using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using NUnit.Framework;

using Xamarin.Tests;

public static class GitHub {
	public static string [] GetProjects (string user, string repo, string hash, string default_branch)
	{
		IEnumerable<string> files;

		var dir = CloneRepository (user, repo, hash, default_branch);
		files = Directory.GetFiles (dir, "*.*", SearchOption.AllDirectories);
		files = files.Select ((v) => v.Substring (dir.Length).TrimStart ('/'));

		return files
			.Where ((v) => {
				var ext = Path.GetExtension (v).ToLowerInvariant ();
				return ext == ".csproj" || ext == ".fsproj";
			}).ToArray ();
	}

	public static string CloneRepository (string org, string repo, string hash, string default_branch, bool clean = true)
	{
		var repo_dir = Path.Combine (Configuration.SampleRootDirectory, repo);

		if (!Directory.Exists (repo_dir)) {
			Console.WriteLine ($"Cloning https://github.com/{org}/{repo} (hash: {hash})");
			Directory.CreateDirectory (repo_dir);
			Assert.AreEqual (0, ExecutionHelper.Execute ("git", new string [] { "init" }, working_directory: repo_dir, timeout: TimeSpan.FromSeconds (10)), "git init");
			Assert.AreEqual (0, ExecutionHelper.Execute ("git", new string [] { "remote", "add", "origin", $"https://github.com/{org}/{repo}" }, working_directory: repo_dir, timeout: TimeSpan.FromSeconds (10)), "git remote add");
			Assert.AreEqual (0, ExecutionHelper.Execute ("git", new string [] { "fetch" }, working_directory: repo_dir, timeout: TimeSpan.FromMinutes (10)), "git fetch");
			Assert.AreEqual (0, ExecutionHelper.Execute ("git", new string [] { "checkout", "-b", "temporary-sample-testing-branch", hash }, working_directory: repo_dir, timeout: TimeSpan.FromMinutes (1)), "git checkout");
			Assert.AreEqual (0, ExecutionHelper.Execute ("git", new string [] { "submodule", "update", "--init", "--recursive" }, working_directory: repo_dir, timeout: TimeSpan.FromMinutes (10)), "git submodule update");

			ExecutionHelper.Execute ("git", new string [] { "log", "--oneline", "--pretty=* @%h %s", "HEAD", $"^origin/{default_branch}" }, out var output1, working_directory: repo_dir, timeout: TimeSpan.FromSeconds (10));
			if (output1.Length > 0) {
				Console.WriteLine ($"Commits not in origin/{default_branch}:");
				Console.WriteLine (output1.ToString ().TrimEnd ('\n'));
			}
			ExecutionHelper.Execute ("git", new string [] { "log", "--oneline", "--pretty=* %h %s", $"origin/{default_branch}", "^HEAD" }, out var output2, working_directory: repo_dir, timeout: TimeSpan.FromSeconds (10));
			if (output2.Length > 0) {
				Console.WriteLine ($"Commits in origin/{default_branch} not being tested:");
				Console.WriteLine (output2.ToString ().TrimEnd ('\n'));
			}

		} else if (clean) {
			Assert.AreEqual (0, ExecutionHelper.Execute ("git", new string [] { "reset", "--hard", hash }, working_directory: repo_dir, timeout: TimeSpan.FromMinutes (1)), "git checkout");
			CleanRepository (repo_dir);
		}

		return repo_dir;
	}

	public static void CleanRepository (string directory, bool submodules = true)
	{
		ExecutionHelper.Execute ("git", new string [] { "clean", "-xffdq" }, working_directory: directory, timeout: TimeSpan.FromSeconds (30));
		if (submodules)
			ExecutionHelper.Execute ("git", new string [] { "submodule", "foreach", "--recursive", "clean -xffdq" }, working_directory: directory, timeout: TimeSpan.FromSeconds (60));
	}
}
