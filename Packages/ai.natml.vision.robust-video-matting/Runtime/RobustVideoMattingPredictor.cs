/* 
*   Robust Video Matting
*   Copyright (c) 2022 NatML Inc. All Rights Reserved.
*/

namespace NatML.Vision {

    using System;
    using NatML.Features;
    using NatML.Internal;
    using NatML.Types;

    /// <summary>
    /// Robust Video Matting for human segmentation.
    /// </summary>
    public sealed partial class RobustVideoMattingPredictor : IMLPredictor<RobustVideoMattingPredictor.Matte> {

        #region --Client API--
        /// <summary>
        /// Create the Robust Video Matting predictor.
        /// </summary>
        /// <param name="model">Robust Video Matting ML model.</param>
        public RobustVideoMattingPredictor (MLModel model) {
            this.model = model as MLEdgeModel;
            var recurrentStateTypes = new [] {
                new MLArrayType(new [] { 1, 16, 135, 240 }, typeof(float)),
                new MLArrayType(new [] { 1, 20, 68, 120 }, typeof(float)),
                new MLArrayType(new [] { 1, 40, 34, 60 }, typeof(float)),
                new MLArrayType(new [] { 1, 64, 17, 30 }, typeof(float))
            };
            this.recurrentState = new MLEdgeFeature[recurrentStateTypes.Length];
            for (var i = 0; i < recurrentStateTypes.Length; ++i) {
                var type = recurrentStateTypes[i];
                var feature = new MLArrayFeature<float>(new float[type.elementCount]);
                recurrentState[i] = (feature as IMLEdgeFeature).Create(type);
            }
        }

        /// <summary>
        /// Compute a human alpha matte on an image.
        /// </summary>
        /// <param name="inputs">Input image.</param>
        /// <returns>Alpha matte.</returns>
        public Matte Predict (params MLFeature[] inputs) {
            // Check
            if (inputs.Length != 1)
                throw new ArgumentException(@"Robust Video Matting predictor expects a single feature", nameof(inputs));
            // Check type
            var input = inputs[0];
            if (!MLImageType.FromType(input.type))
                throw new ArgumentException(@"Robust Video Matting predictor expects an an array or image feature", nameof(inputs));
            // Predict
            using var imageFeature = (input as IMLEdgeFeature).Create(model.inputs[0]);
            var outputFeatures = model.Predict(imageFeature, recurrentState[0], recurrentState[1], recurrentState[2], recurrentState[3]);
            // Update recurrent state
            for (var i = 0; i < recurrentState.Length; ++i) {
                recurrentState[i].Dispose();
                recurrentState[i] = outputFeatures[i + 2];
            }
            // Marshal
            var matte = new MLArrayFeature<float>(outputFeatures[1]);   // (1,1,H,W)
            var result = new Matte(matte.shape[3], matte.shape[2], matte.ToArray());
            // Release
            outputFeatures[0].Dispose();
            outputFeatures[1].Dispose();
            // Return
            return result;
        }

        /// <summary>
        /// Dispose the predictor and release resources.
        /// </summary>
        public void Dispose () {
            for (var i = 0; i < recurrentState.Length; ++i)
                recurrentState[i].Dispose();
        }
        #endregion


        #region --Operations--
        private readonly MLEdgeModel model;
        private MLEdgeFeature[] recurrentState;
        #endregion
    }
}