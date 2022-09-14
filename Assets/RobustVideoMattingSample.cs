/* 
*   Robust Video Matting
*   Copyright (c) 2022 NatML Inc. All Rights Reserved.
*/

namespace NatML.Examples {

    using System.Threading.Tasks;
    using UnityEngine;
    using UnityEngine.UI;
    using NatML.Devices;
    using NatML.Devices.Outputs;
    using NatML.Vision;

    public sealed class RobustVideoMattingSample : MonoBehaviour {

        [Header(@"UI")]
        public RawImage rawImage;
        public AspectRatioFitter aspectFitter;

        private CameraDevice cameraDevice;
        private TextureOutput previewTextureOutput;
        private RenderTexture matteImage;

        private MLModel model;
        private RobustVideoMattingPredictor predictor;

        async void Start () {
            // Request camera permissions
            var permissionStatus = await MediaDeviceQuery.RequestPermissions<CameraDevice>();
            if (permissionStatus != PermissionStatus.Authorized) {
                Debug.LogError(@"User did not grant camera permissions");
                return;
            }
            // Get the default camera device
            var query = new MediaDeviceQuery(MediaDeviceCriteria.CameraDevice);
            cameraDevice = query.current as CameraDevice;
            // Start the camera preview
            cameraDevice.previewResolution = (1280, 720);
            previewTextureOutput = new TextureOutput();
            cameraDevice.StartRunning(previewTextureOutput);
            // Create matte texture
            var previewTexture = await previewTextureOutput;
            matteImage = new RenderTexture(previewTexture.width, previewTexture.height, 0);
            // Display matte texture on UI
            rawImage.texture = matteImage;
            aspectFitter.aspectRatio = (float)previewTexture.width / previewTexture.height;  
            // Create the RVM predictor
            Debug.Log("Fetching model data from NatML...");
            var modelData = await MLModelData.FromHub("@natsuite/robust-video-matting");
            model = modelData.Deserialize();
            predictor = new RobustVideoMattingPredictor(model);
        }

        void Update () {
            // Check that the predictor has been created
            if (predictor == null)
                return;
            // Predict
            var matte = predictor.Predict(previewTextureOutput.texture);
            matte.Render(matteImage);
        }

        void OnDisable () {
            // Dispose the predictor and model
            predictor?.Dispose();
            model?.Dispose();
        }
    }
}