/* 
*   Robust Video Matting
*   Copyright © 2023 NatML Inc. All Rights Reserved.
*/

namespace NatML.Examples {

    using UnityEngine;
    using UnityEngine.UI;
    using NatML.Vision;
    using VideoKit;

    public sealed class RobustVideoMattingSample : MonoBehaviour {

        [Header(@"VideoKit")]
        public VideoKitCameraManager cameraManager;

        [Header(@"UI")]
        public RawImage rawImage;
        public AspectRatioFitter aspectFitter;

        private RobustVideoMattingPredictor predictor;
        private RenderTexture matteTexture;

        private async void Start () {
            // Create the RVM predictor
            predictor = await RobustVideoMattingPredictor.Create();
            // Listen for camera frames
            cameraManager.OnCameraFrame.AddListener(OnCameraFrame);
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
            cameraManager.OnCameraFrame.RemoveListener(OnCameraFrame);
            // Dispose the predictor
            predictor?.Dispose();
        }
    }
}