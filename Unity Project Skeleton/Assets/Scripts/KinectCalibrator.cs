using UnityEngine;
using System.Collections;

public class KinectCalibrator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform kinectRawHead; // The raw tracker data

    [Header("Settings")]
    [SerializeField] private KeyCode calibrationKey = KeyCode.C;
    [SerializeField] private float calibrationDelay = 2.0f;

    // These store the "Snapshot" of the Kinect's world when you are at center
    private Vector3 kinectOriginOffset;
    private Quaternion kinectRotationOffset = Quaternion.identity;

    public Vector3 CalibratedPosition { get; private set; }
    public Quaternion CalibratedRotation { get; private set; }

    void Update()
    {
        if (Input.GetKeyDown(calibrationKey))
        {
            StartCoroutine(CalibrateRoutine());
        }

        // Every frame, calculate the "Clean" position relative to the recorded calibration
        CalculateMagicSpace();
    }

    private IEnumerator CalibrateRoutine()
    {
        Debug.Log("STEP 1: Stand at center. Capturing Origin...");
        // Record where the Kinect thinks (0,0,0) is
        kinectOriginOffset = kinectRawHead.position;
        kinectRotationOffset = Quaternion.identity; // Reset rotation for step 1

        yield return new WaitForSeconds(calibrationDelay);

        Debug.Log("STEP 2: Capturing Rotation...");
        // Vector from the new position back to the origin
        Vector3 directionToOrigin = (kinectOriginOffset - kinectRawHead.position).normalized;
        directionToOrigin.y = 0; // Keep the window vertical

        if (directionToOrigin != Vector3.zero)
        {
            // This determines which way 'Forward' is from the Kinect's perspective
            kinectRotationOffset = Quaternion.LookRotation(directionToOrigin, Vector3.up);
        }

        Debug.Log("Calibration Complete.");
    }

    private void CalculateMagicSpace()
    {
        if (kinectRawHead == null) return;

        // 1. Get position relative to the calibrated origin
        Vector3 localPos = kinectRawHead.position - kinectOriginOffset;

        // 2. Rotate that position by the inverse of our calibrated rotation
        // This "un-spins" the Kinect's slanted data so it aligns with Unity's axes
        CalibratedPosition = Quaternion.Inverse(kinectRotationOffset) * localPos;

        // 3. Optional: also calibrate the head rotation
        CalibratedRotation = Quaternion.Inverse(kinectRotationOffset) * kinectRawHead.rotation;
    }
}