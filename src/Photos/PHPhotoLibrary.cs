//
// PHPhotoLibrary.cs: Provides a couple of overload methods
//
// Authors:
//   Miguel de Icaza
//
// Copyright 2014 Xamarin Inc
//

#if !MONOMAC

using ObjCRuntime;
using Foundation;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Photos
{
	public partial class PHPhotoLibrary
	{
		class __phlib_observer : PHPhotoLibraryChangeObserver {
			Action<PHChange> observer;
			
			public __phlib_observer (Action<PHChange> observer)
			{
				this.observer = observer;
			}

			public override void PhotoLibraryDidChange (PHChange changeInstance)
			{
				observer (changeInstance);
			}
		}
		
		public object RegisterChangeObserver (Action<PHChange> changeObserver)
		{
			var token = new __phlib_observer (changeObserver);
			RegisterChangeObserver (token);
			return token;
		}

		public void UnregisterChangeObserver (object registeredToken)
		{
			if (!(registeredToken is __phlib_observer))
				throw new ArgumentException ("registeredToken should be a value returned by RegisterChangeObserver(PHChange)");
			
			UnregisterChangeObserver (registeredToken as __phlib_observer);
		}
	}
}

#endif
