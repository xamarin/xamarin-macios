using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;

using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;

using Foundation;
using ObjCRuntime;
#if !__TVOS__
using MapKit;
#endif
#if !__WATCHOS__
using CoreAnimation;
#endif
using CoreGraphics;
using CoreLocation;
#if !__WATCHOS__
#endif
using NUnit.Framework;

namespace MonoTouchFixtures {
	[TestFixture]
	[Preserve (AllMembers = true)]
	public partial class MonoRuntimeTests {
		[Test]
		public void Bug18632 ()
		{
			var hit = false;
			var ret1 = Observable.Return ("1");
			Observable.CombineLatest (ret1, ret1, ret1, (a, b, c) => {
				return true;
			}).Subscribe (new AnonymousObserver ());

			var ret2 = Observable.Return (1);
			Observable.CombineLatest (ret2, ret2, ret2, (a, b, c) => {
				hit = true;
				return true;
			}).Subscribe (new AnonymousObserver ());

			Assert.IsTrue (hit, "The second CombineLatest callback wasn't called.");
		}

		class QueryLanguage : IQueryLanguage {
			public virtual Producer<TResult> CombineLatest<TSource1, TSource2, TSource3, TResult> (Producer<TSource1> source1, Producer<TSource2> source2, Producer<TSource3> source3, Func<TSource1, TSource2, TSource3, TResult> resultSelector)
			{
				return new CombineLatest<TSource1, TSource2, TSource3, TResult> (source1, source2, source3, resultSelector);
			}

			public virtual Producer<TResult> Return<TResult> (TResult value)
			{
				return new Return<TResult> (value);
			}
		}

		interface IQueryLanguage {
			Producer<TResult> Return<TResult> (TResult value);
			Producer<TResult> CombineLatest<T1, T2, T3, TResult> (Producer<T1> source1, Producer<T2> source2, Producer<T3> source3, Func<T1, T2, T3, TResult> resultSelector);
		}

		static class Observable {
			static IQueryLanguage s_impl = new QueryLanguage ();
			public static Producer<TResult> Return<TResult> (TResult value)
			{
				return s_impl.Return<TResult> (value);
			}
			public static Producer<TResult> CombineLatest<TSource1, TSource2, TSource3, TResult> (Producer<TSource1> source1, Producer<TSource2> source2, Producer<TSource3> source3, Func<TSource1, TSource2, TSource3, TResult> resultSelector)
			{
				return s_impl.CombineLatest<TSource1, TSource2, TSource3, TResult> (source1, source2, source3, resultSelector);
			}
		}

		class Return<TResult> : Producer<TResult> {
			private readonly TResult _value;

			public Return (TResult value)
			{
				_value = value;
			}

			protected override IDisposable Run (IObserver<TResult> observer)
			{
				var sink = new _ (this, observer);
				return sink.Run ();
			}

			class _ : Sink<TResult> {
				private readonly Return<TResult> _parent;

				public _ (Return<TResult> parent, IObserver<TResult> observer)
					: base (observer)
				{
					_parent = parent;
				}

				public IDisposable Run ()
				{
					Invoke ();
					return null;
				}

				private void Invoke ()
				{
					base._observer.OnNext (_parent._value);
					base._observer.OnCompleted ();
				}
			}
		}

		class CombineLatest<T1, T2, T3, TResult> : Producer<TResult> {
			private readonly Producer<T1> _source1;
			private readonly Producer<T2> _source2;
			private readonly Producer<T3> _source3;
			private readonly Func<T1, T2, T3, TResult> _resultSelector;

			public CombineLatest (Producer<T1> source1, Producer<T2> source2, Producer<T3> source3, Func<T1, T2, T3, TResult> resultSelector)
			{
				_source1 = source1;
				_source2 = source2;
				_source3 = source3;
				_resultSelector = resultSelector;
			}

			protected override IDisposable Run (IObserver<TResult> observer)
			{
				var sink = new _ (this, observer);
				return sink.Run ();
			}

			class _ : CombineLatestSink<TResult> {
				private readonly CombineLatest<T1, T2, T3, TResult> _parent;

				public _ (CombineLatest<T1, T2, T3, TResult> parent, IObserver<TResult> observer)
					: base (3, observer)
				{
					_parent = parent;
				}

				private CombineLatestObserver<T1> _observer1;
				private CombineLatestObserver<T2> _observer2;
				private CombineLatestObserver<T3> _observer3;

				public IDisposable Run ()
				{
					_observer1 = new CombineLatestObserver<T1> (_gate, this, 0);
					_observer2 = new CombineLatestObserver<T2> (_gate, this, 1);
					_observer3 = new CombineLatestObserver<T3> (_gate, this, 2);

					_parent._source1.Subscribe (_observer1);
					_parent._source2.Subscribe (_observer2);
					_parent._source3.Subscribe (_observer3);

					return null;
				}

				protected override TResult GetResult ()
				{
					return _parent._resultSelector (_observer1.Value, _observer2.Value, _observer3.Value);
				}
			}
		}

		interface ICombineLatest {
			void Next (int index);
			void Fail (Exception error);
			void Done (int index);
		}

		abstract class CombineLatestSink<TResult> : Sink<TResult>, ICombineLatest {
			protected readonly object _gate;

			private bool _hasValueAll;
			private readonly bool [] _hasValue;
			private readonly bool [] _isDone;

			public CombineLatestSink (int arity, IObserver<TResult> observer)
				: base (observer)
			{
				_gate = new object ();

				_hasValue = new bool [arity];
				_isDone = new bool [arity];
			}

			public void Next (int index)
			{
				if (!_hasValueAll) {
					_hasValue [index] = true;

					var hasValueAll = true;
					foreach (var hasValue in _hasValue) {
						if (!hasValue) {
							hasValueAll = false;
							break;
						}
					}

					_hasValueAll = hasValueAll;
				}

				if (_hasValueAll) {
					var res = default (TResult);
					try {
						res = GetResult ();
					} catch (Exception ex) {
						base._observer.OnError (ex);
						return;
					}

					base._observer.OnNext (res);
				} else {
					var allOthersDone = true;
					for (int i = 0; i < _isDone.Length; i++) {
						if (i != index && !_isDone [i]) {
							allOthersDone = false;
							break;
						}
					}

					if (allOthersDone) {
						base._observer.OnCompleted ();
					}
				}
			}

			protected abstract TResult GetResult ();

			public void Fail (Exception error)
			{
				base._observer.OnError (error);

			}

			public void Done (int index)
			{
				_isDone [index] = true;

				var allDone = true;
				foreach (var isDone in _isDone) {
					if (!isDone) {
						allDone = false;
						break;
					}
				}

				if (allDone) {
					base._observer.OnCompleted ();
				}
			}
		}

		class CombineLatestObserver<T> : IObserver<T> {
			private readonly object _gate;
			private readonly ICombineLatest _parent;
			private readonly int _index;
			private T _value;

			public CombineLatestObserver (object gate, ICombineLatest parent, int index)
			{
				_gate = gate;
				_parent = parent;
				_index = index;
			}

			public T Value {
				get { return _value; }
			}

			public void OnNext (T value)
			{
				lock (_gate) {
					_value = value;
					_parent.Next (_index);
				}
			}

			public void OnError (Exception error)
			{
				lock (_gate) {
					_parent.Fail (error);
				}
			}

			public void OnCompleted ()
			{
				lock (_gate) {
					_parent.Done (_index);
				}
			}
		}

		class CombineLatest<TSource, TResult> : Producer<TResult> {
			private readonly IEnumerable<Producer<TSource>> _sources;
			private readonly Func<IList<TSource>, TResult> _resultSelector;

			public CombineLatest (IEnumerable<Producer<TSource>> sources, Func<IList<TSource>, TResult> resultSelector)
			{
				_sources = sources;
				_resultSelector = resultSelector;
			}

			protected override IDisposable Run (IObserver<TResult> observer)
			{
				var sink = new _ (this, observer);
				return sink.Run ();
			}

			class _ : Sink<TResult> {
				private readonly CombineLatest<TSource, TResult> _parent;

				public _ (CombineLatest<TSource, TResult> parent, IObserver<TResult> observer)
					: base (observer)
				{
					_parent = parent;
				}

				private object _gate;
				private bool [] _hasValue;
				private bool _hasValueAll;
				private List<TSource> _values;
				private bool [] _isDone;

				public IDisposable Run ()
				{
					var srcs = _parent._sources.ToArray ();

					var N = srcs.Length;

					_hasValue = new bool [N];
					_hasValueAll = false;

					_values = new List<TSource> (N);
					for (int i = 0; i < N; i++)
						_values.Add (default (TSource));

					_isDone = new bool [N];

					_gate = new object ();

					for (int i = 0; i < N; i++) {
						var j = i;

						var o = new O (this, j);
						srcs [j].Subscribe (o);
					}

					return null;
				}

				private void OnNext (int index, TSource value)
				{
					lock (_gate) {
						_values [index] = value;

						_hasValue [index] = true;

						if (_hasValueAll || (_hasValueAll = _hasValue.All (__ => __))) {
							var res = default (TResult);
							try {
								res = _parent._resultSelector (new ReadOnlyCollection<TSource> (_values));
							} catch (Exception ex) {
								base._observer.OnError (ex);
								return;
							}

							_observer.OnNext (res);
						} else if (_isDone.Where ((x, i) => i != index).All (__ => __)) {
							base._observer.OnCompleted ();
							return;
						}
					}
				}

				private void OnError (Exception error)
				{
					lock (_gate) {
						base._observer.OnError (error);
					}
				}

				private void OnCompleted (int index)
				{
					lock (_gate) {
						_isDone [index] = true;

						if (_isDone.All (__ => __)) {
							base._observer.OnCompleted ();
							return;
						} else {

						}
					}
				}

				class O : IObserver<TSource> {
					private readonly _ _parent;
					private readonly int _index;

					public O (_ parent, int index)
					{
						_parent = parent;
						_index = index;
					}

					public void OnNext (TSource value)
					{
						_parent.OnNext (_index, value);
					}

					public void OnError (Exception error)
					{
						_parent.OnError (error);
					}

					public void OnCompleted ()
					{
						_parent.OnCompleted (_index);
					}
				}
			}
		}

		internal abstract class Sink<TSource> {
			protected internal volatile IObserver<TSource> _observer;

			public Sink (IObserver<TSource> observer)
			{
				_observer = observer;
			}

			class _ : IObserver<TSource> {
				private readonly Sink<TSource> _forward;

				public _ (Sink<TSource> forward)
				{
					_forward = forward;
				}

				public void OnNext (TSource value)
				{
					_forward._observer.OnNext (value);
				}

				public void OnError (Exception error)
				{
					_forward._observer.OnError (error);
				}

				public void OnCompleted ()
				{
					_forward._observer.OnCompleted ();
				}
			}
		}

		abstract class Producer<TSource> : IObservable<TSource> {
			public IDisposable Subscribe (IObserver<TSource> observer)
			{
				this.Run (observer);

				return null;
			}

			protected abstract IDisposable Run (IObserver<TSource> observer);
		}

		sealed class AnonymousObserver : IObserver<bool> {
			public void OnNext (bool value)
			{
			}

			public void OnError (Exception error)
			{
			}

			public void OnCompleted ()
			{
			}
		}
	}
}
