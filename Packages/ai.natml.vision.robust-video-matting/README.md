# Robust Video Matting
[Robust Video Matting](https://peterl1n.github.io/RobustVideoMatting/) for human segmentation.

## Installing Robust Video Matting
Add the following items to your Unity project's `Packages/manifest.json`:
```json
{
  "scopedRegistries": [
    {
      "name": "NatML",
      "url": "https://registry.npmjs.com",
      "scopes": ["ai.natml"]
    }
  ],
  "dependencies": {
    "ai.natml.vision.robust-video-matting": "1.0.1"
  }
}
```

## Predicting the Matte
First, create the Robust Video Matting predictor:
```csharp
// Fetch the model data from Hub
var modelData = await MLModelData.FromHub("@natsuite/robust-video-matting");
// Deserialize the model
var model = modelData.Deserialize();
// Create the Robust Video Matting predictor
var predictor = new RobustVideoMattingPredictor(model);
```

Predict the matte for an image:
```csharp
// Compute the matte
Texture2D image = ...; // This can also be a WebCamTexture or an MLImageFeature
RobustVideoMattingPredictor.Matte matte = predictor.Predict(image);
```

Finally, render the predicted matte to a `RenderTexture`:
```csharp
// Visualize the matte in a `RenderTexture`
var result = new RenderTexture(image.width, image.height, 0);
matte.Render(result);
```

___

## Requirements
- Unity 2021.2+

## Quick Tips
- Discover more ML models on [NatML Hub](https://hub.natml.ai).
- See the [NatML documentation](https://docs.natml.ai/unity).
- Join the [NatML community on Discord](https://hub.natml.ai/community).
- Discuss [NatML on Unity Forums](https://forum.unity.com/threads/open-beta-natml-machine-learning-runtime.1109339/).
- Contact us at [hi@natml.ai](mailto:hi@natml.ai).

Thank you very much!