using UnityEngine;
using Vuforia;

public class MarkerSpeedTrack : MonoBehaviour
{
    [Header("Marker Speed (m/s)")]
    [Tooltip("Displays the calculated marker speed in meters per second.")]
    public float speed = 0f;

    [Header("Settings")]
    [Tooltip("Sampling interval for speed calculation (in seconds). Smaller values result in more frequent updates.")]
    public float sampleInterval = 0.1f;

    // --- Private variables ---
    private Vector3 previousPosition;
    private ObserverBehaviour observerBehaviour;
    private bool isTracking = false;
    private float sampleTimer = 0f; // Timer for sampling interval

    void Start()
    {
        observerBehaviour = GetComponent<ObserverBehaviour>();
        if (observerBehaviour)
        {
            observerBehaviour.OnTargetStatusChanged += OnTargetStatusChanged;
        }
        else
        {
            Debug.LogError("ObserverBehaviour not found. Make sure this script is attached to an ImageTarget.");
        }
    }

    void OnDestroy()
    {
        if (observerBehaviour)
        {
            observerBehaviour.OnTargetStatusChanged -= OnTargetStatusChanged;
        }
    }

    private void OnTargetStatusChanged(ObserverBehaviour behaviour, TargetStatus newStatus)
    {
        if (newStatus.Status == Status.TRACKED)
        {
            if (!isTracking)
            {
                // When tracking starts, initialize position and timer
                previousPosition = transform.position;
                sampleTimer = 0f;
            }
            isTracking = true;
        }
        else
        {
            isTracking = false;
            speed = 0f;
        }
    }

    void Update()
    {
        // Do nothing if not currently being tracked
        if (!isTracking) return;

        // Increase the sample timer by deltaTime
        sampleTimer += Time.deltaTime;

        // If the sampling interval has passed, calculate speed
        if (sampleTimer >= sampleInterval)
        {
            // Calculate distance moved
            float distance = Vector3.Distance(transform.position, previousPosition);

            // Calculate speed (speed = distance / time)
            speed = distance / sampleTimer;

            // Reset position and timer for next calculation
            previousPosition = transform.position;
            sampleTimer = 0f;
        }
    }
}
