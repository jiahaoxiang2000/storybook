using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Ensures Canvas uses proper scaling for consistent display across different screen sizes.
/// Attach this to any GameObject in each scene (or use an Editor script to fix all scenes).
/// </summary>
[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(CanvasScaler))]
public class CanvasScalerSetup : MonoBehaviour
{
    void Awake()
    {
        CanvasScaler scaler = GetComponent<CanvasScaler>();

        // Set to Scale With Screen Size mode
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;

        // Set reference resolution (16:9 aspect ratio for WebGL)
        scaler.referenceResolution = new Vector2(1920, 1080);

        // Match width or height (0.5 = balanced)
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        Debug.Log($"Canvas Scaler configured: {scaler.referenceResolution}, mode: {scaler.uiScaleMode}");
    }
}
