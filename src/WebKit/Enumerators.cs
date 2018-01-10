using System;
using System.Collections;
using System.Collections.Generic;
using CoreFoundation;
using Foundation;
using ObjCRuntime;

namespace WebKit {

	public interface IIndexedContainer<T> {
		int Count { get; }
		T this [int index] { get; }
	}

	internal class IndexedContainerEnumerator<T> : IEnumerator<T> {
		public IndexedContainerEnumerator (IIndexedContainer<T> container) {
			_container = container;
			Reset ();
		}

		public void Dispose () {
			_container = null;
		}

		public T Current {
			get {
				return _container [_index];
			}
		}

		object IEnumerator.Current {
			get { return ((IEnumerator<T>) this).Current; }
		}

		public bool MoveNext () {
			return ++_index < _container.Count;
		}

		public void Reset () {
			_index = -1;
		}

		IIndexedContainer<T> _container;
		int _index;
	}

	public partial class DomCssRuleList : IIndexedContainer<DomCssRule>, IEnumerable<DomCssRule> {
		public IEnumerator<DomCssRule> GetEnumerator () {
			return new IndexedContainerEnumerator<DomCssRule> (this);
		}

		IEnumerator IEnumerable.GetEnumerator () {
			return ((IEnumerable<DomCssRule>) this).GetEnumerator ();
		}
	}

	public partial class DomCssStyleDeclaration : IIndexedContainer<string>, IEnumerable<string> {
		public IEnumerator<string> GetEnumerator () {
			return new IndexedContainerEnumerator<string> (this);
		}

		IEnumerator IEnumerable.GetEnumerator () {
			return ((IEnumerable<string>) this).GetEnumerator ();
		}
	}

	public partial class DomHtmlCollection : IIndexedContainer<DomNode>, IEnumerable<DomNode> {
		public IEnumerator<DomNode> GetEnumerator () {
			return new IndexedContainerEnumerator<DomNode> (this);
		}

		IEnumerator IEnumerable.GetEnumerator () {
			return ((IEnumerable<DomNode>) this).GetEnumerator ();
		}
	}

	public partial class DomMediaList : IIndexedContainer<string>, IEnumerable<string> {
		public IEnumerator<string> GetEnumerator () {
			return new IndexedContainerEnumerator<string> (this);
		}

		IEnumerator IEnumerable.GetEnumerator () {
			return ((IEnumerable<string>) this).GetEnumerator ();
		}
	}

	public partial class DomNamedNodeMap : IIndexedContainer<DomNode>, IEnumerable<DomNode> {
		public IEnumerator<DomNode> GetEnumerator () {
			return new IndexedContainerEnumerator<DomNode> (this);
		}

		IEnumerator IEnumerable.GetEnumerator () {
			return ((IEnumerable<DomNode>) this).GetEnumerator ();
		}
	}

	public partial class DomNodeList : IIndexedContainer<DomNode>, IEnumerable<DomNode> {
		public IEnumerator<DomNode> GetEnumerator () {
			return new IndexedContainerEnumerator<DomNode> (this);
		}

		IEnumerator IEnumerable.GetEnumerator () {
			return ((IEnumerable<DomNode>) this).GetEnumerator ();
		}
	}

	public partial class DomStyleSheetList : IIndexedContainer<DomStyleSheet>, IEnumerable<DomStyleSheet> {
		public IEnumerator<DomStyleSheet> GetEnumerator () {
			return new IndexedContainerEnumerator<DomStyleSheet> (this);
		}

		IEnumerator IEnumerable.GetEnumerator () {
			return ((IEnumerable<DomStyleSheet>) this).GetEnumerator ();
		}
	}
}
