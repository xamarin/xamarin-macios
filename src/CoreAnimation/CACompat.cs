#if !XAMCORE_4_0

using System;

using Foundation;
using ObjCRuntime;
using CoreGraphics;

namespace CoreAnimation {

	partial class CAScrollLayer {

		[Obsolete ("Use CAScroll enum")]
		public static NSString ScrollBoth {
			get { return CAScroll.Both.GetConstant (); }
		}

		[Obsolete ("Use CAScroll enum")]
		public static NSString ScrollHorizontally {
			get { return CAScroll.Horizontally.GetConstant (); }
		}

		[Obsolete ("Use CAScroll enum")]
		public static NSString ScrollNone {
			get { return CAScroll.None.GetConstant (); }
		}

		[Obsolete ("Use CAScroll enum")]
		public static NSString ScrollVertically {
			get { return CAScroll.Vertically.GetConstant (); }
		}
	}

	partial class CAAnimation {
		// cannot be handled by the generator (error BI1110 because it's not an protocol/interface)
		public CAAnimationDelegate Delegate {
			get { return WeakDelegate as CAAnimationDelegate; }
			set { WeakDelegate = value; }
		}

		// lack of generated delegate force us to add events manually too

		_CAAnimationDelegate EnsureCAAnimationDelegate ()
		{
			var del = Delegate;
			if (del == null || (!(del is _CAAnimationDelegate))){
				del = new _CAAnimationDelegate ();
				Delegate = del;
			}
			return (_CAAnimationDelegate) del;
		}

#pragma warning disable 672
		[Register]
		sealed class _CAAnimationDelegate : CAAnimationDelegate { 
			public _CAAnimationDelegate () { IsDirectBinding = false; }

			internal EventHandler animationStarted;
			[Preserve (Conditional = true)]
			public override void AnimationStarted (CAAnimation anim)
			{
				EventHandler handler = animationStarted;
				if (handler != null){
					handler (anim, EventArgs.Empty);
				}
			}

			internal EventHandler<CAAnimationStateEventArgs> animationStopped;
			[Preserve (Conditional = true)]
			public override void AnimationStopped (CAAnimation anim, bool finished)
			{
				EventHandler<CAAnimationStateEventArgs> handler = animationStopped;
				if (handler != null){
					var args = new CAAnimationStateEventArgs (finished);
					handler (anim, args);
				}
			}

		}
#pragma warning restore 672

		public event EventHandler AnimationStarted {
			add { EnsureCAAnimationDelegate ().animationStarted += value; }
			remove { EnsureCAAnimationDelegate ().animationStarted -= value; }
		}

		public event EventHandler<CAAnimationStateEventArgs> AnimationStopped {
			add { EnsureCAAnimationDelegate ().animationStopped += value; }
			remove { EnsureCAAnimationDelegate ().animationStopped -= value; }
		}
	}

	public partial class CAAnimationStateEventArgs : EventArgs {
		public CAAnimationStateEventArgs (bool finished)
		{
			this.Finished = finished;
		}
		public bool Finished { get; set; }
	}
}

#endif
