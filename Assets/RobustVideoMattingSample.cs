/* 
*   Robust Video Matting
*   Copyright (c) 2022 NatML Inc. All Rights Reserved.
*/

namespace NatML.Examples {

    using System.Threading.Tasks;
    using UnityEngine;
    using UnityEngine.UI;
    using NatML.VideoKit;
    using NatML.Vision;

    public sealed class RobustVideoMattingSample : MonoBehaviour {

        [Header(@"VideoKit")]
        public VideoKitCameraManager cameraManager;

        [Header(@"UI")]
        public RawImage rawImage;
        public AspectRatioFitter aspectFitter;

        private MLModel model;
        private RobustVideoMattingPredictor predictor;
        private RenderTexture matteTexture;

        private async void Start () {
            // Fetch the model data from Hub
            var modelData = await MLModelData.FromHub("@natsuite/robust-video-matting");
            // Create the model
            model = new MLEdgeModel(modelData);
            // Create the RVM predictor
            predictor = new RobustVideoMattingPredictor(model);
            // Listen for camera frames
            cameraManager.OnFrame.AddListener(OnCameraFrame);
        }

        private void OnCameraFrame (CameraFrame frame) {
            // Predict
            var matte = predictor.Predict(frame);
            // Render the matte to texture
            matteTexture = matteTexture ? matteTexture : new RenderTexture(frame.image.width, frame.image.height, 0);
            matte.Render(matteTexture);
            // Display the matte texture
            rawImage.texture = matteTexture;
            aspectFitter.aspectRatio = (float)matteTexture.width / matteTexture.height;  
        }

        private void OnDisable () {
            // Stop listening for camera frames
            cameraManager.OnFrame.RemoveListener(OnCameraFrame);
            // Dispose the predictor
            predictor?.Dispose();
            // Dispose the model
            model?.Dispose();
        }
    }
}