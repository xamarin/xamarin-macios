// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#nullable enable

using System;

using Foundation;
using Metal;
using ObjCRuntime;

namespace MetalPerformanceShaders {
	partial class MPSNNOptimizerStochasticGradientDescent {
#if !XAMCORE_5_0
		/// <summary>Create a new <see cref="MPSNNOptimizerStochasticGradientDescent" /> instance.</summary>
		/// <param name="device">The device where the filter will run.</param>
		/// <param name="momentumScale">The scale to use when updating the momentum for the values.</param>
		/// <param name="useNestrovMomentum">The Nesterov-style momentum update.</param>
		/// <param name="optimizerDescriptor">Any additional properties to apply.</param>
		public MPSNNOptimizerStochasticGradientDescent (IMTLDevice device, float momentumScale, bool useNestrovMomentum, MPSNNOptimizerDescriptor optimizerDescriptor)
#else
		/// <summary>Create a new <see cref="MPSNNOptimizerStochasticGradientDescent" /> instance.</summary>
		/// <param name="device">The device where the filter will run.</param>
		/// <param name="momentumScale">The scale to use when updating the momentum for the values.</param>
		/// <param name="useNesterovMomentum">The Nesterov-style momentum update.</param>
		/// <param name="optimizerDescriptor">Any additional properties to apply.</param>
		public MPSNNOptimizerStochasticGradientDescent (IMTLDevice device, float momentumScale, bool useNesterovMomentum, MPSNNOptimizerDescriptor optimizerDescriptor)
#endif
			: this (NSObjectFlag.Empty)
		{
#if !XAMCORE_5_0
			var useNesterovMomentum = useNestrovMomentum;
#endif

			InitializeHandle (_InitWithNesterovMomentum (device, momentumScale, useNesterovMomentum, optimizerDescriptor), "initWithDevice:momentumScale:useNestrovMomentum:optimizerDescriptor:");
		}
	}
}
