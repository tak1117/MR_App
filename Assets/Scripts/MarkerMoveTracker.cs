using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class MarkerMoveTracker : MonoBehaviour
{
    [Header("Total Distance Moved (m)")]
    [Tooltip("Displays the total distance the marker has moved since tracking began.")]
    public float totalDistanceMoved = 0f;

    // Variable to store the previous frame's position
    private Vector3 previousPosition;

    // Reference to Vuforia's tracking component
    private ObserverBehaviour observerBehaviour;
    private bool isTracking = false;

    [SerializeField] private MarkerSpeedTrack speedTracker;
    [SerializeField] private TowerDisplay Tower;
    public float minSpeed = 60;

    void Start()
    {
        observerBehaviour = GetComponent<ObserverBehaviour>();
        if (observerBehaviour)
        {
            observerBehaviour.OnTargetStatusChanged += OnTargetStatusChanged;
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
            // When tracking starts, reset values
            if (!isTracking)
            {
                totalDistanceMoved = 0f;
                previousPosition = transform.position;
            }
            isTracking = true;
        }
        else
        {
            isTracking = false;
        }
    }

    void Update()
    {
        // Do nothing if not currently being tracked
        if (!isTracking) return;

        // Calculate distance moved during this frame
        float frameDistance = Vector3.Distance(transform.position, previousPosition);

        // Accumulate distance only if the speed is above the threshold
        if (speedTracker.speed > minSpeed)
        {
            totalDistanceMoved += frameDistance;
            if (Tower.currentHp >= 0)
            {
                Tower.currentHp -= frameDistance / 20;
            }
        }


        // Update previous position for the next frame
        previousPosition = transform.position;
    }
}