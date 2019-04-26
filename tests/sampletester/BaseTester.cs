using System.Reflection;

using NUnit.Framework;

namespace Xamarin.Tests {
	[TestFixture]
	[Parallelizable (ParallelScope.Self)]
	public abstract class BaseTester {
		string repository;
		public virtual string Repository {
			get {
				if (repository == null)
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
				if (hash == null)
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
				if (org == null)
					org = (string) GetType ().GetField ("ORG", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)?.GetValue (null) ?? "xamarin";
				return org;
			}
			protected set {
				org = value;
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
