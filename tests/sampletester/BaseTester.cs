using System.Reflection;

using NUnit.Framework;

namespace Xamarin.Tests {
	[TestFixture]
	[Parallelizable (ParallelScope.Self)]
	public abstract class BaseTester {
		string repository;
		public virtual string Repository {
			get {
				if (repository is null)
					repository = (string) GetType ().GetField ("REPO", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static).GetValue (null);
				return repository;
			}
			protected set {
				repository = value;
			}
		}

		string hash;
		public virtual string Hash {
			get {
				if (hash is null)
					hash = (string) GetType ().GetField ("HASH", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static).GetValue (null);
				return hash;
			}
			protected set {
				hash = value;
			}
		}

		string org;
		public virtual string Org {
			get {
				if (org is null)
					org = (string) GetType ().GetField ("ORG", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)?.GetValue (null) ?? "xamarin";
				return org;
			}
			protected set {
				org = value;
			}
		}

		string default_branch;
		public virtual string DefaultBranch {
			get {
				if (default_branch is null)
					default_branch = (string) GetType ().GetField ("DEFAULT_BRANCH", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)?.GetValue (null);
				return default_branch;

			}
			set {
				default_branch = value;
			}
		}

		protected BaseTester ()
		{
		}

		protected BaseTester (string repository, string hash)
		{
			this.repository = repository;
			this.hash = hash;
		}
	}
}
