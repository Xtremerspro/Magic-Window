using System.ComponentModel;
using UnityEditor.Analytics;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MagicWindow : MonoBehaviour
{
    public enum ScreenRatio { Custom, R16_9, R16_10, R21_9, R4_3, R16_6 }

    public UDPHeadTracker Tracker;

    [Header("Window Dimensions")]
    public float DiagonalInches = 24f;
    public ScreenRatio AspectRatio = ScreenRatio.R16_9;

    public float WindowWidth;  // Calculated automatically
    public float WindowHeight; // Calculated automatically

    public float GameScale = 1f;

    [Header("Calibration")]
    public Transform CameraOffset;

    private Camera _cam;

    // This runs in the Editor whenever you change a value
    private void OnValidate()
    {
        CalculateDimensions();
    }

    void Start()
    {
        Application.targetFrameRate = 165; // Or 120, depending on your monitor
        _cam = GetComponent<Camera>();
        CalculateDimensions();
    }

    void CalculateDimensions()
    {
        // Convert diagonal inches to meters (1 inch = 0.0254 meters)
        float diagonalMeters = DiagonalInches * 0.0254f;
        float ratioValue = 16f / 9f; // Default

        switch (AspectRatio)
        {
            case ScreenRatio.R16_9: ratioValue = 16f / 9f; break;
            case ScreenRatio.R16_10: ratioValue = 16f / 10f; break;
            case ScreenRatio.R21_9: ratioValue = 21f / 9f; break;
            case ScreenRatio.R4_3: ratioValue = 4f / 3f; break;
            case ScreenRatio.R16_6: ratioValue = 16f / 6f; break;
            case ScreenRatio.Custom: return; // Manual entry if you choose custom
        }

        // Math: Height = D / sqrt(R^2 + 1)
        WindowHeight = (diagonalMeters / Mathf.Sqrt(Mathf.Pow(ratioValue, 2) + 1)) * GameScale;
        WindowWidth = WindowHeight * ratioValue;
    }

    void LateUpdate()
    {
        if (Tracker == null || !Tracker.IsTracking) return;

        // 1. Calculate Head Position with Rotation/Position Offset
        Vector3 headPos = CameraOffset.TransformPoint(Tracker.LatestHeadPos) * GameScale;

        // 2. Define the corners of the "Window"
        float left = -WindowWidth / 2.0f;
        float right = WindowWidth / 2.0f;
        float bottom = -WindowHeight / 2.0f;
        float top = WindowHeight / 2.0f;

        // 3. Calculate Off-Axis Projection
        float near = _cam.nearClipPlane;
        float distanceToScreen = Mathf.Abs(headPos.z);
        if (distanceToScreen < 0.01f) distanceToScreen = 0.01f;

        float n_over_d = near / distanceToScreen;

        float l = (left - headPos.x) * n_over_d;
        float r = (right - headPos.x) * n_over_d;
        float b = (bottom - headPos.y) * n_over_d;
        float t = (top - headPos.y) * n_over_d;

        _cam.projectionMatrix = Matrix4x4.Frustum(l, r, b, t, near, _cam.farClipPlane);

        // 4. Update Camera Position
        transform.localPosition = headPos;
        transform.localRotation = Quaternion.identity;
    }
}