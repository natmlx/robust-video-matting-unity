/* 
*   Robust Video Matting
*   Copyright © 2023 NatML Inc. All Rights Reserved.
*/

namespace NatML.Vision {

    using System;
    using System.Threading.Tasks;
    using NatML.Features;
    using NatML.Internal;
    using NatML.Types;

    /// <summary>
    /// Robust Video Matting for human segmentation.
    /// </summary>
    public sealed partial class RobustVideoMattingPredictor : IMLPredictor<RobustVideoMattingPredictor.Matte> {

        #region --Client API--
        /// <summary>
        /// Predictor tag.
        /// </summary>
        public const string Tag = "@natsuite/robust-video-matting";

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
            using var inputFeature = (input as IMLEdgeFeature).Create(model.inputs[0]);
            var outputFeatures = model.Predict(inputFeature, recurrentState[0], recurrentState[1], recurrentState[2], recurrentState[3]);
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
            model.Dispose();
        }

        /// <summary>
        /// Create the Robust Video Matting predictor.
        /// </summary>
        /// <param name="configuration">Model configuration.</param>
        /// <param name="accessKey">NatML access key.</param>
        public static async Task<RobustVideoMattingPredictor> Create (
            MLEdgeModel.Configuration configuration = null,
            string accessKey = null
        ) {
            var model = await MLEdgeModel.Create(Tag, configuration, accessKey);
            var predictor = new RobustVideoMattingPredictor(model);
            return predictor;
        }
        #endregion


        #region --Operations--
        private readonly MLEdgeModel model;
        private MLEdgeFeature[] recurrentState;

        private RobustVideoMattingPredictor (MLEdgeModel model) {
            this.model = model;
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
        #endregion
    }
}