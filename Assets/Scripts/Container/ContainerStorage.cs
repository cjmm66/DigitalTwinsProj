using UnityEngine;

public class ContainerStorage : MonoBehaviour
{
    public enum ContainerState
    {
        Waiting,
        ReadyForShipment,
        BeingTransported,
        Completed
    }

    public enum PenaltyLevel
    {
        None,
        Minor,
        Moderate,
        Severe
    }

    [Header("Storage Settings")]
    [SerializeField] private float requiredStorageTime = 30f;

    private float remainingStorageTime;

    // Time that has passed AFTER the container became ready.
    private float timeSinceReady;

    private ContainerState currentState;

    private PenaltyLevel currentPenalty;

    private bool storageTimerStarted;

    private void Awake()
    {
        currentState = ContainerState.Waiting;

        remainingStorageTime = requiredStorageTime;

        timeSinceReady = 0f;

        currentPenalty = PenaltyLevel.None;
    }

    private void Update()
    {
        if (!storageTimerStarted)
            return;

        // -----------------------------
        // STORAGE WAITING TIME
        // -----------------------------

        if (currentState == ContainerState.Waiting)
        {
            remainingStorageTime -= Time.deltaTime;

            if (remainingStorageTime <= 0f)
            {
                remainingStorageTime = 0f;

                currentState =
                    ContainerState.ReadyForShipment;

                timeSinceReady = 0f;

                currentPenalty =
                    PenaltyLevel.None;

                Debug.Log(
                    gameObject.name +
                    " is now READY FOR SHIPMENT."
                );
            }

            return;
        }

        // -----------------------------
        // TIME AFTER READY
        // -----------------------------

        if (currentState ==
            ContainerState.ReadyForShipment)
        {
            timeSinceReady += Time.deltaTime;

            UpdatePenaltyLevel();
        }
    }

    private void UpdatePenaltyLevel()
    {
        PenaltyLevel newPenalty = PenaltyLevel.None;

        // Severe penalty
        if (timeSinceReady >= requiredStorageTime * 2f)
        {
            newPenalty = PenaltyLevel.Severe;
        }
        // Moderate penalty
        else if (timeSinceReady >= requiredStorageTime)
        {
            newPenalty = PenaltyLevel.Moderate;
        }
        // Minor penalty
        else if (timeSinceReady >= requiredStorageTime * 0.5f)
        {
            newPenalty = PenaltyLevel.Minor;
        }

        // Do nothing if the penalty hasn't changed
        if (newPenalty == currentPenalty)
            return;

        PenaltyLevel previousPenalty = currentPenalty;

        currentPenalty = newPenalty;

        // Log only when a new penalty is applied
        if (newPenalty != PenaltyLevel.None)
        {
            Debug.Log(
                gameObject.name +
                " received a " +
                newPenalty +
                " penalty."
            );
        }

        // Log when a previous penalty is replaced
        if (previousPenalty != PenaltyLevel.None &&
            previousPenalty != newPenalty)
        {
            Debug.Log(
                gameObject.name +
                "'s " +
                previousPenalty +
                " penalty was replaced by " +
                newPenalty +
                "."
            );
        }
    }

    public void StartStorageTimer()
    {
        storageTimerStarted = true;

        currentState =
            ContainerState.Waiting;

        remainingStorageTime =
            requiredStorageTime;

        timeSinceReady = 0f;

        currentPenalty =
            PenaltyLevel.None;

        Debug.Log(
            gameObject.name +
            " entered the warehouse. " +
            "Storage timer started."
        );
    }

    public bool IsReadyForShipment()
    {
        return currentState ==
               ContainerState.ReadyForShipment;
    }

    public ContainerState GetState()
    {
        return currentState;
    }

    public float GetRemainingStorageTime()
    {
        return remainingStorageTime;
    }

    public float GetTimeSinceReady()
    {
        return timeSinceReady;
    }

    public PenaltyLevel GetPenaltyLevel()
    {
        return currentPenalty;
    }

    public bool TryStartShipment()
    {
        if (currentState !=
            ContainerState.ReadyForShipment)
        {
            Debug.Log(
                gameObject.name +
                " is not ready for shipment."
            );

            return false;
        }

        currentState =
            ContainerState.BeingTransported;

        Debug.Log(
            gameObject.name +
            " is now being transported."
        );

        return true;
    }

    public void CompleteShipment()
    {
        currentState =
            ContainerState.Completed;

        Debug.Log(
            gameObject.name +
            " has been shipped."
        );
    }
}