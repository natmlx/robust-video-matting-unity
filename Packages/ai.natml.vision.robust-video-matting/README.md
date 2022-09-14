# Robust Video Matting
[Robust Video Matting](https://peterl1n.github.io/RobustVideoMatting/) for human segmentation. This package requires [NatML](https://github.com/natmlx/NatML).

## Matting People in an Image
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
- Unity 2020.3+
- [NatML 1.0.11+](https://github.com/natmlx/NatML)

## Quick Tips
- Discover more ML models on [NatML Hub](https://hub.natml.ai).
- See the [NatML documentation](https://docs.natml.ai/unity).
- Join the [NatML community on Discord](https://discord.gg/y5vwgXkz2f).
- Discuss [NatML on Unity Forums](https://forum.unity.com/threads/open-beta-natml-machine-learning-runtime.1109339/).
- Contact us at [hi@natml.ai](mailto:hi@natml.ai).

Thank you very much!