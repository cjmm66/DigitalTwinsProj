using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

public class ForkliftMovement : MonoBehaviour
{

    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float rotationSpeed = 8f;

    private bool movementDisabled = false;

    private Vector2 targetPosition;
    private bool isMoving;

    private Rigidbody2D rb;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        targetPosition = rb.position;
    }


    private void FixedUpdate()
    {
        //if (!isMoving)
        //    return;


        //Vector2 direction = (targetPosition - rb.position).normalized;


        //// Rotation
        //float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;


        //float smoothAngle = Mathf.LerpAngle(
        //    rb.rotation,
        //    angle,
        //    rotationSpeed * Time.fixedDeltaTime
        //);


        //rb.MoveRotation(smoothAngle);


        //// Movement
        //Vector2 newPosition = Vector2.MoveTowards(
        //    rb.position,
        //    targetPosition,
        //    moveSpeed * Time.fixedDeltaTime
        //);


        //rb.MovePosition(newPosition);


        //if (Vector2.Distance(rb.position, targetPosition) < 0.05f)
        //{
        //    isMoving = false;
        //}

        if (!isMoving || movementDisabled)
            return;

        Vector2 direction = (targetPosition - rb.position).normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        float smoothAngle = Mathf.LerpAngle(
            rb.rotation,
            angle,
            rotationSpeed * Time.fixedDeltaTime
        );

        rb.MoveRotation(smoothAngle);

        Vector2 newPosition = Vector2.MoveTowards(
            rb.position,
            targetPosition,
            moveSpeed * Time.fixedDeltaTime
        );

        rb.MovePosition(newPosition);

        if (Vector2.Distance(rb.position, targetPosition) < 0.05f)
        {
            isMoving = false;
        }

    }

    public void Stop()
    {
        movementDisabled = true;
    }

    public void Resume()
    {
        movementDisabled = false;
    }

    public void MoveTo(Vector2 destination)
    {
        targetPosition = destination;
        isMoving = true;

        Debug.Log("Moving to: " + destination);
    }
}
