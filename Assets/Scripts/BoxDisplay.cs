using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class BoxDisplay : MonoBehaviour
{
    [Header("Controlled Object")]
    [Tooltip("Set the 3D object to show/hide when the marker is detected.")]
    public GameObject controlledObject;

    [Header("Display Settings")]
    [Tooltip("Time (in seconds) the object stays visible after being shown.")]
    public float timeToDisappear = 5.0f;

    [Header("Cooldown Settings")]
    [Tooltip("Cooldown time (in seconds) before the object can be shown again after disappearing.")]
    public float cooldownDuration = 3.0f; // Default is 3 seconds

    // --- Private variables ---
    private ObserverBehaviour observerBehaviour;
    private bool isTracking = false;

    public float timer = 0f; // Timer to count visible duration

    // Cooldown management
    private bool isCooldown = false; // Whether currently in cooldown
    private float cooldownTimer = 0f; // Timer to count cooldown duration

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

        if (controlledObject != null)
        {
            controlledObject.SetActive(false);
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
                OnTrackingFound();
            }
            isTracking = true;
        }
        else
        {
            if (isTracking)
            {
                OnTrackingLost();
            }
            isTracking = false;
        }
    }

    /// <summary>
    /// Called when marker tracking is first detected
    /// </summary>
    private void OnTrackingFound()
    {
        // If in cooldown, do not display the object
        if (isCooldown)
        {
            Debug.Log($"In cooldown. Cannot show object. Remaining: {(cooldownDuration - cooldownTimer).ToString("F1")} sec");
            return;
        }

        Debug.Log("Marker detected. Showing object.");

        if (controlledObject != null)
        {
            controlledObject.SetActive(true);
            timer = 0f; // Reset display timer
        }
    }

    private void OnTrackingLost()
    {
        Debug.Log("Marker lost.");
        if (controlledObject != null)
        {
            controlledObject.SetActive(false);
        }
    }

    void Update()
    {
        // If the object is currently visible, count the display time
        if (controlledObject != null && controlledObject.activeSelf)
        {
            timer += Time.deltaTime;

            float remainingTime = timeToDisappear - timer;
            Debug.Log($"Object will hide in: {remainingTime.ToString("F1")} sec");

            // If display duration is over, hide the object
            if (timer >= timeToDisappear)
            {
                Debug.LogWarning($"Display duration ({timeToDisappear} sec) exceeded. Hiding object.");
                controlledObject.SetActive(false);

                // Start cooldown
                isCooldown = true;
                cooldownTimer = 0f;
                Debug.Log($"Cooldown started ({cooldownDuration} sec).");
            }
        }

        // Handle cooldown countdown
        if (isCooldown)
        {
            cooldownTimer += Time.deltaTime;
            if (cooldownTimer >= cooldownDuration)
            {
                isCooldown = false;
                Debug.Log("Cooldown ended. Object can be shown again.");
            }
        }
    }
}
