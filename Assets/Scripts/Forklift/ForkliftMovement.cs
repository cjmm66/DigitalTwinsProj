using UnityEngine;

public class ForkliftMovement : MonoBehaviour
{
    

    [SerializeField] private Transform containerPickupPoint;
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float rotationSpeed = 8f;

    // Small safety distance so the forklifts don't visually overlap.
    [SerializeField] private float collisionBuffer = 0.02f;

    private Vector2 targetPosition;
    private bool isMoving;
    private bool movementDisabled;
    private bool stoppedByAvoidance;
    private float movementStartTime;

    private bool isEmpty = true;
    private ContainerStorage carriedContainer;

    private Rigidbody2D rb;
    private Collider2D forkliftCollider;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        forkliftCollider = GetComponent<Collider2D>();

        targetPosition = rb.position;
    }

    private void FixedUpdate()
    {
        if (!isMoving)
            return;

        // Forklift is temporarily waiting for another forklift.
        if (movementDisabled)
        {
            if (stoppedByAvoidance)
            {
                Vector2 direction =
                    (targetPosition - rb.position).normalized;

                float distanceToTarget =
                    Vector2.Distance(
                        rb.position,
                        targetPosition
                    );

                float movementDistance =
                    moveSpeed * Time.fixedDeltaTime;

                movementDistance =
                    Mathf.Min(
                        movementDistance,
                        distanceToTarget
                    );

                // Check if the forklift that was blocking us
                // has moved away.
                if (GetForkliftInPath(
                        direction,
                        movementDistance
                    ) == null)
                {
                    movementDisabled = false;
                    stoppedByAvoidance = false;

                    Debug.Log(
                        gameObject.name +
                        " path is clear. Resuming movement."
                    );
                }
            }

            // Still blocked, so don't move.
            if (movementDisabled)
                return;
        }

        Vector2 moveDirection =
            (targetPosition - rb.position).normalized;

        float distance =
            Vector2.Distance(
                rb.position,
                targetPosition
            );

        float movementDistanceThisFrame =
            moveSpeed * Time.fixedDeltaTime;

        movementDistanceThisFrame =
            Mathf.Min(
                movementDistanceThisFrame,
                distance
            );

        ForkliftMovement blockingForklift =
            GetForkliftInPath(
                moveDirection,
                movementDistanceThisFrame
            );

        if (blockingForklift != null)
        {
            if (HasPriorityOver(blockingForklift))
            {
                // We have priority.
                // The other forklift waits.
                blockingForklift.StopForAvoidance();

                Debug.Log(
                    gameObject.name +
                    " has priority over " +
                    blockingForklift.gameObject.name
                );
            }
            else
            {
                // The other forklift has priority.
                // We wait.
                StopForAvoidance();

                Debug.Log(
                    gameObject.name +
                    " is waiting for " +
                    blockingForklift.gameObject.name
                );

                return;
            }
        }

        // Rotation
        float angle =
            Mathf.Atan2(
                moveDirection.y,
                moveDirection.x
            ) * Mathf.Rad2Deg;

        float smoothAngle =
            Mathf.LerpAngle(
                rb.rotation,
                angle,
                rotationSpeed *
                Time.fixedDeltaTime
            );

        rb.MoveRotation(smoothAngle);

        // Movement
        Vector2 newPosition =
            rb.position +
            moveDirection *
            movementDistanceThisFrame;

        rb.MovePosition(newPosition);

        // Destination reached
        if (distance <= 0.05f)
        {
            rb.MovePosition(targetPosition);

            isMoving = false;
        }
    }

    private ForkliftMovement GetForkliftInPath(
        Vector2 direction,
        float distance
    )
    {
        if (forkliftCollider == null)
            return null;

        RaycastHit2D[] hits =
            new RaycastHit2D[10];

        ContactFilter2D filter =
            new ContactFilter2D();

        filter.useTriggers = false;

        int hitCount =
            forkliftCollider.Cast(
                direction,
                filter,
                hits,
                distance + collisionBuffer
            );

        for (int i = 0; i < hitCount; i++)
        {
            if (hits[i].collider == null)
                continue;

            ForkliftMovement otherForklift =
                hits[i].collider.GetComponent<ForkliftMovement>();

            if (otherForklift != null &&
                otherForklift != this)
            {
                return otherForklift;
            }
        }

        return null;
    }

    private bool HasPriorityOver(
        ForkliftMovement other
    )
    {
        // The forklift that started moving first
        // has priority.
        return movementStartTime < other.movementStartTime;
    }

    public void MoveTo(Vector2 destination)
    {
        targetPosition = destination;

        isMoving = true;

        // Important:
        // Selecting this forklift and giving it a new
        // destination removes its previous avoidance stop.
        movementDisabled = false;

        stoppedByAvoidance = false;

        movementStartTime = Time.time;

        Debug.Log(
            gameObject.name +
            " is moving to " +
            destination
        );
    }

    public void StopForAvoidance()
    {
        movementDisabled = true;

        stoppedByAvoidance = true;

        Debug.Log(
            gameObject.name +
            " stopped for collision avoidance."
        );
    }

    public void Stop()
    {
        movementDisabled = true;

        stoppedByAvoidance = false;

        Debug.Log(
            gameObject.name +
            " stopped."
        );
    }

    public void Resume()
    {
        movementDisabled = false;

        stoppedByAvoidance = false;

        Debug.Log(
            gameObject.name +
            " resumed."
        );
    }

    public bool IsMoving()
    {
        return isMoving;
    }

    public float GetMovementStartTime()
    {
        return movementStartTime;
    }

    public bool IsStoppedByAvoidance()
    {
        return stoppedByAvoidance;
    }

    public bool IsEmpty()
    {
        return isEmpty;
    }

    public bool TryPickupContainer(ContainerStorage container)
    {
        // Forklift is already carrying something.
        if (!isEmpty)
        {
            Debug.Log(
                gameObject.name +
                " cannot pick up " +
                container.gameObject.name +
                " because it is already carrying a container."
            );

            return false;
        }

        // Container isn't ready.
        if (!container.IsReadyForShipment())
        {
            Debug.Log(
                container.gameObject.name +
                " is not ready for shipment."
            );

            return false;
        }

        // Start the shipment state.
        if (!container.TryStartShipment())
        {
            return false;
        }

        // Store reference to the container.
        carriedContainer = container;

        isEmpty = false;

        // Attach container to forklift.
        container.transform.SetParent(transform);

        // Move container to pickup position.
        if (containerPickupPoint != null)
        {
            container.transform.position =
                containerPickupPoint.position;
        }

        Debug.Log(
            gameObject.name +
            " picked up " +
            container.gameObject.name
        );

        return true;
    }

    public void CompleteContainerShipment()
    {
        if (isEmpty || carriedContainer == null)
        {
            Debug.Log(
                gameObject.name +
                " has no container to ship."
            );

            return;
        }

        Debug.Log(
            gameObject.name +
            " delivered " +
            carriedContainer.gameObject.name +
            " to the shipment zone."
        );

        // Tell the container it has been shipped.
        carriedContainer.CompleteShipment();

        // Remove the container from the forklift.
        Destroy(carriedContainer.gameObject);

        // Forklift is empty again.
        carriedContainer = null;
        isEmpty = true;

        Debug.Log(
            gameObject.name +
            " is now empty and ready for another container."
        );

    }
}